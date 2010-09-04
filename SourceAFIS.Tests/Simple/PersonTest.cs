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
            Assert.IsEmpty(person.AllFingerprints);
            Assert.AreEqual(0, person.Count);
            Assert.AreEqual(false, (person as ICollection<Fingerprint>).IsReadOnly);
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
        public void AllFingerprints()
        {
            Fingerprint fp1 = new Fingerprint();
            Fingerprint fp2 = new Fingerprint();
            Fingerprint fp3 = new Fingerprint();

            Person person = new Person() { fp1, fp2, fp3 };
            Assert.AreEqual(new[] { fp1, fp2, fp3 }, person.AllFingerprints);

            person.AllFingerprints = new[] { fp1, fp3 };
            Assert.AreEqual(new[] { fp1, fp3 }, person.AllFingerprints);
            Assert.AreEqual(2, person.Count);

            Assert.Catch(() => { person.AllFingerprints = null; });
            Assert.Catch(() => { person.AllFingerprints = new[] { fp1, null, fp2 }; });

            Assert.AreEqual(new[] { fp1, fp3 }, person.AllFingerprints);
            person.AllFingerprints = new Fingerprint[] { };
            Assert.AreEqual(new Fingerprint[] { }, person.AllFingerprints);
        }

        [Test]
        public void Count()
        {
            Fingerprint fp1 = new Fingerprint();
            Fingerprint fp2 = new Fingerprint();
            Fingerprint fp3 = new Fingerprint();

            Person person = new Person() { fp1, fp2, fp3 };
            Assert.AreEqual(3, person.Count);

            person.AllFingerprints = new Fingerprint[] { };
            Assert.AreEqual(0, person.Count);

            person.AllFingerprints = new[] { fp2 };
            Assert.AreEqual(1, person.Count);
        }

        [Test]
        public void Add()
        {
            Fingerprint fp1 = new Fingerprint();
            Fingerprint fp2 = new Fingerprint();
            Fingerprint fp3 = new Fingerprint();

            Person person = new Person();
            person.Add(fp1);
            Assert.AreEqual(new[] { fp1 }, person.AllFingerprints);
            person.Add(fp2);
            Assert.AreEqual(new[] { fp1, fp2 }, person.AllFingerprints);
            person.Add(fp3);
            Assert.AreEqual(new[] { fp1, fp2, fp3 }, person.AllFingerprints);

            person.Add(fp1);
            Assert.AreEqual(new[] { fp1, fp2, fp3, fp1 }, person.AllFingerprints);

            Assert.Catch(() => { person.Add(null); });
        }

        [Test]
        public void Clear()
        {
            Person person = new Person() { new Fingerprint(), new Fingerprint() };
            person.Clear();
            Assert.AreEqual(0, person.Count);
        }

        [Test]
        public void Remove()
        {
            Fingerprint fp1 = new Fingerprint();
            Fingerprint fp2 = new Fingerprint();
            Fingerprint fp3 = new Fingerprint();
            Person person = new Person() { fp1, fp2, fp3 };

            person.Remove(fp2);
            Assert.AreEqual(new[] { fp1, fp3 }, person.AllFingerprints);
            
            person.Remove(new Fingerprint());
            Assert.AreEqual(new[] { fp1, fp3 }, person.AllFingerprints);

            person.Add(fp1);
            person.Remove(fp1);
            Assert.AreEqual(new[] { fp3, fp1 }, person.AllFingerprints);

            person.Remove(fp2);
            Assert.AreEqual(new[] { fp3, fp1 }, person.AllFingerprints);

            person.Remove(null);
            Assert.AreEqual(new[] { fp3, fp1 }, person.AllFingerprints);
        }

        [Test]
        public void Indexer()
        {
            Fingerprint fp1 = new Fingerprint();
            Fingerprint fp2 = new Fingerprint();
            Fingerprint fp3 = new Fingerprint();
            Person person = new Person() { fp1, fp2 };

            Assert.AreSame(fp1, person[0]);
            Assert.AreSame(fp2, person[1]);

            person[1] = fp3;
            Assert.AreEqual(new[] { fp1, fp3 }, person.AllFingerprints);
            Assert.AreSame(fp1, person[0]);
            Assert.AreSame(fp3, person[1]);

            person[0] = fp3;
            Assert.AreEqual(new[] { fp3, fp3 }, person.AllFingerprints);
            Assert.AreSame(fp3, person[0]);
            Assert.AreSame(fp3, person[1]);

            Fingerprint fp;
            Assert.Catch(() => { fp = person[-1]; });
            Assert.Catch(() => { fp = person[2]; });

            Assert.Catch(() => { person[0] = null; });
            Assert.Catch(() => { person[-1] = fp2; });
            Assert.Catch(() => { person[2] = fp2; });
        }

        [Test]
        public void Insert()
        {
            Fingerprint fp1 = new Fingerprint();
            Fingerprint fp2 = new Fingerprint();
            Fingerprint fp3 = new Fingerprint();
            Person person = new Person() { fp1, fp3 };

            person.Insert(1, fp2);
            Assert.AreEqual(new[] { fp1, fp2, fp3 }, person.AllFingerprints);

            person.Insert(3, fp1);
            Assert.AreEqual(new[] { fp1, fp2, fp3, fp1 }, person.AllFingerprints);

            person.Insert(0, fp3);
            Assert.AreEqual(new[] { fp3, fp1, fp2, fp3, fp1 }, person.AllFingerprints);

            Assert.Catch(() => { person.Insert(0, null); });
            Assert.Catch(() => { person.Insert(-1, fp2); });
            Assert.Catch(() => { person.Insert(6, fp2); });
        }

        [Test]
        public void RemoveAt()
        {
            Fingerprint fp1 = new Fingerprint();
            Fingerprint fp2 = new Fingerprint();
            Fingerprint fp3 = new Fingerprint();
            Fingerprint fp4 = new Fingerprint();
            Fingerprint fp5 = new Fingerprint();
            Person person = new Person() { fp1, fp2, fp3, fp4, fp5 };

            person.RemoveAt(2);
            Assert.AreEqual(new[] { fp1, fp2, fp4, fp5 }, person.AllFingerprints);

            person.RemoveAt(0);
            Assert.AreEqual(new[] { fp2, fp4, fp5 }, person.AllFingerprints);

            person.RemoveAt(2);
            Assert.AreEqual(new[] { fp2, fp4 }, person.AllFingerprints);

            Assert.Catch(() => { person.RemoveAt(-1); });
            Assert.Catch(() => { person.RemoveAt(2); });
        }

        [Test]
        public void Clone()
        {
            Fingerprint fp1 = new Fingerprint() { AsBitmap = Settings.SomeFingerprint };
            Fingerprint fp2 = new Fingerprint() { AsBitmap = Settings.NonMatchingFingerprint };
            Person person1 = new Person() { fp1, fp2 };
            person1.Id = 15;

            Person person2 = person1.Clone();
            Assert.AreEqual(15, person2.Id);
            Assert.AreEqual(2, person2.Count);
            Assert.AreNotSame(fp1, person2[0]);
            Assert.AreNotSame(fp2, person2[1]);
            Assert.AreEqual(fp1.Image, person2[0].Image);
            Assert.AreEqual(fp2.Image, person2[1].Image);

            Person person3 = (person1 as ICloneable).Clone() as Person;
            Assert.AreEqual(15, person3.Id);
            Assert.AreEqual(2, person3.Count);
            Assert.AreNotSame(fp1, person3[0]);
            Assert.AreNotSame(fp2, person3[1]);
            Assert.AreEqual(fp1.Image, person3[0].Image);
            Assert.AreEqual(fp2.Image, person3[1].Image);
        }

        [Test]
        public void Enumerator()
        {
            Fingerprint fp1 = new Fingerprint();
            Fingerprint fp2 = new Fingerprint();
            Fingerprint fp3 = new Fingerprint();
            Person person = new Person() { fp1, fp2, fp3 };

            List<Fingerprint> enum1 = new List<Fingerprint>();
            foreach (Fingerprint fp in person)
                enum1.Add(fp);
            Assert.AreEqual(new[] { fp1, fp2, fp3 }, enum1);

            List<Fingerprint> enum2 = new List<Fingerprint>();
            foreach (object fp in (person as IEnumerable))
                enum2.Add(fp as Fingerprint);
            Assert.AreEqual(new[] { fp1, fp2, fp3 }, enum2);
        }

        [Test]
        public void Contains()
        {
            Fingerprint fp1 = new Fingerprint();
            Fingerprint fp2 = new Fingerprint();
            Fingerprint fp3 = new Fingerprint();
            Person person = new Person() { fp1, fp3 };

            Assert.IsTrue(person.Contains(fp1));
            Assert.IsFalse(person.Contains(fp2));
            Assert.IsTrue(person.Contains(fp3));
            Assert.IsFalse(person.Contains(null));
        }

        [Test]
        public void CopyTo()
        {
            Fingerprint fp1 = new Fingerprint();
            Fingerprint fp2 = new Fingerprint();
            Person person = new Person() { fp1, fp2 };

            Fingerprint[] array = new Fingerprint[4];
            (person as ICollection<Fingerprint>).CopyTo(array, 1);
            Assert.AreEqual(new[] { null, fp1, fp2, null }, array);

            Assert.Catch(() => { (person as ICollection<Fingerprint>).CopyTo(array, -1); });
            Assert.Catch(() => { (person as ICollection<Fingerprint>).CopyTo(array, 3); });
            Assert.Catch(() => { (person as ICollection<Fingerprint>).CopyTo(null, 0); });
        }

        [Test]
        public void IndexOf()
        {
            Fingerprint fp1 = new Fingerprint();
            Fingerprint fp2 = new Fingerprint();
            Person person = new Person() { fp1, fp2 };

            Assert.AreEqual(0, (person as IList<Fingerprint>).IndexOf(fp1));
            Assert.AreEqual(1, (person as IList<Fingerprint>).IndexOf(fp2));
            Assert.AreEqual(-1, (person as IList<Fingerprint>).IndexOf(new Fingerprint()));
            Assert.AreEqual(-1, (person as IList<Fingerprint>).IndexOf(null));
        }

        [Test]
        public void Serialize()
        {
            Fingerprint fp1 = new Fingerprint() { AsBitmap = Settings.SomeFingerprint };
            Fingerprint fp2 = new Fingerprint() { AsBitmap = Settings.NonMatchingFingerprint };
            Person person1 = new Person() { fp1, fp2 };
            person1.Id = 15;

            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, person1);
            stream.Position = 0;
            Person person2 = formatter.Deserialize(stream) as Person;

            Assert.AreEqual(15, person2.Id);
            Assert.AreEqual(2, person2.Count);
            Assert.AreEqual(fp1.Image, person2[0].Image);
            Assert.AreEqual(fp2.Image, person2[1].Image);
        }

        [Test]
        public void XmlSerialize()
        {
            Fingerprint fp1 = new Fingerprint() { AsBitmap = Settings.SomeFingerprint };
            Fingerprint fp2 = new Fingerprint() { AsBitmap = Settings.NonMatchingFingerprint };
            Person person1 = new Person() { fp1, fp2 };
            person1.Id = 15;

            MemoryStream stream = new MemoryStream();
            XmlSerializer serializer = new XmlSerializer(typeof(Person));
            serializer.Serialize(stream, person1);
            stream.Position = 0;
            Person person2 = serializer.Deserialize(stream) as Person;

            Assert.AreEqual(15, person2.Id);
            Assert.AreEqual(2, person2.Count);
            Assert.AreEqual(fp1.Image, person2[0].Image);
            Assert.AreEqual(fp2.Image, person2[1].Image);
        }
    }
}
