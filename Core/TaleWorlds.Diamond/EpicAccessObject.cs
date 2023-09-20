using System;
using Newtonsoft.Json;

namespace TaleWorlds.Diamond
{
	[Serializable]
	public class EpicAccessObject : AccessObject
	{
		public EpicAccessObject()
		{
			base.Type = "Epic";
		}

		[JsonProperty]
		public string AccessToken;

		[JsonProperty]
		public string EpicId;
	}
}
