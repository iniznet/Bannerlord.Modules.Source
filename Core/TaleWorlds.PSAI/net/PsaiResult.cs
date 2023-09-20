using System;

namespace psai.net
{
	// Token: 0x02000011 RID: 17
	public enum PsaiResult
	{
		// Token: 0x04000081 RID: 129
		none,
		// Token: 0x04000082 RID: 130
		OK,
		// Token: 0x04000083 RID: 131
		alreadyActive,
		// Token: 0x04000084 RID: 132
		badCommand,
		// Token: 0x04000085 RID: 133
		channelAllocFailed,
		// Token: 0x04000086 RID: 134
		channelStolen,
		// Token: 0x04000087 RID: 135
		error_file,
		// Token: 0x04000088 RID: 136
		file_couldNotSeek,
		// Token: 0x04000089 RID: 137
		file_diskEjected,
		// Token: 0x0400008A RID: 138
		file_eof,
		// Token: 0x0400008B RID: 139
		file_notFound,
		// Token: 0x0400008C RID: 140
		format_error,
		// Token: 0x0400008D RID: 141
		initialization_error,
		// Token: 0x0400008E RID: 142
		internal_error,
		// Token: 0x0400008F RID: 143
		invalidHandle,
		// Token: 0x04000090 RID: 144
		invalidParam,
		// Token: 0x04000091 RID: 145
		memory_error,
		// Token: 0x04000092 RID: 146
		notReady,
		// Token: 0x04000093 RID: 147
		error_createBufferFailed,
		// Token: 0x04000094 RID: 148
		output_format_error,
		// Token: 0x04000095 RID: 149
		output_init_failed,
		// Token: 0x04000096 RID: 150
		output_failure,
		// Token: 0x04000097 RID: 151
		update_error,
		// Token: 0x04000098 RID: 152
		error_version,
		// Token: 0x04000099 RID: 153
		unknown_theme,
		// Token: 0x0400009A RID: 154
		essential_segment_missing,
		// Token: 0x0400009B RID: 155
		commandIgnored,
		// Token: 0x0400009C RID: 156
		triggerDenied,
		// Token: 0x0400009D RID: 157
		triggerIgnoredFollowingThemeAlreadySet,
		// Token: 0x0400009E RID: 158
		triggerIgnoredLowPriority,
		// Token: 0x0400009F RID: 159
		commandIgnoredMenuModeActive,
		// Token: 0x040000A0 RID: 160
		commandIgnoredCutsceneActive,
		// Token: 0x040000A1 RID: 161
		no_basicmood_set
	}
}
