using System;
using TaleWorlds.Library;

namespace TaleWorlds.InputSystem
{
	public static class Input
	{
		public static InputState InputState { get; private set; }

		public static IInputContext DebugInput { get; private set; }

		public static IInputManager InputManager
		{
			get
			{
				return Input._inputManager;
			}
		}

		public static Vec2 Resolution
		{
			get
			{
				return Input._inputManager.GetResolution();
			}
		}

		public static Vec2 DesktopResolution
		{
			get
			{
				return Input._inputManager.GetDesktopResolution();
			}
		}

		public static void Initialize(IInputManager inputManager, IInputContext debugInput)
		{
			Input._inputManager = inputManager;
			Input.InputState = new InputState();
			Input.keyData = new byte[256];
			Input.DebugInput = new EmptyInputContext();
		}

		public static void UpdateKeyData(byte[] keyData)
		{
			Input._inputManager.UpdateKeyData(keyData);
		}

		public static float GetMouseMoveX()
		{
			return Input._inputManager.GetMouseMoveX();
		}

		public static float GetMouseMoveY()
		{
			return Input._inputManager.GetMouseMoveY();
		}

		public static float GetGyroX()
		{
			return Input._inputManager.GetGyroX();
		}

		public static float GetGyroY()
		{
			return Input._inputManager.GetGyroY();
		}

		public static float GetGyroZ()
		{
			return Input._inputManager.GetGyroZ();
		}

		public static Vec2 GetKeyState(InputKey key)
		{
			return Input._inputManager.GetKeyState(key);
		}

		public static bool IsKeyPressed(InputKey key)
		{
			return Input._inputManager.IsKeyPressed(key);
		}

		public static bool IsKeyDown(InputKey key)
		{
			return Input._inputManager.IsKeyDown(key);
		}

		public static bool IsKeyDownImmediate(InputKey key)
		{
			return Input._inputManager.IsKeyDownImmediate(key);
		}

		public static bool IsKeyReleased(InputKey key)
		{
			return Input._inputManager.IsKeyReleased(key);
		}

		public static bool IsControlOrShiftNotDown()
		{
			return !InputKey.LeftControl.IsDown() && !InputKey.RightControl.IsDown() && !InputKey.LeftShift.IsDown() && !InputKey.RightShift.IsDown();
		}

		public static bool IsMouseActive
		{
			get
			{
				return Input._inputManager.IsMouseActive();
			}
		}

		public static bool IsControllerConnected
		{
			get
			{
				return Input._inputManager.IsControllerConnected();
			}
		}

		public static bool IsGamepadActive
		{
			get
			{
				return Input._isGamepadActive;
			}
			private set
			{
				if (value != Input._isGamepadActive)
				{
					Input._isGamepadActive = value;
					Action onGamepadActiveStateChanged = Input.OnGamepadActiveStateChanged;
					if (onGamepadActiveStateChanged == null)
					{
						return;
					}
					onGamepadActiveStateChanged();
				}
			}
		}

		public static bool IsAnyTouchActive
		{
			get
			{
				return Input._isAnyTouchActive;
			}
			private set
			{
				if (value != Input._isAnyTouchActive)
				{
					Input._isAnyTouchActive = value;
				}
			}
		}

		public static Input.ControllerTypes ControllerType
		{
			get
			{
				return Input._controllerType;
			}
			private set
			{
				if (value != Input._controllerType)
				{
					Input._controllerType = value;
					Action<Input.ControllerTypes> onControllerTypeChanged = Input.OnControllerTypeChanged;
					if (onControllerTypeChanged == null)
					{
						return;
					}
					onControllerTypeChanged(value);
				}
			}
		}

		public static int GetFirstKeyPressedInRange(int startKeyNo)
		{
			int num = -1;
			for (int i = startKeyNo; i < 256; i++)
			{
				if (Input.IsKeyPressed((InputKey)i))
				{
					num = i;
					break;
				}
			}
			return num;
		}

		public static int GetFirstKeyDownInRange(int startKeyNo)
		{
			int num = -1;
			for (int i = startKeyNo; i < 256; i++)
			{
				if (Input.IsKeyDown((InputKey)i))
				{
					num = i;
					break;
				}
			}
			return num;
		}

		public static int GetFirstKeyReleasedInRange(int startKeyNo)
		{
			int num = -1;
			for (int i = startKeyNo; i < 256; i++)
			{
				if (Input.IsKeyReleased((InputKey)i))
				{
					num = i;
					break;
				}
			}
			return num;
		}

		public static void PressKey(InputKey key)
		{
			Input._inputManager.PressKey(key);
		}

		public static void ClearKeys()
		{
			Input._inputManager.ClearKeys();
		}

		public static int GetVirtualKeyCode(InputKey key)
		{
			return Input._inputManager.GetVirtualKeyCode(key);
		}

		public static bool IsDown(this InputKey key)
		{
			return Input.IsKeyDown(key);
		}

		public static bool IsPressed(this InputKey key)
		{
			return Input.IsKeyPressed(key);
		}

		public static bool IsReleased(this InputKey key)
		{
			return Input.IsKeyReleased(key);
		}

		public static void SetClipboardText(string text)
		{
			Input._inputManager.SetClipboardText(text);
		}

