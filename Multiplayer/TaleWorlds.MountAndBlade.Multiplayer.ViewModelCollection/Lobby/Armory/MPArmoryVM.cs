using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.ClassLoadout;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory.CosmeticItem;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.ClassFilter;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory
{
	public class MPArmoryVM : ViewModel
	{
		public MPArmoryVM(Action<BasicCharacterObject> onOpenFacegen, Action<MPArmoryCosmeticItemBaseVM> onItemObtainRequested, Func<string> getExitText)
		{
			this._getExitText = getExitText;
			this._onOpenFacegen = onOpenFacegen;
			this._character = MBObjectManager.Instance.GetObject<BasicCharacterObject>("mp_character");
			this.CanOpenFacegen = true;
			this.ClassFilter = new MPLobbyClassFilterVM(new Action<MPLobbyClassFilterClassItemVM, bool>(this.OnSelectedClassChanged));
			this.HeroPreview = new MPArmoryHeroPreviewVM();
			this.ClassStats = new MPArmoryClassStatsVM();
			this.HeroPerkSelection = new MPArmoryHeroPerkSelectionVM(new Action<HeroPerkVM, MPPerkVM>(this.OnSelectPerk), new Action(this.ForceRefreshCharacter));
			this.Cosmetics = new MPArmoryCosmeticsVM(new Func<List<IReadOnlyPerkObject>>(this.GetSelectedPerks));
			this.InitalizeCallbacks();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.StatsText = new TextObject("{=ffjTMejn}Stats", null).ToString();
			this.CustomizationText = new TextObject("{=sPkRekRL}Customization", null).ToString();
			this.FacegenText = new TextObject("{=RSx1e5Wf}Edit Character", null).ToString();
			this.RefreshManageTauntButtonText();
			this.ClassFilter.RefreshValues();
			this.HeroPreview.RefreshValues();
			this.ClassStats.RefreshValues();
			this.Cosmetics.RefreshValues();
			this.HeroPerkSelection.RefreshValues();
		}

		private void RefreshManageTauntButtonText()
		{
			this.ManageTauntsText = (this.IsManagingTaunts ? new TextObject("{=WiNRdfsm}Done", null).ToString() : new TextObject("{=58O7bWrD}Manage Taunts", null).ToString());
		}

		public override void OnFinalize()
		{
			this.HeroPreview = null;
			this._character = null;
			this.FinalizeCallbacks();
			this.Cosmetics.OnFinalize();
			base.OnFinalize();
		}

		private void InitalizeCallbacks()
		{
			CharacterViewModel.OnCustomAnimationFinished = (Action<CharacterViewModel>)Delegate.Combine(CharacterViewModel.OnCustomAnimationFinished, new Action<CharacterViewModel>(this.OnCharacterCustomAnimationFinished));
			MPArmoryCosmeticsVM.OnCosmeticPreview += this.OnHeroPreviewItemEquipped;
			MPArmoryCosmeticsVM.OnRemoveCosmeticFromPreview += this.RemoveHeroPreviewItem;
			MPArmoryCosmeticsVM.OnTauntAssignmentRefresh += this.OnTauntAssignmentRefresh;
		}

		private void FinalizeCallbacks()
		{
			CharacterViewModel.OnCustomAnimationFinished = (Action<CharacterViewModel>)Delegate.Remove(CharacterViewModel.OnCustomAnimationFinished, new Action<CharacterViewModel>(this.OnCharacterCustomAnimationFinished));
			MPArmoryCosmeticsVM.OnCosmeticPreview -= this.OnHeroPreviewItemEquipped;
			MPArmoryCosmeticsVM.OnRemoveCosmeticFromPreview -= this.RemoveHeroPreviewItem;
			MPArmoryCosmeticsVM.OnTauntAssignmentRefresh -= this.OnTauntAssignmentRefresh;
		}

		public void OnTick(float dt)
		{
			if (this._tauntItemToRefreshNextAnimationWith != null)
			{
				MPArmoryCosmeticTauntItemVM tauntItemToRefreshNextAnimationWith = this._tauntItemToRefreshNextAnimationWith;
				Equipment equipment = LobbyTauntHelper.PrepareForTaunt(Equipment.CreateFromEquipmentCode(this.HeroPreview.HeroVisual.EquipmentCode), tauntItemToRefreshNextAnimationWith.TauntCosmeticElement, false);
				EquipmentIndex equipmentIndex;
				EquipmentIndex equipmentIndex2;
				bool flag;
				equipment.GetInitialWeaponIndicesToEquip(ref equipmentIndex, ref equipmentIndex2, ref flag, 0);
				this.HeroPreview.HeroVisual.EquipmentCode = equipment.CalculateEquipmentCode();
				this.HeroPreview.HeroVisual.RightHandWieldedEquipmentIndex = equipmentIndex;
				if (!flag)
				{
					this.HeroPreview.HeroVisual.LeftHandWieldedEquipmentIndex = equipmentIndex2;
				}
				this.HeroPreview.HeroVisual.ExecuteStartCustomAnimation(CosmeticsManagerHelper.GetSuitableTauntActionForEquipment(equipment, tauntItemToRefreshNextAnimationWith.TauntCosmeticElement), false, 0.25f);
				this._currentTauntPreviewAnimationSource = this._tauntItemToRefreshNextAnimationWith;
				this._tauntItemToRefreshNextAnimationWith = null;
			}
			if (this.HeroPreview.HeroVisual.IsPlayingCustomAnimations && this._currentTauntPreviewAnimationSource != null)
			{
				float num = MathF.Clamp(this.HeroPreview.HeroVisual.CustomAnimationProgressRatio, 0f, 1f);
				this._currentTauntPreviewAnimationSource.PreviewAnimationRatio = (float)((int)(num * 100f));
			}
		}

		public void RefreshPlayerData(PlayerData playerData)
		{
			if (this._character != null)
			{
				this._character.UpdatePlayerCharacterBodyProperties(playerData.BodyProperties, playerData.Race, playerData.IsFemale);
				this._character.Age = playerData.BodyProperties.Age;
				this.HeroPreview.SetCharacter(this._character, playerData.BodyProperties.DynamicProperties, playerData.Race, playerData.IsFemale);
				this.ForceRefreshCharacter();
				this.Cosmetics.RefreshPlayerData(playerData);
			}
		}

		public void ForceRefreshCharacter()
		{
			this.OnSelectedClassChanged(this.ClassFilter.SelectedClassItem, true);
		}

		private void OnIsEnabledChanged()
		{
			PlayerData playerData = NetworkMain.GameClient.PlayerData;
			if (this.IsEnabled && playerData != null)
			{
				this.RefreshPlayerData(playerData);
			}
			if (this.IsEnabled)
			{
				this.Cosmetics.RefreshCosmeticInfoFromNetwork();
				if (this.IsManagingTaunts)
				{
					this.ExecuteToggleManageTauntsState();
					return;
				}
			}
			else
			{
				MPArmoryHeroPerkSelectionVM heroPerkSelection = this.HeroPerkSelection;
				foreach (HeroPerkVM heroPerkVM in ((heroPerkSelection != null) ? heroPerkSelection.Perks : null))
				{
					BasicTooltipViewModel hint = heroPerkVM.Hint;
					if (hint != null)
					{
						hint.ExecuteEndHint();
					}
				}
			}
		}

		private void OnSelectedClassChanged(MPLobbyClassFilterClassItemVM selectedClassItem, bool forceUpdate = false)
		{
			if (this.HeroPreview == null || this.ClassStats == null || this.HeroPerkSelection == null)
			{
				return;
			}
			if (this._currentClassItem == selectedClassItem && !forceUpdate)
			{
				return;
			}
			this._currentClassItem = selectedClassItem;
			this.HeroPerkSelection.RefreshPerksListWithHero(selectedClassItem.HeroClass);
			this.HeroPreview.SetCharacterClass(selectedClassItem.HeroClass.HeroCharacter);
			this.HeroPreview.SetCharacterPerks(this.HeroPerkSelection.CurrentSelectedPerks);
			this.ClassStats.RefreshWith(selectedClassItem.HeroClass);
			this.ClassStats.HeroInformation.RefreshWith(this.HeroPerkSelection.CurrentHeroClass, this.HeroPerkSelection.Perks.Select((HeroPerkVM x) => x.SelectedPerk).ToList<IReadOnlyPerkObject>());
			this.Cosmetics.RefreshSelectedClass(selectedClassItem.HeroClass, this.HeroPerkSelection.CurrentSelectedPerks);
			this.Cosmetics.RefreshCosmeticInfoFromNetwork();
		}

		public void SetCanOpenFacegen(bool enabled)
		{
			this.CanOpenFacegen = enabled;
		}

		private void ExecuteOpenFaceGen()
		{
			Action<BasicCharacterObject> onOpenFacegen = this._onOpenFacegen;
			if (onOpenFacegen == null)
			{
				return;
			}
			onOpenFacegen(this._character);
		}

		public void ExecuteClearTauntSelection()
		{
			this.Cosmetics.ClearTauntSelections();
		}

		public void ExecuteToggleManageTauntsState()
		{
			this.IsManagingTaunts = !this.IsManagingTaunts;
			if (this.IsManagingTaunts)
			{
				this._canOpenFacegenBeforeTauntState = this.CanOpenFacegen;
				this.Cosmetics.RefreshAvailableCategoriesBy(3);
				this.Cosmetics.TauntSlots.ApplyActionOnAllItems(delegate(MPArmoryCosmeticTauntSlotVM s)
				{
					s.IsEnabled = true;
				});
				this.Cosmetics.TauntSlots.ApplyActionOnAllItems(delegate(MPArmoryCosmeticTauntSlotVM s)
				{
					s.IsFocused = false;
				});
			}
			else
			{
				this.Cosmetics.RefreshAvailableCategoriesBy(0);
				this.Cosmetics.TauntSlots.ApplyActionOnAllItems(delegate(MPArmoryCosmeticTauntSlotVM s)
				{
					s.IsEnabled = false;
				});
				this.Cosmetics.ClearTauntSelections();
			}
			this.CanOpenFacegen = !this.IsManagingTaunts && this._canOpenFacegenBeforeTauntState;
			this.RefreshManageTauntButtonText();
		}

		public void ExecuteSelectFocusedSlot()
		{
			foreach (MPArmoryCosmeticTauntSlotVM mparmoryCosmeticTauntSlotVM in this.Cosmetics.TauntSlots)
			{
				if (mparmoryCosmeticTauntSlotVM.IsFocused)
				{
					mparmoryCosmeticTauntSlotVM.ExecuteSelect();
					break;
				}
			}
		}

		public void ExecuteEmptyFocusedSlot()
		{
			foreach (MPArmoryCosmeticTauntSlotVM mparmoryCosmeticTauntSlotVM in this.Cosmetics.TauntSlots)
			{
				if (mparmoryCosmeticTauntSlotVM.IsFocused)
				{
					MPArmoryCosmeticTauntItemVM assignedTauntItem = mparmoryCosmeticTauntSlotVM.AssignedTauntItem;
					if (assignedTauntItem != null && assignedTauntItem.IsUsed)
					{
						mparmoryCosmeticTauntSlotVM.AssignedTauntItem.ExecuteAction();
					}
					mparmoryCosmeticTauntSlotVM.EmptySlotKeyVisual.SetForcedVisibility(new bool?(false));
					mparmoryCosmeticTauntSlotVM.SelectKeyVisual.SetForcedVisibility(new bool?(false));
					break;
				}
			}
		}

		private void OnSelectPerk(HeroPerkVM heroPerk, MPPerkVM candidate)
		{
			if (this.ClassStats.HeroInformation.HeroClass != null && this.HeroPerkSelection.CurrentHeroClass != null)
			{
				List<IReadOnlyPerkObject> currentSelectedPerks = this.HeroPerkSelection.CurrentSelectedPerks;
				if (currentSelectedPerks.Count > 0)
				{
					this.ClassStats.HeroInformation.RefreshWith(this.HeroPerkSelection.CurrentHeroClass, currentSelectedPerks);
					this.HeroPreview.SetCharacterPerks(currentSelectedPerks);
					this.Cosmetics.RefreshSelectedClass(this._currentClassItem.HeroClass, currentSelectedPerks);
				}
			}
		}

		private void RemoveHeroPreviewItem(MPArmoryCosmeticItemBaseVM itemVM)
		{
			MPArmoryCosmeticClothingItemVM mparmoryCosmeticClothingItemVM;
			if ((mparmoryCosmeticClothingItemVM = itemVM as MPArmoryCosmeticClothingItemVM) != null)
			{
				EquipmentIndex cosmeticEquipmentIndex = mparmoryCosmeticClothingItemVM.EquipmentElement.Item.GetCosmeticEquipmentIndex();
				MPArmoryHeroPreviewVM heroPreview = this.HeroPreview;
				if (heroPreview != null)
				{
					heroPreview.HeroVisual.SetEquipment(cosmeticEquipmentIndex, default(EquipmentElement));
				}
				MPArmoryHeroPreviewVM heroPreview2 = this.HeroPreview;
				string text = ((heroPreview2 != null) ? heroPreview2.HeroVisual.EquipmentCode : null);
				if (!string.IsNullOrEmpty(text))
				{
					this._lastValidEquipment = Equipment.CreateFromEquipmentCode(text);
				}
			}
		}

		private void OnHeroPreviewItemEquipped(MPArmoryCosmeticItemBaseVM itemVM)
		{
			MPArmoryCosmeticClothingItemVM mparmoryCosmeticClothingItemVM;
			MPArmoryCosmeticTauntItemVM mparmoryCosmeticTauntItemVM;
			if ((mparmoryCosmeticClothingItemVM = itemVM as MPArmoryCosmeticClothingItemVM) != null)
			{
				EquipmentElement equipmentElement = mparmoryCosmeticClothingItemVM.EquipmentElement;
				EquipmentIndex cosmeticEquipmentIndex = equipmentElement.Item.GetCosmeticEquipmentIndex();
				MPArmoryHeroPreviewVM heroPreview = this.HeroPreview;
				if (heroPreview != null)
				{
					heroPreview.HeroVisual.SetEquipment(cosmeticEquipmentIndex, equipmentElement);
				}
				MPArmoryHeroPreviewVM heroPreview2 = this.HeroPreview;
				string text = ((heroPreview2 != null) ? heroPreview2.HeroVisual.EquipmentCode : null);
				if (!string.IsNullOrEmpty(text))
				{
					this._lastValidEquipment = Equipment.CreateFromEquipmentCode(text);
					return;
				}
			}
			else if ((mparmoryCosmeticTauntItemVM = itemVM as MPArmoryCosmeticTauntItemVM) != null)
			{
				MPArmoryHeroPreviewVM heroPreview3 = this.HeroPreview;
				if (((heroPreview3 != null) ? heroPreview3.HeroVisual : null) != null)
				{
					this.HeroPreview.HeroVisual.ExecuteStopCustomAnimation();
					if (this._lastValidEquipment != null)
					{
						this.HeroPreview.HeroVisual.SetEquipment(this._lastValidEquipment);
					}
				}
				this._tauntItemToRefreshNextAnimationWith = mparmoryCosmeticTauntItemVM;
			}
		}

		private void OnCharacterCustomAnimationFinished(CharacterViewModel character)
		{
			if (character == this.HeroPreview.HeroVisual && this._lastValidEquipment != null)
			{
				MPArmoryHeroPreviewVM heroPreview = this.HeroPreview;
				if (((heroPreview != null) ? heroPreview.HeroVisual : null) != null)
				{
					this.HeroPreview.HeroVisual.SetEquipment(this._lastValidEquipment);
					this.HeroPreview.HeroVisual.LeftHandWieldedEquipmentIndex = -1;
					this.HeroPreview.HeroVisual.RightHandWieldedEquipmentIndex = -1;
					this._currentTauntPreviewAnimationSource.PreviewAnimationRatio = 0f;
					this._currentTauntPreviewAnimationSource = null;
				}
			}
		}

		private void OnTauntAssignmentRefresh()
		{
			this.IsTauntAssignmentActive = this.Cosmetics.SelectedTauntItem != null || this.Cosmetics.SelectedTauntSlot != null;
		}

		private void ResetHeroEquipment()
		{
			MPArmoryHeroPreviewVM heroPreview = this.HeroPreview;
			if (heroPreview == null)
			{
				return;
			}
			heroPreview.HeroVisual.SetEquipment(new Equipment());
		}

		public static void ApplyPerkEffectsToEquipment(ref Equipment equipment, List<IReadOnlyPerkObject> selectedPerks)
		{
			MPPerkObject.MPOnSpawnPerkHandler onSpawnPerkHandler = MPPerkObject.GetOnSpawnPerkHandler(selectedPerks);
			IEnumerable<ValueTuple<EquipmentIndex, EquipmentElement>> enumerable = ((onSpawnPerkHandler != null) ? onSpawnPerkHandler.GetAlternativeEquipments(true) : null);
			if (enumerable != null)
			{
				foreach (ValueTuple<EquipmentIndex, EquipmentElement> valueTuple in enumerable)
				{
					equipment[valueTuple.Item1] = valueTuple.Item2;
				}
			}
		}

		private List<IReadOnlyPerkObject> GetSelectedPerks()
		{
			return this.HeroPerkSelection.CurrentSelectedPerks;
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
					this.OnIsEnabledChanged();
				}
			}
		}

		[DataSourceProperty]
		public bool IsManagingTaunts
		{
			get
			{
				return this._isManagingTaunts;
			}
			set
			{
				if (value != this._isManagingTaunts)
				{
					this._isManagingTaunts = value;
					base.OnPropertyChangedWithValue(value, "IsManagingTaunts");
				}
			}
		}

		[DataSourceProperty]
		public bool IsTauntAssignmentActive
		{
			get
			{
				return this._isTauntAssignmentActive;
			}
			set
			{
				if (value != this._isTauntAssignmentActive)
				{
					this._isTauntAssignmentActive = value;
					base.OnPropertyChangedWithValue(value, "IsTauntAssignmentActive");
					if (this._isTauntAssignmentActive)
					{
						Func<string> getExitText = this._getExitText;
						this.TauntAssignmentClickToCloseText = ((getExitText != null) ? getExitText() : null);
					}
				}
			}
		}

		[DataSourceProperty]
		public bool CanOpenFacegen
		{
			get
			{
				return this._canOpenFacegen;
			}
			set
			{
				if (value != this._canOpenFacegen)
				{
					this._canOpenFacegen = value;
					base.OnPropertyChangedWithValue(value, "CanOpenFacegen");
				}
			}
		}

		[DataSourceProperty]
		public MPLobbyClassFilterVM ClassFilter
		{
			get
			{
				return this._classFilter;
			}
			set
			{
				if (value != this._classFilter)
				{
					this._classFilter = value;
					base.OnPropertyChangedWithValue<MPLobbyClassFilterVM>(value, "ClassFilter");
				}
			}
		}

		[DataSourceProperty]
		public MPArmoryHeroPreviewVM HeroPreview
		{
			get
			{
				return this._heroPreview;
			}
			set
			{
				if (value != this._heroPreview)
				{
					this._heroPreview = value;
					base.OnPropertyChangedWithValue<MPArmoryHeroPreviewVM>(value, "HeroPreview");
				}
			}
		}

		[DataSourceProperty]
		public MPArmoryClassStatsVM ClassStats
		{
			get
			{
				return this._classStats;
			}
			set
			{
				if (value != this._classStats)
				{
					this._classStats = value;
					base.OnPropertyChangedWithValue<MPArmoryClassStatsVM>(value, "ClassStats");
				}
			}
		}

		[DataSourceProperty]
		public MPArmoryHeroPerkSelectionVM HeroPerkSelection
		{
			get
			{
				return this._heroPerkSelection;
			}
			set
			{
				if (value != this._heroPerkSelection)
				{
					this._heroPerkSelection = value;
					base.OnPropertyChangedWithValue<MPArmoryHeroPerkSelectionVM>(value, "HeroPerkSelection");
				}
			}
		}

		[DataSourceProperty]
		public MPArmoryCosmeticsVM Cosmetics
		{
			get
			{
				return this._cosmetics;
			}
			set
			{
				if (value != this._cosmetics)
				{
					this._cosmetics = value;
					base.OnPropertyChangedWithValue<MPArmoryCosmeticsVM>(value, "Cosmetics");
				}
			}
		}

		[DataSourceProperty]
		public string TauntAssignmentClickToCloseText
		{
			get
			{
				return this._tauntAssignmentClickToCloseText;
			}
			set
			{
				if (value != this._tauntAssignmentClickToCloseText)
				{
					this._tauntAssignmentClickToCloseText = value;
					base.OnPropertyChangedWithValue<string>(value, "TauntAssignmentClickToCloseText");
				}
			}
		}

		[DataSourceProperty]
		public string StatsText
		{
			get
			{
				return this._statsText;
			}
			set
			{
				if (value != this._statsText)
				{
					this._statsText = value;
					base.OnPropertyChangedWithValue<string>(value, "StatsText");
				}
			}
		}

		[DataSourceProperty]
		public string CustomizationText
		{
			get
			{
				return this._customizationText;
			}
			set
			{
				if (value != this._customizationText)
				{
					this._customizationText = value;
					base.OnPropertyChangedWithValue<string>(value, "CustomizationText");
				}
			}
		}

		[DataSourceProperty]
		public string FacegenText
		{
			get
			{
				return this._facegenText;
			}
			set
			{
				if (value != this._facegenText)
				{
					this._facegenText = value;
					base.OnPropertyChangedWithValue<string>(value, "FacegenText");
				}
			}
		}

		[DataSourceProperty]
		public string ManageTauntsText
		{
			get
			{
				return this._manageTauntsText;
			}
			set
			{
				if (value != this._manageTauntsText)
				{
					this._manageTauntsText = value;
					base.OnPropertyChangedWithValue<string>(value, "ManageTauntsText");
				}
			}
		}

		private readonly Action<BasicCharacterObject> _onOpenFacegen;

		private bool _canOpenFacegenBeforeTauntState;

		private BasicCharacterObject _character;

		private MPLobbyClassFilterClassItemVM _currentClassItem;

		private Equipment _lastValidEquipment;

		private Func<string> _getExitText;

		private MPArmoryCosmeticTauntItemVM _tauntItemToRefreshNextAnimationWith;

		private MPArmoryCosmeticTauntItemVM _currentTauntPreviewAnimationSource;

		private bool _isEnabled;

		private bool _isManagingTaunts;

		private bool _isTauntAssignmentActive;

		private bool _canOpenFacegen;

		private MPLobbyClassFilterVM _classFilter;

		private MPArmoryHeroPreviewVM _heroPreview;

		private MPArmoryClassStatsVM _classStats;

		private MPArmoryHeroPerkSelectionVM _heroPerkSelection;

		private MPArmoryCosmeticsVM _cosmetics;

		private string _tauntAssignmentClickToCloseText;

		private string _statsText;

		private string _customizationText;

		private string _facegenText;

		private string _manageTauntsText;
	}
}
