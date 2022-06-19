
namespace QServerSDK
{
    public class Bool3
    {
        public bool x;
        // Y component of the vector.
        public bool y;
        // Z component of the vector.
        public bool z;

        public static Bool3 unchanged => new Bool3(false, false, false);

        public bool Changed => x || y || z;
        public bool UnChanged => !x && !y && !z;

        public Bool3(bool x, bool y, bool z) { this.x = x; this.y = y; this.z = z; }

        public static bool operator ==(Bool3 lhs, Bool3 rhs)
        {
            if (lhs is null)
                return rhs is null;
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Bool3 lhs, Bool3 rhs)
        {
            if (lhs is null)
                return !(rhs is null);
            return !lhs.Equals(rhs);
        }

        public override bool Equals(object other)
        {
            if (!(other is Bool3)) return false;

            return Equals((Bool3)other);
        }

        public bool Equals(Bool3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }
        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }
    }
}
