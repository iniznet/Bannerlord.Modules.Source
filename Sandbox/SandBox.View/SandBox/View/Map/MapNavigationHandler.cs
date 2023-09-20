using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Library.EventSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ScreenSystem;

namespace SandBox.View.Map
{
	public class MapNavigationHandler : INavigationHandler
	{
		public bool IsNavigationLocked { get; set; }

		private InquiryData _unsavedChangesInquiry
		{
			get
			{
				return this.GetUnsavedChangedInquiry();
			}
		}

		private InquiryData _unapplicableChangesInquiry
		{
			get
			{
				return this.GetUnapplicableChangedInquiry();
			}
		}

		public MapNavigationHandler()
		{
			this._game = Game.Current;
			this._needToBeInKingdomText = GameTexts.FindText("str_need_to_be_in_kingdom", null);
			this._clanScreenPermissionEvent = new MapNavigationHandler.ClanScreenPermissionEvent(new Action<bool, TextObject>(this.OnClanScreenPermission));
		}

		private InquiryData GetUnsavedChangedInquiry()
		{
			return new InquiryData(string.Empty, GameTexts.FindText("str_unsaved_changes", null).ToString(), true, true, GameTexts.FindText("str_apply", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), null, null, "", 0f, null, null, null);
		}

		private InquiryData GetUnapplicableChangedInquiry()
		{
			return new InquiryData(string.Empty, GameTexts.FindText("str_unapplicable_changes", null).ToString(), true, true, GameTexts.FindText("str_apply", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), null, null, "", 0f, null, null, null);
		}

		private bool IsMapTopScreen()
		{
			return ScreenManager.TopScreen is MapScreen;
		}

		private bool IsNavigationBarEnabled()
		{
			if (Hero.MainHero != null)
			{
				Hero mainHero = Hero.MainHero;
				if (mainHero == null || !mainHero.IsDead)
				{
					Campaign campaign = Campaign.Current;
					if (campaign == null || !campaign.SaveHandler.IsSaving)
					{
						MapScreen mapScreen;
						return !this.IsNavigationLocked && (InventoryManager.InventoryLogic == null || InventoryManager.Instance.CurrentMode == null) && (PartyScreenManager.PartyScreenLogic == null || PartyScreenManager.Instance.CurrentMode == null) && PlayerEncounter.CurrentBattleSimulation == null && ((mapScreen = ScreenManager.TopScreen as MapScreen) == null || !mapScreen.IsInArmyManagement);
					}
				}
			}
			return false;
		}

		public bool PartyEnabled
		{
			get
			{
				if (!this.IsNavigationBarEnabled())
				{
					return false;
				}
				if (this.PartyActive)
				{
					return false;
				}
				if (Hero.MainHero.HeroState == 3)
				{
					return false;
				}
				if (MobileParty.MainParty.MapEvent != null)
				{
					return false;
				}
				Mission mission = Mission.Current;
				return mission == null || mission.IsPartyWindowAccessAllowed;
			}
		}

		public bool InventoryEnabled
		{
			get
			{
				if (!this.IsNavigationBarEnabled())
				{
					return false;
				}
				if (this.InventoryActive)
				{
					return false;
				}
				if (Hero.MainHero.HeroState == 3)
				{
					return false;
				}
				Mission mission = Mission.Current;
				return mission == null || mission.IsInventoryAccessAllowed;
			}
		}

		public bool QuestsEnabled
		{
			get
			{
				if (!this.IsNavigationBarEnabled())
				{
					return false;
				}
				if (this.QuestsActive)
				{
					return false;
				}
				Mission mission = Mission.Current;
				return mission == null || mission.IsQuestScreenAccessAllowed;
			}
		}

		public bool CharacterDeveloperEnabled
		{
			get
			{
				if (!this.IsNavigationBarEnabled())
				{
					return false;
				}
				if (this.CharacterDeveloperActive)
				{
					return false;
				}
				Mission mission = Mission.Current;
				return mission == null || mission.IsCharacterWindowAccessAllowed;
			}
		}

