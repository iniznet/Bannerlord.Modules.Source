using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001CC RID: 460
	public class MBUnusedResourceManager
	{
		// Token: 0x06001A24 RID: 6692 RVA: 0x0005C557 File Offset: 0x0005A757
		public static void SetMeshUsed(string meshName)
		{
			MBAPI.IMBWorld.SetMeshUsed(meshName);
		}

		// Token: 0x06001A25 RID: 6693 RVA: 0x0005C564 File Offset: 0x0005A764
		public static void SetMaterialUsed(string meshName)
		{
			MBAPI.IMBWorld.SetMaterialUsed(meshName);
		}

		// Token: 0x06001A26 RID: 6694 RVA: 0x0005C571 File Offset: 0x0005A771
		public static void SetBodyUsed(string bodyName)
		{
			MBAPI.IMBWorld.SetBodyUsed(bodyName);
		}
	}
}
