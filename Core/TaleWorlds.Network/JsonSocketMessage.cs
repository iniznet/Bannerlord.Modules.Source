using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace TaleWorlds.Network
{
	// Token: 0x0200000A RID: 10
	public class JsonSocketMessage
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00002706 File Offset: 0x00000906
		// (set) Token: 0x06000037 RID: 55 RVA: 0x0000270E File Offset: 0x0000090E
		public MessageInfo MessageInfo { get; private set; }

		// Token: 0x06000038 RID: 56 RVA: 0x00002718 File Offset: 0x00000918
		[Obsolete]
		public JsonSocketMessage()
		{
			this.MessageInfo = new MessageInfo();
			Attribute[] customAttributes = Attribute.GetCustomAttributes(base.GetType(), true);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				PostBoxId postBoxId = customAttributes[i] as PostBoxId;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000039 RID: 57 RVA: 0x0000275A File Offset: 0x0000095A
		[JsonProperty]
		public string SocketMessageTypeId
		{
			get
			{
				return base.GetType().FullName;
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002767 File Offset: 0x00000967
		public static string GetTypeId(Type messageType)
		{
			return messageType.FullName;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002770 File Offset: 0x00000970
		public static Dictionary<string, Type> GetMessageDictionary()
		{
			Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				foreach (KeyValuePair<string, Type> keyValuePair in JsonSocketMessage.RetrieveJSONSocketMessages(assemblies[i]))
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return dictionary;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000027F4 File Offset: 0x000009F4
		private static Dictionary<string, Type> RetrieveJSONSocketMessages(Assembly assembly)
		{
			IEnumerable<Type> exportedTypes = assembly.GetExportedTypes();
			Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
			foreach (Type type in exportedTypes.Where((Type q) => q.IsSubclassOf(typeof(JsonSocketMessage))))
			{
				dictionary.Add(type.FullName, type);
			}
			return dictionary;
		}
	}
}
