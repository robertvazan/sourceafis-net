// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

namespace SourceAFIS
{
    class MutableMinutia
    {
        public IntPoint Position;
        public double Direction;
        public MinutiaType Type;
        public MutableMinutia()
        {
        }
        public MutableMinutia(IntPoint position, double direction, MinutiaType type)
        {
            Position = position;
            Direction = direction;
            Type = type;
        }
    }
}
