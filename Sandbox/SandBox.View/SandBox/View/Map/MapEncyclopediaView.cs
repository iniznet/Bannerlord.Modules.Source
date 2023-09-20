using System;

namespace SandBox.View.Map
{
	// Token: 0x02000046 RID: 70
	public class MapEncyclopediaView : MapView
	{
		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600024F RID: 591 RVA: 0x00015859 File Offset: 0x00013A59
		// (set) Token: 0x06000250 RID: 592 RVA: 0x00015861 File Offset: 0x00013A61
		public bool IsEncyclopediaOpen { get; protected set; }

		// Token: 0x06000251 RID: 593 RVA: 0x0001586A File Offset: 0x00013A6A
		public virtual void CloseEncyclopedia()
		{
		}
	}
}
