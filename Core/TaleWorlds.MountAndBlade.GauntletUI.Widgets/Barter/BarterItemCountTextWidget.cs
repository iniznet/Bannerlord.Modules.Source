using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Barter
{
	public class BarterItemCountTextWidget : TextWidget
	{
		public BarterItemCountTextWidget(UIContext context)
			: base(context)
		{
		}

		[Editor(false)]
		public int Count
		{
			get
			{
				return this._count;
			}
			set
			{
				if (this._count != value)
				{
					this._count = value;
					base.OnPropertyChanged(value, "Count");
					base.IntText = value;
					base.IsVisible = value > 1;
				}
			}
		}

		private int _count;
	}
}
