using System;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.GameState
{
	// Token: 0x0200033E RID: 830
	public class MapState : GameState
	{
		// Token: 0x17000B0F RID: 2831
		// (get) Token: 0x06002E6B RID: 11883 RVA: 0x000BFFF1 File Offset: 0x000BE1F1
		// (set) Token: 0x06002E6C RID: 11884 RVA: 0x000BFFF9 File Offset: 0x000BE1F9
		public MenuContext MenuContext
		{
			get
			{
				return this._menuContext;
			}
			private set
			{
				this._menuContext = value;
			}
		}

		// Token: 0x17000B10 RID: 2832
		// (get) Token: 0x06002E6D RID: 11885 RVA: 0x000C0002 File Offset: 0x000BE202
		// (set) Token: 0x06002E6E RID: 11886 RVA: 0x000C0013 File Offset: 0x000BE213
		public string GameMenuId
		{
			get
			{
				return Campaign.Current.MapStateData.GameMenuId;
			}
			set
			{
				Campaign.Current.MapStateData.GameMenuId = value;
			}
		}

		// Token: 0x17000B11 RID: 2833
		// (get) Token: 0x06002E6F RID: 11887 RVA: 0x000C0025 File Offset: 0x000BE225
		public bool AtMenu
		{
			get
			{
				return this.MenuContext != null;
			}
		}

		// Token: 0x17000B12 RID: 2834
		// (get) Token: 0x06002E70 RID: 11888 RVA: 0x000C0030 File Offset: 0x000BE230
		public bool MapConversationActive
		{
			get
			{
				return this._mapConversationActive;
			}
		}

		// Token: 0x17000B13 RID: 2835
		// (get) Token: 0x06002E71 RID: 11889 RVA: 0x000C0038 File Offset: 0x000BE238
		// (set) Token: 0x06002E72 RID: 11890 RVA: 0x000C0040 File Offset: 0x000BE240
		public IMapStateHandler Handler
		{
			get
			{
				return this._handler;
			}
			set
			{
				this._handler = value;
			}
		}

		// Token: 0x17000B14 RID: 2836
		// (get) Token: 0x06002E73 RID: 11891 RVA: 0x000C0049 File Offset: 0x000BE249
		public bool IsSimulationActive
		{
			get
			{
				return this._battleSimulation != null;
			}
		}

		// Token: 0x06002E74 RID: 11892 RVA: 0x000C0054 File Offset: 0x000BE254
		protected override void OnIdleTick(float dt)
		{
			base.OnIdleTick(dt);
			IMapStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnIdleTick(dt);
		}

		// Token: 0x06002E75 RID: 11893 RVA: 0x000C006E File Offset: 0x000BE26E
		private void RefreshHandler()
		{
			IMapStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnRefreshState();
		}

		// Token: 0x06002E76 RID: 11894 RVA: 0x000C0080 File Offset: 0x000BE280
		public void OnJoinArmy()
		{
			this.RefreshHandler();
		}

		// Token: 0x06002E77 RID: 11895 RVA: 0x000C0088 File Offset: 0x000BE288
		public void OnLeaveArmy()
		{
			this.RefreshHandler();
		}

		// Token: 0x06002E78 RID: 11896 RVA: 0x000C0090 File Offset: 0x000BE290
		public void OnDispersePlayerLeadedArmy()
		{
			this.RefreshHandler();
		}

		// Token: 0x06002E79 RID: 11897 RVA: 0x000C0098 File Offset: 0x000BE298
		public void OnArmyCreated(MobileParty mobileParty)
		{
			this.RefreshHandler();
		}

		// Token: 0x06002E7A RID: 11898 RVA: 0x000C00A0 File Offset: 0x000BE2A0
		public void OnMainPartyEncounter()
		{
			IMapStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnMainPartyEncounter();
		}

		// Token: 0x06002E7B RID: 11899 RVA: 0x000C00B2 File Offset: 0x000BE2B2
		public void ProcessTravel(Vec2 point)
		{
			MobileParty.MainParty.Ai.ForceAiNoPathMode = false;
			MobileParty.MainParty.Ai.SetMoveGoToPoint(point);
		}

		// Token: 0x06002E7C RID: 11900 RVA: 0x000C00D4 File Offset: 0x000BE2D4
		public void ProcessTravel(PartyBase party)
		{
			if (party.IsMobile)
			{
				MobileParty.MainParty.Ai.SetMoveEngageParty(party.MobileParty);
			}
			else
			{
				MobileParty.MainParty.Ai.SetMoveGoToSettlement(party.Settlement);
			}
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.StoppablePlay;
			MobileParty.MainParty.Ai.ForceAiNoPathMode = false;
		}

		// Token: 0x06002E7D RID: 11901 RVA: 0x000C0130 File Offset: 0x000BE330
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (Campaign.Current.SaveHandler.IsSaving)
			{
				Campaign.Current.SaveHandler.SaveTick();
				return;
			}
			if (this._battleSimulation != null)
			{
				this._battleSimulation.Tick(dt);
			}
			else if (this.AtMenu)
			{
				this.OnMenuModeTick(dt);
			}
			this.OnMapModeTick(dt);
			if (!Campaign.Current.SaveHandler.IsSaving)
			{
				Campaign.Current.SaveHandler.CampaignTick();
			}
		}

		// Token: 0x06002E7E RID: 11902 RVA: 0x000C01B1 File Offset: 0x000BE3B1
		private void OnMenuModeTick(float dt)
		{
			this.MenuContext.OnTick(dt);
			IMapStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnMenuModeTick(dt);
		}

		// Token: 0x06002E7F RID: 11903 RVA: 0x000C01D0 File Offset: 0x000BE3D0
		private void OnMapModeTick(float dt)
		{
			if (this._closeScreenNextFrame)
			{
				Game.Current.GameStateManager.CleanStates(0);
				return;
			}
			if (this.Handler != null)
			{
				this.Handler.BeforeTick(dt);
			}
			if (Campaign.Current != null && base.GameStateManager.ActiveState == this)
			{
				Campaign.Current.RealTick(dt);
				IMapStateHandler handler = this.Handler;
				if (handler != null)
				{
					handler.Tick(dt);
				}
				IMapStateHandler handler2 = this.Handler;
				if (handler2 != null)
				{
					handler2.AfterTick(dt);
				}
				Campaign.Current.Tick();
				IMapStateHandler handler3 = this.Handler;
				if (handler3 == null)
				{
					return;
				}
				handler3.AfterWaitTick(dt);
			}
		}

		// Token: 0x06002E80 RID: 11904 RVA: 0x000C026C File Offset: 0x000BE46C
		public void OnLoadingFinished()
		{
			if (!string.IsNullOrEmpty(this.GameMenuId))
			{
				this.EnterMenuMode();
			}
			this.RefreshHandler();
			if (Campaign.Current.CurrentMenuContext != null && Campaign.Current.CurrentMenuContext.GameMenu != null && Campaign.Current.CurrentMenuContext.GameMenu.IsWaitMenu)
			{
				Campaign.Current.CurrentMenuContext.GameMenu.StartWait();
			}
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
		}

		// Token: 0x06002E81 RID: 11905 RVA: 0x000C02E4 File Offset: 0x000BE4E4
		public void OnMapConversationStarts(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData)
		{
			this._mapConversationActive = true;
			IMapStateHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnMapConversationStarts(playerCharacterData, conversationPartnerData);
		}

		// Token: 0x06002E82 RID: 11906 RVA: 0x000C0300 File Offset: 0x000BE500
		public void OnMapConversationOver()
		{
			IMapStateHandler handler = this._handler;
			if (handler != null)
			{
				handler.OnMapConversationOver();
			}
			this._mapConversationActive = false;
			if (Game.Current.GameStateManager.ActiveState is MapState)
			{
				MenuContext menuContext = this.MenuContext;
				if (menuContext != null)
				{
					menuContext.Refresh();
				}
			}
			this.RefreshHandler();
		}

		// Token: 0x06002E83 RID: 11907 RVA: 0x000C0352 File Offset: 0x000BE552
		internal void OnSignalPeriodicEvents()
		{
			IMapStateHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnSignalPeriodicEvents();
		}

		// Token: 0x06002E84 RID: 11908 RVA: 0x000C0364 File Offset: 0x000BE564
		internal void OnHourlyTick()
		{
			IMapStateHandler handler = this._handler;
			if (handler != null)
			{
				handler.OnHourlyTick();
			}
			MenuContext menuContext = this.MenuContext;
			if (menuContext == null)
			{
				return;
			}
			menuContext.OnHourlyTick();
		}

		// Token: 0x06002E85 RID: 11909 RVA: 0x000C0387 File Offset: 0x000BE587
		protected override void OnActivate()
		{
			base.OnActivate();
			MenuContext menuContext = this.MenuContext;
			if (menuContext != null)
			{
				menuContext.Refresh();
			}
			this.RefreshHandler();
		}

		// Token: 0x06002E86 RID: 11910 RVA: 0x000C03A6 File Offset: 0x000BE5A6
		public void EnterMenuMode()
		{
			this.MenuContext = MBObjectManager.Instance.CreateObject<MenuContext>();
			IMapStateHandler handler = this._handler;
			if (handler != null)
			{
				handler.OnEnteringMenuMode(this.MenuContext);
			}
			this.MenuContext.Refresh();
		}

		// Token: 0x06002E87 RID: 11911 RVA: 0x000C03DA File Offset: 0x000BE5DA
		public void ExitMenuMode()
		{
			IMapStateHandler handler = this._handler;
			if (handler != null)
			{
				handler.OnExitingMenuMode();
			}
			this.MenuContext.Destroy();
			MBObjectManager.Instance.UnregisterObject(this.MenuContext);
			this.MenuContext = null;
		}

		// Token: 0x06002E88 RID: 11912 RVA: 0x000C040F File Offset: 0x000BE60F
		public void StartBattleSimulation()
		{
			this._battleSimulation = PlayerEncounter.Current.BattleSimulation;
			IMapStateHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnBattleSimulationStarted(this._battleSimulation);
		}

		// Token: 0x06002E89 RID: 11913 RVA: 0x000C0437 File Offset: 0x000BE637
		public void EndBattleSimulation()
		{
			this._battleSimulation = null;
			IMapStateHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnBattleSimulationEnded();
		}

		// Token: 0x06002E8A RID: 11914 RVA: 0x000C0450 File Offset: 0x000BE650
		public void OnPlayerSiegeActivated()
		{
			IMapStateHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPlayerSiegeActivated();
		}

		// Token: 0x06002E8B RID: 11915 RVA: 0x000C0462 File Offset: 0x000BE662
		public void OnPlayerSiegeDeactivated()
		{
			IMapStateHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPlayerSiegeDeactivated();
		}

		// Token: 0x06002E8C RID: 11916 RVA: 0x000C0474 File Offset: 0x000BE674
		public void OnSiegeEngineClick(MatrixFrame siegeEngineFrame)
		{
			IMapStateHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnSiegeEngineClick(siegeEngineFrame);
		}

		// Token: 0x04000DF4 RID: 3572
		private MenuContext _menuContext;

		// Token: 0x04000DF5 RID: 3573
		private bool _mapConversationActive;

		// Token: 0x04000DF6 RID: 3574
		private bool _closeScreenNextFrame;

		// Token: 0x04000DF7 RID: 3575
		private IMapStateHandler _handler;

		// Token: 0x04000DF8 RID: 3576
		private BattleSimulation _battleSimulation;
	}
}
