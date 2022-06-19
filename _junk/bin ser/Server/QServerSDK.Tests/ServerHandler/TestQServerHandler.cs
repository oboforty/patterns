using NUnit.Framework;
using QServerSDK.Mapping;
using QServerSDK.ServerHandler;
using System;
using System.Collections.Generic;
using System.Text;

namespace QServerSDK.ServerHandler.Tests
{
    class TestQServerHandler
    {
        TestServerHandler hand;
        TestArena arena;

        string connInfo;
        int playerId;

        QCommandMapper cm;
        QSnapshotMapper sm;

        Queue<(int, int, byte[])> receivedPackets;

        [SetUp]
        public void Setup()
        {
            receivedPackets = new Queue<(int, int, byte[])>();

            connInfo = "player1";
            playerId = 1;
            arena = new TestArena();
            hand = new TestServerHandler(arena, receivedPackets);

            cm = new QCommandMapper(arena.NPlayers);
            sm = new QSnapshotMapper(arena.NPlayers);

            int connHash = (new Random()).Next();
            hand.StartJoinPlayer(playerId, connHash, false);
            hand.EndJoinPlayer(playerId, connHash, connInfo);
        }

        [Test]
        public void TestDefaultState()
        {
            var _arena = new TestArena();
            var _hand = new TestServerHandler(arena, null);

            Assert.AreEqual(_arena.NPlayers, _hand.NPlayers);
            Assert.AreEqual(_arena.NPlayers, _hand.MasterState.players.Length);

            Assert.IsFalse(_hand.CanStart);
        }

        [Test]
        public void TestPlayerJoins()
        {
            Assert.AreEqual(arena.NPlayers, hand.NPlayers);
            Assert.AreEqual(arena.NPlayers, hand.MasterState.players.Length);

            // added player(s), check state changes
            Assert.IsTrue(hand.CanStart);

            // Player 0 (not connected)
            var p0 = hand.GetPlayerInfo(0);
            Assert.AreEqual(p0.playerId, 0);
            Assert.AreEqual(p0.ConnHash, -1);
            Assert.IsNull(p0.ConnInfo);
            Assert.IsFalse(p0.Connected);
            Assert.IsFalse(p0.IsSpectator);
            Assert.IsFalse(p0.IsOnline);
            Assert.IsNotNull(p0.snapshots);

            // Player 1 (tested)
            var p1 = hand.GetPlayerInfo(1);
            Assert.AreEqual(p1.playerId, 1);
            Assert.AreNotEqual(p1.ConnHash, -1);
            Assert.AreEqual(p1.ConnInfo, connInfo);
            Assert.IsTrue(p1.Connected);
            Assert.IsFalse(p1.IsSpectator);
            Assert.IsTrue(p1.IsOnline);
            Assert.IsNotNull(p1.snapshots);
        }

        [Test]
        public void TestDeltaDiff_Series()
        {
            int frames = 20;

            // Default state & after command received
            QSnapshot expSnap = new QSnapshot(arena.NPlayers);
            expSnap.players[1].Pos = new Vector3(0, 0, 0);

            // test N packets if they work
            for (int i = 0; i < frames; i++)
            {
                hand.OnSimulation();

                (int playerId, int nbytes, byte[] packet) = receivedPackets.Dequeue();
                var snap = sm.Deserialize(packet);

                Assert.AreEqual(expSnap, snap);
            }
        }

        [Test]
        public void TestDeltaDiff_2()
        {
            // Default state & after command received
            QSnapshot expSnapshot1 = new QSnapshot(arena.NPlayers);
            expSnapshot1.players[1].Pos = new Vector3(0, 0, 0);
            QSnapshot expSnapshot2 = new QSnapshot(arena.NPlayers);
            expSnapshot2.players[1].Pos = new Vector3(1, 0, 0);
            expSnapshot2.players[1].PosChanged.x = true;

            // Check default state
            hand.OnSimulation();
            (int playerId, int nbytes, byte[] packet) = receivedPackets.Dequeue();
            var snap = sm.Deserialize(packet);
            Assert.AreEqual(expSnapshot1, snap);

            // Send Command
            QCommand cmd = new QCommand();
            cmd.DesiredMove = new Vector3(1, 0, 0);
            byte[] cmdpack = cm.Serialize(cmd, out int nbits);
            hand.ReceivePacket(playerId, cmdpack, nbits % 8);

            // See if master state has changed with new player positions
            hand.OnSimulation();
            (playerId, nbytes, packet) = receivedPackets.Dequeue();
            var snap2 = sm.Deserialize(packet);
            Assert.AreEqual(expSnapshot2, snap2);
        }

        [Test]
        public void TestCommand_NonAcked_KeepsSnapshotChanged()
        {

        }

        [Test]
        public void TestCommand_Acked_SnapshotBecomesUnchanged()
        {

        }


        /*
         * TODO: move into Metin2Arena tests
        [Test]
        public void TestMove_Bounds()
        {
            // supplement 
            hand = new TestServerHandler(arena, receivedPackets);

            // Default state & after command received
            QSnapshot expSnapshot1 = new QSnapshot(arena.NPlayers);
            expSnapshot1.players[1].Pos = new Vector3(0, 0, 0);

            // Check default state
            hand.OnSimulation();
            (int playerId, int nbytes, byte[] packet) = receivedPackets.Dequeue();
            var snap = sm.Deserialize(packet);
            Assert.AreEqual(expSnapshot1, snap);

            // Send Command
            QCommand cmd = new QCommand();
            cmd.DesiredMove = new Vector3(-1, 0, 0);
            byte[] cmdpack = cm.Serialize(cmd, out int nbits);
            hand.ReceivePacket(playerId, cmdpack, nbits % 8);

            // master state stays the same, because player1 is already at (0,0,0) [edge of world]
            Assert.DoesNotThrow(() => {
                hand.OnSimulation();
            }, "out_of_bounds");

            (playerId, nbytes, packet) = receivedPackets.Dequeue();
            var snap2 = sm.Deserialize(packet);
            Assert.AreEqual(expSnapshot1, snap2);
        }
        */
        /*
        private bool SnapshotEquals(QSnapshot expsnap)
        {
            hand.OnSimulation();

            (int playerId, int nbytes, byte[] packet) = receivedPackets.Dequeue();
            var snap = sm.Deserialize(packet);

            //Assert.AreEqual(playerId, playerId)

            return expsnap == snap;
        }

        private bool EventualSnapshotIsRead(QSnapshot expsnap, int frames, out int frameChange, QSnapshot expprevsnap = null)
        {
            frameChange = -1;
            bool hasChangedSnap = false;

            for (int i = 0; i < frames; i++)
            {
                hand.OnSimulation();

                (int playerId, int nbytes, byte[] packet) = receivedPackets.Dequeue();
                var snap = sm.Deserialize(packet);

                // compare snap with expsnap
                if (expsnap != snap)
                {
                    // record the frame where change happens
                    if (!hasChangedSnap)
                    {
                        frameChange = i;
                        hasChangedSnap = true;
                    }
                    // else 
                    // OK! value keeps
                }
                else if (expprevsnap != null)
                {
                    if (hasChangedSnap)
                    {
                        // value is switching => doesn't eventually settle down
                        return false;
                    }
                    else
                    {
                        // value is supposed to be old value, check that
                        if (expprevsnap != snap)
                            // check previous value state as well
                            return false;
                        //else
                        //    continue;
                    }
                }
            }

            return frameChange != -1;
        }*/
    }
}
