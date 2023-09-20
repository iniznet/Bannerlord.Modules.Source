using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x02000036 RID: 54
	[DataContract]
	[Serializable]
	public abstract class RestResponseMessage : RestData
	{
		// Token: 0x06000107 RID: 263
		public abstract Message GetMessage();
	}
}
