using System;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Map.Cheat
{
	public abstract class CheatItemBaseVM : ViewModel
	{
		public CheatItemBaseVM()
		{
		}

		public abstract void ExecuteAction();

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		private string _name;
	}
}
