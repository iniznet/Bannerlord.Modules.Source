using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.GameMenu
{
	// Token: 0x02000133 RID: 307
	public class SettlementMenuPartyCharacterListsButtonWidget : ButtonWidget
	{
		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x0600104C RID: 4172 RVA: 0x0002DEE9 File Offset: 0x0002C0E9
		// (set) Token: 0x0600104D RID: 4173 RVA: 0x0002DEF1 File Offset: 0x0002C0F1
		public Brush PartyListButtonBrush { get; set; }

		// Token: 0x170005C4 RID: 1476
		// (get) Token: 0x0600104E RID: 4174 RVA: 0x0002DEFA File Offset: 0x0002C0FA
		// (set) Token: 0x0600104F RID: 4175 RVA: 0x0002DF02 File Offset: 0x0002C102
		public Brush CharacterListButtonBrush { get; set; }

		// Token: 0x170005C5 RID: 1477
		// (get) Token: 0x06001050 RID: 4176 RVA: 0x0002DF0B File Offset: 0x0002C10B
		// (set) Token: 0x06001051 RID: 4177 RVA: 0x0002DF13 File Offset: 0x0002C113
		public ContainerPageControlWidget CharactersList { get; set; }

		// Token: 0x170005C6 RID: 1478
		// (get) Token: 0x06001052 RID: 4178 RVA: 0x0002DF1C File Offset: 0x0002C11C
		// (set) Token: 0x06001053 RID: 4179 RVA: 0x0002DF24 File Offset: 0x0002C124
		public ContainerPageControlWidget PartiesList { get; set; }

		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x06001054 RID: 4180 RVA: 0x0002DF2D File Offset: 0x0002C12D
		// (set) Token: 0x06001055 RID: 4181 RVA: 0x0002DF35 File Offset: 0x0002C135
		public int MaxNumOfVisuals { get; set; } = 5;

		// Token: 0x06001056 RID: 4182 RVA: 0x0002DF3E File Offset: 0x0002C13E
		public SettlementMenuPartyCharacterListsButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06001057 RID: 4183 RVA: 0x0002DF50 File Offset: 0x0002C150
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			base.Brush = (this.ChildPartiesList.IsVisible ? this.PartyListButtonBrush : (this.ChildCharactersList.IsVisible ? this.CharacterListButtonBrush : null));
			if (!this._initialized)
			{
				if (this.CharactersList.IsVisible)
				{
					this.SetCharacterListVisible();
				}
				else if (this.PartiesList.IsVisible)
				{
					this.SetPartyListVisible();
				}
				this._initialized = true;
			}
		}

		// Token: 0x06001058 RID: 4184 RVA: 0x0002DFCC File Offset: 0x0002C1CC
		protected override void OnClick()
		{
			base.OnClick();
			if (!this.PartiesList.IsVisible && this.CharactersList.IsVisible)
			{
				this.SetPartyListVisible();
				return;
			}
			if (this.PartiesList.IsVisible && !this.CharactersList.IsVisible)
			{
				this.SetCharacterListVisible();
			}
		}

		// Token: 0x06001059 RID: 4185 RVA: 0x0002E020 File Offset: 0x0002C220
		private void SetCharacterListVisible()
		{
			this.CharactersList.IsVisible = true;
			this.PartiesList.IsVisible = false;
			this.ChildPartiesList.IsVisible = true;
			this.ChildCharactersList.IsVisible = false;
		}

		// Token: 0x0600105A RID: 4186 RVA: 0x0002E052 File Offset: 0x0002C252
		private void SetPartyListVisible()
		{
			this.CharactersList.IsVisible = false;
			this.PartiesList.IsVisible = true;
			this.ChildPartiesList.IsVisible = false;
			this.ChildCharactersList.IsVisible = true;
		}

		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x0600105B RID: 4187 RVA: 0x0002E084 File Offset: 0x0002C284
		// (set) Token: 0x0600105C RID: 4188 RVA: 0x0002E08C File Offset: 0x0002C28C
		public ListPanel ChildCharactersList
		{
			get
			{
				return this._childCharactersList;
			}
			set
			{
				if (value != this._childCharactersList)
				{
					this._childCharactersList = value;
					this._childCharactersList.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnListItemAdded));
				}
			}
		}

		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x0600105D RID: 4189 RVA: 0x0002E0BA File Offset: 0x0002C2BA
		// (set) Token: 0x0600105E RID: 4190 RVA: 0x0002E0C2 File Offset: 0x0002C2C2
		public ListPanel ChildPartiesList
		{
			get
			{
				return this._childPartiesList;
			}
			set
			{
				if (value != this._childPartiesList)
				{
					this._childPartiesList = value;
					this._childPartiesList.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnListItemAdded));
				}
			}
		}

		// Token: 0x0600105F RID: 4191 RVA: 0x0002E0F0 File Offset: 0x0002C2F0
		private void OnListItemAdded(Widget parent, Widget child)
		{
			if (parent.ChildCount > this.MaxNumOfVisuals)
			{
				child.IsVisible = false;
			}
		}

		// Token: 0x0400078A RID: 1930
		private bool _initialized;

		// Token: 0x0400078B RID: 1931
		private ListPanel _childCharactersList;

		// Token: 0x0400078C RID: 1932
		private ListPanel _childPartiesList;
	}
}
