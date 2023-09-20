using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Load
{
	// Token: 0x02000034 RID: 52
	internal class ElementLoadData : VariableLoadData
	{
		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060001E3 RID: 483 RVA: 0x00008D9C File Offset: 0x00006F9C
		// (set) Token: 0x060001E4 RID: 484 RVA: 0x00008DA4 File Offset: 0x00006FA4
		public ContainerLoadData ContainerLoadData { get; private set; }

		// Token: 0x060001E5 RID: 485 RVA: 0x00008DAD File Offset: 0x00006FAD
		internal ElementLoadData(ContainerLoadData containerLoadData, IReader reader)
			: base(containerLoadData.Context, reader)
		{
			this.ContainerLoadData = containerLoadData;
		}
	}
}
