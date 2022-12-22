﻿using System;
using ServerCore;

namespace Server
{
	class GameRoom : IJobQueue
	{
		List<ClientSession> _sessions = new List<ClientSession>();
		JobQueue _jobQueue = new JobQueue();


        public void Push(Action job)
        {
			_jobQueue.Push(job);
        }

        public void Broadcast(ClientSession session, string chat)
		{
			S_Chat packet = new S_Chat();
			packet.playerId = session.SessionId;
			packet.chat = $"{chat} I am {packet.playerId}";
			ArraySegment<byte> segment = packet.Write();

			// 부하의 이유 : 모든 사람에게 하나씩 보내기 때문이다.
			foreach (ClientSession s in _sessions)
				s.Send(segment);
		}
        public void Enter(ClientSession session)
		{
			_sessions.Add(session);
			session.Room = this;
		}
		public void Leave(ClientSession session)
		{
			_sessions.Remove(session);
		}
    }
}

