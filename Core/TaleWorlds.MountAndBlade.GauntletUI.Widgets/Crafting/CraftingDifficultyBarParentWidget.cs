using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	// Token: 0x02000149 RID: 329
	public class CraftingDifficultyBarParentWidget : Widget
	{
		// Token: 0x0600113D RID: 4413 RVA: 0x0002FBC1 File Offset: 0x0002DDC1
		public CraftingDifficultyBarParentWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600113E RID: 4414 RVA: 0x0002FBD5 File Offset: 0x0002DDD5
		private void OnWidgetPositionUpdated(PropertyOwnerObject ownerObject, string propertyName, object value)
		{
			if (propertyName == "Text")
			{
				this._areOffsetsDirty = true;
			}
		}

		// Token: 0x0600113F RID: 4415 RVA: 0x0002FBEC File Offset: 0x0002DDEC
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

		// Token: 0x1700061B RID: 1563
		// (get) Token: 0x06001140 RID: 4416 RVA: 0x0002FCF3 File Offset: 0x0002DEF3
		// (set) Token: 0x06001141 RID: 4417 RVA: 0x0002FCFB File Offset: 0x0002DEFB
		public int OrderDifficulty { get; set; }

		// Token: 0x1700061C RID: 1564
		// (get) Token: 0x06001142 RID: 4418 RVA: 0x0002FD04 File Offset: 0x0002DF04
		// (set) Token: 0x06001143 RID: 4419 RVA: 0x0002FD0C File Offset: 0x0002DF0C
		public int SmithingLevel { get; set; }

		// Token: 0x1700061D RID: 1565
		// (get) Token: 0x06001144 RID: 4420 RVA: 0x0002FD15 File Offset: 0x0002DF15
		// (set) Token: 0x06001145 RID: 4421 RVA: 0x0002FD1D File Offset: 0x0002DF1D
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

		// Token: 0x1700061E RID: 1566
		// (get) Token: 0x06001146 RID: 4422 RVA: 0x0002FD46 File Offset: 0x0002DF46
		// (set) Token: 0x06001147 RID: 4423 RVA: 0x0002FD4E File Offset: 0x0002DF4E
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

		// Token: 0x040007E7 RID: 2023
		private float _offsetIntolerance = 3f;

		// Token: 0x040007E8 RID: 2024
		private bool _areOffsetsDirty;

		// Token: 0x040007E9 RID: 2025
		private bool _updatePositions;

		// Token: 0x040007EC RID: 2028
		private TextWidget _smithingLevelTextWidget;

		// Token: 0x040007ED RID: 2029
		private TextWidget _orderDifficultyTextWidget;
	}
}
