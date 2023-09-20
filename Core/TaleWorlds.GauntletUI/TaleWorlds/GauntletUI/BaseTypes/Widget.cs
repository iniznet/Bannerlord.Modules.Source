using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x0200006D RID: 109
	public class Widget : PropertyOwnerObject
	{
		// Token: 0x17000209 RID: 521
		// (get) Token: 0x06000711 RID: 1809 RVA: 0x0001F04A File Offset: 0x0001D24A
		// (set) Token: 0x06000712 RID: 1810 RVA: 0x0001F052 File Offset: 0x0001D252
		public float ColorFactor { get; set; } = 1f;

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x06000713 RID: 1811 RVA: 0x0001F05B File Offset: 0x0001D25B
		// (set) Token: 0x06000714 RID: 1812 RVA: 0x0001F063 File Offset: 0x0001D263
		public float AlphaFactor { get; set; } = 1f;

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x06000715 RID: 1813 RVA: 0x0001F06C File Offset: 0x0001D26C
		// (set) Token: 0x06000716 RID: 1814 RVA: 0x0001F074 File Offset: 0x0001D274
		public float ValueFactor { get; set; }

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x06000717 RID: 1815 RVA: 0x0001F07D File Offset: 0x0001D27D
		// (set) Token: 0x06000718 RID: 1816 RVA: 0x0001F085 File Offset: 0x0001D285
		public float SaturationFactor { get; set; }

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x06000719 RID: 1817 RVA: 0x0001F08E File Offset: 0x0001D28E
		// (set) Token: 0x0600071A RID: 1818 RVA: 0x0001F096 File Offset: 0x0001D296
		public float ExtendLeft { get; set; }

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x0600071B RID: 1819 RVA: 0x0001F09F File Offset: 0x0001D29F
		// (set) Token: 0x0600071C RID: 1820 RVA: 0x0001F0A7 File Offset: 0x0001D2A7
		public float ExtendRight { get; set; }

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x0600071D RID: 1821 RVA: 0x0001F0B0 File Offset: 0x0001D2B0
		// (set) Token: 0x0600071E RID: 1822 RVA: 0x0001F0B8 File Offset: 0x0001D2B8
		public float ExtendTop { get; set; }

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x0600071F RID: 1823 RVA: 0x0001F0C1 File Offset: 0x0001D2C1
		// (set) Token: 0x06000720 RID: 1824 RVA: 0x0001F0C9 File Offset: 0x0001D2C9
		public float ExtendBottom { get; set; }

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x06000721 RID: 1825 RVA: 0x0001F0D2 File Offset: 0x0001D2D2
		// (set) Token: 0x06000722 RID: 1826 RVA: 0x0001F0DA File Offset: 0x0001D2DA
		public bool VerticalFlip { get; set; }

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x06000723 RID: 1827 RVA: 0x0001F0E3 File Offset: 0x0001D2E3
		// (set) Token: 0x06000724 RID: 1828 RVA: 0x0001F0EB File Offset: 0x0001D2EB
		public bool HorizontalFlip { get; set; }

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x06000725 RID: 1829 RVA: 0x0001F0F4 File Offset: 0x0001D2F4
		// (set) Token: 0x06000726 RID: 1830 RVA: 0x0001F0FC File Offset: 0x0001D2FC
		public bool FrictionEnabled { get; set; }

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x06000727 RID: 1831 RVA: 0x0001F105 File Offset: 0x0001D305
		// (set) Token: 0x06000728 RID: 1832 RVA: 0x0001F10D File Offset: 0x0001D30D
		public Color Color
		{
			get
			{
				return this._color;
			}
			set
			{
				if (this._color != value)
				{
					this._color = value;
				}
			}
		}

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x06000729 RID: 1833 RVA: 0x0001F124 File Offset: 0x0001D324
		// (set) Token: 0x0600072A RID: 1834 RVA: 0x0001F12C File Offset: 0x0001D32C
		[Editor(false)]
		public string Id
		{
			get
			{
				return this._id;
			}
			set
			{
				if (this._id != value)
				{
					this._id = value;
					base.OnPropertyChanged<string>(value, "Id");
				}
			}
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x0600072B RID: 1835 RVA: 0x0001F14F File Offset: 0x0001D34F
		// (set) Token: 0x0600072C RID: 1836 RVA: 0x0001F157 File Offset: 0x0001D357
		public Vector2 LocalPosition { get; private set; }

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x0600072D RID: 1837 RVA: 0x0001F160 File Offset: 0x0001D360
		public Vector2 GlobalPosition
		{
			get
			{
				if (this.ParentWidget != null)
				{
					return this.LocalPosition + this.ParentWidget.GlobalPosition;
				}
				return this.LocalPosition;
			}
		}

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x0600072E RID: 1838 RVA: 0x0001F187 File Offset: 0x0001D387
		// (set) Token: 0x0600072F RID: 1839 RVA: 0x0001F190 File Offset: 0x0001D390
		[Editor(false)]
		public bool DoNotUseCustomScaleAndChildren
		{
			get
			{
				return this._doNotUseCustomScaleAndChildren;
			}
			set
			{
				if (this._doNotUseCustomScaleAndChildren != value)
				{
					this._doNotUseCustomScaleAndChildren = value;
					base.OnPropertyChanged(value, "DoNotUseCustomScaleAndChildren");
					this.DoNotUseCustomScale = value;
					this._children.ForEach(delegate(Widget _)
					{
						_.DoNotUseCustomScaleAndChildren = value;
					});
				}
			}
		}

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06000730 RID: 1840 RVA: 0x0001F1F8 File Offset: 0x0001D3F8
		// (set) Token: 0x06000731 RID: 1841 RVA: 0x0001F200 File Offset: 0x0001D400
		public bool DoNotUseCustomScale { get; set; }

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x06000732 RID: 1842 RVA: 0x0001F209 File Offset: 0x0001D409
		protected float _scaleToUse
		{
			get
			{
				if (!this.DoNotUseCustomScale)
				{
					return this.Context.CustomScale;
				}
				return this.Context.Scale;
			}
		}

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x06000733 RID: 1843 RVA: 0x0001F22A File Offset: 0x0001D42A
		protected float _inverseScaleToUse
		{
			get
			{
				if (!this.DoNotUseCustomScale)
				{
					return this.Context.CustomInverseScale;
				}
				return this.Context.InverseScale;
			}
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x06000734 RID: 1844 RVA: 0x0001F24B File Offset: 0x0001D44B
		// (set) Token: 0x06000735 RID: 1845 RVA: 0x0001F253 File Offset: 0x0001D453
		public Vector2 Size { get; private set; }

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06000736 RID: 1846 RVA: 0x0001F25C File Offset: 0x0001D45C
		// (set) Token: 0x06000737 RID: 1847 RVA: 0x0001F264 File Offset: 0x0001D464
		[Editor(false)]
		public float SuggestedWidth
		{
			get
			{
				return this._suggestedWidth;
			}
			set
			{
				if (this._suggestedWidth != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._suggestedWidth = value;
					base.OnPropertyChanged(value, "SuggestedWidth");
				}
			}
		}

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06000738 RID: 1848 RVA: 0x0001F288 File Offset: 0x0001D488
		// (set) Token: 0x06000739 RID: 1849 RVA: 0x0001F290 File Offset: 0x0001D490
		[Editor(false)]
		public float SuggestedHeight
		{
			get
			{
				return this._suggestedHeight;
			}
			set
			{
				if (this._suggestedHeight != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._suggestedHeight = value;
					base.OnPropertyChanged(value, "SuggestedHeight");
				}
			}
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x0600073A RID: 1850 RVA: 0x0001F2B4 File Offset: 0x0001D4B4
		// (set) Token: 0x0600073B RID: 1851 RVA: 0x0001F2C3 File Offset: 0x0001D4C3
		public float ScaledSuggestedWidth
		{
			get
			{
				return this._scaleToUse * this.SuggestedWidth;
			}
			set
			{
				this.SuggestedWidth = value * this._inverseScaleToUse;
			}
		}

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x0600073C RID: 1852 RVA: 0x0001F2D3 File Offset: 0x0001D4D3
		// (set) Token: 0x0600073D RID: 1853 RVA: 0x0001F2E2 File Offset: 0x0001D4E2
		public float ScaledSuggestedHeight
		{
			get
			{
				return this._scaleToUse * this.SuggestedHeight;
			}
			set
			{
				this.SuggestedHeight = value * this._inverseScaleToUse;
			}
		}

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x0600073E RID: 1854 RVA: 0x0001F2F2 File Offset: 0x0001D4F2
		// (set) Token: 0x0600073F RID: 1855 RVA: 0x0001F2FC File Offset: 0x0001D4FC
		[Editor(false)]
		public bool TweenPosition
		{
			get
			{
				return this._tweenPosition;
			}
			set
			{
				if (this._tweenPosition != value)
				{
					bool tweenPosition = this._tweenPosition;
					this._tweenPosition = value;
					if (this.ConnectedToRoot && (!tweenPosition || !this._tweenPosition))
					{
						this.EventManager.OnWidgetTweenPositionChanged(this);
					}
				}
			}
		}

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06000740 RID: 1856 RVA: 0x0001F33F File Offset: 0x0001D53F
		// (set) Token: 0x06000741 RID: 1857 RVA: 0x0001F347 File Offset: 0x0001D547
		[Editor(false)]
		public string HoveredCursorState
		{
			get
			{
				return this._hoveredCursorState;
			}
			set
			{
				if (this._hoveredCursorState != value)
				{
					string hoveredCursorState = this._hoveredCursorState;
					this._hoveredCursorState = value;
				}
			}
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06000742 RID: 1858 RVA: 0x0001F365 File Offset: 0x0001D565
		// (set) Token: 0x06000743 RID: 1859 RVA: 0x0001F36D File Offset: 0x0001D56D
		[Editor(false)]
		public bool AlternateClickEventHasSpecialEvent
		{
			get
			{
				return this._alternateClickEventHasSpecialEvent;
			}
			set
			{
				if (this._alternateClickEventHasSpecialEvent != value)
				{
					bool alternateClickEventHasSpecialEvent = this._alternateClickEventHasSpecialEvent;
					this._alternateClickEventHasSpecialEvent = value;
				}
			}
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06000744 RID: 1860 RVA: 0x0001F386 File Offset: 0x0001D586
		// (set) Token: 0x06000745 RID: 1861 RVA: 0x0001F38E File Offset: 0x0001D58E
		public Vector2 PosOffset
		{
			get
			{
				return this._positionOffset;
			}
			set
			{
				if (this._positionOffset != value)
				{
					this.SetLayoutDirty();
					this._positionOffset = value;
					base.OnPropertyChanged(value, "PosOffset");
				}
			}
		}

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x06000746 RID: 1862 RVA: 0x0001F3B7 File Offset: 0x0001D5B7
		public Vector2 ScaledPositionOffset
		{
			get
			{
				return this._positionOffset * this._scaleToUse;
			}
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x06000747 RID: 1863 RVA: 0x0001F3CA File Offset: 0x0001D5CA
		// (set) Token: 0x06000748 RID: 1864 RVA: 0x0001F3D7 File Offset: 0x0001D5D7
		[Editor(false)]
		public float PositionXOffset
		{
			get
			{
				return this._positionOffset.X;
			}
			set
			{
				if (this._positionOffset.X != value)
				{
					this.SetLayoutDirty();
					this._positionOffset.X = value;
					base.OnPropertyChanged(value, "PositionXOffset");
				}
			}
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x06000749 RID: 1865 RVA: 0x0001F405 File Offset: 0x0001D605
		// (set) Token: 0x0600074A RID: 1866 RVA: 0x0001F412 File Offset: 0x0001D612
		[Editor(false)]
		public float PositionYOffset
		{
			get
			{
				return this._positionOffset.Y;
			}
			set
			{
				if (this._positionOffset.Y != value)
				{
					this.SetLayoutDirty();
					this._positionOffset.Y = value;
					base.OnPropertyChanged(value, "PositionYOffset");
				}
			}
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x0600074B RID: 1867 RVA: 0x0001F440 File Offset: 0x0001D640
		// (set) Token: 0x0600074C RID: 1868 RVA: 0x0001F454 File Offset: 0x0001D654
		public float ScaledPositionXOffset
		{
			get
			{
				return this._positionOffset.X * this._scaleToUse;
			}
			set
			{
				float num = value * this._inverseScaleToUse;
				if (num != this._positionOffset.X)
				{
					this.SetLayoutDirty();
					this._positionOffset.X = num;
				}
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x0600074D RID: 1869 RVA: 0x0001F48A File Offset: 0x0001D68A
		// (set) Token: 0x0600074E RID: 1870 RVA: 0x0001F4A0 File Offset: 0x0001D6A0
		public float ScaledPositionYOffset
		{
			get
			{
				return this._positionOffset.Y * this._scaleToUse;
			}
			set
			{
				float num = value * this._inverseScaleToUse;
				if (num != this._positionOffset.Y)
				{
					this.SetLayoutDirty();
					this._positionOffset.Y = num;
				}
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x0600074F RID: 1871 RVA: 0x0001F4D6 File Offset: 0x0001D6D6
		// (set) Token: 0x06000750 RID: 1872 RVA: 0x0001F4E0 File Offset: 0x0001D6E0
		public Widget ParentWidget
		{
			get
			{
				return this._parent;
			}
			set
			{
				if (this.ParentWidget != value)
				{
					if (this._parent != null)
					{
						this._parent.OnChildRemoved(this);
						if (this.ConnectedToRoot)
						{
							this.EventManager.OnWidgetDisconnectedFromRoot(this);
						}
						this._parent._children.Remove(this);
						this._parent.OnAfterChildRemoved(this);
					}
					this._parent = value;
					if (this._parent != null)
					{
						this._parent._children.Add(this);
						if (this.ConnectedToRoot)
						{
							this.EventManager.OnWidgetConnectedToRoot(this);
						}
						this._parent.OnChildAdded(this);
					}
					this.SetMeasureAndLayoutDirty();
				}
			}
		}

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x06000751 RID: 1873 RVA: 0x0001F585 File Offset: 0x0001D785
		public EventManager EventManager
		{
			get
			{
				return this.Context.EventManager;
			}
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x06000752 RID: 1874 RVA: 0x0001F592 File Offset: 0x0001D792
		// (set) Token: 0x06000753 RID: 1875 RVA: 0x0001F59A File Offset: 0x0001D79A
		public UIContext Context { get; private set; }

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06000754 RID: 1876 RVA: 0x0001F5A3 File Offset: 0x0001D7A3
		// (set) Token: 0x06000755 RID: 1877 RVA: 0x0001F5AB File Offset: 0x0001D7AB
		public Vector2 MeasuredSize { get; private set; }

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06000756 RID: 1878 RVA: 0x0001F5B4 File Offset: 0x0001D7B4
		// (set) Token: 0x06000757 RID: 1879 RVA: 0x0001F5BC File Offset: 0x0001D7BC
		[Editor(false)]
		public float MarginTop
		{
			get
			{
				return this._marginTop;
			}
			set
			{
				if (this._marginTop != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._marginTop = value;
					base.OnPropertyChanged(value, "MarginTop");
				}
			}
		}

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x06000758 RID: 1880 RVA: 0x0001F5E0 File Offset: 0x0001D7E0
		// (set) Token: 0x06000759 RID: 1881 RVA: 0x0001F5E8 File Offset: 0x0001D7E8
		[Editor(false)]
		public float MarginLeft
		{
			get
			{
				return this._marginLeft;
			}
			set
			{
				if (this._marginLeft != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._marginLeft = value;
					base.OnPropertyChanged(value, "MarginLeft");
				}
			}
		}

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x0600075A RID: 1882 RVA: 0x0001F60C File Offset: 0x0001D80C
		// (set) Token: 0x0600075B RID: 1883 RVA: 0x0001F614 File Offset: 0x0001D814
		[Editor(false)]
		public float MarginBottom
		{
			get
			{
				return this._marginBottom;
			}
			set
			{
				if (this._marginBottom != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._marginBottom = value;
					base.OnPropertyChanged(value, "MarginBottom");
				}
			}
		}

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x0600075C RID: 1884 RVA: 0x0001F638 File Offset: 0x0001D838
		// (set) Token: 0x0600075D RID: 1885 RVA: 0x0001F640 File Offset: 0x0001D840
		[Editor(false)]
		public float MarginRight
		{
			get
			{
				return this._marginRight;
			}
			set
			{
				if (this._marginRight != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._marginRight = value;
					base.OnPropertyChanged(value, "MarginRight");
				}
			}
		}

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x0600075E RID: 1886 RVA: 0x0001F664 File Offset: 0x0001D864
		public float ScaledMarginTop
		{
			get
			{
				return this._scaleToUse * this.MarginTop;
			}
		}

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x0600075F RID: 1887 RVA: 0x0001F673 File Offset: 0x0001D873
		public float ScaledMarginLeft
		{
			get
			{
				return this._scaleToUse * this.MarginLeft;
			}
		}

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x06000760 RID: 1888 RVA: 0x0001F682 File Offset: 0x0001D882
		public float ScaledMarginBottom
		{
			get
			{
				return this._scaleToUse * this.MarginBottom;
			}
		}

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06000761 RID: 1889 RVA: 0x0001F691 File Offset: 0x0001D891
		public float ScaledMarginRight
		{
			get
			{
				return this._scaleToUse * this.MarginRight;
			}
		}

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06000762 RID: 1890 RVA: 0x0001F6A0 File Offset: 0x0001D8A0
		// (set) Token: 0x06000763 RID: 1891 RVA: 0x0001F6A8 File Offset: 0x0001D8A8
		[Editor(false)]
		public VerticalAlignment VerticalAlignment
		{
			get
			{
				return this._verticalAlignment;
			}
			set
			{
				if (this._verticalAlignment != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._verticalAlignment = value;
					base.OnPropertyChanged<string>(Enum.GetName(typeof(VerticalAlignment), value), "VerticalAlignment");
				}
			}
		}

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06000764 RID: 1892 RVA: 0x0001F6E0 File Offset: 0x0001D8E0
		// (set) Token: 0x06000765 RID: 1893 RVA: 0x0001F6E8 File Offset: 0x0001D8E8
		[Editor(false)]
		public HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return this._horizontalAlignment;
			}
			set
			{
				if (this._horizontalAlignment != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._horizontalAlignment = value;
					base.OnPropertyChanged<string>(Enum.GetName(typeof(HorizontalAlignment), value), "HorizontalAlignment");
				}
			}
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06000766 RID: 1894 RVA: 0x0001F720 File Offset: 0x0001D920
		// (set) Token: 0x06000767 RID: 1895 RVA: 0x0001F72D File Offset: 0x0001D92D
		public float Left
		{
			get
			{
				return this._topLeft.X;
			}
			private set
			{
				if (value != this._topLeft.X)
				{
					this.EventManager.SetPositionsDirty();
					this._topLeft.X = value;
				}
			}
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x06000768 RID: 1896 RVA: 0x0001F754 File Offset: 0x0001D954
		// (set) Token: 0x06000769 RID: 1897 RVA: 0x0001F761 File Offset: 0x0001D961
		public float Top
		{
			get
			{
				return this._topLeft.Y;
			}
			private set
			{
				if (value != this._topLeft.Y)
				{
					this.EventManager.SetPositionsDirty();
					this._topLeft.Y = value;
				}
			}
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x0600076A RID: 1898 RVA: 0x0001F788 File Offset: 0x0001D988
		public float Right
		{
			get
			{
				return this._topLeft.X + this.Size.X;
			}
		}

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x0600076B RID: 1899 RVA: 0x0001F7A1 File Offset: 0x0001D9A1
		public float Bottom
		{
			get
			{
				return this._topLeft.Y + this.Size.Y;
			}
		}

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x0600076C RID: 1900 RVA: 0x0001F7BA File Offset: 0x0001D9BA
		public int ChildCount
		{
			get
			{
				return this._children.Count;
			}
		}

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x0600076D RID: 1901 RVA: 0x0001F7C7 File Offset: 0x0001D9C7
		// (set) Token: 0x0600076E RID: 1902 RVA: 0x0001F7CF File Offset: 0x0001D9CF
		[Editor(false)]
		public bool ForcePixelPerfectRenderPlacement
		{
			get
			{
				return this._forcePixelPerfectRenderPlacement;
			}
			set
			{
				if (this._forcePixelPerfectRenderPlacement != value)
				{
					this._forcePixelPerfectRenderPlacement = value;
					base.OnPropertyChanged(value, "ForcePixelPerfectRenderPlacement");
				}
			}
		}

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x0600076F RID: 1903 RVA: 0x0001F7ED File Offset: 0x0001D9ED
		// (set) Token: 0x06000770 RID: 1904 RVA: 0x0001F7F5 File Offset: 0x0001D9F5
		public bool UseGlobalTimeForAnimation { get; set; }

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x06000771 RID: 1905 RVA: 0x0001F7FE File Offset: 0x0001D9FE
		// (set) Token: 0x06000772 RID: 1906 RVA: 0x0001F806 File Offset: 0x0001DA06
		[Editor(false)]
		public SizePolicy WidthSizePolicy
		{
			get
			{
				return this._widthSizePolicy;
			}
			set
			{
				if (value != this._widthSizePolicy)
				{
					this.SetMeasureAndLayoutDirty();
					this._widthSizePolicy = value;
				}
			}
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x06000773 RID: 1907 RVA: 0x0001F81E File Offset: 0x0001DA1E
		// (set) Token: 0x06000774 RID: 1908 RVA: 0x0001F826 File Offset: 0x0001DA26
		[Editor(false)]
		public SizePolicy HeightSizePolicy
		{
			get
			{
				return this._heightSizePolicy;
			}
			set
			{
				if (value != this._heightSizePolicy)
				{
					this.SetMeasureAndLayoutDirty();
					this._heightSizePolicy = value;
				}
			}
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06000775 RID: 1909 RVA: 0x0001F83E File Offset: 0x0001DA3E
		// (set) Token: 0x06000776 RID: 1910 RVA: 0x0001F846 File Offset: 0x0001DA46
		[Editor(false)]
		public bool AcceptDrag { get; set; }

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06000777 RID: 1911 RVA: 0x0001F84F File Offset: 0x0001DA4F
		// (set) Token: 0x06000778 RID: 1912 RVA: 0x0001F857 File Offset: 0x0001DA57
		[Editor(false)]
		public bool AcceptDrop { get; set; }

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06000779 RID: 1913 RVA: 0x0001F860 File Offset: 0x0001DA60
		// (set) Token: 0x0600077A RID: 1914 RVA: 0x0001F868 File Offset: 0x0001DA68
		[Editor(false)]
		public bool HideOnDrag { get; set; } = true;

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x0600077B RID: 1915 RVA: 0x0001F871 File Offset: 0x0001DA71
		// (set) Token: 0x0600077C RID: 1916 RVA: 0x0001F879 File Offset: 0x0001DA79
		[Editor(false)]
		public Widget DragWidget
		{
			get
			{
				return this._dragWidget;
			}
			set
			{
				if (this._dragWidget != value)
				{
					if (value != null)
					{
						this._dragWidget = value;
						this._dragWidget.IsVisible = false;
						return;
					}
					this._dragWidget = null;
				}
			}
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x0600077D RID: 1917 RVA: 0x0001F8A2 File Offset: 0x0001DAA2
		// (set) Token: 0x0600077E RID: 1918 RVA: 0x0001F8B4 File Offset: 0x0001DAB4
		[Editor(false)]
		public bool ClipContents
		{
			get
			{
				return this.ClipVerticalContent && this.ClipHorizontalContent;
			}
			set
			{
				this.ClipHorizontalContent = value;
				this.ClipVerticalContent = value;
			}
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x0600077F RID: 1919 RVA: 0x0001F8C4 File Offset: 0x0001DAC4
		// (set) Token: 0x06000780 RID: 1920 RVA: 0x0001F8CC File Offset: 0x0001DACC
		[Editor(false)]
		public bool ClipHorizontalContent { get; set; }

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x06000781 RID: 1921 RVA: 0x0001F8D5 File Offset: 0x0001DAD5
		// (set) Token: 0x06000782 RID: 1922 RVA: 0x0001F8DD File Offset: 0x0001DADD
		[Editor(false)]
		public bool ClipVerticalContent { get; set; }

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x06000783 RID: 1923 RVA: 0x0001F8E6 File Offset: 0x0001DAE6
		// (set) Token: 0x06000784 RID: 1924 RVA: 0x0001F8EE File Offset: 0x0001DAEE
		[Editor(false)]
		public bool CircularClipEnabled { get; set; }

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06000785 RID: 1925 RVA: 0x0001F8F7 File Offset: 0x0001DAF7
		// (set) Token: 0x06000786 RID: 1926 RVA: 0x0001F8FF File Offset: 0x0001DAFF
		[Editor(false)]
		public float CircularClipRadius { get; set; }

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x06000787 RID: 1927 RVA: 0x0001F908 File Offset: 0x0001DB08
		// (set) Token: 0x06000788 RID: 1928 RVA: 0x0001F910 File Offset: 0x0001DB10
		[Editor(false)]
		public bool IsCircularClipRadiusHalfOfWidth { get; set; }

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06000789 RID: 1929 RVA: 0x0001F919 File Offset: 0x0001DB19
		// (set) Token: 0x0600078A RID: 1930 RVA: 0x0001F921 File Offset: 0x0001DB21
		[Editor(false)]
		public bool IsCircularClipRadiusHalfOfHeight { get; set; }

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x0600078B RID: 1931 RVA: 0x0001F92A File Offset: 0x0001DB2A
		// (set) Token: 0x0600078C RID: 1932 RVA: 0x0001F932 File Offset: 0x0001DB32
		[Editor(false)]
		public float CircularClipSmoothingRadius { get; set; }

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x0600078D RID: 1933 RVA: 0x0001F93B File Offset: 0x0001DB3B
		// (set) Token: 0x0600078E RID: 1934 RVA: 0x0001F943 File Offset: 0x0001DB43
		[Editor(false)]
		public float CircularClipXOffset { get; set; }

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x0600078F RID: 1935 RVA: 0x0001F94C File Offset: 0x0001DB4C
		// (set) Token: 0x06000790 RID: 1936 RVA: 0x0001F954 File Offset: 0x0001DB54
		[Editor(false)]
		public float CircularClipYOffset { get; set; }

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x06000791 RID: 1937 RVA: 0x0001F95D File Offset: 0x0001DB5D
		// (set) Token: 0x06000792 RID: 1938 RVA: 0x0001F965 File Offset: 0x0001DB65
		[Editor(false)]
		public bool RenderLate { get; set; }

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x06000793 RID: 1939 RVA: 0x0001F96E File Offset: 0x0001DB6E
		// (set) Token: 0x06000794 RID: 1940 RVA: 0x0001F976 File Offset: 0x0001DB76
		[Editor(false)]
		public bool DoNotRenderIfNotFullyInsideScissor { get; set; }

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x06000795 RID: 1941 RVA: 0x0001F97F File Offset: 0x0001DB7F
		public bool FixedWidth
		{
			get
			{
				return this.WidthSizePolicy == SizePolicy.Fixed;
			}
		}

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x06000796 RID: 1942 RVA: 0x0001F98A File Offset: 0x0001DB8A
		public bool FixedHeight
		{
			get
			{
				return this.HeightSizePolicy == SizePolicy.Fixed;
			}
		}

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x06000797 RID: 1943 RVA: 0x0001F995 File Offset: 0x0001DB95
		// (set) Token: 0x06000798 RID: 1944 RVA: 0x0001F99D File Offset: 0x0001DB9D
		public bool IsHovered
		{
			get
			{
				return this._isHovered;
			}
			private set
			{
				if (this._isHovered != value)
				{
					this._isHovered = value;
					this.RefreshState();
					base.OnPropertyChanged(value, "IsHovered");
				}
			}
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x06000799 RID: 1945 RVA: 0x0001F9C1 File Offset: 0x0001DBC1
		// (set) Token: 0x0600079A RID: 1946 RVA: 0x0001F9C9 File Offset: 0x0001DBC9
		[Editor(false)]
		public bool IsDisabled
		{
			get
			{
				return this._isDisabled;
			}
			set
			{
				if (this._isDisabled != value)
				{
					this._isDisabled = value;
					base.OnPropertyChanged(value, "IsDisabled");
					this.RefreshState();
				}
			}
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x0600079B RID: 1947 RVA: 0x0001F9ED File Offset: 0x0001DBED
		// (set) Token: 0x0600079C RID: 1948 RVA: 0x0001F9F5 File Offset: 0x0001DBF5
		[Editor(false)]
		public bool IsFocusable
		{
			get
			{
				return this._isFocusable;
			}
			set
			{
				if (this._isFocusable != value)
				{
					this._isFocusable = value;
					if (this.ConnectedToRoot)
					{
						base.OnPropertyChanged(value, "IsFocusable");
						this.RefreshState();
					}
				}
			}
		}

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x0600079D RID: 1949 RVA: 0x0001FA21 File Offset: 0x0001DC21
		// (set) Token: 0x0600079E RID: 1950 RVA: 0x0001FA29 File Offset: 0x0001DC29
		public bool IsFocused
		{
			get
			{
				return this._isFocused;
			}
			private set
			{
				if (this._isFocused != value)
				{
					this._isFocused = value;
					this.RefreshState();
				}
			}
		}

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x0600079F RID: 1951 RVA: 0x0001FA41 File Offset: 0x0001DC41
		// (set) Token: 0x060007A0 RID: 1952 RVA: 0x0001FA4C File Offset: 0x0001DC4C
		[Editor(false)]
		public bool IsEnabled
		{
			get
			{
				return !this.IsDisabled;
			}
			set
			{
				if (value == this.IsDisabled)
				{
					this.IsDisabled = !value;
					base.OnPropertyChanged(value, "IsEnabled");
				}
			}
		}

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x060007A1 RID: 1953 RVA: 0x0001FA6D File Offset: 0x0001DC6D
		// (set) Token: 0x060007A2 RID: 1954 RVA: 0x0001FA75 File Offset: 0x0001DC75
		[Editor(false)]
		public bool RestartAnimationFirstFrame
		{
			get
			{
				return this._restartAnimationFirstFrame;
			}
			set
			{
				if (this._restartAnimationFirstFrame != value)
				{
					this._restartAnimationFirstFrame = value;
				}
			}
		}

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x060007A3 RID: 1955 RVA: 0x0001FA87 File Offset: 0x0001DC87
		// (set) Token: 0x060007A4 RID: 1956 RVA: 0x0001FA8F File Offset: 0x0001DC8F
		[Editor(false)]
		public bool DoNotPassEventsToChildren
		{
			get
			{
				return this._doNotPassEventsToChildren;
			}
			set
			{
				if (this._doNotPassEventsToChildren != value)
				{
					this._doNotPassEventsToChildren = value;
					base.OnPropertyChanged(value, "DoNotPassEventsToChildren");
				}
			}
		}

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x060007A5 RID: 1957 RVA: 0x0001FAAD File Offset: 0x0001DCAD
		// (set) Token: 0x060007A6 RID: 1958 RVA: 0x0001FAB5 File Offset: 0x0001DCB5
		[Editor(false)]
		public bool DoNotAcceptEvents
		{
			get
			{
				return this._doNotAcceptEvents;
			}
			set
			{
				if (this._doNotAcceptEvents != value)
				{
					this._doNotAcceptEvents = value;
					base.OnPropertyChanged(value, "DoNotAcceptEvents");
				}
			}
		}

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x060007A7 RID: 1959 RVA: 0x0001FAD3 File Offset: 0x0001DCD3
		// (set) Token: 0x060007A8 RID: 1960 RVA: 0x0001FADE File Offset: 0x0001DCDE
		[Editor(false)]
		public bool CanAcceptEvents
		{
			get
			{
				return !this.DoNotAcceptEvents;
			}
			set
			{
				this.DoNotAcceptEvents = !value;
			}
		}

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x060007A9 RID: 1961 RVA: 0x0001FAEA File Offset: 0x0001DCEA
		// (set) Token: 0x060007AA RID: 1962 RVA: 0x0001FAF2 File Offset: 0x0001DCF2
		public bool IsPressed
		{
			get
			{
				return this._isPressed;
			}
			internal set
			{
				if (this._isPressed != value)
				{
					this._isPressed = value;
					this.RefreshState();
				}
			}
		}

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x060007AB RID: 1963 RVA: 0x0001FB0A File Offset: 0x0001DD0A
		// (set) Token: 0x060007AC RID: 1964 RVA: 0x0001FB14 File Offset: 0x0001DD14
		[Editor(false)]
		public bool IsHidden
		{
			get
			{
				return this._isHidden;
			}
			set
			{
				if (this._isHidden != value)
				{
					this.SetMeasureAndLayoutDirty();
					this._isHidden = value;
					this.RefreshState();
					base.OnPropertyChanged(value, "IsHidden");
					base.OnPropertyChanged(!value, "IsVisible");
					if (this.OnVisibilityChanged != null)
					{
						this.OnVisibilityChanged(this);
					}
				}
			}
		}

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x060007AD RID: 1965 RVA: 0x0001FB6C File Offset: 0x0001DD6C
		// (set) Token: 0x060007AE RID: 1966 RVA: 0x0001FB77 File Offset: 0x0001DD77
		[Editor(false)]
		public bool IsVisible
		{
			get
			{
				return !this._isHidden;
			}
			set
			{
				if (value == this._isHidden)
				{
					this.IsHidden = !value;
				}
			}
		}

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x060007AF RID: 1967 RVA: 0x0001FB8C File Offset: 0x0001DD8C
		// (set) Token: 0x060007B0 RID: 1968 RVA: 0x0001FB94 File Offset: 0x0001DD94
		[Editor(false)]
		public Sprite Sprite
		{
			get
			{
				return this._sprite;
			}
			set
			{
				if (value != this._sprite)
				{
					this._sprite = value;
				}
			}
		}

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x060007B1 RID: 1969 RVA: 0x0001FBA6 File Offset: 0x0001DDA6
		// (set) Token: 0x060007B2 RID: 1970 RVA: 0x0001FBB0 File Offset: 0x0001DDB0
		[Editor(false)]
		public VisualDefinition VisualDefinition
		{
			get
			{
				return this._visualDefinition;
			}
			set
			{
				if (this._visualDefinition != value)
				{
					VisualDefinition visualDefinition = this._visualDefinition;
					this._visualDefinition = value;
					this._stateTimer = 0f;
					if (this.ConnectedToRoot && (visualDefinition == null || this._visualDefinition == null))
					{
						this.EventManager.OnWidgetVisualDefinitionChanged(this);
					}
				}
			}
		}

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x060007B3 RID: 1971 RVA: 0x0001FBFE File Offset: 0x0001DDFE
		// (set) Token: 0x060007B4 RID: 1972 RVA: 0x0001FC06 File Offset: 0x0001DE06
		public string CurrentState { get; protected set; } = "";

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x060007B5 RID: 1973 RVA: 0x0001FC0F File Offset: 0x0001DE0F
		// (set) Token: 0x060007B6 RID: 1974 RVA: 0x0001FC17 File Offset: 0x0001DE17
		[Editor(false)]
		public bool UpdateChildrenStates
		{
			get
			{
				return this._updateChildrenStates;
			}
			set
			{
				if (this._updateChildrenStates != value)
				{
					this._updateChildrenStates = value;
					base.OnPropertyChanged(value, "UpdateChildrenStates");
					if (value && this.ChildCount > 0)
					{
						this.SetState(this.CurrentState);
					}
				}
			}
		}

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x060007B7 RID: 1975 RVA: 0x0001FC4D File Offset: 0x0001DE4D
		// (set) Token: 0x060007B8 RID: 1976 RVA: 0x0001FC55 File Offset: 0x0001DE55
		public object Tag { get; set; }

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x060007B9 RID: 1977 RVA: 0x0001FC5E File Offset: 0x0001DE5E
		// (set) Token: 0x060007BA RID: 1978 RVA: 0x0001FC66 File Offset: 0x0001DE66
		public ILayout LayoutImp { get; protected set; }

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x060007BB RID: 1979 RVA: 0x0001FC6F File Offset: 0x0001DE6F
		// (set) Token: 0x060007BC RID: 1980 RVA: 0x0001FC77 File Offset: 0x0001DE77
		[Editor(false)]
		public bool DropEventHandledManually { get; set; }

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x060007BD RID: 1981 RVA: 0x0001FC80 File Offset: 0x0001DE80
		// (set) Token: 0x060007BE RID: 1982 RVA: 0x0001FC88 File Offset: 0x0001DE88
		internal WidgetInfo WidgetInfo { get; private set; }

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x060007BF RID: 1983 RVA: 0x0001FC91 File Offset: 0x0001DE91
		public IEnumerable<Widget> AllChildrenAndThis
		{
			get
			{
				yield return this;
				foreach (Widget widget in this._children)
				{
					foreach (Widget widget2 in widget.AllChildrenAndThis)
					{
						yield return widget2;
					}
					IEnumerator<Widget> enumerator2 = null;
				}
				List<Widget>.Enumerator enumerator = default(List<Widget>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x0001FCA4 File Offset: 0x0001DEA4
		public void ApplyActionOnAllChildren(Action<Widget> action)
		{
			foreach (Widget widget in this._children)
			{
				action(widget);
				widget.ApplyActionOnAllChildren(action);
			}
		}

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x060007C1 RID: 1985 RVA: 0x0001FD00 File Offset: 0x0001DF00
		public IEnumerable<Widget> AllChildren
		{
			get
			{
				foreach (Widget widget in this._children)
				{
					yield return widget;
					foreach (Widget widget2 in widget.AllChildren)
					{
						yield return widget2;
					}
					IEnumerator<Widget> enumerator2 = null;
					widget = null;
				}
				List<Widget>.Enumerator enumerator = default(List<Widget>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x060007C2 RID: 1986 RVA: 0x0001FD10 File Offset: 0x0001DF10
		public List<Widget> Children
		{
			get
			{
				return this._children;
			}
		}

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x060007C3 RID: 1987 RVA: 0x0001FD18 File Offset: 0x0001DF18
		public IEnumerable<Widget> Parents
		{
			get
			{
				for (Widget parent = this.ParentWidget; parent != null; parent = parent.ParentWidget)
				{
					yield return parent;
				}
				yield break;
			}
		}

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x060007C4 RID: 1988 RVA: 0x0001FD28 File Offset: 0x0001DF28
		internal bool ConnectedToRoot
		{
			get
			{
				return this.Id == "Root" || (this.ParentWidget != null && this.ParentWidget.ConnectedToRoot);
			}
		}

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x060007C5 RID: 1989 RVA: 0x0001FD53 File Offset: 0x0001DF53
		// (set) Token: 0x060007C6 RID: 1990 RVA: 0x0001FD5B File Offset: 0x0001DF5B
		internal int OnUpdateListIndex { get; set; } = -1;

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x060007C7 RID: 1991 RVA: 0x0001FD64 File Offset: 0x0001DF64
		// (set) Token: 0x060007C8 RID: 1992 RVA: 0x0001FD6C File Offset: 0x0001DF6C
		internal int OnLateUpdateListIndex { get; set; } = -1;

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x060007C9 RID: 1993 RVA: 0x0001FD75 File Offset: 0x0001DF75
		// (set) Token: 0x060007CA RID: 1994 RVA: 0x0001FD7D File Offset: 0x0001DF7D
		internal int OnUpdateBrushesIndex { get; set; } = -1;

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x060007CB RID: 1995 RVA: 0x0001FD86 File Offset: 0x0001DF86
		// (set) Token: 0x060007CC RID: 1996 RVA: 0x0001FD8E File Offset: 0x0001DF8E
		internal int OnParallelUpdateListIndex { get; set; } = -1;

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x060007CD RID: 1997 RVA: 0x0001FD97 File Offset: 0x0001DF97
		// (set) Token: 0x060007CE RID: 1998 RVA: 0x0001FD9F File Offset: 0x0001DF9F
		internal int OnVisualDefinitionListIndex { get; set; } = -1;

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x060007CF RID: 1999 RVA: 0x0001FDA8 File Offset: 0x0001DFA8
		// (set) Token: 0x060007D0 RID: 2000 RVA: 0x0001FDB0 File Offset: 0x0001DFB0
		internal int OnTweenPositionListIndex { get; set; } = -1;

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x060007D1 RID: 2001 RVA: 0x0001FDB9 File Offset: 0x0001DFB9
		// (set) Token: 0x060007D2 RID: 2002 RVA: 0x0001FDC1 File Offset: 0x0001DFC1
		[Editor(false)]
		public float MaxWidth
		{
			get
			{
				return this._maxWidth;
			}
			set
			{
				if (this._maxWidth != value)
				{
					this._maxWidth = value;
					this._gotMaxWidth = true;
					base.OnPropertyChanged(value, "MaxWidth");
				}
			}
		}

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x060007D3 RID: 2003 RVA: 0x0001FDE6 File Offset: 0x0001DFE6
		// (set) Token: 0x060007D4 RID: 2004 RVA: 0x0001FDEE File Offset: 0x0001DFEE
		[Editor(false)]
		public float MaxHeight
		{
			get
			{
				return this._maxHeight;
			}
			set
			{
				if (this._maxHeight != value)
				{
					this._maxHeight = value;
					this._gotMaxHeight = true;
					base.OnPropertyChanged(value, "MaxHeight");
				}
			}
		}

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x060007D5 RID: 2005 RVA: 0x0001FE13 File Offset: 0x0001E013
		// (set) Token: 0x060007D6 RID: 2006 RVA: 0x0001FE1B File Offset: 0x0001E01B
		[Editor(false)]
		public float MinWidth
		{
			get
			{
				return this._minWidth;
			}
			set
			{
				if (this._minWidth != value)
				{
					this._minWidth = value;
					this._gotMinWidth = true;
					base.OnPropertyChanged(value, "MinWidth");
				}
			}
		}

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x060007D7 RID: 2007 RVA: 0x0001FE40 File Offset: 0x0001E040
		// (set) Token: 0x060007D8 RID: 2008 RVA: 0x0001FE48 File Offset: 0x0001E048
		[Editor(false)]
		public float MinHeight
		{
			get
			{
				return this._minHeight;
			}
			set
			{
				if (this._minHeight != value)
				{
					this._minHeight = value;
					this._gotMinHeight = true;
					base.OnPropertyChanged(value, "MinHeight");
				}
			}
		}

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x060007D9 RID: 2009 RVA: 0x0001FE6D File Offset: 0x0001E06D
		public float ScaledMaxWidth
		{
			get
			{
				return this._scaleToUse * this._maxWidth;
			}
		}

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x060007DA RID: 2010 RVA: 0x0001FE7C File Offset: 0x0001E07C
		public float ScaledMaxHeight
		{
			get
			{
				return this._scaleToUse * this._maxHeight;
			}
		}

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x060007DB RID: 2011 RVA: 0x0001FE8B File Offset: 0x0001E08B
		public float ScaledMinWidth
		{
			get
			{
				return this._scaleToUse * this._minWidth;
			}
		}

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x060007DC RID: 2012 RVA: 0x0001FE9A File Offset: 0x0001E09A
		public float ScaledMinHeight
		{
			get
			{
				return this._scaleToUse * this._minHeight;
			}
		}

		// Token: 0x060007DD RID: 2013 RVA: 0x0001FEAC File Offset: 0x0001E0AC
		public Widget(UIContext context)
		{
			this.DropEventHandledManually = true;
			this.LayoutImp = new DefaultLayout();
			this._children = new List<Widget>();
			this.Context = context;
			this._states = new List<string>();
			this.WidgetInfo = WidgetInfo.GetWidgetInfo(base.GetType());
			this.Sprite = null;
			this._stateTimer = 0f;
			this._currentVisualStateAnimationState = VisualStateAnimationState.None;
			this._isFocusable = false;
			this._seed = 0;
			this._components = new List<WidgetComponent>();
			this.AddState("Default");
			this.SetState("Default");
		}

		// Token: 0x060007DE RID: 2014 RVA: 0x0001FFB4 File Offset: 0x0001E1B4
		public T GetComponent<T>() where T : WidgetComponent
		{
			for (int i = 0; i < this._components.Count; i++)
			{
				WidgetComponent widgetComponent = this._components[i];
				if (widgetComponent is T)
				{
					return (T)((object)widgetComponent);
				}
			}
			return default(T);
		}

		// Token: 0x060007DF RID: 2015 RVA: 0x0001FFFC File Offset: 0x0001E1FC
		public void AddComponent(WidgetComponent component)
		{
			this._components.Add(component);
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x0002000A File Offset: 0x0001E20A
		protected void SetMeasureAndLayoutDirty()
		{
			this.SetMeasureDirty();
			this.SetLayoutDirty();
		}

		// Token: 0x060007E1 RID: 2017 RVA: 0x00020018 File Offset: 0x0001E218
		protected void SetMeasureDirty()
		{
			this.EventManager.SetMeasureDirty();
		}

		// Token: 0x060007E2 RID: 2018 RVA: 0x00020025 File Offset: 0x0001E225
		protected void SetLayoutDirty()
		{
			this.EventManager.SetLayoutDirty();
		}

		// Token: 0x060007E3 RID: 2019 RVA: 0x00020032 File Offset: 0x0001E232
		public void AddState(string stateName)
		{
			if (!this._states.Contains(stateName))
			{
				this._states.Add(stateName);
			}
		}

		// Token: 0x060007E4 RID: 2020 RVA: 0x0002004E File Offset: 0x0001E24E
		public bool ContainsState(string stateName)
		{
			return this._states.Contains(stateName);
		}

		// Token: 0x060007E5 RID: 2021 RVA: 0x0002005C File Offset: 0x0001E25C
		public virtual void SetState(string stateName)
		{
			if (this.CurrentState != stateName)
			{
				this.CurrentState = stateName;
				this._stateTimer = 0f;
				if (this._currentVisualStateAnimationState != VisualStateAnimationState.None)
				{
					this._startVisualState = new VisualState("@StartState");
					this._startVisualState.FillFromWidget(this);
				}
				this._currentVisualStateAnimationState = VisualStateAnimationState.PlayingBasicTranisition;
			}
			if (this.UpdateChildrenStates)
			{
				for (int i = 0; i < this.ChildCount; i++)
				{
					Widget child = this.GetChild(i);
					if (!(child is ImageWidget) || !((ImageWidget)child).OverrideDefaultStateSwitchingEnabled)
					{
						child.SetState(this.CurrentState);
					}
				}
			}
		}

		// Token: 0x060007E6 RID: 2022 RVA: 0x000200F8 File Offset: 0x0001E2F8
		public Widget FindChild(BindingPath path)
		{
			string firstNode = path.FirstNode;
			BindingPath subPath = path.SubPath;
			if (firstNode == "..")
			{
				return this.ParentWidget.FindChild(subPath);
			}
			if (firstNode == ".")
			{
				return this;
			}
			foreach (Widget widget in this._children)
			{
				if (!string.IsNullOrEmpty(widget.Id) && widget.Id == firstNode)
				{
					if (subPath == null)
					{
						return widget;
					}
					return widget.FindChild(subPath);
				}
			}
			return null;
		}

		// Token: 0x060007E7 RID: 2023 RVA: 0x000201B4 File Offset: 0x0001E3B4
		public Widget FindChild(string singlePathNode)
		{
			if (singlePathNode == "..")
			{
				return this.ParentWidget;
			}
			if (singlePathNode == ".")
			{
				return this;
			}
			foreach (Widget widget in this._children)
			{
				if (!string.IsNullOrEmpty(widget.Id) && widget.Id == singlePathNode)
				{
					return widget;
				}
			}
			return null;
		}

		// Token: 0x060007E8 RID: 2024 RVA: 0x00020248 File Offset: 0x0001E448
		public Widget FindChild(WidgetSearchDelegate widgetSearchDelegate)
		{
			for (int i = 0; i < this._children.Count; i++)
			{
				Widget widget = this._children[i];
				if (widgetSearchDelegate(widget))
				{
					return widget;
				}
			}
			return null;
		}

		// Token: 0x060007E9 RID: 2025 RVA: 0x00020284 File Offset: 0x0001E484
		public Widget FindChild(string id, bool includeAllChildren = false)
		{
			IEnumerable<Widget> enumerable;
			if (!includeAllChildren)
			{
				IEnumerable<Widget> children = this._children;
				enumerable = children;
			}
			else
			{
				enumerable = this.AllChildren;
			}
			foreach (Widget widget in enumerable)
			{
				if (!string.IsNullOrEmpty(widget.Id) && widget.Id == id)
				{
					return widget;
				}
			}
			return null;
		}

		// Token: 0x060007EA RID: 2026 RVA: 0x000202FC File Offset: 0x0001E4FC
		public void RemoveAllChildren()
		{
			while (this._children.Count > 0)
			{
				this._children[0].ParentWidget = null;
			}
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x00020320 File Offset: 0x0001E520
		private static float GetEaseOutBack(float t)
		{
			float num = 0.5f;
			float num2 = num + 1f;
			return 1f + num2 * MathF.Pow(t - 1f, 3f) + num * MathF.Pow(t - 1f, 2f);
		}

		// Token: 0x060007EC RID: 2028 RVA: 0x00020368 File Offset: 0x0001E568
		internal void UpdateVisualDefinitions(float dt)
		{
			if (this.VisualDefinition != null && this._currentVisualStateAnimationState == VisualStateAnimationState.PlayingBasicTranisition)
			{
				if (this._startVisualState == null)
				{
					this._startVisualState = new VisualState("@StartState");
					this._startVisualState.FillFromWidget(this);
				}
				VisualState visualState = this.VisualDefinition.GetVisualState(this.CurrentState);
				if (visualState != null)
				{
					float num = (visualState.GotTransitionDuration ? visualState.TransitionDuration : this.VisualDefinition.TransitionDuration);
					float delayOnBegin = this.VisualDefinition.DelayOnBegin;
					if (this._stateTimer < num)
					{
						if (this._stateTimer >= delayOnBegin)
						{
							float num2 = (this._stateTimer - delayOnBegin) / (num - delayOnBegin);
							if (this.VisualDefinition.EaseIn)
							{
								num2 = Widget.GetEaseOutBack(num2);
							}
							this.PositionXOffset = (visualState.GotPositionXOffset ? Mathf.Lerp(this._startVisualState.PositionXOffset, visualState.PositionXOffset, num2) : this.PositionXOffset);
							this.PositionYOffset = (visualState.GotPositionYOffset ? Mathf.Lerp(this._startVisualState.PositionYOffset, visualState.PositionYOffset, num2) : this.PositionYOffset);
							this.SuggestedWidth = (visualState.GotSuggestedWidth ? Mathf.Lerp(this._startVisualState.SuggestedWidth, visualState.SuggestedWidth, num2) : this.SuggestedWidth);
							this.SuggestedHeight = (visualState.GotSuggestedHeight ? Mathf.Lerp(this._startVisualState.SuggestedHeight, visualState.SuggestedHeight, num2) : this.SuggestedHeight);
							this.MarginTop = (visualState.GotMarginTop ? Mathf.Lerp(this._startVisualState.MarginTop, visualState.MarginTop, num2) : this.MarginTop);
							this.MarginBottom = (visualState.GotMarginBottom ? Mathf.Lerp(this._startVisualState.MarginBottom, visualState.MarginBottom, num2) : this.MarginBottom);
							this.MarginLeft = (visualState.GotMarginLeft ? Mathf.Lerp(this._startVisualState.MarginLeft, visualState.MarginLeft, num2) : this.MarginLeft);
							this.MarginRight = (visualState.GotMarginRight ? Mathf.Lerp(this._startVisualState.MarginRight, visualState.MarginRight, num2) : this.MarginRight);
						}
					}
					else
					{
						this.PositionXOffset = (visualState.GotPositionXOffset ? visualState.PositionXOffset : this.PositionXOffset);
						this.PositionYOffset = (visualState.GotPositionYOffset ? visualState.PositionYOffset : this.PositionYOffset);
						this.SuggestedWidth = (visualState.GotSuggestedWidth ? visualState.SuggestedWidth : this.SuggestedWidth);
						this.SuggestedHeight = (visualState.GotSuggestedHeight ? visualState.SuggestedHeight : this.SuggestedHeight);
						this.MarginTop = (visualState.GotMarginTop ? visualState.MarginTop : this.MarginTop);
						this.MarginBottom = (visualState.GotMarginBottom ? visualState.MarginBottom : this.MarginBottom);
						this.MarginLeft = (visualState.GotMarginLeft ? visualState.MarginLeft : this.MarginLeft);
						this.MarginRight = (visualState.GotMarginRight ? visualState.MarginRight : this.MarginRight);
						this._startVisualState = visualState;
						this._currentVisualStateAnimationState = VisualStateAnimationState.None;
					}
				}
				else
				{
					this._currentVisualStateAnimationState = VisualStateAnimationState.None;
				}
			}
			this._stateTimer += dt;
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x0002069B File Offset: 0x0001E89B
		internal void Update(float dt)
		{
			this.OnUpdate(dt);
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x000206A4 File Offset: 0x0001E8A4
		internal void LateUpdate(float dt)
		{
			this.OnLateUpdate(dt);
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x000206AD File Offset: 0x0001E8AD
		internal void ParallelUpdate(float dt)
		{
			this.OnParallelUpdate(dt);
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x000206B6 File Offset: 0x0001E8B6
		protected virtual void OnUpdate(float dt)
		{
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x000206B8 File Offset: 0x0001E8B8
		protected virtual void OnParallelUpdate(float dt)
		{
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x000206BA File Offset: 0x0001E8BA
		protected virtual void OnLateUpdate(float dt)
		{
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x000206BC File Offset: 0x0001E8BC
		protected virtual void RefreshState()
		{
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x000206C0 File Offset: 0x0001E8C0
		public virtual void UpdateAnimationPropertiesSubTask(float alphaFactor)
		{
			this.AlphaFactor = alphaFactor;
			foreach (Widget widget in this.Children)
			{
				widget.UpdateAnimationPropertiesSubTask(alphaFactor);
			}
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x00020718 File Offset: 0x0001E918
		public void Measure(Vector2 measureSpec)
		{
			if (this.IsHidden)
			{
				this.MeasuredSize = Vector2.Zero;
				return;
			}
			this.OnMeasure(measureSpec);
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x00020738 File Offset: 0x0001E938
		private Vector2 ProcessSizeWithBoundaries(Vector2 input)
		{
			Vector2 vector = input;
			if (this._gotMinWidth && input.X < this.ScaledMinWidth)
			{
				vector.X = this.ScaledMinWidth;
			}
			if (this._gotMinHeight && input.Y < this.ScaledMinHeight)
			{
				vector.Y = this.ScaledMinHeight;
			}
			if (this._gotMaxWidth && input.X > this.ScaledMaxWidth)
			{
				vector.X = this.ScaledMaxWidth;
			}
			if (this._gotMaxHeight && input.Y > this.ScaledMaxHeight)
			{
				vector.Y = this.ScaledMaxHeight;
			}
			return vector;
		}

		// Token: 0x060007F7 RID: 2039 RVA: 0x000207D4 File Offset: 0x0001E9D4
		private void OnMeasure(Vector2 measureSpec)
		{
			if (this.WidthSizePolicy == SizePolicy.Fixed)
			{
				measureSpec.X = this.ScaledSuggestedWidth;
			}
			else if (this.WidthSizePolicy == SizePolicy.StretchToParent)
			{
				measureSpec.X -= this.ScaledMarginLeft + this.ScaledMarginRight;
			}
			else
			{
				SizePolicy widthSizePolicy = this.WidthSizePolicy;
			}
			if (this.HeightSizePolicy == SizePolicy.Fixed)
			{
				measureSpec.Y = this.ScaledSuggestedHeight;
			}
			else if (this.HeightSizePolicy == SizePolicy.StretchToParent)
			{
				measureSpec.Y -= this.ScaledMarginTop + this.ScaledMarginBottom;
			}
			else
			{
				SizePolicy heightSizePolicy = this.HeightSizePolicy;
			}
			measureSpec = this.ProcessSizeWithBoundaries(measureSpec);
			Vector2 vector = this.MeasureChildren(measureSpec);
			Vector2 vector2 = new Vector2(0f, 0f);
			if (this.WidthSizePolicy == SizePolicy.Fixed)
			{
				vector2.X = this.ScaledSuggestedWidth;
			}
			else if (this.WidthSizePolicy == SizePolicy.CoverChildren)
			{
				vector2.X = vector.X;
			}
			else if (this.WidthSizePolicy == SizePolicy.StretchToParent)
			{
				vector2.X = measureSpec.X;
			}
			if (this.HeightSizePolicy == SizePolicy.Fixed)
			{
				vector2.Y = this.ScaledSuggestedHeight;
			}
			else if (this.HeightSizePolicy == SizePolicy.CoverChildren)
			{
				vector2.Y = vector.Y;
			}
			else if (this.HeightSizePolicy == SizePolicy.StretchToParent)
			{
				vector2.Y = measureSpec.Y;
			}
			vector2 = this.ProcessSizeWithBoundaries(vector2);
			this.MeasuredSize = vector2;
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x00020924 File Offset: 0x0001EB24
		public bool CheckIsMyChildRecursive(Widget child)
		{
			for (Widget widget = ((child != null) ? child.ParentWidget : null); widget != null; widget = widget.ParentWidget)
			{
				if (widget == this)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x00020951 File Offset: 0x0001EB51
		private Vector2 MeasureChildren(Vector2 measureSpec)
		{
			return this.LayoutImp.MeasureChildren(this, measureSpec, this.Context.SpriteData, this._scaleToUse);
		}

		// Token: 0x060007FA RID: 2042 RVA: 0x00020971 File Offset: 0x0001EB71
		public void AddChild(Widget widget)
		{
			widget.ParentWidget = this;
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x0002097A File Offset: 0x0001EB7A
		public void AddChildAtIndex(Widget widget, int index)
		{
			widget.ParentWidget = this;
			widget.SetSiblingIndex(index, false);
		}

		// Token: 0x060007FC RID: 2044 RVA: 0x0002098C File Offset: 0x0001EB8C
		public void SwapChildren(Widget widget1, Widget widget2)
		{
			int num = this._children.IndexOf(widget1);
			int num2 = this._children.IndexOf(widget2);
			Widget widget3 = this._children[num];
			this._children[num] = this._children[num2];
			this._children[num2] = widget3;
		}

		// Token: 0x060007FD RID: 2045 RVA: 0x000209E8 File Offset: 0x0001EBE8
		protected virtual void OnChildAdded(Widget child)
		{
			this.EventFired("ItemAdd", new object[] { child });
			if (this.DoNotUseCustomScaleAndChildren)
			{
				child.DoNotUseCustomScaleAndChildren = true;
			}
			if (this.UpdateChildrenStates && (!(child is ImageWidget) || !((ImageWidget)child).OverrideDefaultStateSwitchingEnabled))
			{
				child.SetState(this.CurrentState);
			}
		}

		// Token: 0x060007FE RID: 2046 RVA: 0x00020A42 File Offset: 0x0001EC42
		public void RemoveChild(Widget widget)
		{
			widget.ParentWidget = null;
		}

		// Token: 0x060007FF RID: 2047 RVA: 0x00020A4C File Offset: 0x0001EC4C
		public virtual void OnBeforeRemovedChild(Widget widget)
		{
			if (this.IsHovered)
			{
				this.EventFired("HoverEnd", Array.Empty<object>());
			}
			this.Children.ForEach(delegate(Widget c)
			{
				c.OnBeforeRemovedChild(widget);
			});
		}

		// Token: 0x06000800 RID: 2048 RVA: 0x00020A95 File Offset: 0x0001EC95
		public bool HasChild(Widget widget)
		{
			return this._children.Contains(widget);
		}

		// Token: 0x06000801 RID: 2049 RVA: 0x00020AA3 File Offset: 0x0001ECA3
		protected virtual void OnChildRemoved(Widget child)
		{
			this.EventFired("ItemRemove", new object[] { child });
		}

		// Token: 0x06000802 RID: 2050 RVA: 0x00020ABA File Offset: 0x0001ECBA
		protected virtual void OnAfterChildRemoved(Widget child)
		{
			this.EventFired("AfterItemRemove", new object[] { child });
		}

		// Token: 0x06000803 RID: 2051 RVA: 0x00020AD1 File Offset: 0x0001ECD1
		public virtual void UpdateBrushes(float dt)
		{
		}

		// Token: 0x06000804 RID: 2052 RVA: 0x00020AD3 File Offset: 0x0001ECD3
		public int GetChildIndex(Widget child)
		{
			return this._children.IndexOf(child);
		}

		// Token: 0x06000805 RID: 2053 RVA: 0x00020AE4 File Offset: 0x0001ECE4
		public int GetVisibleChildIndex(Widget child)
		{
			int num = -1;
			List<Widget> list = this._children.Where((Widget c) => c.IsVisible).ToList<Widget>();
			if (list.Count > 0)
			{
				num = list.IndexOf(child);
			}
			return num;
		}

		// Token: 0x06000806 RID: 2054 RVA: 0x00020B38 File Offset: 0x0001ED38
		public int GetFilterChildIndex(Widget child, Func<Widget, bool> childrenFilter)
		{
			int num = -1;
			List<Widget> list = this._children.Where((Widget c) => childrenFilter(c)).ToList<Widget>();
			if (list.Count > 0)
			{
				num = list.IndexOf(child);
			}
			return num;
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x00020B83 File Offset: 0x0001ED83
		public Widget GetChild(int i)
		{
			if (i < this._children.Count)
			{
				return this._children[i];
			}
			return null;
		}

		// Token: 0x06000808 RID: 2056 RVA: 0x00020BA4 File Offset: 0x0001EDA4
		public void Layout(float left, float bottom, float right, float top)
		{
			if (this.IsVisible)
			{
				this.SetLayout(left, bottom, right, top);
				Vector2 scaledPositionOffset = this.ScaledPositionOffset;
				this.Left += scaledPositionOffset.X;
				this.Top += scaledPositionOffset.Y;
				this.OnLayout(this.Left, this.Bottom, this.Right, this.Top);
			}
		}

		// Token: 0x06000809 RID: 2057 RVA: 0x00020C10 File Offset: 0x0001EE10
		private void SetLayout(float left, float bottom, float right, float top)
		{
			left += this.ScaledMarginLeft;
			right -= this.ScaledMarginRight;
			top += this.ScaledMarginTop;
			bottom -= this.ScaledMarginBottom;
			float num = right - left;
			float num2 = bottom - top;
			float num3;
			if (this.HorizontalAlignment == HorizontalAlignment.Left)
			{
				num3 = left;
			}
			else if (this.HorizontalAlignment == HorizontalAlignment.Center)
			{
				num3 = left + num / 2f - this.MeasuredSize.X / 2f;
			}
			else
			{
				num3 = right - this.MeasuredSize.X;
			}
			float num4;
			if (this.VerticalAlignment == VerticalAlignment.Top)
			{
				num4 = top;
			}
			else if (this.VerticalAlignment == VerticalAlignment.Center)
			{
				num4 = top + num2 / 2f - this.MeasuredSize.Y / 2f;
			}
			else
			{
				num4 = bottom - this.MeasuredSize.Y;
			}
			this.Left = num3;
			this.Top = num4;
			this.Size = this.MeasuredSize;
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x00020CED File Offset: 0x0001EEED
		private void OnLayout(float left, float bottom, float right, float top)
		{
			this.LayoutImp.OnLayout(this, left, bottom, right, top);
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x00020D00 File Offset: 0x0001EF00
		internal void DoTweenPosition(float dt)
		{
			if (this.IsVisible && dt > 0f)
			{
				float num = this.Left - this.LocalPosition.X;
				float num2 = this.Top - this.LocalPosition.Y;
				if (Mathf.Abs(num) + Mathf.Abs(num2) < 0.003f)
				{
					this.LocalPosition = new Vector2(this.Left, this.Top);
					return;
				}
				num = Mathf.Clamp(num, -100f, 100f);
				num2 = Mathf.Clamp(num2, -100f, 100f);
				float num3 = Mathf.Min(dt * 18f, 1f);
				this.LocalPosition = new Vector2(this.LocalPosition.X + num3 * num, this.LocalPosition.Y + num3 * num2);
			}
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x00020DD4 File Offset: 0x0001EFD4
		internal void ParallelUpdateChildPositions(Vector2 globalPosition)
		{
			Widget.<>c__DisplayClass458_0 CS$<>8__locals1 = new Widget.<>c__DisplayClass458_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.globalPosition = globalPosition;
			TWParallel.For(0, this._children.Count, new TWParallel.ParallelForAuxPredicate(CS$<>8__locals1.<ParallelUpdateChildPositions>g__UpdateChildPositionMT|0), 16);
		}

		// Token: 0x0600080D RID: 2061 RVA: 0x00020E14 File Offset: 0x0001F014
		internal void UpdatePosition(Vector2 parentPosition)
		{
			if (this.IsVisible)
			{
				if (!this.TweenPosition)
				{
					this.LocalPosition = new Vector2(this.Left, this.Top);
				}
				Vector2 vector = this.LocalPosition + parentPosition;
				if (this._children.Count >= 64)
				{
					this.ParallelUpdateChildPositions(vector);
				}
				else
				{
					for (int i = 0; i < this._children.Count; i++)
					{
						this._children[i].UpdatePosition(vector);
					}
				}
				this._cachedGlobalPosition = vector;
			}
		}

		// Token: 0x0600080E RID: 2062 RVA: 0x00020E9C File Offset: 0x0001F09C
		public virtual void HandleInput(IReadOnlyList<int> lastKeysPressed)
		{
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x00020EA0 File Offset: 0x0001F0A0
		public bool IsPointInsideMeasuredArea(Vector2 p)
		{
			Vector2 globalPosition = this.GlobalPosition;
			float num = p.X - globalPosition.X;
			float num2 = p.Y - globalPosition.Y;
			return 0f <= num && num <= this.Size.X && 0f <= num2 && num2 <= this.Size.Y;
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x00020F04 File Offset: 0x0001F104
		public bool IsPointInsideGamepadCursorArea(Vector2 p)
		{
			Vector2 globalPosition = this.GlobalPosition;
			globalPosition.X -= this.ExtendCursorAreaLeft;
			globalPosition.Y -= this.ExtendCursorAreaTop;
			Vector2 size = this.Size;
			size.X += this.ExtendCursorAreaLeft + this.ExtendCursorAreaRight;
			size.Y += this.ExtendCursorAreaTop + this.ExtendCursorAreaBottom;
			return p.X >= globalPosition.X && p.Y > globalPosition.Y && p.X < globalPosition.X + size.X && p.Y < globalPosition.Y + size.Y;
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x00020FB9 File Offset: 0x0001F1B9
		public void Hide()
		{
			this.IsHidden = true;
		}

		// Token: 0x06000812 RID: 2066 RVA: 0x00020FC2 File Offset: 0x0001F1C2
		public void Show()
		{
			this.IsHidden = false;
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x00020FCB File Offset: 0x0001F1CB
		public Vector2 GetLocalPoint(Vector2 globalPoint)
		{
			return globalPoint - this.GlobalPosition;
		}

		// Token: 0x06000814 RID: 2068 RVA: 0x00020FDC File Offset: 0x0001F1DC
		public void SetSiblingIndex(int index, bool force = false)
		{
			int siblingIndex = this.GetSiblingIndex();
			if (siblingIndex != index || force)
			{
				this.ParentWidget._children.RemoveAt(siblingIndex);
				this.ParentWidget._children.Insert(index, this);
				this.SetMeasureAndLayoutDirty();
				this.EventFired("SiblingIndexChanged", Array.Empty<object>());
			}
		}

		// Token: 0x06000815 RID: 2069 RVA: 0x00021034 File Offset: 0x0001F234
		public int GetSiblingIndex()
		{
			Widget parentWidget = this.ParentWidget;
			if (parentWidget == null)
			{
				return -1;
			}
			return parentWidget.GetChildIndex(this);
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x00021048 File Offset: 0x0001F248
		public int GetVisibleSiblingIndex()
		{
			return this.ParentWidget.GetVisibleChildIndex(this);
		}

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06000817 RID: 2071 RVA: 0x00021056 File Offset: 0x0001F256
		// (set) Token: 0x06000818 RID: 2072 RVA: 0x0002105E File Offset: 0x0001F25E
		public bool DisableRender { get; set; }

		// Token: 0x06000819 RID: 2073 RVA: 0x00021068 File Offset: 0x0001F268
		public void Render(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			if (!this.IsHidden && !this.DisableRender)
			{
				Vector2 cachedGlobalPosition = this._cachedGlobalPosition;
				if (this.ClipHorizontalContent || this.ClipVerticalContent)
				{
					int num = (this.ClipHorizontalContent ? ((int)this.Size.X) : (-1));
					int num2 = (this.ClipVerticalContent ? ((int)this.Size.Y) : (-1));
					drawContext.PushScissor((int)cachedGlobalPosition.X, (int)cachedGlobalPosition.Y, num, num2);
				}
				if (this.CircularClipEnabled)
				{
					if (this.IsCircularClipRadiusHalfOfHeight)
					{
						this.CircularClipRadius = this.Size.Y / 2f * this._inverseScaleToUse;
					}
					else if (this.IsCircularClipRadiusHalfOfWidth)
					{
						this.CircularClipRadius = this.Size.X / 2f * this._inverseScaleToUse;
					}
					Vector2 vector = new Vector2(cachedGlobalPosition.X + this.Size.X * 0.5f + this.CircularClipXOffset * this._scaleToUse, cachedGlobalPosition.Y + this.Size.Y * 0.5f + this.CircularClipYOffset * this._scaleToUse);
					drawContext.SetCircualMask(vector, this.CircularClipRadius * this._scaleToUse, this.CircularClipSmoothingRadius * this._scaleToUse);
				}
				bool flag = false;
				if (drawContext.ScissorTestEnabled)
				{
					ScissorTestInfo currentScissor = drawContext.CurrentScissor;
					Rectangle rectangle = new Rectangle(cachedGlobalPosition.X, cachedGlobalPosition.Y, this.Size.X, this.Size.Y);
					Rectangle rectangle2 = new Rectangle((float)currentScissor.X, (float)currentScissor.Y, (float)currentScissor.Width, (float)currentScissor.Height);
					if (rectangle.IsCollide(rectangle2) || this._calculateSizeFirstFrame)
					{
						flag = !this.DoNotRenderIfNotFullyInsideScissor || rectangle.IsSubRectOf(rectangle2);
					}
				}
				else
				{
					Rectangle rectangle3 = new Rectangle(this._cachedGlobalPosition.X, this._cachedGlobalPosition.Y, this.MeasuredSize.X, this.MeasuredSize.Y);
					Rectangle rectangle4 = new Rectangle(this.EventManager.LeftUsableAreaStart, this.EventManager.TopUsableAreaStart, this.EventManager.PageSize.X, this.EventManager.PageSize.Y);
					if (rectangle3.IsCollide(rectangle4) || this._calculateSizeFirstFrame)
					{
						flag = true;
					}
				}
				if (flag)
				{
					this.OnRender(twoDimensionContext, drawContext);
					for (int i = 0; i < this._children.Count; i++)
					{
						Widget widget = this._children[i];
						if (!widget.RenderLate)
						{
							widget.Render(twoDimensionContext, drawContext);
						}
					}
					for (int j = 0; j < this._children.Count; j++)
					{
						Widget widget2 = this._children[j];
						if (widget2.RenderLate)
						{
							widget2.Render(twoDimensionContext, drawContext);
						}
					}
				}
				if (this.CircularClipEnabled)
				{
					drawContext.ClearCircualMask();
				}
				if (this.ClipHorizontalContent || this.ClipVerticalContent)
				{
					drawContext.PopScissor();
				}
			}
			this._calculateSizeFirstFrame = false;
		}

		// Token: 0x0600081A RID: 2074 RVA: 0x00021380 File Offset: 0x0001F580
		protected virtual void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			Vector2 globalPosition = this.GlobalPosition;
			if (this.ForcePixelPerfectRenderPlacement)
			{
				globalPosition.X = (float)MathF.Round(globalPosition.X);
				globalPosition.Y = (float)MathF.Round(globalPosition.Y);
			}
			if (this._sprite != null)
			{
				Texture texture = this._sprite.Texture;
				if (texture != null)
				{
					float num = globalPosition.X;
					float num2 = globalPosition.Y;
					SimpleMaterial simpleMaterial = drawContext.CreateSimpleMaterial();
					simpleMaterial.OverlayEnabled = false;
					simpleMaterial.CircularMaskingEnabled = false;
					simpleMaterial.Texture = texture;
					simpleMaterial.Color = this.Color;
					simpleMaterial.ColorFactor = this.ColorFactor;
					simpleMaterial.AlphaFactor = this.AlphaFactor * this.Context.ContextAlpha;
					simpleMaterial.HueFactor = 0f;
					simpleMaterial.SaturationFactor = this.SaturationFactor;
					simpleMaterial.ValueFactor = this.ValueFactor;
					float num3 = this.ExtendLeft;
					if (this.HorizontalFlip)
					{
						num3 = this.ExtendRight;
					}
					float num4 = this.Size.X;
					num4 += (this.ExtendRight + this.ExtendLeft) * this._scaleToUse;
					num -= num3 * this._scaleToUse;
					float num5 = this.Size.Y;
					float num6 = this.ExtendTop;
					if (this.HorizontalFlip)
					{
						num6 = this.ExtendBottom;
					}
					num5 += (this.ExtendTop + this.ExtendBottom) * this._scaleToUse;
					num2 -= num6 * this._scaleToUse;
					drawContext.DrawSprite(this._sprite, simpleMaterial, num, num2, this._scaleToUse, num4, num5, this.HorizontalFlip, this.VerticalFlip);
				}
			}
		}

		// Token: 0x0600081B RID: 2075 RVA: 0x00021520 File Offset: 0x0001F720
		protected void EventFired(string eventName, params object[] args)
		{
			if (this._eventTargets != null)
			{
				for (int i = 0; i < this._eventTargets.Count; i++)
				{
					this._eventTargets[i](this, eventName, args);
				}
			}
		}

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x0600081C RID: 2076 RVA: 0x0002155F File Offset: 0x0001F75F
		// (remove) Token: 0x0600081D RID: 2077 RVA: 0x00021580 File Offset: 0x0001F780
		public event Action<Widget, string, object[]> EventFire
		{
			add
			{
				if (this._eventTargets == null)
				{
					this._eventTargets = new List<Action<Widget, string, object[]>>();
				}
				this._eventTargets.Add(value);
			}
			remove
			{
				if (this._eventTargets != null)
				{
					this._eventTargets.Remove(value);
				}
			}
		}

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x0600081E RID: 2078 RVA: 0x00021598 File Offset: 0x0001F798
		// (remove) Token: 0x0600081F RID: 2079 RVA: 0x000215D0 File Offset: 0x0001F7D0
		public event Action<Widget> OnVisibilityChanged;

		// Token: 0x06000820 RID: 2080 RVA: 0x00021608 File Offset: 0x0001F808
		public bool IsRecursivelyVisible()
		{
			for (Widget widget = this; widget != null; widget = widget.ParentWidget)
			{
				if (!widget.IsVisible)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000821 RID: 2081 RVA: 0x0002162E File Offset: 0x0001F82E
		internal void HandleOnDisconnectedFromRoot()
		{
			this.OnDisconnectedFromRoot();
			if (this.IsHovered)
			{
				this.EventFired("HoverEnd", Array.Empty<object>());
			}
		}

		// Token: 0x06000822 RID: 2082 RVA: 0x0002164E File Offset: 0x0001F84E
		internal void HandleOnConnectedToRoot()
		{
			if (!this._seedSet)
			{
				this._seed = this.GetSiblingIndex();
				this._seedSet = true;
			}
			this.OnConnectedToRoot();
		}

		// Token: 0x06000823 RID: 2083 RVA: 0x00021671 File Offset: 0x0001F871
		protected virtual void OnDisconnectedFromRoot()
		{
		}

		// Token: 0x06000824 RID: 2084 RVA: 0x00021673 File Offset: 0x0001F873
		protected virtual void OnConnectedToRoot()
		{
			this.EventFired("ConnectedToRoot", Array.Empty<object>());
		}

		// Token: 0x06000825 RID: 2085 RVA: 0x00021685 File Offset: 0x0001F885
		public override string ToString()
		{
			return this.GetFullIDPath();
		}

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06000826 RID: 2086 RVA: 0x0002168D File Offset: 0x0001F88D
		// (set) Token: 0x06000827 RID: 2087 RVA: 0x00021695 File Offset: 0x0001F895
		public float ExtendCursorAreaTop { get; set; }

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06000828 RID: 2088 RVA: 0x0002169E File Offset: 0x0001F89E
		// (set) Token: 0x06000829 RID: 2089 RVA: 0x000216A6 File Offset: 0x0001F8A6
		public float ExtendCursorAreaRight { get; set; }

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x0600082A RID: 2090 RVA: 0x000216AF File Offset: 0x0001F8AF
		// (set) Token: 0x0600082B RID: 2091 RVA: 0x000216B7 File Offset: 0x0001F8B7
		public float ExtendCursorAreaBottom { get; set; }

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x0600082C RID: 2092 RVA: 0x000216C0 File Offset: 0x0001F8C0
		// (set) Token: 0x0600082D RID: 2093 RVA: 0x000216C8 File Offset: 0x0001F8C8
		public float ExtendCursorAreaLeft { get; set; }

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x0600082E RID: 2094 RVA: 0x000216D1 File Offset: 0x0001F8D1
		// (set) Token: 0x0600082F RID: 2095 RVA: 0x000216D9 File Offset: 0x0001F8D9
		public float CursorAreaXOffset { get; set; }

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06000830 RID: 2096 RVA: 0x000216E2 File Offset: 0x0001F8E2
		// (set) Token: 0x06000831 RID: 2097 RVA: 0x000216EA File Offset: 0x0001F8EA
		public float CursorAreaYOffset { get; set; }

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06000832 RID: 2098 RVA: 0x000216F3 File Offset: 0x0001F8F3
		// (set) Token: 0x06000833 RID: 2099 RVA: 0x000216FB File Offset: 0x0001F8FB
		public bool DoNotAcceptNavigation
		{
			get
			{
				return this._doNotAcceptNavigation;
			}
			set
			{
				if (value != this._doNotAcceptNavigation)
				{
					this._doNotAcceptNavigation = value;
					this.EventManager.OnWidgetNavigationStatusChanged(this);
				}
			}
		}

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06000834 RID: 2100 RVA: 0x00021719 File Offset: 0x0001F919
		// (set) Token: 0x06000835 RID: 2101 RVA: 0x00021721 File Offset: 0x0001F921
		public bool IsUsingNavigation
		{
			get
			{
				return this._isUsingNavigation;
			}
			set
			{
				if (value != this._isUsingNavigation)
				{
					this._isUsingNavigation = value;
					base.OnPropertyChanged(value, "IsUsingNavigation");
				}
			}
		}

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x06000836 RID: 2102 RVA: 0x0002173F File Offset: 0x0001F93F
		// (set) Token: 0x06000837 RID: 2103 RVA: 0x00021747 File Offset: 0x0001F947
		public bool UseSiblingIndexForNavigation
		{
			get
			{
				return this._useSiblingIndexForNavigation;
			}
			set
			{
				if (value != this._useSiblingIndexForNavigation)
				{
					this._useSiblingIndexForNavigation = value;
					if (value)
					{
						this.GamepadNavigationIndex = this.GetSiblingIndex();
					}
				}
			}
		}

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06000838 RID: 2104 RVA: 0x00021768 File Offset: 0x0001F968
		// (set) Token: 0x06000839 RID: 2105 RVA: 0x00021770 File Offset: 0x0001F970
		public int GamepadNavigationIndex
		{
			get
			{
				return this._gamepadNavigationIndex;
			}
			set
			{
				if (value != this._gamepadNavigationIndex)
				{
					this._gamepadNavigationIndex = value;
					this.EventManager.OnWidgetNavigationIndexUpdated(this);
					this.OnGamepadNavigationIndexUpdated(value);
					base.OnPropertyChanged(value, "GamepadNavigationIndex");
				}
			}
		}

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x0600083A RID: 2106 RVA: 0x000217A1 File Offset: 0x0001F9A1
		// (set) Token: 0x0600083B RID: 2107 RVA: 0x000217A9 File Offset: 0x0001F9A9
		public GamepadNavigationTypes UsedNavigationMovements
		{
			get
			{
				return this._usedNavigationMovements;
			}
			set
			{
				if (value != this._usedNavigationMovements)
				{
					this._usedNavigationMovements = value;
					this.EventManager.OnWidgetUsedNavigationMovementsUpdated(this);
				}
			}
		}

		// Token: 0x0600083C RID: 2108 RVA: 0x000217C7 File Offset: 0x0001F9C7
		protected virtual void OnGamepadNavigationIndexUpdated(int newIndex)
		{
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x000217C9 File Offset: 0x0001F9C9
		public void OnGamepadNavigationFocusGain()
		{
			Action<Widget> onGamepadNavigationFocusGained = this.OnGamepadNavigationFocusGained;
			if (onGamepadNavigationFocusGained == null)
			{
				return;
			}
			onGamepadNavigationFocusGained(this);
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x000217DC File Offset: 0x0001F9DC
		internal bool PreviewEvent(GauntletEvent gauntletEvent)
		{
			bool flag = false;
			switch (gauntletEvent)
			{
			case GauntletEvent.MouseMove:
				flag = this.OnPreviewMouseMove();
				break;
			case GauntletEvent.MousePressed:
				flag = this.OnPreviewMousePressed();
				break;
			case GauntletEvent.MouseReleased:
				flag = this.OnPreviewMouseReleased();
				break;
			case GauntletEvent.MouseAlternatePressed:
				flag = this.OnPreviewMouseAlternatePressed();
				break;
			case GauntletEvent.MouseAlternateReleased:
				flag = this.OnPreviewMouseAlternateReleased();
				break;
			case GauntletEvent.DragHover:
				flag = this.OnPreviewDragHover();
				break;
			case GauntletEvent.DragBegin:
				flag = this.OnPreviewDragBegin();
				break;
			case GauntletEvent.DragEnd:
				flag = this.OnPreviewDragEnd();
				break;
			case GauntletEvent.Drop:
				flag = this.OnPreviewDrop();
				break;
			case GauntletEvent.MouseScroll:
				flag = this.OnPreviewMouseScroll();
				break;
			case GauntletEvent.RightStickMovement:
				flag = this.OnPreviewRightStickMovement();
				break;
			}
			return flag;
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x00021881 File Offset: 0x0001FA81
		protected virtual bool OnPreviewMousePressed()
		{
			return true;
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x00021884 File Offset: 0x0001FA84
		protected virtual bool OnPreviewMouseReleased()
		{
			return true;
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x00021887 File Offset: 0x0001FA87
		protected virtual bool OnPreviewMouseAlternatePressed()
		{
			return true;
		}

		// Token: 0x06000842 RID: 2114 RVA: 0x0002188A File Offset: 0x0001FA8A
		protected virtual bool OnPreviewMouseAlternateReleased()
		{
			return true;
		}

		// Token: 0x06000843 RID: 2115 RVA: 0x0002188D File Offset: 0x0001FA8D
		protected virtual bool OnPreviewDragBegin()
		{
			return this.AcceptDrag;
		}

		// Token: 0x06000844 RID: 2116 RVA: 0x00021895 File Offset: 0x0001FA95
		protected virtual bool OnPreviewDragEnd()
		{
			return this.AcceptDrag;
		}

		// Token: 0x06000845 RID: 2117 RVA: 0x0002189D File Offset: 0x0001FA9D
		protected virtual bool OnPreviewDrop()
		{
			return this.AcceptDrop;
		}

		// Token: 0x06000846 RID: 2118 RVA: 0x000218A5 File Offset: 0x0001FAA5
		protected virtual bool OnPreviewMouseScroll()
		{
			return false;
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x000218A8 File Offset: 0x0001FAA8
		protected virtual bool OnPreviewRightStickMovement()
		{
			return false;
		}

		// Token: 0x06000848 RID: 2120 RVA: 0x000218AB File Offset: 0x0001FAAB
		protected virtual bool OnPreviewMouseMove()
		{
			return true;
		}

		// Token: 0x06000849 RID: 2121 RVA: 0x000218AE File Offset: 0x0001FAAE
		protected virtual bool OnPreviewDragHover()
		{
			return false;
		}

		// Token: 0x0600084A RID: 2122 RVA: 0x000218B1 File Offset: 0x0001FAB1
		protected internal virtual void OnMousePressed()
		{
			this.IsPressed = true;
			this.EventFired("MouseDown", Array.Empty<object>());
		}

		// Token: 0x0600084B RID: 2123 RVA: 0x000218CA File Offset: 0x0001FACA
		protected internal virtual void OnMouseReleased()
		{
			this.IsPressed = false;
			this.EventFired("MouseUp", Array.Empty<object>());
		}

		// Token: 0x0600084C RID: 2124 RVA: 0x000218E3 File Offset: 0x0001FAE3
		protected internal virtual void OnMouseAlternatePressed()
		{
			this.EventFired("MouseAlternateDown", Array.Empty<object>());
		}

		// Token: 0x0600084D RID: 2125 RVA: 0x000218F5 File Offset: 0x0001FAF5
		protected internal virtual void OnMouseAlternateReleased()
		{
			this.EventFired("MouseAlternateUp", Array.Empty<object>());
		}

		// Token: 0x0600084E RID: 2126 RVA: 0x00021907 File Offset: 0x0001FB07
		protected internal virtual void OnMouseMove()
		{
			this.EventFired("MouseMove", Array.Empty<object>());
		}

		// Token: 0x0600084F RID: 2127 RVA: 0x00021919 File Offset: 0x0001FB19
		protected internal virtual void OnHoverBegin()
		{
			this.IsHovered = true;
			this.EventFired("HoverBegin", Array.Empty<object>());
		}

		// Token: 0x06000850 RID: 2128 RVA: 0x00021932 File Offset: 0x0001FB32
		protected internal virtual void OnHoverEnd()
		{
			this.EventFired("HoverEnd", Array.Empty<object>());
			this.IsHovered = false;
		}

		// Token: 0x06000851 RID: 2129 RVA: 0x0002194B File Offset: 0x0001FB4B
		protected internal virtual void OnDragBegin()
		{
			this.EventManager.BeginDragging(this);
			this.EventFired("DragBegin", Array.Empty<object>());
		}

		// Token: 0x06000852 RID: 2130 RVA: 0x00021969 File Offset: 0x0001FB69
		protected internal virtual void OnDragEnd()
		{
			this.EventFired("DragEnd", Array.Empty<object>());
		}

		// Token: 0x06000853 RID: 2131 RVA: 0x0002197C File Offset: 0x0001FB7C
		protected internal virtual bool OnDrop()
		{
			if (this.AcceptDrop)
			{
				bool flag = true;
				if (this.AcceptDropHandler != null)
				{
					flag = this.AcceptDropHandler(this, this.EventManager.DraggedWidget);
				}
				if (flag)
				{
					Widget widget = this.EventManager.ReleaseDraggedWidget();
					int num = -1;
					if (!this.DropEventHandledManually)
					{
						widget.ParentWidget = this;
					}
					this.EventFired("Drop", new object[] { widget, num });
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000854 RID: 2132 RVA: 0x000219F4 File Offset: 0x0001FBF4
		protected internal virtual void OnMouseScroll()
		{
			this.EventFired("MouseScroll", Array.Empty<object>());
		}

		// Token: 0x06000855 RID: 2133 RVA: 0x00021A06 File Offset: 0x0001FC06
		protected internal virtual void OnRightStickMovement()
		{
		}

		// Token: 0x06000856 RID: 2134 RVA: 0x00021A08 File Offset: 0x0001FC08
		protected internal virtual void OnDragHoverBegin()
		{
			this.EventFired("DragHoverBegin", Array.Empty<object>());
		}

		// Token: 0x06000857 RID: 2135 RVA: 0x00021A1A File Offset: 0x0001FC1A
		protected internal virtual void OnDragHoverEnd()
		{
			this.EventFired("DragHoverEnd", Array.Empty<object>());
		}

		// Token: 0x06000858 RID: 2136 RVA: 0x00021A2C File Offset: 0x0001FC2C
		protected internal virtual void OnGainFocus()
		{
			this.IsFocused = true;
			this.EventFired("FocusGained", Array.Empty<object>());
		}

		// Token: 0x06000859 RID: 2137 RVA: 0x00021A45 File Offset: 0x0001FC45
		protected internal virtual void OnLoseFocus()
		{
			this.IsFocused = false;
			this.EventFired("FocusLost", Array.Empty<object>());
		}

		// Token: 0x0600085A RID: 2138 RVA: 0x00021A5E File Offset: 0x0001FC5E
		protected internal virtual void OnMouseOverBegin()
		{
			this.EventFired("MouseOverBegin", Array.Empty<object>());
		}

		// Token: 0x0600085B RID: 2139 RVA: 0x00021A70 File Offset: 0x0001FC70
		protected internal virtual void OnMouseOverEnd()
		{
			this.EventFired("MouseOverEnd", Array.Empty<object>());
		}

		// Token: 0x04000352 RID: 850
		private Color _color = Color.White;

		// Token: 0x0400035E RID: 862
		private string _id;

		// Token: 0x04000360 RID: 864
		protected Vector2 _cachedGlobalPosition;

		// Token: 0x04000361 RID: 865
		private Widget _parent;

		// Token: 0x04000362 RID: 866
		private List<Widget> _children;

		// Token: 0x04000363 RID: 867
		private bool _doNotUseCustomScaleAndChildren;

		// Token: 0x04000365 RID: 869
		protected bool _calculateSizeFirstFrame = true;

		// Token: 0x04000367 RID: 871
		private float _suggestedWidth;

		// Token: 0x04000368 RID: 872
		private float _suggestedHeight;

		// Token: 0x04000369 RID: 873
		private bool _tweenPosition;

		// Token: 0x0400036A RID: 874
		private string _hoveredCursorState;

		// Token: 0x0400036B RID: 875
		private bool _alternateClickEventHasSpecialEvent;

		// Token: 0x0400036C RID: 876
		private Vector2 _positionOffset;

		// Token: 0x0400036F RID: 879
		private float _marginTop;

		// Token: 0x04000370 RID: 880
		private float _marginLeft;

		// Token: 0x04000371 RID: 881
		private float _marginBottom;

		// Token: 0x04000372 RID: 882
		private float _marginRight;

		// Token: 0x04000373 RID: 883
		private VerticalAlignment _verticalAlignment;

		// Token: 0x04000374 RID: 884
		private HorizontalAlignment _horizontalAlignment;

		// Token: 0x04000375 RID: 885
		private Vector2 _topLeft;

		// Token: 0x04000376 RID: 886
		private bool _forcePixelPerfectRenderPlacement;

		// Token: 0x04000378 RID: 888
		private SizePolicy _widthSizePolicy;

		// Token: 0x04000379 RID: 889
		private SizePolicy _heightSizePolicy;

		// Token: 0x0400037D RID: 893
		private Widget _dragWidget;

		// Token: 0x04000389 RID: 905
		private bool _isHovered;

		// Token: 0x0400038A RID: 906
		private bool _isDisabled;

		// Token: 0x0400038B RID: 907
		private bool _isFocusable;

		// Token: 0x0400038C RID: 908
		private bool _isFocused;

		// Token: 0x0400038D RID: 909
		private bool _restartAnimationFirstFrame;

		// Token: 0x0400038E RID: 910
		private bool _doNotPassEventsToChildren;

		// Token: 0x0400038F RID: 911
		private bool _doNotAcceptEvents;

		// Token: 0x04000390 RID: 912
		public Func<Widget, Widget, bool> AcceptDropHandler;

		// Token: 0x04000391 RID: 913
		private bool _isPressed;

		// Token: 0x04000392 RID: 914
		private bool _isHidden;

		// Token: 0x04000393 RID: 915
		private Sprite _sprite;

		// Token: 0x04000394 RID: 916
		private VisualDefinition _visualDefinition;

		// Token: 0x04000395 RID: 917
		private List<string> _states;

		// Token: 0x04000396 RID: 918
		protected float _stateTimer;

		// Token: 0x04000398 RID: 920
		protected VisualState _startVisualState;

		// Token: 0x04000399 RID: 921
		protected VisualStateAnimationState _currentVisualStateAnimationState;

		// Token: 0x0400039A RID: 922
		private bool _updateChildrenStates;

		// Token: 0x040003A5 RID: 933
		protected int _seed;

		// Token: 0x040003A6 RID: 934
		private bool _seedSet;

		// Token: 0x040003A7 RID: 935
		private float _maxWidth;

		// Token: 0x040003A8 RID: 936
		private float _maxHeight;

		// Token: 0x040003A9 RID: 937
		private float _minWidth;

		// Token: 0x040003AA RID: 938
		private float _minHeight;

		// Token: 0x040003AB RID: 939
		private bool _gotMaxWidth;

		// Token: 0x040003AC RID: 940
		private bool _gotMaxHeight;

		// Token: 0x040003AD RID: 941
		private bool _gotMinWidth;

		// Token: 0x040003AE RID: 942
		private bool _gotMinHeight;

		// Token: 0x040003AF RID: 943
		private List<WidgetComponent> _components;

		// Token: 0x040003B1 RID: 945
		private List<Action<Widget, string, object[]>> _eventTargets;

		// Token: 0x040003B9 RID: 953
		private bool _doNotAcceptNavigation;

		// Token: 0x040003BA RID: 954
		private bool _isUsingNavigation;

		// Token: 0x040003BB RID: 955
		private bool _useSiblingIndexForNavigation;

		// Token: 0x040003BC RID: 956
		protected internal int _gamepadNavigationIndex = -1;

		// Token: 0x040003BD RID: 957
		private GamepadNavigationTypes _usedNavigationMovements;

		// Token: 0x040003BE RID: 958
		public Action<Widget> OnGamepadNavigationFocusGained;
	}
}
