#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;
texture cubeTexture;

sampler TextureSampler = sampler_state
{
	texture = <cubeTexture>;
	mipfilter = LINEAR;
	minfilter = LINEAR;
	magfilter = LINEAR;
};

struct InstancingVSinput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct InstancingVSoutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

InstancingVSoutput InstancingVS(InstancingVSinput input, float4 instanceTransform : POSITION1,
								float2 atlasCoord : TEXCOORD1)
{
	InstancingVSoutput output;

	float4 pos = input.Position + instanceTransform;
	pos = mul(pos, WorldViewProjection);

	output.Position = pos;
	output.TexCoord = float2((input.TexCoord.x / 2.0f) + (1.0f / 2.0f * atlasCoord.x),
							 (input.TexCoord.y / 2.0f) + (1.0f / 2.0f * atlasCoord.y));
	return output;
}

float4 InstancingPS(InstancingVSoutput input) : COLOR0
{
	return tex2D(TextureSampler, input.TexCoord);
}

technique Instancing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL InstancingVS();
		PixelShader = compile PS_SHADERMODEL InstancingPS();
	}
};