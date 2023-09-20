using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002B8 RID: 696
	public class VoiceChatHandler : MissionNetwork
	{
		// Token: 0x1400006C RID: 108
		// (add) Token: 0x060026B8 RID: 9912 RVA: 0x000911E0 File Offset: 0x0008F3E0
		// (remove) Token: 0x060026B9 RID: 9913 RVA: 0x00091218 File Offset: 0x0008F418
		public event Action OnVoiceRecordStarted;

		// Token: 0x1400006D RID: 109
		// (add) Token: 0x060026BA RID: 9914 RVA: 0x00091250 File Offset: 0x0008F450
		// (remove) Token: 0x060026BB RID: 9915 RVA: 0x00091288 File Offset: 0x0008F488
		public event Action OnVoiceRecordStopped;

		// Token: 0x1400006E RID: 110
		// (add) Token: 0x060026BC RID: 9916 RVA: 0x000912C0 File Offset: 0x0008F4C0
		// (remove) Token: 0x060026BD RID: 9917 RVA: 0x000912F8 File Offset: 0x0008F4F8
		public event Action<MissionPeer, bool> OnPeerVoiceStatusUpdated;

		// Token: 0x1400006F RID: 111
		// (add) Token: 0x060026BE RID: 9918 RVA: 0x00091330 File Offset: 0x0008F530
		// (remove) Token: 0x060026BF RID: 9919 RVA: 0x00091368 File Offset: 0x0008F568
		public event Action<MissionPeer> OnPeerMuteStatusUpdated;

		// Token: 0x1700071C RID: 1820
		// (get) Token: 0x060026C0 RID: 9920 RVA: 0x0009139D File Offset: 0x0008F59D
		// (set) Token: 0x060026C1 RID: 9921 RVA: 0x000913A8 File Offset: 0x0008F5A8
		private bool IsVoiceRecordActive
		{
			get
			{
				return this._isVoiceRecordActive;
			}
			set
			{
				if (!this._isVoiceChatDisabled)
				{
					this._isVoiceRecordActive = value;
					if (this._isVoiceRecordActive)
					{
						SoundManager.StartVoiceRecording();
						Action onVoiceRecordStarted = this.OnVoiceRecordStarted;
						if (onVoiceRecordStarted == null)
						{
							return;
						}
						onVoiceRecordStarted();
						return;
					}
					else
					{
						SoundManager.StopVoiceRecording();
						Action onVoiceRecordStopped = this.OnVoiceRecordStopped;
						if (onVoiceRecordStopped == null)
						{
							return;
						}
						onVoiceRecordStopped();
					}
				}
			}
		}

		// Token: 0x060026C2 RID: 9922 RVA: 0x000913F7 File Offset: 0x0008F5F7
		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.Register<SendVoiceToPlay>(new GameNetworkMessage.ServerMessageHandlerDelegate<SendVoiceToPlay>(this.HandleServerEventSendVoiceToPlay));
				return;
			}
			if (GameNetwork.IsServer)
			{
				registerer.Register<SendVoiceRecord>(new GameNetworkMessage.ClientMessageHandlerDelegate<SendVoiceRecord>(this.HandleClientEventSendVoiceRecord));
			}
		}

		// Token: 0x060026C3 RID: 9923 RVA: 0x0009142C File Offset: 0x0008F62C
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			if (!GameNetwork.IsDedicatedServer)
			{
				this._playerVoiceDataList = new List<VoiceChatHandler.PeerVoiceData>();
				SoundManager.InitializeVoicePlayEvent();
				this._voiceToSend = new Queue<byte>();
			}
		}

		// Token: 0x060026C4 RID: 9924 RVA: 0x00091458 File Offset: 0x0008F658
		public override void AfterStart()
		{
			this.UpdateVoiceChatEnabled();
			if (!this._isVoiceChatDisabled)
			{
				MissionPeer.OnTeamChanged += this.MissionPeerOnTeamChanged;
				Mission.Current.GetMissionBehavior<MissionNetworkComponent>().OnClientSynchronizedEvent += this.OnPlayerSynchronized;
			}
			NativeOptions.OnNativeOptionChanged = (NativeOptions.OnNativeOptionChangedDelegate)Delegate.Combine(NativeOptions.OnNativeOptionChanged, new NativeOptions.OnNativeOptionChangedDelegate(this.OnNativeOptionChanged));
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
		}

		// Token: 0x060026C5 RID: 9925 RVA: 0x000914E0 File Offset: 0x0008F6E0
		public override void OnRemoveBehavior()
		{
			if (!this._isVoiceChatDisabled)
			{
				MissionPeer.OnTeamChanged -= this.MissionPeerOnTeamChanged;
			}
			if (!GameNetwork.IsDedicatedServer)
			{
				if (this.IsVoiceRecordActive)
				{
					this.IsVoiceRecordActive = false;
				}
				SoundManager.FinalizeVoicePlayEvent();
			}
			NativeOptions.OnNativeOptionChanged = (NativeOptions.OnNativeOptionChangedDelegate)Delegate.Remove(NativeOptions.OnNativeOptionChanged, new NativeOptions.OnNativeOptionChangedDelegate(this.OnNativeOptionChanged));
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			base.OnRemoveBehavior();
		}

		// Token: 0x060026C6 RID: 9926 RVA: 0x00091567 File Offset: 0x0008F767
		public override void OnPreDisplayMissionTick(float dt)
		{
			if (!GameNetwork.IsDedicatedServer && !this._isVoiceChatDisabled)
			{
				this.VoiceTick(dt);
			}
		}

		// Token: 0x060026C7 RID: 9927 RVA: 0x00091580 File Offset: 0x0008F780
		private bool HandleClientEventSendVoiceRecord(NetworkCommunicator peer, SendVoiceRecord message)
		{
			MissionPeer component = peer.GetComponent<MissionPeer>();
			if (message.BufferLength > 0 && component.Team != null)
			{
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					MissionPeer component2 = networkCommunicator.GetComponent<MissionPeer>();
					if (networkCommunicator.IsSynchronized && component2 != null && component2.Team == component.Team && (message.ReceiverList == null || message.ReceiverList.Contains(networkCommunicator.VirtualPlayer)) && component2 != component)
					{
						GameNetwork.BeginModuleEventAsServerUnreliable(component2.Peer);
						GameNetwork.WriteMessage(new SendVoiceToPlay(peer, message.Buffer, message.BufferLength));
						GameNetwork.EndModuleEventAsServerUnreliable();
					}
				}
			}
			return true;
		}

		// Token: 0x060026C8 RID: 9928 RVA: 0x0009164C File Offset: 0x0008F84C
		private void HandleServerEventSendVoiceToPlay(SendVoiceToPlay message)
		{
			if (!this._isVoiceChatDisabled)
			{
				MissionPeer component = message.Peer.GetComponent<MissionPeer>();
				if (component != null && message.BufferLength > 0 && !component.IsMutedFromGameOrPlatform)
				{
					for (int i = 0; i < this._playerVoiceDataList.Count; i++)
					{
						if (this._playerVoiceDataList[i].Peer == component)
						{
							byte[] array = new byte[8640];
							int num;
							this.DecompressVoiceChunk(message.Peer.Index, message.Buffer, message.BufferLength, ref array, out num);
							this._playerVoiceDataList[i].WriteVoiceData(array, num);
							return;
						}
					}
				}
			}
		}

		// Token: 0x060026C9 RID: 9929 RVA: 0x000916EE File Offset: 0x0008F8EE
		private void CheckStopVoiceRecord()
		{
			if (this._stopRecordingOnNextTick)
			{
				this.IsVoiceRecordActive = false;
				this._stopRecordingOnNextTick = false;
			}
		}

		// Token: 0x060026CA RID: 9930 RVA: 0x00091708 File Offset: 0x0008F908
		private void VoiceTick(float dt)
		{
			int num = 120;
			if (this._playedAnyVoicePreviousTick)
			{
				int num2 = MathF.Ceiling(dt * 1000f);
				num = MathF.Min(num, num2);
				this._playedAnyVoicePreviousTick = false;
			}
			foreach (VoiceChatHandler.PeerVoiceData peerVoiceData in this._playerVoiceDataList)
			{
				Action<MissionPeer, bool> onPeerVoiceStatusUpdated = this.OnPeerVoiceStatusUpdated;
				if (onPeerVoiceStatusUpdated != null)
				{
					onPeerVoiceStatusUpdated(peerVoiceData.Peer, peerVoiceData.HasAnyVoiceData());
				}
			}
			int num3 = num * 12;
			for (int i = 0; i < num3; i++)
			{
				for (int j = 0; j < this._playerVoiceDataList.Count; j++)
				{
					this._playerVoiceDataList[j].ProcessVoiceData();
				}
			}
			for (int k = 0; k < this._playerVoiceDataList.Count; k++)
			{
				Queue<short> voiceToPlayForTick = this._playerVoiceDataList[k].GetVoiceToPlayForTick();
				if (voiceToPlayForTick.Count > 0)
				{
					int count = voiceToPlayForTick.Count;
					byte[] array = new byte[count * 2];
					for (int l = 0; l < count; l++)
					{
						byte[] bytes = BitConverter.GetBytes(voiceToPlayForTick.Dequeue());
						array[l * 2] = bytes[0];
						array[l * 2 + 1] = bytes[1];
					}
					SoundManager.UpdateVoiceToPlay(array, array.Length, k);
					this._playedAnyVoicePreviousTick = true;
				}
			}
			if (this.IsVoiceRecordActive)
			{
				byte[] array2 = new byte[72000];
				int num4;
				SoundManager.GetVoiceData(array2, 72000, out num4);
				for (int m = 0; m < num4; m++)
				{
					this._voiceToSend.Enqueue(array2[m]);
				}
				this.CheckStopVoiceRecord();
			}
			while (this._voiceToSend.Count > 0 && (this._voiceToSend.Count >= 1440 || !this.IsVoiceRecordActive))
			{
				int num5 = MathF.Min(this._voiceToSend.Count, 1440);
				byte[] array3 = new byte[1440];
				for (int n = 0; n < num5; n++)
				{
					array3[n] = this._voiceToSend.Dequeue();
				}
				if (GameNetwork.IsClient)
				{
					byte[] array4 = new byte[8640];
					int num6;
					this.CompressVoiceChunk(0, array3, ref array4, out num6);
					GameNetwork.BeginModuleEventAsClientUnreliable();
					GameNetwork.WriteMessage(new SendVoiceRecord(array4, num6));
					GameNetwork.EndModuleEventAsClientUnreliable();
				}
				else if (GameNetwork.IsServer)
				{
					VoiceChatHandler.<>c__DisplayClass38_0 CS$<>8__locals1 = new VoiceChatHandler.<>c__DisplayClass38_0();
					VoiceChatHandler.<>c__DisplayClass38_0 CS$<>8__locals2 = CS$<>8__locals1;
					NetworkCommunicator myPeer = GameNetwork.MyPeer;
					CS$<>8__locals2.myMissionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
					if (CS$<>8__locals1.myMissionPeer != null)
					{
						this._playerVoiceDataList.Single((VoiceChatHandler.PeerVoiceData x) => x.Peer == CS$<>8__locals1.myMissionPeer).WriteVoiceData(array3, num5);
					}
				}
			}
			if (!this.IsVoiceRecordActive && base.Mission.InputManager.IsGameKeyPressed(33))
			{
				this.IsVoiceRecordActive = true;
			}
			if (this.IsVoiceRecordActive && base.Mission.InputManager.IsGameKeyReleased(33))
			{
				this._stopRecordingOnNextTick = true;
			}
		}

		// Token: 0x060026CB RID: 9931 RVA: 0x00091A10 File Offset: 0x0008FC10
		private void DecompressVoiceChunk(int clientID, byte[] compressedVoiceBuffer, int compressedBufferLength, ref byte[] voiceBuffer, out int bufferLength)
		{
			SoundManager.DecompressData(clientID, compressedVoiceBuffer, compressedBufferLength, voiceBuffer, out bufferLength);
		}

		// Token: 0x060026CC RID: 9932 RVA: 0x00091A1F File Offset: 0x0008FC1F
		private void CompressVoiceChunk(int clientIndex, byte[] voiceBuffer, ref byte[] compressedBuffer, out int compressedBufferLength)
		{
			SoundManager.CompressData(clientIndex, voiceBuffer, 1440, compressedBuffer, out compressedBufferLength);
		}

		// Token: 0x060026CD RID: 9933 RVA: 0x00091A34 File Offset: 0x0008FC34
		private VoiceChatHandler.PeerVoiceData GetPlayerVoiceData(MissionPeer missionPeer)
		{
			for (int i = 0; i < this._playerVoiceDataList.Count; i++)
			{
				if (this._playerVoiceDataList[i].Peer == missionPeer)
				{
					return this._playerVoiceDataList[i];
				}
			}
			return null;
		}

		// Token: 0x060026CE RID: 9934 RVA: 0x00091A7C File Offset: 0x0008FC7C
		private void AddPlayerToVoiceChat(MissionPeer missionPeer)
		{
			VirtualPlayer peer = missionPeer.Peer;
			this._playerVoiceDataList.Add(new VoiceChatHandler.PeerVoiceData(missionPeer));
			SoundManager.CreateVoiceEvent();
			PlatformServices.Instance.CheckPermissionWithUser(Permission.CommunicateUsingVoice, missionPeer.Peer.Id, delegate(bool hasPermission)
			{
				if (Mission.Current != null && Mission.Current.CurrentState == Mission.State.Continuing)
				{
					VoiceChatHandler.PeerVoiceData playerVoiceData = this.GetPlayerVoiceData(missionPeer);
					if (playerVoiceData != null)
					{
						if (!hasPermission)
						{
							PlayerIdProvidedTypes providedType = missionPeer.Peer.Id.ProvidedType;
							LobbyClient gameClient = NetworkMain.GameClient;
							PlayerIdProvidedTypes? playerIdProvidedTypes = ((gameClient != null) ? new PlayerIdProvidedTypes?(gameClient.PlayerID.ProvidedType) : null);
							if ((providedType == playerIdProvidedTypes.GetValueOrDefault()) & (playerIdProvidedTypes != null))
							{
								missionPeer.SetMutedFromPlatform(true);
							}
						}
						playerVoiceData.SetReadyOnPlatform();
					}
				}
			});
			missionPeer.SetMuted(PermaMuteList.IsPlayerMuted(missionPeer.Peer.Id));
			SoundManager.AddSoundClientWithId((ulong)((long)peer.Index));
			Action<MissionPeer> onPeerMuteStatusUpdated = this.OnPeerMuteStatusUpdated;
			if (onPeerMuteStatusUpdated == null)
			{
				return;
			}
			onPeerMuteStatusUpdated(missionPeer);
		}

		// Token: 0x060026CF RID: 9935 RVA: 0x00091B2C File Offset: 0x0008FD2C
		private void RemovePlayerFromVoiceChat(int indexInVoiceDataList)
		{
			VirtualPlayer peer = this._playerVoiceDataList[indexInVoiceDataList].Peer.Peer;
			SoundManager.DeleteSoundClientWithId((ulong)((long)this._playerVoiceDataList[indexInVoiceDataList].Peer.Peer.Index));
			SoundManager.DestroyVoiceEvent(indexInVoiceDataList);
			this._playerVoiceDataList.RemoveAt(indexInVoiceDataList);
		}

		// Token: 0x060026D0 RID: 9936 RVA: 0x00091B83 File Offset: 0x0008FD83
		private void MissionPeerOnTeamChanged(NetworkCommunicator peer, Team previousTeam, Team newTeam)
		{
			if (this._localUserInitialized && peer.VirtualPlayer.Id != PlayerId.Empty)
			{
				this.CheckPlayerForVoiceChatOnTeamChange(peer, previousTeam, newTeam);
			}
		}

		// Token: 0x060026D1 RID: 9937 RVA: 0x00091BB0 File Offset: 0x0008FDB0
		private void OnPlayerSynchronized(NetworkCommunicator networkPeer)
		{
			if (this._localUserInitialized)
			{
				MissionPeer component = networkPeer.GetComponent<MissionPeer>();
				if (!component.IsMine && component.Team != null)
				{
					this.CheckPlayerForVoiceChatOnTeamChange(networkPeer, null, component.Team);
					return;
				}
			}
			else if (networkPeer.IsMine)
			{
				NetworkCommunicator myPeer = GameNetwork.MyPeer;
				MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
				this.CheckPlayerForVoiceChatOnTeamChange(GameNetwork.MyPeer, null, missionPeer.Team);
			}
		}

		// Token: 0x060026D2 RID: 9938 RVA: 0x00091C18 File Offset: 0x0008FE18
		private void CheckPlayerForVoiceChatOnTeamChange(NetworkCommunicator peer, Team previousTeam, Team newTeam)
		{
			if (MBNetwork.VirtualPlayers[peer.Index] == peer.VirtualPlayer)
			{
				NetworkCommunicator myPeer = GameNetwork.MyPeer;
				MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
				if (missionPeer != null)
				{
					MissionPeer component = peer.GetComponent<MissionPeer>();
					if (missionPeer == component)
					{
						this._localUserInitialized = true;
						for (int i = this._playerVoiceDataList.Count - 1; i >= 0; i--)
						{
							this.RemovePlayerFromVoiceChat(i);
						}
						if (newTeam == null)
						{
							return;
						}
						using (IEnumerator<NetworkCommunicator> enumerator = GameNetwork.NetworkPeers.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								NetworkCommunicator networkCommunicator = enumerator.Current;
								MissionPeer component2 = networkCommunicator.GetComponent<MissionPeer>();
								if (missionPeer != component2 && ((component2 != null) ? component2.Team : null) != null && component2.Team == newTeam && networkCommunicator.VirtualPlayer.Id != PlayerId.Empty)
								{
									this.AddPlayerToVoiceChat(component2);
								}
							}
							return;
						}
					}
					if (this._localUserInitialized && missionPeer.Team != null)
					{
						if (missionPeer.Team == previousTeam)
						{
							for (int j = 0; j < this._playerVoiceDataList.Count; j++)
							{
								if (this._playerVoiceDataList[j].Peer == component)
								{
									this.RemovePlayerFromVoiceChat(j);
									return;
								}
							}
							return;
						}
						if (missionPeer.Team == newTeam)
						{
							this.AddPlayerToVoiceChat(component);
						}
					}
				}
			}
		}

		// Token: 0x060026D3 RID: 9939 RVA: 0x00091D74 File Offset: 0x0008FF74
		private void UpdateVoiceChatEnabled()
		{
			float num = 1f;
			this._isVoiceChatDisabled = !BannerlordConfig.EnableVoiceChat || num <= 1E-05f || Game.Current.GetGameHandler<ChatBox>().IsContentRestricted;
		}

		// Token: 0x060026D4 RID: 9940 RVA: 0x00091DAE File Offset: 0x0008FFAE
		private void OnNativeOptionChanged(NativeOptions.NativeOptionsType changedNativeOptionsType)
		{
			if (changedNativeOptionsType == NativeOptions.NativeOptionsType.VoiceChatVolume)
			{
				this.UpdateVoiceChatEnabled();
			}
		}

		// Token: 0x060026D5 RID: 9941 RVA: 0x00091DBA File Offset: 0x0008FFBA
		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType changedManagedOptionType)
		{
			if (changedManagedOptionType == ManagedOptions.ManagedOptionsType.EnableVoiceChat)
			{
				this.UpdateVoiceChatEnabled();
			}
		}

		// Token: 0x060026D6 RID: 9942 RVA: 0x00091DC8 File Offset: 0x0008FFC8
		public override void OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer)
		{
			base.OnPlayerDisconnectedFromServer(networkPeer);
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			if (((component != null) ? component.Team : null) != null && ((missionPeer != null) ? missionPeer.Team : null) != null && component.Team == missionPeer.Team)
			{
				for (int i = 0; i < this._playerVoiceDataList.Count; i++)
				{
					if (this._playerVoiceDataList[i].Peer == component)
					{
						this.RemovePlayerFromVoiceChat(i);
						return;
					}
				}
			}
		}

		// Token: 0x04000E57 RID: 3671
		private const int MillisecondsToShorts = 12;

		// Token: 0x04000E58 RID: 3672
		private const int MillisecondsToBytes = 24;

		// Token: 0x04000E59 RID: 3673
		private const int OpusFrameSizeCoefficient = 6;

		// Token: 0x04000E5A RID: 3674
		private const int VoiceFrameRawSizeInMilliseconds = 60;

		// Token: 0x04000E5B RID: 3675
		public const int VoiceFrameRawSizeInBytes = 1440;

		// Token: 0x04000E5C RID: 3676
		private const int CompressionMaxChunkSizeInBytes = 8640;

		// Token: 0x04000E5D RID: 3677
		private const int VoiceRecordMaxChunkSizeInBytes = 72000;

		// Token: 0x04000E62 RID: 3682
		private List<VoiceChatHandler.PeerVoiceData> _playerVoiceDataList;

		// Token: 0x04000E63 RID: 3683
		private bool _isVoiceChatDisabled = true;

		// Token: 0x04000E64 RID: 3684
		private bool _isVoiceRecordActive;

		// Token: 0x04000E65 RID: 3685
		private bool _stopRecordingOnNextTick;

		// Token: 0x04000E66 RID: 3686
		private Queue<byte> _voiceToSend;

		// Token: 0x04000E67 RID: 3687
		private bool _playedAnyVoicePreviousTick;

		// Token: 0x04000E68 RID: 3688
		private bool _localUserInitialized;

		// Token: 0x020005D4 RID: 1492
		private class PeerVoiceData
		{
			// Token: 0x170009B5 RID: 2485
			// (get) Token: 0x06003C13 RID: 15379 RVA: 0x000F0B47 File Offset: 0x000EED47
			// (set) Token: 0x06003C14 RID: 15380 RVA: 0x000F0B4F File Offset: 0x000EED4F
			public bool IsReadyOnPlatform { get; private set; }

			// Token: 0x06003C15 RID: 15381 RVA: 0x000F0B58 File Offset: 0x000EED58
			public PeerVoiceData(MissionPeer peer)
			{
				this.Peer = peer;
				this._voiceData = new Queue<short>();
				this._voiceToPlayInTick = new Queue<short>();
				this._nextPlayDelayResetTime = MissionTime.Now;
			}

			// Token: 0x06003C16 RID: 15382 RVA: 0x000F0B88 File Offset: 0x000EED88
			public void WriteVoiceData(byte[] dataBuffer, int bufferSize)
			{
				if (this._voiceData.Count == 0 && this._nextPlayDelayResetTime.IsPast)
				{
					this._playDelayRemainingSizeInBytes = 3600;
				}
				for (int i = 0; i < bufferSize; i += 2)
				{
					short num = (short)((int)dataBuffer[i] | ((int)dataBuffer[i + 1] << 8));
					this._voiceData.Enqueue(num);
				}
			}

			// Token: 0x06003C17 RID: 15383 RVA: 0x000F0BDF File Offset: 0x000EEDDF
			public void SetReadyOnPlatform()
			{
				this.IsReadyOnPlatform = true;
			}

			// Token: 0x06003C18 RID: 15384 RVA: 0x000F0BE8 File Offset: 0x000EEDE8
			public bool ProcessVoiceData()
			{
				if (this.IsReadyOnPlatform && this._voiceData.Count > 0)
				{
					bool isMutedFromGameOrPlatform = this.Peer.IsMutedFromGameOrPlatform;
					if (this._playDelayRemainingSizeInBytes > 0)
					{
						this._playDelayRemainingSizeInBytes -= 2;
					}
					else
					{
						short num = this._voiceData.Dequeue();
						this._nextPlayDelayResetTime = MissionTime.Now + MissionTime.Milliseconds(300f);
						if (!isMutedFromGameOrPlatform)
						{
							this._voiceToPlayInTick.Enqueue(num);
						}
					}
					return !isMutedFromGameOrPlatform;
				}
				return false;
			}

			// Token: 0x06003C19 RID: 15385 RVA: 0x000F0C6B File Offset: 0x000EEE6B
			public Queue<short> GetVoiceToPlayForTick()
			{
				return this._voiceToPlayInTick;
			}

			// Token: 0x06003C1A RID: 15386 RVA: 0x000F0C73 File Offset: 0x000EEE73
			public bool HasAnyVoiceData()
			{
				return this.IsReadyOnPlatform && this._voiceData.Count > 0;
			}

			// Token: 0x04001E4F RID: 7759
			private const int PlayDelaySizeInMilliseconds = 150;

			// Token: 0x04001E50 RID: 7760
			private const int PlayDelaySizeInBytes = 3600;

			// Token: 0x04001E51 RID: 7761
			private const float PlayDelayResetTimeInMilliseconds = 300f;

			// Token: 0x04001E52 RID: 7762
			public readonly MissionPeer Peer;

			// Token: 0x04001E54 RID: 7764
			private readonly Queue<short> _voiceData;

			// Token: 0x04001E55 RID: 7765
			private readonly Queue<short> _voiceToPlayInTick;

			// Token: 0x04001E56 RID: 7766
			private int _playDelayRemainingSizeInBytes;

			// Token: 0x04001E57 RID: 7767
			private MissionTime _nextPlayDelayResetTime;
		}
	}
}
