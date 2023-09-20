using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	public class GamepadCursorViewModel : ViewModel
	{
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

		private float _cursorPositionX = 960f;

		private float _cursorPositionY = 540f;

		private bool _isConsoleMouseVisible;

		private bool _isGamepadCursorVisible;
	}
}
