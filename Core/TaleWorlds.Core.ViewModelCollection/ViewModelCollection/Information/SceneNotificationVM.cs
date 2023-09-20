using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	// Token: 0x0200001B RID: 27
	public class SceneNotificationVM : ViewModel
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000136 RID: 310 RVA: 0x00004715 File Offset: 0x00002915
		// (set) Token: 0x06000137 RID: 311 RVA: 0x0000471D File Offset: 0x0000291D
		public SceneNotificationData ActiveData { get; private set; }

		// Token: 0x06000138 RID: 312 RVA: 0x00004726 File Offset: 0x00002926
		public SceneNotificationVM(Action onPositiveTrigger, Action closeNotification, Func<string> getContinueInputText)
		{
			this._onPositiveTrigger = onPositiveTrigger;
			this._closeNotification = closeNotification;
			this._getContinueInputText = getContinueInputText;
			this.IsShown = false;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000474A File Offset: 0x0000294A
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ClickToContinueText = this._getContinueInputText();
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00004763 File Offset: 0x00002963
		public void CreateNotification(SceneNotificationData data)
		{
			this.SetData(data);
		}

		// Token: 0x0600013B RID: 315 RVA: 0x0000476C File Offset: 0x0000296C
		public void ForceClose()
		{
			this.IsShown = false;
			base.OnPropertyChanged("TitleText");
			base.OnPropertyChanged("AffirmativeDescription");
			base.OnPropertyChanged("CancelDescription");
			base.OnPropertyChanged("SceneID");
			base.OnPropertyChanged("IsAffirmativeOptionShown");
			base.OnPropertyChanged("IsNegativeOptionShown");
			base.OnPropertyChanged("AffirmativeText");
			base.OnPropertyChanged("NegativeText");
			base.OnPropertyChanged("AffirmativeAction");
			base.OnPropertyChanged("NegativeAction");
			base.OnPropertyChanged("AffirmativeTitleText");
			base.OnPropertyChanged("NegativeTitleText");
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00004804 File Offset: 0x00002A04
		private void SetData(SceneNotificationData data)
		{
			this.ActiveData = data;
			base.OnPropertyChanged("TitleText");
			base.OnPropertyChanged("AffirmativeDescription");
			base.OnPropertyChanged("CancelDescription");
			base.OnPropertyChanged("SceneID");
			base.OnPropertyChanged("IsButtonOkShown");
			base.OnPropertyChanged("IsButtonCancelShown");
			base.OnPropertyChanged("ButtonOkLabel");
			base.OnPropertyChanged("ButtonCancelLabel");
			base.OnPropertyChanged("AffirmativeAction");
			base.OnPropertyChanged("NegativeAction");
			base.OnPropertyChanged("AffirmativeTitleText");
			base.OnPropertyChanged("NegativeTitleText");
			this.SetAffirmativeHintProperties(this.ActiveData.AffirmativeHintText, this.ActiveData.AffirmativeHintTextExtended);
			this.AffirmativeHint = new BasicTooltipViewModel(() => this._affirmativeHintTooltipProperties);
			this.RefreshValues();
			this.IsShown = true;
		}

		// Token: 0x0600013D RID: 317 RVA: 0x000048DC File Offset: 0x00002ADC
		private void SetAffirmativeHintProperties(TextObject defaultHint, TextObject extendedHint)
		{
			this._affirmativeHintTooltipProperties = new List<TooltipProperty>();
			if (!string.IsNullOrEmpty((defaultHint != null) ? defaultHint.ToString() : null))
			{
				if (!string.IsNullOrEmpty((extendedHint != null) ? extendedHint.ToString() : null))
				{
					this._affirmativeHintTooltipProperties.Add(new TooltipProperty("", defaultHint.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None)
					{
						OnlyShowWhenNotExtended = true
					});
					this._affirmativeHintTooltipProperties.Add(new TooltipProperty("", extendedHint.ToString(), 0, true, TooltipProperty.TooltipPropertyFlags.None));
					return;
				}
				this._affirmativeHintTooltipProperties.Add(new TooltipProperty("", defaultHint.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
			}
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000497C File Offset: 0x00002B7C
		public void ExecuteAffirmativeProcess()
		{
			Action onPositiveTrigger = this._onPositiveTrigger;
			if (onPositiveTrigger != null)
			{
				onPositiveTrigger();
			}
			SceneNotificationData activeData = this.ActiveData;
			if (activeData == null)
			{
				return;
			}
			activeData.OnAffirmativeAction();
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000499F File Offset: 0x00002B9F
		public void ExecuteClose()
		{
			SceneNotificationData activeData = this.ActiveData;
			if (activeData != null)
			{
				activeData.OnCloseAction();
			}
			this._closeNotification();
			this.ClearData();
		}

		// Token: 0x06000140 RID: 320 RVA: 0x000049C3 File Offset: 0x00002BC3
		public void ExecuteNegativeProcess()
		{
			this._closeNotification();
			SceneNotificationData activeData = this.ActiveData;
			if (activeData != null)
			{
				activeData.OnNegativeAction();
			}
			this.ClearData();
		}

		// Token: 0x06000141 RID: 321 RVA: 0x000049E7 File Offset: 0x00002BE7
		public void ClearData()
		{
			this.ActiveData = null;
			this.Scene = null;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x000049F7 File Offset: 0x00002BF7
		public override void OnFinalize()
		{
			base.OnFinalize();
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000143 RID: 323 RVA: 0x000049FF File Offset: 0x00002BFF
		// (set) Token: 0x06000144 RID: 324 RVA: 0x00004A07 File Offset: 0x00002C07
		[DataSourceProperty]
		public bool IsShown
		{
			get
			{
				return this._isShown;
			}
			set
			{
				if (this._isShown != value)
				{
					this._isShown = value;
					base.OnPropertyChangedWithValue(value, "IsShown");
				}
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000145 RID: 325 RVA: 0x00004A25 File Offset: 0x00002C25
		// (set) Token: 0x06000146 RID: 326 RVA: 0x00004A2D File Offset: 0x00002C2D
		[DataSourceProperty]
		public bool IsReady
		{
			get
			{
				return this._isReady;
			}
			set
			{
				if (this._isReady != value)
				{
					this._isReady = value;
					base.OnPropertyChangedWithValue(value, "IsReady");
				}
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000147 RID: 327 RVA: 0x00004A4B File Offset: 0x00002C4B
		// (set) Token: 0x06000148 RID: 328 RVA: 0x00004A53 File Offset: 0x00002C53
		[DataSourceProperty]
		public string ClickToContinueText
		{
			get
			{
				return this._clickToContinueText;
			}
			set
			{
				if (this._clickToContinueText != value)
				{
					this._clickToContinueText = value;
					base.OnPropertyChangedWithValue<string>(value, "ClickToContinueText");
				}
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000149 RID: 329 RVA: 0x00004A76 File Offset: 0x00002C76
		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				SceneNotificationData activeData = this.ActiveData;
				return ((activeData != null) ? activeData.TitleText.ToString() : null) ?? string.Empty;
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600014A RID: 330 RVA: 0x00004A98 File Offset: 0x00002C98
		[DataSourceProperty]
		public string AffirmativeDescription
		{
			get
			{
				SceneNotificationData activeData = this.ActiveData;
				string text;
				if (activeData == null)
				{
					text = null;
				}
				else
				{
					TextObject affirmativeDescriptionText = activeData.AffirmativeDescriptionText;
					text = ((affirmativeDescriptionText != null) ? affirmativeDescriptionText.ToString() : null);
				}
				return text ?? string.Empty;
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x0600014B RID: 331 RVA: 0x00004AC1 File Offset: 0x00002CC1
		[DataSourceProperty]
		public string CancelDescription
		{
			get
			{
				SceneNotificationData activeData = this.ActiveData;
				string text;
				if (activeData == null)
				{
					text = null;
				}
				else
				{
					TextObject negativeDescriptionText = activeData.NegativeDescriptionText;
					text = ((negativeDescriptionText != null) ? negativeDescriptionText.ToString() : null);
				}
				return text ?? string.Empty;
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x0600014C RID: 332 RVA: 0x00004AEA File Offset: 0x00002CEA
		[DataSourceProperty]
		public string SceneID
		{
			get
			{
				SceneNotificationData activeData = this.ActiveData;
				return ((activeData != null) ? activeData.SceneID : null) ?? string.Empty;
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x0600014D RID: 333 RVA: 0x00004B07 File Offset: 0x00002D07
		[DataSourceProperty]
		public string ButtonOkLabel
		{
			get
			{
				SceneNotificationData activeData = this.ActiveData;
				string text;
				if (activeData == null)
				{
					text = null;
				}
				else
				{
					TextObject affirmativeText = activeData.AffirmativeText;
					text = ((affirmativeText != null) ? affirmativeText.ToString() : null);
				}
				return text ?? string.Empty;
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x0600014E RID: 334 RVA: 0x00004B30 File Offset: 0x00002D30
		[DataSourceProperty]
		public string ButtonCancelLabel
		{
			get
			{
				SceneNotificationData activeData = this.ActiveData;
				string text;
				if (activeData == null)
				{
					text = null;
				}
				else
				{
					TextObject negativeText = activeData.NegativeText;
					text = ((negativeText != null) ? negativeText.ToString() : null);
				}
				return text ?? string.Empty;
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x0600014F RID: 335 RVA: 0x00004B59 File Offset: 0x00002D59
		[DataSourceProperty]
		public bool IsButtonOkShown
		{
			get
			{
				SceneNotificationData activeData = this.ActiveData;
				return activeData != null && activeData.IsAffirmativeOptionShown;
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000150 RID: 336 RVA: 0x00004B6C File Offset: 0x00002D6C
		[DataSourceProperty]
		public bool IsButtonCancelShown
		{
			get
			{
				SceneNotificationData activeData = this.ActiveData;
				return activeData != null && activeData.IsNegativeOptionShown;
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000151 RID: 337 RVA: 0x00004B7F File Offset: 0x00002D7F
		[DataSourceProperty]
		public string AffirmativeTitleText
		{
			get
			{
				SceneNotificationData activeData = this.ActiveData;
				string text;
				if (activeData == null)
				{
					text = null;
				}
				else
				{
					TextObject affirmativeTitleText = activeData.AffirmativeTitleText;
					text = ((affirmativeTitleText != null) ? affirmativeTitleText.ToString() : null);
				}
				return text ?? string.Empty;
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000152 RID: 338 RVA: 0x00004BA8 File Offset: 0x00002DA8
		[DataSourceProperty]
		public string NegativeTitleText
		{
			get
			{
				SceneNotificationData activeData = this.ActiveData;
				string text;
				if (activeData == null)
				{
					text = null;
				}
				else
				{
					TextObject negativeTitleText = activeData.NegativeTitleText;
					text = ((negativeTitleText != null) ? negativeTitleText.ToString() : null);
				}
				return text ?? string.Empty;
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000153 RID: 339 RVA: 0x00004BD1 File Offset: 0x00002DD1
		// (set) Token: 0x06000154 RID: 340 RVA: 0x00004BD9 File Offset: 0x00002DD9
		[DataSourceProperty]
		public object Scene
		{
			get
			{
				return this._scene;
			}
			set
			{
				if (this._scene != value)
				{
					this._scene = value;
					base.OnPropertyChangedWithValue<object>(value, "Scene");
				}
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000155 RID: 341 RVA: 0x00004BF7 File Offset: 0x00002DF7
		// (set) Token: 0x06000156 RID: 342 RVA: 0x00004BFF File Offset: 0x00002DFF
		[DataSourceProperty]
		public float EndProgress
		{
			get
			{
				return this._endProgress;
			}
			set
			{
				if (this._endProgress != value)
				{
					this._endProgress = value;
					base.OnPropertyChangedWithValue(value, "EndProgress");
				}
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000157 RID: 343 RVA: 0x00004C1D File Offset: 0x00002E1D
		// (set) Token: 0x06000158 RID: 344 RVA: 0x00004C25 File Offset: 0x00002E25
		[DataSourceProperty]
		public BasicTooltipViewModel AffirmativeHint
		{
			get
			{
				return this._affirmativeHint;
			}
			set
			{
				if (value != this._affirmativeHint)
				{
					this._affirmativeHint = value;
					base.OnPropertyChanged("AffirmativeHint");
				}
			}
		}

		// Token: 0x04000080 RID: 128
		private readonly Action _closeNotification;

		// Token: 0x04000081 RID: 129
		private readonly Action _onPositiveTrigger;

		// Token: 0x04000082 RID: 130
		private readonly Func<string> _getContinueInputText;

		// Token: 0x04000083 RID: 131
		private List<TooltipProperty> _affirmativeHintTooltipProperties;

		// Token: 0x04000084 RID: 132
		private bool _isShown;

		// Token: 0x04000085 RID: 133
		private bool _isReady;

		// Token: 0x04000086 RID: 134
		private object _scene;

		// Token: 0x04000087 RID: 135
		private float _endProgress;

		// Token: 0x04000088 RID: 136
		private string _clickToContinueText;

		// Token: 0x04000089 RID: 137
		private BasicTooltipViewModel _affirmativeHint;
	}
}
