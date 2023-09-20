using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.Launcher.Library.CustomWidgets
{
	// Token: 0x02000022 RID: 34
	public class LauncherHintTriggerWidget : Widget
	{
		// Token: 0x06000152 RID: 338 RVA: 0x00005E59 File Offset: 0x00004059
		public LauncherHintTriggerWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00005E62 File Offset: 0x00004062
		protected override void OnConnectedToRoot()
		{
			base.ParentWidget.EventFire += this.ParentWidgetEventFired;
			base.OnConnectedToRoot();
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00005E81 File Offset: 0x00004081
		protected override void OnDisconnectedFromRoot()
		{
			base.ParentWidget.EventFire -= this.ParentWidgetEventFired;
			base.OnDisconnectedFromRoot();
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00005EA0 File Offset: 0x000040A0
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
				}
			}
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00005EF0 File Offset: 0x000040F0
		protected override bool OnPreviewMousePressed()
		{
			return false;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00005EF3 File Offset: 0x000040F3
		protected override bool OnPreviewDragBegin()
		{
			return false;
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00005EF6 File Offset: 0x000040F6
		protected override bool OnPreviewDrop()
		{
			return false;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00005EF9 File Offset: 0x000040F9
		protected override bool OnPreviewMouseScroll()
		{
			return false;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00005EFC File Offset: 0x000040FC
		protected override bool OnPreviewMouseReleased()
		{
			return false;
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00005EFF File Offset: 0x000040FF
		protected override bool OnPreviewMouseMove()
		{
			return true;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00005F02 File Offset: 0x00004102
		protected override bool OnPreviewDragHover()
		{
			return false;
		}
	}
}
