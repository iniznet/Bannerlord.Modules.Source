using System;

namespace TaleWorlds.Network
{
	// Token: 0x02000011 RID: 17
	public class ServiceException : Exception
	{
		// Token: 0x0600005B RID: 91 RVA: 0x00002A2F File Offset: 0x00000C2F
		public ServiceException(string type, string message)
			: base("ServiceException")
		{
			this.ExceptionType = type;
			this.ExceptionMessage = message;
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00002A4A File Offset: 0x00000C4A
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00002A52 File Offset: 0x00000C52
		public string ExceptionMessage { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00002A5B File Offset: 0x00000C5B
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00002A63 File Offset: 0x00000C63
		public string ExceptionType { get; set; }
	}
}
