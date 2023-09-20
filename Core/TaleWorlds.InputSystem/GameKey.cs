using System;

namespace TaleWorlds.InputSystem
{
	public class GameKey
	{
		public int Id { get; private set; }

		public string StringId { get; private set; }

		public string GroupId { get; private set; }

		public string MainCategoryId { get; private set; }

		public Key KeyboardKey { get; internal set; }

		public Key DefaultKeyboardKey { get; private set; }

		public Key ControllerKey { get; internal set; }

		public Key DefaultControllerKey { get; internal set; }

		public GameKey(int id, string stringId, string groupId, InputKey defaultKeyboardKey, InputKey defaultControllerKey, string mainCategoryId = "")
		{
			this.Id = id;
			this.StringId = stringId;
			this.GroupId = groupId;
			this.MainCategoryId = mainCategoryId;
			this.KeyboardKey = ((defaultKeyboardKey != InputKey.Invalid) ? new Key(defaultKeyboardKey) : null);
			this.DefaultKeyboardKey = ((defaultKeyboardKey != InputKey.Invalid) ? new Key(defaultKeyboardKey) : null);
			this.ControllerKey = ((defaultControllerKey != InputKey.Invalid) ? new Key(defaultControllerKey) : null);
			this.DefaultControllerKey = ((defaultControllerKey != InputKey.Invalid) ? new Key(defaultControllerKey) : null);
		}

		public GameKey(int id, string stringId, string groupId, InputKey defaultKeyboardKey, string mainCategoryId = "")
		{
			this.Id = id;
			this.StringId = stringId;
			this.GroupId = groupId;
			this.MainCategoryId = mainCategoryId;
			this.KeyboardKey = ((defaultKeyboardKey != InputKey.Invalid) ? new Key(defaultKeyboardKey) : null);
			this.DefaultKeyboardKey = ((defaultKeyboardKey != InputKey.Invalid) ? new Key(defaultKeyboardKey) : null);
			this.ControllerKey = new Key(InputKey.Invalid);
			this.DefaultControllerKey = new Key(InputKey.Invalid);
		}

		private bool IsKeyAllowed(Key key, bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			return (isKeysAllowed || !key.IsKeyboardInput) && (isMouseButtonAllowed || !key.IsMouseButtonInput) && (isMouseWheelAllowed || !key.IsMouseWheelInput) && (isControllerAllowed || !key.IsControllerInput);
		}

		internal bool IsUp(bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			bool flag = false;
			if (this.KeyboardKey != null && this.IsKeyAllowed(this.KeyboardKey, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
			{
				flag = flag || !this.KeyboardKey.IsDown();
			}
			if (this.ControllerKey != null && this.IsKeyAllowed(this.ControllerKey, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
			{
				flag = flag || !this.ControllerKey.IsDown();
			}
			return flag;
		}

		internal bool IsDown(bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed, bool checkControllerKey = true)
		{
			bool flag = false;
			if (this.KeyboardKey != null && this.IsKeyAllowed(this.KeyboardKey, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
			{
				flag = flag || this.KeyboardKey.IsDown();
			}
			if (checkControllerKey && this.ControllerKey != null && this.IsKeyAllowed(this.ControllerKey, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
			{
				flag = flag || this.ControllerKey.IsDown();
			}
			return flag;
		}

		internal bool IsDownImmediate(bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			bool flag = false;
			if (this.KeyboardKey != null && this.IsKeyAllowed(this.KeyboardKey, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
			{
				flag = flag || this.KeyboardKey.IsDownImmediate();
			}
			if (this.ControllerKey != null && this.IsKeyAllowed(this.ControllerKey, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
			{
				flag = flag || this.ControllerKey.IsDownImmediate();
			}
			return flag;
		}

		internal bool IsPressed(bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			bool flag = false;
			if (this.KeyboardKey != null && this.IsKeyAllowed(this.KeyboardKey, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
			{
				flag = flag || this.KeyboardKey.IsPressed();
			}
			if (this.ControllerKey != null && this.IsKeyAllowed(this.ControllerKey, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
			{
				flag = flag || this.ControllerKey.IsPressed();
			}
			return flag;
		}

		internal bool IsReleased(bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			bool flag = false;
			if (this.KeyboardKey != null && this.IsKeyAllowed(this.KeyboardKey, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
			{
				flag = flag || this.KeyboardKey.IsReleased();
			}
			if (this.ControllerKey != null && this.IsKeyAllowed(this.ControllerKey, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
			{
				flag = flag || this.ControllerKey.IsReleased();
			}
			return flag;
		}

		internal float GetKeyState(bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			float num = 0f;
			if (this.KeyboardKey != null && this.IsKeyAllowed(this.KeyboardKey, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
			{
				num = this.KeyboardKey.GetKeyState().X;
			}
			if (num == 0f && this.ControllerKey != null && this.IsKeyAllowed(this.ControllerKey, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
			{
				num = this.ControllerKey.GetKeyState().X;
			}
			return num;
		}

		public override string ToString()
		{
			string text = "invalid";
			bool flag = Input.IsControllerConnected && !Input.IsMouseActive;
			if (!flag && this.KeyboardKey != null)
			{
				text = this.KeyboardKey.ToString();
			}
			else if (flag && this.ControllerKey != null)
			{
				text = this.ControllerKey.ToString();
			}
			return text;
		}

		public override bool Equals(object obj)
		{
			GameKey gameKey = obj as GameKey;
			return gameKey != null && gameKey.Id.Equals(this.Id) && gameKey.GroupId.Equals(this.GroupId) && gameKey.KeyboardKey == this.KeyboardKey && gameKey.ControllerKey == this.ControllerKey;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
