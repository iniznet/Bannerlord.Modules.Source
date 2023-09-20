using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000238 RID: 568
	public interface IFaceGeneratorHandler
	{
		// Token: 0x06001F41 RID: 8001
		void ChangeToBodyCamera();

		// Token: 0x06001F42 RID: 8002
		void ChangeToEyeCamera();

		// Token: 0x06001F43 RID: 8003
		void ChangeToNoseCamera();

		// Token: 0x06001F44 RID: 8004
		void ChangeToMouthCamera();

		// Token: 0x06001F45 RID: 8005
		void ChangeToFaceCamera();

		// Token: 0x06001F46 RID: 8006
		void ChangeToHairCamera();

		// Token: 0x06001F47 RID: 8007
		void RefreshCharacterEntity();

		// Token: 0x06001F48 RID: 8008
		void MakeVoice(int voiceIndex, float pitch);

		// Token: 0x06001F49 RID: 8009
		void SetFacialAnimation(string faceAnimation, bool loop);

		// Token: 0x06001F4A RID: 8010
		void Done();

		// Token: 0x06001F4B RID: 8011
		void Cancel();

		// Token: 0x06001F4C RID: 8012
		void UndressCharacterEntity();

		// Token: 0x06001F4D RID: 8013
		void DressCharacterEntity();

		// Token: 0x06001F4E RID: 8014
		void DefaultFace();
	}
}
