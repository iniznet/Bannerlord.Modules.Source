using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Data;

namespace TaleWorlds.GauntletUI.TooltipExtensions
{
	// Token: 0x02000003 RID: 3
	public class TooltipExtensionWidget : Widget
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000021D0 File Offset: 0x000003D0
		public Widget TooltipWidget
		{
			get
			{
				if (base.Children.Count > 0)
				{
					return base.Children[0];
				}
				return null;
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000021EE File Offset: 0x000003EE
		public TooltipExtensionWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000021F7 File Offset: 0x000003F7
		protected override void OnConnectedToRoot()
		{
			base.ParentWidget.EventFire += this.ParentWidgetEventFired;
			this.TooltipWidget.IsVisible = false;
			base.OnConnectedToRoot();
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002222 File Offset: 0x00000422
		protected override void OnDisconnectedFromRoot()
		{
			base.ParentWidget.EventFire -= this.ParentWidgetEventFired;
			base.OnDisconnectedFromRoot();
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002244 File Offset: 0x00000444
		private void ParentWidgetEventFired(Widget widget, string eventName, object[] args)
		{
			if (base.IsVisible)
			{
				if (eventName == "HoverBegin")
				{
					this.UpdateTooltip(true);
					base.EventFired("HoverBegin", Array.Empty<object>());
					return;
				}
				if (eventName == "HoverEnd")
				{
					this.UpdateTooltip(false);
					base.EventFired("HoverEnd", Array.Empty<object>());
				}
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000022A4 File Offset: 0x000004A4
		private void UpdateTooltip(bool status)
		{
			if (status)
			{
				GauntletView component = base.GetComponent<GauntletView>();
				if (component != null)
				{
					component.RefreshBindingWithChildren();
				}
			}
			if (this.TooltipWidget != null)
			{
				this.TooltipWidget.IsVisible = status;
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000022D8 File Offset: 0x000004D8
		protected override bool OnPreviewMousePressed()
		{
			return false;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000022DB File Offset: 0x000004DB
		protected override bool OnPreviewDragBegin()
		{
			return false;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000022DE File Offset: 0x000004DE
		protected override bool OnPreviewDrop()
		{
			return false;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000022E1 File Offset: 0x000004E1
		protected override bool OnPreviewMouseScroll()
		{
			return false;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000022E4 File Offset: 0x000004E4
		protected override bool OnPreviewMouseReleased()
		{
			return false;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000022E7 File Offset: 0x000004E7
		protected override bool OnPreviewMouseMove()
		{
			return true;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000022EA File Offset: 0x000004EA
		protected override bool OnPreviewDragHover()
		{
			return false;
		}
	}
}
