using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CraftingSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign.Order;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	// Token: 0x020000E9 RID: 233
	public class WeaponDesignVM : ViewModel
	{
		// Token: 0x0600154F RID: 5455 RVA: 0x0004F600 File Offset: 0x0004D800
		public WeaponDesignVM(Crafting crafting, ICraftingCampaignBehavior craftingBehavior, Action onRefresh, Action onWeaponCrafted, Func<CraftingAvailableHeroItemVM> getCurrentCraftingHero, Action<CraftingOrder> refreshHeroAvailabilities, Func<WeaponComponentData, ItemObject.ItemUsageSetFlags> getItemUsageSetFlags)
		{
			this._crafting = crafting;
			this._craftingBehavior = craftingBehavior;
			this._onRefresh = onRefresh;
			this._onWeaponCrafted = onWeaponCrafted;
			this._getCurrentCraftingHero = getCurrentCraftingHero;
			this._getItemUsageSetFlags = getItemUsageSetFlags;
			this._refreshHeroAvailabilities = refreshHeroAvailabilities;
			this.MaxDifficulty = 300;
			this._currentCraftingSkillText = new TextObject("{=LEiZWuZm}{SKILL_NAME}: {SKILL_VALUE}", null);
			this.PrimaryPropertyList = new MBBindingList<CraftingListPropertyItem>();
			this.DesignResultPropertyList = new MBBindingList<WeaponDesignResultPropertyItemVM>();
			this._newlyUnlockedPieces = new List<CraftingPiece>();
			this._pieceTierComparer = new WeaponDesignVM.PieceTierComparer();
			this.BladePieceList = new CraftingPieceListVM(new MBBindingList<CraftingPieceVM>(), CraftingPiece.PieceTypes.Blade, new Action<CraftingPiece.PieceTypes>(this.OnSelectPieceType));
			this.GuardPieceList = new CraftingPieceListVM(new MBBindingList<CraftingPieceVM>(), CraftingPiece.PieceTypes.Guard, new Action<CraftingPiece.PieceTypes>(this.OnSelectPieceType));
			this.HandlePieceList = new CraftingPieceListVM(new MBBindingList<CraftingPieceVM>(), CraftingPiece.PieceTypes.Handle, new Action<CraftingPiece.PieceTypes>(this.OnSelectPieceType));
			this.PommelPieceList = new CraftingPieceListVM(new MBBindingList<CraftingPieceVM>(), CraftingPiece.PieceTypes.Pommel, new Action<CraftingPiece.PieceTypes>(this.OnSelectPieceType));
			this.PieceLists = new MBBindingList<CraftingPieceListVM> { this.BladePieceList, this.GuardPieceList, this.HandlePieceList, this.PommelPieceList };
			this._pieceListsDictionary = new Dictionary<CraftingPiece.PieceTypes, CraftingPieceListVM>
			{
				{
					CraftingPiece.PieceTypes.Blade,
					this.BladePieceList
				},
				{
					CraftingPiece.PieceTypes.Guard,
					this.GuardPieceList
				},
				{
					CraftingPiece.PieceTypes.Handle,
					this.HandlePieceList
				},
				{
					CraftingPiece.PieceTypes.Pommel,
					this.PommelPieceList
				}
			};
			this._pieceVMs = new Dictionary<CraftingPiece, CraftingPieceVM>();
			this.TierFilters = new MBBindingList<TierFilterTypeVM>
			{
				new TierFilterTypeVM(WeaponDesignVM.CraftingPieceTierFilter.All, new Action<WeaponDesignVM.CraftingPieceTierFilter>(this.OnSelectPieceTierFilter), GameTexts.FindText("str_crafting_tier_filter_all", null).ToString()),
				new TierFilterTypeVM(WeaponDesignVM.CraftingPieceTierFilter.Tier1, new Action<WeaponDesignVM.CraftingPieceTierFilter>(this.OnSelectPieceTierFilter), GameTexts.FindText("str_tier_one", null).ToString()),
				new TierFilterTypeVM(WeaponDesignVM.CraftingPieceTierFilter.Tier2, new Action<WeaponDesignVM.CraftingPieceTierFilter>(this.OnSelectPieceTierFilter), GameTexts.FindText("str_tier_two", null).ToString()),
				new TierFilterTypeVM(WeaponDesignVM.CraftingPieceTierFilter.Tier3, new Action<WeaponDesignVM.CraftingPieceTierFilter>(this.OnSelectPieceTierFilter), GameTexts.FindText("str_tier_three", null).ToString()),
				new TierFilterTypeVM(WeaponDesignVM.CraftingPieceTierFilter.Tier4, new Action<WeaponDesignVM.CraftingPieceTierFilter>(this.OnSelectPieceTierFilter), GameTexts.FindText("str_tier_four", null).ToString()),
				new TierFilterTypeVM(WeaponDesignVM.CraftingPieceTierFilter.Tier5, new Action<WeaponDesignVM.CraftingPieceTierFilter>(this.OnSelectPieceTierFilter), GameTexts.FindText("str_tier_five", null).ToString())
			};
			this._templateComparer = new WeaponDesignVM.TemplateComparer();
			this._primaryUsages = CraftingTemplate.All.ToList<CraftingTemplate>();
			this._primaryUsages.Sort(this._templateComparer);
			this.SecondaryUsageSelector = new SelectorVM<CraftingSecondaryUsageItemVM>(new List<string>(), 0, null);
			this.CraftingOrderPopup = new CraftingOrderPopupVM(new Action<CraftingOrderItemVM>(this.OnCraftingOrderSelected), this._getCurrentCraftingHero, new Func<CraftingOrder, IEnumerable<CraftingStatData>>(this.GetOrderStatDatas));
			this.WeaponClassSelectionPopup = new WeaponClassSelectionPopupVM(this._craftingBehavior, this._primaryUsages, delegate(int x)
			{
				this.RefreshWeaponDesignMode(null, x, false);
			}, new Func<CraftingTemplate, int>(this.GetUnlockedPartsCount));
			this.WeaponFlagIconsList = new MBBindingList<ItemFlagVM>();
			this.CraftedItemVisual = new ItemCollectionElementViewModel();
			CampaignEvents.CraftingPartUnlockedEvent.AddNonSerializedListener(this, new Action<CraftingPiece>(this.OnNewPieceUnlocked));
			this.CraftingHistory = new CraftingHistoryVM(this._crafting, this._craftingBehavior, delegate
			{
				CraftingOrderItemVM activeCraftingOrder = this.ActiveCraftingOrder;
				if (activeCraftingOrder == null)
				{
					return null;
				}
				return activeCraftingOrder.CraftingOrder;
			}, new Action<WeaponDesignSelectorVM>(this.OnSelectItemFromHistory));
			this.RefreshWeaponDesignMode(null, -1, false);
			this._selectedWeaponClassIndex = this._primaryUsages.IndexOf(this._crafting.CurrentCraftingTemplate);
		}

		// Token: 0x06001550 RID: 5456 RVA: 0x0004F9D0 File Offset: 0x0004DBD0
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ShowOnlyUnlockedPiecesHint = new HintViewModel(new TextObject("{=dOa7frHR}Show only unlocked pieces", null), null);
			this.ComponentSizeLbl = new TextObject("{=OkWLI5C8}Size:", null).ToString();
			this.AlternativeUsageText = new TextObject("{=13wo3QQB}Secondary", null).ToString();
			this.DefaultUsageText = new TextObject("{=ta4R2RR7}Primary", null).ToString();
			this.DifficultyText = GameTexts.FindText("str_difficulty", null).ToString();
			this.ScabbardHint = new HintViewModel(GameTexts.FindText("str_toggle_scabbard", null), null);
			this.RandomizeHint = new HintViewModel(GameTexts.FindText("str_randomize", null), null);
			this.UndoHint = new HintViewModel(GameTexts.FindText("str_undo", null), null);
			this.RedoHint = new HintViewModel(GameTexts.FindText("str_redo", null), null);
			this._primaryPropertyList.ApplyActionOnAllItems(delegate(CraftingListPropertyItem x)
			{
				x.RefreshValues();
			});
			CraftingPieceVM selectedBladePiece = this._selectedBladePiece;
			if (selectedBladePiece != null)
			{
				selectedBladePiece.RefreshValues();
			}
			CraftingPieceVM selectedGuardPiece = this._selectedGuardPiece;
			if (selectedGuardPiece != null)
			{
				selectedGuardPiece.RefreshValues();
			}
			CraftingPieceVM selectedHandlePiece = this._selectedHandlePiece;
			if (selectedHandlePiece != null)
			{
				selectedHandlePiece.RefreshValues();
			}
			CraftingPieceVM selectedPommelPiece = this._selectedPommelPiece;
			if (selectedPommelPiece != null)
			{
				selectedPommelPiece.RefreshValues();
			}
			this._secondaryUsageSelector.RefreshValues();
			this._craftingOrderPopup.RefreshValues();
			this.ChooseOrderText = this.CraftingOrderPopup.OrderCountText;
			this.ChooseWeaponTypeText = new TextObject("{=Gd6zuUwh}Free Build", null).ToString();
			this.CurrentCraftedWeaponTypeText = this._crafting.CurrentCraftingTemplate.TemplateName.ToString();
			this.CurrentCraftedWeaponTemplateId = this._crafting.CurrentCraftingTemplate.StringId;
		}

		// Token: 0x06001551 RID: 5457 RVA: 0x0004FB8C File Offset: 0x0004DD8C
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.CraftingPartUnlockedEvent.ClearListeners(this);
			CraftingHistoryVM craftingHistory = this.CraftingHistory;
			if (craftingHistory != null)
			{
				craftingHistory.OnFinalize();
			}
			ItemCollectionElementViewModel craftedItemVisual = this.CraftedItemVisual;
			if (craftedItemVisual != null)
			{
				craftedItemVisual.OnFinalize();
			}
			WeaponDesignResultPopupVM craftingResultPopup = this.CraftingResultPopup;
			if (craftingResultPopup != null)
			{
				craftingResultPopup.OnFinalize();
			}
			this.CraftedItemVisual = null;
		}

		// Token: 0x06001552 RID: 5458 RVA: 0x0004FBE4 File Offset: 0x0004DDE4
		internal void OnCraftingLogicRefreshed(Crafting newCraftingLogic)
		{
			this._crafting = newCraftingLogic;
			this.InitializeDefaultFromLogic();
		}

		// Token: 0x06001553 RID: 5459 RVA: 0x0004FBF4 File Offset: 0x0004DDF4
		private void FilterPieces(WeaponDesignVM.CraftingPieceTierFilter filter)
		{
			List<int> list = new List<int>();
			switch (filter)
			{
			case WeaponDesignVM.CraftingPieceTierFilter.None:
				goto IL_9B;
			case WeaponDesignVM.CraftingPieceTierFilter.Tier1:
				list.Add(1);
				goto IL_9B;
			case WeaponDesignVM.CraftingPieceTierFilter.Tier2:
				list.Add(2);
				goto IL_9B;
			case WeaponDesignVM.CraftingPieceTierFilter.Tier1 | WeaponDesignVM.CraftingPieceTierFilter.Tier2:
			case WeaponDesignVM.CraftingPieceTierFilter.Tier1 | WeaponDesignVM.CraftingPieceTierFilter.Tier3:
			case WeaponDesignVM.CraftingPieceTierFilter.Tier2 | WeaponDesignVM.CraftingPieceTierFilter.Tier3:
			case WeaponDesignVM.CraftingPieceTierFilter.Tier1 | WeaponDesignVM.CraftingPieceTierFilter.Tier2 | WeaponDesignVM.CraftingPieceTierFilter.Tier3:
				break;
			case WeaponDesignVM.CraftingPieceTierFilter.Tier3:
				list.Add(3);
				goto IL_9B;
			case WeaponDesignVM.CraftingPieceTierFilter.Tier4:
				list.Add(4);
				goto IL_9B;
			default:
				if (filter == WeaponDesignVM.CraftingPieceTierFilter.Tier5)
				{
					list.Add(5);
					goto IL_9B;
				}
				if (filter == WeaponDesignVM.CraftingPieceTierFilter.All)
				{
					list.AddRange(new int[] { 1, 2, 3, 4, 5 });
					goto IL_9B;
				}
				break;
			}
			Debug.FailedAssert("Invalid tier filter", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Crafting\\WeaponDesign\\WeaponDesignVM.cs", "FilterPieces", 217);
			IL_9B:
			foreach (TierFilterTypeVM tierFilterTypeVM in this.TierFilters)
			{
				tierFilterTypeVM.IsSelected = filter.HasAllFlags(tierFilterTypeVM.FilterType);
			}
			foreach (CraftingPieceListVM craftingPieceListVM in this.PieceLists)
			{
				foreach (CraftingPieceVM craftingPieceVM in craftingPieceListVM.Pieces)
				{
					bool flag = list.Contains(craftingPieceVM.CraftingPiece.CraftingPiece.PieceTier);
					bool flag2 = this.ShowOnlyUnlockedPieces && !craftingPieceVM.PlayerHasPiece;
					craftingPieceVM.IsFilteredOut = !flag || flag2;
				}
			}
			this._currentTierFilter = filter;
		}

		// Token: 0x06001554 RID: 5460 RVA: 0x0004FD9C File Offset: 0x0004DF9C
		private void OnNewPieceUnlocked(CraftingPiece piece)
		{
			if (piece.IsValid && !piece.IsHiddenOnDesigner)
			{
				this.SetPieceNewlyUnlocked(piece);
				CraftingPieceVM craftingPieceVM;
				if (this._pieceVMs.TryGetValue(piece, out craftingPieceVM))
				{
					craftingPieceVM.PlayerHasPiece = true;
					craftingPieceVM.IsNewlyUnlocked = true;
				}
			}
		}

		// Token: 0x06001555 RID: 5461 RVA: 0x0004FDE0 File Offset: 0x0004DFE0
		private int GetUnlockedPartsCount(CraftingTemplate template)
		{
			return template.Pieces.Count((CraftingPiece piece) => this._craftingBehavior.IsOpened(piece, template) && !string.IsNullOrEmpty(piece.MeshName));
		}

		// Token: 0x06001556 RID: 5462 RVA: 0x0004FE1D File Offset: 0x0004E01D
		private WeaponClassVM GetCurrentWeaponClass()
		{
			if (this._selectedWeaponClassIndex >= 0 && this._selectedWeaponClassIndex < this.WeaponClassSelectionPopup.WeaponClasses.Count)
			{
				return this.WeaponClassSelectionPopup.WeaponClasses[this._selectedWeaponClassIndex];
			}
			return null;
		}

		// Token: 0x06001557 RID: 5463 RVA: 0x0004FE58 File Offset: 0x0004E058
		private void OnSelectItemFromHistory(WeaponDesignSelectorVM selector)
		{
			WeaponDesign design = selector.Design;
			if (design == null)
			{
				Debug.FailedAssert("History design returned null", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Crafting\\WeaponDesign\\WeaponDesignVM.cs", "OnSelectItemFromHistory", 280);
				return;
			}
			ValueTuple<CraftingPiece, int>[] array = new ValueTuple<CraftingPiece, int>[design.UsedPieces.Length];
			for (int i = 0; i < design.UsedPieces.Length; i++)
			{
				array[i] = new ValueTuple<CraftingPiece, int>(design.UsedPieces[i].CraftingPiece, design.UsedPieces[i].ScalePercentage);
			}
			this.SetDesignManually(design.Template, array, true);
		}

		// Token: 0x06001558 RID: 5464 RVA: 0x0004FEE4 File Offset: 0x0004E0E4
		public void SetPieceNewlyUnlocked(CraftingPiece piece)
		{
			if (!this._newlyUnlockedPieces.Contains(piece))
			{
				this._newlyUnlockedPieces.Add(piece);
			}
		}

		// Token: 0x06001559 RID: 5465 RVA: 0x0004FF00 File Offset: 0x0004E100
		private void UnsetPieceNewlyUnlocked(CraftingPieceVM pieceVM)
		{
			CraftingPiece craftingPiece = pieceVM.CraftingPiece.CraftingPiece;
			if (this._newlyUnlockedPieces.Contains(craftingPiece))
			{
				this._newlyUnlockedPieces.Remove(craftingPiece);
				pieceVM.IsNewlyUnlocked = false;
			}
		}

		// Token: 0x0600155A RID: 5466 RVA: 0x0004FF3B File Offset: 0x0004E13B
		private void OnSelectPieceTierFilter(WeaponDesignVM.CraftingPieceTierFilter filter)
		{
			if (this._currentTierFilter != filter)
			{
				this.FilterPieces(filter);
			}
		}

		// Token: 0x0600155B RID: 5467 RVA: 0x0004FF50 File Offset: 0x0004E150
		private void OnSelectPieceType(CraftingPiece.PieceTypes pieceType)
		{
			CraftingPieceListVM craftingPieceListVM = this.PieceLists.ElementAt(this.SelectedPieceTypeIndex);
			if (craftingPieceListVM != null)
			{
				foreach (CraftingPieceVM craftingPieceVM in craftingPieceListVM.Pieces)
				{
					if (craftingPieceVM.IsNewlyUnlocked)
					{
						this.UnsetPieceNewlyUnlocked(craftingPieceVM);
					}
				}
			}
			CraftingPieceListVM craftingPieceListVM2 = this.PieceLists.FirstOrDefault((CraftingPieceListVM x) => x.PieceType == pieceType);
			foreach (CraftingPieceListVM craftingPieceListVM3 in this.PieceLists)
			{
				craftingPieceListVM3.Refresh();
				craftingPieceListVM3.IsSelected = craftingPieceListVM3 == craftingPieceListVM2;
			}
			this.SelectedPieceTypeIndex = (int)pieceType;
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x00050038 File Offset: 0x0004E238
		private void SelectDefaultPiecesForCurrentTemplate()
		{
			CraftingOrderItemVM activeCraftingOrder = this.ActiveCraftingOrder;
			string text = ((activeCraftingOrder != null) ? activeCraftingOrder.CraftingOrder.GetStatWeapon().WeaponDescriptionId : null);
			WeaponDescription statWeaponUsage = ((text != null) ? MBObjectManager.Instance.GetObject<WeaponDescription>(text) : null);
			WeaponClassVM currentWeaponClass = this.GetCurrentWeaponClass();
			this._shouldRecordHistory = false;
			Func<CraftingPieceVM, bool> <>9__3;
			foreach (CraftingPieceListVM craftingPieceListVM in this.PieceLists)
			{
				if (this._crafting.CurrentCraftingTemplate.IsPieceTypeUsable(craftingPieceListVM.PieceType))
				{
					CraftingPieceVM craftingPieceVM = null;
					if (this.IsInFreeMode && currentWeaponClass != null)
					{
						string selectedPieceID = currentWeaponClass.GetSelectedPieceData(craftingPieceListVM.PieceType);
						craftingPieceVM = craftingPieceListVM.Pieces.FirstOrDefault((CraftingPieceVM p) => p.CraftingPiece.CraftingPiece.StringId == selectedPieceID);
					}
					if (craftingPieceVM == null)
					{
						IOrderedEnumerable<CraftingPieceVM> orderedEnumerable = from p in craftingPieceListVM.Pieces
							orderby p.PlayerHasPiece descending, p.IsNewlyUnlocked descending
							select p;
						Func<CraftingPieceVM, bool> func;
						if ((func = <>9__3) == null)
						{
							func = (<>9__3 = (CraftingPieceVM p) => statWeaponUsage == null || statWeaponUsage.AvailablePieces.Any((CraftingPiece x) => x.StringId == p.CraftingPiece.CraftingPiece.StringId));
						}
						craftingPieceVM = orderedEnumerable.ThenByDescending(func).FirstOrDefault<CraftingPieceVM>();
					}
					if (craftingPieceVM != null)
					{
						craftingPieceVM.ExecuteSelect();
					}
				}
			}
			this._shouldRecordHistory = true;
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x000501CC File Offset: 0x0004E3CC
		private void InitializeDefaultFromLogic()
		{
			this.PrimaryPropertyList.Clear();
			this.BladePieceList.Pieces.Clear();
			this.GuardPieceList.Pieces.Clear();
			this.HandlePieceList.Pieces.Clear();
			this.PommelPieceList.Pieces.Clear();
			this.SelectedBladePiece = new CraftingPieceVM();
			this.SelectedGuardPiece = new CraftingPieceVM();
			this.SelectedHandlePiece = new CraftingPieceVM();
			this.SelectedPommelPiece = new CraftingPieceVM();
			this._pieceVMs.Clear();
			bool flag = Campaign.Current.GameMode == CampaignGameMode.Tutorial;
			foreach (CraftingPieceListVM craftingPieceListVM in this.PieceLists)
			{
				if (this._crafting.CurrentCraftingTemplate.IsPieceTypeUsable(craftingPieceListVM.PieceType))
				{
					int pieceType = (int)craftingPieceListVM.PieceType;
					for (int i = 0; i < this._crafting.UsablePiecesList[pieceType].Count; i++)
					{
						WeaponDesignElement weaponDesignElement = this._crafting.UsablePiecesList[pieceType][i];
						if (flag || !weaponDesignElement.CraftingPiece.IsHiddenOnDesigner)
						{
							bool flag2 = this._craftingBehavior.IsOpened(weaponDesignElement.CraftingPiece, this._crafting.CurrentCraftingTemplate);
							CraftingPieceVM craftingPieceVM = new CraftingPieceVM(new Action<CraftingPieceVM>(this.OnSetItemPieceManually), this._crafting.CurrentCraftingTemplate.StringId, this._crafting.UsablePiecesList[pieceType][i], pieceType, i, flag2);
							craftingPieceListVM.Pieces.Add(craftingPieceVM);
							craftingPieceVM.IsNewlyUnlocked = flag2 && this._newlyUnlockedPieces.Contains(weaponDesignElement.CraftingPiece);
							if (this._crafting.SelectedPieces[pieceType].CraftingPiece == craftingPieceVM.CraftingPiece.CraftingPiece)
							{
								craftingPieceListVM.SelectedPiece = craftingPieceVM;
								craftingPieceVM.IsSelected = true;
							}
							this._pieceVMs.Add(this._crafting.UsablePiecesList[pieceType][i].CraftingPiece, craftingPieceVM);
						}
					}
					craftingPieceListVM.Pieces.Sort(this._pieceTierComparer);
				}
			}
			CraftingPieceListVM craftingPieceListVM2 = this.PieceLists.FirstOrDefault((CraftingPieceListVM x) => x.PieceType == CraftingPiece.PieceTypes.Blade);
			this.SelectedBladePiece = ((craftingPieceListVM2 != null) ? craftingPieceListVM2.SelectedPiece : null);
			CraftingPieceListVM craftingPieceListVM3 = this.PieceLists.FirstOrDefault((CraftingPieceListVM x) => x.PieceType == CraftingPiece.PieceTypes.Guard);
			this.SelectedGuardPiece = ((craftingPieceListVM3 != null) ? craftingPieceListVM3.SelectedPiece : null);
			CraftingPieceListVM craftingPieceListVM4 = this.PieceLists.FirstOrDefault((CraftingPieceListVM x) => x.PieceType == CraftingPiece.PieceTypes.Handle);
			this.SelectedHandlePiece = ((craftingPieceListVM4 != null) ? craftingPieceListVM4.SelectedPiece : null);
			CraftingPieceListVM craftingPieceListVM5 = this.PieceLists.FirstOrDefault((CraftingPieceListVM x) => x.PieceType == CraftingPiece.PieceTypes.Pommel);
			this.SelectedPommelPiece = ((craftingPieceListVM5 != null) ? craftingPieceListVM5.SelectedPiece : null);
			this.ItemName = this._crafting.CraftedWeaponName.ToString();
			this.PommelSize = 0;
			this.GuardSize = 0;
			this.HandleSize = 0;
			this.BladeSize = 0;
			this.RefreshPieceFlags();
			this.RefreshItem();
			this.RefreshAlternativeUsageList();
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x0005054C File Offset: 0x0004E74C
		private void RefreshPieceFlags()
		{
			foreach (CraftingPieceListVM craftingPieceListVM in this.PieceLists)
			{
				craftingPieceListVM.IsEnabled = this._crafting.CurrentCraftingTemplate.IsPieceTypeUsable(craftingPieceListVM.PieceType);
				foreach (CraftingPieceVM craftingPieceVM in craftingPieceListVM.Pieces)
				{
					craftingPieceVM.RefreshFlagIcons();
					if (craftingPieceListVM.PieceType == CraftingPiece.PieceTypes.Blade)
					{
						this.AddClassFlagsToPiece(craftingPieceVM);
					}
				}
			}
			this.RefreshWeaponFlags();
		}

		// Token: 0x0600155F RID: 5471 RVA: 0x00050600 File Offset: 0x0004E800
		private void AddClassFlagsToPiece(CraftingPieceVM piece)
		{
			WeaponComponentData weaponWithUsageIndex = this._crafting.GetCurrentCraftedItemObject(false, null).GetWeaponWithUsageIndex(this.SecondaryUsageSelector.SelectedIndex);
			int indexOfUsageDataWithId = this._crafting.CurrentCraftingTemplate.GetIndexOfUsageDataWithId(weaponWithUsageIndex.WeaponDescriptionId);
			WeaponDescription weaponDescription = this._crafting.CurrentCraftingTemplate.WeaponDescriptions.ElementAtOrDefault(indexOfUsageDataWithId);
			if (weaponDescription != null)
			{
				using (List<ValueTuple<string, TextObject>>.Enumerator enumerator = CampaignUIHelper.GetWeaponFlagDetails(weaponDescription.WeaponFlags, null).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ValueTuple<string, TextObject> flagPath = enumerator.Current;
						if (!piece.ItemAttributeIcons.Any((CraftingItemFlagVM x) => x.Icon.Contains(flagPath.Item1)))
						{
							piece.ItemAttributeIcons.Add(new CraftingItemFlagVM(flagPath.Item1, flagPath.Item2, false));
						}
					}
				}
			}
			using (List<ValueTuple<string, TextObject>>.Enumerator enumerator = CampaignUIHelper.GetFlagDetailsForWeapon(weaponWithUsageIndex, this._getItemUsageSetFlags(weaponWithUsageIndex), null).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ValueTuple<string, TextObject> usageFlag = enumerator.Current;
					if (!piece.ItemAttributeIcons.Any((CraftingItemFlagVM x) => x.Icon.Contains(usageFlag.Item1)))
					{
						piece.ItemAttributeIcons.Add(new CraftingItemFlagVM(usageFlag.Item1, usageFlag.Item2, false));
					}
				}
			}
		}

		// Token: 0x06001560 RID: 5472 RVA: 0x0005078C File Offset: 0x0004E98C
		private void UpdateSecondaryUsageIndex(SelectorVM<CraftingSecondaryUsageItemVM> selector)
		{
			if (selector.SelectedIndex != -1)
			{
				this.RefreshStats();
				this.RefreshPieceFlags();
			}
		}

		// Token: 0x06001561 RID: 5473 RVA: 0x000507A4 File Offset: 0x0004E9A4
		private void UpdateResultPropertyList()
		{
			this.DesignResultPropertyList.Clear();
			int num = this.SecondaryUsageSelector.SelectedIndex;
			this.TrySetSecondaryUsageIndex(num);
			this.RefreshStats();
			if (this.IsInOrderMode)
			{
				WeaponComponentData orderWeapon = this.ActiveCraftingOrder.CraftingOrder.GetStatWeapon();
				num = this._crafting.GetCurrentCraftedItemObject(false, null).Weapons.FindIndex((WeaponComponentData x) => x.WeaponDescriptionId == orderWeapon.WeaponDescriptionId);
			}
			foreach (CraftingListPropertyItem craftingListPropertyItem in this.PrimaryPropertyList)
			{
				float num2 = 0f;
				bool flag = false;
				if (craftingListPropertyItem.Type == CraftingTemplate.CraftingStatTypes.Weight)
				{
					num2 = this.OverridenData.WeightOverriden;
					flag = true;
				}
				else if (craftingListPropertyItem.Type == CraftingTemplate.CraftingStatTypes.SwingDamage)
				{
					num2 = (float)this.OverridenData.SwingDamageOverriden;
				}
				else if (craftingListPropertyItem.Type == CraftingTemplate.CraftingStatTypes.SwingSpeed)
				{
					num2 = (float)this.OverridenData.SwingSpeedOverriden;
				}
				else if (craftingListPropertyItem.Type == CraftingTemplate.CraftingStatTypes.ThrustDamage)
				{
					num2 = (float)this.OverridenData.ThrustDamageOverriden;
				}
				else if (craftingListPropertyItem.Type == CraftingTemplate.CraftingStatTypes.ThrustSpeed)
				{
					num2 = (float)this.OverridenData.ThrustSpeedOverriden;
				}
				else if (craftingListPropertyItem.Type == CraftingTemplate.CraftingStatTypes.Handling)
				{
					num2 = (float)this.OverridenData.Handling;
				}
				if (this.IsInOrderMode)
				{
					this.DesignResultPropertyList.Add(new WeaponDesignResultPropertyItemVM(craftingListPropertyItem.Description, craftingListPropertyItem.PropertyValue, craftingListPropertyItem.TargetValue, num2, flag, craftingListPropertyItem.IsExceedingBeneficial, true));
				}
				else
				{
					this.DesignResultPropertyList.Add(new WeaponDesignResultPropertyItemVM(craftingListPropertyItem.Description, craftingListPropertyItem.PropertyValue, num2, flag));
				}
			}
		}

		// Token: 0x06001562 RID: 5474 RVA: 0x00050960 File Offset: 0x0004EB60
		public void SelectPrimaryWeaponClass(CraftingTemplate template)
		{
			int num = this._primaryUsages.IndexOf(template);
			this._selectedWeaponClassIndex = num;
			if (this._crafting.CurrentCraftingTemplate != template)
			{
				CraftingHelper.ChangeCurrentCraftingTemplate(template);
				return;
			}
			this.AddHistoryKey();
		}

		// Token: 0x06001563 RID: 5475 RVA: 0x0005099C File Offset: 0x0004EB9C
		private void RefreshWeaponDesignMode(CraftingOrderItemVM orderToSelect, int classIndex = -1, bool doNotAutoSelectPieces = false)
		{
			bool flag = false;
			CraftingTemplate selectedCraftingTemplate = null;
			this.SecondaryUsageSelector.SelectedIndex = 0;
			if (orderToSelect != null)
			{
				this.IsInOrderMode = true;
				this.ActiveCraftingOrder = orderToSelect;
				selectedCraftingTemplate = orderToSelect.CraftingOrder.PreCraftedWeaponDesignItem.WeaponDesign.Template;
				this.SelectPrimaryWeaponClass(selectedCraftingTemplate);
				flag = true;
			}
			else
			{
				this.IsInOrderMode = false;
				this.ActiveCraftingOrder = null;
				if (classIndex >= 0)
				{
					selectedCraftingTemplate = this._primaryUsages[classIndex];
					this.SelectPrimaryWeaponClass(selectedCraftingTemplate);
					flag = true;
				}
			}
			WeaponClassVM weaponClassVM = this.WeaponClassSelectionPopup.WeaponClasses.FirstOrDefault((WeaponClassVM x) => x.Template == selectedCraftingTemplate);
			if (weaponClassVM != null)
			{
				weaponClassVM.NewlyUnlockedPieceCount = 0;
			}
			this.CraftingOrderPopup.RefreshOrders();
			this.CraftingHistory.RefreshAvailability();
			this.IsOrderButtonActive = this.CraftingOrderPopup.HasOrders;
			Action onRefresh = this._onRefresh;
			if (onRefresh != null)
			{
				onRefresh();
			}
			Action<CraftingOrder> refreshHeroAvailabilities = this._refreshHeroAvailabilities;
			if (refreshHeroAvailabilities != null)
			{
				CraftingOrderItemVM activeCraftingOrder = this.ActiveCraftingOrder;
				refreshHeroAvailabilities((activeCraftingOrder != null) ? activeCraftingOrder.CraftingOrder : null);
			}
			if (!flag)
			{
				this.InitializeDefaultFromLogic();
			}
			this.RefreshValues();
			this.RefreshItem();
			this.OnSelectPieceType(CraftingPiece.PieceTypes.Blade);
			this.FilterPieces(this._currentTierFilter);
			this.RefreshCurrentHeroSkillLevel();
			if (!doNotAutoSelectPieces)
			{
				this.SelectDefaultPiecesForCurrentTemplate();
			}
		}

		// Token: 0x06001564 RID: 5476 RVA: 0x00050AEB File Offset: 0x0004ECEB
		private void OnCraftingOrderSelected(CraftingOrderItemVM selectedOrder)
		{
			this.RefreshWeaponDesignMode(selectedOrder, -1, false);
		}

		// Token: 0x06001565 RID: 5477 RVA: 0x00050AF8 File Offset: 0x0004ECF8
		public void ExecuteOpenOrderPopup()
		{
			this.CraftingOrderPopup.ExecuteOpenPopup();
			MBBindingList<CraftingOrderItemVM> craftingOrders = this.CraftingOrderPopup.CraftingOrders;
			CraftingOrderItemVM craftingOrderItemVM = ((craftingOrders != null) ? craftingOrders.FirstOrDefault(delegate(CraftingOrderItemVM x)
			{
				CraftingOrder craftingOrder = x.CraftingOrder;
				CraftingOrderItemVM activeCraftingOrder = this.ActiveCraftingOrder;
				return craftingOrder == ((activeCraftingOrder != null) ? activeCraftingOrder.CraftingOrder : null);
			}) : null);
			if (craftingOrderItemVM != null)
			{
				craftingOrderItemVM.IsSelected = true;
			}
		}

		// Token: 0x06001566 RID: 5478 RVA: 0x00050B3E File Offset: 0x0004ED3E
		public void ExecuteCloseOrderPopup()
		{
			this.CraftingOrderPopup.IsVisible = false;
		}

		// Token: 0x06001567 RID: 5479 RVA: 0x00050B4C File Offset: 0x0004ED4C
		public void ExecuteOpenWeaponClassSelectionPopup()
		{
			this.WeaponClassSelectionPopup.UpdateNewlyUnlockedPiecesCount(this._newlyUnlockedPieces);
			this.WeaponClassSelectionPopup.ExecuteOpenPopup();
			if (this.IsInFreeMode)
			{
				this.WeaponClassSelectionPopup.WeaponClasses.ApplyActionOnAllItems(delegate(WeaponClassVM x)
				{
					x.IsSelected = x.SelectionIndex == this._selectedWeaponClassIndex;
				});
				return;
			}
			this.WeaponClassSelectionPopup.WeaponClasses.ApplyActionOnAllItems(delegate(WeaponClassVM x)
			{
				x.IsSelected = false;
			});
		}

		// Token: 0x06001568 RID: 5480 RVA: 0x00050BCC File Offset: 0x0004EDCC
		public void CreateCraftingResultPopup()
		{
			WeaponDesignResultPopupVM craftingResultPopup = this.CraftingResultPopup;
			if (craftingResultPopup != null)
			{
				craftingResultPopup.OnFinalize();
			}
			Action action = new Action(this.ExecuteFinalizeCrafting);
			Crafting crafting = this._crafting;
			CraftingOrderItemVM activeCraftingOrder = this.ActiveCraftingOrder;
			this.CraftingResultPopup = new WeaponDesignResultPopupVM(action, crafting, (activeCraftingOrder != null) ? activeCraftingOrder.CraftingOrder : null, this.WeaponFlagIconsList, this.CraftedItemObject, this._designResultPropertyList, this._itemName, this._craftedItemVisual);
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x00050C37 File Offset: 0x0004EE37
		public void ExecuteToggleShowOnlyUnlockedPieces()
		{
			this.ShowOnlyUnlockedPieces = !this.ShowOnlyUnlockedPieces;
			this.FilterPieces(this._currentTierFilter);
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x00050C54 File Offset: 0x0004EE54
		public void ExecuteUndo()
		{
			if (this._crafting.Undo())
			{
				Action onRefresh = this._onRefresh;
				if (onRefresh != null)
				{
					onRefresh();
				}
				this._updatePiece = false;
				int i2;
				int i;
				for (i = 0; i < 4; i = i2 + 1)
				{
					CraftingPiece.PieceTypes j = (CraftingPiece.PieceTypes)i;
					if (this._crafting.CurrentCraftingTemplate.IsPieceTypeUsable(j))
					{
						CraftingPieceVM craftingPieceVM = this._pieceListsDictionary[j].Pieces.First((CraftingPieceVM piece) => piece.CraftingPiece.CraftingPiece == this._crafting.SelectedPieces[i].CraftingPiece);
						this.OnSetItemPiece(craftingPieceVM, 0, true, false);
					}
					i2 = i;
				}
				this.RefreshItem();
				this._updatePiece = true;
			}
		}

		// Token: 0x0600156B RID: 5483 RVA: 0x00050D0C File Offset: 0x0004EF0C
		public void ExecuteRedo()
		{
			if (this._crafting.Redo())
			{
				Action onRefresh = this._onRefresh;
				if (onRefresh != null)
				{
					onRefresh();
				}
				this._updatePiece = false;
				int i2;
				int i;
				for (i = 0; i < 4; i = i2 + 1)
				{
					CraftingPiece.PieceTypes j = (CraftingPiece.PieceTypes)i;
					if (this._crafting.CurrentCraftingTemplate.IsPieceTypeUsable(j))
					{
						CraftingPieceVM craftingPieceVM = this._pieceListsDictionary[j].Pieces.First((CraftingPieceVM piece) => piece.CraftingPiece.CraftingPiece == this._crafting.SelectedPieces[i].CraftingPiece);
						this.OnSetItemPiece(craftingPieceVM, 0, true, false);
					}
					i2 = i;
				}
				this.RefreshItem();
				this._updatePiece = true;
			}
		}

		// Token: 0x0600156C RID: 5484 RVA: 0x00050DC4 File Offset: 0x0004EFC4
		internal void OnCraftingHeroChanged(CraftingAvailableHeroItemVM newHero)
		{
			this.RefreshCurrentHeroSkillLevel();
			this.RefreshDifficulty();
			this.CraftingOrderPopup.RefreshOrders();
		}

		// Token: 0x0600156D RID: 5485 RVA: 0x00050DE0 File Offset: 0x0004EFE0
		public void ChangeModeIfHeroIsUnavailable()
		{
			CraftingAvailableHeroItemVM craftingAvailableHeroItemVM = this._getCurrentCraftingHero();
			if (this.IsInOrderMode && craftingAvailableHeroItemVM.IsDisabled)
			{
				this.RefreshWeaponDesignMode(null, -1, false);
			}
		}

		// Token: 0x0600156E RID: 5486 RVA: 0x00050E14 File Offset: 0x0004F014
		public void ExecuteRandomize()
		{
			for (int i = 0; i < 4; i++)
			{
				CraftingPiece.PieceTypes pieceTypes = (CraftingPiece.PieceTypes)i;
				if (this._crafting.CurrentCraftingTemplate.IsPieceTypeUsable(pieceTypes))
				{
					CraftingPieceVM randomElementWithPredicate = this._pieceListsDictionary[pieceTypes].Pieces.GetRandomElementWithPredicate((CraftingPieceVM p) => p.PlayerHasPiece);
					if (randomElementWithPredicate != null)
					{
						this.OnSetItemPiece(randomElementWithPredicate, (int)(90f + MBRandom.RandomFloat * 20f), false, true);
					}
				}
			}
			this._updatePiece = false;
			this.RefreshItem();
			this.AddHistoryKey();
			this._updatePiece = true;
		}

		// Token: 0x0600156F RID: 5487 RVA: 0x00050EB0 File Offset: 0x0004F0B0
		public void ExecuteChangeScabbardVisibility()
		{
			if (!this._crafting.CurrentCraftingTemplate.UseWeaponAsHolsterMesh)
			{
				this.IsScabbardVisible = !this.IsScabbardVisible;
			}
		}

		// Token: 0x06001570 RID: 5488 RVA: 0x00050ED4 File Offset: 0x0004F0D4
		public void SelectWeapon(ItemObject itemObject)
		{
			this._crafting.SwitchToCraftedItem(itemObject);
			Action onRefresh = this._onRefresh;
			if (onRefresh != null)
			{
				onRefresh();
			}
			this._updatePiece = false;
			int i;
			int i2;
			for (i = 0; i < 4; i = i2 + 1)
			{
				CraftingPiece.PieceTypes j = (CraftingPiece.PieceTypes)i;
				if (this._crafting.CurrentCraftingTemplate.IsPieceTypeUsable(j))
				{
					CraftingPieceVM craftingPieceVM = this._pieceListsDictionary[j].Pieces.First((CraftingPieceVM piece) => piece.CraftingPiece.CraftingPiece == this._crafting.CurrentWeaponDesign.UsedPieces[i].CraftingPiece);
					this.OnSetItemPiece(craftingPieceVM, this._crafting.CurrentWeaponDesign.UsedPieces[i].ScalePercentage, true, false);
				}
				i2 = i;
			}
			this.RefreshItem();
			this.AddHistoryKey();
			this._updatePiece = true;
		}

		// Token: 0x06001571 RID: 5489 RVA: 0x00050FAC File Offset: 0x0004F1AC
		public bool CanCompleteOrder()
		{
			bool flag = true;
			if (this.IsInOrderMode)
			{
				ItemObject currentCraftedItemObject = this._crafting.GetCurrentCraftedItemObject(false, null);
				flag = this.ActiveCraftingOrder.CraftingOrder.CanHeroCompleteOrder(this._getCurrentCraftingHero().Hero, currentCraftedItemObject);
			}
			return flag;
		}

		// Token: 0x06001572 RID: 5490 RVA: 0x00050FF4 File Offset: 0x0004F1F4
		public void ExecuteFinalizeCrafting()
		{
			if (this._craftingBehavior != null && Campaign.Current.GameMode == CampaignGameMode.Campaign)
			{
				if (GameStateManager.Current.ActiveState is CraftingState)
				{
					if (this.IsInOrderMode)
					{
						this._craftingBehavior.CompleteOrder(Settlement.CurrentSettlement.Town, this.ActiveCraftingOrder.CraftingOrder, this.CraftedItemObject, this._getCurrentCraftingHero().Hero);
						this.CraftedItemObject = null;
						this.RefreshWeaponDesignMode(null, this._selectedWeaponClassIndex, false);
					}
					else
					{
						int bladeSize = this.BladeSize;
						int guardSize = this.GuardSize;
						int handleSize = this.HandleSize;
						int pommelSize = this.PommelSize;
						ICraftingCampaignBehavior craftingBehavior = this._craftingBehavior;
						Func<CraftingAvailableHeroItemVM> getCurrentCraftingHero = this._getCurrentCraftingHero;
						Hero hero;
						if (getCurrentCraftingHero == null)
						{
							hero = null;
						}
						else
						{
							CraftingAvailableHeroItemVM craftingAvailableHeroItemVM = getCurrentCraftingHero();
							hero = ((craftingAvailableHeroItemVM != null) ? craftingAvailableHeroItemVM.Hero : null);
						}
						craftingBehavior.CreateCraftedWeaponInFreeBuildMode(hero, this._crafting.CurrentWeaponDesign, this.ModifierTier, this.OverridenData);
						this.RefreshWeaponDesignMode(null, this._selectedWeaponClassIndex, false);
						this.BladeSize = bladeSize;
						this.GuardSize = guardSize;
						this.HandleSize = handleSize;
						this.PommelSize = pommelSize;
					}
					this.UnregisterTempItemObject();
				}
				this.IsInFinalCraftingStage = false;
			}
		}

		// Token: 0x06001573 RID: 5491 RVA: 0x0005111C File Offset: 0x0004F31C
		public void RegisterTempItemObject()
		{
			this._tempItemObjectForVisual = this._crafting.GetCurrentCraftedItemObject(true, null);
			this._tempItemObjectForVisual.StringId = this._tempItemObjectForVisual.WeaponDesign.HashedCode;
			MBObjectManager.Instance.RegisterObject<ItemObject>(this._tempItemObjectForVisual);
			if (MBObjectManager.Instance.GetObject<ItemObject>(this._tempItemObjectForVisual.StringId) == null)
			{
				MBObjectManager.Instance.RegisterObject<ItemObject>(this._tempItemObjectForVisual);
				return;
			}
			this.CraftedItemVisual.StringId = this._tempItemObjectForVisual.StringId;
			this.IsWeaponCivilian = this._tempItemObjectForVisual.IsCivilian;
		}

		// Token: 0x06001574 RID: 5492 RVA: 0x000511B8 File Offset: 0x0004F3B8
		public void UnregisterTempItemObject()
		{
			if (this._tempItemObjectForVisual != null)
			{
				MBObjectManager.Instance.UnregisterObject(this._tempItemObjectForVisual);
			}
		}

		// Token: 0x06001575 RID: 5493 RVA: 0x000511D2 File Offset: 0x0004F3D2
		private bool DoesCurrentItemHaveSecondaryUsage(int usageIndex)
		{
			return usageIndex >= 0 && usageIndex < this._crafting.GetCurrentCraftedItemObject(false, null).Weapons.Count;
		}

		// Token: 0x06001576 RID: 5494 RVA: 0x000511F4 File Offset: 0x0004F3F4
		private void TrySetSecondaryUsageIndex(int usageIndex)
		{
			int num = 0;
			if (this.DoesCurrentItemHaveSecondaryUsage(usageIndex))
			{
				CraftingSecondaryUsageItemVM craftingSecondaryUsageItemVM = this.SecondaryUsageSelector.ItemList.FirstOrDefault((CraftingSecondaryUsageItemVM x) => x.UsageIndex == usageIndex);
				if (craftingSecondaryUsageItemVM != null)
				{
					num = craftingSecondaryUsageItemVM.SelectorIndex;
				}
			}
			if (num >= 0 && num < this.SecondaryUsageSelector.ItemList.Count)
			{
				this.SecondaryUsageSelector.SelectedIndex = num;
				this.SecondaryUsageSelector.ItemList[num].IsSelected = true;
			}
		}

		// Token: 0x06001577 RID: 5495 RVA: 0x00051280 File Offset: 0x0004F480
		private void RefreshAlternativeUsageList()
		{
			int num = this.SecondaryUsageSelector.SelectedIndex;
			this.SecondaryUsageSelector.Refresh(new List<string>(), 0, new Action<SelectorVM<CraftingSecondaryUsageItemVM>>(this.UpdateSecondaryUsageIndex));
			MBReadOnlyList<WeaponComponentData> weapons = this._crafting.GetCurrentCraftedItemObject(false, null).Weapons;
			int num2 = 0;
			for (int i = 0; i < weapons.Count; i++)
			{
				if (CampaignUIHelper.IsItemUsageApplicable(weapons[i]))
				{
					TextObject textObject = GameTexts.FindText("str_weapon_usage", weapons[i].WeaponDescriptionId);
					this.SecondaryUsageSelector.AddItem(new CraftingSecondaryUsageItemVM(textObject, num2, i, this.SecondaryUsageSelector));
					CraftingOrderItemVM activeCraftingOrder = this.ActiveCraftingOrder;
					if (((activeCraftingOrder != null) ? activeCraftingOrder.CraftingOrder.GetStatWeapon().WeaponDescriptionId : null) == weapons[i].WeaponDescriptionId)
					{
						num = num2;
					}
					num2++;
				}
			}
			this.TrySetSecondaryUsageIndex(num);
		}

		// Token: 0x06001578 RID: 5496 RVA: 0x0005135C File Offset: 0x0004F55C
		private void RefreshStats()
		{
			if (!this.DoesCurrentItemHaveSecondaryUsage(this.SecondaryUsageSelector.SelectedIndex))
			{
				this.TrySetSecondaryUsageIndex(0);
			}
			List<CraftingStatData> list = this._crafting.GetStatDatas(this.SecondaryUsageSelector.SelectedIndex).ToList<CraftingStatData>();
			WeaponComponentData weaponComponentData = (this.IsInOrderMode ? this.ActiveCraftingOrder.CraftingOrder.GetStatWeapon() : null);
			IEnumerable<CraftingStatData> enumerable = (this.IsInOrderMode ? this.GetOrderStatDatas(this.ActiveCraftingOrder.CraftingOrder) : null);
			ItemObject currentCraftedItemObject = this._crafting.GetCurrentCraftedItemObject(false, null);
			WeaponComponentData weaponWithUsageIndex = currentCraftedItemObject.GetWeaponWithUsageIndex(this.SecondaryUsageSelector.SelectedIndex);
			bool flag = weaponComponentData == null || weaponComponentData.WeaponDescriptionId == weaponWithUsageIndex.WeaponDescriptionId;
			if (enumerable != null)
			{
				using (IEnumerator<CraftingStatData> enumerator = enumerable.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CraftingStatData orderStatData = enumerator.Current;
						if (!list.Any((CraftingStatData x) => x.Type == orderStatData.Type && x.DamageType == orderStatData.DamageType))
						{
							if ((orderStatData.Type == CraftingTemplate.CraftingStatTypes.SwingDamage && orderStatData.DamageType != weaponWithUsageIndex.SwingDamageType) || (orderStatData.Type == CraftingTemplate.CraftingStatTypes.ThrustDamage && orderStatData.DamageType != weaponWithUsageIndex.ThrustDamageType))
							{
								list.Add(new CraftingStatData(orderStatData.DescriptionText, 0f, orderStatData.MaxValue, orderStatData.Type, orderStatData.DamageType));
							}
							else
							{
								list.Add(orderStatData);
							}
						}
					}
				}
			}
			this.PrimaryPropertyList.Clear();
			using (List<CraftingStatData>.Enumerator enumerator2 = list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					CraftingStatData statData = enumerator2.Current;
					if (statData.IsValid)
					{
						float num = 0f;
						if (this.IsInOrderMode && flag)
						{
							WeaponAttributeVM weaponAttributeVM = this.ActiveCraftingOrder.WeaponAttributes.FirstOrDefault((WeaponAttributeVM x) => x.AttributeType == statData.Type && x.DamageType == statData.DamageType);
							num = ((weaponAttributeVM != null) ? weaponAttributeVM.AttributeValue : 0f);
						}
						float num2 = MathF.Max(statData.MaxValue, num);
						CraftingListPropertyItem craftingListPropertyItem = new CraftingListPropertyItem(statData.DescriptionText, num2, statData.CurValue, num, statData.Type, false);
						this.PrimaryPropertyList.Add(craftingListPropertyItem);
						craftingListPropertyItem.IsValidForUsage = true;
					}
				}
			}
			this.PrimaryPropertyList.Sort(new WeaponDesignVM.WeaponPropertyComparer());
			CraftingOrderItemVM activeCraftingOrder = this.ActiveCraftingOrder;
			this.MissingPropertyWarningText = CampaignUIHelper.GetCraftingOrderMissingPropertyWarningText((activeCraftingOrder != null) ? activeCraftingOrder.CraftingOrder : null, currentCraftedItemObject);
		}

		// Token: 0x06001579 RID: 5497 RVA: 0x00051644 File Offset: 0x0004F844
		private IEnumerable<CraftingStatData> GetOrderStatDatas(CraftingOrder order)
		{
			if (order == null)
			{
				return null;
			}
			WeaponComponentData weaponComponentData;
			return order.GetStatDataForItem(order.PreCraftedWeaponDesignItem, out weaponComponentData);
		}

		// Token: 0x0600157A RID: 5498 RVA: 0x00051664 File Offset: 0x0004F864
		private void RefreshWeaponFlags()
		{
			this.WeaponFlagIconsList.Clear();
			foreach (CraftingPieceListVM craftingPieceListVM in this.PieceLists)
			{
				if (craftingPieceListVM.SelectedPiece != null)
				{
					using (IEnumerator<CraftingItemFlagVM> enumerator2 = craftingPieceListVM.SelectedPiece.ItemAttributeIcons.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							CraftingItemFlagVM iconData = enumerator2.Current;
							if (!this.WeaponFlagIconsList.Any((ItemFlagVM x) => x.Icon == iconData.Icon))
							{
								this.WeaponFlagIconsList.Add(iconData);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600157B RID: 5499 RVA: 0x0005172C File Offset: 0x0004F92C
		private void RefreshName()
		{
			this._crafting.SetCraftedWeaponName(this.ItemName);
		}

		// Token: 0x0600157C RID: 5500 RVA: 0x0005173F File Offset: 0x0004F93F
		private void OnSetItemPieceManually(CraftingPieceVM piece)
		{
			this.OnSetItemPiece(piece, 0, true, false);
			this.RefreshItem();
			this.AddHistoryKey();
		}

		// Token: 0x0600157D RID: 5501 RVA: 0x00051758 File Offset: 0x0004F958
		private void OnSetItemPiece(CraftingPieceVM piece, int scalePercentage = 0, bool shouldUpdateWholeWeapon = true, bool forceUpdatePiece = false)
		{
			CraftingPiece.PieceTypes pieceType = (CraftingPiece.PieceTypes)piece.PieceType;
			this._pieceListsDictionary[pieceType].SelectedPiece.IsSelected = false;
			bool updatePiece = this._updatePiece;
			this.UnsetPieceNewlyUnlocked(piece);
			if (updatePiece)
			{
				this._crafting.SwitchToPiece(piece.CraftingPiece);
				if (!forceUpdatePiece)
				{
					this._updatePiece = false;
				}
			}
			piece.IsSelected = true;
			this._pieceListsDictionary[pieceType].SelectedPiece = piece;
			int num = ((scalePercentage != 0) ? scalePercentage : this._crafting.SelectedPieces[(int)pieceType].ScalePercentage) - 100;
			switch (pieceType)
			{
			case CraftingPiece.PieceTypes.Blade:
				this.BladeSize = num;
				this.SelectedBladePiece = piece;
				break;
			case CraftingPiece.PieceTypes.Guard:
				this.GuardSize = num;
				this.SelectedGuardPiece = piece;
				break;
			case CraftingPiece.PieceTypes.Handle:
				this.HandleSize = num;
				this.SelectedHandlePiece = piece;
				break;
			case CraftingPiece.PieceTypes.Pommel:
				this.PommelSize = num;
				this.SelectedPommelPiece = piece;
				break;
			}
			if (this.IsInFreeMode)
			{
				WeaponClassVM currentWeaponClass = this.GetCurrentWeaponClass();
				if (currentWeaponClass != null)
				{
					currentWeaponClass.RegisterSelectedPiece(pieceType, piece.CraftingPiece.CraftingPiece.StringId);
				}
			}
			this._updatePiece = updatePiece;
			this.RefreshAlternativeUsageList();
			if (shouldUpdateWholeWeapon)
			{
				Action onRefresh = this._onRefresh;
				if (onRefresh != null)
				{
					onRefresh();
				}
			}
			this.PieceLists.ApplyActionOnAllItems(delegate(CraftingPieceListVM x)
			{
				x.Refresh();
			});
		}

		// Token: 0x0600157E RID: 5502 RVA: 0x000518AF File Offset: 0x0004FAAF
		public void RefreshItem()
		{
			this.RefreshStats();
			this.RefreshWeaponFlags();
			this.RefreshDifficulty();
			Action onRefresh = this._onRefresh;
			if (onRefresh == null)
			{
				return;
			}
			onRefresh();
		}

		// Token: 0x0600157F RID: 5503 RVA: 0x000518D4 File Offset: 0x0004FAD4
		private void RefreshDifficulty()
		{
			this.CurrentDifficulty = Campaign.Current.Models.SmithingModel.CalculateWeaponDesignDifficulty(this._crafting.CurrentWeaponDesign);
			if (this.IsInOrderMode)
			{
				this.CurrentOrderDifficulty = MathF.Round(this.ActiveCraftingOrder.CraftingOrder.OrderDifficulty);
			}
			this._currentCraftingSkillText.SetTextVariable("SKILL_VALUE", this.CurrentHeroCraftingSkill);
			this._currentCraftingSkillText.SetTextVariable("SKILL_NAME", DefaultSkills.Crafting.Name);
			this.CurrentCraftingSkillValueText = this._currentCraftingSkillText.ToString();
			this.CurrentDifficultyText = this.GetCurrentDifficultyText(this.CurrentHeroCraftingSkill, this.CurrentDifficulty);
			this.CurrentOrderDifficultyText = this.GetCurrentOrderDifficultyText(this.CurrentOrderDifficulty);
		}

		// Token: 0x06001580 RID: 5504 RVA: 0x00051996 File Offset: 0x0004FB96
		private string GetCurrentDifficultyText(int skillValue, int difficultyValue)
		{
			this._difficultyTextobj.SetTextVariable("DIFFICULTY", difficultyValue);
			return this._difficultyTextobj.ToString();
		}

		// Token: 0x06001581 RID: 5505 RVA: 0x000519B5 File Offset: 0x0004FBB5
		private string GetCurrentOrderDifficultyText(int orderDifficulty)
		{
			this._orderDifficultyTextObj.SetTextVariable("DIFFICULTY", orderDifficulty.ToString());
			return this._orderDifficultyTextObj.ToString();
		}

		// Token: 0x06001582 RID: 5506 RVA: 0x000519DC File Offset: 0x0004FBDC
		private void RefreshCurrentHeroSkillLevel()
		{
			Func<CraftingAvailableHeroItemVM> getCurrentCraftingHero = this._getCurrentCraftingHero;
			int? num;
			if (getCurrentCraftingHero == null)
			{
				num = null;
			}
			else
			{
				CraftingAvailableHeroItemVM craftingAvailableHeroItemVM = getCurrentCraftingHero();
				num = ((craftingAvailableHeroItemVM != null) ? new int?(craftingAvailableHeroItemVM.Hero.CharacterObject.GetSkillValue(DefaultSkills.Crafting)) : null);
			}
			this.CurrentHeroCraftingSkill = num ?? 0;
			this.IsCurrentHeroAtMaxCraftingSkill = this.CurrentHeroCraftingSkill >= 300;
			this._currentCraftingSkillText.SetTextVariable("SKILL_VALUE", this.CurrentHeroCraftingSkill);
			this.CurrentCraftingSkillValueText = this._currentCraftingSkillText.ToString();
			this.CurrentDifficultyText = this.GetCurrentDifficultyText(this.CurrentHeroCraftingSkill, this.CurrentDifficulty);
		}

		// Token: 0x06001583 RID: 5507 RVA: 0x00051A9C File Offset: 0x0004FC9C
		public bool HaveUnlockedAllSelectedPieces()
		{
			foreach (CraftingPieceListVM craftingPieceListVM in this.PieceLists)
			{
				if (craftingPieceListVM.IsEnabled)
				{
					CraftingPieceVM selectedPiece = craftingPieceListVM.SelectedPiece;
					if (((selectedPiece != null) ? selectedPiece.CraftingPiece : null) != null && !craftingPieceListVM.SelectedPiece.PlayerHasPiece)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06001584 RID: 5508 RVA: 0x00051B14 File Offset: 0x0004FD14
		private void AddHistoryKey()
		{
			if (this._shouldRecordHistory)
			{
				this._crafting.UpdateHistory();
			}
		}

		// Token: 0x06001585 RID: 5509 RVA: 0x00051B2C File Offset: 0x0004FD2C
		public void SwitchToPiece(WeaponDesignElement usedPiece)
		{
			CraftingPieceVM craftingPieceVM = this._pieceListsDictionary[usedPiece.CraftingPiece.PieceType].Pieces.FirstOrDefault((CraftingPieceVM p) => p.CraftingPiece.CraftingPiece == usedPiece.CraftingPiece);
			this.OnSetItemPiece(craftingPieceVM, usedPiece.ScalePercentage, true, false);
		}

		// Token: 0x06001586 RID: 5510 RVA: 0x00051B8C File Offset: 0x0004FD8C
		internal void SetDesignManually(CraftingTemplate craftingTemplate, ValueTuple<CraftingPiece, int>[] pieces, bool forceChangeTemplate = false)
		{
			int num = this._primaryUsages.IndexOf(craftingTemplate);
			if ((this.IsInFreeMode && forceChangeTemplate) || num == this._selectedWeaponClassIndex)
			{
				this.RefreshWeaponDesignMode(this.ActiveCraftingOrder, this._primaryUsages.IndexOf(craftingTemplate), true);
				for (int i = 0; i < pieces.Length; i++)
				{
					ValueTuple<CraftingPiece, int> currentPiece = pieces[i];
					if (currentPiece.Item1 != null)
					{
						CraftingPieceVM craftingPieceVM = this._pieceListsDictionary[currentPiece.Item1.PieceType].Pieces.FirstOrDefault((CraftingPieceVM piece) => piece.CraftingPiece.CraftingPiece == currentPiece.Item1);
						if (craftingPieceVM != null)
						{
							this.OnSetItemPiece(craftingPieceVM, currentPiece.Item2, true, false);
							this._crafting.ScaleThePiece(currentPiece.Item1.PieceType, currentPiece.Item2);
						}
					}
				}
				this.RefreshDifficulty();
				Action onRefresh = this._onRefresh;
				if (onRefresh == null)
				{
					return;
				}
				onRefresh();
			}
		}

		// Token: 0x17000734 RID: 1844
		// (get) Token: 0x06001587 RID: 5511 RVA: 0x00051C8E File Offset: 0x0004FE8E
		// (set) Token: 0x06001588 RID: 5512 RVA: 0x00051C96 File Offset: 0x0004FE96
		[DataSourceProperty]
		public MBBindingList<TierFilterTypeVM> TierFilters
		{
			get
			{
				return this._tierFilters;
			}
			set
			{
				if (value != this._tierFilters)
				{
					this._tierFilters = value;
					base.OnPropertyChangedWithValue<MBBindingList<TierFilterTypeVM>>(value, "TierFilters");
				}
			}
		}

		// Token: 0x17000735 RID: 1845
		// (get) Token: 0x06001589 RID: 5513 RVA: 0x00051CB4 File Offset: 0x0004FEB4
		// (set) Token: 0x0600158A RID: 5514 RVA: 0x00051CBC File Offset: 0x0004FEBC
		[DataSourceProperty]
		public string CurrentCraftedWeaponTemplateId
		{
			get
			{
				return this._currentCraftedWeaponTemplateId;
			}
			set
			{
				if (value != this._currentCraftedWeaponTemplateId)
				{
					this._currentCraftedWeaponTemplateId = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentCraftedWeaponTemplateId");
				}
			}
		}

		// Token: 0x17000736 RID: 1846
		// (get) Token: 0x0600158B RID: 5515 RVA: 0x00051CDF File Offset: 0x0004FEDF
		// (set) Token: 0x0600158C RID: 5516 RVA: 0x00051CE7 File Offset: 0x0004FEE7
		[DataSourceProperty]
		public string ChooseOrderText
		{
			get
			{
				return this._chooseOrderText;
			}
			set
			{
				if (value != this._chooseOrderText)
				{
					this._chooseOrderText = value;
					base.OnPropertyChangedWithValue<string>(value, "ChooseOrderText");
				}
			}
		}

		// Token: 0x17000737 RID: 1847
		// (get) Token: 0x0600158D RID: 5517 RVA: 0x00051D0A File Offset: 0x0004FF0A
		// (set) Token: 0x0600158E RID: 5518 RVA: 0x00051D12 File Offset: 0x0004FF12
		[DataSourceProperty]
		public string ChooseWeaponTypeText
		{
			get
			{
				return this._chooseWeaponTypeText;
			}
			set
			{
				if (value != this._chooseWeaponTypeText)
				{
					this._chooseWeaponTypeText = value;
					base.OnPropertyChangedWithValue<string>(value, "ChooseWeaponTypeText");
				}
			}
		}

		// Token: 0x17000738 RID: 1848
		// (get) Token: 0x0600158F RID: 5519 RVA: 0x00051D35 File Offset: 0x0004FF35
		// (set) Token: 0x06001590 RID: 5520 RVA: 0x00051D3D File Offset: 0x0004FF3D
		[DataSourceProperty]
		public string CurrentCraftedWeaponTypeText
		{
			get
			{
				return this._currentCraftedWeaponTypeText;
			}
			set
			{
				if (value != this._currentCraftedWeaponTypeText)
				{
					this._currentCraftedWeaponTypeText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentCraftedWeaponTypeText");
				}
			}
		}

		// Token: 0x17000739 RID: 1849
		// (get) Token: 0x06001591 RID: 5521 RVA: 0x00051D60 File Offset: 0x0004FF60
		// (set) Token: 0x06001592 RID: 5522 RVA: 0x00051D68 File Offset: 0x0004FF68
		[DataSourceProperty]
		public MBBindingList<CraftingPieceListVM> PieceLists
		{
			get
			{
				return this._pieceLists;
			}
			set
			{
				if (value != this._pieceLists)
				{
					this._pieceLists = value;
					base.OnPropertyChangedWithValue<MBBindingList<CraftingPieceListVM>>(value, "PieceLists");
				}
			}
		}

		// Token: 0x1700073A RID: 1850
		// (get) Token: 0x06001593 RID: 5523 RVA: 0x00051D86 File Offset: 0x0004FF86
		// (set) Token: 0x06001594 RID: 5524 RVA: 0x00051D8E File Offset: 0x0004FF8E
		[DataSourceProperty]
		public int SelectedPieceTypeIndex
		{
			get
			{
				return this._selectedPieceTypeIndex;
			}
			set
			{
				if (value != this._selectedPieceTypeIndex)
				{
					this._selectedPieceTypeIndex = value;
					base.OnPropertyChangedWithValue(value, "SelectedPieceTypeIndex");
				}
			}
		}

		// Token: 0x1700073B RID: 1851
		// (get) Token: 0x06001595 RID: 5525 RVA: 0x00051DAC File Offset: 0x0004FFAC
		// (set) Token: 0x06001596 RID: 5526 RVA: 0x00051DB4 File Offset: 0x0004FFB4
		[DataSourceProperty]
		public bool ShowOnlyUnlockedPieces
		{
			get
			{
				return this._showOnlyUnlockedPieces;
			}
			set
			{
				if (value != this._showOnlyUnlockedPieces)
				{
					this._showOnlyUnlockedPieces = value;
					base.OnPropertyChangedWithValue(value, "ShowOnlyUnlockedPieces");
				}
			}
		}

		// Token: 0x1700073C RID: 1852
		// (get) Token: 0x06001597 RID: 5527 RVA: 0x00051DD2 File Offset: 0x0004FFD2
		// (set) Token: 0x06001598 RID: 5528 RVA: 0x00051DDA File Offset: 0x0004FFDA
		[DataSourceProperty]
		public string MissingPropertyWarningText
		{
			get
			{
				return this._missingPropertyWarningText;
			}
			set
			{
				if (value != this._missingPropertyWarningText)
				{
					this._missingPropertyWarningText = value;
					base.OnPropertyChangedWithValue<string>(value, "MissingPropertyWarningText");
				}
			}
		}

		// Token: 0x1700073D RID: 1853
		// (get) Token: 0x06001599 RID: 5529 RVA: 0x00051DFD File Offset: 0x0004FFFD
		// (set) Token: 0x0600159A RID: 5530 RVA: 0x00051E05 File Offset: 0x00050005
		[DataSourceProperty]
		public WeaponDesignResultPopupVM CraftingResultPopup
		{
			get
			{
				return this._craftingResultPopup;
			}
			set
			{
				if (value != this._craftingResultPopup)
				{
					this._craftingResultPopup = value;
					base.OnPropertyChangedWithValue<WeaponDesignResultPopupVM>(value, "CraftingResultPopup");
				}
			}
		}

		// Token: 0x1700073E RID: 1854
		// (get) Token: 0x0600159B RID: 5531 RVA: 0x00051E23 File Offset: 0x00050023
		// (set) Token: 0x0600159C RID: 5532 RVA: 0x00051E2B File Offset: 0x0005002B
		[DataSourceProperty]
		public bool IsOrderButtonActive
		{
			get
			{
				return this._isOrderButtonActive;
			}
			set
			{
				if (value != this._isOrderButtonActive)
				{
					this._isOrderButtonActive = value;
					base.OnPropertyChangedWithValue(value, "IsOrderButtonActive");
				}
			}
		}

		// Token: 0x1700073F RID: 1855
		// (get) Token: 0x0600159D RID: 5533 RVA: 0x00051E49 File Offset: 0x00050049
		// (set) Token: 0x0600159E RID: 5534 RVA: 0x00051E51 File Offset: 0x00050051
		[DataSourceProperty]
		public bool IsInOrderMode
		{
			get
			{
				return this._isInOrderMode;
			}
			set
			{
				if (value != this._isInOrderMode)
				{
					this._isInOrderMode = value;
					base.OnPropertyChangedWithValue(value, "IsInOrderMode");
					base.OnPropertyChanged("IsInFreeMode");
				}
			}
		}

		// Token: 0x17000740 RID: 1856
		// (get) Token: 0x0600159F RID: 5535 RVA: 0x00051E7A File Offset: 0x0005007A
		// (set) Token: 0x060015A0 RID: 5536 RVA: 0x00051E85 File Offset: 0x00050085
		[DataSourceProperty]
		public bool IsInFreeMode
		{
			get
			{
				return !this._isInOrderMode;
			}
			set
			{
				if (value != this.IsInFreeMode)
				{
					this._isInOrderMode = !value;
					base.OnPropertyChangedWithValue(value, "IsInFreeMode");
					base.OnPropertyChanged("IsInOrderMode");
				}
			}
		}

		// Token: 0x17000741 RID: 1857
		// (get) Token: 0x060015A1 RID: 5537 RVA: 0x00051EB1 File Offset: 0x000500B1
		// (set) Token: 0x060015A2 RID: 5538 RVA: 0x00051EB9 File Offset: 0x000500B9
		[DataSourceProperty]
		public bool WeaponControlsEnabled
		{
			get
			{
				return this._weaponControlsEnabled;
			}
			set
			{
				if (value != this._weaponControlsEnabled)
				{
					this._weaponControlsEnabled = value;
					base.OnPropertyChangedWithValue(value, "WeaponControlsEnabled");
				}
			}
		}

		// Token: 0x17000742 RID: 1858
		// (get) Token: 0x060015A3 RID: 5539 RVA: 0x00051ED7 File Offset: 0x000500D7
		// (set) Token: 0x060015A4 RID: 5540 RVA: 0x00051EDF File Offset: 0x000500DF
		[DataSourceProperty]
		public string FreeModeButtonText
		{
			get
			{
				return this._freeModeButtonText;
			}
			set
			{
				if (value != this._freeModeButtonText)
				{
					this._freeModeButtonText = value;
					base.OnPropertyChangedWithValue<string>(value, "FreeModeButtonText");
				}
			}
		}

		// Token: 0x17000743 RID: 1859
		// (get) Token: 0x060015A5 RID: 5541 RVA: 0x00051F02 File Offset: 0x00050102
		// (set) Token: 0x060015A6 RID: 5542 RVA: 0x00051F0A File Offset: 0x0005010A
		[DataSourceProperty]
		public CraftingOrderItemVM ActiveCraftingOrder
		{
			get
			{
				return this._activeCraftingOrder;
			}
			set
			{
				if (value != this._activeCraftingOrder)
				{
					this._activeCraftingOrder = value;
					base.OnPropertyChangedWithValue<CraftingOrderItemVM>(value, "ActiveCraftingOrder");
				}
			}
		}

		// Token: 0x17000744 RID: 1860
		// (get) Token: 0x060015A7 RID: 5543 RVA: 0x00051F28 File Offset: 0x00050128
		// (set) Token: 0x060015A8 RID: 5544 RVA: 0x00051F30 File Offset: 0x00050130
		[DataSourceProperty]
		public CraftingOrderPopupVM CraftingOrderPopup
		{
			get
			{
				return this._craftingOrderPopup;
			}
			set
			{
				if (value != this._craftingOrderPopup)
				{
					this._craftingOrderPopup = value;
					base.OnPropertyChangedWithValue<CraftingOrderPopupVM>(value, "CraftingOrderPopup");
				}
			}
		}

		// Token: 0x17000745 RID: 1861
		// (get) Token: 0x060015A9 RID: 5545 RVA: 0x00051F4E File Offset: 0x0005014E
		// (set) Token: 0x060015AA RID: 5546 RVA: 0x00051F56 File Offset: 0x00050156
		[DataSourceProperty]
		public WeaponClassSelectionPopupVM WeaponClassSelectionPopup
		{
			get
			{
				return this._weaponClassSelectionPopup;
			}
			set
			{
				if (value != this._weaponClassSelectionPopup)
				{
					this._weaponClassSelectionPopup = value;
					base.OnPropertyChangedWithValue<WeaponClassSelectionPopupVM>(value, "WeaponClassSelectionPopup");
				}
			}
		}

		// Token: 0x17000746 RID: 1862
		// (get) Token: 0x060015AB RID: 5547 RVA: 0x00051F74 File Offset: 0x00050174
		// (set) Token: 0x060015AC RID: 5548 RVA: 0x00051F7C File Offset: 0x0005017C
		[DataSourceProperty]
		public MBBindingList<CraftingListPropertyItem> PrimaryPropertyList
		{
			get
			{
				return this._primaryPropertyList;
			}
			set
			{
				if (value != this._primaryPropertyList)
				{
					this._primaryPropertyList = value;
					base.OnPropertyChangedWithValue<MBBindingList<CraftingListPropertyItem>>(value, "PrimaryPropertyList");
				}
			}
		}

		// Token: 0x17000747 RID: 1863
		// (get) Token: 0x060015AD RID: 5549 RVA: 0x00051F9A File Offset: 0x0005019A
		// (set) Token: 0x060015AE RID: 5550 RVA: 0x00051FA2 File Offset: 0x000501A2
		[DataSourceProperty]
		public MBBindingList<WeaponDesignResultPropertyItemVM> DesignResultPropertyList
		{
			get
			{
				return this._designResultPropertyList;
			}
			set
			{
				if (value != this._designResultPropertyList)
				{
					this._designResultPropertyList = value;
					base.OnPropertyChangedWithValue<MBBindingList<WeaponDesignResultPropertyItemVM>>(value, "DesignResultPropertyList");
				}
			}
		}

		// Token: 0x17000748 RID: 1864
		// (get) Token: 0x060015AF RID: 5551 RVA: 0x00051FC0 File Offset: 0x000501C0
		// (set) Token: 0x060015B0 RID: 5552 RVA: 0x00051FC8 File Offset: 0x000501C8
		[DataSourceProperty]
		public SelectorVM<CraftingSecondaryUsageItemVM> SecondaryUsageSelector
		{
			get
			{
				return this._secondaryUsageSelector;
			}
			set
			{
				if (value != this._secondaryUsageSelector)
				{
					this._secondaryUsageSelector = value;
					base.OnPropertyChangedWithValue<SelectorVM<CraftingSecondaryUsageItemVM>>(value, "SecondaryUsageSelector");
				}
			}
		}

		// Token: 0x17000749 RID: 1865
		// (get) Token: 0x060015B1 RID: 5553 RVA: 0x00051FE6 File Offset: 0x000501E6
		// (set) Token: 0x060015B2 RID: 5554 RVA: 0x00051FEE File Offset: 0x000501EE
		[DataSourceProperty]
		public ItemCollectionElementViewModel CraftedItemVisual
		{
			get
			{
				return this._craftedItemVisual;
			}
			set
			{
				if (value != this._craftedItemVisual)
				{
					this._craftedItemVisual = value;
					base.OnPropertyChangedWithValue<ItemCollectionElementViewModel>(value, "CraftedItemVisual");
				}
			}
		}

		// Token: 0x1700074A RID: 1866
		// (get) Token: 0x060015B3 RID: 5555 RVA: 0x0005200C File Offset: 0x0005020C
		// (set) Token: 0x060015B4 RID: 5556 RVA: 0x00052014 File Offset: 0x00050214
		[DataSourceProperty]
		public bool IsInFinalCraftingStage
		{
			get
			{
				return this._isInFinalCraftingStage;
			}
			set
			{
				if (value != this._isInFinalCraftingStage)
				{
					this._isInFinalCraftingStage = value;
					this.UpdateResultPropertyList();
					base.OnPropertyChangedWithValue(value, "IsInFinalCraftingStage");
				}
			}
		}

		// Token: 0x1700074B RID: 1867
		// (get) Token: 0x060015B5 RID: 5557 RVA: 0x00052038 File Offset: 0x00050238
		// (set) Token: 0x060015B6 RID: 5558 RVA: 0x00052040 File Offset: 0x00050240
		[DataSourceProperty]
		public string ItemName
		{
			get
			{
				return this._itemName;
			}
			set
			{
				if (value != this._itemName)
				{
					this._itemName = value;
					base.OnPropertyChangedWithValue<string>(value, "ItemName");
					this.RefreshName();
				}
			}
		}

		// Token: 0x1700074C RID: 1868
		// (get) Token: 0x060015B7 RID: 5559 RVA: 0x00052069 File Offset: 0x00050269
		// (set) Token: 0x060015B8 RID: 5560 RVA: 0x00052071 File Offset: 0x00050271
		[DataSourceProperty]
		public bool IsScabbardVisible
		{
			get
			{
				return this._isScabbardVisible;
			}
			set
			{
				if (value != this._isScabbardVisible)
				{
					this._isScabbardVisible = value;
					base.OnPropertyChangedWithValue(value, "IsScabbardVisible");
					this._crafting.ReIndex(false);
					Action onRefresh = this._onRefresh;
					if (onRefresh == null)
					{
						return;
					}
					onRefresh();
				}
			}
		}

		// Token: 0x1700074D RID: 1869
		// (get) Token: 0x060015B9 RID: 5561 RVA: 0x000520AB File Offset: 0x000502AB
		// (set) Token: 0x060015BA RID: 5562 RVA: 0x000520B3 File Offset: 0x000502B3
		[DataSourceProperty]
		public bool CurrentWeaponHasScabbard
		{
			get
			{
				return this._currentWeaponHasScabbard;
			}
			set
			{
				if (value != this._currentWeaponHasScabbard)
				{
					this._currentWeaponHasScabbard = value;
					base.OnPropertyChangedWithValue(value, "CurrentWeaponHasScabbard");
				}
			}
		}

		// Token: 0x1700074E RID: 1870
		// (get) Token: 0x060015BB RID: 5563 RVA: 0x000520D1 File Offset: 0x000502D1
		// (set) Token: 0x060015BC RID: 5564 RVA: 0x000520D9 File Offset: 0x000502D9
		[DataSourceProperty]
		public int CurrentDifficulty
		{
			get
			{
				return this._currentDifficulty;
			}
			set
			{
				if (value != this._currentDifficulty)
				{
					this._currentDifficulty = value;
					base.OnPropertyChangedWithValue(value, "CurrentDifficulty");
				}
			}
		}

		// Token: 0x1700074F RID: 1871
		// (get) Token: 0x060015BD RID: 5565 RVA: 0x000520F7 File Offset: 0x000502F7
		// (set) Token: 0x060015BE RID: 5566 RVA: 0x000520FF File Offset: 0x000502FF
		[DataSourceProperty]
		public int CurrentOrderDifficulty
		{
			get
			{
				return this._currentOrderDifficulty;
			}
			set
			{
				if (value != this._currentOrderDifficulty)
				{
					this._currentOrderDifficulty = value;
					base.OnPropertyChangedWithValue(value, "CurrentOrderDifficulty");
				}
			}
		}

		// Token: 0x17000750 RID: 1872
		// (get) Token: 0x060015BF RID: 5567 RVA: 0x0005211D File Offset: 0x0005031D
		// (set) Token: 0x060015C0 RID: 5568 RVA: 0x00052125 File Offset: 0x00050325
		[DataSourceProperty]
		public int MaxDifficulty
		{
			get
			{
				return this._maxDifficulty;
			}
			set
			{
				if (value != this._maxDifficulty)
				{
					this._maxDifficulty = value;
					base.OnPropertyChangedWithValue(value, "MaxDifficulty");
				}
			}
		}

		// Token: 0x17000751 RID: 1873
		// (get) Token: 0x060015C1 RID: 5569 RVA: 0x00052143 File Offset: 0x00050343
		// (set) Token: 0x060015C2 RID: 5570 RVA: 0x0005214B File Offset: 0x0005034B
		[DataSourceProperty]
		public bool IsCurrentHeroAtMaxCraftingSkill
		{
			get
			{
				return this._isCurrentHeroAtMaxCraftingSkill;
			}
			set
			{
				if (value != this._isCurrentHeroAtMaxCraftingSkill)
				{
					this._isCurrentHeroAtMaxCraftingSkill = value;
					base.OnPropertyChangedWithValue(value, "IsCurrentHeroAtMaxCraftingSkill");
				}
			}
		}

		// Token: 0x17000752 RID: 1874
		// (get) Token: 0x060015C3 RID: 5571 RVA: 0x00052169 File Offset: 0x00050369
		// (set) Token: 0x060015C4 RID: 5572 RVA: 0x00052171 File Offset: 0x00050371
		[DataSourceProperty]
		public int CurrentHeroCraftingSkill
		{
			get
			{
				return this._currentHeroCraftingSkill;
			}
			set
			{
				if (value != this._currentHeroCraftingSkill)
				{
					this._currentHeroCraftingSkill = value;
					base.OnPropertyChangedWithValue(value, "CurrentHeroCraftingSkill");
				}
			}
		}

		// Token: 0x17000753 RID: 1875
		// (get) Token: 0x060015C5 RID: 5573 RVA: 0x0005218F File Offset: 0x0005038F
		// (set) Token: 0x060015C6 RID: 5574 RVA: 0x00052197 File Offset: 0x00050397
		[DataSourceProperty]
		public string CurrentDifficultyText
		{
			get
			{
				return this._currentDifficultyText;
			}
			set
			{
				if (value != this._currentDifficultyText)
				{
					this._currentDifficultyText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentDifficultyText");
				}
			}
		}

		// Token: 0x17000754 RID: 1876
		// (get) Token: 0x060015C7 RID: 5575 RVA: 0x000521BA File Offset: 0x000503BA
		// (set) Token: 0x060015C8 RID: 5576 RVA: 0x000521C2 File Offset: 0x000503C2
		[DataSourceProperty]
		public string CurrentOrderDifficultyText
		{
			get
			{
				return this._currentOrderDifficultyText;
			}
			set
			{
				if (value != this._currentOrderDifficultyText)
				{
					this._currentOrderDifficultyText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentOrderDifficultyText");
				}
			}
		}

		// Token: 0x17000755 RID: 1877
		// (get) Token: 0x060015C9 RID: 5577 RVA: 0x000521E5 File Offset: 0x000503E5
		// (set) Token: 0x060015CA RID: 5578 RVA: 0x000521ED File Offset: 0x000503ED
		[DataSourceProperty]
		public string CurrentCraftingSkillValueText
		{
			get
			{
				return this._currentCraftingSkillValueText;
			}
			set
			{
				if (value != this._currentCraftingSkillValueText)
				{
					this._currentCraftingSkillValueText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentCraftingSkillValueText");
				}
			}
		}

		// Token: 0x17000756 RID: 1878
		// (get) Token: 0x060015CB RID: 5579 RVA: 0x00052210 File Offset: 0x00050410
		// (set) Token: 0x060015CC RID: 5580 RVA: 0x00052218 File Offset: 0x00050418
		[DataSourceProperty]
		public string DifficultyText
		{
			get
			{
				return this._difficultyText;
			}
			set
			{
				if (value != this._difficultyText)
				{
					this._difficultyText = value;
					base.OnPropertyChangedWithValue<string>(value, "DifficultyText");
				}
			}
		}

		// Token: 0x17000757 RID: 1879
		// (get) Token: 0x060015CD RID: 5581 RVA: 0x0005223B File Offset: 0x0005043B
		// (set) Token: 0x060015CE RID: 5582 RVA: 0x00052243 File Offset: 0x00050443
		[DataSourceProperty]
		public string DefaultUsageText
		{
			get
			{
				return this._defaultUsageText;
			}
			set
			{
				if (value != this._defaultUsageText)
				{
					this._defaultUsageText = value;
					base.OnPropertyChangedWithValue<string>(value, "DefaultUsageText");
				}
			}
		}

		// Token: 0x17000758 RID: 1880
		// (get) Token: 0x060015CF RID: 5583 RVA: 0x00052266 File Offset: 0x00050466
		// (set) Token: 0x060015D0 RID: 5584 RVA: 0x0005226E File Offset: 0x0005046E
		[DataSourceProperty]
		public string AlternativeUsageText
		{
			get
			{
				return this._alternativeUsageText;
			}
			set
			{
				if (value != this._alternativeUsageText)
				{
					this._alternativeUsageText = value;
					base.OnPropertyChangedWithValue<string>(value, "AlternativeUsageText");
				}
			}
		}

		// Token: 0x17000759 RID: 1881
		// (get) Token: 0x060015D1 RID: 5585 RVA: 0x00052291 File Offset: 0x00050491
		// (set) Token: 0x060015D2 RID: 5586 RVA: 0x00052299 File Offset: 0x00050499
		[DataSourceProperty]
		public HintViewModel ShowOnlyUnlockedPiecesHint
		{
			get
			{
				return this._showOnlyUnlockedPiecesHint;
			}
			set
			{
				if (value != this._showOnlyUnlockedPiecesHint)
				{
					this._showOnlyUnlockedPiecesHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ShowOnlyUnlockedPiecesHint");
				}
			}
		}

		// Token: 0x1700075A RID: 1882
		// (get) Token: 0x060015D3 RID: 5587 RVA: 0x000522B7 File Offset: 0x000504B7
		// (set) Token: 0x060015D4 RID: 5588 RVA: 0x000522BF File Offset: 0x000504BF
		[DataSourceProperty]
		public CraftingPieceListVM BladePieceList
		{
			get
			{
				return this._bladePieceList;
			}
			set
			{
				if (value != this._bladePieceList)
				{
					this._bladePieceList = value;
					base.OnPropertyChangedWithValue<CraftingPieceListVM>(value, "BladePieceList");
				}
			}
		}

		// Token: 0x1700075B RID: 1883
		// (get) Token: 0x060015D5 RID: 5589 RVA: 0x000522DD File Offset: 0x000504DD
		// (set) Token: 0x060015D6 RID: 5590 RVA: 0x000522E5 File Offset: 0x000504E5
		[DataSourceProperty]
		public CraftingPieceListVM GuardPieceList
		{
			get
			{
				return this._guardPieceList;
			}
			set
			{
				if (value != this._guardPieceList)
				{
					this._guardPieceList = value;
					base.OnPropertyChangedWithValue<CraftingPieceListVM>(value, "GuardPieceList");
				}
			}
		}

		// Token: 0x1700075C RID: 1884
		// (get) Token: 0x060015D7 RID: 5591 RVA: 0x00052303 File Offset: 0x00050503
		// (set) Token: 0x060015D8 RID: 5592 RVA: 0x0005230B File Offset: 0x0005050B
		[DataSourceProperty]
		public CraftingPieceListVM HandlePieceList
		{
			get
			{
				return this._handlePieceList;
			}
			set
			{
				if (value != this._handlePieceList)
				{
					this._handlePieceList = value;
					base.OnPropertyChangedWithValue<CraftingPieceListVM>(value, "HandlePieceList");
				}
			}
		}

		// Token: 0x1700075D RID: 1885
		// (get) Token: 0x060015D9 RID: 5593 RVA: 0x00052329 File Offset: 0x00050529
		// (set) Token: 0x060015DA RID: 5594 RVA: 0x00052331 File Offset: 0x00050531
		[DataSourceProperty]
		public CraftingPieceListVM PommelPieceList
		{
			get
			{
				return this._pommelPieceList;
			}
			set
			{
				if (value != this._pommelPieceList)
				{
					this._pommelPieceList = value;
					base.OnPropertyChangedWithValue<CraftingPieceListVM>(value, "PommelPieceList");
				}
			}
		}

		// Token: 0x1700075E RID: 1886
		// (get) Token: 0x060015DB RID: 5595 RVA: 0x0005234F File Offset: 0x0005054F
		// (set) Token: 0x060015DC RID: 5596 RVA: 0x00052357 File Offset: 0x00050557
		[DataSourceProperty]
		public CraftingPieceVM SelectedBladePiece
		{
			get
			{
				return this._selectedBladePiece;
			}
			set
			{
				if (value != this._selectedBladePiece)
				{
					this._selectedBladePiece = value;
					base.OnPropertyChangedWithValue<CraftingPieceVM>(value, "SelectedBladePiece");
				}
			}
		}

		// Token: 0x1700075F RID: 1887
		// (get) Token: 0x060015DD RID: 5597 RVA: 0x00052375 File Offset: 0x00050575
		// (set) Token: 0x060015DE RID: 5598 RVA: 0x0005237D File Offset: 0x0005057D
		[DataSourceProperty]
		public CraftingPieceVM SelectedGuardPiece
		{
			get
			{
				return this._selectedGuardPiece;
			}
			set
			{
				if (value != this._selectedGuardPiece)
				{
					this._selectedGuardPiece = value;
					base.OnPropertyChangedWithValue<CraftingPieceVM>(value, "SelectedGuardPiece");
				}
			}
		}

		// Token: 0x17000760 RID: 1888
		// (get) Token: 0x060015DF RID: 5599 RVA: 0x0005239B File Offset: 0x0005059B
		// (set) Token: 0x060015E0 RID: 5600 RVA: 0x000523A3 File Offset: 0x000505A3
		[DataSourceProperty]
		public CraftingPieceVM SelectedHandlePiece
		{
			get
			{
				return this._selectedHandlePiece;
			}
			set
			{
				if (value != this._selectedHandlePiece)
				{
					this._selectedHandlePiece = value;
					base.OnPropertyChangedWithValue<CraftingPieceVM>(value, "SelectedHandlePiece");
				}
			}
		}

		// Token: 0x17000761 RID: 1889
		// (get) Token: 0x060015E1 RID: 5601 RVA: 0x000523C1 File Offset: 0x000505C1
		// (set) Token: 0x060015E2 RID: 5602 RVA: 0x000523C9 File Offset: 0x000505C9
		[DataSourceProperty]
		public CraftingPieceVM SelectedPommelPiece
		{
			get
			{
				return this._selectedPommelPiece;
			}
			set
			{
				if (value != this._selectedPommelPiece)
				{
					this._selectedPommelPiece = value;
					base.OnPropertyChangedWithValue<CraftingPieceVM>(value, "SelectedPommelPiece");
				}
			}
		}

		// Token: 0x17000762 RID: 1890
		// (get) Token: 0x060015E3 RID: 5603 RVA: 0x000523E7 File Offset: 0x000505E7
		// (set) Token: 0x060015E4 RID: 5604 RVA: 0x000523F0 File Offset: 0x000505F0
		[DataSourceProperty]
		public int BladeSize
		{
			get
			{
				return this._bladeSize;
			}
			set
			{
				if (value != this._bladeSize)
				{
					this._bladeSize = value;
					base.OnPropertyChangedWithValue(value, "BladeSize");
					if (this._crafting != null && this._updatePiece && this._crafting.CurrentCraftingTemplate.IsPieceTypeUsable(CraftingPiece.PieceTypes.Blade))
					{
						int num = 100 + value;
						this._crafting.ScaleThePiece(CraftingPiece.PieceTypes.Blade, num);
						this.RefreshItem();
					}
				}
			}
		}

		// Token: 0x17000763 RID: 1891
		// (get) Token: 0x060015E5 RID: 5605 RVA: 0x00052454 File Offset: 0x00050654
		// (set) Token: 0x060015E6 RID: 5606 RVA: 0x0005245C File Offset: 0x0005065C
		[DataSourceProperty]
		public int GuardSize
		{
			get
			{
				return this._guardSize;
			}
			set
			{
				if (value != this._guardSize)
				{
					this._guardSize = value;
					base.OnPropertyChangedWithValue(value, "GuardSize");
					if (this._crafting != null && this._updatePiece && this._crafting.CurrentCraftingTemplate.IsPieceTypeUsable(CraftingPiece.PieceTypes.Guard))
					{
						int num = 100 + value;
						this._crafting.ScaleThePiece(CraftingPiece.PieceTypes.Guard, num);
						this.RefreshItem();
					}
				}
			}
		}

		// Token: 0x17000764 RID: 1892
		// (get) Token: 0x060015E7 RID: 5607 RVA: 0x000524C0 File Offset: 0x000506C0
		// (set) Token: 0x060015E8 RID: 5608 RVA: 0x000524C8 File Offset: 0x000506C8
		[DataSourceProperty]
		public int HandleSize
		{
			get
			{
				return this._handleSize;
			}
			set
			{
				if (value != this._handleSize)
				{
					this._handleSize = value;
					base.OnPropertyChangedWithValue(value, "HandleSize");
					if (this._crafting != null && this._updatePiece && this._crafting.CurrentCraftingTemplate.IsPieceTypeUsable(CraftingPiece.PieceTypes.Handle))
					{
						int num = 100 + value;
						this._crafting.ScaleThePiece(CraftingPiece.PieceTypes.Handle, num);
						this.RefreshItem();
					}
				}
			}
		}

		// Token: 0x17000765 RID: 1893
		// (get) Token: 0x060015E9 RID: 5609 RVA: 0x0005252C File Offset: 0x0005072C
		// (set) Token: 0x060015EA RID: 5610 RVA: 0x00052534 File Offset: 0x00050734
		[DataSourceProperty]
		public int PommelSize
		{
			get
			{
				return this._pommelSize;
			}
			set
			{
				if (value != this._pommelSize)
				{
					this._pommelSize = value;
					base.OnPropertyChangedWithValue(value, "PommelSize");
					if (this._crafting != null && this._updatePiece && this._crafting.CurrentCraftingTemplate.IsPieceTypeUsable(CraftingPiece.PieceTypes.Pommel))
					{
						int num = 100 + value;
						this._crafting.ScaleThePiece(CraftingPiece.PieceTypes.Pommel, num);
						this.RefreshItem();
					}
				}
			}
		}

		// Token: 0x17000766 RID: 1894
		// (get) Token: 0x060015EB RID: 5611 RVA: 0x00052598 File Offset: 0x00050798
		// (set) Token: 0x060015EC RID: 5612 RVA: 0x000525A0 File Offset: 0x000507A0
		[DataSourceProperty]
		public string ComponentSizeLbl
		{
			get
			{
				return this._componentSizeLbl;
			}
			set
			{
				if (value != this._componentSizeLbl)
				{
					this._componentSizeLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "ComponentSizeLbl");
				}
			}
		}

		// Token: 0x17000767 RID: 1895
		// (get) Token: 0x060015ED RID: 5613 RVA: 0x000525C3 File Offset: 0x000507C3
		// (set) Token: 0x060015EE RID: 5614 RVA: 0x000525CB File Offset: 0x000507CB
		[DataSourceProperty]
		public bool IsWeaponCivilian
		{
			get
			{
				return this._isWeaponCivilian;
			}
			set
			{
				if (value != this._isWeaponCivilian)
				{
					this._isWeaponCivilian = value;
					base.OnPropertyChangedWithValue(value, "IsWeaponCivilian");
				}
			}
		}

		// Token: 0x17000768 RID: 1896
		// (get) Token: 0x060015EF RID: 5615 RVA: 0x000525E9 File Offset: 0x000507E9
		// (set) Token: 0x060015F0 RID: 5616 RVA: 0x000525F1 File Offset: 0x000507F1
		[DataSourceProperty]
		public HintViewModel ScabbardHint
		{
			get
			{
				return this._scabbardHint;
			}
			set
			{
				if (value != this._scabbardHint)
				{
					this._scabbardHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ScabbardHint");
				}
			}
		}

		// Token: 0x17000769 RID: 1897
		// (get) Token: 0x060015F1 RID: 5617 RVA: 0x0005260F File Offset: 0x0005080F
		// (set) Token: 0x060015F2 RID: 5618 RVA: 0x00052617 File Offset: 0x00050817
		[DataSourceProperty]
		public HintViewModel RandomizeHint
		{
			get
			{
				return this._randomizeHint;
			}
			set
			{
				if (value != this._randomizeHint)
				{
					this._randomizeHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "RandomizeHint");
				}
			}
		}

		// Token: 0x1700076A RID: 1898
		// (get) Token: 0x060015F3 RID: 5619 RVA: 0x00052635 File Offset: 0x00050835
		// (set) Token: 0x060015F4 RID: 5620 RVA: 0x0005263D File Offset: 0x0005083D
		[DataSourceProperty]
		public HintViewModel UndoHint
		{
			get
			{
				return this._undoHint;
			}
			set
			{
				if (value != this._undoHint)
				{
					this._undoHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "UndoHint");
				}
			}
		}

		// Token: 0x1700076B RID: 1899
		// (get) Token: 0x060015F5 RID: 5621 RVA: 0x0005265B File Offset: 0x0005085B
		// (set) Token: 0x060015F6 RID: 5622 RVA: 0x00052663 File Offset: 0x00050863
		[DataSourceProperty]
		public HintViewModel RedoHint
		{
			get
			{
				return this._redoHint;
			}
			set
			{
				if (value != this._redoHint)
				{
					this._redoHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "RedoHint");
				}
			}
		}

		// Token: 0x1700076C RID: 1900
		// (get) Token: 0x060015F7 RID: 5623 RVA: 0x00052681 File Offset: 0x00050881
		// (set) Token: 0x060015F8 RID: 5624 RVA: 0x00052689 File Offset: 0x00050889
		[DataSourceProperty]
		public MBBindingList<ItemFlagVM> WeaponFlagIconsList
		{
			get
			{
				return this._weaponFlagIconsList;
			}
			set
			{
				if (value != this._weaponFlagIconsList)
				{
					this._weaponFlagIconsList = value;
					base.OnPropertyChangedWithValue<MBBindingList<ItemFlagVM>>(value, "WeaponFlagIconsList");
				}
			}
		}

		// Token: 0x1700076D RID: 1901
		// (get) Token: 0x060015F9 RID: 5625 RVA: 0x000526A7 File Offset: 0x000508A7
		// (set) Token: 0x060015FA RID: 5626 RVA: 0x000526AF File Offset: 0x000508AF
		[DataSourceProperty]
		public CraftingHistoryVM CraftingHistory
		{
			get
			{
				return this._craftingHistory;
			}
			set
			{
				if (value != this._craftingHistory)
				{
					this._craftingHistory = value;
					base.OnPropertyChangedWithValue<CraftingHistoryVM>(value, "CraftingHistory");
				}
			}
		}

		// Token: 0x040009F7 RID: 2551
		private WeaponDesignVM.CraftingPieceTierFilter _currentTierFilter = WeaponDesignVM.CraftingPieceTierFilter.All;

		// Token: 0x040009F8 RID: 2552
		public const int MAX_SKILL_LEVEL = 300;

		// Token: 0x040009F9 RID: 2553
		public int ModifierTier;

		// Token: 0x040009FA RID: 2554
		public Crafting.OverrideData OverridenData;

		// Token: 0x040009FB RID: 2555
		public ItemObject CraftedItemObject;

		// Token: 0x040009FC RID: 2556
		private int _selectedWeaponClassIndex;

		// Token: 0x040009FD RID: 2557
		private readonly List<CraftingPiece> _newlyUnlockedPieces;

		// Token: 0x040009FE RID: 2558
		private readonly List<CraftingTemplate> _primaryUsages;

		// Token: 0x040009FF RID: 2559
		private readonly WeaponDesignVM.PieceTierComparer _pieceTierComparer;

		// Token: 0x04000A00 RID: 2560
		private readonly WeaponDesignVM.TemplateComparer _templateComparer;

		// Token: 0x04000A01 RID: 2561
		private readonly ICraftingCampaignBehavior _craftingBehavior;

		// Token: 0x04000A02 RID: 2562
		private readonly Action _onRefresh;

		// Token: 0x04000A03 RID: 2563
		private readonly Action _onWeaponCrafted;

		// Token: 0x04000A04 RID: 2564
		private readonly Func<CraftingAvailableHeroItemVM> _getCurrentCraftingHero;

		// Token: 0x04000A05 RID: 2565
		private readonly Action<CraftingOrder> _refreshHeroAvailabilities;

		// Token: 0x04000A06 RID: 2566
		private Func<WeaponComponentData, ItemObject.ItemUsageSetFlags> _getItemUsageSetFlags;

		// Token: 0x04000A07 RID: 2567
		private Crafting _crafting;

		// Token: 0x04000A08 RID: 2568
		private ItemObject _tempItemObjectForVisual;

		// Token: 0x04000A09 RID: 2569
		private bool _updatePiece = true;

		// Token: 0x04000A0A RID: 2570
		private Dictionary<CraftingPiece.PieceTypes, CraftingPieceListVM> _pieceListsDictionary;

		// Token: 0x04000A0B RID: 2571
		private Dictionary<CraftingPiece, CraftingPieceVM> _pieceVMs;

		// Token: 0x04000A0C RID: 2572
		private TextObject _difficultyTextobj = new TextObject("{=cbbUzYX3}Difficulty: {DIFFICULTY}", null);

		// Token: 0x04000A0D RID: 2573
		private TextObject _orderDifficultyTextObj = new TextObject("{=8szijlHj}Order Difficulty: {DIFFICULTY}", null);

		// Token: 0x04000A0E RID: 2574
		private bool _shouldRecordHistory;

		// Token: 0x04000A0F RID: 2575
		private MBBindingList<TierFilterTypeVM> _tierFilters;

		// Token: 0x04000A10 RID: 2576
		private string _currentCraftedWeaponTemplateId;

		// Token: 0x04000A11 RID: 2577
		private string _chooseOrderText;

		// Token: 0x04000A12 RID: 2578
		private string _chooseWeaponTypeText;

		// Token: 0x04000A13 RID: 2579
		private string _currentCraftedWeaponTypeText;

		// Token: 0x04000A14 RID: 2580
		private MBBindingList<CraftingPieceListVM> _pieceLists;

		// Token: 0x04000A15 RID: 2581
		private int _selectedPieceTypeIndex;

		// Token: 0x04000A16 RID: 2582
		private bool _showOnlyUnlockedPieces;

		// Token: 0x04000A17 RID: 2583
		private string _missingPropertyWarningText;

		// Token: 0x04000A18 RID: 2584
		private bool _isInFinalCraftingStage;

		// Token: 0x04000A19 RID: 2585
		private string _componentSizeLbl;

		// Token: 0x04000A1A RID: 2586
		private string _itemName;

		// Token: 0x04000A1B RID: 2587
		private string _difficultyText;

		// Token: 0x04000A1C RID: 2588
		private int _bladeSize;

		// Token: 0x04000A1D RID: 2589
		private int _pommelSize;

		// Token: 0x04000A1E RID: 2590
		private int _handleSize;

		// Token: 0x04000A1F RID: 2591
		private int _guardSize;

		// Token: 0x04000A20 RID: 2592
		private CraftingPieceVM _selectedBladePiece;

		// Token: 0x04000A21 RID: 2593
		private CraftingPieceVM _selectedGuardPiece;

		// Token: 0x04000A22 RID: 2594
		private CraftingPieceVM _selectedHandlePiece;

		// Token: 0x04000A23 RID: 2595
		private CraftingPieceVM _selectedPommelPiece;

		// Token: 0x04000A24 RID: 2596
		private CraftingPieceListVM _bladePieceList;

		// Token: 0x04000A25 RID: 2597
		private CraftingPieceListVM _guardPieceList;

		// Token: 0x04000A26 RID: 2598
		private CraftingPieceListVM _handlePieceList;

		// Token: 0x04000A27 RID: 2599
		private CraftingPieceListVM _pommelPieceList;

		// Token: 0x04000A28 RID: 2600
		private string _alternativeUsageText;

		// Token: 0x04000A29 RID: 2601
		private string _defaultUsageText;

		// Token: 0x04000A2A RID: 2602
		private bool _isScabbardVisible;

		// Token: 0x04000A2B RID: 2603
		private bool _currentWeaponHasScabbard;

		// Token: 0x04000A2C RID: 2604
		public SelectorVM<CraftingSecondaryUsageItemVM> _secondaryUsageSelector;

		// Token: 0x04000A2D RID: 2605
		private ItemCollectionElementViewModel _craftedItemVisual;

		// Token: 0x04000A2E RID: 2606
		private MBBindingList<CraftingListPropertyItem> _primaryPropertyList;

		// Token: 0x04000A2F RID: 2607
		private MBBindingList<WeaponDesignResultPropertyItemVM> _designResultPropertyList;

		// Token: 0x04000A30 RID: 2608
		private int _currentDifficulty;

		// Token: 0x04000A31 RID: 2609
		private int _currentOrderDifficulty;

		// Token: 0x04000A32 RID: 2610
		private int _maxDifficulty;

		// Token: 0x04000A33 RID: 2611
		private string _currentDifficultyText;

		// Token: 0x04000A34 RID: 2612
		private string _currentOrderDifficultyText;

		// Token: 0x04000A35 RID: 2613
		private string _currentCraftingSkillValueText;

		// Token: 0x04000A36 RID: 2614
		private bool _isCurrentHeroAtMaxCraftingSkill;

		// Token: 0x04000A37 RID: 2615
		private int _currentHeroCraftingSkill;

		// Token: 0x04000A38 RID: 2616
		private bool _isWeaponCivilian;

		// Token: 0x04000A39 RID: 2617
		private HintViewModel _scabbardHint;

		// Token: 0x04000A3A RID: 2618
		private HintViewModel _randomizeHint;

		// Token: 0x04000A3B RID: 2619
		private HintViewModel _undoHint;

		// Token: 0x04000A3C RID: 2620
		private HintViewModel _redoHint;

		// Token: 0x04000A3D RID: 2621
		private HintViewModel _showOnlyUnlockedPiecesHint;

		// Token: 0x04000A3E RID: 2622
		private CraftingOrderItemVM _activeCraftingOrder;

		// Token: 0x04000A3F RID: 2623
		private CraftingOrderPopupVM _craftingOrderPopup;

		// Token: 0x04000A40 RID: 2624
		private WeaponClassSelectionPopupVM _weaponClassSelectionPopup;

		// Token: 0x04000A41 RID: 2625
		private string _freeModeButtonText;

		// Token: 0x04000A42 RID: 2626
		private bool _isOrderButtonActive;

		// Token: 0x04000A43 RID: 2627
		private bool _isInOrderMode;

		// Token: 0x04000A44 RID: 2628
		private bool _weaponControlsEnabled;

		// Token: 0x04000A45 RID: 2629
		private WeaponDesignResultPopupVM _craftingResultPopup;

		// Token: 0x04000A46 RID: 2630
		private MBBindingList<ItemFlagVM> _weaponFlagIconsList;

		// Token: 0x04000A47 RID: 2631
		private CraftingHistoryVM _craftingHistory;

		// Token: 0x04000A48 RID: 2632
		private TextObject _currentCraftingSkillText;

		// Token: 0x0200020D RID: 525
		[Flags]
		public enum CraftingPieceTierFilter
		{
			// Token: 0x04001072 RID: 4210
			None = 0,
			// Token: 0x04001073 RID: 4211
			Tier1 = 1,
			// Token: 0x04001074 RID: 4212
			Tier2 = 2,
			// Token: 0x04001075 RID: 4213
			Tier3 = 4,
			// Token: 0x04001076 RID: 4214
			Tier4 = 8,
			// Token: 0x04001077 RID: 4215
			Tier5 = 16,
			// Token: 0x04001078 RID: 4216
			All = 31
		}

		// Token: 0x0200020E RID: 526
		public class PieceTierComparer : IComparer<CraftingPieceVM>
		{
			// Token: 0x060020E8 RID: 8424 RVA: 0x000702AC File Offset: 0x0006E4AC
			public int Compare(CraftingPieceVM x, CraftingPieceVM y)
			{
				if (x.Tier != y.Tier)
				{
					return x.Tier.CompareTo(y.Tier);
				}
				return x.CraftingPiece.CraftingPiece.StringId.CompareTo(y.CraftingPiece.CraftingPiece.StringId);
			}
		}

		// Token: 0x0200020F RID: 527
		public class TemplateComparer : IComparer<CraftingTemplate>
		{
			// Token: 0x060020EA RID: 8426 RVA: 0x00070309 File Offset: 0x0006E509
			public int Compare(CraftingTemplate x, CraftingTemplate y)
			{
				return string.Compare(x.StringId, y.StringId, StringComparison.OrdinalIgnoreCase);
			}
		}

		// Token: 0x02000210 RID: 528
		public class WeaponPropertyComparer : IComparer<CraftingListPropertyItem>
		{
			// Token: 0x060020EC RID: 8428 RVA: 0x00070328 File Offset: 0x0006E528
			public int Compare(CraftingListPropertyItem x, CraftingListPropertyItem y)
			{
				return ((int)x.Type).CompareTo((int)y.Type);
			}
		}
	}
}
