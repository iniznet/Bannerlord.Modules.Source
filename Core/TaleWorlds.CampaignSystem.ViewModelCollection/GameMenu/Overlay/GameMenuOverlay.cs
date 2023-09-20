using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay
{
	// Token: 0x020000A1 RID: 161
	public class GameMenuOverlay : ViewModel
	{
		// Token: 0x06001008 RID: 4104 RVA: 0x0003F290 File Offset: 0x0003D490
		public GameMenuOverlay()
		{
			this.ContextList = new MBBindingList<StringItemWithEnabledAndHintVM>();
		}

		// Token: 0x06001009 RID: 4105 RVA: 0x0003F2B1 File Offset: 0x0003D4B1
		public override void RefreshValues()
		{
			base.RefreshValues();
			GameMenuPartyItemVM contextMenuItem = this._contextMenuItem;
			if (contextMenuItem == null)
			{
				return;
			}
			contextMenuItem.RefreshValues();
		}

		// Token: 0x0600100A RID: 4106 RVA: 0x0003F2C9 File Offset: 0x0003D4C9
		protected virtual void ExecuteOnSetAsActiveContextMenuItem(GameMenuPartyItemVM troop)
		{
			this._contextMenuItem = troop;
		}

		// Token: 0x0600100B RID: 4107 RVA: 0x0003F2D2 File Offset: 0x0003D4D2
		public virtual void ExecuteOnOverlayClosed()
		{
			if (!this._closedHandled)
			{
				CampaignEventDispatcher.Instance.OnCharacterPortraitPopUpClosed();
				this._closedHandled = true;
			}
		}

		// Token: 0x0600100C RID: 4108 RVA: 0x0003F2ED File Offset: 0x0003D4ED
		public virtual void ExecuteOnOverlayOpened()
		{
			this._closedHandled = false;
		}

		// Token: 0x0600100D RID: 4109 RVA: 0x0003F2F6 File Offset: 0x0003D4F6
		public override void OnFinalize()
		{
			base.OnFinalize();
			if (!this._closedHandled)
			{
				this.ExecuteOnOverlayClosed();
			}
		}

		// Token: 0x0600100E RID: 4110 RVA: 0x0003F30C File Offset: 0x0003D50C
		protected void ExecuteTroopAction(object o)
		{
			switch ((GameMenuOverlay.MenuOverlayContextList)o)
			{
			case GameMenuOverlay.MenuOverlayContextList.Encyclopedia:
				if (this._contextMenuItem.Character != null)
				{
					if (this._contextMenuItem.Character.IsHero)
					{
						Campaign.Current.EncyclopediaManager.GoToLink(this._contextMenuItem.Character.HeroObject.EncyclopediaLink);
					}
					else
					{
						Debug.FailedAssert("Character object in menu overlay", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\GameMenu\\Overlay\\GameMenuOverlay.cs", "ExecuteTroopAction", 101);
						Campaign.Current.EncyclopediaManager.GoToLink(this._contextMenuItem.Character.EncyclopediaLink);
					}
				}
				else if (this._contextMenuItem.Party != null)
				{
					CharacterObject visualPartyLeader = CampaignUIHelper.GetVisualPartyLeader(this._contextMenuItem.Party);
					if (visualPartyLeader != null)
					{
						Campaign.Current.EncyclopediaManager.GoToLink(visualPartyLeader.EncyclopediaLink);
					}
				}
				else if (this._contextMenuItem.Settlement != null)
				{
					Campaign.Current.EncyclopediaManager.GoToLink(this._contextMenuItem.Settlement.EncyclopediaLink);
				}
				break;
			case GameMenuOverlay.MenuOverlayContextList.Conversation:
				if (this._contextMenuItem.Character != null)
				{
					if (this._contextMenuItem.Character.IsHero)
					{
						if (PlayerEncounter.Current != null || LocationComplex.Current != null || Campaign.Current.CurrentMenuContext != null)
						{
							Location location = LocationComplex.Current.GetLocationOfCharacter(this._contextMenuItem.Character.HeroObject);
							if (location.StringId == "alley")
							{
								location = LocationComplex.Current.GetLocationWithId("center");
							}
							CampaignEventDispatcher.Instance.OnPlayerStartTalkFromMenu(this._contextMenuItem.Character.HeroObject);
							PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(location, null, this._contextMenuItem.Character, null);
						}
						else
						{
							EncounterManager.StartPartyEncounter(PartyBase.MainParty, this._contextMenuItem.Character.HeroObject.PartyBelongedTo.Party);
						}
					}
					else
					{
						Debug.FailedAssert("Character object in menu overlay", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\GameMenu\\Overlay\\GameMenuOverlay.cs", "ExecuteTroopAction", 145);
					}
				}
				break;
			case GameMenuOverlay.MenuOverlayContextList.QuickConversation:
				if (this._contextMenuItem.Character != null)
				{
					if (this._contextMenuItem.Character.IsHero)
					{
						if (PlayerEncounter.Current != null || LocationComplex.Current != null || Campaign.Current.CurrentMenuContext != null)
						{
							CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, false, false, false, true, false, false), new ConversationCharacterData(this._contextMenuItem.Character, null, false, false, false, true, false, false));
						}
						else
						{
							EncounterManager.StartPartyEncounter(PartyBase.MainParty, this._contextMenuItem.Character.HeroObject.PartyBelongedTo.Party);
						}
					}
					else
					{
						Debug.FailedAssert("Character object in menu overlay", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\GameMenu\\Overlay\\GameMenuOverlay.cs", "ExecuteTroopAction", 168);
					}
				}
				break;
			case GameMenuOverlay.MenuOverlayContextList.ConverseWithLeader:
			{
				PartyBase party = this._contextMenuItem.Party;
				if (((party != null) ? party.LeaderHero : null) != null)
				{
					if (Settlement.CurrentSettlement != null || LocationComplex.Current != null || Campaign.Current.CurrentMenuContext != null)
					{
						this.ConverseWithLeader(PartyBase.MainParty, this._contextMenuItem.Party);
					}
					else
					{
						EncounterManager.StartPartyEncounter(PartyBase.MainParty, this._contextMenuItem.Party);
					}
				}
				break;
			}
			case GameMenuOverlay.MenuOverlayContextList.ArmyDismiss:
			{
				PartyBase party2 = this._contextMenuItem.Party;
				if (((party2 != null) ? party2.MobileParty.Army : null) != null && this._contextMenuItem.Party.MapEvent == null && this._contextMenuItem.Party.MobileParty.Army.LeaderParty != this._contextMenuItem.Party.MobileParty)
				{
					if (this._contextMenuItem.Party.MobileParty.Army.LeaderParty == MobileParty.MainParty && this._contextMenuItem.Party.MobileParty.Army.Parties.Count <= 2)
					{
						DisbandArmyAction.ApplyByNotEnoughParty(this._contextMenuItem.Party.MobileParty.Army);
					}
					else
					{
						this._contextMenuItem.Party.MobileParty.Army = null;
						this._contextMenuItem.Party.MobileParty.Ai.SetMoveModeHold();
					}
				}
				break;
			}
			case GameMenuOverlay.MenuOverlayContextList.ManageGarrison:
				if (this._contextMenuItem.Party != null)
				{
					PartyScreenManager.OpenScreenAsManageTroops(this._contextMenuItem.Party.MobileParty);
				}
				break;
			case GameMenuOverlay.MenuOverlayContextList.DonateTroops:
				if (this._contextMenuItem.Party != null)
				{
					if (this._contextMenuItem.Party.MobileParty.IsGarrison)
					{
						PartyScreenManager.OpenScreenAsDonateGarrisonWithCurrentSettlement();
					}
					else
					{
						PartyScreenManager.OpenScreenAsDonateTroops(this._contextMenuItem.Party.MobileParty);
					}
				}
				break;
			case GameMenuOverlay.MenuOverlayContextList.JoinArmy:
			{
				CharacterObject character = this._contextMenuItem.Character;
				if (character != null && character.IsHero && this._contextMenuItem.Character.HeroObject.PartyBelongedTo != null)
				{
					MobileParty.MainParty.Army = this._contextMenuItem.Character.HeroObject.PartyBelongedTo.Army;
					MobileParty.MainParty.Army.AddPartyToMergedParties(MobileParty.MainParty);
				}
				break;
			}
			case GameMenuOverlay.MenuOverlayContextList.TakeToParty:
			{
				CharacterObject character2 = this._contextMenuItem.Character;
				if (character2 != null && character2.IsHero && this._contextMenuItem.Character.HeroObject.PartyBelongedTo == null)
				{
					Settlement currentSettlement = this._contextMenuItem.Character.HeroObject.CurrentSettlement;
					bool flag;
					if (currentSettlement == null)
					{
						flag = false;
					}
					else
					{
						MBReadOnlyList<Hero> notables = currentSettlement.Notables;
						bool? flag2 = ((notables != null) ? new bool?(notables.Contains(this._contextMenuItem.Character.HeroObject)) : null);
						bool flag3 = true;
						flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
					}
					if (flag)
					{
						LeaveSettlementAction.ApplyForCharacterOnly(this._contextMenuItem.Character.HeroObject);
					}
					AddHeroToPartyAction.Apply(this._contextMenuItem.Character.HeroObject, MobileParty.MainParty, true);
				}
				break;
			}
			case GameMenuOverlay.MenuOverlayContextList.ManageTroops:
			{
				PartyBase party3 = this._contextMenuItem.Party;
				if (((party3 != null) ? party3.MobileParty : null) != null && this._contextMenuItem.Party.MobileParty.ActualClan == Clan.PlayerClan)
				{
					PartyScreenManager.OpenScreenAsManageTroopsAndPrisoners(this._contextMenuItem.Party.MobileParty, null);
				}
				break;
			}
			}
			if (!this._closedHandled)
			{
				CampaignEventDispatcher.Instance.OnCharacterPortraitPopUpClosed();
				this._closedHandled = true;
			}
		}

		// Token: 0x0600100F RID: 4111 RVA: 0x0003F980 File Offset: 0x0003DB80
		private void ConverseWithLeader(PartyBase mainParty1, PartyBase party2)
		{
			bool flag;
			if (mainParty1.Side != BattleSideEnum.Attacker)
			{
				PlayerEncounter playerEncounter = PlayerEncounter.Current;
				flag = playerEncounter != null && playerEncounter.PlayerSide == BattleSideEnum.Attacker;
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (LocationComplex.Current == null || flag2)
			{
				CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, mainParty1, false, false, false, false, false, false), new ConversationCharacterData(ConversationHelper.GetConversationCharacterPartyLeader(party2), party2, false, false, false, false, false, false));
				return;
			}
			Location locationOfCharacter = LocationComplex.Current.GetLocationOfCharacter(party2.LeaderHero);
			CampaignEventDispatcher.Instance.OnPlayerStartTalkFromMenu(party2.LeaderHero);
			PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(locationOfCharacter, null, party2.LeaderHero.CharacterObject, null);
		}

		// Token: 0x06001010 RID: 4112 RVA: 0x0003FA1D File Offset: 0x0003DC1D
		public void ClearOverlay()
		{
		}

		// Token: 0x06001011 RID: 4113 RVA: 0x0003FA20 File Offset: 0x0003DC20
		public static GameMenuOverlay GetOverlay(GameOverlays.MenuOverlayType menuOverlayType)
		{
			if (menuOverlayType == GameOverlays.MenuOverlayType.Encounter)
			{
				return new EncounterMenuOverlayVM();
			}
			if (menuOverlayType == GameOverlays.MenuOverlayType.SettlementWithParties || menuOverlayType == GameOverlays.MenuOverlayType.SettlementWithCharacters || menuOverlayType == GameOverlays.MenuOverlayType.SettlementWithBoth)
			{
				return new SettlementMenuOverlayVM(menuOverlayType);
			}
			Debug.FailedAssert("Game menu overlay: " + menuOverlayType.ToString() + " could not be found", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\GameMenu\\Overlay\\GameMenuOverlay.cs", "GetOverlay", 294);
			return null;
		}

		// Token: 0x06001012 RID: 4114 RVA: 0x0003FA7B File Offset: 0x0003DC7B
		public virtual void Refresh()
		{
		}

		// Token: 0x06001013 RID: 4115 RVA: 0x0003FA7D File Offset: 0x0003DC7D
		public virtual void UpdateOverlayType(GameOverlays.MenuOverlayType newType)
		{
			this.Refresh();
		}

		// Token: 0x06001014 RID: 4116 RVA: 0x0003FA85 File Offset: 0x0003DC85
		public virtual void OnFrameTick(float dt)
		{
		}

		// Token: 0x06001015 RID: 4117 RVA: 0x0003FA87 File Offset: 0x0003DC87
		public void HourlyTick()
		{
			this.Refresh();
		}

		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x06001016 RID: 4118 RVA: 0x0003FA8F File Offset: 0x0003DC8F
		// (set) Token: 0x06001017 RID: 4119 RVA: 0x0003FA97 File Offset: 0x0003DC97
		[DataSourceProperty]
		public bool IsContextMenuEnabled
		{
			get
			{
				return this._isContextMenuEnabled;
			}
			set
			{
				this._isContextMenuEnabled = value;
				base.OnPropertyChangedWithValue(value, "IsContextMenuEnabled");
			}
		}

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x06001018 RID: 4120 RVA: 0x0003FAAC File Offset: 0x0003DCAC
		// (set) Token: 0x06001019 RID: 4121 RVA: 0x0003FAB4 File Offset: 0x0003DCB4
		[DataSourceProperty]
		public bool IsInitializationOver
		{
			get
			{
				return this._isInitializationOver;
			}
			set
			{
				this._isInitializationOver = value;
				base.OnPropertyChangedWithValue(value, "IsInitializationOver");
			}
		}

		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x0600101A RID: 4122 RVA: 0x0003FAC9 File Offset: 0x0003DCC9
		// (set) Token: 0x0600101B RID: 4123 RVA: 0x0003FAD1 File Offset: 0x0003DCD1
		[DataSourceProperty]
		public bool IsInfoBarExtended
		{
			get
			{
				return this._isInfoBarExtended;
			}
			set
			{
				this._isInfoBarExtended = value;
				base.OnPropertyChangedWithValue(value, "IsInfoBarExtended");
			}
		}

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x0600101C RID: 4124 RVA: 0x0003FAE6 File Offset: 0x0003DCE6
		// (set) Token: 0x0600101D RID: 4125 RVA: 0x0003FAEE File Offset: 0x0003DCEE
		[DataSourceProperty]
		public MBBindingList<StringItemWithEnabledAndHintVM> ContextList
		{
			get
			{
				return this._contextList;
			}
			set
			{
				if (value != this._contextList)
				{
					this._contextList = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringItemWithEnabledAndHintVM>>(value, "ContextList");
				}
			}
		}

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x0600101E RID: 4126 RVA: 0x0003FB0C File Offset: 0x0003DD0C
		// (set) Token: 0x0600101F RID: 4127 RVA: 0x0003FB14 File Offset: 0x0003DD14
		[DataSourceProperty]
		public int CurrentOverlayType
		{
			get
			{
				return this._currentOverlayType;
			}
			set
			{
				if (value != this._currentOverlayType)
				{
					this._currentOverlayType = value;
					base.OnPropertyChangedWithValue(value, "CurrentOverlayType");
				}
			}
		}

		// Token: 0x04000774 RID: 1908
		public string GameMenuOverlayName;

		// Token: 0x04000775 RID: 1909
		private bool _closedHandled = true;

		// Token: 0x04000776 RID: 1910
		private bool _isContextMenuEnabled;

		// Token: 0x04000777 RID: 1911
		private int _currentOverlayType = -1;

		// Token: 0x04000778 RID: 1912
		private bool _isInfoBarExtended;

		// Token: 0x04000779 RID: 1913
		private bool _isInitializationOver;

		// Token: 0x0400077A RID: 1914
		private MBBindingList<StringItemWithEnabledAndHintVM> _contextList;

		// Token: 0x0400077B RID: 1915
		protected GameMenuPartyItemVM _contextMenuItem;

		// Token: 0x020001E1 RID: 481
		protected internal enum MenuOverlayContextList
		{
			// Token: 0x0400100A RID: 4106
			Encyclopedia,
			// Token: 0x0400100B RID: 4107
			Conversation,
			// Token: 0x0400100C RID: 4108
			QuickConversation,
			// Token: 0x0400100D RID: 4109
			ConverseWithLeader,
			// Token: 0x0400100E RID: 4110
			ArmyDismiss,
			// Token: 0x0400100F RID: 4111
			ManageGarrison,
			// Token: 0x04001010 RID: 4112
			DonateTroops,
			// Token: 0x04001011 RID: 4113
			JoinArmy,
			// Token: 0x04001012 RID: 4114
			TakeToParty,
			// Token: 0x04001013 RID: 4115
			ManageTroops
		}
	}
}
