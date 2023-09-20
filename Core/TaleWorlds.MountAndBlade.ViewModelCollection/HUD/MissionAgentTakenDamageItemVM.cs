using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	public class MissionAgentTakenDamageItemVM : ViewModel
	{
		public MissionAgentTakenDamageItemVM(Camera missionCamera, Vec3 affectorAgentPos, int damage, bool isRanged, Action<MissionAgentTakenDamageItemVM> onRemove)
		{
			this._affectorAgentPosition = affectorAgentPos;
			this.Damage = damage;
			this.IsRanged = isRanged;
			this._missionCamera = missionCamera;
			this._onRemove = onRemove;
		}

		internal void Update()
		{
			if (this.IsRanged)
			{
				float num = 0f;
				float num2 = 0f;
				float num3 = 0f;
				MBWindowManager.WorldToScreen(this._missionCamera, this._affectorAgentPosition, ref num, ref num2, ref num3);
				this.ScreenPosOfAffectorAgent = new Vec2(num, num2);
				this.IsBehind = num3 < 0f;
			}
		}

		public void ExecuteRemove()
		{
			this._onRemove(this);
		}

		[DataSourceProperty]
		public int Damage
		{
			get
			{
				return this._damage;
			}
			set
			{
				if (value != this._damage)
				{
					this._damage = value;
					base.OnPropertyChangedWithValue(value, "Damage");
				}
			}
		}

		[DataSourceProperty]
		public bool IsRanged
		{
			get
			{
				return this._isRanged;
			}
			set
			{
				if (value != this._isRanged)
				{
					this._isRanged = value;
					base.OnPropertyChangedWithValue(value, "IsRanged");
				}
			}
		}

		[DataSourceProperty]
		public bool IsBehind
		{
			get
			{
				return this._isBehind;
			}
			set
			{
				if (value != this._isBehind)
				{
					this._isBehind = value;
					base.OnPropertyChangedWithValue(value, "IsBehind");
				}
			}
		}

		[DataSourceProperty]
		public Vec2 ScreenPosOfAffectorAgent
		{
			get
			{
				return this._screenPosOfAffectorAgent;
			}
			set
			{
				if (value != this._screenPosOfAffectorAgent)
				{
					this._screenPosOfAffectorAgent = value;
					base.OnPropertyChangedWithValue(value, "ScreenPosOfAffectorAgent");
				}
			}
		}

		private Action<MissionAgentTakenDamageItemVM> _onRemove;

		private Vec3 _affectorAgentPosition;

		private Camera _missionCamera;

		private int _damage;

		private bool _isBehind;

		private bool _isRanged;

		private Vec2 _screenPosOfAffectorAgent;
	}
}
