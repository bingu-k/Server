using System;
using ServerCore;

namespace Server
{
	class GameRoom : IJobQueue
	{
		List<ClientSession> _sessions = new List<ClientSession>();
		JobQueue _jobQueue = new JobQueue();
		List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public void Push(Action job)
        {
			_jobQueue.Push(job);
        }

		public void Flush()
		{
			// 부하의 이유 : 모든 사람에게 하나씩 보내기 때문이다.
			foreach (ClientSession s in _sessions)
				s.Send(_pendingList);

			//Console.WriteLine($"Flushed {_pendingList.Count} items");
			_pendingList.Clear();
		}

        public void Broadcast(ArraySegment<byte> segment)
		{
			_pendingList.Add(segment);
		}
        public void Enter(ClientSession session)
		{
			// Insert player
			_sessions.Add(session);
			session.Room = this;

			// Get all Player
			S_PlayerList players = new S_PlayerList();
			foreach (ClientSession s in _sessions)
			{
				players.players.Add(new S_PlayerList.Player()
				{
					isSelf = (s == session),
					playerId = s.SessionId,
					PosX = s.PosX,
					PosY = s.PosY,
					PosZ = s.PosZ
				});
			}
			session.Send(players.Write());

			// Announce all Player
			S_BroadcastEnterGame enter = new S_BroadcastEnterGame();
			enter.playerId = session.SessionId;
            enter.PosX = 0;
            enter.PosY = 0;
            enter.PosZ = 0;
			Broadcast(enter.Write());
        }
		public void Leave(ClientSession session)
		{
			// Remove player
			_sessions.Remove(session);

            // Announce all Player
            S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
            leave.playerId = session.SessionId;
            Broadcast(leave.Write());
        }

		public void Move(ClientSession session, C_Move packet)
		{
            // 좌표 변경
            session.PosX = packet.PosX;
            session.PosY = packet.PosY;
            session.PosZ = packet.PosZ;

            // 모두에게 알림
			S_BroadcastMove move = new S_BroadcastMove();
			move.playerId = session.SessionId;
			move.PosX = packet.PosX;
			move.PosY = packet.PosY;
			move.PosZ = packet.PosZ;
			Broadcast(move.Write());
        }
    }
}

