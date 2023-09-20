using System;
using System.Collections.ObjectModel;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	// Token: 0x020000DB RID: 219
	public class MissionAgentTakenDamageVM : ViewModel
	{
		// Token: 0x06001430 RID: 5168 RVA: 0x00041F42 File Offset: 0x00040142
		public MissionAgentTakenDamageVM(Camera missionCamera)
		{
			this._missionCamera = missionCamera;
			this.TakenDamageList = new MBBindingList<MissionAgentTakenDamageItemVM>();
		}

		// Token: 0x06001431 RID: 5169 RVA: 0x00041F5C File Offset: 0x0004015C
		public void SetIsEnabled(bool isEnabled)
		{
			this._isEnabled = isEnabled;
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x00041F68 File Offset: 0x00040168
		internal void Tick(float dt)
		{
			if (this._isEnabled)
			{
				for (int i = 0; i < this.TakenDamageList.Count; i++)
				{
					this.TakenDamageList[i].Update();
				}
			}
		}

		// Token: 0x06001433 RID: 5171 RVA: 0x00041FA4 File Offset: 0x000401A4
		internal void OnMainAgentHit(int damage, float distance)
		{
			if (this._isEnabled && damage > 0)
			{
				Collection<MissionAgentTakenDamageItemVM> takenDamageList = this.TakenDamageList;
				Camera missionCamera = this._missionCamera;
				Agent main = Agent.Main;
				takenDamageList.Add(new MissionAgentTakenDamageItemVM(missionCamera, (main != null) ? main.Position : default(Vec3), damage, false, new Action<MissionAgentTakenDamageItemVM>(this.OnRemoveDamageItem)));
			}
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x00041FFA File Offset: 0x000401FA
		private void OnRemoveDamageItem(MissionAgentTakenDamageItemVM item)
		{
			this.TakenDamageList.Remove(item);
		}

		// Token: 0x170006AA RID: 1706
		// (get) Token: 0x06001435 RID: 5173 RVA: 0x00042009 File Offset: 0x00040209
		// (set) Token: 0x06001436 RID: 5174 RVA: 0x00042011 File Offset: 0x00040211
		[DataSourceProperty]
		public MBBindingList<MissionAgentTakenDamageItemVM> TakenDamageList
		{
			get
			{
				return this._takenDamageList;
			}
			set
			{
				if (value != this._takenDamageList)
				{
					this._takenDamageList = value;
					base.OnPropertyChangedWithValue<MBBindingList<MissionAgentTakenDamageItemVM>>(value, "TakenDamageList");
				}
			}
		}

		// Token: 0x040009A9 RID: 2473
		private Camera _missionCamera;

		// Token: 0x040009AA RID: 2474
		private bool _isEnabled;

		// Token: 0x040009AB RID: 2475
		private MBBindingList<MissionAgentTakenDamageItemVM> _takenDamageList;
	}
}
