using System;
using System.Collections.Generic;
using System.Globalization;

namespace TaleWorlds.Library
{
	// Token: 0x0200006C RID: 108
	public class ParameterContainer
	{
		// Token: 0x060003A8 RID: 936 RVA: 0x0000B7C8 File Offset: 0x000099C8
		public ParameterContainer()
		{
			this._parameters = new Dictionary<string, string>();
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x0000B7DB File Offset: 0x000099DB
		public void AddParameter(string key, string value, bool overwriteIfExists)
		{
			if (this._parameters.ContainsKey(key))
			{
				if (overwriteIfExists)
				{
					this._parameters[key] = value;
					return;
				}
			}
			else
			{
				this._parameters.Add(key, value);
			}
		}

		// Token: 0x060003AA RID: 938 RVA: 0x0000B80C File Offset: 0x00009A0C
		public void AddParameterConcurrent(string key, string value, bool overwriteIfExists)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(this._parameters);
			if (dictionary.ContainsKey(key))
			{
				if (overwriteIfExists)
				{
					dictionary[key] = value;
				}
			}
			else
			{
				dictionary.Add(key, value);
			}
			this._parameters = dictionary;
		}

		// Token: 0x060003AB RID: 939 RVA: 0x0000B84C File Offset: 0x00009A4C
		public void AddParametersConcurrent(IEnumerable<KeyValuePair<string, string>> parameters, bool overwriteIfExists)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(this._parameters);
			foreach (KeyValuePair<string, string> keyValuePair in parameters)
			{
				if (dictionary.ContainsKey(keyValuePair.Key))
				{
					if (overwriteIfExists)
					{
						dictionary[keyValuePair.Key] = keyValuePair.Value;
					}
				}
				else
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			this._parameters = dictionary;
		}

		// Token: 0x060003AC RID: 940 RVA: 0x0000B8DC File Offset: 0x00009ADC
		public void ClearParameters()
		{
			this._parameters = new Dictionary<string, string>();
		}

		// Token: 0x060003AD RID: 941 RVA: 0x0000B8E9 File Offset: 0x00009AE9
		public bool TryGetParameter(string key, out string outValue)
		{
			return this._parameters.TryGetValue(key, out outValue);
		}

		// Token: 0x060003AE RID: 942 RVA: 0x0000B8F8 File Offset: 0x00009AF8
		public bool TryGetParameterAsBool(string key, out bool outValue)
		{
			outValue = false;
			string text;
			if (this.TryGetParameter(key, out text))
			{
				outValue = text == "true" || text == "True";
				return true;
			}
			return false;
		}

		// Token: 0x060003AF RID: 943 RVA: 0x0000B934 File Offset: 0x00009B34
		public bool TryGetParameterAsInt(string key, out int outValue)
		{
			outValue = 0;
			string text;
			if (this.TryGetParameter(key, out text))
			{
				outValue = Convert.ToInt32(text);
				return true;
			}
			return false;
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x0000B95C File Offset: 0x00009B5C
		public bool TryGetParameterAsUInt16(string key, out ushort outValue)
		{
			outValue = 0;
			string text;
			if (this.TryGetParameter(key, out text))
			{
				outValue = Convert.ToUInt16(text);
				return true;
			}
			return false;
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x0000B984 File Offset: 0x00009B84
		public bool TryGetParameterAsFloat(string key, out float outValue)
		{
			outValue = 0f;
			string text;
			if (this.TryGetParameter(key, out text))
			{
				outValue = Convert.ToSingle(text, CultureInfo.InvariantCulture);
				return true;
			}
			return false;
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x0000B9B4 File Offset: 0x00009BB4
		public bool TryGetParameterAsByte(string key, out byte outValue)
		{
			outValue = 0;
			string text;
			if (this.TryGetParameter(key, out text))
			{
				outValue = Convert.ToByte(text);
				return true;
			}
			return false;
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x0000B9DC File Offset: 0x00009BDC
		public bool TryGetParameterAsSByte(string key, out sbyte outValue)
		{
			outValue = 0;
			string text;
			if (this.TryGetParameter(key, out text))
			{
				outValue = Convert.ToSByte(text);
				return true;
			}
			return false;
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x0000BA04 File Offset: 0x00009C04
		public bool TryGetParameterAsVec3(string key, out Vec3 outValue)
		{
			outValue = default(Vec3);
			string text;
			if (this.TryGetParameter(key, out text))
			{
				string[] array = text.Split(new char[] { ';' });
				float num = Convert.ToSingle(array[0], CultureInfo.InvariantCulture);
				float num2 = Convert.ToSingle(array[1], CultureInfo.InvariantCulture);
				float num3 = Convert.ToSingle(array[2], CultureInfo.InvariantCulture);
				outValue = new Vec3(num, num2, num3, -1f);
				return true;
			}
			return false;
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x0000BA74 File Offset: 0x00009C74
		public bool TryGetParameterAsVec2(string key, out Vec2 outValue)
		{
			outValue = default(Vec2);
			string text;
			if (this.TryGetParameter(key, out text))
			{
				string[] array = text.Split(new char[] { ';' });
				float num = Convert.ToSingle(array[0], CultureInfo.InvariantCulture);
				float num2 = Convert.ToSingle(array[1], CultureInfo.InvariantCulture);
				outValue = new Vec2(num, num2);
				return true;
			}
			return false;
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x0000BACF File Offset: 0x00009CCF
		public string GetParameter(string key)
		{
			return this._parameters[key];
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060003B7 RID: 951 RVA: 0x0000BADD File Offset: 0x00009CDD
		public IEnumerable<KeyValuePair<string, string>> Iterator
		{
			get
			{
				return this._parameters;
			}
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x0000BAE8 File Offset: 0x00009CE8
		public ParameterContainer Clone()
		{
			ParameterContainer parameterContainer = new ParameterContainer();
			foreach (KeyValuePair<string, string> keyValuePair in this._parameters)
			{
				parameterContainer._parameters.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return parameterContainer;
		}

		// Token: 0x04000119 RID: 281
		private Dictionary<string, string> _parameters;
	}
}
