Shader "Unlit/UVSpace"
{
    Properties
    {
        [NoScaleOffset]_MainTex ("Main Texture", 2D) = "white" {}
		[NoScaleOffset]_BrushTex("Brush Texture", 2D) = "white" {}

		_BrushUVRect("Brush UVRect", Vector) = (1, 1, 1, 1)
		_BrushUVClamp("Brush UVClamp", Vector) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
		ZWrite Off
		ZTest Off
		Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {         
                float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 cameraUVPos : TEXCOORD1;
				float3 cameraSpaceNormal : TEXCOORD2;
            };

            sampler2D _MainTex;	
			sampler2D _BrushTex;

			float4 _BrushColor;
			float4 _BrushUVRect;
			float4 _BrushUVClamp;
	
			float4x4 _MVMatrix;
			float4x4 _PMatrix;

            v2f vert (appdata v)
            {
                v2f o;
#if UNITY_UV_STARTS_AT_TOP
				o.vertex = float4(v.uv.x * 2.0 - 1.0, (1.0 - v.uv.y) * 2.0 - 1.0, 0.0, 1.0);
#else
				o.vertex = float4(v.uv.x * 2.0 - 1.0, v.uv.y * 2.0 - 1.0, 0.0, 1.0);
#endif
				o.uv = v.uv;

				float4 viewPos = mul(_MVMatrix, v.vertex);

				o.cameraSpaceNormal = mul(_MVMatrix, v.normal);

				float4 clipPos = mul(_PMatrix, viewPos);
				o.cameraUVPos = clipPos.xy / clipPos.w * 0.5 + 0.5;

                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				float atten = 0;
				if (i.cameraUVPos.x >= _BrushUVClamp.x
					&& i.cameraUVPos.y >= _BrushUVClamp.y
					&& i.cameraUVPos.x <= _BrushUVClamp.z
					&& i.cameraUVPos.y <= _BrushUVClamp.w)
				{										
					atten = i.cameraSpaceNormal.z > 0;
				}
				
				float2 brushUV = (i.cameraUVPos.xy - _BrushUVRect.xy) / _BrushUVRect.zw;

				fixed4 brush = tex2D(_BrushTex, brushUV) * _BrushColor * atten;
				brush.rgb *= brush.a;

				fixed4 color = tex2D(_MainTex, i.uv) * (1 - brush.a) + brush;

                return color;
            }
            ENDCG
        }
    }
}
