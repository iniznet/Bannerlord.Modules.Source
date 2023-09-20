using System;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace TaleWorlds.Library
{
	public static class MachineId
	{
		public static void Initialize()
		{
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				MachineId.MachineIdString = "nonwindows";
				return;
			}
			MachineId.MachineIdString = MachineId.ProcessId();
		}

		public static int AsInteger()
		{
			if (!string.IsNullOrEmpty(MachineId.MachineIdString))
			{
				return BitConverter.ToInt32(Encoding.ASCII.GetBytes(MachineId.MachineIdString), 0);
			}
			return 0;
		}

		private static string ProcessId()
		{
			return "" + MachineId.GetMotherboardIdentifier() + MachineId.GetCpuIdentifier() + MachineId.GetDiskIdentifier();
		}

		private static string GetMotherboardIdentifier()
		{
			string text = "";
			try
			{
				using (ManagementObjectCollection instances = new ManagementClass("win32_baseboard").GetInstances())
				{
					foreach (ManagementBaseObject managementBaseObject in instances)
					{
						string text2 = (((ManagementObject)managementBaseObject)["SerialNumber"] as string).Trim(new char[] { ' ' });
						text += text2.Replace("-", "");
					}
				}
			}
			catch (Exception)
			{
				return "";
			}
			return text;
		}

		private static string GetCpuIdentifier()
		{
			string text = "";
			try
			{
				using (ManagementObjectCollection instances = new ManagementClass("win32_processor").GetInstances())
				{
					foreach (ManagementBaseObject managementBaseObject in instances)
					{
						string text2 = (((ManagementObject)managementBaseObject)["ProcessorId"] as string).Trim(new char[] { ' ' });
						text += text2.Replace("-", "");
					}
				}
			}
			catch (Exception)
			{
				return "";
			}
			return text;
		}

		private static string GetDiskIdentifier()
		{
			string text = "";
			try
			{
				using (ManagementObjectCollection instances = new ManagementClass("win32_diskdrive").GetInstances())
				{
					foreach (ManagementBaseObject managementBaseObject in instances)
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						if (string.Compare(managementObject["InterfaceType"] as string, "IDE", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							string text2 = (managementObject["SerialNumber"] as string).Trim(new char[] { ' ' });
							text += text2.Replace("-", "");
						}
					}
				}
			}
			catch (Exception)
			{
				return "";
			}
			return text;
		}

		private static string MachineIdString;
	}
}
