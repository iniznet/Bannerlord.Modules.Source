using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace TaleWorlds.Diamond
{
	[KnownType("GetKnownTypes")]
	[JsonConverter(typeof(FunctionResultJsonConverter))]
	[Serializable]
	public abstract class FunctionResult
	{
	}
}
