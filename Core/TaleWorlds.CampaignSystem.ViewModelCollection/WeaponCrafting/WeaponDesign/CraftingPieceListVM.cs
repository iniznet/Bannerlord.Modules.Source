using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	// Token: 0x020000DD RID: 221
	public class CraftingPieceListVM : ViewModel
	{
		// Token: 0x0600149D RID: 5277 RVA: 0x0004DEE0 File Offset: 0x0004C0E0
		public CraftingPieceListVM(MBBindingList<CraftingPieceVM> pieceList, CraftingPiece.PieceTypes pieceType, Action<CraftingPiece.PieceTypes> onSelect)
		{
			this.Pieces = pieceList;
			this.PieceType = pieceType;
			this._onSelect = onSelect;
		}

		// Token: 0x0600149E RID: 5278 RVA: 0x0004DEFD File Offset: 0x0004C0FD
		public void ExecuteSelect()
		{
			Action<CraftingPiece.PieceTypes> onSelect = this._onSelect;
			if (onSelect != null)
			{
				onSelect(this.PieceType);
			}
			this.HasNewlyUnlockedPieces = false;
		}

		// Token: 0x0600149F RID: 5279 RVA: 0x0004DF1D File Offset: 0x0004C11D
		public void Refresh()
		{
			this.HasNewlyUnlockedPieces = this.Pieces.Any((CraftingPieceVM x) => x.IsNewlyUnlocked);
		}

		// Token: 0x170006ED RID: 1773
		// (get) Token: 0x060014A0 RID: 5280 RVA: 0x0004DF4F File Offset: 0x0004C14F
		// (set) Token: 0x060014A1 RID: 5281 RVA: 0x0004DF57 File Offset: 0x0004C157
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

		// Token: 0x170006EE RID: 1774
		// (get) Token: 0x060014A2 RID: 5282 RVA: 0x0004DF75 File Offset: 0x0004C175
		// (set) Token: 0x060014A3 RID: 5283 RVA: 0x0004DF7D File Offset: 0x0004C17D
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

		// Token: 0x170006EF RID: 1775
		// (get) Token: 0x060014A4 RID: 5284 RVA: 0x0004DF9B File Offset: 0x0004C19B
		// (set) Token: 0x060014A5 RID: 5285 RVA: 0x0004DFA3 File Offset: 0x0004C1A3
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

		// Token: 0x170006F0 RID: 1776
		// (get) Token: 0x060014A6 RID: 5286 RVA: 0x0004DFC1 File Offset: 0x0004C1C1
		// (set) Token: 0x060014A7 RID: 5287 RVA: 0x0004DFC9 File Offset: 0x0004C1C9
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

		// Token: 0x170006F1 RID: 1777
		// (get) Token: 0x060014A8 RID: 5288 RVA: 0x0004DFE7 File Offset: 0x0004C1E7
		// (set) Token: 0x060014A9 RID: 5289 RVA: 0x0004DFEF File Offset: 0x0004C1EF
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

		// Token: 0x0400099A RID: 2458
		public CraftingPiece.PieceTypes PieceType;

		// Token: 0x0400099B RID: 2459
		private Action<CraftingPiece.PieceTypes> _onSelect;

		// Token: 0x0400099C RID: 2460
		private bool _hasNewlyUnlockedPieces;

		// Token: 0x0400099D RID: 2461
		private MBBindingList<CraftingPieceVM> _pieces;

		// Token: 0x0400099E RID: 2462
		private bool _isSelected;

		// Token: 0x0400099F RID: 2463
		private bool _isEnabled;

		// Token: 0x040009A0 RID: 2464
		private CraftingPieceVM _selectedPiece;
	}
}
