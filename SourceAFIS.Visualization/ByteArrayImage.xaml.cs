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

namespace SourceAFIS.Visualization
{
    public partial class ByteArrayImage : UserControl
    {
        public static readonly DependencyProperty InputArrayProperty
            = DependencyProperty.Register("InputArray", typeof(byte[,]), typeof(ByteArrayImage),
            new PropertyMetadata((self, args) => { (self as ByteArrayImage).UpdateImage(); }));
        public byte[,] InputArray
        {
            get { return (byte[,])GetValue(InputArrayProperty); }
            set { SetValue(InputArrayProperty, value); }
        }

        public static readonly DependencyProperty InvertedProperty
            = DependencyProperty.Register("Inverted", typeof(bool), typeof(ByteArrayImage),
            new PropertyMetadata(false, (self, args) => { (self as ByteArrayImage).UpdateImage(); }));
        public bool Inverted
        {
            get { return (bool)GetValue(InvertedProperty); }
            set { SetValue(InvertedProperty, value); }
        }

        static readonly DependencyPropertyKey ImageProperty
            = DependencyProperty.RegisterReadOnly("Image", typeof(ImageSource), typeof(ByteArrayImage), null);
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty.DependencyProperty); }
        }

        void UpdateImage()
        {
            if (IsVisible && InputArray != null)
            {
                byte[,] pixels = InputArray;
                int width = pixels.GetLength(1);
                int height = pixels.GetLength(0);

                byte[] flat = new byte[width * height];
                if (!Inverted)
                {
                    for (int y = 0; y < height; ++y)
                        for (int x = 0; x < width; ++x)
                            flat[(height - 1 - y) * width + x] = pixels[y, x];
                }
                else
                {
                    for (int y = 0; y < height; ++y)
                        for (int x = 0; x < width; ++x)
                            flat[(height - 1 - y) * width + x] = (byte)~pixels[y, x];
                }

                ImageSource image = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null, flat, width);
                SetValue(ImageProperty, image);
            }
            else
                SetValue(ImageProperty, null);
        }

        public ByteArrayImage()
        {
            InitializeComponent();
            IsVisibleChanged += (sender, args) => { UpdateImage(); };
        }
    }
}
