using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using SolarSharp.EngineAPI;
using System.Diagnostics.CodeAnalysis;

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
        [MarshalAs(UnmanagedType.Bool)] public bool DepthTest = false;
        [MarshalAs(UnmanagedType.Bool)] public bool DepthWrite = false;
        [MarshalAs(UnmanagedType.I4)] public DepthComparisonFunc DepthFunc = DepthComparisonFunc.NEVER;
        [MarshalAs(UnmanagedType.Bool)] public bool StencilEnable = false;
        [MarshalAs(UnmanagedType.I1)] public byte StencilReadMask = 0;
        [MarshalAs(UnmanagedType.I1)] public byte StencilWriteMask = 0;
        //D3D11_DEPTH_STENCILOP_DESC FrontFace;
        //D3D11_DEPTH_STENCILOP_DESC BackFace;
        public DepthStencilDesc()
        {

        }

        public static bool operator ==(DepthStencilDesc a, DepthStencilDesc b)
        {
            return a.DepthTest == b.DepthTest &&
                a.DepthWrite == b.DepthWrite &&
                a.DepthFunc == b.DepthFunc &&
                a.StencilEnable == b.StencilEnable &&
                a.StencilReadMask == b.StencilReadMask &&
                a.StencilWriteMask == b.StencilWriteMask;
        }

        public static bool operator !=(DepthStencilDesc a, DepthStencilDesc b)
        {
            return !(a == b);
        }
    }

    public class DepthStencilState : DirectXObject
    {
        public DepthStencilDesc Description { get; }
        public DepthStencilState(IntPtr ptr, DepthStencilDesc desc) : base(ptr)
        {
            Description = desc;
        }
    }

    public enum RasterizerFillMode
    {
        WIREFRAME = 2,
        SOLID = 3
    }

    public enum RasterizerCullMode
    {
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

    public class RasterizerState : DirectXObject
    {
        public RasterizerDesc Description { get; }
        public RasterizerState(IntPtr ptr, RasterizerDesc desc) : base(ptr)
        {
            Description = desc;
        }
    }


    public enum BlendFac
    {
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

    public class BlendState : DirectXObject
    {
        public BlendState(IntPtr ptr) : base(ptr)
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

    public class SamplerState : DirectXObject
    {
        public SamplerState(IntPtr ptr) : base(ptr)
        {
        }
    }

    public enum BufferUsage
    {
        DEFAULT = 0,
        IMMUTABLE = 1,
        DYNAMIC = 2,
        STAGING = 3
    };

    [Flags]
    public enum BufferBindFlag
    {
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
    public enum CPUAccessFlag
    {
        D3D11_CPU_ACCESS_WRITE = 0x10000,
        D3D11_CPU_ACCESS_READ = 0x20000
    }

    [Flags]
    public enum ResourceMiscFlag
    {
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
        [MarshalAs(UnmanagedType.I4)] public BufferUsage Usage = BufferUsage.DEFAULT;
        [MarshalAs(UnmanagedType.U4)] public uint BindFlags = 0;
        [MarshalAs(UnmanagedType.U4)] public uint CPUAccessFlags = 0;
        [MarshalAs(UnmanagedType.U4)] public uint MiscFlags = 0;
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

    public class DirectXBuffer : DirectXObject
    {
        public DirectXBuffer(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class Blob : DirectXObject
    {
        public Blob(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class VertexShader : DirectXObject
    {
        public VertexShader(IntPtr ptr) : base(ptr)
        {
        }
    }

    public class PixelShader : DirectXObject
    {
        public PixelShader(IntPtr ptr) : base(ptr)
        {
        }
    }

    public enum DXGIFormat : uint
    {
        UNKNOWN = 0,
        R32G32B32A32_TYPELESS = 1,
        R32G32B32A32_FLOAT = 2,
        R32G32B32A32_UINT = 3,
        R32G32B32A32_SINT = 4,
        R32G32B32_TYPELESS = 5,
        R32G32B32_FLOAT = 6,
        R32G32B32_UINT = 7,
        R32G32B32_SINT = 8,
        R16G16B16A16_TYPELESS = 9,
        R16G16B16A16_FLOAT = 10,
        R16G16B16A16_UNORM = 11,
        R16G16B16A16_UINT = 12,
        R16G16B16A16_SNORM = 13,
        R16G16B16A16_SINT = 14,
        R32G32_TYPELESS = 15,
        R32G32_FLOAT = 16,
        R32G32_UINT = 17,
        R32G32_SINT = 18,
        R32G8X24_TYPELESS = 19,
        D32_FLOAT_S8X24_UINT = 20,
        R32_FLOAT_X8X24_TYPELESS = 21,
        X32_TYPELESS_G8X24_UINT = 22,
        R10G10B10A2_TYPELESS = 23,
        R10G10B10A2_UNORM = 24,
        R10G10B10A2_UINT = 25,
        R11G11B10_FLOAT = 26,
        R8G8B8A8_TYPELESS = 27,
        R8G8B8A8_UNORM = 28,
        R8G8B8A8_UNORM_SRGB = 29,
        R8G8B8A8_UINT = 30,
        R8G8B8A8_SNORM = 31,
        R8G8B8A8_SINT = 32,
        R16G16_TYPELESS = 33,
        R16G16_FLOAT = 34,
        R16G16_UNORM = 35,
        R16G16_UINT = 36,
        R16G16_SNORM = 37,
        R16G16_SINT = 38,
        R32_TYPELESS = 39,
        D32_FLOAT = 40,
        R32_FLOAT = 41,
        R32_UINT = 42,
        R32_SINT = 43,
        R24G8_TYPELESS = 44,
        D24_UNORM_S8_UINT = 45,
        R24_UNORM_X8_TYPELESS = 46,
        X24_TYPELESS_G8_UINT = 47,
        R8G8_TYPELESS = 48,
        R8G8_UNORM = 49,
        R8G8_UINT = 50,
        R8G8_SNORM = 51,
        R8G8_SINT = 52,
        R16_TYPELESS = 53,
        R16_FLOAT = 54,
        D16_UNORM = 55,
        R16_UNORM = 56,
        R16_UINT = 57,
        R16_SNORM = 58,
        R16_SINT = 59,
        R8_TYPELESS = 60,
        R8_UNORM = 61,
        R8_UINT = 62,
        R8_SNORM = 63,
        R8_SINT = 64,
        A8_UNORM = 65,
        R1_UNORM = 66,
        R9G9B9E5_SHAREDEXP = 67,
        R8G8_B8G8_UNORM = 68,
        G8R8_G8B8_UNORM = 69,
        BC1_TYPELESS = 70,
        BC1_UNORM = 71,
        BC1_UNORM_SRGB = 72,
        BC2_TYPELESS = 73,
        BC2_UNORM = 74,
        BC2_UNORM_SRGB = 75,
        BC3_TYPELESS = 76,
        BC3_UNORM = 77,
        BC3_UNORM_SRGB = 78,
        BC4_TYPELESS = 79,
        BC4_UNORM = 80,
        BC4_SNORM = 81,
        BC5_TYPELESS = 82,
        BC5_UNORM = 83,
        BC5_SNORM = 84,
        B5G6R5_UNORM = 85,
        B5G5R5A1_UNORM = 86,
        B8G8R8A8_UNORM = 87,
        B8G8R8X8_UNORM = 88,
        R10G10B10_XR_BIAS_A2_UNORM = 89,
        B8G8R8A8_TYPELESS = 90,
        B8G8R8A8_UNORM_SRGB = 91,
        B8G8R8X8_TYPELESS = 92,
        B8G8R8X8_UNORM_SRGB = 93,
        BC6H_TYPELESS = 94,
        BC6H_UF16 = 95,
        BC6H_SF16 = 96,
        BC7_TYPELESS = 97,
        BC7_UNORM = 98,
        BC7_UNORM_SRGB = 99,
        AYUV = 100,
        Y410 = 101,
        Y416 = 102,
        NV12 = 103,
        P010 = 104,
        P016 = 105,
        _420_OPAQUE = 106,
        YUY2 = 107,
        Y210 = 108,
        Y216 = 109,
        NV11 = 110,
        AI44 = 111,
        IA44 = 112,
        P8 = 113,
        A8P8 = 114,
        B4G4R4A4_UNORM = 115,
        P208 = 130,
        V208 = 131,
        V408 = 132,
        SAMPLER_FEEDBACK_MIN_MIP_OPAQUE,
        SAMPLER_FEEDBACK_MIP_REGION_USED_OPAQUE,
        FORCE_UINT = 0xffffffff
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
        [MarshalAs(UnmanagedType.U4)] public DXGIFormat Format;
        [MarshalAs(UnmanagedType.U4)] public uint InputSlot;
        [MarshalAs(UnmanagedType.U4)] public uint AlignedByteOffset;
        [MarshalAs(UnmanagedType.U4)] public InputClassification InputSlotClass;
        [MarshalAs(UnmanagedType.U4)] public uint InstanceDataStepRate;
    }

    public class InputLayout : DirectXObject
    {
        public InputLayout(IntPtr ptr) : base(ptr)
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
    public class StaticMesh
    {
        public uint StrideBytes { get; set; }
        public uint IndexCount { get; set; }
        public uint VertexCount { get; set; }
        public DirectXBuffer VertexBuffer { get; set; }
        public DirectXBuffer IndexBuffer { get; set; }

        public StaticMesh(Device device, float[] vertices, uint[] indices, VertexLayout layout)
        {
            uint vertexStrideBytes = layout.GetStrideBytes();
            uint indicesStrideBytes = sizeof(uint);

            BufferDesc vertexDesc = new BufferDesc();
            vertexDesc.BindFlags = (uint)(BufferBindFlag.VERTEX_BUFFER);
            vertexDesc.Usage = BufferUsage.DEFAULT;
            vertexDesc.CPUAccessFlags = 0;
            vertexDesc.MiscFlags = 0;
            vertexDesc.ByteWidth = (uint)(vertices.Length * sizeof(float));
            vertexDesc.StructureByteStride = vertexStrideBytes;

            SubResourceData vertexSubResourceData = new SubResourceData();
            GCHandle vertexHandle = GCHandle.Alloc(vertices, GCHandleType.Pinned);
            vertexSubResourceData.pSysMem = vertexHandle.AddrOfPinnedObject();

            BufferDesc indexDesc = new BufferDesc();
            indexDesc.BindFlags = (uint)(BufferBindFlag.INDEX_BUFFER);
            indexDesc.Usage = BufferUsage.DEFAULT;
            indexDesc.CPUAccessFlags = 0;
            indexDesc.MiscFlags = 0;
            indexDesc.ByteWidth = (uint)(indices.Length * sizeof(float));
            indexDesc.StructureByteStride = indicesStrideBytes;

            SubResourceData indexSubResourceData = new SubResourceData();
            GCHandle indexHandle = GCHandle.Alloc(indices, GCHandleType.Pinned);
            indexSubResourceData.pSysMem = indexHandle.AddrOfPinnedObject();

            VertexBuffer = device.CreateBuffer(vertexDesc, vertexSubResourceData);
            IndexBuffer = device.CreateBuffer(indexDesc, indexSubResourceData);
            StrideBytes = vertexStrideBytes;
            VertexCount = (uint)vertices.Length / layout.GetStride();
            IndexCount = (uint)indices.Length;

            vertexHandle.Free();
            indexHandle.Free();
        }

        public static StaticMesh CreateScreenSpaceQuad(Device device)
        {
            float[] vertexData = {
            -1, 1, 0,   0, 0, -1,   0, 0,
            1, -1, 0,   0, 0, -1,   1, 1,
            -1, -1, 0,  0, 0, -1,   0, 1,
            1, 1, 0,    0, 0, -1,   1, 0
            };

            uint[] indexData = {
            0, 1, 2, 0, 3, 1
            };

            return new StaticMesh(device, vertexData, indexData, VertexLayout.PNT);
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

    public class Device : DirectXObject
    {
        public Device(IntPtr ptr) : base(ptr)
        {
        }

        public DepthStencilState CreateDepthStencilState(DepthStencilDesc desc)
        {
           return new DepthStencilState(D3D11API.DeviceCreateDepthStencilState(ref desc), desc);
        }

        public RasterizerState CreateRasterizerState(RasterizerDesc desc)
        {
            return new RasterizerState(D3D11API.DeviceCreateRasterizerState(ref desc), desc);
        }

        public BlendState CreateBlendState(BlendDesc desc)
        {
            return new BlendState(D3D11API.DeviceCreateBlendState(ref desc));
        }

        public SamplerState CreateSamplerState(SamplerDesc desc)
        {
            return new SamplerState(D3D11API.DeviceCreateSamplerState(ref desc));
        }

        public DirectXBuffer CreateBuffer(BufferDesc desc)
        {
            return new DirectXBuffer(D3D11API.DeviceCreateBufferNoSub(ref desc));
        }

        public DirectXBuffer CreateBuffer(BufferDesc desc, SubResourceData subResource)
        {
            return new DirectXBuffer(D3D11API.DeviceCreateBuffer(ref desc, ref subResource));
        }

        public Blob CompileShader(string shaderCode, string entry, string target)
        {
            return new Blob(D3D11API.DeviceCompileShader(shaderCode, entry, target));
        }

        public InputLayout CreateInputLayout(Blob vertexBlob, params InputElementDesc[] inputElementDescs)
        {
            return new InputLayout(D3D11API.DeviceCreateInputLayout(inputElementDescs, inputElementDescs.Length, vertexBlob.Ptr));
        }

        public VertexShader CreateVertexShader(Blob vertexBlob)
        {
            return new VertexShader(D3D11API.DeviceCreateVertexShader(vertexBlob.Ptr));
        }

        public PixelShader CreatePixelShader(Blob pixelBlob)
        {
            return new PixelShader(D3D11API.DeviceCreatePixelShader(pixelBlob.Ptr));
        }
    }

    public class Context : DirectXObject
    {
        public Context(IntPtr ptr) : base(ptr)
        {
        }

        public void ClearRenderTargetView(RenderTargetView renderTargetView)
        {
           D3D11API.ContextClearRenderTargetView(renderTargetView.Ptr, 0.2f, 0.2f, 0.2f, 1.0f);
        }

        public void ClearDepthStencilView(DepthStencilView depthStencilView, uint clearFlags, float depth, byte stencil)
        {
            D3D11API.ContextClearDepthStencilView(depthStencilView.Ptr, clearFlags, depth, stencil);
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

        public void SetDepthStencilState(DepthStencilState depthStencilState, uint stencilRef = 1)
        {
            D3D11API.ContextSetDepthStencilState(depthStencilState.Ptr, stencilRef);
        }

        public void SetRasterizerState(RasterizerState rasterizerState)
        {
            D3D11API.ContextSetRasterizerState(rasterizerState.Ptr);
        }

        public void SetBlendState(BlendState blendState)
        {
            D3D11API.ContextSetBlendState(blendState.Ptr, 0, 0, 0, 0, 0xffffffff);
        }

        public void SetInputLayout(InputLayout inputLayout)
        {
            D3D11API.ContextSetInputLayout(inputLayout.Ptr);
        }

        public void SetVertexShader(VertexShader vertexShader)
        {
            D3D11API.ContextSetVertexShader(vertexShader.Ptr);
        }

        public void SetPixelShader(PixelShader pixelShader)
        {
            D3D11API.ContextSetPixelShader(pixelShader.Ptr);
        }

        public void SetVertexBuffers(DirectXBuffer vertexBuffer, uint stride)
        {
            D3D11API.ContextSetVertexBuffers(vertexBuffer.Ptr, stride);
        }

        public void SetIndexBuffer(DirectXBuffer indexBuffer, DXGIFormat format, uint offset)
        {
            D3D11API.ContextSetIndexBuffer(indexBuffer.Ptr, format, offset);
        }

        public void DrawIndexed(uint indexCount, uint startLocation, uint baseVertexLocation)
        {
            D3D11API.ContextDrawIndexed(indexCount, startLocation, baseVertexLocation);
        }

        public void SetVSConstBuffer(DirectXBuffer constBuffer, uint slot) => D3D11API.ContextSetVSConstBuffer(constBuffer.Ptr, slot);
        public void SetPSConstBuffer(DirectXBuffer constBuffer, uint slot) => D3D11API.ContextSetPSConstBuffer(constBuffer.Ptr, slot);
        public void SetCSConstBuffer(DirectXBuffer constBuffer, uint slot) => D3D11API.ContextSetCSConstBuffer(constBuffer.Ptr, slot);

        public void UpdateSubresource(DirectXBuffer buffer, float[] data)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            D3D11API.ContextUpdateSubresource(buffer.Ptr, 0, IntPtr.Zero, handle.AddrOfPinnedObject(), 0, 0);
            handle.Free();
        }
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
        public Device Device { get; set; }
        public Context Context { get; set; }

        private bool created = false;
        public bool Create()
        {
            IntPtr devicePtr = new IntPtr();
            IntPtr contextPtr = new IntPtr();
            created = D3D11API.CreateDeviceAndContext(ref devicePtr, ref contextPtr);

            if (created)
            {
                Device = new Device(devicePtr);
                Context = new Context(contextPtr);
            }

            return created;
        }
    }

    public class Swapchain
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
