Shader "Custom/NewSurfaceShader" {
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader{
		Pass{
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma multi_compile_fwdbase
			#pragma exclude_renderers d3d11_9x d3d11 xbox360
			#pragma vertex vert
			//#pragma geometry geom
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			/*struct g2f {
				float4 pos : SV_POSITION;
				float3 col : COLOR0;
			};*/
			struct v2f {
				float4 pos : SV_POSITION;
				float3 col : COLOR0;
				float2 uv : TEXCOORD0;
				LIGHTING_COORDS(0, 1)
			};

			//struct v2g{
			//	float4 pos : SV_POSITION;
			//};
			StructuredBuffer<float4>Position;
			StructuredBuffer<float4>Velocity;
			uniform sampler2D _MainTex;

			v2f vert(uint id : SV_VertexID) {
				v2f o;
				float4 worldPos = Position[id];
				o.pos = mul(UNITY_MATRIX_MVP, worldPos);
				o.col.x = (sin((0.3*float(o.pos.x * 30))) * 127 + 128) / 255.0f;
				o.col.y = (sin((0.3*float(o.pos.x * 30)) + 2) * 127 + 128) / 255.0f;
				o.col.z = (sin((0.3*float(o.pos.x * 30)) + 4) * 127 + 128) / 255.0f;
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}

			/*uint i;
			[maxvertexcount(3)]
			void geom(triangle v2g input[3], inout TriangleStream<g2f> triStream)
			{
				for (uint i = 0; i<3; i +=1)
				{
					triStream.RestartStrip();
				}
			}*/
			
			float4 frag(v2f i) : SV_Target{
				return tex2D(_MainTex,i.uv);
			}
			ENDCG
		}
	}
	Fallback "VertexLit"
}
