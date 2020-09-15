Shader "Custom/Target"
{
    Properties
    {
        _OutColor ("Out Color", Color) = (1,1,1,1)
		_InColor("In Color", Color) = (1,1,1,1)
		_OutTex("Out Tex", 2D) = "white" {}
        _MainTex ("Main Tex", 2D) = "white" {}
		_PaintTex ("Paint Tex", 2D) = "white" {}
		_NormalTex("Normal Tex", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

		sampler2D _OutTex;
        sampler2D _MainTex;
		sampler2D _PaintTex;
		sampler2D _NormalTex;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv2_PaintTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _OutColor;
		fixed4 _InColor;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			fixed4 pc = tex2D(_PaintTex, IN.uv_MainTex);

			fixed4 color = _InColor + IN.uv2_PaintTex.y * (_OutColor - _InColor);

			float m = (IN.uv2_PaintTex.y > 0.999);

			color *=  m * tex2D(_OutTex, IN.uv_MainTex) + (1 - m) * color;

            fixed4 bc = tex2D (_MainTex, IN.uv_MainTex) * color;
			o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_MainTex * 2));
            o.Albedo = (1 - pc.a) * bc.rgb + pc.rgb * pc.a;
            o.Metallic = _Metallic;
            o.Smoothness = pc.a * 0.8;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
