using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Map
{
	public class MapCheatsVM : ViewModel
	{
		public MapCheatsVM()
		{
			this.Cheats = new MBBindingList<StringItemWithActionVM>();
		}

		public void AddCheat(string name, string cheatCode, bool closeOnExecute)
		{
			this.Cheats.Add(new CheatItemVM(name, cheatCode, closeOnExecute, new Action(this.ExecuteDisable)));
		}

		public void ExecuteEnable()
		{
			this.IsEnabled = true;
		}

		public void ExecuteDisable()
		{
			this.IsEnabled = false;
		}

		[DataSourceProperty]
		public MBBindingList<StringItemWithActionVM> Cheats
		{
			get
			{
				return this._cheats;
			}
			set
			{
				if (value != this._cheats)
				{
					this._cheats = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringItemWithActionVM>>(value, "Cheats");
				}
			}
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

		private bool _isEnabled;

		private MBBindingList<StringItemWithActionVM> _cheats;
	}
}
