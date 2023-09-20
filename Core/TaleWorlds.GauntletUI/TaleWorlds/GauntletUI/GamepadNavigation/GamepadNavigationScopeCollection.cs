using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.GamepadNavigation
{
	internal class GamepadNavigationScopeCollection
	{
		public EventManager Source { get; private set; }

		public ReadOnlyCollection<GamepadNavigationScope> AllScopes { get; private set; }

		public ReadOnlyCollection<GamepadNavigationScope> UninitializedScopes { get; private set; }

		public ReadOnlyCollection<GamepadNavigationScope> VisibleScopes { get; private set; }

		public ReadOnlyCollection<GamepadNavigationScope> InvisibleScopes { get; private set; }

		public GamepadNavigationScopeCollection(EventManager source, Action<GamepadNavigationScope> onScopeNavigatableWidgetsChanged, Action<GamepadNavigationScope, bool> onScopeVisibilityChanged)
		{
			this._onScopeNavigatableWidgetsChanged = onScopeNavigatableWidgetsChanged;
			this._onScopeVisibilityChanged = onScopeVisibilityChanged;
			this.Source = source;
			this._allScopes = new List<GamepadNavigationScope>();
			this.AllScopes = new ReadOnlyCollection<GamepadNavigationScope>(this._allScopes);
			this._uninitializedScopes = new List<GamepadNavigationScope>();
			this.UninitializedScopes = new ReadOnlyCollection<GamepadNavigationScope>(this._uninitializedScopes);
			this._visibleScopes = new List<GamepadNavigationScope>();
			this.VisibleScopes = new ReadOnlyCollection<GamepadNavigationScope>(this._visibleScopes);
			this._invisibleScopes = new List<GamepadNavigationScope>();
			this.InvisibleScopes = new ReadOnlyCollection<GamepadNavigationScope>(this._invisibleScopes);
			this._dirtyScopes = new List<GamepadNavigationScope>();
		}

		internal void OnFinalize()
		{
			this.ClearAllScopes();
			this._onScopeVisibilityChanged = null;
			this._onScopeNavigatableWidgetsChanged = null;
		}

		internal void HandleScopeVisibilities()
		{
			List<GamepadNavigationScope> dirtyScopes = this._dirtyScopes;
			lock (dirtyScopes)
			{
				for (int i = 0; i < this._dirtyScopes.Count; i++)
				{
					if (this._dirtyScopes[i] != null)
					{
						for (int j = i + 1; j < this._dirtyScopes.Count; j++)
						{
							if (this._dirtyScopes[i] == this._dirtyScopes[j])
							{
								this._dirtyScopes[j] = null;
							}
						}
					}
				}
				foreach (GamepadNavigationScope gamepadNavigationScope in this._dirtyScopes)
				{
					if (gamepadNavigationScope != null)
					{
						bool flag2 = gamepadNavigationScope.IsVisible();
						this._visibleScopes.Remove(gamepadNavigationScope);
						this._invisibleScopes.Remove(gamepadNavigationScope);
						if (flag2)
						{
							this._visibleScopes.Add(gamepadNavigationScope);
						}
						else
						{
							this._invisibleScopes.Add(gamepadNavigationScope);
						}
						this._onScopeVisibilityChanged(gamepadNavigationScope, flag2);
					}
				}
				this._dirtyScopes.Clear();
			}
		}

		private void OnScopeVisibilityChanged(GamepadNavigationScope scope, bool isVisible)
		{
			List<GamepadNavigationScope> dirtyScopes = this._dirtyScopes;
			lock (dirtyScopes)
			{
				this._dirtyScopes.Add(scope);
			}
		}

		private void OnScopeNavigatableWidgetsChanged(GamepadNavigationScope scope)
		{
			this._onScopeNavigatableWidgetsChanged(scope);
		}

		internal int GetTotalNumberOfScopes()
		{
			return this._visibleScopes.Count + this._invisibleScopes.Count + this._uninitializedScopes.Count;
		}

		internal void AddScope(GamepadNavigationScope scope)
		{
			this._uninitializedScopes.Add(scope);
			this._allScopes.Add(scope);
		}

		internal void RemoveScope(GamepadNavigationScope scope)
		{
			this._allScopes.Remove(scope);
			this._uninitializedScopes.Remove(scope);
			this._visibleScopes.Remove(scope);
			this._invisibleScopes.Remove(scope);
			scope.OnVisibilityChanged = (Action<GamepadNavigationScope, bool>)Delegate.Remove(scope.OnVisibilityChanged, new Action<GamepadNavigationScope, bool>(this.OnScopeVisibilityChanged));
			scope.OnNavigatableWidgetsChanged = (Action<GamepadNavigationScope>)Delegate.Remove(scope.OnNavigatableWidgetsChanged, new Action<GamepadNavigationScope>(this.OnScopeNavigatableWidgetsChanged));
		}

		internal bool HasScopeInAnyList(GamepadNavigationScope scope)
		{
			return this._visibleScopes.Contains(scope) || this._invisibleScopes.Contains(scope) || this._uninitializedScopes.Contains(scope);
		}

		internal void OnNavigationScopeInitialized(GamepadNavigationScope scope)
		{
			this._uninitializedScopes.Remove(scope);
			if (scope.IsVisible())
			{
				this._visibleScopes.Add(scope);
			}
			else
			{
				this._invisibleScopes.Add(scope);
			}
			scope.OnVisibilityChanged = (Action<GamepadNavigationScope, bool>)Delegate.Combine(scope.OnVisibilityChanged, new Action<GamepadNavigationScope, bool>(this.OnScopeVisibilityChanged));
			scope.OnNavigatableWidgetsChanged = (Action<GamepadNavigationScope>)Delegate.Combine(scope.OnNavigatableWidgetsChanged, new Action<GamepadNavigationScope>(this.OnScopeNavigatableWidgetsChanged));
		}

		internal void OnWidgetDisconnectedFromRoot(Widget widget)
		{
			for (int i = 0; i < this._visibleScopes.Count; i++)
			{
				if (this._visibleScopes[i].FindIndexOfWidget(widget) != -1)
				{
					this._visibleScopes[i].RemoveWidget(widget);
					return;
				}
			}
			for (int j = 0; j < this._invisibleScopes.Count; j++)
			{
				if (this._invisibleScopes[j].FindIndexOfWidget(widget) != -1)
				{
					this._invisibleScopes[j].RemoveWidget(widget);
					return;
				}
			}
			for (int k = 0; k < this._uninitializedScopes.Count; k++)
			{
				if (this._uninitializedScopes[k].FindIndexOfWidget(widget) != -1)
				{
					this._uninitializedScopes[k].RemoveWidget(widget);
					return;
				}
			}
		}

		private void ClearAllScopes()
		{
			for (int i = 0; i < this._allScopes.Count; i++)
			{
				this._allScopes[i].ClearNavigatableWidgets();
				GamepadNavigationScope gamepadNavigationScope = this._allScopes[i];
				gamepadNavigationScope.OnNavigatableWidgetsChanged = (Action<GamepadNavigationScope>)Delegate.Remove(gamepadNavigationScope.OnNavigatableWidgetsChanged, new Action<GamepadNavigationScope>(this.OnScopeNavigatableWidgetsChanged));
				GamepadNavigationScope gamepadNavigationScope2 = this._allScopes[i];
				gamepadNavigationScope2.OnVisibilityChanged = (Action<GamepadNavigationScope, bool>)Delegate.Remove(gamepadNavigationScope2.OnVisibilityChanged, new Action<GamepadNavigationScope, bool>(this.OnScopeVisibilityChanged));
			}
			this._allScopes.Clear();
			this._uninitializedScopes.Clear();
			this._invisibleScopes.Clear();
			this._visibleScopes.Clear();
		}

		private Action<GamepadNavigationScope> _onScopeNavigatableWidgetsChanged;

		private Action<GamepadNavigationScope, bool> _onScopeVisibilityChanged;

		private List<GamepadNavigationScope> _allScopes;

		private List<GamepadNavigationScope> _uninitializedScopes;

		private List<GamepadNavigationScope> _visibleScopes;

		private List<GamepadNavigationScope> _invisibleScopes;

		private List<GamepadNavigationScope> _dirtyScopes;
	}
}
