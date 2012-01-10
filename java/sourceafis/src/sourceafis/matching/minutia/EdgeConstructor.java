package sourceafis.matching.minutia;

import sourceafis.extraction.templates.Template;
import sourceafis.general.Angle;
import sourceafis.general.Calc;
import sourceafis.general.PolarPoint;
 public  class EdgeConstructor
    {
	   public EdgeInfo Construct(Template template, int reference, int neighbor)
        {
        
            PolarPoint polar = Angle.ToPolar(Calc.Difference(template.Minutiae[neighbor].Position, template.Minutiae[reference].Position));
            EdgeInfo edge=new EdgeInfo();
            edge.Length = (short)polar.Distance;
            edge.ReferenceAngle = Angle.Difference(template.Minutiae[reference].Direction, polar.Angle);
            edge.NeighborAngle = Angle.Difference(template.Minutiae[neighbor].Direction, Angle.Opposite(polar.Angle));
            return edge;
        }
    }