using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TaleWorlds.PlayerServices
{
	public class PlayerIdJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(PlayerId).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return PlayerId.FromString((string)JObject.Load(reader)["_playerId"]);
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
			JProperty jproperty = new JProperty("_playerId", ((PlayerId)value).ToString());
			new JObject { jproperty }.WriteTo(writer, Array.Empty<JsonConverter>());
		}
	}
}
