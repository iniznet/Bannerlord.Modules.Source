using System;
using SandBox.ViewModelCollection.Missions.NameMarker;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ScreenSystem;

namespace SandBox.ViewModelCollection.Nameplate
{
	public class PartyNameplateVM : NameplateVM
	{
		public bool GetIsMainParty
		{
			get
			{
				return this._isMainPartyBind || this.IsMainParty;
			}
		}

		public MobileParty Party { get; }

		private IFaction _mainFaction
		{
			get
			{
				return Hero.MainHero.MapFaction;
			}
		}

		public PartyNameplateVM(MobileParty party, Camera mapCamera, Action resetCamera, Func<bool> getShouldShowFullName)
		{
			this._resetCamera = resetCamera;
			this._mapCamera = mapCamera;
			this._getShouldShowFullName = getShouldShowFullName;
			this.Party = party;
			this._isMainPartyBind = party.IsMainParty;
			this.MainHeroVisual = (this._isMainPartyBind ? new ImageIdentifierVM(SandBoxUIHelper.GetCharacterCode(Hero.MainHero.CharacterObject, false)) : null);
			this._isPartyHeroVisualDirty = true;
			this._isPartyBannerDirty = true;
			this.Quests = new MBBindingList<QuestMarkerVM>();
			this.RegisterEvents();
		}

		public override void OnFinalize()
		{
			this.UnregisterEvents();
			base.OnFinalize();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.RefreshDynamicProperties(true);
		}

		private void AddQuestBindFlagsForParty(MobileParty party)
		{
			if (party != MobileParty.MainParty && party != this.Party)
			{
				Hero leaderHero = party.LeaderHero;
				if (((leaderHero != null) ? leaderHero.Issue : null) != null && (this._questsBind & SandBoxUIHelper.IssueQuestFlags.TrackedIssue) == SandBoxUIHelper.IssueQuestFlags.None && ((this._questsBind & SandBoxUIHelper.IssueQuestFlags.AvailableIssue) == SandBoxUIHelper.IssueQuestFlags.None || (this._questsBind & SandBoxUIHelper.IssueQuestFlags.ActiveIssue) == SandBoxUIHelper.IssueQuestFlags.None))
				{
					this._questsBind |= SandBoxUIHelper.GetIssueType(party.LeaderHero.Issue);
				}
				if (((this._questsBind & SandBoxUIHelper.IssueQuestFlags.TrackedStoryQuest) == SandBoxUIHelper.IssueQuestFlags.None && (this._questsBind & SandBoxUIHelper.IssueQuestFlags.ActiveIssue) == SandBoxUIHelper.IssueQuestFlags.None) || (this._questsBind & SandBoxUIHelper.IssueQuestFlags.ActiveStoryQuest) == SandBoxUIHelper.IssueQuestFlags.None)
				{
					foreach (QuestBase questBase in SandBoxUIHelper.GetQuestsRelatedToParty(party))
					{
						if (party.LeaderHero != null && questBase.QuestGiver == party.LeaderHero)
						{
							if (questBase.IsSpecialQuest && (this._questsBind & SandBoxUIHelper.IssueQuestFlags.ActiveStoryQuest) == SandBoxUIHelper.IssueQuestFlags.None)
							{
								this._questsBind |= SandBoxUIHelper.IssueQuestFlags.ActiveStoryQuest;
							}
							else if (!questBase.IsSpecialQuest && (this._questsBind & SandBoxUIHelper.IssueQuestFlags.ActiveIssue) == SandBoxUIHelper.IssueQuestFlags.None)
							{
								this._questsBind |= SandBoxUIHelper.IssueQuestFlags.ActiveIssue;
							}
						}
						else if (questBase.IsSpecialQuest && (this._questsBind & SandBoxUIHelper.IssueQuestFlags.TrackedStoryQuest) == SandBoxUIHelper.IssueQuestFlags.None)
						{
							this._questsBind |= SandBoxUIHelper.IssueQuestFlags.TrackedStoryQuest;
						}
						else if (!questBase.IsSpecialQuest && (this._questsBind & SandBoxUIHelper.IssueQuestFlags.TrackedIssue) == SandBoxUIHelper.IssueQuestFlags.None)
						{
							this._questsBind |= SandBoxUIHelper.IssueQuestFlags.TrackedIssue;
						}
					}
				}
			}
		}

