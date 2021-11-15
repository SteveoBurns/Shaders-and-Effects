Shader "Unlit/VertOffsetPowerUp"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WaveAmp("Wave Amplitude", Range(0,0.5)) = 0.1
        _ColorA("Color A", color) = (1,1,1,1)
        _ColorB("Color B", color) = (0,0,0,0)
        _Speed("Speed", Range(0,1)) = 1
        _Radius("Radius", float) = 1
        _InsideRadius("Inner Radius", float) = 1
    }
    SubShader
    {
       Tags { "RenderType"="Transparent"
                "Queue" = "Transparent"}
        //LOD 100

        Pass
        {
            Cull Off // this renders the inside of shapes also. Doesnt cull any of the rendering. back - culls the rear, front - culls the front side.
            Zwrite Off // don't write to the depth buffer, z - buffer
            //ZTest Always
            
            Blend One One // additive lightens
            //Blend DstColor Zero // Multiply darkens
            //Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #define TAU 6.283

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _WaveAmp;
            float4 _ColorA;
            float4 _ColorB;
            float _Speed;
            float _Radius;
            float _InsideRadius;

            float GetWave(float2 uv)
            {
                float2 uvsCentered = uv * 2 -1;
                float radialDistance = length(uvsCentered);                
                float wave = cos((radialDistance + _Time.y *_Speed) * TAU * 3);
                wave *= 3 -radialDistance;

                return wave;
            }

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.y = GetWave(v.uv) * _WaveAmp; // the _WaveAmp reduces the amplitude of the wave
                if (v.vertex.y <=0)
                {
                    // Maybe in here I can convert the black to alpha/transparent?
                    //v.vertex.y = 0;
                    //clip(-_WaveAmp * 0.5);
                }
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 coords = i.uv * 2-1;
                float dist = length(coords)-_Radius;
                float distInside = length(coords)- _InsideRadius;
                
                clip(-dist);
                clip(distInside);
                
                float wave = GetWave(i.uv);
                //wave = normalize(wave);

                
                wave *= 1+_Radius; 
                float4 outColor = lerp(_ColorA, _ColorB, _Radius * -1.5); // Lerping between the colors based on the Radius
                return float4(outColor*wave.xxx,1);

            }
            
            ENDCG
        }
    }
}
