using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	public class StringBasedVisibilityWidget : Widget
	{
		public StringBasedVisibilityWidget.WatchTypes WatchType { get; set; }

		public StringBasedVisibilityWidget(UIContext context)
			: base(context)
		{
		}

		[Editor(false)]
		public string FirstString
		{
			get
			{
				return this._firstString;
			}
			set
			{
				if (this._firstString != value)
				{
					this._firstString = value;
					base.OnPropertyChanged<string>(value, "FirstString");
					StringBasedVisibilityWidget.WatchTypes watchType = this.WatchType;
					if (watchType == StringBasedVisibilityWidget.WatchTypes.Equal)
					{
						base.IsVisible = string.Equals(value, this.SecondString, StringComparison.OrdinalIgnoreCase);
						return;
					}
					if (watchType != StringBasedVisibilityWidget.WatchTypes.NotEqual)
					{
						return;
					}
					base.IsVisible = !string.Equals(value, this.SecondString, StringComparison.OrdinalIgnoreCase);
				}
			}
		}

		[Editor(false)]
		public string SecondString
		{
			get
			{
				return this._secondString;
			}
			set
			{
				if (this._secondString != value)
				{
					this._secondString = value;
					base.OnPropertyChanged<string>(value, "SecondString");
					StringBasedVisibilityWidget.WatchTypes watchType = this.WatchType;
					if (watchType == StringBasedVisibilityWidget.WatchTypes.Equal)
					{
						base.IsVisible = string.Equals(value, this.FirstString, StringComparison.OrdinalIgnoreCase);
						return;
					}
					if (watchType != StringBasedVisibilityWidget.WatchTypes.NotEqual)
					{
						return;
					}
					base.IsVisible = !string.Equals(value, this.FirstString, StringComparison.OrdinalIgnoreCase);
				}
			}
		}

		private string _firstString;

		private string _secondString;

		public enum WatchTypes
		{
			Equal,
			NotEqual
		}
	}
}
