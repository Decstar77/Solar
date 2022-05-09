using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using SolarSharp.EngineAPI;
using System.Diagnostics.CodeAnalysis;
using SolarSharp.Assets;

namespace SolarSharp.Rendering
{
    public class DirectXObject
    {
        public IntPtr Ptr { get => ptr; }
        private IntPtr ptr;
        public DirectXObject(IntPtr ptr)
        {
            this.ptr = ptr;
        }

        public void Release()
        {
            if (ptr != IntPtr.Zero) { 
                D3D11API.Release(ptr); 
            }            
            ptr = IntPtr.Zero;
        }
    }

    public enum DepthComparisonFunc
    {
        INVALID = 0,
        NEVER = 1,
        LESS = 2,
        EQUAL = 3,
        LESS_EQUAL = 4,
        GREATER = 5,
        NOT_EQUAL = 6,
        GREATER_EQUAL = 7,
        ALWAYS = 8
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DepthStencilDesc
    {
        [MarshalAs(UnmanagedType.Bool)] public bool depthTest;
        [MarshalAs(UnmanagedType.Bool)] public bool depthWrite;
        [MarshalAs(UnmanagedType.I4)] public DepthComparisonFunc depthFunc;
        [MarshalAs(UnmanagedType.Bool)] public bool stencilEnable;
        [MarshalAs(UnmanagedType.I1)] public byte stencilReadMask;
        [MarshalAs(UnmanagedType.I1)] public byte stencilWriteMask;
        //D3D11_DEPTH_STENCILOP_DESC FrontFace;
        //D3D11_DEPTH_STENCILOP_DESC BackFace;

        public bool DepthTest { get { return depthTest; } set { depthTest = value; } }
        public bool DepthWrite { get { return depthWrite; } set { depthWrite = value; } }        
        public DepthComparisonFunc DepthFunc { get { return depthFunc; } set { depthFunc = value; } }
        public bool StencilEnable { get { return stencilEnable; } set { stencilEnable = value; } }
        public byte StencilReadMask { get { return stencilReadMask; } set { stencilReadMask = value; } }        
        public byte StencilWriteMask { get { return stencilWriteMask; } set { stencilWriteMask = value; } }

        public static bool operator ==(DepthStencilDesc a, DepthStencilDesc b)
        {
            return a.DepthTest == b.DepthTest &&
                a.depthWrite == b.depthWrite &&
                a.depthFunc == b.depthFunc &&
                a.stencilEnable == b.stencilEnable &&
                a.stencilReadMask == b.stencilReadMask &&
                a.stencilWriteMask == b.stencilWriteMask;
        }

        public static bool operator !=(DepthStencilDesc a, DepthStencilDesc b)
        {
            return !(a == b);
        }
    }

    public class DXDepthStencilState : DirectXObject
    {
        public DepthStencilDesc Description { get; }
        public DXDepthStencilState(IntPtr ptr, DepthStencilDesc desc) : base(ptr)
        {
            Description = desc;
        }
    }

    public enum RasterizerFillMode
    {
        INVALID = 0,
        WIREFRAME = 2,
        SOLID = 3
    }

