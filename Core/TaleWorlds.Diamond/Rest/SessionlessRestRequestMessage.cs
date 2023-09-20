using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	[DataContract]
	[Serializable]
	public abstract class SessionlessRestRequestMessage : RestData
	{
	}
}
