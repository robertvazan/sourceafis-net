using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using NUnit.Framework;
using SourceAFIS.Simple;

namespace SimpleApiTests
{
    [TestFixture]
    public class FingerprintTest
    {
        [Test]
        public void DefaultConstructor()
        {
            Fingerprint fp = new Fingerprint();
            Assert.AreEqual(Finger.Any, fp.Finger);
            Assert.IsNull(fp.Template);
            Assert.IsNull(fp.Image);
            Assert.IsNull(fp.BitmapImage);
        }

        [Test]
        public void Image()
        {
            Fingerprint fp = new Fingerprint();
            byte[,] image = new byte[100, 100];
            fp.Image = image;
            Assert.AreSame(image, fp.Image);
            fp.Image = null;
            Assert.IsNull(fp.Image);
        }

        [Test]
        public void BitmapImage()
        {
            Fingerprint fp = new Fingerprint();
            Bitmap bitmap = Settings.SomeFingerprint;

            fp.BitmapImage = bitmap;
            Assert.IsNotNull(fp.Image);
            Assert.AreEqual(bitmap.Height, fp.Image.GetLength(0));
            Assert.AreEqual(bitmap.Width, fp.Image.GetLength(1));

            Bitmap bitmap2 = fp.BitmapImage;
            Assert.AreNotSame(bitmap, bitmap2);
            Assert.AreEqual(bitmap.Height, bitmap2.Height);
            Assert.AreEqual(bitmap.Width, bitmap2.Width);

            MemoryStream saved = new MemoryStream();
            bitmap2.Save(saved, ImageFormat.Bmp);
            Bitmap bitmap3 = new Bitmap(Bitmap.FromStream(saved));

            Fingerprint fp2 = new Fingerprint() { BitmapImage = bitmap3 };
            Assert.AreEqual(bitmap.Height, fp.Image.GetLength(0));
            Assert.AreEqual(bitmap.Width, fp.Image.GetLength(1));
            Assert.AreEqual(fp.Image, fp2.Image);

            fp.BitmapImage = null;
            Assert.IsNull(fp.Image);
        }

        [Test]
        public void Template()
        {
            Fingerprint fp1 = new Fingerprint() { BitmapImage = Settings.SomeFingerprint };
            Assert.IsNull(fp1.Template);

            AfisEngine afis = new AfisEngine();
            afis.Extract(fp1);
            Assert.IsNotNull(fp1.Template);

            byte[] saved = fp1.Template;
            Assert.AreNotSame(fp1.Template, saved);
            Assert.AreEqual(fp1.Template, saved);

            Fingerprint fp2 = new Fingerprint() { Template = fp1.Template };
            Assert.AreEqual(fp1.Template, fp2.Template);
            Assert.IsNull(fp2.Image);

            Person person1 = new Person() { fp1 };
            Person person2 = new Person() { fp2 };
            Person person3 = new Person() { new Fingerprint() { BitmapImage = Settings.NonMatchingFingerprint } };
            afis.Extract(person3[0]);
            afis.Threshold = 0;
            Assert.That(afis.Verify(person1, person2) > afis.Verify(person1, person3));
            Assert.AreEqual(afis.Verify(person1, person3), afis.Verify(person2, person3));

            fp1.Template = null;
            Assert.IsNull(fp1.Template);
        }

        [Test]
        public void Finger_GetSet()
        {
            Fingerprint fp = new Fingerprint();
            fp.Finger = Finger.LeftIndex;
            Assert.AreEqual(Finger.LeftIndex, fp.Finger);
        }

