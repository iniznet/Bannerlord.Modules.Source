using System;
using System.Linq;

namespace TaleWorlds.MountAndBlade.Launcher.Library.UserDatas
{
	public class UserData
	{
		public GameType GameType { get; set; }

		public UserGameTypeData SingleplayerData { get; set; }

		public UserGameTypeData MultiplayerData { get; set; }

		public DLLCheckDataCollection DLLCheckData { get; set; }

		public UserData()
		{
			this.GameType = GameType.Singleplayer;
			this.SingleplayerData = new UserGameTypeData();
			this.MultiplayerData = new UserGameTypeData();
			this.DLLCheckData = new DLLCheckDataCollection();
		}

		public UserModData GetUserModData(bool isMultiplayer, string id)
		{
			return (isMultiplayer ? this.MultiplayerData : this.SingleplayerData).ModDatas.Find((UserModData x) => x.Id == id);
		}

		public uint? GetDLLLatestSizeInBytes(string dllName)
		{
			DLLCheckData dllcheckData = this.DLLCheckData.DLLData.FirstOrDefault((DLLCheckData d) => d.DLLName == dllName);
			if (dllcheckData == null)
			{
				return null;
			}
			return new uint?(dllcheckData.LatestSizeInBytes);
		}

		public bool GetDLLLatestIsDangerous(string dllName)
		{
			DLLCheckData dllcheckData = this.DLLCheckData.DLLData.FirstOrDefault((DLLCheckData d) => d.DLLName == dllName);
			return dllcheckData == null || dllcheckData.IsDangerous;
		}

		public string GetDLLLatestVerifyInformation(string dllName)
		{
			DLLCheckData dllcheckData = this.DLLCheckData.DLLData.FirstOrDefault((DLLCheckData d) => d.DLLName == dllName);
			return ((dllcheckData != null) ? dllcheckData.DLLVerifyInformation : null) ?? "";
		}

		public void SetDLLLatestSizeInBytes(string dllName, uint sizeInBytes)
		{
			this.EnsureDLLIsAdded(dllName);
			this.DLLCheckData.DLLData.Find((DLLCheckData d) => d.DLLName == dllName).LatestSizeInBytes = sizeInBytes;
		}

		public void SetDLLLatestVerifyInformation(string dllName, string verifyInformation)
		{
			this.EnsureDLLIsAdded(dllName);
			this.DLLCheckData.DLLData.Find((DLLCheckData d) => d.DLLName == dllName).DLLVerifyInformation = verifyInformation;
		}

		public void SetDLLLatestIsDangerous(string dllName, bool isDangerous)
		{
			this.EnsureDLLIsAdded(dllName);
			this.DLLCheckData.DLLData.Find((DLLCheckData d) => d.DLLName == dllName).IsDangerous = isDangerous;
		}

		private void EnsureDLLIsAdded(string dllName)
		{
			if (!this.DLLCheckData.DLLData.Any((DLLCheckData d) => d.DLLName == dllName))
			{
				this.DLLCheckData.DLLData.Add(new DLLCheckData(dllName));
			}
		}
	}
}
