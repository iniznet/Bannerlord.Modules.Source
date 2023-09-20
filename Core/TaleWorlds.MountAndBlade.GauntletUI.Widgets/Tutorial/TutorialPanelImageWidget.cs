using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tutorial
{
	// Token: 0x02000043 RID: 67
	public class TutorialPanelImageWidget : ImageWidget
	{
		// Token: 0x06000382 RID: 898 RVA: 0x0000B82C File Offset: 0x00009A2C
		public TutorialPanelImageWidget(UIContext context)
			: base(context)
		{
			base.UseGlobalTimeForAnimation = true;
		}

		// Token: 0x06000383 RID: 899 RVA: 0x0000B83C File Offset: 0x00009A3C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._animState == TutorialPanelImageWidget.AnimState.Start)
			{
				this._tickCount++;
				if (this._tickCount > 20)
				{
					this._animState = TutorialPanelImageWidget.AnimState.Starting;
					return;
				}
			}
			else if (this._animState == TutorialPanelImageWidget.AnimState.Starting)
			{
				BrushListPanel tutorialPanel = this.TutorialPanel;
				if (tutorialPanel != null)
				{
					tutorialPanel.BrushRenderer.RestartAnimation();
				}
				this._animState = TutorialPanelImageWidget.AnimState.Playing;
			}
		}

		// Token: 0x06000384 RID: 900 RVA: 0x0000B89F File Offset: 0x00009A9F
		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			this.Initialize();
		}

		// Token: 0x06000385 RID: 901 RVA: 0x0000B8B0 File Offset: 0x00009AB0
		private void Initialize()
		{
			if (base.IsDisabled)
			{
				this.SetState("Disabled");
				this._animState = TutorialPanelImageWidget.AnimState.Idle;
				this._tickCount = 0;
			}
			else if (this._animState != TutorialPanelImageWidget.AnimState.Start)
			{
				this.SetState("Default");
				this._animState = TutorialPanelImageWidget.AnimState.Start;
				base.Context.TwoDimensionContext.PlaySound("panels/tutorial");
			}
			base.IsVisible = base.IsEnabled;
		}

		// Token: 0x06000386 RID: 902 RVA: 0x0000B91C File Offset: 0x00009B1C
		protected override void RefreshState()
		{
			base.RefreshState();
			this.Initialize();
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000387 RID: 903 RVA: 0x0000B92A File Offset: 0x00009B2A
		// (set) Token: 0x06000388 RID: 904 RVA: 0x0000B932 File Offset: 0x00009B32
		[Editor(false)]
		public BrushListPanel TutorialPanel
		{
			get
			{
				return this._tutorialPanel;
			}
			set
			{
				if (this._tutorialPanel != value)
				{
					this._tutorialPanel = value;
					base.OnPropertyChanged<BrushListPanel>(value, "TutorialPanel");
					if (this._tutorialPanel != null)
					{
						this._tutorialPanel.UseGlobalTimeForAnimation = true;
					}
				}
			}
		}

		// Token: 0x0400017D RID: 381
		private TutorialPanelImageWidget.AnimState _animState;

		// Token: 0x0400017E RID: 382
		private int _tickCount;

		// Token: 0x0400017F RID: 383
		private BrushListPanel _tutorialPanel;

		// Token: 0x02000181 RID: 385
		public enum AnimState
		{
			// Token: 0x040008D0 RID: 2256
			Idle,
			// Token: 0x040008D1 RID: 2257
			Start,
			// Token: 0x040008D2 RID: 2258
			Starting,
			// Token: 0x040008D3 RID: 2259
			Playing
		}
	}
}
