using System;


namespace QServerSDK.Mapping
{
    public class QBitPacker
    {
        bool allowNegatives;

        public QBitPacker(bool allowNegatives = false) 
        {
            this.allowNegatives = allowNegatives;
        }

        public bool UnpackValue(byte[] packet, ref int bitOffset, int bitLength, out Int32 val)
        {
            val = 0;
            int byteOffset = bitOffset / 8;
            int interByteBitOffset = bitOffset % 8;

            if (byteOffset >= packet.Length)
                throw new Exception($"Ran out of bytes ({byteOffset} bytes, {bitLength} bits, {packet.Length} bytes allocated). Please allow dynamic byte allocation option.");

            // if VAL_SET [VS] bit is not set => no value to read
            if ((packet[byteOffset] & (1 << interByteBitOffset)) == 0)
            {
                bitOffset++;
                return false;
            }

            // shift VS bit
            bitOffset++;
            interByteBitOffset = bitOffset % 8;
            byteOffset = bitOffset / 8;

            // required bytes to allocate [bitLength] bits starting from current offset position
            int byteLength = (interByteBitOffset + bitLength - 1) / 8 + 1;
            //interByteBitOffset = bitLength % 8;

            if (byteLength <= 2)
            {
                if (byteOffset + 2 > packet.Length)
                {
                    // pad with trailing zeroes
                    byte[] finPacket = new byte[byteOffset + 2];
                    Buffer.BlockCopy(packet, 0, finPacket, 0, packet.Length);
                    packet = finPacket;
                }

                // 1 byte value -> use 2 bytes for block copying
                Int16 mask = Convert.ToInt16((1 << bitLength) - 1);
                val = BitConverter.ToInt16(packet, byteOffset);
                val = (val >> interByteBitOffset) & mask;
            }
            else
            {
                if (byteOffset + 4 > packet.Length)
                {
                    // pad with trailing zeroes
                    byte[] finPacket = new byte[byteOffset + 4];
                    Buffer.BlockCopy(packet, 0, finPacket, 0, packet.Length);
                    packet = finPacket;
                }

                // 2-3 bytes value -> use 4 bytes for block copying
                Int32 mask = Convert.ToInt32((1 << bitLength) - 1);
                val = BitConverter.ToInt32(packet, byteOffset);
                val = (val >> interByteBitOffset) & mask;
            }

            // finally shift bitOffset with the length of the packet bits
            bitOffset += bitLength;

            return true;
        }

        public void PackValue(byte[] packet, ref int bitOffset, int bitLength, Int32 val, bool bitSet)
        {
            if (!bitSet)
            {
                PackValueBitNotSet(packet, ref bitOffset);
            }
            else
            {
                PackValue(packet, ref bitOffset, bitLength, val);
            }
        }

        public void PackValueBitNotSet(byte[] packet, ref int bitOffset)
        {
            bitOffset++;
        }

        public void PackValue(byte[] packet, ref int bitOffset, int bitLength, Int32 val)
        {
            // check value range
            if (val < 0)
                throw new Exception("out_of_bounds");
            else if (val > Math.Pow(2, bitLength)-1)
                throw new Exception("out_of_bounds");

            // initial byteOffset:
            int b = bitOffset / 8;
            int endOffset = bitOffset + bitLength + 1;
            int mask;

            // 1.a BitSet [BS] added with a mask at start pos, then move to next bit
            int interByteBitoffset = bitOffset % 8;
            packet[b] |= (byte)(1 << interByteBitoffset);
            bitOffset++;
            //byteOffset = bitOffset / 8;
            interByteBitoffset = bitOffset % 8;

            if (interByteBitoffset != 0)
            {
                // 1.b set initial value at remainder bits
                mask = (0xFF << interByteBitoffset);
                packet[b] |= (byte)((val << interByteBitoffset) & mask);

                // we step with number of [iBb offset], however this can be a higher number than the total length
                int step = Math.Min(8 - interByteBitoffset, bitLength);
                bitOffset += step;
                val >>= step;
            }
            b++;

            if (endOffset == bitOffset)
                return;
            else if (endOffset < bitOffset)
                throw new Exception("Unexpected parsing state!");

            // 2. intermediary bytes: mask with 0xFF
            var nrIMBytes = (endOffset - bitOffset) / 8;
            if (nrIMBytes > 0)
            {
                mask = 0xFF;
                // loop on bytes and mask with 0xFF
                for (int i = 0; i < nrIMBytes; i++)
                {
                    packet[b + i] |= (byte)(val & mask);
                    bitOffset += 8;
                    val >>= 8;
                }
                b += nrIMBytes;
            }

            if (endOffset == bitOffset)
                return;
            else if (endOffset < bitOffset)
                throw new Exception("Unexpected parsing state!");

            // 3. append finishing bits
            var remainder = endOffset - bitOffset;
            if (remainder > 0)
            {
                mask = ~(0xFF << remainder);
                packet[b] |= (byte)(val & mask);
                bitOffset += remainder;
                //val >>= remainder;
            }
        }

        //public static int BytesToStore(int bits)
        //{
        //    return bits / 8 + 1;
        //}
    }
}
