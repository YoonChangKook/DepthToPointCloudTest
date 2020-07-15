Shader "Custom/ZBuffer"
{
	SubShader
	{
		//불투명 오브젝트 그릴때
		Tags{ "RenderType" = "Opaque" }
		Pass
		{
		CGPROGRAM
		#pragma vertex vert	
		#pragma fragment frag
		#include "UnityCG.cginc"

		struct v2f
		{
			float4 pos : SV_POSITION;
			float4 screenuv : TEXCOORD1;
		};

		v2f vert(appdata_base v)
		{
			v2f o;
			//월드 코디네이트 정점 클리핑 포지션으로 매핑했을때 
			o.pos = UnityObjectToClipPos(v.vertex);
			//해당 정점의 스크린에서의 위치값을 받아올때 
			o.screenuv = ComputeScreenPos(o.pos);
			return o;
		}

		//(기억)카메라가 촬영하고있는 뎁스 텍스쳐를 가져오기위해
		//이거 미리 정해진 변수명임
		sampler2D _CameraDepthTexture;

		fixed4 frag(v2f i) : SV_Target
		{
			//현재 화면 해상도에 맞게 uv좌표값을 가져온다.
			float2 uv = i.screenuv.xy / i.screenuv.w;
			//테크닉
			//해당 프래그먼트의 uv좌표에서의 카메라 뎁스 값을 가져오고
			//이를 0에서 1로 다시 매핑한다.
			float depth = 1 - Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));

			//그런다음 뎁스값을 색상으로 리턴해주면
			return fixed4(depth, depth, depth, 1);
		}
		ENDCG
	}
	}
}