using System;
using System.IO;
using System.Reflection;

namespace TaleWorlds.Library
{
	// Token: 0x0200009D RID: 157
	public class VirtualFolders
	{
		// Token: 0x060005C9 RID: 1481 RVA: 0x000128FE File Offset: 0x00010AFE
		public static string GetFileContent(string filePath)
		{
			if (VirtualFolders._useVirtualFolders)
			{
				return VirtualFolders.GetVirtualFileContent(filePath);
			}
			if (!File.Exists(filePath))
			{
				return "";
			}
			return File.ReadAllText(filePath);
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x00012924 File Offset: 0x00010B24
		private static string GetVirtualFileContent(string filePath)
		{
			string fileName = Path.GetFileName(filePath);
			string[] array = Path.GetDirectoryName(filePath).Split(new char[] { Path.DirectorySeparatorChar });
			Type type = typeof(VirtualFolders);
			int num = 0;
			while (type != null && num != array.Length)
			{
				if (!string.IsNullOrEmpty(array[num]))
				{
					type = VirtualFolders.GetNestedDirectory(array[num], type);
				}
				num++;
			}
			if (type != null)
			{
				FieldInfo[] fields = type.GetFields();
				for (int i = 0; i < fields.Length; i++)
				{
					VirtualFileAttribute[] array2 = (VirtualFileAttribute[])fields[i].GetCustomAttributes(typeof(VirtualFileAttribute), false);
					if (array2[0].Name == fileName)
					{
						return array2[0].Content;
					}
				}
			}
			return "";
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x000129E8 File Offset: 0x00010BE8
		private static Type GetNestedDirectory(string name, Type type)
		{
			foreach (Type type2 in type.GetNestedTypes())
			{
				if (((VirtualDirectoryAttribute[])type2.GetCustomAttributes(typeof(VirtualDirectoryAttribute), false))[0].Name == name)
				{
					return type2;
				}
			}
			return null;
		}

		// Token: 0x040001AC RID: 428
		private static readonly bool _useVirtualFolders = true;

		// Token: 0x020000E3 RID: 227
		[VirtualDirectory("..")]
		public class Win64_Shipping_Client
		{
			// Token: 0x020000EF RID: 239
			[VirtualDirectory("..")]
			public class bin
			{
				// Token: 0x020000F1 RID: 241
				[VirtualDirectory("Parameters")]
				public class Parameters
				{
					// Token: 0x040002FF RID: 767
					[VirtualFile("Environment", "Yjzjzz0mVwx2addMF.BpsPZnsrAIzbcAgIc9QK7P2ctoKRAGiv4wNCZdffAtcqpIgV41Zo0O._5PX3F89mFJ4_k18G9lErN2T2NBF7z21n3jyW_7HE1ZkS7Nju.pz5mbj.Kco5ObsZZPhJT7mjCRQKR3.84Hbxegc9v4LhwfrFE-")]
					public string Environment;

					// Token: 0x04000300 RID: 768
					[VirtualFile("Version.xml", "<Version>\t<Singleplayer Value=\"v1.1.4\"/></Version>")]
					public string Version;

					// Token: 0x04000301 RID: 769
					[VirtualFile("ClientProfile.xml", "<ClientProfile Value=\"DigitalOcean.Discovery\"/>")]
					public string ClientProfile;

					// Token: 0x020000F2 RID: 242
					[VirtualDirectory("ClientProfiles")]
					public class ClientProfiles
					{
						// Token: 0x020000F3 RID: 243
						[VirtualDirectory("DigitalOcean.Discovery")]
						public class DigitalOceanDiscovery
						{
							// Token: 0x04000302 RID: 770
							[VirtualFile("LobbyClient.xml", "<Configuration>\t<SessionProvider Type=\"ThreadedRest\" />\t<Clients>\t\t<Client Type=\"LobbyClient\" />\t</Clients>\t<Parameters>\t\t<Parameter Name=\"LobbyClient.ServiceDiscovery.Address\" Value=\"https://bannerlord-service-discovery.bannerlord-services-2.net:5100/\" />\t\t\t\t<Parameter Name=\"LobbyClient.Address\" Value=\"service://bannerlord.lobby/\" />\t\t<Parameter Name=\"LobbyClient.Port\" Value=\"443\" />\t\t<Parameter Name=\"LobbyClient.IsSecure\" Value=\"true\" />\t</Parameters></Configuration>")]
							public string LobbyClient;
						}
					}
				}
			}
		}
	}
}
