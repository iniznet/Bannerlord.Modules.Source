using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Launcher.Library.CustomWidgets
{
	public class LauncherBoolBrushWidget : BrushWidget
	{
		public LauncherBoolBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			this.BoolVariableUpdated();
		}

		private void BoolVariableUpdated()
		{
			(this.TargetWidget ?? this).Brush = (this.BoolVariable ? this.OnTrueBrush : this.OnFalseBrush);
		}

		[DataSourceProperty]
		public bool BoolVariable
		{
			get
			{
				return this._boolVariable;
			}
			set
			{
				if (value != this._boolVariable)
				{
					this._boolVariable = value;
					base.OnPropertyChanged(value, "BoolVariable");
					this.BoolVariableUpdated();
				}
			}
		}

		[DataSourceProperty]
		public BrushWidget TargetWidget
		{
			get
			{
				return this._targetWidget;
			}
			set
			{
				if (value != this._targetWidget)
				{
					this._targetWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "TargetWidget");
				}
			}
		}

		[DataSourceProperty]
		public Brush OnTrueBrush
		{
			get
			{
				return this._onTrueBrush;
			}
			set
			{
				if (value != this._onTrueBrush)
				{
					this._onTrueBrush = value;
					base.OnPropertyChanged<Brush>(value, "OnTrueBrush");
				}
			}
		}

		[DataSourceProperty]
		public Brush OnFalseBrush
		{
			get
			{
				return this._onFalseBrush;
			}
			set
			{
				if (value != this._onFalseBrush)
				{
					this._onFalseBrush = value;
					base.OnPropertyChanged<Brush>(value, "OnFalseBrush");
				}
			}
		}

		private bool _boolVariable;

		private BrushWidget _targetWidget;

		private Brush _onTrueBrush;

		private Brush _onFalseBrush;
	}
}
