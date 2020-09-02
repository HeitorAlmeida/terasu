Shader "Custom/UIColorFromBlack"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        //halmeida - my own properties.
        _NewColor ("Black To", Color) = (0.0, 0.0, 0.0, 0.0)
        _Gradient ("Gradient", Vector) = (0.0, 0.0, 0.0, 0.0)
        _GradientColor ("Gradient Color", Color) = (0.0, 0.0, 0.0, 0.0)
        _VertexToCamera ("Vertex To Camera", Vector) = (0.0, 0.0, 0.0, 0.0)
        //halmeida - my own properties end.
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        	CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 screenPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.screenPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.screenPosition);

                OUT.texcoord = v.texcoord;

                OUT.color = v.color * _Color;
                return OUT;
            }

            sampler2D _MainTex;

            //halmeida - my own uniform vars.
            uniform fixed4 _NewColor;
			uniform float4 _Gradient;
			uniform fixed4 _GradientColor;
			uniform float4 _VertexToCamera;
			//halmeida - my own uniform vars end.

            fixed4 frag(v2f IN) : SV_Target
            {
            	//original frag instructions part 1.
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                color.a *= UnityGet2DClipping(IN.screenPosition.xy, _ClipRect);
                //original frag instructions part 1 end.

                //halmeida - my own frag instructions.
                half4 originalColor = color;
                float2 vertexToFragment = _VertexToCamera.xy + IN.screenPosition.xy;
                //halmeida - I will need to divide a value by the gradient's size. To prevent division by zero
                //I could ask if the gradient is zero, but I don't want any if statements in the shader. So I
                //just force the gradient's size to be bigger than zero.
                //The only way I can think of to know how much of the gradient's size a given value covers is by
                //dividing this value by the gradient.*/
                float gradientY = max(0.01, _Gradient.y);
                //halmeida - the color gradient will happen between gradientY(where the color will be strongest)
                //and minus gradientY(where the color will be weakest).
                float gradientEffectY = (vertexToFragment.y / (2.0 * gradientY)) + 0.5;
                //halmeida - we also have to consider the case when the user wants no gradient at all, just the
                //color transformation from black. In that case we have to remove the gradient effect.
                gradientEffectY *= min(1.0, ceil(_Gradient.y));
                //halmeida - keep the effect value between one and zero.
                gradientEffectY = max(0.0, min(1.0, gradientEffectY));

                //halmeida - This shader does two color transformations. One is the transformation of a color into another
                //according to the amount of black within the original color, and the other is the transformation of a color
                //into another respecting the distance from the center. The first transformation is the gradient one that
                //takes the distance into account, and the second, which is performed on top of it, is the transformation
                //from black.
                half3 gradientRGB = ((1.0-gradientEffectY) * originalColor.rgb) + (gradientEffectY * _GradientColor.rgb);
                fixed greyscale = (gradientRGB.r + gradientRGB.g + gradientRGB.b) / 3.0;
                color.rgb = (greyscale * gradientRGB) + ((1.0-greyscale) * _NewColor.rgb);
                //halmeida - my own frag instructions end.

                //original frag instructions part 2.
                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
                //original frag instructions part 2 end.
            }
        	ENDCG
        }
    }
}