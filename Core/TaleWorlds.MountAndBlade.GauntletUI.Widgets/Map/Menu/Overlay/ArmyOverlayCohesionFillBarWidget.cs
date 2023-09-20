using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Menu.Overlay
{
	// Token: 0x02000104 RID: 260
	public class ArmyOverlayCohesionFillBarWidget : FillBarWidget
	{
		// Token: 0x06000D7B RID: 3451 RVA: 0x00025DDA File Offset: 0x00023FDA
		public ArmyOverlayCohesionFillBarWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000D7C RID: 3452 RVA: 0x00025DEA File Offset: 0x00023FEA
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._isWarningDirty)
			{
				this.DetermineBarAnimState();
				this._isWarningDirty = false;
			}
		}

		// Token: 0x06000D7D RID: 3453 RVA: 0x00025E08 File Offset: 0x00024008
		private void DetermineBarAnimState()
		{
			BrushWidget brushWidget;
			if (base.FillWidget != null && (brushWidget = base.FillWidget as BrushWidget) != null)
			{
				brushWidget.RegisterBrushStatesOfWidget();
				if (this.IsCohesionWarningEnabled)
				{
					if (brushWidget.CurrentState == "WarningLeader")
					{
						brushWidget.BrushRenderer.RestartAnimation();
						return;
					}
					if (this.IsArmyLeader)
					{
						brushWidget.SetState("WarningLeader");
						return;
					}
					brushWidget.SetState("WarningNormal");
					return;
				}
				else
				{
					if (brushWidget.CurrentState == "Default")
					{
						brushWidget.BrushRenderer.RestartAnimation();
						return;
					}
					brushWidget.SetState("Default");
				}
			}
		}

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x06000D7E RID: 3454 RVA: 0x00025EA4 File Offset: 0x000240A4
		// (set) Token: 0x06000D7F RID: 3455 RVA: 0x00025EAC File Offset: 0x000240AC
		[Editor(false)]
		public bool IsCohesionWarningEnabled
		{
			get
			{
				return this._isCohesionWarningEnabled;
			}
			set
			{
				if (value != this._isCohesionWarningEnabled)
				{
					this._isCohesionWarningEnabled = value;
					base.OnPropertyChanged(value, "IsCohesionWarningEnabled");
					this.DetermineBarAnimState();
					this._isWarningDirty = true;
				}
			}
		}

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x06000D80 RID: 3456 RVA: 0x00025ED7 File Offset: 0x000240D7
		// (set) Token: 0x06000D81 RID: 3457 RVA: 0x00025EDF File Offset: 0x000240DF
		[Editor(false)]
		public bool IsArmyLeader
		{
			get
			{
				return this._isArmyLeader;
			}
			set
			{
				if (value != this._isArmyLeader)
				{
					this._isArmyLeader = value;
					base.OnPropertyChanged(value, "IsArmyLeader");
					this.DetermineBarAnimState();
					this._isWarningDirty = true;
				}
			}
		}

		// Token: 0x0400063A RID: 1594
		private bool _isWarningDirty = true;

		// Token: 0x0400063B RID: 1595
		private bool _isCohesionWarningEnabled;

		// Token: 0x0400063C RID: 1596
		private bool _isArmyLeader;
	}
}
