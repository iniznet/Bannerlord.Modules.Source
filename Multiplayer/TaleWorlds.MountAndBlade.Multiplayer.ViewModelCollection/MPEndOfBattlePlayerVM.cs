using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection
{
	public class MPEndOfBattlePlayerVM : MPPlayerVM
	{
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

		public override void RefreshValues()
		{
			base.RefreshValues();
			this._scoreTextObj.SetTextVariable("SCORE", this._displayedScore);
			this.ScoreText = this._scoreTextObj.ToString();
			this.PlacementText = Common.ToRoman(this._placement);
		}

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

		private readonly int _placement;

		private readonly int _displayedScore;

		private TextObject _scoreTextObj = new TextObject("{=Kvqb1lQR}{SCORE} Score", null);

		private string _placementText;

		private string _scoreText;
	}
}
