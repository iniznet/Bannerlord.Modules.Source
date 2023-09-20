using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000027 RID: 39
	[ApplicationInterfaceBase]
	internal interface IScene
	{
		// Token: 0x060002FF RID: 767
		[EngineMethod("create_new_scene", false)]
		Scene CreateNewScene(bool initialize_physics, bool enable_decals = true, int atlasGroup = 0, string sceneName = "mono_renderscene");

		// Token: 0x06000300 RID: 768
		[EngineMethod("get_path_between_ai_face_pointers", false)]
		bool GetPathBetweenAIFacePointers(UIntPtr scenePointer, UIntPtr startingAiFace, UIntPtr endingAiFace, Vec2 startingPosition, Vec2 endingPosition, float agentRadius, Vec2[] result, ref int pathSize);

		// Token: 0x06000301 RID: 769
		[EngineMethod("get_path_between_ai_face_indices", false)]
		bool GetPathBetweenAIFaceIndices(UIntPtr scenePointer, int startingAiFace, int endingAiFace, Vec2 startingPosition, Vec2 endingPosition, float agentRadius, Vec2[] result, ref int pathSize);

		// Token: 0x06000302 RID: 770
		[EngineMethod("get_path_distance_between_ai_faces", false)]
		bool GetPathDistanceBetweenAIFaces(UIntPtr scenePointer, int startingAiFace, int endingAiFace, Vec2 startingPosition, Vec2 endingPosition, float agentRadius, float distanceLimit, out float distance);

		// Token: 0x06000303 RID: 771
		[EngineMethod("get_nav_mesh_face_index", false)]
		void GetNavMeshFaceIndex(UIntPtr scenePointer, ref PathFaceRecord record, Vec2 position, bool checkIfDisabled, bool ignoreHeight);

		// Token: 0x06000304 RID: 772
		[EngineMethod("is_multiplayer_scene", false)]
		bool IsMultiplayerScene(Scene scene);

		// Token: 0x06000305 RID: 773
		[EngineMethod("take_photo_mode_picture", false)]
		string TakePhotoModePicture(Scene scene, bool saveAmbientOcclusionPass, bool saveObjectIdPass, bool saveShadowPass);

		// Token: 0x06000306 RID: 774
		[EngineMethod("get_all_color_grade_names", false)]
		string GetAllColorGradeNames(Scene scene);

		// Token: 0x06000307 RID: 775
		[EngineMethod("get_all_filter_names", false)]
		string GetAllFilterNames(Scene scene);

		// Token: 0x06000308 RID: 776
		[EngineMethod("get_photo_mode_roll", false)]
		float GetPhotoModeRoll(Scene scene);

		// Token: 0x06000309 RID: 777
		[EngineMethod("get_photo_mode_fov", false)]
		float GetPhotoModeFov(Scene scene);

		// Token: 0x0600030A RID: 778
		[EngineMethod("get_photo_mode_orbit", false)]
		bool GetPhotoModeOrbit(Scene scene);

		// Token: 0x0600030B RID: 779
		[EngineMethod("get_photo_mode_on", false)]
		bool GetPhotoModeOn(Scene scene);

		// Token: 0x0600030C RID: 780
		[EngineMethod("get_photo_mode_focus", false)]
		void GetPhotoModeFocus(Scene scene, ref float focus, ref float focusStart, ref float focusEnd, ref float exposure, ref bool vignetteOn);

		// Token: 0x0600030D RID: 781
		[EngineMethod("get_scene_color_grade_index", false)]
		int GetSceneColorGradeIndex(Scene scene);

		// Token: 0x0600030E RID: 782
		[EngineMethod("get_scene_filter_index", false)]
		int GetSceneFilterIndex(Scene scene);

		// Token: 0x0600030F RID: 783
		[EngineMethod("get_loading_state_name", false)]
		string GetLoadingStateName(Scene scene);

		// Token: 0x06000310 RID: 784
		[EngineMethod("set_photo_mode_roll", false)]
		void SetPhotoModeRoll(Scene scene, float roll);

		// Token: 0x06000311 RID: 785
		[EngineMethod("set_photo_mode_fov", false)]
		void SetPhotoModeFov(Scene scene, float verticalFov);

		// Token: 0x06000312 RID: 786
		[EngineMethod("set_photo_mode_orbit", false)]
		void SetPhotoModeOrbit(Scene scene, bool orbit);

		// Token: 0x06000313 RID: 787
		[EngineMethod("set_photo_mode_on", false)]
		void SetPhotoModeOn(Scene scene, bool on);

		// Token: 0x06000314 RID: 788
		[EngineMethod("set_photo_mode_focus", false)]
		void SetPhotoModeFocus(Scene scene, float focusStart, float focusEnd, float focus, float exposure);

		// Token: 0x06000315 RID: 789
		[EngineMethod("set_photo_mode_vignette", false)]
		void SetPhotoModeVignette(Scene scene, bool vignetteOn);

		// Token: 0x06000316 RID: 790
		[EngineMethod("set_scene_color_grade_index", false)]
		void SetSceneColorGradeIndex(Scene scene, int index);

		// Token: 0x06000317 RID: 791
		[EngineMethod("set_scene_filter_index", false)]
		int SetSceneFilterIndex(Scene scene, int index);

		// Token: 0x06000318 RID: 792
		[EngineMethod("set_scene_color_grade", false)]
		void SetSceneColorGrade(Scene scene, string textureName);

		// Token: 0x06000319 RID: 793
		[EngineMethod("get_water_level", false)]
		float GetWaterLevel(Scene scene);

		// Token: 0x0600031A RID: 794
		[EngineMethod("get_water_level_at_position", false)]
		float GetWaterLevelAtPosition(Scene scene, Vec2 position, bool checkWaterBodyEntities);

		// Token: 0x0600031B RID: 795
		[EngineMethod("get_terrain_material_index_at_layer", false)]
		int GetTerrainPhysicsMaterialIndexAtLayer(Scene scene, int layerIndex);

		// Token: 0x0600031C RID: 796
		[EngineMethod("create_burst_particle", false)]
		void CreateBurstParticle(Scene scene, int particleId, ref MatrixFrame frame);

		// Token: 0x0600031D RID: 797
		[EngineMethod("get_nav_mesh_face_index3", false)]
		void GetNavMeshFaceIndex3(UIntPtr scenePointer, ref PathFaceRecord record, Vec3 position, bool checkIfDisabled);

		// Token: 0x0600031E RID: 798
		[EngineMethod("set_upgrade_level", false)]
		void SetUpgradeLevel(UIntPtr scenePointer, int level);

		// Token: 0x0600031F RID: 799
		[EngineMethod("create_path_mesh", false)]
		MetaMesh CreatePathMesh(UIntPtr scenePointer, string baseEntityName, bool isWaterPath);

		// Token: 0x06000320 RID: 800
		[EngineMethod("set_active_visibility_levels", false)]
		void SetActiveVisibilityLevels(UIntPtr scenePointer, string levelsAppended);

		// Token: 0x06000321 RID: 801
		[EngineMethod("set_terrain_dynamic_params", false)]
		void SetTerrainDynamicParams(UIntPtr scenePointer, Vec3 dynamic_params);

		// Token: 0x06000322 RID: 802
		[EngineMethod("set_do_not_wait_for_loading_states_to_render", false)]
		void SetDoNotWaitForLoadingStatesToRender(UIntPtr scenePointer, bool value);

		// Token: 0x06000323 RID: 803
		[EngineMethod("create_path_mesh2", false)]
		MetaMesh CreatePathMesh2(UIntPtr scenePointer, UIntPtr[] pathNodes, int pathNodeCount, bool isWaterPath);

		// Token: 0x06000324 RID: 804
		[EngineMethod("clear_all", false)]
		void ClearAll(UIntPtr scenePointer);

		// Token: 0x06000325 RID: 805
		[EngineMethod("check_resources", false)]
		void CheckResources(UIntPtr scenePointer);

		// Token: 0x06000326 RID: 806
		[EngineMethod("force_load_resources", false)]
		void ForceLoadResources(UIntPtr scenePointer);

		// Token: 0x06000327 RID: 807
		[EngineMethod("check_path_entities_frame_changed", false)]
		bool CheckPathEntitiesFrameChanged(UIntPtr scenePointer, string containsName);

		// Token: 0x06000328 RID: 808
		[EngineMethod("tick", false)]
		void Tick(UIntPtr scenePointer, float deltaTime);

		// Token: 0x06000329 RID: 809
		[EngineMethod("add_entity_with_mesh", false)]
		void AddEntityWithMesh(UIntPtr scenePointer, UIntPtr meshPointer, ref MatrixFrame frame);

		// Token: 0x0600032A RID: 810
		[EngineMethod("add_entity_with_multi_mesh", false)]
		void AddEntityWithMultiMesh(UIntPtr scenePointer, UIntPtr multiMeshPointer, ref MatrixFrame frame);

		// Token: 0x0600032B RID: 811
		[EngineMethod("add_item_entity", false)]
		GameEntity AddItemEntity(UIntPtr scenePointer, ref MatrixFrame frame, UIntPtr meshPointer);

		// Token: 0x0600032C RID: 812
		[EngineMethod("remove_entity", false)]
		void RemoveEntity(UIntPtr scenePointer, UIntPtr entityId, int removeReason);

		// Token: 0x0600032D RID: 813
		[EngineMethod("attach_entity", false)]
		bool AttachEntity(UIntPtr scenePointer, UIntPtr entity, bool showWarnings);

		// Token: 0x0600032E RID: 814
		[EngineMethod("get_terrain_height_and_normal", false)]
		void GetTerrainHeightAndNormal(UIntPtr scenePointer, Vec2 position, out float height, out Vec3 normal);

		// Token: 0x0600032F RID: 815
		[EngineMethod("resume_loading_renderings", false)]
		void ResumeLoadingRenderings(UIntPtr scenePointer);

		// Token: 0x06000330 RID: 816
		[EngineMethod("get_upgrade_level_mask", false)]
		uint GetUpgradeLevelMask(UIntPtr scenePointer);

		// Token: 0x06000331 RID: 817
		[EngineMethod("set_upgrade_level_visibility", false)]
		void SetUpgradeLevelVisibility(UIntPtr scenePointer, string concatLevels);

		// Token: 0x06000332 RID: 818
		[EngineMethod("set_upgrade_level_visibility_with_mask", false)]
		void SetUpgradeLevelVisibilityWithMask(UIntPtr scenePointer, uint mask);

		// Token: 0x06000333 RID: 819
		[EngineMethod("stall_loading_renderings", false)]
		void StallLoadingRenderingsUntilFurtherNotice(UIntPtr scenePointer);

		// Token: 0x06000334 RID: 820
		[EngineMethod("get_flora_instance_count", false)]
		int GetFloraInstanceCount(UIntPtr scenePointer);

		// Token: 0x06000335 RID: 821
		[EngineMethod("get_flora_renderer_texture_usage", false)]
		int GetFloraRendererTextureUsage(UIntPtr scenePointer);

		// Token: 0x06000336 RID: 822
		[EngineMethod("get_terrain_memory_usage", false)]
		int GetTerrainMemoryUsage(UIntPtr scenePointer);

		// Token: 0x06000337 RID: 823
		[EngineMethod("get_nav_mesh_face_count", false)]
		int GetNavMeshFaceCount(UIntPtr scenePointer);

		// Token: 0x06000338 RID: 824
		[EngineMethod("get_nav_mesh_face_center_position", false)]
		void GetNavMeshFaceCenterPosition(UIntPtr scenePointer, int navMeshFace, ref Vec3 centerPos);

		// Token: 0x06000339 RID: 825
		[EngineMethod("get_id_of_nav_mesh_face", false)]
		int GetIdOfNavMeshFace(UIntPtr scenePointer, int navMeshFace);

		// Token: 0x0600033A RID: 826
		[EngineMethod("set_cloth_simulation_state", false)]
		void SetClothSimulationState(UIntPtr scenePointer, bool state);

		// Token: 0x0600033B RID: 827
		[EngineMethod("get_first_entity_with_name", false)]
		GameEntity GetFirstEntityWithName(UIntPtr scenePointer, string entityName);

		// Token: 0x0600033C RID: 828
		[EngineMethod("get_campaign_entity_with_name", false)]
		GameEntity GetCampaignEntityWithName(UIntPtr scenePointer, string entityName);

		// Token: 0x0600033D RID: 829
		[EngineMethod("get_all_entities_with_script_component", false)]
		void GetAllEntitiesWithScriptComponent(UIntPtr scenePointer, string scriptComponentName, UIntPtr output);

		// Token: 0x0600033E RID: 830
		[EngineMethod("get_first_entity_with_script_component", false)]
		GameEntity GetFirstEntityWithScriptComponent(UIntPtr scenePointer, string scriptComponentName);

		// Token: 0x0600033F RID: 831
		[EngineMethod("get_upgrade_level_mask_of_level_name", false)]
		uint GetUpgradeLevelMaskOfLevelName(UIntPtr scenePointer, string levelName);

		// Token: 0x06000340 RID: 832
		[EngineMethod("get_level_name_of_level_index", false)]
		string GetUpgradeLevelNameOfIndex(UIntPtr scenePointer, int index);

		// Token: 0x06000341 RID: 833
		[EngineMethod("get_upgrade_level_count", false)]
		int GetUpgradeLevelCount(UIntPtr scenePointer);

		// Token: 0x06000342 RID: 834
		[EngineMethod("get_winter_time_factor", false)]
		float GetWinterTimeFactor(UIntPtr scenePointer);

		// Token: 0x06000343 RID: 835
		[EngineMethod("get_nav_mesh_face_first_vertex_z", false)]
		float GetNavMeshFaceFirstVertexZ(UIntPtr scenePointer, int navMeshFaceIndex);

		// Token: 0x06000344 RID: 836
		[EngineMethod("set_winter_time_factor", false)]
		void SetWinterTimeFactor(UIntPtr scenePointer, float winterTimeFactor);

		// Token: 0x06000345 RID: 837
		[EngineMethod("set_dryness_factor", false)]
		void SetDrynessFactor(UIntPtr scenePointer, float drynessFactor);

		// Token: 0x06000346 RID: 838
		[EngineMethod("get_fog", false)]
		float GetFog(UIntPtr scenePointer);

		// Token: 0x06000347 RID: 839
		[EngineMethod("set_fog", false)]
		void SetFog(UIntPtr scenePointer, float fogDensity, ref Vec3 fogColor, float fogFalloff);

		// Token: 0x06000348 RID: 840
		[EngineMethod("set_fog_advanced", false)]
		void SetFogAdvanced(UIntPtr scenePointer, float fogFalloffOffset, float fogFalloffMinFog, float fogFalloffStartDist);

		// Token: 0x06000349 RID: 841
		[EngineMethod("set_fog_ambient_color", false)]
		void SetFogAmbientColor(UIntPtr scenePointer, ref Vec3 fogAmbientColor);

		// Token: 0x0600034A RID: 842
		[EngineMethod("set_temperature", false)]
		void SetTemperature(UIntPtr scenePointer, float temperature);

		// Token: 0x0600034B RID: 843
		[EngineMethod("set_humidity", false)]
		void SetHumidity(UIntPtr scenePointer, float humidity);

		// Token: 0x0600034C RID: 844
		[EngineMethod("set_dynamic_shadowmap_cascades_radius_multiplier", false)]
		void SetDynamicShadowmapCascadesRadiusMultiplier(UIntPtr scenePointer, float extraRadius);

		// Token: 0x0600034D RID: 845
		[EngineMethod("set_env_map_multiplier", false)]
		void SetEnvironmentMultiplier(UIntPtr scenePointer, bool useMultiplier, float multiplier);

		// Token: 0x0600034E RID: 846
		[EngineMethod("set_sky_rotation", false)]
		void SetSkyRotation(UIntPtr scenePointer, float rotation);

		// Token: 0x0600034F RID: 847
		[EngineMethod("set_sky_brightness", false)]
		void SetSkyBrightness(UIntPtr scenePointer, float brightness);

		// Token: 0x06000350 RID: 848
		[EngineMethod("set_forced_snow", false)]
		void SetForcedSnow(UIntPtr scenePointer, bool value);

		// Token: 0x06000351 RID: 849
		[EngineMethod("set_sun", false)]
		void SetSun(UIntPtr scenePointer, Vec3 color, float altitude, float angle, float intensity);

		// Token: 0x06000352 RID: 850
		[EngineMethod("set_sun_angle_altitude", false)]
		void SetSunAngleAltitude(UIntPtr scenePointer, float angle, float altitude);

		// Token: 0x06000353 RID: 851
		[EngineMethod("set_sun_light", false)]
		void SetSunLight(UIntPtr scenePointer, Vec3 color, Vec3 direction);

		// Token: 0x06000354 RID: 852
		[EngineMethod("set_sun_direction", false)]
		void SetSunDirection(UIntPtr scenePointer, Vec3 direction);

		// Token: 0x06000355 RID: 853
		[EngineMethod("set_sun_size", false)]
		void SetSunSize(UIntPtr scenePointer, float size);

		// Token: 0x06000356 RID: 854
		[EngineMethod("set_sunshafts_strength", false)]
		void SetSunShaftStrength(UIntPtr scenePointer, float strength);

		// Token: 0x06000357 RID: 855
		[EngineMethod("get_rain_density", false)]
		float GetRainDensity(UIntPtr scenePointer);

		// Token: 0x06000358 RID: 856
		[EngineMethod("set_rain_density", false)]
		void SetRainDensity(UIntPtr scenePointer, float density);

		// Token: 0x06000359 RID: 857
		[EngineMethod("get_snow_density", false)]
		float GetSnowDensity(UIntPtr scenePointer);

		// Token: 0x0600035A RID: 858
		[EngineMethod("set_snow_density", false)]
		void SetSnowDensity(UIntPtr scenePointer, float density);

		// Token: 0x0600035B RID: 859
		[EngineMethod("add_decal_instance", false)]
		void AddDecalInstance(UIntPtr scenePointer, UIntPtr decalMeshPointer, string decalSetID, bool deletable);

		// Token: 0x0600035C RID: 860
		[EngineMethod("set_shadow", false)]
		void SetShadow(UIntPtr scenePointer, bool shadowEnabled);

		// Token: 0x0600035D RID: 861
		[EngineMethod("add_point_light", false)]
		int AddPointLight(UIntPtr scenePointer, Vec3 position, float radius);

		// Token: 0x0600035E RID: 862
		[EngineMethod("add_directional_light", false)]
		int AddDirectionalLight(UIntPtr scenePointer, Vec3 position, Vec3 direction, float radius);

		// Token: 0x0600035F RID: 863
		[EngineMethod("set_light_position", false)]
		void SetLightPosition(UIntPtr scenePointer, int lightIndex, Vec3 position);

		// Token: 0x06000360 RID: 864
		[EngineMethod("set_light_diffuse_color", false)]
		void SetLightDiffuseColor(UIntPtr scenePointer, int lightIndex, Vec3 diffuseColor);

		// Token: 0x06000361 RID: 865
		[EngineMethod("set_light_direction", false)]
		void SetLightDirection(UIntPtr scenePointer, int lightIndex, Vec3 direction);

		// Token: 0x06000362 RID: 866
		[EngineMethod("calculate_effective_lighting", false)]
		bool CalculateEffectiveLighting(UIntPtr scenePointer);

		// Token: 0x06000363 RID: 867
		[EngineMethod("set_rayleigh_constant", false)]
		void SetMieScatterStrength(UIntPtr scenePointer, float strength);

		// Token: 0x06000364 RID: 868
		[EngineMethod("set_mie_scatter_particle_size", false)]
		void SetMieScatterFocus(UIntPtr scenePointer, float strength);

		// Token: 0x06000365 RID: 869
		[EngineMethod("set_brightpass_threshold", false)]
		void SetBrightpassTreshold(UIntPtr scenePointer, float threshold);

		// Token: 0x06000366 RID: 870
		[EngineMethod("set_min_exposure", false)]
		void SetMinExposure(UIntPtr scenePointer, float minExposure);

		// Token: 0x06000367 RID: 871
		[EngineMethod("set_max_exposure", false)]
		void SetMaxExposure(UIntPtr scenePointer, float maxExposure);

		// Token: 0x06000368 RID: 872
		[EngineMethod("set_target_exposure", false)]
		void SetTargetExposure(UIntPtr scenePointer, float targetExposure);

		// Token: 0x06000369 RID: 873
		[EngineMethod("set_middle_gray", false)]
		void SetMiddleGray(UIntPtr scenePointer, float middleGray);

		// Token: 0x0600036A RID: 874
		[EngineMethod("set_bloom_strength", false)]
		void SetBloomStrength(UIntPtr scenePointer, float bloomStrength);

		// Token: 0x0600036B RID: 875
		[EngineMethod("set_bloom_amount", false)]
		void SetBloomAmount(UIntPtr scenePointer, float bloomAmount);

		// Token: 0x0600036C RID: 876
		[EngineMethod("set_grain_amount", false)]
		void SetGrainAmount(UIntPtr scenePointer, float grainAmount);

		// Token: 0x0600036D RID: 877
		[EngineMethod("set_lens_flare_amount", false)]
		void SetLensFlareAmount(UIntPtr scenePointer, float lensFlareAmount);

		// Token: 0x0600036E RID: 878
		[EngineMethod("set_lens_flare_threshold", false)]
		void SetLensFlareThreshold(UIntPtr scenePointer, float lensFlareThreshold);

		// Token: 0x0600036F RID: 879
		[EngineMethod("set_lens_flare_strength", false)]
		void SetLensFlareStrength(UIntPtr scenePointer, float lensFlareStrength);

		// Token: 0x06000370 RID: 880
		[EngineMethod("set_lens_flare_dirt_weight", false)]
		void SetLensFlareDirtWeight(UIntPtr scenePointer, float lensFlareDirtWeight);

		// Token: 0x06000371 RID: 881
		[EngineMethod("set_lens_flare_diffraction_weight", false)]
		void SetLensFlareDiffractionWeight(UIntPtr scenePointer, float lensFlareDiffractionWeight);

		// Token: 0x06000372 RID: 882
		[EngineMethod("set_lens_flare_halo_weight", false)]
		void SetLensFlareHaloWeight(UIntPtr scenePointer, float lensFlareHaloWeight);

		// Token: 0x06000373 RID: 883
		[EngineMethod("set_lens_flare_ghost_weight", false)]
		void SetLensFlareGhostWeight(UIntPtr scenePointer, float lensFlareGhostWeight);

		// Token: 0x06000374 RID: 884
		[EngineMethod("set_lens_flare_halo_width", false)]
		void SetLensFlareHaloWidth(UIntPtr scenePointer, float lensFlareHaloWidth);

		// Token: 0x06000375 RID: 885
		[EngineMethod("set_lens_flare_ghost_samples", false)]
		void SetLensFlareGhostSamples(UIntPtr scenePointer, int lensFlareGhostSamples);

		// Token: 0x06000376 RID: 886
		[EngineMethod("set_lens_flare_aberration_offset", false)]
		void SetLensFlareAberrationOffset(UIntPtr scenePointer, float lensFlareAberrationOffset);

		// Token: 0x06000377 RID: 887
		[EngineMethod("set_lens_flare_blur_size", false)]
		void SetLensFlareBlurSize(UIntPtr scenePointer, int lensFlareBlurSize);

		// Token: 0x06000378 RID: 888
		[EngineMethod("set_lens_flare_blur_sigma", false)]
		void SetLensFlareBlurSigma(UIntPtr scenePointer, float lensFlareBlurSigma);

		// Token: 0x06000379 RID: 889
		[EngineMethod("set_streak_amount", false)]
		void SetStreakAmount(UIntPtr scenePointer, float streakAmount);

		// Token: 0x0600037A RID: 890
		[EngineMethod("set_streak_threshold", false)]
		void SetStreakThreshold(UIntPtr scenePointer, float streakThreshold);

		// Token: 0x0600037B RID: 891
		[EngineMethod("set_streak_strength", false)]
		void SetStreakStrength(UIntPtr scenePointer, float strengthAmount);

		// Token: 0x0600037C RID: 892
		[EngineMethod("set_streak_stretch", false)]
		void SetStreakStretch(UIntPtr scenePointer, float stretchAmount);

		// Token: 0x0600037D RID: 893
		[EngineMethod("set_streak_intensity", false)]
		void SetStreakIntensity(UIntPtr scenePointer, float stretchAmount);

		// Token: 0x0600037E RID: 894
		[EngineMethod("set_streak_tint", false)]
		void SetStreakTint(UIntPtr scenePointer, ref Vec3 p_streak_tint_color);

		// Token: 0x0600037F RID: 895
		[EngineMethod("set_hexagon_vignette_color", false)]
		void SetHexagonVignetteColor(UIntPtr scenePointer, ref Vec3 p_hexagon_vignette_color);

		// Token: 0x06000380 RID: 896
		[EngineMethod("set_hexagon_vignette_alpha", false)]
		void SetHexagonVignetteAlpha(UIntPtr scenePointer, float Alpha);

		// Token: 0x06000381 RID: 897
		[EngineMethod("set_vignette_inner_radius", false)]
		void SetVignetteInnerRadius(UIntPtr scenePointer, float vignetteInnerRadius);

		// Token: 0x06000382 RID: 898
		[EngineMethod("set_vignette_outer_radius", false)]
		void SetVignetteOuterRadius(UIntPtr scenePointer, float vignetteOuterRadius);

		// Token: 0x06000383 RID: 899
		[EngineMethod("set_vignette_opacity", false)]
		void SetVignetteOpacity(UIntPtr scenePointer, float vignetteOpacity);

		// Token: 0x06000384 RID: 900
		[EngineMethod("set_aberration_offset", false)]
		void SetAberrationOffset(UIntPtr scenePointer, float aberrationOffset);

		// Token: 0x06000385 RID: 901
		[EngineMethod("set_aberration_size", false)]
		void SetAberrationSize(UIntPtr scenePointer, float aberrationSize);

		// Token: 0x06000386 RID: 902
		[EngineMethod("set_aberration_smooth", false)]
		void SetAberrationSmooth(UIntPtr scenePointer, float aberrationSmooth);

		// Token: 0x06000387 RID: 903
		[EngineMethod("set_lens_distortion", false)]
		void SetLensDistortion(UIntPtr scenePointer, float lensDistortion);

		// Token: 0x06000388 RID: 904
		[EngineMethod("get_height_at_point", false)]
		bool GetHeightAtPoint(UIntPtr scenePointer, Vec2 point, BodyFlags excludeBodyFlags, ref float height);

		// Token: 0x06000389 RID: 905
		[EngineMethod("get_entity_count", false)]
		int GetEntityCount(UIntPtr scenePointer);

		// Token: 0x0600038A RID: 906
		[EngineMethod("get_entities", false)]
		void GetEntities(UIntPtr scenePointer, UIntPtr entityObjectsArrayPointer);

		// Token: 0x0600038B RID: 907
		[EngineMethod("get_root_entity_count", false)]
		int GetRootEntityCount(UIntPtr scenePointer);

		// Token: 0x0600038C RID: 908
		[EngineMethod("get_root_entities", false)]
		void GetRootEntities(Scene scene, NativeObjectArray output);

		// Token: 0x0600038D RID: 909
		[EngineMethod("get_entity_with_guid", false)]
		GameEntity GetEntityWithGuid(UIntPtr scenePointer, string guid);

		// Token: 0x0600038E RID: 910
		[EngineMethod("select_entities_in_box_with_script_component", false)]
		int SelectEntitiesInBoxWithScriptComponent(UIntPtr scenePointer, ref Vec3 boundingBoxMin, ref Vec3 boundingBoxMax, UIntPtr[] entitiesOutput, int maxCount, string scriptComponentName);

		// Token: 0x0600038F RID: 911
		[EngineMethod("select_entities_collided_with", false)]
		int SelectEntitiesCollidedWith(UIntPtr scenePointer, ref Ray ray, UIntPtr[] entityIds, Intersection[] intersections);

		// Token: 0x06000390 RID: 912
		[EngineMethod("generate_contacts_with_capsule", false)]
		int GenerateContactsWithCapsule(UIntPtr scenePointer, ref CapsuleData cap, BodyFlags exclude_flags, Intersection[] intersections);

		// Token: 0x06000391 RID: 913
		[EngineMethod("invalidate_terrain_physics_materials", false)]
		void InvalidateTerrainPhysicsMaterials(UIntPtr scenePointer);

		// Token: 0x06000392 RID: 914
		[EngineMethod("read", false)]
		void Read(UIntPtr scenePointer, string sceneName, ref SceneInitializationData initData, string forcedAtmoName);

		// Token: 0x06000393 RID: 915
		[EngineMethod("read_and_calculate_initial_camera", false)]
		void ReadAndCalculateInitialCamera(UIntPtr scenePointer, ref MatrixFrame outFrame);

		// Token: 0x06000394 RID: 916
		[EngineMethod("optimize_scene", false)]
		void OptimizeScene(UIntPtr scenePointer, bool optimizeFlora, bool optimizeOro);

		// Token: 0x06000395 RID: 917
		[EngineMethod("get_terrain_height", false)]
		float GetTerrainHeight(UIntPtr scenePointer, Vec2 position, bool checkHoles);

		// Token: 0x06000396 RID: 918
		[EngineMethod("get_normal_at", false)]
		Vec3 GetNormalAt(UIntPtr scenePointer, Vec2 position);

		// Token: 0x06000397 RID: 919
		[EngineMethod("has_terrain_heightmap", false)]
		bool HasTerrainHeightmap(UIntPtr scenePointer);

		// Token: 0x06000398 RID: 920
		[EngineMethod("contains_terrain", false)]
		bool ContainsTerrain(UIntPtr scenePointer);

		// Token: 0x06000399 RID: 921
		[EngineMethod("set_dof_focus", false)]
		void SetDofFocus(UIntPtr scenePointer, float dofFocus);

		// Token: 0x0600039A RID: 922
		[EngineMethod("set_dof_params", false)]
		void SetDofParams(UIntPtr scenePointer, float dofFocusStart, float dofFocusEnd, bool isVignetteOn);

		// Token: 0x0600039B RID: 923
		[EngineMethod("get_last_final_render_camera_position", false)]
		Vec3 GetLastFinalRenderCameraPosition(UIntPtr scenePointer);

		// Token: 0x0600039C RID: 924
		[EngineMethod("get_last_final_render_camera_frame", false)]
		void GetLastFinalRenderCameraFrame(UIntPtr scenePointer, ref MatrixFrame outFrame);

		// Token: 0x0600039D RID: 925
		[EngineMethod("get_time_of_day", false)]
		float GetTimeOfDay(UIntPtr scenePointer);

		// Token: 0x0600039E RID: 926
		[EngineMethod("set_time_of_day", false)]
		void SetTimeOfDay(UIntPtr scenePointer, float value);

		// Token: 0x0600039F RID: 927
		[EngineMethod("is_atmosphere_indoor", false)]
		bool IsAtmosphereIndoor(UIntPtr scenePointer);

		// Token: 0x060003A0 RID: 928
		[EngineMethod("set_color_grade_blend", false)]
		void SetColorGradeBlend(UIntPtr scenePointer, string texture1, string texture2, float alpha);

		// Token: 0x060003A1 RID: 929
		[EngineMethod("preload_for_rendering", false)]
		void PreloadForRendering(UIntPtr scenePointer);

		// Token: 0x060003A2 RID: 930
		[EngineMethod("resume_scene_sounds", false)]
		void ResumeSceneSounds(UIntPtr scenePointer);

		// Token: 0x060003A3 RID: 931
		[EngineMethod("finish_scene_sounds", false)]
		void FinishSceneSounds(UIntPtr scenePointer);

		// Token: 0x060003A4 RID: 932
		[EngineMethod("pause_scene_sounds", false)]
		void PauseSceneSounds(UIntPtr scenePointer);

		// Token: 0x060003A5 RID: 933
		[EngineMethod("get_ground_height_at_position", false)]
		float GetGroundHeightAtPosition(UIntPtr scenePointer, Vec3 position, uint excludeFlags);

		// Token: 0x060003A6 RID: 934
		[EngineMethod("get_ground_height_and_normal_at_position", false)]
		float GetGroundHeightAndNormalAtPosition(UIntPtr scenePointer, Vec3 position, ref Vec3 normal, uint excludeFlags);

		// Token: 0x060003A7 RID: 935
		[EngineMethod("check_point_can_see_point", false)]
		bool CheckPointCanSeePoint(UIntPtr scenePointer, Vec3 sourcePoint, Vec3 targetPoint, float distanceToCheck);

		// Token: 0x060003A8 RID: 936
		[EngineMethod("ray_cast_for_closest_entity_or_terrain", false)]
		bool RayCastForClosestEntityOrTerrain(UIntPtr scenePointer, ref Vec3 sourcePoint, ref Vec3 targetPoint, float rayThickness, ref float collisionDistance, ref Vec3 closestPoint, ref UIntPtr entityIndex, BodyFlags bodyExcludeFlags);

		// Token: 0x060003A9 RID: 937
		[EngineMethod("box_cast_only_for_camera", false)]
		bool BoxCastOnlyForCamera(UIntPtr scenePointer, Vec3[] boxPoints, ref Vec3 centerPoint, ref Vec3 dir, float distance, ref float collisionDistance, ref Vec3 closestPoint, ref UIntPtr entityIndex, BodyFlags bodyExcludeFlags, bool preFilter, bool postFilter);

		// Token: 0x060003AA RID: 938
		[EngineMethod("mark_faces_with_id_as_ladder", false)]
		void MarkFacesWithIdAsLadder(UIntPtr scenePointer, int faceGroupId, bool isLadder);

		// Token: 0x060003AB RID: 939
		[EngineMethod("box_cast", false)]
		bool BoxCast(UIntPtr scenePointer, ref Vec3 boxPointBegin, ref Vec3 boxPointEnd, ref Vec3 dir, float distance, ref float collisionDistance, ref Vec3 closestPoint, ref UIntPtr entityIndex, BodyFlags bodyExcludeFlags);

		// Token: 0x060003AC RID: 940
		[EngineMethod("set_ability_of_faces_with_id", false)]
		void SetAbilityOfFacesWithId(UIntPtr scenePointer, int faceGroupId, bool isEnabled);

		// Token: 0x060003AD RID: 941
		[EngineMethod("swap_face_connections_with_id", false)]
		void SwapFaceConnectionsWithId(UIntPtr scenePointer, int hubFaceGroupID, int toBeSeparatedFaceGroupId, int toBeMergedFaceGroupId);

		// Token: 0x060003AE RID: 942
		[EngineMethod("merge_faces_with_id", false)]
		void MergeFacesWithId(UIntPtr scenePointer, int faceGroupId0, int faceGroupId1, int newFaceGroupId);

		// Token: 0x060003AF RID: 943
		[EngineMethod("separate_faces_with_id", false)]
		void SeparateFacesWithId(UIntPtr scenePointer, int faceGroupId0, int faceGroupId1);

		// Token: 0x060003B0 RID: 944
		[EngineMethod("is_any_face_with_id", false)]
		bool IsAnyFaceWithId(UIntPtr scenePointer, int faceGroupId);

		// Token: 0x060003B1 RID: 945
		[EngineMethod("load_nav_mesh_prefab", false)]
		void LoadNavMeshPrefab(UIntPtr scenePointer, string navMeshPrefabName, int navMeshGroupIdShift);

		// Token: 0x060003B2 RID: 946
		[EngineMethod("get_navigation_mesh_face_for_position", false)]
		bool GetNavigationMeshFaceForPosition(UIntPtr scenePointer, ref Vec3 position, ref int faceGroupId, float heightDifferenceLimit);

		// Token: 0x060003B3 RID: 947
		[EngineMethod("get_path_distance_between_positions", false)]
		bool GetPathDistanceBetweenPositions(UIntPtr scenePointer, ref WorldPosition position, ref WorldPosition destination, float agentRadius, ref float pathLength);

		// Token: 0x060003B4 RID: 948
		[EngineMethod("is_line_to_point_clear", false)]
		bool IsLineToPointClear(UIntPtr scenePointer, int startingFace, Vec2 position, Vec2 destination, float agentRadius);

		// Token: 0x060003B5 RID: 949
		[EngineMethod("is_line_to_point_clear2", false)]
		bool IsLineToPointClear2(UIntPtr scenePointer, UIntPtr startingFace, Vec2 position, Vec2 destination, float agentRadius);

		// Token: 0x060003B6 RID: 950
		[EngineMethod("get_last_point_on_navigation_mesh_from_position_to_destination", false)]
		Vec2 GetLastPointOnNavigationMeshFromPositionToDestination(UIntPtr scenePointer, int startingFace, Vec2 position, Vec2 destination);

		// Token: 0x060003B7 RID: 951
		[EngineMethod("get_last_point_on_navigation_mesh_from_world_position_to_destination", false)]
		Vec3 GetLastPointOnNavigationMeshFromWorldPositionToDestination(UIntPtr scenePointer, ref WorldPosition position, Vec2 destination);

		// Token: 0x060003B8 RID: 952
		[EngineMethod("does_path_exist_between_positions", false)]
		bool DoesPathExistBetweenPositions(UIntPtr scenePointer, WorldPosition position, WorldPosition destination);

		// Token: 0x060003B9 RID: 953
		[EngineMethod("does_path_exist_between_faces", false)]
		bool DoesPathExistBetweenFaces(UIntPtr scenePointer, int firstNavMeshFace, int secondNavMeshFace, bool ignoreDisabled);

		// Token: 0x060003BA RID: 954
		[EngineMethod("ensure_postfx_system", false)]
		void EnsurePostfxSystem(UIntPtr scenePointer);

		// Token: 0x060003BB RID: 955
		[EngineMethod("set_bloom", false)]
		void SetBloom(UIntPtr scenePointer, bool mode);

		// Token: 0x060003BC RID: 956
		[EngineMethod("set_dof_mode", false)]
		void SetDofMode(UIntPtr scenePointer, bool mode);

		// Token: 0x060003BD RID: 957
		[EngineMethod("set_occlusion_mode", false)]
		void SetOcclusionMode(UIntPtr scenePointer, bool mode);

		// Token: 0x060003BE RID: 958
		[EngineMethod("set_external_injection_texture", false)]
		void SetExternalInjectionTexture(UIntPtr scenePointer, UIntPtr texturePointer);

		// Token: 0x060003BF RID: 959
		[EngineMethod("set_sunshaft_mode", false)]
		void SetSunshaftMode(UIntPtr scenePointer, bool mode);

		// Token: 0x060003C0 RID: 960
		[EngineMethod("get_sun_direction", false)]
		Vec3 GetSunDirection(UIntPtr scenePointer);

		// Token: 0x060003C1 RID: 961
		[EngineMethod("get_north_angle", false)]
		float GetNorthAngle(UIntPtr scenePointer);

		// Token: 0x060003C2 RID: 962
		[EngineMethod("get_terrain_min_max_height", false)]
		bool GetTerrainMinMaxHeight(Scene scene, ref float min, ref float max);

		// Token: 0x060003C3 RID: 963
		[EngineMethod("get_physics_min_max", false)]
		void GetPhysicsMinMax(UIntPtr scenePointer, ref Vec3 min_max);

		// Token: 0x060003C4 RID: 964
		[EngineMethod("is_editor_scene", false)]
		bool IsEditorScene(UIntPtr scenePointer);

		// Token: 0x060003C5 RID: 965
		[EngineMethod("set_motionblur_mode", false)]
		void SetMotionBlurMode(UIntPtr scenePointer, bool mode);

		// Token: 0x060003C6 RID: 966
		[EngineMethod("set_antialiasing_mode", false)]
		void SetAntialiasingMode(UIntPtr scenePointer, bool mode);

		// Token: 0x060003C7 RID: 967
		[EngineMethod("set_dlss_mode", false)]
		void SetDLSSMode(UIntPtr scenePointer, bool mode);

		// Token: 0x060003C8 RID: 968
		[EngineMethod("get_path_with_name", false)]
		Path GetPathWithName(UIntPtr scenePointer, string name);

		// Token: 0x060003C9 RID: 969
		[EngineMethod("get_soft_boundary_vertex_count", false)]
		int GetSoftBoundaryVertexCount(UIntPtr scenePointer);

		// Token: 0x060003CA RID: 970
		[EngineMethod("delete_path_with_name", false)]
		void DeletePathWithName(UIntPtr scenePointer, string name);

		// Token: 0x060003CB RID: 971
		[EngineMethod("get_hard_boundary_vertex_count", false)]
		int GetHardBoundaryVertexCount(UIntPtr scenePointer);

		// Token: 0x060003CC RID: 972
		[EngineMethod("get_hard_boundary_vertex", false)]
		Vec2 GetHardBoundaryVertex(UIntPtr scenePointer, int index);

		// Token: 0x060003CD RID: 973
		[EngineMethod("add_path", false)]
		void AddPath(UIntPtr scenePointer, string name);

		// Token: 0x060003CE RID: 974
		[EngineMethod("get_soft_boundary_vertex", false)]
		Vec2 GetSoftBoundaryVertex(UIntPtr scenePointer, int index);

		// Token: 0x060003CF RID: 975
		[EngineMethod("add_path_point", false)]
		void AddPathPoint(UIntPtr scenePointer, string name, ref MatrixFrame frame);

		// Token: 0x060003D0 RID: 976
		[EngineMethod("get_bounding_box", false)]
		void GetBoundingBox(UIntPtr scenePointer, ref Vec3 min, ref Vec3 max);

		// Token: 0x060003D1 RID: 977
		[EngineMethod("set_name", false)]
		void SetName(UIntPtr scenePointer, string name);

		// Token: 0x060003D2 RID: 978
		[EngineMethod("get_name", false)]
		string GetName(UIntPtr scenePointer);

		// Token: 0x060003D3 RID: 979
		[EngineMethod("get_module_path", false)]
		string GetModulePath(UIntPtr scenePointer);

		// Token: 0x060003D4 RID: 980
		[EngineMethod("set_time_speed", false)]
		void SetTimeSpeed(UIntPtr scenePointer, float value);

		// Token: 0x060003D5 RID: 981
		[EngineMethod("get_time_speed", false)]
		float GetTimeSpeed(UIntPtr scenePointer);

		// Token: 0x060003D6 RID: 982
		[EngineMethod("set_owner_thread", false)]
		void SetOwnerThread(UIntPtr scenePointer);

		// Token: 0x060003D7 RID: 983
		[EngineMethod("get_number_of_path_with_name_prefix", false)]
		int GetNumberOfPathsWithNamePrefix(UIntPtr ptr, string prefix);

		// Token: 0x060003D8 RID: 984
		[EngineMethod("get_paths_with_name_prefix", false)]
		void GetPathsWithNamePrefix(UIntPtr ptr, UIntPtr[] points, string prefix);

		// Token: 0x060003D9 RID: 985
		[EngineMethod("set_use_constant_time", false)]
		void SetUseConstantTime(UIntPtr ptr, bool value);

		// Token: 0x060003DA RID: 986
		[EngineMethod("set_play_sound_events_after_render_ready", false)]
		void SetPlaySoundEventsAfterReadyToRender(UIntPtr ptr, bool value);

		// Token: 0x060003DB RID: 987
		[EngineMethod("disable_static_shadows", false)]
		void DisableStaticShadows(UIntPtr ptr, bool value);

		// Token: 0x060003DC RID: 988
		[EngineMethod("get_skybox_mesh", false)]
		Mesh GetSkyboxMesh(UIntPtr ptr);

		// Token: 0x060003DD RID: 989
		[EngineMethod("set_atmosphere_with_name", false)]
		void SetAtmosphereWithName(UIntPtr ptr, string name);

		// Token: 0x060003DE RID: 990
		[EngineMethod("fill_entity_with_hard_border_physics_barrier", false)]
		void FillEntityWithHardBorderPhysicsBarrier(UIntPtr scenePointer, UIntPtr entityPointer);

		// Token: 0x060003DF RID: 991
		[EngineMethod("clear_decals", false)]
		void ClearDecals(UIntPtr scenePointer);

		// Token: 0x060003E0 RID: 992
		[EngineMethod("get_scripted_entity_count", false)]
		int GetScriptedEntityCount(UIntPtr scenePointer);

		// Token: 0x060003E1 RID: 993
		[EngineMethod("get_scripted_entity", false)]
		GameEntity GetScriptedEntity(UIntPtr scenePointer, int index);

		// Token: 0x060003E2 RID: 994
		[EngineMethod("world_position_validate_z", false)]
		void WorldPositionValidateZ(ref WorldPosition position, int minimumValidityState);

		// Token: 0x060003E3 RID: 995
		[EngineMethod("world_position_compute_nearest_nav_mesh", false)]
		void WorldPositionComputeNearestNavMesh(ref WorldPosition position);

		// Token: 0x060003E4 RID: 996
		[EngineMethod("get_node_data_count", false)]
		int GetNodeDataCount(Scene scene, int xIndex, int yIndex);

		// Token: 0x060003E5 RID: 997
		[EngineMethod("fill_terrain_height_data", false)]
		void FillTerrainHeightData(Scene scene, int xIndex, int yIndex, float[] heightArray);

		// Token: 0x060003E6 RID: 998
		[EngineMethod("fill_terrain_physics_material_index_data", false)]
		void FillTerrainPhysicsMaterialIndexData(Scene scene, int xIndex, int yIndex, short[] materialIndexArray);

		// Token: 0x060003E7 RID: 999
		[EngineMethod("get_terrain_data", false)]
		void GetTerrainData(Scene scene, out Vec2i nodeDimension, out float nodeSize, out int layerCount, out int layerVersion);

		// Token: 0x060003E8 RID: 1000
		[EngineMethod("get_terrain_node_data", false)]
		void GetTerrainNodeData(Scene scene, int xIndex, int yIndex, out int vertexCountAlongAxis, out float quadLength, out float minHeight, out float maxHeight);
	}
}
