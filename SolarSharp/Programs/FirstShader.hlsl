struct VSInput
{
	float3 pos : POSITION;
	float3 normal : NORMAL;
	float2 uv : TexCord;
};

struct VSOutput
{
	float4 pos : SV_POSITION;
	float3 normal : NORMAL;
	float2 uv : TexCord;
};

cbuffer ModelData : register(b0)
{
	matrix mvp;
	matrix model;
	matrix invM;
};

cbuffer ViewData : register(b1)
{
	matrix persp;
	matrix view;
	matrix screenProjection;
}

VSOutput VSmain(VSInput input)
{
	VSOutput output;
	output.pos = mul(float4(input.pos, 1.0f), mvp);// float4(input.pos, 1.0); 
	output.normal = input.normal;
	output.uv = input.uv;

	return output;
}

float4 PSmain(VSOutput input) : SV_TARGET
{
	//return float4(0.5, 0.5, 0, 1);
	return float4(input.uv.x, input.uv.y, 0, 1);
 }
