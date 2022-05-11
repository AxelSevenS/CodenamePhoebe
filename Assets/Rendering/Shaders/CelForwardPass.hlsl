uniform half3 _AmbientLight;

half3 CelLight(half3 normalWS, half3 viewDirectionWS, half3 lightDirectionWS, half3 lightColor, half lightAttenuation, half3 sssColor = half3(0,0,0)){
    half NdotL = saturate(dot(normalWS, lightDirectionWS));
    half finalAtten = NdotL * lightAttenuation;

    half shadowSize = 0.15;
    half shadowBlend = 0.4;
    // finalAtten = step(0.5, finalAtten);
    finalAtten = smoothstep(shadowSize, shadowSize + shadowBlend, finalAtten);

    half3 radiance = lightColor * _AmbientLight;
    half3 shade = (_AmbientLight*0.07) * radiance;

    half3 finalColor = lerp(shade, radiance, finalAtten);
    
    [branch] if (length(sssColor) > 0){
        finalColor = lerp( lerp(shade, radiance*sssColor, saturate(finalAtten )), radiance, finalAtten );
    }



    // Highlights
    // half3 halfDir = normalize(normalize(viewDirectionWS) + lightDirectionWS);
    // half NdotH = saturate(dot(normalWS, halfDir));

    // half highlightSize = 0.1;
    // half highlightBlend = 0.15;
    // half highlight = smoothstep(highlightSize, highlightSize + highlightBlend, NdotH * NdotH);

    // Rimlight
    // half viewToLight = saturate(dot(lightDirectionWS, -viewDirectionWS));
    // half viewToSurface = saturate(dot(normalWS, -viewDirectionWS));
    // half fresnel = (1 - dot(viewDirectionWS, normalWS));
    
    // half rimLight = fresnel * viewToLight * (1-viewToSurface);
    // rimLight = smoothstep(shadowSize, shadowSize + shadowBlend, rimLight);

    return finalColor;
}

half3 CelLight(half3 normalWS, half3 viewDirectionWS, Light light, half3 sssColor = half3(0,0,0)){
    return CelLight(normalWS, viewDirectionWS, light.direction, light.color * light.distanceAttenuation, light.shadowAttenuation, sssColor);
}

// half3 FragLightingCel(BRDFData brdfData, BRDFData brdfDataClearCoat, Light light, half3 normalWS, half3 viewDirectionWS, half clearCoatMask, bool specularHighlightsOff) {
//     half3 color = CelLight(normalWS, viewDirectionWS, light);

//     half3 brdf = brdfData.diffuse;
//     // #ifndef _SPECULARHIGHLIGHTS_OFF
//     // [branch] if (!specularHighlightsOff) {
//     //     brdf += brdfData.specular * DirectBRDFSpecular(brdfData, normalWS, light.direction, viewDirectionWS);

//     //     // #if defined(_CLEARCOAT) || defined(_CLEARCOATMAP)
//     //     //     // Clear coat evaluates the specular a second time and has some common terms with the base specular.
//     //     //     // We rely on the compiler to merge these and compute them only once.
//     //     //     half brdfCoat = kDielectricSpec.r * DirectBRDFSpecular(brdfDataClearCoat, normalWS, light.direction, viewDirectionWS);

//     //     //     // Mix clear coat and base layer using khronos glTF recommended formula
//     //     //     // https://github.com/KhronosGroup/glTF/blob/master/extensions/2.0/Khronos/KHR_materials_clearcoat/README.md
//     //     //     // Use NoV for direct too instead of LoH as an optimization (NoV is light invariant).
//     //     //     half NoV = saturate(dot(normalWS, viewDirectionWS));
//     //     //     // Use slightly simpler fresnelTerm (Pow4 vs Pow5) as a small optimization.
//     //     //     // It is matching fresnel used in the GI/Env, so should produce a consistent clear coat blend (env vs. direct)
//     //     //     half coatFresnel = kDielectricSpec.x + kDielectricSpec.a * Pow4(1.0 - NoV);

//     //     //     brdf = brdf * (1.0 - clearCoatMask * coatFresnel) + brdfCoat * clearCoatMask;
//     //     // #endif // _CLEARCOAT
//     // }
//     // #endif // _SPECULARHIGHLIGHTS_OFF

