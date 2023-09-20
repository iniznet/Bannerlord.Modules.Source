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
	public class OptionsVM : ViewModel
	{
		public OptionsVM.OptionsMode CurrentOptionsMode { get; private set; }

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

		public OptionsVM(OptionsVM.OptionsMode optionsMode, Action onClose, Action<KeyOptionVM> onKeybindRequest, Action onBrightnessExecute = null, Action onExposureExecute = null)
			: this(false, optionsMode, onKeybindRequest, null, null)
		{
			this._onClose = onClose;
			this._onBrightnessExecute = onBrightnessExecute;
			this._onExposureExecute = onExposureExecute;
		}

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

		public void ExecuteCloseOptions()
		{
			if (this._onClose != null)
			{
				this._onClose();
			}
		}

		protected void OnBrightnessClick()
		{
			if (this._onBrightnessExecute == null)
			{
				this.BrightnessPopUp.Visible = true;
				return;
			}
			this._onBrightnessExecute();
		}

		protected void OnExposureClick()
		{
			if (this._onExposureExecute == null)
			{
				this.ExposurePopUp.Visible = true;
				return;
			}
			this._onExposureExecute();
		}

		public ViewModel GetActiveCategory()
		{
			if (this.CategoryIndex >= 0 && this.CategoryIndex < this._categories.Count)
			{
				return this._categories[this.CategoryIndex];
			}
			return null;
		}

		public int GetIndexOfCategory(ViewModel categoryVM)
		{
			return this._categories.IndexOf(categoryVM);
		}

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

		private bool IsManagedOptionsConflictWithOverallSettings(int overallSettingsOption)
		{
			return this._performanceManagedOptions.Any((IOptionData o) => (float)((int)o.GetValue(false)) != this.GetDefaultOptionForOverallManagedSettings((ManagedOptions.ManagedOptionsType)o.GetOptionType(), overallSettingsOption));
		}

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

		public void SelectPreviousCategory()
		{
			int previousAvailableCategoryIndex = this.GetPreviousAvailableCategoryIndex(this.CategoryIndex);
			this.SetSelectedCategory(previousAvailableCategoryIndex);
		}

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

		public void SelectNextCategory()
		{
			int nextAvailableCategoryIndex = this.GetNextAvailableCategoryIndex(this.CategoryIndex);
			this.SetSelectedCategory(nextAvailableCategoryIndex);
		}

		private void SetSelectedCategory(int index)
		{
			this.CategoryIndex = index;
		}

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

		protected void ExecuteBenchmark()
		{
			GameStateManager.StateActivateCommand = "state_string.benchmark_start";
			bool flag;
			CommandLineFunctionality.CallFunction("benchmark.cpu_benchmark", "", out flag);
		}

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

		protected void ExecuteReset()
		{
			InformationManager.ShowInquiry(new InquiryData("", new TextObject("{=cDzWYQrz}Reset to default settings?", null).ToString(), true, true, new TextObject("{=oHaWR73d}Ok", null).ToString(), new TextObject("{=3CpNUnVl}Cancel", null).ToString(), new Action(this.OnResetToDefaults), null, "", 0f, null, null, null), false, false);
		}

		public bool IsOptionsChanged()
		{
			return (this._groupedCategories.Any((GroupedOptionCategoryVM c) => c.IsChanged()) || this.GameKeyOptionGroups.IsChanged()) | this._performanceOptionCategory.IsChanged();
		}

		private void OnResetToDefaults()
		{
			this._groupedCategories.ForEach(delegate(GroupedOptionCategoryVM g)
			{
				g.ResetData();
			});
			this._performanceOptionCategory.ResetData();
		}

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

		[DataSourceProperty]
		public GameKeyOptionCategoryVM GameKeyOptionGroups
		{
			get
			{
				return this._gameKeyCategory;
			}
		}

		[DataSourceProperty]
		public GamepadOptionCategoryVM GamepadOptions
		{
			get
			{
				return this._gamepadCategory;
			}
		}

		[DataSourceProperty]
		public GroupedOptionCategoryVM PerformanceOptions
		{
			get
			{
				return this._performanceOptionCategory;
			}
		}

		[DataSourceProperty]
		public GroupedOptionCategoryVM AudioOptions
		{
			get
			{
				return this._audioOptionCategory;
			}
		}

		[DataSourceProperty]
		public GroupedOptionCategoryVM GameplayOptions
		{
			get
			{
				return this._gameplayOptionCategory;
			}
		}

		[DataSourceProperty]
		public GroupedOptionCategoryVM VideoOptions
		{
			get
			{
				return this._videoOptionCategory;
			}
		}

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

		public void SetDoneInputKey(HotKey hotkey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		public void SetCancelInputKey(HotKey hotkey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		public void SetPreviousTabInputKey(HotKey hotkey)
		{
			this.PreviousTabInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		public void SetNextTabInputKey(HotKey hotkey)
		{
			this.NextTabInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

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

		private readonly Action _onClose;

		private readonly Action _onBrightnessExecute;

		private readonly Action _onExposureExecute;

		private readonly StringOptionDataVM _overallOption;

		private readonly GenericOptionDataVM _dlssOption;

		private readonly List<GenericOptionDataVM> _dynamicResolutionOptions = new List<GenericOptionDataVM>();

		private readonly GenericOptionDataVM _refreshRateOption;

		private readonly GenericOptionDataVM _resolutionOption;

		private readonly GenericOptionDataVM _monitorOption;

		private readonly bool _autoHandleClose;

		protected readonly GroupedOptionCategoryVM _gameplayOptionCategory;

		private readonly GroupedOptionCategoryVM _audioOptionCategory;

		private readonly GroupedOptionCategoryVM _videoOptionCategory;

		protected readonly GameKeyOptionCategoryVM _gameKeyCategory;

		private readonly GamepadOptionCategoryVM _gamepadCategory;

		private bool _isInitialized;

		private Action<KeyOptionVM> _onKeybindRequest;

		protected readonly GroupedOptionCategoryVM _performanceOptionCategory;

		private readonly int _overallConfigCount = NativeSelectionOptionData.GetOptionsLimit(NativeOptions.NativeOptionsType.OverAll) - 1;

		private bool _isCancelling;

		private readonly IEnumerable<IOptionData> _performanceManagedOptions;

		protected readonly List<GroupedOptionCategoryVM> _groupedCategories;

		private readonly List<ViewModel> _categories;

		private int _categoryIndex;

		private string _optionsLbl;

		private string _cancelLbl;

		private string _doneLbl;

		private string _resetLbl;

		private string _gameVersionText;

		private bool _isDevelopmentMode;

		private bool _isConsole;

		private float _videoMemoryUsageNormalized;

		private string _videoMemoryUsageName;

		private string _videoMemoryUsageText;

		private BrightnessOptionVM _brightnessPopUp;

		private ExposureOptionVM _exposurePopUp;

		private InputKeyItemVM _doneInputKey;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _previousTabInputKey;

		private InputKeyItemVM _nextTabInputKey;

		public enum OptionsDataType
		{
			None = -1,
			BooleanOption,
			NumericOption,
			MultipleSelectionOption = 3,
			InputOption,
			ActionOption
		}

		public enum OptionsMode
		{
			MainMenu,
			Singleplayer,
			Multiplayer
		}
	}
}
