﻿using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.Launcher.Library.CustomWidgets
{
	public class LauncherHintTriggerWidget : Widget
	{
		public LauncherHintTriggerWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnConnectedToRoot()
		{
			base.ParentWidget.EventFire += this.ParentWidgetEventFired;
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
					base.EventFired("HoverBegin", Array.Empty<object>());
					return;
				}
				if (eventName == "HoverEnd")
				{
					base.EventFired("HoverEnd", Array.Empty<object>());
				}
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
