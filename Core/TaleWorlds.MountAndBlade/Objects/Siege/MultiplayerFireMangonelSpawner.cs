using System;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	// Token: 0x020003AB RID: 939
	public class MultiplayerFireMangonelSpawner : MangonelSpawner
	{
		// Token: 0x060032DE RID: 13022 RVA: 0x000D27CE File Offset: 0x000D09CE
		protected internal override void OnPreInit()
		{
			this._spawnerMissionHelperFire = new SpawnerEntityMissionHelper(this, true);
		}
	}
}
