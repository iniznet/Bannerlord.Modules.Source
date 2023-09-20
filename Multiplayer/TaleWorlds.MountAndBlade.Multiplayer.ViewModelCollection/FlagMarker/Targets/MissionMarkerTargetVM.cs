using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.FlagMarker.Targets
{
	public abstract class MissionMarkerTargetVM : ViewModel
	{
		public abstract Vec3 WorldPosition { get; }

		protected abstract float HeightOffset { get; }

		public MissionMarkerTargetVM(MissionMarkerType markerType)
		{
			this.MissionMarkerType = markerType;
			this.MarkerType = (int)markerType;
		}

		public virtual void UpdateScreenPosition(Camera missionCamera)
		{
			float num = -100f;
			float num2 = -100f;
			float num3 = 0f;
			Vec3 worldPosition = this.WorldPosition;
			worldPosition.z += this.HeightOffset;
			MBWindowManager.WorldToScreenInsideUsableArea(missionCamera, worldPosition, ref num, ref num2, ref num3);
			if (num3 > 0f)
			{
				this.ScreenPosition = new Vec2(num, num2);
				this.Distance = (int)(this.WorldPosition - missionCamera.Position).Length;
				return;
			}
			this.Distance = -1;
			this.ScreenPosition = new Vec2(-100f, -100f);
		}

		protected void RefreshColor(uint color, uint color2)
		{
			if (color != 0U)
			{
				string text = color.ToString("X");
				char c = text[0];
				char c2 = text[1];
				text = text.Remove(0, 2);
				text = Extensions.Add(text, c.ToString() + c2.ToString(), false);
				this.Color = "#" + text;
			}
			else
			{
				this.Color = "#FFFFFFFF";
			}
			if (color2 != 0U)
			{
				string text2 = color2.ToString("X");
				char c3 = text2[0];
				char c4 = text2[1];
				text2 = text2.Remove(0, 2);
				text2 = Extensions.Add(text2, c3.ToString() + c4.ToString(), false);
				this.Color2 = "#" + text2;
				return;
			}
			this.Color2 = "#FFFFFFFF";
		}

		[DataSourceProperty]
		public Vec2 ScreenPosition
		{
			get
			{
				return this._screenPosition;
			}
			set
			{
				if (value.x != this._screenPosition.x || value.y != this._screenPosition.y)
				{
					this._screenPosition = value;
					base.OnPropertyChangedWithValue(value, "ScreenPosition");
				}
			}
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public int Distance
		{
			get
			{
				return this._distance;
			}
			set
			{
				if (value != this._distance)
				{
					this._distance = value;
					base.OnPropertyChangedWithValue(value, "Distance");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public string Color
		{
			get
			{
				return this._color;
			}
			set
			{
				if (value != this._color)
				{
					this._color = value;
					base.OnPropertyChangedWithValue<string>(value, "Color");
				}
			}
		}

		[DataSourceProperty]
		public string Color2
		{
			get
			{
				return this._color2;
			}
			set
			{
				if (value != this._color2)
				{
					this._color2 = value;
					base.OnPropertyChangedWithValue<string>(value, "Color2");
				}
			}
		}

		[DataSourceProperty]
		public int MarkerType
		{
			get
			{
				return this._markerType;
			}
			set
			{
				if (value != this._markerType)
				{
					this._markerType = value;
					base.OnPropertyChangedWithValue(value, "MarkerType");
				}
			}
		}

		[DataSourceProperty]
		public string VisualState
		{
			get
			{
				return this._visualState;
			}
			set
			{
				if (value != this._visualState)
				{
					this._visualState = value;
					base.OnPropertyChangedWithValue<string>(value, "VisualState");
				}
			}
		}

		public readonly MissionMarkerType MissionMarkerType;

		private Vec2 _screenPosition;

		private int _distance;

		private string _name;

		private bool _isEnabled;

		private string _color;

		private string _color2;

		private int _markerType;

		private string _visualState;
	}
}
