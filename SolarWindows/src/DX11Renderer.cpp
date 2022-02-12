#pragma once
#include "Win32Platform.h"
#include "DX11Renderer.h"
#include "SolarMath.h"
#include <system_error>

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
			dxresult = debug_fnc(__uuidof(IDXGIInfoQueue), (void**)&renderState.deviceContext.debug.info_queue);
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

//static bool8 InitializeDirectXDrawing()
//{
//	DeviceContext dc = GetDeviceContext();
//	DebugState* ds = Debug::GetState();
//
//	D3D11_BUFFER_DESC vertex_desc = {};
//	vertex_desc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
//	vertex_desc.Usage = D3D11_USAGE_DYNAMIC;
//	vertex_desc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
//	vertex_desc.MiscFlags = 0;
//	vertex_desc.ByteWidth = ds->vertexSizeBytes;
//	vertex_desc.StructureByteStride = sizeof(real32) * ds->vertexStride;
//
//	D3D11_SUBRESOURCE_DATA vertex_res = {};
//	vertex_res.pSysMem = ds->vertexData.data;
//
//	DXCHECK(dc.device->CreateBuffer(&vertex_desc, &vertex_res, &renderState.deviceContext.debug.vertexBuffer));
//
//	ProgramInstance::DEBUGCompileFromFile("Engine/src/renderer/shaders/DebugLine.hlsl", VertexLayoutType::Value::P, &renderState.deviceContext.debug.program);
//
//	return (renderState.deviceContext.debug.program.vs != nullptr) && (renderState.deviceContext.debug.vertexBuffer != nullptr);
//}

//void DEBUGRenderAndFlushDebugDraws()
//{
//	DeviceContext dc = GetDeviceContext();
//	RenderDebug* rd = &renderState.deviceContext.debug;
//	DebugState* ds = Debug::GetState();
//
//	D3D11_MAPPED_SUBRESOURCE resource = {};
//
//	DXCHECK(dc.context->Map(rd->vertexBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &resource));
//	memcpy(resource.pData, ds->vertexData.data, ds->vertexSizeBytes);
//	DXINFO(dc.context->Unmap(rd->vertexBuffer, 0));
//
//	uint32 offset = 0;
//	uint32 vertex_stride_bytes = ds->vertexStride * sizeof(real32);
//	DXINFO(dc.context->IASetVertexBuffers(0, 1, &rd->vertexBuffer, &vertex_stride_bytes, &offset));
//
//	//////////////////////////////////
//	//////////////////////////////////
//
//	RenderCommand::SetProgram(rd->program);
//
//	//////////////////////////////////
//	//////////////////////////////////
//
//	DXINFO(dc.context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_LINELIST));
//
//	DXINFO(dc.context->Draw(ds->nextVertexIndex, 0));
//
//	DXINFO(dc.context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST));
//
//	//////////////////////////////////
//	//////////////////////////////////
//
//	ds->vertexData.Clear();
//	ds->nextVertexIndex = 0;
//}
//

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


static void CreateSwapChainBuffers()
{
	HWND window = winState.window;

	// @NOTE: Get back buffer
	ID3D11Resource* backBuffer = nullptr;
	DXCHECK(renderState.swapChain.swapChain->GetBuffer(0, __uuidof(ID3D11Resource), (void**)&backBuffer));
	DXCHECK(renderState.deviceContext.device->CreateRenderTargetView(backBuffer, nullptr, &renderState.swapChain.renderView));
	backBuffer->Release();

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
	depthDesc.BindFlags = D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_DEPTH_STENCIL;
	DXINFO(renderState.deviceContext.device->CreateTexture2D(&depthDesc, nullptr, &renderState.swapChain.depthTexture));

	D3D11_DEPTH_STENCIL_VIEW_DESC depthViewDesc = {};
	depthViewDesc.Format = DXGI_FORMAT_D32_FLOAT;
	depthViewDesc.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2D;
	depthViewDesc.Texture2D.MipSlice = 0;
	DXCHECK(renderState.deviceContext.device->CreateDepthStencilView(renderState.swapChain.depthTexture, &depthViewDesc, &renderState.swapChain.depthView));

	D3D11_SHADER_RESOURCE_VIEW_DESC shaderViewDesc = {};
	shaderViewDesc.Format = DXGI_FORMAT_R32_FLOAT;
	shaderViewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
	shaderViewDesc.Texture2D.MostDetailedMip = 0;
	shaderViewDesc.Texture2D.MipLevels = 1;
	DXCHECK(renderState.deviceContext.device->CreateShaderResourceView(renderState.swapChain.depthTexture, &shaderViewDesc, &renderState.swapChain.depthShaderView));
}

