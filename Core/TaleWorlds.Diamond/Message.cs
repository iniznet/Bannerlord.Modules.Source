using System;
using Newtonsoft.Json;

namespace TaleWorlds.Diamond
{
	[JsonConverter(typeof(MessageJsonConverter))]
	[Serializable]
	public abstract class Message
	{
	}
}
