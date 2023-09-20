using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Education
{
	// Token: 0x020000D4 RID: 212
	public class EducationVM : ViewModel
	{
		// Token: 0x06001391 RID: 5009 RVA: 0x0004AE78 File Offset: 0x00049078
		public EducationVM(Hero child, Action<bool> onDone, Action<EducationCampaignBehavior.EducationCharacterProperties[]> onOptionSelect, Action<List<BasicCharacterObject>, List<Equipment>> sendPossibleCharactersAndEquipment)
		{
			this._onDone = onDone;
			this._onOptionSelect = onOptionSelect;
			this._sendPossibleCharactersAndEquipment = sendPossibleCharactersAndEquipment;
			this._child = child;
			this._educationBehavior = Campaign.Current.GetCampaignBehavior<IEducationLogic>();
			int num;
			this._educationBehavior.GetStageProperties(this._child, out num);
			this._pageCount = num + 1;
			this.GainedPropertiesController = new EducationGainedPropertiesVM(this._child, this._pageCount);
			this.Options = new MBBindingList<EducationOptionVM>();
			this.Review = new EducationReviewVM(this._pageCount);
			this.CanGoBack = true;
			this.InitWithStageIndex(0);
		}

		// Token: 0x06001392 RID: 5010 RVA: 0x0004AF44 File Offset: 0x00049144
		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject currentPageTitleTextObj = this._currentPageTitleTextObj;
			this.StageTitleText = ((currentPageTitleTextObj != null) ? currentPageTitleTextObj.ToString() : null) ?? "";
			TextObject currentPageDescriptionTextObj = this._currentPageDescriptionTextObj;
			this.PageDescriptionText = ((currentPageDescriptionTextObj != null) ? currentPageDescriptionTextObj.ToString() : null) ?? "";
			TextObject currentPageInstructionTextObj = this._currentPageInstructionTextObj;
			this.ChooseText = ((currentPageInstructionTextObj != null) ? currentPageInstructionTextObj.ToString() : null) ?? "";
			this.Options.ApplyActionOnAllItems(delegate(EducationOptionVM o)
			{
				o.RefreshValues();
			});
			foreach (EducationOptionVM educationOptionVM in this.Options)
			{
				if (educationOptionVM.IsSelected)
				{
					this.OptionEffectText = educationOptionVM.OptionEffect;
					this.OptionDescriptionText = educationOptionVM.OptionDescription;
				}
			}
		}

		// Token: 0x06001393 RID: 5011 RVA: 0x0004B040 File Offset: 0x00049240
		private void InitWithStageIndex(int index)
		{
			this._latestOptionId = null;
			this.CanAdvance = false;
			this._currentPageIndex = index;
			this.OptionEffectText = "";
			this.OptionDescriptionText = "";
			this.Options.Clear();
			if (index < this._pageCount - 1)
			{
				List<BasicCharacterObject> list = new List<BasicCharacterObject>();
				List<Equipment> list2 = new List<Equipment>();
				TextObject textObject;
				TextObject textObject2;
				TextObject textObject3;
				EducationCampaignBehavior.EducationCharacterProperties[] array;
				string[] array2;
				this._educationBehavior.GetPageProperties(this._child, this._selectedOptions.Take(index).ToList<string>(), out textObject, out textObject2, out textObject3, out array, out array2);
				this._currentPageTitleTextObj = textObject;
				this._currentPageDescriptionTextObj = textObject2;
				this._currentPageInstructionTextObj = textObject3;
				for (int i = 0; i < array2.Length; i++)
				{
					TextObject textObject4;
					TextObject textObject5;
					TextObject textObject6;
					ValueTuple<CharacterAttribute, int>[] array3;
					ValueTuple<SkillObject, int>[] array4;
					ValueTuple<SkillObject, int>[] array5;
					EducationCampaignBehavior.EducationCharacterProperties[] array6;
					this._educationBehavior.GetOptionProperties(this._child, array2[i], this._selectedOptions, out textObject4, out textObject5, out textObject6, out array3, out array4, out array5, out array6);
					this.Options.Add(new EducationOptionVM(new Action<object>(this.OnOptionSelect), array2[i], textObject4, textObject5, textObject6, false, array3, array4, array5, array6));
					foreach (EducationCampaignBehavior.EducationCharacterProperties educationCharacterProperties in array6)
					{
						if (educationCharacterProperties.Character != null && !list.Contains(educationCharacterProperties.Character))
						{
							list.Add(educationCharacterProperties.Character);
						}
						if (educationCharacterProperties.Equipment != null && !list2.Contains(educationCharacterProperties.Equipment))
						{
							list2.Add(educationCharacterProperties.Equipment);
						}
					}
				}
				this.OnlyHasOneOption = this.Options.Count == 1;
				if (this._selectedOptions.Count > index)
				{
					string text = this._selectedOptions[index];
					int num = array2.IndexOf(text);
					if (num >= 0)
					{
						Action<EducationCampaignBehavior.EducationCharacterProperties[]> onOptionSelect = this._onOptionSelect;
						if (onOptionSelect != null)
						{
							onOptionSelect(this.Options[num].CharacterProperties);
						}
						if (index == this._currentPageIndex)
						{
							this.Options[num].ExecuteAction();
							this.CanAdvance = true;
						}
					}
				}
				else
				{
					EducationCampaignBehavior.EducationCharacterProperties[] array8 = new EducationCampaignBehavior.EducationCharacterProperties[(array != null) ? array.Length : 1];
					for (int k = 0; k < ((array != null) ? array.Length : 0); k++)
					{
						array8[k] = array[k];
						if (array8[k].Character != null && !list.Contains(array8[k].Character))
						{
							list.Add(array8[k].Character);
						}
						if (array8[k].Equipment != null && !list2.Contains(array8[k].Equipment))
						{
							list2.Add(array8[k].Equipment);
						}
					}
					Action<EducationCampaignBehavior.EducationCharacterProperties[]> onOptionSelect2 = this._onOptionSelect;
					if (onOptionSelect2 != null)
					{
						onOptionSelect2(array8);
					}
				}
				if (this.OnlyHasOneOption)
				{
					this.Options[0].ExecuteAction();
				}
				this._sendPossibleCharactersAndEquipment(list, list2);
			}
			else
			{
				this._currentPageTitleTextObj = new TextObject("{=Ck9HT8fQ}Summary", null);
				this._currentPageInstructionTextObj = null;
				this._currentPageDescriptionTextObj = null;
				this.OnlyHasOneOption = false;
				this.CanAdvance = true;
			}
			TextObject currentPageTitleTextObj = this._currentPageTitleTextObj;
			this.StageTitleText = ((currentPageTitleTextObj != null) ? currentPageTitleTextObj.ToString() : null) ?? "";
			TextObject currentPageInstructionTextObj = this._currentPageInstructionTextObj;
			this.ChooseText = ((currentPageInstructionTextObj != null) ? currentPageInstructionTextObj.ToString() : null) ?? "";
			TextObject currentPageDescriptionTextObj = this._currentPageDescriptionTextObj;
			this.PageDescriptionText = ((currentPageDescriptionTextObj != null) ? currentPageDescriptionTextObj.ToString() : null) ?? "";
			if (this._currentPageIndex == 0)
			{
				this.NextText = this._nextPageTextObj.ToString();
				this.PreviousText = GameTexts.FindText("str_exit", null).ToString();
			}
			else if (this._currentPageIndex == this._pageCount - 1)
			{
				this.NextText = GameTexts.FindText("str_done", null).ToString();
				this.PreviousText = this._previousPageTextObj.ToString();
			}
			else
			{
				this.NextText = this._nextPageTextObj.ToString();
				this.PreviousText = this._previousPageTextObj.ToString();
			}
			this.UpdateGainedProperties();
			this.Review.SetCurrentPage(this._currentPageIndex);
		}

		// Token: 0x06001394 RID: 5012 RVA: 0x0004B474 File Offset: 0x00049674
		private void OnOptionSelect(object optionIdAsObj)
		{
			if (optionIdAsObj != this._latestOptionId)
			{
				string optionId = (string)optionIdAsObj;
				EducationOptionVM educationOptionVM = this.Options.FirstOrDefault((EducationOptionVM o) => (string)o.Identifier == optionId);
				this.Options.ApplyActionOnAllItems(delegate(EducationOptionVM o)
				{
					o.IsSelected = false;
				});
				educationOptionVM.IsSelected = true;
				string actionText = educationOptionVM.ActionText;
				if (this._currentPageIndex == this._selectedOptions.Count)
				{
					this._selectedOptions.Add(optionId);
				}
				else if (this._currentPageIndex < this._selectedOptions.Count)
				{
					this._selectedOptions[this._currentPageIndex] = optionId;
				}
				else
				{
					Debug.FailedAssert("Skipped a stage for education!!!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Education\\EducationVM.cs", "OnOptionSelect", 210);
				}
				this.OptionEffectText = educationOptionVM.OptionEffect;
				this.OptionDescriptionText = educationOptionVM.OptionDescription;
				Action<EducationCampaignBehavior.EducationCharacterProperties[]> onOptionSelect = this._onOptionSelect;
				if (onOptionSelect != null)
				{
					onOptionSelect(educationOptionVM.CharacterProperties);
				}
				this.UpdateGainedProperties();
				this.CanAdvance = true;
				this._latestOptionId = optionIdAsObj;
				this.Review.SetGainForStage(this._currentPageIndex, this.OptionEffectText);
			}
		}

		// Token: 0x06001395 RID: 5013 RVA: 0x0004B5B4 File Offset: 0x000497B4
		private void UpdateGainedProperties()
		{
			this.GainedPropertiesController.UpdateWithSelections(this._selectedOptions, this._currentPageIndex);
		}

		// Token: 0x06001396 RID: 5014 RVA: 0x0004B5D0 File Offset: 0x000497D0
		public void ExecuteNextStage()
		{
			if (this._currentPageIndex + 1 < this._pageCount)
			{
				this.InitWithStageIndex(this._currentPageIndex + 1);
				return;
			}
			this._educationBehavior.Finalize(this._child, this._selectedOptions);
			Action<bool> onDone = this._onDone;
			if (onDone == null)
			{
				return;
			}
			onDone(false);
		}

		// Token: 0x06001397 RID: 5015 RVA: 0x0004B624 File Offset: 0x00049824
		public void ExecutePreviousStage()
		{
			if (this._currentPageIndex > 0)
			{
				this.InitWithStageIndex(this._currentPageIndex - 1);
				return;
			}
			if (this._currentPageIndex == 0)
			{
				Action<bool> onDone = this._onDone;
				if (onDone == null)
				{
					return;
				}
				onDone(true);
			}
		}

		// Token: 0x06001398 RID: 5016 RVA: 0x0004B657 File Offset: 0x00049857
		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey != null)
			{
				cancelInputKey.OnFinalize();
			}
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey == null)
			{
				return;
			}
			doneInputKey.OnFinalize();
		}

		// Token: 0x06001399 RID: 5017 RVA: 0x0004B680 File Offset: 0x00049880
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x0600139A RID: 5018 RVA: 0x0004B68F File Offset: 0x0004988F
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x17000689 RID: 1673
		// (get) Token: 0x0600139B RID: 5019 RVA: 0x0004B69E File Offset: 0x0004989E
		// (set) Token: 0x0600139C RID: 5020 RVA: 0x0004B6A6 File Offset: 0x000498A6
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

		// Token: 0x1700068A RID: 1674
		// (get) Token: 0x0600139D RID: 5021 RVA: 0x0004B6C4 File Offset: 0x000498C4
		// (set) Token: 0x0600139E RID: 5022 RVA: 0x0004B6CC File Offset: 0x000498CC
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

		// Token: 0x1700068B RID: 1675
		// (get) Token: 0x0600139F RID: 5023 RVA: 0x0004B6EA File Offset: 0x000498EA
		// (set) Token: 0x060013A0 RID: 5024 RVA: 0x0004B6F2 File Offset: 0x000498F2
		[DataSourceProperty]
		public string StageTitleText
		{
			get
			{
				return this._stageTitleText;
			}
			set
			{
				if (value != this._stageTitleText)
				{
					this._stageTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "StageTitleText");
				}
			}
		}

		// Token: 0x1700068C RID: 1676
		// (get) Token: 0x060013A1 RID: 5025 RVA: 0x0004B715 File Offset: 0x00049915
		// (set) Token: 0x060013A2 RID: 5026 RVA: 0x0004B71D File Offset: 0x0004991D
		[DataSourceProperty]
		public string ChooseText
		{
			get
			{
				return this._chooseText;
			}
			set
			{
				if (value != this._chooseText)
				{
					this._chooseText = value;
					base.OnPropertyChangedWithValue<string>(value, "ChooseText");
				}
			}
		}

		// Token: 0x1700068D RID: 1677
		// (get) Token: 0x060013A3 RID: 5027 RVA: 0x0004B740 File Offset: 0x00049940
		// (set) Token: 0x060013A4 RID: 5028 RVA: 0x0004B748 File Offset: 0x00049948
		[DataSourceProperty]
		public string PageDescriptionText
		{
			get
			{
				return this._pageDescriptionText;
			}
			set
			{
				if (value != this._pageDescriptionText)
				{
					this._pageDescriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "PageDescriptionText");
				}
			}
		}

		// Token: 0x1700068E RID: 1678
		// (get) Token: 0x060013A5 RID: 5029 RVA: 0x0004B76B File Offset: 0x0004996B
		// (set) Token: 0x060013A6 RID: 5030 RVA: 0x0004B773 File Offset: 0x00049973
		[DataSourceProperty]
		public string OptionEffectText
		{
			get
			{
				return this._optionEffectText;
			}
			set
			{
				if (value != this._optionEffectText)
				{
					this._optionEffectText = value;
					base.OnPropertyChangedWithValue<string>(value, "OptionEffectText");
				}
			}
		}

		// Token: 0x1700068F RID: 1679
		// (get) Token: 0x060013A7 RID: 5031 RVA: 0x0004B796 File Offset: 0x00049996
		// (set) Token: 0x060013A8 RID: 5032 RVA: 0x0004B79E File Offset: 0x0004999E
		[DataSourceProperty]
		public string OptionDescriptionText
		{
			get
			{
				return this._optionDescriptionText;
			}
			set
			{
				if (value != this._optionDescriptionText)
				{
					this._optionDescriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "OptionDescriptionText");
				}
			}
		}

		// Token: 0x17000690 RID: 1680
		// (get) Token: 0x060013A9 RID: 5033 RVA: 0x0004B7C1 File Offset: 0x000499C1
		// (set) Token: 0x060013AA RID: 5034 RVA: 0x0004B7C9 File Offset: 0x000499C9
		[DataSourceProperty]
		public string NextText
		{
			get
			{
				return this._nextText;
			}
			set
			{
				if (value != this._nextText)
				{
					this._nextText = value;
					base.OnPropertyChangedWithValue<string>(value, "NextText");
				}
			}
		}

		// Token: 0x17000691 RID: 1681
		// (get) Token: 0x060013AB RID: 5035 RVA: 0x0004B7EC File Offset: 0x000499EC
		// (set) Token: 0x060013AC RID: 5036 RVA: 0x0004B7F4 File Offset: 0x000499F4
		[DataSourceProperty]
		public string PreviousText
		{
			get
			{
				return this._previousText;
			}
			set
			{
				if (value != this._previousText)
				{
					this._previousText = value;
					base.OnPropertyChangedWithValue<string>(value, "PreviousText");
				}
			}
		}

		// Token: 0x17000692 RID: 1682
		// (get) Token: 0x060013AD RID: 5037 RVA: 0x0004B817 File Offset: 0x00049A17
		// (set) Token: 0x060013AE RID: 5038 RVA: 0x0004B81F File Offset: 0x00049A1F
		[DataSourceProperty]
		public bool CanAdvance
		{
			get
			{
				return this._canAdvance;
			}
			set
			{
				if (value != this._canAdvance)
				{
					this._canAdvance = value;
					base.OnPropertyChangedWithValue(value, "CanAdvance");
				}
			}
		}

		// Token: 0x17000693 RID: 1683
		// (get) Token: 0x060013AF RID: 5039 RVA: 0x0004B83D File Offset: 0x00049A3D
		// (set) Token: 0x060013B0 RID: 5040 RVA: 0x0004B845 File Offset: 0x00049A45
		[DataSourceProperty]
		public bool CanGoBack
		{
			get
			{
				return this._canGoBack;
			}
			set
			{
				if (value != this._canGoBack)
				{
					this._canGoBack = value;
					base.OnPropertyChangedWithValue(value, "CanGoBack");
				}
			}
		}

		// Token: 0x17000694 RID: 1684
		// (get) Token: 0x060013B1 RID: 5041 RVA: 0x0004B863 File Offset: 0x00049A63
		// (set) Token: 0x060013B2 RID: 5042 RVA: 0x0004B86B File Offset: 0x00049A6B
		[DataSourceProperty]
		public bool OnlyHasOneOption
		{
			get
			{
				return this._onlyHasOneOption;
			}
			set
			{
				if (value != this._onlyHasOneOption)
				{
					this._onlyHasOneOption = value;
					base.OnPropertyChangedWithValue(value, "OnlyHasOneOption");
				}
			}
		}

		// Token: 0x17000695 RID: 1685
		// (get) Token: 0x060013B3 RID: 5043 RVA: 0x0004B889 File Offset: 0x00049A89
		// (set) Token: 0x060013B4 RID: 5044 RVA: 0x0004B891 File Offset: 0x00049A91
		[DataSourceProperty]
		public MBBindingList<EducationOptionVM> Options
		{
			get
			{
				return this._options;
			}
			set
			{
				if (value != this._options)
				{
					this._options = value;
					base.OnPropertyChangedWithValue<MBBindingList<EducationOptionVM>>(value, "Options");
				}
			}
		}

		// Token: 0x17000696 RID: 1686
		// (get) Token: 0x060013B5 RID: 5045 RVA: 0x0004B8AF File Offset: 0x00049AAF
		// (set) Token: 0x060013B6 RID: 5046 RVA: 0x0004B8B7 File Offset: 0x00049AB7
		[DataSourceProperty]
		public EducationGainedPropertiesVM GainedPropertiesController
		{
			get
			{
				return this._gainedPropertiesController;
			}
			set
			{
				if (value != this._gainedPropertiesController)
				{
					this._gainedPropertiesController = value;
					base.OnPropertyChangedWithValue<EducationGainedPropertiesVM>(value, "GainedPropertiesController");
				}
			}
		}

		// Token: 0x17000697 RID: 1687
		// (get) Token: 0x060013B7 RID: 5047 RVA: 0x0004B8D5 File Offset: 0x00049AD5
		// (set) Token: 0x060013B8 RID: 5048 RVA: 0x0004B8DD File Offset: 0x00049ADD
		[DataSourceProperty]
		public EducationReviewVM Review
		{
			get
			{
				return this._review;
			}
			set
			{
				if (value != this._review)
				{
					this._review = value;
					base.OnPropertyChangedWithValue<EducationReviewVM>(value, "Review");
				}
			}
		}

		// Token: 0x04000910 RID: 2320
		private readonly Action<bool> _onDone;

		// Token: 0x04000911 RID: 2321
		private readonly Action<EducationCampaignBehavior.EducationCharacterProperties[]> _onOptionSelect;

		// Token: 0x04000912 RID: 2322
		private readonly Action<List<BasicCharacterObject>, List<Equipment>> _sendPossibleCharactersAndEquipment;

		// Token: 0x04000913 RID: 2323
		private readonly IEducationLogic _educationBehavior;

		// Token: 0x04000914 RID: 2324
		private readonly Hero _child;

		// Token: 0x04000915 RID: 2325
		private readonly TextObject _nextPageTextObj = new TextObject("{=Rvr1bcu8}Next", null);

		// Token: 0x04000916 RID: 2326
		private readonly TextObject _previousPageTextObj = new TextObject("{=WXAaWZVf}Previous", null);

		// Token: 0x04000917 RID: 2327
		private readonly int _pageCount;

		// Token: 0x04000918 RID: 2328
		private readonly List<string> _selectedOptions = new List<string>();

		// Token: 0x04000919 RID: 2329
		private TextObject _currentPageTitleTextObj;

		// Token: 0x0400091A RID: 2330
		private TextObject _currentPageDescriptionTextObj;

		// Token: 0x0400091B RID: 2331
		private TextObject _currentPageInstructionTextObj;

		// Token: 0x0400091C RID: 2332
		private object _latestOptionId;

		// Token: 0x0400091D RID: 2333
		private int _currentPageIndex;

		// Token: 0x0400091E RID: 2334
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x0400091F RID: 2335
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000920 RID: 2336
		private string _stageTitleText;

		// Token: 0x04000921 RID: 2337
		private string _chooseText;

		// Token: 0x04000922 RID: 2338
		private string _pageDescriptionText;

		// Token: 0x04000923 RID: 2339
		private string _optionEffectText;

		// Token: 0x04000924 RID: 2340
		private string _optionDescriptionText;

		// Token: 0x04000925 RID: 2341
		private string _nextText;

		// Token: 0x04000926 RID: 2342
		private string _previousText;

		// Token: 0x04000927 RID: 2343
		private bool _canAdvance;

		// Token: 0x04000928 RID: 2344
		private bool _canGoBack;

		// Token: 0x04000929 RID: 2345
		private bool _onlyHasOneOption;

		// Token: 0x0400092A RID: 2346
		private MBBindingList<EducationOptionVM> _options;

		// Token: 0x0400092B RID: 2347
		private EducationGainedPropertiesVM _gainedPropertiesController;

		// Token: 0x0400092C RID: 2348
		private EducationReviewVM _review;
	}
}
