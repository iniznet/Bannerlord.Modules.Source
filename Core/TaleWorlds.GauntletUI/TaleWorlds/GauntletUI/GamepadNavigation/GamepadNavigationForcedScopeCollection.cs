using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.GamepadNavigation
{
	// Token: 0x02000041 RID: 65
	public class GamepadNavigationForcedScopeCollection
	{
		// Token: 0x17000123 RID: 291
		// (get) Token: 0x060003C3 RID: 963 RVA: 0x000108DB File Offset: 0x0000EADB
		// (set) Token: 0x060003C4 RID: 964 RVA: 0x000108E3 File Offset: 0x0000EAE3
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
					Action<GamepadNavigationForcedScopeCollection> onAvailabilityChanged = this.OnAvailabilityChanged;
					if (onAvailabilityChanged == null)
					{
						return;
					}
					onAvailabilityChanged(this);
				}
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x060003C5 RID: 965 RVA: 0x00010906 File Offset: 0x0000EB06
		// (set) Token: 0x060003C6 RID: 966 RVA: 0x00010911 File Offset: 0x0000EB11
		public bool IsDisabled
		{
			get
			{
				return !this.IsEnabled;
			}
			set
			{
				if (value == this.IsEnabled)
				{
					this.IsEnabled = !value;
				}
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x060003C7 RID: 967 RVA: 0x00010926 File Offset: 0x0000EB26
		// (set) Token: 0x060003C8 RID: 968 RVA: 0x0001092E File Offset: 0x0000EB2E
		public string CollectionID { get; set; }

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x060003C9 RID: 969 RVA: 0x00010937 File Offset: 0x0000EB37
		// (set) Token: 0x060003CA RID: 970 RVA: 0x0001093F File Offset: 0x0000EB3F
		public int CollectionOrder { get; set; }

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x060003CB RID: 971 RVA: 0x00010948 File Offset: 0x0000EB48
		// (set) Token: 0x060003CC RID: 972 RVA: 0x00010950 File Offset: 0x0000EB50
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

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x060003CD RID: 973 RVA: 0x000109DE File Offset: 0x0000EBDE
		// (set) Token: 0x060003CE RID: 974 RVA: 0x000109E6 File Offset: 0x0000EBE6
		public List<GamepadNavigationScope> Scopes { get; private set; }

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x060003CF RID: 975 RVA: 0x000109EF File Offset: 0x0000EBEF
		// (set) Token: 0x060003D0 RID: 976 RVA: 0x000109F7 File Offset: 0x0000EBF7
		public GamepadNavigationScope ActiveScope { get; set; }

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x060003D1 RID: 977 RVA: 0x00010A00 File Offset: 0x0000EC00
		// (set) Token: 0x060003D2 RID: 978 RVA: 0x00010A08 File Offset: 0x0000EC08
		public GamepadNavigationScope PreviousScope { get; set; }

		// Token: 0x060003D3 RID: 979 RVA: 0x00010A11 File Offset: 0x0000EC11
		public GamepadNavigationForcedScopeCollection()
		{
			this.Scopes = new List<GamepadNavigationScope>();
			this._invisibleParents = new List<Widget>();
			this.IsEnabled = true;
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x00010A38 File Offset: 0x0000EC38
		private void OnParentVisibilityChanged(Widget parent)
		{
			bool flag = this._invisibleParents.Count == 0;
			if (!parent.IsVisible)
			{
				this._invisibleParents.Add(parent);
			}
			else
			{
				this._invisibleParents.Remove(parent);
			}
			bool flag2 = this._invisibleParents.Count == 0;
			if (flag != flag2)
			{
				Action<GamepadNavigationForcedScopeCollection> onAvailabilityChanged = this.OnAvailabilityChanged;
				if (onAvailabilityChanged == null)
				{
					return;
				}
				onAvailabilityChanged(this);
			}
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00010A9C File Offset: 0x0000EC9C
		public bool IsAvailable()
		{
			if (this.IsEnabled && this._invisibleParents.Count == 0)
			{
				if (this.Scopes.Any((GamepadNavigationScope x) => x.IsAvailable()))
				{
					return this.ParentWidget.EventManager.IsAvailableForNavigation();
				}
			}
			return false;
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x00010AFC File Offset: 0x0000ECFC
		public void AddScope(GamepadNavigationScope scope)
		{
			if (!this.Scopes.Contains(scope))
			{
				this.Scopes.Add(scope);
			}
			Action<GamepadNavigationForcedScopeCollection> onAvailabilityChanged = this.OnAvailabilityChanged;
			if (onAvailabilityChanged == null)
			{
				return;
			}
			onAvailabilityChanged(this);
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x00010B29 File Offset: 0x0000ED29
		public void RemoveScope(GamepadNavigationScope scope)
		{
			if (this.Scopes.Contains(scope))
			{
				this.Scopes.Remove(scope);
			}
			Action<GamepadNavigationForcedScopeCollection> onAvailabilityChanged = this.OnAvailabilityChanged;
			if (onAvailabilityChanged == null)
			{
				return;
			}
			onAvailabilityChanged(this);
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x00010B57 File Offset: 0x0000ED57
		public void ClearScopes()
		{
			this.Scopes.Clear();
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x00010B64 File Offset: 0x0000ED64
		public override string ToString()
		{
			return string.Format("ID:{0} C.C.:{1}", this.CollectionID, this.Scopes.Count);
		}

		// Token: 0x040001F1 RID: 497
		public Action<GamepadNavigationForcedScopeCollection> OnAvailabilityChanged;

		// Token: 0x040001F2 RID: 498
		private List<Widget> _invisibleParents;

		// Token: 0x040001F3 RID: 499
		private bool _isEnabled;

		// Token: 0x040001F6 RID: 502
		private Widget _parentWidget;
	}
}
