using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace QServerSDK.Mapping.Tests
{
    class TestQCommandMapper
    {
        QCommandMapper m;

        [SetUp]
        public void Setup()
        {
            m = new QCommandMapper();
        }

        [Test]
        public void TestBackAndForthMapping()
        {
            var cmd = new QCommand();
            cmd.DesiredMove = new Vector3(1, 0, -1);

            var expBitOffset = 7;

            byte[] packet = m.Serialize(cmd, out int bitOffset);

            var cmd2 = m.Deserialize(packet);

            // check if bitOffset equals to certain fields set (pos 14 bits, rot 6 bits, SB 1 bit)
            Assert.AreEqual(cmd, cmd2);
            Assert.AreEqual(expBitOffset, bitOffset);

        }

        [Test]
        public void TestBitPacking()
        {
            var cmd = new QCommand();
            cmd.DesiredMove = new Vector3(1, 0, -1);

            var expBitOffset = 7;
            var expBits = new byte[]
            {
                0b0__001__0__101
            };

            byte[] packet = m.Serialize(cmd, out int bitOffset);


            Assert.AreEqual(Anyad.BitString(expBits), Anyad.BitString(packet));
            Assert.AreEqual(expBitOffset, bitOffset);
        }

        [Test]
        public void TestBitPacking_Reverse()
        {
            var bitOffset = 0;
            var packet = new byte[]
            {
                0b0__001__0__101
            };

            var cmd = m.Deserialize(packet);
            
            var expCmd = new QCommand();
            expCmd.DesiredMove = new Vector3(1, 0, -1);


            Assert.AreEqual(expCmd, cmd);
        }
    }
}
