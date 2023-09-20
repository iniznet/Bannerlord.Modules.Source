using System;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	// Token: 0x020003AD RID: 941
	public class MultiplayerMangonelSpawner : MangonelSpawner
	{
		// Token: 0x060032E2 RID: 13026 RVA: 0x000D27FC File Offset: 0x000D09FC
		protected internal override void OnPreInit()
		{
			this._spawnerMissionHelper = new SpawnerEntityMissionHelper(this, false);
		}
	}
}
