using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	// Token: 0x02000009 RID: 9
	public class GamepadCursorViewModel : ViewModel
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002FB0 File Offset: 0x000011B0
		// (set) Token: 0x06000030 RID: 48 RVA: 0x00002FB8 File Offset: 0x000011B8
		[DataSourceProperty]
		public bool IsConsoleMouseVisible
		{
			get
			{
				return this._isConsoleMouseVisible;
			}
			set
			{
				if (this._isConsoleMouseVisible != value)
				{
					this._isConsoleMouseVisible = value;
					base.OnPropertyChangedWithValue(value, "IsConsoleMouseVisible");
				}
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002FD6 File Offset: 0x000011D6
		// (set) Token: 0x06000032 RID: 50 RVA: 0x00002FDE File Offset: 0x000011DE
		[DataSourceProperty]
		public bool IsGamepadCursorVisible
		{
			get
			{
				return this._isGamepadCursorVisible;
			}
			set
			{
				if (this._isGamepadCursorVisible != value)
				{
					this._isGamepadCursorVisible = value;
					base.OnPropertyChangedWithValue(value, "IsGamepadCursorVisible");
				}
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00002FFC File Offset: 0x000011FC
		// (set) Token: 0x06000034 RID: 52 RVA: 0x00003004 File Offset: 0x00001204
		[DataSourceProperty]
		public float CursorPositionX
		{
			get
			{
				return this._cursorPositionX;
			}
			set
			{
				if (this._cursorPositionX != value)
				{
					this._cursorPositionX = value;
					base.OnPropertyChangedWithValue(value, "CursorPositionX");
				}
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00003022 File Offset: 0x00001222
		// (set) Token: 0x06000036 RID: 54 RVA: 0x0000302A File Offset: 0x0000122A
		[DataSourceProperty]
		public float CursorPositionY
		{
			get
			{
				return this._cursorPositionY;
			}
			set
			{
				if (this._cursorPositionY != value)
				{
					this._cursorPositionY = value;
					base.OnPropertyChangedWithValue(value, "CursorPositionY");
				}
			}
		}

		// Token: 0x0400001C RID: 28
		private float _cursorPositionX = 960f;

		// Token: 0x0400001D RID: 29
		private float _cursorPositionY = 540f;

		// Token: 0x0400001E RID: 30
		private bool _isConsoleMouseVisible;

		// Token: 0x0400001F RID: 31
		private bool _isGamepadCursorVisible;
	}
}
