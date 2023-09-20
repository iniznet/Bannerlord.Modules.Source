using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tournament
{
	// Token: 0x02000047 RID: 71
	public class TournamentParticipantBrushWidget : BrushWidget
	{
		// Token: 0x060003B8 RID: 952 RVA: 0x0000C674 File Offset: 0x0000A874
		public TournamentParticipantBrushWidget(UIContext context)
			: base(context)
		{
			base.AddState("Current");
			base.AddState("Over");
			base.AddState("Dead");
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x0000C69E File Offset: 0x0000A89E
		protected override void OnMousePressed()
		{
			base.OnMousePressed();
			base.EventFired("ClickEvent", Array.Empty<object>());
		}

		// Token: 0x060003BA RID: 954 RVA: 0x0000C6B6 File Offset: 0x0000A8B6
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.AddState("Current");
			child.AddState("Over");
			child.AddState("Dead");
		}

		// Token: 0x060003BB RID: 955 RVA: 0x0000C6E0 File Offset: 0x0000A8E0
		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
			base.ParentWidget.AddState("Current");
			base.ParentWidget.AddState("Over");
			base.ParentWidget.AddState("Dead");
		}

		// Token: 0x060003BC RID: 956 RVA: 0x0000C718 File Offset: 0x0000A918
		private void SetWidgetState(string state)
		{
			base.ParentWidget.SetState(state);
			this.SetState(state);
		}

		// Token: 0x060003BD RID: 957 RVA: 0x0000C730 File Offset: 0x0000A930
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

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x060003BE RID: 958 RVA: 0x0000C898 File Offset: 0x0000AA98
		// (set) Token: 0x060003BF RID: 959 RVA: 0x0000C8A0 File Offset: 0x0000AAA0
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

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x060003C0 RID: 960 RVA: 0x0000C8B2 File Offset: 0x0000AAB2
		// (set) Token: 0x060003C1 RID: 961 RVA: 0x0000C8BA File Offset: 0x0000AABA
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

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x060003C2 RID: 962 RVA: 0x0000C8D3 File Offset: 0x0000AAD3
		// (set) Token: 0x060003C3 RID: 963 RVA: 0x0000C8DB File Offset: 0x0000AADB
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

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x060003C4 RID: 964 RVA: 0x0000C8F4 File Offset: 0x0000AAF4
		// (set) Token: 0x060003C5 RID: 965 RVA: 0x0000C8FC File Offset: 0x0000AAFC
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

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x060003C6 RID: 966 RVA: 0x0000C915 File Offset: 0x0000AB15
		// (set) Token: 0x060003C7 RID: 967 RVA: 0x0000C91D File Offset: 0x0000AB1D
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

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x060003C8 RID: 968 RVA: 0x0000C92F File Offset: 0x0000AB2F
		// (set) Token: 0x060003C9 RID: 969 RVA: 0x0000C937 File Offset: 0x0000AB37
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

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x060003CA RID: 970 RVA: 0x0000C949 File Offset: 0x0000AB49
		// (set) Token: 0x060003CB RID: 971 RVA: 0x0000C951 File Offset: 0x0000AB51
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

		// Token: 0x0400019C RID: 412
		private bool _stateChanged;

		// Token: 0x0400019D RID: 413
		private bool _brushApplied;

		// Token: 0x0400019E RID: 414
		private int _matchState;

		// Token: 0x0400019F RID: 415
		private bool _isDead;

		// Token: 0x040001A0 RID: 416
		private bool _onMission;

		// Token: 0x040001A1 RID: 417
		private bool _isMainHero;

		// Token: 0x040001A2 RID: 418
		private Brush _mainHeroTextBrush;

		// Token: 0x040001A3 RID: 419
		private Brush _normalTextBrush;

		// Token: 0x040001A4 RID: 420
		private TextWidget _nameTextWidget;
	}
}
