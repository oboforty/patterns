using NUnit.Framework;
using System;

namespace QServerSDK.Mapping.Tests
{
    public class TestQTransformMapper
    {
        QTransformMapper m;

        [SetUp]
        public void Setup()
        {
            m = new QTransformMapper();
        }

        [Test]
        public void Test_Deserialize()
        {
            // deserialize x=417, z=820 and rotY=45 bits 
            //           X  Y             Z  rX         rY  rZ  S - -
            // 1 110100001; 0; 1 1100110100;  0; 1 1000001; 0;  0 0 0
            var bits = new byte[] {
                // X, Y
                0b0100001_1, 0b0__0000011,
                // Z, rX
                0b0110100_1, 0b0__0000110,
                // rY, rZ
                0b0__101101_1,
                // S, Hp, 
                0b000000__0__0,

                // @todo: handle this lagging byte?
                0x00,
            };

            var t = m.Deserialize(bits);

            Assert.AreEqual(417, t.Pos.x);
            Assert.AreEqual(820, t.Pos.z);
            Assert.AreEqual(45, t.Rot.y);
        }

        [Test]
        public void Test_Serialize()
        {
            var snap = new QTransform();
            snap.Pos = new Vector3(417, 0, 820);
            snap.PosChanged = new Bool3(true, false, true);
            snap.Rot = new Vector3(0, 45, 0);
            snap.RotChanged = new Bool3(false, true, false);

            byte[] bits = m.Serialize(snap);

            var expbits = new byte[] {
                // X, Y
                0b0100001_1, 0b0__0000011,
                // Z, rX
                0b0110100_1, 0b0__0000110,
                // rY, rZ
                0b0__101101_1,
                // S, Hp, 
                0b000000__0__0,

                // ?? @todo: add this for fixed testcase
                //0x00,
            };

            Assert.AreEqual(Anyad.BitString(expbits), Anyad.BitString(bits));
        }

        [Test]
        public void Test_BackAndForthComplete()
        {
            var snap = new QTransform();
            snap.Pos = new Vector3(4768, 0, 1017);
            snap.PosChanged = new Bool3(true, false, true);
            snap.Rot = new Vector3(0, 13, 22);
            snap.RotChanged = new Bool3(false, true, true);
            snap.Health = 2;
            snap.HealthChanged = true;

            var expBitOffset = 14 * 2 + 6 * 2 + 3 + 8;

            byte[] packet = m.Serialize(snap);

            int bitOffset = 0;
            var snap2 = m.Deserialize(packet, ref bitOffset);

            // check if bitOffset equals to certain fields set (pos 14 bits, rot 6 bits, SB 1 bit)
            Assert.AreEqual(snap, snap2);
            Assert.AreEqual(expBitOffset, bitOffset);
        }

        [Test]
        public void Test_Serialize_UnchangedButSetValue()
        {
            var snap = new QTransform();
            snap.Pos = new Vector3(1, 0, 0);
            //snap.PosChanged = new Bool3(false, false, true);

            byte[] bits = m.Serialize(snap);

            // 8 BS=0 can be packed in a byte
            var expbits = new byte[] {
                0b0000000,
            };

            Assert.AreEqual(Anyad.BitString(expbits), Anyad.BitString(bits));

            // now let's try 0,0,0 vector and set bit
            var snap2 = new QTransform();
            snap2.Pos = new Vector3(0, 0, 0);
            snap2.PosChanged = new Bool3(true, false, false);

            var bits2 = m.Serialize(snap2);

            var expbits2 = new byte[] {
                // 3 bytes, because 1 BS=1 + 14 bit + 7 BS=0
                0b00000001, 0b0__0000000, 0b00__0__0__0__0__0__0,
            };

            Assert.AreEqual(Anyad.BitString(expbits2), Anyad.BitString(bits2));
        }

        [Test]
        public void Test_NoNegativeValue()
        {
            var snap2 = new QTransform();
            snap2.Pos = new Vector3(-1, 0, 0);
            snap2.PosChanged = new Bool3(true, false, false);

            Assert.Throws<Exception>(() => {
                var bits2 = m.Serialize(snap2);
            }, "out_of_bounds");
        }
    }
}