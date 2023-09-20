using System;
using System.Diagnostics;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x02000078 RID: 120
	[EngineClass("rglResource")]
	public abstract class Resource : NativeObject
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060008C2 RID: 2242 RVA: 0x00008E2E File Offset: 0x0000702E
		public bool IsValid
		{
			get
			{
				return base.Pointer != UIntPtr.Zero;
			}
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x00008E40 File Offset: 0x00007040
		protected Resource()
		{
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x00008E48 File Offset: 0x00007048
		internal Resource(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x00008E57 File Offset: 0x00007057
		[Conditional("_RGL_KEEP_ASSERTS")]
		protected void CheckResourceParameter(Resource param, string paramName = "")
		{
			if (param == null)
			{
				throw new NullReferenceException(paramName);
			}
			if (!param.IsValid)
			{
				throw new ArgumentException(paramName);
			}
		}
	}
}
