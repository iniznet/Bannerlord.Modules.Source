using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaleWorlds.Library;

namespace TaleWorlds.InputSystem
{
	public class InputContext : IInputContext
	{
		public bool IsKeysAllowed { get; set; }

		public bool IsMouseButtonAllowed { get; set; }

		public bool IsMouseWheelAllowed { get; set; }

		public bool IsControllerAllowed
		{
			get
			{
				return this.IsKeysAllowed;
			}
		}

		public bool MouseOnMe { get; set; }

		public InputContext()
		{
			this._categories = new List<GameKeyContext>();
			this.MouseOnMe = false;
		}

		public int GetPointerX()
		{
			float x = Input.Resolution.x;
			return (int)(this.GetMousePositionRanged().x * x);
		}

		public int GetPointerY()
		{
			float y = Input.Resolution.y;
			return (int)(this.GetMousePositionRanged().y * y);
		}

		public Vector2 GetPointerPosition()
		{
			Vec2 resolution = Input.Resolution;
			float x = resolution.x;
			float y = resolution.y;
			float num = this.GetMousePositionRanged().x * x;
			float num2 = this.GetMousePositionRanged().y * y;
			return new Vector2(num, num2);
		}

		public Vec2 GetPointerPositionVec2()
		{
			Vec2 resolution = Input.Resolution;
			float x = resolution.x;
			float y = resolution.y;
			float num = this.GetMousePositionRanged().x * x;
			float num2 = this.GetMousePositionRanged().y * y;
			return new Vec2(num, num2);
		}

		public void RegisterHotKeyCategory(GameKeyContext category)
		{
			this._categories.Add(category);
			foreach (HotKey hotKey in category.RegisteredHotKeys)
			{
				if (!this._registeredHotKeys.ContainsKey(hotKey.Id))
				{
					this._registeredHotKeys.Add(hotKey.Id, hotKey);
				}
			}
			if (this._registeredGameKeys.Count == 0)
			{
				int count = category.RegisteredGameKeys.Count;
				for (int i = 0; i < count; i++)
				{
					this._registeredGameKeys.Add(null);
				}
			}
			foreach (GameKey gameKey in category.RegisteredGameKeys)
			{
				if (gameKey != null)
				{
					this._registeredGameKeys[gameKey.Id] = gameKey;
				}
			}
			foreach (GameAxisKey gameAxisKey in category.RegisteredGameAxisKeys)
			{
				if (!this._registeredGameAxisKeys.ContainsKey(gameAxisKey.Id))
				{
					this._registeredGameAxisKeys.Add(gameAxisKey.Id, gameAxisKey);
				}
			}
		}

		public bool IsCategoryRegistered(GameKeyContext category)
		{
			List<GameKeyContext> categories = this._categories;
			return categories != null && categories.Contains(category);
		}

		public void UpdateLastDownKeys()
		{
			for (int i = this._gameKeysToCurrentlyIgnore.Count - 1; i >= 0; i--)
			{
				if (this.IsGameKeyReleased(this._gameKeysToCurrentlyIgnore[i]))
				{
					this._gameKeysToCurrentlyIgnore.RemoveAt(i);
				}
			}
			for (int j = this._hotKeysToCurrentlyIgnore.Count - 1; j >= 0; j--)
			{
				if (this.IsHotKeyReleased(this._hotKeysToCurrentlyIgnore[j]))
				{
					this._hotKeysToCurrentlyIgnore.RemoveAt(j);
				}
			}
			for (int k = 0; k < this._registeredGameKeys.Count; k++)
			{
				GameKey gameKey = this._registeredGameKeys[k];
				if (gameKey != null)
				{
					bool flag = this.IsGameKeyDown(gameKey);
					if (this._isDownMapsReset && flag)
					{
						this._gameKeysToCurrentlyIgnore.Add(gameKey);
					}
					else if (!this._lastFrameDownGameKeyIDs.Contains(gameKey.Id))
					{
						this._lastFrameDownGameKeyIDs.Add(gameKey.Id);
					}
				}
			}
			foreach (HotKey hotKey in this._registeredHotKeys.Values)
			{
				bool flag2 = this.IsHotKeyDown(hotKey);
				if (this._isDownMapsReset && flag2)
				{
					this._hotKeysToCurrentlyIgnore.Add(hotKey);
				}
				else if (flag2)
				{
					this._lastFrameHotKeyDownMap[hotKey] = true;
				}
			}
			this._isDownMapsReset = false;
		}

