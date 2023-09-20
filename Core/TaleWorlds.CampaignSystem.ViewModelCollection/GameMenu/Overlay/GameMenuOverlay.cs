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
	public class GameMenuOverlay : ViewModel
	{
		public GameMenuOverlay()
		{
			this.ContextList = new MBBindingList<StringItemWithEnabledAndHintVM>();
		}

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

		protected virtual void ExecuteOnSetAsActiveContextMenuItem(GameMenuPartyItemVM troop)
		{
			this._contextMenuItem = troop;
		}

		public virtual void ExecuteOnOverlayClosed()
		{
			if (!this._closedHandled)
			{
				CampaignEventDispatcher.Instance.OnCharacterPortraitPopUpClosed();
				this._closedHandled = true;
			}
		}

		public virtual void ExecuteOnOverlayOpened()
		{
			this._closedHandled = false;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			if (!this._closedHandled)
			{
				this.ExecuteOnOverlayClosed();
			}
		}

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

		public void ClearOverlay()
		{
		}

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

		public virtual void Refresh()
		{
		}

		public virtual void UpdateOverlayType(GameOverlays.MenuOverlayType newType)
		{
			this.Refresh();
		}

		public virtual void OnFrameTick(float dt)
		{
		}

		public void HourlyTick()
		{
			this.Refresh();
		}

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

		public string GameMenuOverlayName;

		private bool _closedHandled = true;

		private bool _isContextMenuEnabled;

		private int _currentOverlayType = -1;

		private bool _isInfoBarExtended;

		private bool _isInitializationOver;

		private MBBindingList<StringItemWithEnabledAndHintVM> _contextList;

		protected GameMenuPartyItemVM _contextMenuItem;

		protected internal enum MenuOverlayContextList
		{
			Encyclopedia,
			Conversation,
			QuickConversation,
			ConverseWithLeader,
			ArmyDismiss,
			ManageGarrison,
			DonateTroops,
			JoinArmy,
			TakeToParty,
			ManageTroops
		}
	}
}
