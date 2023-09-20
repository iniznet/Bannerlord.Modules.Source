using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Options;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GameKeys;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GamepadOptions;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions
{
	// Token: 0x020000FC RID: 252
	public class OptionsVM : ViewModel
	{
		// Token: 0x17000743 RID: 1859
		// (get) Token: 0x06001623 RID: 5667 RVA: 0x00047096 File Offset: 0x00045296
		// (set) Token: 0x06001624 RID: 5668 RVA: 0x0004709E File Offset: 0x0004529E
		public OptionsVM.OptionsMode CurrentOptionsMode { get; private set; }

		// Token: 0x06001625 RID: 5669 RVA: 0x000470A8 File Offset: 0x000452A8
		public OptionsVM(bool autoHandleClose, OptionsVM.OptionsMode optionsMode, Action<KeyOptionVM> onKeybindRequest, Action onBrightnessExecute = null, Action onExposureExecute = null)
		{
			this._onKeybindRequest = onKeybindRequest;
			this._autoHandleClose = autoHandleClose;
			this.CurrentOptionsMode = optionsMode;
			this._onBrightnessExecute = onBrightnessExecute;
			this._onExposureExecute = onExposureExecute;
			this._groupedCategories = new List<GroupedOptionCategoryVM>();
			NativeOptions.RefreshOptionsData();
			bool flag = this.CurrentOptionsMode == OptionsVM.OptionsMode.Multiplayer;
			this._gameplayOptionCategory = new GroupedOptionCategoryVM(this, new TextObject("{=2zcrC0h1}Gameplay", null), OptionsProvider.GetGameplayOptionCategory(flag), true, true);
			this._audioOptionCategory = new GroupedOptionCategoryVM(this, new TextObject("{=xebFLnH2}Audio", null), OptionsProvider.GetAudioOptionCategory(flag), true, false);
			this._videoOptionCategory = new GroupedOptionCategoryVM(this, new TextObject("{=gamevideo}Video", null), OptionsProvider.GetVideoOptionCategory(this.CurrentOptionsMode == OptionsVM.OptionsMode.MainMenu, new Action(this.OnBrightnessClick), new Action(this.OnExposureClick), new Action(this.ExecuteBenchmark)), true, false);
			bool flag2 = true;
			this._performanceOptionCategory = new GroupedOptionCategoryVM(this, new TextObject("{=fM9E7frB}Performance", null), OptionsProvider.GetPerformanceOptionCategory(flag), flag2, false);
			this._groupedCategories.Add(this._videoOptionCategory);
			this._groupedCategories.Add(this._audioOptionCategory);
			this._groupedCategories.Add(this._gameplayOptionCategory);
			this._performanceManagedOptions = this._performanceOptionCategory.GetManagedOptions();
			this._gameKeyCategory = new GameKeyOptionCategoryVM(this._onKeybindRequest, OptionsProvider.GetGameKeyCategoriesList(this.CurrentOptionsMode == OptionsVM.OptionsMode.Multiplayer));
			TextObject textObject = new TextObject("{=SQpGQzTI}Controller", null);
			this._gamepadCategory = new GamepadOptionCategoryVM(this, textObject, OptionsProvider.GetControllerOptionCategory(), true, true);
			this._categories = new List<ViewModel>();
			this._categories.Add(this._videoOptionCategory);
			this._categories.Add(this._performanceOptionCategory);
			this._categories.Add(this._audioOptionCategory);
			this._categories.Add(this._gameplayOptionCategory);
			this._categories.Add(this._gameKeyCategory);
			this._categories.Add(this._gamepadCategory);
			this.SetSelectedCategory(0);
			if (onBrightnessExecute == null)
			{
				this.BrightnessPopUp = new BrightnessOptionVM(null);
			}
			if (onExposureExecute == null)
			{
				this.ExposurePopUp = new ExposureOptionVM(null);
			}
			if (Game.Current != null && this._autoHandleClose)
			{
				Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
			}
			this._refreshRateOption = this.VideoOptions.GetOption(NativeOptions.NativeOptionsType.RefreshRate);
			this._resolutionOption = this.VideoOptions.GetOption(NativeOptions.NativeOptionsType.ScreenResolution);
			this._monitorOption = this.VideoOptions.GetOption(NativeOptions.NativeOptionsType.SelectedMonitor);
			this._overallOption = this.PerformanceOptions.GetOption(NativeOptions.NativeOptionsType.OverAll) as StringOptionDataVM;
			this._dlssOption = this.PerformanceOptions.GetOption(NativeOptions.NativeOptionsType.DLSS);
			this._dynamicResolutionOptions = new List<GenericOptionDataVM>
			{
				this.PerformanceOptions.GetOption(NativeOptions.NativeOptionsType.DynamicResolution),
				this.PerformanceOptions.GetOption(NativeOptions.NativeOptionsType.DynamicResolutionTarget)
			};
			this.IsConsole = true;
			GroupedOptionCategoryVM performanceOptionCategory = this._performanceOptionCategory;
			if (performanceOptionCategory != null)
			{
				performanceOptionCategory.InitializeDependentConfigs(new Action<IOptionData, float>(this.UpdateDependentConfigs));
			}
			this.IsConsole = false;
			this.GameVersionText = Utilities.GetApplicationVersionWithBuildNumber().ToString();
			this.RefreshValues();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Combine(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
			this._isInitialized = true;
		}

		// Token: 0x06001626 RID: 5670 RVA: 0x00047400 File Offset: 0x00045600
		public OptionsVM(OptionsVM.OptionsMode optionsMode, Action onClose, Action<KeyOptionVM> onKeybindRequest, Action onBrightnessExecute = null, Action onExposureExecute = null)
			: this(false, optionsMode, onKeybindRequest, null, null)
		{
			this._onClose = onClose;
			this._onBrightnessExecute = onBrightnessExecute;
			this._onExposureExecute = onExposureExecute;
		}

		// Token: 0x06001627 RID: 5671 RVA: 0x00047424 File Offset: 0x00045624
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.OptionsLbl = new TextObject("{=NqarFr4P}Options", null).ToString();
			this.CancelLbl = new TextObject("{=3CpNUnVl}Cancel", null).ToString();
			this.DoneLbl = new TextObject("{=WiNRdfsm}Done", null).ToString();
			this.ResetLbl = new TextObject("{=mAxXKaXp}Reset", null).ToString();
			this.VideoMemoryUsageName = Module.CurrentModule.GlobalTextManager.FindText("str_gpu_memory_usage", null).ToString();
			this.GameKeyOptionGroups.RefreshValues();
			this.GamepadOptions.RefreshValues();
			BrightnessOptionVM brightnessPopUp = this.BrightnessPopUp;
			if (brightnessPopUp != null)
			{
				brightnessPopUp.RefreshValues();
			}
			ExposureOptionVM exposurePopUp = this.ExposurePopUp;
			if (exposurePopUp != null)
			{
				exposurePopUp.RefreshValues();
			}
			this.UpdateVideoMemoryUsage();
			this._categories.ForEach(delegate(ViewModel g)
			{
				g.RefreshValues();
			});
		}

		// Token: 0x06001628 RID: 5672 RVA: 0x00047517 File Offset: 0x00045717
		public void ExecuteCloseOptions()
		{
			if (this._onClose != null)
			{
				this._onClose();
			}
		}

		// Token: 0x06001629 RID: 5673 RVA: 0x0004752C File Offset: 0x0004572C
		protected void OnBrightnessClick()
		{
			if (this._onBrightnessExecute == null)
			{
				this.BrightnessPopUp.Visible = true;
				return;
			}
			this._onBrightnessExecute();
		}

		// Token: 0x0600162A RID: 5674 RVA: 0x0004754E File Offset: 0x0004574E
		protected void OnExposureClick()
		{
			if (this._onExposureExecute == null)
			{
				this.ExposurePopUp.Visible = true;
				return;
			}
			this._onExposureExecute();
		}

		// Token: 0x0600162B RID: 5675 RVA: 0x00047570 File Offset: 0x00045770
		public ViewModel GetActiveCategory()
		{
			if (this.CategoryIndex >= 0 && this.CategoryIndex < this._categories.Count)
			{
				return this._categories[this.CategoryIndex];
			}
			return null;
		}

		// Token: 0x0600162C RID: 5676 RVA: 0x000475A1 File Offset: 0x000457A1
		public int GetIndexOfCategory(ViewModel categoryVM)
		{
			return this._categories.IndexOf(categoryVM);
		}

		// Token: 0x0600162D RID: 5677 RVA: 0x000475B0 File Offset: 0x000457B0
		public float GetConfig(IOptionData data)
		{
			if (!data.IsNative())
			{
				return ManagedOptions.GetConfig((ManagedOptions.ManagedOptionsType)data.GetOptionType());
			}
			NativeOptions.NativeOptionsType nativeOptionsType = (NativeOptions.NativeOptionsType)data.GetOptionType();
			if (nativeOptionsType == NativeOptions.NativeOptionsType.OverAll)
			{
				return (float)NativeConfig.AutoGFXQuality;
			}
			return NativeOptions.GetConfig(nativeOptionsType);
		}

		// Token: 0x0600162E RID: 5678 RVA: 0x000475F4 File Offset: 0x000457F4
		public void SetConfig(IOptionData data, float val)
		{
			if (!this._isInitialized)
			{
				return;
			}
			this.UpdateDependentConfigs(data, val);
			this.UpdateEnabledStates();
			NativeOptions.ConfigQuality autoGFXQuality = NativeConfig.AutoGFXQuality;
			NativeOptions.ConfigQuality configQuality = (this.IsManagedOptionsConflictWithOverallSettings((int)autoGFXQuality) ? NativeOptions.ConfigQuality.GFXCustom : autoGFXQuality);
			if (data.IsNative())
			{
				NativeOptions.NativeOptionsType nativeOptionsType = (NativeOptions.NativeOptionsType)data.GetOptionType();
				if (nativeOptionsType == NativeOptions.NativeOptionsType.OverAll)
				{
					if (MathF.Abs(val - (float)this._overallConfigCount) <= 0.01f)
					{
						goto IL_1AE;
					}
					Utilities.SetGraphicsPreset((int)val);
					foreach (GenericOptionDataVM genericOptionDataVM in this.VideoOptions.AllOptions)
					{
						if (!genericOptionDataVM.IsAction)
						{
							float num = (genericOptionDataVM.IsNative ? this.GetDefaultOptionForOverallNativeSettings((NativeOptions.NativeOptionsType)genericOptionDataVM.GetOptionType(), (int)val) : this.GetDefaultOptionForOverallManagedSettings((ManagedOptions.ManagedOptionsType)genericOptionDataVM.GetOptionType(), (int)val));
							if (num >= 0f)
							{
								genericOptionDataVM.SetValue(num);
								genericOptionDataVM.UpdateValue();
							}
						}
					}
					using (IEnumerator<GenericOptionDataVM> enumerator = this.PerformanceOptions.AllOptions.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							GenericOptionDataVM genericOptionDataVM2 = enumerator.Current;
							float num2 = (genericOptionDataVM2.IsNative ? this.GetDefaultOptionForOverallNativeSettings((NativeOptions.NativeOptionsType)genericOptionDataVM2.GetOptionType(), (int)val) : this.GetDefaultOptionForOverallManagedSettings((ManagedOptions.ManagedOptionsType)genericOptionDataVM2.GetOptionType(), (int)val));
							if (num2 >= 0f)
							{
								genericOptionDataVM2.SetValue(num2);
								genericOptionDataVM2.UpdateValue();
							}
						}
						goto IL_1AE;
					}
				}
				if (this._overallOption != null && (this._overallOption.Selector.SelectedIndex != this._overallConfigCount || configQuality != NativeOptions.ConfigQuality.GFXCustom) && OptionsProvider.GetDefaultNativeOptions().ContainsKey(nativeOptionsType))
				{
					this._overallOption.Selector.SelectedIndex = (int)configQuality;
				}
				IL_1AE:
				if (!this._isCancelling && (nativeOptionsType == NativeOptions.NativeOptionsType.SelectedAdapter || nativeOptionsType == NativeOptions.NativeOptionsType.SoundDevice || nativeOptionsType == NativeOptions.NativeOptionsType.SoundOutput))
				{
					InformationManager.ShowInquiry(new InquiryData(Module.CurrentModule.GlobalTextManager.FindText("str_option_restart_required", null).ToString(), Module.CurrentModule.GlobalTextManager.FindText("str_option_restart_required_desc", null).ToString(), true, false, Module.CurrentModule.GlobalTextManager.FindText("str_ok", null).ToString(), string.Empty, null, null, "", 0f, null, null, null), false, false);
				}
				this.UpdateVideoMemoryUsage();
				return;
			}
			ManagedOptions.ManagedOptionsType managedOptionsType = (ManagedOptions.ManagedOptionsType)data.GetOptionType();
			if (this._overallOption != null && (this._overallOption.Selector.SelectedIndex != this._overallConfigCount || configQuality != NativeOptions.ConfigQuality.GFXCustom) && OptionsProvider.GetDefaultManagedOptions().ContainsKey(managedOptionsType))
			{
				this._overallOption.Selector.SelectedIndex = (int)configQuality;
			}
		}

		// Token: 0x0600162F RID: 5679 RVA: 0x000478A8 File Offset: 0x00045AA8
		private void UpdateEnabledStates()
		{
			foreach (GenericOptionDataVM genericOptionDataVM in this._groupedCategories.SelectMany((GroupedOptionCategoryVM c) => c.AllOptions))
			{
				genericOptionDataVM.UpdateEnableState();
			}
			foreach (GenericOptionDataVM genericOptionDataVM2 in this._performanceOptionCategory.AllOptions)
			{
				genericOptionDataVM2.UpdateEnableState();
			}
		}

		// Token: 0x06001630 RID: 5680 RVA: 0x00047954 File Offset: 0x00045B54
		private void UpdateDependentConfigs(IOptionData data, float val)
		{
			if (data.IsNative())
			{
				NativeOptions.NativeOptionsType nativeOptionsType = (NativeOptions.NativeOptionsType)data.GetOptionType();
				if (nativeOptionsType == NativeOptions.NativeOptionsType.SelectedMonitor || nativeOptionsType == NativeOptions.NativeOptionsType.DLSS)
				{
					NativeOptions.RefreshOptionsData();
					this._resolutionOption.UpdateData(false);
					this._refreshRateOption.UpdateData(false);
				}
				if (nativeOptionsType == NativeOptions.NativeOptionsType.ScreenResolution || nativeOptionsType == NativeOptions.NativeOptionsType.SelectedMonitor)
				{
					NativeOptions.RefreshOptionsData();
					this._refreshRateOption.UpdateData(false);
					if (NativeOptions.GetIsDLSSAvailable())
					{
						GenericOptionDataVM dlssOption = this._dlssOption;
						if (dlssOption == null)
						{
							return;
						}
						dlssOption.UpdateData(false);
					}
				}
			}
		}

		// Token: 0x06001631 RID: 5681 RVA: 0x000479D0 File Offset: 0x00045BD0
		private bool IsManagedOptionsConflictWithOverallSettings(int overallSettingsOption)
		{
			return this._performanceManagedOptions.Any((IOptionData o) => (float)((int)o.GetValue(false)) != this.GetDefaultOptionForOverallManagedSettings((ManagedOptions.ManagedOptionsType)o.GetOptionType(), overallSettingsOption));
		}

		// Token: 0x06001632 RID: 5682 RVA: 0x00047A08 File Offset: 0x00045C08
		private float GetDefaultOptionForOverallNativeSettings(NativeOptions.NativeOptionsType option, int overallSettingsOption)
		{
			if (overallSettingsOption >= this._overallConfigCount || overallSettingsOption < 0)
			{
				return -1f;
			}
			float[] array;
			if (OptionsProvider.GetDefaultNativeOptions().TryGetValue(option, out array))
			{
				return array[overallSettingsOption];
			}
			return -1f;
		}

		// Token: 0x06001633 RID: 5683 RVA: 0x00047A40 File Offset: 0x00045C40
		private float GetDefaultOptionForOverallManagedSettings(ManagedOptions.ManagedOptionsType option, int overallSettingsOption)
		{
			if (overallSettingsOption >= this._overallConfigCount || overallSettingsOption < 0)
			{
				return -1f;
			}
			float[] array;
			if (OptionsProvider.GetDefaultManagedOptions().TryGetValue(option, out array))
			{
				return array[overallSettingsOption];
			}
			return -1f;
		}

		// Token: 0x06001634 RID: 5684 RVA: 0x00047A78 File Offset: 0x00045C78
		private bool IsCategoryAvailable(ViewModel category)
		{
			GameKeyOptionCategoryVM gameKeyOptionCategoryVM;
			if ((gameKeyOptionCategoryVM = category as GameKeyOptionCategoryVM) != null)
			{
				return gameKeyOptionCategoryVM.IsEnabled;
			}
			GamepadOptionCategoryVM gamepadOptionCategoryVM;
			if ((gamepadOptionCategoryVM = category as GamepadOptionCategoryVM) != null)
			{
				return gamepadOptionCategoryVM.IsEnabled;
			}
			GroupedOptionCategoryVM groupedOptionCategoryVM;
			return (groupedOptionCategoryVM = category as GroupedOptionCategoryVM) == null || groupedOptionCategoryVM.IsEnabled;
		}

		// Token: 0x06001635 RID: 5685 RVA: 0x00047AB9 File Offset: 0x00045CB9
		private int GetPreviousAvailableCategoryIndex(int currentCategoryIndex)
		{
			if (--currentCategoryIndex < 0)
			{
				currentCategoryIndex = this._categories.Count - 1;
			}
			if (!this.IsCategoryAvailable(this._categories[currentCategoryIndex]))
			{
				return this.GetPreviousAvailableCategoryIndex(currentCategoryIndex);
			}
			return currentCategoryIndex;
		}

		// Token: 0x06001636 RID: 5686 RVA: 0x00047AF0 File Offset: 0x00045CF0
		public void SelectPreviousCategory()
		{
			int previousAvailableCategoryIndex = this.GetPreviousAvailableCategoryIndex(this.CategoryIndex);
			this.SetSelectedCategory(previousAvailableCategoryIndex);
		}

		// Token: 0x06001637 RID: 5687 RVA: 0x00047B11 File Offset: 0x00045D11
		private int GetNextAvailableCategoryIndex(int currentCategoryIndex)
		{
			if (++currentCategoryIndex >= this._categories.Count)
			{
				currentCategoryIndex = 0;
			}
			if (!this.IsCategoryAvailable(this._categories[currentCategoryIndex]))
			{
				return this.GetNextAvailableCategoryIndex(currentCategoryIndex);
			}
			return currentCategoryIndex;
		}

		// Token: 0x06001638 RID: 5688 RVA: 0x00047B48 File Offset: 0x00045D48
		public void SelectNextCategory()
		{
			int nextAvailableCategoryIndex = this.GetNextAvailableCategoryIndex(this.CategoryIndex);
			this.SetSelectedCategory(nextAvailableCategoryIndex);
		}

		// Token: 0x06001639 RID: 5689 RVA: 0x00047B69 File Offset: 0x00045D69
		private void SetSelectedCategory(int index)
		{
			this.CategoryIndex = index;
		}

		// Token: 0x0600163A RID: 5690 RVA: 0x00047B72 File Offset: 0x00045D72
		private void OnGamepadActiveStateChanged()
		{
			if (!this.IsCategoryAvailable(this._categories[this.CategoryIndex]))
			{
				if (this.GetNextAvailableCategoryIndex(this.CategoryIndex) > this.CategoryIndex)
				{
					this.SelectNextCategory();
					return;
				}
				this.SelectPreviousCategory();
			}
		}

		// Token: 0x0600163B RID: 5691 RVA: 0x00047BB0 File Offset: 0x00045DB0
		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey != null)
			{
				doneInputKey.OnFinalize();
			}
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey != null)
			{
				cancelInputKey.OnFinalize();
			}
			InputKeyItemVM previousTabInputKey = this.PreviousTabInputKey;
			if (previousTabInputKey != null)
			{
				previousTabInputKey.OnFinalize();
			}
			InputKeyItemVM nextTabInputKey = this.NextTabInputKey;
			if (nextTabInputKey != null)
			{
				nextTabInputKey.OnFinalize();
			}
			GamepadOptionCategoryVM gamepadOptions = this.GamepadOptions;
			if (gamepadOptions != null)
			{
				gamepadOptions.OnFinalize();
			}
			GameKeyOptionCategoryVM gameKeyOptionGroups = this.GameKeyOptionGroups;
			if (gameKeyOptionGroups != null)
			{
				gameKeyOptionGroups.OnFinalize();
			}
			ExposureOptionVM exposurePopUp = this.ExposurePopUp;
			if (exposurePopUp != null)
			{
				exposurePopUp.OnFinalize();
			}
			BrightnessOptionVM brightnessPopUp = this.BrightnessPopUp;
			if (brightnessPopUp != null)
			{
				brightnessPopUp.OnFinalize();
			}
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
		}

		// Token: 0x0600163C RID: 5692 RVA: 0x00047C6C File Offset: 0x00045E6C
		protected void HandleCancel(bool autoHandleClose)
		{
			this._isCancelling = true;
			this._groupedCategories.ForEach(delegate(GroupedOptionCategoryVM c)
			{
				c.Cancel();
			});
			this._gameKeyCategory.Cancel();
			this._performanceOptionCategory.Cancel();
			this.CloseScreen(autoHandleClose);
		}

		// Token: 0x0600163D RID: 5693 RVA: 0x00047CC7 File Offset: 0x00045EC7
		private void CloseScreen(bool autoHandleClose)
		{
			this.ExecuteCloseOptions();
			if (autoHandleClose)
			{
				if (Game.Current != null)
				{
					Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
				}
				ScreenManager.PopScreen();
			}
		}

		// Token: 0x0600163E RID: 5694 RVA: 0x00047CF0 File Offset: 0x00045EF0
		public void ExecuteCancel()
		{
			if (this.IsOptionsChanged())
			{
				string text = new TextObject("{=peUP9ZZj}Are you sure? You made some changes and they will be lost.", null).ToString();
				InformationManager.ShowInquiry(new InquiryData("", text, true, true, new TextObject("{=aeouhelq}Yes", null).ToString(), new TextObject("{=8OkPHu4f}No", null).ToString(), delegate
				{
					this.HandleCancel(this._autoHandleClose);
				}, null, "", 0f, null, null, null), false, false);
				return;
			}
			this.HandleCancel(this._autoHandleClose);
		}

		// Token: 0x0600163F RID: 5695 RVA: 0x00047D74 File Offset: 0x00045F74
		protected void OnDone()
		{
			this.ApplyChangedOptions();
			GenericOptionDataVM resolutionOption = this._resolutionOption;
			if (resolutionOption == null || !resolutionOption.IsChanged())
			{
				GenericOptionDataVM monitorOption = this._monitorOption;
				if (monitorOption == null || !monitorOption.IsChanged())
				{
					this.CloseScreen(this._autoHandleClose);
					return;
				}
			}
			InformationManager.ShowInquiry(new InquiryData(new TextObject("{=m7vOLTpp}Screen Resolution Has Been Changed", null).ToString(), new TextObject("{=pK4EyTZC}Do you want to keep these settings?", null).ToString(), true, true, Module.CurrentModule.GlobalTextManager.FindText("str_ok", null).ToString(), new TextObject("{=3CpNUnVl}Cancel", null).ToString(), delegate
			{
				this.CloseScreen(this._autoHandleClose);
			}, delegate
			{
				this._monitorOption.Cancel();
				this._resolutionOption.Cancel();
				NativeOptions.ApplyConfigChanges(true);
			}, "", 10f, delegate
			{
				this._monitorOption.Cancel();
				this._resolutionOption.Cancel();
				NativeOptions.ApplyConfigChanges(true);
			}, null, null), false, false);
		}

		// Token: 0x06001640 RID: 5696 RVA: 0x00047E48 File Offset: 0x00046048
		private void ApplyChangedOptions()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			foreach (GenericOptionDataVM genericOptionDataVM in this._groupedCategories.SelectMany((GroupedOptionCategoryVM c) => c.AllOptions))
			{
				genericOptionDataVM.UpdateValue();
				if (genericOptionDataVM.IsNative && !genericOptionDataVM.GetOptionData().IsAction())
				{
					NativeOptions.NativeOptionsType nativeOptionsType = (NativeOptions.NativeOptionsType)genericOptionDataVM.GetOptionType();
					if (nativeOptionsType <= NativeOptions.NativeOptionsType.TextureFiltering)
					{
						if (nativeOptionsType <= NativeOptions.NativeOptionsType.TextureBudget)
						{
							switch (nativeOptionsType)
							{
							case NativeOptions.NativeOptionsType.TrailAmount:
								num = (genericOptionDataVM.IsChanged() ? 1 : 0);
								continue;
							case NativeOptions.NativeOptionsType.EnableVibration:
							case NativeOptions.NativeOptionsType.SelectedAdapter:
								continue;
							case NativeOptions.NativeOptionsType.DisplayMode:
							case NativeOptions.NativeOptionsType.SelectedMonitor:
							case NativeOptions.NativeOptionsType.ScreenResolution:
							case NativeOptions.NativeOptionsType.RefreshRate:
								break;
							default:
								if (nativeOptionsType != NativeOptions.NativeOptionsType.TextureBudget)
								{
									continue;
								}
								num2 = (genericOptionDataVM.IsChanged() ? 1 : 0);
								continue;
							}
						}
						else
						{
							if (nativeOptionsType == NativeOptions.NativeOptionsType.TextureQuality)
							{
								num2 = (genericOptionDataVM.IsChanged() ? 1 : 0);
								continue;
							}
							if (nativeOptionsType != NativeOptions.NativeOptionsType.TextureFiltering)
							{
								continue;
							}
							num8 = (genericOptionDataVM.IsChanged() ? 1 : 0);
							continue;
						}
					}
					else if (nativeOptionsType <= NativeOptions.NativeOptionsType.SSR)
					{
						if (nativeOptionsType == NativeOptions.NativeOptionsType.DepthOfField)
						{
							num4 = (genericOptionDataVM.IsChanged() ? 1 : 0);
							continue;
						}
						if (nativeOptionsType != NativeOptions.NativeOptionsType.SSR)
						{
							continue;
						}
						num6 = (genericOptionDataVM.IsChanged() ? 1 : 0);
						continue;
					}
					else
					{
						switch (nativeOptionsType)
						{
						case NativeOptions.NativeOptionsType.Bloom:
							num3 = (genericOptionDataVM.IsChanged() ? 1 : 0);
							continue;
						case NativeOptions.NativeOptionsType.FilmGrain:
							continue;
						case NativeOptions.NativeOptionsType.MotionBlur:
							num5 = (genericOptionDataVM.IsChanged() ? 1 : 0);
							continue;
						case NativeOptions.NativeOptionsType.SharpenAmount:
							num9 = (genericOptionDataVM.IsChanged() ? 1 : 0);
							continue;
						default:
							if (nativeOptionsType != NativeOptions.NativeOptionsType.DynamicResolution)
							{
								if (nativeOptionsType != NativeOptions.NativeOptionsType.DynamicResolutionTarget)
								{
									continue;
								}
								num10 = (genericOptionDataVM.IsChanged() ? 1 : 0);
								continue;
							}
							break;
						}
					}
					num7 = ((num7 == 1 || genericOptionDataVM.IsChanged()) ? 1 : 0);
				}
			}
			NativeOptions.Apply(num2, num9, num3, num4, num5, num6, num7, num8, num, num10);
			SaveResult saveResult = NativeOptions.SaveConfig();
			SaveResult saveResult2 = ManagedOptions.SaveConfig();
			if (saveResult != SaveResult.Success || saveResult2 != SaveResult.Success)
			{
				SaveResult saveResult3 = ((saveResult != SaveResult.Success) ? saveResult : saveResult2);
				InformationManager.ShowInquiry(new InquiryData(new TextObject("{=oZrVNUOk}Error", null).ToString(), Module.CurrentModule.GlobalTextManager.FindText("str_config_save_result", saveResult3.ToString()).ToString(), true, false, Module.CurrentModule.GlobalTextManager.FindText("str_ok", null).ToString(), null, null, null, "", 0f, null, null, null), false, false);
			}
			bool flag = this.GameKeyOptionGroups.IsChanged();
			this.GameKeyOptionGroups.ApplyValues();
			HotKeyManager.Save(flag);
		}

		// Token: 0x06001641 RID: 5697 RVA: 0x00048138 File Offset: 0x00046338
		protected void ExecuteBenchmark()
		{
			GameStateManager.StateActivateCommand = "state_string.benchmark_start";
			bool flag;
			CommandLineFunctionality.CallFunction("benchmark.cpu_benchmark", "", out flag);
		}

		// Token: 0x06001642 RID: 5698 RVA: 0x00048164 File Offset: 0x00046364
		public void ExecuteDone()
		{
			bool flag = false;
			int currentEstimatedGPUMemoryCostMB = Utilities.GetCurrentEstimatedGPUMemoryCostMB();
			int gpumemoryMB = Utilities.GetGPUMemoryMB();
			if (!flag && gpumemoryMB <= currentEstimatedGPUMemoryCostMB)
			{
				InformationManager.ShowInquiry(new InquiryData(Module.CurrentModule.GlobalTextManager.FindText("str_gpu_memory_caution_title", null).ToString(), Module.CurrentModule.GlobalTextManager.FindText("str_gpu_memory_caution_text", null).ToString(), true, false, Module.CurrentModule.GlobalTextManager.FindText("str_ok", null).ToString(), null, delegate
				{
					this.OnDone();
				}, null, "", 0f, null, null, null), false, false);
				return;
			}
			this.OnDone();
		}

		// Token: 0x06001643 RID: 5699 RVA: 0x00048204 File Offset: 0x00046404
		protected void ExecuteReset()
		{
			InformationManager.ShowInquiry(new InquiryData("", new TextObject("{=cDzWYQrz}Reset to default settings?", null).ToString(), true, true, new TextObject("{=oHaWR73d}Ok", null).ToString(), new TextObject("{=3CpNUnVl}Cancel", null).ToString(), new Action(this.OnResetToDefaults), null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06001644 RID: 5700 RVA: 0x00048270 File Offset: 0x00046470
		public bool IsOptionsChanged()
		{
			return (this._groupedCategories.Any((GroupedOptionCategoryVM c) => c.IsChanged()) || this.GameKeyOptionGroups.IsChanged()) | this._performanceOptionCategory.IsChanged();
		}

		// Token: 0x06001645 RID: 5701 RVA: 0x000482C3 File Offset: 0x000464C3
		private void OnResetToDefaults()
		{
			this._groupedCategories.ForEach(delegate(GroupedOptionCategoryVM g)
			{
				g.ResetData();
			});
			this._performanceOptionCategory.ResetData();
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x000482FC File Offset: 0x000464FC
		private void UpdateVideoMemoryUsage()
		{
			int currentEstimatedGPUMemoryCostMB = Utilities.GetCurrentEstimatedGPUMemoryCostMB();
			int gpumemoryMB = Utilities.GetGPUMemoryMB();
			this.VideoMemoryUsageNormalized = (float)currentEstimatedGPUMemoryCostMB / (float)gpumemoryMB;
			TextObject textObject = Module.CurrentModule.GlobalTextManager.FindText("str_gpu_memory_usage_value_text", null);
			textObject.SetTextVariable("ESTIMATED", currentEstimatedGPUMemoryCostMB);
			textObject.SetTextVariable("TOTAL", gpumemoryMB);
			this.VideoMemoryUsageText = textObject.ToString();
		}

		// Token: 0x06001647 RID: 5703 RVA: 0x0004835C File Offset: 0x0004655C
		internal GenericOptionDataVM GetOptionItem(IOptionData option)
		{
			bool flag = false;
			MBTextManager.SetTextVariable("IS_PLAYSTATION", flag ? 1 : 0);
			if (!option.IsAction())
			{
				string text = (option.IsNative() ? ((NativeOptions.NativeOptionsType)option.GetOptionType()).ToString() : ((ManagedOptions.ManagedOptionsType)option.GetOptionType()).ToString());
				TextObject textObject = Module.CurrentModule.GlobalTextManager.FindText("str_options_type", text);
				TextObject textObject2 = Module.CurrentModule.GlobalTextManager.FindText("str_options_description", text);
				textObject2.SetTextVariable("newline", "\n");
				if (option is IBooleanOptionData)
				{
					return new BooleanOptionDataVM(this, option as IBooleanOptionData, textObject, textObject2)
					{
						ImageIDs = new string[]
						{
							text + "_0",
							text + "_1"
						}
					};
				}
				if (option is INumericOptionData)
				{
					return new NumericOptionDataVM(this, option as INumericOptionData, textObject, textObject2);
				}
				if (option is ISelectionOptionData)
				{
					ISelectionOptionData selectionOptionData = option as ISelectionOptionData;
					StringOptionDataVM stringOptionDataVM = new StringOptionDataVM(this, selectionOptionData, textObject, textObject2);
					string[] array = new string[selectionOptionData.GetSelectableOptionsLimit()];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = text + "_" + i;
					}
					stringOptionDataVM.ImageIDs = array;
					return stringOptionDataVM;
				}
				ActionOptionData actionOptionData;
				if ((actionOptionData = option as ActionOptionData) != null)
				{
					TextObject textObject3 = Module.CurrentModule.GlobalTextManager.FindText("str_options_type_action", text);
					return new ActionOptionDataVM(actionOptionData.OnAction, this, actionOptionData, textObject, textObject3, textObject2);
				}
				Debug.FailedAssert("Given option data does not match with any option type!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\GameOptions\\OptionsVM.cs", "GetOptionItem", 873);
				return null;
			}
			else
			{
				ActionOptionData actionOptionData2;
				if ((actionOptionData2 = option as ActionOptionData) != null)
				{
					string text2 = option.GetOptionType() as string;
					TextObject textObject4 = Module.CurrentModule.GlobalTextManager.FindText("str_options_type_action", text2);
					TextObject textObject5 = Module.CurrentModule.GlobalTextManager.FindText("str_options_type", text2);
					TextObject textObject6 = Module.CurrentModule.GlobalTextManager.FindText("str_options_description", text2);
					textObject6.SetTextVariable("newline", "\n");
					return new ActionOptionDataVM(actionOptionData2.OnAction, this, actionOptionData2, textObject5, textObject4, textObject6);
				}
				Debug.FailedAssert("Given option data does not match with any option type!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\GameOptions\\OptionsVM.cs", "GetOptionItem", 896);
				return null;
			}
		}

		// Token: 0x17000744 RID: 1860
		// (get) Token: 0x06001648 RID: 5704 RVA: 0x000485B2 File Offset: 0x000467B2
		// (set) Token: 0x06001649 RID: 5705 RVA: 0x000485BA File Offset: 0x000467BA
		[DataSourceProperty]
		public int CategoryIndex
		{
			get
			{
				return this._categoryIndex;
			}
			set
			{
				if (value != this._categoryIndex)
				{
					this._categoryIndex = value;
					base.OnPropertyChangedWithValue(value, "CategoryIndex");
				}
			}
		}

		// Token: 0x17000745 RID: 1861
		// (get) Token: 0x0600164A RID: 5706 RVA: 0x000485D8 File Offset: 0x000467D8
		// (set) Token: 0x0600164B RID: 5707 RVA: 0x000485E0 File Offset: 0x000467E0
		[DataSourceProperty]
		public string OptionsLbl
		{
			get
			{
				return this._optionsLbl;
			}
			set
			{
				if (value != this._optionsLbl)
				{
					this._optionsLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "OptionsLbl");
				}
			}
		}

		// Token: 0x17000746 RID: 1862
		// (get) Token: 0x0600164C RID: 5708 RVA: 0x00048603 File Offset: 0x00046803
		// (set) Token: 0x0600164D RID: 5709 RVA: 0x0004860B File Offset: 0x0004680B
		[DataSourceProperty]
		public string CancelLbl
		{
			get
			{
				return this._cancelLbl;
			}
			set
			{
				if (value != this._cancelLbl)
				{
					this._cancelLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelLbl");
				}
			}
		}

		// Token: 0x17000747 RID: 1863
		// (get) Token: 0x0600164E RID: 5710 RVA: 0x0004862E File Offset: 0x0004682E
		// (set) Token: 0x0600164F RID: 5711 RVA: 0x00048636 File Offset: 0x00046836
		[DataSourceProperty]
		public string DoneLbl
		{
			get
			{
				return this._doneLbl;
			}
			set
			{
				if (value != this._doneLbl)
				{
					this._doneLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneLbl");
				}
			}
		}

		// Token: 0x17000748 RID: 1864
		// (get) Token: 0x06001650 RID: 5712 RVA: 0x00048659 File Offset: 0x00046859
		// (set) Token: 0x06001651 RID: 5713 RVA: 0x00048661 File Offset: 0x00046861
		[DataSourceProperty]
		public string ResetLbl
		{
			get
			{
				return this._resetLbl;
			}
			set
			{
				if (value != this._resetLbl)
				{
					this._resetLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "ResetLbl");
				}
			}
		}

		// Token: 0x17000749 RID: 1865
		// (get) Token: 0x06001652 RID: 5714 RVA: 0x00048684 File Offset: 0x00046884
		// (set) Token: 0x06001653 RID: 5715 RVA: 0x0004868C File Offset: 0x0004688C
		[DataSourceProperty]
		public string GameVersionText
		{
			get
			{
				return this._gameVersionText;
			}
			set
			{
				if (value != this._gameVersionText)
				{
					this._gameVersionText = value;
					base.OnPropertyChangedWithValue<string>(value, "GameVersionText");
				}
			}
		}

		// Token: 0x1700074A RID: 1866
		// (get) Token: 0x06001654 RID: 5716 RVA: 0x000486AF File Offset: 0x000468AF
		// (set) Token: 0x06001655 RID: 5717 RVA: 0x000486B7 File Offset: 0x000468B7
		[DataSourceProperty]
		public bool IsConsole
		{
			get
			{
				return this._isConsole;
			}
			set
			{
				if (value != this._isConsole)
				{
					this._isConsole = value;
					base.OnPropertyChangedWithValue(value, "IsConsole");
				}
			}
		}

		// Token: 0x1700074B RID: 1867
		// (get) Token: 0x06001656 RID: 5718 RVA: 0x000486D5 File Offset: 0x000468D5
		// (set) Token: 0x06001657 RID: 5719 RVA: 0x000486DD File Offset: 0x000468DD
		public bool IsDevelopmentMode
		{
			get
			{
				return this._isDevelopmentMode;
			}
			set
			{
				if (value != this._isDevelopmentMode)
				{
					this._isDevelopmentMode = value;
					base.OnPropertyChangedWithValue(value, "IsDevelopmentMode");
				}
			}
		}

		// Token: 0x1700074C RID: 1868
		// (get) Token: 0x06001658 RID: 5720 RVA: 0x000486FB File Offset: 0x000468FB
		// (set) Token: 0x06001659 RID: 5721 RVA: 0x00048703 File Offset: 0x00046903
		[DataSourceProperty]
		public string VideoMemoryUsageName
		{
			get
			{
				return this._videoMemoryUsageName;
			}
			set
			{
				if (this._videoMemoryUsageName != value)
				{
					this._videoMemoryUsageName = value;
					base.OnPropertyChangedWithValue<string>(value, "VideoMemoryUsageName");
				}
			}
		}

		// Token: 0x1700074D RID: 1869
		// (get) Token: 0x0600165A RID: 5722 RVA: 0x00048726 File Offset: 0x00046926
		// (set) Token: 0x0600165B RID: 5723 RVA: 0x0004872E File Offset: 0x0004692E
		[DataSourceProperty]
		public string VideoMemoryUsageText
		{
			get
			{
				return this._videoMemoryUsageText;
			}
			set
			{
				if (this._videoMemoryUsageText != value)
				{
					this._videoMemoryUsageText = value;
					base.OnPropertyChangedWithValue<string>(value, "VideoMemoryUsageText");
				}
			}
		}

		// Token: 0x1700074E RID: 1870
		// (get) Token: 0x0600165C RID: 5724 RVA: 0x00048751 File Offset: 0x00046951
		// (set) Token: 0x0600165D RID: 5725 RVA: 0x00048759 File Offset: 0x00046959
		[DataSourceProperty]
		public float VideoMemoryUsageNormalized
		{
			get
			{
				return this._videoMemoryUsageNormalized;
			}
			set
			{
				if (this._videoMemoryUsageNormalized != value)
				{
					this._videoMemoryUsageNormalized = value;
					base.OnPropertyChangedWithValue(value, "VideoMemoryUsageNormalized");
				}
			}
		}

		// Token: 0x1700074F RID: 1871
		// (get) Token: 0x0600165E RID: 5726 RVA: 0x00048777 File Offset: 0x00046977
		[DataSourceProperty]
		public GameKeyOptionCategoryVM GameKeyOptionGroups
		{
			get
			{
				return this._gameKeyCategory;
			}
		}

		// Token: 0x17000750 RID: 1872
		// (get) Token: 0x0600165F RID: 5727 RVA: 0x0004877F File Offset: 0x0004697F
		[DataSourceProperty]
		public GamepadOptionCategoryVM GamepadOptions
		{
			get
			{
				return this._gamepadCategory;
			}
		}

		// Token: 0x17000751 RID: 1873
		// (get) Token: 0x06001660 RID: 5728 RVA: 0x00048787 File Offset: 0x00046987
		[DataSourceProperty]
		public GroupedOptionCategoryVM PerformanceOptions
		{
			get
			{
				return this._performanceOptionCategory;
			}
		}

		// Token: 0x17000752 RID: 1874
		// (get) Token: 0x06001661 RID: 5729 RVA: 0x0004878F File Offset: 0x0004698F
		[DataSourceProperty]
		public GroupedOptionCategoryVM AudioOptions
		{
			get
			{
				return this._audioOptionCategory;
			}
		}

		// Token: 0x17000753 RID: 1875
		// (get) Token: 0x06001662 RID: 5730 RVA: 0x00048797 File Offset: 0x00046997
		[DataSourceProperty]
		public GroupedOptionCategoryVM GameplayOptions
		{
			get
			{
				return this._gameplayOptionCategory;
			}
		}

		// Token: 0x17000754 RID: 1876
		// (get) Token: 0x06001663 RID: 5731 RVA: 0x0004879F File Offset: 0x0004699F
		[DataSourceProperty]
		public GroupedOptionCategoryVM VideoOptions
		{
			get
			{
				return this._videoOptionCategory;
			}
		}

		// Token: 0x17000755 RID: 1877
		// (get) Token: 0x06001664 RID: 5732 RVA: 0x000487A7 File Offset: 0x000469A7
		// (set) Token: 0x06001665 RID: 5733 RVA: 0x000487AF File Offset: 0x000469AF
		[DataSourceProperty]
		public BrightnessOptionVM BrightnessPopUp
		{
			get
			{
				return this._brightnessPopUp;
			}
			set
			{
				if (value != this._brightnessPopUp)
				{
					this._brightnessPopUp = value;
					base.OnPropertyChangedWithValue<BrightnessOptionVM>(value, "BrightnessPopUp");
				}
			}
		}

		// Token: 0x17000756 RID: 1878
		// (get) Token: 0x06001666 RID: 5734 RVA: 0x000487CD File Offset: 0x000469CD
		// (set) Token: 0x06001667 RID: 5735 RVA: 0x000487D5 File Offset: 0x000469D5
		[DataSourceProperty]
		public ExposureOptionVM ExposurePopUp
		{
			get
			{
				return this._exposurePopUp;
			}
			set
			{
				if (value != this._exposurePopUp)
				{
					this._exposurePopUp = value;
					base.OnPropertyChangedWithValue<ExposureOptionVM>(value, "ExposurePopUp");
				}
			}
		}

		// Token: 0x06001668 RID: 5736 RVA: 0x000487F3 File Offset: 0x000469F3
		public void SetDoneInputKey(HotKey hotkey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x06001669 RID: 5737 RVA: 0x00048802 File Offset: 0x00046A02
		public void SetCancelInputKey(HotKey hotkey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x0600166A RID: 5738 RVA: 0x00048811 File Offset: 0x00046A11
		public void SetPreviousTabInputKey(HotKey hotkey)
		{
			this.PreviousTabInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x0600166B RID: 5739 RVA: 0x00048820 File Offset: 0x00046A20
		public void SetNextTabInputKey(HotKey hotkey)
		{
			this.NextTabInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x17000757 RID: 1879
		// (get) Token: 0x0600166C RID: 5740 RVA: 0x0004882F File Offset: 0x00046A2F
		// (set) Token: 0x0600166D RID: 5741 RVA: 0x00048837 File Offset: 0x00046A37
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		// Token: 0x17000758 RID: 1880
		// (get) Token: 0x0600166E RID: 5742 RVA: 0x00048855 File Offset: 0x00046A55
		// (set) Token: 0x0600166F RID: 5743 RVA: 0x0004885D File Offset: 0x00046A5D
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
			}
		}

		// Token: 0x17000759 RID: 1881
		// (get) Token: 0x06001670 RID: 5744 RVA: 0x0004887B File Offset: 0x00046A7B
		// (set) Token: 0x06001671 RID: 5745 RVA: 0x00048883 File Offset: 0x00046A83
		[DataSourceProperty]
		public InputKeyItemVM PreviousTabInputKey
		{
			get
			{
				return this._previousTabInputKey;
			}
			set
			{
				if (value != this._previousTabInputKey)
				{
					this._previousTabInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "PreviousTabInputKey");
				}
			}
		}

		// Token: 0x1700075A RID: 1882
		// (get) Token: 0x06001672 RID: 5746 RVA: 0x000488A1 File Offset: 0x00046AA1
		// (set) Token: 0x06001673 RID: 5747 RVA: 0x000488A9 File Offset: 0x00046AA9
		[DataSourceProperty]
		public InputKeyItemVM NextTabInputKey
		{
			get
			{
				return this._nextTabInputKey;
			}
			set
			{
				if (value != this._nextTabInputKey)
				{
					this._nextTabInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "NextTabInputKey");
				}
			}
		}

		// Token: 0x04000A88 RID: 2696
		private readonly Action _onClose;

		// Token: 0x04000A89 RID: 2697
		private readonly Action _onBrightnessExecute;

		// Token: 0x04000A8A RID: 2698
		private readonly Action _onExposureExecute;

		// Token: 0x04000A8B RID: 2699
		private readonly StringOptionDataVM _overallOption;

		// Token: 0x04000A8C RID: 2700
		private readonly GenericOptionDataVM _dlssOption;

		// Token: 0x04000A8D RID: 2701
		private readonly List<GenericOptionDataVM> _dynamicResolutionOptions = new List<GenericOptionDataVM>();

		// Token: 0x04000A8E RID: 2702
		private readonly GenericOptionDataVM _refreshRateOption;

		// Token: 0x04000A8F RID: 2703
		private readonly GenericOptionDataVM _resolutionOption;

		// Token: 0x04000A90 RID: 2704
		private readonly GenericOptionDataVM _monitorOption;

		// Token: 0x04000A91 RID: 2705
		private readonly bool _autoHandleClose;

		// Token: 0x04000A92 RID: 2706
		protected readonly GroupedOptionCategoryVM _gameplayOptionCategory;

		// Token: 0x04000A93 RID: 2707
		private readonly GroupedOptionCategoryVM _audioOptionCategory;

		// Token: 0x04000A94 RID: 2708
		private readonly GroupedOptionCategoryVM _videoOptionCategory;

		// Token: 0x04000A95 RID: 2709
		protected readonly GameKeyOptionCategoryVM _gameKeyCategory;

		// Token: 0x04000A96 RID: 2710
		private readonly GamepadOptionCategoryVM _gamepadCategory;

		// Token: 0x04000A97 RID: 2711
		private bool _isInitialized;

		// Token: 0x04000A98 RID: 2712
		private Action<KeyOptionVM> _onKeybindRequest;

		// Token: 0x04000A99 RID: 2713
		protected readonly GroupedOptionCategoryVM _performanceOptionCategory;

		// Token: 0x04000A9A RID: 2714
		private readonly int _overallConfigCount = NativeSelectionOptionData.GetOptionsLimit(NativeOptions.NativeOptionsType.OverAll) - 1;

		// Token: 0x04000A9B RID: 2715
		private bool _isCancelling;

		// Token: 0x04000A9C RID: 2716
		private readonly IEnumerable<IOptionData> _performanceManagedOptions;

		// Token: 0x04000A9D RID: 2717
		protected readonly List<GroupedOptionCategoryVM> _groupedCategories;

		// Token: 0x04000A9E RID: 2718
		private readonly List<ViewModel> _categories;

		// Token: 0x04000A9F RID: 2719
		private int _categoryIndex;

		// Token: 0x04000AA0 RID: 2720
		private string _optionsLbl;

		// Token: 0x04000AA1 RID: 2721
		private string _cancelLbl;

		// Token: 0x04000AA2 RID: 2722
		private string _doneLbl;

		// Token: 0x04000AA3 RID: 2723
		private string _resetLbl;

		// Token: 0x04000AA4 RID: 2724
		private string _gameVersionText;

		// Token: 0x04000AA5 RID: 2725
		private bool _isDevelopmentMode;

		// Token: 0x04000AA6 RID: 2726
		private bool _isConsole;

		// Token: 0x04000AA7 RID: 2727
		private float _videoMemoryUsageNormalized;

		// Token: 0x04000AA8 RID: 2728
		private string _videoMemoryUsageName;

		// Token: 0x04000AA9 RID: 2729
		private string _videoMemoryUsageText;

		// Token: 0x04000AAA RID: 2730
		private BrightnessOptionVM _brightnessPopUp;

		// Token: 0x04000AAB RID: 2731
		private ExposureOptionVM _exposurePopUp;

		// Token: 0x04000AAC RID: 2732
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000AAD RID: 2733
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000AAE RID: 2734
		private InputKeyItemVM _previousTabInputKey;

		// Token: 0x04000AAF RID: 2735
		private InputKeyItemVM _nextTabInputKey;

		// Token: 0x02000245 RID: 581
		public enum OptionsDataType
		{
			// Token: 0x04000F04 RID: 3844
			None = -1,
			// Token: 0x04000F05 RID: 3845
			BooleanOption,
			// Token: 0x04000F06 RID: 3846
			NumericOption,
			// Token: 0x04000F07 RID: 3847
			MultipleSelectionOption = 3,
			// Token: 0x04000F08 RID: 3848
			InputOption,
			// Token: 0x04000F09 RID: 3849
			ActionOption
		}

		// Token: 0x02000246 RID: 582
		public enum OptionsMode
		{
			// Token: 0x04000F0B RID: 3851
			MainMenu,
			// Token: 0x04000F0C RID: 3852
			Singleplayer,
			// Token: 0x04000F0D RID: 3853
			Multiplayer
		}
	}
}