//     return brdf * color.xyz;
// }

half3 VertLightingCel(float3 positionWS, half3 viewDirectionWS, half3 normalWS) {
    half3 vertexLightColor = half3(0.0, 0.0, 0.0);

    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        uint lightsCount = GetAdditionalLightsCount();
        for (uint lightIndex = 0u; lightIndex < lightsCount; ++lightIndex) {
            Light light = GetAdditionalLight(lightIndex, positionWS);
            vertexLightColor += CelLight(normalWS, viewDirectionWS, light);
        }
    #endif

    return vertexLightColor;
}

half4 FragmentCel(InputData inputData, SurfaceData surfaceData) {
    #ifdef _SPECULARHIGHLIGHTS_OFF
        bool specularHighlightsOff = true;
    #else
        bool specularHighlightsOff = false;
    #endif

    BRDFData brdfData;

    // NOTE: can modify alpha
    InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, half3(0,0,0), surfaceData.smoothness, surfaceData.alpha, brdfData);

    BRDFData brdfDataClearCoat = (BRDFData)0;
    #if defined(_CLEARCOAT) || defined(_CLEARCOATMAP)
        // base brdfData is modified here, rely on the compiler to eliminate dead computation by InitializeBRDFData()
        InitializeBRDFDataClearCoat(surfaceData.clearCoatMask, surfaceData.clearCoatSmoothness, brdfData, brdfDataClearCoat);
    #endif

    // To ensure backward compatibility we have to avoid using shadowMask input, as it is not present in older shaders
    #if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
        half4 shadowMask = inputData.shadowMask;
    #elif !defined (LIGHTMAP_ON)
        half4 shadowMask = unity_ProbesOcclusion;
    #else
        half4 shadowMask = half4(1, 1, 1, 1);
    #endif

    Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, shadowMask);

    #if defined(_SCREEN_SPACE_OCCLUSION)
        AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(inputData.normalizedScreenSpaceUV);
        mainLight.color *= aoFactor.directAmbientOcclusion;
        surfaceData.occlusion = min(surfaceData.occlusion, aoFactor.indirectAmbientOcclusion);
    #endif

    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);
    half3 color = GlobalIllumination(brdfData, brdfDataClearCoat, surfaceData.clearCoatMask, inputData.bakedGI, surfaceData.occlusion, inputData.normalWS, inputData.viewDirectionWS);
    color += brdfData.diffuse * CelLight(inputData.normalWS, inputData.viewDirectionWS, mainLight, surfaceData.specular).xyz;

    #ifdef _ADDITIONAL_LIGHTS
        uint pixelLightCount = GetAdditionalLightsCount();
        for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex) {
            Light light = GetAdditionalLight(lightIndex, inputData.positionWS, shadowMask);
            #if defined(_SCREEN_SPACE_OCCLUSION)
                light.color *= aoFactor.directAmbientOcclusion;
            #endif
            color += brdfData.diffuse * CelLight(inputData.normalWS, inputData.viewDirectionWS, light).xyz;
        }
    #endif

    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        color += inputData.vertexLighting * brdfData.diffuse;
    #endif

    color += surfaceData.emission;

    return half4(color, surfaceData.alpha);
}

void BuildInputData(Varyings input, SurfaceDescription surfaceDescription, out InputData inputData) {
    inputData.positionWS = input.positionWS;

    #ifdef _NORMALMAP
        #if _NORMAL_DROPOFF_TS
            // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
            float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
            float3 bitangent = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
            inputData.normalWS = TransformTangentToWorld(surfaceDescription.NormalTS, half3x3(input.tangentWS.xyz, bitangent, input.normalWS.xyz));
        #elif _NORMAL_DROPOFF_OS
            inputData.normalWS = TransformObjectToWorldNormal(surfaceDescription.NormalOS);
        #elif _NORMAL_DROPOFF_WS
            inputData.normalWS = surfaceDescription.NormalWS;
        #endif
    #else
        inputData.normalWS = input.normalWS;
    #endif
    inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
    inputData.viewDirectionWS = SafeNormalize(input.viewDirectionWS);

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        inputData.shadowCoord = input.shadowCoord;
    #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
        inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
    #else
        inputData.shadowCoord = float4(0, 0, 0, 0);
    #endif

    inputData.fogCoord = input.fogFactorAndVertexLight.x;
    inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
    inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.sh, inputData.normalWS);
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
    inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUV);
}

