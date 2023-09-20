using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	// Token: 0x020000A7 RID: 167
	public class MultiplayerLobbyArmoryCosmeticItemButtonWidget : ButtonWidget
	{
		// Token: 0x060008AD RID: 2221 RVA: 0x00018F57 File Offset: 0x00017157
		public MultiplayerLobbyArmoryCosmeticItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060008AE RID: 2222 RVA: 0x00018F60 File Offset: 0x00017160
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
