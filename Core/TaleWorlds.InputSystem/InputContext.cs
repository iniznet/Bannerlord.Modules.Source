using System;
using System.Collections.Generic;
using System.Numerics;
using TaleWorlds.Library;

namespace TaleWorlds.InputSystem
{
	// Token: 0x0200000C RID: 12
	public class InputContext : IInputContext
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000FD RID: 253 RVA: 0x0000494E File Offset: 0x00002B4E
		// (set) Token: 0x060000FE RID: 254 RVA: 0x00004956 File Offset: 0x00002B56
		public bool IsKeysAllowed { get; set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000FF RID: 255 RVA: 0x0000495F File Offset: 0x00002B5F
		// (set) Token: 0x06000100 RID: 256 RVA: 0x00004967 File Offset: 0x00002B67
		public bool IsMouseButtonAllowed { get; set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000101 RID: 257 RVA: 0x00004970 File Offset: 0x00002B70
		// (set) Token: 0x06000102 RID: 258 RVA: 0x00004978 File Offset: 0x00002B78
		public bool IsMouseWheelAllowed { get; set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000103 RID: 259 RVA: 0x00004981 File Offset: 0x00002B81
		public bool IsControllerAllowed
		{
			get
			{
				return this.IsKeysAllowed;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000104 RID: 260 RVA: 0x00004989 File Offset: 0x00002B89
		// (set) Token: 0x06000105 RID: 261 RVA: 0x00004991 File Offset: 0x00002B91
		public bool MouseOnMe { get; set; }

		// Token: 0x06000106 RID: 262 RVA: 0x0000499C File Offset: 0x00002B9C
		public InputContext()
		{
			this._categories = new List<GameKeyContext>();
			this.MouseOnMe = false;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00004A10 File Offset: 0x00002C10
		public int GetPointerX()
		{
			float x = Input.Resolution.x;
			return (int)(this.GetMousePositionRanged().x * x);
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00004A38 File Offset: 0x00002C38
		public int GetPointerY()
		{
			float y = Input.Resolution.y;
			return (int)(this.GetMousePositionRanged().y * y);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00004A60 File Offset: 0x00002C60
		public Vector2 GetPointerPosition()
		{
			Vec2 resolution = Input.Resolution;
			float x = resolution.x;
			float y = resolution.y;
			float num = this.GetMousePositionRanged().x * x;
			float num2 = this.GetMousePositionRanged().y * y;
			return new Vector2(num, num2);
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00004AA0 File Offset: 0x00002CA0
		public Vec2 GetPointerPositionVec2()
		{
			Vec2 resolution = Input.Resolution;
			float x = resolution.x;
			float y = resolution.y;
			float num = this.GetMousePositionRanged().x * x;
			float num2 = this.GetMousePositionRanged().y * y;
			return new Vec2(num, num2);
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00004AE0 File Offset: 0x00002CE0
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

		// Token: 0x0600010C RID: 268 RVA: 0x00004C48 File Offset: 0x00002E48
		public bool IsCategoryRegistered(GameKeyContext category)
		{
			List<GameKeyContext> categories = this._categories;
			return categories != null && categories.Contains(category);
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00004C5C File Offset: 0x00002E5C
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

		// Token: 0x0600010E RID: 270 RVA: 0x00004DC8 File Offset: 0x00002FC8
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

		// Token: 0x0600010F RID: 271 RVA: 0x00004E05 File Offset: 0x00003005
		private bool IsHotKeyDown(HotKey hotKey)
		{
			return hotKey.IsDown(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00004E30 File Offset: 0x00003030
		public bool IsHotKeyDown(string hotKey)
		{
			HotKey hotKey2;
			return this._registeredHotKeys.TryGetValue(hotKey, out hotKey2) && this.IsHotKeyDown(hotKey2);
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00004E56 File Offset: 0x00003056
		private bool IsGameKeyDown(GameKey gameKey)
		{
			return gameKey.IsDown(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed, true);
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00004E84 File Offset: 0x00003084
		public bool IsGameKeyDown(int gameKey)
		{
			GameKey gameKey2 = this._registeredGameKeys[gameKey];
			return this.IsGameKeyDown(gameKey2);
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00004EA5 File Offset: 0x000030A5
		private bool IsGameKeyDownImmediate(GameKey gameKey)
		{
			return gameKey.IsDownImmediate(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00004ED0 File Offset: 0x000030D0
		public bool IsGameKeyDownImmediate(int gameKey)
		{
			GameKey gameKey2 = this._registeredGameKeys[gameKey];
			return this.IsGameKeyDownImmediate(gameKey2);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00004EF1 File Offset: 0x000030F1
		private bool IsHotKeyPressed(HotKey hotKey)
		{
			return hotKey.IsPressed(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00004F1C File Offset: 0x0000311C
		public bool IsHotKeyDownAndReleased(string hotkey)
		{
			HotKey hotKey;
			return this._registeredHotKeys.TryGetValue(hotkey, out hotKey) && this._lastFrameHotKeyDownMap.ContainsKey(hotKey) && !this._hotKeysToCurrentlyIgnore.Contains(hotKey) && hotKey.IsReleased(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00004F80 File Offset: 0x00003180
		public bool IsHotKeyPressed(string hotKey)
		{
			HotKey hotKey2;
			return this._registeredHotKeys.TryGetValue(hotKey, out hotKey2) && this.IsHotKeyPressed(hotKey2);
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00004FA6 File Offset: 0x000031A6
		private bool IsGameKeyPressed(GameKey gameKey)
		{
			return gameKey.IsPressed(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00004FD4 File Offset: 0x000031D4
		public bool IsGameKeyDownAndReleased(int gameKey)
		{
			GameKey gameKey2 = this._registeredGameKeys[gameKey];
			return this._lastFrameDownGameKeyIDs.Contains(gameKey2.Id) && !this._gameKeysToCurrentlyIgnore.Contains(gameKey2) && gameKey2.IsReleased(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000503C File Offset: 0x0000323C
		public bool IsGameKeyPressed(int gameKey)
		{
			GameKey gameKey2 = this._registeredGameKeys[gameKey];
			return this.IsGameKeyPressed(gameKey2);
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000505D File Offset: 0x0000325D
		private bool IsHotKeyReleased(HotKey hotKey)
		{
			return hotKey.IsReleased(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00005088 File Offset: 0x00003288
		public bool IsHotKeyReleased(string hotKey)
		{
			HotKey hotKey2;
			return this._registeredHotKeys.TryGetValue(hotKey, out hotKey2) && this.IsHotKeyReleased(hotKey2);
		}

		// Token: 0x0600011D RID: 285 RVA: 0x000050AE File Offset: 0x000032AE
		private bool IsGameKeyReleased(GameKey gameKey)
		{
			return gameKey.IsReleased(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x000050DC File Offset: 0x000032DC
		public bool IsGameKeyReleased(int gameKey)
		{
			GameKey gameKey2 = this._registeredGameKeys[gameKey];
			return this.IsGameKeyReleased(gameKey2);
		}

		// Token: 0x0600011F RID: 287 RVA: 0x000050FD File Offset: 0x000032FD
		private float GetGameKeyState(GameKey gameKey)
		{
			return gameKey.GetKeyState(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00005128 File Offset: 0x00003328
		public float GetGameKeyState(int gameKey)
		{
			GameKey gameKey2 = this._registeredGameKeys[gameKey];
			return this.GetGameKeyState(gameKey2);
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00005149 File Offset: 0x00003349
		private bool IsHotKeyDoublePressed(HotKey hotKey)
		{
			return hotKey.IsDoublePressed(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00005174 File Offset: 0x00003374
		public bool IsHotKeyDoublePressed(string hotKey)
		{
			HotKey hotKey2;
			return this._registeredHotKeys.TryGetValue(hotKey, out hotKey2) && this.IsHotKeyDoublePressed(hotKey2);
		}

		// Token: 0x06000123 RID: 291 RVA: 0x0000519A File Offset: 0x0000339A
		public float GetGameKeyAxis(GameAxisKey gameKey)
		{
			return gameKey.GetAxisState(this.IsKeysAllowed, this.IsMouseButtonAllowed && this.MouseOnMe, this.IsMouseWheelAllowed, this.IsControllerAllowed);
		}

		// Token: 0x06000124 RID: 292 RVA: 0x000051C8 File Offset: 0x000033C8
		public float GetGameKeyAxis(string gameKey)
		{
			GameAxisKey gameAxisKey;
			if (this._registeredGameAxisKeys.TryGetValue(gameKey, out gameAxisKey))
			{
				return this.GetGameKeyAxis(gameAxisKey);
			}
			return 0f;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x000051F4 File Offset: 0x000033F4
		internal bool CanUse(InputKey key)
		{
			if (key == Input.GetControllerClickKey())
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

		// Token: 0x06000126 RID: 294 RVA: 0x0000563D File Offset: 0x0000383D
		public Vec2 GetKeyState(InputKey key)
		{
			if (!this.CanUse(key))
			{
				return new Vec2(0f, 0f);
			}
			return Input.GetKeyState(key);
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000565E File Offset: 0x0000385E
		protected bool IsMouseButton(InputKey key)
		{
			return key == InputKey.LeftMouseButton || key == InputKey.RightMouseButton || key == InputKey.MiddleMouseButton;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0000567A File Offset: 0x0000387A
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

		// Token: 0x06000129 RID: 297 RVA: 0x000056A0 File Offset: 0x000038A0
		public bool IsKeyPressed(InputKey key)
		{
			return this.CanUse(key) && Input.IsKeyPressed(key);
		}

		// Token: 0x0600012A RID: 298 RVA: 0x000056B3 File Offset: 0x000038B3
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

		// Token: 0x0600012B RID: 299 RVA: 0x000056D9 File Offset: 0x000038D9
		public float GetMouseMoveX()
		{
			return Input.GetMouseMoveX();
		}

		// Token: 0x0600012C RID: 300 RVA: 0x000056E0 File Offset: 0x000038E0
		public float GetMouseMoveY()
		{
			return Input.GetMouseMoveY();
		}

		// Token: 0x0600012D RID: 301 RVA: 0x000056E7 File Offset: 0x000038E7
		public Vec2 GetControllerRightStickState()
		{
			return Input.GetKeyState(InputKey.ControllerRStick);
		}

		// Token: 0x0600012E RID: 302 RVA: 0x000056F3 File Offset: 0x000038F3
		public Vec2 GetControllerLeftStickState()
		{
			return Input.GetKeyState(InputKey.ControllerLStick);
		}

		// Token: 0x0600012F RID: 303 RVA: 0x000056FF File Offset: 0x000038FF
		public bool GetIsMouseActive()
		{
			return Input.IsMouseActive;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00005706 File Offset: 0x00003906
		public bool GetIsMouseDown()
		{
			return Input.IsKeyDown(InputKey.LeftMouseButton) || Input.IsKeyDown(InputKey.RightMouseButton);
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00005720 File Offset: 0x00003920
		public Vec2 GetMousePositionPixel()
		{
			return Input.MousePositionPixel;
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00005727 File Offset: 0x00003927
		public float GetDeltaMouseScroll()
		{
			if (!this.IsMouseWheelAllowed)
			{
				return 0f;
			}
			return Input.DeltaMouseScroll;
		}

		// Token: 0x06000133 RID: 307 RVA: 0x0000573C File Offset: 0x0000393C
		public bool GetIsControllerConnected()
		{
			return Input.IsControllerConnected;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00005743 File Offset: 0x00003943
		public Vec2 GetMousePositionRanged()
		{
			return Input.MousePositionRanged;
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000574A File Offset: 0x0000394A
		public float GetMouseSensitivity()
		{
			return Input.MouseSensitivity;
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00005751 File Offset: 0x00003951
		public bool IsControlDown()
		{
			return this.IsKeysAllowed && (Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl));
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00005772 File Offset: 0x00003972
		public bool IsShiftDown()
		{
			return this.IsKeysAllowed && (Input.IsKeyDown(InputKey.LeftShift) || Input.IsKeyDown(InputKey.RightShift));
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00005790 File Offset: 0x00003990
		public bool IsAltDown()
		{
			return this.IsKeysAllowed && (Input.IsKeyDown(InputKey.LeftAlt) || Input.IsKeyDown(InputKey.RightAlt));
		}

		// Token: 0x06000139 RID: 313 RVA: 0x000057B1 File Offset: 0x000039B1
		public InputKey GetControllerClickKey()
		{
			return Input.GetControllerClickKey();
		}

		// Token: 0x0400002E RID: 46
		private Dictionary<string, HotKey> _registeredHotKeys = new Dictionary<string, HotKey>();

		// Token: 0x0400002F RID: 47
		private List<GameKey> _registeredGameKeys = new List<GameKey>();

		// Token: 0x04000030 RID: 48
		private List<int> _lastFrameDownGameKeyIDs = new List<int>();

		// Token: 0x04000031 RID: 49
		private Dictionary<string, GameAxisKey> _registeredGameAxisKeys = new Dictionary<string, GameAxisKey>();

		// Token: 0x04000032 RID: 50
		private List<GameKey> _gameKeysToCurrentlyIgnore = new List<GameKey>();

		// Token: 0x04000033 RID: 51
		private List<HotKey> _hotKeysToCurrentlyIgnore = new List<HotKey>();

		// Token: 0x04000034 RID: 52
		private Dictionary<HotKey, bool> _lastFrameHotKeyDownMap = new Dictionary<HotKey, bool>();

		// Token: 0x04000035 RID: 53
		private bool _isDownMapsReset;

		// Token: 0x0400003A RID: 58
		private readonly List<GameKeyContext> _categories;
	}
}