EDITOR_INTERFACE(bool) Win32CreateRenderer()
{
#if SOL_DEBUG_RENDERING
    InitializeDirectXDebugLogging();
#endif
    
	HWND window = winState.window;
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
		hr = tempDevice->QueryInterface(__uuidof(ID3D11Device1), (void**)&renderState.deviceContext.device);
		if (SUCCEEDED(hr))
		{
			hr = tempContext->QueryInterface(__uuidof(ID3D11DeviceContext), (void**)&renderState.deviceContext.context);
			if (SUCCEEDED(hr))
			{
#if SOL_DEBUG_RENDERING
				hr = tempDevice->QueryInterface(__uuidof(ID3D11Debug), reinterpret_cast<void**>(&renderState.deviceContext.debug.debug));
				if (FAILED(hr))
				{
					SOLFATAL("DX11 Could not find ID3D11Debug interface");
					return false;
				}

				//if (InitializeDirectXDrawing())
				//{
				//	SOLINFO("DX11 Debug drawing enabled");
				//}
				//else
				//{
				//	SOLFATAL("DX11 Could not create debug drawing");
				//	return false;
				//}
#endif

				IDXGIFactory2* dxgiFactory = nullptr;
				hr = CreateDXGIFactory1(IID_PPV_ARGS(&dxgiFactory));
				if (SUCCEEDED(hr))
				{
					DXGI_SWAP_CHAIN_DESC1 swapChainDesc = {};
					swapChainDesc.Width = 0;
					swapChainDesc.Height = 0;
					swapChainDesc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
					swapChainDesc.Stereo = FALSE;
					swapChainDesc.SampleDesc.Count = 1;
					swapChainDesc.SampleDesc.Quality = 0;
					swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
					swapChainDesc.BufferCount = 3;
					swapChainDesc.Scaling = DXGI_SCALING_STRETCH;
					swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL;
					swapChainDesc.AlphaMode = DXGI_ALPHA_MODE_UNSPECIFIED;
					swapChainDesc.Flags = 0;
					
					hr = dxgiFactory->CreateSwapChainForHwnd(renderState.deviceContext.device, window,
						&swapChainDesc, NULL, NULL, &renderState.swapChain.swapChain);


					DXRELEASE(dxgiFactory);

					if (SUCCEEDED(hr))
					{
						DXNAME(renderState.deviceContext.device, "Main device");
						DXNAME(renderState.deviceContext.context, "Main context");
						DXNAME(renderState.swapChain.swapChain, "Swap chain");

						CreateSwapChainBuffers();
						SOLINFO("DX11 Swapchain and device created");

						return true;
					}
				else
				{
					SOLFATAL("DX11 Could not create swapchain1, please make sure all drivers are up to date.")
				}
				}
				else
				{
					SOLFATAL("DX11 Could not create factory2, please make sure all drivers are up to date.")
				}
			}
			else
			{
				SOLFATAL("DX11 Could not create context1, please make sure all drivers are up to date.")
			}
		}
		else
		{
			SOLFATAL("DX11 Could not create device1, please make sure all drivers are up to date.")
		}
	}
	else
	{
		SOLFATAL("DX11 Could not create device, please make sure all drivers are up to date.")
	}

    return false;
}

