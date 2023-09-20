using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200032D RID: 813
	[AttributeUsage(AttributeTargets.Method)]
	public class ConsoleCommandMethod : Attribute
	{
		// Token: 0x170007E8 RID: 2024
		// (get) Token: 0x06002BF3 RID: 11251 RVA: 0x000AA8E8 File Offset: 0x000A8AE8
		// (set) Token: 0x06002BF4 RID: 11252 RVA: 0x000AA8F0 File Offset: 0x000A8AF0
		public string CommandName { get; private set; }

		// Token: 0x170007E9 RID: 2025
		// (get) Token: 0x06002BF5 RID: 11253 RVA: 0x000AA8F9 File Offset: 0x000A8AF9
		// (set) Token: 0x06002BF6 RID: 11254 RVA: 0x000AA901 File Offset: 0x000A8B01
		public string Description { get; private set; }

		// Token: 0x06002BF7 RID: 11255 RVA: 0x000AA90A File Offset: 0x000A8B0A
		public ConsoleCommandMethod(string commandName, string description)
		{
			this.CommandName = commandName;
			this.Description = description;
		}
	}
}
