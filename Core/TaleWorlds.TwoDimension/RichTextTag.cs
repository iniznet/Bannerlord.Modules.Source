using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	public class RichTextTag
	{
		public string Name { get; private set; }

		public RichTextTagType Type { get; set; }

		public RichTextTag(string name)
		{
			this.Name = name;
			this._attributes = new Dictionary<string, string>();
		}

		public void AddAtrribute(string key, string value)
		{
			this._attributes.Add(key, value);
		}

		public string GetAttribute(string key)
		{
			if (this._attributes.ContainsKey(key))
			{
				return this._attributes[key];
			}
			return "";
		}

		private Dictionary<string, string> _attributes;
	}
}
