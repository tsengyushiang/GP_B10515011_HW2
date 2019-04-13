Shader "Custom/shineRing" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_Color("Color", Color) = (1, 1, 1, 1)
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Cull Off
		Blend One OneMinusSrcAlpha

		Pass{

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		sampler2D _MainTex;

	struct v2f {
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
	};

	float distance(float2 st)
	{
		float len = distance(st, float2(0.0, 0.0));
		float ri = abs(0.5 - len);
		float d = 0.1 / ri;

		return  d;
	}

	v2f vert(appdata_base v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord;
		return o;
	}

	fixed4 _Color;
	float4 _MainTex_TexelSize;

	fixed4 frag(v2f i) : COLOR
	{
		float2 uv = i.uv - float2(0.5,0.5);

		float d = distance(uv);

		return  float4(float2(d,0.0) * (sin(_Time.y * 2.7) * 0.3 + 0.4), d * (sin(_Time.y * 3.9) * 0.5 + 0.5),0.0);

	}

			ENDCG
		}
	}
		FallBack "Diffuse"
}