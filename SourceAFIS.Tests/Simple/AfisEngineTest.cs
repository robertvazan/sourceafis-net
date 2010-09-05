using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SourceAFIS.Simple;

namespace SourceAFIS.Tests.Simple
{
    [TestFixture]
    public class AfisEngineTest
    {
        [Test]
        public void DefaultConstructor()
        {
            AfisEngine afis = new AfisEngine();
            Assert.AreEqual(500, afis.Dpi);
            Assert.AreEqual(0, afis.SkipBestMatches);
            Assert.That(afis.Threshold > 0);
        }

        [Test]
        public void Extract()
        {
            AfisEngine afis = new AfisEngine();
            Fingerprint fp = new Fingerprint() { AsBitmap = Settings.SomeFingerprint };
            Assert.IsNull(fp.Template);
            afis.Extract(fp);
            Assert.IsNotNull(fp.Template);

            Assert.Catch(() => { afis.Extract(new Fingerprint()); });
            Assert.Catch(() => { afis.Extract(null); });
        }

        [Test]
        public void Verify()
        {
            AfisEngine afis = new AfisEngine();
            Person person1 = new Person(new Fingerprint() { AsBitmap = Settings.SomeFingerprint });
            afis.Extract(person1.Fingerprints[0]);
            Person person2 = new Person(new Fingerprint() { AsBitmap = Settings.MatchingFingerprint });
            afis.Extract(person2.Fingerprints[0]);
            Person person3 = new Person(new Fingerprint() { AsBitmap = Settings.NonMatchingFingerprint });
            afis.Extract(person3.Fingerprints[0]);

            Assert.That(afis.Verify(person1, person2) > 0);
            Assert.That(afis.Verify(person1, person3) == 0);
            Assert.That(afis.Verify(person1, person1) > 0);

            Person person4 = new Person(person2.Fingerprints[0], person3.Fingerprints[0]);
            Assert.That(afis.Verify(person1, person4) > 0);

            Assert.That(afis.Verify(person1, new Person()) == 0);

            Assert.Catch(() => { afis.Verify(null, person1); });
            Assert.Catch(() => { afis.Verify(person1, null); });
            Assert.Catch(() => { afis.Verify(person1, new Person(null)); });
        }

        [Test]
        public void Identify()
        {
            AfisEngine afis = new AfisEngine();
            Person person1 = new Person(new Fingerprint() { AsBitmap = Settings.SomeFingerprint });
            afis.Extract(person1.Fingerprints[0]);
            Person person2 = new Person(new Fingerprint() { AsBitmap = Settings.MatchingFingerprint });
            afis.Extract(person2.Fingerprints[0]);
            Person person3 = new Person(new Fingerprint() { AsBitmap = Settings.NonMatchingFingerprint });
            afis.Extract(person3.Fingerprints[0]);

            Assert.That(afis.Identify(person1, new[] { person2, person3 }) == person2);
            Assert.That(afis.Identify(person1, new[] { person3, person2 }) == person2);
            Assert.That(afis.Identify(person1, new[] { person3 }) == null);
            Assert.That(afis.Identify(person1, new[] { person2, person3, person1 }) == person1);

            Person person4 = new Person(person2.Fingerprints[0], person3.Fingerprints[0]);
            Assert.That(afis.Identify(person1, new[] { person4 }) == person4);

            var bigArray = Enumerable.Repeat(person3, 100).Concat(new[] { person2 }).Concat(Enumerable.Repeat(person3, 100));
            Assert.That(afis.Identify(person1, bigArray) == person2);

            Assert.That(afis.Identify(person1, new Person[] { }) == null);
            Assert.That(afis.Identify(person1, new[] { new Person() }) == null);
            Assert.That(afis.Identify(new Person(), new[] { person2, person3 }) == null);

            Assert.Catch(() => { afis.Identify(null, new[] { person2, person3 }); });
            Assert.Catch(() => { afis.Identify(new Person(null), new[] { person2, person3 }); });
            Assert.Catch(() => { afis.Identify(person1, null); });
            Assert.Catch(() => { afis.Identify(person1, new[] { person2, null }); });
            Assert.Catch(() => { afis.Identify(person1, new[] { person2, new Person(null) }); });
        }

        [Test]
        public void Dpi()
        {
            AfisEngine afis = new AfisEngine();
            Person person1 = new Person(new Fingerprint() { AsBitmap = Settings.SomeFingerprint });
            Person person2 = person1.Clone();

            afis.Extract(person1.Fingerprints[0]);
            afis.Dpi = 750;
            afis.Extract(person2.Fingerprints[0]);
            Assert.AreNotEqual(person1.Fingerprints[0].Template, person2.Fingerprints[0].Template);
            Assert.That(afis.Verify(person1, person2) == 0);

            afis.Dpi = 230;
            afis.Dpi = 1000;
            Assert.Catch(() => { afis.Dpi = 90; });
            Assert.Catch(() => { afis.Dpi = 10000; });
        }

