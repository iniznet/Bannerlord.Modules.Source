using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.SaveLoad
{
	public class SavedGameTupleButtonWidget : ButtonWidget
	{
		public SavedGameTupleButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnClick()
		{
			base.OnClick();
			if (this.ScreenWidget != null)
			{
				this.ScreenWidget.SetCurrentSaveTuple(this);
			}
		}

		private void OnSaveDeletion(Widget widget)
		{
			this.ScreenWidget.SetCurrentSaveTuple(null);
		}

		[Editor(false)]
		public SaveLoadScreenWidget ScreenWidget
		{
			get
			{
				return this._screenWidget;
			}
			set
			{
				if (this._screenWidget != value)
				{
					this._screenWidget = value;
					base.OnPropertyChanged<SaveLoadScreenWidget>(value, "ScreenWidget");
				}
			}
		}

		private SaveLoadScreenWidget _screenWidget;
	}
}
