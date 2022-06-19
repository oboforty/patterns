using System;
using System.Collections.Generic;


namespace QServerSDK.ServerHandler
{
    public class QPlayerInfo<T>
    {
        public QPlayerInfo(int i)
        {
            this.playerId = i;
        }

        public SlidingList<QSnapshot> snapshots { get; set; }

        public bool IsOnline => Connected && ConnInfo != null;

        public bool Connected { get; set; }
        public T ConnInfo { get; set; }
        public int ConnHash { get; set; } = -1;
        public int playerId { get; private set; }
        public bool IsSpectator { get; set; }
    }
}