		public NavigationPermissionItem ClanPermission
		{
			get
			{
				if (!this.IsNavigationBarEnabled())
				{
					return new NavigationPermissionItem(false, null);
				}
				if (this.ClanActive)
				{
					return new NavigationPermissionItem(false, null);
				}
				Mission mission = Mission.Current;
				if (mission != null && !mission.IsClanWindowAccessAllowed)
				{
					return new NavigationPermissionItem(false, null);
				}
				this._mostRecentClanScreenPermission = null;
				Game.Current.EventManager.TriggerEvent<MapNavigationHandler.ClanScreenPermissionEvent>(this._clanScreenPermissionEvent);
				NavigationPermissionItem? mostRecentClanScreenPermission = this._mostRecentClanScreenPermission;
				if (mostRecentClanScreenPermission == null)
				{
					return new NavigationPermissionItem(true, null);
				}
				return mostRecentClanScreenPermission.GetValueOrDefault();
			}
		}

		public void OnClanScreenPermission(bool isAvailable, TextObject reasonString)
		{
			if (!isAvailable)
			{
				this._mostRecentClanScreenPermission = new NavigationPermissionItem?(new NavigationPermissionItem(isAvailable, reasonString));
			}
		}

		public NavigationPermissionItem KingdomPermission
		{
			get
			{
				if (!this.IsNavigationBarEnabled())
				{
					return new NavigationPermissionItem(false, null);
				}
				if (this.KingdomActive)
				{
					return new NavigationPermissionItem(false, null);
				}
				if (!Hero.MainHero.MapFaction.IsKingdomFaction)
				{
					return new NavigationPermissionItem(false, this._needToBeInKingdomText);
				}
				Mission mission = Mission.Current;
				if (mission != null && !mission.IsKingdomWindowAccessAllowed)
				{
					return new NavigationPermissionItem(false, null);
				}
				return new NavigationPermissionItem(true, null);
			}
		}

		public bool EscapeMenuEnabled
		{
			get
			{
				return this.IsNavigationBarEnabled() && !this.EscapeMenuActive && this._game.GameStateManager.ActiveState is MapState;
			}
		}

		public bool PartyActive
		{
			get
			{
				return this._game.GameStateManager.ActiveState is PartyState;
			}
		}

		public bool InventoryActive
		{
			get
			{
				return this._game.GameStateManager.ActiveState is InventoryState;
			}
		}

		public bool QuestsActive
		{
			get
			{
				return this._game.GameStateManager.ActiveState is QuestsState;
			}
		}

		public bool CharacterDeveloperActive
		{
			get
			{
				return this._game.GameStateManager.ActiveState is CharacterDeveloperState;
			}
		}

		public bool ClanActive
		{
			get
			{
				return this._game.GameStateManager.ActiveState is ClanState;
			}
		}

		public bool KingdomActive
		{
			get
			{
				return this._game.GameStateManager.ActiveState is KingdomState;
			}
		}

		public bool EscapeMenuActive
		{
			get
			{
				if (this._game.GameStateManager.ActiveState is MapState)
				{
					MapScreen instance = MapScreen.Instance;
					return instance != null && instance.IsEscapeMenuOpened;
				}
				return false;
			}
		}

		void INavigationHandler.OpenQuests()
		{
			this.PrepareToOpenQuestsScreen(delegate
			{
				this.OpenQuestsAction();
			});
		}

		void INavigationHandler.OpenQuests(IssueBase issue)
		{
			this.PrepareToOpenQuestsScreen(delegate
			{
				this.OpenQuestsAction(issue);
			});
		}

		void INavigationHandler.OpenQuests(QuestBase quest)
		{
			this.PrepareToOpenQuestsScreen(delegate
			{
				this.OpenQuestsAction(quest);
			});
		}

		void INavigationHandler.OpenQuests(JournalLogEntry log)
		{
			this.PrepareToOpenQuestsScreen(delegate
			{
				this.OpenQuestsAction(log);
			});
		}

		private void PrepareToOpenQuestsScreen(Action openQuestsAction)
		{
			if (this.QuestsEnabled)
			{
				IChangeableScreen changeableScreen;
				if ((changeableScreen = ScreenManager.TopScreen as IChangeableScreen) != null && changeableScreen.AnyUnsavedChanges())
				{
					InquiryData inquiryData = (changeableScreen.CanChangesBeApplied() ? this._unsavedChangesInquiry : this._unapplicableChangesInquiry);
					inquiryData.SetAffirmativeAction(delegate
					{
						this.ApplyCurrentChanges();
						openQuestsAction();
					});
					InformationManager.ShowInquiry(inquiryData, false, false);
					return;
				}
				if (!this.IsMapTopScreen())
				{
					this._game.GameStateManager.PopState(0);
				}
				openQuestsAction();
			}
		}