Varyings BuildCelVaryings(Attributes input) {
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    #if defined(FEATURES_GRAPH_VERTEX)
        // Evaluate Vertex Graph
        VertexDescriptionInputs vertexDescriptionInputs = BuildVertexDescriptionInputs(input);
        VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs);

        #if defined(CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC)
            CustomInterpolatorPassThroughFunc(output, vertexDescription);
        #endif

        // Assign modified vertex attributes
        input.positionOS = vertexDescription.Position;
        #if defined(VARYINGS_NEED_NORMAL_WS)
            input.normalOS = vertexDescription.Normal;
        #endif //FEATURES_GRAPH_NORMAL
        #if defined(VARYINGS_NEED_TANGENT_WS)
            input.tangentOS.xyz = vertexDescription.Tangent.xyz;
        #endif //FEATURES GRAPH TANGENT
    #endif //FEATURES_GRAPH_VERTEX

    // TODO: Avoid path via VertexPositionInputs (Universal)
    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

    // Returns the camera relative position (if enabled)
    float3 positionWS = TransformObjectToWorld(input.positionOS);
    half3 viewDirectionWS = GetWorldSpaceViewDir(positionWS);

    #ifdef ATTRIBUTES_NEED_NORMAL
        float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
    #else
        // Required to compile ApplyVertexModification that doesn't use normal.
        float3 normalWS = float3(0.0, 0.0, 0.0);
    #endif

    #ifdef ATTRIBUTES_NEED_TANGENT
        float4 tangentWS = float4(TransformObjectToWorldDir(input.tangentOS.xyz), input.tangentOS.w);
    #endif

        // TODO: Change to inline ifdef
        // Do vertex modification in camera relative space (if enabled)
    #if defined(HAVE_VERTEX_MODIFICATION)
        ApplyVertexModification(input, normalWS, positionWS, _TimeParameters.xyz);
    #endif

    #ifdef VARYINGS_NEED_POSITION_WS
        output.positionWS = positionWS;
    #endif

    #ifdef VARYINGS_NEED_NORMAL_WS
        output.normalWS = normalWS;         // normalized in TransformObjectToWorldNormal()
    #endif

    #ifdef VARYINGS_NEED_TANGENT_WS
        output.tangentWS = tangentWS;       // normalized in TransformObjectToWorldDir()
    #endif

    #if (SHADERPASS == SHADERPASS_SHADOWCASTER)
        // Define shadow pass specific clip position for Universal
        #if _CASTING_PUNCTUAL_LIGHT_SHADOW
            float3 lightDirectionWS = normalize(_LightPosition - positionWS);
        #else
            float3 lightDirectionWS = _LightDirection;
        #endif

        output.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
        #if UNITY_REVERSED_Z
            output.positionCS.z = min(output.positionCS.z, UNITY_NEAR_CLIP_VALUE);
        #else
            output.positionCS.z = max(output.positionCS.z, UNITY_NEAR_CLIP_VALUE);
        #endif
    #elif (SHADERPASS == SHADERPASS_META)
        output.positionCS = UnityMetaVertexPosition(input.positionOS, input.uv1, input.uv2, unity_LightmapST, unity_DynamicLightmapST);
    #else
        output.positionCS = TransformWorldToHClip(positionWS);
    #endif

    #if defined(VARYINGS_NEED_TEXCOORD0) || defined(VARYINGS_DS_NEED_TEXCOORD0)
        output.texCoord0 = input.uv0;
    #endif
    #ifdef EDITOR_VISUALIZATION
        float2 VizUV = 0;
        float4 LightCoord = 0;
        UnityEditorVizData(input.positionOS, input.uv0, input.uv1, input.uv2, VizUV, LightCoord);
    #endif

    #if defined(VARYINGS_NEED_TEXCOORD1) || defined(VARYINGS_DS_NEED_TEXCOORD1)
        #ifdef EDITOR_VISUALIZATION
            output.texCoord1 = float4(VizUV, 0, 0);
        #else
            output.texCoord1 = input.uv1;
        #endif
    #endif

    #if defined(VARYINGS_NEED_TEXCOORD2) || defined(VARYINGS_DS_NEED_TEXCOORD2)
        #ifdef EDITOR_VISUALIZATION
            output.texCoord2 = LightCoord;
        #else
            output.texCoord2 = input.uv2;
        #endif
    #endif

    #if defined(VARYINGS_NEED_TEXCOORD3) || defined(VARYINGS_DS_NEED_TEXCOORD3)
        output.texCoord3 = input.uv3;
    #endif

    #if defined(VARYINGS_NEED_COLOR) || defined(VARYINGS_DS_NEED_COLOR)
        output.color = input.color;
    #endif

    #ifdef VARYINGS_NEED_VIEWDIRECTION_WS
        // Need the unnormalized direction here as otherwise interpolation is incorrect.
        // It is normalized after interpolation in the fragment shader.
        output.viewDirectionWS = viewDirectionWS;
    #endif

    #ifdef VARYINGS_NEED_SCREENPOSITION
        output.screenPosition = vertexInput.positionNDC;
    #endif

    #if (SHADERPASS == SHADERPASS_FORWARD) || (SHADERPASS == SHADERPASS_GBUFFER)
        OUTPUT_LIGHTMAP_UV(input.uv1, unity_LightmapST, output.staticLightmapUV);
        #if defined(DYNAMICLIGHTMAP_ON)
            output.dynamicLightmapUV.xy = input.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
        #endif
        OUTPUT_SH(normalWS, output.sh);
    #endif

    #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
        half fogFactor = 0;
        #if !defined(_FOG_FRAGMENT)
            fogFactor = ComputeFogFactor(output.positionCS.z);
        #endif
        half3 vertexLight = VertLightingCel(positionWS, viewDirectionWS, normalWS);
        output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
    #endif

    #if defined(VARYINGS_NEED_SHADOW_COORD) && defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        output.shadowCoord = GetShadowCoord(vertexInput);
    #endif

    return output;
}

