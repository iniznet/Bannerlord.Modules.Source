using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tournament
{
	public class TournamentParticipantBrushWidget : BrushWidget
	{
		public TournamentParticipantBrushWidget(UIContext context)
			: base(context)
		{
			base.AddState("Current");
			base.AddState("Over");
			base.AddState("Dead");
		}

		protected override void OnMousePressed()
		{
			base.OnMousePressed();
			base.EventFired("ClickEvent", Array.Empty<object>());
		}

		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.AddState("Current");
			child.AddState("Over");
			child.AddState("Dead");
		}

		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			base.ParentWidget.AddState("Current");
			base.ParentWidget.AddState("Over");
			base.ParentWidget.AddState("Dead");
		}

		private void SetWidgetState(string state)
		{
			base.ParentWidget.SetState(state);
			this.SetState(state);
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._brushApplied)
			{
				this.NameTextWidget.Brush = (this.IsMainHero ? this.MainHeroTextBrush : this.NormalTextBrush);
				this._brushApplied = true;
			}
			if (this._stateChanged && base.ReadOnlyBrush != null && base.BrushRenderer.Brush != null)
			{
				this._stateChanged = false;
				this.SetWidgetState("Default");
				foreach (BrushLayer brushLayer in base.Brush.Layers)
				{
					brushLayer.Color = base.Brush.Color;
				}
				if (this.OnMission)
				{
					base.Brush.GlobalAlphaFactor = 0.75f;
				}
				else
				{
					base.Brush.GlobalAlphaFactor = 1f;
				}
				if (this.MatchState == 0)
				{
					this.SetWidgetState("Default");
					return;
				}
				if (this.MatchState == 1)
				{
					this.SetWidgetState("Current");
					return;
				}
				if (this.MatchState == 2)
				{
					this.SetWidgetState("Over");
					return;
				}
				if (this.MatchState == 3)
				{
					if (this._isDead && this.OnMission)
					{
						this.SetWidgetState("Dead");
						return;
					}
					this.SetWidgetState("Default");
				}
			}
		}

		public TextWidget NameTextWidget
		{
			get
			{
				return this._nameTextWidget;
			}
			set
			{
				if (this._nameTextWidget != value)
				{
					this._nameTextWidget = value;
				}
			}
		}

		public int MatchState
		{
			get
			{
				return this._matchState;
			}
			set
			{
				if (this._matchState != value)
				{
					this._stateChanged = true;
					this._matchState = value;
				}
			}
		}

		public bool IsDead
		{
			get
			{
				return this._isDead;
			}
			set
			{
				if (this._isDead != value)
				{
					this._stateChanged = true;
					this._isDead = value;
				}
			}
		}

		public bool IsMainHero
		{
			get
			{
				return this._isMainHero;
			}
			set
			{
				if (this._isMainHero != value)
				{
					this._isMainHero = value;
					this._brushApplied = false;
				}
			}
		}

		public Brush MainHeroTextBrush
		{
			get
			{
				return this._mainHeroTextBrush;
			}
			set
			{
				if (this._mainHeroTextBrush != value)
				{
					this._mainHeroTextBrush = value;
				}
			}
		}

		public Brush NormalTextBrush
		{
			get
			{
				return this._normalTextBrush;
			}
			set
			{
				if (this._normalTextBrush != value)
				{
					this._normalTextBrush = value;
				}
			}
		}

		public bool OnMission
		{
			get
			{
				return this._onMission;
			}
			set
			{
				if (this._onMission != value)
				{
					this._stateChanged = true;
					this._onMission = value;
				}
			}
		}

		private bool _stateChanged;

		private bool _brushApplied;

		private int _matchState;

		private bool _isDead;

		private bool _onMission;

		private bool _isMainHero;

		private Brush _mainHeroTextBrush;

		private Brush _normalTextBrush;

		private TextWidget _nameTextWidget;
	}
}
