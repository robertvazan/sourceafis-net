package sourceafis.matching.minutia;

import java.util.ArrayList;
import java.util.Hashtable;
import sourceafis.templates.Template;
/*
 * This object should be used like a prototype 
 */
public final class EdgeHash {
	protected EdgeLookup edgeLookup;
	protected Hashtable<Integer,Object> hash = new Hashtable<Integer,Object>();

	@SuppressWarnings("unchecked")
	public EdgeHash(Template template, EdgeLookup lookup) {
		this.edgeLookup = lookup;
		EdgeConstructor edgeConstructor = new EdgeConstructor();
		for (int referenceMinutia = 0; referenceMinutia < template.Minutiae.length; ++referenceMinutia)
			for (int neighborMinutia = 0; neighborMinutia < template.Minutiae.length; ++neighborMinutia)
				if (referenceMinutia != neighborMinutia) {
					
					IndexedEdge edge = new IndexedEdge();
					edge.shape = edgeConstructor.Construct(template,
							referenceMinutia, neighborMinutia);
					edge.location = new EdgeLocation(referenceMinutia,
							neighborMinutia);

					for (Integer key : lookup.HashCoverage(edge.shape)) {

						if (!hash.containsKey(key)) {
							 hash.put(key, edge);//first time only one object
						} else {//It can be single object or a ArrayList
							Object value=hash.get(key);
							if (value instanceof IndexedEdge) {
								ArrayList<IndexedEdge> list=new ArrayList<IndexedEdge>();	
							    list.add((IndexedEdge)value);
							    list.add(edge);
							    hash.put(key,list);
							} else {//list is available
								ArrayList<IndexedEdge> list=(ArrayList<IndexedEdge>)value;
								list.add(edge);
							}
						}
					}
				}
	}
}