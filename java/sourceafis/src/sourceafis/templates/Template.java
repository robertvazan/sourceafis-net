package sourceafis.templates;

import java.io.Serializable;

  

@SuppressWarnings("serial")
public class Template implements Cloneable, Serializable {
 
	public int originalDpi;
	public int originalWidth;
	public int originalHeight;
	public int standardDpiWidth;
	public int standardDpiHeight;
	public Minutia[] minutiae;
	public Object matcherCache;
   /*
	public class Minutia {
		public PointS Position;
		public byte Direction;
		public MinutiaType Type;

		/*public Minutia(TemplateBuilder.Minutia builderMinutia) {
			Position = new PointS(builderMinutia.Position);
			Direction = builderMinutia.Direction;
			if (builderMinutia.Type == MinutiaType.Ending)
				Type = MinutiaType.Ending;
			else
				Type = MinutiaType.Bifurcation;
		}

		public TemplateBuilder.Minutia ToBuilderMinutia() {
			 TemplateBuilder.Minutia builderMinutia = new  TemplateBuilder.Minutia();// new
																							// TemplateBuilder.Minutia();
			builderMinutia.Position = Position.ToPoint();
			builderMinutia.Direction = Direction;
			if (Type == MinutiaType.Ending)
				builderMinutia.Type = MinutiaType.Ending;
			else
				builderMinutia.Type = MinutiaType.Bifurcation;
			return builderMinutia;
		} 
	}*/
	
	public Template(TemplateBuilder builder) {
		originalDpi = builder.originalDpi;
		originalWidth = builder.originalWidth;
		originalHeight = builder.originalHeight;
		standardDpiWidth = builder.getStandardDpiWidth();
		standardDpiHeight = builder.getStandardDpiHeight();

		minutiae = new Minutia[builder.minutiae.size()];
		for (int i = 0; i < builder.minutiae.size(); ++i)
			minutiae[i] = builder.minutiae.get(i);//new Minutia(builder.Minutiae.get(i));
	}

	public TemplateBuilder toTemplateBuilder() {
		TemplateBuilder builder = new TemplateBuilder();

		builder.originalDpi = originalDpi;
		builder.originalWidth = originalWidth;
		builder.originalHeight = originalHeight;
		for (Minutia minutia : minutiae) {
			//builder.Minutiae.add(minutia.ToBuilderMinutia());
			builder.minutiae.add(minutia);
		}
		return builder;
	}

	public Template clone() {
		return new Template(toTemplateBuilder());
	}

 
}
