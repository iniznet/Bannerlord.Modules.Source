using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TaleWorlds.Core
{
	public class BodyPropertiesJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(BodyProperties).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			BodyProperties bodyProperties;
			BodyProperties.FromString((string)JObject.Load(reader)["_data"], out bodyProperties);
			return bodyProperties;
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
			JProperty jproperty = new JProperty("_data", ((BodyProperties)value).ToString());
			new JObject { jproperty }.WriteTo(writer, Array.Empty<JsonConverter>());
		}
	}
}
