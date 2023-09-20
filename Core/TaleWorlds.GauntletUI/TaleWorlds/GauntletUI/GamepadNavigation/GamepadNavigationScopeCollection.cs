using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.GamepadNavigation
{
	// Token: 0x02000044 RID: 68
	internal class GamepadNavigationScopeCollection
	{
		// Token: 0x1700015B RID: 347
		// (get) Token: 0x0600046B RID: 1131 RVA: 0x00012AD5 File Offset: 0x00010CD5
		// (set) Token: 0x0600046C RID: 1132 RVA: 0x00012ADD File Offset: 0x00010CDD
		public EventManager Source { get; private set; }

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x0600046D RID: 1133 RVA: 0x00012AE6 File Offset: 0x00010CE6
		// (set) Token: 0x0600046E RID: 1134 RVA: 0x00012AEE File Offset: 0x00010CEE
		public ReadOnlyCollection<GamepadNavigationScope> AllScopes { get; private set; }

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x0600046F RID: 1135 RVA: 0x00012AF7 File Offset: 0x00010CF7
		// (set) Token: 0x06000470 RID: 1136 RVA: 0x00012AFF File Offset: 0x00010CFF
		public ReadOnlyCollection<GamepadNavigationScope> UninitializedScopes { get; private set; }

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000471 RID: 1137 RVA: 0x00012B08 File Offset: 0x00010D08
		// (set) Token: 0x06000472 RID: 1138 RVA: 0x00012B10 File Offset: 0x00010D10
		public ReadOnlyCollection<GamepadNavigationScope> VisibleScopes { get; private set; }

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06000473 RID: 1139 RVA: 0x00012B19 File Offset: 0x00010D19
		// (set) Token: 0x06000474 RID: 1140 RVA: 0x00012B21 File Offset: 0x00010D21
		public ReadOnlyCollection<GamepadNavigationScope> InvisibleScopes { get; private set; }

		// Token: 0x06000475 RID: 1141 RVA: 0x00012B2C File Offset: 0x00010D2C
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

		// Token: 0x06000476 RID: 1142 RVA: 0x00012BCF File Offset: 0x00010DCF
		internal void OnFinalize()
		{
			this.ClearAllScopes();
			this._onScopeVisibilityChanged = null;
			this._onScopeNavigatableWidgetsChanged = null;
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x00012BE8 File Offset: 0x00010DE8
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

		// Token: 0x06000478 RID: 1144 RVA: 0x00012D40 File Offset: 0x00010F40
		private void OnScopeVisibilityChanged(GamepadNavigationScope scope, bool isVisible)
		{
			List<GamepadNavigationScope> dirtyScopes = this._dirtyScopes;
			lock (dirtyScopes)
			{
				this._dirtyScopes.Add(scope);
			}
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x00012D88 File Offset: 0x00010F88
		private void OnScopeNavigatableWidgetsChanged(GamepadNavigationScope scope)
		{
			this._onScopeNavigatableWidgetsChanged(scope);
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x00012D96 File Offset: 0x00010F96
		internal int GetTotalNumberOfScopes()
		{
			return this._visibleScopes.Count + this._invisibleScopes.Count + this._uninitializedScopes.Count;
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x00012DBB File Offset: 0x00010FBB
		internal void AddScope(GamepadNavigationScope scope)
		{
			this._uninitializedScopes.Add(scope);
			this._allScopes.Add(scope);
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x00012DD8 File Offset: 0x00010FD8
		internal void RemoveScope(GamepadNavigationScope scope)
		{
			this._allScopes.Remove(scope);
			this._uninitializedScopes.Remove(scope);
			this._visibleScopes.Remove(scope);
			this._invisibleScopes.Remove(scope);
			scope.OnVisibilityChanged = (Action<GamepadNavigationScope, bool>)Delegate.Remove(scope.OnVisibilityChanged, new Action<GamepadNavigationScope, bool>(this.OnScopeVisibilityChanged));
			scope.OnNavigatableWidgetsChanged = (Action<GamepadNavigationScope>)Delegate.Remove(scope.OnNavigatableWidgetsChanged, new Action<GamepadNavigationScope>(this.OnScopeNavigatableWidgetsChanged));
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x00012E5D File Offset: 0x0001105D
		internal bool HasScopeInAnyList(GamepadNavigationScope scope)
		{
			return this._visibleScopes.Contains(scope) || this._invisibleScopes.Contains(scope) || this._uninitializedScopes.Contains(scope);
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x00012E8C File Offset: 0x0001108C
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

		// Token: 0x0600047F RID: 1151 RVA: 0x00012F0C File Offset: 0x0001110C
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

		// Token: 0x06000480 RID: 1152 RVA: 0x00012FD4 File Offset: 0x000111D4
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

		// Token: 0x04000229 RID: 553
		private Action<GamepadNavigationScope> _onScopeNavigatableWidgetsChanged;

		// Token: 0x0400022A RID: 554
		private Action<GamepadNavigationScope, bool> _onScopeVisibilityChanged;

		// Token: 0x0400022B RID: 555
		private List<GamepadNavigationScope> _allScopes;

		// Token: 0x0400022C RID: 556
		private List<GamepadNavigationScope> _uninitializedScopes;

		// Token: 0x0400022D RID: 557
		private List<GamepadNavigationScope> _visibleScopes;

		// Token: 0x0400022E RID: 558
		private List<GamepadNavigationScope> _invisibleScopes;

		// Token: 0x0400022F RID: 559
		private List<GamepadNavigationScope> _dirtyScopes;
	}
}
