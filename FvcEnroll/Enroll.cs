using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using SourceAFIS.Simple;

namespace FvcEnroll
{
    class Enroll
    {
        static void WriteLog(string[] args, string status)
        {
            using (FileStream stream = File.Open(args[2], FileMode.Append))
            {
                using (TextWriter writer = new StreamWriter(stream))
                    writer.WriteLine("{0} {1} {2}", args[0], args[1], status);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                AfisEngine afis = new AfisEngine();
                Fingerprint fp = new Fingerprint();
                using (Image image = Image.FromFile(args[0]))
                {
                    using (Bitmap bitmap = new Bitmap(image))
                        fp.AsBitmap = bitmap;
                }
                afis.Extract(fp);
                File.WriteAllBytes(args[1], fp.Template);
                WriteLog(args, "OK");
            }
            catch (Exception)
            {
                WriteLog(args, "FAIL");
                throw;
            }
        }
    }
}
