using System;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000327 RID: 807
	public sealed class NetworkCommunicator : ICommunicator
	{
		// Token: 0x1400008B RID: 139
		// (add) Token: 0x06002B92 RID: 11154 RVA: 0x000A9D88 File Offset: 0x000A7F88
		// (remove) Token: 0x06002B93 RID: 11155 RVA: 0x000A9DBC File Offset: 0x000A7FBC
		public static event Action<PeerComponent> OnPeerComponentAdded;

		// Token: 0x1400008C RID: 140
		// (add) Token: 0x06002B94 RID: 11156 RVA: 0x000A9DF0 File Offset: 0x000A7FF0
		// (remove) Token: 0x06002B95 RID: 11157 RVA: 0x000A9E24 File Offset: 0x000A8024
		public static event Action<NetworkCommunicator> OnPeerSynchronized;

		// Token: 0x1400008D RID: 141
		// (add) Token: 0x06002B96 RID: 11158 RVA: 0x000A9E58 File Offset: 0x000A8058
		// (remove) Token: 0x06002B97 RID: 11159 RVA: 0x000A9E8C File Offset: 0x000A808C
		public static event Action<NetworkCommunicator> OnPeerAveragePingUpdated;

		// Token: 0x170007C7 RID: 1991
		// (get) Token: 0x06002B98 RID: 11160 RVA: 0x000A9EBF File Offset: 0x000A80BF
		public VirtualPlayer VirtualPlayer { get; }

		// Token: 0x170007C8 RID: 1992
		// (get) Token: 0x06002B99 RID: 11161 RVA: 0x000A9EC7 File Offset: 0x000A80C7
		// (set) Token: 0x06002B9A RID: 11162 RVA: 0x000A9ECF File Offset: 0x000A80CF
		public PlayerConnectionInfo PlayerConnectionInfo { get; private set; }

		// Token: 0x170007C9 RID: 1993
		// (get) Token: 0x06002B9B RID: 11163 RVA: 0x000A9ED8 File Offset: 0x000A80D8
		// (set) Token: 0x06002B9C RID: 11164 RVA: 0x000A9EE0 File Offset: 0x000A80E0
		public bool QuitFromMission { get; set; }

		// Token: 0x170007CA RID: 1994
		// (get) Token: 0x06002B9D RID: 11165 RVA: 0x000A9EE9 File Offset: 0x000A80E9
		// (set) Token: 0x06002B9E RID: 11166 RVA: 0x000A9EF1 File Offset: 0x000A80F1
		public int SessionKey { get; internal set; }

		// Token: 0x170007CB RID: 1995
		// (get) Token: 0x06002B9F RID: 11167 RVA: 0x000A9EFA File Offset: 0x000A80FA
		// (set) Token: 0x06002BA0 RID: 11168 RVA: 0x000A9F02 File Offset: 0x000A8102
		public bool JustReconnecting { get; private set; }

		// Token: 0x170007CC RID: 1996
		// (get) Token: 0x06002BA1 RID: 11169 RVA: 0x000A9F0B File Offset: 0x000A810B
		// (set) Token: 0x06002BA2 RID: 11170 RVA: 0x000A9F13 File Offset: 0x000A8113
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

		// Token: 0x170007CD RID: 1997
		// (get) Token: 0x06002BA3 RID: 11171 RVA: 0x000A9F35 File Offset: 0x000A8135
		// (set) Token: 0x06002BA4 RID: 11172 RVA: 0x000A9F3D File Offset: 0x000A813D
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

		// Token: 0x170007CE RID: 1998
		// (get) Token: 0x06002BA5 RID: 11173 RVA: 0x000A9F4F File Offset: 0x000A814F
		public bool IsMine
		{
			get
			{
				return GameNetwork.MyPeer == this;
			}
		}

		// Token: 0x170007CF RID: 1999
		// (get) Token: 0x06002BA6 RID: 11174 RVA: 0x000A9F59 File Offset: 0x000A8159
		// (set) Token: 0x06002BA7 RID: 11175 RVA: 0x000A9F61 File Offset: 0x000A8161
		public bool IsAdmin { get; private set; }

		// Token: 0x170007D0 RID: 2000
		// (get) Token: 0x06002BA8 RID: 11176 RVA: 0x000A9F6A File Offset: 0x000A816A
		public int Index
		{
			get
			{
				return this.VirtualPlayer.Index;
			}
		}

		// Token: 0x170007D1 RID: 2001
		// (get) Token: 0x06002BA9 RID: 11177 RVA: 0x000A9F77 File Offset: 0x000A8177
		public string UserName
		{
			get
			{
				return this.VirtualPlayer.UserName;
			}
		}

		// Token: 0x170007D2 RID: 2002
		// (get) Token: 0x06002BAA RID: 11178 RVA: 0x000A9F84 File Offset: 0x000A8184
		// (set) Token: 0x06002BAB RID: 11179 RVA: 0x000A9F8C File Offset: 0x000A818C
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

		// Token: 0x170007D3 RID: 2003
		// (get) Token: 0x06002BAC RID: 11180 RVA: 0x000A9FE4 File Offset: 0x000A81E4
		// (set) Token: 0x06002BAD RID: 11181 RVA: 0x000A9FEC File Offset: 0x000A81EC
		public bool IsMuted { get; set; }

		// Token: 0x170007D4 RID: 2004
		// (get) Token: 0x06002BAE RID: 11182 RVA: 0x000A9FF5 File Offset: 0x000A81F5
		// (set) Token: 0x06002BAF RID: 11183 RVA: 0x000A9FFD File Offset: 0x000A81FD
		public int ForcedAvatarIndex { get; set; } = -1;

		// Token: 0x170007D5 RID: 2005
		// (get) Token: 0x06002BB0 RID: 11184 RVA: 0x000AA006 File Offset: 0x000A8206
		public bool IsNetworkActive
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170007D6 RID: 2006
		// (get) Token: 0x06002BB1 RID: 11185 RVA: 0x000AA009 File Offset: 0x000A8209
		public bool IsConnectionActive
		{
			get
			{
				return MBNetwork.VirtualPlayers[this.Index] == this.VirtualPlayer && MBAPI.IMBPeer.IsActive(this.Index);
			}
		}

		// Token: 0x170007D7 RID: 2007
		// (get) Token: 0x06002BB2 RID: 11186 RVA: 0x000AA031 File Offset: 0x000A8231
		// (set) Token: 0x06002BB3 RID: 11187 RVA: 0x000AA068 File Offset: 0x000A8268
		public bool IsSynchronized
		{
			get
			{
				if (GameNetwork.IsServer)
				{
					return MBNetwork.VirtualPlayers[this.Index] == this.VirtualPlayer && MBAPI.IMBPeer.GetIsSynchronized(this.Index);
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

		// Token: 0x170007D8 RID: 2008
		// (get) Token: 0x06002BB4 RID: 11188 RVA: 0x000AA159 File Offset: 0x000A8359
		public bool IsServerPeer
		{
			get
			{
				return this._isServerPeer;
			}
		}

		// Token: 0x170007D9 RID: 2009
		// (get) Token: 0x06002BB5 RID: 11189 RVA: 0x000AA161 File Offset: 0x000A8361
		// (set) Token: 0x06002BB6 RID: 11190 RVA: 0x000AA169 File Offset: 0x000A8369
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

		// Token: 0x06002BB7 RID: 11191 RVA: 0x000AA17B File Offset: 0x000A837B
		private NetworkCommunicator(int index, string name, PlayerId playerID)
		{
			this.VirtualPlayer = new VirtualPlayer(index, name, playerID, this);
		}

		// Token: 0x06002BB8 RID: 11192 RVA: 0x000AA19C File Offset: 0x000A839C
		internal static NetworkCommunicator CreateAsServer(PlayerConnectionInfo playerConnectionInfo, int index, bool isAdmin)
		{
			NetworkCommunicator networkCommunicator = new NetworkCommunicator(index, playerConnectionInfo.Name, playerConnectionInfo.PlayerID);
			networkCommunicator.PlayerConnectionInfo = playerConnectionInfo;
			networkCommunicator.IsAdmin = isAdmin;
			MBNetworkPeer mbnetworkPeer = new MBNetworkPeer(networkCommunicator);
			MBAPI.IMBPeer.SetUserData(index, mbnetworkPeer);
			return networkCommunicator;
		}

		// Token: 0x06002BB9 RID: 11193 RVA: 0x000AA1DC File Offset: 0x000A83DC
		internal static NetworkCommunicator CreateAsClient(string name, int index)
		{
			return new NetworkCommunicator(index, name, PlayerId.Empty);
		}

		// Token: 0x06002BBA RID: 11194 RVA: 0x000AA1EC File Offset: 0x000A83EC
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

		// Token: 0x06002BBB RID: 11195 RVA: 0x000AA254 File Offset: 0x000A8454
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

		// Token: 0x06002BBC RID: 11196 RVA: 0x000AA2BA File Offset: 0x000A84BA
		void ICommunicator.OnSynchronizeComponentTo(VirtualPlayer peer, PeerComponent component)
		{
			GameNetwork.BeginModuleEventAsServer(peer);
			GameNetwork.WriteMessage(new AddPeerComponent(this, component.TypeId));
			GameNetwork.EndModuleEventAsServer();
		}

		// Token: 0x06002BBD RID: 11197 RVA: 0x000AA2D8 File Offset: 0x000A84D8
		internal void SetServerPeer(bool serverPeer)
		{
			this._isServerPeer = serverPeer;
		}

		// Token: 0x06002BBE RID: 11198 RVA: 0x000AA2E1 File Offset: 0x000A84E1
		internal double RefreshAndGetAveragePingInMilliseconds()
		{
			this.AveragePingInMilliseconds = MBAPI.IMBPeer.GetAveragePingInMilliseconds(this.Index);
			return this.AveragePingInMilliseconds;
		}

		// Token: 0x06002BBF RID: 11199 RVA: 0x000AA2FF File Offset: 0x000A84FF
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

		// Token: 0x06002BC0 RID: 11200 RVA: 0x000AA31E File Offset: 0x000A851E
		internal double RefreshAndGetAverageLossPercent()
		{
			this.AverageLossPercent = MBAPI.IMBPeer.GetAverageLossPercent(this.Index);
			return this.AverageLossPercent;
		}

		// Token: 0x06002BC1 RID: 11201 RVA: 0x000AA33C File Offset: 0x000A853C
		internal void SetAverageLossPercentAsClient(double lossValue)
		{
			this.AverageLossPercent = lossValue;
		}

		// Token: 0x06002BC2 RID: 11202 RVA: 0x000AA345 File Offset: 0x000A8545
		internal void SetServerPerformanceProblemStateAsClient(ServerPerformanceState serverPerformanceProblemState)
		{
			this.ServerPerformanceProblemState = serverPerformanceProblemState;
		}

		// Token: 0x06002BC3 RID: 11203 RVA: 0x000AA34E File Offset: 0x000A854E
		public void SetRelevantGameOptions(bool sendMeBloodEvents, bool sendMeSoundEvents)
		{
			MBAPI.IMBPeer.SetRelevantGameOptions(this.Index, sendMeBloodEvents, sendMeSoundEvents);
		}

		// Token: 0x06002BC4 RID: 11204 RVA: 0x000AA362 File Offset: 0x000A8562
		public uint GetHost()
		{
			return MBAPI.IMBPeer.GetHost(this.Index);
		}

		// Token: 0x06002BC5 RID: 11205 RVA: 0x000AA374 File Offset: 0x000A8574
		public uint GetReversedHost()
		{
			return MBAPI.IMBPeer.GetReversedHost(this.Index);
		}

		// Token: 0x06002BC6 RID: 11206 RVA: 0x000AA386 File Offset: 0x000A8586
		public ushort GetPort()
		{
			return MBAPI.IMBPeer.GetPort(this.Index);
		}

		// Token: 0x06002BC7 RID: 11207 RVA: 0x000AA398 File Offset: 0x000A8598
		public void UpdateConnectionInfoForReconnect(PlayerConnectionInfo playerConnectionInfo, bool isAdmin)
		{
			this.PlayerConnectionInfo = playerConnectionInfo;
			this.IsAdmin = isAdmin;
		}

		// Token: 0x06002BC8 RID: 11208 RVA: 0x000AA3A8 File Offset: 0x000A85A8
		public void UpdateIndexForReconnectingPlayer(int newIndex)
		{
			this.JustReconnecting = true;
			this.VirtualPlayer.UpdateIndexForReconnectingPlayer(newIndex);
		}

		// Token: 0x04001084 RID: 4228
		private double _averagePingInMilliseconds;

		// Token: 0x04001085 RID: 4229
		private double _averageLossPercent;

		// Token: 0x04001087 RID: 4231
		private Agent _controlledAgent;

		// Token: 0x04001088 RID: 4232
		private bool _isServerPeer;

		// Token: 0x04001089 RID: 4233
		private bool _isSynchronized;

		// Token: 0x0400108C RID: 4236
		private ServerPerformanceState _serverPerformanceProblemState;
	}
}
