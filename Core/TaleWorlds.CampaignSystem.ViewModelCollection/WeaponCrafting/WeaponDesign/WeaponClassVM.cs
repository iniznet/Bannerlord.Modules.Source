using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	// Token: 0x020000E4 RID: 228
	public class WeaponClassVM : ViewModel
	{
		// Token: 0x17000708 RID: 1800
		// (get) Token: 0x060014E5 RID: 5349 RVA: 0x0004E846 File Offset: 0x0004CA46
		// (set) Token: 0x060014E6 RID: 5350 RVA: 0x0004E84E File Offset: 0x0004CA4E
		public int NewlyUnlockedPieceCount { get; set; }

		// Token: 0x17000709 RID: 1801
		// (get) Token: 0x060014E7 RID: 5351 RVA: 0x0004E857 File Offset: 0x0004CA57
		public CraftingTemplate Template { get; }

		// Token: 0x060014E8 RID: 5352 RVA: 0x0004E860 File Offset: 0x0004CA60
		public WeaponClassVM(int selectionIndex, CraftingTemplate template, Action<int> onSelect)
		{
			this._onSelect = onSelect;
			this.SelectionIndex = selectionIndex;
			this.Template = template;
			this._selectedPieces = new Dictionary<CraftingPiece.PieceTypes, string>
			{
				{
					CraftingPiece.PieceTypes.Blade,
					null
				},
				{
					CraftingPiece.PieceTypes.Guard,
					null
				},
				{
					CraftingPiece.PieceTypes.Handle,
					null
				},
				{
					CraftingPiece.PieceTypes.Pommel,
					null
				}
			};
			this.RefreshValues();
		}

		// Token: 0x060014E9 RID: 5353 RVA: 0x0004E8BC File Offset: 0x0004CABC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TemplateName = this.Template.TemplateName.ToString();
			this.UnlockedPiecesLabelText = new TextObject("{=OGbskMfz}Unlocked Parts:", null).ToString();
			this.WeaponType = this.Template.StringId;
		}

		// Token: 0x060014EA RID: 5354 RVA: 0x0004E90C File Offset: 0x0004CB0C
		public void RegisterSelectedPiece(CraftingPiece.PieceTypes type, string pieceID)
		{
			string text;
			if (this._selectedPieces.TryGetValue(type, out text) && text != pieceID)
			{
				this._selectedPieces[type] = pieceID;
			}
		}

		// Token: 0x060014EB RID: 5355 RVA: 0x0004E940 File Offset: 0x0004CB40
		public string GetSelectedPieceData(CraftingPiece.PieceTypes type)
		{
			string text;
			if (this._selectedPieces.TryGetValue(type, out text))
			{
				return text;
			}
			return null;
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x0004E960 File Offset: 0x0004CB60
		public void ExecuteSelect()
		{
			Action<int> onSelect = this._onSelect;
			if (onSelect == null)
			{
				return;
			}
			onSelect(this.SelectionIndex);
		}

		// Token: 0x1700070A RID: 1802
		// (get) Token: 0x060014ED RID: 5357 RVA: 0x0004E978 File Offset: 0x0004CB78
		// (set) Token: 0x060014EE RID: 5358 RVA: 0x0004E980 File Offset: 0x0004CB80
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

		// Token: 0x1700070B RID: 1803
		// (get) Token: 0x060014EF RID: 5359 RVA: 0x0004E99E File Offset: 0x0004CB9E
		// (set) Token: 0x060014F0 RID: 5360 RVA: 0x0004E9A6 File Offset: 0x0004CBA6
		[DataSourceProperty]
		public string UnlockedPiecesLabelText
		{
			get
			{
				return this._unlockedPiecesLabelText;
			}
			set
			{
				if (value != this._unlockedPiecesLabelText)
				{
					this._unlockedPiecesLabelText = value;
					base.OnPropertyChangedWithValue<string>(value, "UnlockedPiecesLabelText");
				}
			}
		}

		// Token: 0x1700070C RID: 1804
		// (get) Token: 0x060014F1 RID: 5361 RVA: 0x0004E9C9 File Offset: 0x0004CBC9
		// (set) Token: 0x060014F2 RID: 5362 RVA: 0x0004E9D1 File Offset: 0x0004CBD1
		[DataSourceProperty]
		public int UnlockedPiecesCount
		{
			get
			{
				return this._unlockedPiecesCount;
			}
			set
			{
				if (value != this._unlockedPiecesCount)
				{
					this._unlockedPiecesCount = value;
					base.OnPropertyChangedWithValue(value, "UnlockedPiecesCount");
				}
			}
		}

		// Token: 0x1700070D RID: 1805
		// (get) Token: 0x060014F3 RID: 5363 RVA: 0x0004E9EF File Offset: 0x0004CBEF
		// (set) Token: 0x060014F4 RID: 5364 RVA: 0x0004E9F7 File Offset: 0x0004CBF7
		[DataSourceProperty]
		public string TemplateName
		{
			get
			{
				return this._templateName;
			}
			set
			{
				if (value != this._templateName)
				{
					this._templateName = value;
					base.OnPropertyChangedWithValue<string>(value, "TemplateName");
				}
			}
		}

		// Token: 0x1700070E RID: 1806
		// (get) Token: 0x060014F5 RID: 5365 RVA: 0x0004EA1A File Offset: 0x0004CC1A
		// (set) Token: 0x060014F6 RID: 5366 RVA: 0x0004EA22 File Offset: 0x0004CC22
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

		// Token: 0x1700070F RID: 1807
		// (get) Token: 0x060014F7 RID: 5367 RVA: 0x0004EA40 File Offset: 0x0004CC40
		// (set) Token: 0x060014F8 RID: 5368 RVA: 0x0004EA48 File Offset: 0x0004CC48
		[DataSourceProperty]
		public int SelectionIndex
		{
			get
			{
				return this._selectionIndex;
			}
			set
			{
				if (value != this._selectionIndex)
				{
					this._selectionIndex = value;
					base.OnPropertyChangedWithValue(value, "SelectionIndex");
				}
			}
		}

		// Token: 0x17000710 RID: 1808
		// (get) Token: 0x060014F9 RID: 5369 RVA: 0x0004EA66 File Offset: 0x0004CC66
		// (set) Token: 0x060014FA RID: 5370 RVA: 0x0004EA6E File Offset: 0x0004CC6E
		[DataSourceProperty]
		public string WeaponType
		{
			get
			{
				return this._weaponType;
			}
			set
			{
				if (value != this._weaponType)
				{
					this._weaponType = value;
					base.OnPropertyChangedWithValue<string>(value, "WeaponType");
				}
			}
		}

		// Token: 0x040009C1 RID: 2497
		private Action<int> _onSelect;

		// Token: 0x040009C2 RID: 2498
		private Dictionary<CraftingPiece.PieceTypes, string> _selectedPieces;

		// Token: 0x040009C3 RID: 2499
		private bool _hasNewlyUnlockedPieces;

		// Token: 0x040009C4 RID: 2500
		private string _unlockedPiecesLabelText;

		// Token: 0x040009C5 RID: 2501
		private int _unlockedPiecesCount;

		// Token: 0x040009C6 RID: 2502
		private string _templateName;

		// Token: 0x040009C7 RID: 2503
		private bool _isSelected;

		// Token: 0x040009C8 RID: 2504
		private int _selectionIndex;

		// Token: 0x040009C9 RID: 2505
		private string _weaponType;
	}
}
