using System;
using System.Collections.Generic;
using RepositoryProvider;

namespace EntityCache
{
    public class EntityCache<T> where T : Entity
    {
        private readonly Dictionary<int, T> _cachedDictionary = new Dictionary<int, T>();
        private readonly IRepositoryProvider _repositoryProvider;
        private readonly IConverter<T> _converter;
        private readonly RepositoryMode _loadingMode;
        private readonly object repositoryLocker = new object();
        public event EventHandler<EntityArgs> Notify;

        public EntityCache(IRepositoryProvider repositoryProvider, IConverter<T> converter, RepositoryMode loadingMode = RepositoryMode.Eager)
        {
            _repositoryProvider = repositoryProvider;
            _converter = converter;
            _loadingMode = loadingMode;

            switch (_loadingMode)
            {
                case RepositoryMode.Eager:
                    GetAllEntities();
                    break;
                case RepositoryMode.Lazy:
                    break;
            }
        }

        private void GetAllEntities()
        {
            lock (repositoryLocker)
            {
                try
                {
                    var entities = _repositoryProvider.GetAll();

                    foreach (var e in entities)
                    {
                        var newEntity = _converter.ToEntity(e);
                        _cachedDictionary.Add(newEntity.getId(), newEntity);
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Cant get all entities");
                }
            }
        }

        public T Get(int id)
        {
            lock (repositoryLocker)
            {
                if (_cachedDictionary.ContainsKey(id)) return _cachedDictionary[id];

                Dictionary<string, string> item = null;

                if (_loadingMode == RepositoryMode.Lazy)
                    item = _repositoryProvider.Get(id.ToString());

                if (item == null)
                    throw new InvalidOperationException("id error");

                var entity = _converter.ToEntity(item);
                _cachedDictionary.Add(entity.getId(), entity);

                return _cachedDictionary[id];
            }
        }

        public void Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null");

            var entityId = entity.getId().ToString();

            lock (repositoryLocker)
            {

                if (_cachedDictionary.ContainsKey(entity.getId())
                    || (_loadingMode == RepositoryMode.Lazy && _repositoryProvider.Get(entityId) != null))
                    throw new InvalidOperationException("entity with the same id already exists");

                try
                {
                    var data = _converter.FromEntity(entity);

                    _repositoryProvider.Add(data);
                    _cachedDictionary.Add(entity.getId(), entity);

                }
                catch (Exception)
                {
                    throw new Exception("cant add a new entity");
                }
            }

            onEntityAction(entity.getId(), EntityActions.Add);
        }

        public void Update(T entity)
        {

            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null");

            var entityId = entity.getId().ToString();

            lock (repositoryLocker)
            {
                if (!_cachedDictionary.ContainsKey(entity.getId()) && _loadingMode == RepositoryMode.Eager)
                    throw new InvalidOperationException("entity does not exist in the repository");

                if ((_loadingMode == RepositoryMode.Lazy && _repositoryProvider.Get(entityId) == null))
                    throw new InvalidOperationException("entity does not exist in the repository");
                else
                {
                    try
                    {
                        _repositoryProvider.Update(entityId, _converter.FromEntity(entity));
                        _cachedDictionary[entity.getId()] = entity;
                    }
                    catch (Exception)
                    {
                        throw new Exception("Failed update");
                    }
                }
            }

            onEntityAction(entity.getId(), EntityActions.Update);
        }

        public void Remove(int id)
        {
            lock (repositoryLocker)
            {

                if (!_cachedDictionary.ContainsKey(id) && _loadingMode == RepositoryMode.Eager)
                    throw new InvalidOperationException("entity does not exist in the repository");

                if ((_loadingMode == RepositoryMode.Lazy && _repositoryProvider.Get(id.ToString()) == null))
                    throw new InvalidOperationException("entity does not exist in the repository");
                else
                {
                    try
                    {
                        _repositoryProvider.Remove(id.ToString());

                        if (_cachedDictionary.ContainsKey(id))
                            _cachedDictionary.Remove(id);
                    }
                    catch (Exception)
                    {
                        throw new Exception("Failed remove");
                    }
                }
            }

            onEntityAction(id, EntityActions.Remove);
        }
        protected virtual void onEntityAction(int entityId, EntityActions action)
        {
            Notify?.Invoke(this, new EntityArgs(entityId, action));
        }
    }
}
