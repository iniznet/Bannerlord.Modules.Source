using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.GamepadNavigation
{
	// Token: 0x02000043 RID: 67
	public class GamepadNavigationScope
	{
		// Token: 0x1700012B RID: 299
		// (get) Token: 0x060003E6 RID: 998 RVA: 0x0001142C File Offset: 0x0000F62C
		// (set) Token: 0x060003E7 RID: 999 RVA: 0x00011434 File Offset: 0x0000F634
		public string ScopeID { get; set; } = "DefaultScopeID";

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x060003E8 RID: 1000 RVA: 0x0001143D File Offset: 0x0000F63D
		// (set) Token: 0x060003E9 RID: 1001 RVA: 0x00011445 File Offset: 0x0000F645
		public bool IsActiveScope { get; private set; }

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x060003EA RID: 1002 RVA: 0x0001144E File Offset: 0x0000F64E
		// (set) Token: 0x060003EB RID: 1003 RVA: 0x00011456 File Offset: 0x0000F656
		public bool DoNotAutomaticallyFindChildren { get; set; }

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x060003EC RID: 1004 RVA: 0x0001145F File Offset: 0x0000F65F
		// (set) Token: 0x060003ED RID: 1005 RVA: 0x00011467 File Offset: 0x0000F667
		public GamepadNavigationTypes ScopeMovements { get; set; }

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x060003EE RID: 1006 RVA: 0x00011470 File Offset: 0x0000F670
		// (set) Token: 0x060003EF RID: 1007 RVA: 0x00011478 File Offset: 0x0000F678
		public GamepadNavigationTypes AlternateScopeMovements { get; set; }

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x060003F0 RID: 1008 RVA: 0x00011481 File Offset: 0x0000F681
		// (set) Token: 0x060003F1 RID: 1009 RVA: 0x00011489 File Offset: 0x0000F689
		public int AlternateMovementStepSize { get; set; }

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x060003F2 RID: 1010 RVA: 0x00011492 File Offset: 0x0000F692
		// (set) Token: 0x060003F3 RID: 1011 RVA: 0x0001149A File Offset: 0x0000F69A
		public bool HasCircularMovement { get; set; }

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x060003F4 RID: 1012 RVA: 0x000114A3 File Offset: 0x0000F6A3
		public ReadOnlyCollection<Widget> NavigatableWidgets { get; }

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x060003F5 RID: 1013 RVA: 0x000114AB File Offset: 0x0000F6AB
		// (set) Token: 0x060003F6 RID: 1014 RVA: 0x000114B4 File Offset: 0x0000F6B4
		public Widget ParentWidget
		{
			get
			{
				return this._parentWidget;
			}
			set
			{
				if (value != this._parentWidget)
				{
					if (this._parentWidget != null)
					{
						this._invisibleParents.Clear();
						for (Widget widget = this._parentWidget; widget != null; widget = widget.ParentWidget)
						{
							widget.OnVisibilityChanged -= this.OnParentVisibilityChanged;
						}
					}
					this._parentWidget = value;
					for (Widget widget2 = this._parentWidget; widget2 != null; widget2 = widget2.ParentWidget)
					{
						if (!widget2.IsVisible)
						{
							this._invisibleParents.Add(widget2);
						}
						widget2.OnVisibilityChanged += this.OnParentVisibilityChanged;
					}
				}
			}
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x060003F7 RID: 1015 RVA: 0x00011542 File Offset: 0x0000F742
		// (set) Token: 0x060003F8 RID: 1016 RVA: 0x0001154A File Offset: 0x0000F74A
		public int LatestNavigationElementIndex { get; set; }

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x060003F9 RID: 1017 RVA: 0x00011553 File Offset: 0x0000F753
		// (set) Token: 0x060003FA RID: 1018 RVA: 0x0001155B File Offset: 0x0000F75B
		public bool DoNotAutoGainNavigationOnInit { get; set; }

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x060003FB RID: 1019 RVA: 0x00011564 File Offset: 0x0000F764
		// (set) Token: 0x060003FC RID: 1020 RVA: 0x0001156C File Offset: 0x0000F76C
		public bool ForceGainNavigationBasedOnDirection { get; set; }

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x060003FD RID: 1021 RVA: 0x00011575 File Offset: 0x0000F775
		// (set) Token: 0x060003FE RID: 1022 RVA: 0x0001157D File Offset: 0x0000F77D
		public bool ForceGainNavigationOnClosestChild { get; set; }

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x060003FF RID: 1023 RVA: 0x00011586 File Offset: 0x0000F786
		// (set) Token: 0x06000400 RID: 1024 RVA: 0x0001158E File Offset: 0x0000F78E
		public bool NavigateFromScopeEdges { get; set; }

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000401 RID: 1025 RVA: 0x00011597 File Offset: 0x0000F797
		// (set) Token: 0x06000402 RID: 1026 RVA: 0x0001159F File Offset: 0x0000F79F
		public bool UseDiscoveryAreaAsScopeEdges { get; set; }

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000403 RID: 1027 RVA: 0x000115A8 File Offset: 0x0000F7A8
		// (set) Token: 0x06000404 RID: 1028 RVA: 0x000115B0 File Offset: 0x0000F7B0
		public bool DoNotAutoNavigateAfterSort { get; set; }

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06000405 RID: 1029 RVA: 0x000115B9 File Offset: 0x0000F7B9
		// (set) Token: 0x06000406 RID: 1030 RVA: 0x000115C1 File Offset: 0x0000F7C1
		public bool FollowMobileTargets { get; set; }

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06000407 RID: 1031 RVA: 0x000115CA File Offset: 0x0000F7CA
		// (set) Token: 0x06000408 RID: 1032 RVA: 0x000115D2 File Offset: 0x0000F7D2
		public bool DoNotAutoCollectChildScopes { get; set; }

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000409 RID: 1033 RVA: 0x000115DB File Offset: 0x0000F7DB
		// (set) Token: 0x0600040A RID: 1034 RVA: 0x000115E3 File Offset: 0x0000F7E3
		public bool IsDefaultNavigationScope { get; set; }

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x0600040B RID: 1035 RVA: 0x000115EC File Offset: 0x0000F7EC
		// (set) Token: 0x0600040C RID: 1036 RVA: 0x000115F4 File Offset: 0x0000F7F4
		public float ExtendDiscoveryAreaRight { get; set; }

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x0600040D RID: 1037 RVA: 0x000115FD File Offset: 0x0000F7FD
		// (set) Token: 0x0600040E RID: 1038 RVA: 0x00011605 File Offset: 0x0000F805
		public float ExtendDiscoveryAreaTop { get; set; }

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x0600040F RID: 1039 RVA: 0x0001160E File Offset: 0x0000F80E
		// (set) Token: 0x06000410 RID: 1040 RVA: 0x00011616 File Offset: 0x0000F816
		public float ExtendDiscoveryAreaBottom { get; set; }

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000411 RID: 1041 RVA: 0x0001161F File Offset: 0x0000F81F
		// (set) Token: 0x06000412 RID: 1042 RVA: 0x00011627 File Offset: 0x0000F827
		public float ExtendDiscoveryAreaLeft { get; set; }

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000413 RID: 1043 RVA: 0x00011630 File Offset: 0x0000F830
		// (set) Token: 0x06000414 RID: 1044 RVA: 0x00011638 File Offset: 0x0000F838
		public float ExtendChildrenCursorAreaLeft
		{
			get
			{
				return this._extendChildrenCursorAreaLeft;
			}
			set
			{
				if (value != this._extendChildrenCursorAreaLeft)
				{
					this._extendChildrenCursorAreaLeft = value;
					for (int i = 0; i < this._navigatableWidgets.Count; i++)
					{
						this._navigatableWidgets[i].ExtendCursorAreaLeft = value;
					}
				}
			}
		}

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06000415 RID: 1045 RVA: 0x0001167D File Offset: 0x0000F87D
		// (set) Token: 0x06000416 RID: 1046 RVA: 0x00011688 File Offset: 0x0000F888
		public float ExtendChildrenCursorAreaRight
		{
			get
			{
				return this._extendChildrenCursorAreaRight;
			}
			set
			{
				if (value != this._extendChildrenCursorAreaRight)
				{
					this._extendChildrenCursorAreaRight = value;
					for (int i = 0; i < this._navigatableWidgets.Count; i++)
					{
						this._navigatableWidgets[i].ExtendCursorAreaRight = value;
					}
				}
			}
		}

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x06000417 RID: 1047 RVA: 0x000116CD File Offset: 0x0000F8CD
		// (set) Token: 0x06000418 RID: 1048 RVA: 0x000116D8 File Offset: 0x0000F8D8
		public float ExtendChildrenCursorAreaTop
		{
			get
			{
				return this._extendChildrenCursorAreaTop;
			}
			set
			{
				if (value != this._extendChildrenCursorAreaTop)
				{
					this._extendChildrenCursorAreaTop = value;
					for (int i = 0; i < this._navigatableWidgets.Count; i++)
					{
						this._navigatableWidgets[i].ExtendCursorAreaTop = value;
					}
				}
			}
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x06000419 RID: 1049 RVA: 0x0001171D File Offset: 0x0000F91D
		// (set) Token: 0x0600041A RID: 1050 RVA: 0x00011728 File Offset: 0x0000F928
		public float ExtendChildrenCursorAreaBottom
		{
			get
			{
				return this._extendChildrenCursorAreaBottom;
			}
			set
			{
				if (value != this._extendChildrenCursorAreaBottom)
				{
					this._extendChildrenCursorAreaBottom = value;
					for (int i = 0; i < this._navigatableWidgets.Count; i++)
					{
						this._navigatableWidgets[i].ExtendCursorAreaBottom = value;
					}
				}
			}
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x0600041B RID: 1051 RVA: 0x0001176D File Offset: 0x0000F96D
		// (set) Token: 0x0600041C RID: 1052 RVA: 0x00011775 File Offset: 0x0000F975
		public float DiscoveryAreaOffsetX { get; set; }

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x0600041D RID: 1053 RVA: 0x0001177E File Offset: 0x0000F97E
		// (set) Token: 0x0600041E RID: 1054 RVA: 0x00011786 File Offset: 0x0000F986
		public float DiscoveryAreaOffsetY { get; set; }

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x0600041F RID: 1055 RVA: 0x0001178F File Offset: 0x0000F98F
		// (set) Token: 0x06000420 RID: 1056 RVA: 0x00011797 File Offset: 0x0000F997
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
					this.IsDisabled = !value;
					Action<GamepadNavigationScope> onNavigatableWidgetsChanged = this.OnNavigatableWidgetsChanged;
					if (onNavigatableWidgetsChanged == null)
					{
						return;
					}
					onNavigatableWidgetsChanged(this);
				}
			}
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x06000421 RID: 1057 RVA: 0x000117C4 File Offset: 0x0000F9C4
		// (set) Token: 0x06000422 RID: 1058 RVA: 0x000117CC File Offset: 0x0000F9CC
		public bool IsDisabled
		{
			get
			{
				return this._isDisabled;
			}
			set
			{
				if (value != this._isDisabled)
				{
					this._isDisabled = value;
					this.IsEnabled = !value;
				}
			}
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x06000423 RID: 1059 RVA: 0x000117E8 File Offset: 0x0000F9E8
		// (set) Token: 0x06000424 RID: 1060 RVA: 0x000117F6 File Offset: 0x0000F9F6
		public string UpNavigationScopeID
		{
			get
			{
				return this.ManualScopeIDs[GamepadNavigationTypes.Up];
			}
			set
			{
				this.ManualScopeIDs[GamepadNavigationTypes.Up] = value;
			}
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x06000425 RID: 1061 RVA: 0x00011805 File Offset: 0x0000FA05
		// (set) Token: 0x06000426 RID: 1062 RVA: 0x00011813 File Offset: 0x0000FA13
		public string RightNavigationScopeID
		{
			get
			{
				return this.ManualScopeIDs[GamepadNavigationTypes.Right];
			}
			set
			{
				this.ManualScopeIDs[GamepadNavigationTypes.Right] = value;
			}
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000427 RID: 1063 RVA: 0x00011822 File Offset: 0x0000FA22
		// (set) Token: 0x06000428 RID: 1064 RVA: 0x00011830 File Offset: 0x0000FA30
		public string DownNavigationScopeID
		{
			get
			{
				return this.ManualScopeIDs[GamepadNavigationTypes.Down];
			}
			set
			{
				this.ManualScopeIDs[GamepadNavigationTypes.Down] = value;
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000429 RID: 1065 RVA: 0x0001183F File Offset: 0x0000FA3F
		// (set) Token: 0x0600042A RID: 1066 RVA: 0x0001184D File Offset: 0x0000FA4D
		public string LeftNavigationScopeID
		{
			get
			{
				return this.ManualScopeIDs[GamepadNavigationTypes.Left];
			}
			set
			{
				this.ManualScopeIDs[GamepadNavigationTypes.Left] = value;
			}
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x0600042B RID: 1067 RVA: 0x0001185C File Offset: 0x0000FA5C
		// (set) Token: 0x0600042C RID: 1068 RVA: 0x0001186A File Offset: 0x0000FA6A
		public GamepadNavigationScope UpNavigationScope
		{
			get
			{
				return this.ManualScopes[GamepadNavigationTypes.Up];
			}
			set
			{
				this.ManualScopes[GamepadNavigationTypes.Up] = value;
			}
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x0600042D RID: 1069 RVA: 0x00011879 File Offset: 0x0000FA79
		// (set) Token: 0x0600042E RID: 1070 RVA: 0x00011887 File Offset: 0x0000FA87
		public GamepadNavigationScope RightNavigationScope
		{
			get
			{
				return this.ManualScopes[GamepadNavigationTypes.Right];
			}
			set
			{
				this.ManualScopes[GamepadNavigationTypes.Right] = value;
			}
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x0600042F RID: 1071 RVA: 0x00011896 File Offset: 0x0000FA96
		// (set) Token: 0x06000430 RID: 1072 RVA: 0x000118A4 File Offset: 0x0000FAA4
		public GamepadNavigationScope DownNavigationScope
		{
			get
			{
				return this.ManualScopes[GamepadNavigationTypes.Down];
			}
			set
			{
				this.ManualScopes[GamepadNavigationTypes.Down] = value;
			}
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06000431 RID: 1073 RVA: 0x000118B3 File Offset: 0x0000FAB3
		// (set) Token: 0x06000432 RID: 1074 RVA: 0x000118C1 File Offset: 0x0000FAC1
		public GamepadNavigationScope LeftNavigationScope
		{
			get
			{
				return this.ManualScopes[GamepadNavigationTypes.Left];
			}
			set
			{
				this.ManualScopes[GamepadNavigationTypes.Left] = value;
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06000433 RID: 1075 RVA: 0x000118D0 File Offset: 0x0000FAD0
		internal Widget LastNavigatedWidget
		{
			get
			{
				if (this.LatestNavigationElementIndex >= 0 && this.LatestNavigationElementIndex < this._navigatableWidgets.Count)
				{
					return this._navigatableWidgets[this.LatestNavigationElementIndex];
				}
				return null;
			}
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x06000434 RID: 1076 RVA: 0x00011901 File Offset: 0x0000FB01
		// (set) Token: 0x06000435 RID: 1077 RVA: 0x00011909 File Offset: 0x0000FB09
		internal bool IsInitialized { get; private set; }

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x06000436 RID: 1078 RVA: 0x00011912 File Offset: 0x0000FB12
		// (set) Token: 0x06000437 RID: 1079 RVA: 0x0001191A File Offset: 0x0000FB1A
		internal GamepadNavigationScope PreviousScope { get; set; }

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x06000438 RID: 1080 RVA: 0x00011923 File Offset: 0x0000FB23
		// (set) Token: 0x06000439 RID: 1081 RVA: 0x0001192B File Offset: 0x0000FB2B
		internal Dictionary<GamepadNavigationTypes, string> ManualScopeIDs { get; private set; }

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x0600043A RID: 1082 RVA: 0x00011934 File Offset: 0x0000FB34
		// (set) Token: 0x0600043B RID: 1083 RVA: 0x0001193C File Offset: 0x0000FB3C
		internal Dictionary<GamepadNavigationTypes, GamepadNavigationScope> ManualScopes { get; private set; }

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x00011945 File Offset: 0x0000FB45
		// (set) Token: 0x0600043D RID: 1085 RVA: 0x0001194D File Offset: 0x0000FB4D
		internal bool IsAdditionalMovementsDirty { get; set; }

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x0600043E RID: 1086 RVA: 0x00011956 File Offset: 0x0000FB56
		// (set) Token: 0x0600043F RID: 1087 RVA: 0x0001195E File Offset: 0x0000FB5E
		internal Dictionary<GamepadNavigationTypes, GamepadNavigationScope> InterScopeMovements { get; private set; }

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x00011967 File Offset: 0x0000FB67
		// (set) Token: 0x06000441 RID: 1089 RVA: 0x0001196F File Offset: 0x0000FB6F
		internal GamepadNavigationScope ParentScope { get; private set; }

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x00011978 File Offset: 0x0000FB78
		// (set) Token: 0x06000443 RID: 1091 RVA: 0x00011980 File Offset: 0x0000FB80
		internal ReadOnlyCollection<GamepadNavigationScope> ChildScopes { get; private set; }

		// Token: 0x06000444 RID: 1092 RVA: 0x0001198C File Offset: 0x0000FB8C
		public GamepadNavigationScope()
		{
			this._widgetIndices = new Dictionary<Widget, int>();
			this._navigatableWidgets = new List<Widget>();
			this.NavigatableWidgets = new ReadOnlyCollection<Widget>(this._navigatableWidgets);
			this._invisibleParents = new List<Widget>();
			this.InterScopeMovements = new Dictionary<GamepadNavigationTypes, GamepadNavigationScope>
			{
				{
					GamepadNavigationTypes.Up,
					null
				},
				{
					GamepadNavigationTypes.Right,
					null
				},
				{
					GamepadNavigationTypes.Down,
					null
				},
				{
					GamepadNavigationTypes.Left,
					null
				}
			};
			this.ManualScopeIDs = new Dictionary<GamepadNavigationTypes, string>
			{
				{
					GamepadNavigationTypes.Up,
					null
				},
				{
					GamepadNavigationTypes.Right,
					null
				},
				{
					GamepadNavigationTypes.Down,
					null
				},
				{
					GamepadNavigationTypes.Left,
					null
				}
			};
			this.ManualScopes = new Dictionary<GamepadNavigationTypes, GamepadNavigationScope>
			{
				{
					GamepadNavigationTypes.Up,
					null
				},
				{
					GamepadNavigationTypes.Right,
					null
				},
				{
					GamepadNavigationTypes.Down,
					null
				},
				{
					GamepadNavigationTypes.Left,
					null
				}
			};
			this._navigatableWidgetComparer = new GamepadNavigationScope.WidgetNavigationIndexComparer();
			this.LatestNavigationElementIndex = -1;
			this._childScopes = new List<GamepadNavigationScope>();
			this.ChildScopes = new ReadOnlyCollection<GamepadNavigationScope>(this._childScopes);
			this.IsInitialized = false;
			this.IsEnabled = true;
		}

		// Token: 0x06000445 RID: 1093 RVA: 0x00011A9C File Offset: 0x0000FC9C
		public void AddWidgetAtIndex(Widget widget, int index)
		{
			if (index < this._navigatableWidgets.Count)
			{
				this._navigatableWidgets.Insert(index, widget);
				this._widgetIndices.Add(widget, index);
			}
			else
			{
				this._navigatableWidgets.Add(widget);
				this._widgetIndices.Add(widget, this._navigatableWidgets.Count - 1);
			}
			Action<GamepadNavigationScope> onNavigatableWidgetsChanged = this.OnNavigatableWidgetsChanged;
			if (onNavigatableWidgetsChanged != null)
			{
				onNavigatableWidgetsChanged(this);
			}
			this.SetCursorAreaExtensionsForChild(widget);
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x00011B11 File Offset: 0x0000FD11
		public void AddWidget(Widget widget)
		{
			this._navigatableWidgets.Add(widget);
			Action<GamepadNavigationScope> onNavigatableWidgetsChanged = this.OnNavigatableWidgetsChanged;
			if (onNavigatableWidgetsChanged != null)
			{
				onNavigatableWidgetsChanged(this);
			}
			this.SetCursorAreaExtensionsForChild(widget);
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x00011B38 File Offset: 0x0000FD38
		public void RemoveWidget(Widget widget)
		{
			this._navigatableWidgets.Remove(widget);
			Action<GamepadNavigationScope> onNavigatableWidgetsChanged = this.OnNavigatableWidgetsChanged;
			if (onNavigatableWidgetsChanged == null)
			{
				return;
			}
			onNavigatableWidgetsChanged(this);
		}

		// Token: 0x06000448 RID: 1096 RVA: 0x00011B58 File Offset: 0x0000FD58
		public void SetParentScope(GamepadNavigationScope scope)
		{
			if (this.ParentScope != null)
			{
				this.ParentScope._childScopes.Remove(this);
			}
			GamepadNavigationScope parentScope = this.ParentScope;
			this.ParentScope = scope;
			Action<GamepadNavigationScope, GamepadNavigationScope> onParentScopeChanged = this.OnParentScopeChanged;
			if (onParentScopeChanged != null)
			{
				onParentScopeChanged(parentScope, this.ParentScope);
			}
			if (this.ParentScope != null)
			{
				this.ParentScope._childScopes.Add(this);
				this.ClearMyWidgetsFromParentScope();
			}
		}

		// Token: 0x06000449 RID: 1097 RVA: 0x00011BC4 File Offset: 0x0000FDC4
		internal void SetIsActiveScope(bool isActive)
		{
			this.IsActiveScope = isActive;
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x00011BCD File Offset: 0x0000FDCD
		internal bool IsVisible()
		{
			return this._invisibleParents.Count == 0;
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x00011BDD File Offset: 0x0000FDDD
		internal bool IsAvailable()
		{
			return this.IsEnabled && this._navigatableWidgets.Count > 0 && this.IsVisible();
		}

		// Token: 0x0600044C RID: 1100 RVA: 0x00011BFD File Offset: 0x0000FDFD
		internal void Initialize()
		{
			if (!this.DoNotAutomaticallyFindChildren)
			{
				this.FindNavigatableChildren();
			}
			this.IsInitialized = true;
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x00011C14 File Offset: 0x0000FE14
		internal void RefreshNavigatableChildren()
		{
			if (this.IsInitialized)
			{
				this.FindNavigatableChildren();
			}
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x00011C24 File Offset: 0x0000FE24
		internal bool HasMovement(GamepadNavigationTypes movement)
		{
			return (this.ScopeMovements & movement) != GamepadNavigationTypes.None || (this.AlternateScopeMovements & movement) > GamepadNavigationTypes.None;
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x00011C3D File Offset: 0x0000FE3D
		private void FindNavigatableChildren()
		{
			this._navigatableWidgets.Clear();
			if (this.IsParentWidgetAvailableForNavigation())
			{
				this.CollectNavigatableChildrenOfWidget(this.ParentWidget);
			}
			Action<GamepadNavigationScope> onNavigatableWidgetsChanged = this.OnNavigatableWidgetsChanged;
			if (onNavigatableWidgetsChanged == null)
			{
				return;
			}
			onNavigatableWidgetsChanged(this);
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x00011C70 File Offset: 0x0000FE70
		private bool IsParentWidgetAvailableForNavigation()
		{
			for (Widget widget = this.ParentWidget; widget != null; widget = widget.ParentWidget)
			{
				if (widget.DoNotAcceptNavigation)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x00011C9C File Offset: 0x0000FE9C
		private void CollectNavigatableChildrenOfWidget(Widget widget)
		{
			if (widget.DoNotAcceptNavigation)
			{
				return;
			}
			for (int i = 0; i < this._childScopes.Count; i++)
			{
				if (this._childScopes[i].ParentWidget == widget)
				{
					return;
				}
			}
			if (widget.GamepadNavigationIndex != -1)
			{
				this._navigatableWidgets.Add(widget);
			}
			List<GamepadNavigationScope> list;
			if (!this.DoNotAutoCollectChildScopes && this.ParentWidget != widget && GauntletGamepadNavigationManager.Instance.NavigationScopeParents.TryGetValue(widget, out list))
			{
				for (int j = 0; j < list.Count; j++)
				{
					list[j].SetParentScope(this);
				}
			}
			for (int k = 0; k < widget.Children.Count; k++)
			{
				this.CollectNavigatableChildrenOfWidget(widget.Children[k]);
			}
			this.ClearMyWidgetsFromParentScope();
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x00011D64 File Offset: 0x0000FF64
		internal GamepadNavigationTypes GetMovementsToReachMyPosition(Vector2 fromPosition)
		{
			Rectangle rectangle = this.GetRectangle();
			GamepadNavigationTypes gamepadNavigationTypes = GamepadNavigationTypes.None;
			if (fromPosition.X > rectangle.X + rectangle.Width)
			{
				gamepadNavigationTypes |= GamepadNavigationTypes.Left;
			}
			else if (fromPosition.X < rectangle.X)
			{
				gamepadNavigationTypes |= GamepadNavigationTypes.Right;
			}
			if (fromPosition.Y > rectangle.Y + rectangle.Height)
			{
				gamepadNavigationTypes |= GamepadNavigationTypes.Up;
			}
			else if (fromPosition.Y < rectangle.Y)
			{
				gamepadNavigationTypes |= GamepadNavigationTypes.Down;
			}
			return gamepadNavigationTypes;
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x00011DD7 File Offset: 0x0000FFD7
		internal bool GetShouldFindScopeByPosition(GamepadNavigationTypes movement)
		{
			return this.ManualScopeIDs[movement] == null && this.ManualScopes[movement] == null;
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x00011DF8 File Offset: 0x0000FFF8
		internal GamepadNavigationTypes GetMovementsInsideScope()
		{
			GamepadNavigationTypes gamepadNavigationTypes = this.ScopeMovements;
			GamepadNavigationTypes gamepadNavigationTypes2 = this.AlternateScopeMovements;
			if (!this.HasCircularMovement || this._navigatableWidgets.Count == 1)
			{
				bool flag = false;
				bool flag2 = false;
				if (this.LatestNavigationElementIndex >= 0 && this.LatestNavigationElementIndex < this._navigatableWidgets.Count)
				{
					for (int i = this.LatestNavigationElementIndex + 1; i < this._navigatableWidgets.Count; i++)
					{
						if (this.IsWidgetVisible(this._navigatableWidgets[i]))
						{
							flag2 = true;
							break;
						}
					}
					int num = this.LatestNavigationElementIndex - 1;
					if (this.HasCircularMovement && num < 0)
					{
						num = this._navigatableWidgets.Count - 1;
					}
					for (int j = num; j >= 0; j--)
					{
						if (this.IsWidgetVisible(this._navigatableWidgets[j]))
						{
							flag = true;
							break;
						}
					}
				}
				if (this.LatestNavigationElementIndex == 0 || !flag)
				{
					gamepadNavigationTypes &= ~GamepadNavigationTypes.Left;
					gamepadNavigationTypes &= ~GamepadNavigationTypes.Up;
				}
				if (this.LatestNavigationElementIndex == this.NavigatableWidgets.Count - 1 || !flag2)
				{
					gamepadNavigationTypes &= ~GamepadNavigationTypes.Right;
					gamepadNavigationTypes &= ~GamepadNavigationTypes.Down;
				}
				if (gamepadNavigationTypes2 != GamepadNavigationTypes.None && this.AlternateMovementStepSize > 0)
				{
					if (this.LatestNavigationElementIndex % this.AlternateMovementStepSize == 0)
					{
						gamepadNavigationTypes &= ~GamepadNavigationTypes.Left;
						gamepadNavigationTypes &= ~GamepadNavigationTypes.Up;
					}
					if (this.LatestNavigationElementIndex % this.AlternateMovementStepSize == this.AlternateMovementStepSize - 1)
					{
						gamepadNavigationTypes &= ~GamepadNavigationTypes.Right;
						gamepadNavigationTypes &= ~GamepadNavigationTypes.Down;
					}
					if (this.LatestNavigationElementIndex - this.AlternateMovementStepSize < 0)
					{
						gamepadNavigationTypes2 &= ~GamepadNavigationTypes.Up;
						gamepadNavigationTypes2 &= ~GamepadNavigationTypes.Left;
					}
					int num2 = this._navigatableWidgets.Count % this.AlternateMovementStepSize;
					if (this._navigatableWidgets.Count > 0 && num2 == 0)
					{
						num2 = this.AlternateMovementStepSize;
					}
					if (this.LatestNavigationElementIndex + num2 > this._navigatableWidgets.Count - 1)
					{
						gamepadNavigationTypes2 &= ~GamepadNavigationTypes.Right;
						gamepadNavigationTypes2 &= ~GamepadNavigationTypes.Down;
					}
				}
			}
			return gamepadNavigationTypes | gamepadNavigationTypes2;
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x00011FCC File Offset: 0x000101CC
		internal int FindIndexOfWidget(Widget widget)
		{
			int num;
			if (widget != null && this._navigatableWidgets.Count != 0 && this._widgetIndices.TryGetValue(widget, out num))
			{
				return num;
			}
			return -1;
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x00011FFC File Offset: 0x000101FC
		internal void SortWidgets()
		{
			this._navigatableWidgets.Sort(this._navigatableWidgetComparer);
			this._widgetIndices.Clear();
			for (int i = 0; i < this._navigatableWidgets.Count; i++)
			{
				this._widgetIndices.Add(this._navigatableWidgets[i], i);
			}
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x00012053 File Offset: 0x00010253
		public void ClearNavigatableWidgets()
		{
			this._navigatableWidgets.Clear();
			this._widgetIndices.Clear();
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x0001206C File Offset: 0x0001026C
		internal Rectangle GetDiscoveryRectangle()
		{
			float customScale = this.ParentWidget.EventManager.Context.CustomScale;
			return new Rectangle(this.DiscoveryAreaOffsetX + this.ParentWidget.GlobalPosition.X - this.ExtendDiscoveryAreaLeft * customScale, this.DiscoveryAreaOffsetY + this.ParentWidget.GlobalPosition.Y - this.ExtendDiscoveryAreaTop * customScale, this.ParentWidget.Size.X + (this.ExtendDiscoveryAreaLeft + this.ExtendDiscoveryAreaRight) * customScale, this.ParentWidget.Size.Y + (this.ExtendDiscoveryAreaTop + this.ExtendDiscoveryAreaBottom) * customScale);
		}

		// Token: 0x06000459 RID: 1113 RVA: 0x00012114 File Offset: 0x00010314
		internal Rectangle GetRectangle()
		{
			if (this.ParentWidget == null)
			{
				return new Rectangle(0f, 0f, 1f, 1f);
			}
			return new Rectangle(this.ParentWidget.GlobalPosition.X, this.ParentWidget.GlobalPosition.Y, this.ParentWidget.Size.X, this.ParentWidget.Size.Y);
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x00012188 File Offset: 0x00010388
		internal bool IsWidgetVisible(Widget widget)
		{
			for (Widget widget2 = widget; widget2 != null; widget2 = widget2.ParentWidget)
			{
				if (!widget2.IsVisible)
				{
					return false;
				}
				if (widget2 == this.ParentWidget)
				{
					return this.IsVisible();
				}
			}
			return true;
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x000121C0 File Offset: 0x000103C0
		internal Widget GetFirstAvailableWidget()
		{
			int num = -1;
			for (int i = 0; i < this._navigatableWidgets.Count; i++)
			{
				if (this.IsWidgetVisible(this._navigatableWidgets[i]))
				{
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				return this._navigatableWidgets[num];
			}
			return null;
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x00012210 File Offset: 0x00010410
		internal Widget GetLastAvailableWidget()
		{
			int num = -1;
			for (int i = this._navigatableWidgets.Count - 1; i >= 0; i--)
			{
				if (this.IsWidgetVisible(this._navigatableWidgets[i]))
				{
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				return this._navigatableWidgets[num];
			}
			return null;
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x00012261 File Offset: 0x00010461
		private int GetApproximatelyClosestWidgetIndexToPosition(Vector2 position, out float distance, GamepadNavigationTypes movement = GamepadNavigationTypes.None, bool angleCheck = false)
		{
			if (this._navigatableWidgets.Count <= 0)
			{
				distance = -1f;
				return -1;
			}
			if (this.AlternateMovementStepSize > 0)
			{
				return this.GetClosesWidgetIndexForWithAlternateMovement(position, out distance, movement, angleCheck);
			}
			return this.GetClosesWidgetIndexForRegular(position, out distance, movement, angleCheck);
		}

		// Token: 0x0600045E RID: 1118 RVA: 0x0001229C File Offset: 0x0001049C
		internal Widget GetApproximatelyClosestWidgetToPosition(Vector2 position, GamepadNavigationTypes movement = GamepadNavigationTypes.None, bool angleCheck = false)
		{
			float num;
			return this.GetApproximatelyClosestWidgetToPosition(position, out num, movement, angleCheck);
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x000122B4 File Offset: 0x000104B4
		internal Widget GetApproximatelyClosestWidgetToPosition(Vector2 position, out float distance, GamepadNavigationTypes movement = GamepadNavigationTypes.None, bool angleCheck = false)
		{
			int approximatelyClosestWidgetIndexToPosition = this.GetApproximatelyClosestWidgetIndexToPosition(position, out distance, movement, angleCheck);
			if (approximatelyClosestWidgetIndexToPosition != -1)
			{
				return this._navigatableWidgets[approximatelyClosestWidgetIndexToPosition];
			}
			return null;
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x000122E0 File Offset: 0x000104E0
		private void OnParentVisibilityChanged(Widget parent)
		{
			bool flag = this.IsVisible();
			if (!parent.IsVisible)
			{
				this._invisibleParents.Add(parent);
			}
			else
			{
				this._invisibleParents.Remove(parent);
			}
			bool flag2 = this.IsVisible();
			if (flag != flag2)
			{
				Action<GamepadNavigationScope, bool> onVisibilityChanged = this.OnVisibilityChanged;
				if (onVisibilityChanged == null)
				{
					return;
				}
				onVisibilityChanged(this, flag2);
			}
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x00012334 File Offset: 0x00010534
		private void ClearMyWidgetsFromParentScope()
		{
			if (this.ParentScope != null)
			{
				for (int i = 0; i < this._navigatableWidgets.Count; i++)
				{
					this.ParentScope.RemoveWidget(this._navigatableWidgets[i]);
				}
			}
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x00012378 File Offset: 0x00010578
		private Vector2 GetRelativePositionRatio(Vector2 position)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			for (int i = 0; i < this._navigatableWidgets.Count; i++)
			{
				if (this.IsWidgetVisible(this._navigatableWidgets[i]))
				{
					num2 = this._navigatableWidgets[i].GlobalPosition.Y;
					num4 = this._navigatableWidgets[i].GlobalPosition.X;
					break;
				}
			}
			for (int j = this._navigatableWidgets.Count - 1; j >= 0; j--)
			{
				if (this.IsWidgetVisible(this._navigatableWidgets[j]))
				{
					num = this._navigatableWidgets[j].GlobalPosition.Y + this._navigatableWidgets[j].Size.Y;
					num3 = this._navigatableWidgets[j].GlobalPosition.X + this._navigatableWidgets[j].Size.X;
					break;
				}
			}
			float num5 = Mathf.Clamp(GamepadNavigationScope.InverseLerp(num4, num3, position.X), 0f, 1f);
			float num6 = Mathf.Clamp(GamepadNavigationScope.InverseLerp(num2, num, position.Y), 0f, 1f);
			return new Vector2(num5, num6);
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x000124D8 File Offset: 0x000106D8
		private bool IsPositionAvailableForMovement(Vector2 fromPos, Vector2 toPos, GamepadNavigationTypes movement)
		{
			if (movement == GamepadNavigationTypes.Right)
			{
				return fromPos.X <= toPos.X;
			}
			if (movement == GamepadNavigationTypes.Left)
			{
				return fromPos.X >= toPos.X;
			}
			if (movement == GamepadNavigationTypes.Up)
			{
				return fromPos.Y >= toPos.Y;
			}
			return movement != GamepadNavigationTypes.Down || fromPos.Y <= toPos.Y;
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x00012540 File Offset: 0x00010740
		private int GetClosesWidgetIndexForWithAlternateMovement(Vector2 fromPos, out float distance, GamepadNavigationTypes movement = GamepadNavigationTypes.None, bool angleCheck = false)
		{
			distance = -1f;
			List<int> list = new List<int>();
			Vector2 relativePositionRatio = this.GetRelativePositionRatio(fromPos);
			float num = float.MaxValue;
			int num2 = -1;
			Rectangle rectangle = this.GetRectangle();
			if (!rectangle.IsPointInside(fromPos))
			{
				List<int> list2 = new List<int>();
				if (fromPos.X < rectangle.X)
				{
					for (int i = 0; i < this._navigatableWidgets.Count; i += this.AlternateMovementStepSize)
					{
						list2.Add(i);
					}
				}
				else if (fromPos.X > rectangle.X2)
				{
					for (int j = MathF.Min(this.AlternateMovementStepSize - 1, this._navigatableWidgets.Count - 1); j < this._navigatableWidgets.Count; j += this.AlternateMovementStepSize)
					{
						list2.Add(j);
					}
				}
				if (list2.Count > 0)
				{
					int[] targetIndicesFromListByRatio = GamepadNavigationScope.GetTargetIndicesFromListByRatio(relativePositionRatio.Y, list2);
					for (int k = 0; k < targetIndicesFromListByRatio.Length; k++)
					{
						list.Add(targetIndicesFromListByRatio[k]);
					}
				}
				if (fromPos.Y < rectangle.Y)
				{
					int num3 = Mathf.Clamp(this.AlternateMovementStepSize - 1, 0, this._navigatableWidgets.Count - 1);
					int[] targetIndicesByRatio = GamepadNavigationScope.GetTargetIndicesByRatio(relativePositionRatio.X, 0, num3, 5);
					for (int l = 0; l < targetIndicesByRatio.Length; l++)
					{
						list.Add(targetIndicesByRatio[l]);
					}
				}
				else if (fromPos.Y > rectangle.Y2)
				{
					int num4 = this._navigatableWidgets.Count % this.AlternateMovementStepSize;
					if (this._navigatableWidgets.Count > 0 && num4 == 0)
					{
						num4 = this.AlternateMovementStepSize;
					}
					int num5 = Mathf.Clamp(this._navigatableWidgets.Count - num4, 0, this._navigatableWidgets.Count - 1);
					int[] targetIndicesByRatio2 = GamepadNavigationScope.GetTargetIndicesByRatio(relativePositionRatio.X, num5, this._navigatableWidgets.Count - 1, 5);
					for (int m = 0; m < targetIndicesByRatio2.Length; m++)
					{
						list.Add(targetIndicesByRatio2[m]);
					}
				}
				for (int n = 0; n < list.Count; n++)
				{
					int num6 = list[n];
					Vector2 vector;
					float distanceToClosestWidgetEdge = GamepadNavigationHelper.GetDistanceToClosestWidgetEdge(this._navigatableWidgets[num6], fromPos, movement, out vector);
					if (distanceToClosestWidgetEdge < num && (!angleCheck || this.IsPositionAvailableForMovement(fromPos, vector, movement)))
					{
						num = distanceToClosestWidgetEdge;
						distance = num;
						num2 = num6;
					}
				}
			}
			else
			{
				num2 = this.GetClosesWidgetIndexForRegular(fromPos, out distance, GamepadNavigationTypes.None, false);
			}
			return num2;
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x000127A8 File Offset: 0x000109A8
		private int GetClosestWidgetIndexForRegularInefficient(Vector2 fromPos, out float distance, GamepadNavigationTypes movement = GamepadNavigationTypes.None, bool angleCheck = false)
		{
			distance = -1f;
			int num = -1;
			float num2 = float.MaxValue;
			for (int i = 0; i < this._navigatableWidgets.Count; i++)
			{
				Vector2 vector;
				float distanceToClosestWidgetEdge = GamepadNavigationHelper.GetDistanceToClosestWidgetEdge(this._navigatableWidgets[i], fromPos, movement, out vector);
				if (distanceToClosestWidgetEdge < num2 && this.IsWidgetVisible(this._navigatableWidgets[i]) && (!angleCheck || this.IsPositionAvailableForMovement(fromPos, vector, movement)))
				{
					num2 = distanceToClosestWidgetEdge;
					distance = num2;
					num = i;
				}
			}
			return num;
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x00012820 File Offset: 0x00010A20
		private int GetClosesWidgetIndexForRegular(Vector2 fromPos, out float distance, GamepadNavigationTypes movement = GamepadNavigationTypes.None, bool angleCheck = false)
		{
			distance = -1f;
			List<int> list = new List<int>();
			Vector2 relativePositionRatio = this.GetRelativePositionRatio(fromPos);
			int[] targetIndicesByRatio = GamepadNavigationScope.GetTargetIndicesByRatio(relativePositionRatio.X, 0, this._navigatableWidgets.Count - 1, 5);
			int[] targetIndicesByRatio2 = GamepadNavigationScope.GetTargetIndicesByRatio(relativePositionRatio.Y, 0, this._navigatableWidgets.Count - 1, 5);
			for (int i = 0; i < targetIndicesByRatio.Length; i++)
			{
				if (!list.Contains(targetIndicesByRatio[i]))
				{
					list.Add(targetIndicesByRatio[i]);
				}
			}
			for (int j = 0; j < targetIndicesByRatio2.Length; j++)
			{
				if (!list.Contains(targetIndicesByRatio2[j]))
				{
					list.Add(targetIndicesByRatio2[j]);
				}
			}
			float num = float.MaxValue;
			int num2 = -1;
			int num3 = 0;
			for (int k = 0; k < list.Count; k++)
			{
				int num4 = list[k];
				if (num4 != -1 && this.IsWidgetVisible(this._navigatableWidgets[num4]))
				{
					num3++;
					Vector2 vector;
					float distanceToClosestWidgetEdge = GamepadNavigationHelper.GetDistanceToClosestWidgetEdge(this._navigatableWidgets[num4], fromPos, movement, out vector);
					if (distanceToClosestWidgetEdge < num && (!angleCheck || this.IsPositionAvailableForMovement(fromPos, vector, movement)))
					{
						num = distanceToClosestWidgetEdge;
						distance = num;
						num2 = num4;
					}
				}
			}
			if (num3 == 0)
			{
				return this.GetClosestWidgetIndexForRegularInefficient(fromPos, out distance, GamepadNavigationTypes.None, false);
			}
			return num2;
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x00012959 File Offset: 0x00010B59
		private static float InverseLerp(float fromValue, float toValue, float value)
		{
			if (fromValue == toValue)
			{
				return 0f;
			}
			return (value - fromValue) / (toValue - fromValue);
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x0001296C File Offset: 0x00010B6C
		private static int[] GetTargetIndicesFromListByRatio(float ratio, List<int> lookupIndices)
		{
			int num = MathF.Round((float)lookupIndices.Count * ratio);
			return new int[]
			{
				lookupIndices[Mathf.Clamp(num - 2, 0, lookupIndices.Count - 1)],
				lookupIndices[Mathf.Clamp(num - 1, 0, lookupIndices.Count - 1)],
				lookupIndices[Mathf.Clamp(num, 0, lookupIndices.Count - 1)],
				lookupIndices[Mathf.Clamp(num + 1, 0, lookupIndices.Count - 1)],
				lookupIndices[Mathf.Clamp(num + 2, 0, lookupIndices.Count - 1)]
			};
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x00012A10 File Offset: 0x00010C10
		private static int[] GetTargetIndicesByRatio(float ratio, int startIndex, int endIndex, int arraySize = 5)
		{
			int num = MathF.Round((float)startIndex + (float)(endIndex - startIndex) * ratio);
			int[] array = new int[arraySize];
			int num2 = MathF.Floor((float)arraySize / 2f);
			for (int i = 0; i < arraySize; i++)
			{
				int num3 = -num2 + i;
				array[i] = Mathf.Clamp(num - num3, 0, endIndex);
			}
			return array;
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x00012A64 File Offset: 0x00010C64
		private void SetCursorAreaExtensionsForChild(Widget child)
		{
			if (this.ExtendChildrenCursorAreaLeft != 0f)
			{
				child.ExtendCursorAreaLeft = this.ExtendChildrenCursorAreaLeft;
			}
			if (this.ExtendChildrenCursorAreaRight != 0f)
			{
				child.ExtendCursorAreaRight = this.ExtendChildrenCursorAreaRight;
			}
			if (this.ExtendChildrenCursorAreaTop != 0f)
			{
				child.ExtendCursorAreaTop = this.ExtendChildrenCursorAreaTop;
			}
			if (this.ExtendChildrenCursorAreaBottom != 0f)
			{
				child.ExtendCursorAreaBottom = this.ExtendChildrenCursorAreaBottom;
			}
		}

		// Token: 0x04000201 RID: 513
		private List<Widget> _navigatableWidgets;

		// Token: 0x04000203 RID: 515
		private Dictionary<Widget, int> _widgetIndices;

		// Token: 0x04000204 RID: 516
		private Widget _parentWidget;

		// Token: 0x04000213 RID: 531
		private float _extendChildrenCursorAreaLeft;

		// Token: 0x04000214 RID: 532
		private float _extendChildrenCursorAreaRight;

		// Token: 0x04000215 RID: 533
		private float _extendChildrenCursorAreaTop;

		// Token: 0x04000216 RID: 534
		private float _extendChildrenCursorAreaBottom;

		// Token: 0x04000219 RID: 537
		private bool _isEnabled;

		// Token: 0x0400021A RID: 538
		private bool _isDisabled;

		// Token: 0x04000221 RID: 545
		private GamepadNavigationScope.WidgetNavigationIndexComparer _navigatableWidgetComparer;

		// Token: 0x04000222 RID: 546
		private List<Widget> _invisibleParents;

		// Token: 0x04000224 RID: 548
		private List<GamepadNavigationScope> _childScopes;

		// Token: 0x04000226 RID: 550
		internal Action<GamepadNavigationScope> OnNavigatableWidgetsChanged;

		// Token: 0x04000227 RID: 551
		internal Action<GamepadNavigationScope, bool> OnVisibilityChanged;

		// Token: 0x04000228 RID: 552
		internal Action<GamepadNavigationScope, GamepadNavigationScope> OnParentScopeChanged;

		// Token: 0x02000080 RID: 128
		private class WidgetNavigationIndexComparer : IComparer<Widget>
		{
			// Token: 0x06000897 RID: 2199 RVA: 0x00022B28 File Offset: 0x00020D28
			public int Compare(Widget x, Widget y)
			{
				return x.GamepadNavigationIndex.CompareTo(y.GamepadNavigationIndex);
			}
		}
	}
}
