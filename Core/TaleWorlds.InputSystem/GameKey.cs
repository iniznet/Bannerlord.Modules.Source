using System;

namespace TaleWorlds.InputSystem
{
	// Token: 0x02000003 RID: 3
	public class GameKey
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000022 RID: 34 RVA: 0x00002137 File Offset: 0x00000337
		// (set) Token: 0x06000023 RID: 35 RVA: 0x0000213F File Offset: 0x0000033F
		public int Id { get; private set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00002148 File Offset: 0x00000348
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00002150 File Offset: 0x00000350
		public string StringId { get; private set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00002159 File Offset: 0x00000359
		// (set) Token: 0x06000027 RID: 39 RVA: 0x00002161 File Offset: 0x00000361
		public string GroupId { get; private set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000028 RID: 40 RVA: 0x0000216A File Offset: 0x0000036A
		// (set) Token: 0x06000029 RID: 41 RVA: 0x00002172 File Offset: 0x00000372
		public string MainCategoryId { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600002A RID: 42 RVA: 0x0000217B File Offset: 0x0000037B
		// (set) Token: 0x0600002B RID: 43 RVA: 0x00002183 File Offset: 0x00000383
		public Key KeyboardKey { get; internal set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600002C RID: 44 RVA: 0x0000218C File Offset: 0x0000038C
		// (set) Token: 0x0600002D RID: 45 RVA: 0x00002194 File Offset: 0x00000394
		public Key DefaultKeyboardKey { get; private set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600002E RID: 46 RVA: 0x0000219D File Offset: 0x0000039D
		// (set) Token: 0x0600002F RID: 47 RVA: 0x000021A5 File Offset: 0x000003A5
		public Key ControllerKey { get; internal set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000030 RID: 48 RVA: 0x000021AE File Offset: 0x000003AE
		// (set) Token: 0x06000031 RID: 49 RVA: 0x000021B6 File Offset: 0x000003B6
		public Key DefaultControllerKey { get; internal set; }

		// Token: 0x06000032 RID: 50 RVA: 0x000021C0 File Offset: 0x000003C0
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

		// Token: 0x06000033 RID: 51 RVA: 0x00002244 File Offset: 0x00000444
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

		// Token: 0x06000034 RID: 52 RVA: 0x000022B6 File Offset: 0x000004B6
		private bool IsKeyAllowed(Key key, bool isKeysAllowed, bool isMouseButtonAllowed, bool isMouseWheelAllowed, bool isControllerAllowed)
		{
			return (isKeysAllowed || !key.IsKeyboardInput) && (isMouseButtonAllowed || !key.IsMouseButtonInput) && (isMouseWheelAllowed || !key.IsMouseWheelInput) && (isControllerAllowed || !key.IsControllerInput);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000022EC File Offset: 0x000004EC
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

		// Token: 0x06000036 RID: 54 RVA: 0x00002368 File Offset: 0x00000568
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

		// Token: 0x06000037 RID: 55 RVA: 0x000023E4 File Offset: 0x000005E4
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

		// Token: 0x06000038 RID: 56 RVA: 0x0000245C File Offset: 0x0000065C
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

		// Token: 0x06000039 RID: 57 RVA: 0x000024D4 File Offset: 0x000006D4
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

		// Token: 0x0600003A RID: 58 RVA: 0x0000254C File Offset: 0x0000074C
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

		// Token: 0x0600003B RID: 59 RVA: 0x000025D4 File Offset: 0x000007D4
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

		// Token: 0x0600003C RID: 60 RVA: 0x00002638 File Offset: 0x00000838
		public override bool Equals(object obj)
		{
			GameKey gameKey = obj as GameKey;
			return gameKey != null && gameKey.Id.Equals(this.Id) && gameKey.GroupId.Equals(this.GroupId) && gameKey.KeyboardKey == this.KeyboardKey && gameKey.ControllerKey == this.ControllerKey;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x0000269E File Offset: 0x0000089E
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
