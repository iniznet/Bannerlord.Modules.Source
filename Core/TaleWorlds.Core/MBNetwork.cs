using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
	public static class MBNetwork
	{
		public static INetworkCommunication NetworkViewCommunication { get; private set; }

		public static bool MultiplayerDisabled
		{
			get
			{
				return MBNetwork.NetworkViewCommunication == null || MBNetwork.NetworkViewCommunication.MultiplayerDisabled;
			}
		}

		public static List<ICommunicator> NetworkPeers { get; private set; }

		public static List<ICommunicator> DisconnectedNetworkPeers { get; private set; }

		public static bool LogDisabled
		{
			get
			{
				return true;
			}
		}

		public static bool IsSessionActive
		{
			get
			{
				return MBNetwork.IsServer || MBNetwork.IsClient;
			}
		}

		public static bool IsServer
		{
			get
			{
				return MBNetwork.NetworkViewCommunication != null && MBNetwork.NetworkViewCommunication.IsServer;
			}
		}

		public static bool IsClient
		{
			get
			{
				return MBNetwork.NetworkViewCommunication != null && MBNetwork.NetworkViewCommunication.IsClient;
			}
		}

		public static void Initialize(INetworkCommunication networkCommunication)
		{
			MBNetwork.VirtualPlayers = new VirtualPlayer[1023];
			MBNetwork.NetworkPeers = new List<ICommunicator>();
			MBNetwork.DisconnectedNetworkPeers = new List<ICommunicator>();
			MBNetwork.NetworkViewCommunication = networkCommunication;
		}

		public static VirtualPlayer MyPeer
		{
			get
			{
				if (MBNetwork.NetworkViewCommunication != null)
				{
					return MBNetwork.NetworkViewCommunication.MyPeer;
				}
				return null;
			}
		}

		public const int MaxPlayerCount = 1023;

		public static VirtualPlayer[] VirtualPlayers;
	}
}
