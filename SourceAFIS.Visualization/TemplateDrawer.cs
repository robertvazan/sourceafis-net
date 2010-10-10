using System;
using System.Collections.Generic;
using System.Text;
using SourceAFIS.General;
using SourceAFIS.Extraction.Templates;

namespace SourceAFIS.Visualization
{
    public static class TemplateDrawer
    {
        const int LineLength = 20;
        const int PenBalast = 1;

        public static void Draw(ColorF[,] output, TemplateBuilder template)
        {
            Draw(output, template, new ColorF(1, 0, 1), new ColorF(0, 1, 1));
        }

        public static void Draw(ColorF[,] output, TemplateBuilder template, ColorF endingColor, ColorF bifurcationColor)
        {
            RectangleC rect = new RectangleC(output.GetLength(1), output.GetLength(0));
            foreach (TemplateBuilder.Minutia minutia in template.Minutiae)
            {
                ColorF color;
                if (minutia.Type == TemplateBuilder.MinutiaType.Ending)
                    color = endingColor;
                else
                    color = bifurcationColor;
                Point directionVector = Calc.Round(Calc.Multiply(LineLength, Angle.ToVector(minutia.Direction)));
                Point[] line = Calc.ConstructLine(minutia.Position, Calc.Add(minutia.Position, directionVector));
                foreach (Point penPoint in new RectangleC(-PenBalast, -PenBalast, 2 * PenBalast + 1, 2 * PenBalast + 1))
                {
                    foreach (Point linePoint in line)
                    {
                        Point drawPoint = Calc.Add(linePoint, penPoint);
                        if (rect.Contains(drawPoint))
                            output[drawPoint.Y, drawPoint.X] = color;
                    }
                }
            }
        }
    }
}
