using System;
using System.Globalization;

namespace QServerSDK
{
    public class Vector3 : IEquatable<Vector3>
    {
        // X component of the vector.
        public int x;
        // Y component of the vector.
        public int y;
        // Z component of the vector.
        public int z;

        public Vector3 Clamp(int minV, int maxV)
        {
            return new Vector3(
                x < minV ? 0 : (x > maxV ? maxV : x),
                y < minV ? 0 : (y > maxV ? maxV : y),
                z < minV ? 0 : (z > maxV ? maxV : z)
            );
        }

        public static Vector3 zero => new Vector3(0, 0, 0);
        public static Vector3 one => new Vector3(1, 1, 1);
        public static Vector3 up => new Vector3(0, 1, 0);
        public static Vector3 down => new Vector3(0, -1, 0);
        public static Vector3 left => new Vector3(-1, 0, 0);
        public static Vector3 right => new Vector3(1, 0, 0);
        public static Vector3 forward => new Vector3(0, 0, 1);

        public static Vector3 back => new Vector3(0, 0, -1);
        //public static Vector3 positiveInfinity = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        //public static Vector3 negativeInfinity => new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        public Vector3(int x, int y, int z) { this.x = x; this.y = y; this.z = z; }

        public Vector3(Vector3 that) { this.x = that.x; this.y = that.y; this.z = that.z; }

        public Bool3 Diff3(Vector3 pos)
        {
            return new Bool3(x != pos.x, y != pos.y, z != pos.z);
        }
        public static Vector3 operator +(Vector3 a, Vector3 b) { return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z); }
        // Subtracts one vector from another.
        public static Vector3 operator -(Vector3 a, Vector3 b) { return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z); }
        // Negates a vector.
        public static Vector3 operator -(Vector3 a) { return new Vector3(-a.x, -a.y, -a.z); }
        // Multiplies a vector by a number.
        public static Vector3 operator *(Vector3 a, int d) { return new Vector3(a.x * d, a.y * d, a.z * d); }
        // Multiplies a vector by a number.
        public static Vector3 operator *(int d, Vector3 a) { return new Vector3(a.x * d, a.y * d, a.z * d); }
        // Divides a vector by a number.
        public static Vector3 operator /(Vector3 a, int d) { return new Vector3(a.x / d, a.y / d, a.z / d); }

        // Returns true if the vectors are equal.
        public static bool operator ==(Vector3 lhs, Vector3 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }

        // Returns true if vectors are different.
        public static bool operator !=(Vector3 lhs, Vector3 rhs)
        {
            // Returns true in the presence of NaN values.
            return !(lhs == rhs);
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector3)) return false;

            return Equals((Vector3)other);
        }

        public bool Equals(Vector3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }

        public override string ToString()
        {
            return ToString(null, CultureInfo.InvariantCulture.NumberFormat);
        }

        public string ToString(string format)
        {
            return ToString(format, CultureInfo.InvariantCulture.NumberFormat);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "F1";
            return $"({x.ToString(format, formatProvider)}, {y.ToString(format, formatProvider)}, {z.ToString(format, formatProvider)})";
        }

    }
}