		private void OpenQuestsAction()
		{
			QuestsState questsState = this._game.GameStateManager.CreateState<QuestsState>();
			this._game.GameStateManager.PushState(questsState, 0);
		}

		private void OpenQuestsAction(IssueBase issue)
		{
			QuestsState questsState = this._game.GameStateManager.CreateState<QuestsState>(new object[] { issue });
			this._game.GameStateManager.PushState(questsState, 0);
		}

		private void OpenQuestsAction(QuestBase quest)
		{
			QuestsState questsState = this._game.GameStateManager.CreateState<QuestsState>(new object[] { quest });
			this._game.GameStateManager.PushState(questsState, 0);
		}

		private void OpenQuestsAction(JournalLogEntry log)
		{
			QuestsState questsState = this._game.GameStateManager.CreateState<QuestsState>(new object[] { log });
			this._game.GameStateManager.PushState(questsState, 0);
		}

		void INavigationHandler.OpenInventory()
		{
			if (this.InventoryEnabled)
			{
				IChangeableScreen changeableScreen;
				if ((changeableScreen = ScreenManager.TopScreen as IChangeableScreen) != null)
				{
					if (changeableScreen.AnyUnsavedChanges())
					{
						InquiryData inquiryData = (changeableScreen.CanChangesBeApplied() ? this._unsavedChangesInquiry : this._unapplicableChangesInquiry);
						inquiryData.SetAffirmativeAction(delegate
						{
							this.ApplyCurrentChanges();
							this.OpenInventoryScreenAction();
						});
						InformationManager.ShowInquiry(inquiryData, false, false);
						return;
					}
					this.OpenInventoryScreenAction();
					return;
				}
				else
				{
					this.OpenInventoryScreenAction();
				}
			}
		}

		private void OpenInventoryScreenAction()
		{
			if (!this.IsMapTopScreen())
			{
				this._game.GameStateManager.PopState(0);
			}
			InventoryManager.OpenScreenAsInventory(null);
		}

		void INavigationHandler.OpenParty()
		{
			if (this.PartyEnabled)
			{
				IChangeableScreen changeableScreen;
				if ((changeableScreen = ScreenManager.TopScreen as IChangeableScreen) != null)
				{
					if (changeableScreen.AnyUnsavedChanges())
					{
						InquiryData inquiryData = (changeableScreen.CanChangesBeApplied() ? this._unsavedChangesInquiry : this._unapplicableChangesInquiry);
						inquiryData.SetAffirmativeAction(delegate
						{
							this.ApplyCurrentChanges();
							this.OpenPartyScreenAction();
						});
						InformationManager.ShowInquiry(inquiryData, false, false);
						return;
					}
					this.OpenPartyScreenAction();
					return;
				}
				else
				{
					this.OpenPartyScreenAction();
				}
			}
		}

		private void OpenPartyScreenAction()
		{
			if (this.PartyEnabled)
			{
				if (!this.IsMapTopScreen())
				{
					this._game.GameStateManager.PopState(0);
				}
				PartyScreenManager.OpenScreenAsNormal();
			}
		}

		void INavigationHandler.OpenCharacterDeveloper()
		{
			this.PrepareToOpenCharacterDeveloper(delegate
			{
				this.OpenCharacterDeveloperScreenAction();
			});
		}

		void INavigationHandler.OpenCharacterDeveloper(Hero hero)
		{
			this.PrepareToOpenCharacterDeveloper(delegate
			{
				this.OpenCharacterDeveloperScreenAction(hero);
			});
		}

		private void PrepareToOpenCharacterDeveloper(Action openCharacterDeveloperAction)
		{
			if (this.CharacterDeveloperEnabled)
			{
				IChangeableScreen changeableScreen;
				if ((changeableScreen = ScreenManager.TopScreen as IChangeableScreen) != null && changeableScreen.AnyUnsavedChanges())
				{
					InquiryData inquiryData = (changeableScreen.CanChangesBeApplied() ? this._unsavedChangesInquiry : this._unapplicableChangesInquiry);
					inquiryData.SetAffirmativeAction(delegate
					{
						this.ApplyCurrentChanges();
						openCharacterDeveloperAction();
					});
					InformationManager.ShowInquiry(inquiryData, false, false);
					return;
				}
				if (!this.IsMapTopScreen())
				{
					this._game.GameStateManager.PopState(0);
				}
				openCharacterDeveloperAction();
			}
		}

