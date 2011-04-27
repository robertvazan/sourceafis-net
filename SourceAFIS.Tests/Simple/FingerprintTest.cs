using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using NUnit.Framework;
using SourceAFIS.Simple;

namespace SourceAFIS.Tests.Simple
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
            Assert.IsNull(fp.AsBitmapSource);
            Assert.IsNull(fp.AsImageData);
            Assert.IsNull(fp.AsIsoTemplate);
        }

        [Test]
        public void Image()
        {
            Fingerprint fp = new Fingerprint();
            byte[,] image = new byte[200, 200];
            fp.Image = image;
            Assert.AreSame(image, fp.Image);
            
            fp.Image = null;
            Assert.IsNull(fp.Image);

            Assert.Throws<ApplicationException>(() => { fp.Image = new byte[50, 200]; });
            Assert.Throws<ApplicationException>(() => { fp.Image = new byte[200, 50]; });
        }

        [Test]
        public void AsImageData()
        {
            Fingerprint fp = new Fingerprint();
            BitmapSource bitmap = Settings.SomeFingerprint;

            fp.AsBitmapSource = bitmap;
            byte[] data = fp.AsImageData;
            Assert.IsNotNull(data);
            Assert.AreEqual(bitmap.PixelHeight * bitmap.PixelWidth + 8, data.Length);

            Fingerprint fp2 = new Fingerprint() { AsImageData = data };
            Assert.AreEqual(bitmap.PixelHeight, fp.Image.GetLength(0));
            Assert.AreEqual(bitmap.PixelWidth, fp.Image.GetLength(1));

            byte[] data2 = fp2.AsImageData;
            Assert.AreNotSame(data, data2);
            Assert.AreEqual(data, data2);

            fp.AsImageData = null;
            Assert.IsNull(fp.Image);

            Assert.Throws<ApplicationException>(() => { fp.AsImageData = new byte[3]; });
            Assert.Throws<ApplicationException>(() => { fp.AsImageData = new byte[100]; });
            Assert.Throws<ApplicationException>(() =>
            {
                fp.AsImageData = BitConverter.GetBytes(200).Concat(BitConverter.GetBytes(200)).Concat(new byte[100]).ToArray();
            });
            Assert.Throws<ApplicationException>(() =>
            {
                fp.AsImageData = BitConverter.GetBytes(200).Concat(BitConverter.GetBytes(200)).Concat(new byte[100000]).ToArray();
            });
        }

        [Test]
        public void AsBitmapSource()
        {
            Fingerprint fp = new Fingerprint();
            BitmapSource bitmap = Settings.SomeFingerprint;

            fp.AsBitmapSource = bitmap;
            Assert.IsNotNull(fp.Image);
            Assert.AreEqual(bitmap.PixelHeight, fp.Image.GetLength(0));
            Assert.AreEqual(bitmap.PixelWidth, fp.Image.GetLength(1));

            BitmapSource bitmap2 = fp.AsBitmapSource;
            Assert.AreNotSame(bitmap, bitmap2);
            Assert.AreEqual(bitmap.PixelHeight, bitmap2.PixelHeight);
            Assert.AreEqual(bitmap.PixelWidth, bitmap2.PixelWidth);

            MemoryStream saved = new MemoryStream();
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap2));
            encoder.Save(saved);
            saved.Close();

            BitmapImage bitmap3 = new BitmapImage();
            bitmap3.BeginInit();
            bitmap3.StreamSource = new MemoryStream(saved.ToArray());
            bitmap3.EndInit();

            Fingerprint fp2 = new Fingerprint() { AsBitmapSource = bitmap3 };
            Assert.AreEqual(bitmap.PixelHeight, fp.Image.GetLength(0));
            Assert.AreEqual(bitmap.PixelWidth, fp.Image.GetLength(1));
            Assert.AreEqual(fp.Image, fp2.Image);

            fp.AsBitmapSource = null;
            Assert.IsNull(fp.Image);

            Assert.Throws<ApplicationException>(() => { fp.AsBitmapSource = new WriteableBitmap(50, 50, 500, 500, PixelFormats.Bgr32, null); });
        }

        [Test]
        public void Template()
        {
            Fingerprint fp1 = new Fingerprint() { AsBitmapSource = Settings.SomeFingerprint };
            Assert.IsNull(fp1.Template);

            AfisEngine afis = new AfisEngine();
            afis.Extract(new Person(fp1));
            Assert.IsNotNull(fp1.Template);

            byte[] saved = fp1.Template;
            Assert.AreNotSame(fp1.Template, saved);
            Assert.AreEqual(fp1.Template, saved);

            Fingerprint fp2 = new Fingerprint() { Template = fp1.Template };
            Assert.AreEqual(fp1.Template, fp2.Template);
            Assert.IsNull(fp2.Image);

            Person person1 = new Person(fp1);
            Person person2 = new Person(fp2);
            Person person3 = new Person(new Fingerprint() { AsBitmapSource = Settings.NonMatchingFingerprint });
            afis.Extract(person3);
            afis.Threshold = 0;
            Assert.That(afis.Verify(person1, person2) > afis.Verify(person1, person3));
            Assert.AreEqual(afis.Verify(person1, person3), afis.Verify(person2, person3));

            fp1.Template = null;
            Assert.IsNull(fp1.Template);

            Assert.Catch(() => { fp1.Template = new byte[100]; });
            Assert.Catch(() => { fp1.Template = fp2.AsIsoTemplate; });
        }

        [Test]
        public void AsIsoTemplate()
        {
            Fingerprint fp1 = new Fingerprint() { AsBitmapSource = Settings.SomeFingerprint };
            Assert.IsNull(fp1.AsIsoTemplate);

            AfisEngine afis = new AfisEngine();
            afis.Extract(new Person(fp1));
            Assert.IsNotNull(fp1.AsIsoTemplate);

            Fingerprint fp2 = new Fingerprint() { AsIsoTemplate = fp1.AsIsoTemplate };
            Assert.AreEqual(fp1.AsIsoTemplate, fp2.AsIsoTemplate);
            Assert.IsNotNull(fp2.Template);
            Assert.IsNull(fp2.Image);

            Person person1 = new Person(fp1);
            Person person2 = new Person(fp2);
            Person person3 = new Person(new Fingerprint() { AsBitmapSource = Settings.NonMatchingFingerprint });
            afis.Extract(person3);
            afis.Threshold = 0;
            Assert.That(afis.Verify(person1, person2) > afis.Verify(person1, person3));

            string isoFolder = Path.Combine(Settings.DataPath, "IsoTemplates");
            fp1.AsIsoTemplate = File.ReadAllBytes(Path.Combine(isoFolder, "1_1.ist"));
            fp2.AsIsoTemplate = File.ReadAllBytes(Path.Combine(isoFolder, "1_2.ist"));
            person3.Fingerprints[0].AsIsoTemplate = File.ReadAllBytes(Path.Combine(isoFolder, "2_2.ist"));
            Assert.That(afis.Verify(person1, person2) > afis.Verify(person2, person3));
            fp2.AsIsoTemplate = File.ReadAllBytes(Path.Combine(isoFolder, "2_1.ist"));
            Assert.That(afis.Verify(person1, person2) < afis.Verify(person2, person3));

            fp1.AsIsoTemplate = null;
            Assert.IsNull(fp1.AsIsoTemplate);

            Assert.Catch(() => { fp1.Template = new byte[100]; });
            Assert.Catch(() => { fp1.AsIsoTemplate = fp2.Template; });
        }

        [Test]
        public void FingerTest()
        {
            AfisEngine afis = new AfisEngine();
            afis.Threshold = 0.0001f;
            Person person1 = new Person(new Fingerprint() { AsBitmapSource = Settings.SomeFingerprint });
            afis.Extract(person1);
            Person person2 = new Person(new Fingerprint() { AsBitmapSource = Settings.MatchingFingerprint });
            afis.Extract(person2);

            person1.Fingerprints[0].Finger = Finger.RightThumb;
            Assert.AreEqual(Finger.RightThumb, person1.Fingerprints[0].Finger);
            person2.Fingerprints[0].Finger = Finger.RightThumb;
            Assert.That(afis.Verify(person1, person2) > 0);
            Assert.That(afis.Identify(person1, new[] { person2 }).Count() > 0);

            person2.Fingerprints[0].Finger = Finger.LeftIndex;
            Assert.That(afis.Verify(person1, person2) == 0);
            Assert.That(afis.Identify(person1, new[] { person2 }).Count() == 0);

            person1.Fingerprints[0].Finger = Finger.Any;
            Assert.That(afis.Verify(person1, person2) > 0);
            Assert.That(afis.Verify(person2, person1) > 0);
            Assert.That(afis.Identify(person1, new[] { person2 }).Count() > 0);
            Assert.That(afis.Identify(person2, new[] { person1 }).Count() > 0);

            person2.Fingerprints[0].Finger = Finger.Any;
            Assert.That(afis.Verify(person1, person2) > 0);
            Assert.That(afis.Identify(person1, new[] { person2 }).Count() > 0);

            Person person3 = new Person(new Fingerprint() { AsBitmapSource = Settings.MatchingFingerprint });
            afis.Extract(person3);
            person1.Fingerprints[0].Finger = Finger.LeftIndex;
            person2.Fingerprints[0].Finger = Finger.LeftIndex;
            person3.Fingerprints[0].Finger = Finger.RightMiddle;
            CollectionAssert.AreEqual(afis.Identify(person1, new[] { person2, person3 }), new[] { person2 });
            person1.Fingerprints[0].Finger = Finger.RightMiddle;
            CollectionAssert.AreEqual(afis.Identify(person1, new[] { person2, person3 }), new[] { person3 });

            Assert.Catch(() => { person1.Fingerprints[0].Finger = (Finger)(-1); });
        }

        [Test]
        public void Clone()
        {
            Fingerprint fp1 = new Fingerprint();
            fp1.AsBitmapSource = Settings.SomeFingerprint;
            fp1.Finger = Finger.RightThumb;
            AfisEngine afis = new AfisEngine();
            afis.Extract(new Person(fp1));
            Assert.IsNotNull(fp1.Template);

            Fingerprint fp2 = fp1.Clone();
            Assert.AreEqual(fp1.Image, fp2.Image);
            Assert.AreEqual(Finger.RightThumb, fp2.Finger);
            Assert.AreEqual(fp1.Template, fp2.Template);

            Fingerprint fp3 = new Fingerprint().Clone();
            Assert.IsNull(fp3.Image);
            Assert.AreEqual(Finger.Any, fp3.Finger);
            Assert.IsNull(fp3.Template);

            Fingerprint fp4 = (fp2 as ICloneable).Clone() as Fingerprint;
            Assert.AreEqual(fp1.Image, fp4.Image);
            Assert.AreEqual(Finger.RightThumb, fp4.Finger);
            Assert.AreEqual(fp1.Template, fp4.Template);
        }

        [Test]
        public void Serialize()
        {
            Fingerprint fp1 = new Fingerprint();
            fp1.AsBitmapSource = Settings.SomeFingerprint;
            fp1.Finger = Finger.RightThumb;
            AfisEngine afis = new AfisEngine();
            afis.Extract(new Person(fp1));
            Assert.IsNotNull(fp1.Template);

            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, fp1);
            stream.Position = 0;
            Fingerprint fp2 = formatter.Deserialize(stream) as Fingerprint;

            Assert.AreEqual(fp1.Image, fp2.Image);
            Assert.AreEqual(fp1.Finger, fp2.Finger);
            Assert.AreEqual(fp1.Template, fp2.Template);
        }

        [Test]
        public void XmlSerialize()
        {
            Fingerprint fp1 = new Fingerprint();
            fp1.AsBitmapSource = Settings.SomeFingerprint;
            fp1.Finger = Finger.RightThumb;
            AfisEngine afis = new AfisEngine();
            afis.Extract(new Person(fp1));
            Assert.IsNotNull(fp1.Template);

            MemoryStream stream = new MemoryStream();
            XmlSerializer serializer = new XmlSerializer(typeof(Fingerprint));
            serializer.Serialize(stream, fp1);
            stream.Position = 0;
            Fingerprint fp2 = serializer.Deserialize(stream) as Fingerprint;

            Assert.AreEqual(fp1.Image, fp2.Image);
            Assert.AreEqual(fp1.Finger, fp2.Finger);
            Assert.AreEqual(fp1.Template, fp2.Template);
        }
    }
}
