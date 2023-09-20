using System;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerWarmupComponent : MissionNetwork
	{
		public static float TotalWarmupDuration
		{
			get
			{
				return (float)(MultiplayerOptions.OptionType.WarmupTimeLimit.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) * 60);
			}
		}

		public event Action OnWarmupEnding;

		public event Action OnWarmupEnded;

		public bool IsInWarmup
		{
			get
			{
				return this.WarmupState != MultiplayerWarmupComponent.WarmupStates.Ended;
			}
		}

		private MultiplayerWarmupComponent.WarmupStates WarmupState
		{
			get
			{
				return this._warmupState;
			}
			set
			{
				this._warmupState = value;
				if (GameNetwork.IsServer)
				{
					this._currentStateStartTime = MissionTime.Now;
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new WarmupStateChange(this._warmupState, this._currentStateStartTime.NumberOfTicks));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				}
			}
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._gameMode = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBase>();
			this._timerComponent = base.Mission.GetMissionBehavior<MultiplayerTimerComponent>();
			this._lobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
		}

		public override void AfterStart()
		{
			base.AfterStart();
			this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
		}

		protected override void OnUdpNetworkHandlerClose()
		{
			this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
		}

		private void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
		{
			GameNetwork.NetworkMessageHandlerRegisterer networkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
			if (GameNetwork.IsClient)
			{
				networkMessageHandlerRegisterer.Register<WarmupStateChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<WarmupStateChange>(this.HandleServerEventWarmupStateChange));
			}
		}

		public bool CheckForWarmupProgressEnd()
		{
			return this._gameMode.CheckForWarmupEnd() || this._timerComponent.GetRemainingTime(false) <= 30f;
		}

		public override void OnPreDisplayMissionTick(float dt)
		{
			if (GameNetwork.IsServer)
			{
				switch (this.WarmupState)
				{
				case MultiplayerWarmupComponent.WarmupStates.WaitingForPlayers:
					this.BeginWarmup();
					return;
				case MultiplayerWarmupComponent.WarmupStates.InProgress:
					if (this.CheckForWarmupProgressEnd())
					{
						this.EndWarmupProgress();
						return;
					}
					break;
				case MultiplayerWarmupComponent.WarmupStates.Ending:
					if (this._timerComponent.CheckIfTimerPassed())
					{
						this.EndWarmup();
						return;
					}
					break;
				case MultiplayerWarmupComponent.WarmupStates.Ended:
					if (this._timerComponent.CheckIfTimerPassed())
					{
						base.Mission.RemoveMissionBehavior(this);
						return;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		private void BeginWarmup()
		{
			this.WarmupState = MultiplayerWarmupComponent.WarmupStates.InProgress;
			Mission.Current.ResetMission();
			this._gameMode.MultiplayerTeamSelectComponent.BalanceTeams();
			this._timerComponent.StartTimerAsServer(MultiplayerWarmupComponent.TotalWarmupDuration);
			this._gameMode.SpawnComponent.SpawningBehavior.Clear();
			SpawnComponent.SetWarmupSpawningBehavior();
		}

		private void EndWarmupProgress()
		{
			this.WarmupState = MultiplayerWarmupComponent.WarmupStates.Ending;
			this._timerComponent.StartTimerAsServer(30f);
			Action onWarmupEnding = this.OnWarmupEnding;
			if (onWarmupEnding == null)
			{
				return;
			}
			onWarmupEnding();
		}

		private void EndWarmup()
		{
			this.WarmupState = MultiplayerWarmupComponent.WarmupStates.Ended;
			this._timerComponent.StartTimerAsServer(3f);
			Action onWarmupEnded = this.OnWarmupEnded;
			if (onWarmupEnded != null)
			{
				onWarmupEnded();
			}
			if (!GameNetwork.IsDedicatedServer)
			{
				this.PlayBattleStartingSound();
			}
			Mission.Current.ResetMission();
			this._gameMode.MultiplayerTeamSelectComponent.BalanceTeams();
			this._gameMode.SpawnComponent.SpawningBehavior.Clear();
			SpawnComponent.SetSpawningBehaviorForCurrentGameType(this._gameMode.GetMissionType());
			if (!this.CanMatchStartAfterWarmup())
			{
				this._lobbyComponent.SetStateEndingAsServer();
			}
		}

		public bool CanMatchStartAfterWarmup()
		{
			bool[] array = new bool[2];
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
				if (((component != null) ? component.Team : null) != null && component.Team.Side != BattleSideEnum.None)
				{
					array[(int)component.Team.Side] = true;
				}
				if (array[1] && array[0])
				{
					return true;
				}
			}
			return false;
		}

		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			this.OnWarmupEnding = null;
			this.OnWarmupEnded = null;
			if (GameNetwork.IsServer && !this._gameMode.UseRoundController() && this._lobbyComponent.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Ending)
			{
				this._gameMode.SpawnComponent.SpawningBehavior.RequestStartSpawnSession();
			}
		}

		protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			if (this.IsInWarmup && !networkPeer.IsServerPeer)
			{
				GameNetwork.BeginModuleEventAsServer(networkPeer);
				GameNetwork.WriteMessage(new WarmupStateChange(this._warmupState, this._currentStateStartTime.NumberOfTicks));
				GameNetwork.EndModuleEventAsServer();
			}
		}

		private void HandleServerEventWarmupStateChange(WarmupStateChange message)
		{
			this.WarmupState = message.WarmupState;
			switch (this.WarmupState)
			{
			case MultiplayerWarmupComponent.WarmupStates.InProgress:
				this._timerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, MultiplayerWarmupComponent.TotalWarmupDuration);
				return;
			case MultiplayerWarmupComponent.WarmupStates.Ending:
			{
				this._timerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, 30f);
				Action onWarmupEnding = this.OnWarmupEnding;
				if (onWarmupEnding == null)
				{
					return;
				}
				onWarmupEnding();
				return;
			}
			case MultiplayerWarmupComponent.WarmupStates.Ended:
			{
				this._timerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, 3f);
				Action onWarmupEnded = this.OnWarmupEnded;
				if (onWarmupEnded != null)
				{
					onWarmupEnded();
				}
				this.PlayBattleStartingSound();
				return;
			}
			default:
				return;
			}
		}

		private void PlayBattleStartingSound()
		{
			MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
			Vec3 vec = cameraFrame.origin + cameraFrame.rotation.u;
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
			if (((missionPeer != null) ? missionPeer.Team : null) != null)
			{
				string text = ((missionPeer.Team.Side == BattleSideEnum.Attacker) ? MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) : MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
				MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/rally/" + text.ToLower()), vec);
				return;
			}
			MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/rally/generic"), vec);
		}

		public const int RespawnPeriodInWarmup = 3;

		public const int WarmupEndWaitTime = 30;

		private MissionMultiplayerGameModeBase _gameMode;

		private MultiplayerTimerComponent _timerComponent;

		private MissionLobbyComponent _lobbyComponent;

		private MissionTime _currentStateStartTime;

		private MultiplayerWarmupComponent.WarmupStates _warmupState;

		public enum WarmupStates
		{
			WaitingForPlayers,
			InProgress,
			Ending,
			Ended
		}
	}
}
