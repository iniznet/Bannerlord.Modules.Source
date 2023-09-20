using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection
{
	public class CharacterViewModel : ViewModel
	{
		public CharacterViewModel()
		{
		}

		public CharacterViewModel(CharacterViewModel.StanceTypes stance = CharacterViewModel.StanceTypes.None)
		{
			this._equipment = new Equipment(false);
			this.EquipmentCode = this._equipment.CalculateEquipmentCode();
			this.StanceIndex = (int)stance;
		}

		public void SetEquipment(EquipmentIndex index, EquipmentElement item)
		{
			this._equipment[(int)index] = item;
			this.EquipmentCode = this._equipment.CalculateEquipmentCode();
			Equipment equipment = this._equipment;
			this.HasMount = ((equipment != null) ? equipment[10].Item : null) != null;
		}

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

		public void FillFrom(CharacterViewModel characterViewModel, int seed = -1)
		{
			this.ArmorColor1 = characterViewModel.ArmorColor1;
			this.ArmorColor2 = characterViewModel.ArmorColor2;
			this.CharStringId = characterViewModel.CharStringId;
			this.IsFemale = characterViewModel.IsFemale;
			this.Race = characterViewModel.Race;
			this.BodyProperties = characterViewModel.BodyProperties;
			this.MountCreationKey = characterViewModel.MountCreationKey;
			this._equipment = characterViewModel._equipment.Clone(false);
			Equipment equipment = this._equipment;
			this.HasMount = ((equipment != null) ? equipment[10].Item : null) != null;
			Equipment equipment2 = this._equipment;
			this.EquipmentCode = ((equipment2 != null) ? equipment2.CalculateEquipmentCode() : null);
		}

		public void ExecuteEquipWeaponAtIndex(EquipmentIndex index, bool isLeftHand)
		{
			Equipment equipment = this._equipment;
			bool flag;
			if (equipment == null)
			{
				flag = null != null;
			}
			else
			{
				ItemObject item = equipment[index].Item;
				flag = ((item != null) ? item.WeaponComponent : null) != null;
			}
			if (flag)
			{
				if (isLeftHand)
				{
					this.LeftHandWieldedEquipmentIndex = (int)index;
					return;
				}
				this.RightHandWieldedEquipmentIndex = (int)index;
			}
		}

		public void ExecuteStartCustomAnimation(string animation, bool loop = false, float loopInterval = 0f)
		{
			this.ExecuteStopCustomAnimation();
			this.CustomAnimation = animation;
			this.ShouldLoopCustomAnimation = loop;
			this.CustomAnimationWaitDuration = loopInterval;
			this.IsPlayingCustomAnimations = true;
		}

		public void ExecuteStopCustomAnimation()
		{
			this._isManuallyStoppingAnimation = true;
			this.CustomAnimation = null;
			this.ShouldLoopCustomAnimation = false;
			this.CustomAnimationWaitDuration = 0f;
			if (this.IsPlayingCustomAnimations)
			{
				Action<CharacterViewModel> onCustomAnimationFinished = CharacterViewModel.OnCustomAnimationFinished;
				if (onCustomAnimationFinished != null)
				{
					onCustomAnimationFinished(this);
				}
			}
			this.IsPlayingCustomAnimations = false;
			this._isManuallyStoppingAnimation = false;
		}

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

		[DataSourceProperty]
		public string CustomAnimation
		{
			get
			{
				return this._customAnimation;
			}
			set
			{
				if (value != this._customAnimation)
				{
					this._customAnimation = value;
					base.OnPropertyChangedWithValue<string>(value, "CustomAnimation");
				}
			}
		}

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

		[DataSourceProperty]
		public bool IsPlayingCustomAnimations
		{
			get
			{
				return this._isPlayingCustomAnimations;
			}
			set
			{
				if (value != this._isPlayingCustomAnimations)
				{
					this._isPlayingCustomAnimations = value;
					base.OnPropertyChangedWithValue(value, "IsPlayingCustomAnimations");
					if (!this._isPlayingCustomAnimations && !this._isManuallyStoppingAnimation && !this.ShouldLoopCustomAnimation)
					{
						Action<CharacterViewModel> onCustomAnimationFinished = CharacterViewModel.OnCustomAnimationFinished;
						if (onCustomAnimationFinished == null)
						{
							return;
						}
						onCustomAnimationFinished(this);
					}
				}
			}
		}

		[DataSourceProperty]
		public bool ShouldLoopCustomAnimation
		{
			get
			{
				return this._shouldLoopCustomAnimation;
			}
			set
			{
				if (value != this._shouldLoopCustomAnimation)
				{
					this._shouldLoopCustomAnimation = value;
					base.OnPropertyChangedWithValue(value, "ShouldLoopCustomAnimation");
				}
			}
		}

		[DataSourceProperty]
		public float CustomAnimationProgressRatio
		{
			get
			{
				return this._customAnimationProgressRatio;
			}
			set
			{
				if (value != this._customAnimationProgressRatio)
				{
					this._customAnimationProgressRatio = value;
					base.OnPropertyChangedWithValue(value, "CustomAnimationProgressRatio");
				}
			}
		}

		[DataSourceProperty]
		public float CustomAnimationWaitDuration
		{
			get
			{
				return this._customAnimationWaitDuration;
			}
			set
			{
				if (value != this._customAnimationWaitDuration)
				{
					this._customAnimationWaitDuration = value;
					base.OnPropertyChangedWithValue(value, "CustomAnimationWaitDuration");
				}
			}
		}

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

		[DataSourceProperty]
		public int LeftHandWieldedEquipmentIndex
		{
			get
			{
				return this._leftHandWieldedEquipmentIndex;
			}
			set
			{
				if (value != this._leftHandWieldedEquipmentIndex)
				{
					this._leftHandWieldedEquipmentIndex = value;
					base.OnPropertyChangedWithValue(value, "LeftHandWieldedEquipmentIndex");
				}
			}
		}

		[DataSourceProperty]
		public int RightHandWieldedEquipmentIndex
		{
			get
			{
				return this._rightHandWieldedEquipmentIndex;
			}
			set
			{
				if (value != this._rightHandWieldedEquipmentIndex)
				{
					this._rightHandWieldedEquipmentIndex = value;
					base.OnPropertyChangedWithValue(value, "RightHandWieldedEquipmentIndex");
				}
			}
		}

		public static Action<CharacterViewModel> OnCustomAnimationFinished;

		private bool _isManuallyStoppingAnimation;

		protected Equipment _equipment;

		private string _mountCreationKey = "";

		private string _bodyProperties = "";

		private string _equipmentCode;

		private string _idleAction;

		private string _idleFaceAnim;

		private string _charStringId;

		private string _customAnimation;

		protected string _bannerCode;

		private bool _hasMount;

		private bool _isFemale;

		private bool _isHidden;

		private bool _isPlayingCustomAnimations;

		private bool _shouldLoopCustomAnimation;

		private float _customAnimationProgressRatio;

		private float _customAnimationWaitDuration;

		private int _race;

		private int _stanceIndex;

		private uint _armorColor1;

		private uint _armorColor2;

		private int _leftHandWieldedEquipmentIndex;

		private int _rightHandWieldedEquipmentIndex;

		public enum StanceTypes
		{
			None,
			EmphasizeFace,
			SideView,
			CelebrateVictory,
			OnMount
		}
	}
}
