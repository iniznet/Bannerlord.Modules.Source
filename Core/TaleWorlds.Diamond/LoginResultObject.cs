using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace TaleWorlds.Diamond
{
	[DataContract]
	[JsonConverter(typeof(LoginResultObjectJsonConverter))]
	[Serializable]
	public abstract class LoginResultObject
	{
	}
}
