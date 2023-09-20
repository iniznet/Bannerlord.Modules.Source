using System;
using System.Runtime.Serialization;
using TaleWorlds.Library;

namespace TaleWorlds.Diamond.Rest
{
	[DataContract]
	[Serializable]
	public class RestDataFunctionResult : RestFunctionResult
	{
		[DataMember]
		public byte[] FunctionResultData { get; private set; }

		public override FunctionResult GetFunctionResult()
		{
			return (FunctionResult)Common.DeserializeObject(this.FunctionResultData);
		}

		public RestDataFunctionResult()
		{
		}

		public RestDataFunctionResult(FunctionResult functionResult)
		{
			this.FunctionResultData = Common.SerializeObject(functionResult);
		}
	}
}
