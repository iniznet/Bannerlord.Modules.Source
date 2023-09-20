using System;
using System.IO;

namespace psai.net
{
	// Token: 0x0200000F RID: 15
	internal interface IPlatformLayer
	{
		// Token: 0x06000138 RID: 312
		void Initialize();

		// Token: 0x06000139 RID: 313
		void Release();

		// Token: 0x0600013A RID: 314
		Stream GetStreamOnPsaiSoundtrackFile(string filename);

		// Token: 0x0600013B RID: 315
		string ConvertFilePathForPlatform(string filepath);
	}
}
