using System;
using System.Runtime.CompilerServices;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService
{
	// Token: 0x02000009 RID: 9
	// (Invoke) Token: 0x06000025 RID: 37
	public delegate void PermissionResults([TupleElementNames(new string[] { "PlayerId", "Permission", "HasPermission" })] ValueTuple<PlayerId, Permission, bool>[] results);
}
