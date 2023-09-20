using System;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000020 RID: 32
	public class ManagedDelegate : DotNetObject
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x00003D68 File Offset: 0x00001F68
		// (set) Token: 0x060000AA RID: 170 RVA: 0x00003D70 File Offset: 0x00001F70
		public ManagedDelegate.DelegateDefinition Instance
		{
			get
			{
				return this._instance;
			}
			set
			{
				this._instance = value;
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00003D81 File Offset: 0x00001F81
		[LibraryCallback]
		public void InvokeAux()
		{
			this.Instance();
		}

		// Token: 0x0400003D RID: 61
		private ManagedDelegate.DelegateDefinition _instance;

		// Token: 0x0200003B RID: 59
		// (Invoke) Token: 0x06000145 RID: 325
		public delegate void DelegateDefinition();
	}
}
