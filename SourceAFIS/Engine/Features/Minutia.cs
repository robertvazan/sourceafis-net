// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Features
{
    readonly struct Minutia
    {
        public readonly IntPoint Position;
        public readonly double Direction;
        public readonly MinutiaType Type;
        public Minutia(IntPoint position, double direction, MinutiaType type)
        {
            Position = position;
            Direction = direction;
            Type = type;
        }
    }
}
