using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	public class CraftingDifficultyBarParentWidget : Widget
	{
		public CraftingDifficultyBarParentWidget(UIContext context)
			: base(context)
		{
		}

		private void OnWidgetPositionUpdated(PropertyOwnerObject ownerObject, string propertyName, object value)
		{
			if (propertyName == "Text")
			{
				this._areOffsetsDirty = true;
			}
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.SmithingLevelTextWidget != null && this.OrderDifficultyTextWidget != null)
			{
				if (this._updatePositions)
				{
					TextWidget textWidget = ((this.OrderDifficulty < this.SmithingLevel) ? this.SmithingLevelTextWidget : this.OrderDifficultyTextWidget);
					TextWidget textWidget2 = ((textWidget == this.SmithingLevelTextWidget) ? this.OrderDifficultyTextWidget : this.SmithingLevelTextWidget);
					if (textWidget.GlobalPosition.Y + (textWidget.Size.Y + this._offsetIntolerance) >= textWidget2.GlobalPosition.Y)
					{
						textWidget.PositionYOffset = -textWidget.Size.Y;
						textWidget2.PositionYOffset = 0f;
					}
					else
					{
						textWidget.PositionYOffset = 0f;
						textWidget2.PositionYOffset = 0f;
					}
					this._updatePositions = false;
				}
				if (this._areOffsetsDirty)
				{
					this.SmithingLevelTextWidget.PositionYOffset = 0f;
					this.OrderDifficultyTextWidget.PositionYOffset = 0f;
					this._updatePositions = true;
					this._areOffsetsDirty = false;
				}
			}
		}

		public int OrderDifficulty { get; set; }

		public int SmithingLevel { get; set; }

		public TextWidget SmithingLevelTextWidget
		{
			get
			{
				return this._smithingLevelTextWidget;
			}
			set
			{
				if (value != this._smithingLevelTextWidget)
				{
					this._smithingLevelTextWidget = value;
					this._smithingLevelTextWidget.PropertyChanged += this.OnWidgetPositionUpdated;
				}
			}
		}

		public TextWidget OrderDifficultyTextWidget
		{
			get
			{
				return this._orderDifficultyTextWidget;
			}
			set
			{
				if (value != this._orderDifficultyTextWidget)
				{
					this._orderDifficultyTextWidget = value;
					this._orderDifficultyTextWidget.PropertyChanged += this.OnWidgetPositionUpdated;
				}
			}
		}

		private float _offsetIntolerance = 3f;

		private bool _areOffsetsDirty;

		private bool _updatePositions;

		private TextWidget _smithingLevelTextWidget;

		private TextWidget _orderDifficultyTextWidget;
	}
}
