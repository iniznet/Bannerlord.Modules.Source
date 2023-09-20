using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	[DataContract]
	[Serializable]
	public abstract class RestRequestMessage : RestData
	{
		[DataMember]
		public byte[] UserCertificate { get; set; }
	}
}
