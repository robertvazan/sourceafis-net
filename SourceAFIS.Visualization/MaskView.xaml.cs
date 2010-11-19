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
    public partial class MaskView : UserControl
    {
        public static readonly DependencyProperty MaskProperty
            = DependencyProperty.Register("Mask", typeof(BinaryMap), typeof(MaskView),
            new PropertyMetadata((self, args) => { (self as MaskView).UpdateImage(); }));
        public BinaryMap Mask
        {
            get { return (BinaryMap)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        public static readonly DependencyProperty ZeroColorProperty
            = DependencyProperty.Register("ZeroColor", typeof(Color), typeof(MaskView),
            new PropertyMetadata(Colors.Black, (self, args) => { (self as MaskView).UpdateImage(); }));
        public Color ZeroColor
        {
            get { return (Color)GetValue(ZeroColorProperty); }
            set { SetValue(ZeroColorProperty, value); }
        }

        public static readonly DependencyProperty OneColorProperty
            = DependencyProperty.Register("OneColor", typeof(Color), typeof(MaskView),
            new PropertyMetadata(Colors.White, (self, args) => { (self as MaskView).UpdateImage(); }));
        public Color OneColor
        {
            get { return (Color)GetValue(OneColorProperty); }
            set { SetValue(OneColorProperty, value); }
        }

        static readonly DependencyPropertyKey ImageProperty
            = DependencyProperty.RegisterReadOnly("Image", typeof(ImageSource), typeof(MaskView), null);
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty.DependencyProperty); }
        }

        void UpdateImage()
        {
            BinaryMap mask = Mask;

            if (IsVisible && mask != null)
            {
                int words = mask.WordWidth;
                int stride = words * BinaryMap.WordBytes;
                int height = mask.Height;

                byte[] pixels = new byte[height * stride];
                for (int y = 0; y < height; ++y)
                    for (int xw = 0; xw < words; ++xw)
                    {
                        uint word = Calc.ReverseBitsInBytes(mask.GetWord(xw, y));
                        int wordAt = (height - 1 - y) * stride + xw * BinaryMap.WordBytes;
                        for (int xb = 0; xb < BinaryMap.WordBytes; ++xb)
                            pixels[wordAt + xb] = (byte)(word >> (xb << 3));
                    }

                BitmapPalette palette = new BitmapPalette(new List<Color>() { ZeroColor, OneColor });
                BitmapSource image = BitmapSource.Create(mask.Width, height, 96, 96, PixelFormats.Indexed1, palette, pixels, stride);

                SetValue(ImageProperty, image);
            }
            else
                SetValue(ImageProperty, null);
        }

        public MaskView()
        {
            InitializeComponent();
            IsVisibleChanged += (sender, args) => { UpdateImage(); };
        }
    }
}
