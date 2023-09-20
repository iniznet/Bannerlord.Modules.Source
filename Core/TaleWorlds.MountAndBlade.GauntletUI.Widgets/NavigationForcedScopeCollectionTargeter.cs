using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class NavigationForcedScopeCollectionTargeter : Widget
	{
		public NavigationForcedScopeCollectionTargeter(UIContext context)
			: base(context)
		{
			this._collection = new GamepadNavigationForcedScopeCollection();
			base.WidthSizePolicy = SizePolicy.Fixed;
			base.HeightSizePolicy = SizePolicy.Fixed;
			base.SuggestedHeight = 0f;
			base.SuggestedWidth = 0f;
			base.IsVisible = false;
		}

		public bool IsCollectionEnabled
		{
			get
			{
				return this._collection.IsEnabled;
			}
			set
			{
				if (value != this._collection.IsEnabled)
				{
					this._collection.IsEnabled = value;
				}
			}
		}

		public bool IsCollectionDisabled
		{
			get
			{
				return this._collection.IsDisabled;
			}
			set
			{
				if (value != this._collection.IsDisabled)
				{
					this._collection.IsDisabled = value;
				}
			}
		}

		public string CollectionID
		{
			get
			{
				return this._collection.CollectionID;
			}
			set
			{
				if (value != this._collection.CollectionID)
				{
					this._collection.CollectionID = value;
				}
			}
		}

		public int CollectionOrder
		{
			get
			{
				return this._collection.CollectionOrder;
			}
			set
			{
				if (value != this._collection.CollectionOrder)
				{
					this._collection.CollectionOrder = value;
				}
			}
		}

		public Widget CollectionParent
		{
			get
			{
				return this._collection.ParentWidget;
			}
			set
			{
				if (this._collection.ParentWidget != value)
				{
					if (this._collection.ParentWidget != null)
					{
						base.EventManager.RemoveForcedScopeCollection(this._collection);
					}
					this._collection.ParentWidget = value;
					if (value != null)
					{
						base.EventManager.AddForcedScopeCollection(this._collection);
					}
				}
			}
		}

		private GamepadNavigationForcedScopeCollection _collection;
	}
}
