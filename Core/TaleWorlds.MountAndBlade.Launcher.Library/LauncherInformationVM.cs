using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	// Token: 0x0200000D RID: 13
	public class LauncherInformationVM : ViewModel
	{
		// Token: 0x06000063 RID: 99 RVA: 0x00003187 File Offset: 0x00001387
		public LauncherInformationVM()
		{
			LauncherUI.OnAddHintInformation += this.ExecuteEnableHint;
			LauncherUI.OnHideHintInformation += this.ExecuteDisableHint;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x000031B1 File Offset: 0x000013B1
		private void ExecuteEnableHint(string text)
		{
			this.IsEnabled = true;
			this.Text = text;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000031C1 File Offset: 0x000013C1
		private void ExecuteDisableHint()
		{
			this.IsEnabled = false;
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000066 RID: 102 RVA: 0x000031CA File Offset: 0x000013CA
		// (set) Token: 0x06000067 RID: 103 RVA: 0x000031D2 File Offset: 0x000013D2
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000068 RID: 104 RVA: 0x000031F0 File Offset: 0x000013F0
		// (set) Token: 0x06000069 RID: 105 RVA: 0x000031F8 File Offset: 0x000013F8
		[DataSourceProperty]
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (value != this._text)
				{
					this._text = value;
					base.OnPropertyChangedWithValue<string>(value, "Text");
				}
			}
		}

		// Token: 0x0400003A RID: 58
		private bool _isEnabled;

		// Token: 0x0400003B RID: 59
		private string _text;
	}
}
