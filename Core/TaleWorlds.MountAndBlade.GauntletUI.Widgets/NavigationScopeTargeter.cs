using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class NavigationScopeTargeter : Widget
	{
		public GamepadNavigationScope NavigationScope { get; private set; }

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

		private void OnParentConnectedToTheRoot(Widget widget, string eventName, object[] arguments)
		{
			if (eventName == "ConnectedToRoot" && !base.EventManager.HasNavigationScope(this.NavigationScope))
			{
				base.EventManager.AddNavigationScope(this.NavigationScope, false);
			}
		}

		private NavigationScopeTargeter _upNavigationScopeTargeter;

		private NavigationScopeTargeter _rightNavigationScopeTargeter;

		private NavigationScopeTargeter _downNavigationScopeTargeter;

		private NavigationScopeTargeter _leftNavigationScopeTargeter;
	}
}
