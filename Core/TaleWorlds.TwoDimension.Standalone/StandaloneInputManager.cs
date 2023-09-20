using System;
using System.Drawing;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;

namespace TaleWorlds.TwoDimension.Standalone
{
	public class StandaloneInputManager : IInputManager
	{
		public StandaloneInputManager(GraphicsForm graphicsForm)
		{
			this._graphicsForm = graphicsForm;
		}

		float IInputManager.GetMousePositionX()
		{
			return this._graphicsForm.MousePosition().X / (float)this._graphicsForm.Width;
		}

		float IInputManager.GetMousePositionY()
		{
			return this._graphicsForm.MousePosition().Y / (float)this._graphicsForm.Height;
		}

		float IInputManager.GetMouseScrollValue()
		{
			return 0f;
		}

		bool IInputManager.IsMouseActive()
		{
			return true;
		}

		bool IInputManager.IsAnyTouchActive()
		{
			return false;
		}

		bool IInputManager.IsControllerConnected()
		{
			return false;
		}

		void IInputManager.PressKey(InputKey key)
		{
		}

		void IInputManager.ClearKeys()
		{
		}

		int IInputManager.GetVirtualKeyCode(InputKey key)
		{
			return -1;
		}

		void IInputManager.SetClipboardText(string text)
		{
		}

		string IInputManager.GetClipboardText()
		{
			return "";
		}

		float IInputManager.GetMouseMoveX()
		{
			throw new NotImplementedException();
		}

		float IInputManager.GetMouseMoveY()
		{
			throw new NotImplementedException();
		}

		float IInputManager.GetGyroX()
		{
			throw new NotImplementedException();
		}

		float IInputManager.GetGyroY()
		{
			throw new NotImplementedException();
		}

		float IInputManager.GetGyroZ()
		{
			throw new NotImplementedException();
		}

		float IInputManager.GetMouseSensitivity()
		{
			return 1f;
		}

		float IInputManager.GetMouseDeltaZ()
		{
			return this._graphicsForm.GetMouseDeltaZ();
		}

		void IInputManager.UpdateKeyData(byte[] keyData)
		{
		}

		Vec2 IInputManager.GetKeyState(InputKey key)
		{
			if (!this._graphicsForm.GetKey(key))
			{
				return new Vec2(0f, 0f);
			}
			return new Vec2(1f, 0f);
		}

		bool IInputManager.IsKeyPressed(InputKey key)
		{
			return this._graphicsForm.GetKeyDown(key);
		}

		bool IInputManager.IsKeyDown(InputKey key)
		{
			return this._graphicsForm.GetKey(key);
		}

		bool IInputManager.IsKeyDownImmediate(InputKey key)
		{
			return this._graphicsForm.GetKey(key);
		}

		bool IInputManager.IsKeyReleased(InputKey key)
		{
			return this._graphicsForm.GetKeyUp(key);
		}

		Vec2 IInputManager.GetResolution()
		{
			return new Vec2((float)this._graphicsForm.Width, (float)this._graphicsForm.Height);
		}

		Vec2 IInputManager.GetDesktopResolution()
		{
			Rectangle rectangle;
			User32.GetClientRect(User32.GetDesktopWindow(), out rectangle);
			return new Vec2((float)rectangle.Width, (float)rectangle.Height);
		}

		void IInputManager.SetCursorPosition(int x, int y)
		{
		}

		void IInputManager.SetCursorFriction(float frictionValue)
		{
		}

		InputKey[] IInputManager.GetClickKeys()
		{
			return new InputKey[]
			{
				InputKey.LeftMouseButton,
				InputKey.ControllerRDown
			};
		}

		public void SetRumbleEffect(float[] lowFrequencyLevels, float[] lowFrequencyDurations, int numLowFrequencyElements, float[] highFrequencyLevels, float[] highFrequencyDurations, int numHighFrequencyElements)
		{
		}

		public void SetTriggerFeedback(byte leftTriggerPosition, byte leftTriggerStrength, byte rightTriggerPosition, byte rightTriggerStrength)
		{
		}

		public void SetTriggerWeaponEffect(byte leftStartPosition, byte leftEnd_position, byte leftStrength, byte rightStartPosition, byte rightEndPosition, byte rightStrength)
		{
		}

		public void SetTriggerVibration(float[] leftTriggerAmplitudes, float[] leftTriggerFrequencies, float[] leftTriggerDurations, int numLeftTriggerElements, float[] rightTriggerAmplitudes, float[] rightTriggerFrequencies, float[] rightTriggerDurations, int numRightTriggerElements)
		{
		}

		public void SetLightbarColor(float red, float green, float blue)
		{
		}

		Input.ControllerTypes IInputManager.GetControllerType()
		{
			return Input.ControllerTypes.Xbox;
		}

		private GraphicsForm _graphicsForm;
	}
}