		public override void RefreshDynamicProperties(bool forceUpdate)
		{
			base.RefreshDynamicProperties(forceUpdate);
			if ((this.IsMainParty && MathF.Abs(Hero.MainHero.Age - this._latestMainHeroAge) >= 1f) || forceUpdate)
			{
				this._latestMainHeroAge = Hero.MainHero.Age;
				this._isPartyHeroVisualDirty = true;
			}
			if (this._isPartyHeroVisualDirty || forceUpdate)
			{
				this._mainHeroVisualBind = (this._isMainPartyBind ? new ImageIdentifierVM(SandBoxUIHelper.GetCharacterCode(Hero.MainHero.CharacterObject, false)) : null);
				this._isPartyHeroVisualDirty = false;
			}
			if (this._isVisibleOnMapBind || forceUpdate)
			{
				MobileParty party = this.Party;
				IssueBase issueBase;
				if (party == null)
				{
					issueBase = null;
				}
				else
				{
					Hero leaderHero = party.LeaderHero;
					issueBase = ((leaderHero != null) ? leaderHero.Issue : null);
				}
				IssueBase issueBase2 = issueBase;
				this._questsBind = SandBoxUIHelper.IssueQuestFlags.None;
				if (this.Party != MobileParty.MainParty)
				{
					if (issueBase2 != null)
					{
						this._questsBind |= SandBoxUIHelper.GetIssueType(issueBase2);
					}
					foreach (QuestBase questBase in SandBoxUIHelper.GetQuestsRelatedToParty(this.Party))
					{
						if (questBase.QuestGiver != null && questBase.QuestGiver == this.Party.LeaderHero)
						{
							this._questsBind |= (questBase.IsSpecialQuest ? SandBoxUIHelper.IssueQuestFlags.ActiveStoryQuest : SandBoxUIHelper.IssueQuestFlags.ActiveIssue);
						}
						else
						{
							this._questsBind |= (questBase.IsSpecialQuest ? SandBoxUIHelper.IssueQuestFlags.TrackedStoryQuest : SandBoxUIHelper.IssueQuestFlags.TrackedIssue);
						}
					}
				}
			}
			this._isInArmyBind = this.Party.Army != null && this.Party.AttachedTo != null;
			this._isArmyBind = this.Party.Army != null && this.Party.Army.LeaderParty == this.Party;
			MobileParty party2 = this.Party;
			this._isInSettlementBind = ((party2 != null) ? party2.CurrentSettlement : null) != null;
			if (this._isArmyBind && (this._isVisibleOnMapBind || forceUpdate))
			{
				this.AddQuestBindFlagsForParty(this.Party.Army.LeaderParty);
				foreach (MobileParty mobileParty in this.Party.Army.LeaderParty.AttachedParties)
				{
					this.AddQuestBindFlagsForParty(mobileParty);
				}
			}
			if (this._isArmyBind || !this._isInArmy || forceUpdate)
			{
				int partyHealthyCount = SandBoxUIHelper.GetPartyHealthyCount(this.Party);
				if (partyHealthyCount != this._latestTotalCount)
				{
					this._latestTotalCount = partyHealthyCount;
					this._countBind = partyHealthyCount.ToString();
				}
				int allWoundedMembersAmount = SandBoxUIHelper.GetAllWoundedMembersAmount(this.Party);
				int allPrisonerMembersAmount = SandBoxUIHelper.GetAllPrisonerMembersAmount(this.Party);
				if (this._latestWoundedAmount != allWoundedMembersAmount || this._latestPrisonerAmount != allPrisonerMembersAmount)
				{
					if (this._latestWoundedAmount != allWoundedMembersAmount)
					{
						this._woundedBind = ((allWoundedMembersAmount == 0) ? "" : SandBoxUIHelper.GetPartyWoundedText(allWoundedMembersAmount));
						this._latestWoundedAmount = allWoundedMembersAmount;
					}
					if (this._latestPrisonerAmount != allPrisonerMembersAmount)
					{
						this._prisonerBind = ((allPrisonerMembersAmount == 0) ? "" : SandBoxUIHelper.GetPartyPrisonerText(allPrisonerMembersAmount));
						this._latestPrisonerAmount = allPrisonerMembersAmount;
					}
					this._extraInfoTextBind = this._woundedBind + this._prisonerBind;
				}
			}
			if (!this.Party.IsMainParty)
			{
				Army army = this.Party.Army;
				if (army == null || !army.LeaderParty.AttachedParties.Contains(MobileParty.MainParty) || !this.Party.Army.LeaderParty.AttachedParties.Contains(this.Party))
				{
					if (FactionManager.IsAtWarAgainstFaction(this.Party.MapFaction, this._mainFaction))
					{
						this._factionColorBind = ((this.Party.Army != null && this.Party.Army.LeaderParty == this.Party) ? PartyNameplateVM.NegativeArmyIndicator : PartyNameplateVM.NegativeIndicator);
						goto IL_491;
					}
					if (FactionManager.IsAlliedWithFaction(this.Party.MapFaction, Hero.MainHero.MapFaction))
					{
						this._factionColorBind = ((this.Party.Army != null && this.Party.Army.LeaderParty == this.Party) ? PartyNameplateVM.PositiveArmyIndicator : PartyNameplateVM.PositiveIndicator);
						goto IL_491;
					}
					this._factionColorBind = ((this.Party.Army != null && this.Party.Army.LeaderParty == this.Party) ? PartyNameplateVM.NeutralArmyIndicator : PartyNameplateVM.NeutralIndicator);
					goto IL_491;
				}
			}
			this._factionColorBind = ((this.Party.Army != null && this.Party.Army.LeaderParty == this.Party) ? PartyNameplateVM.MainPartyArmyIndicator : PartyNameplateVM.MainPartyIndicator);
			IL_491:
			if (this._isPartyBannerDirty || forceUpdate)
			{
				if (this.Party.Party.Banner != null)
				{
					this.PartyBanner = new ImageIdentifierVM(BannerCode.CreateFrom(this.Party.Party.Banner), true);
				}
				else
				{
					IFaction mapFaction = this.Party.MapFaction;
					this.PartyBanner = new ImageIdentifierVM(BannerCode.CreateFrom((mapFaction != null) ? mapFaction.Banner : null), true);
				}
				this._isPartyBannerDirty = false;
			}
			if (this._isVisibleOnMapBind && (this._isInArmyBind || this._isInSettlementBind))
			{
				this._isVisibleOnMapBind = false;
			}
			Army army2 = this.Party.Army;
			TextObject textObject;
			if (army2 != null && army2.DoesLeaderPartyAndAttachedPartiesContain(this.Party))
			{
				textObject = this.Party.ArmyName;
			}
			else if (this.Party.LeaderHero != null)
			{
				textObject = this.Party.LeaderHero.Name;
			}
			else
			{
				textObject = this.Party.Name;
			}
			bool flag;
			if (this.IsMainParty && this.Party.LeaderHero == null)
			{
				Hero mainHero = Hero.MainHero;
				flag = mainHero != null && mainHero.IsAlive;
			}
			else
			{
				flag = false;
			}
			this._isPrisonerBind = flag;
			this._isDisorganizedBind = this.Party.IsDisorganized;
			this._shouldShowFullNameBind = this._getShouldShowFullName != null && this._getShouldShowFullName();
			if (this._latestNameTextObject != textObject || forceUpdate)
			{
				this._latestNameTextObject = textObject;
				this._fullNameBind = this._latestNameTextObject.ToString();
			}
			if (!MBMath.ApproximatelyEqualsTo(this._cachedSpeed, this.Party.Speed, 0.01f))
			{
				this._cachedSpeed = this.Party.Speed;
				this._movementSpeedTextBind = this._cachedSpeed.ToString("F1");
			}
		}