        [Test]
        public void Finger_Skipping()
        {
            AfisEngine afis = new AfisEngine();
            afis.Threshold = 0;
            Person person1 = new Person() { new Fingerprint() { BitmapImage = Settings.SomeFingerprint } };
            afis.Extract(person1[0]);
            Person person2 = new Person() { new Fingerprint() { BitmapImage = Settings.MatchingFingerprint } };
            afis.Extract(person2[0]);

            person1[0].Finger = Finger.RightThumb;
            person2[0].Finger = Finger.RightThumb;
            Assert.That(afis.Verify(person1, person2) > 0);
            Assert.That(afis.Identify(person1, new[] { person2 }) == person2);

            person2[0].Finger = Finger.LeftIndex;
            Assert.That(afis.Verify(person1, person2) == 0);
            Assert.That(afis.Identify(person1, new[] { person2 }) == null);

            person1[0].Finger = Finger.Any;
            Assert.That(afis.Verify(person1, person2) > 0);
            Assert.That(afis.Verify(person2, person1) > 0);
            Assert.That(afis.Identify(person1, new[] { person2 }) == person2);
            Assert.That(afis.Identify(person2, new[] { person1 }) == person1);

            person2[0].Finger = Finger.Any;
            Assert.That(afis.Verify(person1, person2) > 0);
            Assert.That(afis.Identify(person1, new[] { person2 }) == person2);

            Person person3 = new Person() { new Fingerprint() { BitmapImage = Settings.MatchingFingerprint } };
            afis.Extract(person3[0]);
            person1[0].Finger = Finger.LeftIndex;
            person2[0].Finger = Finger.LeftIndex;
            person3[0].Finger = Finger.RightMiddle;
            Assert.That(afis.Identify(person1, new[] { person2, person3 }) == person2);
            person1[0].Finger = Finger.RightMiddle;
            Assert.That(afis.Identify(person1, new[] { person2, person3 }) == person3);
        }

        [Test]
        public void Clone()
        {
            Fingerprint fp1 = new Fingerprint();
            fp1.BitmapImage = Settings.SomeFingerprint;
            fp1.Finger = Finger.RightThumb;
            AfisEngine afis = new AfisEngine();
            afis.Extract(fp1);
            Assert.IsNotNull(fp1.Template);

            Fingerprint fp2 = fp1.Clone();
            Assert.AreEqual(fp1.Image, fp2.Image);
            Assert.AreEqual(Finger.RightThumb, fp2.Finger);
            Assert.AreEqual(fp1.Template, fp2.Template);

            Fingerprint fp3 = new Fingerprint().Clone();
            Assert.IsNull(fp3.Image);
            Assert.AreEqual(Finger.Any, fp3.Finger);
            Assert.IsNull(fp3.Template);
        }

        [Test]
        public void Serialize()
        {
            Fingerprint fp1 = new Fingerprint();
            fp1.BitmapImage = Settings.SomeFingerprint;
            fp1.Finger = Finger.RightThumb;
            AfisEngine afis = new AfisEngine();
            afis.Extract(fp1);
            Assert.IsNotNull(fp1.Template);

            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, fp1);
            stream.Position = 0;
            Fingerprint fp2 = (Fingerprint)formatter.Deserialize(stream);

            Assert.AreEqual(fp1.Image, fp2.Image);
            Assert.AreEqual(fp1.Finger, fp2.Finger);
            Assert.AreEqual(fp1.Template, fp2.Template);
        }

        [Test]
        public void XmlSerialize()
        {
            Fingerprint fp1 = new Fingerprint();
            fp1.BitmapImage = Settings.SomeFingerprint;
            fp1.Finger = Finger.RightThumb;
            AfisEngine afis = new AfisEngine();
            afis.Extract(fp1);
            Assert.IsNotNull(fp1.Template);

            MemoryStream stream = new MemoryStream();
            XmlSerializer serializer = new XmlSerializer(typeof(Fingerprint));
            serializer.Serialize(stream, fp1);
            stream.Position = 0;
            Fingerprint fp2 = (Fingerprint)serializer.Deserialize(stream);

            Assert.AreEqual(fp1.Image, fp2.Image);
            Assert.AreEqual(fp1.Finger, fp2.Finger);
            Assert.AreEqual(fp1.Template, fp2.Template);
        }
    }
}
