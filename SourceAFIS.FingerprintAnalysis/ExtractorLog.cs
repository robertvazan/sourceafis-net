using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Extraction;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.FingerprintAnalysis
{
    public class ExtractorLog
    {
        Options OptionsReference;
        public Options Options
        {
            get { return OptionsReference; }
            set
            {
                if (OptionsReference != null)
                    OptionsReference.PropertyChanged -= OnOptionChanged;
                OptionsReference = value;
                if (OptionsReference != null)
                    OptionsReference.PropertyChanged += OnOptionChanged;
                Refresh();
            }
        }

        FingerprintOptions FingerprintOptionsReference;
        public FingerprintOptions FingerprintOptions
        {
            get { return FingerprintOptionsReference; }
            set
            {
                if (FingerprintOptionsReference != null)
                    FingerprintOptionsReference.PropertyChanged -= OnOptionChanged;
                FingerprintOptionsReference = value;
                if (FingerprintOptionsReference != null)
                    FingerprintOptionsReference.PropertyChanged += OnOptionChanged;
                Refresh();
            }
        }

        public byte[,] InputImage;
        public BlockMap Blocks;
        public byte[,] BlockContrast;
        public BinaryMap AbsoluteContrast;
        public BinaryMap RelativeContrast;
        public BinaryMap LowContrastMajority;
        public BinaryMap SegmentationMask;
        public float[,] Equalized;
        public byte[,] Orientation;
        public float[,] SmoothedRidges;
        public float[,] OrthogonalSmoothing;
        public BinaryMap Binarized;
        public BinaryMap BinarySmoothing;
        public BinaryMap RemovedCrosses;
        public BinaryMap InnerMask;
        public TemplateBuilder MinutiaCollector;
        public Template Template;

        DetailLogger Logger = new DetailLogger();
        Extractor Extractor = new Extractor();

        Dictionary<string, string> LogByProperty = new Dictionary<string,string>();
        
        public ExtractorLog()
        {
            ObjectTree tree = new ObjectTree();
            tree.Scan(Extractor, "Extractor");
            Logger.Attach(tree);

            LogByProperty.Add("Blocks", "Extractor.BlockMap");
            LogByProperty.Add("BlockContrast", "Extractor.Mask.Contrast");
            LogByProperty.Add("AbsoluteContrast", "Extractor.Mask.AbsoluteContrast");
            LogByProperty.Add("RelativeContrast", "Extractor.Mask.RelativeContrast");
            LogByProperty.Add("LowContrastMajority", "Extractor.Mask.LowContrastMajority");
            LogByProperty.Add("SegmentationMask", "Extractor.Mask");
            LogByProperty.Add("Equalized", "Extractor.Equalizer");
            LogByProperty.Add("Orientation", "Extractor.Orientation");
            LogByProperty.Add("SmoothedRidges", "Extractor.RidgeSmoother");
            LogByProperty.Add("OrthogonalSmoothing", "Extractor.OrthogonalSmoother");
            LogByProperty.Add("Binarized", "Extractor.Binarizer");
            LogByProperty.Add("BinarySmoothing", "Extractor.BinarySmoothingResult");
            LogByProperty.Add("RemovedCrosses", "Extractor.CrossRemover");
            LogByProperty.Add("InnerMask", "Extractor.InnerMask");
            LogByProperty.Add("MinutiaCollector", "Extractor.MinutiaCollector");
        }

        void OnOptionChanged(object source, PropertyChangedEventArgs args)
        {
            Refresh(args.PropertyName);
        }

        void Refresh()
        {
            var properties = from property in typeof(Options).GetProperties()
                             select property.Name;
            var fpProperties = from property in typeof(FingerprintOptions).GetProperties()
                               select property.Name;
            Refresh(properties.Concat(fpProperties).ToArray());
        }

        void Refresh(params string[] what)
        {
            if (what.Contains("Path"))
            {
                try
                {
                    InputImage = ImageIO.Load(FingerprintOptions.Path);
                }
                catch
                {
                    InputImage = null;
                }
            }

            if (InputImage != null)
            {
                Logger.Clear();
                Extractor.Extract(InputImage, 500);

                foreach (var pair in LogByProperty)
                    this.GetType().GetProperty(pair.Key).SetValue(this, Logger.Retrieve<object>(pair.Value), null);
                Template = new SerializedFormat().Export(MinutiaCollector);
                
                Logger.Clear();
            }
        }
    }
}
