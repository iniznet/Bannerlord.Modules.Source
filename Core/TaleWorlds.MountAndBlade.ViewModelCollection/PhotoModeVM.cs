using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection
{
	public class PhotoModeVM : ViewModel
	{
		public PhotoModeVM(Scene missionScene, Func<bool> getVignetteOn, Func<bool> getHideAgentsOn)
		{
			this._missionScene = missionScene;
			this.Keys = new MBBindingList<InputKeyItemVM>();
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			bool flag = false;
			float num5 = 65f;
			missionScene.SetPhotoModeFov(num5);
			this._missionScene.GetPhotoModeFocus(ref num, ref num2, ref num3, ref num4, ref flag);
			this.FocusEndValueOption = new PhotoModeValueOptionVM(new TextObject("{=eeJcVeQG}Focus End", null), 0f, 1000f, num3, new Action<float>(this.OnFocusEndValueChange));
			this.FocusStartValueOption = new PhotoModeValueOptionVM(new TextObject("{=j5pLIV91}Focus Start", null), 0f, 100f, num2, new Action<float>(this.OnFocusStartValueChange));
			this.FocusValueOption = new PhotoModeValueOptionVM(new TextObject("{=photomodefocus}Focus", null), 0f, 100f, num, new Action<float>(this.OnFocusValueChange));
			this.ExposureOption = new PhotoModeValueOptionVM(new TextObject("{=iPx4jep6}Exposure", null), -5f, 5f, num4, new Action<float>(this.OnExposureValueChange));
			this.VerticalFovOption = new PhotoModeValueOptionVM(new TextObject("{=7XtICVeZ}Field of View", null), 2f, 140f, num5, new Action<float>(this.OnVerticalFovValueChange));
		}

		private void OnFocusValueChange(float newFocusValue)
		{
			this._missionScene.SetPhotoModeFocus(this.FocusStartValueOption.CurrentValue, this.FocusEndValueOption.CurrentValue, newFocusValue, this.ExposureOption.CurrentValue);
		}

		private void OnFocusStartValueChange(float newFocusStartValue)
		{
			this._missionScene.SetPhotoModeFocus(newFocusStartValue, this.FocusEndValueOption.CurrentValue, this.FocusValueOption.CurrentValue, this.ExposureOption.CurrentValue);
		}

		private void OnFocusEndValueChange(float newFocusEndValue)
		{
			this._missionScene.SetPhotoModeFocus(this.FocusStartValueOption.CurrentValue, newFocusEndValue, this.FocusValueOption.CurrentValue, this.ExposureOption.CurrentValue);
		}

		private void OnExposureValueChange(float newExposureValue)
		{
			this._missionScene.SetPhotoModeFocus(this.FocusStartValueOption.CurrentValue, this.FocusEndValueOption.CurrentValue, this.FocusValueOption.CurrentValue, newExposureValue);
		}

		private void OnVerticalFovValueChange(float newVerticalFov)
		{
			this._missionScene.SetPhotoModeFov(newVerticalFov);
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Keys.ApplyActionOnAllItems(delegate(InputKeyItemVM k)
			{
				k.RefreshValues();
			});
			this.FocusEndValueOption.RefreshValues();
			this.FocusStartValueOption.RefreshValues();
			this.FocusValueOption.RefreshValues();
			this.ExposureOption.RefreshValues();
			this.VerticalFovOption.RefreshValues();
			List<string> list = new List<string>();
			foreach (string text in this._missionScene.GetAllColorGradeNames().Split(new string[] { "*/*" }, StringSplitOptions.RemoveEmptyEntries))
			{
				string text2 = GameTexts.FindText("str_photo_mode_color_grade", text).ToString();
				list.Add(text2);
			}
			if (list.Count == 0)
			{
				list.Add("Photo Mode Not Active");
			}
			this.ColorGradeSelector = new SelectorVM<SelectorItemVM>(list, this._missionScene.GetSceneColorGradeIndex(), new Action<SelectorVM<SelectorItemVM>>(this.OnColorGradeSelectionChanged));
			List<string> list2 = new List<string>();
			foreach (string text3 in this._missionScene.GetAllFilterNames().Split(new string[] { "*/*" }, StringSplitOptions.RemoveEmptyEntries))
			{
				string text4 = GameTexts.FindText("str_photo_mode_overlay", text3).ToString();
				list2.Add(text4);
			}
			if (list2.Count == 0)
			{
				list.Add("Photo Mode Not Active");
			}
			this.OverlaySelector = new SelectorVM<SelectorItemVM>(list2, this._missionScene.GetSceneFilterIndex(), new Action<SelectorVM<SelectorItemVM>>(this.OnOverlaySelectionChanged));
		}

		public void AddKey(GameKey key)
		{
			this.Keys.Add(InputKeyItemVM.CreateFromGameKey(key, false));
		}

		public void AddHotkey(HotKey hotkey)
		{
			this.Keys.Add(InputKeyItemVM.CreateFromHotKey(hotkey, false));
		}

		public void AddHotkeyWithForcedName(HotKey hotkey, TextObject forcedName)
		{
			this.Keys.Add(InputKeyItemVM.CreateFromHotKeyWithForcedName(hotkey, forcedName, false));
		}

		public void AddCustomKey(string keyID, TextObject forcedName)
		{
			this.Keys.Add(InputKeyItemVM.CreateFromForcedID(keyID, forcedName, false));
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			foreach (InputKeyItemVM inputKeyItemVM in this.Keys)
			{
				inputKeyItemVM.OnFinalize();
			}
		}

		private void OnColorGradeSelectionChanged(SelectorVM<SelectorItemVM> obj)
		{
			if (this._missionScene.GetSceneColorGradeIndex() != obj.SelectedIndex)
			{
				this._missionScene.SetSceneColorGradeIndex(obj.SelectedIndex);
			}
		}

		private void OnOverlaySelectionChanged(SelectorVM<SelectorItemVM> obj)
		{
			if (this._missionScene.GetSceneFilterIndex() != obj.SelectedIndex)
			{
				int num = this._missionScene.SetSceneFilterIndex(obj.SelectedIndex);
				if (num >= 0)
				{
					this.ColorGradeSelector.SelectedIndex = num;
				}
			}
		}

		public void Reset()
		{
			this.ColorGradeSelector.SelectedIndex = 0;
			this.OverlaySelector.SelectedIndex = 0;
			this.FocusValueOption.CurrentValue = 0f;
			this.FocusStartValueOption.CurrentValue = 0f;
			this.FocusEndValueOption.CurrentValue = 0f;
			this.ExposureOption.CurrentValue = 0f;
			this.VerticalFovOption.CurrentValue = 65f;
		}

		[DataSourceProperty]
		public MBBindingList<InputKeyItemVM> Keys
		{
			get
			{
				return this._keys;
			}
			set
			{
				if (value != this._keys)
				{
					this._keys = value;
					base.OnPropertyChangedWithValue<MBBindingList<InputKeyItemVM>>(value, "Keys");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> ColorGradeSelector
		{
			get
			{
				return this._colorGradeSelector;
			}
			set
			{
				if (value != this._colorGradeSelector)
				{
					this._colorGradeSelector = value;
					base.OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "ColorGradeSelector");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> OverlaySelector
		{
			get
			{
				return this._overlaySelector;
			}
			set
			{
				if (value != this._overlaySelector)
				{
					this._overlaySelector = value;
					base.OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "OverlaySelector");
				}
			}
		}

		[DataSourceProperty]
		public PhotoModeValueOptionVM FocusEndValueOption
		{
			get
			{
				return this._focusEndValueOption;
			}
			set
			{
				if (value != this._focusEndValueOption)
				{
					this._focusEndValueOption = value;
					base.OnPropertyChangedWithValue<PhotoModeValueOptionVM>(value, "FocusEndValueOption");
				}
			}
		}

		[DataSourceProperty]
		public PhotoModeValueOptionVM FocusStartValueOption
		{
			get
			{
				return this._focusStartValueOption;
			}
			set
			{
				if (value != this._focusStartValueOption)
				{
					this._focusStartValueOption = value;
					base.OnPropertyChangedWithValue<PhotoModeValueOptionVM>(value, "FocusStartValueOption");
				}
			}
		}

		[DataSourceProperty]
		public PhotoModeValueOptionVM FocusValueOption
		{
			get
			{
				return this._focusValueOption;
			}
			set
			{
				if (value != this._focusValueOption)
				{
					this._focusValueOption = value;
					base.OnPropertyChangedWithValue<PhotoModeValueOptionVM>(value, "FocusValueOption");
				}
			}
		}

		[DataSourceProperty]
		public PhotoModeValueOptionVM ExposureOption
		{
			get
			{
				return this._exposureOption;
			}
			set
			{
				if (value != this._exposureOption)
				{
					this._exposureOption = value;
					base.OnPropertyChangedWithValue<PhotoModeValueOptionVM>(value, "ExposureOption");
				}
			}
		}

		[DataSourceProperty]
		public PhotoModeValueOptionVM VerticalFovOption
		{
			get
			{
				return this._verticalFovOption;
			}
			set
			{
				if (value != this._verticalFovOption)
				{
					this._verticalFovOption = value;
					base.OnPropertyChangedWithValue<PhotoModeValueOptionVM>(value, "VerticalFovOption");
				}
			}
		}

		private readonly Scene _missionScene;

		private SelectorVM<SelectorItemVM> _colorGradeSelector;

		private SelectorVM<SelectorItemVM> _overlaySelector;

		private MBBindingList<InputKeyItemVM> _keys;

		private PhotoModeValueOptionVM _focusEndValueOption;

		private PhotoModeValueOptionVM _focusStartValueOption;

		private PhotoModeValueOptionVM _focusValueOption;

		private PhotoModeValueOptionVM _exposureOption;

		private PhotoModeValueOptionVM _verticalFovOption;
	}
}
