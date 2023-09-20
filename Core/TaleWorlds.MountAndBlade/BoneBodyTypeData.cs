using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001C5 RID: 453
	[EngineStruct("Bone_body_type_data")]
	public struct BoneBodyTypeData
	{
		// Token: 0x0400082A RID: 2090
		public readonly BoneBodyPartType BodyPartType;

		// Token: 0x0400082B RID: 2091
		public readonly sbyte Priority;

		// Token: 0x0400082C RID: 2092
		public readonly SkeletonModelBoundsRecFlags DataFlags;
	}
}
