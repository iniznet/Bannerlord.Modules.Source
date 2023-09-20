using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x02000091 RID: 145
	public class MultiplayerLobbyBadgeButtonWidget : ButtonWidget
	{
		// Token: 0x060007A6 RID: 1958 RVA: 0x00016857 File Offset: 0x00014A57
		public MultiplayerLobbyBadgeButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060007A7 RID: 1959 RVA: 0x00016860 File Offset: 0x00014A60
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
