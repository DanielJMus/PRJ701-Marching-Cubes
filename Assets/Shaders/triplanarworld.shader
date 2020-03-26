Shader "Tri-Planar" {
Properties {
        _Tint("Tint", Color) = (1, 1, 1, 1)
        _Top("Top", 2D) = "clear" {} 
        _Side("Sides", 2D) = "clear" {} 
        _Blend("Blend", Range(1, 64)) = 1     
    }
    
    SubShader {

        Tags { "RenderType"="Opaque" "Queue"="Geometry"}

        Pass {

            Cull Back
            ZWrite On
            Lighting On

            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _Top;
            sampler2D _Side;
            float4 _Top_ST;
            float4 _Side_ST;
            float4 _Tint;
            float _Blend;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normal   : NORMAL;
                LIGHTING_COORDS(1,2)
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = worldPos.xyz;
                float3 worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
                o.normal = normalize(worldNormal);
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_TARGET {
                float2 uv_front = TRANSFORM_TEX(i.worldPos.xy, _Side);
                float2 uv_side  = TRANSFORM_TEX(i.worldPos.zy, _Side);
                float2 uv_top   = TRANSFORM_TEX(i.worldPos.xz, _Top);
                float2 uv_bottom   = TRANSFORM_TEX(i.worldPos.xz, _Side);

                fixed4 col_front = tex2D(_Side, uv_front);
                fixed4 col_side = tex2D(_Side, uv_side);
                fixed4 col_top = tex2D(_Top, uv_top);
                fixed4 col_bottom = tex2D(_Side, uv_bottom);

                float3 weights = i.normal;
                weights = abs(weights);
                weights = pow(weights, _Blend);
                weights = weights / (weights.x + weights.y + weights.z);

                col_front *= weights.z;
                col_side *= weights.x;
                col_top *= weights.y;
                if(i.normal.y < 0) col_top = 0;
                col_bottom *= weights.y;
                if(i.normal.y > 0) col_bottom = 0;

                fixed4 col = col_front + col_side + col_top + col_bottom;

                float atten = LIGHT_ATTENUATION(i);
                col *= atten;

                col *= _Tint;
                return col;
            }
            ENDCG
        }
    }
    Fallback "VertexLit"
}