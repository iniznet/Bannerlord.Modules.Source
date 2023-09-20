using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Nameplate
{
	// Token: 0x02000071 RID: 113
	public class PartyNameplateWidget : Widget
	{
		// Token: 0x06000619 RID: 1561 RVA: 0x00012021 File Offset: 0x00010221
		public PartyNameplateWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x0600061A RID: 1562 RVA: 0x0001203C File Offset: 0x0001023C
		private float _animSpeedModifier
		{
			get
			{
				return 8f;
			}
		}

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x0600061B RID: 1563 RVA: 0x00012043 File Offset: 0x00010243
		private int _armyFontSizeOffset
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x0600061C RID: 1564 RVA: 0x00012047 File Offset: 0x00010247
		// (set) Token: 0x0600061D RID: 1565 RVA: 0x0001204F File Offset: 0x0001024F
		public Widget HeadGroupWidget { get; set; }

		// Token: 0x0600061E RID: 1566 RVA: 0x00012058 File Offset: 0x00010258
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._isFirstFrame)
			{
				this.NameplateFullNameTextWidget.Brush.GlobalAlphaFactor = 0f;
				this.NameplateTextWidget.Brush.GlobalAlphaFactor = 0f;
				this.NameplateExtraInfoTextWidget.Brush.GlobalAlphaFactor = 0f;
				this.PartyBannerWidget.Brush.GlobalAlphaFactor = 0f;
				this.SpeedTextWidget.AlphaFactor = 0f;
				this._defaultNameplateFontSize = this.NameplateTextWidget.ReadOnlyBrush.FontSize;
				this._isFirstFrame = false;
			}
			int num = (this.IsArmy ? (this._defaultNameplateFontSize + this._armyFontSizeOffset) : this._defaultNameplateFontSize);
			if (this.NameplateTextWidget.Brush.FontSize != num)
			{
				this.NameplateTextWidget.Brush.FontSize = num;
			}
			this.UpdateNameplatesScreenPosition();
			this.UpdateNameplatesVisibility(dt);
			this.UpdateTutorialStatus();
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x00012150 File Offset: 0x00010350
		private void UpdateNameplatesVisibility(float dt)
		{
			float num = 0f;
			float num2;
			if (this.IsMainParty)
			{
				this._latestIsOutside = this.IsNameplateOutsideScreen();
				this.MainPartyArrowWidget.IsVisible = this._latestIsOutside;
				this.NameplateTextWidget.IsVisible = !this._latestIsOutside && !this.IsInArmy && !this.IsPrisoner && !this.IsInSettlement;
				this.NameplateFullNameTextWidget.IsVisible = !this._latestIsOutside && !this.IsInArmy && !this.IsPrisoner && !this.IsInSettlement;
				this.SpeedTextWidget.IsVisible = !this._latestIsOutside && !this.IsInArmy && !this.IsPrisoner && !this.IsInSettlement;
				this.SpeedIconWidget.IsVisible = !this._latestIsOutside && !this.IsInArmy && !this.IsPrisoner && !this.IsInSettlement;
				this.TrackerFrame.IsVisible = this._latestIsOutside;
				num2 = (float)((!this._latestIsOutside && !this.IsInArmy && !this.IsPrisoner && !this.IsInSettlement) ? 1 : 0);
				this.PartyBannerWidget.IsVisible = !this._latestIsOutside && !this.IsInArmy && !this.IsPrisoner && !this.IsInSettlement;
				this.NameplateExtraInfoTextWidget.IsVisible = !this._latestIsOutside && !this.IsInArmy && !this.IsPrisoner && !this.IsInSettlement;
				base.IsEnabled = this._latestIsOutside;
			}
			else
			{
				this.MainPartyArrowWidget.IsVisible = false;
				this.NameplateTextWidget.IsVisible = true;
				this.NameplateFullNameTextWidget.IsVisible = true;
				this.SpeedTextWidget.IsVisible = true;
				this.SpeedIconWidget.IsVisible = true;
				this.TrackerFrame.IsVisible = false;
				this.PartyBannerWidget.IsVisible = true;
				num2 = 1f;
				base.IsEnabled = false;
			}
			if (!this.IsVisibleOnMap && !this.IsMainParty)
			{
				this.NameplateTextWidget.IsVisible = false;
				this.NameplateFullNameTextWidget.IsVisible = false;
				this.SpeedTextWidget.IsVisible = false;
				this.SpeedIconWidget.IsVisible = false;
				num2 = 0f;
			}
			else
			{
				this._initialDelayAmount -= dt;
				if (this._initialDelayAmount <= 0f)
				{
					num = (float)(this.ShouldShowFullName ? 1 : 0);
				}
				else
				{
					num = 1f;
				}
			}
			this.NameplateTextWidget.Brush.GlobalAlphaFactor = this.LocalLerp(this.NameplateTextWidget.ReadOnlyBrush.GlobalAlphaFactor, num2, dt * this._animSpeedModifier);
			this.NameplateFullNameTextWidget.Brush.GlobalAlphaFactor = this.LocalLerp(this.NameplateFullNameTextWidget.ReadOnlyBrush.GlobalAlphaFactor, num, dt * this._animSpeedModifier);
			this.SpeedTextWidget.Brush.GlobalAlphaFactor = this.LocalLerp(this.SpeedTextWidget.ReadOnlyBrush.GlobalAlphaFactor, num, dt * this._animSpeedModifier);
			this.SpeedIconWidget.AlphaFactor = this.LocalLerp(this.SpeedIconWidget.AlphaFactor, num, dt * this._animSpeedModifier);
			this.NameplateExtraInfoTextWidget.Brush.GlobalAlphaFactor = this.LocalLerp(this.NameplateExtraInfoTextWidget.ReadOnlyBrush.GlobalAlphaFactor, (float)(this.ShouldShowFullName ? 1 : 0), dt * this._animSpeedModifier);
			this.PartyBannerWidget.Brush.GlobalAlphaFactor = this.LocalLerp(this.PartyBannerWidget.ReadOnlyBrush.GlobalAlphaFactor, num2, dt * this._animSpeedModifier);
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x000124E8 File Offset: 0x000106E8
		private void UpdateNameplatesScreenPosition()
		{
			this._screenWidth = base.Context.EventManager.PageSize.X;
			this._screenHeight = base.Context.EventManager.PageSize.Y;
			if (!this.IsVisibleOnMap && !this.IsMainParty)
			{
				base.IsHidden = true;
				return;
			}
			if (this.IsMainParty)
			{
				if (!this.IsBehind && this.Position.x + base.Size.X <= this._screenWidth && this.Position.y - base.Size.Y <= this._screenHeight && this.Position.x >= 0f && this.Position.y >= 0f)
				{
					Widget headGroupWidget = this.HeadGroupWidget;
					float num = ((headGroupWidget != null) ? headGroupWidget.Size.Y : 0f);
					this.NameplateLayoutListPanel.ScaledPositionYOffset = this.Position.y - this.HeadPosition.y + num;
					if (this.IsHigh)
					{
						base.ScaledPositionXOffset = MathF.Clamp(this.HeadPosition.x - base.Size.X / 2f, 0f, this._screenWidth - base.Size.X);
					}
					else
					{
						base.ScaledPositionXOffset = MathF.Clamp(this.HeadPosition.x - base.Size.X / 2f, 0f, this._screenWidth - base.Size.X);
					}
					base.ScaledPositionYOffset = this.HeadPosition.y - num;
				}
				else
				{
					Vec2 vec = new Vec2(base.Context.EventManager.PageSize.X / 2f, base.Context.EventManager.PageSize.Y / 2f);
					Vec2 vec2 = this.HeadPosition;
					vec2 -= vec;
					if (this.IsBehind)
					{
						vec2 *= -1f;
					}
					float num2 = Mathf.Atan2(vec2.y, vec2.x) - 1.5707964f;
					float num3 = Mathf.Cos(num2);
					float num4 = Mathf.Sin(num2);
					vec2 = vec + new Vec2(num4 * 150f, num3 * 150f);
					float num5 = num3 / num4;
					Vec2 vec3 = vec * 1f;
					vec2 = ((num3 > 0f) ? new Vec2(-vec3.y / num5, vec.y) : new Vec2(vec3.y / num5, -vec.y));
					if (vec2.x > vec3.x)
					{
						vec2 = new Vec2(vec3.x, -vec3.x * num5);
					}
					else if (vec2.x < -vec3.x)
					{
						vec2 = new Vec2(-vec3.x, vec3.x * num5);
					}
					vec2 += vec;
					base.ScaledPositionXOffset = MathF.Clamp(vec2.x - base.Size.X / 2f, 0f, this._screenWidth - base.Size.X);
					base.ScaledPositionYOffset = MathF.Clamp(vec2.y - base.Size.Y / 2f, 0f, this._screenHeight - base.Size.Y);
				}
			}
			else
			{
				Widget headGroupWidget2 = this.HeadGroupWidget;
				float num6 = ((headGroupWidget2 != null) ? headGroupWidget2.Size.Y : 0f);
				this.NameplateLayoutListPanel.ScaledPositionYOffset = this.Position.y - this.HeadPosition.y + num6;
				base.ScaledPositionXOffset = this.HeadPosition.x - base.Size.X / 2f;
				base.ScaledPositionYOffset = this.HeadPosition.y - num6;
				base.IsHidden = base.ScaledPositionXOffset > base.Context.TwoDimensionContext.Width || base.ScaledPositionYOffset > base.Context.TwoDimensionContext.Height || base.ScaledPositionXOffset + base.Size.X < 0f || base.ScaledPositionYOffset + base.Size.Y < 0f;
			}
			this.NameplateLayoutListPanel.PositionXOffset = (base.Size.X / 2f - this.PartyBannerWidget.Size.X) * base._inverseScaleToUse;
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x0001297D File Offset: 0x00010B7D
		private void UpdateTutorialStatus()
		{
			if (this._tutorialAnimState == PartyNameplateWidget.TutorialAnimState.Start)
			{
				this._tutorialAnimState = PartyNameplateWidget.TutorialAnimState.FirstFrame;
			}
			else
			{
				PartyNameplateWidget.TutorialAnimState tutorialAnimState = this._tutorialAnimState;
			}
			if (this.IsTargetedByTutorial)
			{
				this.SetState("Default");
				return;
			}
			this.SetState("Disabled");
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x000129BC File Offset: 0x00010BBC
		private bool IsNameplateOutsideScreen()
		{
			return this.Position.x + base.Size.X > this._screenWidth || this.Position.y - base.Size.Y > this._screenHeight || this.Position.x < 0f || this.Position.y < 0f || this.IsBehind || this.IsHigh;
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x00012A3B File Offset: 0x00010C3B
		private float LocalLerp(float start, float end, float delta)
		{
			if (Math.Abs(start - end) > 1E-45f)
			{
				return (end - start) * delta + start;
			}
			return end;
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06000624 RID: 1572 RVA: 0x00012A55 File Offset: 0x00010C55
		// (set) Token: 0x06000625 RID: 1573 RVA: 0x00012A5D File Offset: 0x00010C5D
		public ListPanel NameplateLayoutListPanel
		{
			get
			{
				return this._nameplateLayoutListPanel;
			}
			set
			{
				if (this._nameplateLayoutListPanel != value)
				{
					this._nameplateLayoutListPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "NameplateLayoutListPanel");
				}
			}
		}

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x06000626 RID: 1574 RVA: 0x00012A7B File Offset: 0x00010C7B
		// (set) Token: 0x06000627 RID: 1575 RVA: 0x00012A83 File Offset: 0x00010C83
		public MaskedTextureWidget PartyBannerWidget
		{
			get
			{
				return this._partyBannerWidget;
			}
			set
			{
				if (this._partyBannerWidget != value)
				{
					this._partyBannerWidget = value;
					base.OnPropertyChanged<MaskedTextureWidget>(value, "PartyBannerWidget");
				}
			}
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x06000628 RID: 1576 RVA: 0x00012AA1 File Offset: 0x00010CA1
		// (set) Token: 0x06000629 RID: 1577 RVA: 0x00012AA9 File Offset: 0x00010CA9
		public Widget TrackerFrame
		{
			get
			{
				return this._trackerFrame;
			}
			set
			{
				if (this._trackerFrame != value)
				{
					this._trackerFrame = value;
					base.OnPropertyChanged<Widget>(value, "TrackerFrame");
				}
			}
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x0600062A RID: 1578 RVA: 0x00012AC7 File Offset: 0x00010CC7
		// (set) Token: 0x0600062B RID: 1579 RVA: 0x00012ACF File Offset: 0x00010CCF
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

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x0600062C RID: 1580 RVA: 0x00012AF2 File Offset: 0x00010CF2
		// (set) Token: 0x0600062D RID: 1581 RVA: 0x00012AFA File Offset: 0x00010CFA
		public Vec2 HeadPosition
		{
			get
			{
				return this._headPosition;
			}
			set
			{
				if (this._headPosition != value)
				{
					this._headPosition = value;
					base.OnPropertyChanged(value, "HeadPosition");
				}
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x0600062E RID: 1582 RVA: 0x00012B1D File Offset: 0x00010D1D
		// (set) Token: 0x0600062F RID: 1583 RVA: 0x00012B25 File Offset: 0x00010D25
		public bool ShouldShowFullName
		{
			get
			{
				return this._shouldShowFullName;
			}
			set
			{
				if (this._shouldShowFullName != value)
				{
					this._shouldShowFullName = value;
					base.OnPropertyChanged(value, "ShouldShowFullName");
				}
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x06000630 RID: 1584 RVA: 0x00012B43 File Offset: 0x00010D43
		// (set) Token: 0x06000631 RID: 1585 RVA: 0x00012B4B File Offset: 0x00010D4B
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
					this._tutorialAnimState = PartyNameplateWidget.TutorialAnimState.Start;
				}
			}
		}

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x06000632 RID: 1586 RVA: 0x00012B70 File Offset: 0x00010D70
		// (set) Token: 0x06000633 RID: 1587 RVA: 0x00012B78 File Offset: 0x00010D78
		public bool IsInArmy
		{
			get
			{
				return this._isInArmy;
			}
			set
			{
				if (this._isInArmy != value)
				{
					this._isInArmy = value;
					base.OnPropertyChanged(value, "IsInArmy");
				}
			}
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x06000634 RID: 1588 RVA: 0x00012B96 File Offset: 0x00010D96
		// (set) Token: 0x06000635 RID: 1589 RVA: 0x00012B9E File Offset: 0x00010D9E
		public bool IsInSettlement
		{
			get
			{
				return this._isInSettlement;
			}
			set
			{
				if (this._isInSettlement != value)
				{
					this._isInSettlement = value;
					base.OnPropertyChanged(value, "IsInSettlement");
				}
			}
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06000636 RID: 1590 RVA: 0x00012BBC File Offset: 0x00010DBC
		// (set) Token: 0x06000637 RID: 1591 RVA: 0x00012BC4 File Offset: 0x00010DC4
		public bool IsArmy
		{
			get
			{
				return this._isArmy;
			}
			set
			{
				if (this._isArmy != value)
				{
					this._isArmy = value;
					base.OnPropertyChanged(value, "IsArmy");
				}
			}
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06000638 RID: 1592 RVA: 0x00012BE2 File Offset: 0x00010DE2
		// (set) Token: 0x06000639 RID: 1593 RVA: 0x00012BEA File Offset: 0x00010DEA
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
					this._isVisibleOnMap = value;
					base.OnPropertyChanged(value, "IsVisibleOnMap");
				}
			}
		}

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x0600063A RID: 1594 RVA: 0x00012C08 File Offset: 0x00010E08
		// (set) Token: 0x0600063B RID: 1595 RVA: 0x00012C10 File Offset: 0x00010E10
		public bool IsMainParty
		{
			get
			{
				return this._isMainParty;
			}
			set
			{
				if (this._isMainParty != value)
				{
					this._isMainParty = value;
					base.OnPropertyChanged(value, "IsMainParty");
				}
			}
		}

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x0600063C RID: 1596 RVA: 0x00012C2E File Offset: 0x00010E2E
		// (set) Token: 0x0600063D RID: 1597 RVA: 0x00012C36 File Offset: 0x00010E36
		public bool IsInside
		{
			get
			{
				return this._isInside;
			}
			set
			{
				if (this._isInside != value)
				{
					this._isInside = value;
					base.OnPropertyChanged(value, "IsInside");
				}
			}
		}

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x0600063E RID: 1598 RVA: 0x00012C54 File Offset: 0x00010E54
		// (set) Token: 0x0600063F RID: 1599 RVA: 0x00012C5C File Offset: 0x00010E5C
		public bool IsHigh
		{
			get
			{
				return this._isHigh;
			}
			set
			{
				if (this._isHigh != value)
				{
					this._isHigh = value;
					base.OnPropertyChanged(value, "IsHigh");
				}
			}
		}

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x06000640 RID: 1600 RVA: 0x00012C7A File Offset: 0x00010E7A
		// (set) Token: 0x06000641 RID: 1601 RVA: 0x00012C82 File Offset: 0x00010E82
		public bool IsBehind
		{
			get
			{
				return this._isBehind;
			}
			set
			{
				if (this._isBehind != value)
				{
					this._isBehind = value;
					base.OnPropertyChanged(value, "IsBehind");
				}
			}
		}

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x06000642 RID: 1602 RVA: 0x00012CA0 File Offset: 0x00010EA0
		// (set) Token: 0x06000643 RID: 1603 RVA: 0x00012CA8 File Offset: 0x00010EA8
		public bool IsPrisoner
		{
			get
			{
				return this._isPrisoner;
			}
			set
			{
				if (this._isPrisoner != value)
				{
					this._isPrisoner = value;
					base.OnPropertyChanged(value, "IsPrisoner");
				}
			}
		}

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x06000644 RID: 1604 RVA: 0x00012CC6 File Offset: 0x00010EC6
		// (set) Token: 0x06000645 RID: 1605 RVA: 0x00012CCE File Offset: 0x00010ECE
		public TextWidget NameplateTextWidget
		{
			get
			{
				return this._nameplateTextWidget;
			}
			set
			{
				if (this._nameplateTextWidget != value)
				{
					this._nameplateTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "NameplateTextWidget");
				}
			}
		}

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06000646 RID: 1606 RVA: 0x00012CEC File Offset: 0x00010EEC
		// (set) Token: 0x06000647 RID: 1607 RVA: 0x00012CF4 File Offset: 0x00010EF4
		public TextWidget NameplateExtraInfoTextWidget
		{
			get
			{
				return this._nameplateExtraInfoTextWidget;
			}
			set
			{
				if (this._nameplateExtraInfoTextWidget != value)
				{
					this._nameplateExtraInfoTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "NameplateExtraInfoTextWidget");
				}
			}
		}

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06000648 RID: 1608 RVA: 0x00012D12 File Offset: 0x00010F12
		// (set) Token: 0x06000649 RID: 1609 RVA: 0x00012D1A File Offset: 0x00010F1A
		public TextWidget NameplateFullNameTextWidget
		{
			get
			{
				return this._nameplateFullNameTextWidget;
			}
			set
			{
				if (this._nameplateFullNameTextWidget != value)
				{
					this._nameplateFullNameTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "NameplateFullNameTextWidget");
				}
			}
		}

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x0600064A RID: 1610 RVA: 0x00012D38 File Offset: 0x00010F38
		// (set) Token: 0x0600064B RID: 1611 RVA: 0x00012D40 File Offset: 0x00010F40
		public TextWidget SpeedTextWidget
		{
			get
			{
				return this._speedTextWidget;
			}
			set
			{
				if (this._speedTextWidget != value)
				{
					this._speedTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "SpeedTextWidget");
				}
			}
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x0600064C RID: 1612 RVA: 0x00012D5E File Offset: 0x00010F5E
		// (set) Token: 0x0600064D RID: 1613 RVA: 0x00012D66 File Offset: 0x00010F66
		public Widget SpeedIconWidget
		{
			get
			{
				return this._speedIconWidget;
			}
			set
			{
				if (value != this._speedIconWidget)
				{
					this._speedIconWidget = value;
					base.OnPropertyChanged<Widget>(value, "SpeedIconWidget");
				}
			}
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x0600064E RID: 1614 RVA: 0x00012D84 File Offset: 0x00010F84
		// (set) Token: 0x0600064F RID: 1615 RVA: 0x00012D8C File Offset: 0x00010F8C
		public Widget MainPartyArrowWidget
		{
			get
			{
				return this._mainPartyArrowWidget;
			}
			set
			{
				if (this._mainPartyArrowWidget != value)
				{
					this._mainPartyArrowWidget = value;
					base.OnPropertyChanged<Widget>(value, "MainPartyArrowWidget");
				}
			}
		}

		// Token: 0x040002A6 RID: 678
		private bool _isFirstFrame = true;

		// Token: 0x040002A7 RID: 679
		private float _screenWidth;

		// Token: 0x040002A8 RID: 680
		private float _screenHeight;

		// Token: 0x040002A9 RID: 681
		private bool _latestIsOutside;

		// Token: 0x040002AA RID: 682
		private float _initialDelayAmount = 2f;

		// Token: 0x040002AB RID: 683
		private int _defaultNameplateFontSize;

		// Token: 0x040002AC RID: 684
		private PartyNameplateWidget.TutorialAnimState _tutorialAnimState;

		// Token: 0x040002AE RID: 686
		private Vec2 _position;

		// Token: 0x040002AF RID: 687
		private Vec2 _headPosition;

		// Token: 0x040002B0 RID: 688
		private TextWidget _nameplateTextWidget;

		// Token: 0x040002B1 RID: 689
		private TextWidget _nameplateFullNameTextWidget;

		// Token: 0x040002B2 RID: 690
		private TextWidget _speedTextWidget;

		// Token: 0x040002B3 RID: 691
		private Widget _speedIconWidget;

		// Token: 0x040002B4 RID: 692
		private TextWidget _nameplateExtraInfoTextWidget;

		// Token: 0x040002B5 RID: 693
		private Widget _trackerFrame;

		// Token: 0x040002B6 RID: 694
		private Widget _mainPartyArrowWidget;

		// Token: 0x040002B7 RID: 695
		private ListPanel _nameplateLayoutListPanel;

		// Token: 0x040002B8 RID: 696
		private MaskedTextureWidget _partyBannerWidget;

		// Token: 0x040002B9 RID: 697
		private bool _isVisibleOnMap;

		// Token: 0x040002BA RID: 698
		private bool _isMainParty;

		// Token: 0x040002BB RID: 699
		private bool _isInside;

		// Token: 0x040002BC RID: 700
		private bool _isBehind;

		// Token: 0x040002BD RID: 701
		private bool _isHigh;

		// Token: 0x040002BE RID: 702
		private bool _isInArmy;

		// Token: 0x040002BF RID: 703
		private bool _isInSettlement;

		// Token: 0x040002C0 RID: 704
		private bool _isArmy;

		// Token: 0x040002C1 RID: 705
		private bool _isTargetedByTutorial;

		// Token: 0x040002C2 RID: 706
		private bool _shouldShowFullName;

		// Token: 0x040002C3 RID: 707
		private bool _isPrisoner;

		// Token: 0x02000189 RID: 393
		public enum TutorialAnimState
		{
			// Token: 0x040008DF RID: 2271
			Idle,
			// Token: 0x040008E0 RID: 2272
			Start,
			// Token: 0x040008E1 RID: 2273
			FirstFrame,
			// Token: 0x040008E2 RID: 2274
			Playing
		}
	}
}
