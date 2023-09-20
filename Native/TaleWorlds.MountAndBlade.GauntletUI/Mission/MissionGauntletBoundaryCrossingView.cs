using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission
{
	// Token: 0x02000024 RID: 36
	[OverrideView(typeof(MissionBoundaryCrossingView))]
	public class MissionGauntletBoundaryCrossingView : MissionGauntletBattleUIBase
	{
		// Token: 0x060001A8 RID: 424 RVA: 0x000090C7 File Offset: 0x000072C7
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x000090D0 File Offset: 0x000072D0
		protected override void OnCreateView()
		{
			this._dataSource = new BoundaryCrossingVM(base.Mission, new Action<bool>(this.OnEscapeMenuToggled));
			this._gauntletLayer = new GauntletLayer(47, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("BoundaryCrossing", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00009135 File Offset: 0x00007335
		protected override void OnDestroyView()
		{
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00009150 File Offset: 0x00007350
		private void OnEscapeMenuToggled(bool isOpened)
		{
			if (base.IsViewActive)
			{
				ScreenManager.SetSuspendLayer(this._gauntletLayer, !isOpened);
			}
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00009169 File Offset: 0x00007369
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			if (base.IsViewActive)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
			}
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000918E File Offset: 0x0000738E
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			if (base.IsViewActive)
			{
				this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
			}
		}

		// Token: 0x040000C7 RID: 199
		private GauntletLayer _gauntletLayer;

		// Token: 0x040000C8 RID: 200
		private BoundaryCrossingVM _dataSource;
	}
}
