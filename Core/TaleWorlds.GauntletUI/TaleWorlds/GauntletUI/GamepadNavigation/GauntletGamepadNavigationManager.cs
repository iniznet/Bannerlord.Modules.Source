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
	// Token: 0x02000046 RID: 70
	public class GauntletGamepadNavigationManager
	{
		// Token: 0x17000160 RID: 352
		// (get) Token: 0x06000481 RID: 1153 RVA: 0x0001308E File Offset: 0x0001128E
		// (set) Token: 0x06000482 RID: 1154 RVA: 0x00013095 File Offset: 0x00011295
		public static GauntletGamepadNavigationManager Instance { get; private set; }

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06000483 RID: 1155 RVA: 0x000130A0 File Offset: 0x000112A0
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

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06000484 RID: 1156 RVA: 0x000130F8 File Offset: 0x000112F8
		// (set) Token: 0x06000485 RID: 1157 RVA: 0x00013100 File Offset: 0x00011300
		public bool IsFollowingMobileTarget { get; private set; }

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06000486 RID: 1158 RVA: 0x00013109 File Offset: 0x00011309
		// (set) Token: 0x06000487 RID: 1159 RVA: 0x00013111 File Offset: 0x00011311
		public bool IsHoldingDpadKeysForNavigation { get; private set; }

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000488 RID: 1160 RVA: 0x0001311A File Offset: 0x0001131A
		// (set) Token: 0x06000489 RID: 1161 RVA: 0x00013122 File Offset: 0x00011322
		public bool IsCursorMovingForNavigation { get; private set; }

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x0600048A RID: 1162 RVA: 0x0001312B File Offset: 0x0001132B
		// (set) Token: 0x0600048B RID: 1163 RVA: 0x00013133 File Offset: 0x00011333
		public bool IsInWrapMovement { get; private set; }

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x0600048C RID: 1164 RVA: 0x0001313C File Offset: 0x0001133C
		// (set) Token: 0x0600048D RID: 1165 RVA: 0x0001314D File Offset: 0x0001134D
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

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x0600048E RID: 1166 RVA: 0x00013162 File Offset: 0x00011362
		private bool IsControllerActive
		{
			get
			{
				return Input.IsGamepadActive;
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x0600048F RID: 1167 RVA: 0x00013169 File Offset: 0x00011369
		// (set) Token: 0x06000490 RID: 1168 RVA: 0x00013171 File Offset: 0x00011371
		internal ReadOnlyDictionary<EventManager, GamepadNavigationScopeCollection> NavigationScopes { get; private set; }

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06000491 RID: 1169 RVA: 0x0001317A File Offset: 0x0001137A
		// (set) Token: 0x06000492 RID: 1170 RVA: 0x00013182 File Offset: 0x00011382
		internal ReadOnlyDictionary<Widget, List<GamepadNavigationScope>> NavigationScopeParents { get; private set; }

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x06000493 RID: 1171 RVA: 0x0001318B File Offset: 0x0001138B
		// (set) Token: 0x06000494 RID: 1172 RVA: 0x00013193 File Offset: 0x00011393
		internal ReadOnlyDictionary<Widget, List<GamepadNavigationForcedScopeCollection>> ForcedNavigationScopeParents { get; private set; }

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000495 RID: 1173 RVA: 0x0001319C File Offset: 0x0001139C
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

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06000496 RID: 1174 RVA: 0x000131D8 File Offset: 0x000113D8
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

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06000497 RID: 1175 RVA: 0x0001325E File Offset: 0x0001145E
		public bool AnyWidgetUsingNavigation
		{
			get
			{
				return this._navigationBlockingWidgets.Any((Widget x) => x.IsUsingNavigation);
			}
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x0001328C File Offset: 0x0001148C
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

		// Token: 0x06000499 RID: 1177 RVA: 0x000133BA File Offset: 0x000115BA
		private void OnGamepadActiveStateChanged()
		{
			if (this.IsControllerActive && Input.MouseMoveX == 0f && Input.MouseMoveY == 0f)
			{
				this._isAvailableScopesDirty = true;
				this._isForcedCollectionsDirty = true;
			}
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x000133EA File Offset: 0x000115EA
		public static void Initialize(bool isInWindowedMode)
		{
			if (GauntletGamepadNavigationManager.Instance != null)
			{
				Debug.FailedAssert("Gamepad Navigation already initialized", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\GamepadNavigation\\GauntletGamepadNavigationManager.cs", "Initialize", 206);
				return;
			}
			GauntletGamepadNavigationManager.Instance = new GauntletGamepadNavigationManager();
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x00013418 File Offset: 0x00011618
		public bool TryNavigateTo(Widget widget)
		{
			GamepadNavigationScopeCollection gamepadNavigationScopeCollection;
			if (widget != null && widget.GamepadNavigationIndex != -1 && this._navigationScopes.TryGetValue(widget.EventManager, out gamepadNavigationScopeCollection))
			{
				for (int i = 0; i < gamepadNavigationScopeCollection.VisibleScopes.Count; i++)
				{
					GamepadNavigationScope gamepadNavigationScope = gamepadNavigationScopeCollection.VisibleScopes[i];
					if (gamepadNavigationScope.IsAvailable() && gamepadNavigationScope.ParentWidget.CheckIsMyChildRecursive(widget))
					{
						this.SetCurrentNavigatedWidget(gamepadNavigationScope, widget);
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x0001348C File Offset: 0x0001168C
		public bool TryNavigateTo(GamepadNavigationScope scope)
		{
			if (scope != null && scope.IsAvailable())
			{
				Widget approximatelyClosestWidgetToPosition = scope.GetApproximatelyClosestWidgetToPosition(this.MousePosition, GamepadNavigationTypes.None, false);
				if (approximatelyClosestWidgetToPosition != null)
				{
					this.SetCurrentNavigatedWidget(scope, approximatelyClosestWidgetToPosition);
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x000134C4 File Offset: 0x000116C4
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

		// Token: 0x0600049E RID: 1182 RVA: 0x00013558 File Offset: 0x00011758
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
				if (activeNavigationScope != null && activeNavigationScope.IsAvailable() && this._activeNavigationScope.ParentWidget.EventManager.IsAvailableForNavigation())
				{
					goto IL_78;
				}
			}
			this.OnDpadNavigationStopped();
			IL_78:
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

		// Token: 0x0600049F RID: 1183 RVA: 0x000139F4 File Offset: 0x00011BF4
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

		// Token: 0x060004A0 RID: 1184 RVA: 0x00013A78 File Offset: 0x00011C78
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

		// Token: 0x060004A1 RID: 1185 RVA: 0x00013AE8 File Offset: 0x00011CE8
		internal void OnEventManagerAdded(EventManager source)
		{
			this._navigationScopes.Add(source, new GamepadNavigationScopeCollection(source, new Action<GamepadNavigationScope>(this.OnScopeNavigatableWidgetsChanged), new Action<GamepadNavigationScope, bool>(this.OnScopeVisibilityChanged)));
			this._navigationGainControllers.Add(source, new GauntletGamepadNavigationManager.EventManagerGamepadNavigationGainHandler(source));
			this._latestCachedEventManager = null;
		}

		// Token: 0x060004A2 RID: 1186 RVA: 0x00013B38 File Offset: 0x00011D38
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

		// Token: 0x060004A3 RID: 1187 RVA: 0x00013BA0 File Offset: 0x00011DA0
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

		// Token: 0x060004A4 RID: 1188 RVA: 0x00013BFC File Offset: 0x00011DFC
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

		// Token: 0x060004A5 RID: 1189 RVA: 0x00013CB8 File Offset: 0x00011EB8
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

		// Token: 0x060004A6 RID: 1190 RVA: 0x00013DC8 File Offset: 0x00011FC8
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

		// Token: 0x060004A7 RID: 1191 RVA: 0x00013E1C File Offset: 0x0001201C
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
				Debug.FailedAssert("Trying to add a navigation scope collection more than once", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\GamepadNavigation\\GauntletGamepadNavigationManager.cs", "AddForcedScopeCollection", 599);
			}
			this._isAvailableScopesDirty = true;
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x00013E98 File Offset: 0x00012098
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

		// Token: 0x060004A9 RID: 1193 RVA: 0x00013F10 File Offset: 0x00012110
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

		// Token: 0x060004AA RID: 1194 RVA: 0x00013F74 File Offset: 0x00012174
		internal void RemoveNavigationScope(EventManager source, GamepadNavigationScope scope)
		{
			if (scope == null)
			{
				Debug.FailedAssert("Trying to remove null navigation data", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\GamepadNavigation\\GauntletGamepadNavigationManager.cs", "RemoveNavigationScope", 656);
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
				Debug.FailedAssert("Failed to remove scope completely", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\GamepadNavigation\\GauntletGamepadNavigationManager.cs", "RemoveNavigationScope", 712);
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

		// Token: 0x060004AB RID: 1195 RVA: 0x00014218 File Offset: 0x00012418
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

		// Token: 0x060004AC RID: 1196 RVA: 0x0001427C File Offset: 0x0001247C
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

		// Token: 0x060004AD RID: 1197 RVA: 0x000142B8 File Offset: 0x000124B8
		internal bool HasNavigationScope(EventManager source, GamepadNavigationScope scope)
		{
			GamepadNavigationScopeCollection gamepadNavigationScopeCollection;
			return this._navigationScopes.TryGetValue(source, out gamepadNavigationScopeCollection) && (gamepadNavigationScopeCollection.VisibleScopes.Contains(scope) || gamepadNavigationScopeCollection.UninitializedScopes.Contains(scope) || gamepadNavigationScopeCollection.InvisibleScopes.Contains(scope));
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x00014304 File Offset: 0x00012504
		internal bool HasNavigationScope(EventManager source, Func<GamepadNavigationScope, bool> predicate)
		{
			GamepadNavigationScopeCollection gamepadNavigationScopeCollection;
			return this._navigationScopes.TryGetValue(source, out gamepadNavigationScopeCollection) && (gamepadNavigationScopeCollection.VisibleScopes.Any((GamepadNavigationScope x) => predicate(x)) || gamepadNavigationScopeCollection.InvisibleScopes.Any((GamepadNavigationScope x) => predicate(x)));
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x00014362 File Offset: 0x00012562
		private void OnActiveScopeParentChanged(GamepadNavigationScope oldParent, GamepadNavigationScope newParent)
		{
			if (oldParent != null && newParent == null && oldParent.LatestNavigationElementIndex != -1 && oldParent.IsAvailable())
			{
				this._isAvailableScopesDirty = true;
			}
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x00014382 File Offset: 0x00012582
		private void OnScopeVisibilityChanged(GamepadNavigationScope scope, bool isVisible)
		{
			this._isAvailableScopesDirty = true;
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x0001438B File Offset: 0x0001258B
		private void OnForcedScopeCollectionAvailabilityStateChanged(GamepadNavigationForcedScopeCollection scopeCollection)
		{
			this._isAvailableScopesDirty = true;
			this._isForcedCollectionsDirty = true;
		}

		// Token: 0x060004B2 RID: 1202 RVA: 0x0001439B File Offset: 0x0001259B
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

		// Token: 0x060004B3 RID: 1203 RVA: 0x000143C8 File Offset: 0x000125C8
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

		// Token: 0x060004B4 RID: 1204 RVA: 0x00014440 File Offset: 0x00012640
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

		// Token: 0x060004B5 RID: 1205 RVA: 0x00014594 File Offset: 0x00012794
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
			Debug.FailedAssert("Trying to add same item to source dictionary twice", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\GamepadNavigation\\GauntletGamepadNavigationManager.cs", "AddItemToDictionaryList", 901);
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x000145EC File Offset: 0x000127EC
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
				Debug.FailedAssert("Trying to remove non-existent item from source dictionary", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\GamepadNavigation\\GauntletGamepadNavigationManager.cs", "RemoveItemFromDictionaryList", 922);
			}
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x00014638 File Offset: 0x00012838
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

		// Token: 0x060004B8 RID: 1208 RVA: 0x00014724 File Offset: 0x00012924
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

		// Token: 0x060004B9 RID: 1209 RVA: 0x00014778 File Offset: 0x00012978
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

		// Token: 0x060004BA RID: 1210 RVA: 0x00014818 File Offset: 0x00012A18
		internal void SetEventManagerNavigationGainAfterTime(EventManager source, float seconds, Func<bool> predicate)
		{
			GauntletGamepadNavigationManager.EventManagerGamepadNavigationGainHandler eventManagerGamepadNavigationGainHandler;
			if (this._navigationGainControllers.TryGetValue(source, out eventManagerGamepadNavigationGainHandler))
			{
				eventManagerGamepadNavigationGainHandler.GainNavigationAfterTime(seconds, predicate);
			}
		}

		// Token: 0x060004BB RID: 1211 RVA: 0x00014840 File Offset: 0x00012A40
		internal void SetEventManagerNavigationGainAfterFrames(EventManager source, int frames, Func<bool> predicate)
		{
			GauntletGamepadNavigationManager.EventManagerGamepadNavigationGainHandler eventManagerGamepadNavigationGainHandler;
			if (this._navigationGainControllers.TryGetValue(source, out eventManagerGamepadNavigationGainHandler))
			{
				eventManagerGamepadNavigationGainHandler.GainNavigationAfterFrames(frames, predicate);
			}
		}

		// Token: 0x060004BC RID: 1212 RVA: 0x00014868 File Offset: 0x00012A68
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

		// Token: 0x060004BD RID: 1213 RVA: 0x000149AC File Offset: 0x00012BAC
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

		// Token: 0x060004BE RID: 1214 RVA: 0x00014A80 File Offset: 0x00012C80
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

		// Token: 0x060004BF RID: 1215 RVA: 0x00014BB8 File Offset: 0x00012DB8
		private bool HandleGamepadNavigation(GamepadNavigationTypes movement)
		{
			GamepadNavigationScope activeNavigationScope = this._activeNavigationScope;
			if (((activeNavigationScope != null) ? activeNavigationScope.ParentWidget : null) == null)
			{
				Debug.FailedAssert("Active navigation scope or it's parent widget shouldn't be null", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\GamepadNavigation\\GauntletGamepadNavigationManager.cs", "HandleGamepadNavigation", 1147);
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

		// Token: 0x060004C0 RID: 1216 RVA: 0x00014CFC File Offset: 0x00012EFC
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

		// Token: 0x060004C1 RID: 1217 RVA: 0x00014E00 File Offset: 0x00013000
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
					goto IL_28C;
				}
			}
			this.SetCurrentNavigatedWidget(scope, scope.GetLastAvailableWidget());
			return true;
			Block_35:
			return this.NavigateBetweenScopes(movement, this._activeNavigationScope);
			Block_39:
			flag = true;
			IL_28C:
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

		// Token: 0x060004C2 RID: 1218 RVA: 0x0001512C File Offset: 0x0001332C
		private void SetCurrentNavigatedWidget(GamepadNavigationScope scope, Widget widget)
		{
			if (scope != null)
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
				}
			}
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x000151A4 File Offset: 0x000133A4
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

		// Token: 0x060004C4 RID: 1220 RVA: 0x00015293 File Offset: 0x00013493
		private void MoveCursorToFirstAvailableWidgetInScope(GamepadNavigationScope scope)
		{
			this.SetCurrentNavigatedWidget(scope, scope.GetFirstAvailableWidget());
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x000152A2 File Offset: 0x000134A2
		private void MoveCursorToClosestAvailableWidgetInScope(GamepadNavigationScope scope)
		{
			this.SetCurrentNavigatedWidget(scope, scope.GetApproximatelyClosestWidgetToPosition(this.MousePosition, GamepadNavigationTypes.None, false));
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x000152BC File Offset: 0x000134BC
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

		// Token: 0x060004C7 RID: 1223 RVA: 0x00015310 File Offset: 0x00013510
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

		// Token: 0x060004C8 RID: 1224 RVA: 0x0001554C File Offset: 0x0001374C
		private void RefreshExitMovementForScope(GamepadNavigationScope scope, GamepadNavigationTypes movement)
		{
			scope.InterScopeMovements[movement] = this.GetBestScopeAtDirectionFrom(scope, movement);
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x00015562 File Offset: 0x00013762
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

		// Token: 0x060004CA RID: 1226 RVA: 0x00015590 File Offset: 0x00013790
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

		// Token: 0x060004CB RID: 1227 RVA: 0x00015620 File Offset: 0x00013820
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

		// Token: 0x060004CC RID: 1228 RVA: 0x0001569C File Offset: 0x0001389C
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

		// Token: 0x060004CD RID: 1229 RVA: 0x000156E4 File Offset: 0x000138E4
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

		// Token: 0x060004CE RID: 1230 RVA: 0x00015780 File Offset: 0x00013980
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

		// Token: 0x060004CF RID: 1231 RVA: 0x000158E0 File Offset: 0x00013AE0
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
						bool flag = this._latestGamepadNavigationWidget != null && !this.IsHoldingDpadKeysForNavigation && this.IsControllerActive && Vector2.Distance(this.MousePosition, targetCursorPosition) > 1.44f && Input.MouseMoveX == 0f && Input.MouseMoveY == 0f;
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

		// Token: 0x060004D0 RID: 1232 RVA: 0x00015AB3 File Offset: 0x00013CB3
		private void OnDpadNavigationStopped()
		{
			this._lastNavigatedWidgetPosition = new Vector2(float.NaN, float.NaN);
			this._latestGamepadNavigationWidget = null;
			this._stopCursorNextFrame = true;
			this.IsFollowingMobileTarget = false;
			this.IsInWrapMovement = false;
			this._navigationHoldTimer = 0f;
		}

		// Token: 0x0400023E RID: 574
		private EventManager _latestCachedEventManager;

		// Token: 0x04000243 RID: 579
		private float _time;

		// Token: 0x04000244 RID: 580
		private bool _stopCursorNextFrame;

		// Token: 0x04000245 RID: 581
		private bool _isForcedCollectionsDirty;

		// Token: 0x04000246 RID: 582
		private GauntletGamepadNavigationManager.EventManagerComparer _cachedEventManagerComparer;

		// Token: 0x04000247 RID: 583
		private GauntletGamepadNavigationManager.ForcedScopeComparer _cachedForcedScopeComparer;

		// Token: 0x04000248 RID: 584
		private List<EventManager> _sortedEventManagers;

		// Token: 0x04000249 RID: 585
		private Dictionary<EventManager, GamepadNavigationScopeCollection> _navigationScopes;

		// Token: 0x0400024B RID: 587
		private List<GamepadNavigationScope> _availableScopesThisFrame;

		// Token: 0x0400024C RID: 588
		private List<GamepadNavigationScope> _unsortedScopes;

		// Token: 0x0400024D RID: 589
		private List<GamepadNavigationForcedScopeCollection> _forcedScopeCollections;

		// Token: 0x0400024E RID: 590
		private GamepadNavigationForcedScopeCollection _activeForcedScopeCollection;

		// Token: 0x0400024F RID: 591
		private GamepadNavigationScope _nextScopeToGainNavigation;

		// Token: 0x04000250 RID: 592
		private GamepadNavigationScope _activeNavigationScope;

		// Token: 0x04000251 RID: 593
		private Dictionary<Widget, List<GamepadNavigationScope>> _navigationScopeParents;

		// Token: 0x04000252 RID: 594
		private Dictionary<Widget, List<GamepadNavigationForcedScopeCollection>> _forcedNavigationScopeCollectionParents;

		// Token: 0x04000255 RID: 597
		private Dictionary<string, List<GamepadNavigationScope>> _layerNavigationScopes;

		// Token: 0x04000256 RID: 598
		private Dictionary<string, List<GamepadNavigationScope>> _navigationScopesById;

		// Token: 0x04000257 RID: 599
		private Dictionary<EventManager, GauntletGamepadNavigationManager.EventManagerGamepadNavigationGainHandler> _navigationGainControllers;

		// Token: 0x04000258 RID: 600
		private float _navigationHoldTimer;

		// Token: 0x04000259 RID: 601
		private Vector2 _lastNavigatedWidgetPosition;

		// Token: 0x0400025A RID: 602
		private readonly float _mouseCursorMoveTime = 0.09f;

		// Token: 0x0400025B RID: 603
		private Vector2 _cursorMoveStartPosition = new Vector2(float.NaN, float.NaN);

		// Token: 0x0400025C RID: 604
		private float _cursorMoveStartTime = -1f;

		// Token: 0x0400025D RID: 605
		private Widget _latestGamepadNavigationWidget;

		// Token: 0x0400025E RID: 606
		private List<Widget> _navigationBlockingWidgets;

		// Token: 0x0400025F RID: 607
		private bool _isAvailableScopesDirty;

		// Token: 0x04000260 RID: 608
		private bool _shouldUpdateAvailableScopes;

		// Token: 0x04000261 RID: 609
		private float _autoRefreshTimer;

		// Token: 0x04000262 RID: 610
		private bool _wasCursorInsideActiveScopeLastFrame;

		// Token: 0x02000081 RID: 129
		private class EventManagerComparer : IComparer<EventManager>
		{
			// Token: 0x06000899 RID: 2201 RVA: 0x00022B54 File Offset: 0x00020D54
			public int Compare(EventManager x, EventManager y)
			{
				int lastScreenOrder = x.GetLastScreenOrder();
				int lastScreenOrder2 = y.GetLastScreenOrder();
				return -lastScreenOrder.CompareTo(lastScreenOrder2);
			}
		}

		// Token: 0x02000082 RID: 130
		private class ForcedScopeComparer : IComparer<GamepadNavigationForcedScopeCollection>
		{
			// Token: 0x0600089B RID: 2203 RVA: 0x00022B80 File Offset: 0x00020D80
			public int Compare(GamepadNavigationForcedScopeCollection x, GamepadNavigationForcedScopeCollection y)
			{
				return x.CollectionOrder.CompareTo(y.CollectionOrder);
			}
		}

		// Token: 0x02000083 RID: 131
		private class EventManagerGamepadNavigationGainHandler
		{
			// Token: 0x1700028F RID: 655
			// (get) Token: 0x0600089D RID: 2205 RVA: 0x00022BA9 File Offset: 0x00020DA9
			// (set) Token: 0x0600089E RID: 2206 RVA: 0x00022BB1 File Offset: 0x00020DB1
			public bool HasTarget { get; private set; }

			// Token: 0x0600089F RID: 2207 RVA: 0x00022BBA File Offset: 0x00020DBA
			public EventManagerGamepadNavigationGainHandler(EventManager eventManager)
			{
				this._eventManager = eventManager;
				this.Clear();
			}

			// Token: 0x060008A0 RID: 2208 RVA: 0x00022BCF File Offset: 0x00020DCF
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

			// Token: 0x060008A1 RID: 2209 RVA: 0x00022BF0 File Offset: 0x00020DF0
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

			// Token: 0x060008A2 RID: 2210 RVA: 0x00022C18 File Offset: 0x00020E18
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

			// Token: 0x060008A3 RID: 2211 RVA: 0x00022CBE File Offset: 0x00020EBE
			public void Clear()
			{
				this._gainAfterTime = -1f;
				this._gainAfterFrames = -1;
				this._frameTicker = 0;
				this._gainTimer = 0f;
				this._gainPredicate = null;
			}

			// Token: 0x0400043B RID: 1083
			private readonly EventManager _eventManager;

			// Token: 0x0400043D RID: 1085
			private float _gainAfterTime;

			// Token: 0x0400043E RID: 1086
			private float _gainTimer;

			// Token: 0x0400043F RID: 1087
			private int _gainAfterFrames;

			// Token: 0x04000440 RID: 1088
			private int _frameTicker;

			// Token: 0x04000441 RID: 1089
			private Func<bool> _gainPredicate;
		}
	}
}
