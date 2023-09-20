using System;

namespace TaleWorlds.Network
{
	// Token: 0x02000010 RID: 16
	public class ServiceExceptionModel
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000055 RID: 85 RVA: 0x000029F2 File Offset: 0x00000BF2
		// (set) Token: 0x06000056 RID: 86 RVA: 0x000029FA File Offset: 0x00000BFA
		public string ExceptionMessage { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00002A03 File Offset: 0x00000C03
		// (set) Token: 0x06000058 RID: 88 RVA: 0x00002A0B File Offset: 0x00000C0B
		public string ExceptionType { get; set; }

		// Token: 0x06000059 RID: 89 RVA: 0x00002A14 File Offset: 0x00000C14
		public ServiceException ToServiceException()
		{
			return new ServiceException(this.ExceptionType, this.ExceptionMessage);
		}
	}
}