		public override void RefreshPosition()
		{
			base.RefreshPosition();
			Vec3 visualPosition = this.Party.GetVisualPosition();
			Vec3 vec = visualPosition + new Vec3(0f, 0f, 0.8f, -1f);
			if (this._isMainPartyBind)
			{
				this.RefreshMainPartyScreenPosition(visualPosition);
				MBWindowManager.WorldToScreenInsideUsableArea(this._mapCamera, vec, ref this._latestX, ref this._latestY, ref this._latestW);
				this._headPositionBind = new Vec2(this._latestX, this._latestY);
			}
			else
			{
				this._latestX = 0f;
				this._latestY = 0f;
				this._latestW = 0f;
				MBWindowManager.WorldToScreenInsideUsableArea(this._mapCamera, visualPosition, ref this._latestX, ref this._latestY, ref this._latestW);
				this._partyPositionBind = new Vec2(this._latestX, this._latestY);
				MBWindowManager.WorldToScreenInsideUsableArea(this._mapCamera, vec, ref this._latestX, ref this._latestY, ref this._latestW);
				this._headPositionBind = new Vec2(this._latestX, this._latestY);
			}
			base.DistanceToCamera = visualPosition.Distance(this._mapCamera.Position);
		}

		public override void RefreshTutorialStatus(string newTutorialHighlightElementID)
		{
			base.RefreshTutorialStatus(newTutorialHighlightElementID);
			this._bindIsTargetedByTutorial = this.Party.Party.Id == newTutorialHighlightElementID;
		}

