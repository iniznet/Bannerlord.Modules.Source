using System;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000038 RID: 56
	[ApplicationInterfaceBase]
	internal interface IInput
	{
		// Token: 0x06000501 RID: 1281
		[EngineMethod("clear_keys", false)]
		void ClearKeys();

		// Token: 0x06000502 RID: 1282
		[EngineMethod("get_mouse_sensitivity", false)]
		float GetMouseSensitivity();

		// Token: 0x06000503 RID: 1283
		[EngineMethod("get_mouse_delta_z", false)]
		float GetMouseDeltaZ();

		// Token: 0x06000504 RID: 1284
		[EngineMethod("is_mouse_active", false)]
		bool IsMouseActive();

		// Token: 0x06000505 RID: 1285
		[EngineMethod("is_controller_connected", false)]
		bool IsControllerConnected();

		// Token: 0x06000506 RID: 1286
		[EngineMethod("set_rumble_effect", false)]
		void SetRumbleEffect(float[] lowFrequencyLevels, float[] lowFrequencyDurations, int numLowFrequencyElements, float[] highFrequencyLevels, float[] highFrequencyDurations, int numHighFrequencyElements);

		// Token: 0x06000507 RID: 1287
		[EngineMethod("set_trigger_feedback", false)]
		void SetTriggerFeedback(byte leftTriggerPosition, byte leftTriggerStrength, byte rightTriggerPosition, byte rightTriggerStrength);

		// Token: 0x06000508 RID: 1288
		[EngineMethod("set_trigger_weapon_effect", false)]
		void SetTriggerWeaponEffect(byte leftStartPosition, byte leftEnd_position, byte leftStrength, byte rightStartPosition, byte rightEndPosition, byte rightStrength);

		// Token: 0x06000509 RID: 1289
		[EngineMethod("set_trigger_vibration", false)]
		void SetTriggerVibration(float[] leftTriggerAmplitudes, float[] leftTriggerFrequencies, float[] leftTriggerDurations, int numLeftTriggerElements, float[] rightTriggerAmplitudes, float[] rightTriggerFrequencies, float[] rightTriggerDurations, int numRightTriggerElements);

		// Token: 0x0600050A RID: 1290
		[EngineMethod("set_lightbar_color", false)]
		void SetLightbarColor(float red, float green, float blue);

		// Token: 0x0600050B RID: 1291
		[EngineMethod("press_key", false)]
		void PressKey(InputKey key);

		// Token: 0x0600050C RID: 1292
		[EngineMethod("get_virtual_key_code", false)]
		int GetVirtualKeyCode(InputKey key);

		// Token: 0x0600050D RID: 1293
		[EngineMethod("set_clipboard_text", false)]
		void SetClipboardText(string text);

		// Token: 0x0600050E RID: 1294
		[EngineMethod("get_clipboard_text", false)]
		string GetClipboardText();

		// Token: 0x0600050F RID: 1295
		[EngineMethod("update_key_data", false)]
		void UpdateKeyData(byte[] keyData);

		// Token: 0x06000510 RID: 1296
		[EngineMethod("get_mouse_move_x", false)]
		float GetMouseMoveX();

		// Token: 0x06000511 RID: 1297
		[EngineMethod("get_mouse_move_y", false)]
		float GetMouseMoveY();

		// Token: 0x06000512 RID: 1298
		[EngineMethod("get_mouse_position_x", false)]
		float GetMousePositionX();

		// Token: 0x06000513 RID: 1299
		[EngineMethod("get_mouse_position_y", false)]
		float GetMousePositionY();

		// Token: 0x06000514 RID: 1300
		[EngineMethod("get_mouse_scroll_value", false)]
		float GetMouseScrollValue();

		// Token: 0x06000515 RID: 1301
		[EngineMethod("get_key_state", false)]
		Vec2 GetKeyState(InputKey key);

		// Token: 0x06000516 RID: 1302
		[EngineMethod("is_key_down", false)]
		bool IsKeyDown(InputKey key);

		// Token: 0x06000517 RID: 1303
		[EngineMethod("is_key_down_immediate", false)]
		bool IsKeyDownImmediate(InputKey key);

		// Token: 0x06000518 RID: 1304
		[EngineMethod("is_key_pressed", false)]
		bool IsKeyPressed(InputKey key);

		// Token: 0x06000519 RID: 1305
		[EngineMethod("is_key_released", false)]
		bool IsKeyReleased(InputKey key);

		// Token: 0x0600051A RID: 1306
		[EngineMethod("set_cursor_position", false)]
		void SetCursorPosition(int x, int y);

		// Token: 0x0600051B RID: 1307
		[EngineMethod("set_cursor_friction_value", false)]
		void SetCursorFrictionValue(float frictionValue);
	}
}
