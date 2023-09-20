using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	// Token: 0x020000DC RID: 220
	public class MissionAgentTakenDamageItemVM : ViewModel
	{
		// Token: 0x06001437 RID: 5175 RVA: 0x0004202F File Offset: 0x0004022F
		public MissionAgentTakenDamageItemVM(Camera missionCamera, Vec3 affectorAgentPos, int damage, bool isRanged, Action<MissionAgentTakenDamageItemVM> onRemove)
		{
			this._affectorAgentPosition = affectorAgentPos;
			this.Damage = damage;
			this.IsRanged = isRanged;
			this._missionCamera = missionCamera;
			this._onRemove = onRemove;
		}

		// Token: 0x06001438 RID: 5176 RVA: 0x0004205C File Offset: 0x0004025C
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

		// Token: 0x06001439 RID: 5177 RVA: 0x000420B6 File Offset: 0x000402B6
		public void ExecuteRemove()
		{
			this._onRemove(this);
		}

		// Token: 0x170006AB RID: 1707
		// (get) Token: 0x0600143A RID: 5178 RVA: 0x000420C4 File Offset: 0x000402C4
		// (set) Token: 0x0600143B RID: 5179 RVA: 0x000420CC File Offset: 0x000402CC
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

		// Token: 0x170006AC RID: 1708
		// (get) Token: 0x0600143C RID: 5180 RVA: 0x000420EA File Offset: 0x000402EA
		// (set) Token: 0x0600143D RID: 5181 RVA: 0x000420F2 File Offset: 0x000402F2
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

		// Token: 0x170006AD RID: 1709
		// (get) Token: 0x0600143E RID: 5182 RVA: 0x00042110 File Offset: 0x00040310
		// (set) Token: 0x0600143F RID: 5183 RVA: 0x00042118 File Offset: 0x00040318
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

		// Token: 0x170006AE RID: 1710
		// (get) Token: 0x06001440 RID: 5184 RVA: 0x00042136 File Offset: 0x00040336
		// (set) Token: 0x06001441 RID: 5185 RVA: 0x0004213E File Offset: 0x0004033E
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

		// Token: 0x040009AC RID: 2476
		private Action<MissionAgentTakenDamageItemVM> _onRemove;

		// Token: 0x040009AD RID: 2477
		private Vec3 _affectorAgentPosition;

		// Token: 0x040009AE RID: 2478
		private Camera _missionCamera;

		// Token: 0x040009AF RID: 2479
		private int _damage;

		// Token: 0x040009B0 RID: 2480
		private bool _isBehind;

		// Token: 0x040009B1 RID: 2481
		private bool _isRanged;

		// Token: 0x040009B2 RID: 2482
		private Vec2 _screenPosOfAffectorAgent;
	}
}
