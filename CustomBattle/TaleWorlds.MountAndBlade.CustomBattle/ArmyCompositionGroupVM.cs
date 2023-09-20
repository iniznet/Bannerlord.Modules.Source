using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	public class ArmyCompositionGroupVM : ViewModel
	{
		public ArmyCompositionGroupVM(bool isPlayerSide, TroopTypeSelectionPopUpVM troopTypeSelectionPopUp)
		{
			this._isPlayerSide = isPlayerSide;
			this.MinArmySize = 1;
			this.MaxArmySize = BannerlordConfig.MaxBattleSize;
			foreach (BasicCharacterObject basicCharacterObject in from c in Game.Current.ObjectManager.GetObjectTypeList<BasicCharacterObject>()
				where c.IsSoldier && !c.IsObsolete
				select c)
			{
				this._allCharacterObjects.Add(basicCharacterObject);
			}
			this.CompositionValues = new int[4];
			this.CompositionValues[0] = 25;
			this.CompositionValues[1] = 25;
			this.CompositionValues[2] = 25;
			this.CompositionValues[3] = 25;
			this.MeleeInfantryComposition = new ArmyCompositionItemVM(ArmyCompositionItemVM.CompositionType.MeleeInfantry, this._allCharacterObjects, this._allSkills, new Action<int, int>(this.UpdateSliders), troopTypeSelectionPopUp, this.CompositionValues);
			this.RangedInfantryComposition = new ArmyCompositionItemVM(ArmyCompositionItemVM.CompositionType.RangedInfantry, this._allCharacterObjects, this._allSkills, new Action<int, int>(this.UpdateSliders), troopTypeSelectionPopUp, this.CompositionValues);
			this.MeleeCavalryComposition = new ArmyCompositionItemVM(ArmyCompositionItemVM.CompositionType.MeleeCavalry, this._allCharacterObjects, this._allSkills, new Action<int, int>(this.UpdateSliders), troopTypeSelectionPopUp, this.CompositionValues);
			this.RangedCavalryComposition = new ArmyCompositionItemVM(ArmyCompositionItemVM.CompositionType.RangedCavalry, this._allCharacterObjects, this._allSkills, new Action<int, int>(this.UpdateSliders), troopTypeSelectionPopUp, this.CompositionValues);
			this.ArmySize = BannerlordConfig.GetRealBattleSize() / 5;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ArmySizeTitle = GameTexts.FindText("str_army_size", null).ToString();
			this.MeleeInfantryComposition.RefreshValues();
			this.RangedInfantryComposition.RefreshValues();
			this.MeleeCavalryComposition.RefreshValues();
			this.RangedCavalryComposition.RefreshValues();
		}

		private static int SumOfValues(int[] array, bool[] enabledArray, int excludedIndex = -1)
		{
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (enabledArray[i] && excludedIndex != i)
				{
					num += array[i];
				}
			}
			return num;
		}

		public void SetCurrentSelectedCulture(BasicCultureObject selectedCulture)
		{
			if (this._selectedCulture != selectedCulture)
			{
				this.MeleeInfantryComposition.SetCurrentSelectedCulture(selectedCulture);
				this.RangedInfantryComposition.SetCurrentSelectedCulture(selectedCulture);
				this.MeleeCavalryComposition.SetCurrentSelectedCulture(selectedCulture);
				this.RangedCavalryComposition.SetCurrentSelectedCulture(selectedCulture);
				this._selectedCulture = selectedCulture;
			}
		}

		private void UpdateSliders(int value, int changedSliderIndex)
		{
			if (this._updatingSliders)
			{
				return;
			}
			this._updatingSliders = true;
			bool[] array = new bool[]
			{
				!this.MeleeInfantryComposition.IsLocked,
				!this.RangedInfantryComposition.IsLocked,
				!this.MeleeCavalryComposition.IsLocked,
				!this.RangedCavalryComposition.IsLocked
			};
			int[] array2 = new int[]
			{
				this.CompositionValues[0],
				this.CompositionValues[1],
				this.CompositionValues[2],
				this.CompositionValues[3]
			};
			int[] array3 = new int[]
			{
				this.CompositionValues[0],
				this.CompositionValues[1],
				this.CompositionValues[2],
				this.CompositionValues[3]
			};
			int num = array.Count((bool s) => s);
			if (array[changedSliderIndex])
			{
				num--;
			}
			if (num > 0)
			{
				int num2 = ArmyCompositionGroupVM.SumOfValues(array2, array, -1);
				if (value >= num2)
				{
					value = num2;
				}
				int num3 = value - array2[changedSliderIndex];
				if (num3 != 0)
				{
					int num4 = ArmyCompositionGroupVM.SumOfValues(array2, array, changedSliderIndex);
					int num5 = num4 - num3;
					if (num5 > 0)
					{
						int num6 = 0;
						array3[changedSliderIndex] = value;
						for (int i = 0; i < array.Length; i++)
						{
							if (changedSliderIndex != i && array[i] && array2[i] != 0)
							{
								int num7 = MathF.Round((float)array2[i] / (float)num4 * (float)num5);
								num6 += num7;
								array3[i] = num7;
							}
						}
						int num8 = num5 - num6;
						if (num8 != 0)
						{
							int num9 = 0;
							for (int j = 0; j < array.Length; j++)
							{
								if (array[j] && j != changedSliderIndex && 0 < array2[j] + num8 && 100 > array2[j] + num8)
								{
									num9++;
								}
							}
							for (int k = 0; k < array.Length; k++)
							{
								if (array[k] && k != changedSliderIndex && 0 < array2[k] + num8 && 100 > array2[k] + num8)
								{
									int num10 = MathF.Round((float)num8 / (float)num9);
									array3[k] += num10;
									num8 -= num10;
								}
							}
							if (num8 != 0)
							{
								for (int l = 0; l < array.Length; l++)
								{
									if (array[l] && l != changedSliderIndex && 0 <= array2[l] + num8 && 100 >= array2[l] + num8)
									{
										array3[l] += num8;
										break;
									}
								}
							}
						}
					}
					else
					{
						array3[changedSliderIndex] = value;
						for (int m = 0; m < array.Length; m++)
						{
							if (changedSliderIndex != m && array[m])
							{
								array3[m] = 0;
							}
						}
					}
				}
			}
			this.SetArmyCompositionValue(0, array3[0], this.MeleeInfantryComposition);
			this.SetArmyCompositionValue(1, array3[1], this.RangedInfantryComposition);
			this.SetArmyCompositionValue(2, array3[2], this.MeleeCavalryComposition);
			this.SetArmyCompositionValue(3, array3[3], this.RangedCavalryComposition);
			this._updatingSliders = false;
		}

		private void SetArmyCompositionValue(int index, int value, ArmyCompositionItemVM composition)
		{
			this.CompositionValues[index] = value;
			composition.RefreshCompositionValue();
		}

		public void ExecuteRandomize()
		{
			this.ArmySize = MBRandom.RandomInt(this.MaxArmySize);
			int num = MBRandom.RandomInt(100);
			int num2 = MBRandom.RandomInt(100);
			int num3 = MBRandom.RandomInt(100);
			int num4 = MBRandom.RandomInt(100);
			int num5 = num + num2 + num3 + num4;
			int num6 = MathF.Round(100f * ((float)num / (float)num5));
			int num7 = MathF.Round(100f * ((float)num2 / (float)num5));
			int num8 = MathF.Round(100f * ((float)num3 / (float)num5));
			int num9 = 100 - (num6 + num7 + num8);
			this.MeleeInfantryComposition.ExecuteRandomize(num6);
			this.RangedInfantryComposition.ExecuteRandomize(num7);
			this.MeleeCavalryComposition.ExecuteRandomize(num8);
			this.RangedCavalryComposition.ExecuteRandomize(num9);
		}

		public void OnPlayerTypeChange(CustomBattlePlayerType playerType)
		{
			bool flag = this.ArmySize == this.MinArmySize;
			this.MinArmySize = ((playerType == CustomBattlePlayerType.Commander) ? 1 : 2);
			this.ArmySize = (flag ? this.MinArmySize : this._armySize);
		}

		[DataSourceProperty]
		public ArmyCompositionItemVM MeleeInfantryComposition
		{
			get
			{
				return this._meleeInfantryComposition;
			}
			set
			{
				if (value != this._meleeInfantryComposition)
				{
					this._meleeInfantryComposition = value;
					base.OnPropertyChangedWithValue<ArmyCompositionItemVM>(value, "MeleeInfantryComposition");
				}
			}
		}

		[DataSourceProperty]
		public ArmyCompositionItemVM RangedInfantryComposition
		{
			get
			{
				return this._rangedInfantryComposition;
			}
			set
			{
				if (value != this._rangedInfantryComposition)
				{
					this._rangedInfantryComposition = value;
					base.OnPropertyChangedWithValue<ArmyCompositionItemVM>(value, "RangedInfantryComposition");
				}
			}
		}

		[DataSourceProperty]
		public ArmyCompositionItemVM MeleeCavalryComposition
		{
			get
			{
				return this._meleeCavalryComposition;
			}
			set
			{
				if (value != this._meleeCavalryComposition)
				{
					this._meleeCavalryComposition = value;
					base.OnPropertyChangedWithValue<ArmyCompositionItemVM>(value, "MeleeCavalryComposition");
				}
			}
		}

		[DataSourceProperty]
		public ArmyCompositionItemVM RangedCavalryComposition
		{
			get
			{
				return this._rangedCavalryComposition;
			}
			set
			{
				if (value != this._rangedCavalryComposition)
				{
					this._rangedCavalryComposition = value;
					base.OnPropertyChangedWithValue<ArmyCompositionItemVM>(value, "RangedCavalryComposition");
				}
			}
		}

		[DataSourceProperty]
		public string ArmySizeTitle
		{
			get
			{
				return this._armySizeTitle;
			}
			set
			{
				if (value != this._armySizeTitle)
				{
					this._armySizeTitle = value;
					base.OnPropertyChangedWithValue<string>(value, "ArmySizeTitle");
				}
			}
		}

		public int ArmySize
		{
			get
			{
				return this._armySize;
			}
			set
			{
				if (this._armySize != (int)MathF.Clamp((float)value, (float)this.MinArmySize, (float)this.MaxArmySize))
				{
					this._armySize = (int)MathF.Clamp((float)value, (float)this.MinArmySize, (float)this.MaxArmySize);
					base.OnPropertyChangedWithValue(this._armySize, "ArmySize");
				}
			}
		}

		public int MaxArmySize
		{
			get
			{
				return this._maxArmySize;
			}
			set
			{
				if (this._maxArmySize != value)
				{
					this._maxArmySize = value;
					base.OnPropertyChangedWithValue(value, "MaxArmySize");
				}
			}
		}

		public int MinArmySize
		{
			get
			{
				return this._minArmySize;
			}
			set
			{
				if (this._minArmySize != value)
				{
					this._minArmySize = value;
					base.OnPropertyChangedWithValue(value, "MinArmySize");
				}
			}
		}

		public int[] CompositionValues;

		private readonly bool _isPlayerSide;

		private bool _updatingSliders;

		private BasicCultureObject _selectedCulture;

		private readonly MBReadOnlyList<SkillObject> _allSkills = Game.Current.ObjectManager.GetObjectTypeList<SkillObject>();

		private readonly List<BasicCharacterObject> _allCharacterObjects = new List<BasicCharacterObject>();

		private ArmyCompositionItemVM _meleeInfantryComposition;

		private ArmyCompositionItemVM _rangedInfantryComposition;

		private ArmyCompositionItemVM _meleeCavalryComposition;

		private ArmyCompositionItemVM _rangedCavalryComposition;

		private int _armySize;

		private int _maxArmySize;

		private int _minArmySize;

		private string _armySizeTitle;
	}
}
