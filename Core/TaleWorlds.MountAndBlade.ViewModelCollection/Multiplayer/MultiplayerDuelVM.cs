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
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.HUDExtensions;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x02000042 RID: 66
	public class MultiplayerDuelVM : ViewModel
	{
		// Token: 0x06000581 RID: 1409 RVA: 0x00017774 File Offset: 0x00015974
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

		// Token: 0x06000582 RID: 1410 RVA: 0x00017894 File Offset: 0x00015A94
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PlayerDuelMatch.RefreshValues();
			this.Markers.RefreshValues();
		}

		// Token: 0x06000583 RID: 1411 RVA: 0x000178B4 File Offset: 0x00015AB4
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
			this.Markers.RegisterEvents();
			this.UpdatePlayerScore();
			this._isMyRepresentativeAssigned = true;
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x000179E4 File Offset: 0x00015BE4
		public void Tick(float dt)
		{
			int num;
			int num2;
			if (this._gameMode.CheckTimer(out num, out num2, false))
			{
				this.RemainingRoundTime = TimeSpan.FromSeconds((double)num).ToString("mm':'ss");
			}
			this.Markers.Tick(dt);
			if (this.PlayerDuelMatch.IsEnabled)
			{
				this.PlayerDuelMatch.Tick(dt);
			}
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x00017A44 File Offset: 0x00015C44
		[Conditional("DEBUG")]
		private void DebugTick()
		{
			if (Input.IsKeyReleased(InputKey.Numpad3))
			{
				this._showSpawnPoints = !this._showSpawnPoints;
			}
			if (this._showSpawnPoints)
			{
				string text = "spawnpoint_area(_\\d+)*";
				foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTagExpression(text))
				{
					Vec3 vec = new Vec3(gameEntity.GlobalPosition.x, gameEntity.GlobalPosition.y, gameEntity.GlobalPosition.z, -1f);
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

		// Token: 0x06000586 RID: 1414 RVA: 0x00017C70 File Offset: 0x00015E70
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
				this.Markers.UnregisterEvents();
			}
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x00017DC8 File Offset: 0x00015FC8
		private void OnDuelPrepStarted(MissionPeer opponentPeer, int duelStartTime)
		{
			this.PlayerDuelMatch.OnDuelPrepStarted(opponentPeer, duelStartTime);
			this.AreOngoingDuelsActive = false;
			this.Markers.IsEnabled = false;
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x00017DEA File Offset: 0x00015FEA
		private void OnAgentSpawnedWithoutDuel()
		{
			this.Markers.OnAgentSpawnedWithoutDuel();
			this.AreOngoingDuelsActive = true;
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x00017E00 File Offset: 0x00016000
		private void OnPlayerPreferredZoneChanged(TroopType zoneType)
		{
			if (zoneType != (TroopType)this.PlayerPrefferedArenaType)
			{
				this.PlayerPrefferedArenaType = (int)zoneType;
				this.Markers.OnPlayerPreferredZoneChanged((int)zoneType);
				this._hasPlayerChangedArenaPreferrence = true;
				GameTexts.SetVariable("ARENA_TYPE", this.GetArenaTypeLocalizedName(zoneType));
				InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=nLdQvaRK}Arena preference updated to {ARENA_TYPE}.", null).ToString()));
				return;
			}
			InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=YLZV7dxI}This arena type is already the preferred one.", null).ToString()));
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x00017E78 File Offset: 0x00016078
		private void OnDuelStarted(MissionPeer firstPeer, MissionPeer secondPeer, int flagIndex)
		{
			this.Markers.OnDuelStarted(firstPeer, secondPeer);
			MultiplayerDuelVM.DuelArenaProperties duelArenaProperties = this._duelArenaProperties.First((MultiplayerDuelVM.DuelArenaProperties f) => f.Index == flagIndex);
			if (firstPeer == this._client.MyRepresentative.MissionPeer || secondPeer == this._client.MyRepresentative.MissionPeer)
			{
				this.AreOngoingDuelsActive = false;
				this.IsPlayerInDuel = true;
				this.PlayerDuelMatch.OnDuelStarted(firstPeer, secondPeer, (int)duelArenaProperties.ArenaTroopType);
				return;
			}
			DuelMatchVM duelMatchVM = new DuelMatchVM();
			duelMatchVM.OnDuelStarted(firstPeer, secondPeer, (int)duelArenaProperties.ArenaTroopType);
			this.OngoingDuels.Add(duelMatchVM);
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x00017F20 File Offset: 0x00016120
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

		// Token: 0x0600058C RID: 1420 RVA: 0x0001801C File Offset: 0x0001621C
		private void OnDuelRoundEnded(MissionPeer winnerPeer)
		{
			if (this.PlayerDuelMatch.FirstPlayerPeer == winnerPeer || this.PlayerDuelMatch.SecondPlayerPeer == winnerPeer)
			{
				this.PlayerDuelMatch.OnPeerScored(winnerPeer);
				this.KillNotifications.Add(new MPDuelKillNotificationItemVM(this.PlayerDuelMatch.FirstPlayerPeer, this.PlayerDuelMatch.SecondPlayerPeer, this.PlayerDuelMatch.FirstPlayerScore, this.PlayerDuelMatch.SecondPlayerScore, (TroopType)this.PlayerDuelMatch.ArenaType, new Action<MPDuelKillNotificationItemVM>(this.RemoveKillNotification)));
				return;
			}
			DuelMatchVM duelMatchVM = this.OngoingDuels.FirstOrDefault((DuelMatchVM d) => d.FirstPlayerPeer == winnerPeer || d.SecondPlayerPeer == winnerPeer);
			if (duelMatchVM != null)
			{
				duelMatchVM.OnPeerScored(winnerPeer);
				this.KillNotifications.Add(new MPDuelKillNotificationItemVM(duelMatchVM.FirstPlayerPeer, duelMatchVM.SecondPlayerPeer, duelMatchVM.FirstPlayerScore, duelMatchVM.SecondPlayerScore, (TroopType)duelMatchVM.ArenaType, new Action<MPDuelKillNotificationItemVM>(this.RemoveKillNotification)));
			}
		}

		// Token: 0x0600058D RID: 1421 RVA: 0x00018122 File Offset: 0x00016322
		private void UpdatePlayerScore()
		{
			GameTexts.SetVariable("SCORE", this._client.MyRepresentative.Score);
			this.PlayerScoreText = this._scoreWithSeparatorText.ToString();
		}

		// Token: 0x0600058E RID: 1422 RVA: 0x0001814F File Offset: 0x0001634F
		private void RemoveKillNotification(MPDuelKillNotificationItemVM item)
		{
			this.KillNotifications.Remove(item);
		}

		// Token: 0x0600058F RID: 1423 RVA: 0x0001815E File Offset: 0x0001635E
		public void OnScreenResolutionChanged()
		{
			this.Markers.UpdateScreenCenter();
		}

		// Token: 0x06000590 RID: 1424 RVA: 0x0001816B File Offset: 0x0001636B
		public void OnMainAgentRemoved()
		{
			if (!this.PlayerDuelMatch.IsEnabled)
			{
				this.Markers.IsEnabled = false;
				this.AreOngoingDuelsActive = false;
			}
		}

		// Token: 0x06000591 RID: 1425 RVA: 0x00018190 File Offset: 0x00016390
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
				this.PlayerPrefferedArenaType = (int)MultiplayerDuelVM.GetAgentDefaultPreferredArenaType(Agent.Main);
				this.Markers.OnAgentBuiltForTheFirstTime();
				this._isAgentBuiltForTheFirstTime = false;
				this._cachedPlayerClassID = stringId;
			}
		}

		// Token: 0x06000592 RID: 1426 RVA: 0x00018220 File Offset: 0x00016420
		private string GetArenaTypeName(TroopType duelArenaType)
		{
			switch (duelArenaType)
			{
			case TroopType.Infantry:
				return "infantry";
			case TroopType.Ranged:
				return "archery";
			case TroopType.Cavalry:
				return "cavalry";
			default:
				Debug.FailedAssert("Invalid duel arena type!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\MultiplayerDuelVM.cs", "GetArenaTypeName", 352);
				return "";
			}
		}

		// Token: 0x06000593 RID: 1427 RVA: 0x00018274 File Offset: 0x00016474
		private TextObject GetArenaTypeLocalizedName(TroopType duelArenaType)
		{
			switch (duelArenaType)
			{
			case TroopType.Infantry:
				return new TextObject("{=1Bm1Wk1v}Infantry", null);
			case TroopType.Ranged:
				return new TextObject("{=OJbpmlXu}Ranged", null);
			case TroopType.Cavalry:
				return new TextObject("{=YVGtcLHF}Cavalry", null);
			default:
				Debug.FailedAssert("Invalid duel arena type!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\MultiplayerDuelVM.cs", "GetArenaTypeLocalizedName", 368);
				return TextObject.Empty;
			}
		}

		// Token: 0x06000594 RID: 1428 RVA: 0x000182D8 File Offset: 0x000164D8
		private MultiplayerDuelVM.DuelArenaProperties GetArenaPropertiesOfFlagEntity(GameEntity flagEntity)
		{
			MultiplayerDuelVM.DuelArenaProperties duelArenaProperties;
			duelArenaProperties.FlagEntity = flagEntity;
			string text = flagEntity.Tags.FirstOrDefault((string t) => t.StartsWith("area_flag"));
			if (!text.IsEmpty<char>())
			{
				duelArenaProperties.Index = int.Parse(text.Substring(text.LastIndexOf('_') + 1)) - 1;
			}
			else
			{
				duelArenaProperties.Index = 0;
				Debug.FailedAssert("Flag has duel_area Tag Missing!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\MultiplayerDuelVM.cs", "GetArenaPropertiesOfFlagEntity", 386);
			}
			duelArenaProperties.ArenaTroopType = TroopType.Infantry;
			for (TroopType troopType = TroopType.Infantry; troopType < TroopType.NumberOfTroopTypes; troopType++)
			{
				if (flagEntity.HasTag("flag_" + this.GetArenaTypeName(troopType)))
				{
					duelArenaProperties.ArenaTroopType = troopType;
				}
			}
			return duelArenaProperties;
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x00018397 File Offset: 0x00016597
		public static TroopType GetAgentDefaultPreferredArenaType(Agent agent)
		{
			return agent.Character.DefaultFormationClass.GetTroopTypeForRegularFormation();
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06000596 RID: 1430 RVA: 0x000183A9 File Offset: 0x000165A9
		// (set) Token: 0x06000597 RID: 1431 RVA: 0x000183B1 File Offset: 0x000165B1
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

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06000598 RID: 1432 RVA: 0x000183CF File Offset: 0x000165CF
		// (set) Token: 0x06000599 RID: 1433 RVA: 0x000183D7 File Offset: 0x000165D7
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

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x0600059A RID: 1434 RVA: 0x000183F5 File Offset: 0x000165F5
		// (set) Token: 0x0600059B RID: 1435 RVA: 0x000183FD File Offset: 0x000165FD
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

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x0600059C RID: 1436 RVA: 0x0001841B File Offset: 0x0001661B
		// (set) Token: 0x0600059D RID: 1437 RVA: 0x00018423 File Offset: 0x00016623
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

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x0600059E RID: 1438 RVA: 0x00018441 File Offset: 0x00016641
		// (set) Token: 0x0600059F RID: 1439 RVA: 0x00018449 File Offset: 0x00016649
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

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x060005A0 RID: 1440 RVA: 0x00018467 File Offset: 0x00016667
		// (set) Token: 0x060005A1 RID: 1441 RVA: 0x0001846F File Offset: 0x0001666F
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

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x060005A2 RID: 1442 RVA: 0x00018492 File Offset: 0x00016692
		// (set) Token: 0x060005A3 RID: 1443 RVA: 0x0001849A File Offset: 0x0001669A
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

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x060005A4 RID: 1444 RVA: 0x000184BD File Offset: 0x000166BD
		// (set) Token: 0x060005A5 RID: 1445 RVA: 0x000184C5 File Offset: 0x000166C5
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

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x060005A6 RID: 1446 RVA: 0x000184E3 File Offset: 0x000166E3
		// (set) Token: 0x060005A7 RID: 1447 RVA: 0x000184EB File Offset: 0x000166EB
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

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x060005A8 RID: 1448 RVA: 0x00018509 File Offset: 0x00016709
		// (set) Token: 0x060005A9 RID: 1449 RVA: 0x00018511 File Offset: 0x00016711
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

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x060005AA RID: 1450 RVA: 0x0001852F File Offset: 0x0001672F
		// (set) Token: 0x060005AB RID: 1451 RVA: 0x00018537 File Offset: 0x00016737
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

		// Token: 0x040002CE RID: 718
		private const string ArenaFlagTag = "area_flag";

		// Token: 0x040002CF RID: 719
		private const string AremaTypeFlagTagBase = "flag_";

		// Token: 0x040002D0 RID: 720
		private readonly MissionMultiplayerGameModeDuelClient _client;

		// Token: 0x040002D1 RID: 721
		private readonly MissionMultiplayerGameModeBaseClient _gameMode;

		// Token: 0x040002D2 RID: 722
		private bool _isMyRepresentativeAssigned;

		// Token: 0x040002D3 RID: 723
		private List<MultiplayerDuelVM.DuelArenaProperties> _duelArenaProperties;

		// Token: 0x040002D4 RID: 724
		private TextObject _scoreWithSeparatorText;

		// Token: 0x040002D5 RID: 725
		private bool _isAgentBuiltForTheFirstTime = true;

		// Token: 0x040002D6 RID: 726
		private bool _hasPlayerChangedArenaPreferrence;

		// Token: 0x040002D7 RID: 727
		private string _cachedPlayerClassID;

		// Token: 0x040002D8 RID: 728
		private bool _showSpawnPoints;

		// Token: 0x040002D9 RID: 729
		private Camera _missionCamera;

		// Token: 0x040002DA RID: 730
		private bool _isEnabled;

		// Token: 0x040002DB RID: 731
		private bool _areOngoingDuelsActive;

		// Token: 0x040002DC RID: 732
		private bool _isPlayerInDuel;

		// Token: 0x040002DD RID: 733
		private int _playerBounty;

		// Token: 0x040002DE RID: 734
		private int _playerPreferredArenaType;

		// Token: 0x040002DF RID: 735
		private string _playerScoreText;

		// Token: 0x040002E0 RID: 736
		private string _remainingRoundTime;

		// Token: 0x040002E1 RID: 737
		private MissionDuelMarkersVM _markers;

		// Token: 0x040002E2 RID: 738
		private DuelMatchVM _playerDuelMatch;

		// Token: 0x040002E3 RID: 739
		private MBBindingList<DuelMatchVM> _ongoingDuels;

		// Token: 0x040002E4 RID: 740
		private MBBindingList<MPDuelKillNotificationItemVM> _killNotifications;

		// Token: 0x02000162 RID: 354
		public struct DuelArenaProperties
		{
			// Token: 0x0600192E RID: 6446 RVA: 0x000511B5 File Offset: 0x0004F3B5
			public DuelArenaProperties(GameEntity flagEntity, int index, TroopType arenaTroopType)
			{
				this.FlagEntity = flagEntity;
				this.Index = index;
				this.ArenaTroopType = arenaTroopType;
			}

			// Token: 0x04000C7E RID: 3198
			public GameEntity FlagEntity;

			// Token: 0x04000C7F RID: 3199
			public int Index;

			// Token: 0x04000C80 RID: 3200
			public TroopType ArenaTroopType;
		}
	}
}
