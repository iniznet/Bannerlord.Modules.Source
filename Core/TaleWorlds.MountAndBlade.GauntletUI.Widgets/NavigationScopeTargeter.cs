using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200002B RID: 43
	public class NavigationScopeTargeter : Widget
	{
		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000231 RID: 561 RVA: 0x00007ED1 File Offset: 0x000060D1
		// (set) Token: 0x06000232 RID: 562 RVA: 0x00007ED9 File Offset: 0x000060D9
		public GamepadNavigationScope NavigationScope { get; private set; }

		// Token: 0x06000233 RID: 563 RVA: 0x00007EE2 File Offset: 0x000060E2
		public NavigationScopeTargeter(UIContext context)
			: base(context)
		{
			this.NavigationScope = new GamepadNavigationScope();
			base.WidthSizePolicy = SizePolicy.Fixed;
			base.HeightSizePolicy = SizePolicy.Fixed;
			base.SuggestedHeight = 0f;
			base.SuggestedWidth = 0f;
			base.IsVisible = false;
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000234 RID: 564 RVA: 0x00007F21 File Offset: 0x00006121
		// (set) Token: 0x06000235 RID: 565 RVA: 0x00007F2E File Offset: 0x0000612E
		public string ScopeID
		{
			get
			{
				return this.NavigationScope.ScopeID;
			}
			set
			{
				if (value != this.NavigationScope.ScopeID)
				{
					this.NavigationScope.ScopeID = value;
				}
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000236 RID: 566 RVA: 0x00007F4F File Offset: 0x0000614F
		// (set) Token: 0x06000237 RID: 567 RVA: 0x00007F5C File Offset: 0x0000615C
		public GamepadNavigationTypes ScopeMovements
		{
			get
			{
				return this.NavigationScope.ScopeMovements;
			}
			set
			{
				if (value != this.NavigationScope.ScopeMovements)
				{
					this.NavigationScope.ScopeMovements = value;
				}
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000238 RID: 568 RVA: 0x00007F78 File Offset: 0x00006178
		// (set) Token: 0x06000239 RID: 569 RVA: 0x00007F85 File Offset: 0x00006185
		public GamepadNavigationTypes AlternateScopeMovements
		{
			get
			{
				return this.NavigationScope.AlternateScopeMovements;
			}
			set
			{
				if (value != this.NavigationScope.AlternateScopeMovements)
				{
					this.NavigationScope.AlternateScopeMovements = value;
				}
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x0600023A RID: 570 RVA: 0x00007FA1 File Offset: 0x000061A1
		// (set) Token: 0x0600023B RID: 571 RVA: 0x00007FAE File Offset: 0x000061AE
		public int AlternateMovementStepSize
		{
			get
			{
				return this.NavigationScope.AlternateMovementStepSize;
			}
			set
			{
				if (value != this.NavigationScope.AlternateMovementStepSize)
				{
					this.NavigationScope.AlternateMovementStepSize = value;
				}
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x0600023C RID: 572 RVA: 0x00007FCA File Offset: 0x000061CA
		// (set) Token: 0x0600023D RID: 573 RVA: 0x00007FD7 File Offset: 0x000061D7
		public bool HasCircularMovement
		{
			get
			{
				return this.NavigationScope.HasCircularMovement;
			}
			set
			{
				if (value != this.NavigationScope.HasCircularMovement)
				{
					this.NavigationScope.HasCircularMovement = value;
				}
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x0600023E RID: 574 RVA: 0x00007FF3 File Offset: 0x000061F3
		// (set) Token: 0x0600023F RID: 575 RVA: 0x00008000 File Offset: 0x00006200
		public bool DoNotAutomaticallyFindChildren
		{
			get
			{
				return this.NavigationScope.DoNotAutomaticallyFindChildren;
			}
			set
			{
				if (value != this.NavigationScope.DoNotAutomaticallyFindChildren)
				{
					this.NavigationScope.DoNotAutomaticallyFindChildren = value;
				}
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000240 RID: 576 RVA: 0x0000801C File Offset: 0x0000621C
		// (set) Token: 0x06000241 RID: 577 RVA: 0x00008029 File Offset: 0x00006229
		public bool DoNotAutoGainNavigationOnInit
		{
			get
			{
				return this.NavigationScope.DoNotAutoGainNavigationOnInit;
			}
			set
			{
				if (value != this.NavigationScope.DoNotAutoGainNavigationOnInit)
				{
					this.NavigationScope.DoNotAutoGainNavigationOnInit = value;
				}
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000242 RID: 578 RVA: 0x00008045 File Offset: 0x00006245
		// (set) Token: 0x06000243 RID: 579 RVA: 0x00008052 File Offset: 0x00006252
		public bool ForceGainNavigationBasedOnDirection
		{
			get
			{
				return this.NavigationScope.ForceGainNavigationBasedOnDirection;
			}
			set
			{
				if (value != this.NavigationScope.ForceGainNavigationBasedOnDirection)
				{
					this.NavigationScope.ForceGainNavigationBasedOnDirection = value;
				}
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000244 RID: 580 RVA: 0x0000806E File Offset: 0x0000626E
		// (set) Token: 0x06000245 RID: 581 RVA: 0x0000807B File Offset: 0x0000627B
		public bool ForceGainNavigationOnClosestChild
		{
			get
			{
				return this.NavigationScope.ForceGainNavigationOnClosestChild;
			}
			set
			{
				if (value != this.NavigationScope.ForceGainNavigationOnClosestChild)
				{
					this.NavigationScope.ForceGainNavigationOnClosestChild = value;
				}
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000246 RID: 582 RVA: 0x00008097 File Offset: 0x00006297
		// (set) Token: 0x06000247 RID: 583 RVA: 0x000080A4 File Offset: 0x000062A4
		public bool NavigateFromScopeEdges
		{
			get
			{
				return this.NavigationScope.NavigateFromScopeEdges;
			}
			set
			{
				if (value != this.NavigationScope.NavigateFromScopeEdges)
				{
					this.NavigationScope.NavigateFromScopeEdges = value;
				}
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000248 RID: 584 RVA: 0x000080C0 File Offset: 0x000062C0
		// (set) Token: 0x06000249 RID: 585 RVA: 0x000080CD File Offset: 0x000062CD
		public bool UseDiscoveryAreaAsScopeEdges
		{
			get
			{
				return this.NavigationScope.UseDiscoveryAreaAsScopeEdges;
			}
			set
			{
				if (value != this.NavigationScope.UseDiscoveryAreaAsScopeEdges)
				{
					this.NavigationScope.UseDiscoveryAreaAsScopeEdges = value;
				}
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x0600024A RID: 586 RVA: 0x000080E9 File Offset: 0x000062E9
		// (set) Token: 0x0600024B RID: 587 RVA: 0x000080F6 File Offset: 0x000062F6
		public bool DoNotAutoNavigateAfterSort
		{
			get
			{
				return this.NavigationScope.DoNotAutoNavigateAfterSort;
			}
			set
			{
				if (value != this.NavigationScope.DoNotAutoNavigateAfterSort)
				{
					this.NavigationScope.DoNotAutoNavigateAfterSort = value;
				}
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x0600024C RID: 588 RVA: 0x00008112 File Offset: 0x00006312
		// (set) Token: 0x0600024D RID: 589 RVA: 0x0000811F File Offset: 0x0000631F
		public bool FollowMobileTargets
		{
			get
			{
				return this.NavigationScope.FollowMobileTargets;
			}
			set
			{
				if (value != this.NavigationScope.FollowMobileTargets)
				{
					this.NavigationScope.FollowMobileTargets = value;
				}
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x0600024E RID: 590 RVA: 0x0000813B File Offset: 0x0000633B
		// (set) Token: 0x0600024F RID: 591 RVA: 0x00008148 File Offset: 0x00006348
		public bool DoNotAutoCollectChildScopes
		{
			get
			{
				return this.NavigationScope.DoNotAutoCollectChildScopes;
			}
			set
			{
				if (value != this.NavigationScope.DoNotAutoCollectChildScopes)
				{
					this.NavigationScope.DoNotAutoCollectChildScopes = value;
				}
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000250 RID: 592 RVA: 0x00008164 File Offset: 0x00006364
		// (set) Token: 0x06000251 RID: 593 RVA: 0x00008171 File Offset: 0x00006371
		public bool IsDefaultNavigationScope
		{
			get
			{
				return this.NavigationScope.IsDefaultNavigationScope;
			}
			set
			{
				if (value != this.NavigationScope.IsDefaultNavigationScope)
				{
					this.NavigationScope.IsDefaultNavigationScope = value;
				}
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000252 RID: 594 RVA: 0x0000818D File Offset: 0x0000638D
		// (set) Token: 0x06000253 RID: 595 RVA: 0x0000819A File Offset: 0x0000639A
		public float ExtendDiscoveryAreaTop
		{
			get
			{
				return this.NavigationScope.ExtendDiscoveryAreaTop;
			}
			set
			{
				if (value != this.NavigationScope.ExtendDiscoveryAreaTop)
				{
					this.NavigationScope.ExtendDiscoveryAreaTop = value;
				}
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000254 RID: 596 RVA: 0x000081B6 File Offset: 0x000063B6
		// (set) Token: 0x06000255 RID: 597 RVA: 0x000081C3 File Offset: 0x000063C3
		public float ExtendDiscoveryAreaRight
		{
			get
			{
				return this.NavigationScope.ExtendDiscoveryAreaRight;
			}
			set
			{
				if (value != this.NavigationScope.ExtendDiscoveryAreaRight)
				{
					this.NavigationScope.ExtendDiscoveryAreaRight = value;
				}
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000256 RID: 598 RVA: 0x000081DF File Offset: 0x000063DF
		// (set) Token: 0x06000257 RID: 599 RVA: 0x000081EC File Offset: 0x000063EC
		public float ExtendDiscoveryAreaBottom
		{
			get
			{
				return this.NavigationScope.ExtendDiscoveryAreaBottom;
			}
			set
			{
				if (value != this.NavigationScope.ExtendDiscoveryAreaBottom)
				{
					this.NavigationScope.ExtendDiscoveryAreaBottom = value;
				}
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000258 RID: 600 RVA: 0x00008208 File Offset: 0x00006408
		// (set) Token: 0x06000259 RID: 601 RVA: 0x00008215 File Offset: 0x00006415
		public float ExtendDiscoveryAreaLeft
		{
			get
			{
				return this.NavigationScope.ExtendDiscoveryAreaLeft;
			}
			set
			{
				if (value != this.NavigationScope.ExtendDiscoveryAreaLeft)
				{
					this.NavigationScope.ExtendDiscoveryAreaLeft = value;
				}
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x0600025A RID: 602 RVA: 0x00008231 File Offset: 0x00006431
		// (set) Token: 0x0600025B RID: 603 RVA: 0x0000823E File Offset: 0x0000643E
		public float ExtendChildrenCursorAreaLeft
		{
			get
			{
				return this.NavigationScope.ExtendChildrenCursorAreaLeft;
			}
			set
			{
				if (value != this.NavigationScope.ExtendChildrenCursorAreaLeft)
				{
					this.NavigationScope.ExtendChildrenCursorAreaLeft = value;
				}
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x0600025C RID: 604 RVA: 0x0000825A File Offset: 0x0000645A
		// (set) Token: 0x0600025D RID: 605 RVA: 0x00008267 File Offset: 0x00006467
		public float ExtendChildrenCursorAreaRight
		{
			get
			{
				return this.NavigationScope.ExtendChildrenCursorAreaRight;
			}
			set
			{
				if (value != this.NavigationScope.ExtendChildrenCursorAreaRight)
				{
					this.NavigationScope.ExtendChildrenCursorAreaRight = value;
				}
			}
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x0600025E RID: 606 RVA: 0x00008283 File Offset: 0x00006483
		// (set) Token: 0x0600025F RID: 607 RVA: 0x00008290 File Offset: 0x00006490
		public float ExtendChildrenCursorAreaTop
		{
			get
			{
				return this.NavigationScope.ExtendChildrenCursorAreaTop;
			}
			set
			{
				if (value != this.NavigationScope.ExtendChildrenCursorAreaTop)
				{
					this.NavigationScope.ExtendChildrenCursorAreaTop = value;
				}
			}
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000260 RID: 608 RVA: 0x000082AC File Offset: 0x000064AC
		// (set) Token: 0x06000261 RID: 609 RVA: 0x000082B9 File Offset: 0x000064B9
		public float ExtendChildrenCursorAreaBottom
		{
			get
			{
				return this.NavigationScope.ExtendChildrenCursorAreaBottom;
			}
			set
			{
				if (value != this.NavigationScope.ExtendChildrenCursorAreaBottom)
				{
					this.NavigationScope.ExtendChildrenCursorAreaBottom = value;
				}
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000262 RID: 610 RVA: 0x000082D5 File Offset: 0x000064D5
		// (set) Token: 0x06000263 RID: 611 RVA: 0x000082E2 File Offset: 0x000064E2
		public float DiscoveryAreaOffsetX
		{
			get
			{
				return this.NavigationScope.DiscoveryAreaOffsetX;
			}
			set
			{
				if (value != this.NavigationScope.DiscoveryAreaOffsetX)
				{
					this.NavigationScope.DiscoveryAreaOffsetX = value;
				}
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000264 RID: 612 RVA: 0x000082FE File Offset: 0x000064FE
		// (set) Token: 0x06000265 RID: 613 RVA: 0x0000830B File Offset: 0x0000650B
		public float DiscoveryAreaOffsetY
		{
			get
			{
				return this.NavigationScope.DiscoveryAreaOffsetY;
			}
			set
			{
				if (value != this.NavigationScope.DiscoveryAreaOffsetY)
				{
					this.NavigationScope.DiscoveryAreaOffsetY = value;
				}
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000266 RID: 614 RVA: 0x00008327 File Offset: 0x00006527
		// (set) Token: 0x06000267 RID: 615 RVA: 0x00008334 File Offset: 0x00006534
		public bool IsScopeEnabled
		{
			get
			{
				return this.NavigationScope.IsEnabled;
			}
			set
			{
				if (value != this.NavigationScope.IsEnabled)
				{
					this.NavigationScope.IsEnabled = value;
				}
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06000268 RID: 616 RVA: 0x00008350 File Offset: 0x00006550
		// (set) Token: 0x06000269 RID: 617 RVA: 0x0000835D File Offset: 0x0000655D
		public bool IsScopeDisabled
		{
			get
			{
				return this.NavigationScope.IsDisabled;
			}
			set
			{
				if (value != this.NavigationScope.IsDisabled)
				{
					this.NavigationScope.IsDisabled = value;
				}
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x0600026A RID: 618 RVA: 0x00008379 File Offset: 0x00006579
		// (set) Token: 0x0600026B RID: 619 RVA: 0x00008386 File Offset: 0x00006586
		public string UpNavigationScope
		{
			get
			{
				return this.NavigationScope.UpNavigationScopeID;
			}
			set
			{
				if (value != this.NavigationScope.UpNavigationScopeID)
				{
					this.NavigationScope.UpNavigationScopeID = value;
				}
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x0600026C RID: 620 RVA: 0x000083A7 File Offset: 0x000065A7
		// (set) Token: 0x0600026D RID: 621 RVA: 0x000083B4 File Offset: 0x000065B4
		public string RightNavigationScope
		{
			get
			{
				return this.NavigationScope.RightNavigationScopeID;
			}
			set
			{
				if (value != this.NavigationScope.RightNavigationScopeID)
				{
					this.NavigationScope.RightNavigationScopeID = value;
				}
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x0600026E RID: 622 RVA: 0x000083D5 File Offset: 0x000065D5
		// (set) Token: 0x0600026F RID: 623 RVA: 0x000083E2 File Offset: 0x000065E2
		public string DownNavigationScope
		{
			get
			{
				return this.NavigationScope.DownNavigationScopeID;
			}
			set
			{
				if (value != this.NavigationScope.DownNavigationScopeID)
				{
					this.NavigationScope.DownNavigationScopeID = value;
				}
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000270 RID: 624 RVA: 0x00008403 File Offset: 0x00006603
		// (set) Token: 0x06000271 RID: 625 RVA: 0x00008410 File Offset: 0x00006610
		public string LeftNavigationScope
		{
			get
			{
				return this.NavigationScope.LeftNavigationScopeID;
			}
			set
			{
				if (value != this.NavigationScope.LeftNavigationScopeID)
				{
					this.NavigationScope.LeftNavigationScopeID = value;
				}
			}
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000272 RID: 626 RVA: 0x00008431 File Offset: 0x00006631
		// (set) Token: 0x06000273 RID: 627 RVA: 0x00008439 File Offset: 0x00006639
		public NavigationScopeTargeter UpNavigationScopeTargeter
		{
			get
			{
				return this._upNavigationScopeTargeter;
			}
			set
			{
				if (value != this._upNavigationScopeTargeter)
				{
					this._upNavigationScopeTargeter = value;
					this.NavigationScope.UpNavigationScope = value.NavigationScope;
				}
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000274 RID: 628 RVA: 0x0000845C File Offset: 0x0000665C
		// (set) Token: 0x06000275 RID: 629 RVA: 0x00008464 File Offset: 0x00006664
		public NavigationScopeTargeter RightNavigationScopeTargeter
		{
			get
			{
				return this._rightNavigationScopeTargeter;
			}
			set
			{
				if (value != this._rightNavigationScopeTargeter)
				{
					this._rightNavigationScopeTargeter = value;
					this.NavigationScope.RightNavigationScope = value.NavigationScope;
				}
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000276 RID: 630 RVA: 0x00008487 File Offset: 0x00006687
		// (set) Token: 0x06000277 RID: 631 RVA: 0x0000848F File Offset: 0x0000668F
		public NavigationScopeTargeter DownNavigationScopeTargeter
		{
			get
			{
				return this._downNavigationScopeTargeter;
			}
			set
			{
				if (value != this._downNavigationScopeTargeter)
				{
					this._downNavigationScopeTargeter = value;
					this.NavigationScope.DownNavigationScope = value.NavigationScope;
				}
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000278 RID: 632 RVA: 0x000084B2 File Offset: 0x000066B2
		// (set) Token: 0x06000279 RID: 633 RVA: 0x000084BA File Offset: 0x000066BA
		public NavigationScopeTargeter LeftNavigationScopeTargeter
		{
			get
			{
				return this._leftNavigationScopeTargeter;
			}
			set
			{
				if (value != this._leftNavigationScopeTargeter)
				{
					this._leftNavigationScopeTargeter = value;
					this.NavigationScope.LeftNavigationScope = value.NavigationScope;
				}
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x0600027A RID: 634 RVA: 0x000084DD File Offset: 0x000066DD
		// (set) Token: 0x0600027B RID: 635 RVA: 0x000084EC File Offset: 0x000066EC
		public Widget ScopeParent
		{
			get
			{
				return this.NavigationScope.ParentWidget;
			}
			set
			{
				if (this.NavigationScope.ParentWidget != value)
				{
					if (this.NavigationScope.ParentWidget != null)
					{
						base.EventManager.RemoveNavigationScope(this.NavigationScope);
					}
					this.NavigationScope.ParentWidget = value;
					this.NavigationScope.ParentWidget.EventFire += this.OnParentConnectedToTheRoot;
					base.EventManager.AddNavigationScope(this.NavigationScope, false);
				}
			}
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000855F File Offset: 0x0000675F
		private void OnParentConnectedToTheRoot(Widget widget, string eventName, object[] arguments)
		{
			if (eventName == "ConnectedToRoot" && !base.EventManager.HasNavigationScope(this.NavigationScope))
			{
				base.EventManager.AddNavigationScope(this.NavigationScope, false);
			}
		}

		// Token: 0x04000100 RID: 256
		private NavigationScopeTargeter _upNavigationScopeTargeter;

		// Token: 0x04000101 RID: 257
		private NavigationScopeTargeter _rightNavigationScopeTargeter;

		// Token: 0x04000102 RID: 258
		private NavigationScopeTargeter _downNavigationScopeTargeter;

		// Token: 0x04000103 RID: 259
		private NavigationScopeTargeter _leftNavigationScopeTargeter;
	}
}
