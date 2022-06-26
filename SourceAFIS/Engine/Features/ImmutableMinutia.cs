// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net

using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Features
{
    record ImmutableMinutia(IntPoint Position, double Direction, MinutiaType Type)
    {
        public ImmutableMinutia(MutableMinutia mutable) : this(mutable.Position, mutable.Direction, mutable.Type) { }
        public MutableMinutia Mutable() => new MutableMinutia(Position, Direction, Type);
    }
}
