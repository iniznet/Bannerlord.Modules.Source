using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection
{
	// Token: 0x02000009 RID: 9
	public class CharacterViewModel : ViewModel
	{
		// Token: 0x06000032 RID: 50 RVA: 0x0000237A File Offset: 0x0000057A
		public CharacterViewModel()
		{
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002398 File Offset: 0x00000598
		public CharacterViewModel(CharacterViewModel.StanceTypes stance = CharacterViewModel.StanceTypes.None)
		{
			this._equipment = new Equipment(false);
			this.EquipmentCode = this._equipment.CalculateEquipmentCode();
			this.StanceIndex = (int)stance;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000023E8 File Offset: 0x000005E8
		public void SetEquipment(EquipmentIndex index, EquipmentElement item)
		{
			this._equipment[(int)index] = item;
			this.EquipmentCode = this._equipment.CalculateEquipmentCode();
			Equipment equipment = this._equipment;
			this.HasMount = ((equipment != null) ? equipment[10].Item : null) != null;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002438 File Offset: 0x00000638
		public virtual void SetEquipment(Equipment equipment)
		{
			this._equipment = ((equipment != null) ? equipment.Clone(false) : null);
			Equipment equipment2 = this._equipment;
			this.HasMount = ((equipment2 != null) ? equipment2[10].Item : null) != null;
			Equipment equipment3 = this._equipment;
			this.EquipmentCode = ((equipment3 != null) ? equipment3.CalculateEquipmentCode() : null);
			if (!string.IsNullOrEmpty(this.CharStringId))
			{
				this.MountCreationKey = TaleWorlds.Core.MountCreationKey.GetRandomMountKeyString((equipment != null) ? equipment[10].Item : null, Common.GetDJB2(this.CharStringId));
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000024D0 File Offset: 0x000006D0
		public void FillFrom(BasicCharacterObject character, int seed = -1)
		{
			if (FaceGen.GetMaturityTypeWithAge(character.Age) > BodyMeshMaturityType.Child)
			{
				if (character.Culture != null)
				{
					this.ArmorColor1 = character.Culture.Color;
					this.ArmorColor2 = character.Culture.Color2;
				}
				this.CharStringId = character.StringId;
				this.IsFemale = character.IsFemale;
				this.Race = character.Race;
				this.BodyProperties = character.GetBodyProperties(character.Equipment, seed).ToString();
				Equipment equipment = character.Equipment;
				this.MountCreationKey = TaleWorlds.Core.MountCreationKey.GetRandomMountKeyString((equipment != null) ? equipment[10].Item : null, Common.GetDJB2(character.StringId));
				Equipment equipment2 = character.Equipment;
				this._equipment = ((equipment2 != null) ? equipment2.Clone(false) : null);
				Equipment equipment3 = this._equipment;
				this.HasMount = ((equipment3 != null) ? equipment3[10].Item : null) != null;
				Equipment equipment4 = this._equipment;
				this.EquipmentCode = ((equipment4 != null) ? equipment4.CalculateEquipmentCode() : null);
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000037 RID: 55 RVA: 0x000025E5 File Offset: 0x000007E5
		// (set) Token: 0x06000038 RID: 56 RVA: 0x000025ED File Offset: 0x000007ED
		[DataSourceProperty]
		public string BannerCodeText
		{
			get
			{
				return this._bannerCode;
			}
			set
			{
				if (value != this._bannerCode)
				{
					this._bannerCode = value;
					base.OnPropertyChangedWithValue<string>(value, "BannerCodeText");
				}
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00002610 File Offset: 0x00000810
		// (set) Token: 0x0600003A RID: 58 RVA: 0x00002618 File Offset: 0x00000818
		[DataSourceProperty]
		public string BodyProperties
		{
			get
			{
				return this._bodyProperties;
			}
			set
			{
				if (value != this._bodyProperties)
				{
					this._bodyProperties = value;
					base.OnPropertyChangedWithValue<string>(value, "BodyProperties");
				}
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600003B RID: 59 RVA: 0x0000263B File Offset: 0x0000083B
		// (set) Token: 0x0600003C RID: 60 RVA: 0x00002643 File Offset: 0x00000843
		[DataSourceProperty]
		public string MountCreationKey
		{
			get
			{
				return this._mountCreationKey;
			}
			set
			{
				if (value != this._mountCreationKey)
				{
					this._mountCreationKey = value;
					base.OnPropertyChangedWithValue<string>(value, "MountCreationKey");
				}
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00002666 File Offset: 0x00000866
		// (set) Token: 0x0600003E RID: 62 RVA: 0x0000266E File Offset: 0x0000086E
		[DataSourceProperty]
		public string CharStringId
		{
			get
			{
				return this._charStringId;
			}
			set
			{
				if (value != this._charStringId)
				{
					this._charStringId = value;
					base.OnPropertyChangedWithValue<string>(value, "CharStringId");
				}
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002691 File Offset: 0x00000891
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00002699 File Offset: 0x00000899
		[DataSourceProperty]
		public int StanceIndex
		{
			get
			{
				return this._stanceIndex;
			}
			private set
			{
				if (value != this._stanceIndex)
				{
					this._stanceIndex = value;
					base.OnPropertyChangedWithValue(value, "StanceIndex");
				}
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000041 RID: 65 RVA: 0x000026B7 File Offset: 0x000008B7
		// (set) Token: 0x06000042 RID: 66 RVA: 0x000026BF File Offset: 0x000008BF
		[DataSourceProperty]
		public bool IsFemale
		{
			get
			{
				return this._isFemale;
			}
			set
			{
				if (value != this._isFemale)
				{
					this._isFemale = value;
					base.OnPropertyChangedWithValue(value, "IsFemale");
				}
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000043 RID: 67 RVA: 0x000026DD File Offset: 0x000008DD
		// (set) Token: 0x06000044 RID: 68 RVA: 0x000026E5 File Offset: 0x000008E5
		[DataSourceProperty]
		public bool IsHidden
		{
			get
			{
				return this._isHidden;
			}
			set
			{
				if (value != this._isHidden)
				{
					this._isHidden = value;
					base.OnPropertyChangedWithValue(value, "IsHidden");
				}
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00002703 File Offset: 0x00000903
		// (set) Token: 0x06000046 RID: 70 RVA: 0x0000270B File Offset: 0x0000090B
		[DataSourceProperty]
		public int Race
		{
			get
			{
				return this._race;
			}
			set
			{
				if (value != this._race)
				{
					this._race = value;
					base.OnPropertyChangedWithValue(value, "Race");
				}
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000047 RID: 71 RVA: 0x00002729 File Offset: 0x00000929
		// (set) Token: 0x06000048 RID: 72 RVA: 0x00002731 File Offset: 0x00000931
		[DataSourceProperty]
		public bool HasMount
		{
			get
			{
				return this._hasMount;
			}
			set
			{
				if (value != this._hasMount)
				{
					this._hasMount = value;
					base.OnPropertyChangedWithValue(value, "HasMount");
				}
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000049 RID: 73 RVA: 0x0000274F File Offset: 0x0000094F
		// (set) Token: 0x0600004A RID: 74 RVA: 0x00002757 File Offset: 0x00000957
		[DataSourceProperty]
		public string EquipmentCode
		{
			get
			{
				return this._equipmentCode;
			}
			set
			{
				if (value != this._equipmentCode)
				{
					this._equipmentCode = value;
					base.OnPropertyChangedWithValue<string>(value, "EquipmentCode");
				}
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600004B RID: 75 RVA: 0x0000277A File Offset: 0x0000097A
		// (set) Token: 0x0600004C RID: 76 RVA: 0x00002782 File Offset: 0x00000982
		[DataSourceProperty]
		public string IdleAction
		{
			get
			{
				return this._idleAction;
			}
			set
			{
				if (value != this._idleAction)
				{
					this._idleAction = value;
					base.OnPropertyChangedWithValue<string>(value, "IdleAction");
				}
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600004D RID: 77 RVA: 0x000027A5 File Offset: 0x000009A5
		// (set) Token: 0x0600004E RID: 78 RVA: 0x000027AD File Offset: 0x000009AD
		[DataSourceProperty]
		public string IdleFaceAnim
		{
			get
			{
				return this._idleFaceAnim;
			}
			set
			{
				if (value != this._idleFaceAnim)
				{
					this._idleFaceAnim = value;
					base.OnPropertyChangedWithValue<string>(value, "IdleFaceAnim");
				}
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600004F RID: 79 RVA: 0x000027D0 File Offset: 0x000009D0
		// (set) Token: 0x06000050 RID: 80 RVA: 0x000027D8 File Offset: 0x000009D8
		[DataSourceProperty]
		public uint ArmorColor1
		{
			get
			{
				return this._armorColor1;
			}
			set
			{
				if (value != this._armorColor1)
				{
					this._armorColor1 = value;
					base.OnPropertyChangedWithValue(value, "ArmorColor1");
				}
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000051 RID: 81 RVA: 0x000027F6 File Offset: 0x000009F6
		// (set) Token: 0x06000052 RID: 82 RVA: 0x000027FE File Offset: 0x000009FE
		[DataSourceProperty]
		public uint ArmorColor2
		{
			get
			{
				return this._armorColor2;
			}
			set
			{
				if (value != this._armorColor2)
				{
					this._armorColor2 = value;
					base.OnPropertyChangedWithValue(value, "ArmorColor2");
				}
			}
		}

		// Token: 0x04000009 RID: 9
		protected Equipment _equipment;

		// Token: 0x0400000A RID: 10
		private string _mountCreationKey = "";

		// Token: 0x0400000B RID: 11
		private string _bodyProperties = "";

		// Token: 0x0400000C RID: 12
		private string _equipmentCode;

		// Token: 0x0400000D RID: 13
		private string _idleAction;

		// Token: 0x0400000E RID: 14
		private string _idleFaceAnim;

		// Token: 0x0400000F RID: 15
		private string _charStringId;

		// Token: 0x04000010 RID: 16
		protected string _bannerCode;

		// Token: 0x04000011 RID: 17
		private bool _hasMount;

		// Token: 0x04000012 RID: 18
		private bool _isFemale;

		// Token: 0x04000013 RID: 19
		private bool _isHidden;

		// Token: 0x04000014 RID: 20
		private int _race;

		// Token: 0x04000015 RID: 21
		private int _stanceIndex;

		// Token: 0x04000016 RID: 22
		private uint _armorColor1;

		// Token: 0x04000017 RID: 23
		private uint _armorColor2;

		// Token: 0x02000028 RID: 40
		public enum StanceTypes
		{
			// Token: 0x040000B9 RID: 185
			None,
			// Token: 0x040000BA RID: 186
			EmphasizeFace,
			// Token: 0x040000BB RID: 187
			SideView,
			// Token: 0x040000BC RID: 188
			CelebrateVictory,
			// Token: 0x040000BD RID: 189
			OnMount
		}
	}
}
