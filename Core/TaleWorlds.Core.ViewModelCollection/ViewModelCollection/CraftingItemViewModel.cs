using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection
{
	public class CraftingItemViewModel : ViewModel
	{
		[DataSourceProperty]
		public string UsedPieces
		{
			get
			{
				return this._usedPieces;
			}
			set
			{
				this._usedPieces = value;
				base.OnPropertyChangedWithValue<string>(value, "UsedPieces");
			}
		}

		[DataSourceProperty]
		public int WeaponClass
		{
			get
			{
				return this._weaponClass;
			}
			set
			{
				if (value != this._weaponClass)
				{
					this._weaponClass = value;
					base.OnPropertyChangedWithValue(value, "WeaponClass");
				}
			}
		}

		public WeaponClass GetWeaponClass()
		{
			return (WeaponClass)this.WeaponClass;
		}

		public void SetCraftingData(WeaponClass weaponClass, WeaponDesignElement[] craftingPieces)
		{
			this.WeaponClass = (int)weaponClass;
		}

		public CraftingItemViewModel()
		{
			this.WeaponClass = -1;
		}

		private string _usedPieces;

		private int _weaponClass;
	}
}
