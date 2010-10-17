using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SourceAFIS.General;
using SourceAFIS.Meta;
using SourceAFIS.Extraction;

namespace SourceAFIS.FingerprintAnalysis
{
    public class ExtractionCollector : CollectorBase
    {
        Extractor Extractor = new Extractor();

        byte[,] InputImageValue;
        public byte[,] InputImage
        {
            get { return InputImageValue; }
            set { InputImageValue = value; OnPropertyChanged("InputImage"); }
        }

        public ExtractionCollector(FingerprintOptions options)
        {
            Logger.Attach(new ObjectTree(Extractor));
            Collect(options);
            options.PropertyChanged += OnOptionsChange;
        }

        void OnOptionsChange(object source, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Path")
                Collect(source as FingerprintOptions);
        }

        void Collect(FingerprintOptions options)
        {
            InputImage = options.Path != "" ? ImageIO.GetPixels(ImageIO.Load(options.Path)) : null;

            if (InputImage != null)
                Extractor.Extract(InputImage, 500);
            Logs = Logger.PopLog();
        }
    }
}