    public enum RasterizerCullMode
    {
        INVALID = 0,
        NONE = 1,
        FRONT = 2,
        BACK = 3
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RasterizerDesc
    {
        [MarshalAs(UnmanagedType.I4)] public RasterizerFillMode FillMode = RasterizerFillMode.SOLID;
        [MarshalAs(UnmanagedType.I4)] public RasterizerCullMode CullMode = RasterizerCullMode.NONE;
        [MarshalAs(UnmanagedType.Bool)] public bool FrontCounterClockwise = true;
        [MarshalAs(UnmanagedType.I4)] public int DepthBias = 0;
        [MarshalAs(UnmanagedType.R4)] public float DepthBiasClamp = 0.0f;
        [MarshalAs(UnmanagedType.R4)] public float SlopeScaledDepthBias = 0.0f;
        [MarshalAs(UnmanagedType.Bool)] public bool DepthClipEnable = true;
        [MarshalAs(UnmanagedType.Bool)] public bool ScissorEnable = false;
        [MarshalAs(UnmanagedType.Bool)] public bool MultisampleEnable = false;
        [MarshalAs(UnmanagedType.Bool)] public bool AntialiasedLineEnable = false;

        public RasterizerDesc()
        {

        }

        public static bool operator ==(RasterizerDesc a, RasterizerDesc b)
        {
            return a.FillMode == b.FillMode &&
                a.CullMode == b.CullMode &&
                a.FrontCounterClockwise == b.FrontCounterClockwise &&
                a.DepthBias == b.DepthBias &&
                a.DepthBiasClamp == b.DepthBiasClamp &&
                a.SlopeScaledDepthBias == b.SlopeScaledDepthBias &&
                a.DepthClipEnable == b.DepthClipEnable &&
                a.ScissorEnable == b.ScissorEnable &&
                a.MultisampleEnable == b.MultisampleEnable &&
                a.AntialiasedLineEnable == b.AntialiasedLineEnable;

        }

        public static bool operator !=(RasterizerDesc a, RasterizerDesc b)
        {
            return !(a == b);
        }
    }

    public class DXRasterizerState : DirectXObject
    {
        public RasterizerDesc Description { get; }
        public DXRasterizerState(IntPtr ptr, RasterizerDesc desc) : base(ptr)
        {
            Description = desc;
        }
    }


    public enum BlendFac
    {
        INVALID = 0,
        ZERO = 1,
        ONE = 2,
        SRC_COLOR = 3,
        INV_SRC_COLOR = 4,
        SRC_ALPHA = 5,
        INV_SRC_ALPHA = 6,
        DEST_ALPHA = 7,
        INV_DEST_ALPHA = 8,
        DEST_COLOR = 9,
        INV_DEST_COLOR = 10,
        SRC_ALPHA_SAT = 11,
        BLEND_FACTOR = 14,
        INV_BLEND_FACTOR = 15,
        SRC1_COLOR = 16,
        INV_SRC1_COLOR = 17,
        SRC1_ALPHA = 18,
        INV_SRC1_ALPHA = 19
    }

