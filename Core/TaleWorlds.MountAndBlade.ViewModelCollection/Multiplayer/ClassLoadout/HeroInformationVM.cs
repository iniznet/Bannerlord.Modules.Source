using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout
{
	// Token: 0x020000CA RID: 202
	public class HeroInformationVM : ViewModel
	{
		// Token: 0x17000631 RID: 1585
		// (get) Token: 0x060012DD RID: 4829 RVA: 0x0003DEBB File Offset: 0x0003C0BB
		// (set) Token: 0x060012DE RID: 4830 RVA: 0x0003DEC3 File Offset: 0x0003C0C3
		public MultiplayerClassDivisions.MPHeroClass HeroClass { get; private set; }

		// Token: 0x060012DF RID: 4831 RVA: 0x0003DECC File Offset: 0x0003C0CC
		public HeroInformationVM()
		{
			this._latestSelectedItemGroup = ShallowItemVM.ItemGroup.None;
			this.Item1 = new ShallowItemVM(new Action<ShallowItemVM>(this.UpdateHighlightedItem));
			this.Item2 = new ShallowItemVM(new Action<ShallowItemVM>(this.UpdateHighlightedItem));
			this.Item3 = new ShallowItemVM(new Action<ShallowItemVM>(this.UpdateHighlightedItem));
			this.Item4 = new ShallowItemVM(new Action<ShallowItemVM>(this.UpdateHighlightedItem));
			this.ItemHorse = new ShallowItemVM(new Action<ShallowItemVM>(this.UpdateHighlightedItem));
			this.IsArmyAvailable = MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0;
			this.SetFirstSelectedItem();
			this.RefreshValues();
		}

		// Token: 0x060012E0 RID: 4832 RVA: 0x0003DF88 File Offset: 0x0003C188
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ArmySizeHint = new HintViewModel(GameTexts.FindText("str_army_size", null), null);
			this.MovementSpeedHint = new HintViewModel(GameTexts.FindText("str_movement_speed", null), null);
			this.HitPointsHint = new HintViewModel(GameTexts.FindText("str_hitpoints", null), null);
			this.ArmorHint = new HintViewModel(GameTexts.FindText("str_armor", null), null);
			this.EquipmentText = GameTexts.FindText("str_equipment", null).ToString();
			ShallowItemVM item = this._item1;
			if (item != null)
			{
				item.RefreshValues();
			}
			ShallowItemVM item2 = this._item2;
			if (item2 != null)
			{
				item2.RefreshValues();
			}
			ShallowItemVM item3 = this._item3;
			if (item3 != null)
			{
				item3.RefreshValues();
			}
			ShallowItemVM item4 = this._item4;
			if (item4 != null)
			{
				item4.RefreshValues();
			}
			ShallowItemVM itemHorse = this._itemHorse;
			if (itemHorse != null)
			{
				itemHorse.RefreshValues();
			}
			ShallowItemVM itemSelected = this._itemSelected;
			if (itemSelected != null)
			{
				itemSelected.RefreshValues();
			}
			if (this.HeroClass != null)
			{
				this.NameText = this.HeroClass.HeroName.ToString();
			}
		}

		// Token: 0x060012E1 RID: 4833 RVA: 0x0003E094 File Offset: 0x0003C294
		public void RefreshWith(MultiplayerClassDivisions.MPHeroClass heroClass, List<IReadOnlyPerkObject> perks)
		{
			this.HeroClass = heroClass;
			Equipment equipment = heroClass.HeroCharacter.Equipment.Clone(false);
			MPPerkObject.MPOnSpawnPerkHandler onSpawnPerkHandler = MPPerkObject.GetOnSpawnPerkHandler(perks);
			IEnumerable<ValueTuple<EquipmentIndex, EquipmentElement>> enumerable = ((onSpawnPerkHandler != null) ? onSpawnPerkHandler.GetAlternativeEquipments(true) : null);
			if (enumerable != null)
			{
				foreach (ValueTuple<EquipmentIndex, EquipmentElement> valueTuple in enumerable)
				{
					equipment[valueTuple.Item1] = valueTuple.Item2;
				}
			}
			this.ItemHorse.RefreshWith(EquipmentIndex.ArmorItemEndSlot, equipment);
			this.Item1.RefreshWith(EquipmentIndex.WeaponItemBeginSlot, equipment);
			this.Item2.RefreshWith(EquipmentIndex.Weapon1, equipment);
			this.Item3.RefreshWith(EquipmentIndex.Weapon2, equipment);
			this.Item4.RefreshWith(EquipmentIndex.Weapon3, equipment);
			TextObject heroInformation = heroClass.HeroInformation;
			this.Information = ((heroInformation != null) ? heroInformation.ToString() : null);
			this.NameText = heroClass.HeroName.ToString();
			int num = MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			if (num == 0)
			{
				num = 25;
				this._armySizeHintWithDefaultValue.SetTextVariable("OPTION_VALUE", 25);
				this.ArmySizeHint.HintText = this._armySizeHintWithDefaultValue;
			}
			else
			{
				this.ArmySizeHint.HintText = GameTexts.FindText("str_army_size", null);
			}
			float num2 = (float)MathF.Ceiling((float)num * heroClass.TroopMultiplier);
			if (onSpawnPerkHandler != null)
			{
				num2 *= 1f + onSpawnPerkHandler.GetTroopCountMultiplier();
				num2 += onSpawnPerkHandler.GetExtraTroopCount();
			}
			this.ArmySize = MathF.Max(MathF.Ceiling(num2), 1);
			this.MovementSpeed = (int)(this.HeroClass.HeroMovementSpeedMultiplier * 100f);
			this.HitPoints = heroClass.Health;
			this.Armor = (int)((onSpawnPerkHandler != null) ? onSpawnPerkHandler.GetDrivenPropertyBonusOnSpawn(true, DrivenProperty.ArmorTorso, (float)this.HeroClass.ArmorValue) : 0f) + this.HeroClass.ArmorValue;
			if (!this.TrySetSelectedItemByType(this._latestSelectedItemGroup))
			{
				this.SetFirstSelectedItem();
			}
		}

		// Token: 0x060012E2 RID: 4834 RVA: 0x0003E284 File Offset: 0x0003C484
		private bool TrySetSelectedItemByType(ShallowItemVM.ItemGroup itemGroup)
		{
			if (this.Item1.IsValid && this.Item1.Type == itemGroup)
			{
				this.UpdateHighlightedItem(this.Item1);
				return true;
			}
			if (this.Item2.IsValid && this.Item2.Type == itemGroup)
			{
				this.UpdateHighlightedItem(this.Item2);
				return true;
			}
			if (this.Item3.IsValid && this.Item3.Type == itemGroup)
			{
				this.UpdateHighlightedItem(this.Item3);
				return true;
			}
			if (this.Item4.IsValid && this.Item4.Type == itemGroup)
			{
				this.UpdateHighlightedItem(this.Item4);
				return true;
			}
			if (this.ItemHorse.IsValid && this.ItemHorse.Type == itemGroup)
			{
				this.UpdateHighlightedItem(this.ItemHorse);
				return true;
			}
			return false;
		}

		// Token: 0x060012E3 RID: 4835 RVA: 0x0003E360 File Offset: 0x0003C560
		private void SetFirstSelectedItem()
		{
			ShallowItemVM itemSelected = this.ItemSelected;
			if (itemSelected == null || !itemSelected.IsValid)
			{
				if (this.Item1.IsValid)
				{
					this.UpdateHighlightedItem(this.Item1);
					return;
				}
				if (this.Item2.IsValid)
				{
					this.UpdateHighlightedItem(this.Item2);
					return;
				}
				if (this.Item3.IsValid)
				{
					this.UpdateHighlightedItem(this.Item3);
					return;
				}
				if (this.Item4.IsValid)
				{
					this.UpdateHighlightedItem(this.Item4);
					return;
				}
				if (this.ItemHorse.IsValid)
				{
					this.UpdateHighlightedItem(this.ItemHorse);
				}
			}
		}

		// Token: 0x060012E4 RID: 4836 RVA: 0x0003E408 File Offset: 0x0003C608
		public void UpdateHighlightedItem(ShallowItemVM item)
		{
			this.ItemSelected = item;
			this.Item1.IsSelected = false;
			this.Item2.IsSelected = false;
			this.Item3.IsSelected = false;
			this.Item4.IsSelected = false;
			this.ItemHorse.IsSelected = false;
			item.IsSelected = true;
			this._latestSelectedItemGroup = item.Type;
		}

		// Token: 0x17000632 RID: 1586
		// (get) Token: 0x060012E5 RID: 4837 RVA: 0x0003E46B File Offset: 0x0003C66B
		// (set) Token: 0x060012E6 RID: 4838 RVA: 0x0003E473 File Offset: 0x0003C673
		[DataSourceProperty]
		public HintViewModel ArmySizeHint
		{
			get
			{
				return this._armySizeHint;
			}
			set
			{
				if (value != this._armySizeHint)
				{
					this._armySizeHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ArmySizeHint");
				}
			}
		}

		// Token: 0x17000633 RID: 1587
		// (get) Token: 0x060012E7 RID: 4839 RVA: 0x0003E491 File Offset: 0x0003C691
		// (set) Token: 0x060012E8 RID: 4840 RVA: 0x0003E499 File Offset: 0x0003C699
		[DataSourceProperty]
		public HintViewModel MovementSpeedHint
		{
			get
			{
				return this._movementSpeedHint;
			}
			set
			{
				if (value != this._movementSpeedHint)
				{
					this._movementSpeedHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "MovementSpeedHint");
				}
			}
		}

		// Token: 0x17000634 RID: 1588
		// (get) Token: 0x060012E9 RID: 4841 RVA: 0x0003E4B7 File Offset: 0x0003C6B7
		// (set) Token: 0x060012EA RID: 4842 RVA: 0x0003E4BF File Offset: 0x0003C6BF
		[DataSourceProperty]
		public HintViewModel HitPointsHint
		{
			get
			{
				return this._hitPointsHint;
			}
			set
			{
				if (value != this._hitPointsHint)
				{
					this._hitPointsHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "HitPointsHint");
				}
			}
		}

		// Token: 0x17000635 RID: 1589
		// (get) Token: 0x060012EB RID: 4843 RVA: 0x0003E4DD File Offset: 0x0003C6DD
		// (set) Token: 0x060012EC RID: 4844 RVA: 0x0003E4E5 File Offset: 0x0003C6E5
		[DataSourceProperty]
		public HintViewModel ArmorHint
		{
			get
			{
				return this._armorHint;
			}
			set
			{
				if (value != this._armorHint)
				{
					this._armorHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ArmorHint");
				}
			}
		}

		// Token: 0x17000636 RID: 1590
		// (get) Token: 0x060012ED RID: 4845 RVA: 0x0003E503 File Offset: 0x0003C703
		// (set) Token: 0x060012EE RID: 4846 RVA: 0x0003E50B File Offset: 0x0003C70B
		[DataSourceProperty]
		public ShallowItemVM Item1
		{
			get
			{
				return this._item1;
			}
			set
			{
				if (value != this._item1)
				{
					this._item1 = value;
					base.OnPropertyChangedWithValue<ShallowItemVM>(value, "Item1");
				}
			}
		}

		// Token: 0x17000637 RID: 1591
		// (get) Token: 0x060012EF RID: 4847 RVA: 0x0003E529 File Offset: 0x0003C729
		// (set) Token: 0x060012F0 RID: 4848 RVA: 0x0003E531 File Offset: 0x0003C731
		[DataSourceProperty]
		public ShallowItemVM Item2
		{
			get
			{
				return this._item2;
			}
			set
			{
				if (value != this._item2)
				{
					this._item2 = value;
					base.OnPropertyChangedWithValue<ShallowItemVM>(value, "Item2");
				}
			}
		}

		// Token: 0x17000638 RID: 1592
		// (get) Token: 0x060012F1 RID: 4849 RVA: 0x0003E54F File Offset: 0x0003C74F
		// (set) Token: 0x060012F2 RID: 4850 RVA: 0x0003E557 File Offset: 0x0003C757
		[DataSourceProperty]
		public ShallowItemVM Item3
		{
			get
			{
				return this._item3;
			}
			set
			{
				if (value != this._item3)
				{
					this._item3 = value;
					base.OnPropertyChangedWithValue<ShallowItemVM>(value, "Item3");
				}
			}
		}

		// Token: 0x17000639 RID: 1593
		// (get) Token: 0x060012F3 RID: 4851 RVA: 0x0003E575 File Offset: 0x0003C775
		// (set) Token: 0x060012F4 RID: 4852 RVA: 0x0003E57D File Offset: 0x0003C77D
		[DataSourceProperty]
		public ShallowItemVM Item4
		{
			get
			{
				return this._item4;
			}
			set
			{
				if (value != this._item4)
				{
					this._item4 = value;
					base.OnPropertyChangedWithValue<ShallowItemVM>(value, "Item4");
				}
			}
		}

		// Token: 0x1700063A RID: 1594
		// (get) Token: 0x060012F5 RID: 4853 RVA: 0x0003E59B File Offset: 0x0003C79B
		// (set) Token: 0x060012F6 RID: 4854 RVA: 0x0003E5A3 File Offset: 0x0003C7A3
		[DataSourceProperty]
		public ShallowItemVM ItemHorse
		{
			get
			{
				return this._itemHorse;
			}
			set
			{
				if (value != this._itemHorse)
				{
					this._itemHorse = value;
					base.OnPropertyChangedWithValue<ShallowItemVM>(value, "ItemHorse");
				}
			}
		}

		// Token: 0x1700063B RID: 1595
		// (get) Token: 0x060012F7 RID: 4855 RVA: 0x0003E5C1 File Offset: 0x0003C7C1
		// (set) Token: 0x060012F8 RID: 4856 RVA: 0x0003E5C9 File Offset: 0x0003C7C9
		[DataSourceProperty]
		public ShallowItemVM ItemSelected
		{
			get
			{
				return this._itemSelected;
			}
			set
			{
				if (value != this._itemSelected)
				{
					this._itemSelected = value;
					base.OnPropertyChangedWithValue<ShallowItemVM>(value, "ItemSelected");
				}
			}
		}

		// Token: 0x1700063C RID: 1596
		// (get) Token: 0x060012F9 RID: 4857 RVA: 0x0003E5E7 File Offset: 0x0003C7E7
		// (set) Token: 0x060012FA RID: 4858 RVA: 0x0003E5EF File Offset: 0x0003C7EF
		[DataSourceProperty]
		public string Information
		{
			get
			{
				return this._information;
			}
			set
			{
				if (value != this._information)
				{
					this._information = value;
					base.OnPropertyChangedWithValue<string>(value, "Information");
				}
			}
		}

		// Token: 0x1700063D RID: 1597
		// (get) Token: 0x060012FB RID: 4859 RVA: 0x0003E612 File Offset: 0x0003C812
		// (set) Token: 0x060012FC RID: 4860 RVA: 0x0003E61A File Offset: 0x0003C81A
		[DataSourceProperty]
		public string EquipmentText
		{
			get
			{
				return this._equipmentText;
			}
			set
			{
				if (value != this._equipmentText)
				{
					this._equipmentText = value;
					base.OnPropertyChangedWithValue<string>(value, "EquipmentText");
				}
			}
		}

		// Token: 0x1700063E RID: 1598
		// (get) Token: 0x060012FD RID: 4861 RVA: 0x0003E63D File Offset: 0x0003C83D
		// (set) Token: 0x060012FE RID: 4862 RVA: 0x0003E645 File Offset: 0x0003C845
		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		// Token: 0x1700063F RID: 1599
		// (get) Token: 0x060012FF RID: 4863 RVA: 0x0003E668 File Offset: 0x0003C868
		// (set) Token: 0x06001300 RID: 4864 RVA: 0x0003E670 File Offset: 0x0003C870
		[DataSourceProperty]
		public int MovementSpeed
		{
			get
			{
				return this._movementSpeed;
			}
			set
			{
				if (value != this._movementSpeed)
				{
					this._movementSpeed = value;
					base.OnPropertyChangedWithValue(value, "MovementSpeed");
				}
			}
		}

		// Token: 0x17000640 RID: 1600
		// (get) Token: 0x06001301 RID: 4865 RVA: 0x0003E68E File Offset: 0x0003C88E
		// (set) Token: 0x06001302 RID: 4866 RVA: 0x0003E696 File Offset: 0x0003C896
		[DataSourceProperty]
		public int ArmySize
		{
			get
			{
				return this._armySize;
			}
			set
			{
				if (value != this._armySize)
				{
					this._armySize = value;
					base.OnPropertyChangedWithValue(value, "ArmySize");
				}
			}
		}

		// Token: 0x17000641 RID: 1601
		// (get) Token: 0x06001303 RID: 4867 RVA: 0x0003E6B4 File Offset: 0x0003C8B4
		// (set) Token: 0x06001304 RID: 4868 RVA: 0x0003E6BC File Offset: 0x0003C8BC
		[DataSourceProperty]
		public int HitPoints
		{
			get
			{
				return this._hitPoints;
			}
			set
			{
				if (value != this._hitPoints)
				{
					this._hitPoints = value;
					base.OnPropertyChangedWithValue(value, "HitPoints");
				}
			}
		}

		// Token: 0x17000642 RID: 1602
		// (get) Token: 0x06001305 RID: 4869 RVA: 0x0003E6DA File Offset: 0x0003C8DA
		// (set) Token: 0x06001306 RID: 4870 RVA: 0x0003E6E2 File Offset: 0x0003C8E2
		[DataSourceProperty]
		public int Armor
		{
			get
			{
				return this._armor;
			}
			set
			{
				if (value != this._armor)
				{
					this._armor = value;
					base.OnPropertyChangedWithValue(value, "Armor");
				}
			}
		}

		// Token: 0x17000643 RID: 1603
		// (get) Token: 0x06001307 RID: 4871 RVA: 0x0003E700 File Offset: 0x0003C900
		// (set) Token: 0x06001308 RID: 4872 RVA: 0x0003E708 File Offset: 0x0003C908
		[DataSourceProperty]
		public bool IsArmyAvailable
		{
			get
			{
				return this._armyAvailable;
			}
			set
			{
				if (value != this._armyAvailable)
				{
					this._armyAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsArmyAvailable");
				}
			}
		}

		// Token: 0x0400090E RID: 2318
		private const int _defaultNumberOfBotsPerFormation = 25;

		// Token: 0x0400090F RID: 2319
		private TextObject _armySizeHintWithDefaultValue = new TextObject("{=aalbxe7z}Army Size", null);

		// Token: 0x04000911 RID: 2321
		private ShallowItemVM.ItemGroup _latestSelectedItemGroup;

		// Token: 0x04000912 RID: 2322
		private HintViewModel _armySizeHint;

		// Token: 0x04000913 RID: 2323
		private HintViewModel _movementSpeedHint;

		// Token: 0x04000914 RID: 2324
		private HintViewModel _hitPointsHint;

		// Token: 0x04000915 RID: 2325
		private HintViewModel _armorHint;

		// Token: 0x04000916 RID: 2326
		private ShallowItemVM _item1;

		// Token: 0x04000917 RID: 2327
		private ShallowItemVM _item2;

		// Token: 0x04000918 RID: 2328
		private ShallowItemVM _item3;

		// Token: 0x04000919 RID: 2329
		private ShallowItemVM _item4;

		// Token: 0x0400091A RID: 2330
		private ShallowItemVM _itemHorse;

		// Token: 0x0400091B RID: 2331
		private ShallowItemVM _itemSelected;

		// Token: 0x0400091C RID: 2332
		private string _information;

		// Token: 0x0400091D RID: 2333
		private string _nameText;

		// Token: 0x0400091E RID: 2334
		private string _equipmentText;

		// Token: 0x0400091F RID: 2335
		private int _movementSpeed;

		// Token: 0x04000920 RID: 2336
		private int _hitPoints;

		// Token: 0x04000921 RID: 2337
		private int _armySize;

		// Token: 0x04000922 RID: 2338
		private int _armor;

		// Token: 0x04000923 RID: 2339
		private bool _armyAvailable;
	}
}
