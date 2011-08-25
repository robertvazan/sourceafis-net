package sourceafis.matching.minutia;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

import sourceafis.extraction.templates.Template;
import sourceafis.general.Calc;
import sourceafis.general.Point;
import sourceafis.meta.Nested;
 
 public   class EdgeTable
    {
        @Nested
        public EdgeConstructor EdgeConstructor = new EdgeConstructor();

        //[Parameter(Lower = 30, Upper = 1500)]
        public int MaxDistance = 191;
        //[Parameter(Lower = 2, Upper = 100)]
        public int MaxNeighbors = 9;

        public NeighborEdge[][] Table;

        public void Reset(Template template)
        {
        	
            Table = new NeighborEdge[template.Minutiae.length][];
            List<NeighborEdge> edges = new ArrayList<NeighborEdge>();

            for (int reference = 0; reference < Table.length; ++reference)
            {
            	//PointS to Point
                Point referencePosition = template.Minutiae[reference].Position;
                for (int neighbor = 0; neighbor < template.Minutiae.length; ++neighbor)
                {
                    if (Calc.DistanceSq(referencePosition, template.Minutiae[neighbor].Position)
                        <= Calc.Sq(MaxDistance) && neighbor != reference)
                    {
                        NeighborEdge record = new NeighborEdge();
                        record.Edge = EdgeConstructor.Construct(template, reference, neighbor);
                        record.Neighbor = neighbor;
                        edges.add(record);
                    }
                }
                 /*
                  * Review below porting from net to java 
                  * The sorted order is different in java and .net and java when Edge length 
                  * are same
                  */
                //edges.Sort((left, right) => Calc.Compare(left.Edge.Length, right.Edge.Length));
               Collections.sort(edges,new Comparator<NeighborEdge>() {
				public int compare(NeighborEdge left, NeighborEdge right) {
				  //return	Calc.Compare(left.Edge.Length, right.Edge.Length);
					return	Calc.Compare(left.Edge, right.Edge);
				}
			});
              
                if (edges.size() > MaxNeighbors){
                	 edges.subList(MaxNeighbors, edges.size()).clear();
                }
                //Table[reference] = (NeighborEdge[])edges.toArray();
                //Print the edges
                NeighborEdge[] ne=new NeighborEdge[edges.size()];
                Table[reference] =  edges.toArray(ne);
                edges.clear();
            }
        }
    }