using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.ClassLoadout
{
	public class HeroInformationVM : ViewModel
	{
		public MultiplayerClassDivisions.MPHeroClass HeroClass { get; private set; }

		public HeroInformationVM()
		{
			this._latestSelectedItemGroup = ShallowItemVM.ItemGroup.None;
			this.Item1 = new ShallowItemVM(new Action<ShallowItemVM>(this.UpdateHighlightedItem));
			this.Item2 = new ShallowItemVM(new Action<ShallowItemVM>(this.UpdateHighlightedItem));
			this.Item3 = new ShallowItemVM(new Action<ShallowItemVM>(this.UpdateHighlightedItem));
			this.Item4 = new ShallowItemVM(new Action<ShallowItemVM>(this.UpdateHighlightedItem));
			this.ItemHorse = new ShallowItemVM(new Action<ShallowItemVM>(this.UpdateHighlightedItem));
			this.IsArmyAvailable = MultiplayerOptionsExtensions.GetIntValue(20, 0) > 0;
			this.SetFirstSelectedItem();
			this.RefreshValues();
		}

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
			this.ItemHorse.RefreshWith(10, equipment);
			this.Item1.RefreshWith(0, equipment);
			this.Item2.RefreshWith(1, equipment);
			this.Item3.RefreshWith(2, equipment);
			this.Item4.RefreshWith(3, equipment);
			TextObject heroInformation = heroClass.HeroInformation;
			this.Information = ((heroInformation != null) ? heroInformation.ToString() : null);
			this.NameText = heroClass.HeroName.ToString();
			int num = MultiplayerOptionsExtensions.GetIntValue(20, 0);
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
			this.ArmySize = MPPerkObject.GetTroopCount(heroClass, num, onSpawnPerkHandler);
			this.MovementSpeed = (int)(this.HeroClass.HeroMovementSpeedMultiplier * 100f);
			this.HitPoints = heroClass.Health;
			this.Armor = (int)((onSpawnPerkHandler != null) ? onSpawnPerkHandler.GetDrivenPropertyBonusOnSpawn(true, 51, (float)this.HeroClass.ArmorValue) : 0f) + this.HeroClass.ArmorValue;
			if (!this.TrySetSelectedItemByType(this._latestSelectedItemGroup))
			{
				this.SetFirstSelectedItem();
			}
		}

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

		private const int _defaultNumberOfBotsPerFormation = 25;

		private TextObject _armySizeHintWithDefaultValue = new TextObject("{=aalbxe7z}Army Size", null);

		private ShallowItemVM.ItemGroup _latestSelectedItemGroup;

		private HintViewModel _armySizeHint;

		private HintViewModel _movementSpeedHint;

		private HintViewModel _hitPointsHint;

		private HintViewModel _armorHint;

		private ShallowItemVM _item1;

		private ShallowItemVM _item2;

		private ShallowItemVM _item3;

		private ShallowItemVM _item4;

		private ShallowItemVM _itemHorse;

		private ShallowItemVM _itemSelected;

		private string _information;

		private string _nameText;

		private string _equipmentText;

		private int _movementSpeed;

		private int _hitPoints;

		private int _armySize;

		private int _armor;

		private bool _armyAvailable;
	}
}
