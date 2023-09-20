using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.SaveLoad
{
	public class SaveLoadScreenWidget : Widget
	{
		public SaveLoadScreenWidget(UIContext context)
			: base(context)
		{
		}

		public void SetCurrentSaveTuple(SavedGameTupleButtonWidget tuple)
		{
			if (tuple != null)
			{
				this.LoadButton.IsVisible = true;
				this._currentSelectedTuple = tuple;
				return;
			}
			this.LoadButton.IsEnabled = false;
			this._currentSelectedTuple = null;
		}

		[Editor(false)]
		public ButtonWidget LoadButton
		{
			get
			{
				return this._loadButton;
			}
			set
			{
				if (this._loadButton != value)
				{
					this._loadButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "LoadButton");
				}
			}
		}

		[Editor(false)]
		public bool IsSaving
		{
			get
			{
				return this._isSaving;
			}
			set
			{
				if (this._isSaving != value)
				{
					this._isSaving = value;
					base.OnPropertyChanged(value, "IsSaving");
				}
			}
		}

		private SavedGameTupleButtonWidget _currentSelectedTuple;

		private ButtonWidget _loadButton;

		private bool _isSaving;
	}
}
