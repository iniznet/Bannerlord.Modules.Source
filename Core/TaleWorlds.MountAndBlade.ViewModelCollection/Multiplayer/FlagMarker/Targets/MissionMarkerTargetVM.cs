using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.FlagMarker.Targets
{
	// Token: 0x020000C0 RID: 192
	public abstract class MissionMarkerTargetVM : ViewModel
	{
		// Token: 0x170005F2 RID: 1522
		// (get) Token: 0x06001246 RID: 4678
		public abstract Vec3 WorldPosition { get; }

		// Token: 0x170005F3 RID: 1523
		// (get) Token: 0x06001247 RID: 4679
		protected abstract float HeightOffset { get; }

		// Token: 0x06001248 RID: 4680 RVA: 0x0003C380 File Offset: 0x0003A580
		public MissionMarkerTargetVM(MissionMarkerType markerType)
		{
			this.MissionMarkerType = markerType;
			this.MarkerType = (int)markerType;
		}

		// Token: 0x06001249 RID: 4681 RVA: 0x0003C398 File Offset: 0x0003A598
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

		// Token: 0x0600124A RID: 4682 RVA: 0x0003C430 File Offset: 0x0003A630
		protected void RefreshColor(uint color, uint color2)
		{
			if (color != 0U)
			{
				string text = color.ToString("X");
				char c = text[0];
				char c2 = text[1];
				text = text.Remove(0, 2);
				text = text.Add(c.ToString() + c2.ToString(), false);
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
				text2 = text2.Add(c3.ToString() + c4.ToString(), false);
				this.Color2 = "#" + text2;
				return;
			}
			this.Color2 = "#FFFFFFFF";
		}

		// Token: 0x170005F4 RID: 1524
		// (get) Token: 0x0600124B RID: 4683 RVA: 0x0003C502 File Offset: 0x0003A702
		// (set) Token: 0x0600124C RID: 4684 RVA: 0x0003C50A File Offset: 0x0003A70A
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

		// Token: 0x170005F5 RID: 1525
		// (get) Token: 0x0600124D RID: 4685 RVA: 0x0003C545 File Offset: 0x0003A745
		// (set) Token: 0x0600124E RID: 4686 RVA: 0x0003C54D File Offset: 0x0003A74D
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

		// Token: 0x170005F6 RID: 1526
		// (get) Token: 0x0600124F RID: 4687 RVA: 0x0003C570 File Offset: 0x0003A770
		// (set) Token: 0x06001250 RID: 4688 RVA: 0x0003C578 File Offset: 0x0003A778
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

		// Token: 0x170005F7 RID: 1527
		// (get) Token: 0x06001251 RID: 4689 RVA: 0x0003C596 File Offset: 0x0003A796
		// (set) Token: 0x06001252 RID: 4690 RVA: 0x0003C59E File Offset: 0x0003A79E
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

		// Token: 0x170005F8 RID: 1528
		// (get) Token: 0x06001253 RID: 4691 RVA: 0x0003C5BC File Offset: 0x0003A7BC
		// (set) Token: 0x06001254 RID: 4692 RVA: 0x0003C5C4 File Offset: 0x0003A7C4
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

		// Token: 0x170005F9 RID: 1529
		// (get) Token: 0x06001255 RID: 4693 RVA: 0x0003C5E7 File Offset: 0x0003A7E7
		// (set) Token: 0x06001256 RID: 4694 RVA: 0x0003C5EF File Offset: 0x0003A7EF
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

		// Token: 0x170005FA RID: 1530
		// (get) Token: 0x06001257 RID: 4695 RVA: 0x0003C612 File Offset: 0x0003A812
		// (set) Token: 0x06001258 RID: 4696 RVA: 0x0003C61A File Offset: 0x0003A81A
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

		// Token: 0x170005FB RID: 1531
		// (get) Token: 0x06001259 RID: 4697 RVA: 0x0003C638 File Offset: 0x0003A838
		// (set) Token: 0x0600125A RID: 4698 RVA: 0x0003C640 File Offset: 0x0003A840
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

		// Token: 0x040008BB RID: 2235
		public readonly MissionMarkerType MissionMarkerType;

		// Token: 0x040008BC RID: 2236
		private Vec2 _screenPosition;

		// Token: 0x040008BD RID: 2237
		private int _distance;

		// Token: 0x040008BE RID: 2238
		private string _name;

		// Token: 0x040008BF RID: 2239
		private bool _isEnabled;

		// Token: 0x040008C0 RID: 2240
		private string _color;

		// Token: 0x040008C1 RID: 2241
		private string _color2;

		// Token: 0x040008C2 RID: 2242
		private int _markerType;

		// Token: 0x040008C3 RID: 2243
		private string _visualState;
	}
}
