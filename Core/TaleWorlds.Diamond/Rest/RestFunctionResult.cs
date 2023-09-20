using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x02000033 RID: 51
	[DataContract]
	[Serializable]
	public abstract class RestFunctionResult : RestData
	{
		// Token: 0x060000FF RID: 255
		public abstract FunctionResult GetFunctionResult();
	}
}