		public void ResetLastDownKeys()
		{
			if (!this._isDownMapsReset)
			{
				this._lastFrameDownGameKeyIDs.Clear();
				this._lastFrameHotKeyDownMap.Clear();
				this._hotKeysToCurrentlyIgnore.Clear();
				this._gameKeysToCurrentlyIgnore.Clear();
				this._isDownMapsReset = true;
			}
		}

		private bool IsHotKeyDown(HotKey hotKey)
		{
			return hotKey.IsDown(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		public bool IsHotKeyDown(string hotKey)
		{
			HotKey hotKey2;
			return this._registeredHotKeys.TryGetValue(hotKey, out hotKey2) && this.IsHotKeyDown(hotKey2);
		}

		private bool IsGameKeyDown(GameKey gameKey)
		{
			return gameKey.IsDown(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed, true);
		}

		public bool IsGameKeyDown(int gameKey)
		{
			GameKey gameKey2 = this._registeredGameKeys[gameKey];
			return this.IsGameKeyDown(gameKey2);
		}

		private bool IsGameKeyDownImmediate(GameKey gameKey)
		{
			return gameKey.IsDownImmediate(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		public bool IsGameKeyDownImmediate(int gameKey)
		{
			GameKey gameKey2 = this._registeredGameKeys[gameKey];
			return this.IsGameKeyDownImmediate(gameKey2);
		}

		private bool IsHotKeyPressed(HotKey hotKey)
		{
			return hotKey.IsPressed(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		public bool IsHotKeyDownAndReleased(string hotkey)
		{
			HotKey hotKey;
			return this._registeredHotKeys.TryGetValue(hotkey, out hotKey) && this._lastFrameHotKeyDownMap.ContainsKey(hotKey) && !this._hotKeysToCurrentlyIgnore.Contains(hotKey) && hotKey.IsReleased(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		public bool IsHotKeyPressed(string hotKey)
		{
			HotKey hotKey2;
			return this._registeredHotKeys.TryGetValue(hotKey, out hotKey2) && this.IsHotKeyPressed(hotKey2);
		}

		private bool IsGameKeyPressed(GameKey gameKey)
		{
			return gameKey.IsPressed(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		public bool IsGameKeyDownAndReleased(int gameKey)
		{
			GameKey gameKey2 = this._registeredGameKeys[gameKey];
			return this._lastFrameDownGameKeyIDs.Contains(gameKey2.Id) && !this._gameKeysToCurrentlyIgnore.Contains(gameKey2) && gameKey2.IsReleased(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		public bool IsGameKeyPressed(int gameKey)
		{
			GameKey gameKey2 = this._registeredGameKeys[gameKey];
			return this.IsGameKeyPressed(gameKey2);
		}

		private bool IsHotKeyReleased(HotKey hotKey)
		{
			return hotKey.IsReleased(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		public bool IsHotKeyReleased(string hotKey)
		{
			HotKey hotKey2;
			return this._registeredHotKeys.TryGetValue(hotKey, out hotKey2) && this.IsHotKeyReleased(hotKey2);
		}

		private bool IsGameKeyReleased(GameKey gameKey)
		{
			return gameKey.IsReleased(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		public bool IsGameKeyReleased(int gameKey)
		{
			GameKey gameKey2 = this._registeredGameKeys[gameKey];
			return this.IsGameKeyReleased(gameKey2);
		}

		private float GetGameKeyState(GameKey gameKey)
		{
			return gameKey.GetKeyState(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		public float GetGameKeyState(int gameKey)
		{
			GameKey gameKey2 = this._registeredGameKeys[gameKey];
			return this.GetGameKeyState(gameKey2);
		}

		private bool IsHotKeyDoublePressed(HotKey hotKey)
		{
			return hotKey.IsDoublePressed(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		public bool IsHotKeyDoublePressed(string hotKey)
		{
			HotKey hotKey2;
			return this._registeredHotKeys.TryGetValue(hotKey, out hotKey2) && this.IsHotKeyDoublePressed(hotKey2);
		}

		public float GetGameKeyAxis(GameAxisKey gameKey)
		{
			return gameKey.GetAxisState(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		public float GetGameKeyAxis(string gameKey)
		{
			GameAxisKey gameAxisKey;
			if (this._registeredGameAxisKeys.TryGetValue(gameKey, out gameAxisKey))
			{
				return this.GetGameKeyAxis(gameAxisKey);
			}
			return 0f;
		}

		internal bool CanUse(InputKey key)
		{
			if (Input.GetClickKeys().Any((InputKey k) => k == key))
			{
				return this.IsMouseButtonAllowed || this.IsControllerAllowed;
			}
			switch (key)
			{
			case InputKey.Escape:
			case InputKey.D1:
			case InputKey.D2:
			case InputKey.D3:
			case InputKey.D4:
			case InputKey.D5:
			case InputKey.D6:
			case InputKey.D7:
			case InputKey.D8:
			case InputKey.D9:
			case InputKey.D0:
			case InputKey.Minus:
			case InputKey.Equals:
			case InputKey.BackSpace:
			case InputKey.Tab:
			case InputKey.Q:
			case InputKey.W:
			case InputKey.E:
			case InputKey.R:
			case InputKey.T:
			case InputKey.Y:
			case InputKey.U:
			case InputKey.I:
			case InputKey.O:
			case InputKey.P:
			case InputKey.OpenBraces:
			case InputKey.CloseBraces:
			case InputKey.Enter:
			case InputKey.LeftControl:
			case InputKey.A:
			case InputKey.S:
			case InputKey.D:
			case InputKey.F:
			case InputKey.G:
			case InputKey.H:
			case InputKey.J:
			case InputKey.K:
			case InputKey.L:
			case InputKey.SemiColon:
			case InputKey.Apostrophe:
			case InputKey.Tilde:
			case InputKey.LeftShift:
			case InputKey.BackSlash:
			case InputKey.Z:
			case InputKey.X:
			case InputKey.C:
			case InputKey.V:
			case InputKey.B:
			case InputKey.N:
			case InputKey.M:
			case InputKey.Comma:
			case InputKey.Period:
			case InputKey.Slash:
			case InputKey.RightShift:
			case InputKey.NumpadMultiply:
			case InputKey.LeftAlt:
			case InputKey.Space:
			case InputKey.CapsLock:
			case InputKey.F1:
			case InputKey.F2:
			case InputKey.F3:
			case InputKey.F4:
			case InputKey.F5:
			case InputKey.F6:
			case InputKey.F7:
			case InputKey.F8:
			case InputKey.F9:
			case InputKey.F10:
			case InputKey.Numpad7:
			case InputKey.Numpad8:
			case InputKey.Numpad9:
			case InputKey.NumpadMinus:
			case InputKey.Numpad4:
			case InputKey.Numpad5:
			case InputKey.Numpad6:
			case InputKey.NumpadPlus:
			case InputKey.Numpad1:
			case InputKey.Numpad2:
			case InputKey.Numpad3:
			case InputKey.Numpad0:
			case InputKey.NumpadPeriod:
			case InputKey.F11:
			case InputKey.F12:
			case InputKey.NumpadEnter:
			case InputKey.RightControl:
			case InputKey.NumpadSlash:
			case InputKey.RightAlt:
			case InputKey.NumLock:
			case InputKey.Home:
			case InputKey.Up:
			case InputKey.PageUp:
			case InputKey.Left:
			case InputKey.Right:
			case InputKey.End:
			case InputKey.Down:
			case InputKey.PageDown:
			case InputKey.Insert:
			case InputKey.Delete:
				return this.IsKeysAllowed;
			case InputKey.ControllerLStick:
			case InputKey.ControllerRStick:
			case InputKey.ControllerLStickUp:
			case InputKey.ControllerLStickDown:
			case InputKey.ControllerLStickLeft:
			case InputKey.ControllerLStickRight:
			case InputKey.ControllerRStickUp:
			case InputKey.ControllerRStickDown:
			case InputKey.ControllerRStickLeft:
			case InputKey.ControllerRStickRight:
			case InputKey.ControllerLUp:
			case InputKey.ControllerLDown:
			case InputKey.ControllerLLeft:
			case InputKey.ControllerLRight:
			case InputKey.ControllerRUp:
			case InputKey.ControllerRDown:
			case InputKey.ControllerRLeft:
			case InputKey.ControllerRRight:
			case InputKey.ControllerLBumper:
			case InputKey.ControllerRBumper:
			case InputKey.ControllerLOption:
			case InputKey.ControllerROption:
			case InputKey.ControllerLTrigger:
			case InputKey.ControllerRTrigger:
				return this.IsControllerAllowed;
			case InputKey.LeftMouseButton:
			case InputKey.RightMouseButton:
			case InputKey.MiddleMouseButton:
			case InputKey.X1MouseButton:
			case InputKey.X2MouseButton:
				return this.IsMouseButtonAllowed;
			case InputKey.MouseScrollUp:
			case InputKey.MouseScrollDown:
				return this.IsMouseWheelAllowed;
			}
			return false;
		}

		public Vec2 GetKeyState(InputKey key)
		{
			if (!this.CanUse(key))
			{
				return new Vec2(0f, 0f);
			}
			return Input.GetKeyState(key);
		}

		protected bool IsMouseButton(InputKey key)
		{
			return key == InputKey.LeftMouseButton || key == InputKey.RightMouseButton || key == InputKey.MiddleMouseButton;
		}

		public bool IsKeyDown(InputKey key)
		{
			if (this.IsMouseButton(key))
			{
				if (!this.MouseOnMe)
				{
					return false;
				}
			}
			else if (!this.CanUse(key))
			{
				return false;
			}
			return Input.IsKeyDown(key);
		}

		public bool IsKeyPressed(InputKey key)
		{
			return this.CanUse(key) && Input.IsKeyPressed(key);
		}

		public bool IsKeyReleased(InputKey key)
		{
			if (this.IsMouseButton(key))
			{
				if (!this.MouseOnMe)
				{
					return false;
				}
			}
			else if (!this.CanUse(key))
			{
				return false;
			}
			return Input.IsKeyReleased(key);
		}

		public float GetMouseMoveX()
		{
			return Input.GetMouseMoveX();
		}

		public float GetMouseMoveY()
		{
			return Input.GetMouseMoveY();
		}

		public Vec2 GetControllerRightStickState()
		{
			return Input.GetKeyState(InputKey.ControllerRStick);
		}

		public Vec2 GetControllerLeftStickState()
		{
			return Input.GetKeyState(InputKey.ControllerLStick);
		}

		public bool GetIsMouseActive()
		{
			return Input.IsMouseActive;
		}

		public bool GetIsMouseDown()
		{
			return Input.IsKeyDown(InputKey.LeftMouseButton) || Input.IsKeyDown(InputKey.RightMouseButton);
		}

		public Vec2 GetMousePositionPixel()
		{
			return Input.MousePositionPixel;
		}

		public float GetDeltaMouseScroll()
		{
			if (!this.IsMouseWheelAllowed)
			{
				return 0f;
			}
			return Input.DeltaMouseScroll;
		}

		public bool GetIsControllerConnected()
		{
			return Input.IsControllerConnected;
		}

		public Vec2 GetMousePositionRanged()
		{
			return Input.MousePositionRanged;
		}

		public float GetMouseSensitivity()
		{
			return Input.MouseSensitivity;
		}

		public bool IsControlDown()
		{
			return this.IsKeysAllowed && (Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl));
		}

		public bool IsShiftDown()
		{
			return this.IsKeysAllowed && (Input.IsKeyDown(InputKey.LeftShift) || Input.IsKeyDown(InputKey.RightShift));
		}

		public bool IsAltDown()
		{
			return this.IsKeysAllowed && (Input.IsKeyDown(InputKey.LeftAlt) || Input.IsKeyDown(InputKey.RightAlt));
		}

		public InputKey[] GetClickKeys()
		{
			return Input.GetClickKeys();
		}

		private Dictionary<string, HotKey> _registeredHotKeys = new Dictionary<string, HotKey>();

		private List<GameKey> _registeredGameKeys = new List<GameKey>();

		private List<int> _lastFrameDownGameKeyIDs = new List<int>();

		private Dictionary<string, GameAxisKey> _registeredGameAxisKeys = new Dictionary<string, GameAxisKey>();

		private List<GameKey> _gameKeysToCurrentlyIgnore = new List<GameKey>();

		private List<HotKey> _hotKeysToCurrentlyIgnore = new List<HotKey>();

		private Dictionary<HotKey, bool> _lastFrameHotKeyDownMap = new Dictionary<HotKey, bool>();

		private bool _isDownMapsReset;

		private readonly List<GameKeyContext> _categories;
	}
}
