using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class ParallaxContainerWidget : Widget
	{
		public ParallaxContainerWidget(UIContext context)
			: base(context)
		{
		}

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

		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			ParallaxItemBrushWidget parallaxItemBrushWidget;
			if ((parallaxItemBrushWidget = child as ParallaxItemBrushWidget) != null)
			{
				this._parallaxItems.Add(parallaxItemBrushWidget);
			}
		}

		protected override void OnChildRemoved(Widget child)
		{
			base.OnChildRemoved(child);
			ParallaxItemBrushWidget parallaxItemBrushWidget;
			if ((parallaxItemBrushWidget = child as ParallaxItemBrushWidget) != null)
			{
				this._parallaxItems.Remove(parallaxItemBrushWidget);
			}
		}

		private List<ParallaxItemBrushWidget> _parallaxItems = new List<ParallaxItemBrushWidget>();
	}
}
