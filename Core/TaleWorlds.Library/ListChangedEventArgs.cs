using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000056 RID: 86
	public class ListChangedEventArgs : EventArgs
	{
		// Token: 0x0600026F RID: 623 RVA: 0x00006BE5 File Offset: 0x00004DE5
		public ListChangedEventArgs(ListChangedType listChangedType, int newIndex)
		{
			this.ListChangedType = listChangedType;
			this.NewIndex = newIndex;
			this.OldIndex = -1;
		}

		// Token: 0x06000270 RID: 624 RVA: 0x00006C02 File Offset: 0x00004E02
		public ListChangedEventArgs(ListChangedType listChangedType, int newIndex, int oldIndex)
		{
			this.ListChangedType = listChangedType;
			this.NewIndex = newIndex;
			this.OldIndex = oldIndex;
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000271 RID: 625 RVA: 0x00006C1F File Offset: 0x00004E1F
		// (set) Token: 0x06000272 RID: 626 RVA: 0x00006C27 File Offset: 0x00004E27
		public ListChangedType ListChangedType { get; private set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000273 RID: 627 RVA: 0x00006C30 File Offset: 0x00004E30
		// (set) Token: 0x06000274 RID: 628 RVA: 0x00006C38 File Offset: 0x00004E38
		public int NewIndex { get; private set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000275 RID: 629 RVA: 0x00006C41 File Offset: 0x00004E41
		// (set) Token: 0x06000276 RID: 630 RVA: 0x00006C49 File Offset: 0x00004E49
		public int OldIndex { get; private set; }
	}
}
