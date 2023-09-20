using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	public class LauncherHintVM : ViewModel
	{
		public LauncherHintVM(string text)
		{
			this.Text = text;
		}

		public void ExecuteBeginHint()
		{
			if (!string.IsNullOrEmpty(this.Text))
			{
				LauncherUI.AddHintInformation(this.Text);
			}
		}

		public void ExecuteEndHint()
		{
			LauncherUI.HideHintInformation();
		}

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

		private string _text;
	}
}
