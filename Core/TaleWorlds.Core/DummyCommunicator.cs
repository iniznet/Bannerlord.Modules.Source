using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Core
{
	public class DummyCommunicator : ICommunicator
	{
		public VirtualPlayer VirtualPlayer { get; }

		public void OnSynchronizeComponentTo(VirtualPlayer peer, PeerComponent component)
		{
		}

		public void OnAddComponent(PeerComponent component)
		{
		}

		public void OnRemoveComponent(PeerComponent component)
		{
		}

		public bool IsNetworkActive
		{
			get
			{
				return false;
			}
		}

		public bool IsConnectionActive
		{
			get
			{
				return false;
			}
		}

		public bool IsServerPeer
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		private DummyCommunicator(int index, string name)
		{
			this.VirtualPlayer = new VirtualPlayer(index, name, PlayerId.Empty, this);
		}

		public static DummyCommunicator CreateAsServer(int index, string name)
		{
			return new DummyCommunicator(index, name);
		}

		public static DummyCommunicator CreateAsClient(string name, int index)
		{
			return new DummyCommunicator(index, name);
		}
	}
}