    public enum BlendOp
    {
        INVALID = 0,
        ADD = 1,
        SUBTRACT = 2,
        REV_SUBTRACT = 3,
        MIN = 4,
        MAX = 5
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct RenderTargetBlendDesc
    {
        [MarshalAs(UnmanagedType.Bool)] public bool BlendEnable = false;
        [MarshalAs(UnmanagedType.I4)] public BlendFac SrcBlend = BlendFac.ZERO;
        [MarshalAs(UnmanagedType.I4)] public BlendFac DestBlend = BlendFac.ZERO;
        [MarshalAs(UnmanagedType.I4)] public BlendOp BlendOp = BlendOp.ADD;
        [MarshalAs(UnmanagedType.I4)] public BlendFac SrcBlendAlpha = BlendFac.ZERO;
        [MarshalAs(UnmanagedType.I4)] public BlendFac DestBlendAlpha = BlendFac.ZERO;
        [MarshalAs(UnmanagedType.I4)] public BlendOp BlendOpAlpha = BlendOp.ADD;
        [MarshalAs(UnmanagedType.I1)] public byte RenderTargetWriteMask = 0;

        public RenderTargetBlendDesc()
        {

        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BlendDesc
    {
        [MarshalAs(UnmanagedType.Bool)] bool AlphaToCoverageEnable = false;
        [MarshalAs(UnmanagedType.Bool)] bool IndependentBlendEnable = false;

        public RenderTargetBlendDesc renderTargetBlendDesc0 = new RenderTargetBlendDesc();
        public RenderTargetBlendDesc renderTargetBlendDesc1 = new RenderTargetBlendDesc();
        public RenderTargetBlendDesc renderTargetBlendDesc2 = new RenderTargetBlendDesc();
        public RenderTargetBlendDesc renderTargetBlendDesc3 = new RenderTargetBlendDesc();
        public RenderTargetBlendDesc renderTargetBlendDesc4 = new RenderTargetBlendDesc();
        public RenderTargetBlendDesc renderTargetBlendDesc5 = new RenderTargetBlendDesc();
        public RenderTargetBlendDesc renderTargetBlendDesc6 = new RenderTargetBlendDesc();
        public RenderTargetBlendDesc renderTargetBlendDesc7 = new RenderTargetBlendDesc();

        public BlendDesc()
        {

        }
    }

    public class DXBlendState : DirectXObject
    {
        public DXBlendState(IntPtr ptr) : base(ptr)
        {
        }
    }

    public enum Filter
    {
        MIN_MAG_MIP_POINT = 0,
        MIN_MAG_POINT_MIP_LINEAR = 0x1,
        MIN_POINT_MAG_LINEAR_MIP_POINT = 0x4,
        MIN_POINT_MAG_MIP_LINEAR = 0x5,
        MIN_LINEAR_MAG_MIP_POINT = 0x10,
        MIN_LINEAR_MAG_POINT_MIP_LINEAR = 0x11,
        MIN_MAG_LINEAR_MIP_POINT = 0x14,
        MIN_MAG_MIP_LINEAR = 0x15,
        ANISOTROPIC = 0x55,
        COMPARISON_MIN_MAG_MIP_POINT = 0x80,
        COMPARISON_MIN_MAG_POINT_MIP_LINEAR = 0x81,
        COMPARISON_MIN_POINT_MAG_LINEAR_MIP_POINT = 0x84,
        COMPARISON_MIN_POINT_MAG_MIP_LINEAR = 0x85,
        COMPARISON_MIN_LINEAR_MAG_MIP_POINT = 0x90,
        COMPARISON_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 0x91,
        COMPARISON_MIN_MAG_LINEAR_MIP_POINT = 0x94,
        COMPARISON_MIN_MAG_MIP_LINEAR = 0x95,
        COMPARISON_ANISOTROPIC = 0xd5,
        MINIMUM_MIN_MAG_MIP_POINT = 0x100,
        MINIMUM_MIN_MAG_POINT_MIP_LINEAR = 0x101,
        MINIMUM_MIN_POINT_MAG_LINEAR_MIP_POINT = 0x104,
        MINIMUM_MIN_POINT_MAG_MIP_LINEAR = 0x105,
        MINIMUM_MIN_LINEAR_MAG_MIP_POINT = 0x110,
        MINIMUM_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 0x111,
        MINIMUM_MIN_MAG_LINEAR_MIP_POINT = 0x114,
        MINIMUM_MIN_MAG_MIP_LINEAR = 0x115,
        MINIMUM_ANISOTROPIC = 0x155,
        MAXIMUM_MIN_MAG_MIP_POINT = 0x180,
        MAXIMUM_MIN_MAG_POINT_MIP_LINEAR = 0x181,
        MAXIMUM_MIN_POINT_MAG_LINEAR_MIP_POINT = 0x184,
        MAXIMUM_MIN_POINT_MAG_MIP_LINEAR = 0x185,
        MAXIMUM_MIN_LINEAR_MAG_MIP_POINT = 0x190,
        MAXIMUM_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 0x191,
        MAXIMUM_MIN_MAG_LINEAR_MIP_POINT = 0x194,
        MAXIMUM_MIN_MAG_MIP_LINEAR = 0x195,
        MAXIMUM_ANISOTROPIC = 0x1d5
    }

    public enum TextureAddressMode
    {
        INVALID = 0,
        WRAP = 1,
        MIRROR = 2,
        CLAMP = 3,
        BORDER = 4,
        MIRROR_ONCE = 5
    }

    public enum SamplerComparisionFunc
    {
        NONE = 0,
        NEVER = 1,
        LESS = 2,
        EQUAL = 3,
        LESS_EQUAL = 4,
        GREATER = 5,
        NOT_EQUAL = 6,
        GREATER_EQUAL = 7,
        ALWAYS = 8
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SamplerDesc
    {
        [MarshalAs(UnmanagedType.I4)] public Filter Filter = Filter.MIN_MAG_MIP_POINT;
        [MarshalAs(UnmanagedType.I4)] public TextureAddressMode AddressU = TextureAddressMode.WRAP;
        [MarshalAs(UnmanagedType.I4)] public TextureAddressMode AddressV = TextureAddressMode.WRAP;
        [MarshalAs(UnmanagedType.I4)] public TextureAddressMode AddressW = TextureAddressMode.WRAP;
        [MarshalAs(UnmanagedType.R4)] public float MipLODBias = 0.0f;
        [MarshalAs(UnmanagedType.I1)] public byte MaxAnisotropy = 0;
        [MarshalAs(UnmanagedType.I4)] public SamplerComparisionFunc ComparisonFunc = SamplerComparisionFunc.NONE;
        [MarshalAs(UnmanagedType.R4)] public float BorderColor0 = 0.0f;
        [MarshalAs(UnmanagedType.R4)] public float BorderColor1 = 0.0f;
        [MarshalAs(UnmanagedType.R4)] public float BorderColor2 = 0.0f;
        [MarshalAs(UnmanagedType.R4)] public float BorderColor3 = 0.0f;
        [MarshalAs(UnmanagedType.R4)] public float MinLOD = 0.0f;
        [MarshalAs(UnmanagedType.R4)] public float MaxLOD = 0.0f;

        public SamplerDesc()
        {

        }
    }

    public class DXSamplerState : DirectXObject
    {
        public DXSamplerState(IntPtr ptr) : base(ptr)
        {
        }
    }

    public enum DXUsage
    {
        DEFAULT = 0,
        IMMUTABLE = 1,
        DYNAMIC = 2,
        STAGING = 3
    };

    [Flags]
    public enum DXBindFlag
    {
        INVALID = 0,
        VERTEX_BUFFER = 0x1,
        INDEX_BUFFER = 0x2,
        CONSTANT_BUFFER = 0x4,
        SHADER_RESOURCE = 0x8,
        STREAM_OUTPUT = 0x10,
        RENDER_TARGET = 0x20,
        DEPTH_STENCIL = 0x40,
        UNORDERED_ACCESS = 0x80,
        DECODER = 0x200,
        VIDEO_ENCODER = 0x400
    }

    [Flags]
    public enum DXCPUAccessFlag
    {
        NONE = 0,
        D3D11_CPU_ACCESS_WRITE = 0x10000,
        D3D11_CPU_ACCESS_READ = 0x20000
    }

    [Flags]
    public enum DXResourceMiscFlag
    {
        NONE = 0,
        GENERATE_MIPS = 0x1,
        SHARED = 0x2,
        TEXTURECUBE = 0x4,
        DRAWINDIRECT_ARGS = 0x10,
        BUFFER_ALLOW_RAW_VIEWS = 0x20,
        BUFFER_STRUCTURED = 0x40,
        RESOURCE_CLAMP = 0x80,
        SHARED_KEYEDMUTEX = 0x100,
        GDI_COMPATIBLE = 0x200,
        SHARED_NTHANDLE = 0x800,
        RESTRICTED_CONTENT = 0x1000,
        RESTRICT_SHARED_RESOURCE = 0x2000,
        RESTRICT_SHARED_RESOURCE_DRIVER = 0x4000,
        GUARDED = 0x8000,
        TILE_POOL = 0x20000,
        TILED = 0x40000,
        HW_PROTECTED = 0x80000,
        SHARED_DISPLAYABLE,
        SHARED_EXCLUSIVE_WRITER
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct BufferDesc
    {
        [MarshalAs(UnmanagedType.U4)] public uint ByteWidth = 0;
        [MarshalAs(UnmanagedType.I4)] public DXUsage Usage = DXUsage.DEFAULT;
        [MarshalAs(UnmanagedType.U4)] public DXBindFlag BindFlags = 0;
        [MarshalAs(UnmanagedType.U4)] public DXCPUAccessFlag CPUAccessFlags = 0;
        [MarshalAs(UnmanagedType.U4)] public DXResourceMiscFlag MiscFlags = 0;
        [MarshalAs(UnmanagedType.U4)] public uint StructureByteStride = 0;

        public BufferDesc()
        {

        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SubResourceData
    {
        public IntPtr pSysMem = IntPtr.Zero;
        [MarshalAs(UnmanagedType.U4)] public uint SysMemPitch = 0;
        [MarshalAs(UnmanagedType.U4)] public uint SysMemSlicePitch = 0;

        public SubResourceData()
        {

        }
    }

    public class DXBuffer : DirectXObject
    {
        public DXBuffer(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class DXBlob : DirectXObject
    {
        public DXBlob(IntPtr ptr) : base(ptr)
        {
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SampleDesc
    {
        [MarshalAs(UnmanagedType.U4)] public uint Count;
        [MarshalAs(UnmanagedType.U4)] public uint Quality;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXTexture2DDesc
    {
        [MarshalAs(UnmanagedType.U4)] public uint Width = 0;
        [MarshalAs(UnmanagedType.U4)] public uint Height = 0;
        [MarshalAs(UnmanagedType.U4)] public uint MipLevels = 0;
        [MarshalAs(UnmanagedType.U4)] public uint ArraySize = 0;
        [MarshalAs(UnmanagedType.I4)] public TextureFormat Format = 0;
        public SampleDesc SampleDesc = new SampleDesc();
        [MarshalAs(UnmanagedType.I4)] public DXUsage Usage = 0;
        [MarshalAs(UnmanagedType.U4)] public DXBindFlag BindFlags = 0;
        [MarshalAs(UnmanagedType.U4)] public DXCPUAccessFlag CPUAccessFlags = 0;
        [MarshalAs(UnmanagedType.U4)] public DXResourceMiscFlag MiscFlags = 0;

        public DXTexture2DDesc()
        {

        }
    }

    public class DXTexture2D : DirectXObject
    {
        public DXTexture2DDesc Desc { get; }
        public DXTexture2D(IntPtr ptr, DXTexture2DDesc desc) : base(ptr)
        {
            Desc = desc;
        }
    }

    public enum DXShaderResourceViewDimension
    {
        UNKNOWN = 0,
        BUFFER = 1,
        TEXTURE1D = 2,
        TEXTURE1DARRAY = 3,
        TEXTURE2D = 4,
        TEXTURE2DARRAY = 5,
        TEXTURE2DMS = 6,
        TEXTURE2DMSARRAY = 7,
        TEXTURE3D = 8,
        TEXTURECUBE = 9,
        TEXTURECUBEARRAY = 10,
        BUFFEREX = 11
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DXTexutre2DSRV
    {
        [MarshalAs(UnmanagedType.U4)] public uint MostDetailedMip;
        [MarshalAs(UnmanagedType.U4)] public uint MipLevels;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct DXTexutre2DArraySRV
    {
        [MarshalAs(UnmanagedType.U4)] public uint MostDetailedMip;
        [MarshalAs(UnmanagedType.U4)] public uint MipLevels;
        [MarshalAs(UnmanagedType.U4)] public uint FirstArraySlice;
        [MarshalAs(UnmanagedType.U4)] public uint ArraySize;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct DXShaderResourceViewDesc
    {
        [FieldOffset(0)] public TextureFormat Format;
        [FieldOffset(4)] public DXShaderResourceViewDimension ViewDimension;
        [FieldOffset(8)] public DXTexutre2DSRV Texture2D;
        [FieldOffset(8)] public DXTexutre2DArraySRV Texture2DArray;
    }

    public class DXShaderResourceView : DirectXObject
    {
        public DXShaderResourceViewDesc Desc { get; set; }
        public DXShaderResourceView(IntPtr ptr, DXShaderResourceViewDesc desc) : base(ptr)
        {
            Desc = desc;
        }
    }

    public class DXVertexShader : DirectXObject
    {
        public DXVertexShader(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class DXPixelShader : DirectXObject
    {
        public DXPixelShader(IntPtr ptr) : base(ptr)
        {
        }
    }  

    public enum InputClassification
    {
        PER_VERTEX_DATA = 0,
        PER_INSTANCE_DATA = 1
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InputElementDesc
    {
        [MarshalAs(UnmanagedType.LPStr)] public string SemanticName;
        [MarshalAs(UnmanagedType.U4)] public uint SemanticIndex;
        [MarshalAs(UnmanagedType.U4)] public TextureFormat Format;
        [MarshalAs(UnmanagedType.U4)] public uint InputSlot;
        [MarshalAs(UnmanagedType.U4)] public uint AlignedByteOffset;
        [MarshalAs(UnmanagedType.U4)] public InputClassification InputSlotClass;
        [MarshalAs(UnmanagedType.U4)] public uint InstanceDataStepRate;
    }

    public class DXInputLayout : DirectXObject
    {
        public DXInputLayout(IntPtr ptr) : base(ptr)
        {
        }
    }

    public static class VertexLayoutExtensions
    {
        public static uint GetStride(this VertexLayout layout)
        {
            switch (layout)
            {
                case VertexLayout.P: return 3;
                case VertexLayout.PNT: return (3 + 3 + 2);
            }

            return 0;
        }

        public static uint GetStrideBytes(this VertexLayout layout)
        {
            switch (layout)
            {
                case VertexLayout.P: return sizeof(float) * 3;
                case VertexLayout.PNT: return sizeof(float) * (3 + 3 + 2);
            }

            return 0;
        }
    }  

    public enum PrimitiveTopology
    {
        UNDEFINED = 0,
        POINTLIST = 1,
        LINELIST = 2,
        LINESTRIP = 3,
        TRIANGLELIST = 4,
        TRIANGLESTRIP = 5,
        LINELIST_ADJ = 10,
        LINESTRIP_ADJ = 11,
        TRIANGLELIST_ADJ = 12,
        TRIANGLESTRIP_ADJ = 13,
    }

    [Flags]
    public enum ClearFlag : uint
    {
        INVALID = 0,
        D3D11_CLEAR_DEPTH = 0x1,
        D3D11_CLEAR_STENCIL = 0x2
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DirectXBox
    {
        [MarshalAs(UnmanagedType.U4)] public uint left;
        [MarshalAs(UnmanagedType.U4)] public uint top;
        [MarshalAs(UnmanagedType.U4)] public uint front;
        [MarshalAs(UnmanagedType.U4)] public uint right;
        [MarshalAs(UnmanagedType.U4)] public uint bottom;
        [MarshalAs(UnmanagedType.U4)] public uint back;
    }

    public class DXDevice : DirectXObject
    {
        public DXDevice(IntPtr ptr) : base(ptr)
        {
        }

        public DXDepthStencilState CreateDepthStencilState(DepthStencilDesc desc)
           => new DXDepthStencilState(D3D11API.DeviceCreateDepthStencilState(ref desc), desc);

        public DXRasterizerState CreateRasterizerState(RasterizerDesc desc)
            => new DXRasterizerState(D3D11API.DeviceCreateRasterizerState(ref desc), desc);

        public DXBlendState CreateBlendState(BlendDesc desc)
            => new DXBlendState(D3D11API.DeviceCreateBlendState(ref desc));

        public DXSamplerState CreateSamplerState(SamplerDesc desc)
            => new DXSamplerState(D3D11API.DeviceCreateSamplerState(ref desc));

        public DXBuffer CreateBuffer(BufferDesc desc)
            => new DXBuffer(D3D11API.DeviceCreateBufferNoSub(ref desc));

        public DXBuffer CreateBuffer(BufferDesc desc, SubResourceData subResource) => 
            new DXBuffer(D3D11API.DeviceCreateBuffer(ref desc, ref subResource));

        public DXTexture2D CreateTexture2D(DXTexture2DDesc desc, SubResourceData resourceData) 
            => new DXTexture2D(D3D11API.DeviceCreateTexture2D(ref desc, ref resourceData), desc);

        public DXTexture2D CreateTexture2DNoSub(DXTexture2DDesc desc) 
            => new DXTexture2D(D3D11API.DeviceCreateTexture2DNoSub(ref desc), desc);

        public DXShaderResourceView CreateShaderResourceView(DXTexture2D resource, DXShaderResourceViewDesc desc) 
            => new DXShaderResourceView(D3D11API.DeviceCreateShaderResourceView(resource.Ptr, ref desc), desc );

        public DXBlob CompileShader(string shaderCode, string entry, string target)
            => new DXBlob(D3D11API.DeviceCompileShader(shaderCode, entry, target));

        public DXInputLayout CreateInputLayout(DXBlob vertexBlob, params InputElementDesc[] inputElementDescs)
            => new DXInputLayout(D3D11API.DeviceCreateInputLayout(inputElementDescs, inputElementDescs.Length, vertexBlob.Ptr));

        public DXVertexShader CreateVertexShader(DXBlob vertexBlob)
            => new DXVertexShader(D3D11API.DeviceCreateVertexShader(vertexBlob.Ptr));

        public DXPixelShader CreatePixelShader(DXBlob pixelBlob)
            => new DXPixelShader(D3D11API.DeviceCreatePixelShader(pixelBlob.Ptr));
    }

    public class DXContext : DirectXObject
    {
        public DXContext(IntPtr ptr) : base(ptr)
        {
        }

        public void ClearRenderTargetView(RenderTargetView renderTargetView, Vector4 colour)
        {
            D3D11API.ContextClearRenderTargetView(renderTargetView.Ptr, colour.x, colour.y, colour.z, colour.w);
        }

        public void ClearDepthStencilView(DepthStencilView depthStencilView, ClearFlag clearFlags, float depth, byte stencil)
        {
            D3D11API.ContextClearDepthStencilView(depthStencilView.Ptr, (uint)clearFlags, depth, stencil);
        }

        public void SetRenderTargets(DepthStencilView depthStencilView, params RenderTargetView[] renderTargetViews)
        {
            D3D11API.ContextSetRenderTargets(renderTargetViews.Select(x => x.Ptr).ToArray(), (uint)renderTargetViews.Length, depthStencilView.Ptr);
        }

        public void SetViewPortState(float width, float height, float minDepth = 0.0f, float maxDepth = 1.0f, float topLeftX = 0.0f, float topLeftY = 0.0f)
        {
            D3D11API.ContextSetViewPortState(width, height, minDepth, maxDepth, topLeftX, topLeftY);
        }

        public void SetPrimitiveTopology(PrimitiveTopology primitiveTopology)
        {
            D3D11API.ContextSetPrimitiveTopology(primitiveTopology);
        }

        public void SetDepthStencilState(DXDepthStencilState depthStencilState, uint stencilRef = 1)
        {
            D3D11API.ContextSetDepthStencilState(depthStencilState.Ptr, stencilRef);
        }

        public void SetRasterizerState(DXRasterizerState rasterizerState)
        {
            D3D11API.ContextSetRasterizerState(rasterizerState.Ptr);
        }

        public void SetBlendState(DXBlendState blendState)
        {
            D3D11API.ContextSetBlendState(blendState.Ptr, 0, 0, 0, 0, 0xffffffff);
        }

        public void SetInputLayout(DXInputLayout inputLayout)
        {
            D3D11API.ContextSetInputLayout(inputLayout.Ptr);
        }

        public void SetVertexShader(DXVertexShader vertexShader)
        {
            D3D11API.ContextSetVertexShader(vertexShader.Ptr);
        }

        public void SetPixelShader(DXPixelShader pixelShader)
        {
            D3D11API.ContextSetPixelShader(pixelShader.Ptr);
        }

        public void SetVertexBuffers(DXBuffer vertexBuffer, uint stride)
        {
            D3D11API.ContextSetVertexBuffers(vertexBuffer.Ptr, stride);
        }

        public void SetIndexBuffer(DXBuffer indexBuffer, TextureFormat format, uint offset)
        {
            D3D11API.ContextSetIndexBuffer(indexBuffer.Ptr, format, offset);
        }

        public void DrawIndexed(uint indexCount, uint startLocation, uint baseVertexLocation)
        {
            D3D11API.ContextDrawIndexed(indexCount, startLocation, baseVertexLocation);
        }

        public void SetVSConstBuffer(DXBuffer constBuffer, uint slot) => D3D11API.ContextSetVSConstBuffer(constBuffer.Ptr, slot);
        public void SetPSConstBuffer(DXBuffer constBuffer, uint slot) => D3D11API.ContextSetPSConstBuffer(constBuffer.Ptr, slot);
        public void SetCSConstBuffer(DXBuffer constBuffer, uint slot) => D3D11API.ContextSetCSConstBuffer(constBuffer.Ptr, slot);

        public void UpdateSubresource(DXBuffer buffer, float[] data)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            D3D11API.ContextUpdateSubresource(buffer.Ptr, 0, IntPtr.Zero, handle.AddrOfPinnedObject(), 0, 0);
            handle.Free();
        }

        public void SetPSSampler(DXSamplerState sampler, uint slot) => D3D11API.ContextSetPSSampler(sampler.Ptr, slot);
        public void SetCSSampler(DXSamplerState sampler, uint slot) => D3D11API.ContextSetCSSampler(sampler.Ptr, slot);
        public void SetPSShaderResources(DXShaderResourceView srv, uint slot) => D3D11API.ContextSetPSShaderResources(srv.Ptr, slot);
        public void SetCSShaderResources(DXShaderResourceView srv, uint slot) => D3D11API.ContextSetCSShaderResources(srv.Ptr, slot);

        //public void SetPSShaderResources(DXShader srv, uint32 slot);
        //public void SetCSShaderResources(void* srv, uint32 slot);

    }

    public class RenderTargetView : DirectXObject
    {
        public RenderTargetView(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class DepthStencilView : DirectXObject
    {
        public DepthStencilView(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class DeviceContext 
    {
        public DXDevice Device { get; set; }
        public DXContext Context { get; set; }

        private bool created = false;
        public bool Create()
        {
            IntPtr devicePtr = new IntPtr();
            IntPtr contextPtr = new IntPtr();
            created = D3D11API.CreateDeviceAndContext(ref devicePtr, ref contextPtr);

            if (created)
            {
                Device = new DXDevice(devicePtr);
                Context = new DXContext(contextPtr);
            }

            return created;
        }
    }

    public class DXSwapchain
    {
        public RenderTargetView renderTargetView;
        public DepthStencilView depthStencilView;

        public IntPtr Ptr { get => ptr; }
        private IntPtr ptr;

        public bool Create()
        {
            ptr = D3D11API.CreateSwapchain();

            if (ptr != IntPtr.Zero)
            {
                renderTargetView = new RenderTargetView(D3D11API.CreateSwapchainRenderTargetView());                
                depthStencilView = new DepthStencilView(D3D11API.CreateSwapchainDepthStencilView());
                return true;
            }

            return false; 
        }

        public void Present(bool vsync)
        {
            D3D11API.SwapchainPresent(vsync ? 1 : 0);
        }
    }

}
