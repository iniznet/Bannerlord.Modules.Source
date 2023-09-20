﻿using System;
using TaleWorlds.Library;

namespace TaleWorlds.InputSystem
{
	public interface IInputManager
	{
		float GetMousePositionX();

		float GetMousePositionY();

		float GetMouseScrollValue();

		bool IsMouseActive();

		bool IsControllerConnected();

		void PressKey(InputKey key);

		void ClearKeys();

		int GetVirtualKeyCode(InputKey key);

		void SetClipboardText(string text);

		string GetClipboardText();

		float GetMouseMoveX();

		float GetMouseMoveY();

		float GetMouseSensitivity();

		float GetMouseDeltaZ();

		void UpdateKeyData(byte[] keyData);

		Vec2 GetKeyState(InputKey key);

		bool IsKeyPressed(InputKey key);

		bool IsKeyDown(InputKey key);

		bool IsKeyDownImmediate(InputKey key);

		bool IsKeyReleased(InputKey key);

		Vec2 GetResolution();

		Vec2 GetDesktopResolution();

		void SetCursorPosition(int x, int y);

		void SetCursorFriction(float frictionValue);

		InputKey GetControllerClickKey();

		void SetRumbleEffect(float[] lowFrequencyLevels, float[] lowFrequencyDurations, int numLowFrequencyElements, float[] highFrequencyLevels, float[] highFrequencyDurations, int numHighFrequencyElements);

		void SetTriggerFeedback(byte leftTriggerPosition, byte leftTriggerStrength, byte rightTriggerPosition, byte rightTriggerStrength);

		void SetTriggerWeaponEffect(byte leftStartPosition, byte leftEnd_position, byte leftStrength, byte rightStartPosition, byte rightEndPosition, byte rightStrength);

		void SetTriggerVibration(float[] leftTriggerAmplitudes, float[] leftTriggerFrequencies, float[] leftTriggerDurations, int numLeftTriggerElements, float[] rightTriggerAmplitudes, float[] rightTriggerFrequencies, float[] rightTriggerDurations, int numRightTriggerElements);

		void SetLightbarColor(float red, float green, float blue);
	}
}
