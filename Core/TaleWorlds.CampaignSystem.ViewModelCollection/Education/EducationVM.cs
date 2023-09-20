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
	public class EducationVM : ViewModel
	{
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

		private void UpdateGainedProperties()
		{
			this.GainedPropertiesController.UpdateWithSelections(this._selectedOptions, this._currentPageIndex);
		}

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

		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
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

		private readonly Action<bool> _onDone;

		private readonly Action<EducationCampaignBehavior.EducationCharacterProperties[]> _onOptionSelect;

		private readonly Action<List<BasicCharacterObject>, List<Equipment>> _sendPossibleCharactersAndEquipment;

		private readonly IEducationLogic _educationBehavior;

		private readonly Hero _child;

		private readonly TextObject _nextPageTextObj = new TextObject("{=Rvr1bcu8}Next", null);

		private readonly TextObject _previousPageTextObj = new TextObject("{=WXAaWZVf}Previous", null);

		private readonly int _pageCount;

		private readonly List<string> _selectedOptions = new List<string>();

		private TextObject _currentPageTitleTextObj;

		private TextObject _currentPageDescriptionTextObj;

		private TextObject _currentPageInstructionTextObj;

		private object _latestOptionId;

		private int _currentPageIndex;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private string _stageTitleText;

		private string _chooseText;

		private string _pageDescriptionText;

		private string _optionEffectText;

		private string _optionDescriptionText;

		private string _nextText;

		private string _previousText;

		private bool _canAdvance;

		private bool _canGoBack;

		private bool _onlyHasOneOption;

		private MBBindingList<EducationOptionVM> _options;

		private EducationGainedPropertiesVM _gainedPropertiesController;

		private EducationReviewVM _review;
	}
}
