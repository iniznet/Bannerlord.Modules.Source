using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
	internal class MBNetworkPeer : DotNetObject
	{
		public NetworkCommunicator NetworkPeer { get; }

		internal MBNetworkPeer(NetworkCommunicator networkPeer)
		{
			this.NetworkPeer = networkPeer;
		}
	}
}
