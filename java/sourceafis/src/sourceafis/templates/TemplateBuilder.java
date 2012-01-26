package sourceafis.templates;

import java.util.ArrayList;
import java.util.List;

import sourceafis.general.Calc;
 
public class TemplateBuilder {
	 
	public List<Minutia> Minutiae = new ArrayList<Minutia>();

	/*
	public static class Minutia {
		public Point Position=new Point(0,0);
		public byte Direction;
		public MinutiaType Type;
	}
	*/
	public int OriginalDpi;
	public int OriginalWidth;
	public int OriginalHeight;

	public int getStandardDpiWidth() {
		return Calc.DivRoundUp(OriginalWidth * 500, OriginalDpi);
	}

	public void setStandardDpiWidth(int value) {
		OriginalWidth = value * OriginalDpi / 500;
	}

	public int getStandardDpiHeight() {
		return Calc.DivRoundUp(OriginalHeight * 500, OriginalDpi);
	}

	public void setStandardDpiHeight(int value) {
		OriginalHeight = value * OriginalDpi / 500;
	}
 

}
