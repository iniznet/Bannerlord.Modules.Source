using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	// Token: 0x020000E1 RID: 225
	public class WeaponClassSelectionPopupVM : ViewModel
	{
		// Token: 0x060014D1 RID: 5329 RVA: 0x0004E4D8 File Offset: 0x0004C6D8
		public WeaponClassSelectionPopupVM(ICraftingCampaignBehavior craftingBehavior, List<CraftingTemplate> templatesList, Action<int> onSelect, Func<CraftingTemplate, int> getUnlockedPiecesCount)
		{
			this.WeaponClasses = new MBBindingList<WeaponClassVM>();
			this._craftingBehavior = craftingBehavior;
			this._onSelect = onSelect;
			this._templatesList = templatesList;
			this._getUnlockedPiecesCount = getUnlockedPiecesCount;
			foreach (CraftingTemplate craftingTemplate in this._templatesList)
			{
				this.WeaponClasses.Add(new WeaponClassVM(this._templatesList.IndexOf(craftingTemplate), craftingTemplate, new Action<int>(this.ExecuteSelectWeaponClass)));
			}
			this.RefreshList();
			this.RefreshValues();
		}

		// Token: 0x060014D2 RID: 5330 RVA: 0x0004E588 File Offset: 0x0004C788
		private void RefreshList()
		{
			foreach (WeaponClassVM weaponClassVM in this.WeaponClasses)
			{
				WeaponClassVM weaponClassVM2 = weaponClassVM;
				Func<CraftingTemplate, int> getUnlockedPiecesCount = this._getUnlockedPiecesCount;
				weaponClassVM2.UnlockedPiecesCount = ((getUnlockedPiecesCount != null) ? getUnlockedPiecesCount(weaponClassVM.Template) : 0);
				weaponClassVM.HasNewlyUnlockedPieces = weaponClassVM.NewlyUnlockedPieceCount > 0;
			}
		}

		// Token: 0x060014D3 RID: 5331 RVA: 0x0004E5FC File Offset: 0x0004C7FC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PopupHeader = new TextObject("{=wZGj3qO1}Choose What to Craft", null).ToString();
		}

		// Token: 0x060014D4 RID: 5332 RVA: 0x0004E61C File Offset: 0x0004C81C
		public void UpdateNewlyUnlockedPiecesCount(List<CraftingPiece> newlyUnlockedPieces)
		{
			for (int i = 0; i < this.WeaponClasses.Count; i++)
			{
				WeaponClassVM weaponClassVM = this.WeaponClasses[i];
				int num = 0;
				for (int j = 0; j < newlyUnlockedPieces.Count; j++)
				{
					CraftingPiece craftingPiece = newlyUnlockedPieces[j];
					if (weaponClassVM.Template.IsPieceTypeUsable(craftingPiece.PieceType))
					{
						CraftingPiece craftingPiece2 = this.FindPieceInTemplate(weaponClassVM.Template, craftingPiece);
						if (craftingPiece2 != null && !craftingPiece2.IsHiddenOnDesigner && this._craftingBehavior.IsOpened(craftingPiece2, weaponClassVM.Template))
						{
							num++;
						}
					}
				}
				weaponClassVM.NewlyUnlockedPieceCount = num;
			}
		}

		// Token: 0x060014D5 RID: 5333 RVA: 0x0004E6BC File Offset: 0x0004C8BC
		private CraftingPiece FindPieceInTemplate(CraftingTemplate template, CraftingPiece piece)
		{
			foreach (CraftingPiece craftingPiece in template.Pieces)
			{
				if (piece.StringId == craftingPiece.StringId)
				{
					return craftingPiece;
				}
			}
			return null;
		}

		// Token: 0x060014D6 RID: 5334 RVA: 0x0004E724 File Offset: 0x0004C924
		public void ExecuteSelectWeaponClass(int index)
		{
			if (this.WeaponClasses[index].IsSelected)
			{
				this.ExecuteClosePopup();
				return;
			}
			Action<int> onSelect = this._onSelect;
			if (onSelect != null)
			{
				onSelect(index);
			}
			this.ExecuteClosePopup();
		}

		// Token: 0x060014D7 RID: 5335 RVA: 0x0004E758 File Offset: 0x0004C958
		public void ExecuteClosePopup()
		{
			this.IsVisible = false;
		}

		// Token: 0x060014D8 RID: 5336 RVA: 0x0004E761 File Offset: 0x0004C961
		public void ExecuteOpenPopup()
		{
			this.IsVisible = true;
			this.RefreshList();
		}

		// Token: 0x17000703 RID: 1795
		// (get) Token: 0x060014D9 RID: 5337 RVA: 0x0004E770 File Offset: 0x0004C970
		// (set) Token: 0x060014DA RID: 5338 RVA: 0x0004E778 File Offset: 0x0004C978
		[DataSourceProperty]
		public string PopupHeader
		{
			get
			{
				return this._popupHeader;
			}
			set
			{
				if (value != this._popupHeader)
				{
					this._popupHeader = value;
					base.OnPropertyChangedWithValue<string>(value, "PopupHeader");
				}
			}
		}

		// Token: 0x17000704 RID: 1796
		// (get) Token: 0x060014DB RID: 5339 RVA: 0x0004E79B File Offset: 0x0004C99B
		// (set) Token: 0x060014DC RID: 5340 RVA: 0x0004E7A3 File Offset: 0x0004C9A3
		[DataSourceProperty]
		public bool IsVisible
		{
			get
			{
				return this._isVisible;
			}
			set
			{
				if (value != this._isVisible)
				{
					this._isVisible = value;
					base.OnPropertyChangedWithValue(value, "IsVisible");
					Game game = Game.Current;
					if (game == null)
					{
						return;
					}
					game.EventManager.TriggerEvent<CraftingWeaponClassSelectionOpenedEvent>(new CraftingWeaponClassSelectionOpenedEvent(this._isVisible));
				}
			}
		}

		// Token: 0x17000705 RID: 1797
		// (get) Token: 0x060014DD RID: 5341 RVA: 0x0004E7E0 File Offset: 0x0004C9E0
		// (set) Token: 0x060014DE RID: 5342 RVA: 0x0004E7E8 File Offset: 0x0004C9E8
		[DataSourceProperty]
		public MBBindingList<WeaponClassVM> WeaponClasses
		{
			get
			{
				return this._weaponClasses;
			}
			set
			{
				if (value != this._weaponClasses)
				{
					this._weaponClasses = value;
					base.OnPropertyChangedWithValue<MBBindingList<WeaponClassVM>>(value, "WeaponClasses");
				}
			}
		}

		// Token: 0x040009B6 RID: 2486
		private readonly ICraftingCampaignBehavior _craftingBehavior;

		// Token: 0x040009B7 RID: 2487
		private readonly Action<int> _onSelect;

		// Token: 0x040009B8 RID: 2488
		private readonly List<CraftingTemplate> _templatesList;

		// Token: 0x040009B9 RID: 2489
		private readonly Func<CraftingTemplate, int> _getUnlockedPiecesCount;

		// Token: 0x040009BA RID: 2490
		private string _popupHeader;

		// Token: 0x040009BB RID: 2491
		private bool _isVisible;

		// Token: 0x040009BC RID: 2492
		private MBBindingList<WeaponClassVM> _weaponClasses;
	}
}
