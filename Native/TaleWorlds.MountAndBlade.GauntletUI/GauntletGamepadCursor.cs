using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	// Token: 0x02000008 RID: 8
	public class GauntletGamepadCursor : GlobalLayer
	{
		// Token: 0x0600002B RID: 43 RVA: 0x00002E48 File Offset: 0x00001048
		public GauntletGamepadCursor()
		{
			this._dataSource = new GamepadCursorViewModel();
			this._layer = new GauntletLayer(100001, "GauntletLayer", false);
			this._layer.LoadMovie("GamepadCursor", this._dataSource);
			this._layer.InputRestrictions.SetInputRestrictions(false, 0);
			base.Layer = this._layer;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002EB1 File Offset: 0x000010B1
		public static void Initialize()
		{
			if (GauntletGamepadCursor._current == null)
			{
				GauntletGamepadCursor._current = new GauntletGamepadCursor();
				ScreenManager.AddGlobalLayer(GauntletGamepadCursor._current, false);
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002ED0 File Offset: 0x000010D0
		protected override void OnLateTick(float dt)
		{
			base.OnLateTick(dt);
			if (ScreenManager.IsMouseCursorHidden())
			{
				this._dataSource.IsGamepadCursorVisible = true;
				this._dataSource.IsConsoleMouseVisible = false;
				Vec2 cursorPosition = GauntletGamepadCursor.GetCursorPosition();
				this._dataSource.CursorPositionX = cursorPosition.X;
				this._dataSource.CursorPositionY = cursorPosition.Y;
				return;
			}
			this._dataSource.IsGamepadCursorVisible = false;
			this._dataSource.IsConsoleMouseVisible = false;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002F48 File Offset: 0x00001148
		private static Vec2 GetCursorPosition()
		{
			Vec2 mousePositionPixel = Input.MousePositionPixel;
			Vec2 vec = Vec2.One - ScreenManager.UsableArea;
			float num = vec.x * Screen.RealScreenResolution.x / 2f;
			float num2 = vec.y * Screen.RealScreenResolution.y / 2f;
			return new Vec2(mousePositionPixel.X - num, mousePositionPixel.Y - num2);
		}

		// Token: 0x04000019 RID: 25
		private GamepadCursorViewModel _dataSource;

		// Token: 0x0400001A RID: 26
		private GauntletLayer _layer;

		// Token: 0x0400001B RID: 27
		private static GauntletGamepadCursor _current;
	}
}
