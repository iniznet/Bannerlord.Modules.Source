using System;

namespace TaleWorlds.LinQuick
{
	// Token: 0x02000004 RID: 4
	internal static class Error
	{
		// Token: 0x0600005E RID: 94 RVA: 0x00003715 File Offset: 0x00001915
		internal static Exception ArgumentNull(string s)
		{
			return new ArgumentNullException(s);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x0000371D File Offset: 0x0000191D
		internal static Exception ArgumentOutOfRange(string s)
		{
			return new ArgumentOutOfRangeException(s);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003725 File Offset: 0x00001925
		internal static Exception MoreThanOneElement()
		{
			return new InvalidOperationException("Sequence contains more than one element");
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003731 File Offset: 0x00001931
		internal static Exception MoreThanOneMatch()
		{
			return new InvalidOperationException("Sequence contains more than one matching element");
		}

		// Token: 0x06000062 RID: 98 RVA: 0x0000373D File Offset: 0x0000193D
		internal static Exception NoElements()
		{
			return new InvalidOperationException("Sequence contains no elements");
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003749 File Offset: 0x00001949
		internal static Exception NoMatch()
		{
			return new InvalidOperationException("Sequence contains no matching element");
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00003755 File Offset: 0x00001955
		internal static Exception NotSupported()
		{
			return new NotSupportedException();
		}
	}
}
