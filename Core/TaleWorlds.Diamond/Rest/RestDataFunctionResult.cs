using System;
using System.Runtime.Serialization;
using TaleWorlds.Library;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x02000030 RID: 48
	[DataContract]
	[Serializable]
	public class RestDataFunctionResult : RestFunctionResult
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000EE RID: 238 RVA: 0x0000376F File Offset: 0x0000196F
		// (set) Token: 0x060000EF RID: 239 RVA: 0x00003777 File Offset: 0x00001977
		[DataMember]
		public byte[] FunctionResultData { get; private set; }

		// Token: 0x060000F0 RID: 240 RVA: 0x00003780 File Offset: 0x00001980
		public override FunctionResult GetFunctionResult()
		{
			return (FunctionResult)Common.DeserializeObject(this.FunctionResultData);
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00003792 File Offset: 0x00001992
		public RestDataFunctionResult()
		{
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x0000379A File Offset: 0x0000199A
		public RestDataFunctionResult(FunctionResult functionResult)
		{
			this.FunctionResultData = Common.SerializeObject(functionResult);
		}
	}
}
