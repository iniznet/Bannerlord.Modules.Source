using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.TeamSelection;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	// Token: 0x0200003A RID: 58
	[OverrideView(typeof(MultiplayerCultureSelectUIHandler))]
	public class MissionGauntletCultureSelection : MissionView
	{
		// Token: 0x060002CE RID: 718 RVA: 0x0000FE1C File Offset: 0x0000E01C
		public MissionGauntletCultureSelection()
		{
			this.ViewOrderPriority = 22;
		}

		// Token: 0x060002CF RID: 719 RVA: 0x0000FE2C File Offset: 0x0000E02C
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._missionLobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this._missionLobbyComponent.OnCultureSelectionRequested += this.OnCultureSelectionRequested;
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x0000FE5C File Offset: 0x0000E05C
		public override void OnMissionScreenFinalize()
		{
			this._missionLobbyComponent.OnCultureSelectionRequested -= this.OnCultureSelectionRequested;
			base.OnMissionScreenFinalize();
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0000FE7B File Offset: 0x0000E07B
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this._toOpen && base.MissionScreen.SetDisplayDialog(true))
			{
				this._toOpen = false;
				this.OnOpen();
			}
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x0000FEA8 File Offset: 0x0000E0A8
		private void OnOpen()
		{
			this._dataSource = new MultiplayerCultureSelectVM(new Action<BasicCultureObject>(this.OnCultureSelected), new Action(this.OnClose));
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MultiplayerCultureSelection", this._dataSource);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x0000FF3C File Offset: 0x0000E13C
		private void OnClose()
		{
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			base.MissionScreen.SetDisplayDialog(false);
			this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x0000FF90 File Offset: 0x0000E190
		private void OnCultureSelectionRequested()
		{
			this._toOpen = true;
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x0000FF99 File Offset: 0x0000E199
		private void OnCultureSelected(BasicCultureObject culture)
		{
			this._missionLobbyComponent.OnCultureSelected(culture);
			this.OnClose();
		}

		// Token: 0x04000175 RID: 373
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000176 RID: 374
		private MultiplayerCultureSelectVM _dataSource;

		// Token: 0x04000177 RID: 375
		private MissionLobbyComponent _missionLobbyComponent;

		// Token: 0x04000178 RID: 376
		private bool _toOpen;
	}
}
