using System;
using System.Collections.Generic;
using EntityCache;

namespace CrudEntityCache
{
    public class PersonConverter : IConverter<Person>
    {
        public Dictionary<string, string> FromEntity(Person entity)
        {
            if (entity == null)
                throw new NullReferenceException("Entity cannot be null");

            var attributes = new Dictionary<string, string>
            {
                {"id", entity.getId().ToString()},
                {"Name", entity.Name},
            };

            return attributes;
        }

        public Person ToEntity(Dictionary<string, string> data)
        {
            var id = int.Parse(data["id"]);
            var name = data["Name"];

            return new Person(id, name);
        }
    }
}