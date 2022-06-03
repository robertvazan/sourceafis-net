// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Features
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
