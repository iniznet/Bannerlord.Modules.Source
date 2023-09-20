using System;
using System.Xml;

namespace TaleWorlds.Library
{
	// Token: 0x02000009 RID: 9
	[Serializable]
	public struct ApplicationVersion
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000020D8 File Offset: 0x000002D8
		// (set) Token: 0x0600000D RID: 13 RVA: 0x000020E0 File Offset: 0x000002E0
		public ApplicationVersionType ApplicationVersionType { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000020E9 File Offset: 0x000002E9
		// (set) Token: 0x0600000F RID: 15 RVA: 0x000020F1 File Offset: 0x000002F1
		public int Major { get; private set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000020FA File Offset: 0x000002FA
		// (set) Token: 0x06000011 RID: 17 RVA: 0x00002102 File Offset: 0x00000302
		public int Minor { get; private set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000012 RID: 18 RVA: 0x0000210B File Offset: 0x0000030B
		// (set) Token: 0x06000013 RID: 19 RVA: 0x00002113 File Offset: 0x00000313
		public int Revision { get; private set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000014 RID: 20 RVA: 0x0000211C File Offset: 0x0000031C
		// (set) Token: 0x06000015 RID: 21 RVA: 0x00002124 File Offset: 0x00000324
		public int ChangeSet { get; private set; }

		// Token: 0x06000016 RID: 22 RVA: 0x0000212D File Offset: 0x0000032D
		public ApplicationVersion(ApplicationVersionType applicationVersionType, int major, int minor, int revision, int changeSet)
		{
			this.ApplicationVersionType = applicationVersionType;
			this.Major = major;
			this.Minor = minor;
			this.Revision = revision;
			this.ChangeSet = changeSet;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002154 File Offset: 0x00000354
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
			return ApplicationVersion.FromString(xmlDocument.ChildNodes[0].ChildNodes[0].Attributes["Value"].InnerText, 17949);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000021D4 File Offset: 0x000003D4
		public static ApplicationVersion FromString(string versionAsString, int defaultChangeSet = 17949)
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

		// Token: 0x06000019 RID: 25 RVA: 0x00002267 File Offset: 0x00000467
		public bool IsSame(ApplicationVersion other)
		{
			return this.ApplicationVersionType == other.ApplicationVersionType && this.Major == other.Major && this.Minor == other.Minor && this.Revision == other.Revision;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000022A8 File Offset: 0x000004A8
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

		// Token: 0x0600001B RID: 27 RVA: 0x00002314 File Offset: 0x00000514
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

		// Token: 0x0600001C RID: 28 RVA: 0x00002374 File Offset: 0x00000574
		public override string ToString()
		{
			string prefix = ApplicationVersion.GetPrefix(this.ApplicationVersionType);
			return string.Concat(new object[] { prefix, this.Major, ".", this.Minor, ".", this.Revision, ".", this.ChangeSet });
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000023EC File Offset: 0x000005EC
		public static bool operator ==(ApplicationVersion a, ApplicationVersion b)
		{
			return a.Major == b.Major && a.Minor == b.Minor && a.Revision == b.Revision && a.ApplicationVersionType == b.ApplicationVersionType;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x0000243B File Offset: 0x0000063B
		public static bool operator !=(ApplicationVersion a, ApplicationVersion b)
		{
			return !(a == b);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002447 File Offset: 0x00000647
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002459 File Offset: 0x00000659
		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && (ApplicationVersion)obj == this;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002490 File Offset: 0x00000690
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

		// Token: 0x06000022 RID: 34 RVA: 0x00002516 File Offset: 0x00000716
		public static bool operator <(ApplicationVersion a, ApplicationVersion b)
		{
			return !(a == b) && !(a > b);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x0000252D File Offset: 0x0000072D
		public static bool operator >=(ApplicationVersion a, ApplicationVersion b)
		{
			return a == b || a > b;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002544 File Offset: 0x00000744
		public static bool operator <=(ApplicationVersion a, ApplicationVersion b)
		{
			return a == b || a < b;
		}

		// Token: 0x0400001B RID: 27
		public const int DefaultChangeSet = 17949;

		// Token: 0x0400001C RID: 28
		public static readonly ApplicationVersion Empty = new ApplicationVersion(ApplicationVersionType.Invalid, -1, -1, -1, -1);
	}
}
