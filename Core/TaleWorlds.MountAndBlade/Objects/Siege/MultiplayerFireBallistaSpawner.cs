using System;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	// Token: 0x020003AA RID: 938
	public class MultiplayerFireBallistaSpawner : BallistaSpawner
	{
		// Token: 0x060032DC RID: 13020 RVA: 0x000D27B7 File Offset: 0x000D09B7
		protected internal override void OnPreInit()
		{
			this._spawnerMissionHelperFire = new SpawnerEntityMissionHelper(this, true);
		}
	}
}
