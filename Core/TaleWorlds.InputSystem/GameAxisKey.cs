using System;
using TaleWorlds.Library;

namespace TaleWorlds.InputSystem
{
	public class GameAxisKey
	{
		public string Id { get; private set; }

		public Key AxisKey { get; internal set; }

		public Key DefaultAxisKey { get; private set; }

		public GameKey PositiveKey { get; internal set; }

		public GameKey NegativeKey { get; internal set; }

		public GameAxisKey.AxisType Type { get; private set; }

		internal bool IsBinded { get; private set; }

		public GameAxisKey(string id, InputKey axisKey, GameKey positiveKey, GameKey negativeKey, GameAxisKey.AxisType type = GameAxisKey.AxisType.X)
		{
			this.Id = id;
			this.AxisKey = new Key(axisKey);
			this.DefaultAxisKey = new Key(axisKey);
			this.PositiveKey = positiveKey;
			this.NegativeKey = negativeKey;
			this.Type = type;
			this.IsBinded = this.PositiveKey != null || this.NegativeKey != null;
		}

		private bool IsKeyAllowed(Key key, bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			return (isKeysAllowed || !key.IsKeyboardInput) && (isMouseButtonAllowed || !key.IsMouseButtonInput) && (isMouseWheelAllowed || !key.IsMouseWheelInput) && (isControllerAllowed || !key.IsControllerInput);
		}

		public float GetAxisState(bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			GameKey positiveKey = this.PositiveKey;
			bool flag = positiveKey != null && positiveKey.IsDown(isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed, false);
			GameKey negativeKey = this.NegativeKey;
			bool flag2 = negativeKey != null && negativeKey.IsDown(isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed, false);
			if (flag || flag2)
			{
				return (flag ? 1f : 0f) - (flag2 ? 1f : 0f);
			}
			Vec2 keyState = new Vec2(0f, 0f);
			if (this.AxisKey != null && this.IsKeyAllowed(this.AxisKey, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
			{
				keyState = this.AxisKey.GetKeyState();
			}
			if (this.Type == GameAxisKey.AxisType.X)
			{
				return keyState.X;
			}
			if (this.Type == GameAxisKey.AxisType.Y)
			{
				return keyState.Y;
			}
			return 0f;
		}

		public override string ToString()
		{
			string text = "";
			if (this.AxisKey != null)
			{
				text = this.AxisKey.ToString();
			}
			return text;
		}

		public enum AxisType
		{
			X,
			Y
		}
	}
}
