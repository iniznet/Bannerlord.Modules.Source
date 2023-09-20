using System;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.GameState
{
	public class MapState : GameState
	{
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

		public bool AtMenu
		{
			get
			{
				return this.MenuContext != null;
			}
		}

		public bool MapConversationActive
		{
			get
			{
				return this._mapConversationActive;
			}
		}

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

		public bool IsSimulationActive
		{
			get
			{
				return this._battleSimulation != null;
			}
		}

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

		private void RefreshHandler()
		{
			IMapStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnRefreshState();
		}

		public void OnJoinArmy()
		{
			this.RefreshHandler();
		}

		public void OnLeaveArmy()
		{
			this.RefreshHandler();
		}

		public void OnDispersePlayerLeadedArmy()
		{
			this.RefreshHandler();
		}

		public void OnArmyCreated(MobileParty mobileParty)
		{
			this.RefreshHandler();
		}

		public void OnMainPartyEncounter()
		{
			IMapStateHandler handler = this.Handler;
			if (handler == null)
			{
				return;
			}
			handler.OnMainPartyEncounter();
		}

		public void ProcessTravel(Vec2 point)
		{
			MobileParty.MainParty.Ai.ForceAiNoPathMode = false;
			MobileParty.MainParty.Ai.SetMoveGoToPoint(point);
		}

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

		internal void OnSignalPeriodicEvents()
		{
			IMapStateHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnSignalPeriodicEvents();
		}

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

		public void OnPlayerSiegeActivated()
		{
			IMapStateHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPlayerSiegeActivated();
		}

		public void OnPlayerSiegeDeactivated()
		{
			IMapStateHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPlayerSiegeDeactivated();
		}

		public void OnSiegeEngineClick(MatrixFrame siegeEngineFrame)
		{
			IMapStateHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnSiegeEngineClick(siegeEngineFrame);
		}

		private MenuContext _menuContext;

		private bool _mapConversationActive;

		private bool _closeScreenNextFrame;

		private IMapStateHandler _handler;

		private BattleSimulation _battleSimulation;
	}
}
