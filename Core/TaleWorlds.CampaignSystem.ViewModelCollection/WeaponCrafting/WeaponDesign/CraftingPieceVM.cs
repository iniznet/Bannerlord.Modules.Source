using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	public class CraftingPieceVM : ViewModel
	{
		public CraftingPieceVM()
		{
			this.ImageIdentifier = new ImageIdentifierVM(ImageIdentifierType.Null);
		}

		public CraftingPieceVM(Action<CraftingPieceVM> selectWeaponPart, string templateId, WeaponDesignElement usableCraftingPiece, int pieceType, int index, bool isOpened)
		{
			this._selectWeaponPiece = selectWeaponPart;
			this.CraftingPiece = usableCraftingPiece;
			this.Tier = usableCraftingPiece.CraftingPiece.PieceTier;
			this.TierText = Common.ToRoman(this.Tier);
			this.ImageIdentifier = new ImageIdentifierVM(usableCraftingPiece.CraftingPiece, templateId);
			this.PieceType = pieceType;
			this.Index = index;
			this.PlayerHasPiece = isOpened;
			this.ItemAttributeIcons = new MBBindingList<CraftingItemFlagVM>();
			this.IsEmpty = string.IsNullOrEmpty(this.CraftingPiece.CraftingPiece.MeshName);
			this.RefreshFlagIcons();
		}

		public void RefreshFlagIcons()
		{
			this.ItemAttributeIcons.Clear();
			foreach (Tuple<string, TextObject> tuple in CampaignUIHelper.GetItemFlagDetails(this.CraftingPiece.CraftingPiece.AdditionalItemFlags))
			{
				this.ItemAttributeIcons.Add(new CraftingItemFlagVM(tuple.Item1, tuple.Item2, true));
			}
			foreach (ValueTuple<string, TextObject> valueTuple in CampaignUIHelper.GetWeaponFlagDetails(this.CraftingPiece.CraftingPiece.AdditionalWeaponFlags, null))
			{
				this.ItemAttributeIcons.Add(new CraftingItemFlagVM(valueTuple.Item1, valueTuple.Item2, true));
			}
		}

		public void ExecuteOpenTooltip()
		{
			InformationManager.ShowTooltip(typeof(WeaponDesignElement), new object[] { this.CraftingPiece });
		}

		public void ExecuteCloseTooltip()
		{
			MBInformationManager.HideInformations();
		}

		public void ExecuteSelect()
		{
			this._selectWeaponPiece(this);
			this.IsNewlyUnlocked = false;
		}

		[DataSourceProperty]
		public bool IsFilteredOut
		{
			get
			{
				return this._isFilteredOut;
			}
			set
			{
				if (value != this._isFilteredOut)
				{
					this._isFilteredOut = value;
					base.OnPropertyChangedWithValue(value, "IsFilteredOut");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CraftingItemFlagVM> ItemAttributeIcons
		{
			get
			{
				return this._itemAttributeIcons;
			}
			set
			{
				if (value != this._itemAttributeIcons)
				{
					this._itemAttributeIcons = value;
					base.OnPropertyChangedWithValue<MBBindingList<CraftingItemFlagVM>>(value, "ItemAttributeIcons");
				}
			}
		}

		[DataSourceProperty]
		public bool PlayerHasPiece
		{
			get
			{
				return this._playerHasPiece;
			}
			set
			{
				if (this._playerHasPiece != value)
				{
					this._playerHasPiece = value;
					base.OnPropertyChangedWithValue(value, "PlayerHasPiece");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEmpty
		{
			get
			{
				return this._isEmpty;
			}
			set
			{
				if (this._isEmpty != value)
				{
					this._isEmpty = value;
					base.OnPropertyChangedWithValue(value, "IsEmpty");
				}
			}
		}

		[DataSourceProperty]
		public string TierText
		{
			get
			{
				return this._tierText;
			}
			set
			{
				if (this._tierText != value)
				{
					this._tierText = value;
					base.OnPropertyChangedWithValue<string>(value, "TierText");
				}
			}
		}

		[DataSourceProperty]
		public int Tier
		{
			get
			{
				return this._tier;
			}
			set
			{
				if (this._tier != value)
				{
					this._tier = value;
					base.OnPropertyChangedWithValue(value, "Tier");
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
				if (this._isSelected != value)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM ImageIdentifier
		{
			get
			{
				return this._imageIdentifier;
			}
			set
			{
				if (this._imageIdentifier != value)
				{
					this._imageIdentifier = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ImageIdentifier");
				}
			}
		}

		[DataSourceProperty]
		public int PieceType
		{
			get
			{
				return this._pieceType;
			}
			set
			{
				if (this._pieceType != value)
				{
					this._pieceType = value;
					base.OnPropertyChangedWithValue(value, "PieceType");
				}
			}
		}

		[DataSourceProperty]
		public bool IsNewlyUnlocked
		{
			get
			{
				return this._isNewlyUnlocked;
			}
			set
			{
				if (value != this._isNewlyUnlocked)
				{
					this._isNewlyUnlocked = value;
					base.OnPropertyChangedWithValue(value, "IsNewlyUnlocked");
				}
			}
		}

		public WeaponDesignElement CraftingPiece;

		public int Index;

		private readonly Action<CraftingPieceVM> _selectWeaponPiece;

		private bool _isFilteredOut;

		public ImageIdentifierVM _imageIdentifier;

		public int _pieceType = -1;

		public int _tier;

		public bool _isSelected;

		public bool _playerHasPiece;

		private bool _isEmpty;

		public string _tierText;

		private MBBindingList<CraftingItemFlagVM> _itemAttributeIcons;

		private bool _isNewlyUnlocked;
	}
}
