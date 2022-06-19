using System;


namespace QServerSDK.Mapping
{
    public class QTransformMapper
    {
        const int pos_BL = 14;  // 16384  -->  
        const int rot_BL = 6;   //    64  -->  360/64 = 5.625° increment (0° -> 360°)
        const int sca_BL = 4;   //    16  -->  2/16 = 0.0125 scale increment   (0.5 -> 2.5 scale)
        const int hp_BL = 3;    //     8  -->  8 health points (0 -> 8 health)

        QBitPacker bp = new QBitPacker();

        public QTransform Deserialize(byte[] packet)
        {
            int bitOffset = 0;

            return Deserialize(packet, ref bitOffset);
        }

        public byte[] Serialize(QTransform snap)
        {
            var packet = new byte[10];
            int bitOffset = 0;

            Serialize(snap, packet, ref bitOffset);

            // trim unused bits/bytes
            int byteLength = (bitOffset - 1) / 8 + 1;
            byte[] finPacket = new byte[byteLength];
            Buffer.BlockCopy(packet, 0, finPacket, 0, byteLength);
            packet = finPacket;

            return packet;
        }

        public QTransform Deserialize(byte[] packet, ref int bitOffset)
        {
            var snap = new QTransform();
 
            // Position
            if (bp.UnpackValue(packet, ref bitOffset, pos_BL, out int posx))
            {
                snap.Pos.x = posx;
                snap.PosChanged.x = true;
            }
            if (bp.UnpackValue(packet, ref bitOffset, pos_BL, out int posy))
            {
                snap.Pos.y = posy;
                snap.PosChanged.y = true;
            }
            if (bp.UnpackValue(packet, ref bitOffset, pos_BL, out int posz))
            {
                snap.Pos.z = posz;
                snap.PosChanged.z = true;
            }

            // Rotation
            if (bp.UnpackValue(packet, ref bitOffset, rot_BL, out int rotx))
            {
                snap.Rot.x = rotx;
                snap.RotChanged.x = true;
            }
            if (bp.UnpackValue(packet, ref bitOffset, rot_BL, out int roty))
            {
                snap.Rot.y = roty;
                snap.RotChanged.y = true;
            }
            if (bp.UnpackValue(packet, ref bitOffset, rot_BL, out int rotz))
            {
                snap.Rot.z = rotz;
                snap.RotChanged.z = true;
            }

            // Scale
            if (bp.UnpackValue(packet, ref bitOffset, sca_BL, out int scale))
            {
                snap.Scale = scale;
                snap.ScaleChanged = true;
            }

            // Health
            if (bp.UnpackValue(packet, ref bitOffset, hp_BL, out int hp))
            {
                snap.Health = hp;
                snap.HealthChanged = true;
            }

            return snap;
        }

        public void Serialize(QTransform snap, byte[] packet, ref int bitOffset)
        {
            // Position
            bp.PackValue(packet, ref bitOffset, pos_BL, snap.Pos.x, snap.PosChanged.x);
            bp.PackValue(packet, ref bitOffset, pos_BL, snap.Pos.y, snap.PosChanged.y);
            bp.PackValue(packet, ref bitOffset, pos_BL, snap.Pos.z, snap.PosChanged.z);
            // Rotation
            bp.PackValue(packet, ref bitOffset, rot_BL, snap.Rot.x, snap.RotChanged.x);
            bp.PackValue(packet, ref bitOffset, rot_BL, snap.Rot.y, snap.RotChanged.y);
            bp.PackValue(packet, ref bitOffset, rot_BL, snap.Rot.z, snap.RotChanged.z);
            // Scale
            bp.PackValue(packet, ref bitOffset, sca_BL, snap.Scale, snap.ScaleChanged);
            // Health
            bp.PackValue(packet, ref bitOffset, hp_BL, snap.Health, snap.HealthChanged);

        }
    }
}
