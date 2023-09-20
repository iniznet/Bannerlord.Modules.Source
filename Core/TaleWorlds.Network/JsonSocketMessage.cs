using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace TaleWorlds.Network
{
	public class JsonSocketMessage
	{
		public MessageInfo MessageInfo { get; private set; }

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

		[JsonProperty]
		public string SocketMessageTypeId
		{
			get
			{
				return base.GetType().FullName;
			}
		}

		public static string GetTypeId(Type messageType)
		{
			return messageType.FullName;
		}

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
