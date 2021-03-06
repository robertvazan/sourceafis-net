// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System;

namespace SourceAFIS
{
    class ImmutableMinutia
    {
        public readonly IntPoint Position;
        public readonly double Direction;
        public readonly MinutiaType Type;
        public ImmutableMinutia(MutableMinutia mutable)
        {
            Position = mutable.Position;
            Direction = mutable.Direction;
            Type = mutable.Type;
        }
        public MutableMinutia Mutable()
        {
            return new MutableMinutia(Position, Direction, Type);
        }
    }
}
