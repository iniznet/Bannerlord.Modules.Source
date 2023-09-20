using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x0200000E RID: 14
	public class RichTextTag
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00004736 File Offset: 0x00002936
		// (set) Token: 0x0600007D RID: 125 RVA: 0x0000473E File Offset: 0x0000293E
		public string Name { get; private set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00004747 File Offset: 0x00002947
		// (set) Token: 0x0600007F RID: 127 RVA: 0x0000474F File Offset: 0x0000294F
		public RichTextTagType Type { get; set; }

		// Token: 0x06000080 RID: 128 RVA: 0x00004758 File Offset: 0x00002958
		public RichTextTag(string name)
		{
			this.Name = name;
			this._attributes = new Dictionary<string, string>();
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00004772 File Offset: 0x00002972
		public void AddAtrribute(string key, string value)
		{
			this._attributes.Add(key, value);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00004781 File Offset: 0x00002981
		public string GetAttribute(string key)
		{
			if (this._attributes.ContainsKey(key))
			{
				return this._attributes[key];
			}
			return "";
		}

		// Token: 0x04000053 RID: 83
		private Dictionary<string, string> _attributes;
	}
}
