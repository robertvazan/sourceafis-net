package sourceafis.templates;

import java.util.ArrayList;
import java.util.List;

import sourceafis.general.Calc;
 
public class TemplateBuilder {
	 
	public List<Minutia> minutiae = new ArrayList<Minutia>();

	/*
	public static class Minutia {
		public Point Position=new Point(0,0);
		public byte Direction;
		public MinutiaType Type;
	}
	*/
	public int originalDpi;
	public int originalWidth;
	public int originalHeight;

	public int getStandardDpiWidth() {
		return Calc.DivRoundUp(originalWidth * 500, originalDpi);
	}

	public void setStandardDpiWidth(int value) {
		originalWidth = value * originalDpi / 500;
	}

	public int getStandardDpiHeight() {
		return Calc.DivRoundUp(originalHeight * 500, originalDpi);
	}

	public void setStandardDpiHeight(int value) {
		originalHeight = value * originalDpi / 500;
	}
 

}
