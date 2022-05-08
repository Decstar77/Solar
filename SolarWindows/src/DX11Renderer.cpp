#pragma once
#include "Win32Platform.h"
#include "DX11Renderer.h"
#include "SolarMath.h"
#include <system_error>

static RenderState renderState = {};

RenderState* GetRenderState()
{
	return &renderState;
}

#if SOL_DEBUG_RENDERING
static void InitializeDirectXDebugLogging()
{
	HRESULT dxresult = {};
	typedef HRESULT(WINAPI* DXGIGetDebugInterface)(REFIID, void**);

	HMODULE mod_debug = LoadLibraryExA("dxgidebug.dll", nullptr, LOAD_LIBRARY_SEARCH_SYSTEM32);
	if (mod_debug)
	{
		DXGIGetDebugInterface debug_fnc = reinterpret_cast<DXGIGetDebugInterface>(
			reinterpret_cast<void*>(GetProcAddress(mod_debug, "DXGIGetDebugInterface")));
		if (debug_fnc)
		{
            SOLTRACE("Intialized directx 11 debug logging");
			dxresult = debug_fnc(__uuidof(IDXGIInfoQueue), (void**)&renderState.debug.info_queue);
		}
		else
		{
			SOLERROR("Could not intialize directx 11 debug logging");
		}
	}
	else
	{
		SOLERROR("Could not intialize directx 11 debug logging");
	}
}

void LogDirectXDebugGetMessages(RenderDebug* debug)
{
	uint64 end = debug->info_queue->GetNumStoredMessages(DXGI_DEBUG_ALL);
	for (uint64 i = debug->next; i < end; i++)
	{
		SIZE_T messageLength = 0;
		debug->info_queue->GetMessage(DXGI_DEBUG_ALL, i, nullptr, &messageLength);

		auto storage = std::make_unique<byte[]>(messageLength);
		byte* bytes = storage.get();
		DXGI_INFO_QUEUE_MESSAGE* message = reinterpret_cast<DXGI_INFO_QUEUE_MESSAGE*>(bytes);

		debug->info_queue->GetMessage(DXGI_DEBUG_ALL, i, message, &messageLength);

		SOLERROR(message->pDescription);
	}
}

#endif

EDITOR_INTERFACE(bool) CreateDeviceAndContext(void **device, void** context)
{
#if SOL_DEBUG_RENDERING
    InitializeDirectXDebugLogging();
#endif
   
	D3D_FEATURE_LEVEL featureLevel = D3D_FEATURE_LEVEL_11_0;
	uint32 deviceDebug = 0;
#if SOL_DEBUG_RENDERING
	deviceDebug = D3D11_CREATE_DEVICE_DEBUG;
#endif

	ID3D11Device* tempDevice = nullptr;
	ID3D11DeviceContext* tempContext = nullptr;
	auto hr = D3D11CreateDevice(
		NULL, D3D_DRIVER_TYPE_HARDWARE,
		NULL, deviceDebug,
		NULL, 0, D3D11_SDK_VERSION,
		&tempDevice, &featureLevel, &tempContext
	);

	if (SUCCEEDED(hr))
	{
		hr = tempDevice->QueryInterface(__uuidof(ID3D11Device1), (void**)&renderState.device);
		if (SUCCEEDED(hr))
		{
			hr = tempContext->QueryInterface(__uuidof(ID3D11DeviceContext), (void**)&renderState.context);
			if (SUCCEEDED(hr))
			{
#if SOL_DEBUG_RENDERING
				hr = tempDevice->QueryInterface(__uuidof(ID3D11Debug), reinterpret_cast<void**>(&renderState.debug.debug));
				if (FAILED(hr))
				{
					SOLFATAL("INTERNAL: DX11 Could not find ID3D11Debug interface");
					return false;
				}
#endif

				*device = renderState.device;
				*context = renderState.context;

				SOLINFO("INTERNAL: Created device");
				return true;
			}
		}
	}

	return false;
}

EDITOR_INTERFACE(void*) CreateSwapchain()
{
	IDXGIFactory2* dxgiFactory = nullptr;
	auto hr = CreateDXGIFactory1(IID_PPV_ARGS(&dxgiFactory));

	if (SUCCEEDED(hr))
	{
		DXGI_SWAP_CHAIN_DESC1 swapChainDesc = {};
		swapChainDesc.Width = 0;
		swapChainDesc.Height = 0;
		swapChainDesc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
		swapChainDesc.SampleDesc.Count = 1;
		swapChainDesc.SampleDesc.Quality = 0;
		swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
		swapChainDesc.BufferCount = 1;
		//swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL;
		swapChainDesc.Flags = 0;

		HWND window = winState.window;
		DXCHECK(dxgiFactory->CreateSwapChainForHwnd(renderState.device, window,
			&swapChainDesc, NULL, NULL, &renderState.swapChain.swapChain));

		//DXRELEASE(dxgiFactory);
		//if (SUCCEEDED(hr))
		//{
		//	SOLINFO("INTERNAL: Created swapchain");
			return renderState.swapChain.swapChain;
		//}	
	}

	return nullptr;
}

