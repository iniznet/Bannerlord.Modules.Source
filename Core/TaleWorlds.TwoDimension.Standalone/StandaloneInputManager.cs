using System;
using System.Drawing;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;

namespace TaleWorlds.TwoDimension.Standalone
{
	// Token: 0x0200000A RID: 10
	public class StandaloneInputManager : IInputManager
	{
		// Token: 0x06000079 RID: 121 RVA: 0x000040B5 File Offset: 0x000022B5
		public StandaloneInputManager(GraphicsForm graphicsForm)
		{
			this._graphicsForm = graphicsForm;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000040C4 File Offset: 0x000022C4
		float IInputManager.GetMousePositionX()
		{
			return this._graphicsForm.MousePosition().X / (float)this._graphicsForm.Width;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000040E3 File Offset: 0x000022E3
		float IInputManager.GetMousePositionY()
		{
			return this._graphicsForm.MousePosition().Y / (float)this._graphicsForm.Height;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00004102 File Offset: 0x00002302
		float IInputManager.GetMouseScrollValue()
		{
			return 0f;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00004109 File Offset: 0x00002309
		bool IInputManager.IsMouseActive()
		{
			return true;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x0000410C File Offset: 0x0000230C
		bool IInputManager.IsControllerConnected()
		{
			return false;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x0000410F File Offset: 0x0000230F
		void IInputManager.PressKey(InputKey key)
		{
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00004111 File Offset: 0x00002311
		void IInputManager.ClearKeys()
		{
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00004113 File Offset: 0x00002313
		int IInputManager.GetVirtualKeyCode(InputKey key)
		{
			return -1;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00004116 File Offset: 0x00002316
		void IInputManager.SetClipboardText(string text)
		{
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00004118 File Offset: 0x00002318
		string IInputManager.GetClipboardText()
		{
			return "";
		}

		// Token: 0x06000084 RID: 132 RVA: 0x0000411F File Offset: 0x0000231F
		float IInputManager.GetMouseMoveX()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00004126 File Offset: 0x00002326
		float IInputManager.GetMouseMoveY()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000086 RID: 134 RVA: 0x0000412D File Offset: 0x0000232D
		float IInputManager.GetMouseSensitivity()
		{
			return 1f;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00004134 File Offset: 0x00002334
		float IInputManager.GetMouseDeltaZ()
		{
			return this._graphicsForm.GetMouseDeltaZ();
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00004141 File Offset: 0x00002341
		void IInputManager.UpdateKeyData(byte[] keyData)
		{
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00004143 File Offset: 0x00002343
		Vec2 IInputManager.GetKeyState(InputKey key)
		{
			if (!this._graphicsForm.GetKey(key))
			{
				return new Vec2(0f, 0f);
			}
			return new Vec2(1f, 0f);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00004172 File Offset: 0x00002372
		bool IInputManager.IsKeyPressed(InputKey key)
		{
			return this._graphicsForm.GetKeyDown(key);
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00004180 File Offset: 0x00002380
		bool IInputManager.IsKeyDown(InputKey key)
		{
			return this._graphicsForm.GetKey(key);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x0000418E File Offset: 0x0000238E
		bool IInputManager.IsKeyDownImmediate(InputKey key)
		{
			return this._graphicsForm.GetKey(key);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x0000419C File Offset: 0x0000239C
		bool IInputManager.IsKeyReleased(InputKey key)
		{
			return this._graphicsForm.GetKeyUp(key);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000041AA File Offset: 0x000023AA
		Vec2 IInputManager.GetResolution()
		{
			return new Vec2((float)this._graphicsForm.Width, (float)this._graphicsForm.Height);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000041CC File Offset: 0x000023CC
		Vec2 IInputManager.GetDesktopResolution()
		{
			Rectangle rectangle;
			User32.GetClientRect(User32.GetDesktopWindow(), out rectangle);
			return new Vec2((float)rectangle.Width, (float)rectangle.Height);
		}

		// Token: 0x06000090 RID: 144 RVA: 0x000041FB File Offset: 0x000023FB
		void IInputManager.SetCursorPosition(int x, int y)
		{
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000041FD File Offset: 0x000023FD
		void IInputManager.SetCursorFriction(float frictionValue)
		{
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000041FF File Offset: 0x000023FF
		InputKey IInputManager.GetControllerClickKey()
		{
			return InputKey.ControllerRDown;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00004206 File Offset: 0x00002406
		public void SetRumbleEffect(float[] lowFrequencyLevels, float[] lowFrequencyDurations, int numLowFrequencyElements, float[] highFrequencyLevels, float[] highFrequencyDurations, int numHighFrequencyElements)
		{
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00004208 File Offset: 0x00002408
		public void SetTriggerFeedback(byte leftTriggerPosition, byte leftTriggerStrength, byte rightTriggerPosition, byte rightTriggerStrength)
		{
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000420A File Offset: 0x0000240A
		public void SetTriggerWeaponEffect(byte leftStartPosition, byte leftEnd_position, byte leftStrength, byte rightStartPosition, byte rightEndPosition, byte rightStrength)
		{
		}

		// Token: 0x06000096 RID: 150 RVA: 0x0000420C File Offset: 0x0000240C
		public void SetTriggerVibration(float[] leftTriggerAmplitudes, float[] leftTriggerFrequencies, float[] leftTriggerDurations, int numLeftTriggerElements, float[] rightTriggerAmplitudes, float[] rightTriggerFrequencies, float[] rightTriggerDurations, int numRightTriggerElements)
		{
		}

		// Token: 0x06000097 RID: 151 RVA: 0x0000420E File Offset: 0x0000240E
		public void SetLightbarColor(float red, float green, float blue)
		{
		}

		// Token: 0x0400003D RID: 61
		private GraphicsForm _graphicsForm;
	}
}
