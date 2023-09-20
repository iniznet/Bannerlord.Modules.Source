using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class NetworkCommunication : INetworkCommunication
	{
		VirtualPlayer INetworkCommunication.MyPeer
		{
			get
			{
				NetworkCommunicator myPeer = GameNetwork.MyPeer;
				if (myPeer == null)
				{
					return null;
				}
				return myPeer.VirtualPlayer;
			}
		}

		bool INetworkCommunication.IsServer
		{
			get
			{
				return GameNetwork.IsServer;
			}
		}

		bool INetworkCommunication.IsClient
		{
			get
			{
				return GameNetwork.IsClient;
			}
		}

		bool INetworkCommunication.MultiplayerDisabled
		{
			get
			{
				return GameNetwork.MultiplayerDisabled;
			}
		}
	}
}
