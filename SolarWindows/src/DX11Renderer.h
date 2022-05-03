#pragma once
#include "Core.h"

#include <d3d11_3.h>
#include <dxgi1_2.h>

#include <d3dcompiler.h>

#define DirectXDebugMessageCount 10
#if SOL_DEBUG_RENDERING
#include <dxgidebug.h>
#define DXCHECK(call)                                                                                                   \
    {                                                                                                                   \
        renderState.debug.next = renderState.debug.info_queue->GetNumStoredMessages(DXGI_DEBUG_ALL);                    \
        HRESULT dxresult = (call);                                                                                      \
        if (FAILED(dxresult))                                                                                           \
        {                                                                                                               \
            char *output = nullptr;                                                                                     \
            FormatMessageA(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_ALLOCATE_BUFFER, \
                           NULL, dxresult, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPSTR)&output, 0, NULL);         \
            if (output)                                                                                                 \
            {                                                                                                           \
                LogDirectXDebugGetMessages(&renderState.debug);															\
            }                                                                                                           \
        }                                                                                                               \
    }

#define DXINFO(call)                                         \
    {                                                        \
        renderState.debug.next = renderState.debug.info_queue->GetNumStoredMessages(DXGI_DEBUG_ALL); \
		(call);                                              \
        LogDirectXDebugGetMessages(&renderState.debug);		 \
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

struct SwapChain
{
    IDXGISwapChain1* swapChain;
	ID3D11Texture2D* depthTexture;
    ID3D11DepthStencilView* depthView;
    ID3D11RenderTargetView* renderView;
};

#if SOL_DEBUG_RENDERING
struct RenderDebug
{
    uint64 next;
    struct IDXGIInfoQueue* info_queue;
    ID3D11Debug* debug;
    ID3D11Buffer* vertexBuffer;
};

void LogDirectXDebugGetMessages(RenderDebug* debug);
#endif

struct RenderState
{
    SwapChain swapChain;
	ID3D11Device1* device;
	ID3D11DeviceContext1* context;

#if SOL_DEBUG_RENDERING
	RenderDebug debug;
#endif

	//std::vector<ID3D11UnorderedAccessView*> uavs;
	//std::vector<ID3D11Texture2D*> textures2D;
};

RenderState* GetRenderState();