using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	public class WeaponClassSelectionPopupVM : ViewModel
	{
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

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PopupHeader = new TextObject("{=wZGj3qO1}Choose What to Craft", null).ToString();
		}

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

		public void ExecuteClosePopup()
		{
			this.IsVisible = false;
		}

		public void ExecuteOpenPopup()
		{
			this.IsVisible = true;
			this.RefreshList();
		}

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

		private readonly ICraftingCampaignBehavior _craftingBehavior;

		private readonly Action<int> _onSelect;

		private readonly List<CraftingTemplate> _templatesList;

		private readonly Func<CraftingTemplate, int> _getUnlockedPiecesCount;

		private string _popupHeader;

		private bool _isVisible;

		private MBBindingList<WeaponClassVM> _weaponClasses;
	}
}
