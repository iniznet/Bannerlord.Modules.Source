using System;
using System.Diagnostics;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Nameplate
{
	// Token: 0x02000075 RID: 117
	public class SettlementNameplateWidget : Widget, IComparable<SettlementNameplateWidget>
	{
		// Token: 0x06000674 RID: 1652 RVA: 0x000132DE File Offset: 0x000114DE
		public SettlementNameplateWidget(UIContext context)
			: base(context)
		{
			this._positionTimer = new Stopwatch();
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x06000675 RID: 1653 RVA: 0x00013316 File Offset: 0x00011516
		private float _screenEdgeAlphaTarget
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x06000676 RID: 1654 RVA: 0x0001331D File Offset: 0x0001151D
		private float _normalNeutralAlphaTarget
		{
			get
			{
				return 0.35f;
			}
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x06000677 RID: 1655 RVA: 0x00013324 File Offset: 0x00011524
		private float _normalAllyAlphaTarget
		{
			get
			{
				return 0.5f;
			}
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06000678 RID: 1656 RVA: 0x0001332B File Offset: 0x0001152B
		private float _normalEnemyAlphaTarget
		{
			get
			{
				return 0.35f;
			}
		}

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x06000679 RID: 1657 RVA: 0x00013332 File Offset: 0x00011532
		private float _trackedAlphaTarget
		{
			get
			{
				return 0.8f;
			}
		}

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x0600067A RID: 1658 RVA: 0x00013339 File Offset: 0x00011539
		private float _trackedColorFactorTarget
		{
			get
			{
				return 1.3f;
			}
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x0600067B RID: 1659 RVA: 0x00013340 File Offset: 0x00011540
		private float _normalColorFactorTarget
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x0600067C RID: 1660 RVA: 0x00013348 File Offset: 0x00011548
		protected override void OnParallelUpdate(float dt)
		{
			base.OnParallelUpdate(dt);
			SettlementNameplateItemWidget currentNameplate = this._currentNameplate;
			if (currentNameplate != null)
			{
				currentNameplate.ParallelUpdate(dt);
			}
			if (this._currentNameplate != null && this._cachedItemSize != this._currentNameplate.Size)
			{
				this._cachedItemSize = this._currentNameplate.Size;
				if (this._eventsListPanel != null)
				{
					this._eventsListPanel.ScaledPositionXOffset = this._cachedItemSize.X;
				}
				if (this._notificationListPanel != null)
				{
					this._notificationListPanel.ScaledPositionYOffset = -this._cachedItemSize.Y;
				}
				base.SuggestedWidth = this._cachedItemSize.X * base._inverseScaleToUse;
				base.SuggestedHeight = this._cachedItemSize.Y * base._inverseScaleToUse;
				base.ScaledSuggestedWidth = this._cachedItemSize.X;
				base.ScaledSuggestedHeight = this._cachedItemSize.Y;
			}
			base.IsEnabled = this.IsVisibleOnMap;
			this.UpdateNameplateTransparencyAndBrightness(dt);
			this.UpdatePosition();
			this.UpdateTutorialState();
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x00013454 File Offset: 0x00011654
		private void UpdatePosition()
		{
			bool flag = false;
			if (this.IsVisibleOnMap || (this._positionTimer.IsRunning && this._positionTimer.Elapsed.Seconds < 2))
			{
				float x = base.Context.EventManager.PageSize.X;
				float y = base.Context.EventManager.PageSize.Y;
				Vec2 vec = this.Position;
				if (this.IsTracked)
				{
					if (this.WSign > 0 && vec.x - base.Size.X / 2f > 0f && vec.x + base.Size.X / 2f < x && vec.y > 0f && vec.y + base.Size.Y < y)
					{
						base.ScaledPositionXOffset = vec.x - base.Size.X / 2f;
						base.ScaledPositionYOffset = vec.y - base.Size.Y;
					}
					else
					{
						Vec2 vec2 = new Vec2(x / 2f, y / 2f);
						vec -= vec2;
						if (this.WSign < 0)
						{
							vec *= -1f;
						}
						float num = Mathf.Atan2(vec.y, vec.x) - 1.5707964f;
						float num2 = Mathf.Cos(num);
						float num3 = Mathf.Sin(num);
						float num4 = num2 / num3;
						Vec2 vec3 = vec2 * 1f;
						vec = ((num2 > 0f) ? new Vec2(-vec3.y / num4, vec2.y) : new Vec2(vec3.y / num4, -vec2.y));
						if (vec.x > vec3.x)
						{
							vec = new Vec2(vec3.x, -vec3.x * num4);
						}
						else if (vec.x < -vec3.x)
						{
							vec = new Vec2(-vec3.x, vec3.x * num4);
						}
						vec += vec2;
						flag = vec.y - base.Size.Y - this._currentNameplate.MapEventVisualWidget.Size.Y <= 0f;
						base.ScaledPositionXOffset = Mathf.Clamp(vec.x - base.Size.X / 2f, 0f, x - this._currentNameplate.Size.X);
						base.ScaledPositionYOffset = Mathf.Clamp(vec.y - base.Size.Y, 0f, y - (this._currentNameplate.Size.Y + 55f));
					}
				}
				else
				{
					base.ScaledPositionXOffset = vec.x - base.Size.X / 2f;
					base.ScaledPositionYOffset = vec.y - base.Size.Y;
				}
			}
			if (flag)
			{
				this._currentNameplate.MapEventVisualWidget.VerticalAlignment = VerticalAlignment.Bottom;
				this._currentNameplate.MapEventVisualWidget.ScaledPositionYOffset = this._currentNameplate.MapEventVisualWidget.Size.Y;
				return;
			}
			this._currentNameplate.MapEventVisualWidget.VerticalAlignment = VerticalAlignment.Top;
			this._currentNameplate.MapEventVisualWidget.ScaledPositionYOffset = -this._currentNameplate.MapEventVisualWidget.Size.Y;
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x000137E0 File Offset: 0x000119E0
		private void OnNotificationListUpdated(Widget widget)
		{
			this._updatePositionNextFrame = true;
			this.AddLateUpdateAction();
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x000137EF File Offset: 0x000119EF
		private void OnNotificationListUpdated(Widget parentWidget, Widget addedWidget)
		{
			this._updatePositionNextFrame = true;
			this.AddLateUpdateAction();
		}

		// Token: 0x06000680 RID: 1664 RVA: 0x000137FE File Offset: 0x000119FE
		private void AddLateUpdateAction()
		{
			if (!this._lateUpdateActionAdded)
			{
				base.EventManager.AddLateUpdateAction(this, new Action<float>(this.CustomLateUpdate), 1);
				this._lateUpdateActionAdded = true;
			}
		}

		// Token: 0x06000681 RID: 1665 RVA: 0x00013828 File Offset: 0x00011A28
		private void CustomLateUpdate(float dt)
		{
			if (this._updatePositionNextFrame)
			{
				this.UpdatePosition();
				this._updatePositionNextFrame = false;
			}
			this._lateUpdateActionAdded = false;
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x00013846 File Offset: 0x00011A46
		private void UpdateTutorialState()
		{
			if (this._tutorialAnimState == SettlementNameplateWidget.TutorialAnimState.Start)
			{
				this._tutorialAnimState = SettlementNameplateWidget.TutorialAnimState.FirstFrame;
			}
			else
			{
				SettlementNameplateWidget.TutorialAnimState tutorialAnimState = this._tutorialAnimState;
			}
			if (this.IsTargetedByTutorial)
			{
				this.SetState("Default");
				return;
			}
			this.SetState("Disabled");
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x00013884 File Offset: 0x00011A84
		private void SetNameplateTypeVisual(int type)
		{
			if (this._currentNameplate == null)
			{
				this.SmallNameplateWidget.IsVisible = false;
				this.NormalNameplateWidget.IsVisible = false;
				this.BigNameplateWidget.IsVisible = false;
				switch (type)
				{
				case 0:
					this._currentNameplate = this.SmallNameplateWidget;
					this.SmallNameplateWidget.IsVisible = true;
					return;
				case 1:
					this._currentNameplate = this.NormalNameplateWidget;
					this.NormalNameplateWidget.IsVisible = true;
					return;
				case 2:
					this._currentNameplate = this.BigNameplateWidget;
					this.BigNameplateWidget.IsVisible = true;
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x00013920 File Offset: 0x00011B20
		private void SetNameplateRelationType(int type)
		{
			if (this._currentNameplate != null)
			{
				switch (type)
				{
				case 0:
					this._currentNameplate.Color = Color.Black;
					return;
				case 1:
					this._currentNameplate.Color = Color.ConvertStringToColor("#245E05FF");
					return;
				case 2:
					this._currentNameplate.Color = Color.ConvertStringToColor("#870707FF");
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06000685 RID: 1669 RVA: 0x00013984 File Offset: 0x00011B84
		private void UpdateNameplateTransparencyAndBrightness(float dt)
		{
			float num = dt * this._lerpModifier;
			if (this.IsVisibleOnMap)
			{
				base.IsVisible = true;
				float num2 = this.DetermineTargetAlphaValue();
				float num3 = this.DetermineTargetColorFactor();
				float num4 = MathF.Lerp(this._currentNameplate.AlphaFactor, num2, num, 1E-05f);
				float num5 = MathF.Lerp(this._currentNameplate.ColorFactor, num3, num, 1E-05f);
				float num6 = MathF.Lerp(this._currentNameplate.SettlementNameTextWidget.ReadOnlyBrush.GlobalAlphaFactor, 1f, num, 1E-05f);
				this._currentNameplate.AlphaFactor = num4;
				this._currentNameplate.ColorFactor = num5;
				this._currentNameplate.SettlementNameTextWidget.Brush.GlobalAlphaFactor = num6;
				this._currentNameplate.SettlementBannerWidget.Brush.GlobalAlphaFactor = num6;
				this._currentNameplate.SettlementPartiesGridWidget.SetGlobalAlphaRecursively(num6);
				this._eventsListPanel.SetGlobalAlphaRecursively(num6);
			}
			else if (this._currentNameplate.AlphaFactor > this._lerpThreshold)
			{
				float num7 = MathF.Lerp(this._currentNameplate.AlphaFactor, 0f, num, 1E-05f);
				this._currentNameplate.AlphaFactor = num7;
				this._currentNameplate.SettlementNameTextWidget.Brush.GlobalAlphaFactor = num7;
				this._currentNameplate.SettlementBannerWidget.Brush.GlobalAlphaFactor = num7;
				this._currentNameplate.SettlementPartiesGridWidget.SetGlobalAlphaRecursively(num7);
				this._eventsListPanel.SetGlobalAlphaRecursively(num7);
			}
			else
			{
				base.IsVisible = false;
			}
			Widget settlementNameplateInspectedWidget = this._currentNameplate.SettlementNameplateInspectedWidget;
			if (this.IsInRange && this.IsVisibleOnMap)
			{
				if (Math.Abs(settlementNameplateInspectedWidget.AlphaFactor - 1f) > this._lerpThreshold)
				{
					settlementNameplateInspectedWidget.AlphaFactor = MathF.Lerp(settlementNameplateInspectedWidget.AlphaFactor, 1f, num, 1E-05f);
					return;
				}
			}
			else if (this._currentNameplate.AlphaFactor - 0f > this._lerpThreshold)
			{
				settlementNameplateInspectedWidget.AlphaFactor = MathF.Lerp(settlementNameplateInspectedWidget.AlphaFactor, 0f, num, 1E-05f);
			}
		}

		// Token: 0x06000686 RID: 1670 RVA: 0x00013B9C File Offset: 0x00011D9C
		private float DetermineTargetAlphaValue()
		{
			if (this.IsInsideWindow)
			{
				if (this.IsTracked)
				{
					return this._trackedAlphaTarget;
				}
				if (this.RelationType == 0)
				{
					return this._normalNeutralAlphaTarget;
				}
				if (this.RelationType == 1)
				{
					return this._normalAllyAlphaTarget;
				}
				return this._normalEnemyAlphaTarget;
			}
			else
			{
				if (this.IsTracked)
				{
					return this._screenEdgeAlphaTarget;
				}
				return 0f;
			}
		}

		// Token: 0x06000687 RID: 1671 RVA: 0x00013BFA File Offset: 0x00011DFA
		private float DetermineTargetColorFactor()
		{
			if (this.IsTracked)
			{
				return this._trackedColorFactorTarget;
			}
			return this._normalColorFactorTarget;
		}

		// Token: 0x06000688 RID: 1672 RVA: 0x00013C14 File Offset: 0x00011E14
		public int CompareTo(SettlementNameplateWidget other)
		{
			return other.DistanceToCamera.CompareTo(this.DistanceToCamera);
		}

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x06000689 RID: 1673 RVA: 0x00013C35 File Offset: 0x00011E35
		// (set) Token: 0x0600068A RID: 1674 RVA: 0x00013C3D File Offset: 0x00011E3D
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

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x0600068B RID: 1675 RVA: 0x00013C60 File Offset: 0x00011E60
		// (set) Token: 0x0600068C RID: 1676 RVA: 0x00013C68 File Offset: 0x00011E68
		public bool IsVisibleOnMap
		{
			get
			{
				return this._isVisibleOnMap;
			}
			set
			{
				if (this._isVisibleOnMap != value)
				{
					if (this._isVisibleOnMap && !value)
					{
						this._positionTimer.Restart();
					}
					this._isVisibleOnMap = value;
					base.OnPropertyChanged(value, "IsVisibleOnMap");
				}
			}
		}

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x0600068D RID: 1677 RVA: 0x00013C9C File Offset: 0x00011E9C
		// (set) Token: 0x0600068E RID: 1678 RVA: 0x00013CA4 File Offset: 0x00011EA4
		public bool IsTracked
		{
			get
			{
				return this._isTracked;
			}
			set
			{
				if (this._isTracked != value)
				{
					this._isTracked = value;
					base.OnPropertyChanged(value, "IsTracked");
				}
			}
		}

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x0600068F RID: 1679 RVA: 0x00013CC2 File Offset: 0x00011EC2
		// (set) Token: 0x06000690 RID: 1680 RVA: 0x00013CCA File Offset: 0x00011ECA
		public bool IsTargetedByTutorial
		{
			get
			{
				return this._isTargetedByTutorial;
			}
			set
			{
				if (this._isTargetedByTutorial != value)
				{
					this._isTargetedByTutorial = value;
					base.OnPropertyChanged(value, "IsTargetedByTutorial");
					if (value)
					{
						this._tutorialAnimState = SettlementNameplateWidget.TutorialAnimState.Start;
					}
				}
			}
		}

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x06000691 RID: 1681 RVA: 0x00013CF2 File Offset: 0x00011EF2
		// (set) Token: 0x06000692 RID: 1682 RVA: 0x00013CFA File Offset: 0x00011EFA
		public bool IsInsideWindow
		{
			get
			{
				return this._isInsideWindow;
			}
			set
			{
				if (this._isInsideWindow != value)
				{
					this._isInsideWindow = value;
					base.OnPropertyChanged(value, "IsInsideWindow");
				}
			}
		}

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x06000693 RID: 1683 RVA: 0x00013D18 File Offset: 0x00011F18
		// (set) Token: 0x06000694 RID: 1684 RVA: 0x00013D20 File Offset: 0x00011F20
		public bool IsInRange
		{
			get
			{
				return this._isInRange;
			}
			set
			{
				if (this._isInRange != value)
				{
					this._isInRange = value;
				}
			}
		}

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x06000695 RID: 1685 RVA: 0x00013D32 File Offset: 0x00011F32
		// (set) Token: 0x06000696 RID: 1686 RVA: 0x00013D3A File Offset: 0x00011F3A
		public int NameplateType
		{
			get
			{
				return this._nameplateType;
			}
			set
			{
				if (this._nameplateType != value)
				{
					this._nameplateType = value;
					base.OnPropertyChanged(value, "NameplateType");
					this.SetNameplateTypeVisual(value);
				}
			}
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x06000697 RID: 1687 RVA: 0x00013D5F File Offset: 0x00011F5F
		// (set) Token: 0x06000698 RID: 1688 RVA: 0x00013D67 File Offset: 0x00011F67
		public int RelationType
		{
			get
			{
				return this._relationType;
			}
			set
			{
				if (this._relationType != value)
				{
					this._relationType = value;
					base.OnPropertyChanged(value, "RelationType");
					this.SetNameplateRelationType(value);
				}
			}
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06000699 RID: 1689 RVA: 0x00013D8C File Offset: 0x00011F8C
		// (set) Token: 0x0600069A RID: 1690 RVA: 0x00013D94 File Offset: 0x00011F94
		public int WSign
		{
			get
			{
				return this._wSign;
			}
			set
			{
				if (this._wSign != value)
				{
					this._wSign = value;
					base.OnPropertyChanged(value, "WSign");
				}
			}
		}

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x0600069B RID: 1691 RVA: 0x00013DB2 File Offset: 0x00011FB2
		// (set) Token: 0x0600069C RID: 1692 RVA: 0x00013DBA File Offset: 0x00011FBA
		public float WPos
		{
			get
			{
				return this._wPos;
			}
			set
			{
				if (this._wPos != value)
				{
					this._wPos = value;
					base.OnPropertyChanged(value, "WPos");
				}
			}
		}

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x0600069D RID: 1693 RVA: 0x00013DD8 File Offset: 0x00011FD8
		// (set) Token: 0x0600069E RID: 1694 RVA: 0x00013DE0 File Offset: 0x00011FE0
		public float DistanceToCamera
		{
			get
			{
				return this._distanceToCamera;
			}
			set
			{
				if (this._distanceToCamera != value)
				{
					this._distanceToCamera = value;
					base.OnPropertyChanged(value, "DistanceToCamera");
				}
			}
		}

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x0600069F RID: 1695 RVA: 0x00013DFE File Offset: 0x00011FFE
		// (set) Token: 0x060006A0 RID: 1696 RVA: 0x00013E08 File Offset: 0x00012008
		public ListPanel NotificationListPanel
		{
			get
			{
				return this._notificationListPanel;
			}
			set
			{
				if (this._notificationListPanel != value)
				{
					this._notificationListPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "NotificationListPanel");
					this._notificationListPanel.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnNotificationListUpdated));
					this._notificationListPanel.ItemAfterRemoveEventHandlers.Add(new Action<Widget>(this.OnNotificationListUpdated));
				}
			}
		}

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x060006A1 RID: 1697 RVA: 0x00013E69 File Offset: 0x00012069
		// (set) Token: 0x060006A2 RID: 1698 RVA: 0x00013E71 File Offset: 0x00012071
		public ListPanel EventsListPanel
		{
			get
			{
				return this._eventsListPanel;
			}
			set
			{
				if (value != this._eventsListPanel)
				{
					this._eventsListPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "EventsListPanel");
				}
			}
		}

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x060006A3 RID: 1699 RVA: 0x00013E8F File Offset: 0x0001208F
		// (set) Token: 0x060006A4 RID: 1700 RVA: 0x00013E97 File Offset: 0x00012097
		public SettlementNameplateItemWidget SmallNameplateWidget
		{
			get
			{
				return this._smallNameplateWidget;
			}
			set
			{
				if (this._smallNameplateWidget != value)
				{
					this._smallNameplateWidget = value;
					base.OnPropertyChanged<SettlementNameplateItemWidget>(value, "SmallNameplateWidget");
				}
			}
		}

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x060006A5 RID: 1701 RVA: 0x00013EB5 File Offset: 0x000120B5
		// (set) Token: 0x060006A6 RID: 1702 RVA: 0x00013EBD File Offset: 0x000120BD
		public SettlementNameplateItemWidget NormalNameplateWidget
		{
			get
			{
				return this._normalNameplateWidget;
			}
			set
			{
				if (this._normalNameplateWidget != value)
				{
					this._normalNameplateWidget = value;
					base.OnPropertyChanged<SettlementNameplateItemWidget>(value, "NormalNameplateWidget");
				}
			}
		}

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x060006A7 RID: 1703 RVA: 0x00013EDB File Offset: 0x000120DB
		// (set) Token: 0x060006A8 RID: 1704 RVA: 0x00013EE3 File Offset: 0x000120E3
		public SettlementNameplateItemWidget BigNameplateWidget
		{
			get
			{
				return this._bigNameplateWidget;
			}
			set
			{
				if (this._bigNameplateWidget != value)
				{
					this._bigNameplateWidget = value;
					base.OnPropertyChanged<SettlementNameplateItemWidget>(value, "BigNameplateWidget");
				}
			}
		}

		// Token: 0x040002D4 RID: 724
		private readonly Stopwatch _positionTimer;

		// Token: 0x040002D5 RID: 725
		private SettlementNameplateItemWidget _currentNameplate;

		// Token: 0x040002D6 RID: 726
		private bool _updatePositionNextFrame;

		// Token: 0x040002D7 RID: 727
		private SettlementNameplateWidget.TutorialAnimState _tutorialAnimState;

		// Token: 0x040002D8 RID: 728
		private float _lerpThreshold = 5E-05f;

		// Token: 0x040002D9 RID: 729
		private float _lerpModifier = 10f;

		// Token: 0x040002DA RID: 730
		private Vector2 _cachedItemSize;

		// Token: 0x040002DB RID: 731
		private bool _lateUpdateActionAdded;

		// Token: 0x040002DC RID: 732
		private Vec2 _position;

		// Token: 0x040002DD RID: 733
		private bool _isVisibleOnMap;

		// Token: 0x040002DE RID: 734
		private bool _isTracked;

		// Token: 0x040002DF RID: 735
		private bool _isInsideWindow;

		// Token: 0x040002E0 RID: 736
		private bool _isTargetedByTutorial;

		// Token: 0x040002E1 RID: 737
		private int _nameplateType = -1;

		// Token: 0x040002E2 RID: 738
		private int _relationType = -1;

		// Token: 0x040002E3 RID: 739
		private int _wSign;

		// Token: 0x040002E4 RID: 740
		private float _wPos;

		// Token: 0x040002E5 RID: 741
		private float _distanceToCamera;

		// Token: 0x040002E6 RID: 742
		private bool _isInRange;

		// Token: 0x040002E7 RID: 743
		private SettlementNameplateItemWidget _smallNameplateWidget;

		// Token: 0x040002E8 RID: 744
		private SettlementNameplateItemWidget _normalNameplateWidget;

		// Token: 0x040002E9 RID: 745
		private SettlementNameplateItemWidget _bigNameplateWidget;

		// Token: 0x040002EA RID: 746
		private ListPanel _notificationListPanel;

		// Token: 0x040002EB RID: 747
		private ListPanel _eventsListPanel;

		// Token: 0x0200018A RID: 394
		public enum TutorialAnimState
		{
			// Token: 0x040008E4 RID: 2276
			Idle,
			// Token: 0x040008E5 RID: 2277
			Start,
			// Token: 0x040008E6 RID: 2278
			FirstFrame,
			// Token: 0x040008E7 RID: 2279
			Playing
		}
	}
}
