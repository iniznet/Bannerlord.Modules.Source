using System;
using System.Threading.Tasks;
using TaleWorlds.Diamond.Rest;
using TaleWorlds.Library;
using TaleWorlds.Library.Http;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public static class Gatekeeper
	{
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

		private static Random _random;

		private static int _roll;

		private static readonly bool _isDeployBuild;
	}
}
