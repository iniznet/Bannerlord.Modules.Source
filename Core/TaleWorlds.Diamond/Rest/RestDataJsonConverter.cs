using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TaleWorlds.Diamond.Rest
{
	public class RestDataJsonConverter : JsonConverter<RestData>
	{
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		private RestData Create(Type objectType, JObject jObject)
		{
			if (jObject == null)
			{
				throw new ArgumentNullException("jObject");
			}
			string text = null;
			if (jObject["TypeName"] != null)
			{
				text = Extensions.Value<string>(jObject["TypeName"]);
			}
			else if (jObject["typeName"] != null)
			{
				text = Extensions.Value<string>(jObject["typeName"]);
			}
			if (text != null)
			{
				return Activator.CreateInstance(Type.GetType(text)) as RestData;
			}
			return null;
		}

		public T ReadJson<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json);
		}

		public override RestData ReadJson(JsonReader reader, Type objectType, RestData existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			if (reader.TokenType == 11)
			{
				return null;
			}
			JObject jobject = JObject.Load(reader);
			RestData restData = this.Create(objectType, jobject);
			serializer.Populate(jobject.CreateReader(), restData);
			return restData;
		}

		public override void WriteJson(JsonWriter writer, RestData value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
