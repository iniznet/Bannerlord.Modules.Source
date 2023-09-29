using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TaleWorlds.Library
{
	public class ApplicationVersionJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(ApplicationVersion).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return ApplicationVersion.FromString((string)JObject.Load(reader)["_version"], 27066);
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
			JProperty jproperty = new JProperty("_version", ((ApplicationVersion)value).ToString());
			new JObject { jproperty }.WriteTo(writer, Array.Empty<JsonConverter>());
		}
	}
}
