using System;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000009 RID: 9
	internal class CustomParameter<T> : DotNetObject
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600001E RID: 30 RVA: 0x00002788 File Offset: 0x00000988
		// (set) Token: 0x0600001F RID: 31 RVA: 0x00002790 File Offset: 0x00000990
		public T Target { get; set; }

		// Token: 0x06000020 RID: 32 RVA: 0x00002799 File Offset: 0x00000999
		public CustomParameter(T target)
		{
			this.Target = target;
		}
	}
}