EDITOR_INTERFACE(void) Win32DestroyRenderer()
{
	DXINFO(renderState.deviceContext.context->ClearState());

	DXRELEASE(renderState.swapChain.depthView);
	DXRELEASE(renderState.swapChain.depthShaderView);
	DXRELEASE(renderState.swapChain.renderView);
	DXRELEASE(renderState.swapChain.depthTexture);
	DXRELEASE(renderState.swapChain.swapChain);
	DXINFO(renderState.deviceContext.context->Flush());
	DXRELEASE(renderState.deviceContext.context);
	DXRELEASE(renderState.deviceContext.device);

#if SOL_DEBUG_RENDERING
	renderState.deviceContext.debug.info_queue->Release();
	renderState.deviceContext.debug.debug->Release();
#endif
}

void RendererClearRenderTarget(ID3D11RenderTargetView* target, const Vec4f& colour)
{
	DeviceContext dc = renderState.deviceContext;
	DXINFO(dc.context->ClearRenderTargetView(target, colour.ptr));
}

void RendererClearDepthBuffer(ID3D11DepthStencilView* depth)
{
	DeviceContext dc = renderState.deviceContext;
	DXINFO(dc.context->ClearDepthStencilView(depth, D3D11_CLEAR_DEPTH, 1.0f, 0))
}

void RendererSetRenderTargets(ID3D11RenderTargetView* colour0, ID3D11DepthStencilView* depth)
{
	DeviceContext dc = renderState.deviceContext;
	ID3D11RenderTargetView* views[] = { colour0 };
	DXINFO(dc.context->OMSetRenderTargets(1, views, depth));
}

EDITOR_INTERFACE(void) RendererBeginFrame()
{
	RendererClearRenderTarget(renderState.swapChain.renderView, Vec4f(0.2f, 0.2f, 0.2f, 1.0f));
	RendererClearDepthBuffer(renderState.swapChain.depthView);

	RendererSetRenderTargets(renderState.swapChain.renderView, renderState.swapChain.depthView);
}

EDITOR_INTERFACE(void) RendererEndFrame(int vsync)
{
	DXGI_PRESENT_PARAMETERS parameters = { 0 };
	DXCHECK(renderState.swapChain.swapChain->Present1(vsync, 0, &parameters));
	//DXCHECK(renderState.swapChain.swapChain->Present1(0, DXGI_PRESENT_DO_NOT_WAIT, &parameters));
}

EDITOR_INTERFACE(void) RendererSetViewportState(int width, int height)
{
	D3D11_VIEWPORT viewport = {};

	viewport.Width = (float)width;
	viewport.Height = (float)height;
	viewport.MinDepth = 0.0f;
	viewport.MaxDepth = 1.0f;
	viewport.TopLeftX = 0.0f;
	viewport.TopLeftY = 0.0f;

	DXINFO(renderState.deviceContext.context->RSSetViewports(1, &viewport));
}

EDITOR_INTERFACE(void) RendererSetTopologyState(int topo)
{
	DXINFO(renderState.deviceContext.context->IASetPrimitiveTopology((D3D_PRIMITIVE_TOPOLOGY)topo));
}

EDITOR_INTERFACE(void) RendererSetRasterState(int id)
{
	ID3D11RasterizerState* rasterState = renderState.rasterStates.at(id);
	DXINFO(renderState.deviceContext.context->RSSetState(rasterState));
}

EDITOR_INTERFACE(void) RendererSetDepthState(int id)
{
	ID3D11DepthStencilState* depthState = renderState.depthStates.at(id);
	DXINFO(renderState.deviceContext.context->OMSetDepthStencilState(depthState, 1));
}

EDITOR_INTERFACE(void) RendererSetBlendState(int id)
{
	ID3D11BlendState* blendState = renderState.blendStates.at(id);
	DXINFO(renderState.deviceContext.context->OMSetBlendState(blendState, nullptr, 0xffffffff));
}

