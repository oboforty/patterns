using System;
using System.Collections.Generic;
using System.Text;

namespace QServerSDK
{
    public class QState
    {
        public QTransform[] players;

        public string Name {get; private set ;}

        public QState(int nPlayers, bool init = true, string name = null)
        {
            players = new QTransform[nPlayers];
            Name = name;

            if (init)
            {
                for (int i = 0; i < nPlayers; i++)
                    players[i] = new QTransform(true);
            }
        }

        public static bool operator ==(QState lhs, QState rhs)
        {
            if (lhs is null)
                return rhs is null;
            return lhs.Equals(rhs);
        }

        public static bool operator !=(QState lhs, QState rhs)
        {
            if (lhs is null)
                return !(rhs is null);
            return !lhs.Equals(rhs);
        }

        public override bool Equals(object other)
        {
            if (!(other is QState)) return false;

            return Equals((QState)other);
        }

        public virtual bool Equals(QState other)
        {
            if (players.Length != other.players.Length)
                return false;

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] != other.players[i])
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Name != null)
                sb.Append(Name);

            if (players.Length == 2)
            {
                sb.Append("[");
                sb.Append(players[0].ToString());
                sb.Append("]");
                sb.Append("  vs  ");
                sb.Append("[");
                sb.Append(players[1].ToString());
                sb.Append("]");
            }
            else
            {
                // @todo: @later:
                throw new NotImplementedException();
            }

            return sb.ToString();
        }

        public override int GetHashCode()
        {
            // @todo: not implemented
            throw new NotImplementedException();
        }
    }
}
