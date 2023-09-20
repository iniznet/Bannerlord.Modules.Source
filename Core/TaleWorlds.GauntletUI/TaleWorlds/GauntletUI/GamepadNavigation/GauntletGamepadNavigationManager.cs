using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.GamepadNavigation
{
	public class GauntletGamepadNavigationManager
	{
		public static GauntletGamepadNavigationManager Instance { get; private set; }

		private EventManager LatestEventManager
		{
			get
			{
				if (this._latestCachedEventManager == null)
				{
					for (int i = 0; i < this._sortedEventManagers.Count; i++)
					{
						if (this._sortedEventManagers[i].IsAvailableForNavigation())
						{
							this._latestCachedEventManager = this._sortedEventManagers[i];
							break;
						}
					}
				}
				return this._latestCachedEventManager;
			}
		}

		public bool IsFollowingMobileTarget { get; private set; }

		public bool IsHoldingDpadKeysForNavigation { get; private set; }

		public bool IsCursorMovingForNavigation { get; private set; }

		public bool IsInWrapMovement { get; private set; }

		private Vector2 MousePosition
		{
			get
			{
				return (Vector2)Input.InputState.MousePositionPixel;
			}
			set
			{
				Input.SetMousePosition((int)value.X, (int)value.Y);
			}
		}

		private bool IsControllerActive
		{
			get
			{
				return Input.IsGamepadActive;
			}
		}

		internal ReadOnlyDictionary<EventManager, GamepadNavigationScopeCollection> NavigationScopes { get; private set; }

		internal ReadOnlyDictionary<Widget, List<GamepadNavigationScope>> NavigationScopeParents { get; private set; }

		internal ReadOnlyDictionary<Widget, List<GamepadNavigationForcedScopeCollection>> ForcedNavigationScopeParents { get; private set; }

		public Widget LastTargetedWidget
		{
			get
			{
				GamepadNavigationScope activeNavigationScope = this._activeNavigationScope;
				Widget widget = ((activeNavigationScope != null) ? activeNavigationScope.LastNavigatedWidget : null);
				if (widget != null && (this.IsCursorMovingForNavigation || widget.IsPointInsideGamepadCursorArea(this.MousePosition)))
				{
					return widget;
				}
				return null;
			}
		}

		public bool TargetedWidgetHasAction
		{
			get
			{
				if (this.LastTargetedWidget != null)
				{
					if (this.LastTargetedWidget.UsedNavigationMovements == GamepadNavigationTypes.None)
					{
						if (!this.LastTargetedWidget.AllChildren.Any((Widget c) => c.UsedNavigationMovements > GamepadNavigationTypes.None))
						{
							return this.LastTargetedWidget.Parents.Any((Widget p) => p.UsedNavigationMovements > GamepadNavigationTypes.None);
						}
					}
					return true;
				}
				return false;
			}
		}

		public bool AnyWidgetUsingNavigation
		{
			get
			{
				return this._navigationBlockingWidgets.Any((Widget x) => x.IsUsingNavigation);
			}
		}

		private GauntletGamepadNavigationManager()
		{
			this._cachedEventManagerComparer = new GauntletGamepadNavigationManager.EventManagerComparer();
			this._cachedForcedScopeComparer = new GauntletGamepadNavigationManager.ForcedScopeComparer();
			this._navigationScopes = new Dictionary<EventManager, GamepadNavigationScopeCollection>();
			this.NavigationScopes = new ReadOnlyDictionary<EventManager, GamepadNavigationScopeCollection>(this._navigationScopes);
			this._navigationScopeParents = new Dictionary<Widget, List<GamepadNavigationScope>>();
			this._forcedNavigationScopeCollectionParents = new Dictionary<Widget, List<GamepadNavigationForcedScopeCollection>>();
			this.NavigationScopeParents = new ReadOnlyDictionary<Widget, List<GamepadNavigationScope>>(this._navigationScopeParents);
			this.ForcedNavigationScopeParents = new ReadOnlyDictionary<Widget, List<GamepadNavigationForcedScopeCollection>>(this._forcedNavigationScopeCollectionParents);
			this._sortedEventManagers = new List<EventManager>();
			this._availableScopesThisFrame = new List<GamepadNavigationScope>();
			this._unsortedScopes = new List<GamepadNavigationScope>();
			this._forcedScopeCollections = new List<GamepadNavigationForcedScopeCollection>();
			this._layerNavigationScopes = new Dictionary<string, List<GamepadNavigationScope>>();
			this._navigationScopesById = new Dictionary<string, List<GamepadNavigationScope>>();
			this._navigationGainControllers = new Dictionary<EventManager, GauntletGamepadNavigationManager.EventManagerGamepadNavigationGainHandler>();
			this._navigationBlockingWidgets = new List<Widget>();
			this._isAvailableScopesDirty = false;
			this._isForcedCollectionsDirty = false;
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Combine(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
		}

		private void OnGamepadActiveStateChanged()
		{
			if (this.IsControllerActive && Input.MouseMoveX == 0f && Input.MouseMoveY == 0f)
			{
				this._isAvailableScopesDirty = true;
				this._isForcedCollectionsDirty = true;
			}
		}

		public static void Initialize(bool isInWindowedMode)
		{
			if (GauntletGamepadNavigationManager.Instance != null)
			{
				Debug.FailedAssert("Gamepad Navigation already initialized", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\GamepadNavigation\\GauntletGamepadNavigationManager.cs", "Initialize", 206);
				return;
			}
			GauntletGamepadNavigationManager.Instance = new GauntletGamepadNavigationManager();
		}

		public bool TryNavigateTo(Widget widget)
		{
			GamepadNavigationScopeCollection gamepadNavigationScopeCollection;
			if (widget != null && widget.GamepadNavigationIndex != -1 && this._navigationScopes.TryGetValue(widget.EventManager, out gamepadNavigationScopeCollection))
			{
				for (int i = 0; i < gamepadNavigationScopeCollection.VisibleScopes.Count; i++)
				{
					GamepadNavigationScope gamepadNavigationScope = gamepadNavigationScopeCollection.VisibleScopes[i];
					if (gamepadNavigationScope.IsAvailable() && (gamepadNavigationScope.ParentWidget == widget || gamepadNavigationScope.ParentWidget.CheckIsMyChildRecursive(widget)))
					{
						return this.SetCurrentNavigatedWidget(gamepadNavigationScope, widget);
					}
				}
			}
			return false;
		}

		public bool TryNavigateTo(GamepadNavigationScope scope)
		{
			if (scope != null && scope.IsAvailable())
			{
				Widget approximatelyClosestWidgetToPosition = scope.GetApproximatelyClosestWidgetToPosition(this.MousePosition, GamepadNavigationTypes.None, false);
				if (approximatelyClosestWidgetToPosition != null)
				{
					return this.SetCurrentNavigatedWidget(scope, approximatelyClosestWidgetToPosition);
				}
			}
			return false;
		}

		public void OnFinalize()
		{
			foreach (KeyValuePair<EventManager, GamepadNavigationScopeCollection> keyValuePair in this._navigationScopes)
			{
				keyValuePair.Value.OnFinalize();
			}
			this._navigationScopes.Clear();
			this._navigationScopeParents.Clear();
			GauntletGamepadNavigationManager.Instance = null;
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
		}

		public void Update(float dt)
		{
			this._time += dt;
			if (this._stopCursorNextFrame)
			{
				this.IsCursorMovingForNavigation = false;
				this._stopCursorNextFrame = false;
			}
			if (this.IsControllerActive && Input.MouseMoveX <= 0f && Input.MouseMoveY <= 0f)
			{
				GamepadNavigationScope activeNavigationScope = this._activeNavigationScope;
				if (activeNavigationScope != null && activeNavigationScope.IsAvailable() && this._activeNavigationScope.ParentWidget.EventManager.IsAvailableForNavigation() && !Input.IsAnyTouchActive)
				{
					goto IL_7F;
				}
			}
			this.OnDpadNavigationStopped();
			IL_7F:
			foreach (KeyValuePair<EventManager, GauntletGamepadNavigationManager.EventManagerGamepadNavigationGainHandler> keyValuePair in this._navigationGainControllers)
			{
				keyValuePair.Value.Tick(dt);
			}
			if (this.LastTargetedWidget != null)
			{
				Vector2.Distance(this.LastTargetedWidget.GlobalPosition + this.LastTargetedWidget.Size / 2f, this.MousePosition);
			}
			if (Input.GetKeyState(InputKey.ControllerRStick).X == 0f)
			{
				bool flag = Input.GetKeyState(InputKey.ControllerRStick).Y != 0f;
			}
			if (this._autoRefreshTimer > -1f)
			{
				this._autoRefreshTimer += dt;
				if (this._autoRefreshTimer > 0.6f)
				{
					this._autoRefreshTimer = -1f;
					this._isAvailableScopesDirty = true;
				}
			}
			if (!this._isAvailableScopesDirty)
			{
				GamepadNavigationScope activeNavigationScope2 = this._activeNavigationScope;
				if (activeNavigationScope2 == null || !activeNavigationScope2.IsAvailable())
				{
					this._isAvailableScopesDirty = true;
				}
			}
			this._sortedEventManagers.Clear();
			foreach (KeyValuePair<EventManager, GamepadNavigationScopeCollection> keyValuePair2 in this._navigationScopes)
			{
				this._sortedEventManagers.Add(keyValuePair2.Key);
				keyValuePair2.Value.HandleScopeVisibilities();
			}
			this._sortedEventManagers.Sort(this._cachedEventManagerComparer);
			foreach (KeyValuePair<EventManager, GamepadNavigationScopeCollection> keyValuePair3 in this._navigationScopes)
			{
				if (keyValuePair3.Value.UninitializedScopes.Count > 0)
				{
					List<GamepadNavigationScope> list = keyValuePair3.Value.UninitializedScopes.ToList<GamepadNavigationScope>();
					for (int i = 0; i < list.Count; i++)
					{
						this.InitializeScope(keyValuePair3.Key, list[i]);
					}
				}
			}
			if (this._unsortedScopes.Count > 0)
			{
				bool flag2 = false;
				for (int j = 0; j < this._unsortedScopes.Count; j++)
				{
					if (this._unsortedScopes[j] == this._activeNavigationScope)
					{
						flag2 = true;
					}
					this._unsortedScopes[j].SortWidgets();
				}
				this._unsortedScopes.Clear();
				if (flag2 && !this._activeNavigationScope.DoNotAutoNavigateAfterSort && this._activeNavigationScope != null && this._activeNavigationScope.IsAvailable() && (this._wasCursorInsideActiveScopeLastFrame || this._activeNavigationScope.GetRectangle().IsPointInside(this.MousePosition)))
				{
					if (this._activeNavigationScope.ForceGainNavigationOnClosestChild)
					{
						this.MoveCursorToClosestAvailableWidgetInScope(this._activeNavigationScope);
					}
					else
					{
						this.MoveCursorToFirstAvailableWidgetInScope(this._activeNavigationScope);
					}
				}
			}
			if (this._activeForcedScopeCollection != null && !this._activeForcedScopeCollection.IsAvailable())
			{
				this._isAvailableScopesDirty = true;
				this._isForcedCollectionsDirty = true;
			}
			if (this._shouldUpdateAvailableScopes)
			{
				GamepadNavigationForcedScopeCollection activeForcedScopeCollection = this._activeForcedScopeCollection;
				this._activeForcedScopeCollection = this.FindAvailableForcedScope();
				if (this._activeForcedScopeCollection != null && activeForcedScopeCollection == null)
				{
					this._activeForcedScopeCollection.PreviousScope = this._activeNavigationScope;
				}
				this.RefreshAvailableScopes();
				this._shouldUpdateAvailableScopes = false;
				if (activeForcedScopeCollection != null && !activeForcedScopeCollection.IsAvailable())
				{
					this.TryMoveCursorToPreviousScope(activeForcedScopeCollection);
				}
				else if (this._nextScopeToGainNavigation != null)
				{
					this.MoveCursorToFirstAvailableWidgetInScope(this._nextScopeToGainNavigation);
					this._nextScopeToGainNavigation = null;
				}
				else
				{
					GamepadNavigationScope activeNavigationScope3 = this._activeNavigationScope;
					if (activeNavigationScope3 == null || !activeNavigationScope3.IsAvailable() || !this._availableScopesThisFrame.Contains(this._activeNavigationScope))
					{
						this.MoveCursorToBestAvailableScope(false, GamepadNavigationTypes.None);
					}
				}
			}
			if (this._isAvailableScopesDirty)
			{
				this._shouldUpdateAvailableScopes = true;
				this._isAvailableScopesDirty = false;
			}
			this.HandleInput(dt);
			this.HandleCursorMovement();
			GamepadNavigationScope activeNavigationScope4 = this._activeNavigationScope;
			this._wasCursorInsideActiveScopeLastFrame = activeNavigationScope4 != null && activeNavigationScope4.GetRectangle().IsPointInside(this.MousePosition);
		}

		internal void OnMovieLoaded(EventManager source, string movieName)
		{
			GamepadNavigationScopeCollection gamepadNavigationScopeCollection;
			if (this._navigationScopes.TryGetValue(source, out gamepadNavigationScopeCollection))
			{
				List<GamepadNavigationScope> list = gamepadNavigationScopeCollection.UninitializedScopes.ToList<GamepadNavigationScope>();
				for (int i = 0; i < list.Count; i++)
				{
					if (!list[i].DoNotAutomaticallyFindChildren)
					{
						this.InitializeScope(source, list[i]);
					}
					this.AddItemToDictionaryList<string, GamepadNavigationScope>(this._layerNavigationScopes, movieName, list[i]);
				}
			}
			this._autoRefreshTimer = 0f;
			this._isAvailableScopesDirty = true;
			this._latestCachedEventManager = null;
		}

		internal void OnMovieReleased(EventManager source, string movieName)
		{
			List<GamepadNavigationScope> list;
			if (this._layerNavigationScopes.TryGetValue(movieName, out list))
			{
				List<GamepadNavigationScope> list2 = list.ToList<GamepadNavigationScope>();
				for (int i = 0; i < list2.Count; i++)
				{
					this.RemoveNavigationScope(source, list2[i]);
					this.RemoveItemFromDictionaryList<string, GamepadNavigationScope>(this._layerNavigationScopes, movieName, list2[i]);
				}
				this._latestCachedEventManager = null;
			}
			this._autoRefreshTimer = 0f;
			this._isAvailableScopesDirty = true;
		}

		internal void OnEventManagerAdded(EventManager source)
		{
			this._navigationScopes.Add(source, new GamepadNavigationScopeCollection(source, new Action<GamepadNavigationScope>(this.OnScopeNavigatableWidgetsChanged), new Action<GamepadNavigationScope, bool>(this.OnScopeVisibilityChanged)));
			this._navigationGainControllers.Add(source, new GauntletGamepadNavigationManager.EventManagerGamepadNavigationGainHandler(source));
			this._latestCachedEventManager = null;
		}

		private void OnEventManagerRemoved(EventManager source)
		{
			GamepadNavigationScopeCollection gamepadNavigationScopeCollection;
			if (this._navigationScopes.TryGetValue(source, out gamepadNavigationScopeCollection))
			{
				gamepadNavigationScopeCollection.OnFinalize();
				this._navigationScopes.Remove(source);
			}
			GauntletGamepadNavigationManager.EventManagerGamepadNavigationGainHandler eventManagerGamepadNavigationGainHandler;
			if (this._navigationGainControllers.TryGetValue(source, out eventManagerGamepadNavigationGainHandler))
			{
				eventManagerGamepadNavigationGainHandler.Clear();
				this._navigationGainControllers.Remove(source);
			}
			this._sortedEventManagers.Remove(source);
			this._latestCachedEventManager = null;
		}

		internal void OnEventManagerFinalized(EventManager source)
		{
			int count = this._sortedEventManagers.Count;
			this.OnEventManagerRemoved(source);
			if (count != this._sortedEventManagers.Count)
			{
				this._sortedEventManagers = this._navigationScopes.Keys.ToList<EventManager>();
				this._sortedEventManagers.Sort(this._cachedEventManagerComparer);
			}
			this._isAvailableScopesDirty = true;
		}

		private Vector2 GetTargetCursorPosition()
		{
			if (this._latestGamepadNavigationWidget != null)
			{
				Vector2 globalPosition = this._latestGamepadNavigationWidget.GlobalPosition;
				Vector2 size = this._latestGamepadNavigationWidget.Size;
				globalPosition.X -= this._latestGamepadNavigationWidget.ExtendCursorAreaLeft;
				globalPosition.Y -= this._latestGamepadNavigationWidget.ExtendCursorAreaTop;
				size.X += this._latestGamepadNavigationWidget.ExtendCursorAreaLeft + this._latestGamepadNavigationWidget.ExtendCursorAreaRight;
				size.Y += this._latestGamepadNavigationWidget.ExtendCursorAreaTop + this._latestGamepadNavigationWidget.ExtendCursorAreaBottom;
				return globalPosition + size / 2f;
			}
			return (Vector2)Vec2.Invalid;
		}

		private void RefreshAvailableScopes()
		{
			this._availableScopesThisFrame.Clear();
			if (this._activeForcedScopeCollection != null)
			{
				for (int i = 0; i < this._activeForcedScopeCollection.Scopes.Count; i++)
				{
					this._availableScopesThisFrame.Add(this._activeForcedScopeCollection.Scopes[i]);
				}
				return;
			}
			for (int j = 0; j < this._sortedEventManagers.Count; j++)
			{
				EventManager eventManager = this._sortedEventManagers[j];
				if (eventManager.IsAvailableForNavigation())
				{
					for (int k = 0; k < this._navigationScopes[eventManager].VisibleScopes.Count; k++)
					{
						GamepadNavigationScope gamepadNavigationScope = this._navigationScopes[eventManager].VisibleScopes[k];
						if (gamepadNavigationScope.IsAvailable())
						{
							Widget parentWidget = gamepadNavigationScope.ParentWidget;
							Vector2 vector = parentWidget.GlobalPosition + parentWidget.Size / 2f;
							if (!eventManager.GetIsBlockedAtPosition(vector))
							{
								this._availableScopesThisFrame.Add(gamepadNavigationScope);
							}
						}
					}
				}
			}
		}

		internal void OnWidgetUsedNavigationMovementsUpdated(Widget widget)
		{
			if (widget.UsedNavigationMovements != GamepadNavigationTypes.None && !this._navigationBlockingWidgets.Contains(widget))
			{
				this._navigationBlockingWidgets.Add(widget);
				return;
			}
			if (widget.UsedNavigationMovements == GamepadNavigationTypes.None && this._navigationBlockingWidgets.Contains(widget))
			{
				this._navigationBlockingWidgets.Remove(widget);
			}
		}

		internal void AddForcedScopeCollection(GamepadNavigationForcedScopeCollection forcedCollection)
		{
			if (!this._forcedScopeCollections.Contains(forcedCollection))
			{
				this._forcedScopeCollections.Add(forcedCollection);
				this.AddItemToDictionaryList<Widget, GamepadNavigationForcedScopeCollection>(this._forcedNavigationScopeCollectionParents, forcedCollection.ParentWidget, forcedCollection);
				this.CollectScopesForForcedCollection(forcedCollection);
				forcedCollection.OnAvailabilityChanged = new Action<GamepadNavigationForcedScopeCollection>(this.OnForcedScopeCollectionAvailabilityStateChanged);
				this._isForcedCollectionsDirty = true;
			}
			else
			{
				Debug.FailedAssert("Trying to add a navigation scope collection more than once", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\GamepadNavigation\\GauntletGamepadNavigationManager.cs", "AddForcedScopeCollection", 598);
			}
			this._isAvailableScopesDirty = true;
		}

		internal void RemoveForcedScopeCollection(GamepadNavigationForcedScopeCollection collection)
		{
			if (this._forcedScopeCollections.Contains(collection))
			{
				collection.ClearScopes();
				this._forcedScopeCollections.Remove(collection);
				if (collection.ParentWidget != null && this._forcedNavigationScopeCollectionParents.ContainsKey(collection.ParentWidget))
				{
					this.RemoveItemFromDictionaryList<Widget, GamepadNavigationForcedScopeCollection>(this._forcedNavigationScopeCollectionParents, collection.ParentWidget, collection);
				}
			}
			collection.OnAvailabilityChanged = null;
			collection.ParentWidget = null;
			this._isForcedCollectionsDirty = true;
			this._isAvailableScopesDirty = true;
		}

		internal void AddNavigationScope(EventManager source, GamepadNavigationScope scope, bool initializeScope = false)
		{
			GamepadNavigationScopeCollection gamepadNavigationScopeCollection;
			if (this._navigationScopes.TryGetValue(source, out gamepadNavigationScopeCollection))
			{
				gamepadNavigationScopeCollection.AddScope(scope);
			}
			else
			{
				this.OnEventManagerAdded(source);
				this._navigationScopes[source].AddScope(scope);
			}
			this.AddItemToDictionaryList<Widget, GamepadNavigationScope>(this._navigationScopeParents, scope.ParentWidget, scope);
			if (initializeScope)
			{
				this.InitializeScope(source, scope);
			}
			this._isAvailableScopesDirty = true;
		}

		internal void RemoveNavigationScope(EventManager source, GamepadNavigationScope scope)
		{
			if (scope == null)
			{
				Debug.FailedAssert("Trying to remove null navigation data", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\GamepadNavigation\\GauntletGamepadNavigationManager.cs", "RemoveNavigationScope", 655);
				return;
			}
			GamepadNavigationScopeCollection gamepadNavigationScopeCollection;
			if (this._navigationScopes.TryGetValue(source, out gamepadNavigationScopeCollection))
			{
				gamepadNavigationScopeCollection.RemoveScope(scope);
				scope.ClearNavigatableWidgets();
				if (gamepadNavigationScopeCollection.GetTotalNumberOfScopes() == 0)
				{
					this.OnEventManagerRemoved(source);
				}
			}
			else
			{
				foreach (KeyValuePair<EventManager, GamepadNavigationScopeCollection> keyValuePair in this._navigationScopes.Where((KeyValuePair<EventManager, GamepadNavigationScopeCollection> x) => x.Value.AllScopes.Contains(scope)))
				{
					keyValuePair.Value.RemoveScope(scope);
					scope.ClearNavigatableWidgets();
					if (keyValuePair.Value.GetTotalNumberOfScopes() == 0)
					{
						this.OnEventManagerRemoved(source);
					}
				}
			}
			for (int i = 0; i < this._forcedScopeCollections.Count; i++)
			{
				if (this._forcedScopeCollections[i].Scopes.Contains(scope))
				{
					this._forcedScopeCollections[i].RemoveScope(scope);
				}
			}
			if (scope.ParentWidget != null)
			{
				this._navigationScopeParents.Remove(scope.ParentWidget);
			}
			List<GamepadNavigationScope> list;
			if (this._navigationScopesById.TryGetValue(scope.ScopeID, out list) && list.Contains(scope))
			{
				this.RemoveItemFromDictionaryList<string, GamepadNavigationScope>(this._navigationScopesById, scope.ScopeID, scope);
			}
			if (this._navigationScopes.Any((KeyValuePair<EventManager, GamepadNavigationScopeCollection> x) => x.Value.HasScopeInAnyList(scope)))
			{
				Debug.FailedAssert("Failed to remove scope completely", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\GamepadNavigation\\GauntletGamepadNavigationManager.cs", "RemoveNavigationScope", 711);
				foreach (KeyValuePair<EventManager, GamepadNavigationScopeCollection> keyValuePair2 in this._navigationScopes)
				{
					keyValuePair2.Value.RemoveScope(scope);
					if (keyValuePair2.Value.GetTotalNumberOfScopes() == 0)
					{
						keyValuePair2.Value.OnFinalize();
					}
				}
			}
			scope.ParentWidget = null;
			if (this._activeNavigationScope == scope)
			{
				this._activeNavigationScope = null;
			}
			this._latestCachedEventManager = null;
			for (int j = 0; j < this._availableScopesThisFrame.Count; j++)
			{
				this._availableScopesThisFrame[j].IsAdditionalMovementsDirty = true;
			}
			this._isAvailableScopesDirty = true;
		}

		internal void OnWidgetNavigationStatusChanged(EventManager source, Widget widget)
		{
			GamepadNavigationScopeCollection gamepadNavigationScopeCollection;
			if (this._navigationScopes.TryGetValue(source, out gamepadNavigationScopeCollection))
			{
				for (int i = 0; i < gamepadNavigationScopeCollection.AllScopes.Count; i++)
				{
					GamepadNavigationScope gamepadNavigationScope = gamepadNavigationScopeCollection.AllScopes[i];
					if (gamepadNavigationScope.ParentWidget.CheckIsMyChildRecursive(widget) || widget.CheckIsMyChildRecursive(gamepadNavigationScope.ParentWidget))
					{
						gamepadNavigationScope.RefreshNavigatableChildren();
					}
				}
			}
		}

		internal void OnWidgetNavigationIndexUpdated(EventManager source, Widget widget)
		{
			if (widget != null)
			{
				GamepadNavigationScope gamepadNavigationScope = this.FindClosestParentScopeOfWidget(widget);
				if (gamepadNavigationScope != null && !gamepadNavigationScope.DoNotAutomaticallyFindChildren)
				{
					gamepadNavigationScope.RemoveWidget(widget);
					if (widget.GamepadNavigationIndex != -1)
					{
						gamepadNavigationScope.AddWidget(widget);
					}
				}
			}
		}

		internal bool HasNavigationScope(EventManager source, GamepadNavigationScope scope)
		{
			GamepadNavigationScopeCollection gamepadNavigationScopeCollection;
			return this._navigationScopes.TryGetValue(source, out gamepadNavigationScopeCollection) && (gamepadNavigationScopeCollection.VisibleScopes.Contains(scope) || gamepadNavigationScopeCollection.UninitializedScopes.Contains(scope) || gamepadNavigationScopeCollection.InvisibleScopes.Contains(scope));
		}

		internal bool HasNavigationScope(EventManager source, Func<GamepadNavigationScope, bool> predicate)
		{
			GamepadNavigationScopeCollection gamepadNavigationScopeCollection;
			return this._navigationScopes.TryGetValue(source, out gamepadNavigationScopeCollection) && (gamepadNavigationScopeCollection.VisibleScopes.Any((GamepadNavigationScope x) => predicate(x)) || gamepadNavigationScopeCollection.InvisibleScopes.Any((GamepadNavigationScope x) => predicate(x)));
		}

		private void OnActiveScopeParentChanged(GamepadNavigationScope oldParent, GamepadNavigationScope newParent)
		{
			if (oldParent != null && newParent == null && oldParent.LatestNavigationElementIndex != -1 && oldParent.IsAvailable())
			{
				this._isAvailableScopesDirty = true;
			}
		}

		private void OnScopeVisibilityChanged(GamepadNavigationScope scope, bool isVisible)
		{
			this._isAvailableScopesDirty = true;
		}

		private void OnForcedScopeCollectionAvailabilityStateChanged(GamepadNavigationForcedScopeCollection scopeCollection)
		{
			this._isAvailableScopesDirty = true;
			this._isForcedCollectionsDirty = true;
		}

		private void OnScopeNavigatableWidgetsChanged(GamepadNavigationScope scope)
		{
			if (!this._unsortedScopes.Contains(scope))
			{
				this._unsortedScopes.Add(scope);
			}
			if (scope.IsInitialized)
			{
				this._isAvailableScopesDirty = true;
			}
		}

		private void CollectScopesForForcedCollection(GamepadNavigationForcedScopeCollection collection)
		{
			collection.ClearScopes();
			GamepadNavigationScopeCollection gamepadNavigationScopeCollection;
			if (this._navigationScopes.TryGetValue(collection.ParentWidget.EventManager, out gamepadNavigationScopeCollection))
			{
				for (int i = 0; i < gamepadNavigationScopeCollection.AllScopes.Count; i++)
				{
					GamepadNavigationScope gamepadNavigationScope = gamepadNavigationScopeCollection.AllScopes[i];
					if (collection.ParentWidget == gamepadNavigationScope.ParentWidget || collection.ParentWidget.CheckIsMyChildRecursive(gamepadNavigationScope.ParentWidget))
					{
						collection.AddScope(gamepadNavigationScope);
					}
				}
			}
		}

		private void InitializeScope(EventManager source, GamepadNavigationScope scope)
		{
			GamepadNavigationScopeCollection gamepadNavigationScopeCollection;
			if (this._navigationScopes.TryGetValue(source, out gamepadNavigationScopeCollection))
			{
				gamepadNavigationScopeCollection.OnNavigationScopeInitialized(scope);
			}
			scope.Initialize();
			for (int i = this._forcedScopeCollections.Count - 1; i >= 0; i--)
			{
				if (this._forcedScopeCollections[i].ParentWidget == scope.ParentWidget || this._forcedScopeCollections[i].ParentWidget.CheckIsMyChildRecursive(scope.ParentWidget))
				{
					this._forcedScopeCollections[i].AddScope(scope);
					break;
				}
			}
			for (int j = 0; j < this._availableScopesThisFrame.Count; j++)
			{
				this._availableScopesThisFrame[j].IsAdditionalMovementsDirty = true;
			}
			if (!string.IsNullOrEmpty(scope.ScopeID))
			{
				this.AddItemToDictionaryList<string, GamepadNavigationScope>(this._navigationScopesById, scope.ScopeID, scope);
			}
			if (scope.ParentScope == null)
			{
				foreach (Widget widget in scope.ParentWidget.Parents)
				{
					List<GamepadNavigationScope> list;
					if (GauntletGamepadNavigationManager.Instance.NavigationScopeParents.TryGetValue(widget, out list))
					{
						if (list.Count > 0)
						{
							scope.SetParentScope(list[0]);
							break;
						}
						break;
					}
				}
			}
			this._isAvailableScopesDirty = true;
		}

		private void AddItemToDictionaryList<TKey, TValue>(Dictionary<TKey, List<TValue>> sourceDict, TKey key, TValue item)
		{
			List<TValue> list;
			if (!sourceDict.TryGetValue(key, out list))
			{
				sourceDict.Add(key, new List<TValue> { item });
				return;
			}
			if (!list.Contains(item))
			{
				list.Add(item);
				return;
			}
			Debug.FailedAssert("Trying to add same item to source dictionary twice", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\GamepadNavigation\\GauntletGamepadNavigationManager.cs", "AddItemToDictionaryList", 900);
		}

		private void RemoveItemFromDictionaryList<TKey, TValue>(Dictionary<TKey, List<TValue>> sourceDict, TKey key, TValue item)
		{
			List<TValue> list;
			if (sourceDict.TryGetValue(key, out list))
			{
				list.Remove(item);
				if (list.Count == 0)
				{
					sourceDict.Remove(key);
					return;
				}
			}
			else
			{
				Debug.FailedAssert("Trying to remove non-existent item from source dictionary", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\GamepadNavigation\\GauntletGamepadNavigationManager.cs", "RemoveItemFromDictionaryList", 921);
			}
		}

		internal void OnWidgetHoverBegin(EventManager source, Widget widget)
		{
			if (!this.IsCursorMovingForNavigation && !this.IsInWrapMovement && widget.GamepadNavigationIndex != -1 && !this._isAvailableScopesDirty && !this._shouldUpdateAvailableScopes)
			{
				GamepadNavigationForcedScopeCollection activeForcedScopeCollection = this._activeForcedScopeCollection;
				if (activeForcedScopeCollection == null || activeForcedScopeCollection.Scopes.Contains(this._activeNavigationScope))
				{
					int num = this._activeNavigationScope.FindIndexOfWidget(widget);
					if (this._activeNavigationScope != null && num != -1)
					{
						this._activeNavigationScope.LatestNavigationElementIndex = num;
						return;
					}
					int i = 0;
					while (i < this._availableScopesThisFrame.Count)
					{
						GamepadNavigationScope gamepadNavigationScope = this._availableScopesThisFrame[i];
						int num2 = gamepadNavigationScope.FindIndexOfWidget(widget);
						if (!gamepadNavigationScope.DoNotAutoGainNavigationOnInit && num2 != -1)
						{
							if (this._activeNavigationScope != gamepadNavigationScope && gamepadNavigationScope.IsAvailable())
							{
								this.SetActiveNavigationScope(gamepadNavigationScope);
								this._activeNavigationScope.LatestNavigationElementIndex = num2;
								return;
							}
							break;
						}
						else
						{
							i++;
						}
					}
				}
			}
		}

		internal void OnWidgetHoverEnd(Widget widget)
		{
			if (this.IsControllerActive && !this.IsCursorMovingForNavigation && widget.GamepadNavigationIndex != -1)
			{
				GamepadNavigationScope activeNavigationScope = this._activeNavigationScope;
				if (activeNavigationScope != null && activeNavigationScope.IsAvailable())
				{
					this._activeNavigationScope.GetRectangle().IsPointInside(this.MousePosition);
				}
			}
		}

		internal void OnWidgetDisconnectedFromRoot(EventManager source, Widget widget)
		{
			GamepadNavigationScopeCollection gamepadNavigationScopeCollection;
			if (this._navigationScopes.TryGetValue(source, out gamepadNavigationScopeCollection))
			{
				gamepadNavigationScopeCollection.OnWidgetDisconnectedFromRoot(widget);
			}
			List<GamepadNavigationScope> list;
			if (this._navigationScopeParents.TryGetValue(widget, out list))
			{
				List<GamepadNavigationScope> list2 = list.ToList<GamepadNavigationScope>();
				for (int i = 0; i < list2.Count; i++)
				{
					list2[i].ClearNavigatableWidgets();
					this.RemoveNavigationScope(source, list2[i]);
				}
			}
			List<GamepadNavigationForcedScopeCollection> list3;
			if (this._forcedNavigationScopeCollectionParents.TryGetValue(widget, out list3))
			{
				for (int j = 0; j < list3.Count; j++)
				{
					this.RemoveForcedScopeCollection(list3[j]);
				}
			}
		}

		internal void SetEventManagerNavigationGainAfterTime(EventManager source, float seconds, Func<bool> predicate)
		{
			GauntletGamepadNavigationManager.EventManagerGamepadNavigationGainHandler eventManagerGamepadNavigationGainHandler;
			if (this._navigationGainControllers.TryGetValue(source, out eventManagerGamepadNavigationGainHandler))
			{
				eventManagerGamepadNavigationGainHandler.GainNavigationAfterTime(seconds, predicate);
			}
		}

		internal void SetEventManagerNavigationGainAfterFrames(EventManager source, int frames, Func<bool> predicate)
		{
			GauntletGamepadNavigationManager.EventManagerGamepadNavigationGainHandler eventManagerGamepadNavigationGainHandler;
			if (this._navigationGainControllers.TryGetValue(source, out eventManagerGamepadNavigationGainHandler))
			{
				eventManagerGamepadNavigationGainHandler.GainNavigationAfterFrames(frames, predicate);
			}
		}

		internal void OnEventManagerGainedNavigation(EventManager source)
		{
			if (this.IsControllerActive && source != null)
			{
				GamepadNavigationScope activeNavigationScope = this._activeNavigationScope;
				EventManager eventManager;
				if (activeNavigationScope == null)
				{
					eventManager = null;
				}
				else
				{
					Widget parentWidget = activeNavigationScope.ParentWidget;
					eventManager = ((parentWidget != null) ? parentWidget.EventManager : null);
				}
				GamepadNavigationScopeCollection gamepadNavigationScopeCollection;
				if (eventManager != source && source.IsAvailableForNavigation() && this._navigationScopes.TryGetValue(source, out gamepadNavigationScopeCollection))
				{
					this.RefreshAvailableScopes();
					GamepadNavigationScope gamepadNavigationScope = gamepadNavigationScopeCollection.VisibleScopes.FirstOrDefault((GamepadNavigationScope x) => x.IsDefaultNavigationScope && x.IsAvailable());
					if (gamepadNavigationScope != null && this._availableScopesThisFrame.Contains(gamepadNavigationScope))
					{
						if (this._availableScopesThisFrame.Contains(gamepadNavigationScope))
						{
							this.MoveCursorToFirstAvailableWidgetInScope(gamepadNavigationScope);
						}
						return;
					}
					for (int i = 0; i < this._availableScopesThisFrame.Count; i++)
					{
						if (gamepadNavigationScopeCollection.HasScopeInAnyList(this._availableScopesThisFrame[i]))
						{
							this.MoveCursorToFirstAvailableWidgetInScope(this._availableScopesThisFrame[i]);
							return;
						}
					}
					for (int j = 0; j < gamepadNavigationScopeCollection.VisibleScopes.Count; j++)
					{
						if (gamepadNavigationScopeCollection.VisibleScopes[j].IsAvailable() && this._availableScopesThisFrame.Contains(this._nextScopeToGainNavigation))
						{
							this._nextScopeToGainNavigation = gamepadNavigationScopeCollection.VisibleScopes[j];
							return;
						}
					}
				}
			}
		}

		private void SetActiveNavigationScope(GamepadNavigationScope scope)
		{
			if (scope != null && scope != this._activeNavigationScope)
			{
				if (this._activeForcedScopeCollection != null && this._activeForcedScopeCollection.Scopes.Contains(scope))
				{
					this._activeForcedScopeCollection.ActiveScope = scope;
				}
				if (this._activeNavigationScope != null)
				{
					GamepadNavigationScope activeNavigationScope = this._activeNavigationScope;
					activeNavigationScope.OnParentScopeChanged = (Action<GamepadNavigationScope, GamepadNavigationScope>)Delegate.Remove(activeNavigationScope.OnParentScopeChanged, new Action<GamepadNavigationScope, GamepadNavigationScope>(this.OnActiveScopeParentChanged));
				}
				GamepadNavigationScope activeNavigationScope2 = this._activeNavigationScope;
				this._activeNavigationScope = scope;
				this._activeNavigationScope.PreviousScope = activeNavigationScope2;
				if (activeNavigationScope2 != null)
				{
					activeNavigationScope2.SetIsActiveScope(false);
				}
				this._activeNavigationScope.SetIsActiveScope(true);
				if (this._activeNavigationScope != null)
				{
					GamepadNavigationScope activeNavigationScope3 = this._activeNavigationScope;
					activeNavigationScope3.OnParentScopeChanged = (Action<GamepadNavigationScope, GamepadNavigationScope>)Delegate.Combine(activeNavigationScope3.OnParentScopeChanged, new Action<GamepadNavigationScope, GamepadNavigationScope>(this.OnActiveScopeParentChanged));
				}
			}
		}

		private void OnGamepadNavigation(GamepadNavigationTypes movement)
		{
			if (this._isAvailableScopesDirty || this._isForcedCollectionsDirty || this.LatestEventManager == null)
			{
				return;
			}
			if (this.AnyWidgetUsingNavigation)
			{
				return;
			}
			GamepadNavigationScope activeNavigationScope = this._activeNavigationScope;
			if (((activeNavigationScope != null) ? activeNavigationScope.ParentWidget : null) != null)
			{
				GamepadNavigationScope activeNavigationScope2 = this._activeNavigationScope;
				if (activeNavigationScope2 != null && activeNavigationScope2.IsAvailable())
				{
					if (this.HandleGamepadNavigation(movement) && this._latestGamepadNavigationWidget != null)
					{
						Rectangle rectangle = new Rectangle(this._latestGamepadNavigationWidget.GlobalPosition.X, this._latestGamepadNavigationWidget.GlobalPosition.Y, this._latestGamepadNavigationWidget.Size.X, this._latestGamepadNavigationWidget.Size.Y);
						GamepadNavigationTypes movementsToReachRectangle = GamepadNavigationHelper.GetMovementsToReachRectangle(this.MousePosition, rectangle);
						if (((movementsToReachRectangle & GamepadNavigationTypes.Left) != GamepadNavigationTypes.None && movement == GamepadNavigationTypes.Right) || ((movementsToReachRectangle & GamepadNavigationTypes.Right) != GamepadNavigationTypes.None && movement == GamepadNavigationTypes.Left) || ((movementsToReachRectangle & GamepadNavigationTypes.Up) != GamepadNavigationTypes.None && movement == GamepadNavigationTypes.Down) || ((movementsToReachRectangle & GamepadNavigationTypes.Down) != GamepadNavigationTypes.None && movement == GamepadNavigationTypes.Up))
						{
							this.IsInWrapMovement = true;
							return;
						}
					}
					else if (!this.IsCursorMovingForNavigation && !this.IsInWrapMovement && (this._activeNavigationScope == null || !this._activeNavigationScope.GetRectangle().IsPointInside(this.MousePosition)))
					{
						this.MoveCursorToBestAvailableScope(true, movement);
					}
					return;
				}
			}
			this.MoveCursorToBestAvailableScope(false, movement);
		}

		private bool HandleGamepadNavigation(GamepadNavigationTypes movement)
		{
			GamepadNavigationScope activeNavigationScope = this._activeNavigationScope;
			if (((activeNavigationScope != null) ? activeNavigationScope.ParentWidget : null) == null)
			{
				Debug.FailedAssert("Active navigation scope or it's parent widget shouldn't be null", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\GamepadNavigation\\GauntletGamepadNavigationManager.cs", "HandleGamepadNavigation", 1146);
				this.MoveCursorToBestAvailableScope(true, GamepadNavigationTypes.None);
				return false;
			}
			if (!this.IsInWrapMovement)
			{
				if ((this._activeNavigationScope.ScopeMovements & movement) == GamepadNavigationTypes.None && (this._activeNavigationScope.AlternateScopeMovements & movement) == GamepadNavigationTypes.None)
				{
					bool flag = this.NavigateBetweenScopes(movement, this._activeNavigationScope);
					if (!flag && !this.IsHoldingDpadKeysForNavigation)
					{
						Widget lastNavigatedWidget = this._activeNavigationScope.LastNavigatedWidget;
						if (lastNavigatedWidget == null || !lastNavigatedWidget.IsPointInsideGamepadCursorArea(this.MousePosition))
						{
							this.SetCurrentNavigatedWidget(this._activeNavigationScope, this._activeNavigationScope.LastNavigatedWidget);
							flag = true;
						}
					}
					return flag;
				}
				if (this._activeNavigationScope.IsAvailable())
				{
					bool flag2 = this.NavigateWithinScope(this._activeNavigationScope, movement);
					if (!flag2 && !this.IsHoldingDpadKeysForNavigation)
					{
						GamepadNavigationScope activeNavigationScope2 = this._activeNavigationScope;
						bool flag3;
						if (activeNavigationScope2 == null)
						{
							flag3 = true;
						}
						else
						{
							Widget lastNavigatedWidget2 = activeNavigationScope2.LastNavigatedWidget;
							bool? flag4 = ((lastNavigatedWidget2 != null) ? new bool?(lastNavigatedWidget2.IsPointInsideMeasuredArea(this.MousePosition)) : null);
							bool flag5 = true;
							flag3 = !((flag4.GetValueOrDefault() == flag5) & (flag4 != null));
						}
						if (flag3)
						{
							flag2 = this.MoveCursorToBestAvailableScope(true, movement);
						}
					}
					return flag2;
				}
			}
			return false;
		}

		private bool NavigateBetweenScopes(GamepadNavigationTypes movement, GamepadNavigationScope currentScope)
		{
			this.RefreshExitMovementForScope(currentScope, movement);
			GamepadNavigationScope gamepadNavigationScope = currentScope.InterScopeMovements[movement];
			if (gamepadNavigationScope != null)
			{
				Widget bestWidgetToScope = this.GetBestWidgetToScope(currentScope, gamepadNavigationScope, movement);
				if (bestWidgetToScope != null)
				{
					if (gamepadNavigationScope.ChildScopes.Count > 0)
					{
						float num;
						GamepadNavigationScope closestChildScopeAtDirection = GamepadNavigationHelper.GetClosestChildScopeAtDirection(gamepadNavigationScope, this.MousePosition, movement, false, out num);
						float distanceToClosestWidgetEdge = GamepadNavigationHelper.GetDistanceToClosestWidgetEdge(gamepadNavigationScope.ParentWidget, this.MousePosition, movement);
						if (closestChildScopeAtDirection != null && closestChildScopeAtDirection != currentScope && num < distanceToClosestWidgetEdge)
						{
							Widget bestWidgetToScope2 = this.GetBestWidgetToScope(currentScope, closestChildScopeAtDirection, movement);
							if (bestWidgetToScope2 != null)
							{
								this.SetCurrentNavigatedWidget(closestChildScopeAtDirection, bestWidgetToScope2);
								return true;
							}
						}
					}
					else if (currentScope.ParentScope != null && (currentScope.ParentScope.ScopeMovements & movement) != GamepadNavigationTypes.None)
					{
						Widget bestWidgetToScope3 = this.GetBestWidgetToScope(currentScope, currentScope.ParentScope, movement);
						if (bestWidgetToScope3 != null)
						{
							float distanceToClosestWidgetEdge2 = GamepadNavigationHelper.GetDistanceToClosestWidgetEdge(bestWidgetToScope3, this.MousePosition, movement);
							float distanceToClosestWidgetEdge3 = GamepadNavigationHelper.GetDistanceToClosestWidgetEdge(gamepadNavigationScope.ParentWidget, this.MousePosition, movement);
							if (distanceToClosestWidgetEdge2 < distanceToClosestWidgetEdge3)
							{
								this.SetCurrentNavigatedWidget(currentScope.ParentScope, bestWidgetToScope3);
								return true;
							}
						}
					}
					this.SetCurrentNavigatedWidget(gamepadNavigationScope, bestWidgetToScope);
					return true;
				}
			}
			return false;
		}

		private bool NavigateWithinScope(GamepadNavigationScope scope, GamepadNavigationTypes movement)
		{
			if (scope.NavigatableWidgets.Count == 0)
			{
				return false;
			}
			if ((scope.ScopeMovements & movement) == GamepadNavigationTypes.None && (scope.AlternateScopeMovements & movement) == GamepadNavigationTypes.None)
			{
				return false;
			}
			int num = ((movement == GamepadNavigationTypes.Right || movement == GamepadNavigationTypes.Down) ? 1 : (-1));
			if (scope.LatestNavigationElementIndex < 0 || scope.LatestNavigationElementIndex >= scope.NavigatableWidgets.Count)
			{
				scope.LatestNavigationElementIndex = scope.NavigatableWidgets.Count - 1;
			}
			int latestNavigationElementIndex = scope.LatestNavigationElementIndex;
			int num2 = latestNavigationElementIndex;
			if ((movement & scope.AlternateScopeMovements) != GamepadNavigationTypes.None)
			{
				num *= scope.AlternateMovementStepSize;
			}
			ReadOnlyCollection<Widget> navigatableWidgets = scope.NavigatableWidgets;
			bool flag = false;
			for (;;)
			{
				if (!scope.HasCircularMovement)
				{
					bool flag2 = false;
					if (scope.AlternateMovementStepSize > 0)
					{
						if ((movement & scope.ScopeMovements) != GamepadNavigationTypes.None && Math.Abs(num) == 1)
						{
							if (num2 % scope.AlternateMovementStepSize == 0 && num < 0)
							{
								flag2 = true;
							}
							else if (num2 % scope.AlternateMovementStepSize == scope.AlternateMovementStepSize - 1 && num > 0)
							{
								flag2 = true;
							}
							else if (num2 + num < 0 || num2 + num > scope.NavigatableWidgets.Count - 1)
							{
								flag2 = true;
							}
						}
						if (!flag2 && (movement & scope.AlternateScopeMovements) != GamepadNavigationTypes.None && Math.Abs(num) > 1)
						{
							int num3 = scope.NavigatableWidgets.Count % scope.AlternateMovementStepSize;
							if (scope.NavigatableWidgets.Count > 0 && num3 == 0)
							{
								num3 = scope.AlternateMovementStepSize;
							}
							int num4;
							if (num3 > 0)
							{
								num4 = scope.NavigatableWidgets.Count - num3;
								if (scope.NavigatableWidgets.Count != num3)
								{
									if (num2 < num4 && num2 + num >= scope.NavigatableWidgets.Count)
									{
										break;
									}
									if (num2 >= num4 && num2 + num >= scope.NavigatableWidgets.Count)
									{
										flag2 = true;
									}
								}
								else
								{
									flag2 = true;
								}
							}
							else
							{
								num4 = Math.Max(0, scope.NavigatableWidgets.Count - scope.AlternateMovementStepSize - 1);
							}
							if (num2 > num4 && num2 < scope.NavigatableWidgets.Count && num > 1)
							{
								flag2 = true;
							}
							if (num2 >= 0 && num2 < scope.AlternateMovementStepSize && num < 1)
							{
								flag2 = true;
							}
						}
					}
					else if (num2 + num < 0 || num2 + num > scope.NavigatableWidgets.Count - 1)
					{
						flag2 = true;
					}
					if (flag2)
					{
						goto Block_35;
					}
				}
				num2 += num;
				if (num2 > scope.NavigatableWidgets.Count - 1 && !scope.HasCircularMovement)
				{
					return false;
				}
				num2 %= scope.NavigatableWidgets.Count;
				if (num2 < 0)
				{
					num2 = navigatableWidgets.Count - 1;
				}
				if (scope.IsWidgetVisible(navigatableWidgets[num2]))
				{
					goto Block_39;
				}
				if (num2 < 0 || num2 >= navigatableWidgets.Count || num2 == latestNavigationElementIndex)
				{
					goto IL_28D;
				}
			}
			this.SetCurrentNavigatedWidget(scope, scope.GetLastAvailableWidget());
			return true;
			Block_35:
			return this.NavigateBetweenScopes(movement, this._activeNavigationScope);
			Block_39:
			flag = true;
			IL_28D:
			if (num2 >= 0 && flag)
			{
				if (scope.ChildScopes.Count > 0)
				{
					float num5;
					GamepadNavigationScope closestChildScopeAtDirection = GamepadNavigationHelper.GetClosestChildScopeAtDirection(scope, this.MousePosition, movement, false, out num5);
					if (num5 != -1f && closestChildScopeAtDirection != null)
					{
						Vector2 vector;
						GamepadNavigationHelper.GetDistanceToClosestWidgetEdge(navigatableWidgets[num2], this.MousePosition, movement, out vector);
						if (GamepadNavigationHelper.GetDirectionalDistanceBetweenTwoPoints(movement, this.MousePosition, vector) > num5)
						{
							this.SetCurrentNavigatedWidget(closestChildScopeAtDirection, closestChildScopeAtDirection.GetApproximatelyClosestWidgetToPosition(this.MousePosition, movement, false));
							return true;
						}
					}
				}
				this.SetCurrentNavigatedWidget(scope, scope.NavigatableWidgets[num2]);
				return true;
			}
			return false;
		}

		private bool SetCurrentNavigatedWidget(GamepadNavigationScope scope, Widget widget)
		{
			if (scope != null && Input.MouseMoveX == 0f && Input.MouseMoveY == 0f)
			{
				int num = scope.FindIndexOfWidget(widget);
				if (num != -1)
				{
					if (this._activeNavigationScope != scope)
					{
						this.SetActiveNavigationScope(scope);
					}
					this._latestGamepadNavigationWidget = widget;
					this._activeNavigationScope.LatestNavigationElementIndex = num;
					if (this.IsControllerActive)
					{
						this._cursorMoveStartTime = this._time;
						this._cursorMoveStartPosition = this.MousePosition;
						this._stopCursorNextFrame = false;
						this.IsCursorMovingForNavigation = true;
						this._latestGamepadNavigationWidget.OnGamepadNavigationFocusGain();
					}
					return true;
				}
			}
			return false;
		}

		private bool MoveCursorToBestAvailableScope(bool isFromInput, GamepadNavigationTypes preferredMovement = GamepadNavigationTypes.None)
		{
			GamepadNavigationScope gamepadNavigationScope = null;
			if (preferredMovement != GamepadNavigationTypes.None)
			{
				float num;
				gamepadNavigationScope = GamepadNavigationHelper.GetClosestScopeAtDirectionFromList(this._availableScopesThisFrame, this.MousePosition, preferredMovement, isFromInput, false, out num, Array.Empty<GamepadNavigationScope>());
			}
			if (gamepadNavigationScope == null)
			{
				gamepadNavigationScope = GamepadNavigationHelper.GetClosestScopeFromList(this._availableScopesThisFrame, this.MousePosition, true);
			}
			if (gamepadNavigationScope != null)
			{
				bool flag = this._activeForcedScopeCollection != null && this._activeForcedScopeCollection.Scopes.Contains(this._activeNavigationScope) && gamepadNavigationScope.LastNavigatedWidget != null;
				Widget widget;
				if ((this._activeNavigationScope != null && !this._activeNavigationScope.IsAvailable() && this._activeNavigationScope.ParentScope == gamepadNavigationScope) || flag)
				{
					widget = gamepadNavigationScope.LastNavigatedWidget;
				}
				else if (!isFromInput && !gamepadNavigationScope.ForceGainNavigationOnClosestChild)
				{
					widget = gamepadNavigationScope.GetFirstAvailableWidget();
				}
				else if (preferredMovement != GamepadNavigationTypes.None)
				{
					widget = gamepadNavigationScope.GetApproximatelyClosestWidgetToPosition(this.MousePosition, preferredMovement, false);
				}
				else
				{
					widget = gamepadNavigationScope.GetApproximatelyClosestWidgetToPosition(this.MousePosition, GamepadNavigationTypes.None, false);
				}
				if (widget != null)
				{
					this.SetCurrentNavigatedWidget(gamepadNavigationScope, widget);
					return true;
				}
			}
			return false;
		}

		private void MoveCursorToFirstAvailableWidgetInScope(GamepadNavigationScope scope)
		{
			this.SetCurrentNavigatedWidget(scope, scope.GetFirstAvailableWidget());
		}

		private void MoveCursorToClosestAvailableWidgetInScope(GamepadNavigationScope scope)
		{
			this.SetCurrentNavigatedWidget(scope, scope.GetApproximatelyClosestWidgetToPosition(this.MousePosition, GamepadNavigationTypes.None, false));
		}

		private void TryMoveCursorToPreviousScope(GamepadNavigationForcedScopeCollection fromCollection)
		{
			GamepadNavigationScope gamepadNavigationScope = ((fromCollection != null) ? fromCollection.PreviousScope : null);
			if (gamepadNavigationScope != null && this._availableScopesThisFrame.Contains(gamepadNavigationScope))
			{
				if (gamepadNavigationScope.LastNavigatedWidget == null)
				{
					this.SetCurrentNavigatedWidget(gamepadNavigationScope, gamepadNavigationScope.GetFirstAvailableWidget());
					return;
				}
				this.SetCurrentNavigatedWidget(gamepadNavigationScope, gamepadNavigationScope.LastNavigatedWidget);
			}
		}

		private GamepadNavigationScope GetBestScopeAtDirectionFrom(GamepadNavigationScope fromScope, GamepadNavigationTypes movement)
		{
			if (fromScope.ChildScopes.Count > 0 && fromScope.HasMovement(movement))
			{
				float num;
				GamepadNavigationScope closestChildScopeAtDirection = GamepadNavigationHelper.GetClosestChildScopeAtDirection(fromScope, this.MousePosition, movement, false, out num);
				if (closestChildScopeAtDirection != null && closestChildScopeAtDirection != this._activeNavigationScope && num > 0f)
				{
					return closestChildScopeAtDirection;
				}
			}
			GamepadNavigationScope gamepadNavigationScope = fromScope.ManualScopes[movement];
			if (gamepadNavigationScope == null)
			{
				if (!string.IsNullOrEmpty(fromScope.ManualScopeIDs[movement]))
				{
					gamepadNavigationScope = this.GetManualScopeAtDirection(movement, fromScope);
				}
				else if (fromScope.GetShouldFindScopeByPosition(movement))
				{
					if (fromScope.ParentScope != null && fromScope.ParentScope.HasMovement(movement))
					{
						List<GamepadNavigationScope> list = fromScope.ParentScope.ChildScopes.ToList<GamepadNavigationScope>();
						list.Remove(fromScope);
						if (list.Count > 0)
						{
							float num2;
							gamepadNavigationScope = GamepadNavigationHelper.GetClosestScopeAtDirectionFromList(list, fromScope, this.MousePosition, movement, false, out num2);
						}
						if (gamepadNavigationScope == null && fromScope.ParentScope != null)
						{
							GamepadNavigationForcedScopeCollection activeForcedScopeCollection = this._activeForcedScopeCollection;
							if (activeForcedScopeCollection == null || activeForcedScopeCollection.Scopes.Contains(fromScope.ParentScope))
							{
								if (fromScope.ParentScope.HasMovement(movement))
								{
									gamepadNavigationScope = fromScope.ParentScope;
									if (gamepadNavigationScope.GetApproximatelyClosestWidgetToPosition(this.MousePosition, movement, true) == null)
									{
										return this.GetBestScopeAtDirectionFrom(gamepadNavigationScope, movement);
									}
								}
								else
								{
									bool flag = this._availableScopesThisFrame.Remove(fromScope.ParentScope);
									float num2;
									gamepadNavigationScope = GamepadNavigationHelper.GetClosestScopeAtDirectionFromList(this._availableScopesThisFrame, fromScope, this.MousePosition, movement, false, out num2);
									if (flag)
									{
										this._availableScopesThisFrame.Add(fromScope.ParentScope);
									}
								}
							}
						}
					}
					else
					{
						bool flag2 = fromScope.ChildScopes.Count > 0;
						List<GamepadNavigationScope> list2 = this._availableScopesThisFrame;
						if (flag2)
						{
							list2 = list2.ToList<GamepadNavigationScope>();
							for (int i = 0; i < fromScope.ChildScopes.Count; i++)
							{
								list2.Remove(fromScope.ChildScopes[i]);
							}
						}
						float num3;
						gamepadNavigationScope = GamepadNavigationHelper.GetClosestScopeAtDirectionFromList(list2, fromScope, this.MousePosition, movement, false, out num3);
						if (gamepadNavigationScope != null && gamepadNavigationScope.ChildScopes.Count > 0)
						{
							float num4;
							GamepadNavigationScope closestChildScopeAtDirection2 = GamepadNavigationHelper.GetClosestChildScopeAtDirection(gamepadNavigationScope, this.MousePosition, movement, false, out num4);
							float num5;
							Widget approximatelyClosestWidgetToPosition = gamepadNavigationScope.GetApproximatelyClosestWidgetToPosition(this.MousePosition, out num5, movement, true);
							if (closestChildScopeAtDirection2 != null && closestChildScopeAtDirection2 != this._activeNavigationScope && (num4 < num3 || (approximatelyClosestWidgetToPosition != null && num4 < num5)))
							{
								gamepadNavigationScope = closestChildScopeAtDirection2;
							}
						}
					}
				}
			}
			return gamepadNavigationScope;
		}

		private void RefreshExitMovementForScope(GamepadNavigationScope scope, GamepadNavigationTypes movement)
		{
			scope.InterScopeMovements[movement] = this.GetBestScopeAtDirectionFrom(scope, movement);
		}

		private GamepadNavigationTypes GetMovementForInput(InputKey input)
		{
			if (input == InputKey.ControllerLUp)
			{
				return GamepadNavigationTypes.Up;
			}
			if (input == InputKey.ControllerLRight)
			{
				return GamepadNavigationTypes.Right;
			}
			if (input == InputKey.ControllerLDown)
			{
				return GamepadNavigationTypes.Down;
			}
			if (input == InputKey.ControllerLLeft)
			{
				return GamepadNavigationTypes.Left;
			}
			return GamepadNavigationTypes.None;
		}

		private GamepadNavigationScope GetManualScopeAtDirection(GamepadNavigationTypes movement, GamepadNavigationScope fromScope)
		{
			GamepadNavigationScope gamepadNavigationScope = fromScope.ManualScopes[movement];
			string text = fromScope.ManualScopeIDs[movement];
			if (gamepadNavigationScope == null)
			{
				if (string.IsNullOrEmpty(text) || text == "None")
				{
					return null;
				}
				List<GamepadNavigationScope> list;
				if (this._navigationScopesById.TryGetValue(text, out list))
				{
					if (list.Count == 1)
					{
						gamepadNavigationScope = list[0];
					}
					else
					{
						float num;
						gamepadNavigationScope = GamepadNavigationHelper.GetClosestScopeAtDirectionFromList(list, this.MousePosition, movement, false, false, out num, Array.Empty<GamepadNavigationScope>());
					}
					if (gamepadNavigationScope != null && !gamepadNavigationScope.IsAvailable())
					{
						gamepadNavigationScope = this.GetManualScopeAtDirection(movement, gamepadNavigationScope);
					}
				}
			}
			return gamepadNavigationScope;
		}

		private Widget GetBestWidgetToScope(GamepadNavigationScope fromScope, GamepadNavigationScope toScope, GamepadNavigationTypes movement)
		{
			Widget widget;
			if (toScope.ForceGainNavigationBasedOnDirection && (fromScope == null || toScope != fromScope.ParentScope) && ((toScope.ScopeMovements & movement) != GamepadNavigationTypes.None || (toScope.AlternateScopeMovements & movement) != GamepadNavigationTypes.None))
			{
				if ((movement & GamepadNavigationTypes.Up) != GamepadNavigationTypes.None || (movement & GamepadNavigationTypes.Left) != GamepadNavigationTypes.None)
				{
					widget = toScope.GetLastAvailableWidget();
				}
				else
				{
					widget = toScope.GetFirstAvailableWidget();
				}
			}
			else if (fromScope.ParentScope == toScope)
			{
				widget = toScope.GetApproximatelyClosestWidgetToPosition(this.MousePosition, movement, true);
			}
			else
			{
				widget = toScope.GetApproximatelyClosestWidgetToPosition(this.MousePosition, movement, false);
			}
			return widget;
		}

		private GamepadNavigationScope FindClosestParentScopeOfWidget(Widget widget)
		{
			Widget widget2 = widget;
			while (widget2 != null && !widget2.DoNotAcceptNavigation)
			{
				List<GamepadNavigationScope> list;
				if (this._navigationScopeParents.TryGetValue(widget2, out list))
				{
					if (list.Count > 0)
					{
						return list[0];
					}
					return null;
				}
				else
				{
					widget2 = widget2.ParentWidget;
				}
			}
			return null;
		}

		private GamepadNavigationForcedScopeCollection FindAvailableForcedScope()
		{
			if (this._forcedScopeCollections.Count > 0)
			{
				if (this._isForcedCollectionsDirty)
				{
					this._forcedScopeCollections.Sort(this._cachedForcedScopeComparer);
					this._forcedScopeCollections.ForEach(delegate(GamepadNavigationForcedScopeCollection x)
					{
						this.CollectScopesForForcedCollection(x);
					});
					this._isForcedCollectionsDirty = false;
					this._isAvailableScopesDirty = true;
				}
				for (int i = this._forcedScopeCollections.Count - 1; i >= 0; i--)
				{
					if (this.IsControllerActive && this._forcedScopeCollections[i].IsAvailable())
					{
						return this._forcedScopeCollections[i];
					}
				}
			}
			return null;
		}

		private void HandleInput(float dt)
		{
			if (this.IsControllerActive)
			{
				GamepadNavigationTypes gamepadNavigationTypes = GamepadNavigationTypes.None;
				if (Input.IsKeyPressed(InputKey.ControllerLLeft))
				{
					gamepadNavigationTypes = GamepadNavigationTypes.Left;
				}
				else if (Input.IsKeyPressed(InputKey.ControllerLRight))
				{
					gamepadNavigationTypes = GamepadNavigationTypes.Right;
				}
				else if (Input.IsKeyPressed(InputKey.ControllerLDown))
				{
					gamepadNavigationTypes = GamepadNavigationTypes.Down;
				}
				else if (Input.IsKeyPressed(InputKey.ControllerLUp))
				{
					gamepadNavigationTypes = GamepadNavigationTypes.Up;
				}
				if (gamepadNavigationTypes != GamepadNavigationTypes.None)
				{
					this.OnGamepadNavigation(gamepadNavigationTypes);
				}
				this._navigationHoldTimer += dt;
				if (!this.IsHoldingDpadKeysForNavigation && this._navigationHoldTimer > 0.15f)
				{
					this.IsHoldingDpadKeysForNavigation = true;
					this._navigationHoldTimer = 0f;
				}
				else if (this.IsHoldingDpadKeysForNavigation && this._navigationHoldTimer > 0.08f)
				{
					InputKey inputKey = (InputKey)0;
					if (Input.IsKeyDown(InputKey.ControllerLUp))
					{
						inputKey = InputKey.ControllerLUp;
					}
					else if (Input.IsKeyDown(InputKey.ControllerLRight))
					{
						inputKey = InputKey.ControllerLRight;
					}
					else if (Input.IsKeyDown(InputKey.ControllerLDown))
					{
						inputKey = InputKey.ControllerLDown;
					}
					else if (Input.IsKeyDown(InputKey.ControllerLLeft))
					{
						inputKey = InputKey.ControllerLLeft;
					}
					if (inputKey != (InputKey)0)
					{
						GamepadNavigationTypes movementForInput = this.GetMovementForInput(inputKey);
						this.OnGamepadNavigation(movementForInput);
					}
					this._navigationHoldTimer = 0f;
				}
			}
			if (!Input.IsKeyDown(InputKey.ControllerLUp) && !Input.IsKeyDown(InputKey.ControllerLRight) && !Input.IsKeyDown(InputKey.ControllerLDown) && !Input.IsKeyDown(InputKey.ControllerLLeft))
			{
				this.IsHoldingDpadKeysForNavigation = false;
				this._navigationHoldTimer = 0f;
			}
		}

		private void HandleCursorMovement()
		{
			Vector2 targetCursorPosition = this.GetTargetCursorPosition();
			if (this._latestGamepadNavigationWidget != null && targetCursorPosition != Vec2.Invalid)
			{
				if (this.IsCursorMovingForNavigation)
				{
					if (this._time - this._cursorMoveStartTime <= this._mouseCursorMoveTime)
					{
						this.MousePosition = (this.IsFollowingMobileTarget ? targetCursorPosition : Vector2.Lerp(this._cursorMoveStartPosition, targetCursorPosition, (this._time - this._cursorMoveStartTime) / this._mouseCursorMoveTime));
						this.IsCursorMovingForNavigation = true;
					}
					else
					{
						bool flag = this._latestGamepadNavigationWidget != null && !this.IsHoldingDpadKeysForNavigation && this.IsControllerActive && !Input.IsAnyTouchActive && Vector2.Distance(this.MousePosition, targetCursorPosition) > 1.44f && Input.MouseMoveX == 0f && Input.MouseMoveY == 0f;
						this.MousePosition = targetCursorPosition;
						if (!flag)
						{
							this._latestGamepadNavigationWidget = null;
							this._stopCursorNextFrame = true;
							this.IsInWrapMovement = false;
							this.IsFollowingMobileTarget = false;
						}
					}
				}
				else if (this._latestGamepadNavigationWidget != null)
				{
					this._latestGamepadNavigationWidget = null;
					this._stopCursorNextFrame = true;
					this.IsFollowingMobileTarget = false;
					this.IsInWrapMovement = false;
				}
			}
			if (!this.IsCursorMovingForNavigation && this._activeNavigationScope != null && this._activeNavigationScope.FollowMobileTargets && this._wasCursorInsideActiveScopeLastFrame)
			{
				Widget lastNavigatedWidget = this._activeNavigationScope.LastNavigatedWidget;
				if (lastNavigatedWidget != null)
				{
					Vector2 vector = lastNavigatedWidget.GlobalPosition + lastNavigatedWidget.Size / 2f;
					if (this._lastNavigatedWidgetPosition.X != float.NaN && Vector2.Distance(vector, this._lastNavigatedWidgetPosition) > 1.44f)
					{
						this.SetCurrentNavigatedWidget(this._activeNavigationScope, this._activeNavigationScope.LastNavigatedWidget);
						this._autoRefreshTimer = 0f;
						this.IsFollowingMobileTarget = true;
					}
					this._lastNavigatedWidgetPosition = vector;
				}
			}
		}

		private void OnDpadNavigationStopped()
		{
			this._lastNavigatedWidgetPosition = new Vector2(float.NaN, float.NaN);
			this._latestGamepadNavigationWidget = null;
			this._stopCursorNextFrame = true;
			this.IsFollowingMobileTarget = false;
			this.IsInWrapMovement = false;
			this._navigationHoldTimer = 0f;
		}

		private EventManager _latestCachedEventManager;

		private float _time;

		private bool _stopCursorNextFrame;

		private bool _isForcedCollectionsDirty;

		private GauntletGamepadNavigationManager.EventManagerComparer _cachedEventManagerComparer;

		private GauntletGamepadNavigationManager.ForcedScopeComparer _cachedForcedScopeComparer;

		private List<EventManager> _sortedEventManagers;

		private Dictionary<EventManager, GamepadNavigationScopeCollection> _navigationScopes;

		private List<GamepadNavigationScope> _availableScopesThisFrame;

		private List<GamepadNavigationScope> _unsortedScopes;

		private List<GamepadNavigationForcedScopeCollection> _forcedScopeCollections;

		private GamepadNavigationForcedScopeCollection _activeForcedScopeCollection;

		private GamepadNavigationScope _nextScopeToGainNavigation;

		private GamepadNavigationScope _activeNavigationScope;

		private Dictionary<Widget, List<GamepadNavigationScope>> _navigationScopeParents;

		private Dictionary<Widget, List<GamepadNavigationForcedScopeCollection>> _forcedNavigationScopeCollectionParents;

		private Dictionary<string, List<GamepadNavigationScope>> _layerNavigationScopes;

		private Dictionary<string, List<GamepadNavigationScope>> _navigationScopesById;

		private Dictionary<EventManager, GauntletGamepadNavigationManager.EventManagerGamepadNavigationGainHandler> _navigationGainControllers;

		private float _navigationHoldTimer;

		private Vector2 _lastNavigatedWidgetPosition;

		private readonly float _mouseCursorMoveTime = 0.09f;

		private Vector2 _cursorMoveStartPosition = new Vector2(float.NaN, float.NaN);

		private float _cursorMoveStartTime = -1f;

		private Widget _latestGamepadNavigationWidget;

		private List<Widget> _navigationBlockingWidgets;

		private bool _isAvailableScopesDirty;

		private bool _shouldUpdateAvailableScopes;

		private float _autoRefreshTimer;

		private bool _wasCursorInsideActiveScopeLastFrame;

		private class EventManagerComparer : IComparer<EventManager>
		{
			public int Compare(EventManager x, EventManager y)
			{
				int lastScreenOrder = x.GetLastScreenOrder();
				int lastScreenOrder2 = y.GetLastScreenOrder();
				return -lastScreenOrder.CompareTo(lastScreenOrder2);
			}
		}

		private class ForcedScopeComparer : IComparer<GamepadNavigationForcedScopeCollection>
		{
			public int Compare(GamepadNavigationForcedScopeCollection x, GamepadNavigationForcedScopeCollection y)
			{
				return x.CollectionOrder.CompareTo(y.CollectionOrder);
			}
		}

		private class EventManagerGamepadNavigationGainHandler
		{
			public bool HasTarget { get; private set; }

			public EventManagerGamepadNavigationGainHandler(EventManager eventManager)
			{
				this._eventManager = eventManager;
				this.Clear();
			}

			public void GainNavigationAfterFrames(int frameCount, Func<bool> predicate = null)
			{
				this.Clear();
				if (frameCount >= 0)
				{
					this._gainAfterFrames = frameCount;
					this._gainPredicate = predicate;
					this.HasTarget = true;
				}
			}

			public void GainNavigationAfterTime(float seconds, Func<bool> predicate = null)
			{
				this.Clear();
				if (seconds >= 0f)
				{
					this._gainAfterTime = seconds;
					this._gainPredicate = predicate;
					this.HasTarget = true;
				}
			}

			public void Tick(float dt)
			{
				if (this._gainAfterTime != -1f)
				{
					this._gainTimer += dt;
					if (this._gainTimer > this._gainAfterTime)
					{
						Func<bool> gainPredicate = this._gainPredicate;
						if (gainPredicate == null || gainPredicate())
						{
							this._eventManager.OnGainNavigation();
						}
						this.Clear();
						return;
					}
				}
				else if (this._gainAfterFrames != -1)
				{
					this._frameTicker++;
					if (this._frameTicker > this._gainAfterFrames)
					{
						Func<bool> gainPredicate2 = this._gainPredicate;
						if (gainPredicate2 == null || gainPredicate2())
						{
							this._eventManager.OnGainNavigation();
						}
						this.Clear();
					}
				}
			}

			public void Clear()
			{
				this._gainAfterTime = -1f;
				this._gainAfterFrames = -1;
				this._frameTicker = 0;
				this._gainTimer = 0f;
				this._gainPredicate = null;
			}

			private readonly EventManager _eventManager;

			private float _gainAfterTime;

			private float _gainTimer;

			private int _gainAfterFrames;

			private int _frameTicker;

			private Func<bool> _gainPredicate;
		}
	}
}