EDITOR_INTERFACE(void*) CreateSwapchainRenderTargetView()
{
	ID3D11Resource* backBuffer = nullptr;
	DXCHECK(renderState.swapChain.swapChain->GetBuffer(0, __uuidof(ID3D11Resource), (void**)&backBuffer));
	DXCHECK(renderState.device->CreateRenderTargetView(backBuffer, nullptr, &renderState.swapChain.renderView));
	backBuffer->Release();

	return renderState.swapChain.renderView;
}

EDITOR_INTERFACE(void*) CreateSwapchainDepthStencilView()
{
	HWND window = winState.window;
	RECT windowRect = {};
	GetClientRect(window, &windowRect);

	D3D11_TEXTURE2D_DESC depthDesc = {};
	depthDesc.Width = (uint32)(windowRect.right - windowRect.left);
	depthDesc.Height = (uint32)(windowRect.bottom - windowRect.top);
	depthDesc.MipLevels = 1;
	depthDesc.ArraySize = 1;
	depthDesc.Format = DXGI_FORMAT_R32_TYPELESS;
	depthDesc.SampleDesc.Count = 1;
	depthDesc.SampleDesc.Quality = 0;
	depthDesc.Usage = D3D11_USAGE_DEFAULT;
	depthDesc.BindFlags = D3D11_BIND_DEPTH_STENCIL;
	DXINFO(renderState.device->CreateTexture2D(&depthDesc, nullptr, &renderState.swapChain.depthTexture));
	
	D3D11_DEPTH_STENCIL_VIEW_DESC depthViewDesc = {};
	depthViewDesc.Format = DXGI_FORMAT_D32_FLOAT;
	depthViewDesc.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2D;
	depthViewDesc.Texture2D.MipSlice = 0;
	DXCHECK(renderState.device->CreateDepthStencilView(renderState.swapChain.depthTexture, &depthViewDesc, &renderState.swapChain.depthView));

	return renderState.swapChain.depthView;
}

EDITOR_INTERFACE(void) SwapchainPresent(int vsync)
{
	DXGI_PRESENT_PARAMETERS parameters = { 0 };
	//DXCHECK(renderState.swapChain.swapChain->Present(0, 0));
	//XCHECK(renderState.swapChain.swapChain->Present1(0, DXGI_PRESENT_RESTART, &parameters));
	DXCHECK(renderState.swapChain.swapChain->Present1(vsync, 0, &parameters));
	//DXCHECK(renderState.swapChain.swapChain->Present1(0, DXGI_PRESENT_DO_NOT_WAIT, &parameters));
}

struct DepthStencilDesc
{
	bool32 DepthTest;
	bool32 DepthWrite;
	int32 DepthFunc;
	bool32 StencilEnable;
	uint8 StencilReadMask;
	uint8 StencilWriteMask;
};

EDITOR_INTERFACE(void*) DeviceCreateDepthStencilState(DepthStencilDesc *desc)
{
	D3D11_DEPTH_STENCIL_DESC ds = {};
	ds.DepthEnable = desc->DepthTest;
	ds.DepthWriteMask = desc->DepthWrite ? D3D11_DEPTH_WRITE_MASK_ALL : D3D11_DEPTH_WRITE_MASK_ZERO;
	ds.DepthFunc = (D3D11_COMPARISON_FUNC)desc->DepthFunc;
	ds.StencilEnable = desc->StencilEnable;
	ds.StencilReadMask = desc->StencilReadMask;
	ds.StencilWriteMask = desc->StencilWriteMask;

	ID3D11DepthStencilState* d = nullptr;
	DXCHECK(renderState.device->CreateDepthStencilState(&ds, &d));

	return d;
}

EDITOR_INTERFACE(void*) DeviceCreateRasterizerState(void *desc)
{
	D3D11_RASTERIZER_DESC rsDesc = *(D3D11_RASTERIZER_DESC*)desc;
	ID3D11RasterizerState* rs = nullptr;
	DXCHECK(renderState.device->CreateRasterizerState(&rsDesc, &rs));
	
	return rs;
}

EDITOR_INTERFACE(void*) DeviceCreateBlendState(void* desc)
{
	D3D11_BLEND_DESC blendDesc = *(D3D11_BLEND_DESC*)desc;
	ID3D11BlendState* blend = nullptr;
	DXCHECK(renderState.device->CreateBlendState(&blendDesc, &blend));

	return blend;
}

