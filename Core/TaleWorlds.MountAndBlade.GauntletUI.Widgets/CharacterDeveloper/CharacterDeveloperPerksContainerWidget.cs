using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	// Token: 0x0200015D RID: 349
	public class CharacterDeveloperPerksContainerWidget : Widget
	{
		// Token: 0x060011F8 RID: 4600 RVA: 0x000319E9 File Offset: 0x0002FBE9
		public CharacterDeveloperPerksContainerWidget(UIContext context)
			: base(context)
		{
			this._perkWidgets = new List<PerkItemButtonWidget>();
			this._navigationScopes = new List<GamepadNavigationScope>();
		}

		// Token: 0x060011F9 RID: 4601 RVA: 0x00031A10 File Offset: 0x0002FC10
		private void RefreshScopes()
		{
			foreach (GamepadNavigationScope gamepadNavigationScope in this._navigationScopes)
			{
				base.EventManager.RemoveNavigationScope(gamepadNavigationScope);
			}
			this._navigationScopes.Clear();
			GamepadNavigationScope gamepadNavigationScope2 = this.BuildNewScope(this.FirstScopeID);
			this._navigationScopes.Add(gamepadNavigationScope2);
			base.EventManager.AddNavigationScope(gamepadNavigationScope2, true);
			int num = -1;
			if (this._perkWidgets.Count > 0)
			{
				num = this._perkWidgets[0].AlternativeType;
			}
			for (int i = 0; i < this._perkWidgets.Count; i++)
			{
				if (this._perkWidgets[i].AlternativeType == 0 || num == 0)
				{
					GamepadNavigationScope gamepadNavigationScope3 = this.BuildNewScope("Scope-" + i);
					this._navigationScopes.Add(gamepadNavigationScope3);
					base.EventManager.AddNavigationScope(gamepadNavigationScope3, true);
				}
				this._perkWidgets[i].GamepadNavigationIndex = 0;
				this._navigationScopes[this._navigationScopes.Count - 1].AddWidget(this._perkWidgets[i]);
				num = this._perkWidgets[i].AlternativeType;
			}
			for (int j = 0; j < this._navigationScopes.Count; j++)
			{
				List<Widget> list = this._navigationScopes[j].NavigatableWidgets.ToList<Widget>();
				list = list.OrderBy((Widget w) => ((PerkItemButtonWidget)w).AlternativeType).ToList<Widget>();
				this._navigationScopes[j].ClearNavigatableWidgets();
				for (int k = 0; k < list.Count; k++)
				{
					list[k].GamepadNavigationIndex = k;
					this._navigationScopes[j].AddWidget(list[k]);
				}
				if (this._navigationScopes[j].NavigatableWidgets.Count > 1)
				{
					this._navigationScopes[j].AlternateMovementStepSize = MathF.Round((float)this._navigationScopes[j].NavigatableWidgets.Count / 2f);
					this._navigationScopes[j].AlternateScopeMovements = GamepadNavigationTypes.Vertical;
				}
				this._navigationScopes[j].DownNavigationScopeID = this.DownScopeID;
				this._navigationScopes[j].UpNavigationScopeID = this.UpScopeID;
				if (j == 0)
				{
					this._navigationScopes[j].LeftNavigationScopeID = this.LeftScopeID;
					if (this._navigationScopes.Count > 1)
					{
						this._navigationScopes[j].RightNavigationScopeID = this._navigationScopes[j + 1].ScopeID;
					}
				}
				else if (j == this._navigationScopes.Count - 1)
				{
					if (this._navigationScopes.Count > 1)
					{
						this._navigationScopes[j].LeftNavigationScopeID = this._navigationScopes[j - 1].ScopeID;
					}
					this._navigationScopes[j].RightNavigationScopeID = this.RightScopeID;
				}
				else if (j > 0 && j < this._navigationScopes.Count - 1)
				{
					this._navigationScopes[j].LeftNavigationScopeID = this._navigationScopes[j - 1].ScopeID;
					this._navigationScopes[j].RightNavigationScopeID = this._navigationScopes[j + 1].ScopeID;
				}
			}
		}

		// Token: 0x060011FA RID: 4602 RVA: 0x00031DDC File Offset: 0x0002FFDC
		protected override void OnLateUpdate(float dt)
		{
			if (!this._initialized || this._lastPerkCount != this._perkWidgets.Count)
			{
				this.RefreshScopes();
				this._initialized = true;
				this._lastPerkCount = this._perkWidgets.Count;
			}
		}

		// Token: 0x060011FB RID: 4603 RVA: 0x00031E17 File Offset: 0x00030017
		private GamepadNavigationScope BuildNewScope(string scopeID)
		{
			return new GamepadNavigationScope
			{
				ScopeID = scopeID,
				ParentWidget = this,
				ScopeMovements = GamepadNavigationTypes.Horizontal,
				DoNotAutomaticallyFindChildren = true
			};
		}

		// Token: 0x060011FC RID: 4604 RVA: 0x00031E3C File Offset: 0x0003003C
		protected override void OnChildAdded(Widget child)
		{
			PerkItemButtonWidget perkItemButtonWidget;
			if ((perkItemButtonWidget = child as PerkItemButtonWidget) != null)
			{
				this._perkWidgets.Add(perkItemButtonWidget);
				this._initialized = false;
			}
		}

		// Token: 0x060011FD RID: 4605 RVA: 0x00031E68 File Offset: 0x00030068
		protected override void OnChildRemoved(Widget child)
		{
			PerkItemButtonWidget perkItemButtonWidget;
			if ((perkItemButtonWidget = child as PerkItemButtonWidget) != null)
			{
				this._perkWidgets.Remove(perkItemButtonWidget);
				this._initialized = false;
			}
		}

		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x060011FE RID: 4606 RVA: 0x00031E93 File Offset: 0x00030093
		// (set) Token: 0x060011FF RID: 4607 RVA: 0x00031E9B File Offset: 0x0003009B
		public string LeftScopeID { get; set; }

		// Token: 0x17000658 RID: 1624
		// (get) Token: 0x06001200 RID: 4608 RVA: 0x00031EA4 File Offset: 0x000300A4
		// (set) Token: 0x06001201 RID: 4609 RVA: 0x00031EAC File Offset: 0x000300AC
		public string RightScopeID { get; set; }

		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x06001202 RID: 4610 RVA: 0x00031EB5 File Offset: 0x000300B5
		// (set) Token: 0x06001203 RID: 4611 RVA: 0x00031EBD File Offset: 0x000300BD
		public string DownScopeID { get; set; }

		// Token: 0x1700065A RID: 1626
		// (get) Token: 0x06001204 RID: 4612 RVA: 0x00031EC6 File Offset: 0x000300C6
		// (set) Token: 0x06001205 RID: 4613 RVA: 0x00031ECE File Offset: 0x000300CE
		public string UpScopeID { get; set; }

		// Token: 0x1700065B RID: 1627
		// (get) Token: 0x06001206 RID: 4614 RVA: 0x00031ED7 File Offset: 0x000300D7
		// (set) Token: 0x06001207 RID: 4615 RVA: 0x00031EDF File Offset: 0x000300DF
		public string FirstScopeID { get; set; }

		// Token: 0x0400083A RID: 2106
		private List<GamepadNavigationScope> _navigationScopes;

		// Token: 0x0400083B RID: 2107
		private List<PerkItemButtonWidget> _perkWidgets;

		// Token: 0x0400083C RID: 2108
		private bool _initialized;

		// Token: 0x0400083D RID: 2109
		private int _lastPerkCount = -1;
	}
}
