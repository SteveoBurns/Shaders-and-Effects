Shader "Custom/Blink" 
{
Properties 
    {
        _MainTex ("Particle Texture", 2D) = "red" {}
        _Blink ( "Blink", Float ) = 0
        _Color("Color", Color) = (1,1,1)
        _ScrollXSpeed ("X Scroll Speed", Range(-5, 5)) = 0
        _ScrollYSpeed ("Y Scroll Speed", Range(-5, 5)) = 0
    }

SubShader 
    {
    Tags {"RenderType"="Transparent"}
        ZWrite On
        
        Blend SrcAlpha One
        
        

    

    CGPROGRAM
    #pragma surface surf Lambert
    
    
    sampler2D _MainTex;
    float _Blink;
    float3 _Color;
    fixed _ScrollXSpeed;
    fixed _ScrollYSpeed;

    struct Input
    {
        float2 uv_MainTex;
    };

    void surf (Input IN, inout SurfaceOutput o)
    {
        fixed varX = _ScrollXSpeed * _Time;
        fixed varY = _ScrollYSpeed * _Time;
        fixed2 uv_Tex = IN.uv_MainTex + fixed2(varX, varY);
        
        half4 c = tex2D (_MainTex, uv_Tex );
        if( _Blink == 1.0f ) 
            c *=  ( 0.2f + abs( sin( _Time.w ) ) );
        o.Albedo    = c.rgb*_Color;
        o.Emission = c.rgb*_Color.rgb;
        o.Alpha     = c.a;
        
    }
    fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
         return fixed4(0,0,0,0);//half4(s.Albedo, s.Alpha);
     }
    
    ENDCG
} 
//Fallback "Diffuse"
}