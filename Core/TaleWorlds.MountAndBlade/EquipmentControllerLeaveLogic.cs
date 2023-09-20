using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200026A RID: 618
	public class EquipmentControllerLeaveLogic : MissionLogic
	{
		// Token: 0x17000654 RID: 1620
		// (get) Token: 0x06002101 RID: 8449 RVA: 0x00076D4B File Offset: 0x00074F4B
		// (set) Token: 0x06002102 RID: 8450 RVA: 0x00076D53 File Offset: 0x00074F53
		public bool IsEquipmentSelectionActive { get; private set; }

		// Token: 0x06002103 RID: 8451 RVA: 0x00076D5C File Offset: 0x00074F5C
		public void SetIsEquipmentSelectionActive(bool isActive)
		{
			this.IsEquipmentSelectionActive = isActive;
			Debug.Print("IsEquipmentSelectionActive: " + isActive.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x06002104 RID: 8452 RVA: 0x00076D87 File Offset: 0x00074F87
		public override InquiryData OnEndMissionRequest(out bool canLeave)
		{
			canLeave = !this.IsEquipmentSelectionActive;
			return null;
		}
	}
}
