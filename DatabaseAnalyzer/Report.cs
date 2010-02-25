using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DatabaseAnalyzer
{
    abstract class Report
    {
        public XmlDocument XmlDocument;
        public string Path;
        protected XmlElement RootElement;

        public abstract void Create();

        protected void CreateDocument(string rootName)
        {
            XmlDocument = new XmlDocument();
            RootElement = XmlDocument.CreateElement(rootName);
            XmlDocument.AppendChild(RootElement);
        }

        protected void AddProperty(string name, string value)
        {
            XmlElement element = XmlDocument.CreateElement(name);
            RootElement.AppendChild(element);
            element.AppendChild(XmlDocument.CreateTextNode(value));
        }

        protected void AddProperty(string name, int value)
        {
            AddProperty(name, value.ToString());
        }

        protected void AddProperty(string name, double value)
        {
            AddProperty(name, value.ToString("F0"));
        }

        public void Save()
        {
            XmlDocument.Save(Path);
        }
    }
}
