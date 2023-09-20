using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay
{
	// Token: 0x0200009E RID: 158
	[MenuOverlay("ArmyMenuOverlay")]
	public class ArmyMenuOverlayVM : GameMenuOverlay
	{
		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x06000F9C RID: 3996 RVA: 0x0003CAF2 File Offset: 0x0003ACF2
		private IEnumerable<MobileParty> _mergedPartiesAndLeaderParty
		{
			get
			{
				yield return this._army.LeaderParty;
				foreach (MobileParty mobileParty in this._army.LeaderParty.AttachedParties)
				{
					yield return mobileParty;
				}
				List<MobileParty>.Enumerator enumerator = default(List<MobileParty>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x06000F9D RID: 3997 RVA: 0x0003CB04 File Offset: 0x0003AD04
		public ArmyMenuOverlayVM()
		{
			this.PartyList = new MBBindingList<GameMenuPartyItemVM>();
			base.CurrentOverlayType = 2;
			base.IsInitializationOver = false;
			this._army = MobileParty.MainParty.Army ?? MobileParty.MainParty.TargetParty.Army;
			this._savedPartyList = new List<MobileParty>();
			this.CohesionHint = new BasicTooltipViewModel();
			this.ManCountHint = new BasicTooltipViewModel();
			this.FoodHint = new BasicTooltipViewModel();
			this.TutorialNotification = new ElementNotificationVM();
			this.ManageArmyHint = new HintViewModel();
			this.Refresh();
			this._contextMenuItem = null;
			CampaignEvents.ArmyOverlaySetDirtyEvent.AddNonSerializedListener(this, new Action(this.Refresh));
			CampaignEvents.PartyAttachedAnotherParty.AddNonSerializedListener(this, new Action<MobileParty>(this.OnPartyAttachedAnotherParty));
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			this._cohesionConceptObj = Concept.All.SingleOrDefault((Concept c) => c.StringId == "str_game_objects_army_cohesion");
			base.IsInitializationOver = true;
		}

		// Token: 0x06000F9E RID: 3998 RVA: 0x0003CC22 File Offset: 0x0003AE22
		public override void RefreshValues()
		{
			base.RefreshValues();
			ElementNotificationVM tutorialNotification = this.TutorialNotification;
			if (tutorialNotification != null)
			{
				tutorialNotification.RefreshValues();
			}
			this.Refresh();
		}

		// Token: 0x06000F9F RID: 3999 RVA: 0x0003CC44 File Offset: 0x0003AE44
		protected override void ExecuteOnSetAsActiveContextMenuItem(GameMenuPartyItemVM troop)
		{
			base.ExecuteOnSetAsActiveContextMenuItem(troop);
			base.ContextList.Clear();
			MobileParty mobileParty = this._contextMenuItem.Party.MobileParty;
			if (((mobileParty != null) ? mobileParty.Army : null) != null && this._contextMenuItem.Party.MobileParty.Army.LeaderParty == MobileParty.MainParty && this._contextMenuItem.Party.MapEvent == null && this._contextMenuItem.Party != this._contextMenuItem.Party.MobileParty.Army.LeaderParty.Party)
			{
				TextObject textObject;
				bool mapScreenActionIsEnabledWithReason = CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out textObject);
				base.ContextList.Add(new StringItemWithEnabledAndHintVM(new Action<object>(base.ExecuteTroopAction), GameTexts.FindText("str_menu_overlay_context_list", GameMenuOverlay.MenuOverlayContextList.ArmyDismiss.ToString()).ToString(), mapScreenActionIsEnabledWithReason, GameMenuOverlay.MenuOverlayContextList.ArmyDismiss, textObject));
			}
			MobileParty mobileParty2 = troop.Party.MobileParty;
			bool flag = mobileParty2 != null && mobileParty2.Position2D.DistanceSquared(MobileParty.MainParty.Position2D) < 9f;
			bool flag2;
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter playerEncounter = PlayerEncounter.Current;
				flag2 = playerEncounter != null && !playerEncounter.IsEnemy;
			}
			else
			{
				flag2 = true;
			}
			bool flag3 = flag2;
			if (this._contextMenuItem.Party.LeaderHero != null && flag && flag3 && this._contextMenuItem.Party != PartyBase.MainParty)
			{
				PlayerEncounter playerEncounter2 = PlayerEncounter.Current;
				if (((playerEncounter2 != null) ? playerEncounter2.BattleSimulation : null) == null)
				{
					base.ContextList.Add(new StringItemWithEnabledAndHintVM(new Action<object>(base.ExecuteTroopAction), GameTexts.FindText("str_menu_overlay_context_list", GameMenuOverlay.MenuOverlayContextList.DonateTroops.ToString()).ToString(), true, GameMenuOverlay.MenuOverlayContextList.DonateTroops, null));
					if (MobileParty.MainParty.CurrentSettlement == null && LocationComplex.Current == null)
					{
						base.ContextList.Add(new StringItemWithEnabledAndHintVM(new Action<object>(base.ExecuteTroopAction), GameTexts.FindText("str_menu_overlay_context_list", GameMenuOverlay.MenuOverlayContextList.ConverseWithLeader.ToString()).ToString(), true, GameMenuOverlay.MenuOverlayContextList.ConverseWithLeader, null));
					}
				}
			}
			base.ContextList.Add(new StringItemWithEnabledAndHintVM(new Action<object>(base.ExecuteTroopAction), GameTexts.FindText("str_menu_overlay_context_list", GameMenuOverlay.MenuOverlayContextList.Encyclopedia.ToString()).ToString(), true, GameMenuOverlay.MenuOverlayContextList.Encyclopedia, null));
			CharacterObject characterObject = this._contextMenuItem.Character ?? this._contextMenuItem.Party.LeaderHero.CharacterObject;
			CampaignEventDispatcher.Instance.OnCharacterPortraitPopUpOpened(characterObject);
		}

		// Token: 0x06000FA0 RID: 4000 RVA: 0x0003CEDC File Offset: 0x0003B0DC
		public override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			TextObject textObject;
			this.CanManageArmy = this.GetCanManageArmyWithReason(out textObject);
			this.ManageArmyHint.HintText = textObject;
			for (int i = 0; i < this.PartyList.Count; i++)
			{
				this.PartyList[i].RefreshQuestStatus();
			}
			if (this._isVisualsDirty)
			{
				this.RefreshVisualsOfItems();
				this._isVisualsDirty = false;
			}
		}

		// Token: 0x06000FA1 RID: 4001 RVA: 0x0003CF48 File Offset: 0x0003B148
		private bool GetCanManageArmyWithReason(out TextObject reasonText)
		{
			if (Hero.MainHero.IsPrisoner)
			{
				reasonText = GameTexts.FindText("str_action_disabled_reason_prisoner", null);
				return false;
			}
			if (PlayerEncounter.Current != null)
			{
				if (PlayerEncounter.EncounterSettlement == null)
				{
					reasonText = GameTexts.FindText("str_action_disabled_reason_encounter", null);
					return false;
				}
				Village village = PlayerEncounter.EncounterSettlement.Village;
				if (village != null && village.VillageState == Village.VillageStates.BeingRaided && MobileParty.MainParty.MapEvent != null && MobileParty.MainParty.MapEvent.IsRaid)
				{
					reasonText = GameTexts.FindText("str_action_disabled_reason_raid", null);
					return false;
				}
			}
			if (!this.IsPlayerArmyLeader)
			{
				reasonText = TextObject.Empty;
				return false;
			}
			if (MapEvent.PlayerMapEvent != null)
			{
				reasonText = GameTexts.FindText("str_cannot_manage_army_while_in_event", null);
				return false;
			}
			reasonText = TextObject.Empty;
			return true;
		}

		// Token: 0x06000FA2 RID: 4002 RVA: 0x0003D003 File Offset: 0x0003B203
		public sealed override void Refresh()
		{
			if (PartyBase.MainParty.MobileParty.Army != null)
			{
				base.IsInitializationOver = false;
				this.UpdateLists();
				this.UpdateProperties();
				base.IsInitializationOver = true;
			}
		}

		// Token: 0x06000FA3 RID: 4003 RVA: 0x0003D030 File Offset: 0x0003B230
		private void UpdateProperties()
		{
			MBTextManager.SetTextVariable("newline", "\n", false);
			int num = this._army.LeaderParty.ItemRoster.TotalFood;
			foreach (MobileParty mobileParty in this._army.LeaderParty.AttachedParties)
			{
				num += mobileParty.ItemRoster.TotalFood;
			}
			this.Food = num;
			this.Cohesion = (int)MobileParty.MainParty.Army.Cohesion;
			this.ManCountText = CampaignUIHelper.GetPartyNameplateText(this._army.LeaderParty);
			this.FoodHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetArmyFoodTooltip(this._army));
			this.CohesionHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetArmyCohesionTooltip(this._army));
			this.ManCountHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetArmyManCountTooltip(this._army));
			this.IsCohesionWarningEnabled = MobileParty.MainParty.Army.Cohesion <= 30f;
			this.IsPlayerArmyLeader = MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty;
		}

		// Token: 0x06000FA4 RID: 4004 RVA: 0x0003D170 File Offset: 0x0003B370
		private void UpdateLists()
		{
			List<MobileParty> list = this._army.Parties.Except(this._savedPartyList).ToList<MobileParty>();
			list.Remove(this._army.LeaderParty);
			List<MobileParty> list2 = this._savedPartyList.Except(this._army.Parties).ToList<MobileParty>();
			this._savedPartyList = this._army.Parties.ToList<MobileParty>();
			foreach (MobileParty mobileParty in list2)
			{
				for (int i = this.PartyList.Count - 1; i >= 0; i--)
				{
					if (this.PartyList[i].Party == mobileParty.Party)
					{
						this.PartyList.RemoveAt(i);
						break;
					}
				}
			}
			foreach (MobileParty mobileParty2 in list)
			{
				this.PartyList.Add(new GameMenuPartyItemVM(new Action<GameMenuPartyItemVM>(this.ExecuteOnSetAsActiveContextMenuItem), mobileParty2.Party, true));
			}
			foreach (GameMenuPartyItemVM gameMenuPartyItemVM in this.PartyList)
			{
				gameMenuPartyItemVM.RefreshProperties();
			}
			if (this.PartyList.Count > 0 && this.PartyList[0].Party != this._army.LeaderParty.Party)
			{
				if (this.PartyList.SingleOrDefault((GameMenuPartyItemVM p) => p.Party == this._army.LeaderParty.Party) != null)
				{
					int num = this.PartyList.IndexOf(this.PartyList.SingleOrDefault((GameMenuPartyItemVM p) => p.Party == this._army.LeaderParty.Party));
					this.PartyList.RemoveAt(num);
				}
				GameMenuPartyItemVM gameMenuPartyItemVM2 = new GameMenuPartyItemVM(new Action<GameMenuPartyItemVM>(this.ExecuteOnSetAsActiveContextMenuItem), this._army.LeaderParty.Party, true)
				{
					IsLeader = true
				};
				this.PartyList.Insert(0, gameMenuPartyItemVM2);
				gameMenuPartyItemVM2.RefreshProperties();
			}
		}

		// Token: 0x06000FA5 RID: 4005 RVA: 0x0003D3B0 File Offset: 0x0003B5B0
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.ArmyOverlaySetDirtyEvent.ClearListeners(this);
			CampaignEvents.PartyAttachedAnotherParty.ClearListeners(this);
			Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
		}

		// Token: 0x06000FA6 RID: 4006 RVA: 0x0003D3EC File Offset: 0x0003B5EC
		public void ExecuteOpenArmyManagement()
		{
			MobileParty mainParty = MobileParty.MainParty;
			if (((mainParty != null) ? mainParty.Army : null) != null)
			{
				MobileParty leaderParty = MobileParty.MainParty.Army.LeaderParty;
				if (leaderParty != null && leaderParty.IsMainParty)
				{
					Action openArmyManagement = this.OpenArmyManagement;
					if (openArmyManagement == null)
					{
						return;
					}
					openArmyManagement();
				}
			}
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x0003D439 File Offset: 0x0003B639
		private void ExecuteCohesionLink()
		{
			if (this._cohesionConceptObj != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this._cohesionConceptObj.EncyclopediaLink);
				return;
			}
			Debug.FailedAssert("Couldn't find Cohesion encyclopedia page", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\GameMenu\\Overlay\\ArmyMenuOverlayVM.cs", "ExecuteCohesionLink", 292);
		}

		// Token: 0x06000FA8 RID: 4008 RVA: 0x0003D478 File Offset: 0x0003B678
		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent obj)
		{
			if (obj.NewNotificationElementID != this._latestTutorialElementID)
			{
				if (this._latestTutorialElementID != null)
				{
					this.TutorialNotification.ElementID = string.Empty;
				}
				this._latestTutorialElementID = obj.NewNotificationElementID;
				if (this._latestTutorialElementID != null)
				{
					this.TutorialNotification.ElementID = this._latestTutorialElementID;
				}
			}
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x0003D4D8 File Offset: 0x0003B6D8
		private void RefreshVisualsOfItems()
		{
			for (int i = 0; i < this.PartyList.Count; i++)
			{
				this.PartyList[i].RefreshVisual();
			}
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x0003D50C File Offset: 0x0003B70C
		private void OnPartyAttachedAnotherParty(MobileParty party)
		{
			MobileParty attachedTo = party.AttachedTo;
			if (((attachedTo != null) ? attachedTo.Army : null) != null && party.AttachedTo.Army == MobileParty.MainParty.Army)
			{
				this._isVisualsDirty = true;
			}
		}

		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x06000FAB RID: 4011 RVA: 0x0003D540 File Offset: 0x0003B740
		// (set) Token: 0x06000FAC RID: 4012 RVA: 0x0003D548 File Offset: 0x0003B748
		[DataSourceProperty]
		public ElementNotificationVM TutorialNotification
		{
			get
			{
				return this._tutorialNotification;
			}
			set
			{
				if (value != this._tutorialNotification)
				{
					this._tutorialNotification = value;
					base.OnPropertyChangedWithValue<ElementNotificationVM>(value, "TutorialNotification");
				}
			}
		}

		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x06000FAD RID: 4013 RVA: 0x0003D566 File Offset: 0x0003B766
		// (set) Token: 0x06000FAE RID: 4014 RVA: 0x0003D56E File Offset: 0x0003B76E
		[DataSourceProperty]
		public HintViewModel ManageArmyHint
		{
			get
			{
				return this._manageArmyHint;
			}
			set
			{
				if (value != this._manageArmyHint)
				{
					this._manageArmyHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ManageArmyHint");
				}
			}
		}

		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x06000FAF RID: 4015 RVA: 0x0003D58C File Offset: 0x0003B78C
		// (set) Token: 0x06000FB0 RID: 4016 RVA: 0x0003D594 File Offset: 0x0003B794
		[DataSourceProperty]
		public int Cohesion
		{
			get
			{
				return this._cohesion;
			}
			set
			{
				if (value != this._cohesion)
				{
					this._cohesion = value;
					base.OnPropertyChangedWithValue(value, "Cohesion");
				}
			}
		}

		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x06000FB1 RID: 4017 RVA: 0x0003D5B2 File Offset: 0x0003B7B2
		// (set) Token: 0x06000FB2 RID: 4018 RVA: 0x0003D5BA File Offset: 0x0003B7BA
		[DataSourceProperty]
		public bool IsCohesionWarningEnabled
		{
			get
			{
				return this._isCohesionWarningEnabled;
			}
			set
			{
				if (value != this._isCohesionWarningEnabled)
				{
					this._isCohesionWarningEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsCohesionWarningEnabled");
				}
			}
		}

		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x06000FB3 RID: 4019 RVA: 0x0003D5D8 File Offset: 0x0003B7D8
		// (set) Token: 0x06000FB4 RID: 4020 RVA: 0x0003D5E0 File Offset: 0x0003B7E0
		[DataSourceProperty]
		public bool CanManageArmy
		{
			get
			{
				return this._canManageArmy;
			}
			set
			{
				if (value != this._canManageArmy)
				{
					this._canManageArmy = value;
					base.OnPropertyChangedWithValue(value, "CanManageArmy");
				}
			}
		}

		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x06000FB5 RID: 4021 RVA: 0x0003D5FE File Offset: 0x0003B7FE
		// (set) Token: 0x06000FB6 RID: 4022 RVA: 0x0003D606 File Offset: 0x0003B806
		[DataSourceProperty]
		public bool IsPlayerArmyLeader
		{
			get
			{
				return this._isPlayerArmyLeader;
			}
			set
			{
				if (value != this._isPlayerArmyLeader)
				{
					this._isPlayerArmyLeader = value;
					base.OnPropertyChangedWithValue(value, "IsPlayerArmyLeader");
				}
			}
		}

		// Token: 0x1700052A RID: 1322
		// (get) Token: 0x06000FB7 RID: 4023 RVA: 0x0003D624 File Offset: 0x0003B824
		// (set) Token: 0x06000FB8 RID: 4024 RVA: 0x0003D62C File Offset: 0x0003B82C
		[DataSourceProperty]
		public string ManCountText
		{
			get
			{
				return this._manCountText;
			}
			set
			{
				if (value != this._manCountText)
				{
					this._manCountText = value;
					base.OnPropertyChangedWithValue<string>(value, "ManCountText");
				}
			}
		}

		// Token: 0x1700052B RID: 1323
		// (get) Token: 0x06000FB9 RID: 4025 RVA: 0x0003D64F File Offset: 0x0003B84F
		// (set) Token: 0x06000FBA RID: 4026 RVA: 0x0003D657 File Offset: 0x0003B857
		[DataSourceProperty]
		public int Food
		{
			get
			{
				return this._food;
			}
			set
			{
				if (value != this._food)
				{
					this._food = value;
					base.OnPropertyChangedWithValue(value, "Food");
				}
			}
		}

		// Token: 0x1700052C RID: 1324
		// (get) Token: 0x06000FBB RID: 4027 RVA: 0x0003D675 File Offset: 0x0003B875
		// (set) Token: 0x06000FBC RID: 4028 RVA: 0x0003D67D File Offset: 0x0003B87D
		[DataSourceProperty]
		public MBBindingList<GameMenuPartyItemVM> PartyList
		{
			get
			{
				return this._partyList;
			}
			set
			{
				if (value != this._partyList)
				{
					this._partyList = value;
					base.OnPropertyChangedWithValue<MBBindingList<GameMenuPartyItemVM>>(value, "PartyList");
				}
			}
		}

		// Token: 0x1700052D RID: 1325
		// (get) Token: 0x06000FBD RID: 4029 RVA: 0x0003D69B File Offset: 0x0003B89B
		// (set) Token: 0x06000FBE RID: 4030 RVA: 0x0003D6A3 File Offset: 0x0003B8A3
		[DataSourceProperty]
		public BasicTooltipViewModel CohesionHint
		{
			get
			{
				return this._cohesionHint;
			}
			set
			{
				if (value != this._cohesionHint)
				{
					this._cohesionHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "CohesionHint");
				}
			}
		}

		// Token: 0x1700052E RID: 1326
		// (get) Token: 0x06000FBF RID: 4031 RVA: 0x0003D6C1 File Offset: 0x0003B8C1
		// (set) Token: 0x06000FC0 RID: 4032 RVA: 0x0003D6C9 File Offset: 0x0003B8C9
		[DataSourceProperty]
		public BasicTooltipViewModel ManCountHint
		{
			get
			{
				return this._manCountHint;
			}
			set
			{
				if (value != this._manCountHint)
				{
					this._manCountHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "ManCountHint");
				}
			}
		}

		// Token: 0x1700052F RID: 1327
		// (get) Token: 0x06000FC1 RID: 4033 RVA: 0x0003D6E7 File Offset: 0x0003B8E7
		// (set) Token: 0x06000FC2 RID: 4034 RVA: 0x0003D6EF File Offset: 0x0003B8EF
		[DataSourceProperty]
		public BasicTooltipViewModel FoodHint
		{
			get
			{
				return this._foodHint;
			}
			set
			{
				if (value != this._foodHint)
				{
					this._foodHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "FoodHint");
				}
			}
		}

		// Token: 0x17000530 RID: 1328
		// (get) Token: 0x06000FC3 RID: 4035 RVA: 0x0003D70D File Offset: 0x0003B90D
		[DataSourceProperty]
		public MBBindingList<StringItemWithHintVM> IssueList
		{
			get
			{
				if (this._issueList == null)
				{
					this._issueList = new MBBindingList<StringItemWithHintVM>();
				}
				return this._issueList;
			}
		}

		// Token: 0x04000740 RID: 1856
		private readonly Army _army;

		// Token: 0x04000741 RID: 1857
		private List<MobileParty> _savedPartyList;

		// Token: 0x04000742 RID: 1858
		private const float CohesionWarningMin = 30f;

		// Token: 0x04000743 RID: 1859
		public Action OpenArmyManagement;

		// Token: 0x04000744 RID: 1860
		private readonly Concept _cohesionConceptObj;

		// Token: 0x04000745 RID: 1861
		private string _latestTutorialElementID;

		// Token: 0x04000746 RID: 1862
		private bool _isVisualsDirty;

		// Token: 0x04000747 RID: 1863
		private MBBindingList<GameMenuPartyItemVM> _partyList;

		// Token: 0x04000748 RID: 1864
		private string _manCountText;

		// Token: 0x04000749 RID: 1865
		private int _cohesion;

		// Token: 0x0400074A RID: 1866
		private int _food;

		// Token: 0x0400074B RID: 1867
		private bool _isCohesionWarningEnabled;

		// Token: 0x0400074C RID: 1868
		private bool _isPlayerArmyLeader;

		// Token: 0x0400074D RID: 1869
		private bool _canManageArmy;

		// Token: 0x0400074E RID: 1870
		private HintViewModel _manageArmyHint;

		// Token: 0x0400074F RID: 1871
		public ElementNotificationVM _tutorialNotification;

		// Token: 0x04000750 RID: 1872
		private BasicTooltipViewModel _cohesionHint;

		// Token: 0x04000751 RID: 1873
		private BasicTooltipViewModel _manCountHint;

		// Token: 0x04000752 RID: 1874
		private BasicTooltipViewModel _foodHint;

		// Token: 0x04000753 RID: 1875
		private MBBindingList<StringItemWithHintVM> _issueList;
	}
}
