using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.NameMarker
{
	// Token: 0x020000DB RID: 219
	public class NameMarkerListPanel : ListPanel
	{
		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x06000B24 RID: 2852 RVA: 0x0001EF91 File Offset: 0x0001D191
		// (set) Token: 0x06000B25 RID: 2853 RVA: 0x0001EF99 File Offset: 0x0001D199
		public float FarAlphaTarget { get; set; } = 0.2f;

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x06000B26 RID: 2854 RVA: 0x0001EFA2 File Offset: 0x0001D1A2
		// (set) Token: 0x06000B27 RID: 2855 RVA: 0x0001EFAA File Offset: 0x0001D1AA
		public float FarDistanceCutoff { get; set; } = 50f;

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x06000B28 RID: 2856 RVA: 0x0001EFB3 File Offset: 0x0001D1B3
		// (set) Token: 0x06000B29 RID: 2857 RVA: 0x0001EFBB File Offset: 0x0001D1BB
		public float CloseDistanceCutoff { get; set; } = 25f;

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x06000B2A RID: 2858 RVA: 0x0001EFC4 File Offset: 0x0001D1C4
		// (set) Token: 0x06000B2B RID: 2859 RVA: 0x0001EFCC File Offset: 0x0001D1CC
		public bool HasTypeMarker { get; private set; }

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x06000B2C RID: 2860 RVA: 0x0001EFD5 File Offset: 0x0001D1D5
		// (set) Token: 0x06000B2D RID: 2861 RVA: 0x0001EFDD File Offset: 0x0001D1DD
		public MarkerRect Rect { get; private set; }

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x06000B2E RID: 2862 RVA: 0x0001EFE6 File Offset: 0x0001D1E6
		// (set) Token: 0x06000B2F RID: 2863 RVA: 0x0001EFEE File Offset: 0x0001D1EE
		public bool IsInScreenBoundaries { get; private set; }

		// Token: 0x06000B30 RID: 2864 RVA: 0x0001EFF8 File Offset: 0x0001D1F8
		public NameMarkerListPanel(UIContext context)
			: base(context)
		{
			this._parentScreenWidget = base.EventManager.Root.GetChild(0).GetChild(0);
			this.Rect = new MarkerRect();
		}

		// Token: 0x06000B31 RID: 2865 RVA: 0x0001F06C File Offset: 0x0001D26C
		public void Update(float dt)
		{
			this._transitionDT = MathF.Clamp(dt * 12f, 0f, 1f);
			this._targetAlpha = (this.IsMarkerEnabled ? this.GetDistanceRelatedAlphaTarget(this.Distance) : 0f);
			this.ApplyActionForThisAndAllChildren(new Action<Widget>(this.UpdateAlpha));
			TextWidget nameTextWidget = this.NameTextWidget;
			if ((nameTextWidget != null && nameTextWidget.IsVisible) || this.TypeVisualWidget.IsVisible)
			{
				base.ScaledPositionYOffset = this.Position.y - base.Size.Y / 2f;
				base.ScaledPositionXOffset = this.Position.x - base.Size.X / 2f;
			}
			this.UpdateRectangle();
		}

		// Token: 0x06000B32 RID: 2866 RVA: 0x0001F138 File Offset: 0x0001D338
		private void UpdateAlpha(Widget item)
		{
			if ((item == this.NameTextWidget || item == this.DistanceTextWidget || item == this.DistanceIconWidget) && this.HasTypeMarker && !this.IsFocused)
			{
				return;
			}
			float num = this.LocalLerp(item.AlphaFactor, this._targetAlpha, this._transitionDT);
			item.SetAlpha(num);
			item.IsVisible = (double)item.AlphaFactor > 0.05;
		}

		// Token: 0x06000B33 RID: 2867 RVA: 0x0001F1AC File Offset: 0x0001D3AC
		public void UpdateRectangle()
		{
			this.Rect.Reset();
			this.Rect.UpdatePoints(base.ScaledPositionXOffset, base.ScaledPositionXOffset + base.Size.X, base.ScaledPositionYOffset, base.ScaledPositionYOffset + base.Size.Y);
			this.IsInScreenBoundaries = this.Rect.Left > -50f && this.Rect.Right < base.EventManager.PageSize.X + 50f && this.Rect.Top > -50f && this.Rect.Bottom < base.EventManager.PageSize.Y + 50f;
		}

		// Token: 0x06000B34 RID: 2868 RVA: 0x0001F274 File Offset: 0x0001D474
		private float GetDistanceRelatedAlphaTarget(int distance)
		{
			if (this.IsFocused)
			{
				return 1f;
			}
			if ((float)distance > this.FarDistanceCutoff)
			{
				return this.FarAlphaTarget;
			}
			if ((float)distance <= this.FarDistanceCutoff && (float)distance >= this.CloseDistanceCutoff)
			{
				float num = (float)Math.Pow((double)(((float)distance - this.CloseDistanceCutoff) / (this.FarDistanceCutoff - this.CloseDistanceCutoff)), 0.3333333333333333);
				return MathF.Clamp(MathF.Lerp(1f, this.FarAlphaTarget, num, 1E-05f), this.FarAlphaTarget, 1f);
			}
			return 1f;
		}

		// Token: 0x06000B35 RID: 2869 RVA: 0x0001F308 File Offset: 0x0001D508
		private float LocalLerp(float start, float end, float delta)
		{
			if (Math.Abs(start - end) > 1E-45f)
			{
				return (end - start) * delta + start;
			}
			return end;
		}

		// Token: 0x06000B36 RID: 2870 RVA: 0x0001F324 File Offset: 0x0001D524
		private void OnStateChanged()
		{
			TextWidget nameTextWidget = this.NameTextWidget;
			if (nameTextWidget != null)
			{
				nameTextWidget.SetState(this.NameType);
			}
			BrushWidget typeVisualWidget = this.TypeVisualWidget;
			if (typeVisualWidget != null)
			{
				typeVisualWidget.SetState(this.IconType);
			}
			this.HasTypeMarker = this.IconType != string.Empty;
			if (this.HasTypeMarker && this.IsFocused)
			{
				TextWidget nameTextWidget2 = this.NameTextWidget;
				if (nameTextWidget2 != null)
				{
					nameTextWidget2.SetAlpha(1f);
				}
				TextWidget distanceTextWidget = this.DistanceTextWidget;
				if (distanceTextWidget != null)
				{
					distanceTextWidget.SetAlpha(1f);
				}
				BrushWidget distanceIconWidget = this.DistanceIconWidget;
				if (distanceIconWidget != null)
				{
					distanceIconWidget.SetAlpha(1f);
				}
			}
			else if (this.HasTypeMarker && !this.IsFocused)
			{
				TextWidget nameTextWidget3 = this.NameTextWidget;
				if (nameTextWidget3 != null)
				{
					nameTextWidget3.SetAlpha(0f);
				}
				TextWidget distanceTextWidget2 = this.DistanceTextWidget;
				if (distanceTextWidget2 != null)
				{
					distanceTextWidget2.SetAlpha(0f);
				}
				BrushWidget distanceIconWidget2 = this.DistanceIconWidget;
				if (distanceIconWidget2 != null)
				{
					distanceIconWidget2.SetAlpha(0f);
				}
			}
			if (this.IsEnemy)
			{
				this.TypeVisualWidget.Brush.GlobalColor = this.EnemyColor;
			}
			else if (this.IsFriendly)
			{
				this.TypeVisualWidget.Brush.GlobalColor = this.FriendlyColor;
			}
			else if (this.HasMainQuest)
			{
				this.TypeVisualWidget.Brush.GlobalColor = this.MainQuestNotificationColor;
			}
			else if (this.HasIssue)
			{
				this.TypeVisualWidget.Brush.GlobalColor = this.IssueNotificationColor;
			}
			BrushWidget typeVisualWidget2 = this.TypeVisualWidget;
			Sprite sprite;
			if (typeVisualWidget2 == null)
			{
				sprite = null;
			}
			else
			{
				Style style = typeVisualWidget2.Brush.GetStyle(this.IconType);
				if (style == null)
				{
					sprite = null;
				}
				else
				{
					StyleLayer layer = style.GetLayer(0);
					sprite = ((layer != null) ? layer.Sprite : null);
				}
			}
			Sprite sprite2 = sprite;
			if (sprite2 != null)
			{
				base.SuggestedWidth = base.SuggestedHeight / (float)sprite2.Height * (float)sprite2.Width;
			}
		}

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x06000B37 RID: 2871 RVA: 0x0001F4EF File Offset: 0x0001D6EF
		// (set) Token: 0x06000B38 RID: 2872 RVA: 0x0001F4F7 File Offset: 0x0001D6F7
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
					this.OnStateChanged();
				}
			}
		}

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x06000B39 RID: 2873 RVA: 0x0001F51B File Offset: 0x0001D71B
		// (set) Token: 0x06000B3A RID: 2874 RVA: 0x0001F523 File Offset: 0x0001D723
		[DataSourceProperty]
		public BrushWidget TypeVisualWidget
		{
			get
			{
				return this._typeVisualWidget;
			}
			set
			{
				if (this._typeVisualWidget != value)
				{
					this._typeVisualWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "TypeVisualWidget");
					this.OnStateChanged();
				}
			}
		}

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x06000B3B RID: 2875 RVA: 0x0001F547 File Offset: 0x0001D747
		// (set) Token: 0x06000B3C RID: 2876 RVA: 0x0001F54F File Offset: 0x0001D74F
		[DataSourceProperty]
		public BrushWidget DistanceIconWidget
		{
			get
			{
				return this._distanceIconWidget;
			}
			set
			{
				if (this._distanceIconWidget != value)
				{
					this._distanceIconWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "DistanceIconWidget");
					this.OnStateChanged();
				}
			}
		}

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x06000B3D RID: 2877 RVA: 0x0001F573 File Offset: 0x0001D773
		// (set) Token: 0x06000B3E RID: 2878 RVA: 0x0001F57B File Offset: 0x0001D77B
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
					this.OnStateChanged();
				}
			}
		}

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x06000B3F RID: 2879 RVA: 0x0001F59F File Offset: 0x0001D79F
		// (set) Token: 0x06000B40 RID: 2880 RVA: 0x0001F5A7 File Offset: 0x0001D7A7
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

		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06000B41 RID: 2881 RVA: 0x0001F5CA File Offset: 0x0001D7CA
		// (set) Token: 0x06000B42 RID: 2882 RVA: 0x0001F5D2 File Offset: 0x0001D7D2
		[Editor(false)]
		public Color IssueNotificationColor
		{
			get
			{
				return this._issueNotificationColor;
			}
			set
			{
				if (value != this._issueNotificationColor)
				{
					this._issueNotificationColor = value;
					base.OnPropertyChanged(value, "IssueNotificationColor");
					this.OnStateChanged();
				}
			}
		}

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06000B43 RID: 2883 RVA: 0x0001F5FB File Offset: 0x0001D7FB
		// (set) Token: 0x06000B44 RID: 2884 RVA: 0x0001F603 File Offset: 0x0001D803
		[Editor(false)]
		public Color MainQuestNotificationColor
		{
			get
			{
				return this._mainQuestNotificationColor;
			}
			set
			{
				if (value != this._mainQuestNotificationColor)
				{
					this._mainQuestNotificationColor = value;
					base.OnPropertyChanged(value, "MainQuestNotificationColor");
					this.OnStateChanged();
				}
			}
		}

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06000B45 RID: 2885 RVA: 0x0001F62C File Offset: 0x0001D82C
		// (set) Token: 0x06000B46 RID: 2886 RVA: 0x0001F634 File Offset: 0x0001D834
		[Editor(false)]
		public Color EnemyColor
		{
			get
			{
				return this._enemyColor;
			}
			set
			{
				if (value != this._enemyColor)
				{
					this._enemyColor = value;
					base.OnPropertyChanged(value, "EnemyColor");
					this.OnStateChanged();
				}
			}
		}

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06000B47 RID: 2887 RVA: 0x0001F65D File Offset: 0x0001D85D
		// (set) Token: 0x06000B48 RID: 2888 RVA: 0x0001F665 File Offset: 0x0001D865
		[Editor(false)]
		public Color FriendlyColor
		{
			get
			{
				return this._friendlyColor;
			}
			set
			{
				if (value != this._friendlyColor)
				{
					this._friendlyColor = value;
					base.OnPropertyChanged(value, "FriendlyColor");
					this.OnStateChanged();
				}
			}
		}

		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x06000B49 RID: 2889 RVA: 0x0001F68E File Offset: 0x0001D88E
		// (set) Token: 0x06000B4A RID: 2890 RVA: 0x0001F696 File Offset: 0x0001D896
		[Editor(false)]
		public string IconType
		{
			get
			{
				return this._iconType;
			}
			set
			{
				if (value != this._iconType)
				{
					this._iconType = value;
					base.OnPropertyChanged<string>(value, "IconType");
					this.OnStateChanged();
				}
			}
		}

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x06000B4B RID: 2891 RVA: 0x0001F6BF File Offset: 0x0001D8BF
		// (set) Token: 0x06000B4C RID: 2892 RVA: 0x0001F6C7 File Offset: 0x0001D8C7
		[Editor(false)]
		public string NameType
		{
			get
			{
				return this._nameType;
			}
			set
			{
				if (value != this._nameType)
				{
					this._nameType = value;
					base.OnPropertyChanged<string>(value, "NameType");
					this.OnStateChanged();
				}
			}
		}

		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x06000B4D RID: 2893 RVA: 0x0001F6F0 File Offset: 0x0001D8F0
		// (set) Token: 0x06000B4E RID: 2894 RVA: 0x0001F6F8 File Offset: 0x0001D8F8
		[DataSourceProperty]
		public int Distance
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

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x06000B4F RID: 2895 RVA: 0x0001F716 File Offset: 0x0001D916
		// (set) Token: 0x06000B50 RID: 2896 RVA: 0x0001F71E File Offset: 0x0001D91E
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

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x06000B51 RID: 2897 RVA: 0x0001F73C File Offset: 0x0001D93C
		// (set) Token: 0x06000B52 RID: 2898 RVA: 0x0001F744 File Offset: 0x0001D944
		[DataSourceProperty]
		public bool HasIssue
		{
			get
			{
				return this._hasIssue;
			}
			set
			{
				if (this._hasIssue != value)
				{
					this._hasIssue = value;
					base.OnPropertyChanged(value, "HasIssue");
					this.OnStateChanged();
				}
			}
		}

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x06000B53 RID: 2899 RVA: 0x0001F768 File Offset: 0x0001D968
		// (set) Token: 0x06000B54 RID: 2900 RVA: 0x0001F770 File Offset: 0x0001D970
		[DataSourceProperty]
		public bool HasMainQuest
		{
			get
			{
				return this._hasMainQuest;
			}
			set
			{
				if (this._hasMainQuest != value)
				{
					this._hasMainQuest = value;
					base.OnPropertyChanged(value, "HasMainQuest");
					this.OnStateChanged();
				}
			}
		}

		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x06000B55 RID: 2901 RVA: 0x0001F794 File Offset: 0x0001D994
		// (set) Token: 0x06000B56 RID: 2902 RVA: 0x0001F79C File Offset: 0x0001D99C
		[DataSourceProperty]
		public bool IsEnemy
		{
			get
			{
				return this._isEnemy;
			}
			set
			{
				if (this._isEnemy != value)
				{
					this._isEnemy = value;
					base.OnPropertyChanged(value, "IsEnemy");
					this.OnStateChanged();
				}
			}
		}

		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x06000B57 RID: 2903 RVA: 0x0001F7C0 File Offset: 0x0001D9C0
		// (set) Token: 0x06000B58 RID: 2904 RVA: 0x0001F7C8 File Offset: 0x0001D9C8
		[DataSourceProperty]
		public bool IsFriendly
		{
			get
			{
				return this._isFriendly;
			}
			set
			{
				if (this._isFriendly != value)
				{
					this._isFriendly = value;
					base.OnPropertyChanged(value, "IsFriendly");
					this.OnStateChanged();
				}
			}
		}

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x06000B59 RID: 2905 RVA: 0x0001F7EC File Offset: 0x0001D9EC
		// (set) Token: 0x06000B5A RID: 2906 RVA: 0x0001F7F4 File Offset: 0x0001D9F4
		[Editor(false)]
		public new bool IsFocused
		{
			get
			{
				return this._isFocused;
			}
			set
			{
				if (value != this._isFocused)
				{
					this._isFocused = value;
					base.OnPropertyChanged(value, "IsFocused");
					if (!value && this.IsMarkerEnabled)
					{
						TextWidget nameTextWidget = this.NameTextWidget;
						if (nameTextWidget != null)
						{
							nameTextWidget.SetAlpha(0f);
						}
						TextWidget distanceTextWidget = this.DistanceTextWidget;
						if (distanceTextWidget != null)
						{
							distanceTextWidget.SetAlpha(0f);
						}
						BrushWidget distanceIconWidget = this.DistanceIconWidget;
						if (distanceIconWidget != null)
						{
							distanceIconWidget.SetAlpha(0f);
						}
					}
					else if (value && this.IsMarkerEnabled)
					{
						TextWidget nameTextWidget2 = this.NameTextWidget;
						if (nameTextWidget2 != null)
						{
							nameTextWidget2.SetAlpha(1f);
						}
						TextWidget distanceTextWidget2 = this.DistanceTextWidget;
						if (distanceTextWidget2 != null)
						{
							distanceTextWidget2.SetAlpha(1f);
						}
						BrushWidget distanceIconWidget2 = this.DistanceIconWidget;
						if (distanceIconWidget2 != null)
						{
							distanceIconWidget2.SetAlpha(1f);
						}
					}
					base.RenderLate = value;
				}
			}
		}

		// Token: 0x0400051B RID: 1307
		private Widget _parentScreenWidget;

		// Token: 0x0400051F RID: 1311
		private const float BoundaryOffset = 50f;

		// Token: 0x04000520 RID: 1312
		private float _transitionDT;

		// Token: 0x04000521 RID: 1313
		private float _targetAlpha;

		// Token: 0x04000522 RID: 1314
		private string _iconType = string.Empty;

		// Token: 0x04000523 RID: 1315
		private string _nameType = string.Empty;

		// Token: 0x04000524 RID: 1316
		private int _distance;

		// Token: 0x04000525 RID: 1317
		private TextWidget _nameTextWidget;

		// Token: 0x04000526 RID: 1318
		private BrushWidget _typeVisualWidget;

		// Token: 0x04000527 RID: 1319
		private BrushWidget _distanceIconWidget;

		// Token: 0x04000528 RID: 1320
		private TextWidget _distanceTextWidget;

		// Token: 0x04000529 RID: 1321
		private Vec2 _position;

		// Token: 0x0400052A RID: 1322
		private Color _issueNotificationColor;

		// Token: 0x0400052B RID: 1323
		private Color _mainQuestNotificationColor;

		// Token: 0x0400052C RID: 1324
		private Color _enemyColor;

		// Token: 0x0400052D RID: 1325
		private Color _friendlyColor;

		// Token: 0x0400052E RID: 1326
		private bool _isMarkerEnabled;

		// Token: 0x0400052F RID: 1327
		private bool _hasIssue;

		// Token: 0x04000530 RID: 1328
		private bool _hasMainQuest;

		// Token: 0x04000531 RID: 1329
		private bool _isEnemy;

		// Token: 0x04000532 RID: 1330
		private bool _isFriendly;

		// Token: 0x04000533 RID: 1331
		private bool _isFocused;
	}
}