		public static string GetClipboardText()
		{
			return Input._inputManager.GetClipboardText();
		}

		public static float MouseMoveX
		{
			get
			{
				return Input._inputManager.GetMouseMoveX();
			}
		}

		public static float MouseMoveY
		{
			get
			{
				return Input._inputManager.GetMouseMoveY();
			}
		}

		public static float GyroX
		{
			get
			{
				return Input._inputManager.GetGyroX();
			}
		}

		public static float GyroY
		{
			get
			{
				return Input._inputManager.GetGyroY();
			}
		}

		public static float GyroZ
		{
			get
			{
				return Input._inputManager.GetGyroZ();
			}
		}

		public static float MouseSensitivity
		{
			get
			{
				return Input._inputManager.GetMouseSensitivity();
			}
		}

		public static float DeltaMouseScroll
		{
			get
			{
				return Input._inputManager.GetMouseDeltaZ();
			}
		}

		public static Vec2 MousePositionRanged
		{
			get
			{
				return Input.InputState.MousePositionRanged;
			}
		}

		public static Vec2 MousePositionPixel
		{
			get
			{
				return Input.InputState.MousePositionPixel;
			}
		}

		public static void Update()
		{
			float mousePositionX = Input._inputManager.GetMousePositionX();
			float mousePositionY = Input._inputManager.GetMousePositionY();
			float mouseScrollValue = Input._inputManager.GetMouseScrollValue();
			Input.IsMousePositionUpdated = Input.InputState.UpdateMousePosition(mousePositionX, mousePositionY);
			Input.IsMouseScrollChanged = Input.InputState.UpdateMouseScroll(mouseScrollValue);
			Input.IsGamepadActive = Input.IsControllerConnected && !Input.IsMouseActive;
			Input.IsAnyTouchActive = Input._inputManager.IsAnyTouchActive();
			Input.ControllerType = Input._inputManager.GetControllerType();
			Input.UpdateKeyData(Input.keyData);
		}

		public static bool IsMousePositionUpdated { get; private set; }

		public static bool IsMouseScrollChanged { get; private set; }

		public static bool IsControllerKey(InputKey key)
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
			case InputKey.LeftMouseButton:
			case InputKey.RightMouseButton:
			case InputKey.MiddleMouseButton:
			case InputKey.X1MouseButton:
			case InputKey.X2MouseButton:
			case InputKey.MouseScrollUp:
			case InputKey.MouseScrollDown:
				return false;
			}
			return true;
		}

		public static void SetMousePosition(int x, int y)
		{
			Input._inputManager.SetCursorPosition(x, y);
		}

		public static void SetCursorFriction(float frictionValue)
		{
			Input._inputManager.SetCursorFriction(frictionValue);
		}

		public static InputKey[] GetClickKeys()
		{
			return Input._inputManager.GetClickKeys();
		}

		public static void SetRumbleEffect(float[] lowFrequencyLevels, float[] lowFrequencyDurations, int numLowFrequencyElements, float[] highFrequencyLevels, float[] highFrequencyDurations, int numHighFrequencyElements)
		{
			Input._inputManager.SetRumbleEffect(lowFrequencyLevels, lowFrequencyDurations, numLowFrequencyElements, highFrequencyLevels, highFrequencyDurations, numHighFrequencyElements);
		}

		public static void SetTriggerFeedback(byte leftTriggerPosition, byte leftTriggerStrength, byte rightTriggerPosition, byte rightTriggerStrength)
		{
			Input._inputManager.SetTriggerFeedback(leftTriggerPosition, leftTriggerStrength, rightTriggerPosition, rightTriggerStrength);
		}

		public static void SetTriggerWeaponEffect(byte leftStartPosition, byte leftEnd_position, byte leftStrength, byte rightStartPosition, byte rightEndPosition, byte rightStrength)
		{
			Input._inputManager.SetTriggerWeaponEffect(leftStartPosition, leftEnd_position, leftStrength, rightStartPosition, rightEndPosition, rightStrength);
		}

		public static void SetTriggerVibration(float[] leftTriggerAmplitudes, float[] leftTriggerFrequencies, float[] leftTriggerDurations, int numLeftTriggerElements, float[] rightTriggerAmplitudes, float[] rightTriggerFrequencies, float[] rightTriggerDurations, int numRightTriggerElements)
		{
			Input._inputManager.SetTriggerVibration(leftTriggerAmplitudes, leftTriggerFrequencies, leftTriggerDurations, numLeftTriggerElements, rightTriggerAmplitudes, rightTriggerFrequencies, rightTriggerDurations, numRightTriggerElements);
		}

		public static void SetLightbarColor(float red, float green, float blue)
		{
			Input._inputManager.SetLightbarColor(red, green, blue);
		}

		public const int NumberOfKeys = 256;

		private static byte[] keyData;

		private static IInputManager _inputManager;

		public static Action OnGamepadActiveStateChanged;

		private static bool _isGamepadActive;

		private static bool _isAnyTouchActive;

		public static Action<Input.ControllerTypes> OnControllerTypeChanged;

		private static Input.ControllerTypes _controllerType;

		public enum ControllerTypes
		{
			None,
			Xbox,
			PlayStationDualShock,
			PlayStationDualSense = 4
		}
	}
}
