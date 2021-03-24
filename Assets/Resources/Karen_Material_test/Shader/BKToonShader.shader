Shader "BeatifulKnights/BKToonShader"
{
	Properties
	{
		[Header(Toon Shading Setting)]
		_MainTex ("Base Shading Texture", 2D) = "white" {}
		_BaseColor("Base Shading Color", Color) = (1,1,1,1)
		_BaseColor_Step("Base Area", Range(0,1)) = 1
		_BaseShade_Falloff("Base Area Falloff",  Range(0,1)) = 1
		
		_ShadeTex ("Shading Texture", 2D) = "white" {}
		_ShadeColor1("Step1 Shading Color", Color) = (1,1,1,1)

		_ShadeColor2("Step2 Shading Color", Color) = (1,1,1,1)
		_ShadeColor_Step("Shading2 Area",Range(0,1)) = 0
		_Shade_Falloff("Shading2 Falloff", Range(0,1)) = 1

		[HideInInspector]_ShadowLevel("Shadow Level", float) = 0
		
		
		[Space(20)][Header(Mask Setting)]
		_MaskTex ("Mask(R:Metal,G:XX,B:XX) Texture", 2D) = "white" {}

		[Space(20)][Header(HighLight Setting)]
		_HighLight_Color("HighLight Color", Color) = (1,1,1,1)
		_HighColor_Power("HighLight Power",  Range(0,1)) = 0.5
		_HighColor_Range("HighLight Area", float) = 0
		_HighColor_Falloff("HighLight Blend", Range(0,1)) = 0.5
		
		[Space(20)][Header(NormalMap Setting)]_NormalMap("NormalMap", 2D) = "bump" {}
		_BumpScale("Normal Scale", Range(0,1)) = 1
		_NormalBlend("Normal Shading Blend", Range(0,1)) = 1

		[Space(20)][Header(MatCap Setting)]_MatCap_Sampler("MatCap Texture", 2D) = "white" {}
		_MatCapBlurLevel("MatCap LOD Level", Range(0,10)) = 0
		_MatCapBlend("MatCap Blend", Range(0,1)) = 0

		[Space(20)][Header(Rim Light Setting)]_RimLight_Color("Rim Light Color", Color) = (1,1,1,1)
		_RimArea("Rim Light Area", Range(0,1)) = 0
		_RimLight_Power("Rim Light Power", Range(0,1)) = 0

	
		[Space(20)][Header(Outline Setting)]_Outline_Color("Outline Color", color) = (1,1,1,1)
		_Outline_Width("Outline Width", float) = 1
		[HideInInspector] _FarDistance("Far Distance", float) = 100
		[HideInInspector] _NearDistance("Near Distance", float) = 0.5
		[HideInInspector] _Offset_Z("Offset Camera Z", float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Name "BK_Outline"
			Tags{}
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 texcoord0 : TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
			};

	
			uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
		


			uniform float _FarDistance;
			uniform float _NearDistance;
			uniform float4 _Outline_Color;
			uniform float _Outline_Width;
			uniform float _Offset_Z;
			
			VertexOutput vert (VertexInput v)
			{
				VertexOutput o = (VertexOutput)0;
				o.uv0 = v.texcoord0;
				float4 objPos = mul (unity_ObjectToWorld, float4(0,0,0,1));
				float2 Set_UV0 = o.uv0;
				
				o.normalDir = UnityObjectToWorldNormal(v.normal);

				float4 _ClipCameraPos = mul(UNITY_MATRIX_VP, float4(_WorldSpaceCameraPos.xyz,1));

				float dist = smoothstep(_FarDistance, _NearDistance, distance(objPos,_WorldSpaceCameraPos)).r;
				float Set_Outline_Width = _Outline_Width * 0.001 * dist;

				#if defined(UNITY_REVERSED_Z)
					_Offset_Z = _Offset_Z * -0.01;
				#else
					_Offset_Z = _Offset_Z * 0.01;
				#endif

				Set_Outline_Width = Set_Outline_Width * 2;
				o.pos = UnityObjectToClipPos( float4(v.vertex.xyz + normalize(v.normal) * Set_Outline_Width, 1));
				o.pos.z = o.pos.z + _Offset_Z;
				return o;
			}
			
			float4 frag (VertexOutput i) : SV_Target
			{
				float4 main_texture = tex2D(_MainTex, TRANSFORM_TEX(i.uv0, _MainTex));
				float4 col = _Outline_Color * main_texture;
				col.a = 1;

				return col;
			}
			ENDCG
		}

		Pass
		{
			Name "FORWARD"
			Tags
			{
				"LightMode" = "ForwardBase"
			}
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"

			#pragma multi_compile_fwdbase_fullshadows
			#pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal vulkan xboxone ps4 switch
            #pragma target 3.0



			uniform float4 _BaseColor;
			uniform float4 _ShadeColor1;
			uniform float4 _ShadeColor2;
			uniform float4 _HighLight_Color;
			uniform float _ShadowLevel;
			uniform float _BaseColor_Step;
			uniform float _BaseShade_Falloff;
			uniform float _ShadeColor_Step;
			uniform float _Shade_Falloff;


			uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
			uniform sampler2D _ShadeTex; uniform float4 _ShadeTex_ST;
			uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
			uniform sampler2D _MaskTex; uniform float4 _MaskTex_ST;
			uniform float _BumpScale;
			uniform float _NormalBlend;
			uniform float _HighColor_Power;
			uniform float _HighColor_Range;
			uniform float _HighColor_Falloff;

			uniform sampler2D _MatCap_Sampler; uniform float4 _MatCap_Sampler_ST;
			uniform float _MatCapBlurLevel;
			uniform float _MatCapBlend;

			uniform float _RimLight_Power;
			uniform float4 _RimLight_Color;
			uniform float _RimArea;

			fixed3 DecodeLightProbe(fixed3 N)
			{
				return ShadeSH9(float4(N,1));
			}

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 texcoord0 : TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
				float3 tangentDir : TEXCOORD3;
				float3 bitangentDir : TEXCOORD4;

				LIGHTING_COORDS(5,6)
				UNITY_FOG_COORDS(7)
			};

			VertexOutput vert (VertexInput v)
			{
				VertexOutput o = (VertexOutput)0;
				o.uv0 = v.texcoord0;
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				o.tangentDir = normalize(mul( unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
				o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir)* v.tangent.w);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				float3 lightColor = _LightColor0.rgb;
				o.pos = UnityObjectToClipPos(v.vertex);

				UNITY_TRANSFER_FOG(o, o.pos);
				//TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}

			float4 frag(VertexOutput i) : SV_TARGET 
			{
				i.normalDir = normalize(i.normalDir);
				float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				float2 Set_UV0 = i.uv0;

				float3 _NormalMap_var = UnpackScaleNormal(tex2D(_NormalMap, TRANSFORM_TEX(Set_UV0, _NormalMap )), _BumpScale);
				float3 normalLocal = _NormalMap_var.rgb;
				float3 normalDirection = normalize(mul( normalLocal, tangentTransform));
				float4 _MainTex_var = tex2D(_MainTex, TRANSFORM_TEX(Set_UV0, _MainTex));
				float4 _MaskTex_var = tex2D(_MaskTex, TRANSFORM_TEX(Set_UV0, _MaskTex));

				UNITY_LIGHT_ATTENUATION(attenuation, i, i.posWorld.xyz);
			
				float3 defaultLightDirection = normalize(UNITY_MATRIX_V[2].xyz + UNITY_MATRIX_V[1].xyz);
				float3 lightDirection = normalize(lerp(defaultLightDirection, _WorldSpaceLightPos0.xyz, any(_WorldSpaceLightPos0.xyz)));
				float3 halfDirection = normalize(viewDirection + lightDirection);

				float3 lightColor = _LightColor0.rgb;

				float3 Set_LightColor = lightColor.rgb;
				float3 Set_BaseColor = _BaseColor.rgb *  _MainTex_var.rgb * Set_LightColor;

				float3 envLightColor = DecodeLightProbe(normalDirection) < float3(1,1,1) ? DecodeLightProbe(normalDirection) : float3(1,1,1);


				float4 _ShadeMap_var1 = tex2D(_ShadeTex, TRANSFORM_TEX(Set_UV0, _ShadeTex));
				float3 Set_ShadeColor1 = _ShadeColor1.rgb * _ShadeMap_var1.rgb * Set_LightColor;
				float3 Set_ShadeColor2 = _ShadeColor2.rgb * _ShadeMap_var1.rgb * Set_LightColor;

				float3 Set_NormalBlend = lerp( i.normalDir, normalDirection, _NormalBlend);
				float _HalfLambert_var = dot(Set_NormalBlend, lightDirection) + 0.5 * 0.5;

				float Set_FinalShadowMask = saturate( 1 + (_HalfLambert_var * saturate(1.0 + _ShadowLevel) - (_BaseColor_Step - _BaseShade_Falloff)) / (_BaseColor_Step - (_BaseColor_Step - _BaseShade_Falloff)));
				float Shade2 = saturate(1+(_HalfLambert_var - (_ShadeColor_Step - _Shade_Falloff)) / (_ShadeColor_Step - (_ShadeColor_Step - _Shade_Falloff))) ;

				float3 viewNormal = (mul(UNITY_MATRIX_V, float4(Set_NormalBlend,0))).rgb;
				float2 viewNormalUV = viewNormal.rg * 0.5 + 0.5;
				float MatCap_Mask = _MaskTex_var.r;
				float4 _MatCap_Sampler_var = tex2Dlod(_MatCap_Sampler, float4(TRANSFORM_TEX(viewNormalUV, _MatCap_Sampler), 0.0, lerp(5, 0,MatCap_Mask) + _MatCapBlurLevel));
				_MatCap_Sampler_var = _MatCap_Sampler_var * step(0.01,MatCap_Mask)* min(1, (Set_FinalShadowMask + 0.4));

				float _RimArea_var = (1.0 - dot(normalDirection, viewDirection));// * (1-dot(normalDirection, lightDirection));
				float _RimLightPower_var = saturate( pow(_RimArea_var + _RimArea , exp2(lerp(0,10,_RimLight_Power)))) ;
				float3 Set_RimLight = (saturate(_RimLightPower_var) * attenuation) * _RimLight_Color;

				float _Speculer_var = dot(halfDirection, normalDirection) * 0.5 + 0.5;
				float _HighArea_var = saturate( pow(_Speculer_var + (0.01 * _HighColor_Range) , exp(lerp(1,11, _HighColor_Power))));
				float _HighColor_var = lerp( 0, _HighArea_var * attenuation * lerp(0, 1, MatCap_Mask  ), _HighColor_Falloff);
				float3 Set_HighColor_var = _HighColor_var * (_HighLight_Color.rgb * Set_LightColor);

				float3 finalColor = lerp(Set_BaseColor,lerp(Set_ShadeColor2, Set_ShadeColor1, Shade2), max(1-Set_FinalShadowMask, 1-attenuation));
				finalColor = lerp(finalColor, (1 - (1 - finalColor) * (1 - _MatCap_Sampler_var.rgb * Set_LightColor)), _MatCapBlend);
				finalColor += Set_RimLight + Set_HighColor_var;

				fixed4 finalRGBA = fixed4(finalColor,1);

				UNITY_APPLY_FOG(i.fogCoord, finalRGBA);

				return finalRGBA;
			}
			ENDCG
		}

		Pass
		{
			Name "ShadowCaster"
			Tags
			{
				"LightMode"="ShadowCaster"
			}
			Offset 1,1
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_fog
			#pragma target 3.0


			struct VertexInput
			{
				float4 vertex : POSITION;
			};

			struct VertexOutput
			{
				V2F_SHADOW_CASTER;
			};

			VertexOutput vert(VertexInput v)
			{
				VertexOutput o = (VertexOutput)0;
				o.pos = UnityObjectToClipPos( v.vertex );
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}

			float4 frag(VertexOutput i) : SV_TARGET
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
	FallBack "Legacy Shaders/vertexLit"
}
