using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	// Token: 0x02000007 RID: 7
	public class ArmyCompositionGroupVM : ViewModel
	{
		// Token: 0x0600001E RID: 30 RVA: 0x00004C88 File Offset: 0x00002E88
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

		// Token: 0x0600001F RID: 31 RVA: 0x00004E3C File Offset: 0x0000303C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ArmySizeTitle = GameTexts.FindText("str_army_size", null).ToString();
			this.MeleeInfantryComposition.RefreshValues();
			this.RangedInfantryComposition.RefreshValues();
			this.MeleeCavalryComposition.RefreshValues();
			this.RangedCavalryComposition.RefreshValues();
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00004E94 File Offset: 0x00003094
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

		// Token: 0x06000021 RID: 33 RVA: 0x00004EC4 File Offset: 0x000030C4
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

		// Token: 0x06000022 RID: 34 RVA: 0x00004F14 File Offset: 0x00003114
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

		// Token: 0x06000023 RID: 35 RVA: 0x000051F0 File Offset: 0x000033F0
		private void SetArmyCompositionValue(int index, int value, ArmyCompositionItemVM composition)
		{
			this.CompositionValues[index] = value;
			composition.RefreshCompositionValue();
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00005204 File Offset: 0x00003404
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

		// Token: 0x06000025 RID: 37 RVA: 0x000052C8 File Offset: 0x000034C8
		public void OnPlayerTypeChange(CustomBattlePlayerType playerType)
		{
			bool flag = this.ArmySize == this.MinArmySize;
			this.MinArmySize = ((playerType == CustomBattlePlayerType.Commander) ? 1 : 2);
			this.ArmySize = (flag ? this.MinArmySize : this._armySize);
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00005308 File Offset: 0x00003508
		// (set) Token: 0x06000027 RID: 39 RVA: 0x00005310 File Offset: 0x00003510
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

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000028 RID: 40 RVA: 0x0000532E File Offset: 0x0000352E
		// (set) Token: 0x06000029 RID: 41 RVA: 0x00005336 File Offset: 0x00003536
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

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00005354 File Offset: 0x00003554
		// (set) Token: 0x0600002B RID: 43 RVA: 0x0000535C File Offset: 0x0000355C
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

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600002C RID: 44 RVA: 0x0000537A File Offset: 0x0000357A
		// (set) Token: 0x0600002D RID: 45 RVA: 0x00005382 File Offset: 0x00003582
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

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600002E RID: 46 RVA: 0x000053A0 File Offset: 0x000035A0
		// (set) Token: 0x0600002F RID: 47 RVA: 0x000053A8 File Offset: 0x000035A8
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

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000030 RID: 48 RVA: 0x000053CB File Offset: 0x000035CB
		// (set) Token: 0x06000031 RID: 49 RVA: 0x000053D4 File Offset: 0x000035D4
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

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000032 RID: 50 RVA: 0x0000542C File Offset: 0x0000362C
		// (set) Token: 0x06000033 RID: 51 RVA: 0x00005434 File Offset: 0x00003634
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

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00005452 File Offset: 0x00003652
		// (set) Token: 0x06000035 RID: 53 RVA: 0x0000545A File Offset: 0x0000365A
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

		// Token: 0x0400002C RID: 44
		public int[] CompositionValues;

		// Token: 0x0400002D RID: 45
		private readonly bool _isPlayerSide;

		// Token: 0x0400002E RID: 46
		private bool _updatingSliders;

		// Token: 0x0400002F RID: 47
		private BasicCultureObject _selectedCulture;

		// Token: 0x04000030 RID: 48
		private readonly MBReadOnlyList<SkillObject> _allSkills = Game.Current.ObjectManager.GetObjectTypeList<SkillObject>();

		// Token: 0x04000031 RID: 49
		private readonly List<BasicCharacterObject> _allCharacterObjects = new List<BasicCharacterObject>();

		// Token: 0x04000032 RID: 50
		private ArmyCompositionItemVM _meleeInfantryComposition;

		// Token: 0x04000033 RID: 51
		private ArmyCompositionItemVM _rangedInfantryComposition;

		// Token: 0x04000034 RID: 52
		private ArmyCompositionItemVM _meleeCavalryComposition;

		// Token: 0x04000035 RID: 53
		private ArmyCompositionItemVM _rangedCavalryComposition;

		// Token: 0x04000036 RID: 54
		private int _armySize;

		// Token: 0x04000037 RID: 55
		private int _maxArmySize;

		// Token: 0x04000038 RID: 56
		private int _minArmySize;

		// Token: 0x04000039 RID: 57
		private string _armySizeTitle;
	}
}
