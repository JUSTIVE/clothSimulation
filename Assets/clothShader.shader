// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/clothShader"
{
	/*Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}*/
	SubShader{
		Pass{

		// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members pos,col)
			CGPROGRAM
			#pragma exclude_renderers d3d11_9x d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			struct Point {
				float3 pos;
				float3 vel;
			};
			struct v2f {
				float4 pos : SV_POSITION;
				float3 col : COLOR0;
				float4 uv : TEXCOORD0;
			};
			float x, y, z;
			StructuredBuffer<Point> value;

			v2f vert(uint id : SV_VertexID) {
				v2f o;
				float3 worldPos = value[id].pos;
				o.pos = UnityObjectToClipPos(worldPos);
				o.col.x = (sin((0.3*float(o.pos.x * 30))) * 127 + 128) / 255.0f;
				o.col.y = (sin((0.3*float(o.pos.x * 30)) + 2) * 127 + 128) / 255.0f;
				o.col.z = (sin((0.3*float(o.pos.x * 30)) + 4) * 127 + 128) / 255.0f;
				
				return o;
			}
			float4 frag(v2f i) : COLOR{
				return float4(i.col,1.0f);
			}
			ENDCG
		}
	}
	Fallback Off
}
