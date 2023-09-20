﻿using System;

namespace TaleWorlds.Core
{
	public interface ICommunicator
	{
		VirtualPlayer VirtualPlayer { get; }

		void OnSynchronizeComponentTo(VirtualPlayer peer, PeerComponent component);

		void OnAddComponent(PeerComponent component);

		void OnRemoveComponent(PeerComponent component);

		bool IsNetworkActive { get; }

		bool IsConnectionActive { get; }

		bool IsServerPeer { get; }

		bool IsSynchronized { get; set; }
	}
}
