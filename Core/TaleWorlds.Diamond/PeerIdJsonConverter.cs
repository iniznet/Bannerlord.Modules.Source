using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TaleWorlds.Diamond
{
	public class PeerIdJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(PeerId).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return PeerId.FromString((string)JObject.Load(reader)["_peerId"]);
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			JProperty jproperty = new JProperty("_peerId", ((PeerId)value).ToString());
			new JObject { jproperty }.WriteTo(writer, Array.Empty<JsonConverter>());
		}
	}
}
