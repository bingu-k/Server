using System;
using ServerCore;
using System.Collections.Generic;

class PacketManager
{
    #region Singletone
    static PacketManager _instance;
	public static PacketManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = new PacketManager();
			return _instance;
		}
	}
    #endregion

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
    public void Register()
    {

        _onRecv.Add((ushort)PacketID.S_Test, MakePacket<S_Test>);
        _handler.Add((ushort)PacketID.S_Test, PacketHandler.S_TestHandler);

    }

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Action<PacketSession, ArraySegment<byte>> action = null;
        if (_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffer);
    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T packet = new T();
        packet.Read(buffer);

        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(packet.Protocol, out action))
            action.Invoke(session, packet);
    }
}