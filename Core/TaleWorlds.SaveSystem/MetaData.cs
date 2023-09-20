using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x02000012 RID: 18
	public class MetaData
	{
		// Token: 0x06000050 RID: 80 RVA: 0x00002BE9 File Offset: 0x00000DE9
		public void Add(string key, string value)
		{
			this._list.Add(key, value);
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00002BF8 File Offset: 0x00000DF8
		[JsonIgnore]
		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002C05 File Offset: 0x00000E05
		public bool TryGetValue(string key, out string value)
		{
			return this._list.TryGetValue(key, out value);
		}

		// Token: 0x1700000E RID: 14
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

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000055 RID: 85 RVA: 0x00002C41 File Offset: 0x00000E41
		[JsonIgnore]
		public Dictionary<string, string>.KeyCollection Keys
		{
			get
			{
				return this._list.Keys;
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002C50 File Offset: 0x00000E50
		public void Serialize(Stream stream)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
			stream.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
			stream.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00002C8C File Offset: 0x00000E8C
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

		// Token: 0x04000019 RID: 25
		[JsonProperty("List")]
		private Dictionary<string, string> _list = new Dictionary<string, string>();
	}
}
