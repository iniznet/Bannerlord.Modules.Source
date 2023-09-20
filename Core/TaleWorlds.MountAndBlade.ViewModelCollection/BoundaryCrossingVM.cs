using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection
{
	// Token: 0x02000004 RID: 4
	public class BoundaryCrossingVM : ViewModel
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002058 File Offset: 0x00000258
		public BoundaryCrossingVM(Mission mission, Action<bool> onEscapeMenuToggled)
		{
			this._onEscapeMenuToggled = onEscapeMenuToggled;
			this._mission = mission;
			this._missionBoundaryCrossingHandler = this._mission.GetMissionBehavior<MissionBoundaryCrossingHandler>();
			this._missionBoundaryCrossingHandler.StartTime += this.OnStartTime;
			this._missionBoundaryCrossingHandler.StopTime += this.OnStopTime;
			this._missionBoundaryCrossingHandler.TimeCount += this.OnTimeCount;
			this._show = false;
			this.WarningText = "";
			this.WarningProgress = 0.0;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000004 RID: 4 RVA: 0x0000210D File Offset: 0x0000030D
		// (set) Token: 0x06000005 RID: 5 RVA: 0x00002115 File Offset: 0x00000315
		[DataSourceProperty]
		public bool Show
		{
			get
			{
				return this._show;
			}
			set
			{
				if (value != this._show)
				{
					this._show = value;
					base.OnPropertyChangedWithValue(value, "Show");
					this._onEscapeMenuToggled(value);
				}
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000006 RID: 6 RVA: 0x0000213F File Offset: 0x0000033F
		// (set) Token: 0x06000007 RID: 7 RVA: 0x00002147 File Offset: 0x00000347
		[DataSourceProperty]
		public string WarningText
		{
			get
			{
				return this._warningText;
			}
			set
			{
				if (value != this._warningText)
				{
					this._warningText = value;
					base.OnPropertyChangedWithValue<string>(value, "WarningText");
				}
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000008 RID: 8 RVA: 0x0000216A File Offset: 0x0000036A
		// (set) Token: 0x06000009 RID: 9 RVA: 0x00002172 File Offset: 0x00000372
		[DataSourceProperty]
		public double WarningProgress
		{
			get
			{
				return this._warningProgress;
			}
			set
			{
				if (value != this._warningProgress)
				{
					this._warningProgress = value;
					base.OnPropertyChangedWithValue(value, "WarningProgress");
					this.WarningIntProgress = (int)(value * 100.0);
				}
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000021A2 File Offset: 0x000003A2
		// (set) Token: 0x0600000B RID: 11 RVA: 0x000021AA File Offset: 0x000003AA
		[DataSourceProperty]
		public int WarningIntProgress
		{
			get
			{
				return this._warningIntProgress;
			}
			set
			{
				if (value != this._warningIntProgress)
				{
					this._warningIntProgress = value;
					base.OnPropertyChangedWithValue(value, "WarningIntProgress");
				}
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000021C8 File Offset: 0x000003C8
		// (set) Token: 0x0600000D RID: 13 RVA: 0x000021D0 File Offset: 0x000003D0
		[DataSourceProperty]
		public int Countdown
		{
			get
			{
				return this._countdown;
			}
			set
			{
				if (value != this._countdown)
				{
					this._countdown = value;
					base.OnPropertyChangedWithValue(value, "Countdown");
				}
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000021F0 File Offset: 0x000003F0
		private void OnStartTime(float duration, float progress)
		{
			TextObject textObject = new TextObject("{=eGuQKRhb}You are leaving the area!", null);
			switch (this._mission.Mode)
			{
			case MissionMode.StartUp:
			case MissionMode.Battle:
			case MissionMode.Duel:
			case MissionMode.Stealth:
			case MissionMode.Deployment:
				goto IL_66;
			case MissionMode.Conversation:
				textObject = TextObject.Empty;
				goto IL_66;
			case MissionMode.Barter:
				textObject = TextObject.Empty;
				goto IL_66;
			case MissionMode.CutScene:
				textObject = TextObject.Empty;
				goto IL_66;
			}
			throw new ArgumentOutOfRangeException();
			IL_66:
			MBTextManager.SetTextVariable("MISSION_MODE", textObject, false);
			this.WarningText = GameTexts.FindText("str_out_of_mission_bound", null).ToString();
			this.WarningProgress = 0.0;
			this._duration = duration;
			this.Show = true;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000022A2 File Offset: 0x000004A2
		private void OnStopTime()
		{
			this.Show = false;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000022AB File Offset: 0x000004AB
		private void OnTimeCount(float progress)
		{
			this.WarningProgress = (double)progress;
			this.Countdown = MathF.Ceiling((1f - progress) * this._duration);
		}

		// Token: 0x04000001 RID: 1
		private readonly Mission _mission;

		// Token: 0x04000002 RID: 2
		private readonly MissionBoundaryCrossingHandler _missionBoundaryCrossingHandler;

		// Token: 0x04000003 RID: 3
		private readonly Action<bool> _onEscapeMenuToggled;

		// Token: 0x04000004 RID: 4
		private float _duration;

		// Token: 0x04000005 RID: 5
		private bool _show = true;

		// Token: 0x04000006 RID: 6
		private string _warningText;

		// Token: 0x04000007 RID: 7
		private double _warningProgress = -1.0;

		// Token: 0x04000008 RID: 8
		private int _warningIntProgress = -1;

		// Token: 0x04000009 RID: 9
		private int _countdown;
	}
}
