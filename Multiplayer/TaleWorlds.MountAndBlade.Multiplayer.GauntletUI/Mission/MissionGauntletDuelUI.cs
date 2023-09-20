using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.Multiplayer.View.MissionViews;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.Multiplayer.GauntletUI.Mission
{
	[OverrideView(typeof(MissionMultiplayerDuelUI))]
	public class MissionGauntletDuelUI : MissionView
	{
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this.ViewOrderPriority = 15;
			this._client = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeDuelClient>();
			this._dataSource = new MultiplayerDuelVM(base.MissionScreen.CombatCamera, this._client);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MultiplayerDuel", this._dataSource);
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._mpMissionCategory = spriteData.SpriteCategories["ui_mpmission"];
			this._mpMissionCategory.Load(resourceContext, uiresourceDepot);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			this._equipmentController = base.Mission.GetMissionBehavior<MissionLobbyEquipmentNetworkComponent>();
			this._equipmentController.OnEquipmentRefreshed += new MissionLobbyEquipmentNetworkComponent.OnRefreshEquipmentEventDelegate(this.OnEquipmentRefreshed);
			MissionPeer.OnEquipmentIndexRefreshed += new MissionPeer.OnUpdateEquipmentSetIndexEventDelegate(this.OnPeerEquipmentIndexRefreshed);
			this._lobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this._lobbyComponent.OnPostMatchEnded += this.OnPostMatchEnded;
			NativeOptions.OnNativeOptionChanged = (NativeOptions.OnNativeOptionChangedDelegate)Delegate.Combine(NativeOptions.OnNativeOptionChanged, new NativeOptions.OnNativeOptionChangedDelegate(this.OnNativeOptionChanged));
			this._dataSource.IsEnabled = true;
			this._isPeerEquipmentsDirty = true;
		}

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			SpriteCategory mpMissionCategory = this._mpMissionCategory;
			if (mpMissionCategory != null)
			{
				mpMissionCategory.Unload();
			}
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._gauntletLayer = null;
			this._equipmentController.OnEquipmentRefreshed -= new MissionLobbyEquipmentNetworkComponent.OnRefreshEquipmentEventDelegate(this.OnEquipmentRefreshed);
			MissionPeer.OnEquipmentIndexRefreshed -= new MissionPeer.OnUpdateEquipmentSetIndexEventDelegate(this.OnPeerEquipmentIndexRefreshed);
			this._lobbyComponent.OnPostMatchEnded -= this.OnPostMatchEnded;
			NativeOptions.OnNativeOptionChanged = (NativeOptions.OnNativeOptionChangedDelegate)Delegate.Remove(NativeOptions.OnNativeOptionChanged, new NativeOptions.OnNativeOptionChangedDelegate(this.OnNativeOptionChanged));
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this._dataSource.Tick(dt);
			DuelMissionRepresentative myRepresentative = this._client.MyRepresentative;
			if (((myRepresentative != null) ? myRepresentative.ControlledAgent : null) != null && base.Input.IsGameKeyReleased(13))
			{
				this._client.MyRepresentative.OnInteraction();
			}
			if (this._isPeerEquipmentsDirty)
			{
				this._dataSource.Markers.RefreshPeerEquipments();
				this._isPeerEquipmentsDirty = false;
			}
		}

		private void OnNativeOptionChanged(NativeOptions.NativeOptionsType optionType)
		{
			if (optionType == 23)
			{
				this._dataSource.OnScreenResolutionChanged();
			}
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
			if (affectedAgent == Agent.Main)
			{
				this._dataSource.OnMainAgentRemoved();
			}
		}

		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			base.OnAgentBuild(agent, banner);
			if (agent == Agent.Main)
			{
				this._dataSource.OnMainAgentBuild();
			}
		}

		public override void OnFocusGained(Agent agent, IFocusable focusableObject, bool isInteractable)
		{
			base.OnFocusGained(agent, focusableObject, isInteractable);
			if (!(focusableObject is DuelZoneLandmark) && !(focusableObject is Agent))
			{
				this._dataSource.Markers.OnFocusGained();
			}
		}

		public override void OnFocusLost(Agent agent, IFocusable focusableObject)
		{
			base.OnFocusLost(agent, focusableObject);
			if (!(focusableObject is DuelZoneLandmark) && !(focusableObject is Agent))
			{
				this._dataSource.Markers.OnFocusLost();
			}
		}

		public void OnPeerEquipmentIndexRefreshed(MissionPeer peer, int equipmentSetIndex)
		{
			this._dataSource.Markers.OnPeerEquipmentRefreshed(peer);
		}

		private void OnEquipmentRefreshed(MissionPeer peer)
		{
			this._dataSource.Markers.OnPeerEquipmentRefreshed(peer);
		}

		private void OnPostMatchEnded()
		{
			this._dataSource.IsEnabled = false;
		}

		private MultiplayerDuelVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private SpriteCategory _mpMissionCategory;

		private MissionMultiplayerGameModeDuelClient _client;

		private MissionLobbyEquipmentNetworkComponent _equipmentController;

		private MissionLobbyComponent _lobbyComponent;

		private bool _isPeerEquipmentsDirty;
	}
}
