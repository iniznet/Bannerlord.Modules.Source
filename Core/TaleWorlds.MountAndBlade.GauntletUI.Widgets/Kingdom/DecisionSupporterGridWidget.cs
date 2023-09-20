using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	public class DecisionSupporterGridWidget : GridWidget
	{
		public int VisibleCount { get; set; } = 4;

		public TextWidget MoreTextWidget { get; set; }

		public DecisionSupporterGridWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.IsVisible = child.GetSiblingIndex() < this.VisibleCount;
			this.UpdateMoreText();
		}

		private void UpdateMoreText()
		{
			if (this.MoreTextWidget != null)
			{
				this.MoreTextWidget.IsVisible = base.ChildCount > this.VisibleCount;
				if (this.MoreTextWidget.IsVisible)
				{
					this.MoreTextWidget.Text = "+" + (base.ChildCount - this.VisibleCount);
				}
			}
		}
	}
}
