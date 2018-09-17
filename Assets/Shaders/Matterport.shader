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

Shader "Unlit/Matterport"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_CUBE("CubeMap", CUBE) = "" {}
		_CUBEDEPTH("CubeMapDepth", CUBE) = "" {}
		_Infillquality("Infill Qality", Range(1, 10)) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			
			float _ProjectorX = 0.0;
			float _ProjectorY = 0.0;
			float _ProjectorZ = 0.0;
			uniform float4x4 _TextureRotation;
			samplerCUBE _CUBE;
			uniform samplerCUBE _CUBEDEPTH;
			sampler2D _MainTex;
			uniform float _Infillquality;

			struct vertexInput {
				float4 vertex : POSITION;
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 worldpos : TEXCOORD0;
				float3 vWorldPosition0 : TEXCOORD1;
			};

			vertexOutput vert (vertexInput input)
			{
				vertexOutput output;
				output.pos = UnityObjectToClipPos(input.vertex);
				output.worldpos = mul(unity_ObjectToWorld, input.vertex);
				float3 pano0Position = float3(_ProjectorX, _ProjectorY, _ProjectorZ);
				float4 positionLocalToPanoCenter0 = float4((output.worldpos.xyz - pano0Position),1);
				output.vWorldPosition0 = mul(positionLocalToPanoCenter0, _TextureRotation).xyz;
				//output.vWorldPosition0.x *= -1.0;
				return output;
			}
			
			fixed4 frag (vertexOutput i) : COLOR
			{
				float3 dir = i.worldpos - float3(_ProjectorX, _ProjectorY, _ProjectorZ);
				float dist = length(dir) - 0.2f;
					dir = normalize(dir);

				float4 infillcol = texCUBEbias(_CUBE, float4(i.vWorldPosition0.xyz, _Infillquality));
				float4 colorFromPanos = texCUBE(_CUBE, i.vWorldPosition0);
				float depth = texCUBE(_CUBEDEPTH, dir);

				if (dist > depth) {
					return infillcol;
				}

				return float4(colorFromPanos.rgb, 1);
			}
			ENDCG
		}
	}
}
