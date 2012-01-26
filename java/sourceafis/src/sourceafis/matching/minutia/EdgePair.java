package sourceafis.matching.minutia;
 public class EdgePair
    {
        public MinutiaPair reference;
        public MinutiaPair neighbor;

        public EdgePair(MinutiaPair reference, MinutiaPair neighbor)
        {
            this.reference = reference;
            this.neighbor = neighbor;
        }
    }