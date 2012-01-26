package sourceafis.matching.minutia;

import sourceafis.general.Angle;
import sourceafis.general.Calc;
import sourceafis.general.PolarPoint;
import sourceafis.templates.Template;
/*
 *  Call to Construct can be static and no need to create EdgeConstructor object
 */
public final class EdgeConstructor
{
	   public EdgeShape Construct(Template template, int reference, int neighbor)
       {
           PolarPoint polar = Angle.ToPolar(Calc.Difference(template.Minutiae[neighbor].Position, template.Minutiae[reference].Position));
           EdgeShape edge=new EdgeShape();
           edge.length = (short)polar.Distance;
           edge.referenceAngle = Angle.Difference(template.Minutiae[reference].Direction, polar.Angle);
           edge.neighborAngle = Angle.Difference(template.Minutiae[neighbor].Direction, Angle.Opposite(polar.Angle));
           return edge;
       }
}