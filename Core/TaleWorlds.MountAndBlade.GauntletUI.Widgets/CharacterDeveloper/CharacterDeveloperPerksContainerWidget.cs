using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.GamepadNavigation;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	public class CharacterDeveloperPerksContainerWidget : Widget
	{
		public CharacterDeveloperPerksContainerWidget(UIContext context)
			: base(context)
		{
			this._perkWidgets = new List<PerkItemButtonWidget>();
			this._navigationScopes = new List<GamepadNavigationScope>();
		}

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

		protected override void OnLateUpdate(float dt)
		{
			if (!this._initialized || this._lastPerkCount != this._perkWidgets.Count)
			{
				this.RefreshScopes();
				this._initialized = true;
				this._lastPerkCount = this._perkWidgets.Count;
			}
		}

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

		protected override void OnChildAdded(Widget child)
		{
			PerkItemButtonWidget perkItemButtonWidget;
			if ((perkItemButtonWidget = child as PerkItemButtonWidget) != null)
			{
				this._perkWidgets.Add(perkItemButtonWidget);
				this._initialized = false;
			}
		}

		protected override void OnChildRemoved(Widget child)
		{
			PerkItemButtonWidget perkItemButtonWidget;
			if ((perkItemButtonWidget = child as PerkItemButtonWidget) != null)
			{
				this._perkWidgets.Remove(perkItemButtonWidget);
				this._initialized = false;
			}
		}

		public string LeftScopeID { get; set; }

		public string RightScopeID { get; set; }

		public string DownScopeID { get; set; }

		public string UpScopeID { get; set; }

		public string FirstScopeID { get; set; }

		private List<GamepadNavigationScope> _navigationScopes;

		private List<PerkItemButtonWidget> _perkWidgets;

		private bool _initialized;

		private int _lastPerkCount = -1;
	}
}
