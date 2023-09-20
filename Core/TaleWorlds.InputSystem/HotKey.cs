using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.InputSystem
{
	public class HotKey
	{
		private bool _isDoublePressActive
		{
			get
			{
				int num = Environment.TickCount - this._doublePressTime;
				return num < 500 && num >= 0;
			}
		}

		public List<Key> Keys { get; internal set; }

		public List<Key> DefaultKeys { get; private set; }

		public HotKey(string id, string groupId, List<Key> keys, HotKey.Modifiers modifiers = HotKey.Modifiers.None, HotKey.Modifiers negativeModifiers = HotKey.Modifiers.None)
		{
			this.Id = id;
			this.GroupId = groupId;
			this.Keys = keys;
			this.DefaultKeys = new List<Key>();
			for (int i = 0; i < this.Keys.Count; i++)
			{
				this.DefaultKeys.Add(new Key(this.Keys[i].InputKey));
			}
			this._modifiers = modifiers;
			this._negativeModifiers = negativeModifiers;
		}

		public HotKey(string id, string groupId, InputKey inputKey, HotKey.Modifiers modifiers = HotKey.Modifiers.None, HotKey.Modifiers negativeModifiers = HotKey.Modifiers.None)
		{
			this.Id = id;
			this.GroupId = groupId;
			this.Keys = new List<Key>
			{
				new Key(inputKey)
			};
			this.DefaultKeys = new List<Key>
			{
				new Key(inputKey)
			};
			this._modifiers = modifiers;
			this._negativeModifiers = negativeModifiers;
		}

		private bool IsKeyAllowed(Key key, bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			return (isKeysAllowed || !key.IsKeyboardInput) && (isMouseButtonAllowed || !key.IsMouseButtonInput) && (isMouseWheelAllowed || !key.IsMouseWheelInput) && (isControllerAllowed || !key.IsControllerInput);
		}

		private bool CheckModifiers()
		{
			bool flag = Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl);
			bool flag2 = Input.IsKeyDown(InputKey.LeftAlt) || Input.IsKeyDown(InputKey.RightAlt);
			bool flag3 = Input.IsKeyDown(InputKey.LeftShift) || Input.IsKeyDown(InputKey.RightShift);
			bool flag4 = true;
			bool flag5 = true;
			bool flag6 = true;
			if (this._modifiers.HasAnyFlag(HotKey.Modifiers.Control))
			{
				flag4 = flag;
			}
			if (this._modifiers.HasAnyFlag(HotKey.Modifiers.Alt))
			{
				flag5 = flag2;
			}
			if (this._modifiers.HasAnyFlag(HotKey.Modifiers.Shift))
			{
				flag6 = flag3;
			}
			if (this._negativeModifiers.HasAnyFlag(HotKey.Modifiers.Control))
			{
				flag4 = !flag;
			}
			if (this._negativeModifiers.HasAnyFlag(HotKey.Modifiers.Alt))
			{
				flag5 = !flag2;
			}
			if (this._negativeModifiers.HasAnyFlag(HotKey.Modifiers.Shift))
			{
				flag6 = !flag3;
			}
			return flag4 && flag5 && flag6;
		}

		private bool IsDown(Key key, bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			return this.IsKeyAllowed(key, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed) && (this._modifiers == HotKey.Modifiers.None || this.CheckModifiers()) && key.IsDown();
		}

		internal bool IsDown(bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			foreach (Key key in this.Keys)
			{
				if (this.IsDown(key, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsDownImmediate(Key key, bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			return this.IsKeyAllowed(key, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed) && (this._modifiers == HotKey.Modifiers.None || this.CheckModifiers()) && key.IsDownImmediate();
		}

		internal bool IsDownImmediate(bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			foreach (Key key in this.Keys)
			{
				if (this.IsDownImmediate(key, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsDoublePressed(Key key, bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			if (!this.IsKeyAllowed(key, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
			{
				return false;
			}
			if (this._modifiers != HotKey.Modifiers.None && !this.CheckModifiers())
			{
				return false;
			}
			if (key.IsPressed())
			{
				if (this._isDoublePressActive)
				{
					this._doublePressTime = 0;
					return true;
				}
				this._doublePressTime = Environment.TickCount;
			}
			return false;
		}

		internal bool IsDoublePressed(bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			foreach (Key key in this.Keys)
			{
				if (this.IsDoublePressed(key, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsPressed(Key key, bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			return this.IsKeyAllowed(key, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed) && (this._modifiers == HotKey.Modifiers.None || this.CheckModifiers()) && key.IsPressed();
		}

		internal bool IsPressed(bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			foreach (Key key in this.Keys)
			{
				if (this.IsPressed(key, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsReleased(Key key, bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			return this.IsKeyAllowed(key, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed) && (this._modifiers == HotKey.Modifiers.None || this.CheckModifiers()) && key.IsReleased();
		}

		internal bool IsReleased(bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			foreach (Key key in this.Keys)
			{
				if (this.IsReleased(key, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed))
				{
					return true;
				}
			}
			return false;
		}

		public bool HasModifier(HotKey.Modifiers modifier)
		{
			return this._modifiers.HasAnyFlag(modifier);
		}

		public bool HasSameModifiers(HotKey other)
		{
			return this._modifiers == other._modifiers;
		}

		public override string ToString()
		{
			string text = "";
			bool flag = Input.IsControllerConnected && !Input.IsMouseActive;
			for (int i = 0; i < this.Keys.Count; i++)
			{
				if ((!flag && !this.Keys[i].IsControllerInput) || (flag && this.Keys[i].IsControllerInput))
				{
					return this.Keys[i].ToString();
				}
			}
			return text;
		}

		public override bool Equals(object obj)
		{
			HotKey hotKey = obj as HotKey;
			return hotKey != null && hotKey.Id.Equals(this.Id) && hotKey.GroupId.Equals(this.GroupId) && hotKey.Keys.SequenceEqual(this.Keys);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private const int DOUBLE_PRESS_TIME = 500;

		private int _doublePressTime;

		public string Id;

		public string GroupId;

		private HotKey.Modifiers _modifiers;

		private HotKey.Modifiers _negativeModifiers;

		[Flags]
		public enum Modifiers
		{
			None = 0,
			Shift = 1,
			Alt = 2,
			Control = 4
		}
	}
}
