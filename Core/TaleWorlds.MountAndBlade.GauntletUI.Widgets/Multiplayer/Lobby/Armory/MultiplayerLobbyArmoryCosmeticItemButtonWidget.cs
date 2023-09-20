using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	public class MultiplayerLobbyArmoryCosmeticItemButtonWidget : ButtonWidget
	{
		public MultiplayerLobbyArmoryCosmeticItemButtonWidget(UIContext context)
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
