using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	// Token: 0x020000C8 RID: 200
	public class CrosshairWidget : Widget
	{
		// Token: 0x06000A04 RID: 2564 RVA: 0x0001C9FD File Offset: 0x0001ABFD
		public CrosshairWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000A05 RID: 2565 RVA: 0x0001CA08 File Offset: 0x0001AC08
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (base.IsVisible)
			{
				base.SuggestedWidth = (float)((int)(74.0 + this.CrosshairAccuracy * 300.0));
				base.SuggestedHeight = (float)((int)(74.0 + this.CrosshairAccuracy * 300.0));
			}
			this.LeftArrow.Brush.AlphaFactor = (float)this.LeftArrowOpacity;
			this.RightArrow.Brush.AlphaFactor = (float)this.RightArrowOpacity;
			this.TopArrow.Brush.AlphaFactor = (float)this.TopArrowOpacity;
			this.BottomArrow.Brush.AlphaFactor = (float)this.BottomArrowOpacity;
		}

		// Token: 0x06000A06 RID: 2566 RVA: 0x0001CAC4 File Offset: 0x0001ACC4
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.AddState("Invalid");
		}

		// Token: 0x06000A07 RID: 2567 RVA: 0x0001CAD8 File Offset: 0x0001ACD8
		private void HitMarkerUpdated()
		{
			if (this.HitMarker != null)
			{
				this.HitMarker.AddState("Show");
			}
		}

		// Token: 0x06000A08 RID: 2568 RVA: 0x0001CAF2 File Offset: 0x0001ACF2
		private void HeadshotMarkerUpdated()
		{
			if (this.HeadshotMarker != null)
			{
				this.HitMarker.AddState("Show");
			}
		}

		// Token: 0x06000A09 RID: 2569 RVA: 0x0001CB0C File Offset: 0x0001AD0C
		private void ShowHitMarkerChanged()
		{
			if (this.HitMarker == null)
			{
				return;
			}
			string text = (this.IsVictimDead ? "ShowDeath" : "Show");
			if (this.HitMarker.CurrentState != text)
			{
				this.HitMarker.SetState(text);
				return;
			}
			this.HitMarker.BrushRenderer.RestartAnimation();
		}

		// Token: 0x06000A0A RID: 2570 RVA: 0x0001CB68 File Offset: 0x0001AD68
		private void ShowHeadshotMarkerChanged()
		{
			if (this.HeadshotMarker == null)
			{
				return;
			}
			string text = (this.IsHumanoidHeadshot ? "Show" : "Default");
			if (this.HeadshotMarker.CurrentState != text)
			{
				this.HeadshotMarker.SetState(text);
			}
			this.HeadshotMarker.BrushRenderer.RestartAnimation();
		}

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x06000A0B RID: 2571 RVA: 0x0001CBC2 File Offset: 0x0001ADC2
		// (set) Token: 0x06000A0C RID: 2572 RVA: 0x0001CBCA File Offset: 0x0001ADCA
		[Editor(false)]
		public double TopArrowOpacity
		{
			get
			{
				return this._topArrowOpacity;
			}
			set
			{
				if (value != this._topArrowOpacity)
				{
					this._topArrowOpacity = value;
					base.OnPropertyChanged(value, "TopArrowOpacity");
				}
			}
		}

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x06000A0D RID: 2573 RVA: 0x0001CBE8 File Offset: 0x0001ADE8
		// (set) Token: 0x06000A0E RID: 2574 RVA: 0x0001CBF0 File Offset: 0x0001ADF0
		[Editor(false)]
		public double BottomArrowOpacity
		{
			get
			{
				return this._bottomArrowOpacity;
			}
			set
			{
				if (value != this._bottomArrowOpacity)
				{
					this._bottomArrowOpacity = value;
					base.OnPropertyChanged(value, "BottomArrowOpacity");
				}
			}
		}

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x06000A0F RID: 2575 RVA: 0x0001CC0E File Offset: 0x0001AE0E
		// (set) Token: 0x06000A10 RID: 2576 RVA: 0x0001CC16 File Offset: 0x0001AE16
		[Editor(false)]
		public double RightArrowOpacity
		{
			get
			{
				return this._rightArrowOpacity;
			}
			set
			{
				if (value != this._rightArrowOpacity)
				{
					this._rightArrowOpacity = value;
					base.OnPropertyChanged(value, "RightArrowOpacity");
				}
			}
		}

		// Token: 0x17000383 RID: 899
		// (get) Token: 0x06000A11 RID: 2577 RVA: 0x0001CC34 File Offset: 0x0001AE34
		// (set) Token: 0x06000A12 RID: 2578 RVA: 0x0001CC3C File Offset: 0x0001AE3C
		[Editor(false)]
		public double LeftArrowOpacity
		{
			get
			{
				return this._leftArrowOpacity;
			}
			set
			{
				if (value != this._leftArrowOpacity)
				{
					this._leftArrowOpacity = value;
					base.OnPropertyChanged(value, "LeftArrowOpacity");
				}
			}
		}

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x06000A13 RID: 2579 RVA: 0x0001CC5A File Offset: 0x0001AE5A
		// (set) Token: 0x06000A14 RID: 2580 RVA: 0x0001CC64 File Offset: 0x0001AE64
		[Editor(false)]
		public bool IsTargetInvalid
		{
			get
			{
				return this._isTargetInvalid;
			}
			set
			{
				if (value != this._isTargetInvalid)
				{
					this._isTargetInvalid = value;
					base.OnPropertyChanged(value, "IsTargetInvalid");
					foreach (Widget widget in base.AllChildren)
					{
						widget.SetState(value ? "Invalid" : "Default");
					}
				}
			}
		}

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x06000A15 RID: 2581 RVA: 0x0001CCDC File Offset: 0x0001AEDC
		// (set) Token: 0x06000A16 RID: 2582 RVA: 0x0001CCE4 File Offset: 0x0001AEE4
		[Editor(false)]
		public double CrosshairAccuracy
		{
			get
			{
				return this._crosshairAccuracy;
			}
			set
			{
				if (value != this._crosshairAccuracy)
				{
					this._crosshairAccuracy = value;
					base.OnPropertyChanged(value, "CrosshairAccuracy");
				}
			}
		}

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x06000A17 RID: 2583 RVA: 0x0001CD02 File Offset: 0x0001AF02
		// (set) Token: 0x06000A18 RID: 2584 RVA: 0x0001CD0A File Offset: 0x0001AF0A
		[Editor(false)]
		public double CrosshairScale
		{
			get
			{
				return this._crosshairScale;
			}
			set
			{
				if (value != this._crosshairScale)
				{
					this._crosshairScale = value;
					base.OnPropertyChanged(value, "CrosshairScale");
				}
			}
		}

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x06000A19 RID: 2585 RVA: 0x0001CD28 File Offset: 0x0001AF28
		// (set) Token: 0x06000A1A RID: 2586 RVA: 0x0001CD30 File Offset: 0x0001AF30
		[Editor(false)]
		public bool IsVictimDead
		{
			get
			{
				return this._isVictimDead;
			}
			set
			{
				if (value != this._isVictimDead)
				{
					this._isVictimDead = value;
					base.OnPropertyChanged(value, "IsVictimDead");
				}
			}
		}

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x06000A1B RID: 2587 RVA: 0x0001CD4E File Offset: 0x0001AF4E
		// (set) Token: 0x06000A1C RID: 2588 RVA: 0x0001CD56 File Offset: 0x0001AF56
		[Editor(false)]
		public bool IsHumanoidHeadshot
		{
			get
			{
				return this._isHumanoidHeadshot;
			}
			set
			{
				if (value != this._isHumanoidHeadshot)
				{
					this._isHumanoidHeadshot = value;
					base.OnPropertyChanged(value, "IsHumanoidHeadshot");
					this.ShowHeadshotMarkerChanged();
				}
			}
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x06000A1D RID: 2589 RVA: 0x0001CD7A File Offset: 0x0001AF7A
		// (set) Token: 0x06000A1E RID: 2590 RVA: 0x0001CD82 File Offset: 0x0001AF82
		[Editor(false)]
		public bool ShowHitMarker
		{
			get
			{
				return this._showHitMarker;
			}
			set
			{
				if (value != this._showHitMarker)
				{
					this._showHitMarker = value;
					base.OnPropertyChanged(value, "ShowHitMarker");
					this.ShowHitMarkerChanged();
				}
			}
		}

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06000A1F RID: 2591 RVA: 0x0001CDA6 File Offset: 0x0001AFA6
		// (set) Token: 0x06000A20 RID: 2592 RVA: 0x0001CDAE File Offset: 0x0001AFAE
		[Editor(false)]
		public BrushWidget LeftArrow
		{
			get
			{
				return this._leftArrow;
			}
			set
			{
				if (value != this._leftArrow)
				{
					this._leftArrow = value;
					base.OnPropertyChanged<BrushWidget>(value, "LeftArrow");
				}
			}
		}

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x06000A21 RID: 2593 RVA: 0x0001CDCC File Offset: 0x0001AFCC
		// (set) Token: 0x06000A22 RID: 2594 RVA: 0x0001CDD4 File Offset: 0x0001AFD4
		[Editor(false)]
		public BrushWidget RightArrow
		{
			get
			{
				return this._rightArrow;
			}
			set
			{
				if (value != this._rightArrow)
				{
					this._rightArrow = value;
					base.OnPropertyChanged<BrushWidget>(value, "RightArrow");
				}
			}
		}

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x06000A23 RID: 2595 RVA: 0x0001CDF2 File Offset: 0x0001AFF2
		// (set) Token: 0x06000A24 RID: 2596 RVA: 0x0001CDFA File Offset: 0x0001AFFA
		[Editor(false)]
		public BrushWidget TopArrow
		{
			get
			{
				return this._topArrow;
			}
			set
			{
				if (value != this._topArrow)
				{
					this._topArrow = value;
					base.OnPropertyChanged<BrushWidget>(value, "TopArrow");
				}
			}
		}

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x06000A25 RID: 2597 RVA: 0x0001CE18 File Offset: 0x0001B018
		// (set) Token: 0x06000A26 RID: 2598 RVA: 0x0001CE20 File Offset: 0x0001B020
		[Editor(false)]
		public BrushWidget BottomArrow
		{
			get
			{
				return this._bottomArrow;
			}
			set
			{
				if (value != this._bottomArrow)
				{
					this._bottomArrow = value;
					base.OnPropertyChanged<BrushWidget>(value, "BottomArrow");
				}
			}
		}

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x06000A27 RID: 2599 RVA: 0x0001CE3E File Offset: 0x0001B03E
		// (set) Token: 0x06000A28 RID: 2600 RVA: 0x0001CE46 File Offset: 0x0001B046
		[Editor(false)]
		public BrushWidget HitMarker
		{
			get
			{
				return this._hitMarker;
			}
			set
			{
				if (value != this._hitMarker)
				{
					this._hitMarker = value;
					base.OnPropertyChanged<BrushWidget>(value, "HitMarker");
					this.HitMarkerUpdated();
				}
			}
		}

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x06000A29 RID: 2601 RVA: 0x0001CE6A File Offset: 0x0001B06A
		// (set) Token: 0x06000A2A RID: 2602 RVA: 0x0001CE72 File Offset: 0x0001B072
		[Editor(false)]
		public BrushWidget HeadshotMarker
		{
			get
			{
				return this._headshotMarker;
			}
			set
			{
				if (value != this._headshotMarker)
				{
					this._headshotMarker = value;
					base.OnPropertyChanged<BrushWidget>(value, "HeadshotMarker");
					this.HeadshotMarkerUpdated();
				}
			}
		}

		// Token: 0x04000491 RID: 1169
		private double _crosshairAccuracy;

		// Token: 0x04000492 RID: 1170
		private double _crosshairScale;

		// Token: 0x04000493 RID: 1171
		private bool _isTargetInvalid;

		// Token: 0x04000494 RID: 1172
		private double _topArrowOpacity;

		// Token: 0x04000495 RID: 1173
		private double _bottomArrowOpacity;

		// Token: 0x04000496 RID: 1174
		private double _rightArrowOpacity;

		// Token: 0x04000497 RID: 1175
		private double _leftArrowOpacity;

		// Token: 0x04000498 RID: 1176
		private bool _isVictimDead;

		// Token: 0x04000499 RID: 1177
		private bool _showHitMarker;

		// Token: 0x0400049A RID: 1178
		private bool _isHumanoidHeadshot;

		// Token: 0x0400049B RID: 1179
		private BrushWidget _leftArrow;

		// Token: 0x0400049C RID: 1180
		private BrushWidget _rightArrow;

		// Token: 0x0400049D RID: 1181
		private BrushWidget _topArrow;

		// Token: 0x0400049E RID: 1182
		private BrushWidget _bottomArrow;

		// Token: 0x0400049F RID: 1183
		private BrushWidget _hitMarker;

		// Token: 0x040004A0 RID: 1184
		private BrushWidget _headshotMarker;
	}
}
