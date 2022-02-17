#pragma once
#include "Core.h"
#include "RendererTypes.h"

#include <array>

#include <d3d11_3.h>
#include <dxgi1_2.h>

#define DirectXDebugMessageCount 10
#if SOL_DEBUG_RENDERING
#include <dxgidebug.h>
#define DXCHECK(call)                                                                                                   \
    {                                                                                                                   \
        renderState.deviceContext.debug.next = renderState.deviceContext.debug.info_queue->GetNumStoredMessages(DXGI_DEBUG_ALL);                    \
        HRESULT dxresult = (call);                                                                                      \
        if (FAILED(dxresult))                                                                                           \
        {                                                                                                               \
            char *output = nullptr;                                                                                     \
            FormatMessageA(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_ALLOCATE_BUFFER, \
                           NULL, dxresult, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPSTR)&output, 0, NULL);         \
            if (output)                                                                                                 \
            {                                                                                                           \
                LogDirectXDebugGetMessages(&renderState.deviceContext.debug);															\
            }                                                                                                           \
        }                                                                                                               \
    }

#define DXINFO(call)                                         \
    {                                                        \
        renderState.deviceContext.debug.next = renderState.deviceContext.debug.info_queue->GetNumStoredMessages(DXGI_DEBUG_ALL); \
		(call);                                              \
        LogDirectXDebugGetMessages(&renderState.deviceContext.debug);		 \
    }
#define DXNAME(obj, name) (obj->SetPrivateData(WKPDID_D3DDebugObjectName, sizeof(name) - 1, name))
#else 
#define DXCHECK(call) {(call);}
#define DXINFO(call) {(call);}
#define DXNAME(obj, name) {}
#endif

#define DXRELEASE(object)  \
    if ((object))          \
    {                      \
        object->Release(); \
        object = nullptr;  \
    }

class Topology
{
public:
	enum class Value
	{
		TRIANGLE_LIST,
		LINE_LIST,
	};

	Topology(Value value)
	{
		this->value = value;
	}

	inline D3D_PRIMITIVE_TOPOLOGY GetDXFormat() const
	{
		switch (value)
		{
		case Value::TRIANGLE_LIST: return D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST;
		case Value::LINE_LIST: return D3D11_PRIMITIVE_TOPOLOGY_LINELIST;
		}

		return {};
	}

private:
	Value value;
};

struct SwapChain
{
    IDXGISwapChain1* swapChain;
    ID3D11Texture2D* depthTexture;
    ID3D11DepthStencilView* depthView;
    ID3D11ShaderResourceView* depthShaderView;
    ID3D11RenderTargetView* renderView;
};

struct StaticProgram
{   
    ID3D11VertexShader* vs;
    ID3D11PixelShader* ps;
    ID3D11ComputeShader* cs;
    ID3D11InputLayout* layout;

	operator bool() const { return vs || cs; }
};

#if SOL_DEBUG_RENDERING
struct RenderDebug
{
    uint64 next;
    struct IDXGIInfoQueue* info_queue;
    ID3D11Debug* debug;
	StaticProgram program;
    ID3D11Buffer* vertexBuffer;
};

void LogDirectXDebugGetMessages(RenderDebug* debug);
#endif

struct DeviceContext
{
    ID3D11Device1* device;
    ID3D11DeviceContext1* context;
#if SOL_DEBUG_RENDERING
    RenderDebug debug;
#endif
};

struct StaticMesh
{
	uint32 strideBytes;
	uint32 indexCount;

	ID3D11Buffer* vertexBuffer;
	ID3D11Buffer* indexBuffer;

	operator bool() const { return vertexBuffer && indexBuffer; }	
};

struct DynamicMesh
{
	ID3D11Buffer* vertexBuffer;
	uint32 strideBytes;
	operator bool() const { return vertexBuffer; }
};

struct RenderState
{
    SwapChain swapChain;
    DeviceContext deviceContext;

	std::array<ID3D11RasterizerState*, 8> rasterStates;
	std::array<ID3D11DepthStencilState*, 8> depthStates;
	std::array<ID3D11BlendState*, 8> blendStates;
	std::array<ID3D11SamplerState*, 8> sampleStates;
	std::array<ID3D11Buffer*, 8> constBuffers;

	std::array<StaticMesh, 1024> staticMeshes;
	std::array<DynamicMesh, 1024> dynamicMeshes;

	std::array<StaticProgram, 1024> staticPrograms;
};