PackedVaryings vert(Attributes input) {
    Varyings output = (Varyings)0;
    output = BuildCelVaryings(input);
    PackedVaryings packedOutput = (PackedVaryings)0;
    packedOutput = PackVaryings(output);
    return packedOutput;
}

half4 frag(PackedVaryings packedInput) : SV_TARGET {

    Varyings unpacked = UnpackVaryings(packedInput);
    UNITY_SETUP_INSTANCE_ID(unpacked);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(unpacked);

    SurfaceDescriptionInputs surfaceDescriptionInputs = BuildSurfaceDescriptionInputs(unpacked);
    SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);

    #if _AlphaClip
        half alpha = surfaceDescription.Alpha;
        clip(alpha - surfaceDescription.AlphaClipThreshold);
    #elif _SURFACE_TYPE_TRANSPARENT
        half alpha = surfaceDescription.Alpha;
    #else
        half alpha = 1;
    #endif

    InputData inputData;
    BuildInputData(unpacked, surfaceDescription, inputData);

    #ifdef _SPECULAR_SETUP
        float3 specular = surfaceDescription.Specular;
        float metallic = 1;
    #else
        float3 specular = 0;
        float metallic = surfaceDescription.Metallic;
    #endif

    SurfaceData surface         = (SurfaceData)0;
    surface.albedo              = surfaceDescription.BaseColor;
    surface.metallic            = saturate(metallic);
    surface.specular            = specular;
    surface.smoothness          = saturate(surfaceDescription.Smoothness),
    surface.occlusion           = surfaceDescription.Occlusion,
    surface.emission            = surfaceDescription.Emission,
    surface.alpha               = saturate(alpha);
    surface.clearCoatMask       = 0;
    surface.clearCoatSmoothness = 1;

    #ifdef _CLEARCOAT
        surface.clearCoatMask       = saturate(surfaceDescription.CoatMask);
        surface.clearCoatSmoothness = saturate(surfaceDescription.CoatSmoothness);
    #endif

    half4 color = FragmentCel(inputData, surface);

    color.rgb = MixFog(color.rgb, inputData.fogCoord);
    return color;
}
