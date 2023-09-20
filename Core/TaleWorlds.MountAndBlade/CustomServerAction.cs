using System;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200022F RID: 559
	public class CustomServerAction
	{
		// Token: 0x17000617 RID: 1559
		// (get) Token: 0x06001EC4 RID: 7876 RVA: 0x0006E407 File Offset: 0x0006C607
		// (set) Token: 0x06001EC5 RID: 7877 RVA: 0x0006E40F File Offset: 0x0006C60F
		public Action Execute { get; private set; }

		// Token: 0x17000618 RID: 1560
		// (get) Token: 0x06001EC6 RID: 7878 RVA: 0x0006E418 File Offset: 0x0006C618
		// (set) Token: 0x06001EC7 RID: 7879 RVA: 0x0006E420 File Offset: 0x0006C620
		public GameServerEntry GameServerEntry { get; private set; }

		// Token: 0x17000619 RID: 1561
		// (get) Token: 0x06001EC8 RID: 7880 RVA: 0x0006E429 File Offset: 0x0006C629
		// (set) Token: 0x06001EC9 RID: 7881 RVA: 0x0006E431 File Offset: 0x0006C631
		public string Name { get; private set; }

		// Token: 0x06001ECA RID: 7882 RVA: 0x0006E43A File Offset: 0x0006C63A
		public CustomServerAction(Action execute, GameServerEntry gameServerEntry, string name)
		{
			this.Execute = execute;
			this.GameServerEntry = gameServerEntry;
			this.Name = name;
		}
	}
}
