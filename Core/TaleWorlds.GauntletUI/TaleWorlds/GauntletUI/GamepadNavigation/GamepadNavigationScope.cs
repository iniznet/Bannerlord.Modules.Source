using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.GamepadNavigation
{
	public class GamepadNavigationScope
	{
		public string ScopeID { get; set; } = "DefaultScopeID";

		public bool IsActiveScope { get; private set; }

		public bool DoNotAutomaticallyFindChildren { get; set; }

		public GamepadNavigationTypes ScopeMovements { get; set; }

		public GamepadNavigationTypes AlternateScopeMovements { get; set; }

		public int AlternateMovementStepSize { get; set; }

		public bool HasCircularMovement { get; set; }

		public ReadOnlyCollection<Widget> NavigatableWidgets { get; }

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

		public int LatestNavigationElementIndex { get; set; }

		public bool DoNotAutoGainNavigationOnInit { get; set; }

		public bool ForceGainNavigationBasedOnDirection { get; set; }

		public bool ForceGainNavigationOnClosestChild { get; set; }

		public bool NavigateFromScopeEdges { get; set; }

		public bool UseDiscoveryAreaAsScopeEdges { get; set; }

		public bool DoNotAutoNavigateAfterSort { get; set; }

		public bool FollowMobileTargets { get; set; }

		public bool DoNotAutoCollectChildScopes { get; set; }

		public bool IsDefaultNavigationScope { get; set; }

		public float ExtendDiscoveryAreaRight { get; set; }

		public float ExtendDiscoveryAreaTop { get; set; }

		public float ExtendDiscoveryAreaBottom { get; set; }

		public float ExtendDiscoveryAreaLeft { get; set; }

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

		public float DiscoveryAreaOffsetX { get; set; }

		public float DiscoveryAreaOffsetY { get; set; }

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

		internal bool IsInitialized { get; private set; }

		internal GamepadNavigationScope PreviousScope { get; set; }

		internal Dictionary<GamepadNavigationTypes, string> ManualScopeIDs { get; private set; }

		internal Dictionary<GamepadNavigationTypes, GamepadNavigationScope> ManualScopes { get; private set; }

		internal bool IsAdditionalMovementsDirty { get; set; }

		internal Dictionary<GamepadNavigationTypes, GamepadNavigationScope> InterScopeMovements { get; private set; }

		internal GamepadNavigationScope ParentScope { get; private set; }

		internal ReadOnlyCollection<GamepadNavigationScope> ChildScopes { get; private set; }

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

		internal void SetIsActiveScope(bool isActive)
		{
			this.IsActiveScope = isActive;
		}

		internal bool IsVisible()
		{
			return this._invisibleParents.Count == 0;
		}

		internal bool IsAvailable()
		{
			return this.IsEnabled && this._navigatableWidgets.Count > 0 && this.IsVisible();
		}

		internal void Initialize()
		{
			if (!this.DoNotAutomaticallyFindChildren)
			{
				this.FindNavigatableChildren();
			}
			this.IsInitialized = true;
		}

		internal void RefreshNavigatableChildren()
		{
			if (this.IsInitialized)
			{
				this.FindNavigatableChildren();
			}
		}

		internal bool HasMovement(GamepadNavigationTypes movement)
		{
			return (this.ScopeMovements & movement) != GamepadNavigationTypes.None || (this.AlternateScopeMovements & movement) > GamepadNavigationTypes.None;
		}

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

		internal bool GetShouldFindScopeByPosition(GamepadNavigationTypes movement)
		{
			return this.ManualScopeIDs[movement] == null && this.ManualScopes[movement] == null;
		}

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

		internal int FindIndexOfWidget(Widget widget)
		{
			int num;
			if (widget != null && this._navigatableWidgets.Count != 0 && this._widgetIndices.TryGetValue(widget, out num))
			{
				return num;
			}
			return -1;
		}

		internal void SortWidgets()
		{
			this._navigatableWidgets.Sort(this._navigatableWidgetComparer);
			this._widgetIndices.Clear();
			for (int i = 0; i < this._navigatableWidgets.Count; i++)
			{
				this._widgetIndices.Add(this._navigatableWidgets[i], i);
			}
		}

		public void ClearNavigatableWidgets()
		{
			this._navigatableWidgets.Clear();
			this._widgetIndices.Clear();
		}

		internal Rectangle GetDiscoveryRectangle()
		{
			float customScale = this.ParentWidget.EventManager.Context.CustomScale;
			return new Rectangle(this.DiscoveryAreaOffsetX + this.ParentWidget.GlobalPosition.X - this.ExtendDiscoveryAreaLeft * customScale, this.DiscoveryAreaOffsetY + this.ParentWidget.GlobalPosition.Y - this.ExtendDiscoveryAreaTop * customScale, this.ParentWidget.Size.X + (this.ExtendDiscoveryAreaLeft + this.ExtendDiscoveryAreaRight) * customScale, this.ParentWidget.Size.Y + (this.ExtendDiscoveryAreaTop + this.ExtendDiscoveryAreaBottom) * customScale);
		}

		internal Rectangle GetRectangle()
		{
			if (this.ParentWidget == null)
			{
				return new Rectangle(0f, 0f, 1f, 1f);
			}
			return new Rectangle(this.ParentWidget.GlobalPosition.X, this.ParentWidget.GlobalPosition.Y, this.ParentWidget.Size.X, this.ParentWidget.Size.Y);
		}

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

		internal Widget GetApproximatelyClosestWidgetToPosition(Vector2 position, GamepadNavigationTypes movement = GamepadNavigationTypes.None, bool angleCheck = false)
		{
			float num;
			return this.GetApproximatelyClosestWidgetToPosition(position, out num, movement, angleCheck);
		}

		internal Widget GetApproximatelyClosestWidgetToPosition(Vector2 position, out float distance, GamepadNavigationTypes movement = GamepadNavigationTypes.None, bool angleCheck = false)
		{
			int approximatelyClosestWidgetIndexToPosition = this.GetApproximatelyClosestWidgetIndexToPosition(position, out distance, movement, angleCheck);
			if (approximatelyClosestWidgetIndexToPosition != -1)
			{
				return this._navigatableWidgets[approximatelyClosestWidgetIndexToPosition];
			}
			return null;
		}

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

		private static float InverseLerp(float fromValue, float toValue, float value)
		{
			if (fromValue == toValue)
			{
				return 0f;
			}
			return (value - fromValue) / (toValue - fromValue);
		}

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

		private List<Widget> _navigatableWidgets;

		private Dictionary<Widget, int> _widgetIndices;

		private Widget _parentWidget;

		private float _extendChildrenCursorAreaLeft;

		private float _extendChildrenCursorAreaRight;

		private float _extendChildrenCursorAreaTop;

		private float _extendChildrenCursorAreaBottom;

		private bool _isEnabled;

		private bool _isDisabled;

		private GamepadNavigationScope.WidgetNavigationIndexComparer _navigatableWidgetComparer;

		private List<Widget> _invisibleParents;

		private List<GamepadNavigationScope> _childScopes;

		internal Action<GamepadNavigationScope> OnNavigatableWidgetsChanged;

		internal Action<GamepadNavigationScope, bool> OnVisibilityChanged;

		internal Action<GamepadNavigationScope, GamepadNavigationScope> OnParentScopeChanged;

		private class WidgetNavigationIndexComparer : IComparer<Widget>
		{
			public int Compare(Widget x, Widget y)
			{
				return x.GamepadNavigationIndex.CompareTo(y.GamepadNavigationIndex);
			}
		}
	}
}
