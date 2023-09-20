using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	public class PartyUpgradesContainerWidget : Widget
	{
		public PartyUpgradesContainerWidget(UIContext context)
			: base(context)
		{
		}

		private void OnAnyUpgradeHasRequirementChanged(bool value)
		{
			base.ScaledPositionYOffset = (value ? 0f : 8f);
		}

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

		private const float _noRequirementOffset = 8f;

		private bool _anyUpgradeHasRequirement = true;
	}
}
