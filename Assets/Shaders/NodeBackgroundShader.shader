Shader "Custom/NodeBackgroundShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("MainTexture", 2D) = "white" {}

        _Speed ("Speed", Range(0.1, 5)) = 1
        _ContrastStrength ("Contrast Strength", Range(0, 1)) = 0.1
        _Darkness ("Darkness", Range(0, 2)) = 1
    }
    SubShader
    {
        Tags {  "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True" }
        LOD 100

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            Stencil {
                Ref 1  //Customize this value
                Comp Equal //Customize the compare function
                Pass Keep
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            float3 rgb_to_hsv_no_clip(float3 RGB)
            {
                float3 HSV;

                float minChannel, maxChannel;
                if (RGB.x > RGB.y) {
                    maxChannel = RGB.x;
                    minChannel = RGB.y;
                }
                else {
                    maxChannel = RGB.y;
                    minChannel = RGB.x;
                }

                if (RGB.z > maxChannel) maxChannel = RGB.z;
                if (RGB.z < minChannel) minChannel = RGB.z;

                HSV.xy = 0;
                HSV.z = maxChannel;
                float delta = maxChannel - minChannel;             //Delta RGB value
                if (delta != 0) {                    // If gray, leave H  S at zero
                    HSV.y = delta / HSV.z;
                    float3 delRGB;
                    delRGB = (HSV.zzz - RGB + 3 * delta) / (6.0 * delta);
                    if (RGB.x == HSV.z) HSV.x = delRGB.z - delRGB.y;
                    else if (RGB.y == HSV.z) HSV.x = (1.0 / 3.0) + delRGB.x - delRGB.z;
                    else if (RGB.z == HSV.z) HSV.x = (2.0 / 3.0) + delRGB.y - delRGB.x;
                }
                return (HSV);
            }

            float3 hsv_to_rgb(float3 HSV)
            {
                float3 RGB = HSV.z;

                float var_h = HSV.x * 6;
                float var_i = floor(var_h);   // Or ... var_i = floor( var_h )
                float var_1 = HSV.z * (1.0 - HSV.y);
                float var_2 = HSV.z * (1.0 - HSV.y * (var_h - var_i));
                float var_3 = HSV.z * (1.0 - HSV.y * (1 - (var_h - var_i)));
                if (var_i == 0) { RGB = float3(HSV.z, var_3, var_1); }
                else if (var_i == 1) { RGB = float3(var_2, HSV.z, var_1); }
                else if (var_i == 2) { RGB = float3(var_1, HSV.z, var_3); }
                else if (var_i == 3) { RGB = float3(var_1, var_2, HSV.z); }
                else if (var_i == 4) { RGB = float3(var_3, var_1, HSV.z); }
                else { RGB = float3(HSV.z, var_1, var_2); }

                return (RGB);
            }

            float4 _Color;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Speed;
            float _ContrastStrength;
            float _Darkness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 hsvCol = rgb_to_hsv_no_clip(_Color.xyz);
                fixed4 texCol = tex2D(_MainTex, i.uv);

                _Speed *= texCol.x;

                fixed4 col = fixed4(hsv_to_rgb(float3(hsvCol.x, hsvCol.y, 1 - ((sin(_Time.w * _Speed) / 2) + _Darkness) / (1 / _ContrastStrength))), 1) * texCol;
                return col;
            }
            ENDCG
        }
    }
}