		private void RefreshMainPartyScreenPosition(Vec3 partyWorldPosition)
		{
			this._latestX = 0f;
			this._latestY = 0f;
			this._latestW = 0f;
			MatrixFrame identity = MatrixFrame.Identity;
			this._mapCamera.GetViewProjMatrix(ref identity);
			Vec3 vec = partyWorldPosition;
			vec.w = 1f;
			Vec3 vec2 = vec * identity;
			this._isBehindBind = vec2.w < 0f;
			vec2.w = MathF.Abs(vec2.w);
			vec2.x /= vec2.w;
			vec2.y /= vec2.w;
			vec2.z /= vec2.w;
			vec2.w /= vec2.w;
			vec2 *= 0.5f;
			vec2.x += 0.5f;
			vec2.y += 0.5f;
			vec2.y = 1f - vec2.y;
			if (this._isBehindBind)
			{
				vec2.y = 1f;
			}
			int num = (int)(Screen.RealScreenResolutionWidth * ScreenManager.UsableArea.X);
			int num2 = (int)(Screen.RealScreenResolutionHeight * ScreenManager.UsableArea.Y);
			this._latestX = vec2.x * (float)num;
			this._latestY = vec2.y * (float)num2;
			this._latestW = (this.IsBehind ? (-vec2.w) : vec2.w);
			this._isHighBind = this._mapCamera.Position.Distance(vec) >= 110f;
			this._partyPositionBind = new Vec2(this._latestX, this._latestY);
		}

		public void DetermineIsVisibleOnMap()
		{
			this._isVisibleOnMapBind = this._latestW < 100f && this._latestW > 0f && this._mapCamera.Position.z < 200f;
		}

