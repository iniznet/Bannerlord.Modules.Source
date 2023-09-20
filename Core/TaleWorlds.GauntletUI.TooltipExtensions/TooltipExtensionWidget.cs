using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Data;

namespace TaleWorlds.GauntletUI.TooltipExtensions
{
	public class TooltipExtensionWidget : Widget
	{
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

		public TooltipExtensionWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnConnectedToRoot()
		{
			base.ParentWidget.EventFire += this.ParentWidgetEventFired;
			this.TooltipWidget.IsVisible = false;
			base.OnConnectedToRoot();
		}

		protected override void OnDisconnectedFromRoot()
		{
			base.ParentWidget.EventFire -= this.ParentWidgetEventFired;
			base.OnDisconnectedFromRoot();
		}

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

		protected override bool OnPreviewMousePressed()
		{
			return false;
		}

		protected override bool OnPreviewDragBegin()
		{
			return false;
		}

		protected override bool OnPreviewDrop()
		{
			return false;
		}

		protected override bool OnPreviewMouseScroll()
		{
			return false;
		}

		protected override bool OnPreviewMouseReleased()
		{
			return false;
		}

		protected override bool OnPreviewMouseMove()
		{
			return true;
		}

		protected override bool OnPreviewDragHover()
		{
			return false;
		}
	}
}
