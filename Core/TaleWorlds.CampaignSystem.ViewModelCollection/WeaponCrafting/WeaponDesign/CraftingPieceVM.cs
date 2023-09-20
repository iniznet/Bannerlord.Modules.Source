using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	// Token: 0x020000DE RID: 222
	public class CraftingPieceVM : ViewModel
	{
		// Token: 0x060014AA RID: 5290 RVA: 0x0004E00D File Offset: 0x0004C20D
		public CraftingPieceVM()
		{
			this.ImageIdentifier = new ImageIdentifierVM(ImageIdentifierType.Null);
		}

		// Token: 0x060014AB RID: 5291 RVA: 0x0004E028 File Offset: 0x0004C228
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

		// Token: 0x060014AC RID: 5292 RVA: 0x0004E0C8 File Offset: 0x0004C2C8
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

		// Token: 0x060014AD RID: 5293 RVA: 0x0004E1B4 File Offset: 0x0004C3B4
		public void ExecuteOpenTooltip()
		{
			InformationManager.ShowTooltip(typeof(WeaponDesignElement), new object[] { this.CraftingPiece });
		}

		// Token: 0x060014AE RID: 5294 RVA: 0x0004E1D4 File Offset: 0x0004C3D4
		public void ExecuteCloseTooltip()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x060014AF RID: 5295 RVA: 0x0004E1DB File Offset: 0x0004C3DB
		public void ExecuteSelect()
		{
			this._selectWeaponPiece(this);
			this.IsNewlyUnlocked = false;
		}

		// Token: 0x170006F2 RID: 1778
		// (get) Token: 0x060014B0 RID: 5296 RVA: 0x0004E1F0 File Offset: 0x0004C3F0
		// (set) Token: 0x060014B1 RID: 5297 RVA: 0x0004E1F8 File Offset: 0x0004C3F8
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

		// Token: 0x170006F3 RID: 1779
		// (get) Token: 0x060014B2 RID: 5298 RVA: 0x0004E216 File Offset: 0x0004C416
		// (set) Token: 0x060014B3 RID: 5299 RVA: 0x0004E21E File Offset: 0x0004C41E
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

		// Token: 0x170006F4 RID: 1780
		// (get) Token: 0x060014B4 RID: 5300 RVA: 0x0004E23C File Offset: 0x0004C43C
		// (set) Token: 0x060014B5 RID: 5301 RVA: 0x0004E244 File Offset: 0x0004C444
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

		// Token: 0x170006F5 RID: 1781
		// (get) Token: 0x060014B6 RID: 5302 RVA: 0x0004E262 File Offset: 0x0004C462
		// (set) Token: 0x060014B7 RID: 5303 RVA: 0x0004E26A File Offset: 0x0004C46A
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

		// Token: 0x170006F6 RID: 1782
		// (get) Token: 0x060014B8 RID: 5304 RVA: 0x0004E288 File Offset: 0x0004C488
		// (set) Token: 0x060014B9 RID: 5305 RVA: 0x0004E290 File Offset: 0x0004C490
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

		// Token: 0x170006F7 RID: 1783
		// (get) Token: 0x060014BA RID: 5306 RVA: 0x0004E2B3 File Offset: 0x0004C4B3
		// (set) Token: 0x060014BB RID: 5307 RVA: 0x0004E2BB File Offset: 0x0004C4BB
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

		// Token: 0x170006F8 RID: 1784
		// (get) Token: 0x060014BC RID: 5308 RVA: 0x0004E2D9 File Offset: 0x0004C4D9
		// (set) Token: 0x060014BD RID: 5309 RVA: 0x0004E2E1 File Offset: 0x0004C4E1
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

		// Token: 0x170006F9 RID: 1785
		// (get) Token: 0x060014BE RID: 5310 RVA: 0x0004E2FF File Offset: 0x0004C4FF
		// (set) Token: 0x060014BF RID: 5311 RVA: 0x0004E307 File Offset: 0x0004C507
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

		// Token: 0x170006FA RID: 1786
		// (get) Token: 0x060014C0 RID: 5312 RVA: 0x0004E325 File Offset: 0x0004C525
		// (set) Token: 0x060014C1 RID: 5313 RVA: 0x0004E32D File Offset: 0x0004C52D
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

		// Token: 0x170006FB RID: 1787
		// (get) Token: 0x060014C2 RID: 5314 RVA: 0x0004E34B File Offset: 0x0004C54B
		// (set) Token: 0x060014C3 RID: 5315 RVA: 0x0004E353 File Offset: 0x0004C553
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

		// Token: 0x040009A1 RID: 2465
		public WeaponDesignElement CraftingPiece;

		// Token: 0x040009A2 RID: 2466
		public int Index;

		// Token: 0x040009A3 RID: 2467
		private readonly Action<CraftingPieceVM> _selectWeaponPiece;

		// Token: 0x040009A4 RID: 2468
		private bool _isFilteredOut;

		// Token: 0x040009A5 RID: 2469
		public ImageIdentifierVM _imageIdentifier;

		// Token: 0x040009A6 RID: 2470
		public int _pieceType = -1;

		// Token: 0x040009A7 RID: 2471
		public int _tier;

		// Token: 0x040009A8 RID: 2472
		public bool _isSelected;

		// Token: 0x040009A9 RID: 2473
		public bool _playerHasPiece;

		// Token: 0x040009AA RID: 2474
		private bool _isEmpty;

		// Token: 0x040009AB RID: 2475
		public string _tierText;

		// Token: 0x040009AC RID: 2476
		private MBBindingList<CraftingItemFlagVM> _itemAttributeIcons;

		// Token: 0x040009AD RID: 2477
		private bool _isNewlyUnlocked;
	}
}
