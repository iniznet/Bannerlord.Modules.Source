﻿using System;

namespace psai.net
{
	public enum PsaiResult
	{
		none,
		OK,
		alreadyActive,
		badCommand,
		channelAllocFailed,
		channelStolen,
		error_file,
		file_couldNotSeek,
		file_diskEjected,
		file_eof,
		file_notFound,
		format_error,
		initialization_error,
		internal_error,
		invalidHandle,
		invalidParam,
		memory_error,
		notReady,
		error_createBufferFailed,
		output_format_error,
		output_init_failed,
		output_failure,
		update_error,
		error_version,
		unknown_theme,
		essential_segment_missing,
		commandIgnored,
		triggerDenied,
		triggerIgnoredFollowingThemeAlreadySet,
		triggerIgnoredLowPriority,
		commandIgnoredMenuModeActive,
		commandIgnoredCutsceneActive,
		no_basicmood_set
	}
}
