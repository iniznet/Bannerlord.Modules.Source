using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200002E RID: 46
	public class ParallaxContainerWidget : Widget
	{
		// Token: 0x06000299 RID: 665 RVA: 0x000089E2 File Offset: 0x00006BE2
		public ParallaxContainerWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600029A RID: 666 RVA: 0x000089F8 File Offset: 0x00006BF8
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			using (List<ParallaxItemBrushWidget>.Enumerator enumerator = this._parallaxItems.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current.InitialDirection)
					{
					}
				}
			}
		}

		// Token: 0x0600029B RID: 667 RVA: 0x00008A6C File Offset: 0x00006C6C
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			ParallaxItemBrushWidget parallaxItemBrushWidget;
			if ((parallaxItemBrushWidget = child as ParallaxItemBrushWidget) != null)
			{
				this._parallaxItems.Add(parallaxItemBrushWidget);
			}
		}

		// Token: 0x0600029C RID: 668 RVA: 0x00008A98 File Offset: 0x00006C98
		protected override void OnChildRemoved(Widget child)
		{
			base.OnChildRemoved(child);
			ParallaxItemBrushWidget parallaxItemBrushWidget;
			if ((parallaxItemBrushWidget = child as ParallaxItemBrushWidget) != null)
			{
				this._parallaxItems.Remove(parallaxItemBrushWidget);
			}
		}

		// Token: 0x0400010F RID: 271
		private List<ParallaxItemBrushWidget> _parallaxItems = new List<ParallaxItemBrushWidget>();
	}
}
