using System;
using System.Threading.Tasks;
using TaleWorlds.Diamond.Rest;
using TaleWorlds.Library;
using TaleWorlds.Library.Http;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000112 RID: 274
	public static class Gatekeeper
	{
		// Token: 0x0600053B RID: 1339 RVA: 0x00007D54 File Offset: 0x00005F54
		public static async Task<bool> IsGenerous()
		{
			bool flag;
			if (Gatekeeper._isDeployBuild)
			{
				if (Gatekeeper._random == null)
				{
					Gatekeeper._random = new Random(MachineId.AsInteger());
					Gatekeeper._roll = Gatekeeper._random.Next() % 101;
				}
				int num = await Gatekeeper.GetAdmittancePercentage();
				flag = Gatekeeper._roll <= num;
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x00007D94 File Offset: 0x00005F94
		private static async Task<int> GetAdmittancePercentage()
		{
			int num;
			try
			{
				HttpGetRequest getRequest = new HttpGetRequest("https://taleworldswebsiteassets.blob.core.windows.net/upload/blconfig.json");
				await getRequest.DoTask().ConfigureAwait(false);
				string responseData = getRequest.ResponseData;
				num = new RestDataJsonConverter().ReadJson<BannerlordConfig>(responseData).AdmittancePercentage;
			}
			catch (Exception)
			{
				num = 100;
			}
			return num;
		}

		// Token: 0x04000281 RID: 641
		private static Random _random;

		// Token: 0x04000282 RID: 642
		private static int _roll;

		// Token: 0x04000283 RID: 643
		private static readonly bool _isDeployBuild;
	}
}
