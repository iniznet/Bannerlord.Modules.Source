using System;
using TaleWorlds.Library;

namespace TaleWorlds.ScreenSystem
{
	public class InputRestrictions
	{
		public int Order { get; private set; }

		public Guid Id { get; private set; }

		public bool MouseVisibility { get; private set; }

		public bool CanOverrideFocusOnHit { get; private set; }

		public InputUsageMask InputUsageMask { get; private set; }

		public InputRestrictions(int order)
		{
			this.Id = default(Guid);
			this.InputUsageMask = InputUsageMask.Invalid;
			this.Order = order;
		}

		public void SetMouseVisibility(bool isVisible)
		{
			this.MouseVisibility = isVisible;
		}

		public void SetInputRestrictions(bool isMouseVisible = true, InputUsageMask mask = InputUsageMask.All)
		{
			this.InputUsageMask = mask;
			this.SetMouseVisibility(isMouseVisible);
		}

		public void ResetInputRestrictions()
		{
			this.InputUsageMask = InputUsageMask.Invalid;
			this.SetMouseVisibility(false);
		}

		public void SetCanOverrideFocusOnHit(bool canOverrideFocusOnHit)
		{
			this.CanOverrideFocusOnHit = canOverrideFocusOnHit;
		}
	}
}
