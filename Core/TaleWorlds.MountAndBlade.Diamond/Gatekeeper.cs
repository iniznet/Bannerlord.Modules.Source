using System;
using System.Threading.Tasks;
using TaleWorlds.Diamond.Rest;
using TaleWorlds.Library;

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
				string text = await HttpHelper.DownloadStringTaskAsync("https://taleworldswebsiteassets.blob.core.windows.net/upload/blconfig.json").ConfigureAwait(false);
				num = new RestDataJsonConverter().ReadJson<BannerlordConfig>(text).AdmittancePercentage;
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
