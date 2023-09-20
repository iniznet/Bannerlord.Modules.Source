using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.HUDExtensions;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.KillFeed;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection
{
	public class MultiplayerDuelVM : ViewModel
	{
		public MultiplayerDuelVM(Camera missionCamera, MissionMultiplayerGameModeDuelClient client)
		{
			this._missionCamera = missionCamera;
			this._client = client;
			MissionMultiplayerGameModeDuelClient client2 = this._client;
			client2.OnMyRepresentativeAssigned = (Action)Delegate.Combine(client2.OnMyRepresentativeAssigned, new Action(this.OnMyRepresentativeAssigned));
			this._gameMode = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			this.PlayerDuelMatch = new DuelMatchVM();
			this.OngoingDuels = new MBBindingList<DuelMatchVM>();
			this._duelArenaProperties = new List<MultiplayerDuelVM.DuelArenaProperties>();
			List<GameEntity> list = new List<GameEntity>();
			list.AddRange(Mission.Current.Scene.FindEntitiesWithTagExpression("area_flag(_\\d+)*"));
			foreach (GameEntity gameEntity in list)
			{
				MultiplayerDuelVM.DuelArenaProperties arenaPropertiesOfFlagEntity = this.GetArenaPropertiesOfFlagEntity(gameEntity);
				this._duelArenaProperties.Add(arenaPropertiesOfFlagEntity);
			}
			this.Markers = new MissionDuelMarkersVM(missionCamera, this._client);
			this.KillNotifications = new MBBindingList<MPDuelKillNotificationItemVM>();
			this._scoreWithSeparatorText = new TextObject("{=J5rb5YVV}/ {SCORE}", null);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PlayerDuelMatch.RefreshValues();
			this.Markers.RefreshValues();
		}

		private void OnMyRepresentativeAssigned()
		{
			DuelMissionRepresentative myRepresentative = this._client.MyRepresentative;
			myRepresentative.OnDuelPrepStartedEvent = (Action<MissionPeer, int>)Delegate.Combine(myRepresentative.OnDuelPrepStartedEvent, new Action<MissionPeer, int>(this.OnDuelPrepStarted));
			DuelMissionRepresentative myRepresentative2 = this._client.MyRepresentative;
			myRepresentative2.OnAgentSpawnedWithoutDuelEvent = (Action)Delegate.Combine(myRepresentative2.OnAgentSpawnedWithoutDuelEvent, new Action(this.OnAgentSpawnedWithoutDuel));
			DuelMissionRepresentative myRepresentative3 = this._client.MyRepresentative;
			myRepresentative3.OnDuelPreparationStartedForTheFirstTimeEvent = (Action<MissionPeer, MissionPeer, int>)Delegate.Combine(myRepresentative3.OnDuelPreparationStartedForTheFirstTimeEvent, new Action<MissionPeer, MissionPeer, int>(this.OnDuelStarted));
			DuelMissionRepresentative myRepresentative4 = this._client.MyRepresentative;
			myRepresentative4.OnDuelEndedEvent = (Action<MissionPeer>)Delegate.Combine(myRepresentative4.OnDuelEndedEvent, new Action<MissionPeer>(this.OnDuelEnded));
			DuelMissionRepresentative myRepresentative5 = this._client.MyRepresentative;
			myRepresentative5.OnDuelRoundEndedEvent = (Action<MissionPeer>)Delegate.Combine(myRepresentative5.OnDuelRoundEndedEvent, new Action<MissionPeer>(this.OnDuelRoundEnded));
			DuelMissionRepresentative myRepresentative6 = this._client.MyRepresentative;
			myRepresentative6.OnMyPreferredZoneChanged = (Action<TroopType>)Delegate.Combine(myRepresentative6.OnMyPreferredZoneChanged, new Action<TroopType>(this.OnPlayerPreferredZoneChanged));
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			this.Markers.RegisterEvents();
			this.UpdatePlayerScore();
			this._isMyRepresentativeAssigned = true;
		}

		public void Tick(float dt)
		{
			int num;
			int num2;
			if (this._gameMode.CheckTimer(ref num, ref num2, false))
			{
				this.RemainingRoundTime = TimeSpan.FromSeconds((double)num).ToString("mm':'ss");
			}
			this.Markers.Tick(dt);
			if (this.PlayerDuelMatch.IsEnabled)
			{
				this.PlayerDuelMatch.Tick(dt);
			}
		}

		[Conditional("DEBUG")]
		private void DebugTick()
		{
			if (Input.IsKeyReleased(81))
			{
				this._showSpawnPoints = !this._showSpawnPoints;
			}
			if (this._showSpawnPoints)
			{
				string text = "spawnpoint_area(_\\d+)*";
				foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTagExpression(text))
				{
					Vec3 vec;
					vec..ctor(gameEntity.GlobalPosition.x, gameEntity.GlobalPosition.y, gameEntity.GlobalPosition.z, -1f);
					Vec3 vec2 = this._missionCamera.WorldPointToViewPortPoint(ref vec);
					vec2.y = 1f - vec2.y;
					if (vec2.z < 0f)
					{
						vec2.x = 1f - vec2.x;
						vec2.y = 1f - vec2.y;
						vec2.z = 0f;
						float num = 0f;
						num = ((vec2.x > num) ? vec2.x : num);
						num = ((vec2.y > num) ? vec2.y : num);
						num = ((vec2.z > num) ? vec2.z : num);
						vec2 /= num;
					}
					if (float.IsPositiveInfinity(vec2.x))
					{
						vec2.x = 1f;
					}
					else if (float.IsNegativeInfinity(vec2.x))
					{
						vec2.x = 0f;
					}
					if (float.IsPositiveInfinity(vec2.y))
					{
						vec2.y = 1f;
					}
					else if (float.IsNegativeInfinity(vec2.y))
					{
						vec2.y = 0f;
					}
					vec2.x = MathF.Clamp(vec2.x, 0f, 1f) * Screen.RealScreenResolutionWidth;
					vec2.y = MathF.Clamp(vec2.y, 0f, 1f) * Screen.RealScreenResolutionHeight;
				}
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			MissionMultiplayerGameModeDuelClient client = this._client;
			client.OnMyRepresentativeAssigned = (Action)Delegate.Remove(client.OnMyRepresentativeAssigned, new Action(this.OnMyRepresentativeAssigned));
			if (this._isMyRepresentativeAssigned)
			{
				DuelMissionRepresentative myRepresentative = this._client.MyRepresentative;
				myRepresentative.OnDuelPrepStartedEvent = (Action<MissionPeer, int>)Delegate.Remove(myRepresentative.OnDuelPrepStartedEvent, new Action<MissionPeer, int>(this.OnDuelPrepStarted));
				DuelMissionRepresentative myRepresentative2 = this._client.MyRepresentative;
				myRepresentative2.OnAgentSpawnedWithoutDuelEvent = (Action)Delegate.Remove(myRepresentative2.OnAgentSpawnedWithoutDuelEvent, new Action(this.OnAgentSpawnedWithoutDuel));
				DuelMissionRepresentative myRepresentative3 = this._client.MyRepresentative;
				myRepresentative3.OnDuelPreparationStartedForTheFirstTimeEvent = (Action<MissionPeer, MissionPeer, int>)Delegate.Remove(myRepresentative3.OnDuelPreparationStartedForTheFirstTimeEvent, new Action<MissionPeer, MissionPeer, int>(this.OnDuelStarted));
				DuelMissionRepresentative myRepresentative4 = this._client.MyRepresentative;
				myRepresentative4.OnDuelEndedEvent = (Action<MissionPeer>)Delegate.Remove(myRepresentative4.OnDuelEndedEvent, new Action<MissionPeer>(this.OnDuelEnded));
				DuelMissionRepresentative myRepresentative5 = this._client.MyRepresentative;
				myRepresentative5.OnDuelRoundEndedEvent = (Action<MissionPeer>)Delegate.Remove(myRepresentative5.OnDuelRoundEndedEvent, new Action<MissionPeer>(this.OnDuelRoundEnded));
				DuelMissionRepresentative myRepresentative6 = this._client.MyRepresentative;
				myRepresentative6.OnMyPreferredZoneChanged = (Action<TroopType>)Delegate.Remove(myRepresentative6.OnMyPreferredZoneChanged, new Action<TroopType>(this.OnPlayerPreferredZoneChanged));
				ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
				this.Markers.UnregisterEvents();
			}
		}

		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			if (changedManagedOptionsType == 32)
			{
				this._ongoingDuels.ApplyActionOnAllItems(delegate(DuelMatchVM d)
				{
					d.RefreshNames(true);
				});
			}
		}

		private void OnDuelPrepStarted(MissionPeer opponentPeer, int duelStartTime)
		{
			this.PlayerDuelMatch.OnDuelPrepStarted(opponentPeer, duelStartTime);
			this.AreOngoingDuelsActive = false;
			this.Markers.IsEnabled = false;
		}

		private void OnAgentSpawnedWithoutDuel()
		{
			this.Markers.OnAgentSpawnedWithoutDuel();
			this.AreOngoingDuelsActive = true;
		}

		private void OnPlayerPreferredZoneChanged(TroopType zoneType)
		{
			if (zoneType != this.PlayerPrefferedArenaType)
			{
				this.PlayerPrefferedArenaType = zoneType;
				this.Markers.OnPlayerPreferredZoneChanged(zoneType);
				this._hasPlayerChangedArenaPreferrence = true;
				GameTexts.SetVariable("ARENA_TYPE", this.GetArenaTypeLocalizedName(zoneType));
				InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=nLdQvaRK}Arena preference updated to {ARENA_TYPE}.", null).ToString()));
				return;
			}
			InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=YLZV7dxI}This arena type is already the preferred one.", null).ToString()));
		}

		private void OnDuelStarted(MissionPeer firstPeer, MissionPeer secondPeer, int flagIndex)
		{
			this.Markers.OnDuelStarted(firstPeer, secondPeer);
			MultiplayerDuelVM.DuelArenaProperties duelArenaProperties = this._duelArenaProperties.First((MultiplayerDuelVM.DuelArenaProperties f) => f.Index == flagIndex);
			if (firstPeer == this._client.MyRepresentative.MissionPeer || secondPeer == this._client.MyRepresentative.MissionPeer)
			{
				this.AreOngoingDuelsActive = false;
				this.IsPlayerInDuel = true;
				this.PlayerDuelMatch.OnDuelStarted(firstPeer, secondPeer, duelArenaProperties.ArenaTroopType);
				return;
			}
			DuelMatchVM duelMatchVM = new DuelMatchVM();
			duelMatchVM.OnDuelStarted(firstPeer, secondPeer, duelArenaProperties.ArenaTroopType);
			this.OngoingDuels.Add(duelMatchVM);
		}

		private void OnDuelEnded(MissionPeer winnerPeer)
		{
			if (this.PlayerDuelMatch.FirstPlayerPeer == winnerPeer || this.PlayerDuelMatch.SecondPlayerPeer == winnerPeer)
			{
				this.AreOngoingDuelsActive = true;
				this.IsPlayerInDuel = false;
				this.Markers.IsEnabled = true;
				this.Markers.SetMarkerOfPeerEnabled(this.PlayerDuelMatch.FirstPlayerPeer, true);
				this.Markers.SetMarkerOfPeerEnabled(this.PlayerDuelMatch.SecondPlayerPeer, true);
				this.PlayerDuelMatch.OnDuelEnded();
				this.PlayerBounty = this._client.MyRepresentative.Bounty;
				this.UpdatePlayerScore();
			}
			DuelMatchVM duelMatchVM = this.OngoingDuels.FirstOrDefault((DuelMatchVM d) => d.FirstPlayerPeer == winnerPeer || d.SecondPlayerPeer == winnerPeer);
			if (duelMatchVM != null)
			{
				this.Markers.SetMarkerOfPeerEnabled(duelMatchVM.FirstPlayerPeer, true);
				this.Markers.SetMarkerOfPeerEnabled(duelMatchVM.SecondPlayerPeer, true);
				this.OngoingDuels.Remove(duelMatchVM);
			}
		}

		private void OnDuelRoundEnded(MissionPeer winnerPeer)
		{
			if (this.PlayerDuelMatch.FirstPlayerPeer == winnerPeer || this.PlayerDuelMatch.SecondPlayerPeer == winnerPeer)
			{
				this.PlayerDuelMatch.OnPeerScored(winnerPeer);
				this.KillNotifications.Add(new MPDuelKillNotificationItemVM(this.PlayerDuelMatch.FirstPlayerPeer, this.PlayerDuelMatch.SecondPlayerPeer, this.PlayerDuelMatch.FirstPlayerScore, this.PlayerDuelMatch.SecondPlayerScore, this.PlayerDuelMatch.ArenaType, new Action<MPDuelKillNotificationItemVM>(this.RemoveKillNotification)));
				return;
			}
			DuelMatchVM duelMatchVM = this.OngoingDuels.FirstOrDefault((DuelMatchVM d) => d.FirstPlayerPeer == winnerPeer || d.SecondPlayerPeer == winnerPeer);
			if (duelMatchVM != null)
			{
				duelMatchVM.OnPeerScored(winnerPeer);
				this.KillNotifications.Add(new MPDuelKillNotificationItemVM(duelMatchVM.FirstPlayerPeer, duelMatchVM.SecondPlayerPeer, duelMatchVM.FirstPlayerScore, duelMatchVM.SecondPlayerScore, duelMatchVM.ArenaType, new Action<MPDuelKillNotificationItemVM>(this.RemoveKillNotification)));
			}
		}

		private void UpdatePlayerScore()
		{
			GameTexts.SetVariable("SCORE", this._client.MyRepresentative.Score);
			this.PlayerScoreText = this._scoreWithSeparatorText.ToString();
		}

		private void RemoveKillNotification(MPDuelKillNotificationItemVM item)
		{
			this.KillNotifications.Remove(item);
		}

		public void OnScreenResolutionChanged()
		{
			this.Markers.UpdateScreenCenter();
		}

		public void OnMainAgentRemoved()
		{
			if (!this.PlayerDuelMatch.IsEnabled)
			{
				this.Markers.IsEnabled = false;
				this.AreOngoingDuelsActive = false;
			}
		}

		public void OnMainAgentBuild()
		{
			if (!this.PlayerDuelMatch.IsEnabled)
			{
				this.Markers.IsEnabled = true;
				this.AreOngoingDuelsActive = true;
			}
			string stringId = MultiplayerClassDivisions.GetMPHeroClassForPeer(this._client.MyRepresentative.MissionPeer, false).StringId;
			if (this._isAgentBuiltForTheFirstTime || (stringId != this._cachedPlayerClassID && !this._hasPlayerChangedArenaPreferrence))
			{
				this.PlayerPrefferedArenaType = MultiplayerDuelVM.GetAgentDefaultPreferredArenaType(Agent.Main);
				this.Markers.OnAgentBuiltForTheFirstTime();
				this._isAgentBuiltForTheFirstTime = false;
				this._cachedPlayerClassID = stringId;
			}
		}

		private string GetArenaTypeName(TroopType duelArenaType)
		{
			switch (duelArenaType)
			{
			case 0:
				return "infantry";
			case 1:
				return "archery";
			case 2:
				return "cavalry";
			default:
				Debug.FailedAssert("Invalid duel arena type!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\MultiplayerDuelVM.cs", "GetArenaTypeName", 363);
				return "";
			}
		}

		private TextObject GetArenaTypeLocalizedName(TroopType duelArenaType)
		{
			switch (duelArenaType)
			{
			case 0:
				return new TextObject("{=1Bm1Wk1v}Infantry", null);
			case 1:
				return new TextObject("{=OJbpmlXu}Ranged", null);
			case 2:
				return new TextObject("{=YVGtcLHF}Cavalry", null);
			default:
				Debug.FailedAssert("Invalid duel arena type!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\MultiplayerDuelVM.cs", "GetArenaTypeLocalizedName", 379);
				return TextObject.Empty;
			}
		}

		private MultiplayerDuelVM.DuelArenaProperties GetArenaPropertiesOfFlagEntity(GameEntity flagEntity)
		{
			MultiplayerDuelVM.DuelArenaProperties duelArenaProperties;
			duelArenaProperties.FlagEntity = flagEntity;
			string text = flagEntity.Tags.FirstOrDefault((string t) => t.StartsWith("area_flag"));
			if (!Extensions.IsEmpty<char>(text))
			{
				duelArenaProperties.Index = int.Parse(text.Substring(text.LastIndexOf('_') + 1)) - 1;
			}
			else
			{
				duelArenaProperties.Index = 0;
				Debug.FailedAssert("Flag has duel_area Tag Missing!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\MultiplayerDuelVM.cs", "GetArenaPropertiesOfFlagEntity", 397);
			}
			duelArenaProperties.ArenaTroopType = 0;
			for (TroopType troopType = 0; troopType < 3; troopType++)
			{
				if (flagEntity.HasTag("flag_" + this.GetArenaTypeName(troopType)))
				{
					duelArenaProperties.ArenaTroopType = troopType;
				}
			}
			return duelArenaProperties;
		}

		public static TroopType GetAgentDefaultPreferredArenaType(Agent agent)
		{
			return FormationClassExtensions.GetTroopTypeForRegularFormation(agent.Character.DefaultFormationClass);
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool AreOngoingDuelsActive
		{
			get
			{
				return this._areOngoingDuelsActive;
			}
			set
			{
				if (value != this._areOngoingDuelsActive)
				{
					this._areOngoingDuelsActive = value;
					base.OnPropertyChangedWithValue(value, "AreOngoingDuelsActive");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPlayerInDuel
		{
			get
			{
				return this._isPlayerInDuel;
			}
			set
			{
				if (value != this._isPlayerInDuel)
				{
					this._isPlayerInDuel = value;
					base.OnPropertyChangedWithValue(value, "IsPlayerInDuel");
				}
			}
		}

		[DataSourceProperty]
		public int PlayerBounty
		{
			get
			{
				return this._playerBounty;
			}
			set
			{
				if (value != this._playerBounty)
				{
					this._playerBounty = value;
					base.OnPropertyChangedWithValue(value, "PlayerBounty");
				}
			}
		}

		[DataSourceProperty]
		public int PlayerPrefferedArenaType
		{
			get
			{
				return this._playerPreferredArenaType;
			}
			set
			{
				if (value != this._playerPreferredArenaType)
				{
					this._playerPreferredArenaType = value;
					base.OnPropertyChangedWithValue(value, "PlayerPrefferedArenaType");
				}
			}
		}

		[DataSourceProperty]
		public string PlayerScoreText
		{
			get
			{
				return this._playerScoreText;
			}
			set
			{
				if (value != this._playerScoreText)
				{
					this._playerScoreText = value;
					base.OnPropertyChangedWithValue<string>(value, "PlayerScoreText");
				}
			}
		}

		[DataSourceProperty]
		public string RemainingRoundTime
		{
			get
			{
				return this._remainingRoundTime;
			}
			set
			{
				if (value != this._remainingRoundTime)
				{
					this._remainingRoundTime = value;
					base.OnPropertyChangedWithValue<string>(value, "RemainingRoundTime");
				}
			}
		}

		[DataSourceProperty]
		public MissionDuelMarkersVM Markers
		{
			get
			{
				return this._markers;
			}
			set
			{
				if (value != this._markers)
				{
					this._markers = value;
					base.OnPropertyChangedWithValue<MissionDuelMarkersVM>(value, "Markers");
				}
			}
		}

		[DataSourceProperty]
		public DuelMatchVM PlayerDuelMatch
		{
			get
			{
				return this._playerDuelMatch;
			}
			set
			{
				if (value != this._playerDuelMatch)
				{
					this._playerDuelMatch = value;
					base.OnPropertyChangedWithValue<DuelMatchVM>(value, "PlayerDuelMatch");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<DuelMatchVM> OngoingDuels
		{
			get
			{
				return this._ongoingDuels;
			}
			set
			{
				if (value != this._ongoingDuels)
				{
					this._ongoingDuels = value;
					base.OnPropertyChangedWithValue<MBBindingList<DuelMatchVM>>(value, "OngoingDuels");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPDuelKillNotificationItemVM> KillNotifications
		{
			get
			{
				return this._killNotifications;
			}
			set
			{
				if (value != this._killNotifications)
				{
					this._killNotifications = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPDuelKillNotificationItemVM>>(value, "KillNotifications");
				}
			}
		}

		private const string ArenaFlagTag = "area_flag";

		private const string AremaTypeFlagTagBase = "flag_";

		private readonly MissionMultiplayerGameModeDuelClient _client;

		private readonly MissionMultiplayerGameModeBaseClient _gameMode;

		private bool _isMyRepresentativeAssigned;

		private List<MultiplayerDuelVM.DuelArenaProperties> _duelArenaProperties;

		private TextObject _scoreWithSeparatorText;

		private bool _isAgentBuiltForTheFirstTime = true;

		private bool _hasPlayerChangedArenaPreferrence;

		private string _cachedPlayerClassID;

		private bool _showSpawnPoints;

		private Camera _missionCamera;

		private bool _isEnabled;

		private bool _areOngoingDuelsActive;

		private bool _isPlayerInDuel;

		private int _playerBounty;

		private int _playerPreferredArenaType;

		private string _playerScoreText;

		private string _remainingRoundTime;

		private MissionDuelMarkersVM _markers;

		private DuelMatchVM _playerDuelMatch;

		private MBBindingList<DuelMatchVM> _ongoingDuels;

		private MBBindingList<MPDuelKillNotificationItemVM> _killNotifications;

		public struct DuelArenaProperties
		{
			public DuelArenaProperties(GameEntity flagEntity, int index, TroopType arenaTroopType)
			{
				this.FlagEntity = flagEntity;
				this.Index = index;
				this.ArenaTroopType = arenaTroopType;
			}

			public GameEntity FlagEntity;

			public int Index;

			public TroopType ArenaTroopType;
		}
	}
}
