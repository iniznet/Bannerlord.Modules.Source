using System;
using System.Runtime.CompilerServices;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.PlatformService
{
	public delegate void PermissionResults([TupleElementNames(new string[] { "PlayerId", "Permission", "HasPermission" })] ValueTuple<PlayerId, Permission, bool>[] results);
}
