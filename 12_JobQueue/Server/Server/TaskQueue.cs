using System;

// 예시용, 아직은 이렇게 class를 만드는 방식이 더 많음
// 람다를 사용하는 방식은 위험성이 짙기에 잘 써야한다.
namespace Server
{
	interface ITask
	{
		void Execute();
	}

	class BroadcastTask : ITask
	{
		GameRoom _room;
		ClientSession _session;
		string _chat;

		BroadcastTask(GameRoom room, ClientSession session, string chat)
		{
			_room = room;
			_session = session;
			_chat = chat;
		}


		public void Execute()
        {
			_room.Broadcast(_session, _chat);
        }
    }

	public class TaskQueue
	{
		Queue<ITask> _queue = new Queue<ITask>();
	}
}

