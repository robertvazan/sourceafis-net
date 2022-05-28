// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

using SourceAFIS.Primitives;

namespace SourceAFIS.Features
{
    class MutableMinutia
    {
        public IntPoint Position;
        public double Direction;
        public MinutiaType Type;
        public MutableMinutia() { }
        public MutableMinutia(IntPoint position, double direction, MinutiaType type)
        {
            Position = position;
            Direction = direction;
            Type = type;
        }
    }
}
