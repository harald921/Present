Shader "Tilemap/Tilemap2D" 
{
	Properties 
	{
		_DataMap ("DataMap", 2D) 		  = "black" {}		// The texture containing the pixels that should be represented as textures
		_DataMapSize("DataMapSize", Float) = 0				// The size of the data texture		

		_SpriteSheet ("Sprite Sheet", 2D) = "black" {}		// The sprite sheet containing the sprites I want to use
		_SpriteSheetSize("Sprite Sheet Size", Float) = 0
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
			float 	  _SpriteSheetSize;
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
				fixed mip 		 : NORMAL;
			};
			
			vertexOut vert(vertexInput IN)
			{
				vertexOut OUT;
				
				OUT.vertex    = UnityObjectToClipPos(IN.vertex);
				
				OUT.texcoord  = IN.texcoord * float2(1,-1);
				OUT.texcoord2 = IN.texcoord * _DataMapSize;

				float tileSize = _ScreenParams.x / (unity_OrthoParams.x * 2.0);
				OUT.mip = max(0, log2(_SpriteSheetSize / 16.0 / tileSize) - 1.0);

				return OUT;
			}
			
			float4 frag(vertexOut OUT) : COLOR
			{
				fixed3 index;

				index.xy = (floor(tex2D(_DataMap, OUT.texcoord) * 16.0) + frac(OUT.texcoord2)) * 0.0625;

				return tex2Dlod(_SpriteSheet, fixed4(index, OUT.mip));
			}
			
			ENDCG
		}
	} 
}
