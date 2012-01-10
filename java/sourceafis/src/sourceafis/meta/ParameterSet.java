package sourceafis.meta;

import java.lang.reflect.Field;
import java.lang.reflect.InvocationTargetException;
import java.util.ArrayList;
import java.util.HashMap;

import sourceafis.general.DetailLogger;

public class ParameterSet {
	private HashMap<String, ParameterValue> byPath = new HashMap<String, ParameterValue>();
	
	public ArrayList<ParameterValue> getAllParameters() {
		return new ArrayList<ParameterValue>(byPath.values());
	}

	public ParameterSet(ObjectTree tree) { add(tree); }

	public void add(ObjectTree tree) {
		for (String objectPath : tree.getAllPaths())
			for (Field field : tree.getObject(objectPath).getClass().getFields())
				if (field.isAnnotationPresent(Parameter.class))
					add(new ParameterValue(objectPath, tree, field));
	}
	
	public void add(ParameterValue parameter) {
		byPath.put(parameter.fieldPathNoCase, parameter);
	}
	
	public ParameterValue get(String path) {
		return byPath.get(path.toLowerCase());
	}
	
	public boolean contains(String path) {
		return byPath.containsKey(path.toLowerCase());
	}
	
	public void rebind(ObjectTree tree) {
		for (ParameterValue parameter : getAllParameters())
			parameter.rebind(tree);
	}
	
	public void read() {
		for (ParameterValue parameter : getAllParameters())
			parameter.read();
	}
	
	public void write() {
		for (ParameterValue parameter : getAllParameters())
			parameter.write();
	}
	
	public static <T> T clonePrototype(T original) {
		try {
			@SuppressWarnings("unchecked")
			T clone = (T)original.getClass().getConstructor().newInstance();
			ParameterSet parameters = new ParameterSet(new ObjectTree(original));
			parameters.rebind(new ObjectTree(clone));
			parameters.write();
			DetailLogger.copyHooks(original, clone);
			return clone;
		} catch (NoSuchMethodException e) {
			throw new RuntimeException(e);
		} catch (InvocationTargetException e) {
			throw new RuntimeException(e);
		} catch (IllegalAccessException e) {
			throw new RuntimeException(e);
		} catch (InstantiationException e) {
			throw new RuntimeException(e);
		}
	}
}
