using System;
using System.Collections.Generic;
using System.Text;

namespace QServerSDK.Mapping
{
    public class QSnapshotMapper
    {
        QTransformMapper tm = new QTransformMapper();

        public int NPlayers { get; }
        int defaultBitOffset;
        int defaultPacketAlloc;

        public QSnapshotMapper(int nplayers = 2, int defaultBitOffset = 0, int defaultPacketAlloc = 20)
        {
            NPlayers = nplayers;
            this.defaultBitOffset = defaultBitOffset;
            this.defaultPacketAlloc = defaultPacketAlloc;
        }

        public QSnapshot Deserialize(byte[] packet)
        {
            int bitOffset = defaultBitOffset;

            var snap = new QSnapshot(NPlayers, init: false);

            // deserialize players
            for (var p = 0; p < NPlayers; p++)
            {
                var trans1 = tm.Deserialize(packet, ref bitOffset);
                snap.players[p] = trans1;
            }

            // @todo: @later deserialize projectiles

            return snap;
        }

        public byte[] Serialize(QSnapshot snap, out int bitOffset)
        {
            var packet = new byte[defaultPacketAlloc];
            bitOffset = defaultBitOffset;

            // serialize players
            for (var p = 0; p < NPlayers; p++)
            {
                tm.Serialize(snap.players[p], packet, ref bitOffset);
            }

            // @todo: @later serialize projectiles


            // trim unused bits/bytes
            int byteLength = (bitOffset - 1) / 8 + 1;
            byte[] finPacket = new byte[byteLength];
            Buffer.BlockCopy(packet, 0, finPacket, 0, byteLength);
            packet = finPacket;

            return packet;
        }
    }
}
