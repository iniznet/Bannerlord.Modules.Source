using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class EquipmentControllerLeaveLogic : MissionLogic
	{
		public bool IsEquipmentSelectionActive { get; private set; }

		public void SetIsEquipmentSelectionActive(bool isActive)
		{
			this.IsEquipmentSelectionActive = isActive;
			Debug.Print("IsEquipmentSelectionActive: " + isActive.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
		}

		public override InquiryData OnEndMissionRequest(out bool canLeave)
		{
			canLeave = !this.IsEquipmentSelectionActive;
			return null;
		}
	}
}
