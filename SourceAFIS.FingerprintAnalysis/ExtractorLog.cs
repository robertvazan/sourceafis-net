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
        
        public ExtractorLog()
        {
            ObjectTree tree = new ObjectTree();
            tree.Scan(Extractor, "Extractor");
            Logger.Attach(tree);
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

                Blocks = Logger.Retrieve<BlockMap>("Extractor.BlockMap");
                BlockContrast = Logger.Retrieve<byte[,]>("Extractor.Mask.Contrast");
                AbsoluteContrast = Logger.Retrieve<BinaryMap>("Extractor.Mask.AbsoluteContrast");
                RelativeContrast = Logger.Retrieve<BinaryMap>("Extractor.Mask.RelativeContrast");
                LowContrastMajority = Logger.Retrieve<BinaryMap>("Extractor.Mask.LowContrastMajority");
                SegmentationMask = Logger.Retrieve<BinaryMap>("Extractor.Mask");
                Equalized = Logger.Retrieve<float[,]>("Extractor.Equalizer");
                Orientation = Logger.Retrieve<byte[,]>("Extractor.Orientation");
                SmoothedRidges = Logger.Retrieve<float[,]>("Extractor.RidgeSmoother");
                OrthogonalSmoothing = Logger.Retrieve<float[,]>("Extractor.OrthogonalSmoother");
                Binarized = Logger.Retrieve<BinaryMap>("Extractor.Binarizer");
                BinarySmoothing = Logger.Retrieve<BinaryMap>("Extractor.BinarySmoothingResult");
                RemovedCrosses = Logger.Retrieve<BinaryMap>("Extractor.CrossRemover");
                InnerMask = Logger.Retrieve<BinaryMap>("Extractor.InnerMask");
                MinutiaCollector = Logger.Retrieve<TemplateBuilder>("Extractor.MinutiaCollector");
                Template = new SerializedFormat().Export(MinutiaCollector);
                
                Logger.Clear();
            }
        }
    }
}
