using System;
using EntityCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrudEntityCache;

namespace UnitTestProject
{
    [TestClass]
    public class EntityCacheTests
    {
        private static readonly  PersonConverter PersonConverter = new PersonConverter();
        private static readonly XmlRepository XmlRepository = new XmlRepository();
        private readonly EntityCache<Person> _entityCache = new EntityCache<Person>(XmlRepository, PersonConverter, RepositoryMode.Lazy);

        [TestMethod]
        public void TestAdd()
        {
            var person = new Person(14, "Mark");
            _entityCache.Add(person);

            Assert.AreEqual(person, _entityCache.Get(14));
            _entityCache.Remove(14);
        }

        [TestMethod]
        public void TestUpdate()
        {
            _entityCache.Add(new Person(14, "Mark"));

            var person = new Person(14, "GOOGI");
            _entityCache.Update(person);

            Assert.AreEqual(person, _entityCache.Get(14));
            _entityCache.Remove(14);
        }

        [TestMethod]
        public void TestGet()
        {
            var person = new Person(14, "Mark");
            _entityCache.Add(person);

            Assert.IsNotNull(_entityCache.Get(13));
            _entityCache.Remove(14);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRemove()
        {
            var person = new Person(14, "Mark");
            _entityCache.Add(person);
            _entityCache.Remove(14);

            _entityCache.Get(14);
        }
    }
}
