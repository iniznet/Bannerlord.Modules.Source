using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x02000034 RID: 52
	[DataContract]
	[Serializable]
	public class RestObjectFunctionResult : RestFunctionResult
	{
		// Token: 0x06000101 RID: 257 RVA: 0x000038D7 File Offset: 0x00001AD7
		public override FunctionResult GetFunctionResult()
		{
			return this._functionResult;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x000038DF File Offset: 0x00001ADF
		public RestObjectFunctionResult()
		{
		}

		// Token: 0x06000103 RID: 259 RVA: 0x000038E7 File Offset: 0x00001AE7
		public RestObjectFunctionResult(FunctionResult functionResult)
		{
			this._functionResult = functionResult;
		}

		// Token: 0x04000044 RID: 68
		[DataMember]
		private FunctionResult _functionResult;
	}
}
