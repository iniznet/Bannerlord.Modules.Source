using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TaleWorlds.Diamond
{
	public class LoginResultObjectJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(LoginResultObject).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jobject = JObject.Load(reader);
			string text = (string)jobject["_type"];
			Type type;
			if (LoginResultObjectJsonConverter._knownTypes.TryGetValue(text, out type))
			{
				LoginResultObject loginResultObject = (LoginResultObject)Activator.CreateInstance(type);
				serializer.Populate(jobject.CreateReader(), loginResultObject);
				return loginResultObject;
			}
			return null;
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
			JProperty jproperty = new JProperty("_type", value.GetType().FullName);
			JObject jobject = new JObject();
			jobject.Add(jproperty);
			foreach (PropertyInfo propertyInfo in value.GetType().GetProperties())
			{
				if (propertyInfo.CanRead)
				{
					object value2 = propertyInfo.GetValue(value);
					if (value2 != null)
					{
						jobject.Add(propertyInfo.Name, JToken.FromObject(value2, serializer));
					}
				}
			}
			jobject.WriteTo(writer, Array.Empty<JsonConverter>());
		}

		private static readonly Dictionary<string, Type> _knownTypes = (from t in (from a in AppDomain.CurrentDomain.GetAssemblies()
				where !a.GlobalAssemblyCache
				select a).SelectMany((Assembly a) => a.GetTypes())
			where t.IsSubclassOf(typeof(LoginResultObject))
			select t).ToDictionary((Type item) => item.FullName, (Type item) => item);
	}
}
