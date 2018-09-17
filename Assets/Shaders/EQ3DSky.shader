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

Shader "Skybox/EQ3DSky" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_Rotation("Rotation", Range(0, 360)) = 0
		_Tex("EQ Texture", 2D) = "" {}
	}

		Subshader{
		Tags{ "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
		Pass{
		Cull Off ZWrite Off
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"


	struct appdata_t {
		float4 vertex : POSITION;
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		float3 texcoord : TEXCOORD0;
	};

#define PI  3.14159265358979323846  /* pi */

	float4 _Color;
	sampler2D _Tex;
	float _Rotation;

	float3 RotateAroundYInDegrees(float3 vertex, float degrees)
	{
		float alpha = degrees * UNITY_PI / 180.0;
		float sina, cosa;
		sincos(alpha, sina, cosa);
		float2x2 m = float2x2(cosa, -sina, sina, cosa);
		return float3(mul(m, vertex.xz), vertex.y).xzy;
	}

	float2 dirtoSpherical(float3 xyz) {
		xyz = -xyz;
		float r = length(xyz);
		xyz *= 1.f / r;
		float theta = asin(xyz.y);
		float phi = atan2(xyz.z, xyz.x);
		phi += (phi < 0) ? 2 * PI : 0;  // only if you want [0,2pi)


		float2 uv;

		uv.y = (1 - (theta / PI + 0.5) + (1-unity_StereoEyeIndex)) *0.5;
		uv.x = 1 - phi / (2 * PI);

		return uv;
	}

	v2f vert(appdata_t v)
	{
		v2f o;
		float3 rotated = RotateAroundYInDegrees(v.vertex, _Rotation);
		o.vertex = UnityObjectToClipPos(rotated);
		o.texcoord = v.vertex.xyz;
		return o;
	}



	fixed4 frag(v2f input) : COLOR
	{
		float3 dir = normalize(input.texcoord);
		float3 col = tex2D(_Tex, dirtoSpherical(dir)) * _Color.rgb;
		return float4(_Color.rgb,1);

	}
		ENDCG
	}
	}
}
