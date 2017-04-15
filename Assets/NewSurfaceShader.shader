// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/NewSurfaceShader" {
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BackTex("Texture", 2D) = "white" {}
	}
	SubShader{
		Pass{
			
			Tags{ "LightMode" = "ForwardBase" }
			LOD 100
			CGPROGRAM
			#pragma multi_compile_fwdbase
			#pragma exclude_renderers d3d11_9x d3d11 xbox360
			#pragma vertex vert
			//#pragma geometry geom
			#pragma fragment frag
			#pragma target 4.0
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			
			struct v2f {
				float4 pos : SV_POSITION;
				float3 col : COLOR0;
				float2 uv : TEXCOORD0;
				int id : VERTID;
			};

			StructuredBuffer<int>Trimap;
			StructuredBuffer<float4>Position;
			StructuredBuffer<float4>Velocity;
			StructuredBuffer<float2>TC;
			uniform sampler2D _MainTex;
			uniform sampler2D _BackTex;

			v2f vert(uint id : SV_VertexID) {
				v2f o;
				float4 worldPos = Position[Trimap[id]];
				o.pos = UnityObjectToClipPos(worldPos);
				o.uv = TC[Trimap[id]];
				o.id= id;
				/*float4 worldPos = Position[id];
				o.pos = UnityObjectToClipPos(worldPos);
				o.uv = TC[id];
				*/
				//TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}

			
			
			float4 frag(v2f i) : SV_Target{
				//return float4(1,1,1,0);
				if(i.id<(31*31)*6)
					return tex2D(_MainTex,i.uv);
				return tex2D(_BackTex,i.uv);	
			}
			ENDCG
		}
	}
	Fallback "VertexLit"
}
