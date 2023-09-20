using System;
using System.Threading.Tasks;

namespace TaleWorlds.Library
{
	// Token: 0x0200003E RID: 62
	public interface IPlatformFileHelper
	{
		// Token: 0x060001E1 RID: 481
		SaveResult SaveFile(PlatformFilePath path, byte[] data);

		// Token: 0x060001E2 RID: 482
		SaveResult SaveFileString(PlatformFilePath path, string data);

		// Token: 0x060001E3 RID: 483
		SaveResult AppendLineToFileString(PlatformFilePath path, string data);

		// Token: 0x060001E4 RID: 484
		Task<SaveResult> SaveFileAsync(PlatformFilePath path, byte[] data);

		// Token: 0x060001E5 RID: 485
		Task<SaveResult> SaveFileStringAsync(PlatformFilePath path, string data);

		// Token: 0x060001E6 RID: 486
		bool FileExists(PlatformFilePath path);

		// Token: 0x060001E7 RID: 487
		Task<string> GetFileContentStringAsync(PlatformFilePath path);

		// Token: 0x060001E8 RID: 488
		string GetFileContentString(PlatformFilePath path);

		// Token: 0x060001E9 RID: 489
		byte[] GetFileContent(PlatformFilePath filePath);

		// Token: 0x060001EA RID: 490
		bool DeleteFile(PlatformFilePath path);

		// Token: 0x060001EB RID: 491
		PlatformFilePath[] GetFiles(PlatformDirectoryPath path, string searchPattern);

		// Token: 0x060001EC RID: 492
		string GetFileFullPath(PlatformFilePath filePath);

		// Token: 0x060001ED RID: 493
		string GetError();
	}
}