		public void OnPlayerCharacterChanged(Hero newPlayer)
		{
			if (this.IsMainParty && this.Party.LeaderHero != newPlayer)
			{
				this._isMainPartyBind = false;
				this._mainHeroVisualBind = new ImageIdentifierVM(0);
			}
			else if (this.Party.LeaderHero == newPlayer)
			{
				this._isMainPartyBind = true;
				this._mainHeroVisualBind = new ImageIdentifierVM(CharacterCode.CreateFrom(Hero.MainHero.CharacterObject));
			}
			this._isPartyHeroVisualDirty = true;
		}

		private bool IsInsideWindow()
		{
			return this._latestX <= Screen.RealScreenResolutionWidth && this._latestY <= Screen.RealScreenResolutionHeight && this._latestX + 100f >= 0f && this._latestY + 30f >= 0f;
		}

		public void RefreshBinding()
		{
			base.Position = this._partyPositionBind;
			this.HeadPosition = this._headPositionBind;
			base.IsVisibleOnMap = this._isVisibleOnMapBind;
			this.IsInSettlement = this._isInSettlementBind;
			this.IsMainParty = this._isMainPartyBind;
			base.FactionColor = this._factionColorBind;
			this.MainHeroVisual = this._mainHeroVisualBind;
			this.IsHigh = this._isHighBind;
			this.Count = this._countBind;
			this.Prisoner = this._prisonerBind;
			this.Wounded = this._woundedBind;
			this.IsBehind = this._isBehindBind;
			this.FullName = this._fullNameBind;
			base.IsTargetedByTutorial = this._bindIsTargetedByTutorial;
			this.ShouldShowFullName = this._shouldShowFullNameBind;
			this.IsInArmy = this._isInArmyBind;
			this.IsArmy = this._isArmyBind;
			this.ExtraInfoText = this._extraInfoTextBind;
			this.IsPrisoner = this._isPrisonerBind;
			this.IsDisorganized = this._isDisorganizedBind;
			this.MovementSpeedText = this._movementSpeedTextBind;
			this.Quests.Clear();
			foreach (SandBoxUIHelper.IssueQuestFlags issueQuestFlags in SandBoxUIHelper.IssueQuestFlagsValues)
			{
				if (issueQuestFlags != SandBoxUIHelper.IssueQuestFlags.None && (this._questsBind & issueQuestFlags) != SandBoxUIHelper.IssueQuestFlags.None)
				{
					this.Quests.Add(new QuestMarkerVM(issueQuestFlags));
				}
			}
		}

		public void ExecuteSetCameraPosition()
		{
			if (this.IsMainParty)
			{
				this._resetCamera();
			}
		}

		private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			bool flag = this.Party.HomeSettlement != null && (this.Party.HomeSettlement.IsVillage ? settlement.BoundVillages.Contains(this.Party.HomeSettlement.Village) : (this.Party.HomeSettlement == settlement));
			if ((this.Party.IsCaravan || this.Party.IsVillager) && flag)
			{
				this._isPartyBannerDirty = true;
			}
		}

		private void OnClanChangeKingdom(Clan arg1, Kingdom arg2, Kingdom arg3, ChangeKingdomAction.ChangeKingdomActionDetail arg4, bool showNotification)
		{
			Hero leaderHero = this.Party.LeaderHero;
			if (((leaderHero != null) ? leaderHero.Clan : null) == arg1)
			{
				this._isPartyBannerDirty = true;
			}
		}

		private void OnClanLeaderChanged(Hero arg1, Hero arg2)
		{
			if (arg2.MapFaction == this.Party.MapFaction)
			{
				this._isPartyBannerDirty = true;
			}
		}

