using System;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	public sealed class NetworkCommunicator : ICommunicator
	{
		public static event Action<PeerComponent> OnPeerComponentAdded;

		public static event Action<NetworkCommunicator> OnPeerSynchronized;

		public static event Action<NetworkCommunicator> OnPeerAveragePingUpdated;

		public VirtualPlayer VirtualPlayer { get; }

		public PlayerConnectionInfo PlayerConnectionInfo { get; private set; }

		public bool QuitFromMission { get; set; }

		public int SessionKey { get; internal set; }

		public bool JustReconnecting { get; private set; }

		public double AveragePingInMilliseconds
		{
			get
			{
				return this._averagePingInMilliseconds;
			}
			private set
			{
				if (value != this._averagePingInMilliseconds)
				{
					this._averagePingInMilliseconds = value;
					Action<NetworkCommunicator> onPeerAveragePingUpdated = NetworkCommunicator.OnPeerAveragePingUpdated;
					if (onPeerAveragePingUpdated == null)
					{
						return;
					}
					onPeerAveragePingUpdated(this);
				}
			}
		}

		public double AverageLossPercent
		{
			get
			{
				return this._averageLossPercent;
			}
			private set
			{
				if (value != this._averageLossPercent)
				{
					this._averageLossPercent = value;
				}
			}
		}

		public bool IsMine
		{
			get
			{
				return GameNetwork.MyPeer == this;
			}
		}

		public bool IsAdmin { get; private set; }

		public int Index
		{
			get
			{
				return this.VirtualPlayer.Index;
			}
		}

		public string UserName
		{
			get
			{
				return this.VirtualPlayer.UserName;
			}
		}

		public Agent ControlledAgent
		{
			get
			{
				return this._controlledAgent;
			}
			set
			{
				this._controlledAgent = value;
				if (GameNetwork.IsServer)
				{
					Mission mission = ((value != null) ? value.Mission : null);
					UIntPtr uintPtr = ((mission != null) ? mission.Pointer : UIntPtr.Zero);
					int num = ((value == null) ? (-1) : value.Index);
					MBAPI.IMBPeer.SetControlledAgent(this.Index, uintPtr, num);
				}
			}
		}

		public bool IsMuted { get; set; }

		public int ForcedAvatarIndex { get; set; } = -1;

		public bool IsNetworkActive
		{
			get
			{
				return true;
			}
		}

		public bool IsConnectionActive
		{
			get
			{
				return GameNetwork.VirtualPlayers[this.Index] == this.VirtualPlayer && MBAPI.IMBPeer.IsActive(this.Index);
			}
		}

		public bool IsSynchronized
		{
			get
			{
				if (GameNetwork.IsServer)
				{
					return GameNetwork.VirtualPlayers[this.Index] == this.VirtualPlayer && MBAPI.IMBPeer.GetIsSynchronized(this.Index);
				}
				return this._isSynchronized;
			}
			set
			{
				if (value != this._isSynchronized || this.JustReconnecting)
				{
					if (GameNetwork.IsServer)
					{
						MBAPI.IMBPeer.SetIsSynchronized(this.Index, value);
					}
					this._isSynchronized = value;
					if (this._isSynchronized)
					{
						this.JustReconnecting = false;
						Action<NetworkCommunicator> onPeerSynchronized = NetworkCommunicator.OnPeerSynchronized;
						if (onPeerSynchronized != null)
						{
							onPeerSynchronized(this);
						}
					}
					if (GameNetwork.IsServer && !this.IsServerPeer)
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new SynchronizingDone(this, value));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeTargetPlayer, this);
						GameNetwork.BeginModuleEventAsServer(this);
						GameNetwork.WriteMessage(new SynchronizingDone(this, value));
						GameNetwork.EndModuleEventAsServer();
						if (value)
						{
							MBDebug.Print("Server: " + this.UserName + " is now synchronized.", 0, Debug.DebugColor.White, 17179869184UL);
							return;
						}
						MBDebug.Print("Server: " + this.UserName + " is not synchronized.", 0, Debug.DebugColor.White, 17179869184UL);
					}
				}
			}
		}

		public bool IsServerPeer
		{
			get
			{
				return this._isServerPeer;
			}
		}

		public ServerPerformanceState ServerPerformanceProblemState
		{
			get
			{
				return this._serverPerformanceProblemState;
			}
			private set
			{
				if (value != this._serverPerformanceProblemState)
				{
					this._serverPerformanceProblemState = value;
				}
			}
		}

		private NetworkCommunicator(int index, string name, PlayerId playerID)
		{
			this.VirtualPlayer = new VirtualPlayer(index, name, playerID, this);
		}

		internal static NetworkCommunicator CreateAsServer(PlayerConnectionInfo playerConnectionInfo, int index, bool isAdmin)
		{
			NetworkCommunicator networkCommunicator = new NetworkCommunicator(index, playerConnectionInfo.Name, playerConnectionInfo.PlayerID);
			networkCommunicator.PlayerConnectionInfo = playerConnectionInfo;
			networkCommunicator.IsAdmin = isAdmin;
			MBNetworkPeer mbnetworkPeer = new MBNetworkPeer(networkCommunicator);
			MBAPI.IMBPeer.SetUserData(index, mbnetworkPeer);
			return networkCommunicator;
		}

		internal static NetworkCommunicator CreateAsClient(string name, int index)
		{
			return new NetworkCommunicator(index, name, PlayerId.Empty);
		}

		void ICommunicator.OnAddComponent(PeerComponent component)
		{
			if (GameNetwork.IsServer)
			{
				if (!this.IsServerPeer)
				{
					GameNetwork.BeginModuleEventAsServer(this);
					GameNetwork.WriteMessage(new AddPeerComponent(this, component.TypeId));
					GameNetwork.EndModuleEventAsServer();
				}
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new AddPeerComponent(this, component.TypeId));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeTargetPlayer | GameNetwork.EventBroadcastFlags.AddToMissionRecord, this);
			}
			Action<PeerComponent> onPeerComponentAdded = NetworkCommunicator.OnPeerComponentAdded;
			if (onPeerComponentAdded == null)
			{
				return;
			}
			onPeerComponentAdded(component);
		}

		void ICommunicator.OnRemoveComponent(PeerComponent component)
		{
			if (GameNetwork.IsServer)
			{
				if (!this.IsServerPeer && (this.IsSynchronized || !this.JustReconnecting))
				{
					GameNetwork.BeginModuleEventAsServer(this);
					GameNetwork.WriteMessage(new RemovePeerComponent(this, component.TypeId));
					GameNetwork.EndModuleEventAsServer();
				}
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new RemovePeerComponent(this, component.TypeId));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeTargetPlayer | GameNetwork.EventBroadcastFlags.AddToMissionRecord, this);
			}
		}

		void ICommunicator.OnSynchronizeComponentTo(VirtualPlayer peer, PeerComponent component)
		{
			GameNetwork.BeginModuleEventAsServer(peer);
			GameNetwork.WriteMessage(new AddPeerComponent(this, component.TypeId));
			GameNetwork.EndModuleEventAsServer();
		}

		internal void SetServerPeer(bool serverPeer)
		{
			this._isServerPeer = serverPeer;
		}

		internal double RefreshAndGetAveragePingInMilliseconds()
		{
			this.AveragePingInMilliseconds = MBAPI.IMBPeer.GetAveragePingInMilliseconds(this.Index);
			return this.AveragePingInMilliseconds;
		}

		internal void SetAveragePingInMillisecondsAsClient(double pingValue)
		{
			this.AveragePingInMilliseconds = pingValue;
			Agent controlledAgent = this.ControlledAgent;
			if (controlledAgent == null)
			{
				return;
			}
			controlledAgent.SetAveragePingInMilliseconds(this.AveragePingInMilliseconds);
		}

		internal double RefreshAndGetAverageLossPercent()
		{
			this.AverageLossPercent = MBAPI.IMBPeer.GetAverageLossPercent(this.Index);
			return this.AverageLossPercent;
		}

		internal void SetAverageLossPercentAsClient(double lossValue)
		{
			this.AverageLossPercent = lossValue;
		}

		internal void SetServerPerformanceProblemStateAsClient(ServerPerformanceState serverPerformanceProblemState)
		{
			this.ServerPerformanceProblemState = serverPerformanceProblemState;
		}

		public void SetRelevantGameOptions(bool sendMeBloodEvents, bool sendMeSoundEvents)
		{
			MBAPI.IMBPeer.SetRelevantGameOptions(this.Index, sendMeBloodEvents, sendMeSoundEvents);
		}

		public uint GetHost()
		{
			return MBAPI.IMBPeer.GetHost(this.Index);
		}

		public uint GetReversedHost()
		{
			return MBAPI.IMBPeer.GetReversedHost(this.Index);
		}

		public ushort GetPort()
		{
			return MBAPI.IMBPeer.GetPort(this.Index);
		}

		public void UpdateConnectionInfoForReconnect(PlayerConnectionInfo playerConnectionInfo, bool isAdmin)
		{
			this.PlayerConnectionInfo = playerConnectionInfo;
			this.IsAdmin = isAdmin;
		}

		public void UpdateIndexForReconnectingPlayer(int newIndex)
		{
			this.JustReconnecting = true;
			this.VirtualPlayer.UpdateIndexForReconnectingPlayer(newIndex);
		}

		private double _averagePingInMilliseconds;

		private double _averageLossPercent;

		private Agent _controlledAgent;

		private bool _isServerPeer;

		private bool _isSynchronized;

		private ServerPerformanceState _serverPerformanceProblemState;
	}
}
