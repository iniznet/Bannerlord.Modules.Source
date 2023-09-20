using System;
using System.Xml;

namespace TaleWorlds.Library
{
	[Serializable]
	public struct ApplicationVersion
	{
		public ApplicationVersionType ApplicationVersionType { get; private set; }

		public int Major { get; private set; }

		public int Minor { get; private set; }

		public int Revision { get; private set; }

		public int ChangeSet { get; private set; }

		public ApplicationVersion(ApplicationVersionType applicationVersionType, int major, int minor, int revision, int changeSet)
		{
			this.ApplicationVersionType = applicationVersionType;
			this.Major = major;
			this.Minor = minor;
			this.Revision = revision;
			this.ChangeSet = changeSet;
		}

		public static ApplicationVersion FromParametersFile(string customParameterFilePath = null)
		{
			string text = ((customParameterFilePath == null) ? (BasePath.Name + "Parameters/Version.xml") : customParameterFilePath);
			XmlDocument xmlDocument = new XmlDocument();
			string fileContent = VirtualFolders.GetFileContent(text);
			if (fileContent == "")
			{
				return ApplicationVersion.Empty;
			}
			xmlDocument.LoadXml(fileContent);
			return ApplicationVersion.FromString(xmlDocument.ChildNodes[0].ChildNodes[0].Attributes["Value"].InnerText, 21456);
		}

		public static ApplicationVersion FromString(string versionAsString, int defaultChangeSet = 21456)
		{
			string[] array = versionAsString.Split(new char[] { '.' });
			if (array.Length != 3 && array.Length != 4)
			{
				throw new Exception("Wrong version as string");
			}
			ApplicationVersionType applicationVersionType = ApplicationVersion.ApplicationVersionTypeFromString(array[0][0].ToString());
			string text = array[0].Substring(1);
			string text2 = array[1];
			string text3 = array[2];
			int num = Convert.ToInt32(text);
			int num2 = Convert.ToInt32(text2);
			int num3 = Convert.ToInt32(text3);
			int num4 = ((array.Length > 3) ? Convert.ToInt32(array[3]) : defaultChangeSet);
			return new ApplicationVersion(applicationVersionType, num, num2, num3, num4);
		}

		public bool IsSame(ApplicationVersion other)
		{
			return this.ApplicationVersionType == other.ApplicationVersionType && this.Major == other.Major && this.Minor == other.Minor && this.Revision == other.Revision;
		}

		public static ApplicationVersionType ApplicationVersionTypeFromString(string applicationVersionTypeAsString)
		{
			ApplicationVersionType applicationVersionType;
			if (!(applicationVersionTypeAsString == "a"))
			{
				if (!(applicationVersionTypeAsString == "b"))
				{
					if (!(applicationVersionTypeAsString == "e"))
					{
						if (!(applicationVersionTypeAsString == "v"))
						{
							if (!(applicationVersionTypeAsString == "d"))
							{
								applicationVersionType = ApplicationVersionType.Invalid;
							}
							else
							{
								applicationVersionType = ApplicationVersionType.Development;
							}
						}
						else
						{
							applicationVersionType = ApplicationVersionType.Release;
						}
					}
					else
					{
						applicationVersionType = ApplicationVersionType.EarlyAccess;
					}
				}
				else
				{
					applicationVersionType = ApplicationVersionType.Beta;
				}
			}
			else
			{
				applicationVersionType = ApplicationVersionType.Alpha;
			}
			return applicationVersionType;
		}

		public static string GetPrefix(ApplicationVersionType applicationVersionType)
		{
			string text;
			switch (applicationVersionType)
			{
			case ApplicationVersionType.Alpha:
				text = "a";
				break;
			case ApplicationVersionType.Beta:
				text = "b";
				break;
			case ApplicationVersionType.EarlyAccess:
				text = "e";
				break;
			case ApplicationVersionType.Release:
				text = "v";
				break;
			case ApplicationVersionType.Development:
				text = "d";
				break;
			default:
				text = "i";
				break;
			}
			return text;
		}

		public override string ToString()
		{
			string prefix = ApplicationVersion.GetPrefix(this.ApplicationVersionType);
			return string.Concat(new object[] { prefix, this.Major, ".", this.Minor, ".", this.Revision, ".", this.ChangeSet });
		}

		public static bool operator ==(ApplicationVersion a, ApplicationVersion b)
		{
			return a.Major == b.Major && a.Minor == b.Minor && a.Revision == b.Revision && a.ApplicationVersionType == b.ApplicationVersionType;
		}

		public static bool operator !=(ApplicationVersion a, ApplicationVersion b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && (ApplicationVersion)obj == this;
		}

		public static bool operator >(ApplicationVersion a, ApplicationVersion b)
		{
			if (a.ApplicationVersionType > b.ApplicationVersionType)
			{
				return true;
			}
			if (a.ApplicationVersionType == b.ApplicationVersionType)
			{
				if (a.Major > b.Major)
				{
					return true;
				}
				if (a.Major == b.Major)
				{
					if (a.Minor > b.Minor)
					{
						return true;
					}
					if (a.Minor == b.Minor && a.Revision > b.Revision)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool operator <(ApplicationVersion a, ApplicationVersion b)
		{
			return !(a == b) && !(a > b);
		}

		public static bool operator >=(ApplicationVersion a, ApplicationVersion b)
		{
			return a == b || a > b;
		}

		public static bool operator <=(ApplicationVersion a, ApplicationVersion b)
		{
			return a == b || a < b;
		}

		public const int DefaultChangeSet = 21456;

		public static readonly ApplicationVersion Empty = new ApplicationVersion(ApplicationVersionType.Invalid, -1, -1, -1, -1);
	}
}
