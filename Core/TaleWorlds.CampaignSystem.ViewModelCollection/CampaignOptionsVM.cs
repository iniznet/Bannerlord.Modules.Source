using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	public class CampaignOptionsVM : ViewModel
	{
		public CampaignOptionsVM(Action onClose)
		{
			this._onClose = onClose;
			MBBindingList<CampaignOptionItemVM> mbbindingList = new MBBindingList<CampaignOptionItemVM>();
			List<ICampaignOptionData> gameplayCampaignOptions = CampaignOptionsManager.GetGameplayCampaignOptions();
			for (int i = 0; i < gameplayCampaignOptions.Count; i++)
			{
				mbbindingList.Add(new CampaignOptionItemVM(gameplayCampaignOptions[i]));
			}
			this.OptionsController = new CampaignOptionsControllerVM(mbbindingList);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=PXT6aA4J}Campaign Options", null).ToString();
			this.DoneText = GameTexts.FindText("str_done", null).ToString();
			this.ResetTutorialText = new TextObject("{=oUz16Nav}Reset Tutorial", null).ToString();
			this.OptionsController.RefreshValues();
		}

		public void ExecuteDone()
		{
			Action onClose = this._onClose;
			if (onClose == null)
			{
				return;
			}
			onClose();
		}

		[DataSourceProperty]
		public CampaignOptionsControllerVM OptionsController
		{
			get
			{
				return this._optionsController;
			}
			set
			{
				if (value != this._optionsController)
				{
					this._optionsController = value;
					base.OnPropertyChangedWithValue<CampaignOptionsControllerVM>(value, "OptionsController");
				}
			}
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public string DoneText
		{
			get
			{
				return this._doneText;
			}
			set
			{
				if (value != this._doneText)
				{
					this._doneText = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneText");
				}
			}
		}

		[DataSourceProperty]
		public string ResetTutorialText
		{
			get
			{
				return this._resetTutorialText;
			}
			set
			{
				if (value != this._resetTutorialText)
				{
					this._resetTutorialText = value;
					base.OnPropertyChangedWithValue<string>(value, "ResetTutorialText");
				}
			}
		}

		private readonly Action _onClose;

		private string _titleText;

		private string _doneText;

		private string _resetTutorialText;

		private CampaignOptionsControllerVM _optionsController;
	}
}
