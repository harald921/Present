Shader "Tilemap/Tilemap2D" 
{
	Properties 
	{
		_DataMap ("DataMap", 2D) 		  = "black" {}		// The texture containing the pixels that should be represented as textures
		_DataMapSize("DataMapSize", Float) = 0				// The size of the data texture		

		_SpriteSheet ("Sprite Sheet", 2D) = "black" {}		// The sprite sheet containing the sprites I want to use
	}

	SubShader 
	{
		pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
	
			sampler2D _DataMap;
			float     _DataMapSize;
			sampler2D _SpriteSheet;

			struct vertexInput
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			struct vertexOut
			{
				float4 vertex    : SV_POSITION;
				float2 texcoord  : TEXCOORD0;
				float2 texcoord2 : TEXCOORD2;
			};
			
			vertexOut vert(vertexInput IN)
			{
				vertexOut OUT;
				
				OUT.vertex    = UnityObjectToClipPos(IN.vertex);
				
				OUT.texcoord  = IN.texcoord * float2(1,-1);
				OUT.texcoord2 = IN.texcoord * _DataMapSize;

				return OUT;
			}
			
			float4 frag(vertexOut OUT) : COLOR
			{
				fixed3 index;

				index.xy = (floor(tex2D(_DataMap, OUT.texcoord) * 16.0) + frac(OUT.texcoord2)) * 0.0625;

				return tex2D(_SpriteSheet, index);
			}
			
			ENDCG
		}
	} 
}
