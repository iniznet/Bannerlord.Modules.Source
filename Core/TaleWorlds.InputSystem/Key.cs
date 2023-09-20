using System;
using TaleWorlds.Library;

namespace TaleWorlds.InputSystem
{
	// Token: 0x02000010 RID: 16
	[Serializable]
	public class Key
	{
		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600014C RID: 332 RVA: 0x00005A93 File Offset: 0x00003C93
		// (set) Token: 0x0600014D RID: 333 RVA: 0x00005A9B File Offset: 0x00003C9B
		public bool IsKeyboardInput { get; private set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600014E RID: 334 RVA: 0x00005AA4 File Offset: 0x00003CA4
		// (set) Token: 0x0600014F RID: 335 RVA: 0x00005AAC File Offset: 0x00003CAC
		public bool IsMouseButtonInput { get; private set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000150 RID: 336 RVA: 0x00005AB5 File Offset: 0x00003CB5
		// (set) Token: 0x06000151 RID: 337 RVA: 0x00005ABD File Offset: 0x00003CBD
		public bool IsMouseWheelInput { get; private set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000152 RID: 338 RVA: 0x00005AC6 File Offset: 0x00003CC6
		// (set) Token: 0x06000153 RID: 339 RVA: 0x00005ACE File Offset: 0x00003CCE
		public bool IsControllerInput { get; private set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000154 RID: 340 RVA: 0x00005AD7 File Offset: 0x00003CD7
		// (set) Token: 0x06000155 RID: 341 RVA: 0x00005ADF File Offset: 0x00003CDF
		public InputKey InputKey { get; private set; }

		// Token: 0x06000156 RID: 342 RVA: 0x00005AE8 File Offset: 0x00003CE8
		public Key(InputKey key)
		{
			this.ChangeKey(key);
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00005AF7 File Offset: 0x00003CF7
		public Key()
		{
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00005B00 File Offset: 0x00003D00
		public void ChangeKey(InputKey key)
		{
			this.InputKey = key;
			this.IsKeyboardInput = Key.GetInputType(key) == Key.InputType.Keyboard;
			this.IsMouseButtonInput = Key.GetInputType(key) == Key.InputType.MouseButton;
			this.IsMouseWheelInput = Key.GetInputType(key) == Key.InputType.MouseWheel;
			this.IsControllerInput = Key.GetInputType(key) == Key.InputType.Controller;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00005B50 File Offset: 0x00003D50
		internal bool IsPressed()
		{
			return Input.IsKeyPressed(this.InputKey);
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00005B5D File Offset: 0x00003D5D
		internal bool IsDown()
		{
			return Input.IsKeyDown(this.InputKey);
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00005B6A File Offset: 0x00003D6A
		internal bool IsDownImmediate()
		{
			if (this.IsKeyboardInput || this.IsMouseButtonInput)
			{
				return Input.IsKeyDownImmediate(this.InputKey);
			}
			return Input.IsKeyDown(this.InputKey);
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00005B93 File Offset: 0x00003D93
		internal bool IsReleased()
		{
			return Input.IsKeyReleased(this.InputKey);
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00005BA0 File Offset: 0x00003DA0
		internal Vec2 GetKeyState()
		{
			return Input.GetKeyState(this.InputKey);
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00005BB0 File Offset: 0x00003DB0
		public override string ToString()
		{
			if (this.IsKeyboardInput)
			{
				int virtualKeyCode = Input.GetVirtualKeyCode(this.InputKey);
				if (virtualKeyCode != 0)
				{
					VirtualKeyCode virtualKeyCode2 = (VirtualKeyCode)virtualKeyCode;
					return virtualKeyCode2.ToString();
				}
			}
			return this.InputKey.ToString();
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00005BF8 File Offset: 0x00003DF8
		public override bool Equals(object obj)
		{
			return (obj as Key).InputKey == this.InputKey;
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00005C0D File Offset: 0x00003E0D
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00005C15 File Offset: 0x00003E15
		public static bool operator ==(Key k1, Key k2)
		{
			return k1 == k2 || (k1 != null && k2 != null && k1.InputKey == k2.InputKey);
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00005C33 File Offset: 0x00003E33
		public static bool operator !=(Key k1, Key k2)
		{
			return !(k1 == k2);
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00005C3F File Offset: 0x00003E3F
		public static bool IsLeftAnalogInput(InputKey key)
		{
			return key == InputKey.ControllerLStick || key - InputKey.ControllerLStickUp <= 3 || key == InputKey.ControllerLThumb;
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00005C5E File Offset: 0x00003E5E
		public static bool IsLeftBumperOrTriggerInput(InputKey key)
		{
			return key == InputKey.ControllerLBumper || key == InputKey.ControllerLTrigger;
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00005C73 File Offset: 0x00003E73
		public static bool IsRightBumperOrTriggerInput(InputKey key)
		{
			return key == InputKey.ControllerRBumper || key == InputKey.ControllerRTrigger;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00005C88 File Offset: 0x00003E88
		public static bool IsFaceKeyInput(InputKey key)
		{
			return key - InputKey.ControllerRUp <= 3;
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00005C97 File Offset: 0x00003E97
		public static bool IsRightAnalogInput(InputKey key)
		{
			return key == InputKey.ControllerRStick || key - InputKey.ControllerRStickUp <= 3 || key == InputKey.ControllerRThumb;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00005CB6 File Offset: 0x00003EB6
		public static bool IsDpadInput(InputKey key)
		{
			return key - InputKey.ControllerLUp <= 3;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00005CC8 File Offset: 0x00003EC8
		public static Key.InputType GetInputType(InputKey key)
		{
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
			case InputKey.Extended:
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
				return Key.InputType.Keyboard;
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
			case InputKey.ControllerLThumb:
			case InputKey.ControllerRThumb:
			case InputKey.ControllerLTrigger:
			case InputKey.ControllerRTrigger:
				return Key.InputType.Controller;
			case InputKey.LeftMouseButton:
			case InputKey.RightMouseButton:
			case InputKey.MiddleMouseButton:
			case InputKey.X1MouseButton:
			case InputKey.X2MouseButton:
				return Key.InputType.MouseButton;
			case InputKey.MouseScrollUp:
			case InputKey.MouseScrollDown:
				return Key.InputType.MouseWheel;
			}
			return Key.InputType.Invalid;
		}

		// Token: 0x02000015 RID: 21
		public enum InputType
		{
			// Token: 0x0400015B RID: 347
			Invalid = -1,
			// Token: 0x0400015C RID: 348
			Keyboard,
			// Token: 0x0400015D RID: 349
			MouseButton,
			// Token: 0x0400015E RID: 350
			MouseWheel,
			// Token: 0x0400015F RID: 351
			Controller
		}
	}
}
