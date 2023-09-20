using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	public class SceneNotificationVM : ViewModel
	{
		public SceneNotificationData ActiveData { get; private set; }

		public SceneNotificationVM(Action onPositiveTrigger, Action closeNotification, Func<string> getContinueInputText)
		{
			this._onPositiveTrigger = onPositiveTrigger;
			this._closeNotification = closeNotification;
			this._getContinueInputText = getContinueInputText;
			this.IsShown = false;
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ClickToContinueText = this._getContinueInputText();
		}

		public void CreateNotification(SceneNotificationData data)
		{
			this.SetData(data);
		}

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

		public void ClearData()
		{
			this.ActiveData = null;
			this.Scene = null;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
		}

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

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				SceneNotificationData activeData = this.ActiveData;
				return ((activeData != null) ? activeData.TitleText.ToString() : null) ?? string.Empty;
			}
		}

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

		[DataSourceProperty]
		public string SceneID
		{
			get
			{
				SceneNotificationData activeData = this.ActiveData;
				return ((activeData != null) ? activeData.SceneID : null) ?? string.Empty;
			}
		}

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

		[DataSourceProperty]
		public bool IsButtonOkShown
		{
			get
			{
				SceneNotificationData activeData = this.ActiveData;
				return activeData != null && activeData.IsAffirmativeOptionShown;
			}
		}

		[DataSourceProperty]
		public bool IsButtonCancelShown
		{
			get
			{
				SceneNotificationData activeData = this.ActiveData;
				return activeData != null && activeData.IsNegativeOptionShown;
			}
		}

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

		private readonly Action _closeNotification;

		private readonly Action _onPositiveTrigger;

		private readonly Func<string> _getContinueInputText;

		private List<TooltipProperty> _affirmativeHintTooltipProperties;

		private bool _isShown;

		private bool _isReady;

		private object _scene;

		private float _endProgress;

		private string _clickToContinueText;

		private BasicTooltipViewModel _affirmativeHint;
	}
}