		private void OpenCharacterDeveloperScreenAction()
		{
			CharacterDeveloperState characterDeveloperState = this._game.GameStateManager.CreateState<CharacterDeveloperState>();
			this._game.GameStateManager.PushState(characterDeveloperState, 0);
		}

		private void OpenCharacterDeveloperScreenAction(Hero hero)
		{
			CharacterDeveloperState characterDeveloperState = this._game.GameStateManager.CreateState<CharacterDeveloperState>(new object[] { hero });
			this._game.GameStateManager.PushState(characterDeveloperState, 0);
		}

		void INavigationHandler.OpenKingdom()
		{
			this.PrepareToOpenKingdomScreen(delegate
			{
				this.OpenKingdomAction();
			});
		}

		void INavigationHandler.OpenKingdom(Army army)
		{
			this.PrepareToOpenKingdomScreen(delegate
			{
				this.OpenKingdomAction(army);
			});
		}

		void INavigationHandler.OpenKingdom(Settlement settlement)
		{
			this.PrepareToOpenKingdomScreen(delegate
			{
				this.OpenKingdomAction(settlement);
			});
		}

		void INavigationHandler.OpenKingdom(Clan clan)
		{
			this.PrepareToOpenKingdomScreen(delegate
			{
				this.OpenKingdomAction(clan);
			});
		}

		void INavigationHandler.OpenKingdom(PolicyObject policy)
		{
			this.PrepareToOpenKingdomScreen(delegate
			{
				this.OpenKingdomAction(policy);
			});
		}

		void INavigationHandler.OpenKingdom(IFaction faction)
		{
			this.PrepareToOpenKingdomScreen(delegate
			{
				this.OpenKingdomAction(faction);
			});
		}

		private void PrepareToOpenKingdomScreen(Action openKingdomAction)
		{
			if (this.KingdomPermission.IsAuthorized)
			{
				IChangeableScreen changeableScreen;
				if ((changeableScreen = ScreenManager.TopScreen as IChangeableScreen) != null && changeableScreen.AnyUnsavedChanges())
				{
					InquiryData inquiryData = (changeableScreen.CanChangesBeApplied() ? this._unsavedChangesInquiry : this._unapplicableChangesInquiry);
					inquiryData.SetAffirmativeAction(delegate
					{
						this.ApplyCurrentChanges();
						openKingdomAction();
					});
					InformationManager.ShowInquiry(inquiryData, false, false);
					return;
				}
				if (!this.IsMapTopScreen())
				{
					this._game.GameStateManager.PopState(0);
				}
				openKingdomAction();
			}
		}

		private void OpenKingdomAction()
		{
			KingdomState kingdomState = this._game.GameStateManager.CreateState<KingdomState>();
			this._game.GameStateManager.PushState(kingdomState, 0);
		}

		private void OpenKingdomAction(Army army)
		{
			KingdomState kingdomState = this._game.GameStateManager.CreateState<KingdomState>(new object[] { army });
			this._game.GameStateManager.PushState(kingdomState, 0);
		}

		private void OpenKingdomAction(Settlement settlement)
		{
			KingdomState kingdomState = this._game.GameStateManager.CreateState<KingdomState>(new object[] { settlement });
			this._game.GameStateManager.PushState(kingdomState, 0);
		}

		private void OpenKingdomAction(Clan clan)
		{
			KingdomState kingdomState = this._game.GameStateManager.CreateState<KingdomState>(new object[] { clan });
			this._game.GameStateManager.PushState(kingdomState, 0);
		}

		private void OpenKingdomAction(PolicyObject policy)
		{
			KingdomState kingdomState = this._game.GameStateManager.CreateState<KingdomState>(new object[] { policy });
			this._game.GameStateManager.PushState(kingdomState, 0);
		}

		private void OpenKingdomAction(IFaction faction)
		{
			KingdomState kingdomState = this._game.GameStateManager.CreateState<KingdomState>(new object[] { faction });
			this._game.GameStateManager.PushState(kingdomState, 0);
		}

		void INavigationHandler.OpenClan()
		{
			this.PrepareToOpenClanScreen(delegate
			{
				this.OpenClanScreenAction();
			});
		}

		void INavigationHandler.OpenClan(Hero hero)
		{
			this.PrepareToOpenClanScreen(delegate
			{
				this.OpenClanScreenAction(hero);
			});
		}

