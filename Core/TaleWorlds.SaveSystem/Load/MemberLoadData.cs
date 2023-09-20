using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Load
{
	// Token: 0x0200003A RID: 58
	internal abstract class MemberLoadData : VariableLoadData
	{
		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600020C RID: 524 RVA: 0x00009913 File Offset: 0x00007B13
		// (set) Token: 0x0600020D RID: 525 RVA: 0x0000991B File Offset: 0x00007B1B
		public ObjectLoadData ObjectLoadData { get; private set; }

		// Token: 0x0600020E RID: 526 RVA: 0x00009924 File Offset: 0x00007B24
		protected MemberLoadData(ObjectLoadData objectLoadData, IReader reader)
			: base(objectLoadData.Context, reader)
		{
			this.ObjectLoadData = objectLoadData;
		}
	}
}
