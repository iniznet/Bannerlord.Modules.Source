using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000338 RID: 824
	[Obsolete]
	public class DestructedPrefabInfoMissionObject : MissionObject
	{
		// Token: 0x040010D8 RID: 4312
		public string DestructedPrefabName;

		// Token: 0x040010D9 RID: 4313
		public Vec3 Translate = new Vec3(0f, 0f, 0f, -1f);

		// Token: 0x040010DA RID: 4314
		public Vec3 Rotation = new Vec3(0f, 0f, 0f, -1f);

		// Token: 0x040010DB RID: 4315
		public Vec3 Scale = new Vec3(1f, 1f, 1f, -1f);
	}
}
