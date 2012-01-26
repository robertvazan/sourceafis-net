package sourceafis.templates;

import java.io.Serializable;

import sourceafis.general.Point;

@SuppressWarnings("serial")
public class Minutia implements Serializable {
		public Point Position=new Point(0,0);
		public byte Direction;
		public MinutiaType Type;
}