EDITOR_INTERFACE(void*) DeviceCreateSamplerState(void* desc)
{
	D3D11_SAMPLER_DESC sampDesc = *(D3D11_SAMPLER_DESC*)desc;
	ID3D11SamplerState* sampler = nullptr;
	DXCHECK(renderState.device->CreateSamplerState(&sampDesc, &sampler));
	
	return sampler;
}

EDITOR_INTERFACE(void*) DeviceCreateBuffer(void* desc, void* data)
{
	D3D11_BUFFER_DESC bufferDesc = *(D3D11_BUFFER_DESC*)desc;
	
	ID3D11Buffer* buffer = nullptr;
	if (data)
	{
		D3D11_SUBRESOURCE_DATA sub = *(D3D11_SUBRESOURCE_DATA*)data;		
		DXCHECK(renderState.device->CreateBuffer(&bufferDesc, &sub, &buffer));
	}
	else
	{
		DXCHECK(renderState.device->CreateBuffer(&bufferDesc, nullptr, &buffer));
	}

	return buffer;
}

EDITOR_INTERFACE(void*) DeviceCreateBufferNoSub(void* desc)
{
	return DeviceCreateBuffer(desc, nullptr);
}

inline std::wstring AnsiToWString(const std::string& str)
{
	WCHAR buffer[512];
	MultiByteToWideChar(CP_ACP, 0, str.c_str(), -1, buffer, 512);
	return std::wstring(buffer);
}

EDITOR_INTERFACE(void*) DeviceCompileShader(const char* code, const char* entry, const char* target)
{
	ID3DBlob* shader = nullptr;
	ID3DBlob* errorBuff = nullptr;

	int flags = 0;
#if SOL_DEBUG_RENDERING
	flags = D3DCOMPILE_DEBUG | D3DCOMPILE_SKIP_OPTIMIZATION;
#endif

	HRESULT hr = D3DCompile(
		code, strlen(code),
		NULL, NULL, D3D_COMPILE_STANDARD_FILE_INCLUDE,
		entry, target, flags, 0, &shader, &errorBuff);

	if (FAILED(hr))
	{
		OutputDebugStringA((char*)errorBuff->GetBufferPointer());
		SOLERROR((char*)errorBuff->GetBufferPointer());
		return nullptr;
	}

	return shader;
}

EDITOR_INTERFACE(void*) DeviceCreateInputLayout(void* lts, int count, void* vertexBlob)
{
	D3D11_INPUT_ELEMENT_DESC* layouts = (D3D11_INPUT_ELEMENT_DESC*)lts;
	ID3DBlob* vertexData = (ID3DBlob*)vertexBlob;
	ID3D11InputLayout* layout = nullptr;

	DXCHECK(renderState.device->CreateInputLayout(layouts, count, vertexData->GetBufferPointer(), vertexData->GetBufferSize(), &layout));

	return layout;
}

EDITOR_INTERFACE(void*) DeviceCreateVertexShader(void *blob)
{
	ID3DBlob* vertexData = (ID3DBlob*)blob;
	ID3D11VertexShader* vs = nullptr;

	DXCHECK(renderState.device->CreateVertexShader(vertexData->GetBufferPointer(),
		vertexData->GetBufferSize(), nullptr, &vs));

	return vs;
}

EDITOR_INTERFACE(void*) DeviceCreatePixelShader(void* blob)
{
	ID3DBlob* pixelData = (ID3DBlob*)blob;
	ID3D11PixelShader* ps = nullptr;

	DXCHECK(renderState.device->CreatePixelShader(pixelData->GetBufferPointer(),
		pixelData->GetBufferSize(), nullptr, &ps));

	return ps;
}

EDITOR_INTERFACE(void) Release(void* d3d11Object)
{
	IUnknown* obj = (IUnknown*)d3d11Object;
	obj->Release();
}

EDITOR_INTERFACE(void) ContextClearRenderTargetView(void* rt, float r, float g, float b, float a)
{
	float colour[4] = { r, g, b, a };
	DXINFO(renderState.context->ClearRenderTargetView((ID3D11RenderTargetView*)rt, colour));
}

EDITOR_INTERFACE(void) ContextClearDepthStencilView(void* rt, uint32 clearFlags, float depth, uint8 stencil)
{
	DXINFO(renderState.context->ClearDepthStencilView((ID3D11DepthStencilView*)rt, clearFlags, depth, stencil));
}

