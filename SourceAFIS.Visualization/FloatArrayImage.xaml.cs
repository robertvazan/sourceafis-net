using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SourceAFIS.General;

namespace SourceAFIS.Visualization
{
    public partial class FloatArrayImage : UserControl
    {
        public static readonly DependencyProperty InputArrayProperty
            = DependencyProperty.Register("InputArray", typeof(float[,]), typeof(FloatArrayImage),
            new PropertyMetadata((self, args) => { (self as FloatArrayImage).UpdateImage(); }));
        public float[,] InputArray
        {
            get { return (float[,])GetValue(InputArrayProperty); }
            set { SetValue(InputArrayProperty, value); }
        }

        public static readonly DependencyProperty WhiteValueProperty
            = DependencyProperty.Register("WhiteValue", typeof(float), typeof(FloatArrayImage),
            new PropertyMetadata(-1f, (self, args) => { (self as FloatArrayImage).UpdateImage(); }));
        public float WhiteValue
        {
            get { return (float)GetValue(WhiteValueProperty); }
            set { SetValue(WhiteValueProperty, value); }
        }

        public static readonly DependencyProperty BlackValueProperty
            = DependencyProperty.Register("BlackValue", typeof(float), typeof(FloatArrayImage),
            new PropertyMetadata(1f, (self, args) => { (self as FloatArrayImage).UpdateImage(); }));
        public float BlackValue
        {
            get { return (float)GetValue(BlackValueProperty); }
            set { SetValue(BlackValueProperty, value); }
        }

        public static readonly DependencyProperty NormalizedProperty
            = DependencyProperty.Register("Normalized", typeof(bool), typeof(FloatArrayImage),
            new PropertyMetadata(false, (self, args) => { (self as FloatArrayImage).UpdateImage(); }));
        public bool Normalized
        {
            get { return (bool)GetValue(NormalizedProperty); }
            set { SetValue(NormalizedProperty, value); }
        }

        static readonly DependencyPropertyKey ImageProperty
            = DependencyProperty.RegisterReadOnly("Image", typeof(ImageSource), typeof(FloatArrayImage), null);
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty.DependencyProperty); }
        }

        void UpdateImage()
        {
            if (IsVisible && InputArray != null && BlackValue != WhiteValue)
            {
                float[,] pixels = InputArray;
                int width = pixels.GetLength(1);
                int height = pixels.GetLength(0);
                
                float white = WhiteValue;
                float black = BlackValue;
                if (Normalized)
                {
                    white = Single.MaxValue;
                    black = Single.MinValue;
                    for (int y = 0; y < height; ++y)
                        for (int x = 0; x < width; ++x)
                        {
                            if (pixels[y, x] < white)
                                white = pixels[y, x];
                            if (pixels[y, x] > black)
                                black = pixels[y, x];
                        }
                }
                float scaling = 1 / (white - black) * 255;

                byte[] flat = new byte[width * height];
                for (int y = 0; y < height; ++y)
                    for (int x = 0; x < width; ++x)
                        flat[(height - 1 - y) * width + x] = Convert.ToByte((pixels[y, x] - black) * scaling);
                
                ImageSource image = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null, flat, width);
                SetValue(ImageProperty, image);
            }
            else
                SetValue(ImageProperty, null);
        }

        public FloatArrayImage()
        {
            InitializeComponent();
            IsVisibleChanged += (sender, args) => { UpdateImage(); };
        }
    }
}
