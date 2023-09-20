using System;
using System.Collections.ObjectModel;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	public class MissionAgentTakenDamageVM : ViewModel
	{
		public MissionAgentTakenDamageVM(Camera missionCamera)
		{
			this._missionCamera = missionCamera;
			this.TakenDamageList = new MBBindingList<MissionAgentTakenDamageItemVM>();
		}

		public void SetIsEnabled(bool isEnabled)
		{
			this._isEnabled = isEnabled;
		}

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

		private void OnRemoveDamageItem(MissionAgentTakenDamageItemVM item)
		{
			this.TakenDamageList.Remove(item);
		}

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

		private Camera _missionCamera;

		private bool _isEnabled;

		private MBBindingList<MissionAgentTakenDamageItemVM> _takenDamageList;
	}
}
