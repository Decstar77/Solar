using SolarSharp.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering
{
    public static class ShaderFactory
    {
        private static Device device;

        public static bool Initialize(Device device) {
            ShaderFactory.device = device;
            return device != null;
        }

        public static GraphicsShader CreateGraphicsShader(string name, string path)
        {
			string fullPath = path + name + ".hlsl";
			StreamWriter writer = new StreamWriter(fullPath);
			writer.Write(GraphicsShader);
			writer.Close();

            return new GraphicsShader(device).Create(AssetSystem.LoadShaderAsset(fullPath));
        }

        private static string GraphicsShader = @"
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
	output.pos = mul(float4(input.pos, 1.0f), mvp);
	//output.pos = float4(input.pos, 1.0);
	output.normal = input.normal;
	output.uv = input.uv;

	return output;
}

float4 PSmain(VSOutput input) : SV_TARGET
{
	return float4(0.5, 0.5, 0, 1);
//	return float4(input.uv.x, input.uv.y, 0, 1);
}
        ";
    }
}
