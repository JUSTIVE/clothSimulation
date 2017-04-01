﻿Shader "Custom/NewSurfaceShader" {
	SubShader{
		Pass{
			CGPROGRAM
			#pragma exclude_renderers d3d11_9x d3d11 xbox360
			#pragma vertex vert
			//#pragma geometry geom
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			struct g2f {
				float4 pos : SV_POSITION;
				float3 col : COLOR0;
			};
			struct v2f {
				float4 pos : SV_POSITION;
			};

			struct v2g{
				float4 pos : SV_POSITION;
			};
			StructuredBuffer<float4>Position;
			StructuredBuffer<float4>Velocity;
		

			v2f vert(uint id : SV_VertexID) {
				v2f o;
				float4 worldPos = Position[id];
				o.pos = mul(UNITY_MATRIX_MVP, worldPos);
				return o;
			}

			uint i;
			/*[maxvertexcount(3)]
			void geom(triangle v2g input[3], inout TriangleStream<g2f> triStream)
			{
				for (uint i = 0; i<3; i +=1)
				{
					triStream.RestartStrip();
				}
			}*/

			float4 frag(v2f i) : COLOR{
				return float4(1.0f,0.0f,0.0f,1.0f);
			}
			ENDCG
		}
	}
		Fallback "Diffuse"
}