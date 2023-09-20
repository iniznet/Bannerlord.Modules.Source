using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	public class CraftingPieceListVM : ViewModel
	{
		public CraftingPieceListVM(MBBindingList<CraftingPieceVM> pieceList, CraftingPiece.PieceTypes pieceType, Action<CraftingPiece.PieceTypes> onSelect)
		{
			this.Pieces = pieceList;
			this.PieceType = pieceType;
			this._onSelect = onSelect;
		}

		public void ExecuteSelect()
		{
			Action<CraftingPiece.PieceTypes> onSelect = this._onSelect;
			if (onSelect != null)
			{
				onSelect(this.PieceType);
			}
			this.HasNewlyUnlockedPieces = false;
		}

		public void Refresh()
		{
			this.HasNewlyUnlockedPieces = this.Pieces.Any((CraftingPieceVM x) => x.IsNewlyUnlocked);
		}

		[DataSourceProperty]
		public bool HasNewlyUnlockedPieces
		{
			get
			{
				return this._hasNewlyUnlockedPieces;
			}
			set
			{
				if (value != this._hasNewlyUnlockedPieces)
				{
					this._hasNewlyUnlockedPieces = value;
					base.OnPropertyChangedWithValue(value, "HasNewlyUnlockedPieces");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CraftingPieceVM> Pieces
		{
			get
			{
				return this._pieces;
			}
			set
			{
				if (value != this._pieces)
				{
					this._pieces = value;
					base.OnPropertyChangedWithValue<MBBindingList<CraftingPieceVM>>(value, "Pieces");
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
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public CraftingPieceVM SelectedPiece
		{
			get
			{
				return this._selectedPiece;
			}
			set
			{
				if (value != this._selectedPiece)
				{
					this._selectedPiece = value;
					base.OnPropertyChangedWithValue<CraftingPieceVM>(value, "SelectedPiece");
				}
			}
		}

		public CraftingPiece.PieceTypes PieceType;

		private Action<CraftingPiece.PieceTypes> _onSelect;

		private bool _hasNewlyUnlockedPieces;

		private MBBindingList<CraftingPieceVM> _pieces;

		private bool _isSelected;

		private bool _isEnabled;

		private CraftingPieceVM _selectedPiece;
	}
}
