using EntityCache;

namespace CrudEntityCache
{
    public class Person : Entity
    {
        private readonly int _id;
        public string Name { get; private set; }

        public Person(int id, string name)
        {
            _id = id;
            Name = name;
        }

        public int getId()
        {
            return _id;
        }

        public override bool Equals(object obj)
        {
            var otherEntity = obj as Person;

            return _id == otherEntity?.getId() && Name == otherEntity.Name;
        }
    }
}