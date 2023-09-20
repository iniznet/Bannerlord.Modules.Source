using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	public class GauntletGamepadCursor : GlobalLayer
	{
		public GauntletGamepadCursor()
		{
			this._dataSource = new GamepadCursorViewModel();
			this._layer = new GauntletLayer(100001, "GauntletLayer", false);
			this._layer.LoadMovie("GamepadCursor", this._dataSource);
			this._layer.InputRestrictions.SetInputRestrictions(false, 0);
			base.Layer = this._layer;
		}

		public static void Initialize()
		{
			if (GauntletGamepadCursor._current == null)
			{
				GauntletGamepadCursor._current = new GauntletGamepadCursor();
				ScreenManager.AddGlobalLayer(GauntletGamepadCursor._current, false);
			}
		}

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

		private static Vec2 GetCursorPosition()
		{
			Vec2 mousePositionPixel = Input.MousePositionPixel;
			Vec2 vec = Vec2.One - ScreenManager.UsableArea;
			float num = vec.x * Screen.RealScreenResolution.x / 2f;
			float num2 = vec.y * Screen.RealScreenResolution.y / 2f;
			return new Vec2(mousePositionPixel.X - num, mousePositionPixel.Y - num2);
		}

		private GamepadCursorViewModel _dataSource;

		private GauntletLayer _layer;

		private static GauntletGamepadCursor _current;
	}
}
