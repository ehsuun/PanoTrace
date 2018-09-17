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

Shader "Projector/EQ" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("EQ Texture", 2D) = "" {}
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

			#define PI  3.14159265358979323846  /* pi */

			fixed4 _Color;
			sampler2D _MainTex;
			

			uniform samplerCUBE _CUBEDEPTH;
			fixed _Blend;
			fixed4 _tempColor;

			uniform float _Infillangle = 0.0;
			uniform float _Infillquality;


			float2 dirtoSpherical(float3 xyz) {
				xyz = -xyz;
				float r = length(xyz);
				xyz *= 1.f / r;
				float theta = asin(xyz.y);
				float phi = atan2(xyz.z, xyz.x);
				phi += (phi < 0) ? 2 * PI : 0;  // only if you want [0,2pi)
				
				
				float2 uv;

				
				uv.y = 1-(theta/PI +0.5);
				uv.x = 1-phi/(2*PI);
				return uv;
			}

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
				fixed4 col1 = tex2D(_MainTex, dirtoSpherical(mul(rotMatrix, dir)));
				float4 infillcol = tex2Dbias(_MainTex, float4(dirtoSpherical(mul(rotMatrix, dir)).xyy, _Infillquality));
				float depth = texCUBE(_CUBEDEPTH, dir);
				float dist = length(input.worldpos - float3(_ProjectorX, _ProjectorY, _ProjectorZ))-0.2f;

				
				fixed4 col = col1;
				if (dist > depth) {
					
					return infillcol;
				}
				else {
					_tempColor = col;
					
				}
				return col;
				

			}
			ENDCG
		}
	}
}
