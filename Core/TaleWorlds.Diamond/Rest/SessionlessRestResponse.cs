using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	[DataContract]
	[Serializable]
	public sealed class SessionlessRestResponse : RestData
	{
		[DataMember]
		public bool Successful { get; private set; }

		[DataMember]
		public string SuccessfulReason { get; private set; }

		[DataMember]
		public RestFunctionResult FunctionResult { get; set; }

		public void SetSuccessful(bool successful, string succressfulReason)
		{
			this.Successful = successful;
			this.SuccessfulReason = succressfulReason;
		}
	}
}
