using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.BoardGame
{
	// Token: 0x0200003C RID: 60
	public class BoardGameInstructionVM : ViewModel
	{
		// Token: 0x06000446 RID: 1094 RVA: 0x00012F3E File Offset: 0x0001113E
		public BoardGameInstructionVM(CultureObject.BoardGameType game, int instructionIndex)
		{
			this._game = game;
			this._instructionIndex = instructionIndex;
			this.GameType = this._game.ToString();
			this.RefreshValues();
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x00012F74 File Offset: 0x00011174
		public override void RefreshValues()
		{
			base.RefreshValues();
			GameTexts.SetVariable("newline", "\n");
			this.TitleText = GameTexts.FindText("str_board_game_title", this._game.ToString() + "_" + this._instructionIndex).ToString();
			this.DescriptionText = GameTexts.FindText("str_board_game_instruction", this._game.ToString() + "_" + this._instructionIndex).ToString();
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x0001300C File Offset: 0x0001120C
		// (set) Token: 0x06000449 RID: 1097 RVA: 0x00013014 File Offset: 0x00011214
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x0600044A RID: 1098 RVA: 0x00013032 File Offset: 0x00011232
		// (set) Token: 0x0600044B RID: 1099 RVA: 0x0001303A File Offset: 0x0001123A
		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x0600044C RID: 1100 RVA: 0x0001305D File Offset: 0x0001125D
		// (set) Token: 0x0600044D RID: 1101 RVA: 0x00013065 File Offset: 0x00011265
		[DataSourceProperty]
		public string DescriptionText
		{
			get
			{
				return this._descriptionText;
			}
			set
			{
				if (value != this._descriptionText)
				{
					this._descriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "DescriptionText");
				}
			}
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x0600044E RID: 1102 RVA: 0x00013088 File Offset: 0x00011288
		// (set) Token: 0x0600044F RID: 1103 RVA: 0x00013090 File Offset: 0x00011290
		[DataSourceProperty]
		public string GameType
		{
			get
			{
				return this._gameType;
			}
			set
			{
				if (value != this._gameType)
				{
					this._gameType = value;
					base.OnPropertyChangedWithValue<string>(value, "GameType");
				}
			}
		}

		// Token: 0x04000238 RID: 568
		private readonly CultureObject.BoardGameType _game;

		// Token: 0x04000239 RID: 569
		private readonly int _instructionIndex;

		// Token: 0x0400023A RID: 570
		private bool _isEnabled;

		// Token: 0x0400023B RID: 571
		private string _titleText;

		// Token: 0x0400023C RID: 572
		private string _descriptionText;

		// Token: 0x0400023D RID: 573
		private string _gameType;
	}
}
