using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.GamepadNavigation
{
	public class GamepadNavigationForcedScopeCollection
	{
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

		public string CollectionID { get; set; }

		public int CollectionOrder { get; set; }

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

		public List<GamepadNavigationScope> Scopes { get; private set; }

		public GamepadNavigationScope ActiveScope { get; set; }

		public GamepadNavigationScope PreviousScope { get; set; }

		public GamepadNavigationForcedScopeCollection()
		{
			this.Scopes = new List<GamepadNavigationScope>();
			this._invisibleParents = new List<Widget>();
			this.IsEnabled = true;
		}

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

		public void ClearScopes()
		{
			this.Scopes.Clear();
		}

		public override string ToString()
		{
			return string.Format("ID:{0} C.C.:{1}", this.CollectionID, this.Scopes.Count);
		}

		public Action<GamepadNavigationForcedScopeCollection> OnAvailabilityChanged;

		private List<Widget> _invisibleParents;

		private bool _isEnabled;

		private Widget _parentWidget;
	}
}
