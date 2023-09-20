using System;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.SaveLoad
{
	public class SavedGameGroupVM : ViewModel
	{
		public SavedGameGroupVM()
		{
			this.SavedGamesList = new MBBindingList<SavedGameVM>();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.SavedGamesList.ApplyActionOnAllItems(delegate(SavedGameVM s)
			{
				s.RefreshValues();
			});
		}

		[DataSourceProperty]
		public bool IsFilteredOut
		{
			get
			{
				return this._isFilteredOut;
			}
			set
			{
				if (value != this._isFilteredOut)
				{
					this._isFilteredOut = value;
					base.OnPropertyChangedWithValue(value, "IsFilteredOut");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<SavedGameVM> SavedGamesList
		{
			get
			{
				return this._savedGamesList;
			}
			set
			{
				if (value != this._savedGamesList)
				{
					this._savedGamesList = value;
					base.OnPropertyChangedWithValue<MBBindingList<SavedGameVM>>(value, "SavedGamesList");
				}
			}
		}

		[DataSourceProperty]
		public string IdentifierID
		{
			get
			{
				return this._identifierID;
			}
			set
			{
				if (value != this._identifierID)
				{
					this._identifierID = value;
					base.OnPropertyChangedWithValue<string>(value, "IdentifierID");
				}
			}
		}

		private bool _isFilteredOut;

		private MBBindingList<SavedGameVM> _savedGamesList;

		private string _identifierID;
	}
}
