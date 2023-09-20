using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items
{
	public class EncyclopediaUnitEquipmentSetSelectorItemVM : SelectorItemVM
	{
		public Equipment EquipmentSet { get; private set; }

		public EncyclopediaUnitEquipmentSetSelectorItemVM(Equipment equipmentSet, string name = "")
			: base(name)
		{
			this.EquipmentSet = equipmentSet;
			this.LeftEquipmentList = new MBBindingList<CharacterEquipmentItemVM>();
			this.RightEquipmentList = new MBBindingList<CharacterEquipmentItemVM>();
			this.RefreshEquipment();
		}

		private void RefreshEquipment()
		{
			this.LeftEquipmentList.Clear();
			this.LeftEquipmentList.Add(new CharacterEquipmentItemVM(this.EquipmentSet[EquipmentIndex.NumAllWeaponSlots].Item));
			this.LeftEquipmentList.Add(new CharacterEquipmentItemVM(this.EquipmentSet[EquipmentIndex.Cape].Item));
			this.LeftEquipmentList.Add(new CharacterEquipmentItemVM(this.EquipmentSet[EquipmentIndex.Body].Item));
			this.LeftEquipmentList.Add(new CharacterEquipmentItemVM(this.EquipmentSet[EquipmentIndex.Gloves].Item));
			this.LeftEquipmentList.Add(new CharacterEquipmentItemVM(this.EquipmentSet[EquipmentIndex.Leg].Item));
			this.LeftEquipmentList.Add(new CharacterEquipmentItemVM(this.EquipmentSet[EquipmentIndex.ArmorItemEndSlot].Item));
			this.RightEquipmentList.Clear();
			this.RightEquipmentList.Add(new CharacterEquipmentItemVM(this.EquipmentSet[EquipmentIndex.WeaponItemBeginSlot].Item));
			this.RightEquipmentList.Add(new CharacterEquipmentItemVM(this.EquipmentSet[EquipmentIndex.Weapon1].Item));
			this.RightEquipmentList.Add(new CharacterEquipmentItemVM(this.EquipmentSet[EquipmentIndex.Weapon2].Item));
			this.RightEquipmentList.Add(new CharacterEquipmentItemVM(this.EquipmentSet[EquipmentIndex.Weapon3].Item));
			this.RightEquipmentList.Add(new CharacterEquipmentItemVM(this.EquipmentSet[EquipmentIndex.ExtraWeaponSlot].Item));
		}

		[DataSourceProperty]
		public MBBindingList<CharacterEquipmentItemVM> LeftEquipmentList
		{
			get
			{
				return this._leftEquipmentList;
			}
			set
			{
				if (value != this._leftEquipmentList)
				{
					this._leftEquipmentList = value;
					base.OnPropertyChangedWithValue<MBBindingList<CharacterEquipmentItemVM>>(value, "LeftEquipmentList");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CharacterEquipmentItemVM> RightEquipmentList
		{
			get
			{
				return this._rightEquipmentList;
			}
			set
			{
				if (value != this._rightEquipmentList)
				{
					this._rightEquipmentList = value;
					base.OnPropertyChangedWithValue<MBBindingList<CharacterEquipmentItemVM>>(value, "RightEquipmentList");
				}
			}
		}

		private MBBindingList<CharacterEquipmentItemVM> _leftEquipmentList;

		private MBBindingList<CharacterEquipmentItemVM> _rightEquipmentList;
	}
}
