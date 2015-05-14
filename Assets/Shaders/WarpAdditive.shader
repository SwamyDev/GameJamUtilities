Shader "CyberG/Warp Additive" 
{
	Properties 
	{
		_ColorMap ("Color Map (RGBA)", 2D) = "white" {}
		_DistortionMap ("Distortion Map (RGB)", 2D) = "white"  {}
		_ShiftSpeedU ("Shift Speed U", Float) = 0.0
		_ShiftSpeedV ("Shift Speed V", Float) =  -0.25
		_ShiftScaleU ("Distortion Scale U", Float) = 0.4
		_ShiftScaleV ("Distortion Scale V", Float) = 0.4
		_StrenghtU ("Strength U", Float) = 0.7
		_StrenghtV ("Strength V", Float) = 0.18
		_ColorPower ("Color Power", Float) = 2.268
		_Color ("Color", Color) = (0.613642,0.289158,0.0687724)
		_ColorMulti ("Color Multiplier", Float) = 4.2439
	}

	SubShader 
	{
		Tags { "Queue" ="Transparent" "RenderType"="Transparent" }

		Pass  
		{
			Blend One One
			Lighting Off
			ZWrite Off

			CGPROGRAM
			
			#pragma vertex vert
	        #pragma fragment frag
	        #pragma target 3.0

	        sampler2D _ColorMap;
	        sampler2D _DistortionMap;
	        float _ShiftSpeedU;
	        float _ShiftSpeedV;
	        float _ShiftScaleU;
	        float _ShiftScaleV;
	        float _StrenghtU; 
	        float _StrenghtV;
	        float _ColorPower;
	        float3 _Color;
	        float _ColorMulti;


	        struct VertIn 
	        {
	            float4 vertex : POSITION;
	            float4 texCoords : TEXCOORD0;
	            fixed4 color : COLOR;
	        };

	        struct PixelIn 
	        {
	            float4 position : SV_POSITION;
	            float4 uv : TEXCOORD0;
	            fixed4 color : COLOR;
	        };

	        PixelIn vert(VertIn input) : POSITION 
	        {
	        	PixelIn output;

	        	output.position = mul(UNITY_MATRIX_MVP, input.vertex);
	        	output.uv = float4(input.texCoords.xy, 0, 0);
	        	output.color = input.color;

	        	return output;
	        }


	        fixed4 frag(PixelIn input) : COLOR
	        {
	        	float2 shiftedUv = (input.uv.xy + (float2(_ShiftSpeedU, _ShiftSpeedV) * _Time.y) * float2(_ShiftScaleU, _ShiftScaleV));
	        	float2 scaledUv = shiftedUv * float2(_ShiftScaleU, _ShiftScaleV);
	        	float2 distortion = ((2.0 * tex2D(_DistortionMap, scaledUv).rg) - float2(1.0, 1.0)) * float2(_StrenghtU, _StrenghtV);
	        	float2 distortedCoords = input.uv.xy + distortion;

	        	fixed4 sampledColor = tex2D(_ColorMap, distortedCoords);
	        	float3 powColor = pow(sampledColor.rgb, float3(_ColorPower, _ColorPower, _ColorPower));
	        	fixed3 finalColor = powColor * input.color * _Color * _ColorMulti;

	        	return fixed4(finalColor, 1.0);
	        }

			ENDCG
		}
	}
}