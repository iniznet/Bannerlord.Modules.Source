using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TaleWorlds.Diamond
{
	public class AccessObjectJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(AccessObject).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jobject = JObject.Load(reader);
			string text = (string)jobject["Type"];
			AccessObject accessObject;
			if (text == "Steam")
			{
				accessObject = new SteamAccessObject();
			}
			else if (text == "Epic")
			{
				accessObject = new EpicAccessObject();
			}
			else if (text == "GOG")
			{
				accessObject = new GOGAccessObject();
			}
			else if (text == "GDK")
			{
				accessObject = new GDKAccessObject();
			}
			else if (text == "PS")
			{
				accessObject = new PSAccessObject();
			}
			else
			{
				if (!(text == "Test"))
				{
					return null;
				}
				accessObject = new TestAccessObject();
			}
			serializer.Populate(jobject.CreateReader(), accessObject);
			return accessObject;
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
		}
	}
}
