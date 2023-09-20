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
	// Token: 0x020000A9 RID: 169
	public class MPArmoryVM : ViewModel
	{
		// Token: 0x0600102F RID: 4143 RVA: 0x00035CD8 File Offset: 0x00033ED8
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

		// Token: 0x06001030 RID: 4144 RVA: 0x00035D94 File Offset: 0x00033F94
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

		// Token: 0x06001031 RID: 4145 RVA: 0x00035E20 File Offset: 0x00034020
		public override void OnFinalize()
		{
			this._heroPreview = null;
			this._character = null;
			base.OnFinalize();
		}

		// Token: 0x06001032 RID: 4146 RVA: 0x00035E38 File Offset: 0x00034038
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

		// Token: 0x06001033 RID: 4147 RVA: 0x00035EC0 File Offset: 0x000340C0
		public void ForceRefreshCharacter()
		{
			this.OnSelectedClassChanged(this.ClassFilter.SelectedClassItem, true);
		}

		// Token: 0x06001034 RID: 4148 RVA: 0x00035ED4 File Offset: 0x000340D4
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

		// Token: 0x06001035 RID: 4149 RVA: 0x00035F68 File Offset: 0x00034168
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

		// Token: 0x06001036 RID: 4150 RVA: 0x00036066 File Offset: 0x00034266
		public void SetCanOpenFacegen(bool enabled)
		{
			this.CanOpenFacegen = enabled;
		}

		// Token: 0x06001037 RID: 4151 RVA: 0x0003606F File Offset: 0x0003426F
		private void ExecuteOpenFaceGen()
		{
			Action<BasicCharacterObject> onOpenFacegen = this._onOpenFacegen;
			if (onOpenFacegen == null)
			{
				return;
			}
			onOpenFacegen(this._character);
		}

		// Token: 0x06001038 RID: 4152 RVA: 0x00036088 File Offset: 0x00034288
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

		// Token: 0x06001039 RID: 4153 RVA: 0x00036108 File Offset: 0x00034308
		private void OnHeroPreviewItemEquipped(EquipmentIndex equipmentIndex, EquipmentElement equipmentElement)
		{
			MPArmoryHeroPreviewVM heroPreview = this._heroPreview;
			if (heroPreview == null)
			{
				return;
			}
			heroPreview.HeroVisual.SetEquipment(equipmentIndex, equipmentElement);
		}

		// Token: 0x0600103A RID: 4154 RVA: 0x00036121 File Offset: 0x00034321
		private void ResetHeroEquipment()
		{
			MPArmoryHeroPreviewVM heroPreview = this._heroPreview;
			if (heroPreview == null)
			{
				return;
			}
			heroPreview.HeroVisual.SetEquipment(new Equipment());
		}

		// Token: 0x0600103B RID: 4155 RVA: 0x00036140 File Offset: 0x00034340
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

		// Token: 0x0600103C RID: 4156 RVA: 0x000361AC File Offset: 0x000343AC
		private List<IReadOnlyPerkObject> GetSelectedPerks()
		{
			return this.HeroPerkSelection.CurrentSelectedPerks;
		}

		// Token: 0x1700052F RID: 1327
		// (get) Token: 0x0600103D RID: 4157 RVA: 0x000361B9 File Offset: 0x000343B9
		// (set) Token: 0x0600103E RID: 4158 RVA: 0x000361C1 File Offset: 0x000343C1
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

		// Token: 0x17000530 RID: 1328
		// (get) Token: 0x0600103F RID: 4159 RVA: 0x000361E5 File Offset: 0x000343E5
		// (set) Token: 0x06001040 RID: 4160 RVA: 0x000361ED File Offset: 0x000343ED
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

		// Token: 0x17000531 RID: 1329
		// (get) Token: 0x06001041 RID: 4161 RVA: 0x0003620B File Offset: 0x0003440B
		// (set) Token: 0x06001042 RID: 4162 RVA: 0x00036213 File Offset: 0x00034413
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

		// Token: 0x17000532 RID: 1330
		// (get) Token: 0x06001043 RID: 4163 RVA: 0x00036231 File Offset: 0x00034431
		// (set) Token: 0x06001044 RID: 4164 RVA: 0x00036239 File Offset: 0x00034439
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

		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x06001045 RID: 4165 RVA: 0x00036257 File Offset: 0x00034457
		// (set) Token: 0x06001046 RID: 4166 RVA: 0x0003625F File Offset: 0x0003445F
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

		// Token: 0x17000534 RID: 1332
		// (get) Token: 0x06001047 RID: 4167 RVA: 0x0003627D File Offset: 0x0003447D
		// (set) Token: 0x06001048 RID: 4168 RVA: 0x00036285 File Offset: 0x00034485
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

		// Token: 0x17000535 RID: 1333
		// (get) Token: 0x06001049 RID: 4169 RVA: 0x000362A3 File Offset: 0x000344A3
		// (set) Token: 0x0600104A RID: 4170 RVA: 0x000362AB File Offset: 0x000344AB
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

		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x0600104B RID: 4171 RVA: 0x000362C9 File Offset: 0x000344C9
		// (set) Token: 0x0600104C RID: 4172 RVA: 0x000362D1 File Offset: 0x000344D1
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

		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x0600104D RID: 4173 RVA: 0x000362F4 File Offset: 0x000344F4
		// (set) Token: 0x0600104E RID: 4174 RVA: 0x000362FC File Offset: 0x000344FC
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

		// Token: 0x17000538 RID: 1336
		// (get) Token: 0x0600104F RID: 4175 RVA: 0x0003631F File Offset: 0x0003451F
		// (set) Token: 0x06001050 RID: 4176 RVA: 0x00036327 File Offset: 0x00034527
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

		// Token: 0x040007A9 RID: 1961
		private readonly Action<BasicCharacterObject> _onOpenFacegen;

		// Token: 0x040007AA RID: 1962
		private BasicCharacterObject _character;

		// Token: 0x040007AB RID: 1963
		private MPLobbyClassFilterClassItemVM _currentClassItem;

		// Token: 0x040007AC RID: 1964
		private bool _isEnabled;

		// Token: 0x040007AD RID: 1965
		private bool _canOpenFacegen;

		// Token: 0x040007AE RID: 1966
		private MPLobbyClassFilterVM _classFilter;

		// Token: 0x040007AF RID: 1967
		private MPArmoryHeroPreviewVM _heroPreview;

		// Token: 0x040007B0 RID: 1968
		private MPArmoryClassStatsVM _classStats;

		// Token: 0x040007B1 RID: 1969
		private MPArmoryHeroPerkSelectionVM _heroPerkSelection;

		// Token: 0x040007B2 RID: 1970
		private MPArmoryCosmeticsVM _cosmetics;

		// Token: 0x040007B3 RID: 1971
		private string _statsText;

		// Token: 0x040007B4 RID: 1972
		private string _customizationText;

		// Token: 0x040007B5 RID: 1973
		private string _facegenText;
	}
}
