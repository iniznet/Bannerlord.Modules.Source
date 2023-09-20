using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.InputSystem
{
	// Token: 0x02000006 RID: 6
	public class HotKey
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000063 RID: 99 RVA: 0x00002C80 File Offset: 0x00000E80
		private bool _isDoublePressActive
		{
			get
			{
				int num = Environment.TickCount - this._doublePressTime;
				return num < 500 && num >= 0;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00002CAB File Offset: 0x00000EAB
		// (set) Token: 0x06000065 RID: 101 RVA: 0x00002CB3 File Offset: 0x00000EB3
		public List<Key> Keys { get; internal set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00002CBC File Offset: 0x00000EBC
		// (set) Token: 0x06000067 RID: 103 RVA: 0x00002CC4 File Offset: 0x00000EC4
		public List<Key> DefaultKeys { get; private set; }

		// Token: 0x06000068 RID: 104 RVA: 0x00002CD0 File Offset: 0x00000ED0
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

		// Token: 0x06000069 RID: 105 RVA: 0x00002D4C File Offset: 0x00000F4C
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

		// Token: 0x0600006A RID: 106 RVA: 0x00002DAB File Offset: 0x00000FAB
		private bool IsKeyAllowed(Key key, bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			return (isKeysAllowed || !key.IsKeyboardInput) && (isMouseButtonAllowed || !key.IsMouseButtonInput) && (isMouseWheelAllowed || !key.IsMouseWheelInput) && (isControllerAllowed || !key.IsControllerInput);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00002DE0 File Offset: 0x00000FE0
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

		// Token: 0x0600006C RID: 108 RVA: 0x00002EAB File Offset: 0x000010AB
		private bool IsDown(Key key, bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			return this.IsKeyAllowed(key, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed) && (this._modifiers == HotKey.Modifiers.None || this.CheckModifiers()) && key.IsDown();
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00002ED8 File Offset: 0x000010D8
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

		// Token: 0x0600006E RID: 110 RVA: 0x00002F3C File Offset: 0x0000113C
		private bool IsDownImmediate(Key key, bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			return this.IsKeyAllowed(key, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed) && (this._modifiers == HotKey.Modifiers.None || this.CheckModifiers()) && key.IsDownImmediate();
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00002F68 File Offset: 0x00001168
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

		// Token: 0x06000070 RID: 112 RVA: 0x00002FCC File Offset: 0x000011CC
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

		// Token: 0x06000071 RID: 113 RVA: 0x00003024 File Offset: 0x00001224
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

		// Token: 0x06000072 RID: 114 RVA: 0x00003088 File Offset: 0x00001288
		private bool IsPressed(Key key, bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			return this.IsKeyAllowed(key, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed) && (this._modifiers == HotKey.Modifiers.None || this.CheckModifiers()) && key.IsPressed();
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000030B4 File Offset: 0x000012B4
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

		// Token: 0x06000074 RID: 116 RVA: 0x00003118 File Offset: 0x00001318
		private bool IsReleased(Key key, bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			return this.IsKeyAllowed(key, isKeysAllowed, isMouseButtonAllowed, isMouseWheelAllowed, isControllerAllowed) && (this._modifiers == HotKey.Modifiers.None || this.CheckModifiers()) && key.IsReleased();
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00003144 File Offset: 0x00001344
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

		// Token: 0x06000076 RID: 118 RVA: 0x000031A8 File Offset: 0x000013A8
		public bool HasModifier(HotKey.Modifiers modifier)
		{
			return this._modifiers.HasAnyFlag(modifier);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000031B6 File Offset: 0x000013B6
		public bool HasSameModifiers(HotKey other)
		{
			return this._modifiers == other._modifiers;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000031C8 File Offset: 0x000013C8
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

		// Token: 0x06000079 RID: 121 RVA: 0x00003244 File Offset: 0x00001444
		public override bool Equals(object obj)
		{
			HotKey hotKey = obj as HotKey;
			return hotKey != null && hotKey.Id.Equals(this.Id) && hotKey.GroupId.Equals(this.GroupId) && hotKey.Keys.SequenceEqual(this.Keys);
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00003294 File Offset: 0x00001494
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x04000016 RID: 22
		private const int DOUBLE_PRESS_TIME = 500;

		// Token: 0x04000017 RID: 23
		private int _doublePressTime;

		// Token: 0x04000018 RID: 24
		public string Id;

		// Token: 0x04000019 RID: 25
		public string GroupId;

		// Token: 0x0400001C RID: 28
		private HotKey.Modifiers _modifiers;

		// Token: 0x0400001D RID: 29
		private HotKey.Modifiers _negativeModifiers;

		// Token: 0x02000013 RID: 19
		[Flags]
		public enum Modifiers
		{
			// Token: 0x04000156 RID: 342
			None = 0,
			// Token: 0x04000157 RID: 343
			Shift = 1,
			// Token: 0x04000158 RID: 344
			Alt = 2,
			// Token: 0x04000159 RID: 345
			Control = 4
		}
	}
}