EDITOR_INTERFACE(void) ContextSetRenderTargets(void** renderTargetViews, uint32 count, void* depthStencilView)
{	
	ID3D11RenderTargetView** views = (ID3D11RenderTargetView **)renderTargetViews;
	DXINFO(renderState.context->OMSetRenderTargets(count, views, (ID3D11DepthStencilView*)depthStencilView));
}

EDITOR_INTERFACE(void) ContextSetViewPortState(float width, float height, float minDepth, float maxDepth, float topLeftX, float topLeftY)
{
	D3D11_VIEWPORT viewport = {};
	viewport.Width = width;
	viewport.Height = height;
	viewport.MinDepth = minDepth;
	viewport.MaxDepth = maxDepth;
	viewport.TopLeftX = topLeftX;
	viewport.TopLeftY = topLeftY;

	DXINFO(renderState.context->RSSetViewports(1, &viewport));
}

EDITOR_INTERFACE(void) ContextSetPrimitiveTopology(int topo)
{
	DXINFO(renderState.context->IASetPrimitiveTopology((D3D_PRIMITIVE_TOPOLOGY)topo));
}

EDITOR_INTERFACE(void) ContextSetDepthStencilState(void* depthStencilState, uint32 stencilRef)
{
	DXINFO(renderState.context->OMSetDepthStencilState((ID3D11DepthStencilState*)depthStencilState, stencilRef));
}

EDITOR_INTERFACE(void) ContextSetRasterizerState(void* rasterizerState)
{
	DXINFO(renderState.context->RSSetState((ID3D11RasterizerState*)rasterizerState));
}

EDITOR_INTERFACE(void) ContextSetBlendState(void* blendState, float blendFactor0, float blendFactor1, float blendFactor2, float blendFactor3, uint32 sampleMask)
{
	float blendFactors[4] = { blendFactor0, blendFactor1, blendFactor2, blendFactor3 };
	DXINFO(renderState.context->OMSetBlendState((ID3D11BlendState*)blendState, nullptr, sampleMask));
}

EDITOR_INTERFACE(void) ContextSetInputLayout(void* inputLayout)
{
	DXINFO(renderState.context->IASetInputLayout((ID3D11InputLayout*)inputLayout));
}

EDITOR_INTERFACE(void) ContextSetVertexShader(void* vertexShader)
{
	DXINFO(renderState.context->VSSetShader((ID3D11VertexShader*)vertexShader, nullptr, 0));
}

EDITOR_INTERFACE(void) ContextSetPixelShader(void* pixelShader)
{
	DXINFO(renderState.context->PSSetShader((ID3D11PixelShader*)pixelShader, nullptr, 0));
}

EDITOR_INTERFACE(void) ContextSetVertexBuffers(void* vertexBuffer, uint32 stride)
{
	uint32 offset = 0;
	ID3D11Buffer* buffers[] = { (ID3D11Buffer*) vertexBuffer };
	DXINFO(renderState.context->IASetVertexBuffers(0, 1, buffers, &stride, &offset));
}

EDITOR_INTERFACE(void) ContextSetIndexBuffer(void* indexBuffer, uint32 format, uint32 offset)
{
	DXINFO(renderState.context->IASetIndexBuffer((ID3D11Buffer*)indexBuffer, (DXGI_FORMAT)format, offset));
}

EDITOR_INTERFACE(void) ContextDrawIndexed(uint32 indexCount, uint32 startLocation, uint32 baseVertexLocation)
{
	DXINFO(renderState.context->DrawIndexed(indexCount, startLocation, baseVertexLocation));
}

EDITOR_INTERFACE(void) ContextSetVSConstBuffer(void* buf, uint32 slot)
{
	ID3D11Buffer* buffers[] = { (ID3D11Buffer*)buf };
	DXINFO(renderState.context->VSSetConstantBuffers(slot, 1, buffers));
}

EDITOR_INTERFACE(void) ContextSetPSConstBuffer(void* buf, uint32 slot)
{
	ID3D11Buffer* buffers[] = { (ID3D11Buffer*)buf };
	DXINFO(renderState.context->PSSetConstantBuffers(slot, 1, buffers));
}

EDITOR_INTERFACE(void) ContextSetCSConstBuffer(void* buf, uint32 slot)
{
	ID3D11Buffer* buffers[] = { (ID3D11Buffer*)buf };
	DXINFO(renderState.context->CSSetConstantBuffers(slot, 1, buffers));
}

EDITOR_INTERFACE(void) ContextUpdateSubresource(void* buffer, uint32 subResource, void* box, void* data, uint32 rowPitch, uint32 depthPitch)
{
	DXINFO(renderState.context->UpdateSubresource((ID3D11Resource*)buffer, subResource, (D3D11_BOX*)box, data, rowPitch, depthPitch));
}
