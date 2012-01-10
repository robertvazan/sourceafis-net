package sourceafis.meta;

import java.lang.reflect.Field;

public class ParameterValue {
    public String objectPath;
    public Object objectReference;
    public String fieldPath;
    public String fieldPathNoCase;
    public Field field;
    public Parameter annotation;
	public double value;
	
	public ParameterValue(String path, ObjectTree tree, Field fieldInfo) {
		objectPath = path;
		objectReference = tree.getObject(path);
		field = fieldInfo;
		fieldPath = objectPath + "." + field.getName();
		fieldPathNoCase = fieldPath.toLowerCase();
		annotation = field.getAnnotation(Parameter.class);
		read();
	}

	public int getPrecision() {
		if (annotation.precision() != -1)
			return annotation.precision();
		else if (field.getType().getName() == "float" || field.getType().getName() == "double")
			return 2;
		else
			return 0;
	}
	
	public double getPrecisionMultiplier() {
		return Math.pow(10, getPrecision());
	}
	
	public void read() {
		String type = field.getType().getName();
		try {
			if (type.equals("double"))
				value = field.getDouble(objectReference);
			else if (type.equals("float"))
				value = field.getFloat(objectReference);
			else if (type.equals("int"))
				value = field.getInt(objectReference);
			else if (type.equals("byte"))
				value = field.getByte(objectReference);
			else
				throw new RuntimeException();
		} catch (IllegalAccessException e) {
			throw new RuntimeException(e);
		}
		value = Math.round(value * getPrecisionMultiplier()) / getPrecisionMultiplier();
	}
	
	public void write() {
		String type = field.getType().getName();
		try {
			if (type.equals("double"))
				field.setDouble(objectReference, value);
			else if (type.equals("float"))
				field.setFloat(objectReference, (float)value);
			else if (type.equals("int"))
				field.setInt(objectReference, (int)Math.round(value));
			else if (type.equals("byte"))
				field.setByte(objectReference, (byte)Math.round(value));
			else
				throw new RuntimeException();
		} catch (IllegalAccessException e) {
			throw new RuntimeException(e);
		}
	}
	
	public void rebind(ObjectTree tree) {
		objectReference = tree.getObject(objectPath);
	}
}
