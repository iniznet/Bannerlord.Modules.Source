using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tutorial
{
	public class TutorialPanelImageWidget : ImageWidget
	{
		public TutorialPanelImageWidget(UIContext context)
			: base(context)
		{
			base.UseGlobalTimeForAnimation = true;
		}

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

		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			this.Initialize();
		}

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

		protected override void RefreshState()
		{
			base.RefreshState();
			this.Initialize();
		}

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

		private TutorialPanelImageWidget.AnimState _animState;

		private int _tickCount;

		private BrushListPanel _tutorialPanel;

		public enum AnimState
		{
			Idle,
			Start,
			Starting,
			Playing
		}
	}
}
