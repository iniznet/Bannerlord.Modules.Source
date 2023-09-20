using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	// Token: 0x0200000E RID: 14
	public class LauncherHintVM : ViewModel
	{
		// Token: 0x0600006A RID: 106 RVA: 0x0000321B File Offset: 0x0000141B
		public LauncherHintVM(string text)
		{
			this.Text = text;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x0000322A File Offset: 0x0000142A
		public void ExecuteBeginHint()
		{
			if (!string.IsNullOrEmpty(this.Text))
			{
				LauncherUI.AddHintInformation(this.Text);
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003244 File Offset: 0x00001444
		public void ExecuteEndHint()
		{
			LauncherUI.HideHintInformation();
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600006D RID: 109 RVA: 0x0000324B File Offset: 0x0000144B
		// (set) Token: 0x0600006E RID: 110 RVA: 0x00003253 File Offset: 0x00001453
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

		// Token: 0x0400003C RID: 60
		private string _text;
	}
}
