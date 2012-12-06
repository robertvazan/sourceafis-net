/**
 * @author Veaceslav Dubenco
 * @since 16.10.2012
 */
package sourceafis.meta;

import java.lang.annotation.Annotation;
import java.lang.reflect.Field;
import java.util.ArrayList;
import java.util.List;

/**
 * 
 */
public final class DpiAdjuster {
	public int DefaultDpi = 500;

	class Parameter {
		public ParameterValue Value;
		public double OriginalValue;
		public DpiAdjusted Attribute;

		public void Adjust(double factor) {
			Value.value = OriginalValue * factor;
			if (Value.value < Attribute.min())
				Value.value = Attribute.min();
			Value.write();
		}

		public void Revert() {
			Value.value = OriginalValue;
			Value.write();
		}
	}

	List<Parameter> Parameters = new ArrayList<Parameter>();

	public void Initialize(ObjectTree tree) {
		Parameters.clear();
		for (String path : tree.getAllPaths()) {
			Object nested = tree.getObject(path);
			for (Field field : nested.getClass().getFields()) {
				for (Annotation attribute : field.getAnnotations()) {
					if (attribute instanceof DpiAdjusted) {
						Parameter parameter = new Parameter();
						parameter.Value = new ParameterValue(path, tree, field);
						parameter.OriginalValue = parameter.Value.value;
						parameter.Attribute = (DpiAdjusted) attribute;
						Parameters.add(parameter);
					}
				}
			}
		}
	}

	public void Adjust(int dpi) {
		double factor = dpi / (double) DefaultDpi;
		for (Parameter parameter : Parameters)
			parameter.Adjust(factor);
	}

	public void Revert() {
		for (Parameter parameter : Parameters)
			parameter.Revert();
	}

	public void Adjust(Object root, int dpi, Action function) {
		ObjectTree tree = new ObjectTree();
		tree.scan(root, "Dpi");
		Initialize(tree);
		try {
			Adjust(dpi);
			function.function();
		} finally {
			Revert();
		}
	}
}
