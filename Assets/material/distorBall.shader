Shader "Custom/distorBall" {
	Properties{
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

	struct v2f {
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
	};

	v2f vert(appdata_base v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord;
		return o;
	}

	fixed4 _Color;

	fixed4 frag(v2f i) : COLOR
	{
		float2 v = (i.uv-float2(0.5,0.5)) * 50.0;

		float x = v.x;
		float y = v.y;
	
		float t = _Time.y * 20.0;
		float r;
		for (int i = 0; i < 5; i++) {
			float d = 3.14159265 / float(500) * float(i) * 5.0;
			r = length(float2(x,y)) + 0.01;
			float xx = x;
			x = x + cos(y + cos(2.*r) + d+t);
			y = y - sin(xx + cos(2.*r) + d+t);
		}

		float4 animatioRing = float4(float3(cos(r*(atan(.1))), cos(r*(atan(.1))), cos(r*(atan(.1)))), 1.0);


		if (animatioRing.r < 0.1 && animatioRing.g < 0.1 && animatioRing.b < 0.1)
		{
			animatioRing.a = 0.0;
		}
		else 
		{
			animatioRing = lerp(animatioRing, _Color, 0.5);
		}
		return animatioRing;
	}

		ENDCG
	}

	}
		FallBack "Diffuse"
}
