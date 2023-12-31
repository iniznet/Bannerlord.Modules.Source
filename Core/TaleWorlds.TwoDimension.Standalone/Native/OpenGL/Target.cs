﻿using System;

namespace TaleWorlds.TwoDimension.Standalone.Native.OpenGL
{
	internal enum Target : uint
	{
		CurrentColor = 2816U,
		CurrentIndex,
		CurrentNormal,
		CURRENT_TEXTURE_COORDS,
		CURRENT_RASTER_COLOR,
		CURRENT_RASTER_INDEX,
		CURRENT_RASTER_TEXTURE_COORDS,
		CURRENT_RASTER_POSITION,
		CURRENT_RASTER_POSITION_VALID,
		CURRENT_RASTER_DISTANCE,
		POINT_SMOOTH = 2832U,
		POINT_SIZE,
		POINT_SIZE_RANGE,
		POINT_SIZE_GRANULARITY,
		LINE_SMOOTH = 2848U,
		LINE_WIDTH,
		LINE_WIDTH_RANGE,
		LINE_WIDTH_GRANULARITY,
		LINE_STIPPLE,
		LINE_STIPPLE_PATTERN,
		LINE_STIPPLE_REPEAT,
		LIST_MODE = 2864U,
		MAX_LIST_NESTING,
		LIST_BASE,
		LIST_INDEX,
		POLYGON_MODE = 2880U,
		POLYGON_SMOOTH,
		POLYGON_STIPPLE,
		EDGE_FLAG,
		CULL_FACE,
		CULL_FACE_MODE,
		FRONT_FACE,
		Lighting = 2896U,
		LIGHT_MODEL_LOCAL_VIEWER,
		LIGHT_MODEL_TWO_SIDE,
		LIGHT_MODEL_AMBIENT,
		ShadeModel,
		COLOR_MATERIAL_FACE,
		COLOR_MATERIAL_PARAMETER,
		COLOR_MATERIAL,
		Fog = 2912U,
		FOG_INDEX,
		FOG_DENSITY,
		FOG_START,
		FOG_END,
		FOG_MODE,
		FOG_COLOR,
		DEPTH_RANGE = 2928U,
		DepthTest,
		DEPTH_WRITEMASK,
		DEPTH_CLEAR_VALUE,
		DEPTH_FUNC,
		ACCUM_CLEAR_VALUE = 2944U,
		STENCIL_TEST = 2960U,
		STENCIL_CLEAR_VALUE,
		STENCIL_FUNC,
		STENCIL_VALUE_MASK,
		STENCIL_FAIL,
		STENCIL_PASS_DEPTH_FAIL,
		STENCIL_PASS_DEPTH_PASS,
		STENCIL_REF,
		STENCIL_WRITEMASK,
		MatrixMode = 2976U,
		Normalize,
		VIEWPORT,
		MODELVIEW_STACK_DEPTH,
		PROJECTION_STACK_DEPTH,
		TEXTURE_STACK_DEPTH,
		MODELVIEW_MATRIX,
		PROJECTION_MATRIX,
		TEXTURE_MATRIX,
		ATTRIB_STACK_DEPTH = 2992U,
		CLIENT_ATTRIB_STACK_DEPTH,
		AlphaTest = 3008U,
		ALPHA_TEST_FUNC,
		ALPHA_TEST_REF,
		DITHER = 3024U,
		BLEND_DST = 3040U,
		BLEND_SRC,
		Blend,
		LOGIC_OP_MODE = 3056U,
		INDEX_LOGIC_OP,
		COLOR_LOGIC_OP,
		AUX_BUFFERS = 3072U,
		DRAW_BUFFER,
		READ_BUFFER,
		SCISSOR_BOX = 3088U,
		SCISSOR_TEST,
		INDEX_CLEAR_VALUE = 3104U,
		INDEX_WRITEMASK,
		COLOR_CLEAR_VALUE,
		COLOR_WRITEMASK,
		INDEX_MODE = 3120U,
		RGBA_MODE,
		DOUBLEBUFFER,
		STEREO,
		RENDER_MODE = 3136U,
		PERSPECTIVE_CORRECTION_HINT = 3152U,
		POINT_SMOOTH_HINT,
		LINE_SMOOTH_HINT,
		POLYGON_SMOOTH_HINT,
		FOG_HINT,
		TEXTURE_GEN_S = 3168U,
		TEXTURE_GEN_T,
		TEXTURE_GEN_R,
		TEXTURE_GEN_Q,
		PIXEL_MAP_I_TO_I = 3184U,
		PIXEL_MAP_S_TO_S,
		PIXEL_MAP_I_TO_R,
		PIXEL_MAP_I_TO_G,
		PIXEL_MAP_I_TO_B,
		PIXEL_MAP_I_TO_A,
		PIXEL_MAP_R_TO_R,
		PIXEL_MAP_G_TO_G,
		PIXEL_MAP_B_TO_B,
		PIXEL_MAP_A_TO_A,
		PIXEL_MAP_I_TO_I_SIZE = 3248U,
		PIXEL_MAP_S_TO_S_SIZE,
		PIXEL_MAP_I_TO_R_SIZE,
		PIXEL_MAP_I_TO_G_SIZE,
		PIXEL_MAP_I_TO_B_SIZE,
		PIXEL_MAP_I_TO_A_SIZE,
		PIXEL_MAP_R_TO_R_SIZE,
		PIXEL_MAP_G_TO_G_SIZE,
		PIXEL_MAP_B_TO_B_SIZE,
		PIXEL_MAP_A_TO_A_SIZE,
		UNPACK_SWAP_BYTES = 3312U,
		UNPACK_LSB_FIRST,
		UNPACK_ROW_LENGTH,
		UNPACK_SKIP_ROWS,
		UNPACK_SKIP_PIXELS,
		UNPACK_ALIGNMENT,
		PACK_SWAP_BYTES = 3328U,
		PACK_LSB_FIRST,
		PACK_ROW_LENGTH,
		PACK_SKIP_ROWS,
		PACK_SKIP_PIXELS,
		PACK_ALIGNMENT,
		MAP_COLOR = 3344U,
		MAP_STENCIL,
		INDEX_SHIFT,
		INDEX_OFFSET,
		RED_SCALE,
		RED_BIAS,
		ZOOM_X,
		ZOOM_Y,
		GREEN_SCALE,
		GREEN_BIAS,
		BLUE_SCALE,
		BLUE_BIAS,
		ALPHA_SCALE,
		ALPHA_BIAS,
		DEPTH_SCALE,
		DEPTH_BIAS,
		MAX_EVAL_ORDER = 3376U,
		MAX_LIGHTS,
		MAX_CLIP_PLANES,
		MAX_TEXTURE_SIZE,
		MAX_PIXEL_MAP_TABLE,
		MAX_ATTRIB_STACK_DEPTH,
		MAX_MODELVIEW_STACK_DEPTH,
		MAX_NAME_STACK_DEPTH,
		MAX_PROJECTION_STACK_DEPTH,
		MAX_TEXTURE_STACK_DEPTH,
		MAX_VIEWPORT_DIMS,
		MAX_CLIENT_ATTRIB_STACK_DEPTH,
		SUBPIXEL_BITS = 3408U,
		INDEX_BITS,
		RED_BITS,
		GREEN_BITS,
		BLUE_BITS,
		ALPHA_BITS,
		DEPTH_BITS,
		STENCIL_BITS,
		ACCUM_RED_BITS,
		ACCUM_GREEN_BITS,
		ACCUM_BLUE_BITS,
		ACCUM_ALPHA_BITS,
		NAME_STACK_DEPTH = 3440U,
		AUTO_NORMAL = 3456U,
		MAP1_COLOR_4 = 3472U,
		MAP1_INDEX,
		MAP1_NORMAL,
		MAP1_TEXTURE_COORD_1,
		MAP1_TEXTURE_COORD_2,
		MAP1_TEXTURE_COORD_3,
		MAP1_TEXTURE_COORD_4,
		MAP1_VERTEX_3,
		MAP1_VERTEX_4,
		MAP2_COLOR_4 = 3504U,
		MAP2_INDEX,
		MAP2_NORMAL,
		MAP2_TEXTURE_COORD_1,
		MAP2_TEXTURE_COORD_2,
		MAP2_TEXTURE_COORD_3,
		MAP2_TEXTURE_COORD_4,
		MAP2_VERTEX_3,
		MAP2_VERTEX_4,
		MAP1_GRID_DOMAIN = 3536U,
		MAP1_GRID_SEGMENTS,
		MAP2_GRID_DOMAIN,
		MAP2_GRID_SEGMENTS,
		Texture1D = 3552U,
		Texture2D,
		FEEDBACK_BUFFER_POINTER = 3568U,
		FEEDBACK_BUFFER_SIZE,
		FEEDBACK_BUFFER_TYPE,
		SELECTION_BUFFER_POINTER,
		SELECTION_BUFFER_SIZE
	}
}
