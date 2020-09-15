Shader "Unlit/Depth"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
		Tags { "RenderType" = "Painted" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f
			{
				float depth : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(float4 pos : POSITION)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(pos);
				o.depth = abs(UnityObjectToViewPos(pos).z);
				return o;
			}

			float frag(v2f i) : SV_Target
			{
				return i.depth;			
			}
			ENDCG
		}
    }
}
