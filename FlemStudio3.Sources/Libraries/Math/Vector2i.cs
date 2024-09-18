using System.Numerics;
using System.Runtime.CompilerServices;

namespace FlemStudio.Math
{
    public partial struct Vector2i : IEquatable<Vector2i>
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Vector2i(int value) : this(value, value)
        {
        }


        public Vector2i(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector2i(Vector2i other)
        {
            X = other.X;
            Y = other.Y;
        }
        public Vector2i(Vector2 other)
        {
            X = (int)other.X;
            Y = (int)other.Y;
        }




        public static Vector2i Zero
        {
            get => default;
        }

        public static Vector2i One
        {
            get => new Vector2i(1);
        }

        public static Vector2i UnitX
        {
            get => new Vector2i(1, 0);
        }

        public static Vector2i UnitY
        {
            get => new Vector2i(0, 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i operator +(Vector2i left, Vector2i right)
        {
            return new Vector2i(
                left.X + right.X,
                left.Y + right.Y
            );
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i operator /(Vector2i value1, int value2)
        {
            return new Vector2i(value1.X / value2, value1.Y / value2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector2i left, Vector2i right)
        {
            return left.X == right.X
                && left.Y == right.Y;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector2i left, Vector2i right)
        {
            return !(left == right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i operator *(Vector2i left, Vector2i right)
        {
            return new Vector2i(
                left.X * right.X,
                left.Y * right.Y
            );
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i operator *(Vector2i left, int right)
        {
            return new Vector2i(
                left.X * right,
                left.Y * right
            );
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i operator *(int left, Vector2i right)
        {
            return right * left;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i operator -(Vector2i left, Vector2i right)
        {
            return new Vector2i(
                left.X - right.X,
                left.Y - right.Y
            );
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i operator -(Vector2i value)
        {
            return Zero - value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i Abs(Vector2i value)
        {
            return new Vector2i(
                value.X > 0 ? value.X : -value.X,
                value.Y > 0 ? value.Y : -value.Y
            );
        }

        public bool Equals(Vector2i other)
        {
            return this == other;
        }

        public override string ToString()
        {
            return "{x: " + X + ", y: " + Y + "}";
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }
    }
}
