using System;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	public class MissionMultiplayerTeamDeathmatchClient : MissionMultiplayerGameModeBaseClient
	{
		public event Action<GoldGain> OnGoldGainEvent;

		public override bool IsGameModeUsingGold
		{
			get
			{
				return true;
			}
		}

		public override bool IsGameModeTactical
		{
			get
			{
				return false;
			}
		}

		public override bool IsGameModeUsingRoundCountdown
		{
			get
			{
				return true;
			}
		}

		public override MultiplayerGameType GameType
		{
			get
			{
				return MultiplayerGameType.TeamDeathmatch;
			}
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			base.MissionNetworkComponent.OnMyClientSynchronized += this.OnMyClientSynchronized;
			base.ScoreboardComponent.OnRoundPropertiesChanged += this.OnTeamScoresChanged;
		}

		public override void OnGoldAmountChangedForRepresentative(MissionRepresentativeBase representative, int goldAmount)
		{
			if (representative != null && base.MissionLobbyComponent.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Ending)
			{
				representative.UpdateGold(goldAmount);
				base.ScoreboardComponent.PlayerPropertiesChanged(representative.MissionPeer);
			}
		}

		public override void AfterStart()
		{
			base.Mission.SetMissionMode(MissionMode.Battle, true);
		}

		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.RegisterBaseHandler<SyncGoldsForSkirmish>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventUpdateGold));
				registerer.RegisterBaseHandler<GoldGain>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventTDMGoldGain));
			}
		}

		private void OnMyClientSynchronized()
		{
			this._myRepresentative = GameNetwork.MyPeer.GetComponent<TeamDeathmatchMissionRepresentative>();
		}

		private void HandleServerEventUpdateGold(GameNetworkMessage baseMessage)
		{
			SyncGoldsForSkirmish syncGoldsForSkirmish = (SyncGoldsForSkirmish)baseMessage;
			MissionRepresentativeBase component = syncGoldsForSkirmish.VirtualPlayer.GetComponent<MissionRepresentativeBase>();
			this.OnGoldAmountChangedForRepresentative(component, syncGoldsForSkirmish.GoldAmount);
		}

		private void HandleServerEventTDMGoldGain(GameNetworkMessage baseMessage)
		{
			GoldGain goldGain = (GoldGain)baseMessage;
			Action<GoldGain> onGoldGainEvent = this.OnGoldGainEvent;
			if (onGoldGainEvent == null)
			{
				return;
			}
			onGoldGainEvent(goldGain);
		}

		public override int GetGoldAmount()
		{
			return this._myRepresentative.Gold;
		}

		public override void OnRemoveBehavior()
		{
			base.MissionNetworkComponent.OnMyClientSynchronized -= this.OnMyClientSynchronized;
			base.ScoreboardComponent.OnRoundPropertiesChanged -= this.OnTeamScoresChanged;
			base.OnRemoveBehavior();
		}

		private void OnTeamScoresChanged()
		{
			if (!GameNetwork.IsDedicatedServer && !this._battleEndingNotificationGiven && this._myRepresentative.MissionPeer.Team != null && this._myRepresentative.MissionPeer.Team.Side != BattleSideEnum.None)
			{
				int intValue = MultiplayerOptions.OptionType.MinScoreToWinMatch.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				float num = (float)(intValue - base.ScoreboardComponent.GetRoundScore(this._myRepresentative.MissionPeer.Team.Side)) / (float)intValue;
				float num2 = (float)(intValue - base.ScoreboardComponent.GetRoundScore(this._myRepresentative.MissionPeer.Team.Side.GetOppositeSide())) / (float)intValue;
				MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
				Vec3 vec = cameraFrame.origin + cameraFrame.rotation.u;
				if (num <= 0.1f && num2 > 0.1f)
				{
					MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/battle_winning"), vec);
					this._battleEndingNotificationGiven = true;
				}
				if (num2 <= 0.1f && num > 0.1f)
				{
					MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/battle_losing"), vec);
					this._battleEndingNotificationGiven = true;
				}
			}
		}

		private const string BattleWinningSoundEventString = "event:/alerts/report/battle_winning";

		private const string BattleLosingSoundEventString = "event:/alerts/report/battle_losing";

		private const float BattleWinLoseAlertThreshold = 0.1f;

		private TeamDeathmatchMissionRepresentative _myRepresentative;

		private bool _battleEndingNotificationGiven;
	}
}
