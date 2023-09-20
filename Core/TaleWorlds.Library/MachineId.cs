using System;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace TaleWorlds.Library
{
	// Token: 0x02000059 RID: 89
	public static class MachineId
	{
		// Token: 0x06000282 RID: 642 RVA: 0x000070B4 File Offset: 0x000052B4
		public static void Initialize()
		{
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				MachineId.MachineIdString = "nonwindows";
				return;
			}
			MachineId.MachineIdString = MachineId.ProcessId();
		}

		// Token: 0x06000283 RID: 643 RVA: 0x000070D7 File Offset: 0x000052D7
		public static int AsInteger()
		{
			if (!string.IsNullOrEmpty(MachineId.MachineIdString))
			{
				return BitConverter.ToInt32(Encoding.ASCII.GetBytes(MachineId.MachineIdString), 0);
			}
			return 0;
		}

		// Token: 0x06000284 RID: 644 RVA: 0x000070FC File Offset: 0x000052FC
		private static string ProcessId()
		{
			return "" + MachineId.GetMotherboardIdentifier() + MachineId.GetCpuIdentifier() + MachineId.GetDiskIdentifier();
		}

		// Token: 0x06000285 RID: 645 RVA: 0x00007124 File Offset: 0x00005324
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

		// Token: 0x06000286 RID: 646 RVA: 0x000071EC File Offset: 0x000053EC
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

		// Token: 0x06000287 RID: 647 RVA: 0x000072B4 File Offset: 0x000054B4
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

		// Token: 0x040000F0 RID: 240
		private static string MachineIdString;
	}
}
