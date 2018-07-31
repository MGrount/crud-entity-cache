using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using RepositoryProvider;

namespace CrudEntityCache
{
    public class XmlRepository : IRepositoryProvider
    {
        private readonly string _path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\")) + @"\CrudEntityCache\Data.xml";

        public void Add(Dictionary<string, string> data)
        {
            if (data == null)
                throw new NullReferenceException("Data cannot be null");

            var xmlDoc = XDocument.Load(_path);
            var entity = toXElement(data);

            xmlDoc.Element("People")?.Add(entity);

            xmlDoc.Save(_path);
        }

        private XElement toXElement(Dictionary<string, string> data)
        {
            var entity = new XElement("Person");

            foreach (var d in data)
                entity.Add(new XElement(d.Key, d.Value));

            return entity;
        }

        public void Update(string id, Dictionary<string, string> data)
        {
            if (data == null)
                throw new NullReferenceException("Data cannot be null");

            var entity = toXElement(data);

            var xmlDoc = XDocument.Load(_path);

            xmlDoc.Element("People")
                .Elements()
                .FirstOrDefault(e => e.Element("id").Value == entity.Element("id").Value)
                .ReplaceWith(entity);

            xmlDoc.Save(_path);
        }

        public void Remove(string id)
        {
            var xmlDoc = XDocument.Load(_path);

            xmlDoc.Element("People")
                ?.Elements()
                .Where(e => e.Element("id")?.Value == id)
                .Remove();

            xmlDoc.Save(_path);
        }

        public Dictionary<string, string> Get(string id)
        {
            var data = XDocument.Load(_path)
                .Element("People")
                ?.Elements()
                .FirstOrDefault(x => x.Element("id")?.Value == id);

            return data?.Elements().ToDictionary(x => x.Name.LocalName, x => x.Value);
        }

        public IEnumerable<Dictionary<string, string>> GetAll()
        {
            var data = from person in XDocument.Load(_path)
                    .Descendants("Person")
                       select person;

            return data.Select(d => d.Elements().ToDictionary(x => x.Name.LocalName, x => x.Value)).ToList();
        }
    }
}