        [Test]
        public void Threshold()
        {
            AfisEngine afis = new AfisEngine();
            Person person1 = new Person(new Fingerprint() { AsBitmap = Settings.SomeFingerprint });
            afis.Extract(person1.Fingerprints[0]);
            Person person2 = new Person(new Fingerprint() { AsBitmap = Settings.MatchingFingerprint });
            afis.Extract(person2.Fingerprints[0]);

            float score = afis.Verify(person1, person2);
            Assert.That(score > 0);

            afis.Threshold = 1.001f * score;
            Assert.That(afis.Verify(person1, person2) == 0);
            Assert.That(afis.Identify(person1, new[] { person2 }) == null);

            afis.Threshold = 0.999f * score;
            Assert.That(afis.Verify(person1, person2) > 0);
            Assert.That(afis.Identify(person1, new[] { person2 }) == person2);

            afis.Threshold = score;
            Assert.That(afis.Verify(person1, person2) > 0);
            Assert.That(afis.Identify(person1, new[] { person2 }) == person2);

            afis.Threshold = 0;
            Assert.That(afis.Verify(person1, person2) > 0);
            Assert.That(afis.Identify(person1, new[] { person2 }) == person2);

            Assert.Catch(() => { afis.Threshold = -0.001f; });
        }

        [Test]
        public void SkipBestMatches()
        {
            AfisEngine afis = new AfisEngine();
            Fingerprint[] fps = new[] {
                new Fingerprint() { AsBitmap = Settings.SomeFingerprint },
                new Fingerprint() { AsBitmap = Settings.MatchingFingerprint },
                new Fingerprint() { AsBitmap = Settings.NonMatchingFingerprint }
            };
            foreach (Fingerprint fp in fps)
                afis.Extract(fp);

            Person person1 = new Person(fps[0]);
            Person person2 = new Person(fps[1], fps[2]);
            Person person3 = new Person(fps[2], fps[1]);
            Person person4 = new Person(fps[1]);
            Person person5 = new Person(fps[1], fps[2], fps[1]);
            Person person6 = new Person(fps[2], fps[1], fps[2]);
            Person person7 = new Person(fps[1], fps[1], fps[1]);
            Person person8 = new Person(fps[2], fps[2]);

            foreach (Person person in new[] { person2, person3, person4, person5, person6, person7 })
                Assert.That(afis.Verify(person1, person) > 0);
            Assert.That(afis.Verify(person1, person8) == 0);
            foreach (Person person in new[] { person2, person3, person4, person5, person6, person7 })
                Assert.That(afis.Identify(person1, new[] { person }) != null);
            Assert.That(afis.Identify(person1, new[] { person8 }) == null);

            afis.SkipBestMatches = 1;
            foreach (Person person in new[] { person2, person3, person6, person8 })
                Assert.That(afis.Verify(person1, person) == 0);
            foreach (Person person in new[] { person4, person5, person7 })
                Assert.That(afis.Verify(person1, person) > 0);
            Assert.That(afis.Identify(person1, new[] { person2, person3, person6, person8 }) == null);
            foreach (Person person in new[] { person4, person5, person7 })
                Assert.That(afis.Identify(person1, new[] { person }) != null);

            afis.SkipBestMatches = 2;
            foreach (Person person in new[] { person2, person3, person5, person6, person8 })
                Assert.That(afis.Verify(person1, person) == 0);
            Assert.That(afis.Verify(person1, person4) > 0);
            Assert.That(afis.Verify(person1, person7) > 0);
            Assert.That(afis.Identify(person1, new[] { person2, person3, person5, person6, person8 }) == null);
            Assert.That(afis.Identify(person1, new[] { person4 }) != null);
            Assert.That(afis.Identify(person1, new[] { person7 }) != null);

            afis.SkipBestMatches = 3;
            foreach (Person person in new[] { person2, person3, person5, person6, person8 })
                Assert.That(afis.Verify(person1, person) == 0);
            Assert.That(afis.Verify(person1, person4) > 0);
            Assert.That(afis.Verify(person1, person7) > 0);
            Assert.That(afis.Identify(person1, new[] { person2, person3, person5, person6, person8 }) == null);
            Assert.That(afis.Identify(person1, new[] { person4 }) != null);
            Assert.That(afis.Identify(person1, new[] { person7 }) != null);

            Person person9 = new Person(fps[0], fps[0]);
            Person person10 = new Person(fps[1], fps[1], fps[2]);
            afis.SkipBestMatches = 0;
            Assert.That(afis.Verify(person9, person10) > 0);
            afis.SkipBestMatches = 1;
            Assert.That(afis.Verify(person9, person10) > 0);
            afis.SkipBestMatches = 2;
            Assert.That(afis.Verify(person9, person10) > 0);
            afis.SkipBestMatches = 3;
            Assert.That(afis.Verify(person9, person10) > 0);
            afis.SkipBestMatches = 4;
            Assert.That(afis.Verify(person9, person10) == 0);
            afis.SkipBestMatches = 5;
            Assert.That(afis.Verify(person9, person10) == 0);
            afis.SkipBestMatches = 6;
            Assert.That(afis.Verify(person9, person10) == 0);
        }
    }
}
