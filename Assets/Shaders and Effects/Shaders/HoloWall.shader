Shader "Custom/HoloWall" 
{
Properties 
    {
        _MainTex ("Particle Texture", 2D) = "red" {}
        _Color("Color", Color) = (1,1,1)
        _ScrollXSpeed ("X Scroll Speed", Range(-5, 5)) = 0
        _ScrollYSpeed ("Y Scroll Speed", Range(-5, 5)) = 0
        _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
        _RimColor("Rim Color" , Color) = (1,1,1,1)
        _Cube ("Cubemap", CUBE) = "" {}
    }

SubShader 
    {
    Tags {"RenderType"="Transparent"
              "Queue" = "Transparent"}
        ZWrite On
        
        Blend SrcAlpha One
        //Blend One One
        

    

    CGPROGRAM
    #pragma surface surf Lambert
    
    
    sampler2D _MainTex;
    samplerCUBE _Cube;
    float3 _Color;
    fixed _ScrollXSpeed;
    fixed _ScrollYSpeed;
    float _RimPower;
    float4 _RimColor;

    struct Input
    {
        float2 uv_MainTex;
        float3 worldRefl;
        float3 viewDir;
    };

    void surf (Input IN, inout SurfaceOutput o)
    {
         fixed varX = _ScrollXSpeed * _Time;
         fixed varY = _ScrollYSpeed * _Time;
         fixed2 uv_Tex = IN.uv_MainTex + fixed2(varX, varY);
        //
        // half3 c = tex2D (_MainTex, uv_Tex ).rgb * 0.5;
        
        fixed4 tex = tex2D (_MainTex, uv_Tex);
        o.Albedo = tex.rgb * _Color;
        half rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
        o.Emission = _RimColor.rgb * pow(rim, _RimPower) * _Color;
        //o.Alpha     = c.a;
        
    }
     fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
          return fixed4(0,0,0,0);//half4(s.Albedo, s.Alpha);
     }
    
    ENDCG
} 
//Fallback "Diffuse"
}