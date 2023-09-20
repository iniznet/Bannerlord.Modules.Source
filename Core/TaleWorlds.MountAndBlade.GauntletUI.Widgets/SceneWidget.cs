using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class SceneWidget : TextureWidget
	{
		private bool _isClickToContinueActive
		{
			get
			{
				return this._clickToContinueStartTime != -1f && base.EventManager.Time - this._clickToContinueStartTime >= this._clickToContinueDelayInSeconds;
			}
		}

		private float _clickToContinueDelayInSeconds
		{
			get
			{
				return 2f;
			}
		}

		public SceneWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "SceneTextureProvider";
			this._isRenderRequestedPreviousFrame = true;
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.Scene != null && !this._initialized)
			{
				this.DetermineInitContinueState();
				this._initialized = true;
			}
			if (this.Scene != null && !this.IsReady)
			{
				bool? flag = (bool?)base.GetTextureProviderProperty("IsReady");
				bool flag2 = true;
				this.IsReady = (flag.GetValueOrDefault() == flag2) & (flag != null);
			}
			if (this._isInClickToContinueState)
			{
				if (this._isClickToContinueActive)
				{
					this.ClickToContinueTextWidget.SetGlobalAlphaRecursively(Mathf.Lerp(this.ClickToContinueTextWidget.ReadOnlyBrush.GlobalAlphaFactor, 1f, dt * 10f));
					if (!this._prevIsClickToContinueActive)
					{
						this.ClickToContinueTextWidget.BrushRenderer.RestartAnimation();
					}
				}
				this.CancelButton.SetGlobalAlphaRecursively(Mathf.Lerp(this.CancelButton.ReadOnlyBrush.GlobalAlphaFactor, 0f, dt * 10f));
				this.AffirmativeButton.SetGlobalAlphaRecursively(Mathf.Lerp(this.AffirmativeButton.ReadOnlyBrush.GlobalAlphaFactor, 0f, dt * 10f));
			}
			else
			{
				this.ClickToContinueTextWidget.SetGlobalAlphaRecursively(Mathf.Lerp(this.ClickToContinueTextWidget.ReadOnlyBrush.GlobalAlphaFactor, 0f, dt * 10f));
				this.CancelButton.SetGlobalAlphaRecursively(Mathf.Lerp(this.CancelButton.ReadOnlyBrush.GlobalAlphaFactor, (float)(this.IsCancelShown ? 1 : 0), dt * 10f));
				this.AffirmativeButton.SetGlobalAlphaRecursively(Mathf.Lerp(this.AffirmativeButton.ReadOnlyBrush.GlobalAlphaFactor, (float)(this.IsOkShown ? 1 : 0), dt * 10f));
			}
			this.UpdateVisibilityOfWidgetBasedOnAlpha(this.ClickToContinueTextWidget);
			this.UpdateVisibilityOfWidgetBasedOnAlpha(this.CancelButton);
			this.UpdateVisibilityOfWidgetBasedOnAlpha(this.AffirmativeButton);
			this.HandleTitleTextChange();
			this.FadeImageWidget.AlphaFactor = (this.IsReady ? this.EndProgress : 1f);
			this.PreparingVisualWidget.IsVisible = !this.IsReady;
			this._prevIsClickToContinueActive = this._isClickToContinueActive;
		}

		private void UpdateVisibilityOfWidgetBasedOnAlpha(BrushWidget widget)
		{
			widget.IsVisible = !widget.ReadOnlyBrush.GlobalAlphaFactor.ApproximatelyEqualsTo(0f, 0.01f);
		}

		protected override void OnMousePressed()
		{
			base.OnMousePressed();
			if (this._isClickToContinueActive)
			{
				base.EventFired("Close", Array.Empty<object>());
				this.ResetStates();
			}
		}

		private void OnAnyActionButtonClick()
		{
			this._isInClickToContinueState = true;
			base.DoNotAcceptEvents = false;
			base.DoNotPassEventsToChildren = true;
			this.ClickToContinueTextWidget.BrushRenderer.RestartAnimation();
			this._clickToContinueStartTime = base.EventManager.Time;
		}

		private void ResetStates()
		{
			this._isInClickToContinueState = false;
			base.DoNotAcceptEvents = true;
			base.DoNotPassEventsToChildren = false;
			this._initialized = false;
			this.IsReady = false;
			this._titleChangeStartTime = -1f;
			this._currentTitleTextToUpdateTo = string.Empty;
			this.TitleTextWidget.SetAlpha(1f);
		}

		private void OnAffirmativeButtonClick(Widget obj)
		{
			this.SetNewTitleText(this.AffirmativeTitleText);
			this.OnAnyActionButtonClick();
		}

		private void OnCancelButtonClick(Widget obj)
		{
			this.SetNewTitleText(this.NegativeTitleText);
			this.OnAnyActionButtonClick();
		}

		private void SetNewTitleText(string newText)
		{
			if (!string.IsNullOrEmpty(newText))
			{
				this._currentTitleTextToUpdateTo = newText;
				this._titleChangeStartTime = base.EventManager.Time;
			}
		}

		private void DetermineInitContinueState()
		{
			this.CancelButton.IsVisible = this.IsCancelShown;
			this.CancelButton.SetGlobalAlphaRecursively((float)(this.IsCancelShown ? 1 : 0));
			this.AffirmativeButton.IsVisible = this.IsOkShown;
			this.AffirmativeButton.SetGlobalAlphaRecursively((float)(this.IsOkShown ? 1 : 0));
			this.ClickToContinueTextWidget.SetGlobalAlphaRecursively(0f);
			this._isInClickToContinueState = !this.IsCancelShown && !this.IsOkShown;
			if (this._isInClickToContinueState)
			{
				this._clickToContinueStartTime = base.EventManager.Time;
			}
			base.DoNotAcceptEvents = !this._isInClickToContinueState;
			base.DoNotPassEventsToChildren = this._isInClickToContinueState;
		}

		private void HandleTitleTextChange()
		{
			if (this._titleChangeStartTime != -1f)
			{
				if (!string.IsNullOrEmpty(this._currentTitleTextToUpdateTo) && this.TitleTextWidget != null && base.EventManager.Time - this._titleChangeStartTime < this._titleChangeTotalTimeInSeconds)
				{
					if (base.EventManager.Time - this._titleChangeStartTime >= this._titleChangeTotalTimeInSeconds / 2f)
					{
						this.TitleTextWidget.Text = this._currentTitleTextToUpdateTo;
					}
					float num = 1f - MathF.PingPong(0f, 1f, base.EventManager.Time - this._titleChangeStartTime);
					this.TitleTextWidget.SetAlpha(num);
					return;
				}
				this.TitleTextWidget.SetAlpha(1f);
			}
		}

		[Editor(false)]
		public object Scene
		{
			get
			{
				return this._scene;
			}
			set
			{
				if (value != this._scene)
				{
					this._scene = value;
					base.OnPropertyChanged<object>(value, "Scene");
					base.SetTextureProviderProperty("Scene", value);
					if (value != null)
					{
						this._isTargetSizeDirty = true;
						this.ResetStates();
					}
				}
			}
		}

		[Editor(false)]
		public ButtonWidget AffirmativeButton
		{
			get
			{
				return this._affirmativeButton;
			}
			set
			{
				if (value != this._affirmativeButton)
				{
					this._affirmativeButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "AffirmativeButton");
					ButtonWidget affirmativeButton = this._affirmativeButton;
					if (affirmativeButton == null)
					{
						return;
					}
					affirmativeButton.ClickEventHandlers.Add(new Action<Widget>(this.OnAffirmativeButtonClick));
				}
			}
		}

		[Editor(false)]
		public ButtonWidget CancelButton
		{
			get
			{
				return this._cancelButton;
			}
			set
			{
				if (value != this._cancelButton)
				{
					this._cancelButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "CancelButton");
					ButtonWidget cancelButton = this._cancelButton;
					if (cancelButton == null)
					{
						return;
					}
					cancelButton.ClickEventHandlers.Add(new Action<Widget>(this.OnCancelButtonClick));
				}
			}
		}

		[Editor(false)]
		public RichTextWidget ClickToContinueTextWidget
		{
			get
			{
				return this._clickToContinueTextWidget;
			}
			set
			{
				if (value != this._clickToContinueTextWidget)
				{
					this._clickToContinueTextWidget = value;
					base.OnPropertyChanged<RichTextWidget>(value, "ClickToContinueTextWidget");
				}
			}
		}

		[Editor(false)]
		public TextWidget TitleTextWidget
		{
			get
			{
				return this._titleTextWidget;
			}
			set
			{
				if (value != this._titleTextWidget)
				{
					this._titleTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "TitleTextWidget");
				}
			}
		}

		[Editor(false)]
		public Widget FadeImageWidget
		{
			get
			{
				return this._fadeImageWidget;
			}
			set
			{
				if (value != this._fadeImageWidget)
				{
					this._fadeImageWidget = value;
					base.OnPropertyChanged<Widget>(value, "FadeImageWidget");
				}
			}
		}

		[Editor(false)]
		public Widget PreparingVisualWidget
		{
			get
			{
				return this._preparingVisualWidget;
			}
			set
			{
				if (value != this._preparingVisualWidget)
				{
					this._preparingVisualWidget = value;
					base.OnPropertyChanged<Widget>(value, "PreparingVisualWidget");
				}
			}
		}

		[Editor(false)]
		public float EndProgress
		{
			get
			{
				return this._endProgress;
			}
			set
			{
				if (value != this._endProgress)
				{
					this._endProgress = value;
					base.OnPropertyChanged(value, "EndProgress");
				}
			}
		}

		[Editor(false)]
		public bool IsOkShown
		{
			get
			{
				return this._isOkShown;
			}
			set
			{
				if (value != this._isOkShown)
				{
					this._isOkShown = value;
					base.OnPropertyChanged(value, "IsOkShown");
					this.DetermineInitContinueState();
				}
			}
		}

		[Editor(false)]
		public bool IsCancelShown
		{
			get
			{
				return this._isCancelShown;
			}
			set
			{
				if (value != this._isCancelShown)
				{
					this._isCancelShown = value;
					base.OnPropertyChanged(value, "IsCancelShown");
					this.DetermineInitContinueState();
				}
			}
		}

		[Editor(false)]
		public bool IsReady
		{
			get
			{
				return this._isReady;
			}
			set
			{
				if (value != this._isReady)
				{
					this._isReady = value;
					base.OnPropertyChanged(value, "IsReady");
				}
			}
		}

		[Editor(false)]
		public string AffirmativeTitleText
		{
			get
			{
				return this._affirmativeTitleText;
			}
			set
			{
				if (value != this._affirmativeTitleText)
				{
					this._affirmativeTitleText = value;
					base.OnPropertyChanged<string>(value, "AffirmativeTitleText");
				}
			}
		}

		[Editor(false)]
		public string NegativeTitleText
		{
			get
			{
				return this._negativeTitleText;
			}
			set
			{
				if (value != this._negativeTitleText)
				{
					this._negativeTitleText = value;
					base.OnPropertyChanged<string>(value, "NegativeTitleText");
				}
			}
		}

		private bool _isInClickToContinueState;

		private bool _prevIsClickToContinueActive;

		private bool _initialized;

		private float _clickToContinueStartTime = -1f;

		private string _currentTitleTextToUpdateTo = string.Empty;

		private float _titleChangeStartTime = -1f;

		private float _titleChangeTotalTimeInSeconds = 2f;

		private object _scene;

		private ButtonWidget _cancelButton;

		private ButtonWidget _affirmativeButton;

		private RichTextWidget _clickToContinueTextWidget;

		private Widget _fadeImageWidget;

		private TextWidget _titleTextWidget;

		private float _endProgress;

		private string _affirmativeTitleText;

		private string _negativeTitleText;

		private Widget _preparingVisualWidget;

		private bool _isCancelShown;

		private bool _isOkShown;

		private bool _isReady;
	}
}
