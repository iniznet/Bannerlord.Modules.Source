using System;
using System.Linq;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Diamond.Cosmetics;
using TaleWorlds.MountAndBlade.Diamond.Cosmetics.CosmeticTypes;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory.CosmeticItem
{
	public class MPArmoryCosmeticTauntItemVM : MPArmoryCosmeticItemBaseVM
	{
		public MPArmoryCosmeticsVM.TauntCategoryFlag TauntCategory { get; }

		public TauntCosmeticElement TauntCosmeticElement { get; }

		public MPArmoryCosmeticTauntItemVM(string tauntId, CosmeticElement cosmetic, string cosmeticID)
			: base(cosmetic, cosmeticID, 3)
		{
			this.TauntID = tauntId;
			this.TauntCosmeticElement = cosmetic as TauntCosmeticElement;
			this.TauntCategory = this.GetCategoryOfTaunt();
			this.TauntUsages = new MBBindingList<StringItemWithEnabledAndHintVM>();
			this.RefreshTauntUsages();
			this.BlocksMovementOnUsageHint = new HintViewModel();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.BlocksMovementOnUsageHint != null)
			{
				this.BlocksMovementOnUsageHint.HintText = new TextObject("{=BUQsaZMg}Blocks Movement on Usage", null);
			}
			this.SelectSlotText = new TextObject("{=4gfAb1ar}Select a Slot", null).ToString();
			this.CancelEquipText = new TextObject("{=avYRbfHA}Cancel Equip", null).ToString();
			TauntCosmeticElement tauntCosmeticElement = this.TauntCosmeticElement;
			string text;
			if (tauntCosmeticElement == null)
			{
				text = null;
			}
			else
			{
				TextObject name = tauntCosmeticElement.Name;
				text = ((name != null) ? name.ToString() : null);
			}
			base.Name = text;
		}

		private void RefreshTauntUsages()
		{
			this.TauntUsages.Clear();
			TauntUsageManager.TauntUsageSet usageSet = TauntUsageManager.GetUsageSet(this.TauntCosmeticElement.Id);
			TauntUsageManager.TauntUsage.TauntUsageFlag? tauntUsageFlag;
			if (usageSet == null)
			{
				tauntUsageFlag = null;
			}
			else
			{
				MBReadOnlyList<TauntUsageManager.TauntUsage> usages = usageSet.GetUsages();
				tauntUsageFlag = ((usages != null) ? new TauntUsageManager.TauntUsage.TauntUsageFlag?(usages.FirstOrDefault<TauntUsageManager.TauntUsage>().UsageFlag) : null);
			}
			TauntUsageManager.TauntUsage.TauntUsageFlag tauntUsageFlag2 = tauntUsageFlag ?? 0;
			TextObject textObject = new TextObject("{=aeDp7IEK}Usable with {USAGE}", null);
			if ((tauntUsageFlag2 & 32) == null)
			{
				TextObject textObject2 = new TextObject("{=PiHpR4QL}One Handed", null);
				TextObject textObject3 = textObject.CopyTextObject().SetTextVariable("USAGE", textObject2);
				this.TauntUsages.Add(new StringItemWithEnabledAndHintVM(null, "UsableWithOneHanded", true, null, textObject3));
			}
			if ((tauntUsageFlag2 & 16) == null)
			{
				TextObject textObject4 = new TextObject("{=t78atYqH}Two Handed", null);
				TextObject textObject5 = textObject.CopyTextObject().SetTextVariable("USAGE", textObject4);
				this.TauntUsages.Add(new StringItemWithEnabledAndHintVM(null, "UsableWithTwoHanded", true, null, textObject5));
			}
			if ((tauntUsageFlag2 & 128) == null)
			{
				TextObject textObject6 = new TextObject("{=5rj7xQE4}Bow", null);
				TextObject textObject7 = textObject.CopyTextObject().SetTextVariable("USAGE", textObject6);
				this.TauntUsages.Add(new StringItemWithEnabledAndHintVM(null, "UsableWithBow", true, null, textObject7));
			}
			if ((tauntUsageFlag2 & 256) == null)
			{
				TextObject textObject8 = new TextObject("{=TTWL7RLe}Crossbow", null);
				TextObject textObject9 = textObject.CopyTextObject().SetTextVariable("USAGE", textObject8);
				this.TauntUsages.Add(new StringItemWithEnabledAndHintVM(null, "UsableWithCrossbow", true, null, textObject9));
			}
			if ((tauntUsageFlag2 & 64) == null)
			{
				TextObject textObject10 = new TextObject("{=Jd0Kq9lD}Shield", null);
				TextObject textObject11 = textObject.CopyTextObject().SetTextVariable("USAGE", textObject10);
				this.TauntUsages.Add(new StringItemWithEnabledAndHintVM(null, "UsableWithShield", true, null, textObject11));
			}
			if ((tauntUsageFlag2 & 8) == null)
			{
				TextObject textObject12 = new TextObject("{=uGM8DWrm}Mount", null);
				TextObject textObject13 = textObject.CopyTextObject().SetTextVariable("USAGE", textObject12);
				this.TauntUsages.Add(new StringItemWithEnabledAndHintVM(null, "UsableWithMount", true, null, textObject13));
			}
			this.RequiresOnFoot = (tauntUsageFlag2 & 8) > 0;
		}

		private MPArmoryCosmeticsVM.TauntCategoryFlag GetCategoryOfTaunt()
		{
			TauntUsageManager.TauntUsageSet usageSet = TauntUsageManager.GetUsageSet(this.TauntCosmeticElement.Id);
			MBReadOnlyList<TauntUsageManager.TauntUsage> mbreadOnlyList = ((usageSet != null) ? usageSet.GetUsages() : null);
			MPArmoryCosmeticsVM.TauntCategoryFlag tauntCategoryFlag = MPArmoryCosmeticsVM.TauntCategoryFlag.None;
			if (mbreadOnlyList == null || mbreadOnlyList.Count <= 0)
			{
				return tauntCategoryFlag;
			}
			if (this.AnyUsageNotHaveFlag(mbreadOnlyList, 8))
			{
				tauntCategoryFlag |= MPArmoryCosmeticsVM.TauntCategoryFlag.UsableWithMount;
			}
			if (this.AnyUsageNotHaveFlag(mbreadOnlyList, 64))
			{
				tauntCategoryFlag |= MPArmoryCosmeticsVM.TauntCategoryFlag.UsableWithShield;
			}
			if (this.AnyUsageNotHaveFlag(mbreadOnlyList, 32))
			{
				tauntCategoryFlag |= MPArmoryCosmeticsVM.TauntCategoryFlag.UsableWithOneHanded;
			}
			if (this.AnyUsageNotHaveFlag(mbreadOnlyList, 16))
			{
				tauntCategoryFlag |= MPArmoryCosmeticsVM.TauntCategoryFlag.UsableWithTwoHanded;
			}
			if (this.AnyUsageNotHaveFlag(mbreadOnlyList, 128))
			{
				tauntCategoryFlag |= MPArmoryCosmeticsVM.TauntCategoryFlag.UsableWithBow;
			}
			if (this.AnyUsageNotHaveFlag(mbreadOnlyList, 256))
			{
				tauntCategoryFlag |= MPArmoryCosmeticsVM.TauntCategoryFlag.UsableWithCrossbow;
			}
			return tauntCategoryFlag;
		}

		private bool AllUsagesHaveFlag(MBReadOnlyList<TauntUsageManager.TauntUsage> list, TauntUsageManager.TauntUsage.TauntUsageFlag flag)
		{
			return list.All((TauntUsageManager.TauntUsage u) => (u.UsageFlag & flag) > 0);
		}

		private bool AnyUsageHaveFlag(MBReadOnlyList<TauntUsageManager.TauntUsage> list, TauntUsageManager.TauntUsage.TauntUsageFlag flag)
		{
			return list.Any((TauntUsageManager.TauntUsage u) => (u.UsageFlag & flag) > 0);
		}

		private bool AnyUsageNotHaveFlag(MBReadOnlyList<TauntUsageManager.TauntUsage> list, TauntUsageManager.TauntUsage.TauntUsageFlag flag)
		{
			return list.Any((TauntUsageManager.TauntUsage u) => (u.UsageFlag & flag) == 0);
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
					base.UpdatePreviewAndActionTexts();
				}
			}
		}

		[DataSourceProperty]
		public bool RequiresOnFoot
		{
			get
			{
				return this._requiresOnFoot;
			}
			set
			{
				if (value != this._requiresOnFoot)
				{
					this._requiresOnFoot = value;
					base.OnPropertyChangedWithValue(value, "RequiresOnFoot");
				}
			}
		}

		[DataSourceProperty]
		public float PreviewAnimationRatio
		{
			get
			{
				return this._previewAnimationRatio;
			}
			set
			{
				if (value != this._previewAnimationRatio)
				{
					this._previewAnimationRatio = value;
					base.OnPropertyChangedWithValue(value, "PreviewAnimationRatio");
				}
			}
		}

		[DataSourceProperty]
		public string SelectSlotText
		{
			get
			{
				return this._selectSlotText;
			}
			set
			{
				if (value != this._selectSlotText)
				{
					this._selectSlotText = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectSlotText");
					base.UpdatePreviewAndActionTexts();
				}
			}
		}

		[DataSourceProperty]
		public string CancelEquipText
		{
			get
			{
				return this._cancelEquipText;
			}
			set
			{
				if (value != this._cancelEquipText)
				{
					this._cancelEquipText = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelEquipText");
					base.UpdatePreviewAndActionTexts();
				}
			}
		}

		[DataSourceProperty]
		public string TauntID
		{
			get
			{
				return this._tauntId;
			}
			set
			{
				if (value != this._tauntId)
				{
					this._tauntId = value;
					base.OnPropertyChangedWithValue<string>(value, "TauntID");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<StringItemWithEnabledAndHintVM> TauntUsages
		{
			get
			{
				return this._tauntUsages;
			}
			set
			{
				if (value != this._tauntUsages)
				{
					this._tauntUsages = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringItemWithEnabledAndHintVM>>(value, "TauntUsages");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel BlocksMovementOnUsageHint
		{
			get
			{
				return this._blocksMovementOnUsageHint;
			}
			set
			{
				if (value != this._blocksMovementOnUsageHint)
				{
					this._blocksMovementOnUsageHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "BlocksMovementOnUsageHint");
				}
			}
		}

		private bool _isSelected;

		private bool _requiresOnFoot;

		private float _previewAnimationRatio;

		private string _selectSlotText;

		private string _cancelEquipText;

		private string _tauntId;

		private HintViewModel _blocksMovementOnUsageHint;

		private MBBindingList<StringItemWithEnabledAndHintVM> _tauntUsages;
	}
}
