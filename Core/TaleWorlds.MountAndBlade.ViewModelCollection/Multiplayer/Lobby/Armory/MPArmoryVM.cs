using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.ClassFilter;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Armory
{
	public class MPArmoryVM : ViewModel
	{
		public MPArmoryVM(Action<BasicCharacterObject> onOpenFacegen, Action<MPArmoryCosmeticItemVM> onItemObtainRequested)
		{
			this._onOpenFacegen = onOpenFacegen;
			this._character = MBObjectManager.Instance.GetObject<BasicCharacterObject>("mp_character");
			this.CanOpenFacegen = true;
			this.ClassFilter = new MPLobbyClassFilterVM(new Action<MPLobbyClassFilterClassItemVM, bool>(this.OnSelectedClassChanged));
			this.HeroPreview = new MPArmoryHeroPreviewVM();
			this.ClassStats = new MPArmoryClassStatsVM();
			this.HeroPerkSelection = new MPArmoryHeroPerkSelectionVM(new Action<HeroPerkVM, MPPerkVM>(this.OnSelectPerk), new Action(this.ForceRefreshCharacter));
			this.Cosmetics = new MPArmoryCosmeticsVM(new Action<EquipmentIndex, EquipmentElement>(this.OnHeroPreviewItemEquipped), new Action(this.ResetHeroEquipment), onItemObtainRequested, new Func<List<IReadOnlyPerkObject>>(this.GetSelectedPerks));
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.StatsText = new TextObject("{=ffjTMejn}Stats", null).ToString();
			this.CustomizationText = new TextObject("{=sPkRekRL}Customization", null).ToString();
			this.FacegenText = new TextObject("{=RSx1e5Wf}Edit Character", null).ToString();
			this.ClassFilter.RefreshValues();
			this.HeroPreview.RefreshValues();
			this.ClassStats.RefreshValues();
			this.Cosmetics.RefreshValues();
			this.HeroPerkSelection.RefreshValues();
		}

		public override void OnFinalize()
		{
			this._heroPreview = null;
			this._character = null;
			base.OnFinalize();
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
				this.Cosmetics.RefreshCosmeticsInfo();
				return;
			}
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
			this.Cosmetics.RefreshCosmeticsInfo();
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

		private void OnHeroPreviewItemEquipped(EquipmentIndex equipmentIndex, EquipmentElement equipmentElement)
		{
			MPArmoryHeroPreviewVM heroPreview = this._heroPreview;
			if (heroPreview == null)
			{
				return;
			}
			heroPreview.HeroVisual.SetEquipment(equipmentIndex, equipmentElement);
		}

		private void ResetHeroEquipment()
		{
			MPArmoryHeroPreviewVM heroPreview = this._heroPreview;
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

		private readonly Action<BasicCharacterObject> _onOpenFacegen;

		private BasicCharacterObject _character;

		private MPLobbyClassFilterClassItemVM _currentClassItem;

		private bool _isEnabled;

		private bool _canOpenFacegen;

		private MPLobbyClassFilterVM _classFilter;

		private MPArmoryHeroPreviewVM _heroPreview;

		private MPArmoryClassStatsVM _classStats;

		private MPArmoryHeroPerkSelectionVM _heroPerkSelection;

		private MPArmoryCosmeticsVM _cosmetics;

		private string _statsText;

		private string _customizationText;

		private string _facegenText;
	}
}