EDITOR_INTERFACE(void) RendererSetSamplerState(int id, int slot)
{
	Assert(slot >= 0, "Invalid sampler register");

	ID3D11SamplerState* sampler = renderState.sampleStates.at(id);
	DXINFO(renderState.deviceContext.context->PSSetSamplers(slot, 1, &sampler));
	DXINFO(renderState.deviceContext.context->CSSetSamplers(slot, 1, &sampler));
}

EDITOR_INTERFACE(void) RendererDrawStaticMesh(int id)
{
	uint32 offset = 0;

	StaticMesh mesh = renderState.staticMeshes.at(id);
	DXINFO(renderState.deviceContext.context->IASetVertexBuffers(0, 1, &mesh.vertexBuffer, &mesh.strideBytes, &offset));
	DXINFO(renderState.deviceContext.context->IASetIndexBuffer(mesh.indexBuffer, DXGI_FORMAT_R32_UINT, 0));
	DXINFO(renderState.deviceContext.context->DrawIndexed(mesh.indexCount, 0, 0));
}

EDITOR_INTERFACE(void) RendererSetStaticProgram(int id)
{
	StaticProgram program = renderState.staticPrograms.at(id);
	if (program.vs)
	{
		DXINFO(renderState.deviceContext.context->VSSetShader(program.vs, nullptr, 0));
		DXINFO(renderState.deviceContext.context->IASetInputLayout(program.layout));
	}
	if (program.ps)
	{
		DXINFO(renderState.deviceContext.context->PSSetShader(program.ps, nullptr, 0));
	}
	if (program.cs)
	{
		DXINFO(renderState.deviceContext.context->CSSetShader(program.cs, nullptr, 0));
	}
}

EDITOR_INTERFACE(void) RendererSetVertexConstBuffer(int id, int slot)
{
	ID3D11Buffer* buffer = renderState.constBuffers.at(id);
	DXINFO(renderState.deviceContext.context->VSSetConstantBuffers(slot, 1, &buffer));
}

struct Temp
{
	Mat4f mvp;
	Mat4f model;
	Mat4f inv;
};
static bool swapper = true;

EDITOR_INTERFACE(void) RendererSetConstBufferData(int id, float *data)
{
	Temp temp = {};

	Mat4f proj = PerspectiveLH(DegToRad(45.0f), 1.777777f, 0.1f, 100.0f);
	Mat4f view = Inverse(LookAtLH(Vec3f(3), Vec3f(0), Vec3f(0, 1, 0)));

	if (swapper)
	{
		Mat4f m = Mat4f(1);
		Mat4f mvp = Transpose(m * view * proj);
		temp.mvp = mvp;
		swapper = false;
	}
	else
	{
		Mat4f m = Translate(Mat4f(1), Vec3f(0, 4, 0));

		Mat4f mvp = Transpose(m * view * proj);
		temp.mvp = mvp;
		swapper = true;
	}
	

	//temp.mvp =  Translate(Mat4f(1), Vec3f(4, 0, 3)) * PerspectiveLH(40.0f, 1.777777f, 0.1f, 100.0f);

	//std::cout << temp.mvp.row0.x << " " << temp.mvp.row0.y << " " << temp.mvp.row0.z << " " << temp.mvp.row0.w << std::endl;
	//std::cout << temp.mvp.row1.x << " " << temp.mvp.row1.y << " " << temp.mvp.row1.z << " " << temp.mvp.row1.w << std::endl;
	//std::cout << temp.mvp.row2.x << " " << temp.mvp.row2.y << " " << temp.mvp.row2.z << " " << temp.mvp.row2.w << std::endl;
	//std::cout << temp.mvp.row3.x << " " << temp.mvp.row3.y << " " << temp.mvp.row3.z << " " << temp.mvp.row3.w << std::endl;

	//temp.mvp = Transpose(temp.mvp);

	//for (int i = 0; i < 16; i++)
	//	std::cout << data[i] << "  ";
	//std::cout << std::endl;

	//for (int i = 0; i < 16; i++)
	//	std::cout << temp.mvp.ptr[i] << "  ";
	//std::cout << std::endl;

	ID3D11Buffer* buffer = renderState.constBuffers.at(id);
	DXINFO(renderState.deviceContext.context->UpdateSubresource(buffer, 0, nullptr,data, 0, 0));
}

