using NUnit.Framework;
using System;
using System.Collections;
using System.Text;


namespace QServerSDK.Mapping.Tests
{
    class TestQBitPacker
    {
        QBitPacker bp;

        [SetUp]
        public void Setup()
        {
            bp = new QBitPacker();

        }

        [Test]
        public void TestBitOrder()
        {
            /**
             * Tests binary format in C#
             */

            //      [1]      [0]
            //     0x0F     0x15
            // 00001111 00010101
            var bits = new byte[] { 21, 15 };
            var exp = "00001111 00010101";

            Assert.AreEqual(exp, Anyad.BitString(bits));

            // Test GetBytes and ToInt16
            short v = 3861;
            var bitsShort = BitConverter.GetBytes(v);
            short v2 = BitConverter.ToInt16(bits);
            Assert.AreEqual(bits, bitsShort);
            Assert.AreEqual(v, v2);
            Assert.AreEqual(exp, Anyad.BitString(bitsShort));

            // Test back and forth converting


            // now let's try shifting
            // 00001111 00010101
            short v3 = 3861;
            v3 >>= 2;
            string expshift = "00000011 11000101";
            var bitsShift = BitConverter.GetBytes(v3);
            Assert.AreEqual(expshift, Anyad.BitString(bitsShift));
        }

        [Test]
        public void TestQDeserializationCore()
        {
            /**
             * Tests internal bitmanipulations used in Deserialization
             */

            //      [3]      [2]      [1]      [0]
            //     0x00     0x00     0x0F     0x15
            // 00000000 00000000 01011111 00010100
            var bits = new byte[] { 0x14, 0x5F, 0x00, 0x00 };

            //         ...        [1]      [0]  [VS]  ..
            // 00000000010  {00000011 11100010}  1    00
            ushort bitOffset = 2;
            ushort bitLength = 10;
            ushort expValue = 0b00000011_11100010;
            string expBits = "00000000 00000000 00000011 11100010";

            // skip BS
            bitOffset++;
            int byteOffset = bitOffset / 8;
            int byteLength = (bitLength - 1) / 8 + 1;
            int interByteBitoffset = bitLength % 8;

            // test masks
            Int32 mask = Convert.ToInt32((1 << bitLength) - 1);
            Assert.AreEqual("00000000 00000000 00000011 11111111", Anyad.BitString(mask));

            //Int16 mask = ~(Int16.MaxValue << 3);//~(0xFFFF << bitLength);

            // shift and find value
            Assert.AreEqual(2, byteLength);
            var val = BitConverter.ToInt32(bits, byteOffset);
            val >>= bitOffset;
            val &= mask;
            Assert.AreEqual(expBits, Anyad.BitString(val));
            Assert.AreEqual(expValue, val);
        }

        [Test]
        public void TestQSerializationCore()
        {
            /**
             * Tests internal bitmanipulations used in Serialization
             */
            // bit mask shifting
            int mask;
            int bitOffset = 5;
            int bitLength = 13;

            /*
             11100000   1. - starts at 5th index
             11111111   2. - filled byte
             00000011   3. - last 2 bits
             3+8+2 = 13
             */

            byte expMask1 = 0b11100000;
            byte expMask2 = 0b11111111;
            byte expMask3 = 0b00000011;
            int expIMBytes = 1;

            // 1. initial bits (ignoring [BS])
            mask = (0xFF << bitOffset);
            Assert.AreEqual(Anyad.BitString(expMask1), Anyad.BitString((byte)mask));
            Assert.AreEqual(expMask1, (byte)mask);

            // 2. intermediate (filled) bytes
            var nrIMBytes = (bitLength - (8 - bitOffset)) / 8;
            Assert.AreEqual(expIMBytes, nrIMBytes);
            mask = 0xFF;
            Assert.AreEqual(Anyad.BitString(expMask2), Anyad.BitString((byte)mask));
            Assert.AreEqual(expMask2, (byte)mask);

            // 3. finishing bits
            mask = ~(0xFF << ((bitLength + bitOffset) % 8));
            Assert.AreEqual(Anyad.BitString(expMask3), Anyad.BitString((byte)mask));
            Assert.AreEqual(expMask3, (byte)mask);
        }

        [Test]
        public void Test_ProcessBits_1()
        {
            // Same as Test Case #2 but with the mapper method

            //      [3]      [2]      [1]      [0]
            //     0x00     0x00     0x0F     0x15
            // 00000000 00000000 01011111 00010100
            var bits = new byte[] { 0x14, 0x5F, 0x00, 0x00 };

            //         ...        [1]      [0]  [VS]  ..
            // 00000000010  {00000011 11100010}  1    00
            int bitOffset = 2;
            int bitLength = 10;

            bool res = bp.UnpackValue(bits, ref bitOffset, bitLength, out int val);
            int expValue = 0b00000011_11100010;

            Assert.IsTrue(res);
            Assert.AreEqual(expValue, val);
        }

        [Test]
        public void Test_PackValue()
        {
            var bits = new byte[3];

            // 13 bits:  10011 10111011
            var packedValue = 5051;
            var bitsLength = 13;

            var expectedResult = new byte[]
            {
            //  [0]          [1]                    [2]
            //  BS, byte1    byte1, byte2, 2junk    byte2, 8 junk
                0b0111011_1,  0b00__10011_1,        0b00000000
            // masks:
            //    1111111       00__11111_1
            };

            int bitOffset = 0;
            bp.PackValue(bits, ref bitOffset, bitsLength, packedValue);

            Assert.AreEqual(Anyad.BitString(expectedResult), Anyad.BitString(bits));
            Assert.AreEqual(expectedResult, bits);
        }

        [Test]
        public void Test_BackAndForthValuePack()
        {
            // 1011000011
            int value1 = 707, value2 = 248;
            var bits = new byte[4];
            int bitsLength1 = 14, bitsLength2 = 12;
            int expectedOffset = bitsLength1 + 1, expectedOffset2 = bitsLength1 + bitsLength2 + 2;

            var expectedBits1 = new byte[] { 0b1000011_1, 0b0__0000101, 0x00, 0x00 };
            var expectedBits2 = new byte[] { 0b1000011_1, 0b1__0000101, 0b11111000, 0b0000__0000 };

            int bitOffset = 0;

            // Pack 1st value
            bp.PackValue(bits, ref bitOffset, bitsLength1, value1);
            Assert.AreEqual(Anyad.BitString(expectedBits1), Anyad.BitString(bits));
            Assert.AreEqual(expectedBits1, bits);
            Assert.AreEqual(expectedOffset, bitOffset);

            // Unpack 1st value and check
            var unpackBitOffset = 0;
            bp.UnpackValue(bits, ref unpackBitOffset, bitsLength1, out int resultValue);
            Assert.AreEqual(value1, resultValue);
            Assert.AreEqual(expectedOffset, unpackBitOffset);

            // Pack 2nd value
            bp.PackValue(bits, ref bitOffset, bitsLength2, value2);
            Assert.AreEqual(Anyad.BitString(expectedBits2), Anyad.BitString(bits));
            Assert.AreEqual(expectedBits2, bits);
            Assert.AreEqual(expectedOffset2, bitOffset);

            // Unpack 1st value and check
            bp.UnpackValue(bits, ref unpackBitOffset, bitsLength2, out resultValue);
            Assert.AreEqual(value2, resultValue);
            Assert.AreEqual(expectedOffset2, unpackBitOffset);
        }

    }
}