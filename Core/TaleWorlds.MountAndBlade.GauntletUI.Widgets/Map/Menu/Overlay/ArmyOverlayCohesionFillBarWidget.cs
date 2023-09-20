using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Menu.Overlay
{
	public class ArmyOverlayCohesionFillBarWidget : FillBarWidget
	{
		public ArmyOverlayCohesionFillBarWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._isWarningDirty)
			{
				this.DetermineBarAnimState();
				this._isWarningDirty = false;
			}
		}

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

		private bool _isWarningDirty = true;

		private bool _isCohesionWarningEnabled;

		private bool _isArmyLeader;
	}
}