template<typename T, size_t size>
int FindFree(const std::array<T, size> &arr)
{
	for (int i = 1; i < (int)arr.size(); i++)
	{
		if (!arr.at(i))
		{
			return i;			
		}
	}

	return 0;
}

EDITOR_INTERFACE(int) RendererCreateConstBuffer(int sizeBytes)
{
	D3D11_BUFFER_DESC desc = {};
	desc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
	desc.Usage = D3D11_USAGE_DEFAULT; //D3D11_USAGE_DYNAMIC;
	desc.CPUAccessFlags = 0;
	desc.MiscFlags = 0;
	desc.ByteWidth = sizeBytes;
	desc.StructureByteStride = 0;

	int index = FindFree(renderState.constBuffers);

	if (index)
	{
		DXCHECK(renderState.deviceContext.device->CreateBuffer(&desc, NULL, &renderState.constBuffers.at(index)));
	}
	else
	{
		SOLERROR("RendererCreateConstBuffer");
	}

	return index;
}

EDITOR_INTERFACE(int) RendererCreateRasterState(int fillMode, int cullMode)
{
	D3D11_RASTERIZER_DESC rsDesc = {};
	rsDesc.FillMode = (D3D11_FILL_MODE)fillMode;
	rsDesc.CullMode = (D3D11_CULL_MODE)cullMode;
	rsDesc.FrontCounterClockwise = TRUE;
	rsDesc.DepthBias = 0;
	rsDesc.DepthBiasClamp = 1.0f;
	rsDesc.SlopeScaledDepthBias = 0.0f;
	rsDesc.DepthClipEnable = TRUE;
	rsDesc.MultisampleEnable = FALSE;
	rsDesc.AntialiasedLineEnable = FALSE;

	int index = FindFree(renderState.rasterStates);
	
	if (index)
	{
		DXCHECK(renderState.deviceContext.device->CreateRasterizerState(&rsDesc, &renderState.rasterStates.at(index)));
	}
	else
	{
		SOLERROR("RendererCreateRasterState");
	}
	
	return index;
}

EDITOR_INTERFACE(int) RendererCreateDepthStencilState(int enabled, int write, int comparison)
{
	D3D11_DEPTH_STENCIL_DESC ds = {};
	ds.DepthEnable = enabled;
	ds.DepthWriteMask = write ? D3D11_DEPTH_WRITE_MASK_ALL : D3D11_DEPTH_WRITE_MASK_ZERO;
	ds.DepthFunc = (D3D11_COMPARISON_FUNC)comparison; 
	ds.StencilEnable = FALSE;

	int index = FindFree(renderState.depthStates);
	if (index)
	{	
		DXCHECK(renderState.deviceContext.device->CreateDepthStencilState(&ds, &renderState.depthStates.at(index)));
	}
	else
	{
		SOLERROR("RendererCreateDepthStencilState");
	}

	return index;
}

EDITOR_INTERFACE(int) RendererCreateSamplerState(int filter, int wrapMode)
{
	D3D11_SAMPLER_DESC sampDesc = {};
	sampDesc.Filter = (D3D11_FILTER)filter;
	sampDesc.AddressU = (D3D11_TEXTURE_ADDRESS_MODE)wrapMode;
	sampDesc.AddressV = (D3D11_TEXTURE_ADDRESS_MODE)wrapMode;
	sampDesc.AddressW = (D3D11_TEXTURE_ADDRESS_MODE)wrapMode;

	int index = FindFree(renderState.sampleStates);

	if (index)
	{
		DXCHECK(renderState.deviceContext.device->CreateSamplerState(&sampDesc, &renderState.sampleStates.at(index)));
	}
	else
	{
		SOLERROR("RendererCreateSamplerState");
	}

	return index;
}

