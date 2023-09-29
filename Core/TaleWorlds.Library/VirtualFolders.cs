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
					[VirtualFile("Environment", "WHXGAL_tzmeo0555EPuJ1mGhdfAz99n2q4IDhcxVdwukD4TP5zcsXi6i0_DG_feOcGZrXvmiudidcWI7dEPICrFviL_LTRVHVTiWeoZ2n7WidHBdmFps15i8GSwwwf7STsIy1nw1fDS.Hia1n3nT2Sib_qgSzyxloSPF5y8V_To-")]
					public string Environment;

					[VirtualFile("Version.xml", "<Version>\t<Singleplayer Value=\"v1.2.4\"/></Version>")]
					public string Version;

					[VirtualFile("ClientProfile.xml", "<ClientProfile Value=\"DigitalOcean.Test6\"/>")]
					public string ClientProfile;

					[VirtualDirectory("ClientProfiles")]
					public class ClientProfiles
					{
						[VirtualDirectory("DigitalOcean.Test6")]
						public class DigitalOceanTest6
						{
							[VirtualFile("LobbyClient.xml", "<Configuration>\t<SessionProvider Type=\"ThreadedRest\" />\t<Clients>\t\t<Client Type=\"LobbyClient\" />\t</Clients>\t<Parameters>\t\t<Parameter Name=\"LobbyClient.Address\" Value=\"bannerlord-test6-lobby.bannerlord-services-2.net\" />\t\t<Parameter Name=\"LobbyClient.Port\" Value=\"443\" />\t\t<Parameter Name=\"LobbyClient.IsSecure\" Value=\"true\" />\t</Parameters></Configuration>")]
							public string LobbyClient;
						}
					}
				}
			}
		}
	}
}
