using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Scoreboard
{
	// Token: 0x0200004B RID: 75
	public class ScoreboardGainedSkillsListPanel : ListPanel
	{
		// Token: 0x060003F2 RID: 1010 RVA: 0x0000CDF7 File Offset: 0x0000AFF7
		public ScoreboardGainedSkillsListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x0000CE0B File Offset: 0x0000B00B
		protected override void OnLateUpdate(float dt)
		{
			if (base.IsVisible)
			{
				this.UpdateVerticalPosRelatedToCurrentUnit();
			}
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x0000CE1C File Offset: 0x0000B01C
		private void UpdateVerticalPosRelatedToCurrentUnit()
		{
			if (this._currentUnit == null)
			{
				base.ScaledPositionYOffset = -1000f;
				return;
			}
			float num = base.EventManager.PageSize.Y - 107f * base._scaleToUse;
			if (num - (this._currentUnit.GlobalPosition.Y + base.Size.Y) < base.Size.Y + this._scrollGradientPadding)
			{
				float num2 = num - (this._scrollGradientPadding * base._scaleToUse + base.Size.Y);
				base.ScaledPositionYOffset = num2 - this._currentUnit.GlobalPosition.Y + this._currentUnit.ParentWidget.ParentWidget.ParentWidget.LocalPosition.Y;
				return;
			}
			base.ScaledPositionYOffset = this._currentUnit.ParentWidget.ParentWidget.ParentWidget.LocalPosition.Y;
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x0000CF0C File Offset: 0x0000B10C
		public void SetCurrentUnit(ScoreboardSkillItemHoverToggleWidget unit)
		{
			this._currentUnit = unit;
			if (unit != null)
			{
				if (base.ChildCount > 0)
				{
					base.RemoveAllChildren();
				}
				List<Widget> allSkillWidgets = this._currentUnit.GetAllSkillWidgets();
				for (int i = 0; i < allSkillWidgets.Count; i++)
				{
					Widget widget = allSkillWidgets[i];
					Widget widget2 = new Widget(base.Context);
					this.CopyWidgetProperties(ref widget2, widget);
					Widget widget3 = new Widget(base.Context);
					this.CopyWidgetProperties(ref widget3, widget.Children[0]);
					TextWidget textWidget = new TextWidget(base.Context);
					this.CopyWidgetProperties(ref textWidget, (TextWidget)widget.Children[1]);
					base.AddChild(widget2);
					widget2.AddChild(widget3);
					widget2.AddChild(textWidget);
				}
				this.UpdateVerticalPosRelatedToCurrentUnit();
				base.IsVisible = true;
				return;
			}
			base.RemoveAllChildren();
			base.IsVisible = false;
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x0000CFEC File Offset: 0x0000B1EC
		private void CopyWidgetProperties(ref TextWidget targetTextWidget, TextWidget sourceTextWidget)
		{
			targetTextWidget.Text = sourceTextWidget.Text;
			Widget widget = targetTextWidget;
			this.CopyWidgetProperties(ref widget, sourceTextWidget);
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x0000D014 File Offset: 0x0000B214
		private void CopyWidgetProperties(ref Widget targetWidget, Widget sourceWidget)
		{
			targetWidget.WidthSizePolicy = sourceWidget.WidthSizePolicy;
			targetWidget.HeightSizePolicy = sourceWidget.HeightSizePolicy;
			targetWidget.SuggestedWidth = sourceWidget.SuggestedWidth;
			targetWidget.SuggestedHeight = sourceWidget.SuggestedHeight;
			targetWidget.MarginTop = sourceWidget.MarginTop;
			targetWidget.MarginBottom = sourceWidget.MarginBottom;
			targetWidget.MarginLeft = sourceWidget.MarginLeft;
			targetWidget.MarginRight = sourceWidget.MarginRight;
			targetWidget.Sprite = sourceWidget.Sprite;
			BrushWidget brushWidget;
			BrushWidget brushWidget2;
			if ((brushWidget = targetWidget as BrushWidget) != null && (brushWidget2 = sourceWidget as BrushWidget) != null)
			{
				brushWidget.Brush = brushWidget2.ReadOnlyBrush;
			}
			targetWidget.VerticalAlignment = sourceWidget.VerticalAlignment;
		}

		// Token: 0x040001B7 RID: 439
		private float _scrollGradientPadding = 55f;

		// Token: 0x040001B8 RID: 440
		private ScoreboardSkillItemHoverToggleWidget _currentUnit;
	}
}