		public void RegisterEvents()
		{
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangeKingdom));
			CampaignEvents.OnClanLeaderChangedEvent.AddNonSerializedListener(this, new Action<Hero, Hero>(this.OnClanLeaderChanged));
		}

		public void UnregisterEvents()
		{
			CampaignEvents.OnSettlementOwnerChangedEvent.ClearListeners(this);
			CampaignEvents.ClanChangedKingdom.ClearListeners(this);
			CampaignEvents.OnClanLeaderChangedEvent.ClearListeners(this);
		}

		public Vec2 HeadPosition
		{
			get
			{
				return this._headPosition;
			}
			set
			{
				if (value != this._headPosition)
				{
					this._headPosition = value;
					base.OnPropertyChangedWithValue(value, "HeadPosition");
				}
			}
		}

		public string Count
		{
			get
			{
				return this._count;
			}
			set
			{
				if (value != this._count)
				{
					this._count = value;
					base.OnPropertyChangedWithValue<string>(value, "Count");
				}
			}
		}

		public string Prisoner
		{
			get
			{
				return this._prisoner;
			}
			set
			{
				if (value != this._prisoner)
				{
					this._prisoner = value;
					base.OnPropertyChangedWithValue<string>(value, "Prisoner");
				}
			}
		}

		public MBBindingList<QuestMarkerVM> Quests
		{
			get
			{
				return this._quests;
			}
			set
			{
				if (value != this._quests)
				{
					this._quests = value;
					base.OnPropertyChangedWithValue<MBBindingList<QuestMarkerVM>>(value, "Quests");
				}
			}
		}

		public string Wounded
		{
			get
			{
				return this._wounded;
			}
			set
			{
				if (value != this._wounded)
				{
					this._wounded = value;
					base.OnPropertyChangedWithValue<string>(value, "Wounded");
				}
			}
		}

		public string ExtraInfoText
		{
			get
			{
				return this._extraInfoText;
			}
			set
			{
				if (value != this._extraInfoText)
				{
					this._extraInfoText = value;
					base.OnPropertyChangedWithValue<string>(value, "ExtraInfoText");
				}
			}
		}

		public string MovementSpeedText
		{
			get
			{
				return this._movementSpeedText;
			}
			set
			{
				if (value != this._movementSpeedText)
				{
					this._movementSpeedText = value;
					base.OnPropertyChangedWithValue<string>(value, "MovementSpeedText");
				}
			}
		}

		public string FullName
		{
			get
			{
				return this._fullName;
			}
			set
			{
				if (value != this._fullName)
				{
					this._fullName = value;
					base.OnPropertyChangedWithValue<string>(value, "FullName");
				}
			}
		}

		public bool IsMainParty
		{
			get
			{
				return this._isMainParty;
			}
			set
			{
				if (value != this._isMainParty)
				{
					this._isMainParty = value;
					base.OnPropertyChangedWithValue(value, "IsMainParty");
				}
			}
		}

		public bool IsInArmy
		{
			get
			{
				return this._isInArmy;
			}
			set
			{
				if (value != this._isInArmy)
				{
					this._isInArmy = value;
					base.OnPropertyChangedWithValue(value, "IsInArmy");
				}
			}
		}

		public bool IsInSettlement
		{
			get
			{
				return this._isInSettlement;
			}
			set
			{
				if (value != this._isInSettlement)
				{
					this._isInSettlement = value;
					base.OnPropertyChangedWithValue(value, "IsInSettlement");
				}
			}
		}

		public bool IsDisorganized
		{
			get
			{
				return this._isDisorganized;
			}
			set
			{
				if (value != this._isDisorganized)
				{
					this._isDisorganized = value;
					base.OnPropertyChangedWithValue(value, "IsDisorganized");
				}
			}
		}

		public bool IsArmy
		{
			get
			{
				return this._isArmy;
			}
			set
			{
				if (value != this._isArmy)
				{
					this._isArmy = value;
					base.OnPropertyChangedWithValue(value, "IsArmy");
				}
			}
		}

		public bool IsBehind
		{
			get
			{
				return this._isBehind;
			}
			set
			{
				if (value != this._isBehind)
				{
					this._isBehind = value;
					base.OnPropertyChangedWithValue(value, "IsBehind");
				}
			}
		}

		public bool IsHigh
		{
			get
			{
				return this._isHigh;
			}
			set
			{
				if (value != this._isHigh)
				{
					this._isHigh = value;
					base.OnPropertyChangedWithValue(value, "IsHigh");
				}
			}
		}

		public bool IsPrisoner
		{
			get
			{
				return this._isPrisoner;
			}
			set
			{
				if (value != this._isPrisoner)
				{
					this._isPrisoner = value;
					base.OnPropertyChangedWithValue(value, "IsPrisoner");
				}
			}
		}

		public bool ShouldShowFullName
		{
			get
			{
				return this._shouldShowFullName || this._bindIsTargetedByTutorial;
			}
			set
			{
				if (value != this._shouldShowFullName)
				{
					this._shouldShowFullName = value;
					base.OnPropertyChangedWithValue(value, "ShouldShowFullName");
				}
			}
		}

		public ImageIdentifierVM MainHeroVisual
		{
			get
			{
				return this._mainHeroVisual;
			}
			set
			{
				if (value != this._mainHeroVisual)
				{
					this._mainHeroVisual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "MainHeroVisual");
				}
			}
		}

		public ImageIdentifierVM PartyBanner
		{
			get
			{
				return this._partyBanner;
			}
			set
			{
				if (value != this._partyBanner)
				{
					this._partyBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "PartyBanner");
				}
			}
		}

		public static string PositiveIndicator = Color.FromUint(4285650500U).ToString();

		public static string PositiveArmyIndicator = Color.FromUint(4288804731U).ToString();

		public static string NegativeIndicator = Color.FromUint(4292232774U).ToString();

		public static string NegativeArmyIndicator = Color.FromUint(4294931829U).ToString();

		public static string NeutralIndicator = Color.FromUint(4291877096U).ToString();

		public static string NeutralArmyIndicator = Color.FromUint(4294573055U).ToString();

		public static string MainPartyIndicator = Color.FromUint(4287421380U).ToString();

		public static string MainPartyArmyIndicator = Color.FromUint(4289593317U).ToString();

		private float _latestX;

		private float _latestY;

		private float _latestW;

		private float _cachedSpeed;

		private readonly Camera _mapCamera;

		private readonly Action _resetCamera;

		private readonly Func<bool> _getShouldShowFullName;

		private int _latestPrisonerAmount = -1;

		private int _latestWoundedAmount = -1;

		private int _latestTotalCount = -1;

		private bool _isPartyBannerDirty;

		private bool _isPartyHeroVisualDirty;

		private float _latestMainHeroAge = -1f;

		private TextObject _latestNameTextObject;

		private SandBoxUIHelper.IssueQuestFlags _questsBind;

		private Vec2 _partyPositionBind;

		private Vec2 _headPositionBind;

		private ImageIdentifierVM _mainHeroVisualBind;

		private bool _isMainPartyBind;

		private bool _isHighBind;

		private bool _isBehindBind;

		private bool _isInArmyBind;

		private bool _isInSettlementBind;

		private bool _isVisibleOnMapBind;

		private bool _shouldShowFullNameBind;

		private bool _isArmyBind;

		private bool _isPrisonerBind;

		private bool _isDisorganizedBind;

		private string _factionColorBind;

		private string _countBind;

		private string _woundedBind;

		private string _prisonerBind;

		private string _extraInfoTextBind;

		private string _fullNameBind;

		private string _movementSpeedTextBind;

		private string _count;

		private string _wounded;

		private string _prisoner;

		private MBBindingList<QuestMarkerVM> _quests;

		private string _fullName;

		private string _extraInfoText;

		private string _movementSpeedText;

		private bool _isMainParty;

		private bool _isBehind;

		private bool _isHigh;

		private bool _shouldShowFullName;

		private bool _isInArmy;

		private bool _isArmy;

		private bool _isPrisoner;

		private bool _isInSettlement;

		private bool _isDisorganized;

		private ImageIdentifierVM _mainHeroVisual;

		private ImageIdentifierVM _partyBanner;

		private Vec2 _headPosition;
	}
}
