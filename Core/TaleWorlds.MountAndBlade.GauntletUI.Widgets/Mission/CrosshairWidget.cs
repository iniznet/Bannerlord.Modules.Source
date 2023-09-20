using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	public class CrosshairWidget : Widget
	{
		public CrosshairWidget(UIContext context)
			: base(context)
		{
		}

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

		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.AddState("Invalid");
		}

		private void HitMarkerUpdated()
		{
			if (this.HitMarker != null)
			{
				this.HitMarker.AddState("Show");
			}
		}

		private void HeadshotMarkerUpdated()
		{
			if (this.HeadshotMarker != null)
			{
				this.HitMarker.AddState("Show");
			}
		}

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

		private double _crosshairAccuracy;

		private double _crosshairScale;

		private bool _isTargetInvalid;

		private double _topArrowOpacity;

		private double _bottomArrowOpacity;

		private double _rightArrowOpacity;

		private double _leftArrowOpacity;

		private bool _isVictimDead;

		private bool _showHitMarker;

		private bool _isHumanoidHeadshot;

		private BrushWidget _leftArrow;

		private BrushWidget _rightArrow;

		private BrushWidget _topArrow;

		private BrushWidget _bottomArrow;

		private BrushWidget _hitMarker;

		private BrushWidget _headshotMarker;
	}
}
