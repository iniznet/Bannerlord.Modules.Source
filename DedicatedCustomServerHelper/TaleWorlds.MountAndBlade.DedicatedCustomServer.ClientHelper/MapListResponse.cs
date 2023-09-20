using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TaleWorlds.MountAndBlade.DedicatedCustomServer.ClientHelper
{
	[Serializable]
	public class MapListResponse
	{
		public string CurrentlyPlaying { get; private set; }

		public List<MapListItemResponse> Maps { get; private set; }

		[JsonConstructor]
		public MapListResponse(string currentlyPlaying, List<MapListItemResponse> maps)
		{
			this.CurrentlyPlaying = currentlyPlaying;
			this.Maps = maps;
		}
	}
}
