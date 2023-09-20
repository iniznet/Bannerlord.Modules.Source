using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Quest
{
	public class QuestStageItemWidget : Widget
	{
		public QuestStageItemWidget(UIContext context)
			: base(context)
		{
			this._firstFrame = true;
		}

		protected override void OnParallelUpdate(float dt)
		{
			base.OnParallelUpdate(dt);
			this._previousHoverBegan = this._hoverBegan;
			if (!this._firstFrame && this.IsNew)
			{
				bool flag = this.IsMouseOverWidget();
				if (flag && !this._hoverBegan)
				{
					this._hoverBegan = true;
				}
				else if (!flag && this._hoverBegan)
				{
					this._hoverBegan = false;
				}
			}
			this._firstFrame = false;
			if (this._previousHoverBegan && !this._hoverBegan)
			{
				base.EventFired("ResetGlow", Array.Empty<object>());
			}
		}

		private bool IsMouseOverWidget()
		{
			Vector2 globalPosition = base.GlobalPosition;
			return this.IsBetween(base.EventManager.MousePosition.X, globalPosition.X, globalPosition.X + base.Size.X) && this.IsBetween(base.EventManager.MousePosition.Y, globalPosition.Y, globalPosition.Y + base.Size.Y);
		}

		private bool IsBetween(float number, float min, float max)
		{
			return number >= min && number <= max;
		}

		[Editor(false)]
		public bool IsNew
		{
			get
			{
				return this._isNew;
			}
			set
			{
				if (this._isNew != value)
				{
					this._isNew = value;
					base.OnPropertyChanged(value, "IsNew");
				}
			}
		}

		private bool _firstFrame;

		private bool _previousHoverBegan;

		private bool _hoverBegan;

		private bool _isNew;
	}
}
