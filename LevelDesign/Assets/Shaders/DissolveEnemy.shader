// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "PlagueHunter/Dissolve" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Metallic("Metallic", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "white" {}
		_SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
		_SliceAmount("Slice Amount", Range(0.0,1.0)) = 0.5
		_Smooth("Smoothness", Range(0,1)) = 1
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "False" }
		LOD 200
		
			Blend SrcAlpha OneMinusSrcAlpha
			//Fog {Mode Exp }

			Pass {
				ZWrite On
				ColorMask 0
			}

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard  alpha
		#pragma multi_compile ___ UNITY_HDR_ON
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		#include "UnityCG.cginc"
		#pragma multi_compile_fog

		sampler2D _MainTex;
		sampler2D _Metallic;
		sampler2D _BumpMap;
		
		struct Input {
			float2 uv_MainTex;
			float2 uv_Metallic;
			half fog;
		};

		fixed4 _Color;

		sampler2D _SliceGuide;
		float _SliceAmount;
		float _Smooth;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o) {

			clip(tex2D(_SliceGuide, IN.uv_MainTex * 3) - _SliceAmount);
			//UNITY_APPLY_FOG_COLOR(IN.fog, _MainTex, unity_fogColor);
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = tex2D(_Metallic, IN.uv_Metallic) * _Smooth;
			o.Smoothness = tex2D(_Metallic, IN.uv_Metallic).a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			o.Alpha = c.a;
			
		}

	ENDCG
	}
		FallBack "Diffuse"
}