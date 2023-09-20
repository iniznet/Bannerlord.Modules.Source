using System;

namespace TaleWorlds.Core
{
	public interface INetworkCommunication
	{
		VirtualPlayer MyPeer { get; }

		bool IsServer { get; }

		bool IsClient { get; }

		bool MultiplayerDisabled { get; }
	}
}
