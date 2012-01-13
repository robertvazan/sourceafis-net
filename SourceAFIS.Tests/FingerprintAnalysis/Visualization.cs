using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using White.Core.UIItems;
using White.Core.UIItems.ListBoxItems;

namespace SourceAFIS.Tests.FingerprintAnalysis
{
    [TestFixture, RequiresSTA]
    [Category("UI")]
    public class Visualization : Common
    {
        Dictionary<string, int> Checksums;

        void SaveChecksum(Dictionary<string, string> options, string key)
        {
            SetOptions(options);
            SaveChecksum(key);
        }

        void SaveChecksum(string key)
        {
            SaveChecksum(Left, "Left: " + key);
            SaveChecksum(Right, "Right: " + key);
        }

        void SaveChecksum(MatchSide side, string key)
        {
            Checksums.Add(key, ChecksumImage(side));
        }

        [Test]
        public void AllDifferent()
        {
            SelectFiles();

            List<Dictionary<string, int>> retries = new List<Dictionary<string, int>>();

            for (int retry = 0; retry < 2; ++retry)
            {
                Checksums = new Dictionary<string, int>();

                SaveChecksum(GetResetOptions(), "Reset");
                SaveChecksum(GetFullOptions(), "Full");
                
                foreach (string name in OptionNames)
                {
                    UIItem control = GetOptionControl(name);
                    if (control is CheckBox)
                        SaveChecksum(GetSingleOption(name, Boolean.TrueString), name);
                }
                
                foreach (string value in BitmapLayerChoice.Items.Select(item => item.Name))
                    if (value != "OriginalImage")
                        SaveChecksum(GetSingleOption("BitmapLayerChoice", value), "BitmapLayerChoice: " + value);

                var markerValues = MarkerLayerChoice.Items.Select(item => item.Name).ToList();
                foreach (string value in markerValues)
                    if (!new[] { "UniqueMinutiaSorter", "MinutiaCloudRemover", "UniqueMinutiaSorter" }.Contains(value))
                    {
                        SaveChecksum(GetSingleOption("MarkerLayerChoice", value), "MarkerLayerChoice: " + value);
                        if (markerValues.IndexOf(value) >= 1 && markerValues.IndexOf(value) <= 8)
                        {
                            Dictionary<string, string> valleys = GetSingleOption("MarkerLayerChoice", value);
                            valleys["SkeletonChoice"] = "Valleys";
                            SaveChecksum(valleys, "MarkerLayerChoice (valleys): " + value);
                        }
                    }

                foreach (string value in MaskChoice.Items.Select(item => item.Name))
                    if (value != "None" && value != "LowContrastMajority")
                        SaveChecksum(GetSingleOption("MaskChoice", value), "MaskChoice: " + value);

                CollectionAssert.AllItemsAreUnique(Checksums);
                retries.Add(Checksums);
            }

            CollectionAssert.AreEquivalent(retries[0], retries[1]);
        }
    }
}
