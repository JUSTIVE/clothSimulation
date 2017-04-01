Shader "Custom/NewSurfaceShader" {
	Properties {
		
		_MainTex ("Texture", 2D) = "white" {}
		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		
		#pragma surface surf
		#pragma geometry geom
		#pragma fragment frag
		#pragma target 3.0


		struct Input {
			float2 uv_MainTex;
		};

		struct v2f{

		}

		void geom(triangle v2f input[3],inout TriangleStream<v2f> OutputStream){

		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
