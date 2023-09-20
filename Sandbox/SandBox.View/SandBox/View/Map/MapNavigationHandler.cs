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
	// Token: 0x0200004A RID: 74
	public class MapNavigationHandler : INavigationHandler
	{
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000256 RID: 598 RVA: 0x0001588C File Offset: 0x00013A8C
		// (set) Token: 0x06000257 RID: 599 RVA: 0x00015894 File Offset: 0x00013A94
		public bool IsNavigationLocked { get; set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000258 RID: 600 RVA: 0x0001589D File Offset: 0x00013A9D
		private InquiryData _unsavedChangesInquiry
		{
			get
			{
				return this.GetUnsavedChangedInquiry();
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000259 RID: 601 RVA: 0x000158A5 File Offset: 0x00013AA5
		private InquiryData _unapplicableChangesInquiry
		{
			get
			{
				return this.GetUnapplicableChangedInquiry();
			}
		}

		// Token: 0x0600025A RID: 602 RVA: 0x000158AD File Offset: 0x00013AAD
		public MapNavigationHandler()
		{
			this._game = Game.Current;
			this._needToBeInKingdomText = GameTexts.FindText("str_need_to_be_in_kingdom", null);
			this._clanScreenPermissionEvent = new MapNavigationHandler.ClanScreenPermissionEvent(new Action<bool, TextObject>(this.OnClanScreenPermission));
		}

		// Token: 0x0600025B RID: 603 RVA: 0x000158E8 File Offset: 0x00013AE8
		private InquiryData GetUnsavedChangedInquiry()
		{
			return new InquiryData(string.Empty, GameTexts.FindText("str_unsaved_changes", null).ToString(), true, true, GameTexts.FindText("str_apply", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), null, null, "", 0f, null, null, null);
		}

		// Token: 0x0600025C RID: 604 RVA: 0x00015940 File Offset: 0x00013B40
		private InquiryData GetUnapplicableChangedInquiry()
		{
			return new InquiryData(string.Empty, GameTexts.FindText("str_unapplicable_changes", null).ToString(), true, true, GameTexts.FindText("str_apply", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), null, null, "", 0f, null, null, null);
		}

		// Token: 0x0600025D RID: 605 RVA: 0x00015998 File Offset: 0x00013B98
		private bool IsMapTopScreen()
		{
			return ScreenManager.TopScreen is MapScreen;
		}

		// Token: 0x0600025E RID: 606 RVA: 0x000159A8 File Offset: 0x00013BA8
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

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600025F RID: 607 RVA: 0x00015A40 File Offset: 0x00013C40
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

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000260 RID: 608 RVA: 0x00015A97 File Offset: 0x00013C97
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

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000261 RID: 609 RVA: 0x00015AD5 File Offset: 0x00013CD5
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

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000262 RID: 610 RVA: 0x00015B04 File Offset: 0x00013D04
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

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000263 RID: 611 RVA: 0x00015B34 File Offset: 0x00013D34
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

		// Token: 0x06000264 RID: 612 RVA: 0x00015BBF File Offset: 0x00013DBF
		public void OnClanScreenPermission(bool isAvailable, TextObject reasonString)
		{
			if (!isAvailable)
			{
				this._mostRecentClanScreenPermission = new NavigationPermissionItem?(new NavigationPermissionItem(isAvailable, reasonString));
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000265 RID: 613 RVA: 0x00015BD8 File Offset: 0x00013DD8
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

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000266 RID: 614 RVA: 0x00015C48 File Offset: 0x00013E48
		public bool EscapeMenuEnabled
		{
			get
			{
				return this.IsNavigationBarEnabled() && !this.EscapeMenuActive && this._game.GameStateManager.ActiveState is MapState;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000267 RID: 615 RVA: 0x00015C76 File Offset: 0x00013E76
		public bool PartyActive
		{
			get
			{
				return this._game.GameStateManager.ActiveState is PartyState;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000268 RID: 616 RVA: 0x00015C90 File Offset: 0x00013E90
		public bool InventoryActive
		{
			get
			{
				return this._game.GameStateManager.ActiveState is InventoryState;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000269 RID: 617 RVA: 0x00015CAA File Offset: 0x00013EAA
		public bool QuestsActive
		{
			get
			{
				return this._game.GameStateManager.ActiveState is QuestsState;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600026A RID: 618 RVA: 0x00015CC4 File Offset: 0x00013EC4
		public bool CharacterDeveloperActive
		{
			get
			{
				return this._game.GameStateManager.ActiveState is CharacterDeveloperState;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600026B RID: 619 RVA: 0x00015CDE File Offset: 0x00013EDE
		public bool ClanActive
		{
			get
			{
				return this._game.GameStateManager.ActiveState is ClanState;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600026C RID: 620 RVA: 0x00015CF8 File Offset: 0x00013EF8
		public bool KingdomActive
		{
			get
			{
				return this._game.GameStateManager.ActiveState is KingdomState;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600026D RID: 621 RVA: 0x00015D12 File Offset: 0x00013F12
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

		// Token: 0x0600026E RID: 622 RVA: 0x00015D3D File Offset: 0x00013F3D
		void INavigationHandler.OpenQuests()
		{
			this.PrepareToOpenQuestsScreen(delegate
			{
				this.OpenQuestsAction();
			});
		}

		// Token: 0x0600026F RID: 623 RVA: 0x00015D54 File Offset: 0x00013F54
		void INavigationHandler.OpenQuests(IssueBase issue)
		{
			this.PrepareToOpenQuestsScreen(delegate
			{
				this.OpenQuestsAction(issue);
			});
		}

		// Token: 0x06000270 RID: 624 RVA: 0x00015D88 File Offset: 0x00013F88
		void INavigationHandler.OpenQuests(QuestBase quest)
		{
			this.PrepareToOpenQuestsScreen(delegate
			{
				this.OpenQuestsAction(quest);
			});
		}

		// Token: 0x06000271 RID: 625 RVA: 0x00015DBC File Offset: 0x00013FBC
		void INavigationHandler.OpenQuests(JournalLogEntry log)
		{
			this.PrepareToOpenQuestsScreen(delegate
			{
				this.OpenQuestsAction(log);
			});
		}

		// Token: 0x06000272 RID: 626 RVA: 0x00015DF0 File Offset: 0x00013FF0
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

		// Token: 0x06000273 RID: 627 RVA: 0x00015E84 File Offset: 0x00014084
		private void OpenQuestsAction()
		{
			QuestsState questsState = this._game.GameStateManager.CreateState<QuestsState>();
			this._game.GameStateManager.PushState(questsState, 0);
		}

		// Token: 0x06000274 RID: 628 RVA: 0x00015EB4 File Offset: 0x000140B4
		private void OpenQuestsAction(IssueBase issue)
		{
			QuestsState questsState = this._game.GameStateManager.CreateState<QuestsState>(new object[] { issue });
			this._game.GameStateManager.PushState(questsState, 0);
		}

		// Token: 0x06000275 RID: 629 RVA: 0x00015EF0 File Offset: 0x000140F0
		private void OpenQuestsAction(QuestBase quest)
		{
			QuestsState questsState = this._game.GameStateManager.CreateState<QuestsState>(new object[] { quest });
			this._game.GameStateManager.PushState(questsState, 0);
		}

		// Token: 0x06000276 RID: 630 RVA: 0x00015F2C File Offset: 0x0001412C
		private void OpenQuestsAction(JournalLogEntry log)
		{
			QuestsState questsState = this._game.GameStateManager.CreateState<QuestsState>(new object[] { log });
			this._game.GameStateManager.PushState(questsState, 0);
		}

		// Token: 0x06000277 RID: 631 RVA: 0x00015F68 File Offset: 0x00014168
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

		// Token: 0x06000278 RID: 632 RVA: 0x00015FD0 File Offset: 0x000141D0
		private void OpenInventoryScreenAction()
		{
			if (!this.IsMapTopScreen())
			{
				this._game.GameStateManager.PopState(0);
			}
			InventoryManager.OpenScreenAsInventory(null);
		}

		// Token: 0x06000279 RID: 633 RVA: 0x00015FF4 File Offset: 0x000141F4
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

		// Token: 0x0600027A RID: 634 RVA: 0x0001605C File Offset: 0x0001425C
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

		// Token: 0x0600027B RID: 635 RVA: 0x00016084 File Offset: 0x00014284
		void INavigationHandler.OpenCharacterDeveloper()
		{
			this.PrepareToOpenCharacterDeveloper(delegate
			{
				this.OpenCharacterDeveloperScreenAction();
			});
		}

		// Token: 0x0600027C RID: 636 RVA: 0x00016098 File Offset: 0x00014298
		void INavigationHandler.OpenCharacterDeveloper(Hero hero)
		{
			this.PrepareToOpenCharacterDeveloper(delegate
			{
				this.OpenCharacterDeveloperScreenAction(hero);
			});
		}

		// Token: 0x0600027D RID: 637 RVA: 0x000160CC File Offset: 0x000142CC
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

		// Token: 0x0600027E RID: 638 RVA: 0x00016160 File Offset: 0x00014360
		private void OpenCharacterDeveloperScreenAction()
		{
			CharacterDeveloperState characterDeveloperState = this._game.GameStateManager.CreateState<CharacterDeveloperState>();
			this._game.GameStateManager.PushState(characterDeveloperState, 0);
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00016190 File Offset: 0x00014390
		private void OpenCharacterDeveloperScreenAction(Hero hero)
		{
			CharacterDeveloperState characterDeveloperState = this._game.GameStateManager.CreateState<CharacterDeveloperState>(new object[] { hero });
			this._game.GameStateManager.PushState(characterDeveloperState, 0);
		}

		// Token: 0x06000280 RID: 640 RVA: 0x000161CA File Offset: 0x000143CA
		void INavigationHandler.OpenKingdom()
		{
			this.PrepareToOpenKingdomScreen(delegate
			{
				this.OpenKingdomAction();
			});
		}

		// Token: 0x06000281 RID: 641 RVA: 0x000161E0 File Offset: 0x000143E0
		void INavigationHandler.OpenKingdom(Army army)
		{
			this.PrepareToOpenKingdomScreen(delegate
			{
				this.OpenKingdomAction(army);
			});
		}

		// Token: 0x06000282 RID: 642 RVA: 0x00016214 File Offset: 0x00014414
		void INavigationHandler.OpenKingdom(Settlement settlement)
		{
			this.PrepareToOpenKingdomScreen(delegate
			{
				this.OpenKingdomAction(settlement);
			});
		}

		// Token: 0x06000283 RID: 643 RVA: 0x00016248 File Offset: 0x00014448
		void INavigationHandler.OpenKingdom(Clan clan)
		{
			this.PrepareToOpenKingdomScreen(delegate
			{
				this.OpenKingdomAction(clan);
			});
		}

		// Token: 0x06000284 RID: 644 RVA: 0x0001627C File Offset: 0x0001447C
		void INavigationHandler.OpenKingdom(PolicyObject policy)
		{
			this.PrepareToOpenKingdomScreen(delegate
			{
				this.OpenKingdomAction(policy);
			});
		}

		// Token: 0x06000285 RID: 645 RVA: 0x000162B0 File Offset: 0x000144B0
		void INavigationHandler.OpenKingdom(IFaction faction)
		{
			this.PrepareToOpenKingdomScreen(delegate
			{
				this.OpenKingdomAction(faction);
			});
		}

		// Token: 0x06000286 RID: 646 RVA: 0x000162E4 File Offset: 0x000144E4
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

		// Token: 0x06000287 RID: 647 RVA: 0x00016380 File Offset: 0x00014580
		private void OpenKingdomAction()
		{
			KingdomState kingdomState = this._game.GameStateManager.CreateState<KingdomState>();
			this._game.GameStateManager.PushState(kingdomState, 0);
		}

		// Token: 0x06000288 RID: 648 RVA: 0x000163B0 File Offset: 0x000145B0
		private void OpenKingdomAction(Army army)
		{
			KingdomState kingdomState = this._game.GameStateManager.CreateState<KingdomState>(new object[] { army });
			this._game.GameStateManager.PushState(kingdomState, 0);
		}

		// Token: 0x06000289 RID: 649 RVA: 0x000163EC File Offset: 0x000145EC
		private void OpenKingdomAction(Settlement settlement)
		{
			KingdomState kingdomState = this._game.GameStateManager.CreateState<KingdomState>(new object[] { settlement });
			this._game.GameStateManager.PushState(kingdomState, 0);
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00016428 File Offset: 0x00014628
		private void OpenKingdomAction(Clan clan)
		{
			KingdomState kingdomState = this._game.GameStateManager.CreateState<KingdomState>(new object[] { clan });
			this._game.GameStateManager.PushState(kingdomState, 0);
		}

		// Token: 0x0600028B RID: 651 RVA: 0x00016464 File Offset: 0x00014664
		private void OpenKingdomAction(PolicyObject policy)
		{
			KingdomState kingdomState = this._game.GameStateManager.CreateState<KingdomState>(new object[] { policy });
			this._game.GameStateManager.PushState(kingdomState, 0);
		}

		// Token: 0x0600028C RID: 652 RVA: 0x000164A0 File Offset: 0x000146A0
		private void OpenKingdomAction(IFaction faction)
		{
			KingdomState kingdomState = this._game.GameStateManager.CreateState<KingdomState>(new object[] { faction });
			this._game.GameStateManager.PushState(kingdomState, 0);
		}

		// Token: 0x0600028D RID: 653 RVA: 0x000164DA File Offset: 0x000146DA
		void INavigationHandler.OpenClan()
		{
			this.PrepareToOpenClanScreen(delegate
			{
				this.OpenClanScreenAction();
			});
		}

		// Token: 0x0600028E RID: 654 RVA: 0x000164F0 File Offset: 0x000146F0
		void INavigationHandler.OpenClan(Hero hero)
		{
			this.PrepareToOpenClanScreen(delegate
			{
				this.OpenClanScreenAction(hero);
			});
		}

		// Token: 0x0600028F RID: 655 RVA: 0x00016524 File Offset: 0x00014724
		void INavigationHandler.OpenClan(PartyBase party)
		{
			this.PrepareToOpenClanScreen(delegate
			{
				this.OpenClanScreenAction(party);
			});
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00016558 File Offset: 0x00014758
		void INavigationHandler.OpenClan(Settlement settlement)
		{
			this.PrepareToOpenClanScreen(delegate
			{
				this.OpenClanScreenAction(settlement);
			});
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0001658C File Offset: 0x0001478C
		void INavigationHandler.OpenClan(Workshop workshop)
		{
			this.PrepareToOpenClanScreen(delegate
			{
				this.OpenClanScreenAction(workshop);
			});
		}

		// Token: 0x06000292 RID: 658 RVA: 0x000165C0 File Offset: 0x000147C0
		void INavigationHandler.OpenClan(Alley alley)
		{
			this.PrepareToOpenClanScreen(delegate
			{
				this.OpenClanScreenAction(alley);
			});
		}

		// Token: 0x06000293 RID: 659 RVA: 0x000165F4 File Offset: 0x000147F4
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

		// Token: 0x06000294 RID: 660 RVA: 0x00016690 File Offset: 0x00014890
		private void OpenClanScreenAction()
		{
			ClanState clanState = this._game.GameStateManager.CreateState<ClanState>();
			this._game.GameStateManager.PushState(clanState, 0);
		}

		// Token: 0x06000295 RID: 661 RVA: 0x000166C0 File Offset: 0x000148C0
		private void OpenClanScreenAction(Hero hero)
		{
			ClanState clanState = this._game.GameStateManager.CreateState<ClanState>(new object[] { hero });
			this._game.GameStateManager.PushState(clanState, 0);
		}

		// Token: 0x06000296 RID: 662 RVA: 0x000166FC File Offset: 0x000148FC
		private void OpenClanScreenAction(PartyBase party)
		{
			ClanState clanState = this._game.GameStateManager.CreateState<ClanState>(new object[] { party });
			this._game.GameStateManager.PushState(clanState, 0);
		}

		// Token: 0x06000297 RID: 663 RVA: 0x00016738 File Offset: 0x00014938
		private void OpenClanScreenAction(Settlement settlement)
		{
			ClanState clanState = this._game.GameStateManager.CreateState<ClanState>(new object[] { settlement });
			this._game.GameStateManager.PushState(clanState, 0);
		}

		// Token: 0x06000298 RID: 664 RVA: 0x00016774 File Offset: 0x00014974
		private void OpenClanScreenAction(Workshop workshop)
		{
			ClanState clanState = this._game.GameStateManager.CreateState<ClanState>(new object[] { workshop });
			this._game.GameStateManager.PushState(clanState, 0);
		}

		// Token: 0x06000299 RID: 665 RVA: 0x000167B0 File Offset: 0x000149B0
		private void OpenClanScreenAction(Alley alley)
		{
			ClanState clanState = this._game.GameStateManager.CreateState<ClanState>(new object[] { alley });
			this._game.GameStateManager.PushState(clanState, 0);
		}

		// Token: 0x0600029A RID: 666 RVA: 0x000167EA File Offset: 0x000149EA
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

		// Token: 0x0600029B RID: 667 RVA: 0x00016804 File Offset: 0x00014A04
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

		// Token: 0x0400014A RID: 330
		private readonly Game _game;

		// Token: 0x0400014B RID: 331
		private NavigationPermissionItem? _mostRecentClanScreenPermission;

		// Token: 0x0400014C RID: 332
		private readonly TextObject _needToBeInKingdomText;

		// Token: 0x0400014D RID: 333
		private readonly MapNavigationHandler.ClanScreenPermissionEvent _clanScreenPermissionEvent;

		// Token: 0x0200007B RID: 123
		public class ClanScreenPermissionEvent : EventBase
		{
			// Token: 0x17000073 RID: 115
			// (get) Token: 0x0600043A RID: 1082 RVA: 0x00022CD1 File Offset: 0x00020ED1
			// (set) Token: 0x0600043B RID: 1083 RVA: 0x00022CD9 File Offset: 0x00020ED9
			public Action<bool, TextObject> IsClanScreenAvailable { get; private set; }

			// Token: 0x0600043C RID: 1084 RVA: 0x00022CE2 File Offset: 0x00020EE2
			public ClanScreenPermissionEvent(Action<bool, TextObject> isClanScreenAvailable)
			{
				this.IsClanScreenAvailable = isClanScreenAvailable;
			}
		}
	}
}
