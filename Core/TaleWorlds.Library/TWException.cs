using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Library
{
	// Token: 0x0200008C RID: 140
	public class TWException : ApplicationException
	{
		// Token: 0x060004DC RID: 1244 RVA: 0x0000FC4E File Offset: 0x0000DE4E
		public TWException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x0000FC58 File Offset: 0x0000DE58
		public TWException(string message)
			: base(message)
		{
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x0000FC61 File Offset: 0x0000DE61
		public TWException()
		{
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x0000FC69 File Offset: 0x0000DE69
		public TWException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
