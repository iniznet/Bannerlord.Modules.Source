using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	public class LauncherInformationVM : ViewModel
	{
		public LauncherInformationVM()
		{
			LauncherUI.OnAddHintInformation += this.ExecuteEnableHint;
			LauncherUI.OnHideHintInformation += this.ExecuteDisableHint;
		}

		private void ExecuteEnableHint(string text)
		{
			this.IsEnabled = true;
			this.Text = text;
		}

		private void ExecuteDisableHint()
		{
			this.IsEnabled = false;
		}

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

		private bool _isEnabled;

		private string _text;
	}
}
