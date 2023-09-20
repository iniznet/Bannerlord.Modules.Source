﻿using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement
{
	public class DevelopmentRingVisualButtonWidget : ButtonWidget
	{
		public DevelopmentRingVisualButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!base.IsSelected)
			{
				this.SetState(base.ParentWidget.CurrentState);
				return;
			}
			this.SetState("Selected");
		}
	}
}
