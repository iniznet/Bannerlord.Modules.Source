using System;
using System.IO;
using System.Reflection;

namespace TaleWorlds.Library
{
	public class VirtualFolders
	{
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

		private static readonly bool _useVirtualFolders = true;

		[VirtualDirectory("..")]
		public class Win64_Shipping_Client
		{
			[VirtualDirectory("..")]
			public class bin
			{
				[VirtualDirectory("Parameters")]
				public class Parameters
				{
					[VirtualFile("Environment", "Hv_Vp7wj3pHzimvCVGckW.tGxD_meNQ2RShr6kjmP3M.aSo8SXmfrtk_iqfYbgiP8SqEFY8WxnOY_9X9MUVMFLyP2aZeUjwQ2NXGsrvabRlpVVYmVpvzlKX1zppg7v8bzs8ffwQK_vfXp7T9sQLSEP5lGcBKnjbqEh97dtf4c.o-")]
					public string Environment;

					[VirtualFile("Version.xml", "<Version>\t<Singleplayer Value=\"v1.1.6\"/></Version>")]
					public string Version;

					[VirtualFile("ClientProfile.xml", "<ClientProfile Value=\"DigitalOcean.Discovery\"/>")]
					public string ClientProfile;

					[VirtualDirectory("ClientProfiles")]
					public class ClientProfiles
					{
						[VirtualDirectory("DigitalOcean.Discovery")]
						public class DigitalOceanDiscovery
						{
							[VirtualFile("LobbyClient.xml", "<Configuration>\t<SessionProvider Type=\"ThreadedRest\" />\t<Clients>\t\t<Client Type=\"LobbyClient\" />\t</Clients>\t<Parameters>\t\t<Parameter Name=\"LobbyClient.ServiceDiscovery.Address\" Value=\"https://bannerlord-service-discovery.bannerlord-services-3.net/\" />\t\t\t\t<Parameter Name=\"LobbyClient.Address\" Value=\"service://bannerlord.lobby/\" />\t\t<Parameter Name=\"LobbyClient.Port\" Value=\"443\" />\t\t<Parameter Name=\"LobbyClient.IsSecure\" Value=\"true\" />\t</Parameters></Configuration>")]
							public string LobbyClient;
						}
					}
				}
			}
		}
	}
}
