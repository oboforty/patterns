using System;
using System.Collections.Generic;
using System.Text;

namespace QServerSDK.Mapping
{
    public class QCommandMapper
    {
        /*
         
         This supports negative -1, 1 and 0 values for DesiredMove 
         */
        const int move_BL = 2;

        QBitPacker bp;
        public int NPlayers { get; }
        int defaultBitOffset;
        int defaultPacketAlloc;

        public QCommandMapper(int nplayers = 2, int defaultBitOffset = 0, int defaultPacketAlloc = 2)
        {
            bp = new QBitPacker(allowNegatives: true);

            // @TODO: better support for negative values? maybe skip Vectors entirely?

            NPlayers = nplayers;
            this.defaultBitOffset = defaultBitOffset;
            this.defaultPacketAlloc = defaultPacketAlloc;
        }


        public QCommand Deserialize(byte[] packet)
        {
            int bitOffset = defaultBitOffset;

            var cmd = new QCommand();

            // Desired move
            if (bp.UnpackValue(packet, ref bitOffset, move_BL, out int movex))
                cmd.DesiredMove.x = movex-1;
            if (bp.UnpackValue(packet, ref bitOffset, move_BL, out int movey))
                cmd.DesiredMove.y = movey-1;
            if (bp.UnpackValue(packet, ref bitOffset, move_BL, out int movez))
                cmd.DesiredMove.z = movez-1;

            return cmd;
        }

        public byte[] Serialize(QCommand cmd, out int bitOffset)
        {
            var packet = new byte[defaultPacketAlloc];
            bitOffset = defaultBitOffset;

            bp.PackValue(packet, ref bitOffset, move_BL, cmd.DesiredMove.x+1, cmd.DesiredMove.x != 0);
            bp.PackValue(packet, ref bitOffset, move_BL, cmd.DesiredMove.y+1, cmd.DesiredMove.y != 0);
            bp.PackValue(packet, ref bitOffset, move_BL, cmd.DesiredMove.z+1, cmd.DesiredMove.z != 0);


            // trim unused bits/bytes
            int byteLength = (bitOffset - 1) / 8 + 1;
            byte[] finPacket = new byte[byteLength];
            Buffer.BlockCopy(packet, 0, finPacket, 0, byteLength);
            packet = finPacket;

            return packet;
        }
    }
}
