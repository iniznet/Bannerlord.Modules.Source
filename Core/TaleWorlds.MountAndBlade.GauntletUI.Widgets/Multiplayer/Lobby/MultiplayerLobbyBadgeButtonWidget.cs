﻿using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	public class MultiplayerLobbyBadgeButtonWidget : ButtonWidget
	{
		public MultiplayerLobbyBadgeButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (base.EventManager.HoveredView == this && Input.IsKeyPressed(InputKey.ControllerRUp))
			{
				this.OnMouseAlternatePressed();
				return;
			}
			if (base.EventManager.HoveredView == this && Input.IsKeyReleased(InputKey.ControllerRUp))
			{
				this.OnMouseAlternateReleased();
			}
		}
	}
}
