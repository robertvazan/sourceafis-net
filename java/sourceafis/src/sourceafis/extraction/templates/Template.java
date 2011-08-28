package sourceafis.extraction.templates;

import java.io.Serializable;

  

@SuppressWarnings("serial")
public class Template implements Cloneable, Serializable {
 
	public int OriginalDpi;
	public int OriginalWidth;
	public int OriginalHeight;
	public int StandardDpiWidth;
	public int StandardDpiHeight;
	public Minutia[] Minutiae;
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
		OriginalDpi = builder.OriginalDpi;
		OriginalWidth = builder.OriginalWidth;
		OriginalHeight = builder.OriginalHeight;
		StandardDpiWidth = builder.getStandardDpiWidth();
		StandardDpiHeight = builder.getStandardDpiHeight();

		Minutiae = new Minutia[builder.Minutiae.size()];
		for (int i = 0; i < builder.Minutiae.size(); ++i)
			Minutiae[i] = builder.Minutiae.get(i);//new Minutia(builder.Minutiae.get(i));
	}

	public TemplateBuilder ToTemplateBuilder() {
		TemplateBuilder builder = new TemplateBuilder();

		builder.OriginalDpi = OriginalDpi;
		builder.OriginalWidth = OriginalWidth;
		builder.OriginalHeight = OriginalHeight;
		for (Minutia minutia : Minutiae) {
			//builder.Minutiae.add(minutia.ToBuilderMinutia());
			builder.Minutiae.add(minutia);
		}
		return builder;
	}

	public Template clone() {
		return new Template(ToTemplateBuilder());
	}

 
}
