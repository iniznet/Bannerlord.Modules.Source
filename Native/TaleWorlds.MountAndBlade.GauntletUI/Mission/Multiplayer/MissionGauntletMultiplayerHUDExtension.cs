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
	[OverrideView(typeof(MissionMultiplayerHUDExtensionUIHandler))]
	public class MissionGauntletMultiplayerHUDExtension : MissionView
	{
		public MissionGauntletMultiplayerHUDExtension()
		{
			this.ViewOrderPriority = 2;
		}

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

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this._dataSource.Tick(dt);
		}

		private void OnMissionPlayerToggledOrderViewEvent(MissionPlayerToggledOrderViewEvent eventObj)
		{
			this._dataSource.IsOrderActive = eventObj.IsOrderEnabled;
		}

		private void OnPostMatchEnded()
		{
			this._dataSource.ShowHud = false;
		}

		private MissionMultiplayerHUDExtensionVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private SpriteCategory _mpMissionCategory;

		private MissionLobbyComponent _lobbyComponent;
	}
}
