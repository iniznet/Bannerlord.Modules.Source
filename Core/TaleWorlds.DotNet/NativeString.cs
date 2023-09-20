using System;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000029 RID: 41
	[EngineClass("ftdnNative_string")]
	public sealed class NativeString : NativeObject
	{
		// Token: 0x06000102 RID: 258 RVA: 0x00005138 File Offset: 0x00003338
		internal NativeString(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00005147 File Offset: 0x00003347
		public static NativeString Create()
		{
			return LibraryApplicationInterface.INativeString.Create();
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00005153 File Offset: 0x00003353
		public string GetString()
		{
			return LibraryApplicationInterface.INativeString.GetString(this);
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00005160 File Offset: 0x00003360
		public void SetString(string newString)
		{
			LibraryApplicationInterface.INativeString.SetString(this, newString);
		}
	}
}
