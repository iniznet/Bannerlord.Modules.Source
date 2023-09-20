using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.FaceGenerator
{
	public class FaceGenVM : ViewModel
	{
		private bool _isAgeAvailable
		{
			get
			{
				return this._openedFromMultiplayer || this._showDebugValues;
			}
		}

		private bool _isWeightAvailable
		{
			get
			{
				return !this._openedFromMultiplayer || this._showDebugValues;
			}
		}

		private bool _isBuildAvailable
		{
			get
			{
				return !this._openedFromMultiplayer || this._showDebugValues;
			}
		}

		private bool _isRaceAvailable
		{
			get
			{
				return (FaceGen.GetRaceCount() > 1 && !this._openedFromMultiplayer) || this._showDebugValues;
			}
		}

		public void SetFaceGenerationParams(FaceGenerationParams faceGenerationParams)
		{
			this._faceGenerationParams = faceGenerationParams;
		}

		public FaceGenVM(BodyGenerator bodyGenerator, IFaceGeneratorHandler faceGeneratorScreen, Action<float> onHeightChanged, Action onAgeChanged, TextObject affirmitiveText, TextObject negativeText, int currentStageIndex, int totalStagesCount, int furthestIndex, Action<int> goToIndex, bool canChangeGender, bool openedFromMultiplayer, IFaceGeneratorCustomFilter filter)
		{
			this._bodyGenerator = bodyGenerator;
			this._faceGeneratorScreen = faceGeneratorScreen;
			this._showDebugValues = FaceGen.ShowDebugValues;
			this._affirmitiveText = affirmitiveText;
			this._negativeText = negativeText;
			this._openedFromMultiplayer = openedFromMultiplayer;
			this._filter = filter;
			this.CanChangeGender = canChangeGender || this._showDebugValues;
			this._onHeightChanged = onHeightChanged;
			this._onAgeChanged = onAgeChanged;
			this._goToIndex = goToIndex;
			this.TotalStageCount = totalStagesCount;
			this.CurrentStageIndex = currentStageIndex;
			this.FurthestIndex = furthestIndex;
			this.CameraControlKeys = new MBBindingList<InputKeyItemVM>();
			this.BodyProperties = new MBBindingList<FaceGenPropertyVM>();
			this.FaceProperties = new MBBindingList<FaceGenPropertyVM>();
			this.EyesProperties = new MBBindingList<FaceGenPropertyVM>();
			this.NoseProperties = new MBBindingList<FaceGenPropertyVM>();
			this.MouthProperties = new MBBindingList<FaceGenPropertyVM>();
			this.HairProperties = new MBBindingList<FaceGenPropertyVM>();
			this.TaintProperties = new MBBindingList<FaceGenPropertyVM>();
			this._tabProperties = new Dictionary<FaceGenVM.FaceGenTabs, MBBindingList<FaceGenPropertyVM>>
			{
				{
					FaceGenVM.FaceGenTabs.Body,
					this.BodyProperties
				},
				{
					FaceGenVM.FaceGenTabs.Face,
					this.FaceProperties
				},
				{
					FaceGenVM.FaceGenTabs.Eyes,
					this.EyesProperties
				},
				{
					FaceGenVM.FaceGenTabs.Nose,
					this.NoseProperties
				},
				{
					FaceGenVM.FaceGenTabs.Mouth,
					this.MouthProperties
				},
				{
					FaceGenVM.FaceGenTabs.Hair,
					this.HairProperties
				},
				{
					FaceGenVM.FaceGenTabs.Taint,
					this.TaintProperties
				}
			};
			this.TaintTypes = new MBBindingList<FacegenListItemVM>();
			this.BeardTypes = new MBBindingList<FacegenListItemVM>();
			this.HairTypes = new MBBindingList<FacegenListItemVM>();
			this._tab = -1;
			this.IsDressed = false;
			this.genderBasedSelectedValues = new FaceGenVM.GenderBasedSelectedValue[2];
			this.genderBasedSelectedValues[0].Reset();
			this.genderBasedSelectedValues[1].Reset();
			this._undoCommands = new List<FaceGenVM.UndoRedoKey>(100);
			this._redoCommands = new List<FaceGenVM.UndoRedoKey>(100);
			this.CanChangeRace = this._isRaceAvailable;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.BodyHint = new HintViewModel(GameTexts.FindText("str_body", null), null);
			this.FaceHint = new HintViewModel(GameTexts.FindText("str_face", null), null);
			this.EyesHint = new HintViewModel(GameTexts.FindText("str_eyes", null), null);
			this.NoseHint = new HintViewModel(GameTexts.FindText("str_nose", null), null);
			this.HairHint = new HintViewModel(GameTexts.FindText("str_hair", null), null);
			this.TaintHint = new HintViewModel(GameTexts.FindText("str_face_gen_markings", null), null);
			this.MouthHint = new HintViewModel(GameTexts.FindText("str_mouth", null), null);
			this.RedoHint = new HintViewModel(GameTexts.FindText("str_redo", null), null);
			this.UndoHint = new HintViewModel(GameTexts.FindText("str_undo", null), null);
			this.RandomizeHint = new HintViewModel(GameTexts.FindText("str_randomize", null), null);
			this.RandomizeAllHint = new HintViewModel(GameTexts.FindText("str_randomize_all", null), null);
			this.ResetHint = new HintViewModel(GameTexts.FindText("str_reset", null), null);
			this.ResetAllHint = new HintViewModel(GameTexts.FindText("str_reset_all", null), null);
			this.ClothHint = new HintViewModel(GameTexts.FindText("str_face_gen_change_cloth", null), null);
			this.FlipHairLbl = new TextObject("{=74PKmRWJ}Flip Hair", null).ToString();
			this.SkinColorLbl = GameTexts.FindText("sf_facegen_skin_color", null).ToString();
			this.GenderLbl = GameTexts.FindText("sf_facegen_gender", null).ToString();
			this.RaceLbl = GameTexts.FindText("sf_facegen_race", null).ToString();
			this.Title = GameTexts.FindText("sf_facegen_title", null).ToString();
			this.DoneBtnLbl = this._affirmitiveText.ToString();
			this.CancelBtnLbl = this._negativeText.ToString();
			FacegenListItemVM selectedTaintType = this._selectedTaintType;
			if (selectedTaintType != null)
			{
				selectedTaintType.RefreshValues();
			}
			FacegenListItemVM selectedBeardType = this._selectedBeardType;
			if (selectedBeardType != null)
			{
				selectedBeardType.RefreshValues();
			}
			FacegenListItemVM selectedHairType = this._selectedHairType;
			if (selectedHairType != null)
			{
				selectedHairType.RefreshValues();
			}
			this._bodyProperties.ApplyActionOnAllItems(delegate(FaceGenPropertyVM x)
			{
				x.RefreshValues();
			});
			this._faceProperties.ApplyActionOnAllItems(delegate(FaceGenPropertyVM x)
			{
				x.RefreshValues();
			});
			this._eyesProperties.ApplyActionOnAllItems(delegate(FaceGenPropertyVM x)
			{
				x.RefreshValues();
			});
			this._noseProperties.ApplyActionOnAllItems(delegate(FaceGenPropertyVM x)
			{
				x.RefreshValues();
			});
			this._mouthProperties.ApplyActionOnAllItems(delegate(FaceGenPropertyVM x)
			{
				x.RefreshValues();
			});
			this._hairProperties.ApplyActionOnAllItems(delegate(FaceGenPropertyVM x)
			{
				x.RefreshValues();
			});
			this._taintProperties.ApplyActionOnAllItems(delegate(FaceGenPropertyVM x)
			{
				x.RefreshValues();
			});
			this._taintTypes.ApplyActionOnAllItems(delegate(FacegenListItemVM x)
			{
				x.RefreshValues();
			});
			this._beardTypes.ApplyActionOnAllItems(delegate(FacegenListItemVM x)
			{
				x.RefreshValues();
			});
			this._hairTypes.ApplyActionOnAllItems(delegate(FacegenListItemVM x)
			{
				x.RefreshValues();
			});
			FaceGenPropertyVM soundPreset = this._soundPreset;
			if (soundPreset != null)
			{
				soundPreset.RefreshValues();
			}
			FaceGenPropertyVM faceTypes = this._faceTypes;
			if (faceTypes != null)
			{
				faceTypes.RefreshValues();
			}
			FaceGenPropertyVM teethTypes = this._teethTypes;
			if (teethTypes != null)
			{
				teethTypes.RefreshValues();
			}
			FaceGenPropertyVM eyebrowTypes = this._eyebrowTypes;
			if (eyebrowTypes != null)
			{
				eyebrowTypes.RefreshValues();
			}
			SelectorVM<SelectorItemVM> skinColorSelector = this._skinColorSelector;
			if (skinColorSelector != null)
			{
				skinColorSelector.RefreshValues();
			}
			SelectorVM<SelectorItemVM> hairColorSelector = this._hairColorSelector;
			if (hairColorSelector != null)
			{
				hairColorSelector.RefreshValues();
			}
			SelectorVM<SelectorItemVM> tattooColorSelector = this._tattooColorSelector;
			if (tattooColorSelector != null)
			{
				tattooColorSelector.RefreshValues();
			}
			SelectorVM<SelectorItemVM> raceSelector = this._raceSelector;
			if (raceSelector == null)
			{
				return;
			}
			raceSelector.RefreshValues();
		}

		private void FilterCategories()
		{
			FaceGeneratorStage[] availableStages = this._filter.GetAvailableStages();
			this.IsBodyEnabled = availableStages.Contains(FaceGeneratorStage.Body);
			this.IsFaceEnabled = availableStages.Contains(FaceGeneratorStage.Face);
			this.IsEyesEnabled = availableStages.Contains(FaceGeneratorStage.Eyes);
			this.IsNoseEnabled = availableStages.Contains(FaceGeneratorStage.Nose);
			this.IsMouthEnabled = availableStages.Contains(FaceGeneratorStage.Mouth);
			this.IsHairEnabled = availableStages.Contains(FaceGeneratorStage.Hair);
			this.IsTaintEnabled = availableStages.Contains(FaceGeneratorStage.Taint);
			this.Tab = (int)availableStages.FirstOrDefault<FaceGeneratorStage>();
		}

		private void SetColorCodes()
		{
			this._skinColors = MBBodyProperties.GetSkinColorGradientPoints(this._selectedRace, this.SelectedGender, (int)this._bodyGenerator.Character.Age);
			this._hairColors = MBBodyProperties.GetHairColorGradientPoints(this._selectedRace, this.SelectedGender, (int)this._bodyGenerator.Character.Age);
			this._tattooColors = MBBodyProperties.GetTatooColorGradientPoints(this._selectedRace, this.SelectedGender, (int)this._bodyGenerator.Character.Age);
			this.SkinColorSelector = new SelectorVM<SelectorItemVM>(this._skinColors.Select(delegate(uint t)
			{
				t %= 4278190080U;
				return "#" + Convert.ToString((long)((ulong)t), 16).PadLeft(6, '0').ToUpper() + "FF";
			}).ToList<string>(), MathF.Round(this._faceGenerationParams._curSkinColorOffset * (float)(this._skinColors.Count - 1)), new Action<SelectorVM<SelectorItemVM>>(this.OnSelectSkinColor));
			this.HairColorSelector = new SelectorVM<SelectorItemVM>(this._hairColors.Select(delegate(uint t)
			{
				t %= 4278190080U;
				return "#" + Convert.ToString((long)((ulong)t), 16).PadLeft(6, '0').ToUpper() + "FF";
			}).ToList<string>(), MathF.Round(this._faceGenerationParams._curHairColorOffset * (float)(this._hairColors.Count - 1)), new Action<SelectorVM<SelectorItemVM>>(this.OnSelectHairColor));
			this.TattooColorSelector = new SelectorVM<SelectorItemVM>(this._tattooColors.Select(delegate(uint t)
			{
				t %= 4278190080U;
				return "#" + Convert.ToString((long)((ulong)t), 16).PadLeft(6, '0').ToUpper() + "FF";
			}).ToList<string>(), MathF.Round(this._faceGenerationParams._curFaceTattooColorOffset1 * (float)(this._tattooColors.Count - 1)), new Action<SelectorVM<SelectorItemVM>>(this.OnSelectTattooColor));
		}

		private void OnSelectSkinColor(SelectorVM<SelectorItemVM> s)
		{
			this.AddCommand();
			this._faceGenerationParams._curSkinColorOffset = (float)s.SelectedIndex / (float)(this._skinColors.Count - 1);
			this.UpdateFace();
		}

		private void OnSelectTattooColor(SelectorVM<SelectorItemVM> s)
		{
			this.AddCommand();
			this._faceGenerationParams._curFaceTattooColorOffset1 = (float)s.SelectedIndex / (float)(this._tattooColors.Count - 1);
			this.UpdateFace();
		}

		private void OnSelectHairColor(SelectorVM<SelectorItemVM> s)
		{
			this.AddCommand();
			this._faceGenerationParams._curHairColorOffset = (float)s.SelectedIndex / (float)(this._hairColors.Count - 1);
			this.UpdateFace();
		}

		private void OnSelectRace(SelectorVM<SelectorItemVM> s)
		{
			this.AddCommand();
			this._selectedRace = s.SelectedIndex;
			if (this._initialRace == -1)
			{
				this._initialRace = this._selectedRace;
			}
			this.UpdateRaceAndGenderBasedResources();
			this.Refresh(true);
		}

		private void OnHeightChanged()
		{
			Action<float> onHeightChanged = this._onHeightChanged;
			if (onHeightChanged == null)
			{
				return;
			}
			FaceGenPropertyVM heightSlider = this._heightSlider;
			onHeightChanged((heightSlider != null) ? heightSlider.Value : 0f);
		}

		private void OnAgeChanged()
		{
			Action onAgeChanged = this._onAgeChanged;
			if (onAgeChanged == null)
			{
				return;
			}
			onAgeChanged();
		}

		private void SetTabAvailabilities()
		{
			this._tabAvailabilities = new MBList<bool> { this.IsBodyEnabled, this.IsFaceEnabled, this.IsEyesEnabled, this.IsNoseEnabled, this.IsMouthEnabled, this.IsHairEnabled, this.IsTaintEnabled };
		}

		public void OnTabClicked(int index)
		{
			this.Tab = index;
		}

		public void SelectPreviousTab()
		{
			int num = ((this.Tab == 0) ? 6 : (this.Tab - 1));
			while (!this._tabAvailabilities[num] && num != this.Tab)
			{
				num = ((num == 0) ? 6 : (num - 1));
			}
			this.Tab = num;
		}

		public void SelectNextTab()
		{
			int num = (this.Tab + 1) % 7;
			while (!this._tabAvailabilities[num] && num != this.Tab)
			{
				num = (num + 1) % 7;
			}
			this.Tab = num;
		}

		public void Refresh(bool clearProperties)
		{
			if (!this._characterRefreshEnabled)
			{
				return;
			}
			this._characterRefreshEnabled = false;
			base.OnPropertyChanged("FlipHairCb");
			this._selectedRace = this._faceGenerationParams._currentRace;
			this._selectedGender = this._faceGenerationParams._currentGender;
			this.SetColorCodes();
			int num = 0;
			MBBodyProperties.GetParamsMax(this._selectedRace, this.SelectedGender, (int)this._faceGenerationParams._curAge, ref num, ref this.beardNum, ref this.faceTextureNum, ref this.mouthTextureNum, ref this.faceTattooNum, ref this._newSoundPresetSize, ref this.eyebrowTextureNum, ref this._scale);
			this.HairNum = num;
			MBBodyProperties.GetZeroProbabilities(this._selectedRace, this.SelectedGender, this._faceGenerationParams._curAge, ref this._faceGenerationParams._tattooZeroProbability);
			if (clearProperties)
			{
				foreach (KeyValuePair<FaceGenVM.FaceGenTabs, MBBindingList<FaceGenPropertyVM>> keyValuePair in this._tabProperties)
				{
					keyValuePair.Value.Clear();
				}
			}
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			if (clearProperties)
			{
				int faceGenInstancesLength = MBBodyProperties.GetFaceGenInstancesLength(this._faceGenerationParams._currentRace, this._faceGenerationParams._currentGender, (int)this._faceGenerationParams._curAge);
				for (int i = 0; i < faceGenInstancesLength; i++)
				{
					DeformKeyData deformKeyData = MBBodyProperties.GetDeformKeyData(i, this._faceGenerationParams._currentRace, this._faceGenerationParams._currentGender, (int)this._faceGenerationParams._curAge);
					TextObject textObject = new TextObject("{=bsiRNJtk}{NAME}:", null);
					textObject.SetTextVariable("NAME", GameTexts.FindText("str_facegen_skin", deformKeyData.Id));
					if (GameTexts.FindText("str_facegen_skin", deformKeyData.Id).ToString().Contains("exist"))
					{
						Debug.FailedAssert(deformKeyData.Id + " id name doesn't exist", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\FaceGenerator\\FaceGenVM.cs", "Refresh", 439);
					}
					if (deformKeyData.Id == "weight")
					{
						num3 = deformKeyData.KeyTimePoint;
					}
					else if (deformKeyData.Id == "build")
					{
						num5 = deformKeyData.KeyTimePoint;
					}
					else if (deformKeyData.Id == "height")
					{
						num2 = deformKeyData.KeyTimePoint;
					}
					else if (deformKeyData.Id == "age")
					{
						num4 = deformKeyData.KeyTimePoint;
					}
					else
					{
						FaceGenPropertyVM faceGenPropertyVM = new FaceGenPropertyVM(i, 0.0, 1.0, textObject, deformKeyData.KeyTimePoint, deformKeyData.GroupId, (double)this._faceGenerationParams.KeyWeights[i], new Action<int, float, bool, bool>(this.UpdateFace), new Action(this.AddCommand), new Action(this.ResetSliderPrevValues), true, false);
						if (deformKeyData.GroupId > -1 && deformKeyData.GroupId < 7)
						{
							this._tabProperties[(FaceGenVM.FaceGenTabs)deformKeyData.GroupId].Add(faceGenPropertyVM);
						}
					}
				}
			}
			if (this._filter != null)
			{
				this.FilterCategories();
			}
			else if (this._tab == -1)
			{
				this.IsBodyEnabled = true;
				this.IsFaceEnabled = true;
				this.IsEyesEnabled = true;
				this.IsNoseEnabled = true;
				this.IsMouthEnabled = true;
				this.IsHairEnabled = true;
				this.IsTaintEnabled = true;
				this.Tab = 0;
			}
			this.SetTabAvailabilities();
			if (clearProperties)
			{
				FaceGenPropertyVM faceGenPropertyVM = new FaceGenPropertyVM(-19, 0.0, 1.0, new TextObject("{=G6hYIR5k}Voice Pitch:", null), -19, 0, (double)this._faceGenerationParams._voicePitch, new Action<int, float, bool, bool>(this.UpdateFace), new Action(this.AddCommand), new Action(this.ResetSliderPrevValues), true, false);
				this._tabProperties[FaceGenVM.FaceGenTabs.Body].Add(faceGenPropertyVM);
				this._heightSlider = new FaceGenPropertyVM(-16, (double)(this._openedFromMultiplayer ? 0.25f : 0f), (double)(this._openedFromMultiplayer ? 0.75f : 1f), new TextObject("{=cLJdeUWz}Height:", null), num2, 0, (double)((this._heightSlider == null) ? this._faceGenerationParams._heightMultiplier : this._heightSlider.Value), new Action<int, float, bool, bool>(this.UpdateFace), new Action(this.AddCommand), new Action(this.ResetSliderPrevValues), true, false);
				this._tabProperties[FaceGenVM.FaceGenTabs.Body].Add(this._heightSlider);
				this.UpdateVoiceIndiciesFromCurrentParameters();
				if (this._isAgeAvailable)
				{
					double num6 = (double)(this._openedFromMultiplayer ? 25 : 3);
					faceGenPropertyVM = new FaceGenPropertyVM(-11, num6, 128.0, new TextObject("{=H1emUb6k}Age:", null), num4, 0, (double)this._faceGenerationParams._curAge, new Action<int, float, bool, bool>(this.UpdateFace), new Action(this.AddCommand), new Action(this.ResetSliderPrevValues), true, false);
					this._tabProperties[FaceGenVM.FaceGenTabs.Body].Add(faceGenPropertyVM);
				}
				if (this._isWeightAvailable)
				{
					faceGenPropertyVM = new FaceGenPropertyVM(-17, 0.0, 1.0, new TextObject("{=zBld61ck}Weight:", null), num3, 0, (double)this._faceGenerationParams._curWeight, new Action<int, float, bool, bool>(this.UpdateFace), new Action(this.AddCommand), new Action(this.ResetSliderPrevValues), true, false);
					this._tabProperties[FaceGenVM.FaceGenTabs.Body].Add(faceGenPropertyVM);
				}
				if (this._isBuildAvailable)
				{
					faceGenPropertyVM = new FaceGenPropertyVM(-18, 0.0, 1.0, new TextObject("{=EUAKPHek}Build:", null), num5, 0, (double)this._faceGenerationParams._curBuild, new Action<int, float, bool, bool>(this.UpdateFace), new Action(this.AddCommand), new Action(this.ResetSliderPrevValues), true, false);
					this._tabProperties[FaceGenVM.FaceGenTabs.Body].Add(faceGenPropertyVM);
				}
				faceGenPropertyVM = new FaceGenPropertyVM(-12, 0.0, 1.0, new TextObject("{=qXxpITdc}Eye Color:", null), -12, 2, (double)this._faceGenerationParams._curEyeColorOffset, new Action<int, float, bool, bool>(this.UpdateFace), new Action(this.AddCommand), new Action(this.ResetSliderPrevValues), true, false);
				this._tabProperties[FaceGenVM.FaceGenTabs.Eyes].Add(faceGenPropertyVM);
				this.RaceSelector = new SelectorVM<SelectorItemVM>(FaceGen.GetRaceNames(), this._selectedRace, new Action<SelectorVM<SelectorItemVM>>(this.OnSelectRace));
			}
			this.UpdateRaceAndGenderBasedResources();
			if (!this._initialValuesSet)
			{
				this._initialSelectedTaintType = this._faceGenerationParams._curFaceTattoo;
				this._initialSelectedBeardType = this._faceGenerationParams._curBeard;
				this._initialSelectedHairType = this._faceGenerationParams._currentHair;
				this._initialSelectedHairColor = this._faceGenerationParams._curHairColorOffset;
				this._initialSelectedSkinColor = this._faceGenerationParams._curSkinColorOffset;
				this._initialSelectedTaintColor = this._faceGenerationParams._curFaceTattooColorOffset1;
				this._initialRace = this._selectedRace;
				this._initialGender = this.SelectedGender;
				this._initialValuesSet = true;
			}
			this._characterRefreshEnabled = true;
			this.UpdateFace();
		}

		private void UpdateRaceAndGenderBasedResources()
		{
			int num = 0;
			MBBodyProperties.GetParamsMax(this._selectedRace, this.SelectedGender, (int)this._faceGenerationParams._curAge, ref num, ref this.beardNum, ref this.faceTextureNum, ref this.mouthTextureNum, ref this.faceTattooNum, ref this._newSoundPresetSize, ref this.eyebrowTextureNum, ref this._scale);
			this.HairNum = num;
			int[] array = Enumerable.Range(0, num).ToArray<int>();
			int[] array2 = Enumerable.Range(0, this.beardNum).ToArray<int>();
			if (this._filter != null)
			{
				array = this._filter.GetHaircutIndices(this._bodyGenerator.Character);
				array2 = this._filter.GetFacialHairIndices(this._bodyGenerator.Character);
			}
			this.BeardTypes.Clear();
			for (int i = 0; i < this.beardNum; i++)
			{
				if (array2.Contains(i) || i == this._faceGenerationParams._curBeard)
				{
					FacegenListItemVM facegenListItemVM = new FacegenListItemVM("FaceGen\\Beard\\img" + i, i, new Action<FacegenListItemVM, bool>(this.SetSelectedBeardType));
					this.BeardTypes.Add(facegenListItemVM);
				}
			}
			string text = ((this._selectedGender == 1) ? "Female" : "Male");
			this.HairTypes.Clear();
			for (int j = 0; j < num; j++)
			{
				if (array.Contains(j) || j == this._faceGenerationParams._currentHair)
				{
					FacegenListItemVM facegenListItemVM2 = new FacegenListItemVM(string.Concat(new object[] { "FaceGen\\Hair\\", text, "\\img", j }), j, new Action<FacegenListItemVM, bool>(this.SetSelectedHairType));
					this.HairTypes.Add(facegenListItemVM2);
				}
			}
			this.TaintTypes.Clear();
			for (int k = 0; k < this.faceTattooNum; k++)
			{
				FacegenListItemVM facegenListItemVM3 = new FacegenListItemVM(string.Concat(new object[] { "FaceGen\\Tattoo\\", text, "\\img", k }), k, new Action<FacegenListItemVM, bool>(this.SetSelectedTattooType));
				this.TaintTypes.Add(facegenListItemVM3);
			}
			this.UpdateFace(-20, (float)this._selectedRace, true, true);
			this.UpdateFace(-1, (float)this._selectedGender, true, true);
			if (this.BeardTypes.Count > 0)
			{
				this.SetSelectedBeardType(this._faceGenerationParams._curBeard, false);
			}
			this.SetSelectedHairType(this._faceGenerationParams._currentHair, false);
			if (this.TaintTypes.Count > 0)
			{
				this.SetSelectedTattooType(this.TaintTypes[this._faceGenerationParams._curFaceTattoo], false);
			}
			this.UpdateVoiceIndiciesFromCurrentParameters();
			if (!this._openedFromMultiplayer)
			{
				this._faceGenerationParams._currentVoice = this.GetVoiceRealIndex(0);
			}
			this._faceGenerationParams._curFaceTexture = MBMath.ClampInt(this._faceGenerationParams._curFaceTexture, 0, this.faceTextureNum - 1);
			this.FaceTypes = new FaceGenPropertyVM(-3, 0.0, (double)(this.faceTextureNum - 1), new TextObject("{=DmaP2qaR}Skin Type", null), -3, 1, (double)this._faceGenerationParams._curFaceTexture, new Action<int, float, bool, bool>(this.UpdateFace), new Action(this.AddCommand), new Action(this.ResetSliderPrevValues), true, false);
			this._faceGenerationParams._curMouthTexture = MBMath.ClampInt(this._faceGenerationParams._curMouthTexture, 0, this.mouthTextureNum - 1);
			this.TeethTypes = new FaceGenPropertyVM(-14, 0.0, (double)(this.mouthTextureNum - 1), new TextObject("{=l2CNxPXG}Teeth Type", null), -14, 4, (double)this._faceGenerationParams._curMouthTexture, new Action<int, float, bool, bool>(this.UpdateFace), new Action(this.AddCommand), new Action(this.ResetSliderPrevValues), true, false);
			this._faceGenerationParams._curEyebrow = MBMath.ClampInt(this._faceGenerationParams._curEyebrow, 0, this.eyebrowTextureNum - 1);
			this.EyebrowTypes = new FaceGenPropertyVM(-15, 0.0, (double)(this.eyebrowTextureNum - 1), new TextObject("{=bIcFZT6L}Eyebrow Type", null), -15, 4, (double)this._faceGenerationParams._curEyebrow, new Action<int, float, bool, bool>(this.UpdateFace), new Action(this.AddCommand), new Action(this.ResetSliderPrevValues), true, false);
		}

		private void UpdateVoiceIndiciesFromCurrentParameters()
		{
			this._isVoiceTypeUsableForOnlyNpc = MBBodyProperties.GetVoiceTypeUsableForPlayerData(this._faceGenerationParams._currentRace, this.SelectedGender, (float)((int)this._faceGenerationParams._curAge), this._newSoundPresetSize);
			int num = 0;
			for (int i = 0; i < this._isVoiceTypeUsableForOnlyNpc.Count; i++)
			{
				if (!this._isVoiceTypeUsableForOnlyNpc[i])
				{
					num++;
				}
			}
			this.SoundPreset = new FaceGenPropertyVM(-9, 0.0, (double)(num - 1), new TextObject("{=macpKFaG}Voice", null), -9, 0, (double)this.GetVoiceUIIndex(), new Action<int, float, bool, bool>(this.UpdateFace), new Action(this.AddCommand), new Action(this.ResetSliderPrevValues), true, false);
			Debug.Print("Called GetVoiceTypeUsableForPlayerData", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		private void UpdateFace()
		{
			if (this._characterRefreshEnabled)
			{
				this._bodyGenerator.RefreshFace(this._faceGenerationParams, this.IsDressed);
				this._faceGeneratorScreen.RefreshCharacterEntity();
			}
			this.SaveGenderBasedSelectedValues();
		}

		private void UpdateFace(int keyNo, float value, bool calledFromInit, bool isNeedRefresh = true)
		{
			if (this._enforceConstraints)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (keyNo > -1)
			{
				this._faceGenerationParams.KeyWeights[keyNo] = value;
				this._enforceConstraints = MBBodyProperties.EnforceConstraints(ref this._faceGenerationParams);
				flag3 = this._enforceConstraints && !calledFromInit;
			}
			else
			{
				switch (keyNo)
				{
				case -20:
					this.RestoreRaceGenderBasedSelectedValues();
					this._faceGenerationParams.SetRaceGenderAndAdjustParams((int)value, this.SelectedGender, (int)this._faceGenerationParams._curAge);
					goto IL_2CB;
				case -19:
					this._faceGenerationParams._voicePitch = value;
					goto IL_2CB;
				case -18:
					this._faceGenerationParams._curBuild = value;
					this._enforceConstraints = MBBodyProperties.EnforceConstraints(ref this._faceGenerationParams);
					flag3 = this._enforceConstraints && !calledFromInit;
					goto IL_2CB;
				case -17:
					this._faceGenerationParams._curWeight = value;
					this._enforceConstraints = MBBodyProperties.EnforceConstraints(ref this._faceGenerationParams);
					flag3 = this._enforceConstraints && !calledFromInit;
					goto IL_2CB;
				case -16:
					this._faceGenerationParams._heightMultiplier = (this._openedFromMultiplayer ? MathF.Clamp(value, 0.25f, 0.75f) : MathF.Clamp(value, 0f, 1f));
					this._enforceConstraints = MBBodyProperties.EnforceConstraints(ref this._faceGenerationParams);
					flag3 = this._enforceConstraints && !calledFromInit;
					flag2 = true;
					goto IL_2CB;
				case -15:
					this._faceGenerationParams._curEyebrow = (int)value;
					goto IL_2CB;
				case -14:
					this._faceGenerationParams._curMouthTexture = (int)value;
					goto IL_2CB;
				case -12:
					this._faceGenerationParams._curEyeColorOffset = value;
					goto IL_2CB;
				case -11:
				{
					this._faceGenerationParams._curAge = value;
					this._enforceConstraints = MBBodyProperties.EnforceConstraints(ref this._faceGenerationParams);
					flag3 = this._enforceConstraints && !calledFromInit;
					flag = true;
					flag2 = true;
					BodyMeshMaturityType maturityTypeWithAge = FaceGen.GetMaturityTypeWithAge(this._faceGenerationParams._curAge);
					if (this._latestMaturityType != maturityTypeWithAge)
					{
						this.UpdateVoiceIndiciesFromCurrentParameters();
						this._latestMaturityType = maturityTypeWithAge;
						goto IL_2CB;
					}
					goto IL_2CB;
				}
				case -10:
					this._faceGenerationParams._curFaceTattoo = (int)value;
					goto IL_2CB;
				case -9:
					this._faceGenerationParams._currentVoice = this.GetVoiceRealIndex((int)value);
					goto IL_2CB;
				case -7:
					this._faceGenerationParams._curBeard = (int)value;
					goto IL_2CB;
				case -6:
					this._faceGenerationParams._currentHair = (int)value;
					goto IL_2CB;
				case -3:
					this._faceGenerationParams._curFaceTexture = (int)value;
					goto IL_2CB;
				case -1:
					this.RestoreRaceGenderBasedSelectedValues();
					this._faceGenerationParams.SetRaceGenderAndAdjustParams(this._faceGenerationParams._currentRace, (int)value, (int)this._faceGenerationParams._curAge);
					goto IL_2CB;
				}
				MBDebug.ShowWarning("Unknown preset!");
			}
			IL_2CB:
			if (flag3)
			{
				this.UpdateFacegen();
			}
			if (isNeedRefresh)
			{
				this.UpdateFace();
			}
			else
			{
				this.SaveGenderBasedSelectedValues();
			}
			if (!calledFromInit && !this._isRandomizing && keyNo < 0)
			{
				if (keyNo != -14)
				{
					if (keyNo == -9)
					{
						this._faceGeneratorScreen.MakeVoice(this._faceGenerationParams._currentVoice, this._faceGenerationParams._voicePitch);
					}
				}
				else
				{
					this._faceGeneratorScreen.SetFacialAnimation("facegen_teeth", false);
				}
			}
			this._enforceConstraints = false;
			if (flag)
			{
				this.OnAgeChanged();
			}
			if (flag2)
			{
				this.OnHeightChanged();
			}
		}

		private void RestoreRaceGenderBasedSelectedValues()
		{
			if (this.genderBasedSelectedValues[this.SelectedGender].FaceTexture > -1)
			{
				this._faceGenerationParams._curFaceTexture = this.genderBasedSelectedValues[this.SelectedGender].FaceTexture;
			}
			if (this.genderBasedSelectedValues[this.SelectedGender].Hair > -1)
			{
				this._faceGenerationParams._currentHair = this.genderBasedSelectedValues[this.SelectedGender].Hair;
			}
			if (this.genderBasedSelectedValues[this.SelectedGender].Beard > -1)
			{
				this._faceGenerationParams._curBeard = this.genderBasedSelectedValues[this.SelectedGender].Beard;
			}
			if (this.genderBasedSelectedValues[this.SelectedGender].Tattoo > -1)
			{
				this._faceGenerationParams._curFaceTattoo = this.genderBasedSelectedValues[this.SelectedGender].Tattoo;
			}
			if (this.genderBasedSelectedValues[this.SelectedGender].SoundPreset > -1)
			{
				this._faceGenerationParams._currentVoice = this.genderBasedSelectedValues[this.SelectedGender].SoundPreset;
			}
			if (this.genderBasedSelectedValues[this.SelectedGender].MouthTexture > -1)
			{
				this._faceGenerationParams._curMouthTexture = this.genderBasedSelectedValues[this.SelectedGender].MouthTexture;
			}
			if (this.genderBasedSelectedValues[this.SelectedGender].EyebrowTexture > -1)
			{
				this._faceGenerationParams._curEyebrow = this.genderBasedSelectedValues[this.SelectedGender].EyebrowTexture;
			}
		}

		private void SaveGenderBasedSelectedValues()
		{
			this.genderBasedSelectedValues[this.SelectedGender].FaceTexture = this._faceGenerationParams._curFaceTexture;
			this.genderBasedSelectedValues[this.SelectedGender].Hair = this._faceGenerationParams._currentHair;
			this.genderBasedSelectedValues[this.SelectedGender].Beard = this._faceGenerationParams._curBeard;
			this.genderBasedSelectedValues[this.SelectedGender].Tattoo = this._faceGenerationParams._curFaceTattoo;
			this.genderBasedSelectedValues[this.SelectedGender].SoundPreset = this._faceGenerationParams._currentVoice;
			this.genderBasedSelectedValues[this.SelectedGender].MouthTexture = this._faceGenerationParams._curMouthTexture;
			this.genderBasedSelectedValues[this.SelectedGender].EyebrowTexture = this._faceGenerationParams._curEyebrow;
		}

		private int GetVoiceUIIndex()
		{
			int num = 0;
			for (int i = 0; i < this._faceGenerationParams._currentVoice; i++)
			{
				if (!this._isVoiceTypeUsableForOnlyNpc[i])
				{
					num++;
				}
			}
			return num;
		}

		private int GetVoiceRealIndex(int UIValue)
		{
			int num = 0;
			for (int i = 0; i < this._newSoundPresetSize; i++)
			{
				if (!this._isVoiceTypeUsableForOnlyNpc[i])
				{
					if (num == UIValue)
					{
						return i;
					}
					num++;
				}
			}
			Debug.FailedAssert("Cannot calculate voice index", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\FaceGenerator\\FaceGenVM.cs", "GetVoiceRealIndex", 925);
			return -1;
		}

		public void ExecuteHearCurrentVoiceSample()
		{
			this._faceGeneratorScreen.MakeVoice(this._faceGenerationParams._currentVoice, this._faceGenerationParams._voicePitch);
		}

		public void ExecuteReset()
		{
			string text = GameTexts.FindText("str_reset", null).ToString();
			string text2 = new TextObject("{=hiKTvBgF}Are you sure want to reset changes done in this tab? Your changes will be lost.", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(text, text2, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.Reset), null, "", 0f, null, null, null), false, false);
		}

		private void Reset()
		{
			this.AddCommand();
			this._characterRefreshEnabled = false;
			bool flag = this._initialRace != this.RaceSelector.SelectedIndex;
			switch (this.Tab)
			{
			case 0:
				this.SelectedGender = this._initialGender;
				this.RaceSelector.SelectedIndex = this._initialRace;
				this.SoundPreset.Reset();
				this.SkinColorSelector.SelectedIndex = MathF.Round(this._initialSelectedSkinColor * (float)(this._skinColors.Count - 1));
				break;
			case 1:
				this.FaceTypes.Reset();
				break;
			case 2:
				this.EyebrowTypes.Reset();
				break;
			case 4:
				this.TeethTypes.Reset();
				break;
			case 5:
			{
				FacegenListItemVM facegenListItemVM = ((this._selectedGender == 1) ? this.BeardTypes.FirstOrDefault<FacegenListItemVM>() : this.BeardTypes.FirstOrDefault((FacegenListItemVM b) => b.Index == this._initialSelectedBeardType));
				this.SetSelectedBeardType(facegenListItemVM, false);
				if (this._initialSelectedHairType > this.HairTypes.Count - 1)
				{
					this.SetSelectedHairType(this.HairTypes[this.HairTypes.Count - 1], false);
				}
				else
				{
					this.SetSelectedHairType(this.HairTypes[this._initialSelectedHairType], false);
				}
				this.HairColorSelector.SelectedIndex = MathF.Round(this._initialSelectedHairColor * (float)(this._hairColors.Count - 1));
				break;
			}
			case 6:
				if (this._initialSelectedTaintType > this.TaintTypes.Count - 1)
				{
					this.SetSelectedTattooType(this.TaintTypes[this.TaintTypes.Count - 1], false);
				}
				else
				{
					this.SetSelectedTattooType(this.TaintTypes[this._initialSelectedTaintType], false);
				}
				this.TattooColorSelector.SelectedIndex = MathF.Round(this._initialSelectedTaintColor * (float)(this._tattooColors.Count - 1));
				break;
			}
			foreach (FaceGenPropertyVM faceGenPropertyVM in this._tabProperties[(FaceGenVM.FaceGenTabs)this.Tab])
			{
				if (faceGenPropertyVM.TabID == this.Tab)
				{
					faceGenPropertyVM.Reset();
				}
			}
			this._characterRefreshEnabled = true;
			if (flag)
			{
				this.Refresh(true);
			}
			this.UpdateFace();
		}

		private void ResetAll()
		{
			this.SelectedGender = this._initialGender;
			this._raceSelector.SelectedIndex = this._initialRace;
			foreach (KeyValuePair<FaceGenVM.FaceGenTabs, MBBindingList<FaceGenPropertyVM>> keyValuePair in this._tabProperties)
			{
				foreach (FaceGenPropertyVM faceGenPropertyVM in keyValuePair.Value)
				{
					faceGenPropertyVM.Reset();
				}
			}
			this.FaceTypes.Reset();
			this.SoundPreset.Reset();
			this.TeethTypes.Reset();
			this.EyebrowTypes.Reset();
			this._faceGenerationParams = this._bodyGenerator.InitBodyGenerator(this.IsDressed);
			this._undoCommands.Clear();
			this._redoCommands.Clear();
			this._characterRefreshEnabled = true;
			this.Refresh(FaceGen.UpdateDeformKeys);
		}

		public void ExecuteResetAll()
		{
			string text = GameTexts.FindText("str_reset_all", null).ToString();
			string text2 = new TextObject("{=1hnq3Kb1}Are you sure want to reset all properties? Your changes will be lost.", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(text, text2, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.ResetAll), null, "", 0f, null, null, null), false, false);
		}

		public void ExecuteRandomize()
		{
			this.AddCommand();
			this._characterRefreshEnabled = false;
			this._isRandomizing = true;
			foreach (FaceGenPropertyVM faceGenPropertyVM in this._tabProperties[(FaceGenVM.FaceGenTabs)this.Tab])
			{
				faceGenPropertyVM.Randomize();
			}
			switch (this.Tab)
			{
			case 0:
				this.SkinColorSelector.SelectedIndex = MBRandom.RandomInt(this._skinColors.Count);
				break;
			case 1:
				this.FaceTypes.Value = (float)MBRandom.RandomInt((int)this.FaceTypes.Max + 1);
				break;
			case 2:
				this.EyebrowTypes.Value = (float)MBRandom.RandomInt((int)this.EyebrowTypes.Max + 1);
				break;
			case 4:
				this.TeethTypes.Value = (float)MBRandom.RandomInt((int)this.TeethTypes.Max + 1);
				break;
			case 5:
				this.SetSelectedBeardType(this.BeardTypes[MBRandom.RandomInt(this.BeardTypes.Count)], false);
				this.SetSelectedHairType(this.HairTypes[MBRandom.RandomInt(this.HairTypes.Count)], false);
				this.HairColorSelector.SelectedIndex = MBRandom.RandomInt(this._hairColors.Count);
				break;
			case 6:
				this.SetSelectedTattooType(this.TaintTypes[MBRandom.RandomInt(this.TaintTypes.Count)], false);
				this.TattooColorSelector.SelectedIndex = MBRandom.RandomInt(this._tattooColors.Count);
				break;
			}
			this._characterRefreshEnabled = true;
			this._isRandomizing = false;
			this.UpdateFace();
		}

		public void ExecuteRandomizeAll()
		{
			this.AddCommand();
			this._characterRefreshEnabled = false;
			this._isRandomizing = true;
			foreach (KeyValuePair<FaceGenVM.FaceGenTabs, MBBindingList<FaceGenPropertyVM>> keyValuePair in this._tabProperties)
			{
				foreach (FaceGenPropertyVM faceGenPropertyVM in keyValuePair.Value)
				{
					faceGenPropertyVM.Randomize();
				}
			}
			this.FaceTypes.Value = (float)MBRandom.RandomInt((int)this.FaceTypes.Max + 1);
			if (this.BeardTypes.Count > 0)
			{
				this.SetSelectedBeardType(this.BeardTypes[MBRandom.RandomInt(this.BeardTypes.Count)], false);
			}
			if (this.HairTypes.Count > 0)
			{
				this.SetSelectedHairType(this.HairTypes[MBRandom.RandomInt(this.HairTypes.Count)], false);
			}
			this.EyebrowTypes.Value = (float)MBRandom.RandomInt((int)this.EyebrowTypes.Max + 1);
			this.TeethTypes.Value = (float)MBRandom.RandomInt((int)this.TeethTypes.Max + 1);
			if (this.TaintTypes.Count > 0)
			{
				if (MBRandom.RandomFloat < this._faceGenerationParams._tattooZeroProbability)
				{
					this.SetSelectedTattooType(this.TaintTypes[0], false);
				}
				else
				{
					this.SetSelectedTattooType(this.TaintTypes[MBRandom.RandomInt(1, this.TaintTypes.Count)], false);
				}
			}
			this.TattooColorSelector.SelectedIndex = MBRandom.RandomInt(this._tattooColors.Count);
			this.HairColorSelector.SelectedIndex = MBRandom.RandomInt(this._hairColors.Count);
			this.SkinColorSelector.SelectedIndex = MBRandom.RandomInt(this._skinColors.Count);
			this._characterRefreshEnabled = true;
			this.UpdateFace();
			this._isRandomizing = false;
		}

		public void ExecuteCancel()
		{
			this._faceGeneratorScreen.Cancel();
		}

		public void ExecuteDone()
		{
			this._faceGeneratorScreen.Done();
		}

		public void ExecuteRedo()
		{
			if (this._redoCommands.Count > 0)
			{
				int num = this._redoCommands.Count - 1;
				BodyProperties bodyProperties = this._redoCommands[num].BodyProperties;
				int gender = this._redoCommands[num].Gender;
				int race = this._redoCommands[num].Race;
				this._redoCommands.RemoveAt(num);
				this._undoCommands.Add(new FaceGenVM.UndoRedoKey(this._faceGenerationParams._currentGender, this._faceGenerationParams._currentRace, this._bodyGenerator.CurrentBodyProperties));
				this._characterRefreshEnabled = false;
				this.SetBodyProperties(bodyProperties, false, race, gender, false);
				this._characterRefreshEnabled = true;
			}
		}

		public void ExecuteUndo()
		{
			if (this._undoCommands.Count > 0)
			{
				int num = this._undoCommands.Count - 1;
				BodyProperties bodyProperties = this._undoCommands[num].BodyProperties;
				int gender = this._undoCommands[num].Gender;
				int race = this._undoCommands[num].Race;
				this._undoCommands.RemoveAt(num);
				this._redoCommands.Add(new FaceGenVM.UndoRedoKey(this._faceGenerationParams._currentGender, this._faceGenerationParams._currentRace, this._bodyGenerator.CurrentBodyProperties));
				this._characterRefreshEnabled = false;
				this.SetBodyProperties(bodyProperties, false, race, gender, false);
				this._characterRefreshEnabled = true;
			}
		}

		public void ExecuteChangeClothing()
		{
			if (this.IsDressed)
			{
				this._faceGeneratorScreen.UndressCharacterEntity();
				this.IsDressed = false;
				return;
			}
			this._faceGeneratorScreen.DressCharacterEntity();
			this.IsDressed = true;
		}

		public void AddCommand()
		{
			if (this._characterRefreshEnabled)
			{
				if (this._undoCommands.Count + 1 == this._undoCommands.Capacity)
				{
					this._undoCommands.RemoveAt(0);
				}
				this._undoCommands.Add(new FaceGenVM.UndoRedoKey(this._faceGenerationParams._currentGender, this._faceGenerationParams._currentRace, this._bodyGenerator.CurrentBodyProperties));
				this._redoCommands.Clear();
			}
		}

		private void UpdateTitle()
		{
		}

		private void ExecuteGoToIndex(int index)
		{
			this._goToIndex(index);
		}

		public void SetBodyProperties(BodyProperties bodyProperties, bool ignoreDebugValues, int race = 0, int gender = -1, bool recordChange = false)
		{
			this._characterRefreshEnabled = false;
			bool flag = false;
			if (gender == -1)
			{
				this._faceGenerationParams._currentGender = this._selectedGender;
			}
			else
			{
				this._faceGenerationParams._currentGender = gender;
			}
			if (this._isRaceAvailable)
			{
				flag = this._faceGenerationParams._currentRace != race;
				this._faceGenerationParams._currentRace = race;
			}
			if (this._openedFromMultiplayer)
			{
				bodyProperties = bodyProperties.ClampForMultiplayer();
			}
			float num = (this._isAgeAvailable ? bodyProperties.Age : this._bodyGenerator.CurrentBodyProperties.Age);
			float num2 = (this._isWeightAvailable ? bodyProperties.Weight : this._bodyGenerator.CurrentBodyProperties.Weight);
			float num3 = (this._isWeightAvailable ? bodyProperties.Build : this._bodyGenerator.CurrentBodyProperties.Build);
			bodyProperties = new BodyProperties(new DynamicBodyProperties(num, num2, num3), bodyProperties.StaticProperties);
			this._bodyGenerator.CurrentBodyProperties = bodyProperties;
			MBBodyProperties.GetParamsFromKey(ref this._faceGenerationParams, bodyProperties, this.IsDressed && this._bodyGenerator.Character.Equipment.EarsAreHidden, this.IsDressed && this._bodyGenerator.Character.Equipment.MouthIsHidden);
			if (flag)
			{
				this._characterRefreshEnabled = true;
				this.Refresh(true);
			}
			else
			{
				this.UpdateFacegen();
				this._characterRefreshEnabled = true;
				this.UpdateFace();
			}
			if (recordChange)
			{
				this._characterRefreshEnabled = true;
				this.AddCommand();
			}
		}

		private void ResetSliderPrevValues()
		{
			foreach (MBBindingList<FaceGenPropertyVM> mbbindingList in this._tabProperties.Values)
			{
				foreach (FaceGenPropertyVM faceGenPropertyVM in mbbindingList)
				{
					faceGenPropertyVM.PrevValue = -1.0;
				}
			}
		}

		public void UpdateFacegen()
		{
			foreach (MBBindingList<FaceGenPropertyVM> mbbindingList in this._tabProperties.Values)
			{
				foreach (FaceGenPropertyVM faceGenPropertyVM in mbbindingList)
				{
					if (faceGenPropertyVM.KeyNo < 0)
					{
						switch (faceGenPropertyVM.KeyNo)
						{
						case -18:
							faceGenPropertyVM.Value = this._faceGenerationParams._curBuild;
							break;
						case -17:
							faceGenPropertyVM.Value = this._faceGenerationParams._curWeight;
							break;
						case -16:
							faceGenPropertyVM.Value = (this._openedFromMultiplayer ? MathF.Clamp(this._faceGenerationParams._heightMultiplier, 0.25f, 0.75f) : MathF.Clamp(this._faceGenerationParams._heightMultiplier, 0f, 1f));
							break;
						case -12:
							faceGenPropertyVM.Value = this._faceGenerationParams._curEyeColorOffset;
							break;
						case -11:
							faceGenPropertyVM.Value = this._faceGenerationParams._curAge;
							break;
						}
					}
					else
					{
						faceGenPropertyVM.Value = this._faceGenerationParams.KeyWeights[faceGenPropertyVM.KeyNo];
					}
					faceGenPropertyVM.PrevValue = -1.0;
				}
			}
			this.SelectedGender = this._faceGenerationParams._currentGender;
			this.SoundPreset.Value = (float)this.GetVoiceUIIndex();
			this.FaceTypes.Value = (float)this._faceGenerationParams._curFaceTexture;
			this.EyebrowTypes.Value = (float)this._faceGenerationParams._curEyebrow;
			this.TeethTypes.Value = (float)this._faceGenerationParams._curMouthTexture;
			if (this.TaintTypes.Count > this._faceGenerationParams._curFaceTattoo)
			{
				this.SetSelectedTattooType(this.TaintTypes[this._faceGenerationParams._curFaceTattoo], false);
			}
			if (this.BeardTypes.Count > this._faceGenerationParams._curBeard)
			{
				this.SetSelectedBeardType(this.BeardTypes[this._faceGenerationParams._curBeard], false);
			}
			if (this.HairTypes.Count > this._faceGenerationParams._currentHair)
			{
				this.SetSelectedHairType(this.HairTypes[this._faceGenerationParams._currentHair], false);
			}
			this.SkinColorSelector.SelectedIndex = MathF.Round(this._faceGenerationParams._curSkinColorOffset * (float)(this._skinColors.Count - 1));
			this.HairColorSelector.SelectedIndex = MathF.Round(this._faceGenerationParams._curHairColorOffset * (float)(this._hairColors.Count - 1));
			this.TattooColorSelector.SelectedIndex = MathF.Round(this._faceGenerationParams._curFaceTattooColorOffset1 * (float)(this._tattooColors.Count - 1));
		}

		private void SetSelectedHairType(FacegenListItemVM item, bool addCommand)
		{
			if (this._selectedHairType != null)
			{
				this._selectedHairType.IsSelected = false;
			}
			this._selectedHairType = item;
			this._selectedHairType.IsSelected = true;
			this._faceGenerationParams._currentHair = item.Index;
			if (!addCommand)
			{
				return;
			}
			this.AddCommand();
			this.UpdateFace(-6, (float)item.Index, false, true);
		}

		private void SetSelectedHairType(int index, bool addCommand)
		{
			foreach (FacegenListItemVM facegenListItemVM in this.HairTypes)
			{
				if (facegenListItemVM.Index == index)
				{
					this.SetSelectedHairType(facegenListItemVM, addCommand);
					break;
				}
			}
		}

		private void SetSelectedTattooType(FacegenListItemVM item, bool addCommand)
		{
			if (this._selectedTaintType != null)
			{
				this._selectedTaintType.IsSelected = false;
			}
			this._selectedTaintType = item;
			this._selectedTaintType.IsSelected = true;
			this._faceGenerationParams._curFaceTattoo = item.Index;
			if (!addCommand)
			{
				return;
			}
			this.AddCommand();
			this.UpdateFace(-10, (float)item.Index, false, true);
		}

		private void SetSelectedBeardType(FacegenListItemVM item, bool addCommand)
		{
			if (this._selectedBeardType != null)
			{
				this._selectedBeardType.IsSelected = false;
			}
			this._selectedBeardType = item;
			this._selectedBeardType.IsSelected = true;
			this._faceGenerationParams._curBeard = item.Index;
			if (!addCommand)
			{
				return;
			}
			this.AddCommand();
			this.UpdateFace(-7, (float)item.Index, false, true);
		}

		private void SetSelectedBeardType(int index, bool addCommand)
		{
			foreach (FacegenListItemVM facegenListItemVM in this.BeardTypes)
			{
				if (facegenListItemVM.Index == index)
				{
					this.SetSelectedBeardType(facegenListItemVM, addCommand);
					break;
				}
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey != null)
			{
				cancelInputKey.OnFinalize();
			}
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey != null)
			{
				doneInputKey.OnFinalize();
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
			for (int i = 0; i < this.CameraControlKeys.Count; i++)
			{
				this.CameraControlKeys[i].OnFinalize();
			}
		}

		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetPreviousTabInputKey(HotKey hotKey)
		{
			this.PreviousTabInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetNextTabInputKey(HotKey hotKey)
		{
			this.NextTabInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void AddCameraControlInputKey(HotKey hotKey)
		{
			InputKeyItemVM inputKeyItemVM = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.CameraControlKeys.Add(inputKeyItemVM);
		}

		public void AddCameraControlInputKey(GameKey gameKey)
		{
			InputKeyItemVM inputKeyItemVM = InputKeyItemVM.CreateFromGameKey(gameKey, true);
			this.CameraControlKeys.Add(inputKeyItemVM);
		}

		public void AddCameraControlInputKey(GameAxisKey gameAxisKey)
		{
			TextObject textObject = Module.CurrentModule.GlobalTextManager.FindText("str_key_name", typeof(FaceGenHotkeyCategory).Name + "_" + gameAxisKey.Id);
			InputKeyItemVM inputKeyItemVM = InputKeyItemVM.CreateFromForcedID(gameAxisKey.AxisKey.ToString(), textObject, true);
			this.CameraControlKeys.Add(inputKeyItemVM);
		}

		[DataSourceProperty]
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

		[DataSourceProperty]
		public MBBindingList<InputKeyItemVM> CameraControlKeys
		{
			get
			{
				return this._cameraControlKeys;
			}
			set
			{
				if (value != this._cameraControlKeys)
				{
					this._cameraControlKeys = value;
					base.OnPropertyChangedWithValue<MBBindingList<InputKeyItemVM>>(value, "CameraControlKeys");
				}
			}
		}

		[DataSourceProperty]
		public bool AreAllTabsEnabled
		{
			get
			{
				return this.IsBodyEnabled && this.IsFaceEnabled && this.IsEyesEnabled && this.IsNoseEnabled && this.IsMouthEnabled && this.IsHairEnabled && this.IsTaintEnabled;
			}
		}

		[DataSourceProperty]
		public bool IsBodyEnabled
		{
			get
			{
				return this._isBodyEnabled;
			}
			set
			{
				if (value != this._isBodyEnabled)
				{
					this._isBodyEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsBodyEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFaceEnabled
		{
			get
			{
				return this._isFaceEnabled;
			}
			set
			{
				if (value != this._isFaceEnabled)
				{
					this._isFaceEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsFaceEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEyesEnabled
		{
			get
			{
				return this._isEyesEnabled;
			}
			set
			{
				if (value != this._isEyesEnabled)
				{
					this._isEyesEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEyesEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsNoseEnabled
		{
			get
			{
				return this._isNoseEnabled;
			}
			set
			{
				if (value != this._isNoseEnabled)
				{
					this._isNoseEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsNoseEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsMouthEnabled
		{
			get
			{
				return this._isMouthEnabled;
			}
			set
			{
				if (value != this._isMouthEnabled)
				{
					this._isMouthEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsMouthEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsHairEnabled
		{
			get
			{
				return this._isHairEnabled;
			}
			set
			{
				if (value != this._isHairEnabled)
				{
					this._isHairEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsHairEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsTaintEnabled
		{
			get
			{
				return this._isTaintEnabled;
			}
			set
			{
				if (value != this._isTaintEnabled)
				{
					this._isTaintEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsTaintEnabled");
				}
			}
		}

		[DataSourceProperty]
		public string FlipHairLbl
		{
			get
			{
				return this._flipHairLbl;
			}
			set
			{
				if (value != this._flipHairLbl)
				{
					this._flipHairLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "FlipHairLbl");
				}
			}
		}

		[DataSourceProperty]
		public string SkinColorLbl
		{
			get
			{
				return this._skinColorLbl;
			}
			set
			{
				if (value != this._skinColorLbl)
				{
					this._skinColorLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "SkinColorLbl");
				}
			}
		}

		[DataSourceProperty]
		public string RaceLbl
		{
			get
			{
				return this._raceLbl;
			}
			set
			{
				if (value != this._raceLbl)
				{
					this._raceLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "RaceLbl");
				}
			}
		}

		[DataSourceProperty]
		public string GenderLbl
		{
			get
			{
				return this._genderLbl;
			}
			set
			{
				if (value != this._genderLbl)
				{
					this._genderLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "GenderLbl");
				}
			}
		}

		[DataSourceProperty]
		public string CancelBtnLbl
		{
			get
			{
				return this._cancelBtnLbl;
			}
			set
			{
				if (value != this._cancelBtnLbl)
				{
					this._cancelBtnLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelBtnLbl");
				}
			}
		}

		[DataSourceProperty]
		public string DoneBtnLbl
		{
			get
			{
				return this._doneBtnLbl;
			}
			set
			{
				if (value != this._doneBtnLbl)
				{
					this._doneBtnLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneBtnLbl");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel BodyHint
		{
			get
			{
				return this._bodyHint;
			}
			set
			{
				if (value != this._bodyHint)
				{
					this._bodyHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "BodyHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel FaceHint
		{
			get
			{
				return this._faceHint;
			}
			set
			{
				if (value != this._faceHint)
				{
					this._faceHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "FaceHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel EyesHint
		{
			get
			{
				return this._eyesHint;
			}
			set
			{
				if (value != this._eyesHint)
				{
					this._eyesHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "EyesHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel NoseHint
		{
			get
			{
				return this._noseHint;
			}
			set
			{
				if (value != this._noseHint)
				{
					this._noseHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "NoseHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel HairHint
		{
			get
			{
				return this._hairHint;
			}
			set
			{
				if (value != this._hairHint)
				{
					this._hairHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "HairHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel TaintHint
		{
			get
			{
				return this._taintHint;
			}
			set
			{
				if (value != this._taintHint)
				{
					this._taintHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "TaintHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel MouthHint
		{
			get
			{
				return this._mouthHint;
			}
			set
			{
				if (value != this._mouthHint)
				{
					this._mouthHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "MouthHint");
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
		public HintViewModel RandomizeAllHint
		{
			get
			{
				return this._randomizeAllHint;
			}
			set
			{
				if (value != this._randomizeAllHint)
				{
					this._randomizeAllHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "RandomizeAllHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ResetHint
		{
			get
			{
				return this._resetHint;
			}
			set
			{
				if (value != this._resetHint)
				{
					this._resetHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ResetHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ResetAllHint
		{
			get
			{
				return this._resetAllHint;
			}
			set
			{
				if (value != this._resetAllHint)
				{
					this._resetAllHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ResetAllHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ClothHint
		{
			get
			{
				return this._clothHint;
			}
			set
			{
				if (value != this._clothHint)
				{
					this._clothHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ClothHint");
				}
			}
		}

		[DataSourceProperty]
		public int HairNum
		{
			get
			{
				return this.hairNum;
			}
			set
			{
				if (value != this.hairNum)
				{
					this.hairNum = value;
					base.OnPropertyChangedWithValue(value, "HairNum");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> SkinColorSelector
		{
			get
			{
				return this._skinColorSelector;
			}
			set
			{
				if (value != this._skinColorSelector)
				{
					this._skinColorSelector = value;
					base.OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "SkinColorSelector");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> HairColorSelector
		{
			get
			{
				return this._hairColorSelector;
			}
			set
			{
				if (value != this._hairColorSelector)
				{
					this._hairColorSelector = value;
					base.OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "HairColorSelector");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> TattooColorSelector
		{
			get
			{
				return this._tattooColorSelector;
			}
			set
			{
				if (value != this._tattooColorSelector)
				{
					this._tattooColorSelector = value;
					base.OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "TattooColorSelector");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> RaceSelector
		{
			get
			{
				return this._raceSelector;
			}
			set
			{
				if (value != this._raceSelector)
				{
					this._raceSelector = value;
					base.OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "RaceSelector");
				}
			}
		}

		[DataSourceProperty]
		public int Tab
		{
			get
			{
				return this._tab;
			}
			set
			{
				if (this._tab != value)
				{
					this._tab = value;
					base.OnPropertyChangedWithValue(value, "Tab");
					if (value == 0)
					{
						this._faceGeneratorScreen.ChangeToBodyCamera();
					}
				}
				if (value == 2)
				{
					this._faceGeneratorScreen.ChangeToEyeCamera();
				}
				else if (value == 3)
				{
					this._faceGeneratorScreen.ChangeToNoseCamera();
				}
				else if (value == 4)
				{
					this._faceGeneratorScreen.ChangeToMouthCamera();
				}
				else if (value == 1 || value == 6)
				{
					this._faceGeneratorScreen.ChangeToFaceCamera();
				}
				else if (value == 5)
				{
					this._faceGeneratorScreen.ChangeToHairCamera();
				}
				this.UpdateTitle();
			}
		}

		[DataSourceProperty]
		public int SelectedGender
		{
			get
			{
				return this._selectedGender;
			}
			set
			{
				if (this._initialGender == -1)
				{
					this._initialGender = value;
				}
				if (this._selectedGender != value)
				{
					this.AddCommand();
					this._selectedGender = value;
					this.UpdateRaceAndGenderBasedResources();
					this.Refresh(FaceGen.UpdateDeformKeys);
					base.OnPropertyChangedWithValue(value, "SelectedGender");
					base.OnPropertyChanged("IsFemale");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFemale
		{
			get
			{
				return this.SelectedGender != 0;
			}
		}

		[DataSourceProperty]
		public MBBindingList<FaceGenPropertyVM> BodyProperties
		{
			get
			{
				return this._bodyProperties;
			}
			set
			{
				if (value != this._bodyProperties)
				{
					this._bodyProperties = value;
					base.OnPropertyChangedWithValue<MBBindingList<FaceGenPropertyVM>>(value, "BodyProperties");
				}
			}
		}

		[DataSourceProperty]
		public bool CanChangeGender
		{
			get
			{
				return this._canChangeGender;
			}
			set
			{
				if (value != this._canChangeGender)
				{
					this._canChangeGender = value;
					base.OnPropertyChangedWithValue(value, "CanChangeGender");
				}
			}
		}

		[DataSourceProperty]
		public bool CanChangeRace
		{
			get
			{
				return this._canChangeRace;
			}
			set
			{
				if (value != this._canChangeRace)
				{
					this._canChangeRace = value;
					base.OnPropertyChangedWithValue(value, "CanChangeRace");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<FaceGenPropertyVM> FaceProperties
		{
			get
			{
				return this._faceProperties;
			}
			set
			{
				if (value != this._faceProperties)
				{
					this._faceProperties = value;
					base.OnPropertyChangedWithValue<MBBindingList<FaceGenPropertyVM>>(value, "FaceProperties");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<FaceGenPropertyVM> EyesProperties
		{
			get
			{
				return this._eyesProperties;
			}
			set
			{
				if (value != this._eyesProperties)
				{
					this._eyesProperties = value;
					base.OnPropertyChangedWithValue<MBBindingList<FaceGenPropertyVM>>(value, "EyesProperties");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<FaceGenPropertyVM> NoseProperties
		{
			get
			{
				return this._noseProperties;
			}
			set
			{
				if (value != this._noseProperties)
				{
					this._noseProperties = value;
					base.OnPropertyChangedWithValue<MBBindingList<FaceGenPropertyVM>>(value, "NoseProperties");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<FaceGenPropertyVM> MouthProperties
		{
			get
			{
				return this._mouthProperties;
			}
			set
			{
				if (value != this._mouthProperties)
				{
					this._mouthProperties = value;
					base.OnPropertyChangedWithValue<MBBindingList<FaceGenPropertyVM>>(value, "MouthProperties");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<FaceGenPropertyVM> HairProperties
		{
			get
			{
				return this._hairProperties;
			}
			set
			{
				if (value != this._hairProperties)
				{
					this._hairProperties = value;
					base.OnPropertyChangedWithValue<MBBindingList<FaceGenPropertyVM>>(value, "HairProperties");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<FaceGenPropertyVM> TaintProperties
		{
			get
			{
				return this._taintProperties;
			}
			set
			{
				if (value != this._taintProperties)
				{
					this._taintProperties = value;
					base.OnPropertyChangedWithValue<MBBindingList<FaceGenPropertyVM>>(value, "TaintProperties");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<FacegenListItemVM> TaintTypes
		{
			get
			{
				return this._taintTypes;
			}
			set
			{
				if (value != this._taintTypes)
				{
					this._taintTypes = value;
					base.OnPropertyChangedWithValue<MBBindingList<FacegenListItemVM>>(value, "TaintTypes");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<FacegenListItemVM> BeardTypes
		{
			get
			{
				return this._beardTypes;
			}
			set
			{
				if (value != this._beardTypes)
				{
					this._beardTypes = value;
					base.OnPropertyChangedWithValue<MBBindingList<FacegenListItemVM>>(value, "BeardTypes");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<FacegenListItemVM> HairTypes
		{
			get
			{
				return this._hairTypes;
			}
			set
			{
				if (value != this._hairTypes)
				{
					this._hairTypes = value;
					base.OnPropertyChangedWithValue<MBBindingList<FacegenListItemVM>>(value, "HairTypes");
				}
			}
		}

		[DataSourceProperty]
		public FaceGenPropertyVM SoundPreset
		{
			get
			{
				return this._soundPreset;
			}
			set
			{
				if (value != this._soundPreset)
				{
					this._soundPreset = value;
					base.OnPropertyChangedWithValue<FaceGenPropertyVM>(value, "SoundPreset");
				}
			}
		}

		[DataSourceProperty]
		public FaceGenPropertyVM EyebrowTypes
		{
			get
			{
				return this._eyebrowTypes;
			}
			set
			{
				if (value != this._eyebrowTypes)
				{
					this._eyebrowTypes = value;
					base.OnPropertyChangedWithValue<FaceGenPropertyVM>(value, "EyebrowTypes");
				}
			}
		}

		[DataSourceProperty]
		public FaceGenPropertyVM TeethTypes
		{
			get
			{
				return this._teethTypes;
			}
			set
			{
				if (value != this._teethTypes)
				{
					this._teethTypes = value;
					base.OnPropertyChangedWithValue<FaceGenPropertyVM>(value, "TeethTypes");
				}
			}
		}

		[DataSourceProperty]
		public bool FlipHairCb
		{
			get
			{
				return this._faceGenerationParams._isHairFlipped;
			}
			set
			{
				if (value != this._faceGenerationParams._isHairFlipped)
				{
					this._faceGenerationParams._isHairFlipped = value;
					base.OnPropertyChangedWithValue(value, "FlipHairCb");
					this.UpdateFace();
				}
			}
		}

		[DataSourceProperty]
		public bool IsDressed
		{
			get
			{
				return this._isDressed;
			}
			set
			{
				if (value != this._isDressed)
				{
					this._isDressed = value;
					base.OnPropertyChangedWithValue(value, "IsDressed");
				}
			}
		}

		[DataSourceProperty]
		public bool CharacterGamepadControlsEnabled
		{
			get
			{
				return this._characterGamepadControlsEnabled;
			}
			set
			{
				if (value != this._characterGamepadControlsEnabled)
				{
					this._characterGamepadControlsEnabled = value;
					base.OnPropertyChangedWithValue(value, "CharacterGamepadControlsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public FaceGenPropertyVM FaceTypes
		{
			get
			{
				return this._faceTypes;
			}
			set
			{
				if (value != this._faceTypes)
				{
					this._faceTypes = value;
					base.OnPropertyChangedWithValue<FaceGenPropertyVM>(value, "FaceTypes");
				}
			}
		}

		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
				}
			}
		}

		[DataSourceProperty]
		public int TotalStageCount
		{
			get
			{
				return this._totalStageCount;
			}
			set
			{
				if (value != this._totalStageCount)
				{
					this._totalStageCount = value;
					base.OnPropertyChangedWithValue(value, "TotalStageCount");
				}
			}
		}

		[DataSourceProperty]
		public int CurrentStageIndex
		{
			get
			{
				return this._currentStageIndex;
			}
			set
			{
				if (value != this._currentStageIndex)
				{
					this._currentStageIndex = value;
					base.OnPropertyChangedWithValue(value, "CurrentStageIndex");
				}
			}
		}

		[DataSourceProperty]
		public int FurthestIndex
		{
			get
			{
				return this._furthestIndex;
			}
			set
			{
				if (value != this._furthestIndex)
				{
					this._furthestIndex = value;
					base.OnPropertyChangedWithValue(value, "FurthestIndex");
				}
			}
		}

		private const float MultiplayerHeightSliderMinValue = 0.25f;

		private const float MultiplayerHeightSliderMaxValue = 0.75f;

		private readonly IFaceGeneratorHandler _faceGeneratorScreen;

		private bool _characterRefreshEnabled = true;

		private bool _initialValuesSet;

		private readonly BodyGenerator _bodyGenerator;

		private readonly TextObject _affirmitiveText;

		private readonly TextObject _negativeText;

		private FaceGenerationParams _faceGenerationParams = FaceGenerationParams.Create();

		private List<FaceGenVM.UndoRedoKey> _undoCommands;

		private List<FaceGenVM.UndoRedoKey> _redoCommands;

		private List<bool> _isVoiceTypeUsableForOnlyNpc;

		private MBReadOnlyList<bool> _tabAvailabilities;

		private Action<float> _onHeightChanged;

		private Action _onAgeChanged;

		private int _initialRace = -1;

		private int _initialGender = -1;

		private BodyMeshMaturityType _latestMaturityType;

		private bool _isRandomizing;

		private readonly Action<int> _goToIndex;

		private FaceGenVM.GenderBasedSelectedValue[] genderBasedSelectedValues;

		private readonly Dictionary<FaceGenVM.FaceGenTabs, MBBindingList<FaceGenPropertyVM>> _tabProperties;

		private List<uint> _skinColors;

		private List<uint> _hairColors;

		private List<uint> _tattooColors;

		private readonly bool _showDebugValues;

		private readonly bool _openedFromMultiplayer;

		private bool _enforceConstraints;

		private IFaceGeneratorCustomFilter _filter;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private InputKeyItemVM _previousTabInputKey;

		private InputKeyItemVM _nextTabInputKey;

		private MBBindingList<InputKeyItemVM> _cameraControlKeys;

		private bool _isBodyEnabled;

		private bool _isFaceEnabled;

		private bool _isEyesEnabled;

		private bool _isNoseEnabled;

		private bool _isMouthEnabled;

		private bool _isHairEnabled;

		private bool _isTaintEnabled;

		private string _cancelBtnLbl;

		private string _doneBtnLbl;

		private int _initialSelectedTaintType;

		private int _initialSelectedHairType;

		private int _initialSelectedBeardType;

		private float _initialSelectedSkinColor;

		private float _initialSelectedHairColor;

		private float _initialSelectedTaintColor;

		private string _flipHairLbl;

		private string _skinColorLbl;

		private string _raceLbl;

		private string _genderLbl;

		private FaceGenPropertyVM _heightSlider;

		private HintViewModel _bodyHint;

		private HintViewModel _faceHint;

		private HintViewModel _eyesHint;

		private HintViewModel _noseHint;

		private HintViewModel _hairHint;

		private HintViewModel _taintHint;

		private HintViewModel _mouthHint;

		private HintViewModel _redoHint;

		private HintViewModel _undoHint;

		private HintViewModel _randomizeHint;

		private HintViewModel _randomizeAllHint;

		private HintViewModel _resetHint;

		private HintViewModel _resetAllHint;

		private HintViewModel _clothHint;

		private int hairNum;

		private int beardNum;

		private int faceTextureNum;

		private int mouthTextureNum;

		private int eyebrowTextureNum;

		private int faceTattooNum;

		private int _newSoundPresetSize;

		private float _scale = 1f;

		private int _tab = -1;

		private int _selectedRace = -1;

		private int _selectedGender = -1;

		private bool _canChangeGender;

		private bool _canChangeRace;

		private bool _isDressed;

		private bool _characterGamepadControlsEnabled;

		private MBBindingList<FaceGenPropertyVM> _bodyProperties;

		private MBBindingList<FaceGenPropertyVM> _faceProperties;

		private MBBindingList<FaceGenPropertyVM> _eyesProperties;

		private MBBindingList<FaceGenPropertyVM> _noseProperties;

		private MBBindingList<FaceGenPropertyVM> _mouthProperties;

		private MBBindingList<FaceGenPropertyVM> _hairProperties;

		private MBBindingList<FaceGenPropertyVM> _taintProperties;

		private MBBindingList<FacegenListItemVM> _taintTypes;

		private MBBindingList<FacegenListItemVM> _beardTypes;

		private MBBindingList<FacegenListItemVM> _hairTypes;

		private FaceGenPropertyVM _soundPreset;

		private FaceGenPropertyVM _faceTypes;

		private FaceGenPropertyVM _teethTypes;

		private FaceGenPropertyVM _eyebrowTypes;

		private SelectorVM<SelectorItemVM> _skinColorSelector;

		private SelectorVM<SelectorItemVM> _hairColorSelector;

		private SelectorVM<SelectorItemVM> _tattooColorSelector;

		private SelectorVM<SelectorItemVM> _raceSelector;

		private FacegenListItemVM _selectedTaintType;

		private FacegenListItemVM _selectedBeardType;

		private FacegenListItemVM _selectedHairType;

		private string _title = "";

		private int _totalStageCount = -1;

		private int _currentStageIndex = -1;

		private int _furthestIndex = -1;

		public enum FaceGenTabs
		{
			None = -1,
			Body,
			Face,
			Eyes,
			Nose,
			Mouth,
			Hair,
			Taint,
			NumOfFaceGenTabs
		}

		public enum Presets
		{
			Gender = -1,
			FacePresets = -2,
			FaceType = -3,
			EyePresets = -4,
			HairBeardPreset = -5,
			HairType = -6,
			BeardType = -7,
			TaintPresets = -8,
			SoundPresets = -9,
			TaintType = -10,
			Age = -11,
			EyeColor = -12,
			HairAndBeardColor = -13,
			TeethType = -14,
			EyebrowType = -15,
			Scale = -16,
			Weight = -17,
			Build = -18,
			Pitch = -19,
			Race = -20
		}

		public struct GenderBasedSelectedValue
		{
			public void Reset()
			{
				this.Hair = -1;
				this.Beard = -1;
				this.FaceTexture = -1;
				this.MouthTexture = -1;
				this.Tattoo = -1;
				this.SoundPreset = -1;
				this.EyebrowTexture = -1;
			}

			public int Hair;

			public int Beard;

			public int FaceTexture;

			public int MouthTexture;

			public int Tattoo;

			public int SoundPreset;

			public int EyebrowTexture;
		}

		private struct UndoRedoKey
		{
			public UndoRedoKey(int gender, int race, BodyProperties bodyProperties)
			{
				this.Gender = gender;
				this.Race = race;
				this.BodyProperties = bodyProperties;
			}

			public readonly int Gender;

			public readonly int Race;

			public readonly BodyProperties BodyProperties;
		}
	}
}
