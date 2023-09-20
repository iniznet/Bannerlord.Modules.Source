using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x02000031 RID: 49
	public class RestDataJsonConverter : JsonConverter<RestData>
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x000037AE File Offset: 0x000019AE
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x000037B1 File Offset: 0x000019B1
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x000037B4 File Offset: 0x000019B4
		private RestData Create(Type objectType, JObject jObject)
		{
			if (jObject == null)
			{
				throw new ArgumentNullException("jObject");
			}
			string text = null;
			if (jObject["TypeName"] != null)
			{
				text = jObject["TypeName"].Value<string>();
			}
			else if (jObject["typeName"] != null)
			{
				text = jObject["typeName"].Value<string>();
			}
			if (text != null)
			{
				return Activator.CreateInstance(Type.GetType(text)) as RestData;
			}
			return null;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00003824 File Offset: 0x00001A24
		public T ReadJson<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json);
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x0000382C File Offset: 0x00001A2C
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
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			JObject jobject = JObject.Load(reader);
			RestData restData = this.Create(objectType, jobject);
			serializer.Populate(jobject.CreateReader(), restData);
			return restData;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00003881 File Offset: 0x00001A81
		public override void WriteJson(JsonWriter writer, RestData value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
