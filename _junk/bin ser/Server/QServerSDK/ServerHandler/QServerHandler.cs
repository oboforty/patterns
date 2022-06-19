using QServerSDK.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace QServerSDK.ServerHandler
{
    public abstract class QServerHandler<T> where T : class
    {
        protected int Frame = 0;
        protected readonly int FPS = 32;

        protected readonly QSnapshot DummyState;

        public QSnapshot MasterState { get; protected set; }

        protected IDictionary<T, QPlayerInfo<T>> playerConn = new Dictionary<T, QPlayerInfo<T>>();
        protected QPlayerInfo<T>[] players;

        protected IArenaHandler arena;
        
        protected int defaultBitOffset = 0;

        protected QSnapshotMapper sm;
        protected QCommandMapper cm;

        public bool CanStart { get; protected set; }
        public int NPlayers => players.Length;

        // ICommandBuilder cmdBuilder
        public QServerHandler(IArenaHandler arena, int fps = 30)
        {
            FPS = fps;

            this.arena = arena;

            DummyState = new QSnapshot(arena.NPlayers, name:"Dummy");
            MasterState = new QSnapshot(arena.NPlayers, name:"Master");

            playerConn = new Dictionary<T, QPlayerInfo<T>>();
            players = new QPlayerInfo<T>[arena.NPlayers];

            // target max latency: 1066 ms (30/60/120 hz)
            int NSnapshots = FPS == 30 ? 32 : (FPS == 60 ? 64 : 128);

            for (int i = 0; i < arena.NPlayers; i++)
            {
                players[i] = new QPlayerInfo<T>(i);
                players[i].snapshots = new SlidingList<QSnapshot>(NSnapshots);
            }

            arena.BuildArena(MasterState);

            // @todo: Build available command mapping

            sm = new QSnapshotMapper(2, defaultBitOffset: defaultBitOffset, defaultPacketAlloc: 20);
            cm = new QCommandMapper(defaultBitOffset: defaultBitOffset, defaultPacketAlloc: 2);
        }

        public void ReceivePacket(T connInfo, byte[] packet, int nbytes)
        {
            if (playerConn.TryGetValue(connInfo, out var player))
                ReceivePacket(player.playerId, packet, nbytes);
        }

        public virtual void ReceivePacket(int playerId, byte[] packet, int nbytes)
        {
            arena.ReceiveCommand(playerId, cm.Deserialize(packet));
        }

        public abstract void SendPacket(int playerId, byte[] packet, int nbytes);

        public virtual void Send(int playerId, QSnapshot snapshot)
        {
            byte[] packet = sm.Serialize(snapshot, out int bitOffset);

            int byteLength = (bitOffset - 1) / 8 + 1;
            SendPacket(playerId, packet, byteLength);
        }

        public virtual void Send(int playerId, QCommand command)
        {
            QCommandMapper cm = new QCommandMapper(players.Length);
            byte[] packet = cm.Serialize(command, out int bitOffset);

            int byteLength = (bitOffset - 1) / 8 + 1;
            SendPacket(playerId, packet, byteLength);
        }

        public virtual bool StartJoinPlayer(int playerId, int connHash, bool isSpectator)
        {
            var player = GetPlayerInfo(playerId);

            if (player is null)
                return false;

            player.ConnHash = connHash;
            player.IsSpectator = isSpectator;

            // @todo: handle previous UDP connection, disconnect
            // @todo: how to handle this? (player.Connected flag prevents from finishing UDP conn at the moment)
            //player.Connected = false;

            return true;
        }

        public virtual bool EndJoinPlayer(int playerId, int connHash, T connInfo)
        {
            var player = GetPlayerInfo(playerId);
            player.Connected = false;

            if (player is null)
                return false;
            else if (connHash != player.ConnHash)
                return false;

            // set player info
            playerConn[connInfo] = player;
            player.ConnInfo = connInfo;
            player.Connected = true;

            // @TODO: calculate connected players and set return value based on that
            if (player.Connected)
                CanStart = true;

            return player.Connected;
        }

        public QPlayerInfo<T> GetPlayerInfo(int playerId)
        {
            if (playerId < 0 || playerId >= players.Length)
                return null;

            return players[playerId];
        }

        public QPlayerInfo<T> GetPlayerInfo(T connInfo)
        {
            if (!playerConn.TryGetValue(connInfo, out var player))
                return null;

            return player;
        }

        public void OnSimulation()
        {
            Frame++;

            // @later: simulate bots 

            // @later: simulate physics (projectiles)
            // add new gamestate snapshot to each player
            for (int p = 0; p < players.Length; p++)
            {
                var player = players[p];
                if (player.IsOnline)
                {
                    // only send gamestate to player if they're online
                    var snapshot = GetDeltaDiffSnap(p);
                    player.snapshots.Insert(snapshot);

                    Send(p, snapshot);
                }
            }
        }
        
        protected QSnapshot GetDeltaDiffSnap(int player)
        {
            /**
             * Differentiate between server and client (=last acked player) state
             * from the perspective of player parameter
             */
            QSnapshot prevState = players[player].snapshots.LastAcked;
            QSnapshot nextState = new QSnapshot(players.Length);

            if (prevState is null)
                prevState = DummyState;
            
            for (int p = 0; p < players.Length; p++)
            {
                QTransform nextPS = nextState.players[p];
                QTransform prevPS = prevState.players[p];

                // clone from master state && calculate difference from previous sent 
                nextPS.CopyFrom(MasterState.players[p]);

                // and then mark changed fields from previous acked state
                nextPS.PosChanged = nextPS.Pos.Diff3(prevPS.Pos);
                nextPS.RotChanged = nextPS.Rot.Diff3(prevPS.Pos);
                nextPS.ScaleChanged = nextPS.Scale != prevPS.Scale;
                nextPS.HealthChanged = nextPS.Health != prevPS.Health;
            }

            return nextState;
        }
    }
}
