using System;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	// Token: 0x020003A8 RID: 936
	public class MultiplayerBallistaSpawner : BallistaSpawner
	{
		// Token: 0x060032D8 RID: 13016 RVA: 0x000D273F File Offset: 0x000D093F
		protected internal override void OnPreInit()
		{
			this._spawnerMissionHelper = new SpawnerEntityMissionHelper(this, false);
		}
	}
}
