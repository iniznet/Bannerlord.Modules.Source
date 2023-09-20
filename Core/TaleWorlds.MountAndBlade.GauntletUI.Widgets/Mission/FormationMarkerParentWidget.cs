using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	public class FormationMarkerParentWidget : Widget
	{
		public float FarAlphaTarget { get; set; } = 0.2f;

		public float FarDistanceCutoff { get; set; } = 50f;

		public float CloseDistanceCutoff { get; set; } = 25f;

		public float ClosestFadeoutRange { get; set; } = 3f;

		public FormationMarkerParentWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			MathF.Clamp(dt * 12f, 0f, 1f);
			if (this._isMarkersDirty)
			{
				Sprite sprite = base.Context.SpriteData.GetSprite("General\\compass\\" + this.MarkerType);
				if (sprite != null)
				{
					this.FormationTypeMarker.Sprite = sprite;
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

		private bool _isMarkersDirty = true;

		private string _markerType;

		private int _teamType;

		private Widget _formationTypeMarker;

		private Widget _teamTypeMarker;
	}
}
