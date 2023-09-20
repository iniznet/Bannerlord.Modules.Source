using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	// Token: 0x02000062 RID: 98
	public class PartyUpgradesContainerWidget : Widget
	{
		// Token: 0x06000538 RID: 1336 RVA: 0x0000FDAC File Offset: 0x0000DFAC
		public PartyUpgradesContainerWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x0000FDBC File Offset: 0x0000DFBC
		private void OnAnyUpgradeHasRequirementChanged(bool value)
		{
			base.ScaledPositionYOffset = (value ? 0f : 8f);
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x0600053A RID: 1338 RVA: 0x0000FDD3 File Offset: 0x0000DFD3
		// (set) Token: 0x0600053B RID: 1339 RVA: 0x0000FDDB File Offset: 0x0000DFDB
		[Editor(false)]
		public bool AnyUpgradeHasRequirement
		{
			get
			{
				return this._anyUpgradeHasRequirement;
			}
			set
			{
				if (this._anyUpgradeHasRequirement != value)
				{
					this._anyUpgradeHasRequirement = value;
					this.OnAnyUpgradeHasRequirementChanged(value);
					base.OnPropertyChanged(value, "AnyUpgradeHasRequirement");
				}
			}
		}

		// Token: 0x04000243 RID: 579
		private const float _noRequirementOffset = 8f;

		// Token: 0x04000244 RID: 580
		private bool _anyUpgradeHasRequirement = true;
	}
}
