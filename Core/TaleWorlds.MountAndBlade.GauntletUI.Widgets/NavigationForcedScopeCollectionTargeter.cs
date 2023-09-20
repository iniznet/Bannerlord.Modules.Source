using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200002A RID: 42
	public class NavigationForcedScopeCollectionTargeter : Widget
	{
		// Token: 0x06000226 RID: 550 RVA: 0x00007D80 File Offset: 0x00005F80
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

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000227 RID: 551 RVA: 0x00007DBF File Offset: 0x00005FBF
		// (set) Token: 0x06000228 RID: 552 RVA: 0x00007DCC File Offset: 0x00005FCC
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

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000229 RID: 553 RVA: 0x00007DE8 File Offset: 0x00005FE8
		// (set) Token: 0x0600022A RID: 554 RVA: 0x00007DF5 File Offset: 0x00005FF5
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

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x0600022B RID: 555 RVA: 0x00007E11 File Offset: 0x00006011
		// (set) Token: 0x0600022C RID: 556 RVA: 0x00007E1E File Offset: 0x0000601E
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

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x0600022D RID: 557 RVA: 0x00007E3F File Offset: 0x0000603F
		// (set) Token: 0x0600022E RID: 558 RVA: 0x00007E4C File Offset: 0x0000604C
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

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x0600022F RID: 559 RVA: 0x00007E68 File Offset: 0x00006068
		// (set) Token: 0x06000230 RID: 560 RVA: 0x00007E78 File Offset: 0x00006078
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

		// Token: 0x040000FE RID: 254
		private GamepadNavigationForcedScopeCollection _collection;
	}
}
