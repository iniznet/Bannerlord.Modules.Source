using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x0200003C RID: 60
	public class MPEndOfBattlePlayerVM : MPPlayerVM
	{
		// Token: 0x0600053F RID: 1343 RVA: 0x00016B60 File Offset: 0x00014D60
		public MPEndOfBattlePlayerVM(MissionPeer peer, int displayedScore, int placement)
			: base(peer)
		{
			this._placement = placement;
			this._displayedScore = displayedScore;
			BasicCharacterObject @object = MBObjectManager.Instance.GetObject<BasicCharacterObject>("mp_character");
			@object.UpdatePlayerCharacterBodyProperties(peer.Peer.BodyProperties, peer.Peer.Race, peer.Peer.IsFemale);
			@object.Age = peer.Peer.BodyProperties.Age;
			base.RefreshPreview(@object, peer.Peer.BodyProperties.DynamicProperties, peer.Peer.IsFemale);
			this.RefreshValues();
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x00016C10 File Offset: 0x00014E10
		public override void RefreshValues()
		{
			base.RefreshValues();
			this._scoreTextObj.SetTextVariable("SCORE", this._displayedScore);
			this.ScoreText = this._scoreTextObj.ToString();
			this.PlacementText = Common.ToRoman(this._placement);
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06000541 RID: 1345 RVA: 0x00016C5C File Offset: 0x00014E5C
		// (set) Token: 0x06000542 RID: 1346 RVA: 0x00016C64 File Offset: 0x00014E64
		[DataSourceProperty]
		public string PlacementText
		{
			get
			{
				return this._placementText;
			}
			set
			{
				if (value != this._placementText)
				{
					this._placementText = value;
					base.OnPropertyChangedWithValue<string>(value, "PlacementText");
				}
			}
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x06000543 RID: 1347 RVA: 0x00016C87 File Offset: 0x00014E87
		// (set) Token: 0x06000544 RID: 1348 RVA: 0x00016C8F File Offset: 0x00014E8F
		[DataSourceProperty]
		public string ScoreText
		{
			get
			{
				return this._scoreText;
			}
			set
			{
				if (value != this._scoreText)
				{
					this._scoreText = value;
					base.OnPropertyChangedWithValue<string>(value, "ScoreText");
				}
			}
		}

		// Token: 0x040002A8 RID: 680
		private readonly int _placement;

		// Token: 0x040002A9 RID: 681
		private readonly int _displayedScore;

		// Token: 0x040002AA RID: 682
		private TextObject _scoreTextObj = new TextObject("{=Kvqb1lQR}{SCORE} Score", null);

		// Token: 0x040002AB RID: 683
		private string _placementText;

		// Token: 0x040002AC RID: 684
		private string _scoreText;
	}
}
