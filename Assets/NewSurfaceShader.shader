// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/NewSurfaceShader" {
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader{
		Pass{
			
			Tags{ "LightMode" = "ForwardBase" }
			LOD 100
			CGPROGRAM
			#pragma multi_compile_fwdbase
			#pragma exclude_renderers d3d11_9x d3d11 xbox360
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			#pragma target 4.0
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			struct g2f {
				float4 pos : SV_POSITION;
				float3 col : COLOR0;
			};
			struct v2f {
				float4 pos : SV_POSITION;
				float3 col : COLOR0;
				float2 uv : TEXCOORD0;
				LIGHTING_COORDS(0, 1)
			};

			
			StructuredBuffer<float4>Position;
			StructuredBuffer<float4>Velocity;
			StructuredBuffer<float2>TC;
			uniform sampler2D _MainTex;

			v2f vert(uint id : SV_VertexID) {
				v2f o;
				float4 worldPos = Position[id];
				o.pos = UnityObjectToClipPos(worldPos);
				o.uv = TC[id];
				
//				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}

			uint i;
			[maxvertexcount(3)]
			void geom(triangle v2f input[3], inout TriangleStream<v2f> triStream)
			{
				v2f test=(v2f)0;
				for(i=0;i<3;i++){
					test.pos=input[i].pos;
					test.uv = input[i].uv;
					triStream.Append(test);
				}
			}
			
			float4 frag(v2f i) : SV_Target{
				return tex2D(_MainTex,i.uv);
			}
			ENDCG
		}
	}
	Fallback "VertexLit"
}
