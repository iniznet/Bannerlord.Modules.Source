using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tutorial
{
	// Token: 0x02000040 RID: 64
	public class TutorialHighlightItemBrushWidget : BrushWidget
	{
		// Token: 0x17000129 RID: 297
		// (get) Token: 0x06000355 RID: 853 RVA: 0x0000AB04 File Offset: 0x00008D04
		// (set) Token: 0x06000356 RID: 854 RVA: 0x0000AB0C File Offset: 0x00008D0C
		public Widget CustomSizeSyncTarget { get; set; }

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x06000357 RID: 855 RVA: 0x0000AB15 File Offset: 0x00008D15
		// (set) Token: 0x06000358 RID: 856 RVA: 0x0000AB1D File Offset: 0x00008D1D
		public bool DoNotOverrideWidth { get; set; }

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x06000359 RID: 857 RVA: 0x0000AB26 File Offset: 0x00008D26
		// (set) Token: 0x0600035A RID: 858 RVA: 0x0000AB2E File Offset: 0x00008D2E
		public bool DoNotOverrideHeight { get; set; }

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x0600035B RID: 859 RVA: 0x0000AB37 File Offset: 0x00008D37
		private Widget _syncTarget
		{
			get
			{
				return this.CustomSizeSyncTarget ?? base.ParentWidget;
			}
		}

		// Token: 0x0600035C RID: 860 RVA: 0x0000AB49 File Offset: 0x00008D49
		public TutorialHighlightItemBrushWidget(UIContext context)
			: base(context)
		{
			base.UseGlobalTimeForAnimation = true;
			base.DoNotAcceptEvents = true;
		}

		// Token: 0x0600035D RID: 861 RVA: 0x0000AB60 File Offset: 0x00008D60
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._animState == TutorialHighlightItemBrushWidget.AnimState.Start)
			{
				this._animState = TutorialHighlightItemBrushWidget.AnimState.FirstFrame;
			}
			else if (this._animState == TutorialHighlightItemBrushWidget.AnimState.FirstFrame)
			{
				if (base.BrushRenderer.Brush == null)
				{
					this._animState = TutorialHighlightItemBrushWidget.AnimState.Start;
				}
				else
				{
					this._animState = TutorialHighlightItemBrushWidget.AnimState.Playing;
					base.BrushRenderer.RestartAnimation();
				}
			}
			if (this.IsHighlightEnabled && this._isDisabled)
			{
				this._isDisabled = false;
				this.SetState("Default");
			}
			else if (!this.IsHighlightEnabled && !this._isDisabled)
			{
				this.SetState("Disabled");
				this._isDisabled = true;
			}
			if (this._shouldSyncSize && this._syncTarget.Size.X > 1f && this._syncTarget.Size.Y > 1f)
			{
				if (!this.DoNotOverrideWidth)
				{
					base.ScaledSuggestedWidth = this._syncTarget.Size.X - 1f;
				}
				if (!this.DoNotOverrideHeight)
				{
					base.ScaledSuggestedHeight = this._syncTarget.Size.Y - 1f;
				}
			}
			if (this._syncTarget.HeightSizePolicy == SizePolicy.CoverChildren || this._syncTarget.WidthSizePolicy == SizePolicy.CoverChildren)
			{
				if (!this.DoNotOverrideWidth)
				{
					base.WidthSizePolicy = SizePolicy.Fixed;
				}
				if (!this.DoNotOverrideHeight)
				{
					base.HeightSizePolicy = SizePolicy.Fixed;
				}
				this._shouldSyncSize = true;
				return;
			}
			base.WidthSizePolicy = SizePolicy.StretchToParent;
			base.HeightSizePolicy = SizePolicy.StretchToParent;
			this._shouldSyncSize = false;
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x0600035E RID: 862 RVA: 0x0000ACD2 File Offset: 0x00008ED2
		// (set) Token: 0x0600035F RID: 863 RVA: 0x0000ACDC File Offset: 0x00008EDC
		[Editor(false)]
		public bool IsHighlightEnabled
		{
			get
			{
				return this._isHighlightEnabled;
			}
			set
			{
				if (this._isHighlightEnabled != value)
				{
					this._isHighlightEnabled = value;
					base.OnPropertyChanged(value, "IsHighlightEnabled");
					if (this.IsHighlightEnabled)
					{
						this._animState = TutorialHighlightItemBrushWidget.AnimState.Start;
					}
					base.IsVisible = value;
					TaleWorlds.GauntletUI.EventManager.UIEventManager.TriggerEvent<TutorialHighlightItemBrushWidget.HighlightElementToggledEvent>(new TutorialHighlightItemBrushWidget.HighlightElementToggledEvent(value, value ? this : null));
				}
			}
		}

		// Token: 0x04000164 RID: 356
		private TutorialHighlightItemBrushWidget.AnimState _animState;

		// Token: 0x04000165 RID: 357
		private bool _isDisabled;

		// Token: 0x04000166 RID: 358
		private bool _shouldSyncSize;

		// Token: 0x04000167 RID: 359
		private bool _isHighlightEnabled;

		// Token: 0x0200017C RID: 380
		public enum AnimState
		{
			// Token: 0x040008BF RID: 2239
			Idle,
			// Token: 0x040008C0 RID: 2240
			Start,
			// Token: 0x040008C1 RID: 2241
			FirstFrame,
			// Token: 0x040008C2 RID: 2242
			Playing
		}

		// Token: 0x0200017D RID: 381
		public class HighlightElementToggledEvent : EventBase
		{
			// Token: 0x170006A4 RID: 1700
			// (get) Token: 0x060012E1 RID: 4833 RVA: 0x00034188 File Offset: 0x00032388
			// (set) Token: 0x060012E2 RID: 4834 RVA: 0x00034190 File Offset: 0x00032390
			public bool IsEnabled { get; private set; }

			// Token: 0x170006A5 RID: 1701
			// (get) Token: 0x060012E3 RID: 4835 RVA: 0x00034199 File Offset: 0x00032399
			// (set) Token: 0x060012E4 RID: 4836 RVA: 0x000341A1 File Offset: 0x000323A1
			public TutorialHighlightItemBrushWidget HighlightFrameWidget { get; private set; }

			// Token: 0x060012E5 RID: 4837 RVA: 0x000341AA File Offset: 0x000323AA
			public HighlightElementToggledEvent(bool isEnabled, TutorialHighlightItemBrushWidget highlightFrameWidget)
			{
				this.IsEnabled = isEnabled;
				this.HighlightFrameWidget = highlightFrameWidget;
			}
		}
	}
}
