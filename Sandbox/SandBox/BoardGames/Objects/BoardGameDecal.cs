using System;
using TaleWorlds.Engine;

namespace SandBox.BoardGames.Objects
{
	// Token: 0x020000BF RID: 191
	public class BoardGameDecal : ScriptComponentBehavior
	{
		// Token: 0x06000B80 RID: 2944 RVA: 0x0005C544 File Offset: 0x0005A744
		protected override void OnInit()
		{
			base.OnInit();
			this.SetAlpha(0f);
		}

		// Token: 0x06000B81 RID: 2945 RVA: 0x0005C557 File Offset: 0x0005A757
		public void SetAlpha(float alpha)
		{
			base.GameEntity.SetAlpha(alpha);
		}

		// Token: 0x06000B82 RID: 2946 RVA: 0x0005C565 File Offset: 0x0005A765
		protected override bool MovesEntity()
		{
			return false;
		}
	}
}
