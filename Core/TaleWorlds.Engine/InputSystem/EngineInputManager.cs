using System;
using TaleWorlds.Engine.Options;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.Engine.InputSystem
{
	// Token: 0x020000A6 RID: 166
	public class EngineInputManager : IInputManager
	{
		// Token: 0x06000BFC RID: 3068 RVA: 0x0000E7BD File Offset: 0x0000C9BD
		float IInputManager.GetMousePositionX()
		{
			return EngineApplicationInterface.IInput.GetMousePositionX();
		}

		// Token: 0x06000BFD RID: 3069 RVA: 0x0000E7C9 File Offset: 0x0000C9C9
		float IInputManager.GetMousePositionY()
		{
			return EngineApplicationInterface.IInput.GetMousePositionY();
		}

		// Token: 0x06000BFE RID: 3070 RVA: 0x0000E7D5 File Offset: 0x0000C9D5
		float IInputManager.GetMouseScrollValue()
		{
			return EngineApplicationInterface.IInput.GetMouseScrollValue();
		}

		// Token: 0x06000BFF RID: 3071 RVA: 0x0000E7E1 File Offset: 0x0000C9E1
		bool IInputManager.IsMouseActive()
		{
			return EngineApplicationInterface.IInput.IsMouseActive();
		}

		// Token: 0x06000C00 RID: 3072 RVA: 0x0000E7ED File Offset: 0x0000C9ED
		bool IInputManager.IsControllerConnected()
		{
			return EngineApplicationInterface.IInput.IsControllerConnected();
		}

		// Token: 0x06000C01 RID: 3073 RVA: 0x0000E7F9 File Offset: 0x0000C9F9
		void IInputManager.PressKey(InputKey key)
		{
			EngineApplicationInterface.IInput.PressKey(key);
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x0000E806 File Offset: 0x0000CA06
		void IInputManager.ClearKeys()
		{
			EngineApplicationInterface.IInput.ClearKeys();
		}

		// Token: 0x06000C03 RID: 3075 RVA: 0x0000E812 File Offset: 0x0000CA12
		int IInputManager.GetVirtualKeyCode(InputKey key)
		{
			return EngineApplicationInterface.IInput.GetVirtualKeyCode(key);
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x0000E81F File Offset: 0x0000CA1F
		void IInputManager.SetClipboardText(string text)
		{
			EngineApplicationInterface.IInput.SetClipboardText(text);
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x0000E82C File Offset: 0x0000CA2C
		string IInputManager.GetClipboardText()
		{
			return EngineApplicationInterface.IInput.GetClipboardText();
		}

		// Token: 0x06000C06 RID: 3078 RVA: 0x0000E838 File Offset: 0x0000CA38
		float IInputManager.GetMouseMoveX()
		{
			return EngineApplicationInterface.IInput.GetMouseMoveX();
		}

		// Token: 0x06000C07 RID: 3079 RVA: 0x0000E844 File Offset: 0x0000CA44
		float IInputManager.GetMouseMoveY()
		{
			return EngineApplicationInterface.IInput.GetMouseMoveY();
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x0000E850 File Offset: 0x0000CA50
		float IInputManager.GetMouseSensitivity()
		{
			return EngineApplicationInterface.IInput.GetMouseSensitivity();
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x0000E85C File Offset: 0x0000CA5C
		float IInputManager.GetMouseDeltaZ()
		{
			return EngineApplicationInterface.IInput.GetMouseDeltaZ();
		}

		// Token: 0x06000C0A RID: 3082 RVA: 0x0000E868 File Offset: 0x0000CA68
		void IInputManager.UpdateKeyData(byte[] keyData)
		{
			EngineApplicationInterface.IInput.UpdateKeyData(keyData);
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x0000E875 File Offset: 0x0000CA75
		Vec2 IInputManager.GetKeyState(InputKey key)
		{
			return EngineApplicationInterface.IInput.GetKeyState(key);
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x0000E882 File Offset: 0x0000CA82
		bool IInputManager.IsKeyPressed(InputKey key)
		{
			return EngineApplicationInterface.IInput.IsKeyPressed(key);
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x0000E88F File Offset: 0x0000CA8F
		bool IInputManager.IsKeyDown(InputKey key)
		{
			return EngineApplicationInterface.IInput.IsKeyDown(key);
		}

		// Token: 0x06000C0E RID: 3086 RVA: 0x0000E89C File Offset: 0x0000CA9C
		bool IInputManager.IsKeyDownImmediate(InputKey key)
		{
			return EngineApplicationInterface.IInput.IsKeyDownImmediate(key);
		}

		// Token: 0x06000C0F RID: 3087 RVA: 0x0000E8A9 File Offset: 0x0000CAA9
		bool IInputManager.IsKeyReleased(InputKey key)
		{
			return EngineApplicationInterface.IInput.IsKeyReleased(key);
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x0000E8B6 File Offset: 0x0000CAB6
		Vec2 IInputManager.GetResolution()
		{
			return Screen.RealScreenResolution;
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x0000E8BD File Offset: 0x0000CABD
		Vec2 IInputManager.GetDesktopResolution()
		{
			return Screen.DesktopResolution;
		}

		// Token: 0x06000C12 RID: 3090 RVA: 0x0000E8C4 File Offset: 0x0000CAC4
		void IInputManager.SetCursorPosition(int x, int y)
		{
			float num = 1f;
			float num2 = 1f;
			if (NativeOptions.GetConfig(NativeOptions.NativeOptionsType.DisplayMode) != 0f)
			{
				num = Input.DesktopResolution.X / Input.Resolution.X;
				num2 = Input.DesktopResolution.Y / Input.Resolution.Y;
			}
			EngineApplicationInterface.IInput.SetCursorPosition((int)((float)x * num), (int)((float)y * num2));
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x0000E939 File Offset: 0x0000CB39
		void IInputManager.SetCursorFriction(float frictionValue)
		{
			EngineApplicationInterface.IInput.SetCursorFrictionValue(frictionValue);
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x0000E946 File Offset: 0x0000CB46
		InputKey IInputManager.GetControllerClickKey()
		{
			if (!EngineApplicationInterface.IScreen.IsEnterButtonCross())
			{
				return InputKey.ControllerRRight;
			}
			return InputKey.ControllerRDown;
		}

		// Token: 0x06000C15 RID: 3093 RVA: 0x0000E95F File Offset: 0x0000CB5F
		public void SetRumbleEffect(float[] lowFrequencyLevels, float[] lowFrequencyDurations, int numLowFrequencyElements, float[] highFrequencyLevels, float[] highFrequencyDurations, int numHighFrequencyElements)
		{
			EngineApplicationInterface.IInput.SetRumbleEffect(lowFrequencyLevels, lowFrequencyDurations, numLowFrequencyElements, highFrequencyLevels, highFrequencyDurations, numHighFrequencyElements);
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x0000E974 File Offset: 0x0000CB74
		public void SetTriggerFeedback(byte leftTriggerPosition, byte leftTriggerStrength, byte rightTriggerPosition, byte rightTriggerStrength)
		{
			EngineApplicationInterface.IInput.SetTriggerFeedback(leftTriggerPosition, leftTriggerStrength, rightTriggerPosition, rightTriggerStrength);
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x0000E985 File Offset: 0x0000CB85
		public void SetTriggerWeaponEffect(byte leftStartPosition, byte leftEnd_position, byte leftStrength, byte rightStartPosition, byte rightEndPosition, byte rightStrength)
		{
			EngineApplicationInterface.IInput.SetTriggerWeaponEffect(leftStartPosition, leftEnd_position, leftStrength, rightStartPosition, rightEndPosition, rightStrength);
		}

		// Token: 0x06000C18 RID: 3096 RVA: 0x0000E99C File Offset: 0x0000CB9C
		public void SetTriggerVibration(float[] leftTriggerAmplitudes, float[] leftTriggerFrequencies, float[] leftTriggerDurations, int numLeftTriggerElements, float[] rightTriggerAmplitudes, float[] rightTriggerFrequencies, float[] rightTriggerDurations, int numRightTriggerElements)
		{
			EngineApplicationInterface.IInput.SetTriggerVibration(leftTriggerAmplitudes, leftTriggerFrequencies, leftTriggerDurations, numLeftTriggerElements, rightTriggerAmplitudes, rightTriggerFrequencies, rightTriggerDurations, numRightTriggerElements);
		}

		// Token: 0x06000C19 RID: 3097 RVA: 0x0000E9C0 File Offset: 0x0000CBC0
		public void SetLightbarColor(float red, float green, float blue)
		{
			EngineApplicationInterface.IInput.SetLightbarColor(red, green, blue);
		}
	}
}
