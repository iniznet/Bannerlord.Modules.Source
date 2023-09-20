using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Core
{
	// Token: 0x02000098 RID: 152
	public class MBException : ApplicationException
	{
		// Token: 0x060007F7 RID: 2039 RVA: 0x0001B763 File Offset: 0x00019963
		public MBException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x0001B76D File Offset: 0x0001996D
		public MBException(string message)
			: base(message)
		{
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x0001B776 File Offset: 0x00019976
		public MBException()
		{
		}

		// Token: 0x060007FA RID: 2042 RVA: 0x0001B77E File Offset: 0x0001997E
		public MBException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
