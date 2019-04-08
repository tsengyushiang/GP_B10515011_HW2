﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/Mask"
{
	Properties
	{
		_MainTex("Sprite Texture", 2D) = "white" {}
	[HideInInspector] _AlphaTex("Alpha mask", 2D) = "white" {}
	_Color("Tint", Color) = (1,1,1,1)
	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
	}

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		struct appdata_t
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
		fixed4 color : COLOR;
		half2 texcoord  : TEXCOORD0;
	};

	fixed4 _Color;

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.vertex = UnityObjectToClipPos(IN.vertex);
		OUT.texcoord = IN.texcoord;
		OUT.color = IN.color * _Color;

		return OUT;
	}

	sampler2D _MainTex;
	sampler2D _AlphaTex;

	fixed4 frag(v2f IN) : COLOR
	{
		fixed4 c0 = tex2D(_MainTex, IN.texcoord) * IN.color;
	fixed4 c1 = tex2D(_AlphaTex, IN.texcoord);
	return fixed4(c0.rgb, min(c1.r, c0.a));
	}
		ENDCG
	}
	}
}