EDITOR_INTERFACE(int) RendererCreateBlendState()
{
	D3D11_BLEND_DESC blendDesc = {};
	blendDesc.RenderTarget[0].BlendEnable = true;
	blendDesc.RenderTarget[0].SrcBlend = D3D11_BLEND_SRC_ALPHA;
	blendDesc.RenderTarget[0].DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
	blendDesc.RenderTarget[0].BlendOp = D3D11_BLEND_OP_ADD;
	blendDesc.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_ZERO;
	blendDesc.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_ZERO;
	blendDesc.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
	blendDesc.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;
	
	int index = FindFree(renderState.blendStates);
	if (index)
	{
		DXCHECK(renderState.deviceContext.device->CreateBlendState(&blendDesc, &renderState.blendStates.at(index)));
		renderState.deviceContext.context->OMSetBlendState(renderState.blendStates.at(index), nullptr, 0xffffffff);
	}
	else
	{
		SOLERROR("RendererCreateBlendState");
	}
	
	return index;	
}

EDITOR_INTERFACE(int) RendererCreateStaticMesh(float* vertices, int vertexCount, uint32* indices, int indexCount, int _layout)
{
	VertexLayoutType layout = VertexLayoutType(_layout);

	uint32 vertexStrideBytes = sizeof(real32) * layout.GetStride();
	uint32 indicesStrideBytes = sizeof(uint32);

	D3D11_BUFFER_DESC vertex_desc = {};
	vertex_desc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
	vertex_desc.Usage = D3D11_USAGE_DEFAULT;
	vertex_desc.CPUAccessFlags = 0;
	vertex_desc.MiscFlags = 0;
	vertex_desc.ByteWidth = vertexCount * sizeof(real32);
	vertex_desc.StructureByteStride = vertexStrideBytes;

	D3D11_SUBRESOURCE_DATA vertex_res = {};
	vertex_res.pSysMem = vertices;

	D3D11_BUFFER_DESC index_desc = {};
	index_desc.BindFlags = D3D11_BIND_INDEX_BUFFER;
	index_desc.Usage = D3D11_USAGE_DEFAULT;
	index_desc.CPUAccessFlags = 0;
	index_desc.MiscFlags = 0;
	index_desc.ByteWidth = indexCount * sizeof(uint32);
	index_desc.StructureByteStride = vertexStrideBytes;

	D3D11_SUBRESOURCE_DATA index_res = {};
	index_res.pSysMem = indices;

	int index = FindFree(renderState.staticMeshes);

	if (index)
	{
		DXCHECK(renderState.deviceContext.device->CreateBuffer(&vertex_desc, &vertex_res, &renderState.staticMeshes.at(index).vertexBuffer));
		DXCHECK(renderState.deviceContext.device->CreateBuffer(&index_desc, &index_res, &renderState.staticMeshes.at(index).indexBuffer));
		renderState.staticMeshes.at(index).strideBytes = vertexStrideBytes;
		renderState.staticMeshes.at(index).indexCount = indexCount;
	}
	else
	{
		SOLERROR("RendererCreateStaticMesh");
	}

	return index;
}

#define ArrayCount(Array) (sizeof(Array) / sizeof((Array)[0]))

