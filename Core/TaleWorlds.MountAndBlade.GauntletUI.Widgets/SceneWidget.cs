using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000032 RID: 50
	public class SceneWidget : TextureWidget
	{
		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x060002B7 RID: 695 RVA: 0x00009039 File Offset: 0x00007239
		private bool _isClickToContinueActive
		{
			get
			{
				return this._clickToContinueStartTime != -1f && base.EventManager.Time - this._clickToContinueStartTime >= this._clickToContinueDelayInSeconds;
			}
		}

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x060002B8 RID: 696 RVA: 0x00009067 File Offset: 0x00007267
		private float _clickToContinueDelayInSeconds
		{
			get
			{
				return 2f;
			}
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x00009070 File Offset: 0x00007270
		public SceneWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "SceneTextureProvider";
			this._isRenderRequestedPreviousFrame = true;
		}

		// Token: 0x060002BA RID: 698 RVA: 0x000090C4 File Offset: 0x000072C4
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

		// Token: 0x060002BB RID: 699 RVA: 0x000092E2 File Offset: 0x000074E2
		private void UpdateVisibilityOfWidgetBasedOnAlpha(BrushWidget widget)
		{
			widget.IsVisible = !widget.ReadOnlyBrush.GlobalAlphaFactor.ApproximatelyEqualsTo(0f, 0.01f);
		}

		// Token: 0x060002BC RID: 700 RVA: 0x00009307 File Offset: 0x00007507
		protected override void OnMousePressed()
		{
			base.OnMousePressed();
			if (this._isClickToContinueActive)
			{
				base.EventFired("Close", Array.Empty<object>());
				this.ResetStates();
			}
		}

		// Token: 0x060002BD RID: 701 RVA: 0x0000932D File Offset: 0x0000752D
		private void OnAnyActionButtonClick()
		{
			this._isInClickToContinueState = true;
			base.DoNotAcceptEvents = false;
			base.DoNotPassEventsToChildren = true;
			this.ClickToContinueTextWidget.BrushRenderer.RestartAnimation();
			this._clickToContinueStartTime = base.EventManager.Time;
		}

		// Token: 0x060002BE RID: 702 RVA: 0x00009368 File Offset: 0x00007568
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

		// Token: 0x060002BF RID: 703 RVA: 0x000093BE File Offset: 0x000075BE
		private void OnAffirmativeButtonClick(Widget obj)
		{
			this.SetNewTitleText(this.AffirmativeTitleText);
			this.OnAnyActionButtonClick();
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x000093D2 File Offset: 0x000075D2
		private void OnCancelButtonClick(Widget obj)
		{
			this.SetNewTitleText(this.NegativeTitleText);
			this.OnAnyActionButtonClick();
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x000093E6 File Offset: 0x000075E6
		private void SetNewTitleText(string newText)
		{
			if (!string.IsNullOrEmpty(newText))
			{
				this._currentTitleTextToUpdateTo = newText;
				this._titleChangeStartTime = base.EventManager.Time;
			}
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x00009408 File Offset: 0x00007608
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

		// Token: 0x060002C3 RID: 707 RVA: 0x000094C8 File Offset: 0x000076C8
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

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x060002C4 RID: 708 RVA: 0x00009590 File Offset: 0x00007790
		// (set) Token: 0x060002C5 RID: 709 RVA: 0x00009598 File Offset: 0x00007798
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

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x060002C6 RID: 710 RVA: 0x000095D2 File Offset: 0x000077D2
		// (set) Token: 0x060002C7 RID: 711 RVA: 0x000095DA File Offset: 0x000077DA
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

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x00009619 File Offset: 0x00007819
		// (set) Token: 0x060002C9 RID: 713 RVA: 0x00009621 File Offset: 0x00007821
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

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x060002CA RID: 714 RVA: 0x00009660 File Offset: 0x00007860
		// (set) Token: 0x060002CB RID: 715 RVA: 0x00009668 File Offset: 0x00007868
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

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x060002CC RID: 716 RVA: 0x00009686 File Offset: 0x00007886
		// (set) Token: 0x060002CD RID: 717 RVA: 0x0000968E File Offset: 0x0000788E
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

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x060002CE RID: 718 RVA: 0x000096AC File Offset: 0x000078AC
		// (set) Token: 0x060002CF RID: 719 RVA: 0x000096B4 File Offset: 0x000078B4
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

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x000096D2 File Offset: 0x000078D2
		// (set) Token: 0x060002D1 RID: 721 RVA: 0x000096DA File Offset: 0x000078DA
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

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x000096F8 File Offset: 0x000078F8
		// (set) Token: 0x060002D3 RID: 723 RVA: 0x00009700 File Offset: 0x00007900
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

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x0000971E File Offset: 0x0000791E
		// (set) Token: 0x060002D5 RID: 725 RVA: 0x00009726 File Offset: 0x00007926
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

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x060002D6 RID: 726 RVA: 0x0000974A File Offset: 0x0000794A
		// (set) Token: 0x060002D7 RID: 727 RVA: 0x00009752 File Offset: 0x00007952
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

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x060002D8 RID: 728 RVA: 0x00009776 File Offset: 0x00007976
		// (set) Token: 0x060002D9 RID: 729 RVA: 0x0000977E File Offset: 0x0000797E
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

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x060002DA RID: 730 RVA: 0x0000979C File Offset: 0x0000799C
		// (set) Token: 0x060002DB RID: 731 RVA: 0x000097A4 File Offset: 0x000079A4
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

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x060002DC RID: 732 RVA: 0x000097C7 File Offset: 0x000079C7
		// (set) Token: 0x060002DD RID: 733 RVA: 0x000097CF File Offset: 0x000079CF
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

		// Token: 0x04000118 RID: 280
		private bool _isInClickToContinueState;

		// Token: 0x04000119 RID: 281
		private bool _prevIsClickToContinueActive;

		// Token: 0x0400011A RID: 282
		private bool _initialized;

		// Token: 0x0400011B RID: 283
		private float _clickToContinueStartTime = -1f;

		// Token: 0x0400011C RID: 284
		private string _currentTitleTextToUpdateTo = string.Empty;

		// Token: 0x0400011D RID: 285
		private float _titleChangeStartTime = -1f;

		// Token: 0x0400011E RID: 286
		private float _titleChangeTotalTimeInSeconds = 2f;

		// Token: 0x0400011F RID: 287
		private object _scene;

		// Token: 0x04000120 RID: 288
		private ButtonWidget _cancelButton;

		// Token: 0x04000121 RID: 289
		private ButtonWidget _affirmativeButton;

		// Token: 0x04000122 RID: 290
		private RichTextWidget _clickToContinueTextWidget;

		// Token: 0x04000123 RID: 291
		private Widget _fadeImageWidget;

		// Token: 0x04000124 RID: 292
		private TextWidget _titleTextWidget;

		// Token: 0x04000125 RID: 293
		private float _endProgress;

		// Token: 0x04000126 RID: 294
		private string _affirmativeTitleText;

		// Token: 0x04000127 RID: 295
		private string _negativeTitleText;

		// Token: 0x04000128 RID: 296
		private Widget _preparingVisualWidget;

		// Token: 0x04000129 RID: 297
		private bool _isCancelShown;

		// Token: 0x0400012A RID: 298
		private bool _isOkShown;

		// Token: 0x0400012B RID: 299
		private bool _isReady;
	}
}
