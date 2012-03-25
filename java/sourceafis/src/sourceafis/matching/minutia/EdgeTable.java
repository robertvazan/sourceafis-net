package sourceafis.matching.minutia;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

import sourceafis.general.Calc;
import sourceafis.general.DetailLogger;
import sourceafis.general.Point;
import sourceafis.meta.Nested;
import sourceafis.meta.Parameter;
import sourceafis.templates.Template;
 
 public  final class EdgeTable
    {
        @Nested
        public EdgeConstructor EdgeConstructor = new EdgeConstructor();

        @Parameter(lower=30,upper=1500)
        public int MaxDistance = 490;
        @Parameter(lower=2,upper=100)
        public int MaxNeighbors = 9;

        public DetailLogger.Hook logger = DetailLogger.off;

        public NeighborEdge[][] Table;
        public void reset(Template template)
        {
            synchronized (template){
                Table = (NeighborEdge[][])template.matcherCache;
            }
            if (Table == null)
            {
                buildTable(template);
                synchronized (template) {
                	 template.matcherCache = Table;
				}  
                   
            }

            logger.log(this);
        }
        public void buildTable(Template template)
        {
        	
            Table = new NeighborEdge[template.minutiae.length][];
            List<NeighborEdge> edges = new ArrayList<NeighborEdge>();
            int[] allSqDistances = new int[template.minutiae.length];

            for (int reference = 0; reference < Table.length; ++reference)
            {
            	//PointS to Point
                Point referencePosition = template.minutiae[reference].Position;
                int sqMaxDistance = Calc.Sq(MaxDistance);
                if (template.minutiae.length - 1 > MaxNeighbors)
                {
                    for (int neighbor = 0; neighbor < template.minutiae.length; ++neighbor)
                        allSqDistances[neighbor] = Calc.DistanceSq(referencePosition, template.minutiae[neighbor].Position);
                    //Array.Sort(allSqDistances);
                    Arrays.sort(allSqDistances);
                    sqMaxDistance = allSqDistances[MaxNeighbors];
                }
                
                for (int neighbor = 0; neighbor < template.minutiae.length; ++neighbor)
                {
                	if (neighbor != reference && Calc.DistanceSq(referencePosition, template.minutiae[neighbor].Position)
                        <= sqMaxDistance)
                    {
                        NeighborEdge record = new NeighborEdge();
                        record.edge = EdgeConstructor.Construct(template, reference, neighbor);
                        record.neighbor = neighbor;
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
					//return Calc.ChainCompare(
					//		Calc.Compare(left.Edge.length, right.Edge.length),
					//		Calc.Compare(left.Neighbor, right.Neighbor));
					int result = Calc.Compare(left.edge.length, right.edge.length);
		              if (result != 0)
		                  return result;
		              return Calc.Compare(left.neighbor, right.neighbor);
				}
			   });
                //Remove Range (MaxNeighbors, edges.Count - MaxNeighbors)
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