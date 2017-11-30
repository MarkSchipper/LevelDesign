// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Ice" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGBA)", 2D) = "white" {}
		_Metallic("Metallic (RGBA)", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "white" {}

		_SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
		_SliceAmount("Slice Amount", Range(0.0,1.0)) = 0.5

	}


	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200

			Name "Base"
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
			
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows alpha vertex: vert fragment:frag

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _Metallic;
			sampler2D _BumpMap;
			sampler2D _SliceGuide;

			float _SliceAmount;

			struct Input {
				float2 uv_MainTex;
				float2 uv_Metallic;
				float2 uv_BumpMap;
			};

			void surf(Input IN, inout SurfaceOutputStandard o) {
				
				clip(tex2D(_SliceGuide, IN.uv_MainTex * 3) - _SliceAmount);

				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 m = tex2D(_Metallic, IN.uv_Metallic);
				o.Albedo = c.rgb;
				o.Metallic = m;
				o.Smoothness = m.a;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

				o.Alpha = c.a;
			}

			ENDCG
		}
		

}
