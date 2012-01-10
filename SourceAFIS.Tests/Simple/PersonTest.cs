using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using NUnit.Framework;
using SourceAFIS.Simple;

namespace SourceAFIS.Tests.Simple
{
    [TestFixture]
    public class PersonTest
    {
        [Test]
        public void DefaultConstructor()
        {
            Person person = new Person();
            Assert.AreEqual(0, person.Id);
            Assert.IsNotNull(person.Fingerprints);
            Assert.IsEmpty(person.Fingerprints);
        }

        [Test]
        public void Id()
        {
            Person person = new Person() { Id = 13 };
            Assert.AreEqual(13, person.Id);
            person.Id = 25;
            Assert.AreEqual(25, person.Id);
        }

        [Test]
        public void Fingerprints()
        {
            Fingerprint fp1 = new Fingerprint();
            Fingerprint fp2 = new Fingerprint();
            Fingerprint fp3 = new Fingerprint();

            Person person = new Person(fp1, fp2, fp3);
            Assert.AreEqual(new[] { fp1, fp2, fp3 }, person.Fingerprints);

            person.Fingerprints = new List<Fingerprint> { fp1, fp3 };
            Assert.AreEqual(new[] { fp1, fp3 }, person.Fingerprints);

            Assert.Catch(() => { person.Fingerprints = null; });
            Assert.AreEqual(new[] { fp1, fp3 }, person.Fingerprints);

            person.Fingerprints[1] = person.Fingerprints[0];
            person.Fingerprints[0] = fp2;
            Assert.AreEqual(new[] { fp2, fp1 }, person.Fingerprints);

            person.Fingerprints = new List<Fingerprint>();
            Assert.AreEqual(new Fingerprint[] { }, person.Fingerprints);
        }

        [Test]
        public void Clone()
        {
            Fingerprint fp1 = new Fingerprint() { AsBitmapSource = Settings.SomeFingerprint };
            Fingerprint fp2 = new Fingerprint() { AsBitmapSource = Settings.NonMatchingFingerprint };
            Person person1 = new Person(fp1, fp2);
            person1.Id = 15;

            Person person2 = person1.Clone();
            Assert.AreEqual(15, person2.Id);
            Assert.AreNotSame(person1.Fingerprints, person2.Fingerprints);
            Assert.AreEqual(2, person2.Fingerprints.Count);
            Assert.AreNotSame(fp1, person2.Fingerprints[0]);
            Assert.AreNotSame(fp2, person2.Fingerprints[1]);
            Assert.AreEqual(fp1.Image, person2.Fingerprints[0].Image);
            Assert.AreEqual(fp2.Image, person2.Fingerprints[1].Image);

            Person person3 = (person1 as ICloneable).Clone() as Person;
            Assert.AreEqual(15, person3.Id);
            Assert.AreNotSame(person1.Fingerprints, person3.Fingerprints);
            Assert.AreEqual(2, person3.Fingerprints.Count);
            Assert.AreNotSame(fp1, person3.Fingerprints[0]);
            Assert.AreNotSame(fp2, person3.Fingerprints[1]);
            Assert.AreEqual(fp1.Image, person3.Fingerprints[0].Image);
            Assert.AreEqual(fp2.Image, person3.Fingerprints[1].Image);
        }

        [Test]
        public void Serialize()
        {
            Fingerprint fp1 = new Fingerprint() { AsBitmapSource = Settings.SomeFingerprint };
            Fingerprint fp2 = new Fingerprint() { AsBitmapSource = Settings.NonMatchingFingerprint };
            Person person1 = new Person(fp1, fp2);
            person1.Id = 15;

            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, person1);
            stream.Position = 0;
            Person person2 = formatter.Deserialize(stream) as Person;

            Assert.AreEqual(15, person2.Id);
            Assert.AreEqual(2, person2.Fingerprints.Count);
            Assert.AreEqual(fp1.Image, person2.Fingerprints[0].Image);
            Assert.AreEqual(fp2.Image, person2.Fingerprints[1].Image);
        }

        [Test]
        public void XmlSerialize()
        {
            Fingerprint fp1 = new Fingerprint() { AsBitmapSource = Settings.SomeFingerprint };
            Fingerprint fp2 = new Fingerprint() { AsBitmapSource = Settings.NonMatchingFingerprint };
            Person person1 = new Person(fp1, fp2);
            person1.Id = 15;

            MemoryStream stream = new MemoryStream();
            XmlSerializer serializer = new XmlSerializer(typeof(Person));
            serializer.Serialize(stream, person1);
            stream.Position = 0;
            Person person2 = serializer.Deserialize(stream) as Person;

            Assert.AreEqual(15, person2.Id);
            Assert.AreEqual(2, person2.Fingerprints.Count);
            Assert.AreEqual(fp1.Image, person2.Fingerprints[0].Image);
            Assert.AreEqual(fp2.Image, person2.Fingerprints[1].Image);
        }
    }
}
