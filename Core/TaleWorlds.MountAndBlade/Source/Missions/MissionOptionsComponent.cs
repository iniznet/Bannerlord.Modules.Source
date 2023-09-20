using System;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
	// Token: 0x020003F6 RID: 1014
	public class MissionOptionsComponent : MissionLogic
	{
		// Token: 0x1400009C RID: 156
		// (add) Token: 0x060034DD RID: 13533 RVA: 0x000DC09C File Offset: 0x000DA29C
		// (remove) Token: 0x060034DE RID: 13534 RVA: 0x000DC0D4 File Offset: 0x000DA2D4
		public event OnMissionAddOptionsDelegate OnOptionsAdded;

		// Token: 0x060034DF RID: 13535 RVA: 0x000DC109 File Offset: 0x000DA309
		public void OnAddOptionsUIHandler()
		{
			if (this.OnOptionsAdded != null)
			{
				this.OnOptionsAdded();
			}
		}
	}
}
