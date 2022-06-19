using System;
using System.Text;

namespace QServerSDK
{
    public class QTransform 
    {

        public Vector3 Pos { get; set; }
        public Bool3 PosChanged { get; set; }

        public Vector3 Rot { get; set; }
        public Bool3 RotChanged { get; set; }

        public int Scale { get; set; }
        public bool ScaleChanged { get; set; }

        public int Health { get; set; }
        public bool HealthChanged { get; set; }

        public QTransform(bool init = true) 
        {
            if (init)
            {
                Pos = Vector3.zero;
                PosChanged = Bool3.unchanged;
                Rot = Vector3.zero;
                RotChanged = Bool3.unchanged;
            }
        }

        public void CopyFrom(QTransform state, bool copyStateChanged = false)
        {
            Pos = new Vector3(state.Pos);
            Rot = new Vector3(state.Rot);
            Scale = state.Scale;
            Health = state.Health;

            if (copyStateChanged)
                throw new NotImplementedException();
        }

        public static bool operator ==(QTransform lhs, QTransform rhs)
        {
            if (lhs is null)
                return rhs is null;
            return lhs.Equals(rhs);
        }

        public static bool operator !=(QTransform lhs, QTransform rhs)
        {
            if (lhs is null)
                return !(rhs is null);
            return !lhs.Equals(rhs);
        }

        public override bool Equals(object other)
        {
            if (!(other is QTransform)) return false;

            return Equals((QTransform)other);
        }

        public bool Equals(QTransform other)
        {
            return
                Pos.Equals(other.Pos) && PosChanged.Equals(other.PosChanged) &&
                Rot.Equals(other.Rot) && RotChanged.Equals(other.RotChanged) &&
                Scale.Equals(other.Scale) && ScaleChanged.Equals(other.ScaleChanged) &&
                Health.Equals(other.Health) && HealthChanged.Equals(other.HealthChanged);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (PosChanged.x) sb.Append("x=" + Pos.x+", ");
            if (PosChanged.y) sb.Append("y=" + Pos.y+", ");
            if (PosChanged.z) sb.Append("z=" + Pos.z+", ");

            if (RotChanged.x) sb.Append("rx=" + Rot.x+", ");
            if (RotChanged.y) sb.Append("ry=" + Rot.y+", ");
            if (RotChanged.z) sb.Append("rz=" + Rot.z+", ");

            if (ScaleChanged) sb.Append("s=" + Scale+", ");

            if (HealthChanged) sb.Append("hp=" + Health);

            return sb.ToString();
        }
    }
}
