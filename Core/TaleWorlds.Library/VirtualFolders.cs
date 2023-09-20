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
					[VirtualFile("Environment", "AS_irYb6U6uI24y.HucZCv7CvahQ79hhN3W_.q.Tl5LD7nSWfpD1q1UaZt_if5tnwE42NFiGpgBSZvt9IfZKdnNRMxN8YBsZkq3F8rByvPNBEJTPS7kkoCqbj9mQK9w8KSsxBSAh4V8FUl_QMOvtq3y.37CaHiQ5hsrAzuok5g0-")]
					public string Environment;

					[VirtualFile("Version.xml", "<Version>\t<Singleplayer Value=\"v1.1.5\"/></Version>")]
					public string Version;

					[VirtualFile("ClientProfile.xml", "<ClientProfile Value=\"DigitalOcean.Discovery\"/>")]
					public string ClientProfile;

					[VirtualDirectory("ClientProfiles")]
					public class ClientProfiles
					{
						[VirtualDirectory("DigitalOcean.Discovery")]
						public class DigitalOceanDiscovery
						{
							[VirtualFile("LobbyClient.xml", "<Configuration>\t<SessionProvider Type=\"ThreadedRest\" />\t<Clients>\t\t<Client Type=\"LobbyClient\" />\t</Clients>\t<Parameters>\t\t<Parameter Name=\"LobbyClient.ServiceDiscovery.Address\" Value=\"https://bannerlord-service-discovery.bannerlord-services-2.net:5100/\" />\t\t\t\t<Parameter Name=\"LobbyClient.Address\" Value=\"service://bannerlord.lobby/\" />\t\t<Parameter Name=\"LobbyClient.Port\" Value=\"443\" />\t\t<Parameter Name=\"LobbyClient.IsSecure\" Value=\"true\" />\t</Parameters></Configuration>")]
							public string LobbyClient;
						}
					}
				}
			}
		}
	}
}
