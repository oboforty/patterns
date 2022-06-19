using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QServerSDK
{
    public class QCommand
    {
        public Vector3 DesiredMove { get; set; } = Vector3.zero;


        //public float Timestamp { get; set; }
        //public int Frame { get; set; }
        //public int WeaponID { get; set; }
        //public bool UpBtnHeld { get; set; }
        //public bool DownBtnHeld { get; set; }
        //public bool RightBtnHeld { get; set; }
        //public bool LeftBtnHeld { get; set; }


        public override bool Equals(object other)
        {
            if (!(other is QCommand)) return false;

            return Equals((QCommand)other);
        }

        public bool Equals(QCommand other)
        {
            return
                DesiredMove.Equals(other.DesiredMove);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("x=" + DesiredMove.x + ", ");
            sb.Append("y=" + DesiredMove.y + ", ");
            sb.Append("z=" + DesiredMove.z + ", ");

            return sb.ToString();
        }
    }
}
