using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	// Token: 0x02000038 RID: 56
	[OverrideView(typeof(MultiplayerAdminPanelUIHandler))]
	public class MissionGauntletAdminPanel : MissionView
	{
		// Token: 0x060002B7 RID: 695 RVA: 0x0000F78A File Offset: 0x0000D98A
		public MissionGauntletAdminPanel()
		{
			this.ViewOrderPriority = 45;
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x0000F79C File Offset: 0x0000D99C
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._multiplayerAdminComponent = base.Mission.GetMissionBehavior<MultiplayerAdminComponent>();
			this._dataSource = new MultiplayerAdminPanelVM(new Action<bool>(this.OnEscapeMenuToggled), this._multiplayerAdminComponent);
			this._multiplayerAdminComponent.OnShowAdminMenu += new MultiplayerAdminComponent.OnShowAdminMenuDelegate(this.OnShowAdminPanel);
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0000F7F4 File Offset: 0x0000D9F4
		public override void OnMissionScreenFinalize()
		{
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this.OnEscapeMenuToggled(false);
			base.OnMissionScreenFinalize();
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0000F815 File Offset: 0x0000DA15
		public override bool OnEscape()
		{
			if (this._isActive)
			{
				this.OnExitAdminPanel();
				return true;
			}
			return base.OnEscape();
		}

		// Token: 0x060002BB RID: 699 RVA: 0x0000F82D File Offset: 0x0000DA2D
		private void OnExitAdminPanel()
		{
			this.OnEscapeMenuToggled(false);
		}

		// Token: 0x060002BC RID: 700 RVA: 0x0000F836 File Offset: 0x0000DA36
		private void OnShowAdminPanel()
		{
			this.OnEscapeMenuToggled(true);
		}

		// Token: 0x060002BD RID: 701 RVA: 0x0000F840 File Offset: 0x0000DA40
		private void OnEscapeMenuToggled(bool isOpened)
		{
			if (isOpened == this._isActive || !base.MissionScreen.SetDisplayDialog(isOpened))
			{
				return;
			}
			this._isActive = isOpened;
			if (isOpened)
			{
				this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
				this._movie = this._gauntletLayer.LoadMovie("Lobby.HostGame", this._dataSource);
				this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 1);
				base.MissionScreen.AddLayer(this._gauntletLayer);
				return;
			}
			this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
		}

		// Token: 0x04000164 RID: 356
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000165 RID: 357
		private MultiplayerAdminPanelVM _dataSource;

		// Token: 0x04000166 RID: 358
		private IGauntletMovie _movie;

		// Token: 0x04000167 RID: 359
		private bool _isActive;

		// Token: 0x04000168 RID: 360
		private MultiplayerAdminComponent _multiplayerAdminComponent;
	}
}
