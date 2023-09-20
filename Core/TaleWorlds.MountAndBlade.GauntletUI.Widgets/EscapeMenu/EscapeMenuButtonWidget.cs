using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.EscapeMenu
{
	public class EscapeMenuButtonWidget : ButtonWidget
	{
		public EscapeMenuButtonWidget(UIContext context)
			: base(context)
		{
		}

		private void PositiveBehavioredStateUpdated()
		{
			if (this.IsPositiveBehaviored)
			{
				base.Brush = this.PositiveBehaviorBrush;
			}
		}

		[Editor(false)]
		public bool IsPositiveBehaviored
		{
			get
			{
				return this._isPositiveBehaviored;
			}
			set
			{
				if (this._isPositiveBehaviored != value)
				{
					this._isPositiveBehaviored = value;
					base.OnPropertyChanged(value, "IsPositiveBehaviored");
					this.PositiveBehavioredStateUpdated();
				}
			}
		}

		[Editor(false)]
		public Brush PositiveBehaviorBrush
		{
			get
			{
				return this._positiveBehaviorBrush;
			}
			set
			{
				if (this._positiveBehaviorBrush != value)
				{
					this._positiveBehaviorBrush = value;
					base.OnPropertyChanged<Brush>(value, "PositiveBehaviorBrush");
				}
			}
		}

		private bool _isPositiveBehaviored;

		private Brush _positiveBehaviorBrush;
	}
}
