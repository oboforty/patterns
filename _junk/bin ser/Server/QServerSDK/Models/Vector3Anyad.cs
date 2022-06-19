using System;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace QServerSDK
{
    /*
    [StructLayout(LayoutKind.Sequential)]
    public partial class Vector3Anyad
    {
        public const float kEpsilon = 0.00001F;
        public const float kEpsilonNormalSqrt = 1e-15F;

        // X component of the vector.
        public float x;
        // Y component of the vector.
        public float y;
        // Z component of the vector.
        public float z;

        // Linearly interpolates between two vectors.
        public static Vector3Anyad Lerp(Vector3Anyad a, Vector3Anyad b, float t)
        {
            return new Vector3Anyad(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t
            );
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        public static Vector3Anyad LerpUnclamped(Vector3Anyad a, Vector3Anyad b, float t)
        {
            return new Vector3Anyad(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t
            );
        }

        // Moves a point /current/ in a straight line towards a /target/ point.
        public static Vector3Anyad MoveTowards(Vector3Anyad current, Vector3Anyad target, float maxDistanceDelta)
        {
            // avoid vector ops because current scripting backends are terrible at inlining
            float toVector_x = target.x - current.x;
            float toVector_y = target.y - current.y;
            float toVector_z = target.z - current.z;

            float sqdist = toVector_x * toVector_x + toVector_y * toVector_y + toVector_z * toVector_z;

            if (sqdist == 0 || (maxDistanceDelta >= 0 && sqdist <= maxDistanceDelta * maxDistanceDelta))
                return target;
            var dist = (float)Math.Sqrt(sqdist);

            return new Vector3Anyad(current.x + toVector_x / dist * maxDistanceDelta,
                current.y + toVector_y / dist * maxDistanceDelta,
                current.z + toVector_z / dist * maxDistanceDelta);
        }

        // Gradually changes a vector towards a desired goal over time.
        public static Vector3Anyad SmoothDamp(Vector3Anyad current, Vector3Anyad target, ref Vector3Anyad currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            float output_x = 0f;
            float output_y = 0f;
            float output_z = 0f;

            // Based on Game Programming Gems 4 Chapter 1.10
            smoothTime = 0.0001F > smoothTime ? 0.0001f : smoothTime;
            float omega = 2F / smoothTime;

            float x = omega * deltaTime;
            float exp = 1F / (1F + x + 0.48F * x * x + 0.235F * x * x * x);

            float change_x = current.x - target.x;
            float change_y = current.y - target.y;
            float change_z = current.z - target.z;
            Vector3Anyad originalTo = target;

            // Clamp maximum speed
            float maxChange = maxSpeed * smoothTime;

            float maxChangeSq = maxChange * maxChange;
            float sqrmag = change_x * change_x + change_y * change_y + change_z * change_z;
            if (sqrmag > maxChangeSq)
            {
                var mag = (float)Math.Sqrt(sqrmag);
                change_x = change_x / mag * maxChange;
                change_y = change_y / mag * maxChange;
                change_z = change_z / mag * maxChange;
            }

            target.x = current.x - change_x;
            target.y = current.y - change_y;
            target.z = current.z - change_z;

            float temp_x = (currentVelocity.x + omega * change_x) * deltaTime;
            float temp_y = (currentVelocity.y + omega * change_y) * deltaTime;
            float temp_z = (currentVelocity.z + omega * change_z) * deltaTime;

            currentVelocity.x = (currentVelocity.x - omega * temp_x) * exp;
            currentVelocity.y = (currentVelocity.y - omega * temp_y) * exp;
            currentVelocity.z = (currentVelocity.z - omega * temp_z) * exp;

            output_x = target.x + (change_x + temp_x) * exp;
            output_y = target.y + (change_y + temp_y) * exp;
            output_z = target.z + (change_z + temp_z) * exp;

            // Prevent overshooting
            float origMinusCurrent_x = originalTo.x - current.x;
            float origMinusCurrent_y = originalTo.y - current.y;
            float origMinusCurrent_z = originalTo.z - current.z;
            float outMinusOrig_x = output_x - originalTo.x;
            float outMinusOrig_y = output_y - originalTo.y;
            float outMinusOrig_z = output_z - originalTo.z;

            if (origMinusCurrent_x * outMinusOrig_x + origMinusCurrent_y * outMinusOrig_y + origMinusCurrent_z * outMinusOrig_z > 0)
            {
                output_x = originalTo.x;
                output_y = originalTo.y;
                output_z = originalTo.z;

                currentVelocity.x = (output_x - originalTo.x) / deltaTime;
                currentVelocity.y = (output_y - originalTo.y) / deltaTime;
                currentVelocity.z = (output_z - originalTo.z) / deltaTime;
            }

            return new Vector3Anyad(output_x, output_y, output_z);
        }

        // Access the x, y, z components using [0], [1], [2] respectively.
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }

            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
        }

        // Creates a new vector with given x, y, z components.
        public Vector3Anyad(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
        // Creates a new vector with given x, y components and sets /z/ to zero.
        public Vector3Anyad(float x, float y) { this.x = x; this.y = y; z = 0F; }

        // Set x, y and z components of an existing Vector3.
        public void Set(float newX, float newY, float newZ) { x = newX; y = newY; z = newZ; }

        // Multiplies two vectors component-wise.
        public static Vector3Anyad Scale(Vector3Anyad a, Vector3Anyad b) { return new Vector3Anyad(a.x * b.x, a.y * b.y, a.z * b.z); }

        // Multiplies every component of this vector by the same component of /scale/.
        public void Scale(Vector3Anyad scale) { x *= scale.x; y *= scale.y; z *= scale.z; }

        // Cross Product of two vectors.
        public static Vector3Anyad Cross(Vector3Anyad lhs, Vector3Anyad rhs)
        {
            return new Vector3Anyad(
                lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.x * rhs.y - lhs.y * rhs.x);
        }

        // used to allow Vector3s to be used as keys in hash tables
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }

        // also required for being able to use Vector3s as keys in hash tables


        // Reflects a vector off the plane defined by a normal.
        public static Vector3Anyad Reflect(Vector3Anyad inDirection, Vector3Anyad inNormal)
        {
            float factor = -2F * Dot(inNormal, inDirection);
            return new Vector3Anyad(factor * inNormal.x + inDirection.x,
                factor * inNormal.y + inDirection.y,
                factor * inNormal.z + inDirection.z);
        }

        // *undoc* --- we have normalized property now
        public static Vector3Anyad Normalize(Vector3Anyad value)
        {
            float mag = Magnitude(value);
            if (mag > kEpsilon)
                return value / mag;
            else
                return zero;
        }

        // Makes this vector have a ::ref::magnitude of 1.
        public void Normalize()
        {
            float mag = Magnitude(this);

            if (mag > kEpsilon)
            {
                x /= mag;
                y /= mag;
                z /= mag;
            }
            else
            {
                x = y = z = 0;
            }
        }

        // Returns this vector with a ::ref::magnitude of 1 (RO).
        public Vector3Anyad normalized { get { return Vector3Anyad.Normalize(this); } }

        // Dot Product of two vectors.
        public static float Dot(Vector3Anyad lhs, Vector3Anyad rhs) { return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z; }

        // Projects a vector onto another vector.
        public static Vector3Anyad Project(Vector3Anyad vector, Vector3Anyad onNormal)
        {
            float sqrMag = Dot(onNormal, onNormal);
            if (sqrMag < kEpsilon)
                return zero;
            else
            {
                var dot = Dot(vector, onNormal);
                return new Vector3Anyad(onNormal.x * dot / sqrMag,
                    onNormal.y * dot / sqrMag,
                    onNormal.z * dot / sqrMag);
            }
        }

        // Projects a vector onto a plane defined by a normal orthogonal to the plane.
        public static Vector3Anyad ProjectOnPlane(Vector3Anyad vector, Vector3Anyad planeNormal)
        {
            float sqrMag = Dot(planeNormal, planeNormal);
            if (sqrMag < kEpsilon)
                return vector;
            else
            {
                var dot = Dot(vector, planeNormal);
                return new Vector3Anyad(vector.x - planeNormal.x * dot / sqrMag,
                    vector.y - planeNormal.y * dot / sqrMag,
                    vector.z - planeNormal.z * dot / sqrMag);
            }
        }

        // Returns the angle in radians between /from/ and /to/. This is always the smallest
        public static float Angle(Vector3Anyad from, Vector3Anyad to)
        {
            // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
            float denominator = (float)Math.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            if (denominator < kEpsilonNormalSqrt)
                return 0F;

            float dot = Clamp(Dot(from, to) / denominator, -1F, 1F);
            return ((float)Math.Acos(dot));
        }

        public static float Clamp(float v, float vmin, float vmax)
        {
            if (v < vmin) return vmin;
            else if (v > vmax) return vmax;
            return v;
        }

        // The smaller of the two possible angles between the two vectors is returned, therefore the result will never be greater than 180 degrees or smaller than -180 degrees.
        // If you imagine the from and to vectors as lines on a piece of paper, both originating from the same point, then the /axis/ vector would point up out of the paper.
        // The measured angle between the two vectors would be positive in a clockwise direction and negative in an anti-clockwise direction.
        public static float SignedAngle(Vector3Anyad from, Vector3Anyad to, Vector3Anyad axis)
        {
            float unsignedAngle = Angle(from, to);

            float cross_x = from.y * to.z - from.z * to.y;
            float cross_y = from.z * to.x - from.x * to.z;
            float cross_z = from.x * to.y - from.y * to.x;
            float sign = Sign(axis.x * cross_x + axis.y * cross_y + axis.z * cross_z);
            return unsignedAngle * sign;
        }

        public static float Sign(float v)
        {
            if (v > 0) return 1;
            else if (v < 0) return -1;
            else return 0;
        }

        // Returns the distance between /a/ and /b/.
        public static float Distance(Vector3Anyad a, Vector3Anyad b)
        {
            float diff_x = a.x - b.x;
            float diff_y = a.y - b.y;
            float diff_z = a.z - b.z;
            return (float)Math.Sqrt(diff_x * diff_x + diff_y * diff_y + diff_z * diff_z);
        }

        // Returns a copy of /vector/ with its magnitude clamped to /maxLength/.
        public static Vector3Anyad ClampMagnitude(Vector3Anyad vector, float maxLength)
        {
            float sqrmag = vector.sqrMagnitude;
            if (sqrmag > maxLength * maxLength)
            {
                float mag = (float)Math.Sqrt(sqrmag);
                //these intermediate variables force the intermediate result to be
                //of float precision. without this, the intermediate result can be of higher
                //precision, which changes behavior.
                float normalized_x = vector.x / mag;
                float normalized_y = vector.y / mag;
                float normalized_z = vector.z / mag;
                return new Vector3Anyad(normalized_x * maxLength,
                    normalized_y * maxLength,
                    normalized_z * maxLength);
            }
            return vector;
        }

        // *undoc* --- there's a property now
        public static float Magnitude(Vector3Anyad vector) { return (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z); }

        // Returns the length of this vector (RO).
        public float magnitude { get { return (float)Math.Sqrt(x * x + y * y + z * z); } }

        // *undoc* --- there's a property now
        public static float SqrMagnitude(Vector3Anyad vector) { return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z; }

        // Returns the squared length of this vector (RO).
        public float sqrMagnitude { get { return x * x + y * y + z * z; } }

        // Returns a vector that is made from the smallest components of two vectors.
        //public static Vector3 Min(Vector3 lhs, Vector3 rhs)
        //{
        //    return new Vector3(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z));
        //}

        // Returns a vector that is made from the largest components of two vectors.
        //public static Vector3 Max(Vector3 lhs, Vector3 rhs)
        //{
        //    return new Vector3(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z));
        //}

        static readonly Vector3Anyad zeroVector = new Vector3Anyad(0F, 0F, 0F);
        static readonly Vector3Anyad oneVector = new Vector3Anyad(1F, 1F, 1F);
        static readonly Vector3Anyad upVector = new Vector3Anyad(0F, 1F, 0F);
        static readonly Vector3Anyad downVector = new Vector3Anyad(0F, -1F, 0F);
        static readonly Vector3Anyad leftVector = new Vector3Anyad(-1F, 0F, 0F);
        static readonly Vector3Anyad rightVector = new Vector3Anyad(1F, 0F, 0F);
        static readonly Vector3Anyad forwardVector = new Vector3Anyad(0F, 0F, 1F);
        static readonly Vector3Anyad backVector = new Vector3Anyad(0F, 0F, -1F);
        static readonly Vector3Anyad positiveInfinityVector = new Vector3Anyad(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        static readonly Vector3Anyad negativeInfinityVector = new Vector3Anyad(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        // Shorthand for writing @@Vector3(0, 0, 0)@@
        public static Vector3Anyad zero { get { return zeroVector; } }
        // Shorthand for writing @@Vector3(1, 1, 1)@@
        public static Vector3Anyad one { get { return oneVector; } }
        // Shorthand for writing @@Vector3(0, 0, 1)@@
        public static Vector3Anyad forward { get { return forwardVector; } }
        public static Vector3Anyad back { get { return backVector; } }
        // Shorthand for writing @@Vector3(0, 1, 0)@@
        public static Vector3Anyad up { get { return upVector; } }
        public static Vector3Anyad down { get { return downVector; } }
        public static Vector3Anyad left { get { return leftVector; } }
        // Shorthand for writing @@Vector3(1, 0, 0)@@
        public static Vector3Anyad right { get { return rightVector; } }
        // Shorthand for writing @@Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity)@@
        public static Vector3Anyad positiveInfinity { get { return positiveInfinityVector; } }
        // Shorthand for writing @@Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity)@@
        public static Vector3Anyad negativeInfinity { get { return negativeInfinityVector; } }

        // Adds two vectors.



    }
   */
}
