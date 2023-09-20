using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	// Token: 0x0200003C RID: 60
	[OverrideView(typeof(MissionMultiplayerDuelUI))]
	public class MissionGauntletDuelUI : MissionView
	{
		// Token: 0x060002DC RID: 732 RVA: 0x0001013C File Offset: 0x0000E33C
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
			this._lobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this._lobbyComponent.OnPostMatchEnded += this.OnPostMatchEnded;
			NativeOptions.OnNativeOptionChanged = (NativeOptions.OnNativeOptionChangedDelegate)Delegate.Combine(NativeOptions.OnNativeOptionChanged, new NativeOptions.OnNativeOptionChangedDelegate(this.OnNativeOptionChanged));
			this._dataSource.IsEnabled = true;
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00010274 File Offset: 0x0000E474
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
			this._lobbyComponent.OnPostMatchEnded -= this.OnPostMatchEnded;
			NativeOptions.OnNativeOptionChanged = (NativeOptions.OnNativeOptionChangedDelegate)Delegate.Remove(NativeOptions.OnNativeOptionChanged, new NativeOptions.OnNativeOptionChangedDelegate(this.OnNativeOptionChanged));
		}

		// Token: 0x060002DE RID: 734 RVA: 0x00010310 File Offset: 0x0000E510
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this._dataSource.Tick(dt);
			DuelMissionRepresentative myRepresentative = this._client.MyRepresentative;
			if (((myRepresentative != null) ? myRepresentative.ControlledAgent : null) != null && base.Input.IsGameKeyReleased(13))
			{
				this._client.MyRepresentative.OnInteraction();
			}
		}

		// Token: 0x060002DF RID: 735 RVA: 0x00010368 File Offset: 0x0000E568
		private void OnNativeOptionChanged(NativeOptions.NativeOptionsType optionType)
		{
			if (optionType == 19)
			{
				this._dataSource.OnScreenResolutionChanged();
			}
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x0001037A File Offset: 0x0000E57A
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
			if (affectedAgent == Agent.Main)
			{
				this._dataSource.OnMainAgentRemoved();
			}
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x0001039A File Offset: 0x0000E59A
		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			base.OnAgentBuild(agent, banner);
			if (agent == Agent.Main)
			{
				this._dataSource.OnMainAgentBuild();
			}
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x000103B7 File Offset: 0x0000E5B7
		public override void OnFocusGained(Agent agent, IFocusable focusableObject, bool isInteractable)
		{
			base.OnFocusGained(agent, focusableObject, isInteractable);
			if (!(focusableObject is DuelZoneLandmark) && !(focusableObject is Agent))
			{
				this._dataSource.Markers.OnFocusGained();
			}
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x000103E2 File Offset: 0x0000E5E2
		public override void OnFocusLost(Agent agent, IFocusable focusableObject)
		{
			base.OnFocusLost(agent, focusableObject);
			if (!(focusableObject is DuelZoneLandmark) && !(focusableObject is Agent))
			{
				this._dataSource.Markers.OnFocusLost();
			}
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x0001040C File Offset: 0x0000E60C
		private void OnEquipmentRefreshed(MissionPeer peer)
		{
			this._dataSource.Markers.OnPeerEquipmentRefreshed(peer);
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x0001041F File Offset: 0x0000E61F
		private void OnPostMatchEnded()
		{
			this._dataSource.IsEnabled = false;
		}

		// Token: 0x0400017C RID: 380
		private MultiplayerDuelVM _dataSource;

		// Token: 0x0400017D RID: 381
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400017E RID: 382
		private SpriteCategory _mpMissionCategory;

		// Token: 0x0400017F RID: 383
		private MissionMultiplayerGameModeDuelClient _client;

		// Token: 0x04000180 RID: 384
		private MissionLobbyEquipmentNetworkComponent _equipmentController;

		// Token: 0x04000181 RID: 385
		private MissionLobbyComponent _lobbyComponent;
	}
}
