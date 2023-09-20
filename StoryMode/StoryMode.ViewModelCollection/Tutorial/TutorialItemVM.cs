using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace StoryMode.ViewModelCollection.Tutorial
{
	public class TutorialItemVM : ViewModel
	{
		public Action<bool> SetIsActive { get; private set; }

		public TutorialItemVM()
		{
			this.CenterImage = new ImageIdentifierVM(0);
			this.IsEnabled = false;
		}

		public void Init(string tutorialTypeId, bool requiresMouse, Action onFinishTutorial)
		{
			this.IsEnabled = false;
			this.StepCountText = "DISABLED";
			this.RequiresMouse = requiresMouse;
			this.IsEnabled = true;
			this._onFinishTutorial = onFinishTutorial;
			this._tutorialTypeId = tutorialTypeId;
			this.AreTutorialsEnabled = BannerlordConfig.EnableTutorialHints;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.DisableCurrentTutorialHint = new HintViewModel(GameTexts.FindText("str_disable_current_tutorial_step", null), null);
			this.DisableAllTutorialsHint = new HintViewModel(GameTexts.FindText("str_disable_all_tutorials", null), null);
			this.TutorialsEnabledText = GameTexts.FindText("str_tutorials_enabled", null).ToString();
			this.TutorialTitleText = GameTexts.FindText("str_initial_menu_option", "Tutorial").ToString();
			this.TitleText = GameTexts.FindText("str_campaign_tutorial_title", this._tutorialTypeId).ToString();
			TextObject textObject;
			if (Input.IsControllerConnected && !Input.IsMouseActive && GameTexts.TryGetText("str_campaign_tutorial_description", ref textObject, this._tutorialTypeId + "_controller"))
			{
				this.DescriptionText = textObject.ToString();
				return;
			}
			this.DescriptionText = GameTexts.FindText("str_campaign_tutorial_description", this._tutorialTypeId).ToString();
		}

		public void CloseTutorialPanel()
		{
			this.IsEnabled = false;
		}

		private void ExecuteFinishTutorial()
		{
			this._onFinishTutorial();
		}

		private void ExecuteToggleDisableAllTutorials()
		{
			this.AreTutorialsEnabled = !this.AreTutorialsEnabled;
		}

		[DataSourceProperty]
		public HintViewModel DisableCurrentTutorialHint
		{
			get
			{
				return this._disableCurrentTutorialHint;
			}
			set
			{
				if (value != this._disableCurrentTutorialHint)
				{
					this._disableCurrentTutorialHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DisableCurrentTutorialHint");
				}
			}
		}

		[DataSourceProperty]
		public bool AreTutorialsEnabled
		{
			get
			{
				return this._areTutorialsEnabled;
			}
			set
			{
				if (value != this._areTutorialsEnabled)
				{
					this._areTutorialsEnabled = value;
					base.OnPropertyChangedWithValue(value, "AreTutorialsEnabled");
					BannerlordConfig.EnableTutorialHints = value;
				}
			}
		}

		[DataSourceProperty]
		public string TutorialsEnabledText
		{
			get
			{
				return this._tutorialsEnabledText;
			}
			set
			{
				if (value != this._tutorialsEnabledText)
				{
					this._tutorialsEnabledText = value;
					base.OnPropertyChangedWithValue<string>(value, "TutorialsEnabledText");
				}
			}
		}

		[DataSourceProperty]
		public string TutorialTitleText
		{
			get
			{
				return this._tutorialTitleText;
			}
			set
			{
				if (value != this._tutorialTitleText)
				{
					this._tutorialTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TutorialTitleText");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel DisableAllTutorialsHint
		{
			get
			{
				return this._disableAllTutorialsHint;
			}
			set
			{
				if (value != this._disableAllTutorialsHint)
				{
					this._disableAllTutorialsHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DisableAllTutorialsHint");
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
		public string StepCountText
		{
			get
			{
				return this._stepCountText;
			}
			set
			{
				if (value != this._stepCountText)
				{
					this._stepCountText = value;
					base.OnPropertyChangedWithValue<string>(value, "StepCountText");
				}
			}
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
				}
			}
		}

		[DataSourceProperty]
		public string DescriptionText
		{
			get
			{
				return this._descriptionText;
			}
			set
			{
				if (value != this._descriptionText)
				{
					this._descriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "DescriptionText");
				}
			}
		}

		[DataSourceProperty]
		public string SoundId
		{
			get
			{
				return this._soundId;
			}
			set
			{
				if (value != this._soundId)
				{
					this._soundId = value;
					base.OnPropertyChanged("SoundId");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM CenterImage
		{
			get
			{
				return this._centerImage;
			}
			set
			{
				if (value != this._centerImage)
				{
					this._centerImage = value;
					base.OnPropertyChanged("CenterImage");
				}
			}
		}

		[DataSourceProperty]
		public bool RequiresMouse
		{
			get
			{
				return this._requiresMouse;
			}
			set
			{
				if (value != this._requiresMouse)
				{
					this._requiresMouse = value;
					base.OnPropertyChanged("RequiresMouse");
				}
			}
		}

		private const string ControllerIdentificationModifier = "_controller";

		private string _tutorialTypeId;

		private Action _onFinishTutorial;

		private string _titleText;

		private string _descriptionText;

		private ImageIdentifierVM _centerImage;

		private string _soundId;

		private string _stepCountText;

		private string _tutorialsEnabledText;

		private string _tutorialTitleText;

		private bool _isEnabled;

		private bool _requiresMouse;

		private HintViewModel _disableCurrentTutorialHint;

		private HintViewModel _disableAllTutorialsHint;

		private bool _areTutorialsEnabled;

		public enum ItemPlacements
		{
			Left,
			Right,
			Top,
			Bottom,
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight,
			Center
		}
	}
}
