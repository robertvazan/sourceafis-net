/**
 * @author Veaceslav Dubenco
 * @since 16.10.2012
 */
package sourceafis.meta;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * 
 */
@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.FIELD)
public @interface DpiAdjusted {
	double min() default 1;
}
