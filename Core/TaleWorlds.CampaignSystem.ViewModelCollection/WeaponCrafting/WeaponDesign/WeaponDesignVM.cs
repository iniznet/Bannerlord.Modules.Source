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
	public class WeaponDesignVM : ViewModel
	{
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
			this.OrderDisabledReasonHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetOrdersDisabledReasonTooltip(this.CraftingOrderPopup.CraftingOrders, this._getCurrentCraftingHero().Hero));
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

		internal void OnCraftingLogicRefreshed(Crafting newCraftingLogic)
		{
			this._crafting = newCraftingLogic;
			this.InitializeDefaultFromLogic();
		}

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
			Debug.FailedAssert("Invalid tier filter", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Crafting\\WeaponDesign\\WeaponDesignVM.cs", "FilterPieces", 218);
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

		private int GetUnlockedPartsCount(CraftingTemplate template)
		{
			return template.Pieces.Count((CraftingPiece piece) => this._craftingBehavior.IsOpened(piece, template) && !string.IsNullOrEmpty(piece.MeshName));
		}

		private WeaponClassVM GetCurrentWeaponClass()
		{
			if (this._selectedWeaponClassIndex >= 0 && this._selectedWeaponClassIndex < this.WeaponClassSelectionPopup.WeaponClasses.Count)
			{
				return this.WeaponClassSelectionPopup.WeaponClasses[this._selectedWeaponClassIndex];
			}
			return null;
		}

		private void OnSelectItemFromHistory(WeaponDesignSelectorVM selector)
		{
			WeaponDesign design = selector.Design;
			if (design == null)
			{
				Debug.FailedAssert("History design returned null", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Crafting\\WeaponDesign\\WeaponDesignVM.cs", "OnSelectItemFromHistory", 281);
				return;
			}
			ValueTuple<CraftingPiece, int>[] array = new ValueTuple<CraftingPiece, int>[design.UsedPieces.Length];
			for (int i = 0; i < design.UsedPieces.Length; i++)
			{
				array[i] = new ValueTuple<CraftingPiece, int>(design.UsedPieces[i].CraftingPiece, design.UsedPieces[i].ScalePercentage);
			}
			this.SetDesignManually(design.Template, array, true);
		}

		public void SetPieceNewlyUnlocked(CraftingPiece piece)
		{
			if (!this._newlyUnlockedPieces.Contains(piece))
			{
				this._newlyUnlockedPieces.Add(piece);
			}
		}

		private void UnsetPieceNewlyUnlocked(CraftingPieceVM pieceVM)
		{
			CraftingPiece craftingPiece = pieceVM.CraftingPiece.CraftingPiece;
			if (this._newlyUnlockedPieces.Contains(craftingPiece))
			{
				this._newlyUnlockedPieces.Remove(craftingPiece);
				pieceVM.IsNewlyUnlocked = false;
			}
		}

		private void OnSelectPieceTierFilter(WeaponDesignVM.CraftingPieceTierFilter filter)
		{
			if (this._currentTierFilter != filter)
			{
				this.FilterPieces(filter);
			}
		}

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

		private void SelectDefaultPiecesForCurrentTemplate()
		{
			CraftingOrderItemVM activeCraftingOrder = this.ActiveCraftingOrder;
			string text = ((activeCraftingOrder != null) ? activeCraftingOrder.CraftingOrder.GetStatWeapon().WeaponDescriptionId : null);
			WeaponDescription statWeaponUsage = ((text != null) ? MBObjectManager.Instance.GetObject<WeaponDescription>(text) : null);
			WeaponClassVM currentWeaponClass = this.GetCurrentWeaponClass();
			this._shouldRecordHistory = false;
			this._isAutoSelectingPieces = true;
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
							orderby p.PlayerHasPiece descending, !p.IsNewlyUnlocked descending
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
			this._isAutoSelectingPieces = false;
		}

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

		private void AddClassFlagsToPiece(CraftingPieceVM piece)
		{
			WeaponComponentData weaponWithUsageIndex = this._crafting.GetCurrentCraftedItemObject(false).GetWeaponWithUsageIndex(this.SecondaryUsageSelector.SelectedIndex);
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

		private void UpdateSecondaryUsageIndex(SelectorVM<CraftingSecondaryUsageItemVM> selector)
		{
			if (selector.SelectedIndex != -1)
			{
				this.RefreshStats();
				this.RefreshPieceFlags();
			}
		}

		private void UpdateResultPropertyList()
		{
			this.DesignResultPropertyList.Clear();
			int num = this.SecondaryUsageSelector.SelectedIndex;
			if (this.IsInOrderMode)
			{
				WeaponComponentData orderWeapon = this.ActiveCraftingOrder.CraftingOrder.GetStatWeapon();
				num = this._crafting.GetCurrentCraftedItemObject(false).Weapons.FindIndex((WeaponComponentData x) => x.WeaponDescriptionId == orderWeapon.WeaponDescriptionId);
			}
			this.TrySetSecondaryUsageIndex(num);
			this.RefreshStats();
			ItemModifier currentItemModifier = this._craftingBehavior.GetCurrentItemModifier();
			foreach (CraftingListPropertyItem craftingListPropertyItem in this.PrimaryPropertyList)
			{
				float num2 = 0f;
				bool flag = craftingListPropertyItem.Type == CraftingTemplate.CraftingStatTypes.Weight;
				if (currentItemModifier != null)
				{
					float num3 = craftingListPropertyItem.PropertyValue;
					if (craftingListPropertyItem.Type == CraftingTemplate.CraftingStatTypes.SwingDamage)
					{
						num3 = (float)currentItemModifier.ModifyDamage((int)craftingListPropertyItem.PropertyValue);
					}
					else if (craftingListPropertyItem.Type == CraftingTemplate.CraftingStatTypes.SwingSpeed)
					{
						num3 = (float)currentItemModifier.ModifySpeed((int)craftingListPropertyItem.PropertyValue);
					}
					else if (craftingListPropertyItem.Type == CraftingTemplate.CraftingStatTypes.ThrustDamage)
					{
						num3 = (float)currentItemModifier.ModifyDamage((int)craftingListPropertyItem.PropertyValue);
					}
					else if (craftingListPropertyItem.Type == CraftingTemplate.CraftingStatTypes.ThrustSpeed)
					{
						num3 = (float)currentItemModifier.ModifySpeed((int)craftingListPropertyItem.PropertyValue);
					}
					else if (craftingListPropertyItem.Type == CraftingTemplate.CraftingStatTypes.Handling)
					{
						num3 = (float)currentItemModifier.ModifySpeed((int)craftingListPropertyItem.PropertyValue);
					}
					if (num3 != craftingListPropertyItem.PropertyValue)
					{
						num2 = num3 - craftingListPropertyItem.PropertyValue;
					}
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
			this.IsOrderButtonActive = this.CraftingOrderPopup.HasEnabledOrders;
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

		private void OnCraftingOrderSelected(CraftingOrderItemVM selectedOrder)
		{
			this.RefreshWeaponDesignMode(selectedOrder, -1, false);
		}

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

		public void ExecuteCloseOrderPopup()
		{
			this.CraftingOrderPopup.IsVisible = false;
		}

		public void ExecuteOpenOrdersTab()
		{
			if (this.IsInFreeMode)
			{
				MBBindingList<CraftingOrderItemVM> craftingOrders = this.CraftingOrderPopup.CraftingOrders;
				CraftingOrderItemVM craftingOrderItemVM;
				if (craftingOrders == null)
				{
					craftingOrderItemVM = null;
				}
				else
				{
					craftingOrderItemVM = craftingOrders.FirstOrDefault((CraftingOrderItemVM x) => x.IsEnabled);
				}
				CraftingOrderItemVM craftingOrderItemVM2 = craftingOrderItemVM;
				if (craftingOrderItemVM2 != null)
				{
					this.CraftingOrderPopup.SelectOrder(craftingOrderItemVM2);
					return;
				}
				this.CraftingOrderPopup.ExecuteOpenPopup();
			}
		}

		public void ExecuteOpenWeaponClassSelectionPopup()
		{
			this.WeaponClassSelectionPopup.UpdateNewlyUnlockedPiecesCount(this._newlyUnlockedPieces);
			this.WeaponClassSelectionPopup.WeaponClasses.ApplyActionOnAllItems(delegate(WeaponClassVM x)
			{
				x.IsSelected = x.SelectionIndex == this._selectedWeaponClassIndex;
			});
			this.WeaponClassSelectionPopup.ExecuteOpenPopup();
		}

		public void ExecuteOpenFreeBuildTab()
		{
			if (this.IsInOrderMode)
			{
				this.WeaponClassSelectionPopup.UpdateNewlyUnlockedPiecesCount(this._newlyUnlockedPieces);
				this.WeaponClassSelectionPopup.WeaponClasses.ApplyActionOnAllItems(delegate(WeaponClassVM x)
				{
					x.IsSelected = false;
				});
				this.WeaponClassSelectionPopup.ExecuteSelectWeaponClass(0);
			}
		}

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

		public void ExecuteToggleShowOnlyUnlockedPieces()
		{
			this.ShowOnlyUnlockedPieces = !this.ShowOnlyUnlockedPieces;
			this.FilterPieces(this._currentTierFilter);
		}

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

		internal void OnCraftingHeroChanged(CraftingAvailableHeroItemVM newHero)
		{
			this.RefreshCurrentHeroSkillLevel();
			this.RefreshDifficulty();
			this.CraftingOrderPopup.RefreshOrders();
			this.IsOrderButtonActive = this.CraftingOrderPopup.HasEnabledOrders;
		}

		public void ChangeModeIfHeroIsUnavailable()
		{
			CraftingAvailableHeroItemVM craftingAvailableHeroItemVM = this._getCurrentCraftingHero();
			if (this.IsInOrderMode && craftingAvailableHeroItemVM.IsDisabled)
			{
				this.RefreshWeaponDesignMode(null, -1, false);
			}
		}

		public void ExecuteBeginHeroHint()
		{
			CraftingOrderItemVM activeCraftingOrder = this._activeCraftingOrder;
			if (((activeCraftingOrder != null) ? activeCraftingOrder.CraftingOrder.OrderOwner : null) != null)
			{
				InformationManager.ShowTooltip(typeof(Hero), new object[]
				{
					this._activeCraftingOrder.CraftingOrder.OrderOwner,
					false
				});
			}
		}

		public void ExecuteEndHeroHint()
		{
			MBInformationManager.HideInformations();
		}

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

		public void ExecuteChangeScabbardVisibility()
		{
			if (!this._crafting.CurrentCraftingTemplate.UseWeaponAsHolsterMesh)
			{
				this.IsScabbardVisible = !this.IsScabbardVisible;
			}
		}

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

		public bool CanCompleteOrder()
		{
			bool flag = true;
			if (this.IsInOrderMode)
			{
				ItemObject currentCraftedItemObject = this._crafting.GetCurrentCraftedItemObject(false);
				flag = this.ActiveCraftingOrder.CraftingOrder.CanHeroCompleteOrder(this._getCurrentCraftingHero().Hero, currentCraftedItemObject);
			}
			return flag;
		}

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
						this.CraftingOrderPopup.RefreshOrders();
						CraftingOrderItemVM craftingOrderItemVM = this.CraftingOrderPopup.CraftingOrders.FirstOrDefault((CraftingOrderItemVM x) => x.IsEnabled);
						if (craftingOrderItemVM != null)
						{
							this.CraftingOrderPopup.SelectOrder(craftingOrderItemVM);
						}
						else
						{
							this.ExecuteOpenFreeBuildTab();
						}
					}
					else
					{
						int bladeSize = this.BladeSize;
						int guardSize = this.GuardSize;
						int handleSize = this.HandleSize;
						int pommelSize = this.PommelSize;
						ItemModifier currentItemModifier = this._craftingBehavior.GetCurrentItemModifier();
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
						craftingBehavior.CreateCraftedWeaponInFreeBuildMode(hero, this._crafting.CurrentWeaponDesign, currentItemModifier);
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

		public void RegisterTempItemObject()
		{
			this._tempItemObjectForVisual = this._crafting.GetCurrentCraftedItemObject(true);
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

		public void AddCraftingModifier(WeaponDesign weaponDesign, Hero hero)
		{
			ItemModifier craftedWeaponModifier = Campaign.Current.Models.SmithingModel.GetCraftedWeaponModifier(weaponDesign, hero);
			this._craftingBehavior.SetCurrentItemModifier(craftedWeaponModifier);
		}

		public void UnregisterTempItemObject()
		{
			if (this._tempItemObjectForVisual != null)
			{
				MBObjectManager.Instance.UnregisterObject(this._tempItemObjectForVisual);
			}
		}

		private bool DoesCurrentItemHaveSecondaryUsage(int usageIndex)
		{
			return usageIndex >= 0 && usageIndex < this._crafting.GetCurrentCraftedItemObject(false).Weapons.Count;
		}

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

		private void RefreshAlternativeUsageList()
		{
			int num = this.SecondaryUsageSelector.SelectedIndex;
			this.SecondaryUsageSelector.Refresh(new List<string>(), 0, new Action<SelectorVM<CraftingSecondaryUsageItemVM>>(this.UpdateSecondaryUsageIndex));
			MBReadOnlyList<WeaponComponentData> weapons = this._crafting.GetCurrentCraftedItemObject(false).Weapons;
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

		private void RefreshStats()
		{
			if (!this.DoesCurrentItemHaveSecondaryUsage(this.SecondaryUsageSelector.SelectedIndex))
			{
				this.TrySetSecondaryUsageIndex(0);
			}
			List<CraftingStatData> list = this._crafting.GetStatDatas(this.SecondaryUsageSelector.SelectedIndex).ToList<CraftingStatData>();
			WeaponComponentData weaponComponentData = (this.IsInOrderMode ? this.ActiveCraftingOrder.CraftingOrder.GetStatWeapon() : null);
			IEnumerable<CraftingStatData> enumerable = (this.IsInOrderMode ? this.GetOrderStatDatas(this.ActiveCraftingOrder.CraftingOrder) : null);
			ItemObject currentCraftedItemObject = this._crafting.GetCurrentCraftedItemObject(false);
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

		private IEnumerable<CraftingStatData> GetOrderStatDatas(CraftingOrder order)
		{
			if (order == null)
			{
				return null;
			}
			WeaponComponentData weaponComponentData;
			return order.GetStatDataForItem(order.PreCraftedWeaponDesignItem, out weaponComponentData);
		}

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

		private void RefreshName()
		{
			this._crafting.SetCraftedWeaponName(this.ItemName);
		}

		private void OnSetItemPieceManually(CraftingPieceVM piece)
		{
			this.OnSetItemPiece(piece, 0, true, false);
			this.RefreshItem();
			this.AddHistoryKey();
		}

		private void OnSetItemPiece(CraftingPieceVM piece, int scalePercentage = 0, bool shouldUpdateWholeWeapon = true, bool forceUpdatePiece = false)
		{
			CraftingPiece.PieceTypes pieceType = (CraftingPiece.PieceTypes)piece.PieceType;
			this._pieceListsDictionary[pieceType].SelectedPiece.IsSelected = false;
			bool updatePiece = this._updatePiece;
			if (!this._isAutoSelectingPieces)
			{
				this.UnsetPieceNewlyUnlocked(piece);
			}
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

		private string GetCurrentDifficultyText(int skillValue, int difficultyValue)
		{
			this._difficultyTextobj.SetTextVariable("DIFFICULTY", difficultyValue);
			return this._difficultyTextobj.ToString();
		}

		private string GetCurrentOrderDifficultyText(int orderDifficulty)
		{
			this._orderDifficultyTextObj.SetTextVariable("DIFFICULTY", orderDifficulty.ToString());
			return this._orderDifficultyTextObj.ToString();
		}

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

		private void AddHistoryKey()
		{
			if (this._shouldRecordHistory)
			{
				this._crafting.UpdateHistory();
			}
		}

		public void SwitchToPiece(WeaponDesignElement usedPiece)
		{
			CraftingPieceVM craftingPieceVM = this._pieceListsDictionary[usedPiece.CraftingPiece.PieceType].Pieces.FirstOrDefault((CraftingPieceVM p) => p.CraftingPiece.CraftingPiece == usedPiece.CraftingPiece);
			this.OnSetItemPiece(craftingPieceVM, usedPiece.ScalePercentage, true, false);
		}

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

		[DataSourceProperty]
		public BasicTooltipViewModel OrderDisabledReasonHint
		{
			get
			{
				return this._orderDisabledReasonHint;
			}
			set
			{
				if (value != this._orderDisabledReasonHint)
				{
					this._orderDisabledReasonHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "OrderDisabledReasonHint");
				}
			}
		}

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

		private WeaponDesignVM.CraftingPieceTierFilter _currentTierFilter = WeaponDesignVM.CraftingPieceTierFilter.All;

		public const int MAX_SKILL_LEVEL = 300;

		public ItemObject CraftedItemObject;

		private int _selectedWeaponClassIndex;

		private readonly List<CraftingPiece> _newlyUnlockedPieces;

		private readonly List<CraftingTemplate> _primaryUsages;

		private readonly WeaponDesignVM.PieceTierComparer _pieceTierComparer;

		private readonly WeaponDesignVM.TemplateComparer _templateComparer;

		private readonly ICraftingCampaignBehavior _craftingBehavior;

		private readonly Action _onRefresh;

		private readonly Action _onWeaponCrafted;

		private readonly Func<CraftingAvailableHeroItemVM> _getCurrentCraftingHero;

		private readonly Action<CraftingOrder> _refreshHeroAvailabilities;

		private Func<WeaponComponentData, ItemObject.ItemUsageSetFlags> _getItemUsageSetFlags;

		private Crafting _crafting;

		private ItemObject _tempItemObjectForVisual;

		private bool _updatePiece = true;

		private Dictionary<CraftingPiece.PieceTypes, CraftingPieceListVM> _pieceListsDictionary;

		private Dictionary<CraftingPiece, CraftingPieceVM> _pieceVMs;

		private TextObject _difficultyTextobj = new TextObject("{=cbbUzYX3}Difficulty: {DIFFICULTY}", null);

		private TextObject _orderDifficultyTextObj = new TextObject("{=8szijlHj}Order Difficulty: {DIFFICULTY}", null);

		private bool _isAutoSelectingPieces;

		private bool _shouldRecordHistory;

		private MBBindingList<TierFilterTypeVM> _tierFilters;

		private string _currentCraftedWeaponTemplateId;

		private string _chooseOrderText;

		private string _chooseWeaponTypeText;

		private string _currentCraftedWeaponTypeText;

		private MBBindingList<CraftingPieceListVM> _pieceLists;

		private int _selectedPieceTypeIndex;

		private bool _showOnlyUnlockedPieces;

		private string _missingPropertyWarningText;

		private bool _isInFinalCraftingStage;

		private string _componentSizeLbl;

		private string _itemName;

		private string _difficultyText;

		private int _bladeSize;

		private int _pommelSize;

		private int _handleSize;

		private int _guardSize;

		private CraftingPieceVM _selectedBladePiece;

		private CraftingPieceVM _selectedGuardPiece;

		private CraftingPieceVM _selectedHandlePiece;

		private CraftingPieceVM _selectedPommelPiece;

		private CraftingPieceListVM _bladePieceList;

		private CraftingPieceListVM _guardPieceList;

		private CraftingPieceListVM _handlePieceList;

		private CraftingPieceListVM _pommelPieceList;

		private string _alternativeUsageText;

		private string _defaultUsageText;

		private bool _isScabbardVisible;

		private bool _currentWeaponHasScabbard;

		public SelectorVM<CraftingSecondaryUsageItemVM> _secondaryUsageSelector;

		private ItemCollectionElementViewModel _craftedItemVisual;

		private MBBindingList<CraftingListPropertyItem> _primaryPropertyList;

		private MBBindingList<WeaponDesignResultPropertyItemVM> _designResultPropertyList;

		private int _currentDifficulty;

		private int _currentOrderDifficulty;

		private int _maxDifficulty;

		private string _currentDifficultyText;

		private string _currentOrderDifficultyText;

		private string _currentCraftingSkillValueText;

		private bool _isCurrentHeroAtMaxCraftingSkill;

		private int _currentHeroCraftingSkill;

		private bool _isWeaponCivilian;

		private HintViewModel _scabbardHint;

		private HintViewModel _randomizeHint;

		private HintViewModel _undoHint;

		private HintViewModel _redoHint;

		private HintViewModel _showOnlyUnlockedPiecesHint;

		private BasicTooltipViewModel _orderDisabledReasonHint;

		private CraftingOrderItemVM _activeCraftingOrder;

		private CraftingOrderPopupVM _craftingOrderPopup;

		private WeaponClassSelectionPopupVM _weaponClassSelectionPopup;

		private string _freeModeButtonText;

		private bool _isOrderButtonActive;

		private bool _isInOrderMode;

		private bool _weaponControlsEnabled;

		private WeaponDesignResultPopupVM _craftingResultPopup;

		private MBBindingList<ItemFlagVM> _weaponFlagIconsList;

		private CraftingHistoryVM _craftingHistory;

		private TextObject _currentCraftingSkillText;

		[Flags]
		public enum CraftingPieceTierFilter
		{
			None = 0,
			Tier1 = 1,
			Tier2 = 2,
			Tier3 = 4,
			Tier4 = 8,
			Tier5 = 16,
			All = 31
		}

		public class PieceTierComparer : IComparer<CraftingPieceVM>
		{
			public int Compare(CraftingPieceVM x, CraftingPieceVM y)
			{
				if (x.Tier != y.Tier)
				{
					return x.Tier.CompareTo(y.Tier);
				}
				return x.CraftingPiece.CraftingPiece.StringId.CompareTo(y.CraftingPiece.CraftingPiece.StringId);
			}
		}

		public class TemplateComparer : IComparer<CraftingTemplate>
		{
			public int Compare(CraftingTemplate x, CraftingTemplate y)
			{
				return string.Compare(x.StringId, y.StringId, StringComparison.OrdinalIgnoreCase);
			}
		}

		public class WeaponPropertyComparer : IComparer<CraftingListPropertyItem>
		{
			public int Compare(CraftingListPropertyItem x, CraftingListPropertyItem y)
			{
				return ((int)x.Type).CompareTo((int)y.Type);
			}
		}
	}
}
