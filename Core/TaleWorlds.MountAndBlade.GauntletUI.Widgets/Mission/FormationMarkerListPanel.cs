﻿using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	public class FormationMarkerListPanel : ListPanel
	{
		public float FarAlphaTarget { get; set; } = 0.2f;

		public float FarDistanceCutoff { get; set; } = 50f;

		public float CloseDistanceCutoff { get; set; } = 25f;

		public float ClosestFadeoutRange { get; set; } = 3f;

		public FormationMarkerListPanel(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			float num = MathF.Clamp(dt * 12f, 0f, 1f);
			if (this._isMarkersDirty)
			{
				Sprite sprite = base.Context.SpriteData.GetSprite("General\\compass\\" + this.MarkerType);
				if (sprite != null)
				{
					this.FormationTypeMarker.Sprite = sprite;
				}
				else
				{
					Debug.FailedAssert("Couldn't find formation marker type image", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Mission\\FormationMarkerListPanel.cs", "OnLateUpdate", 43);
				}
				this.TeamTypeMarker.RegisterBrushStatesOfWidget();
				if (this.TeamType == 0)
				{
					this.TeamTypeMarker.SetState("Player");
				}
				else if (this.TeamType == 1)
				{
					this.TeamTypeMarker.SetState("Ally");
				}
				else
				{
					this.TeamTypeMarker.SetState("Enemy");
				}
				this._isMarkersDirty = false;
			}
			if (this.IsMarkerEnabled)
			{
				float distanceRelatedAlphaTarget = this.GetDistanceRelatedAlphaTarget(this.Distance);
				this.SetGlobalAlphaRecursively(distanceRelatedAlphaTarget);
				base.IsVisible = (double)distanceRelatedAlphaTarget > 0.05;
			}
			else
			{
				float num2 = this.LocalLerp(base.AlphaFactor, 0f, num);
				this.SetGlobalAlphaRecursively(num2);
				base.IsVisible = (double)base.AlphaFactor > 0.05;
			}
			this.UpdateVisibility();
			if (base.IsVisible)
			{
				this.UpdateScreenPosition();
				this.DistanceTextWidget.Text = ((int)this.Distance).ToString();
			}
		}

		private void UpdateVisibility()
		{
			base.IsVisible = this.IsInsideScreenBoundaries || this.IsTargetingAFormation;
		}

		private void UpdateScreenPosition()
		{
			float num = this.Position.X - base.Size.X / 2f;
			float num2 = this.Position.Y - base.Size.Y / 2f;
			if (this.WSign > 0 && num - base.Size.X / 2f > 0f && num + base.Size.X / 2f < base.Context.EventManager.PageSize.X && num2 > 0f && num2 + base.Size.Y < base.Context.EventManager.PageSize.Y)
			{
				base.IsVisible = true;
				base.ScaledPositionXOffset = num;
				base.ScaledPositionYOffset = num2;
				return;
			}
			if (this.IsTargetingAFormation)
			{
				base.IsVisible = true;
				Vec2 vec = this.Position;
				Vector2 pageSize = base.Context.EventManager.PageSize;
				Vec2 vec2 = new Vec2(base.Context.EventManager.PageSize.X / 2f, base.Context.EventManager.PageSize.Y / 2f);
				vec -= vec2;
				if (this.WSign < 0)
				{
					vec *= -1f;
				}
				float num3 = Mathf.Atan2(vec.y, vec.x) - 1.5707964f;
				float num4 = Mathf.Cos(num3);
				float num5 = Mathf.Sin(num3);
				float num6 = num4 / num5;
				Vec2 vec3 = vec2 * 1f;
				vec = ((num4 > 0f) ? new Vec2(-vec3.y / num6, vec2.y) : new Vec2(vec3.y / num6, -vec2.y));
				if (vec.x > vec3.x)
				{
					vec = new Vec2(vec3.x, -vec3.x * num6);
				}
				else if (vec.x < -vec3.x)
				{
					vec = new Vec2(-vec3.x, vec3.x * num6);
				}
				vec += vec2;
				base.ScaledPositionXOffset = Mathf.Clamp(vec.x - base.Size.X / 2f, 0f, pageSize.X - base.Size.X);
				base.ScaledPositionYOffset = Mathf.Clamp(vec.y - base.Size.Y, 0f, pageSize.Y - base.Size.Y);
				return;
			}
			base.IsVisible = false;
		}

		private float GetDistanceRelatedAlphaTarget(float distance)
		{
			if (distance > this.FarDistanceCutoff)
			{
				return this.FarAlphaTarget;
			}
			if (distance <= this.FarDistanceCutoff && distance >= this.CloseDistanceCutoff)
			{
				float num = (float)Math.Pow((double)((distance - this.CloseDistanceCutoff) / (this.FarDistanceCutoff - this.CloseDistanceCutoff)), 0.3333333333333333);
				return MathF.Clamp(MathF.Lerp(1f, this.FarAlphaTarget, num, 1E-05f), this.FarAlphaTarget, 1f);
			}
			if (distance < this.CloseDistanceCutoff && distance > this.CloseDistanceCutoff - this.ClosestFadeoutRange)
			{
				float num2 = (distance - (this.CloseDistanceCutoff - this.ClosestFadeoutRange)) / this.ClosestFadeoutRange;
				return MathF.Lerp(0f, 1f, num2, 1E-05f);
			}
			return 0f;
		}

		private float LocalLerp(float start, float end, float delta)
		{
			if (Math.Abs(start - end) > 1E-45f)
			{
				return (end - start) * delta + start;
			}
			return end;
		}

		[DataSourceProperty]
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
					base.OnPropertyChanged<TextWidget>(value, "NameTextWidget");
				}
			}
		}

		[DataSourceProperty]
		public TextWidget DistanceTextWidget
		{
			get
			{
				return this._distanceTextWidget;
			}
			set
			{
				if (this._distanceTextWidget != value)
				{
					this._distanceTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "DistanceTextWidget");
				}
			}
		}

		[DataSourceProperty]
		public Widget FormationTypeMarker
		{
			get
			{
				return this._formationTypeMarker;
			}
			set
			{
				if (this._formationTypeMarker != value)
				{
					this._formationTypeMarker = value;
					base.OnPropertyChanged<Widget>(value, "FormationTypeMarker");
					this._isMarkersDirty = true;
				}
			}
		}

		[DataSourceProperty]
		public Widget TeamTypeMarker
		{
			get
			{
				return this._teamTypeMarker;
			}
			set
			{
				if (this._teamTypeMarker != value)
				{
					this._teamTypeMarker = value;
					base.OnPropertyChanged<Widget>(value, "TeamTypeMarker");
					this._isMarkersDirty = true;
				}
			}
		}

		[DataSourceProperty]
		public Vec2 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (this._position != value)
				{
					this._position = value;
					base.OnPropertyChanged(value, "Position");
				}
			}
		}

		[DataSourceProperty]
		public float Distance
		{
			get
			{
				return this._distance;
			}
			set
			{
				if (this._distance != value)
				{
					this._distance = value;
					base.OnPropertyChanged(value, "Distance");
				}
			}
		}

		[DataSourceProperty]
		public int TeamType
		{
			get
			{
				return this._teamType;
			}
			set
			{
				if (this._teamType != value)
				{
					this._teamType = value;
					base.OnPropertyChanged(value, "TeamType");
					this._isMarkersDirty = true;
				}
			}
		}

		[DataSourceProperty]
		public int WSign
		{
			get
			{
				return this._wSign;
			}
			set
			{
				if (this._teamType != value)
				{
					this._wSign = value;
					base.OnPropertyChanged(value, "WSign");
				}
			}
		}

		[DataSourceProperty]
		public string MarkerType
		{
			get
			{
				return this._markerType;
			}
			set
			{
				if (this._markerType != value)
				{
					this._markerType = value;
					base.OnPropertyChanged<string>(value, "MarkerType");
					this._isMarkersDirty = true;
				}
			}
		}

		[DataSourceProperty]
		public bool IsMarkerEnabled
		{
			get
			{
				return this._isMarkerEnabled;
			}
			set
			{
				if (this._isMarkerEnabled != value)
				{
					this._isMarkerEnabled = value;
					base.OnPropertyChanged(value, "IsMarkerEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsTargetingAFormation
		{
			get
			{
				return this._isTargetingAFormation;
			}
			set
			{
				if (this._isTargetingAFormation != value)
				{
					this._isTargetingAFormation = value;
					base.OnPropertyChanged(value, "IsTargetingAFormation");
				}
			}
		}

		[DataSourceProperty]
		public bool IsInsideScreenBoundaries
		{
			get
			{
				return this._isInsideScreenBoundaries;
			}
			set
			{
				if (this._isInsideScreenBoundaries != value)
				{
					this._isInsideScreenBoundaries = value;
					base.OnPropertyChanged(value, "IsInsideScreenBoundaries");
				}
			}
		}

		private bool _isMarkersDirty = true;

		private float _distance;

		private TextWidget _nameTextWidget;

		private TextWidget _distanceTextWidget;

		private Vec2 _position;

		private bool _isMarkerEnabled;

		private bool _isTargetingAFormation;

		private bool _isInsideScreenBoundaries;

		private string _markerType;

		private int _teamType;

		private int _wSign;

		private Widget _formationTypeMarker;

		private Widget _teamTypeMarker;
	}
}
