// Part of SourceAFIS for .NET: https://sourceafis.machinezoo.com/net
using System.Runtime.InteropServices;
using SourceAFIS.Engine.Primitives;

namespace SourceAFIS.Engine.Features
{
    // Explicitly request sequential layout for predictable memory usage.
    // Do not pack the struct, so that the float field remains aligned in minutia arrays.
    [StructLayout(LayoutKind.Sequential)]
    readonly struct Minutia
    {
        // Mind the field order. Let the point and float take aligned positions.
        public readonly ShortPoint Position;
        public readonly float Direction;
        public readonly MinutiaType Type;

        // Struct alignment will force padding after type field.
        public const int Memory = ShortPoint.Memory + sizeof(float) + sizeof(float);

        public Minutia(ShortPoint position, float direction, MinutiaType type)
        {
            Position = position;
            Direction = direction;
            Type = type;
        }
    }
}
