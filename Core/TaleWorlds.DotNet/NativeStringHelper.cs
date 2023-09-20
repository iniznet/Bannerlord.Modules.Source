using System;

namespace TaleWorlds.DotNet
{
	// Token: 0x0200002A RID: 42
	internal static class NativeStringHelper
	{
		// Token: 0x06000106 RID: 262 RVA: 0x0000516E File Offset: 0x0000336E
		internal static UIntPtr CreateRglVarString(string text)
		{
			return LibraryApplicationInterface.INativeStringHelper.CreateRglVarString(text);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000517B File Offset: 0x0000337B
		internal static UIntPtr GetThreadLocalCachedRglVarString()
		{
			return LibraryApplicationInterface.INativeStringHelper.GetThreadLocalCachedRglVarString();
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00005187 File Offset: 0x00003387
		internal static void SetRglVarString(UIntPtr pointer, string text)
		{
			LibraryApplicationInterface.INativeStringHelper.SetRglVarString(pointer, text);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00005195 File Offset: 0x00003395
		internal static void DeleteRglVarString(UIntPtr pointer)
		{
			LibraryApplicationInterface.INativeStringHelper.DeleteRglVarString(pointer);
		}
	}
}
