using System;
using System.Collections.Generic;
using System.Globalization;

namespace TaleWorlds.Library
{
	public class ParameterContainer
	{
		public ParameterContainer()
		{
			this._parameters = new Dictionary<string, string>();
		}

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

		public void ClearParameters()
		{
			this._parameters = new Dictionary<string, string>();
		}

		public bool TryGetParameter(string key, out string outValue)
		{
			return this._parameters.TryGetValue(key, out outValue);
		}

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

		public string GetParameter(string key)
		{
			return this._parameters[key];
		}

		public IEnumerable<KeyValuePair<string, string>> Iterator
		{
			get
			{
				return this._parameters;
			}
		}

		public ParameterContainer Clone()
		{
			ParameterContainer parameterContainer = new ParameterContainer();
			foreach (KeyValuePair<string, string> keyValuePair in this._parameters)
			{
				parameterContainer._parameters.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return parameterContainer;
		}

		private Dictionary<string, string> _parameters;
	}
}
