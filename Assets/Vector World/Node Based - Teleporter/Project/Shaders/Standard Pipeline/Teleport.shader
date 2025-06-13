// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Teleport"
{
	Properties
	{
		[Toggle]____________________________________________________________________________________________________________________________________________________________________________________________________________________________3("____________________________________________________________________________________________________________________________________________________________________________________________________________________________", Float) = 0
		_OuterEmmisive("Outer Emmisive", Color) = (0,0,0,0)
		_EmmisionWidth("Emmision Width", Range( 0 , 1)) = 0
		_EmmisionBlend("Emmision Blend", Range( 0 , 1)) = 0
		_InnerEmission("Inner Emission", Color) = (0,0,0,0)
		_EmmisionInnerWidth("Emmision Inner Width", Range( 0 , 1)) = 0
		_EmmisionInnerBlend("Emmision Inner Blend", Range( 0 , 1)) = 0
		[Toggle]____________________________________________________________________________________________________________________________________________________________________________________________________________________________("____________________________________________________________________________________________________________________________________________________________________________________________________________________________", Float) = 0
		_GI_Reflection("GI_Reflection", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Occulsion("Occulsion", Range( 0 , 1)) = 0
		[Toggle]____________________________________________________________________________________________________________________________________________________________________________________________________________________________1("____________________________________________________________________________________________________________________________________________________________________________________________________________________________", Float) = 0
		[HDR]_TeleportColor("Teleport Color", Color) = (0,0,0,0)
		_Scale("Scale", Range( 0 , 100)) = 0
		_EdgeWidth("EdgeWidth", Float) = 0.22
		[Toggle]____________________________________________________________________________________________________________________________________________________________________________________________________________________________2("____________________________________________________________________________________________________________________________________________________________________________________________________________________________", Float) = 0
		_Axis("Axis", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		ZWrite On
		Blend One One , SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustomLighting keepalpha noshadow nofog nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float3 viewDir;
			float3 worldNormal;
			float2 uv_texcoord;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float ____________________________________________________________________________________________________________________________________________________________________________________________________________________________;
		uniform float ____________________________________________________________________________________________________________________________________________________________________________________________________________________________2;
		uniform float ____________________________________________________________________________________________________________________________________________________________________________________________________________________________1;
		uniform float ____________________________________________________________________________________________________________________________________________________________________________________________________________________________3;
		uniform float3 _Axis;
		uniform float _EmmisionInnerWidth;
		uniform float _EmmisionInnerBlend;
		uniform float4 _InnerEmission;
		uniform float _EmmisionWidth;
		uniform float _EmmisionBlend;
		uniform float4 _OuterEmmisive;
		uniform float4 _TeleportColor;
		uniform float _Scale;
		uniform float _EdgeWidth;
		uniform float _Smoothness;
		uniform float _Occulsion;
		uniform float _GI_Reflection;


		float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
		{
			original -= center;
			float C = cos( angle );
			float S = sin( angle );
			float t = 1 - C;
			float m00 = t * u.x * u.x + C;
			float m01 = t * u.x * u.y - S * u.z;
			float m02 = t * u.x * u.z + S * u.y;
			float m10 = t * u.x * u.y + S * u.z;
			float m11 = t * u.y * u.y + C;
			float m12 = t * u.y * u.z - S * u.x;
			float m20 = t * u.x * u.z - S * u.y;
			float m21 = t * u.y * u.z + S * u.x;
			float m22 = t * u.z * u.z + C;
			float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
			return mul( finalMatrix, original ) + center;
		}


		inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }

		inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }

		inline float valueNoise (float2 uv)
		{
			float2 i = floor(uv);
			float2 f = frac( uv );
			f = f* f * (3.0 - 2.0 * f);
			uv = abs( frac(uv) - 0.5);
			float2 c0 = i + float2( 0.0, 0.0 );
			float2 c1 = i + float2( 1.0, 0.0 );
			float2 c2 = i + float2( 0.0, 1.0 );
			float2 c3 = i + float2( 1.0, 1.0 );
			float r0 = noise_randomValue( c0 );
			float r1 = noise_randomValue( c1 );
			float r2 = noise_randomValue( c2 );
			float r3 = noise_randomValue( c3 );
			float bottomOfGrid = noise_interpolate( r0, r1, f.x );
			float topOfGrid = noise_interpolate( r2, r3, f.x );
			float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
			return t;
		}


		float SimpleNoise(float2 UV)
		{
			float t = 0.0;
			float freq = pow( 2.0, float( 0 ) );
			float amp = pow( 0.5, float( 3 - 0 ) );
			t += valueNoise( UV/freq )*amp;
			freq = pow(2.0, float(1));
			amp = pow(0.5, float(3-1));
			t += valueNoise( UV/freq )*amp;
			freq = pow(2.0, float(2));
			amp = pow(0.5, float(3-2));
			t += valueNoise( UV/freq )*amp;
			return t;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 rotatedValue131 = RotateAroundAxis( float3( 0,0,0 ), ase_vertex3Pos, normalize( _Axis ), _SinTime.z );
			float3 Rotator141 = rotatedValue131;
			v.vertex.xyz = Rotator141;
			v.vertex.w = 1;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float3 ase_worldNormal = i.worldNormal;
			Unity_GlossyEnvironmentData g124 = UnityGlossyEnvironmentSetup( _Smoothness, data.worldViewDir, ase_worldNormal, float3(0,0,0));
			float3 indirectSpecular124 = UnityGI_IndirectSpecular( data, _Occulsion, ase_worldNormal, g124 );
			float3 GI75 = ( indirectSpecular124 * _GI_Reflection );
			c.rgb = GI75;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float dotResult90 = dot( i.viewDir , ase_normWorldNormal );
			float smoothstepResult114 = smoothstep( _EmmisionInnerWidth , _EmmisionInnerBlend , dotResult90);
			float4 Rim02109 = ( smoothstepResult114 * _InnerEmission );
			float dotResult85 = dot( i.viewDir , ase_normWorldNormal );
			float smoothstepResult77 = smoothstep( _EmmisionWidth , _EmmisionBlend , ( 1.0 - dotResult85 ));
			float4 Rim0199 = ( smoothstepResult77 * _OuterEmmisive );
			float simpleNoise21 = SimpleNoise( i.uv_texcoord*_Scale );
			float Alpha30 = simpleNoise21;
			float temp_output_18_0 = (cos( _Time.y )*0.5 + 0.5);
			float Threshold31 = temp_output_18_0;
			clip( Alpha30 - Threshold31);
			float4 Dissolve58 = ( _TeleportColor * step( Alpha30 , ( temp_output_18_0 + _EdgeWidth ) ) );
			o.Emission = ( Rim02109 + Rim0199 + Dissolve58 ).rgb;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18800
1920;0;1920;1019;2254.274;661.4687;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;8;-1969.282,-394.5663;Inherit;False;1507.685;754;Comment;16;81;144;57;49;47;48;31;40;39;30;27;21;23;16;11;149;Dissolve Magic;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;149;-1948.142,8.18399;Inherit;False;606.2002;235.5061;Comment;4;13;9;12;18;For animating by time;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;9;-1898.142,58.18442;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;73;-1948.794,-1349.61;Inherit;False;998.4226;523.004;;9;120;119;115;113;96;92;88;85;77;Rim Outer;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-1696,-352;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-1741.597,-216.5666;Inherit;False;Property;_Scale;Scale;15;0;Create;True;0;0;0;True;0;False;0;57.94;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;13;-1690.142,58.18442;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1720.712,127.6901;Inherit;False;Constant;_Float1;Float 1;2;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;21;-1456,-352;Inherit;False;Simple;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;30;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;120;-1898.794,-1299.61;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;23;-1271,89;Inherit;False;Property;_EdgeWidth;EdgeWidth;16;0;Create;True;0;0;0;False;0;False;0.22;0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;71;-1944.359,-1928.755;Inherit;False;998.4226;523.004;;8;114;108;107;102;91;90;82;79;Rim Inner;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;88;-1890.794,-1108.61;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ScaleAndOffsetNode;18;-1558.942,58.18399;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;85;-1672.371,-1186.84;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;79;-1894.359,-1878.755;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;30;-1280,-352;Inherit;False;Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;91;-1886.359,-1687.755;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-1104,0;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;39;-995.5977,-143.5668;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;92;-1528.371,-1186.84;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-1296,-112;Inherit;False;Threshold;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;90;-1667.936,-1765.985;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;115;-1677.793,-1081.61;Inherit;False;Property;_EmmisionWidth;Emmision Width;4;0;Create;True;0;0;0;False;0;False;0;0.49;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;102;-1658.358,-1667.755;Inherit;False;Property;_EmmisionInnerWidth;Emmision Inner Width;7;0;Create;True;0;0;0;False;0;False;0;0.87;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;40;-1041.598,-344.5663;Inherit;False;Property;_TeleportColor;Teleport Color;14;1;[HDR];Create;True;0;0;0;True;0;False;0,0,0,0;11.98431,11.556,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;108;-1657.358,-1582.756;Inherit;False;Property;_EmmisionInnerBlend;Emmision Inner Blend;8;0;Create;True;0;0;0;False;0;False;0;1.14;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;113;-1668.793,-993.6095;Inherit;False;Property;_EmmisionBlend;Emmision Blend;5;0;Create;True;0;0;0;False;0;False;0;0.84;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;74;-1958.733,-765.593;Inherit;False;784;318;Comment;5;80;94;124;89;101;Glass Mat;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;101;-1861.145,-596.9144;Inherit;False;Property;_Occulsion;Occulsion;12;0;Create;True;0;0;0;False;0;False;0;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;49;-880,32;Inherit;False;31;Threshold;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;96;-1348.277,-1038.606;Inherit;False;Property;_OuterEmmisive;Outer Emmisive;3;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;140;-1136,448;Inherit;False;548.5958;545.7407;Comment;4;132;139;135;131;Rotator;1,1,1,1;0;0
Node;AmplifyShaderEditor.SmoothstepOpNode;77;-1368.371,-1186.84;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;114;-1363.937,-1765.985;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-1861.145,-692.9144;Inherit;False;Property;_Smoothness;Smoothness;11;0;Create;True;0;0;0;False;0;False;0;0.96;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;82;-1343.843,-1617.751;Inherit;False;Property;_InnerEmission;Inner Emission;6;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.6415094,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;47;-880,-48;Inherit;False;30;Alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-826.5976,-152.5668;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClipNode;57;-656,-144;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-1107.937,-1765.985;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;-1112.371,-1186.84;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.IndirectSpecularLight;124;-1557.145,-692.9144;Inherit;False;Tangent;3;0;FLOAT3;0,0,1;False;1;FLOAT;0.5;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;132;-1104,800;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;94;-1557.145,-564.9144;Inherit;False;Property;_GI_Reflection;GI_Reflection;10;0;Create;True;0;0;0;False;0;False;0;0.51;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;135;-1088,512;Inherit;False;Property;_Axis;Axis;18;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SinTimeNode;139;-1072,656;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;-1317.145,-692.9144;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;58;-448,-144;Inherit;False;Dissolve;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;99;-903.3707,-1186.84;Inherit;False;Rim01;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RotateAboutAxisNode;131;-896,512;Inherit;False;True;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;109;-899.9369,-1765.985;Inherit;False;Rim02;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-1157.145,-692.9144;Inherit;False;GI;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;130;-1570,449;Inherit;False;381.873;496.7636;Comment;4;136;127;128;129;Separators;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;141;-544,512;Inherit;False;Rotator;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;123;-240,-880;Inherit;False;58;Dissolve;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;125;-240,-720;Inherit;False;99;Rim01;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;112;-240,-800;Inherit;False;109;Rim02;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-1728,-112;Inherit;False;Property;_FaderInOut;Fader In/Out;1;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;136;-1506,769;Inherit;False;Property;____________________________________________________________________________________________________________________________________________________________________________________________________________________________3;____________________________________________________________________________________________________________________________________________________________________________________________________________________________;2;1;[Toggle];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;128;-1506,593;Inherit;False;Property;____________________________________________________________________________________________________________________________________________________________________________________________________________________________1;____________________________________________________________________________________________________________________________________________________________________________________________________________________________;13;1;[Toggle];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;144;-1456,-112;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;142;-48,-544;Inherit;False;141;Rotator;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;129;-1505,673;Inherit;False;Property;____________________________________________________________________________________________________________________________________________________________________________________________________________________________2;____________________________________________________________________________________________________________________________________________________________________________________________________________________________;17;1;[Toggle];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;127;-1506,513;Inherit;False;Property;____________________________________________________________________________________________________________________________________________________________________________________________________________________________;____________________________________________________________________________________________________________________________________________________________________________________________________________________________;9;1;[Toggle];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;126;-48,-640;Inherit;False;75;GI;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;87;0,-800;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;190.2336,-869.2214;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Teleport;False;False;False;False;False;False;False;False;False;True;True;True;False;False;False;False;False;False;False;False;False;Back;1;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Opaque;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;4;1;False;-1;1;False;-1;2;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;13;0;9;0
WireConnection;21;0;16;0
WireConnection;21;1;11;0
WireConnection;18;0;13;0
WireConnection;18;1;12;0
WireConnection;18;2;12;0
WireConnection;85;0;120;0
WireConnection;85;1;88;0
WireConnection;30;0;21;0
WireConnection;27;0;18;0
WireConnection;27;1;23;0
WireConnection;39;0;30;0
WireConnection;39;1;27;0
WireConnection;92;0;85;0
WireConnection;31;0;18;0
WireConnection;90;0;79;0
WireConnection;90;1;91;0
WireConnection;77;0;92;0
WireConnection;77;1;115;0
WireConnection;77;2;113;0
WireConnection;114;0;90;0
WireConnection;114;1;102;0
WireConnection;114;2;108;0
WireConnection;48;0;40;0
WireConnection;48;1;39;0
WireConnection;57;0;48;0
WireConnection;57;1;47;0
WireConnection;57;2;49;0
WireConnection;107;0;114;0
WireConnection;107;1;82;0
WireConnection;119;0;77;0
WireConnection;119;1;96;0
WireConnection;124;1;89;0
WireConnection;124;2;101;0
WireConnection;80;0;124;0
WireConnection;80;1;94;0
WireConnection;58;0;57;0
WireConnection;99;0;119;0
WireConnection;131;0;135;0
WireConnection;131;1;139;3
WireConnection;131;3;132;0
WireConnection;109;0;107;0
WireConnection;75;0;80;0
WireConnection;141;0;131;0
WireConnection;87;0;112;0
WireConnection;87;1;125;0
WireConnection;87;2;123;0
WireConnection;0;2;87;0
WireConnection;0;13;126;0
WireConnection;0;11;142;0
ASEEND*/
//CHKSM=8528650BE0BAC50044D9B05D06555D5090112BEC