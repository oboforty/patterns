using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace QServerSDK.Mapping.Tests
{
    class TestQSnapshotMapper
    {
        /**
         * This testcase tests QSnapshotMatter by providing random combinations 
         * of BitSets [BS] and values and serializing/deserializing them
         * through the entire possible value sets for each field.
         * 
         * This way we try to cover all edge cases and guarantee
         * that bugs won't appear in a networking scenario.
         */
        QSnapshotMapper sm;
        int NP;

        [SetUp]
        public void Setup()
        {
            NP = 2;
            sm = new QSnapshotMapper(NP);
        }


        [Test]
        public void Test_AddsBothPlayers()
        {
            var snap = new QSnapshot(NP);
            snap.players[0].Pos = new Vector3(1, 0, 0);
            snap.players[0].PosChanged = new Bool3(true, false, false);

            var expbits = new byte[] {
                // player 0 - 1 BS + 14 bit + 7 BS=0  --->  22 bits
                // player 1 - 2 BS (filled on last 2 remaining free bits)
                0b00000011, 0b0__0000000, 0b00__0__0__0__0__0__0,
                // player 1 - 6 BS
                0b00__0__0__0__0__0__0, //0b00000000,
            };
            int expBitoffset = 30; // 22 + 8

            var bits = sm.Serialize(snap, out int bitOffset);

            Assert.AreEqual(Anyad.BitString(expbits), Anyad.BitString(bits));
            Assert.AreEqual(expBitoffset, bitOffset);
        }

        [Test]
        public void Test_AddsBothPlayers_2()
        {
            var snap = new QSnapshot(NP);
            snap.players[1].Pos = new Vector3(1, 0, 0);
            snap.players[1].PosChanged = new Bool3(true, false, false);

            var expbits = new byte[] {
                // player 0 - 8 BS=0
                0b00000000, 
                // player 1 - 1 BS + 14 bit + 7 BS=0
                0b00000011, 0b0__0000000, 0b00__0__0__0__0__0__0,
            };
            int expBitoffset = 30; // 22 + 8

            var bits = sm.Serialize(snap, out int bitOffset);

            Assert.AreEqual(Anyad.BitString(expbits), Anyad.BitString(bits));
            Assert.AreEqual(expBitoffset, bitOffset);
        }

        //[Test]
        //public void TestBackAndForthMapping()
        //{

        //}
    }
}
