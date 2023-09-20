using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace StoryMode.ViewModelCollection.Tutorial
{
	// Token: 0x02000002 RID: 2
	public class TutorialItemVM : ViewModel
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
		public Action<bool> SetIsActive { get; private set; }

		// Token: 0x06000003 RID: 3 RVA: 0x00002059 File Offset: 0x00000259
		public TutorialItemVM()
		{
			this.CenterImage = new ImageIdentifierVM(0);
			this.IsEnabled = false;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002074 File Offset: 0x00000274
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

		// Token: 0x06000005 RID: 5 RVA: 0x000020C0 File Offset: 0x000002C0
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

		// Token: 0x06000006 RID: 6 RVA: 0x000021A6 File Offset: 0x000003A6
		public void CloseTutorialPanel()
		{
			this.IsEnabled = false;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021AF File Offset: 0x000003AF
		private void ExecuteFinishTutorial()
		{
			this._onFinishTutorial();
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000021BC File Offset: 0x000003BC
		private void ExecuteToggleDisableAllTutorials()
		{
			this.AreTutorialsEnabled = !this.AreTutorialsEnabled;
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000009 RID: 9 RVA: 0x000021CD File Offset: 0x000003CD
		// (set) Token: 0x0600000A RID: 10 RVA: 0x000021D5 File Offset: 0x000003D5
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

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000021F3 File Offset: 0x000003F3
		// (set) Token: 0x0600000C RID: 12 RVA: 0x000021FB File Offset: 0x000003FB
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

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000D RID: 13 RVA: 0x0000221F File Offset: 0x0000041F
		// (set) Token: 0x0600000E RID: 14 RVA: 0x00002227 File Offset: 0x00000427
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

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000F RID: 15 RVA: 0x0000224A File Offset: 0x0000044A
		// (set) Token: 0x06000010 RID: 16 RVA: 0x00002252 File Offset: 0x00000452
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

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000011 RID: 17 RVA: 0x00002275 File Offset: 0x00000475
		// (set) Token: 0x06000012 RID: 18 RVA: 0x0000227D File Offset: 0x0000047D
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

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000013 RID: 19 RVA: 0x0000229B File Offset: 0x0000049B
		// (set) Token: 0x06000014 RID: 20 RVA: 0x000022A3 File Offset: 0x000004A3
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

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000022C6 File Offset: 0x000004C6
		// (set) Token: 0x06000016 RID: 22 RVA: 0x000022CE File Offset: 0x000004CE
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

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000017 RID: 23 RVA: 0x000022F1 File Offset: 0x000004F1
		// (set) Token: 0x06000018 RID: 24 RVA: 0x000022F9 File Offset: 0x000004F9
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

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002317 File Offset: 0x00000517
		// (set) Token: 0x0600001A RID: 26 RVA: 0x0000231F File Offset: 0x0000051F
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

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002342 File Offset: 0x00000542
		// (set) Token: 0x0600001C RID: 28 RVA: 0x0000234A File Offset: 0x0000054A
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

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600001D RID: 29 RVA: 0x0000236C File Offset: 0x0000056C
		// (set) Token: 0x0600001E RID: 30 RVA: 0x00002374 File Offset: 0x00000574
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

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002391 File Offset: 0x00000591
		// (set) Token: 0x06000020 RID: 32 RVA: 0x00002399 File Offset: 0x00000599
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

		// Token: 0x04000001 RID: 1
		private const string ControllerIdentificationModifier = "_controller";

		// Token: 0x04000002 RID: 2
		private string _tutorialTypeId;

		// Token: 0x04000003 RID: 3
		private Action _onFinishTutorial;

		// Token: 0x04000005 RID: 5
		private string _titleText;

		// Token: 0x04000006 RID: 6
		private string _descriptionText;

		// Token: 0x04000007 RID: 7
		private ImageIdentifierVM _centerImage;

		// Token: 0x04000008 RID: 8
		private string _soundId;

		// Token: 0x04000009 RID: 9
		private string _stepCountText;

		// Token: 0x0400000A RID: 10
		private string _tutorialsEnabledText;

		// Token: 0x0400000B RID: 11
		private string _tutorialTitleText;

		// Token: 0x0400000C RID: 12
		private bool _isEnabled;

		// Token: 0x0400000D RID: 13
		private bool _requiresMouse;

		// Token: 0x0400000E RID: 14
		private HintViewModel _disableCurrentTutorialHint;

		// Token: 0x0400000F RID: 15
		private HintViewModel _disableAllTutorialsHint;

		// Token: 0x04000010 RID: 16
		private bool _areTutorialsEnabled;

		// Token: 0x02000007 RID: 7
		public enum ItemPlacements
		{
			// Token: 0x0400002E RID: 46
			Left,
			// Token: 0x0400002F RID: 47
			Right,
			// Token: 0x04000030 RID: 48
			Top,
			// Token: 0x04000031 RID: 49
			Bottom,
			// Token: 0x04000032 RID: 50
			TopLeft,
			// Token: 0x04000033 RID: 51
			TopRight,
			// Token: 0x04000034 RID: 52
			BottomLeft,
			// Token: 0x04000035 RID: 53
			BottomRight,
			// Token: 0x04000036 RID: 54
			Center
		}
	}
}
