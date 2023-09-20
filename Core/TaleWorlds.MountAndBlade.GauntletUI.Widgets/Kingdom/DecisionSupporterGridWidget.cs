using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	// Token: 0x02000110 RID: 272
	public class DecisionSupporterGridWidget : GridWidget
	{
		// Token: 0x170004EC RID: 1260
		// (get) Token: 0x06000DD9 RID: 3545 RVA: 0x00026D51 File Offset: 0x00024F51
		// (set) Token: 0x06000DDA RID: 3546 RVA: 0x00026D59 File Offset: 0x00024F59
		public int VisibleCount { get; set; } = 4;

		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x06000DDB RID: 3547 RVA: 0x00026D62 File Offset: 0x00024F62
		// (set) Token: 0x06000DDC RID: 3548 RVA: 0x00026D6A File Offset: 0x00024F6A
		public TextWidget MoreTextWidget { get; set; }

		// Token: 0x06000DDD RID: 3549 RVA: 0x00026D73 File Offset: 0x00024F73
		public DecisionSupporterGridWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000DDE RID: 3550 RVA: 0x00026D83 File Offset: 0x00024F83
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.IsVisible = child.GetSiblingIndex() < this.VisibleCount;
			this.UpdateMoreText();
		}

		// Token: 0x06000DDF RID: 3551 RVA: 0x00026DA8 File Offset: 0x00024FA8
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
