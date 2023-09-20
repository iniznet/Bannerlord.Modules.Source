using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond.Cosmetics;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory.CosmeticItem;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory.CosmeticCategory
{
	public abstract class MPArmoryCosmeticCategoryBaseVM : ViewModel
	{
		public MPArmoryCosmeticCategoryBaseVM(CosmeticsManager.CosmeticType cosmeticType)
		{
			this.AvailableCosmetics = new MBBindingList<MPArmoryCosmeticItemBaseVM>();
			this.CosmeticType = cosmeticType;
			this.CosmeticTypeName = cosmeticType.ToString();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.AvailableCosmetics.ApplyActionOnAllItems(delegate(MPArmoryCosmeticItemBaseVM c)
			{
				c.RefreshValues();
			});
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.AvailableCosmetics.ApplyActionOnAllItems(delegate(MPArmoryCosmeticItemBaseVM c)
			{
				c.OnFinalize();
			});
		}

		protected abstract void ExecuteSelectCategory();

		public void Sort(MPArmoryCosmeticsVM.CosmeticItemComparer comparer)
		{
			this.AvailableCosmetics.Sort(comparer);
		}

		[DataSourceProperty]
		public string CosmeticTypeName
		{
			get
			{
				return this._cosmeticTypeName;
			}
			set
			{
				if (value != this._cosmeticTypeName)
				{
					this._cosmeticTypeName = value;
					base.OnPropertyChangedWithValue<string>(value, "CosmeticTypeName");
				}
			}
		}

		[DataSourceProperty]
		public string CosmeticCategoryName
		{
			get
			{
				return this._cosmeticCategoryName;
			}
			set
			{
				if (value != this._cosmeticCategoryName)
				{
					this._cosmeticCategoryName = value;
					base.OnPropertyChangedWithValue<string>(value, "CosmeticCategoryName");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPArmoryCosmeticItemBaseVM> AvailableCosmetics
		{
			get
			{
				return this._availableCosmetics;
			}
			set
			{
				if (value != this._availableCosmetics)
				{
					this._availableCosmetics = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPArmoryCosmeticItemBaseVM>>(value, "AvailableCosmetics");
				}
			}
		}

		public readonly CosmeticsManager.CosmeticType CosmeticType;

		private string _cosmeticTypeName;

		private string _cosmeticCategoryName;

		private bool _isSelected;

		private MBBindingList<MPArmoryCosmeticItemBaseVM> _availableCosmetics;
	}
}
