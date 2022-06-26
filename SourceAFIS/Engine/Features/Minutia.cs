// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Features
{
    readonly struct Minutia
    {
        public readonly ShortPoint Position;
        public readonly float Direction;
        public readonly MinutiaType Type;
        public Minutia(ShortPoint position, float direction, MinutiaType type)
        {
            Position = position;
            Direction = direction;
            Type = type;
        }
    }
}
