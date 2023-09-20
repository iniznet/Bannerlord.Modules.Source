using System;
using System.Linq;

namespace TaleWorlds.MountAndBlade.Launcher.Library.UserDatas
{
	// Token: 0x02000018 RID: 24
	public class UserData
	{
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000FF RID: 255 RVA: 0x0000548A File Offset: 0x0000368A
		// (set) Token: 0x06000100 RID: 256 RVA: 0x00005492 File Offset: 0x00003692
		public GameType GameType { get; set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000101 RID: 257 RVA: 0x0000549B File Offset: 0x0000369B
		// (set) Token: 0x06000102 RID: 258 RVA: 0x000054A3 File Offset: 0x000036A3
		public UserGameTypeData SingleplayerData { get; set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000103 RID: 259 RVA: 0x000054AC File Offset: 0x000036AC
		// (set) Token: 0x06000104 RID: 260 RVA: 0x000054B4 File Offset: 0x000036B4
		public UserGameTypeData MultiplayerData { get; set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000105 RID: 261 RVA: 0x000054BD File Offset: 0x000036BD
		// (set) Token: 0x06000106 RID: 262 RVA: 0x000054C5 File Offset: 0x000036C5
		public DLLCheckDataCollection DLLCheckData { get; set; }

		// Token: 0x06000107 RID: 263 RVA: 0x000054CE File Offset: 0x000036CE
		public UserData()
		{
			this.GameType = GameType.Singleplayer;
			this.SingleplayerData = new UserGameTypeData();
			this.MultiplayerData = new UserGameTypeData();
			this.DLLCheckData = new DLLCheckDataCollection();
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00005500 File Offset: 0x00003700
		public UserModData GetUserModData(bool isMultiplayer, string id)
		{
			return (isMultiplayer ? this.MultiplayerData : this.SingleplayerData).ModDatas.Find((UserModData x) => x.Id == id);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00005544 File Offset: 0x00003744
		public uint? GetDLLLatestSizeInBytes(string dllName)
		{
			DLLCheckData dllcheckData = this.DLLCheckData.DLLData.FirstOrDefault((DLLCheckData d) => d.DLLName == dllName);
			if (dllcheckData == null)
			{
				return null;
			}
			return new uint?(dllcheckData.LatestSizeInBytes);
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00005594 File Offset: 0x00003794
		public bool GetDLLLatestIsDangerous(string dllName)
		{
			DLLCheckData dllcheckData = this.DLLCheckData.DLLData.FirstOrDefault((DLLCheckData d) => d.DLLName == dllName);
			return dllcheckData == null || dllcheckData.IsDangerous;
		}

		// Token: 0x0600010B RID: 267 RVA: 0x000055D8 File Offset: 0x000037D8
		public string GetDLLLatestVerifyInformation(string dllName)
		{
			DLLCheckData dllcheckData = this.DLLCheckData.DLLData.FirstOrDefault((DLLCheckData d) => d.DLLName == dllName);
			return ((dllcheckData != null) ? dllcheckData.DLLVerifyInformation : null) ?? "";
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00005624 File Offset: 0x00003824
		public void SetDLLLatestSizeInBytes(string dllName, uint sizeInBytes)
		{
			this.EnsureDLLIsAdded(dllName);
			this.DLLCheckData.DLLData.Find((DLLCheckData d) => d.DLLName == dllName).LatestSizeInBytes = sizeInBytes;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x0000566C File Offset: 0x0000386C
		public void SetDLLLatestVerifyInformation(string dllName, string verifyInformation)
		{
			this.EnsureDLLIsAdded(dllName);
			this.DLLCheckData.DLLData.Find((DLLCheckData d) => d.DLLName == dllName).DLLVerifyInformation = verifyInformation;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000056B4 File Offset: 0x000038B4
		public void SetDLLLatestIsDangerous(string dllName, bool isDangerous)
		{
			this.EnsureDLLIsAdded(dllName);
			this.DLLCheckData.DLLData.Find((DLLCheckData d) => d.DLLName == dllName).IsDangerous = isDangerous;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x000056FC File Offset: 0x000038FC
		private void EnsureDLLIsAdded(string dllName)
		{
			if (!this.DLLCheckData.DLLData.Any((DLLCheckData d) => d.DLLName == dllName))
			{
				this.DLLCheckData.DLLData.Add(new DLLCheckData(dllName));
			}
		}
	}
}
