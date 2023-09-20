using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace TaleWorlds.SaveSystem
{
	public class MetaData
	{
		public void Add(string key, string value)
		{
			this._list.Add(key, value);
		}

		[JsonIgnore]
		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		public bool TryGetValue(string key, out string value)
		{
			return this._list.TryGetValue(key, out value);
		}

		public string this[string key]
		{
			get
			{
				if (!this._list.ContainsKey(key))
				{
					return null;
				}
				return this._list[key];
			}
			set
			{
				this._list[key] = value;
			}
		}

		[JsonIgnore]
		public Dictionary<string, string>.KeyCollection Keys
		{
			get
			{
				return this._list.Keys;
			}
		}

		public void Serialize(Stream stream)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
			stream.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
			stream.Write(bytes, 0, bytes.Length);
		}

		public static MetaData Deserialize(Stream stream)
		{
			MetaData metaData;
			try
			{
				byte[] array = new byte[4];
				stream.Read(array, 0, 4);
				int num = BitConverter.ToInt32(array, 0);
				byte[] array2 = new byte[num];
				stream.Read(array2, 0, num);
				metaData = JsonConvert.DeserializeObject<MetaData>(Encoding.UTF8.GetString(array2));
			}
			catch
			{
				metaData = null;
			}
			return metaData;
		}

		[JsonProperty("List")]
		private Dictionary<string, string> _list = new Dictionary<string, string>();
	}
}
