using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD
{
	// Token: 0x020000DA RID: 218
	public class MissionAgentLockItemVM : ViewModel
	{
		// Token: 0x170006A7 RID: 1703
		// (get) Token: 0x06001427 RID: 5159 RVA: 0x00041EB1 File Offset: 0x000400B1
		// (set) Token: 0x06001428 RID: 5160 RVA: 0x00041EB9 File Offset: 0x000400B9
		public Agent TrackedAgent { get; private set; }

		// Token: 0x06001429 RID: 5161 RVA: 0x00041EC2 File Offset: 0x000400C2
		public MissionAgentLockItemVM(Agent agent, MissionAgentLockItemVM.LockStates initialLockState)
		{
			this.TrackedAgent = agent;
			this.LockState = (int)initialLockState;
		}

		// Token: 0x0600142A RID: 5162 RVA: 0x00041EDF File Offset: 0x000400DF
		public void SetLockState(MissionAgentLockItemVM.LockStates lockState)
		{
			this.LockState = (int)lockState;
		}

		// Token: 0x0600142B RID: 5163 RVA: 0x00041EE8 File Offset: 0x000400E8
		public void UpdatePosition(Vec2 position)
		{
			this.Position = position;
		}

		// Token: 0x170006A8 RID: 1704
		// (get) Token: 0x0600142C RID: 5164 RVA: 0x00041EF1 File Offset: 0x000400F1
		// (set) Token: 0x0600142D RID: 5165 RVA: 0x00041EF9 File Offset: 0x000400F9
		[DataSourceProperty]
		public Vec2 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (value != this._position)
				{
					this._position = value;
					base.OnPropertyChangedWithValue(value, "Position");
				}
			}
		}

		// Token: 0x170006A9 RID: 1705
		// (get) Token: 0x0600142E RID: 5166 RVA: 0x00041F1C File Offset: 0x0004011C
		// (set) Token: 0x0600142F RID: 5167 RVA: 0x00041F24 File Offset: 0x00040124
		[DataSourceProperty]
		public int LockState
		{
			get
			{
				return this._lockState;
			}
			set
			{
				if (value != this._lockState)
				{
					this._lockState = value;
					base.OnPropertyChangedWithValue(value, "LockState");
				}
			}
		}

		// Token: 0x040009A7 RID: 2471
		private Vec2 _position;

		// Token: 0x040009A8 RID: 2472
		private int _lockState = -1;

		// Token: 0x0200022C RID: 556
		public enum LockStates
		{
			// Token: 0x04000EB4 RID: 3764
			Possible,
			// Token: 0x04000EB5 RID: 3765
			Active
		}
	}
}
