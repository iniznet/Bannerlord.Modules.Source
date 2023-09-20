using System;

namespace TaleWorlds.ObjectSystem
{
	// Token: 0x02000005 RID: 5
	public class MBIllegalRegisterException : ObjectSystemException
	{
		// Token: 0x06000015 RID: 21 RVA: 0x000021F6 File Offset: 0x000003F6
		internal MBIllegalRegisterException()
			: base("A registered Object exists with same name.")
		{
		}
	}
}
