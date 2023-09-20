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
	public class VoiceChatHandler : MissionNetwork
	{
		public event Action OnVoiceRecordStarted;

		public event Action OnVoiceRecordStopped;

		public event Action<MissionPeer, bool> OnPeerVoiceStatusUpdated;

		public event Action<MissionPeer> OnPeerMuteStatusUpdated;

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

		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.RegisterBaseHandler<SendVoiceToPlay>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSendVoiceToPlay));
				return;
			}
			if (GameNetwork.IsServer)
			{
				registerer.RegisterBaseHandler<SendVoiceRecord>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventSendVoiceRecord));
			}
		}

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

		public override void OnPreDisplayMissionTick(float dt)
		{
			if (!GameNetwork.IsDedicatedServer && !this._isVoiceChatDisabled)
			{
				this.VoiceTick(dt);
			}
		}

		private bool HandleClientEventSendVoiceRecord(NetworkCommunicator peer, GameNetworkMessage baseMessage)
		{
			SendVoiceRecord sendVoiceRecord = (SendVoiceRecord)baseMessage;
			MissionPeer component = peer.GetComponent<MissionPeer>();
			if (sendVoiceRecord.BufferLength > 0 && component.Team != null)
			{
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					MissionPeer component2 = networkCommunicator.GetComponent<MissionPeer>();
					if (networkCommunicator.IsSynchronized && component2 != null && component2.Team == component.Team && (sendVoiceRecord.ReceiverList == null || sendVoiceRecord.ReceiverList.Contains(networkCommunicator.VirtualPlayer)) && component2 != component)
					{
						GameNetwork.BeginModuleEventAsServerUnreliable(component2.Peer);
						GameNetwork.WriteMessage(new SendVoiceToPlay(peer, sendVoiceRecord.Buffer, sendVoiceRecord.BufferLength));
						GameNetwork.EndModuleEventAsServerUnreliable();
					}
				}
			}
			return true;
		}

		private void HandleServerEventSendVoiceToPlay(GameNetworkMessage baseMessage)
		{
			SendVoiceToPlay sendVoiceToPlay = (SendVoiceToPlay)baseMessage;
			if (!this._isVoiceChatDisabled)
			{
				MissionPeer component = sendVoiceToPlay.Peer.GetComponent<MissionPeer>();
				if (component != null && sendVoiceToPlay.BufferLength > 0 && !component.IsMutedFromGameOrPlatform)
				{
					for (int i = 0; i < this._playerVoiceDataList.Count; i++)
					{
						if (this._playerVoiceDataList[i].Peer == component)
						{
							byte[] array = new byte[8640];
							int num;
							this.DecompressVoiceChunk(sendVoiceToPlay.Peer.Index, sendVoiceToPlay.Buffer, sendVoiceToPlay.BufferLength, ref array, out num);
							this._playerVoiceDataList[i].WriteVoiceData(array, num);
							return;
						}
					}
				}
			}
		}

		private void CheckStopVoiceRecord()
		{
			if (this._stopRecordingOnNextTick)
			{
				this.IsVoiceRecordActive = false;
				this._stopRecordingOnNextTick = false;
			}
		}

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

		private void DecompressVoiceChunk(int clientID, byte[] compressedVoiceBuffer, int compressedBufferLength, ref byte[] voiceBuffer, out int bufferLength)
		{
			SoundManager.DecompressData(clientID, compressedVoiceBuffer, compressedBufferLength, voiceBuffer, out bufferLength);
		}

		private void CompressVoiceChunk(int clientIndex, byte[] voiceBuffer, ref byte[] compressedBuffer, out int compressedBufferLength)
		{
			SoundManager.CompressData(clientIndex, voiceBuffer, 1440, compressedBuffer, out compressedBufferLength);
		}

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

		private void RemovePlayerFromVoiceChat(int indexInVoiceDataList)
		{
			VirtualPlayer peer = this._playerVoiceDataList[indexInVoiceDataList].Peer.Peer;
			SoundManager.DeleteSoundClientWithId((ulong)((long)this._playerVoiceDataList[indexInVoiceDataList].Peer.Peer.Index));
			SoundManager.DestroyVoiceEvent(indexInVoiceDataList);
			this._playerVoiceDataList.RemoveAt(indexInVoiceDataList);
		}

		private void MissionPeerOnTeamChanged(NetworkCommunicator peer, Team previousTeam, Team newTeam)
		{
			if (this._localUserInitialized && peer.VirtualPlayer.Id != PlayerId.Empty)
			{
				this.CheckPlayerForVoiceChatOnTeamChange(peer, previousTeam, newTeam);
			}
		}

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

		private void UpdateVoiceChatEnabled()
		{
			float num = 1f;
			this._isVoiceChatDisabled = !BannerlordConfig.EnableVoiceChat || num <= 1E-05f || Game.Current.GetGameHandler<ChatBox>().IsContentRestricted;
		}

		private void OnNativeOptionChanged(NativeOptions.NativeOptionsType changedNativeOptionsType)
		{
			if (changedNativeOptionsType == NativeOptions.NativeOptionsType.VoiceChatVolume)
			{
				this.UpdateVoiceChatEnabled();
			}
		}

		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType changedManagedOptionType)
		{
			if (changedManagedOptionType == ManagedOptions.ManagedOptionsType.EnableVoiceChat)
			{
				this.UpdateVoiceChatEnabled();
			}
		}

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

		private const int MillisecondsToShorts = 12;

		private const int MillisecondsToBytes = 24;

		private const int OpusFrameSizeCoefficient = 6;

		private const int VoiceFrameRawSizeInMilliseconds = 60;

		public const int VoiceFrameRawSizeInBytes = 1440;

		private const int CompressionMaxChunkSizeInBytes = 8640;

		private const int VoiceRecordMaxChunkSizeInBytes = 72000;

		private List<VoiceChatHandler.PeerVoiceData> _playerVoiceDataList;

		private bool _isVoiceChatDisabled = true;

		private bool _isVoiceRecordActive;

		private bool _stopRecordingOnNextTick;

		private Queue<byte> _voiceToSend;

		private bool _playedAnyVoicePreviousTick;

		private bool _localUserInitialized;

		private class PeerVoiceData
		{
			public bool IsReadyOnPlatform { get; private set; }

			public PeerVoiceData(MissionPeer peer)
			{
				this.Peer = peer;
				this._voiceData = new Queue<short>();
				this._voiceToPlayInTick = new Queue<short>();
				this._nextPlayDelayResetTime = MissionTime.Now;
			}

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

			public void SetReadyOnPlatform()
			{
				this.IsReadyOnPlatform = true;
			}

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

			public Queue<short> GetVoiceToPlayForTick()
			{
				return this._voiceToPlayInTick;
			}

			public bool HasAnyVoiceData()
			{
				return this.IsReadyOnPlatform && this._voiceData.Count > 0;
			}

			private const int PlayDelaySizeInMilliseconds = 150;

			private const int PlayDelaySizeInBytes = 3600;

			private const float PlayDelayResetTimeInMilliseconds = 300f;

			public readonly MissionPeer Peer;

			private readonly Queue<short> _voiceData;

			private readonly Queue<short> _voiceToPlayInTick;

			private int _playDelayRemainingSizeInBytes;

			private MissionTime _nextPlayDelayResetTime;
		}
	}
}