StaticProgram CreateGraphicsProgram(ID3DBlob* vertexData, ID3DBlob* pixelData, VertexLayoutType vertexLayout)
{
	DeviceContext dc = renderState.deviceContext;

	StaticProgram program = {};

	switch (vertexLayout.Get())
	{
	case VertexLayoutType::Value::P:
	{
		D3D11_INPUT_ELEMENT_DESC pos_desc = {};
		pos_desc.SemanticName = "Position";
		pos_desc.SemanticIndex = 0;
		pos_desc.Format = DXGI_FORMAT_R32G32B32_FLOAT;
		pos_desc.InputSlot = 0;
		pos_desc.AlignedByteOffset = 0;
		pos_desc.InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
		pos_desc.InstanceDataStepRate = 0;

		D3D11_INPUT_ELEMENT_DESC layouts[] = { pos_desc };

		DXCHECK(dc.device->CreateInputLayout(layouts, ArrayCount(layouts), vertexData->GetBufferPointer(),
			vertexData->GetBufferSize(), &program.layout));

		DXCHECK(dc.device->CreateVertexShader(vertexData->GetBufferPointer(),
			vertexData->GetBufferSize(), nullptr, &program.vs));

		DXCHECK(dc.device->CreatePixelShader(pixelData->GetBufferPointer(),
			pixelData->GetBufferSize(), nullptr, &program.ps));

		return program;
	}break;

	case VertexLayoutType::Value::PNT:
	{
		D3D11_INPUT_ELEMENT_DESC pos_desc = {};
		pos_desc.SemanticName = "Position";
		pos_desc.SemanticIndex = 0;
		pos_desc.Format = DXGI_FORMAT_R32G32B32_FLOAT;
		pos_desc.InputSlot = 0;
		pos_desc.AlignedByteOffset = 0;
		pos_desc.InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
		pos_desc.InstanceDataStepRate = 0;

		D3D11_INPUT_ELEMENT_DESC nrm_desc = {};
		nrm_desc.SemanticName = "Normal";
		nrm_desc.SemanticIndex = 0;
		nrm_desc.Format = DXGI_FORMAT_R32G32B32_FLOAT;
		nrm_desc.InputSlot = 0;
		nrm_desc.AlignedByteOffset = D3D11_APPEND_ALIGNED_ELEMENT;
		nrm_desc.InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
		nrm_desc.InstanceDataStepRate = 0;

		D3D11_INPUT_ELEMENT_DESC txc_desc = {};
		txc_desc.SemanticName = "TexCord";
		txc_desc.SemanticIndex = 0;
		txc_desc.Format = DXGI_FORMAT_R32G32_FLOAT;
		txc_desc.InputSlot = 0;
		txc_desc.AlignedByteOffset = D3D11_APPEND_ALIGNED_ELEMENT;
		txc_desc.InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
		txc_desc.InstanceDataStepRate = 0;

		D3D11_INPUT_ELEMENT_DESC layouts[] = { pos_desc, nrm_desc, txc_desc };

		DXCHECK(dc.device->CreateInputLayout(layouts, ArrayCount(layouts), vertexData->GetBufferPointer(),
			vertexData->GetBufferSize(), &program.layout));

		DXCHECK(dc.device->CreateVertexShader(vertexData->GetBufferPointer(),
			vertexData->GetBufferSize(), nullptr, &program.vs));

		DXCHECK(dc.device->CreatePixelShader(pixelData->GetBufferPointer(),
			pixelData->GetBufferSize(), nullptr, &program.ps));

	
		return program;
	}break;
	case VertexLayoutType::Value::PNTC:
	{
		D3D11_INPUT_ELEMENT_DESC pos_desc = {};
		pos_desc.SemanticName = "Position";
		pos_desc.SemanticIndex = 0;
		pos_desc.Format = DXGI_FORMAT_R32G32B32_FLOAT;
		pos_desc.InputSlot = 0;
		pos_desc.AlignedByteOffset = 0;
		pos_desc.InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
		pos_desc.InstanceDataStepRate = 0;

		D3D11_INPUT_ELEMENT_DESC nrm_desc = {};
		nrm_desc.SemanticName = "Normal";
		nrm_desc.SemanticIndex = 0;
		nrm_desc.Format = DXGI_FORMAT_R32G32B32_FLOAT;
		nrm_desc.InputSlot = 0;
		nrm_desc.AlignedByteOffset = D3D11_APPEND_ALIGNED_ELEMENT;
		nrm_desc.InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
		nrm_desc.InstanceDataStepRate = 0;

		D3D11_INPUT_ELEMENT_DESC txc_desc = {};
		txc_desc.SemanticName = "TexCord";
		txc_desc.SemanticIndex = 0;
		txc_desc.Format = DXGI_FORMAT_R32G32_FLOAT;
		txc_desc.InputSlot = 0;
		txc_desc.AlignedByteOffset = D3D11_APPEND_ALIGNED_ELEMENT;
		txc_desc.InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
		txc_desc.InstanceDataStepRate = 0;

		D3D11_INPUT_ELEMENT_DESC col_desc = {};
		col_desc.SemanticName = "Colour";
		col_desc.SemanticIndex = 0;
		col_desc.Format = DXGI_FORMAT_R32G32B32A32_FLOAT;
		col_desc.InputSlot = 0;
		col_desc.AlignedByteOffset = D3D11_APPEND_ALIGNED_ELEMENT;
		col_desc.InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
		col_desc.InstanceDataStepRate = 0;

		D3D11_INPUT_ELEMENT_DESC layouts[] = { pos_desc, nrm_desc, txc_desc, col_desc };

		DXCHECK(dc.device->CreateInputLayout(layouts, ArrayCount(layouts), vertexData->GetBufferPointer(),
			vertexData->GetBufferSize(), &program.layout));

		DXCHECK(dc.device->CreateVertexShader(vertexData->GetBufferPointer(),
			vertexData->GetBufferSize(), nullptr, &program.vs));

		DXCHECK(dc.device->CreatePixelShader(pixelData->GetBufferPointer(),
			pixelData->GetBufferSize(), nullptr, &program.ps));

	
		return program;
	}break;
	case VertexLayoutType::Value::TEXT:
	{
		D3D11_INPUT_ELEMENT_DESC desc = {};
		desc.SemanticName = "POSITION";
		desc.SemanticIndex = 0;
		desc.Format = DXGI_FORMAT_R32G32B32A32_FLOAT;
		desc.InputSlot = 0;
		desc.AlignedByteOffset = 0;
		desc.InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
		desc.InstanceDataStepRate = 0;
		D3D11_INPUT_ELEMENT_DESC layouts[] = { desc };

		DXCHECK(dc.device->CreateInputLayout(layouts, ArrayCount(layouts), vertexData->GetBufferPointer(),
			vertexData->GetBufferSize(), &program.layout));

		DXCHECK(dc.device->CreateVertexShader(vertexData->GetBufferPointer(),
			vertexData->GetBufferSize(), nullptr, &program.vs));

		DXCHECK(dc.device->CreatePixelShader(pixelData->GetBufferPointer(),
			pixelData->GetBufferSize(), nullptr, &program.ps));


		return program;

	}break;
	}

	SOLFATAL("Shader program -> CreateGraphics unkown format");
	Assert(0, "ShaderInstance::CreateGraphics unkown format");

	return {};
}

#include <d3dcompiler.h>

inline std::wstring AnsiToWString(const std::string& str)
{
	WCHAR buffer[512];
	MultiByteToWideChar(CP_ACP, 0, str.c_str(), -1, buffer, 512);
	return std::wstring(buffer);
}

static ID3DBlob* CompileShader(const char *code, const char* entry, const char* target)
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

EDITOR_INTERFACE(int) RendererCreateStaticProgram(const char * shaderCode, int _layout)
{
	ID3DBlob* vertexShader = CompileShader(shaderCode, "VSmain", "vs_5_0");
	ID3DBlob* pixelShader = CompileShader(shaderCode, "PSmain", "ps_5_0");

	if (!(vertexShader && pixelShader))
	{
		SOLERROR("Could not compile program");
		return 0;
	}

	StaticProgram program = CreateGraphicsProgram(vertexShader, pixelShader, VertexLayoutType(_layout));

	int index = FindFree(renderState.staticPrograms);

	if (index)
	{
		renderState.staticPrograms.at(index) = program;
	}
	else
	{
		SOLERROR("RendererCreateStaticProgram");
	}

	vertexShader->Release();
	pixelShader->Release();

	return index;
}
//
