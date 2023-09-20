using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class WarningTextWidget : TextWidget
	{
		public WarningTextWidget(UIContext context)
			: base(context)
		{
		}

		[Editor(false)]
		public bool IsWarned
		{
			get
			{
				return this._isWarned;
			}
			set
			{
				if (this._isWarned != value)
				{
					this._isWarned = value;
					base.OnPropertyChanged(value, "IsWarned");
					this.SetState(this._isWarned ? "Warned" : "Default");
				}
			}
		}

		private bool _isWarned;
	}
}
