using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	[DataContract]
	[Serializable]
	public class RestObjectFunctionResult : RestFunctionResult
	{
		public override FunctionResult GetFunctionResult()
		{
			return this._functionResult;
		}

		public RestObjectFunctionResult()
		{
		}

		public RestObjectFunctionResult(FunctionResult functionResult)
		{
			this._functionResult = functionResult;
		}

		[DataMember]
		private FunctionResult _functionResult;
	}
}
