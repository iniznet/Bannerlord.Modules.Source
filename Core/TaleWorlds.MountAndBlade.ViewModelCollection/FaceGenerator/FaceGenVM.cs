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
	// Token: 0x02000107 RID: 263
	public class FaceGenVM : ViewModel
	{
		// Token: 0x17000781 RID: 1921
		// (get) Token: 0x0600170E RID: 5902 RVA: 0x0004AFE8 File Offset: 0x000491E8
		private bool _isAgeAvailable
		{
			get
			{
				return this._openedFromMultiplayer || this._showDebugValues;
			}
		}

		// Token: 0x17000782 RID: 1922
		// (get) Token: 0x0600170F RID: 5903 RVA: 0x0004AFFA File Offset: 0x000491FA
		private bool _isWeightAvailable
		{
			get
			{
				return !this._openedFromMultiplayer || this._showDebugValues;
			}
		}

		// Token: 0x17000783 RID: 1923
		// (get) Token: 0x06001710 RID: 5904 RVA: 0x0004B00C File Offset: 0x0004920C
		private bool _isBuildAvailable
		{
			get
			{
				return !this._openedFromMultiplayer || this._showDebugValues;
			}
		}

		// Token: 0x17000784 RID: 1924
		// (get) Token: 0x06001711 RID: 5905 RVA: 0x0004B01E File Offset: 0x0004921E
		private bool _isRaceAvailable
		{
			get
			{
				return (FaceGen.GetRaceCount() > 1 && !this._openedFromMultiplayer) || this._showDebugValues;
			}
		}

		// Token: 0x06001712 RID: 5906 RVA: 0x0004B038 File Offset: 0x00049238
		public void SetFaceGenerationParams(FaceGenerationParams faceGenerationParams)
		{
			this._faceGenerationParams = faceGenerationParams;
		}

		// Token: 0x06001713 RID: 5907 RVA: 0x0004B044 File Offset: 0x00049244
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

		// Token: 0x06001714 RID: 5908 RVA: 0x0004B27C File Offset: 0x0004947C
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

		// Token: 0x06001715 RID: 5909 RVA: 0x0004B6C0 File Offset: 0x000498C0
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

		// Token: 0x06001716 RID: 5910 RVA: 0x0004B740 File Offset: 0x00049940
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

		// Token: 0x06001717 RID: 5911 RVA: 0x0004B8F4 File Offset: 0x00049AF4
		private void OnSelectSkinColor(SelectorVM<SelectorItemVM> s)
		{
			this.AddCommand();
			this._faceGenerationParams._curSkinColorOffset = (float)s.SelectedIndex / (float)(this._skinColors.Count - 1);
			this.UpdateFace();
		}

		// Token: 0x06001718 RID: 5912 RVA: 0x0004B923 File Offset: 0x00049B23
		private void OnSelectTattooColor(SelectorVM<SelectorItemVM> s)
		{
			this.AddCommand();
			this._faceGenerationParams._curFaceTattooColorOffset1 = (float)s.SelectedIndex / (float)(this._tattooColors.Count - 1);
			this.UpdateFace();
		}

		// Token: 0x06001719 RID: 5913 RVA: 0x0004B952 File Offset: 0x00049B52
		private void OnSelectHairColor(SelectorVM<SelectorItemVM> s)
		{
			this.AddCommand();
			this._faceGenerationParams._curHairColorOffset = (float)s.SelectedIndex / (float)(this._hairColors.Count - 1);
			this.UpdateFace();
		}

		// Token: 0x0600171A RID: 5914 RVA: 0x0004B981 File Offset: 0x00049B81
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

		// Token: 0x0600171B RID: 5915 RVA: 0x0004B9B7 File Offset: 0x00049BB7
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

		// Token: 0x0600171C RID: 5916 RVA: 0x0004B9DF File Offset: 0x00049BDF
		private void OnAgeChanged()
		{
			Action onAgeChanged = this._onAgeChanged;
			if (onAgeChanged == null)
			{
				return;
			}
			onAgeChanged();
		}

		// Token: 0x0600171D RID: 5917 RVA: 0x0004B9F4 File Offset: 0x00049BF4
		private void SetTabAvailabilities()
		{
			this._tabAvailabilities = new MBList<bool> { this.IsBodyEnabled, this.IsFaceEnabled, this.IsEyesEnabled, this.IsNoseEnabled, this.IsMouthEnabled, this.IsHairEnabled, this.IsTaintEnabled };
		}

		// Token: 0x0600171E RID: 5918 RVA: 0x0004BA60 File Offset: 0x00049C60
		public void OnTabClicked(int index)
		{
			this.Tab = index;
		}

		// Token: 0x0600171F RID: 5919 RVA: 0x0004BA6C File Offset: 0x00049C6C
		public void SelectPreviousTab()
		{
			int num = ((this.Tab == 0) ? 6 : (this.Tab - 1));
			while (!this._tabAvailabilities[num] && num != this.Tab)
			{
				num = ((num == 0) ? 6 : (num - 1));
			}
			this.Tab = num;
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x0004BAB8 File Offset: 0x00049CB8
		public void SelectNextTab()
		{
			int num = (this.Tab + 1) % 7;
			while (!this._tabAvailabilities[num] && num != this.Tab)
			{
				num = (num + 1) % 7;
			}
			this.Tab = num;
		}

		// Token: 0x06001721 RID: 5921 RVA: 0x0004BAF8 File Offset: 0x00049CF8
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

		// Token: 0x06001722 RID: 5922 RVA: 0x0004C214 File Offset: 0x0004A414
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

		// Token: 0x06001723 RID: 5923 RVA: 0x0004C65C File Offset: 0x0004A85C
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

		// Token: 0x06001724 RID: 5924 RVA: 0x0004C72D File Offset: 0x0004A92D
		private void UpdateFace()
		{
			if (this._characterRefreshEnabled)
			{
				this._bodyGenerator.RefreshFace(this._faceGenerationParams, this.IsDressed);
				this._faceGeneratorScreen.RefreshCharacterEntity();
			}
			this.SaveGenderBasedSelectedValues();
		}

		// Token: 0x06001725 RID: 5925 RVA: 0x0004C760 File Offset: 0x0004A960
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

		// Token: 0x06001726 RID: 5926 RVA: 0x0004CAC0 File Offset: 0x0004ACC0
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

		// Token: 0x06001727 RID: 5927 RVA: 0x0004CC64 File Offset: 0x0004AE64
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

		// Token: 0x06001728 RID: 5928 RVA: 0x0004CD58 File Offset: 0x0004AF58
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

		// Token: 0x06001729 RID: 5929 RVA: 0x0004CD90 File Offset: 0x0004AF90
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

		// Token: 0x0600172A RID: 5930 RVA: 0x0004CDE2 File Offset: 0x0004AFE2
		public void ExecuteHearCurrentVoiceSample()
		{
			this._faceGeneratorScreen.MakeVoice(this._faceGenerationParams._currentVoice, this._faceGenerationParams._voicePitch);
		}

		// Token: 0x0600172B RID: 5931 RVA: 0x0004CE08 File Offset: 0x0004B008
		public void ExecuteReset()
		{
			string text = GameTexts.FindText("str_reset", null).ToString();
			string text2 = new TextObject("{=hiKTvBgF}Are you sure want to reset changes done in this tab? Your changes will be lost.", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(text, text2, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.Reset), null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x0600172C RID: 5932 RVA: 0x0004CE80 File Offset: 0x0004B080
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

		// Token: 0x0600172D RID: 5933 RVA: 0x0004D0F4 File Offset: 0x0004B2F4
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

		// Token: 0x0600172E RID: 5934 RVA: 0x0004D204 File Offset: 0x0004B404
		public void ExecuteResetAll()
		{
			string text = GameTexts.FindText("str_reset_all", null).ToString();
			string text2 = new TextObject("{=1hnq3Kb1}Are you sure want to reset all properties? Your changes will be lost.", null).ToString();
			InformationManager.ShowInquiry(new InquiryData(text, text2, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.ResetAll), null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x0600172F RID: 5935 RVA: 0x0004D27C File Offset: 0x0004B47C
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

		// Token: 0x06001730 RID: 5936 RVA: 0x0004D450 File Offset: 0x0004B650
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

		// Token: 0x06001731 RID: 5937 RVA: 0x0004D664 File Offset: 0x0004B864
		public void ExecuteCancel()
		{
			this._faceGeneratorScreen.Cancel();
		}

		// Token: 0x06001732 RID: 5938 RVA: 0x0004D671 File Offset: 0x0004B871
		public void ExecuteDone()
		{
			this._faceGeneratorScreen.Done();
		}

		// Token: 0x06001733 RID: 5939 RVA: 0x0004D680 File Offset: 0x0004B880
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

		// Token: 0x06001734 RID: 5940 RVA: 0x0004D738 File Offset: 0x0004B938
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

		// Token: 0x06001735 RID: 5941 RVA: 0x0004D7F0 File Offset: 0x0004B9F0
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

		// Token: 0x06001736 RID: 5942 RVA: 0x0004D820 File Offset: 0x0004BA20
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

		// Token: 0x06001737 RID: 5943 RVA: 0x0004D897 File Offset: 0x0004BA97
		private void UpdateTitle()
		{
		}

		// Token: 0x06001738 RID: 5944 RVA: 0x0004D899 File Offset: 0x0004BA99
		private void ExecuteGoToIndex(int index)
		{
			this._goToIndex(index);
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x0004D8A8 File Offset: 0x0004BAA8
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

		// Token: 0x0600173A RID: 5946 RVA: 0x0004DA28 File Offset: 0x0004BC28
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

		// Token: 0x0600173B RID: 5947 RVA: 0x0004DAB4 File Offset: 0x0004BCB4
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

		// Token: 0x0600173C RID: 5948 RVA: 0x0004DDDC File Offset: 0x0004BFDC
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

		// Token: 0x0600173D RID: 5949 RVA: 0x0004DE3C File Offset: 0x0004C03C
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

		// Token: 0x0600173E RID: 5950 RVA: 0x0004DE98 File Offset: 0x0004C098
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

		// Token: 0x0600173F RID: 5951 RVA: 0x0004DEF8 File Offset: 0x0004C0F8
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

		// Token: 0x06001740 RID: 5952 RVA: 0x0004DF58 File Offset: 0x0004C158
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

		// Token: 0x06001741 RID: 5953 RVA: 0x0004DFB4 File Offset: 0x0004C1B4
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

		// Token: 0x06001742 RID: 5954 RVA: 0x0004E032 File Offset: 0x0004C232
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001743 RID: 5955 RVA: 0x0004E041 File Offset: 0x0004C241
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x0004E050 File Offset: 0x0004C250
		public void SetPreviousTabInputKey(HotKey hotKey)
		{
			this.PreviousTabInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001745 RID: 5957 RVA: 0x0004E05F File Offset: 0x0004C25F
		public void SetNextTabInputKey(HotKey hotKey)
		{
			this.NextTabInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001746 RID: 5958 RVA: 0x0004E070 File Offset: 0x0004C270
		public void AddCameraControlInputKey(HotKey hotKey)
		{
			InputKeyItemVM inputKeyItemVM = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.CameraControlKeys.Add(inputKeyItemVM);
		}

		// Token: 0x06001747 RID: 5959 RVA: 0x0004E094 File Offset: 0x0004C294
		public void AddCameraControlInputKey(GameKey gameKey)
		{
			InputKeyItemVM inputKeyItemVM = InputKeyItemVM.CreateFromGameKey(gameKey, true);
			this.CameraControlKeys.Add(inputKeyItemVM);
		}

		// Token: 0x06001748 RID: 5960 RVA: 0x0004E0B8 File Offset: 0x0004C2B8
		public void AddCameraControlInputKey(GameAxisKey gameAxisKey)
		{
			TextObject textObject = Module.CurrentModule.GlobalTextManager.FindText("str_key_name", typeof(FaceGenHotkeyCategory).Name + "_" + gameAxisKey.Id);
			InputKeyItemVM inputKeyItemVM = InputKeyItemVM.CreateFromForcedID(gameAxisKey.AxisKey.ToString(), textObject, true);
			this.CameraControlKeys.Add(inputKeyItemVM);
		}

		// Token: 0x17000785 RID: 1925
		// (get) Token: 0x06001749 RID: 5961 RVA: 0x0004E118 File Offset: 0x0004C318
		// (set) Token: 0x0600174A RID: 5962 RVA: 0x0004E120 File Offset: 0x0004C320
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

		// Token: 0x17000786 RID: 1926
		// (get) Token: 0x0600174B RID: 5963 RVA: 0x0004E13E File Offset: 0x0004C33E
		// (set) Token: 0x0600174C RID: 5964 RVA: 0x0004E146 File Offset: 0x0004C346
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

		// Token: 0x17000787 RID: 1927
		// (get) Token: 0x0600174D RID: 5965 RVA: 0x0004E164 File Offset: 0x0004C364
		// (set) Token: 0x0600174E RID: 5966 RVA: 0x0004E16C File Offset: 0x0004C36C
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

		// Token: 0x17000788 RID: 1928
		// (get) Token: 0x0600174F RID: 5967 RVA: 0x0004E18A File Offset: 0x0004C38A
		// (set) Token: 0x06001750 RID: 5968 RVA: 0x0004E192 File Offset: 0x0004C392
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

		// Token: 0x17000789 RID: 1929
		// (get) Token: 0x06001751 RID: 5969 RVA: 0x0004E1B0 File Offset: 0x0004C3B0
		// (set) Token: 0x06001752 RID: 5970 RVA: 0x0004E1B8 File Offset: 0x0004C3B8
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

		// Token: 0x1700078A RID: 1930
		// (get) Token: 0x06001753 RID: 5971 RVA: 0x0004E1D6 File Offset: 0x0004C3D6
		[DataSourceProperty]
		public bool AreAllTabsEnabled
		{
			get
			{
				return this.IsBodyEnabled && this.IsFaceEnabled && this.IsEyesEnabled && this.IsNoseEnabled && this.IsMouthEnabled && this.IsHairEnabled && this.IsTaintEnabled;
			}
		}

		// Token: 0x1700078B RID: 1931
		// (get) Token: 0x06001754 RID: 5972 RVA: 0x0004E210 File Offset: 0x0004C410
		// (set) Token: 0x06001755 RID: 5973 RVA: 0x0004E218 File Offset: 0x0004C418
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

		// Token: 0x1700078C RID: 1932
		// (get) Token: 0x06001756 RID: 5974 RVA: 0x0004E236 File Offset: 0x0004C436
		// (set) Token: 0x06001757 RID: 5975 RVA: 0x0004E23E File Offset: 0x0004C43E
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

		// Token: 0x1700078D RID: 1933
		// (get) Token: 0x06001758 RID: 5976 RVA: 0x0004E25C File Offset: 0x0004C45C
		// (set) Token: 0x06001759 RID: 5977 RVA: 0x0004E264 File Offset: 0x0004C464
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

		// Token: 0x1700078E RID: 1934
		// (get) Token: 0x0600175A RID: 5978 RVA: 0x0004E282 File Offset: 0x0004C482
		// (set) Token: 0x0600175B RID: 5979 RVA: 0x0004E28A File Offset: 0x0004C48A
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

		// Token: 0x1700078F RID: 1935
		// (get) Token: 0x0600175C RID: 5980 RVA: 0x0004E2A8 File Offset: 0x0004C4A8
		// (set) Token: 0x0600175D RID: 5981 RVA: 0x0004E2B0 File Offset: 0x0004C4B0
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

		// Token: 0x17000790 RID: 1936
		// (get) Token: 0x0600175E RID: 5982 RVA: 0x0004E2CE File Offset: 0x0004C4CE
		// (set) Token: 0x0600175F RID: 5983 RVA: 0x0004E2D6 File Offset: 0x0004C4D6
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

		// Token: 0x17000791 RID: 1937
		// (get) Token: 0x06001760 RID: 5984 RVA: 0x0004E2F4 File Offset: 0x0004C4F4
		// (set) Token: 0x06001761 RID: 5985 RVA: 0x0004E2FC File Offset: 0x0004C4FC
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

		// Token: 0x17000792 RID: 1938
		// (get) Token: 0x06001762 RID: 5986 RVA: 0x0004E31A File Offset: 0x0004C51A
		// (set) Token: 0x06001763 RID: 5987 RVA: 0x0004E322 File Offset: 0x0004C522
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

		// Token: 0x17000793 RID: 1939
		// (get) Token: 0x06001764 RID: 5988 RVA: 0x0004E345 File Offset: 0x0004C545
		// (set) Token: 0x06001765 RID: 5989 RVA: 0x0004E34D File Offset: 0x0004C54D
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

		// Token: 0x17000794 RID: 1940
		// (get) Token: 0x06001766 RID: 5990 RVA: 0x0004E370 File Offset: 0x0004C570
		// (set) Token: 0x06001767 RID: 5991 RVA: 0x0004E378 File Offset: 0x0004C578
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

		// Token: 0x17000795 RID: 1941
		// (get) Token: 0x06001768 RID: 5992 RVA: 0x0004E39B File Offset: 0x0004C59B
		// (set) Token: 0x06001769 RID: 5993 RVA: 0x0004E3A3 File Offset: 0x0004C5A3
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

		// Token: 0x17000796 RID: 1942
		// (get) Token: 0x0600176A RID: 5994 RVA: 0x0004E3C6 File Offset: 0x0004C5C6
		// (set) Token: 0x0600176B RID: 5995 RVA: 0x0004E3CE File Offset: 0x0004C5CE
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

		// Token: 0x17000797 RID: 1943
		// (get) Token: 0x0600176C RID: 5996 RVA: 0x0004E3F1 File Offset: 0x0004C5F1
		// (set) Token: 0x0600176D RID: 5997 RVA: 0x0004E3F9 File Offset: 0x0004C5F9
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

		// Token: 0x17000798 RID: 1944
		// (get) Token: 0x0600176E RID: 5998 RVA: 0x0004E41C File Offset: 0x0004C61C
		// (set) Token: 0x0600176F RID: 5999 RVA: 0x0004E424 File Offset: 0x0004C624
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

		// Token: 0x17000799 RID: 1945
		// (get) Token: 0x06001770 RID: 6000 RVA: 0x0004E442 File Offset: 0x0004C642
		// (set) Token: 0x06001771 RID: 6001 RVA: 0x0004E44A File Offset: 0x0004C64A
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

		// Token: 0x1700079A RID: 1946
		// (get) Token: 0x06001772 RID: 6002 RVA: 0x0004E468 File Offset: 0x0004C668
		// (set) Token: 0x06001773 RID: 6003 RVA: 0x0004E470 File Offset: 0x0004C670
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

		// Token: 0x1700079B RID: 1947
		// (get) Token: 0x06001774 RID: 6004 RVA: 0x0004E48E File Offset: 0x0004C68E
		// (set) Token: 0x06001775 RID: 6005 RVA: 0x0004E496 File Offset: 0x0004C696
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

		// Token: 0x1700079C RID: 1948
		// (get) Token: 0x06001776 RID: 6006 RVA: 0x0004E4B4 File Offset: 0x0004C6B4
		// (set) Token: 0x06001777 RID: 6007 RVA: 0x0004E4BC File Offset: 0x0004C6BC
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

		// Token: 0x1700079D RID: 1949
		// (get) Token: 0x06001778 RID: 6008 RVA: 0x0004E4DA File Offset: 0x0004C6DA
		// (set) Token: 0x06001779 RID: 6009 RVA: 0x0004E4E2 File Offset: 0x0004C6E2
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

		// Token: 0x1700079E RID: 1950
		// (get) Token: 0x0600177A RID: 6010 RVA: 0x0004E500 File Offset: 0x0004C700
		// (set) Token: 0x0600177B RID: 6011 RVA: 0x0004E508 File Offset: 0x0004C708
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

		// Token: 0x1700079F RID: 1951
		// (get) Token: 0x0600177C RID: 6012 RVA: 0x0004E526 File Offset: 0x0004C726
		// (set) Token: 0x0600177D RID: 6013 RVA: 0x0004E52E File Offset: 0x0004C72E
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

		// Token: 0x170007A0 RID: 1952
		// (get) Token: 0x0600177E RID: 6014 RVA: 0x0004E54C File Offset: 0x0004C74C
		// (set) Token: 0x0600177F RID: 6015 RVA: 0x0004E554 File Offset: 0x0004C754
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

		// Token: 0x170007A1 RID: 1953
		// (get) Token: 0x06001780 RID: 6016 RVA: 0x0004E572 File Offset: 0x0004C772
		// (set) Token: 0x06001781 RID: 6017 RVA: 0x0004E57A File Offset: 0x0004C77A
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

		// Token: 0x170007A2 RID: 1954
		// (get) Token: 0x06001782 RID: 6018 RVA: 0x0004E598 File Offset: 0x0004C798
		// (set) Token: 0x06001783 RID: 6019 RVA: 0x0004E5A0 File Offset: 0x0004C7A0
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

		// Token: 0x170007A3 RID: 1955
		// (get) Token: 0x06001784 RID: 6020 RVA: 0x0004E5BE File Offset: 0x0004C7BE
		// (set) Token: 0x06001785 RID: 6021 RVA: 0x0004E5C6 File Offset: 0x0004C7C6
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

		// Token: 0x170007A4 RID: 1956
		// (get) Token: 0x06001786 RID: 6022 RVA: 0x0004E5E4 File Offset: 0x0004C7E4
		// (set) Token: 0x06001787 RID: 6023 RVA: 0x0004E5EC File Offset: 0x0004C7EC
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

		// Token: 0x170007A5 RID: 1957
		// (get) Token: 0x06001788 RID: 6024 RVA: 0x0004E60A File Offset: 0x0004C80A
		// (set) Token: 0x06001789 RID: 6025 RVA: 0x0004E612 File Offset: 0x0004C812
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

		// Token: 0x170007A6 RID: 1958
		// (get) Token: 0x0600178A RID: 6026 RVA: 0x0004E630 File Offset: 0x0004C830
		// (set) Token: 0x0600178B RID: 6027 RVA: 0x0004E638 File Offset: 0x0004C838
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

		// Token: 0x170007A7 RID: 1959
		// (get) Token: 0x0600178C RID: 6028 RVA: 0x0004E656 File Offset: 0x0004C856
		// (set) Token: 0x0600178D RID: 6029 RVA: 0x0004E65E File Offset: 0x0004C85E
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

		// Token: 0x170007A8 RID: 1960
		// (get) Token: 0x0600178E RID: 6030 RVA: 0x0004E67C File Offset: 0x0004C87C
		// (set) Token: 0x0600178F RID: 6031 RVA: 0x0004E684 File Offset: 0x0004C884
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

		// Token: 0x170007A9 RID: 1961
		// (get) Token: 0x06001790 RID: 6032 RVA: 0x0004E6A2 File Offset: 0x0004C8A2
		// (set) Token: 0x06001791 RID: 6033 RVA: 0x0004E6AA File Offset: 0x0004C8AA
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

		// Token: 0x170007AA RID: 1962
		// (get) Token: 0x06001792 RID: 6034 RVA: 0x0004E6C8 File Offset: 0x0004C8C8
		// (set) Token: 0x06001793 RID: 6035 RVA: 0x0004E6D0 File Offset: 0x0004C8D0
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

		// Token: 0x170007AB RID: 1963
		// (get) Token: 0x06001794 RID: 6036 RVA: 0x0004E6EE File Offset: 0x0004C8EE
		// (set) Token: 0x06001795 RID: 6037 RVA: 0x0004E6F8 File Offset: 0x0004C8F8
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

		// Token: 0x170007AC RID: 1964
		// (get) Token: 0x06001796 RID: 6038 RVA: 0x0004E78C File Offset: 0x0004C98C
		// (set) Token: 0x06001797 RID: 6039 RVA: 0x0004E794 File Offset: 0x0004C994
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

		// Token: 0x170007AD RID: 1965
		// (get) Token: 0x06001798 RID: 6040 RVA: 0x0004E7EF File Offset: 0x0004C9EF
		[DataSourceProperty]
		public bool IsFemale
		{
			get
			{
				return this.SelectedGender != 0;
			}
		}

		// Token: 0x170007AE RID: 1966
		// (get) Token: 0x06001799 RID: 6041 RVA: 0x0004E7FA File Offset: 0x0004C9FA
		// (set) Token: 0x0600179A RID: 6042 RVA: 0x0004E802 File Offset: 0x0004CA02
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

		// Token: 0x170007AF RID: 1967
		// (get) Token: 0x0600179B RID: 6043 RVA: 0x0004E820 File Offset: 0x0004CA20
		// (set) Token: 0x0600179C RID: 6044 RVA: 0x0004E828 File Offset: 0x0004CA28
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

		// Token: 0x170007B0 RID: 1968
		// (get) Token: 0x0600179D RID: 6045 RVA: 0x0004E846 File Offset: 0x0004CA46
		// (set) Token: 0x0600179E RID: 6046 RVA: 0x0004E84E File Offset: 0x0004CA4E
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

		// Token: 0x170007B1 RID: 1969
		// (get) Token: 0x0600179F RID: 6047 RVA: 0x0004E86C File Offset: 0x0004CA6C
		// (set) Token: 0x060017A0 RID: 6048 RVA: 0x0004E874 File Offset: 0x0004CA74
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

		// Token: 0x170007B2 RID: 1970
		// (get) Token: 0x060017A1 RID: 6049 RVA: 0x0004E892 File Offset: 0x0004CA92
		// (set) Token: 0x060017A2 RID: 6050 RVA: 0x0004E89A File Offset: 0x0004CA9A
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

		// Token: 0x170007B3 RID: 1971
		// (get) Token: 0x060017A3 RID: 6051 RVA: 0x0004E8B8 File Offset: 0x0004CAB8
		// (set) Token: 0x060017A4 RID: 6052 RVA: 0x0004E8C0 File Offset: 0x0004CAC0
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

		// Token: 0x170007B4 RID: 1972
		// (get) Token: 0x060017A5 RID: 6053 RVA: 0x0004E8DE File Offset: 0x0004CADE
		// (set) Token: 0x060017A6 RID: 6054 RVA: 0x0004E8E6 File Offset: 0x0004CAE6
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

		// Token: 0x170007B5 RID: 1973
		// (get) Token: 0x060017A7 RID: 6055 RVA: 0x0004E904 File Offset: 0x0004CB04
		// (set) Token: 0x060017A8 RID: 6056 RVA: 0x0004E90C File Offset: 0x0004CB0C
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

		// Token: 0x170007B6 RID: 1974
		// (get) Token: 0x060017A9 RID: 6057 RVA: 0x0004E92A File Offset: 0x0004CB2A
		// (set) Token: 0x060017AA RID: 6058 RVA: 0x0004E932 File Offset: 0x0004CB32
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

		// Token: 0x170007B7 RID: 1975
		// (get) Token: 0x060017AB RID: 6059 RVA: 0x0004E950 File Offset: 0x0004CB50
		// (set) Token: 0x060017AC RID: 6060 RVA: 0x0004E958 File Offset: 0x0004CB58
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

		// Token: 0x170007B8 RID: 1976
		// (get) Token: 0x060017AD RID: 6061 RVA: 0x0004E976 File Offset: 0x0004CB76
		// (set) Token: 0x060017AE RID: 6062 RVA: 0x0004E97E File Offset: 0x0004CB7E
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

		// Token: 0x170007B9 RID: 1977
		// (get) Token: 0x060017AF RID: 6063 RVA: 0x0004E99C File Offset: 0x0004CB9C
		// (set) Token: 0x060017B0 RID: 6064 RVA: 0x0004E9A4 File Offset: 0x0004CBA4
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

		// Token: 0x170007BA RID: 1978
		// (get) Token: 0x060017B1 RID: 6065 RVA: 0x0004E9C2 File Offset: 0x0004CBC2
		// (set) Token: 0x060017B2 RID: 6066 RVA: 0x0004E9CA File Offset: 0x0004CBCA
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

		// Token: 0x170007BB RID: 1979
		// (get) Token: 0x060017B3 RID: 6067 RVA: 0x0004E9E8 File Offset: 0x0004CBE8
		// (set) Token: 0x060017B4 RID: 6068 RVA: 0x0004E9F0 File Offset: 0x0004CBF0
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

		// Token: 0x170007BC RID: 1980
		// (get) Token: 0x060017B5 RID: 6069 RVA: 0x0004EA0E File Offset: 0x0004CC0E
		// (set) Token: 0x060017B6 RID: 6070 RVA: 0x0004EA16 File Offset: 0x0004CC16
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

		// Token: 0x170007BD RID: 1981
		// (get) Token: 0x060017B7 RID: 6071 RVA: 0x0004EA34 File Offset: 0x0004CC34
		// (set) Token: 0x060017B8 RID: 6072 RVA: 0x0004EA41 File Offset: 0x0004CC41
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

		// Token: 0x170007BE RID: 1982
		// (get) Token: 0x060017B9 RID: 6073 RVA: 0x0004EA6F File Offset: 0x0004CC6F
		// (set) Token: 0x060017BA RID: 6074 RVA: 0x0004EA77 File Offset: 0x0004CC77
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

		// Token: 0x170007BF RID: 1983
		// (get) Token: 0x060017BB RID: 6075 RVA: 0x0004EA95 File Offset: 0x0004CC95
		// (set) Token: 0x060017BC RID: 6076 RVA: 0x0004EA9D File Offset: 0x0004CC9D
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

		// Token: 0x170007C0 RID: 1984
		// (get) Token: 0x060017BD RID: 6077 RVA: 0x0004EABB File Offset: 0x0004CCBB
		// (set) Token: 0x060017BE RID: 6078 RVA: 0x0004EAC3 File Offset: 0x0004CCC3
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

		// Token: 0x170007C1 RID: 1985
		// (get) Token: 0x060017BF RID: 6079 RVA: 0x0004EAE1 File Offset: 0x0004CCE1
		// (set) Token: 0x060017C0 RID: 6080 RVA: 0x0004EAE9 File Offset: 0x0004CCE9
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

		// Token: 0x170007C2 RID: 1986
		// (get) Token: 0x060017C1 RID: 6081 RVA: 0x0004EB0C File Offset: 0x0004CD0C
		// (set) Token: 0x060017C2 RID: 6082 RVA: 0x0004EB14 File Offset: 0x0004CD14
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

		// Token: 0x170007C3 RID: 1987
		// (get) Token: 0x060017C3 RID: 6083 RVA: 0x0004EB32 File Offset: 0x0004CD32
		// (set) Token: 0x060017C4 RID: 6084 RVA: 0x0004EB3A File Offset: 0x0004CD3A
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

		// Token: 0x170007C4 RID: 1988
		// (get) Token: 0x060017C5 RID: 6085 RVA: 0x0004EB58 File Offset: 0x0004CD58
		// (set) Token: 0x060017C6 RID: 6086 RVA: 0x0004EB60 File Offset: 0x0004CD60
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

		// Token: 0x04000AF2 RID: 2802
		private const float MultiplayerHeightSliderMinValue = 0.25f;

		// Token: 0x04000AF3 RID: 2803
		private const float MultiplayerHeightSliderMaxValue = 0.75f;

		// Token: 0x04000AF4 RID: 2804
		private readonly IFaceGeneratorHandler _faceGeneratorScreen;

		// Token: 0x04000AF5 RID: 2805
		private bool _characterRefreshEnabled = true;

		// Token: 0x04000AF6 RID: 2806
		private bool _initialValuesSet;

		// Token: 0x04000AF7 RID: 2807
		private readonly BodyGenerator _bodyGenerator;

		// Token: 0x04000AF8 RID: 2808
		private readonly TextObject _affirmitiveText;

		// Token: 0x04000AF9 RID: 2809
		private readonly TextObject _negativeText;

		// Token: 0x04000AFA RID: 2810
		private FaceGenerationParams _faceGenerationParams = FaceGenerationParams.Create();

		// Token: 0x04000AFB RID: 2811
		private List<FaceGenVM.UndoRedoKey> _undoCommands;

		// Token: 0x04000AFC RID: 2812
		private List<FaceGenVM.UndoRedoKey> _redoCommands;

		// Token: 0x04000AFD RID: 2813
		private List<bool> _isVoiceTypeUsableForOnlyNpc;

		// Token: 0x04000AFE RID: 2814
		private MBReadOnlyList<bool> _tabAvailabilities;

		// Token: 0x04000AFF RID: 2815
		private Action<float> _onHeightChanged;

		// Token: 0x04000B00 RID: 2816
		private Action _onAgeChanged;

		// Token: 0x04000B01 RID: 2817
		private int _initialRace = -1;

		// Token: 0x04000B02 RID: 2818
		private int _initialGender = -1;

		// Token: 0x04000B03 RID: 2819
		private BodyMeshMaturityType _latestMaturityType;

		// Token: 0x04000B04 RID: 2820
		private bool _isRandomizing;

		// Token: 0x04000B05 RID: 2821
		private readonly Action<int> _goToIndex;

		// Token: 0x04000B06 RID: 2822
		private FaceGenVM.GenderBasedSelectedValue[] genderBasedSelectedValues;

		// Token: 0x04000B07 RID: 2823
		private readonly Dictionary<FaceGenVM.FaceGenTabs, MBBindingList<FaceGenPropertyVM>> _tabProperties;

		// Token: 0x04000B08 RID: 2824
		private List<uint> _skinColors;

		// Token: 0x04000B09 RID: 2825
		private List<uint> _hairColors;

		// Token: 0x04000B0A RID: 2826
		private List<uint> _tattooColors;

		// Token: 0x04000B0B RID: 2827
		private readonly bool _showDebugValues;

		// Token: 0x04000B0C RID: 2828
		private readonly bool _openedFromMultiplayer;

		// Token: 0x04000B0D RID: 2829
		private bool _enforceConstraints;

		// Token: 0x04000B0E RID: 2830
		private IFaceGeneratorCustomFilter _filter;

		// Token: 0x04000B0F RID: 2831
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000B10 RID: 2832
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000B11 RID: 2833
		private InputKeyItemVM _previousTabInputKey;

		// Token: 0x04000B12 RID: 2834
		private InputKeyItemVM _nextTabInputKey;

		// Token: 0x04000B13 RID: 2835
		private MBBindingList<InputKeyItemVM> _cameraControlKeys;

		// Token: 0x04000B14 RID: 2836
		private bool _isBodyEnabled;

		// Token: 0x04000B15 RID: 2837
		private bool _isFaceEnabled;

		// Token: 0x04000B16 RID: 2838
		private bool _isEyesEnabled;

		// Token: 0x04000B17 RID: 2839
		private bool _isNoseEnabled;

		// Token: 0x04000B18 RID: 2840
		private bool _isMouthEnabled;

		// Token: 0x04000B19 RID: 2841
		private bool _isHairEnabled;

		// Token: 0x04000B1A RID: 2842
		private bool _isTaintEnabled;

		// Token: 0x04000B1B RID: 2843
		private string _cancelBtnLbl;

		// Token: 0x04000B1C RID: 2844
		private string _doneBtnLbl;

		// Token: 0x04000B1D RID: 2845
		private int _initialSelectedTaintType;

		// Token: 0x04000B1E RID: 2846
		private int _initialSelectedHairType;

		// Token: 0x04000B1F RID: 2847
		private int _initialSelectedBeardType;

		// Token: 0x04000B20 RID: 2848
		private float _initialSelectedSkinColor;

		// Token: 0x04000B21 RID: 2849
		private float _initialSelectedHairColor;

		// Token: 0x04000B22 RID: 2850
		private float _initialSelectedTaintColor;

		// Token: 0x04000B23 RID: 2851
		private string _flipHairLbl;

		// Token: 0x04000B24 RID: 2852
		private string _skinColorLbl;

		// Token: 0x04000B25 RID: 2853
		private string _raceLbl;

		// Token: 0x04000B26 RID: 2854
		private string _genderLbl;

		// Token: 0x04000B27 RID: 2855
		private FaceGenPropertyVM _heightSlider;

		// Token: 0x04000B28 RID: 2856
		private HintViewModel _bodyHint;

		// Token: 0x04000B29 RID: 2857
		private HintViewModel _faceHint;

		// Token: 0x04000B2A RID: 2858
		private HintViewModel _eyesHint;

		// Token: 0x04000B2B RID: 2859
		private HintViewModel _noseHint;

		// Token: 0x04000B2C RID: 2860
		private HintViewModel _hairHint;

		// Token: 0x04000B2D RID: 2861
		private HintViewModel _taintHint;

		// Token: 0x04000B2E RID: 2862
		private HintViewModel _mouthHint;

		// Token: 0x04000B2F RID: 2863
		private HintViewModel _redoHint;

		// Token: 0x04000B30 RID: 2864
		private HintViewModel _undoHint;

		// Token: 0x04000B31 RID: 2865
		private HintViewModel _randomizeHint;

		// Token: 0x04000B32 RID: 2866
		private HintViewModel _randomizeAllHint;

		// Token: 0x04000B33 RID: 2867
		private HintViewModel _resetHint;

		// Token: 0x04000B34 RID: 2868
		private HintViewModel _resetAllHint;

		// Token: 0x04000B35 RID: 2869
		private HintViewModel _clothHint;

		// Token: 0x04000B36 RID: 2870
		private int hairNum;

		// Token: 0x04000B37 RID: 2871
		private int beardNum;

		// Token: 0x04000B38 RID: 2872
		private int faceTextureNum;

		// Token: 0x04000B39 RID: 2873
		private int mouthTextureNum;

		// Token: 0x04000B3A RID: 2874
		private int eyebrowTextureNum;

		// Token: 0x04000B3B RID: 2875
		private int faceTattooNum;

		// Token: 0x04000B3C RID: 2876
		private int _newSoundPresetSize;

		// Token: 0x04000B3D RID: 2877
		private float _scale = 1f;

		// Token: 0x04000B3E RID: 2878
		private int _tab = -1;

		// Token: 0x04000B3F RID: 2879
		private int _selectedRace = -1;

		// Token: 0x04000B40 RID: 2880
		private int _selectedGender = -1;

		// Token: 0x04000B41 RID: 2881
		private bool _canChangeGender;

		// Token: 0x04000B42 RID: 2882
		private bool _canChangeRace;

		// Token: 0x04000B43 RID: 2883
		private bool _isDressed;

		// Token: 0x04000B44 RID: 2884
		private bool _characterGamepadControlsEnabled;

		// Token: 0x04000B45 RID: 2885
		private MBBindingList<FaceGenPropertyVM> _bodyProperties;

		// Token: 0x04000B46 RID: 2886
		private MBBindingList<FaceGenPropertyVM> _faceProperties;

		// Token: 0x04000B47 RID: 2887
		private MBBindingList<FaceGenPropertyVM> _eyesProperties;

		// Token: 0x04000B48 RID: 2888
		private MBBindingList<FaceGenPropertyVM> _noseProperties;

		// Token: 0x04000B49 RID: 2889
		private MBBindingList<FaceGenPropertyVM> _mouthProperties;

		// Token: 0x04000B4A RID: 2890
		private MBBindingList<FaceGenPropertyVM> _hairProperties;

		// Token: 0x04000B4B RID: 2891
		private MBBindingList<FaceGenPropertyVM> _taintProperties;

		// Token: 0x04000B4C RID: 2892
		private MBBindingList<FacegenListItemVM> _taintTypes;

		// Token: 0x04000B4D RID: 2893
		private MBBindingList<FacegenListItemVM> _beardTypes;

		// Token: 0x04000B4E RID: 2894
		private MBBindingList<FacegenListItemVM> _hairTypes;

		// Token: 0x04000B4F RID: 2895
		private FaceGenPropertyVM _soundPreset;

		// Token: 0x04000B50 RID: 2896
		private FaceGenPropertyVM _faceTypes;

		// Token: 0x04000B51 RID: 2897
		private FaceGenPropertyVM _teethTypes;

		// Token: 0x04000B52 RID: 2898
		private FaceGenPropertyVM _eyebrowTypes;

		// Token: 0x04000B53 RID: 2899
		private SelectorVM<SelectorItemVM> _skinColorSelector;

		// Token: 0x04000B54 RID: 2900
		private SelectorVM<SelectorItemVM> _hairColorSelector;

		// Token: 0x04000B55 RID: 2901
		private SelectorVM<SelectorItemVM> _tattooColorSelector;

		// Token: 0x04000B56 RID: 2902
		private SelectorVM<SelectorItemVM> _raceSelector;

		// Token: 0x04000B57 RID: 2903
		private FacegenListItemVM _selectedTaintType;

		// Token: 0x04000B58 RID: 2904
		private FacegenListItemVM _selectedBeardType;

		// Token: 0x04000B59 RID: 2905
		private FacegenListItemVM _selectedHairType;

		// Token: 0x04000B5A RID: 2906
		private string _title = "";

		// Token: 0x04000B5B RID: 2907
		private int _totalStageCount = -1;

		// Token: 0x04000B5C RID: 2908
		private int _currentStageIndex = -1;

		// Token: 0x04000B5D RID: 2909
		private int _furthestIndex = -1;

		// Token: 0x02000254 RID: 596
		public enum FaceGenTabs
		{
			// Token: 0x04000F4F RID: 3919
			None = -1,
			// Token: 0x04000F50 RID: 3920
			Body,
			// Token: 0x04000F51 RID: 3921
			Face,
			// Token: 0x04000F52 RID: 3922
			Eyes,
			// Token: 0x04000F53 RID: 3923
			Nose,
			// Token: 0x04000F54 RID: 3924
			Mouth,
			// Token: 0x04000F55 RID: 3925
			Hair,
			// Token: 0x04000F56 RID: 3926
			Taint,
			// Token: 0x04000F57 RID: 3927
			NumOfFaceGenTabs
		}

		// Token: 0x02000255 RID: 597
		public enum Presets
		{
			// Token: 0x04000F59 RID: 3929
			Gender = -1,
			// Token: 0x04000F5A RID: 3930
			FacePresets = -2,
			// Token: 0x04000F5B RID: 3931
			FaceType = -3,
			// Token: 0x04000F5C RID: 3932
			EyePresets = -4,
			// Token: 0x04000F5D RID: 3933
			HairBeardPreset = -5,
			// Token: 0x04000F5E RID: 3934
			HairType = -6,
			// Token: 0x04000F5F RID: 3935
			BeardType = -7,
			// Token: 0x04000F60 RID: 3936
			TaintPresets = -8,
			// Token: 0x04000F61 RID: 3937
			SoundPresets = -9,
			// Token: 0x04000F62 RID: 3938
			TaintType = -10,
			// Token: 0x04000F63 RID: 3939
			Age = -11,
			// Token: 0x04000F64 RID: 3940
			EyeColor = -12,
			// Token: 0x04000F65 RID: 3941
			HairAndBeardColor = -13,
			// Token: 0x04000F66 RID: 3942
			TeethType = -14,
			// Token: 0x04000F67 RID: 3943
			EyebrowType = -15,
			// Token: 0x04000F68 RID: 3944
			Scale = -16,
			// Token: 0x04000F69 RID: 3945
			Weight = -17,
			// Token: 0x04000F6A RID: 3946
			Build = -18,
			// Token: 0x04000F6B RID: 3947
			Pitch = -19,
			// Token: 0x04000F6C RID: 3948
			Race = -20
		}

		// Token: 0x02000256 RID: 598
		public struct GenderBasedSelectedValue
		{
			// Token: 0x06001BBB RID: 7099 RVA: 0x0005836F File Offset: 0x0005656F
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

			// Token: 0x04000F6D RID: 3949
			public int Hair;

			// Token: 0x04000F6E RID: 3950
			public int Beard;

			// Token: 0x04000F6F RID: 3951
			public int FaceTexture;

			// Token: 0x04000F70 RID: 3952
			public int MouthTexture;

			// Token: 0x04000F71 RID: 3953
			public int Tattoo;

			// Token: 0x04000F72 RID: 3954
			public int SoundPreset;

			// Token: 0x04000F73 RID: 3955
			public int EyebrowTexture;
		}

		// Token: 0x02000257 RID: 599
		private struct UndoRedoKey
		{
			// Token: 0x06001BBC RID: 7100 RVA: 0x000583A2 File Offset: 0x000565A2
			public UndoRedoKey(int gender, int race, BodyProperties bodyProperties)
			{
				this.Gender = gender;
				this.Race = race;
				this.BodyProperties = bodyProperties;
			}

			// Token: 0x04000F74 RID: 3956
			public readonly int Gender;

			// Token: 0x04000F75 RID: 3957
			public readonly int Race;

			// Token: 0x04000F76 RID: 3958
			public readonly BodyProperties BodyProperties;
		}
	}
}
