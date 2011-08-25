package sourceafis.general;

import java.lang.reflect.Field;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.lang.reflect.Modifier;
import java.util.ArrayList;
import java.util.HashMap;

import sourceafis.meta.ObjectTree;

public class DetailLogger {
	public static final Hook off = new NullHook();
	private LogData currentLog = new LogData();
	
	public void attach(ObjectTree tree) {
		try {
			for (Object reference : tree.getAllObjects())
				for (Field field : reference.getClass().getFields())
					if (field.getName().equals("logger"))
						field.set(reference, new ActiveHook(tree.getPath(reference)));
		} catch (IllegalAccessException e) {
			throw new RuntimeException(e);
		}
	}
	
	public synchronized void log(String path, Object data) {
		Object logged = data;
		try {
			Method clone = data.getClass().getMethod("clone");
			if (Modifier.isPublic(clone.getModifiers()))
				logged = clone.invoke(data);
		} catch (NoSuchMethodException e) {
			throw new RuntimeException(e);
		} catch (InvocationTargetException e) {
			throw new RuntimeException(e);
		} catch (IllegalAccessException e) {
			throw new RuntimeException(e);
		}
		currentLog.append(path, logged);
	}
	
	public LogData popLog() {
		LogData result = currentLog;
		currentLog = new LogData();
		return result;
	}
	
	public static abstract class Hook {
        public abstract boolean isActive();
        public abstract void log(Object data);
        public abstract void log(String part, Object data);
	}
	
	private static class NullHook extends Hook {
        public boolean isActive() { return false; }
        public void log(Object data) {}
        public void log(String part, Object data) {}
	}
	
	private class ActiveHook extends Hook implements Cloneable {
		private String path;
		
		public ActiveHook(String p) { path = p; }
        public boolean isActive() { return true; }
        public void log(Object data) { DetailLogger.this.log(path, data); }
        public void log(String part, Object data) {
        	DetailLogger.this.log(path + (path != "" ? "." : "") + part, data);
    	}
	}
	
	public class LogData {
		private HashMap<String, ArrayList<Object>> history = new HashMap<String, ArrayList<Object>>();
		
		public void append(String path, Object data) {
			if (!history.containsKey(path))
				history.put(path, new ArrayList<Object>());
			history.get(path).add(data);
		}
		
		public Object retrieve(String path, int index) {
			if (history.containsKey(path) && index < history.get(path).size())
				return history.get(path).get(index);
			else
				return null;
		}
		
		public Object retrieve(String path) { return retrieve(path, 0); }
	}
}
