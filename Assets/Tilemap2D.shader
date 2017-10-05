Shader "Tilemap/Tilemap2D" 
{
	Properties 
	{
		_SpriteSheet ("Sprite Sheet", 2D) = "black" {}						 // The sprite sheet
		_DataMap ("DataMap", 2D) 		  = "black" {}						 // The data map
		_SpriteSheetSize ("SheetSize, SpriteCount", vector) = (0,0,0,0)      // Sprite count and sprite sheet dimensions 
	}

	SubShader 
	{
		pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
	
			sampler2D _SpriteSheet;
			sampler2D _DataMap;
			float4 _SpriteSheetSize;
			
			struct vertexInput
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			struct vertexOut
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			vertexOut vert(vertexInput IN)
			{
				vertexOut OUT;
				
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				
				return OUT;
			}
			
			float4 frag(vertexOut OUT) : COLOR
			{
				float2 spriteIndex = tex2D(_DataMap, OUT.texcoord);	// get sprite; spriteIndex.x and spriteIndex.y is the spriteposition
				
				float2 spriteOffset = frac(OUT.texcoord * _SpriteSheetSize.xy) / _SpriteSheetSize.zw;
				
				float2 bias = float2(-0.001, 0);
				
				return tex2D(_SpriteSheet, spriteIndex.xy + spriteOffset+bias);
			}
			
			ENDCG
		}
	} 
}
