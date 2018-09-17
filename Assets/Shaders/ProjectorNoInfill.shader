// Copyright (C) 2018 The Regents of the University of California (Regents).
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//
//     * Redistributions in binary form must reproduce the above
//       copyright notice, this list of conditions and the following
//       disclaimer in the documentation and/or other materials provided
//       with the distribution.
//
//     * Neither the name of The Regents or University of California nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDERS OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.
//
// Please contact the author of this library if you have any questions.
// Author: Ehsan Sayyad (ehsan@mat.ucsb.edu)

Shader "Projector/CubeNoInfill" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_CUBE("CubeMap", CUBE) = "" {}
		_CUBE2("CubeMap2", CUBE) = "" {}
		_CUBEDEPTH("CubeMapDepth", CUBE) = "" {}
		_Blend("Blending value", Range(0, 1)) = 0.0
		_Infillangle("Infill Angle", Range(0, 3.1415)) = 0.0
		_Infillquality("Infill Qality", Range(1, 10)) = 1.0
	}

		Subshader{
		Tags{ "RenderType" = "Opaque" }
		Pass{
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"


			struct vertexInput {
				float4 vertex : POSITION;
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 worldpos : TEXCOORD0;
			};


			float3 _ProjectorPosition;
			float _ProjectorX =0.0;
			float _ProjectorY = 0.0;
			float _ProjectorZ = 0.0;
			float _ProjectorYAngle = 0.0;

			float _Projector2X = 0.0;
			float _Projector2Y = 0.0;
			float _Projector2Z = 0.0;
			float _Projector2YAngle = 0.0;

			float4x4 _TextureRotation;

			fixed4 _Color;
			samplerCUBE _CUBE;
			samplerCUBE _CUBE2;
			uniform samplerCUBE _CUBEDEPTH;
			fixed _Blend;
			fixed4 _tempColor;

			uniform float _Infillangle = 0.0;
			uniform float _Infillquality;




			vertexOutput vert(vertexInput input)
			{

				vertexOutput output;
				output.pos = UnityObjectToClipPos(input.vertex);
				output.worldpos = mul(unity_ObjectToWorld, input.vertex);
				return output;
			}








			fixed4 frag(vertexOutput input) : COLOR
			{
				//Rotation Matrix used to rotate around the scene
				float3x3 rotMatrix = { cos(_ProjectorYAngle), 0.0, sin(_ProjectorYAngle),

				0.0,				1.0,				0.0,

				-sin(_ProjectorYAngle), 0.0, cos(_ProjectorYAngle) };

			//Rotation Matrix used to rotate around the scene for 2nd Cubemap
			float3x3 rotMatrix2 = { cos(_Projector2YAngle), 0.0, sin(_Projector2YAngle),

				0.0,				1.0,				0.0,

				-sin(_Projector2YAngle), 0.0, cos(_Projector2YAngle) };

			float3x3 rotMatrix3 = { cos(_Infillangle), 0.0, sin(_Infillangle),

				0.0,				1.0,				0.0,

				-sin(_Infillangle), 0.0, cos(_Infillangle) };


				float3 dir = normalize(input.worldpos - float3(_ProjectorX, _ProjectorY, _ProjectorZ));
				float3 dir2 = normalize(input.worldpos - float3(_Projector2X, _Projector2Y, _Projector2Z));
			
				fixed4 col1 = texCUBE(_CUBE, mul(_TextureRotation, dir).xyz);
				fixed4 col2 = texCUBE(_CUBE2, mul(_TextureRotation, dir2).xyz);
				float4 infillcol = texCUBEbias(_CUBE, float4(mul(rotMatrix3, mul(_TextureRotation, dir).xyz), _Infillquality));
				float depth = texCUBE(_CUBEDEPTH, dir);

				float dist = length(input.worldpos - float3(_ProjectorX, _ProjectorY, _ProjectorZ))-0.2f;



				if (length(col2.xyz*_Blend) > 0.3) {
					return _Color;
				}

				//change col2 to col1
				fixed4 col = lerp(col1, col1, _Blend);

				
				return col;
				

			}
			ENDCG
		}
	}
}
