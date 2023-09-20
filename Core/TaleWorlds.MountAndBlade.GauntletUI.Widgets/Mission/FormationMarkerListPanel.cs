using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	// Token: 0x020000C9 RID: 201
	public class FormationMarkerListPanel : ListPanel
	{
		// Token: 0x17000390 RID: 912
		// (get) Token: 0x06000A2B RID: 2603 RVA: 0x0001CE96 File Offset: 0x0001B096
		// (set) Token: 0x06000A2C RID: 2604 RVA: 0x0001CE9E File Offset: 0x0001B09E
		public float FarAlphaTarget { get; set; } = 0.2f;

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x06000A2D RID: 2605 RVA: 0x0001CEA7 File Offset: 0x0001B0A7
		// (set) Token: 0x06000A2E RID: 2606 RVA: 0x0001CEAF File Offset: 0x0001B0AF
		public float FarDistanceCutoff { get; set; } = 50f;

		// Token: 0x17000392 RID: 914
		// (get) Token: 0x06000A2F RID: 2607 RVA: 0x0001CEB8 File Offset: 0x0001B0B8
		// (set) Token: 0x06000A30 RID: 2608 RVA: 0x0001CEC0 File Offset: 0x0001B0C0
		public float CloseDistanceCutoff { get; set; } = 25f;

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06000A31 RID: 2609 RVA: 0x0001CEC9 File Offset: 0x0001B0C9
		// (set) Token: 0x06000A32 RID: 2610 RVA: 0x0001CED1 File Offset: 0x0001B0D1
		public float ClosestFadeoutRange { get; set; } = 3f;

		// Token: 0x06000A33 RID: 2611 RVA: 0x0001CEDA File Offset: 0x0001B0DA
		public FormationMarkerListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000A34 RID: 2612 RVA: 0x0001CF18 File Offset: 0x0001B118
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
			if (base.IsVisible)
			{
				this.DistanceTextWidget.Text = ((int)this.Distance).ToString();
			}
			base.ScaledPositionYOffset = this.Position.y - base.Size.Y / 2f;
			base.ScaledPositionXOffset = this.Position.x - base.Size.X / 2f;
		}

		// Token: 0x06000A35 RID: 2613 RVA: 0x0001D0BC File Offset: 0x0001B2BC
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

		// Token: 0x06000A36 RID: 2614 RVA: 0x0001D184 File Offset: 0x0001B384
		private float LocalLerp(float start, float end, float delta)
		{
			if (Math.Abs(start - end) > 1E-45f)
			{
				return (end - start) * delta + start;
			}
			return end;
		}

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x06000A37 RID: 2615 RVA: 0x0001D19E File Offset: 0x0001B39E
		// (set) Token: 0x06000A38 RID: 2616 RVA: 0x0001D1A6 File Offset: 0x0001B3A6
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

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x06000A39 RID: 2617 RVA: 0x0001D1C4 File Offset: 0x0001B3C4
		// (set) Token: 0x06000A3A RID: 2618 RVA: 0x0001D1CC File Offset: 0x0001B3CC
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

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x06000A3B RID: 2619 RVA: 0x0001D1EA File Offset: 0x0001B3EA
		// (set) Token: 0x06000A3C RID: 2620 RVA: 0x0001D1F2 File Offset: 0x0001B3F2
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

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x06000A3D RID: 2621 RVA: 0x0001D217 File Offset: 0x0001B417
		// (set) Token: 0x06000A3E RID: 2622 RVA: 0x0001D21F File Offset: 0x0001B41F
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

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x06000A3F RID: 2623 RVA: 0x0001D244 File Offset: 0x0001B444
		// (set) Token: 0x06000A40 RID: 2624 RVA: 0x0001D24C File Offset: 0x0001B44C
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

		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06000A41 RID: 2625 RVA: 0x0001D26F File Offset: 0x0001B46F
		// (set) Token: 0x06000A42 RID: 2626 RVA: 0x0001D277 File Offset: 0x0001B477
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

		// Token: 0x1700039A RID: 922
		// (get) Token: 0x06000A43 RID: 2627 RVA: 0x0001D295 File Offset: 0x0001B495
		// (set) Token: 0x06000A44 RID: 2628 RVA: 0x0001D29D File Offset: 0x0001B49D
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

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06000A45 RID: 2629 RVA: 0x0001D2C2 File Offset: 0x0001B4C2
		// (set) Token: 0x06000A46 RID: 2630 RVA: 0x0001D2CA File Offset: 0x0001B4CA
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

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06000A47 RID: 2631 RVA: 0x0001D2F4 File Offset: 0x0001B4F4
		// (set) Token: 0x06000A48 RID: 2632 RVA: 0x0001D2FC File Offset: 0x0001B4FC
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
				}
			}
		}

		// Token: 0x040004A5 RID: 1189
		private bool _isMarkersDirty = true;

		// Token: 0x040004A6 RID: 1190
		private float _distance;

		// Token: 0x040004A7 RID: 1191
		private TextWidget _nameTextWidget;

		// Token: 0x040004A8 RID: 1192
		private TextWidget _distanceTextWidget;

		// Token: 0x040004A9 RID: 1193
		private Vec2 _position;

		// Token: 0x040004AA RID: 1194
		private bool _isMarkerEnabled;

		// Token: 0x040004AB RID: 1195
		private string _markerType;

		// Token: 0x040004AC RID: 1196
		private int _teamType;

		// Token: 0x040004AD RID: 1197
		private Widget _formationTypeMarker;

		// Token: 0x040004AE RID: 1198
		private Widget _teamTypeMarker;
	}
}
