Shader "Custom/HoloDisplay"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Float) = 1
    }
    SubShader {
     Tags {"Rendertype" = "Opaque"}
     CGPROGRAM
     #pragma surface surf Lambert
     
     struct Input{
         float2 uv_MainTex;
         float3 viewDir;
     };
     
     sampler2D _MainTex;
     sampler2D _BumpTex;
     float4 _RimColor;
     float _RimPower;
     
     void surf(Input IN,inout SurfaceOutput o){
         fixed4 tex = tex2D (_MainTex, IN.uv_MainTex);
         o.Albedo = tex.rgb;
         o.Normal =  UnpackNormal (tex2D (_BumpTex, IN.uv_MainTex));
         half rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
         o.Emission = _RimColor.rgb * pow(rim, _RimPower);
     }
     ENDCG
 } 
 Fallback "Diffuse"
}