		void INavigationHandler.OpenClan(PartyBase party)
		{
			this.PrepareToOpenClanScreen(delegate
			{
				this.OpenClanScreenAction(party);
			});
		}

		void INavigationHandler.OpenClan(Settlement settlement)
		{
			this.PrepareToOpenClanScreen(delegate
			{
				this.OpenClanScreenAction(settlement);
			});
		}

		void INavigationHandler.OpenClan(Workshop workshop)
		{
			this.PrepareToOpenClanScreen(delegate
			{
				this.OpenClanScreenAction(workshop);
			});
		}

		void INavigationHandler.OpenClan(Alley alley)
		{
			this.PrepareToOpenClanScreen(delegate
			{
				this.OpenClanScreenAction(alley);
			});
		}

		private void PrepareToOpenClanScreen(Action openClanScreenAction)
		{
			if (this.ClanPermission.IsAuthorized)
			{
				IChangeableScreen changeableScreen;
				if ((changeableScreen = ScreenManager.TopScreen as IChangeableScreen) != null && changeableScreen.AnyUnsavedChanges())
				{
					InquiryData inquiryData = (changeableScreen.CanChangesBeApplied() ? this._unsavedChangesInquiry : this._unapplicableChangesInquiry);
					inquiryData.SetAffirmativeAction(delegate
					{
						this.ApplyCurrentChanges();
						openClanScreenAction();
					});
					InformationManager.ShowInquiry(inquiryData, false, false);
					return;
				}
				if (!this.IsMapTopScreen())
				{
					this._game.GameStateManager.PopState(0);
				}
				openClanScreenAction();
			}
		}

		private void OpenClanScreenAction()
		{
			ClanState clanState = this._game.GameStateManager.CreateState<ClanState>();
			this._game.GameStateManager.PushState(clanState, 0);
		}

		private void OpenClanScreenAction(Hero hero)
		{
			ClanState clanState = this._game.GameStateManager.CreateState<ClanState>(new object[] { hero });
			this._game.GameStateManager.PushState(clanState, 0);
		}

		private void OpenClanScreenAction(PartyBase party)
		{
			ClanState clanState = this._game.GameStateManager.CreateState<ClanState>(new object[] { party });
			this._game.GameStateManager.PushState(clanState, 0);
		}

		private void OpenClanScreenAction(Settlement settlement)
		{
			ClanState clanState = this._game.GameStateManager.CreateState<ClanState>(new object[] { settlement });
			this._game.GameStateManager.PushState(clanState, 0);
		}

		private void OpenClanScreenAction(Workshop workshop)
		{
			ClanState clanState = this._game.GameStateManager.CreateState<ClanState>(new object[] { workshop });
			this._game.GameStateManager.PushState(clanState, 0);
		}

		private void OpenClanScreenAction(Alley alley)
		{
			ClanState clanState = this._game.GameStateManager.CreateState<ClanState>(new object[] { alley });
			this._game.GameStateManager.PushState(clanState, 0);
		}

		void INavigationHandler.OpenEscapeMenu()
		{
			if (this.EscapeMenuEnabled)
			{
				MapScreen instance = MapScreen.Instance;
				if (instance == null)
				{
					return;
				}
				instance.OpenEscapeMenu();
			}
		}

		private void ApplyCurrentChanges()
		{
			IChangeableScreen changeableScreen;
			if ((changeableScreen = ScreenManager.TopScreen as IChangeableScreen) != null && changeableScreen.AnyUnsavedChanges())
			{
				if (changeableScreen.CanChangesBeApplied())
				{
					changeableScreen.ApplyChanges();
				}
				else
				{
					changeableScreen.ResetChanges();
				}
			}
			if (!this.IsMapTopScreen())
			{
				this._game.GameStateManager.PopState(0);
			}
		}

		private readonly Game _game;

		private NavigationPermissionItem? _mostRecentClanScreenPermission;

		private readonly TextObject _needToBeInKingdomText;

		private readonly MapNavigationHandler.ClanScreenPermissionEvent _clanScreenPermissionEvent;

		public class ClanScreenPermissionEvent : EventBase
		{
			public Action<bool, TextObject> IsClanScreenAvailable { get; private set; }

			public ClanScreenPermissionEvent(Action<bool, TextObject> isClanScreenAvailable)
			{
				this.IsClanScreenAvailable = isClanScreenAvailable;
			}
		}
	}
}
