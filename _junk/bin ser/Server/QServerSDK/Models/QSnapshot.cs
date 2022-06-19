using System;
using System.Collections.Generic;
using System.Text;

namespace QServerSDK
{
    public class QSnapshot : QState, IAckable
    {
        public bool Ack { get; set; }

        public QSnapshot(int nPlayers, bool init = true, string name = null)
            : base(nPlayers, init, name)
        {
        }

        public override bool Equals(QState other)
        {
            if (other is QSnapshot)
                return Ack == (other as QSnapshot).Ack && base.Equals(other);
            else
                return base.Equals(other);
        }

        public override string ToString()
        {
            return base.ToString() + (Ack ? " ✓" : "");
        }
    }
}