inline RenderState renderState = {};

inline DXGI_FORMAT GetTextureFormatToD3D(const TextureFormat& format)
{
	switch (format.Get())
	{
	case TextureFormat::Value::R8G8B8A8_UNORM: return DXGI_FORMAT_R8G8B8A8_UNORM;
	case TextureFormat::Value::R16G16_UNORM: return DXGI_FORMAT_R16G16_UNORM;
	case TextureFormat::Value::R8_BYTE: return DXGI_FORMAT_R8_UINT;
	case TextureFormat::Value::R32_FLOAT: return DXGI_FORMAT_R32_FLOAT;
	case TextureFormat::Value::D32_FLOAT: return DXGI_FORMAT_D32_FLOAT;
	case TextureFormat::Value::R32_TYPELESS: return DXGI_FORMAT_R32_TYPELESS;
	case TextureFormat::Value::R16_UNORM: return DXGI_FORMAT_R16_UNORM;
	case TextureFormat::Value::D16_UNORM: return DXGI_FORMAT_D16_UNORM;
	case TextureFormat::Value::R16_TYPELESS: return DXGI_FORMAT_R16_TYPELESS;
	case TextureFormat::Value::R32G32_FLOAT: return DXGI_FORMAT_R32G32_FLOAT;
	case TextureFormat::Value::R32G32B32_FLOAT: return DXGI_FORMAT_R32G32B32_FLOAT;
	case TextureFormat::Value::R32G32B32A32_FLOAT: return DXGI_FORMAT_R32G32B32A32_FLOAT;
	case TextureFormat::Value::R16G16B16A16_FLOAT: return DXGI_FORMAT_R16G16B16A16_FLOAT;
	default: Assert(0, "TextureFormatToD3D ??");
	}

	return DXGI_FORMAT_UNKNOWN;
}

inline D3D11_TEXTURE_ADDRESS_MODE GetTextureWrapModeToD3D(const TextureWrapMode& wrap)
{
	switch (wrap.Get())
	{
	case TextureWrapMode::Value::REPEAT: return D3D11_TEXTURE_ADDRESS_WRAP;
	case TextureWrapMode::Value::CLAMP_EDGE:return D3D11_TEXTURE_ADDRESS_CLAMP;
	default: Assert(0, "TextureWrapModeToD3D ??");
	}

	return D3D11_TEXTURE_ADDRESS_WRAP;
}

inline D3D11_FILTER GetTextureFilterModeToD3D(const TextureFilterMode& mode)
{
	switch (mode.Get())
	{
	case TextureFilterMode::Value::POINT:		return D3D11_FILTER_MIN_MAG_MIP_POINT;
	case TextureFilterMode::Value::BILINEAR:	return D3D11_FILTER_MIN_MAG_LINEAR_MIP_POINT;
	case TextureFilterMode::Value::TRILINEAR:	return D3D11_FILTER_MIN_MAG_MIP_LINEAR;
	default: Assert(0, "TextureFilterModeToD3D ??");
	}

	return D3D11_FILTER_MIN_MAG_MIP_POINT;
}

inline int32 GetTextureUsageToD3DBindFlags(const BindUsage& usage)
{
	switch (usage.Get())
	{
	case BindUsage::Value::NONE:  return 0;
	case BindUsage::Value::SHADER_RESOURCE: return D3D11_BIND_SHADER_RESOURCE;
	case BindUsage::Value::RENDER_TARGET: return D3D11_BIND_RENDER_TARGET;
	case BindUsage::Value::DEPTH_SCENCIL_BUFFER: return D3D11_BIND_DEPTH_STENCIL;
	case BindUsage::Value::COMPUTER_SHADER_RESOURCE: return D3D11_BIND_UNORDERED_ACCESS;
	default: Assert(0, "TextureUsageToD3DBindFlags ??");
	}

	return 0;
}

inline int32 GetCPUFlagsToD3DFlags(const ResourceCPUFlags& flags)
{
	switch (flags.Get())
	{
	case ResourceCPUFlags::Value::NONE: return 0;
	case ResourceCPUFlags::Value::READ: return D3D11_CPU_ACCESS_READ;
	case ResourceCPUFlags::Value::WRITE: return D3D11_CPU_ACCESS_WRITE;
	case ResourceCPUFlags::Value::READ_WRITE: return D3D11_CPU_ACCESS_READ | D3D11_CPU_ACCESS_WRITE;
	}
	return 0;
}
