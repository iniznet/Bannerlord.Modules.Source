using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200001F RID: 31
	public class HintWidget : Widget
	{
		// Token: 0x06000189 RID: 393 RVA: 0x000065F0 File Offset: 0x000047F0
		public HintWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600018A RID: 394 RVA: 0x000065F9 File Offset: 0x000047F9
		protected override void OnConnectedToRoot()
		{
			base.ParentWidget.EventFire += this.ParentWidgetEventFired;
			base.OnConnectedToRoot();
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00006618 File Offset: 0x00004818
		protected override void OnDisconnectedFromRoot()
		{
			base.ParentWidget.EventFire -= this.ParentWidgetEventFired;
			base.OnDisconnectedFromRoot();
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00006638 File Offset: 0x00004838
		private void ParentWidgetEventFired(Widget widget, string eventName, object[] args)
		{
			if (base.IsVisible)
			{
				if (eventName == "HoverBegin")
				{
					base.EventFired("HoverBegin", Array.Empty<object>());
					return;
				}
				if (eventName == "HoverEnd")
				{
					base.EventFired("HoverEnd", Array.Empty<object>());
					return;
				}
				if (eventName == "DragHoverBegin")
				{
					base.EventFired("DragHoverBegin", Array.Empty<object>());
					return;
				}
				if (eventName == "DragHoverEnd")
				{
					base.EventFired("DragHoverEnd", Array.Empty<object>());
				}
			}
		}

		// Token: 0x0600018D RID: 397 RVA: 0x000066C4 File Offset: 0x000048C4
		protected override bool OnPreviewMousePressed()
		{
			return false;
		}

		// Token: 0x0600018E RID: 398 RVA: 0x000066C7 File Offset: 0x000048C7
		protected override bool OnPreviewDragBegin()
		{
			return false;
		}

		// Token: 0x0600018F RID: 399 RVA: 0x000066CA File Offset: 0x000048CA
		protected override bool OnPreviewDrop()
		{
			return false;
		}

		// Token: 0x06000190 RID: 400 RVA: 0x000066CD File Offset: 0x000048CD
		protected override bool OnPreviewMouseScroll()
		{
			return false;
		}

		// Token: 0x06000191 RID: 401 RVA: 0x000066D0 File Offset: 0x000048D0
		protected override bool OnPreviewMouseReleased()
		{
			return false;
		}

		// Token: 0x06000192 RID: 402 RVA: 0x000066D3 File Offset: 0x000048D3
		protected override bool OnPreviewMouseMove()
		{
			return true;
		}

		// Token: 0x06000193 RID: 403 RVA: 0x000066D6 File Offset: 0x000048D6
		protected override bool OnPreviewDragHover()
		{
			return false;
		}
	}
}
