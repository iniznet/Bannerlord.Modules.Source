using System;
using Newtonsoft.Json;

namespace TaleWorlds.Diamond
{
	[JsonConverter(typeof(AccessObjectJsonConverter))]
	[Serializable]
	public abstract class AccessObject
	{
		public string Type { get; set; }
	}
}
