using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.HUDExtensions;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	// Token: 0x02000042 RID: 66
	[OverrideView(typeof(MissionMultiplayerHUDExtensionUIHandler))]
	public class MissionGauntletMultiplayerHUDExtension : MissionView
	{
		// Token: 0x0600030B RID: 779 RVA: 0x00010FBC File Offset: 0x0000F1BC
		public MissionGauntletMultiplayerHUDExtension()
		{
			this.ViewOrderPriority = 2;
		}

		// Token: 0x0600030C RID: 780 RVA: 0x00010FCC File Offset: 0x0000F1CC
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._mpMissionCategory = spriteData.SpriteCategories["ui_mpmission"];
			this._mpMissionCategory.Load(resourceContext, uiresourceDepot);
			this._dataSource = new MissionMultiplayerHUDExtensionVM(base.Mission);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("HUDExtension", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			base.MissionScreen.OnSpectateAgentFocusIn += this._dataSource.OnSpectatedAgentFocusIn;
			base.MissionScreen.OnSpectateAgentFocusOut += this._dataSource.OnSpectatedAgentFocusOut;
			Game.Current.EventManager.RegisterEvent<MissionPlayerToggledOrderViewEvent>(new Action<MissionPlayerToggledOrderViewEvent>(this.OnMissionPlayerToggledOrderViewEvent));
			this._lobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this._lobbyComponent.OnPostMatchEnded += this.OnPostMatchEnded;
		}

		// Token: 0x0600030D RID: 781 RVA: 0x000110E0 File Offset: 0x0000F2E0
		public override void OnMissionScreenFinalize()
		{
			this._lobbyComponent.OnPostMatchEnded -= this.OnPostMatchEnded;
			base.MissionScreen.OnSpectateAgentFocusIn -= this._dataSource.OnSpectatedAgentFocusIn;
			base.MissionScreen.OnSpectateAgentFocusOut -= this._dataSource.OnSpectatedAgentFocusOut;
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			SpriteCategory mpMissionCategory = this._mpMissionCategory;
			if (mpMissionCategory != null)
			{
				mpMissionCategory.Unload();
			}
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._gauntletLayer = null;
			Game.Current.EventManager.UnregisterEvent<MissionPlayerToggledOrderViewEvent>(new Action<MissionPlayerToggledOrderViewEvent>(this.OnMissionPlayerToggledOrderViewEvent));
			base.OnMissionScreenFinalize();
		}

		// Token: 0x0600030E RID: 782 RVA: 0x00011198 File Offset: 0x0000F398
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this._dataSource.Tick(dt);
		}

		// Token: 0x0600030F RID: 783 RVA: 0x000111AD File Offset: 0x0000F3AD
		private void OnMissionPlayerToggledOrderViewEvent(MissionPlayerToggledOrderViewEvent eventObj)
		{
			this._dataSource.IsOrderActive = eventObj.IsOrderEnabled;
		}

		// Token: 0x06000310 RID: 784 RVA: 0x000111C0 File Offset: 0x0000F3C0
		private void OnPostMatchEnded()
		{
			this._dataSource.ShowHud = false;
		}

		// Token: 0x0400019D RID: 413
		private MissionMultiplayerHUDExtensionVM _dataSource;

		// Token: 0x0400019E RID: 414
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400019F RID: 415
		private SpriteCategory _mpMissionCategory;

		// Token: 0x040001A0 RID: 416
		private MissionLobbyComponent _lobbyComponent;
	}
}
