package sourceafis.meta;

import java.lang.reflect.Field;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.IdentityHashMap;

public class ObjectTree {
	private static class Item {
		public String path;
		public Object reference;
	}
	
	private HashMap<String, Item> byPath = new HashMap<String, Item>();
	private IdentityHashMap<Object, Item> byReference = new IdentityHashMap<Object, Item>();

	public ObjectTree() {}
	public ObjectTree(Object root) { scan(root); }
	public ObjectTree(Object root, String path) { scan(root, path); }
	public void scan(Object root) { scan(root, ""); }
	
	public void scan(Object root, String path) {
		if (!byReference.containsKey(root) && !byPath.containsKey(path)) {
			Item item = new Item();
			item.path = path;
			item.reference = root;
			byPath.put(path, item);
			byReference.put(root, item);
			
			try {
				for (Field field : root.getClass().getFields())
					if (field.isAnnotationPresent(Nested.class))
						scan(field.get(root), path + (!path.isEmpty() ? "." : "") + field.getName());
			} catch (IllegalAccessException e) {
				throw new RuntimeException(e);
			}
		}
	}
	
	public Object getObject(String path) { return byPath.get(path).reference; }
	public String getPath(Object reference) { return byReference.get(reference).path; }
	public ArrayList<Object> getAllObjects() { return new ArrayList<Object>(byReference.keySet()); }
	public ArrayList<String> getAllPaths() { return new ArrayList<String>(byPath.keySet()); }
	public boolean containsObject(Object reference) { return byReference.containsKey(reference); }
	public boolean containsPath(String path) { return byPath.containsKey(path); }
}
