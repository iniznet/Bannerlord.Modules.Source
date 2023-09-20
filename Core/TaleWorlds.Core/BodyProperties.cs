using System;
using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	[JsonConverter(typeof(BodyPropertiesJsonConverter))]
	[Serializable]
	public struct BodyProperties
	{
		public StaticBodyProperties StaticProperties
		{
			get
			{
				return this._staticBodyProperties;
			}
		}

		public DynamicBodyProperties DynamicProperties
		{
			get
			{
				return this._dynamicBodyProperties;
			}
		}

		public float Age
		{
			get
			{
				return this._dynamicBodyProperties.Age;
			}
		}

		public float Weight
		{
			get
			{
				return this._dynamicBodyProperties.Weight;
			}
		}

		public float Build
		{
			get
			{
				return this._dynamicBodyProperties.Build;
			}
		}

		public ulong KeyPart1
		{
			get
			{
				return this._staticBodyProperties.KeyPart1;
			}
		}

		public ulong KeyPart2
		{
			get
			{
				return this._staticBodyProperties.KeyPart2;
			}
		}

		public ulong KeyPart3
		{
			get
			{
				return this._staticBodyProperties.KeyPart3;
			}
		}

		public ulong KeyPart4
		{
			get
			{
				return this._staticBodyProperties.KeyPart4;
			}
		}

		public ulong KeyPart5
		{
			get
			{
				return this._staticBodyProperties.KeyPart5;
			}
		}

		public ulong KeyPart6
		{
			get
			{
				return this._staticBodyProperties.KeyPart6;
			}
		}

		public ulong KeyPart7
		{
			get
			{
				return this._staticBodyProperties.KeyPart7;
			}
		}

		public ulong KeyPart8
		{
			get
			{
				return this._staticBodyProperties.KeyPart8;
			}
		}

		public BodyProperties(DynamicBodyProperties dynamicBodyProperties, StaticBodyProperties staticBodyProperties)
		{
			this._dynamicBodyProperties = dynamicBodyProperties;
			this._staticBodyProperties = staticBodyProperties;
		}

		public static bool FromXmlNode(XmlNode node, out BodyProperties bodyProperties)
		{
			float num = 30f;
			float num2 = 0.5f;
			float num3 = 0.5f;
			if (node.Attributes["age"] != null)
			{
				float.TryParse(node.Attributes["age"].Value, out num);
			}
			if (node.Attributes["weight"] != null)
			{
				float.TryParse(node.Attributes["weight"].Value, out num2);
			}
			if (node.Attributes["build"] != null)
			{
				float.TryParse(node.Attributes["build"].Value, out num3);
			}
			DynamicBodyProperties dynamicBodyProperties = new DynamicBodyProperties(num, num2, num3);
			StaticBodyProperties staticBodyProperties;
			if (StaticBodyProperties.FromXmlNode(node, out staticBodyProperties))
			{
				bodyProperties = new BodyProperties(dynamicBodyProperties, staticBodyProperties);
				return true;
			}
			bodyProperties = default(BodyProperties);
			return false;
		}

		public static bool FromString(string keyValue, out BodyProperties bodyProperties)
		{
			if (keyValue.StartsWith("<BodyProperties ", StringComparison.InvariantCultureIgnoreCase) || keyValue.StartsWith("<BodyPropertiesMax ", StringComparison.InvariantCultureIgnoreCase))
			{
				XmlDocument xmlDocument = new XmlDocument();
				try
				{
					xmlDocument.LoadXml(keyValue);
				}
				catch (XmlException)
				{
					bodyProperties = default(BodyProperties);
					return false;
				}
				if (xmlDocument.FirstChild.Name.Equals("BodyProperties", StringComparison.InvariantCultureIgnoreCase) || xmlDocument.FirstChild.Name.Equals("BodyPropertiesMax", StringComparison.InvariantCultureIgnoreCase))
				{
					BodyProperties.FromXmlNode(xmlDocument.FirstChild, out bodyProperties);
					float num = 20f;
					float num2 = 0f;
					float num3 = 0f;
					if (xmlDocument.FirstChild.Attributes["age"] != null)
					{
						float.TryParse(xmlDocument.FirstChild.Attributes["age"].Value, out num);
					}
					if (xmlDocument.FirstChild.Attributes["weight"] != null)
					{
						float.TryParse(xmlDocument.FirstChild.Attributes["weight"].Value, out num2);
					}
					if (xmlDocument.FirstChild.Attributes["build"] != null)
					{
						float.TryParse(xmlDocument.FirstChild.Attributes["build"].Value, out num3);
					}
					bodyProperties = new BodyProperties(new DynamicBodyProperties(num, num2, num3), bodyProperties.StaticProperties);
					return true;
				}
				bodyProperties = default(BodyProperties);
				return false;
			}
			Debug.FailedAssert("unknown body properties format:\n" + keyValue, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\BodyProperties.cs", "FromString", 148);
			bodyProperties = default(BodyProperties);
			return false;
		}

		public static BodyProperties GetRandomBodyProperties(int race, bool isFemale, BodyProperties bodyPropertiesMin, BodyProperties bodyPropertiesMax, int hairCoverType, int seed, string hairTags, string beardTags, string tattooTags)
		{
			return FaceGen.GetRandomBodyProperties(race, isFemale, bodyPropertiesMin, bodyPropertiesMax, hairCoverType, seed, hairTags, beardTags, tattooTags);
		}

		public static bool operator ==(BodyProperties a, BodyProperties b)
		{
			return a == b || (a != null && b != null && a._staticBodyProperties == b._staticBodyProperties && a._dynamicBodyProperties == b._dynamicBodyProperties);
		}

		public static bool operator !=(BodyProperties a, BodyProperties b)
		{
			return !(a == b);
		}

		public override string ToString()
		{
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(150, "ToString");
			mbstringBuilder.Append<string>("<BodyProperties version=\"4\" ");
			mbstringBuilder.Append<string>(this._dynamicBodyProperties.ToString() + " ");
			mbstringBuilder.Append<string>(this._staticBodyProperties.ToString());
			mbstringBuilder.Append<string>(" />");
			return mbstringBuilder.ToStringAndRelease();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is BodyProperties))
			{
				return false;
			}
			BodyProperties bodyProperties = (BodyProperties)obj;
			return EqualityComparer<DynamicBodyProperties>.Default.Equals(this._dynamicBodyProperties, bodyProperties._dynamicBodyProperties) && EqualityComparer<StaticBodyProperties>.Default.Equals(this._staticBodyProperties, bodyProperties._staticBodyProperties);
		}

		public override int GetHashCode()
		{
			return (2041866711 * -1521134295 + EqualityComparer<DynamicBodyProperties>.Default.GetHashCode(this._dynamicBodyProperties)) * -1521134295 + EqualityComparer<StaticBodyProperties>.Default.GetHashCode(this._staticBodyProperties);
		}

		public BodyProperties ClampForMultiplayer()
		{
			float num = MathF.Clamp(this.DynamicProperties.Age, 22f, 128f);
			DynamicBodyProperties dynamicBodyProperties = new DynamicBodyProperties(num, 0.5f, 0.5f);
			StaticBodyProperties staticProperties = this.StaticProperties;
			StaticBodyProperties staticBodyProperties = this.ClampHeightMultiplierFaceKey(staticProperties);
			return new BodyProperties(dynamicBodyProperties, staticBodyProperties);
		}

		private StaticBodyProperties ClampHeightMultiplierFaceKey(in StaticBodyProperties staticBodyProperties)
		{
			StaticBodyProperties staticBodyProperties2 = staticBodyProperties;
			ulong keyPart = staticBodyProperties2.KeyPart8;
			float num = (float)BodyProperties.GetBitsValueFromKey(keyPart, 19, 6) / 63f;
			if (num < 0.25f || num > 0.75f)
			{
				num = 0.5f;
				int num2 = (int)(num * 63f);
				ulong num3 = BodyProperties.SetBits(keyPart, 19, 6, num2);
				staticBodyProperties2 = staticBodyProperties;
				ulong keyPart2 = staticBodyProperties2.KeyPart1;
				staticBodyProperties2 = staticBodyProperties;
				ulong keyPart3 = staticBodyProperties2.KeyPart2;
				staticBodyProperties2 = staticBodyProperties;
				ulong keyPart4 = staticBodyProperties2.KeyPart3;
				staticBodyProperties2 = staticBodyProperties;
				ulong keyPart5 = staticBodyProperties2.KeyPart4;
				staticBodyProperties2 = staticBodyProperties;
				ulong keyPart6 = staticBodyProperties2.KeyPart5;
				staticBodyProperties2 = staticBodyProperties;
				ulong keyPart7 = staticBodyProperties2.KeyPart6;
				ulong num4 = num3;
				staticBodyProperties2 = staticBodyProperties;
				return new StaticBodyProperties(keyPart2, keyPart3, keyPart4, keyPart5, keyPart6, keyPart7, num4, staticBodyProperties2.KeyPart8);
			}
			return staticBodyProperties;
		}

		private static ulong SetBits(in ulong ipart7, int startBit, int numBits, int inewValue)
		{
			ulong num = ipart7;
			ulong num2 = MathF.PowTwo64(numBits) - 1UL << startBit;
			return (num & ~num2) | (ulong)((ulong)((long)inewValue) << startBit);
		}

		private static int GetBitsValueFromKey(in ulong part, int startBit, int numBits)
		{
			ulong num = part >> startBit;
			ulong num2 = MathF.PowTwo64(numBits) - 1UL;
			return (int)(num & num2);
		}

		public static BodyProperties Default
		{
			get
			{
				return new BodyProperties(new DynamicBodyProperties(20f, 0f, 0f), default(StaticBodyProperties));
			}
		}

		private readonly DynamicBodyProperties _dynamicBodyProperties;

		private readonly StaticBodyProperties _staticBodyProperties;

		private const float DefaultAge = 30f;

		private const float DefaultWeight = 0.5f;

		private const float DefaultBuild = 0.5f;
	}
}
