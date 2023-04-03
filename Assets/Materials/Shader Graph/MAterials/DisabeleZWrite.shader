Shader "Custom/DisabeleZWrite"
{
	SubShader{
		Tags{
			"RenderType" = "Opaque"
			}
		Pass{
			ZWrite Off
			}
		}
}