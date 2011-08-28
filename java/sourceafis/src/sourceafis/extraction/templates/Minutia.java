package sourceafis.extraction.templates;

import java.io.Serializable;

import sourceafis.general.Point;

public class Minutia implements Serializable {
		public Point Position=new Point(0,0);
		public byte Direction;
		public MinutiaType Type;
}