using SolarSharp.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering
{
    public class StaticTexture
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public DXTexture2D texture2D { get; set; }
        public DXShaderResourceView srv { get; set; }

        public StaticTexture(DXDevice device, TextureAsset textureAsset)
        {
            DXTexture2DDesc texture2DDesc = new DXTexture2DDesc
            {
                Width = (uint)textureAsset.width,
                Height = (uint)textureAsset.height,
                MipLevels = 1,
                ArraySize = 1,
                Format = textureAsset.format,
                SampleDesc = new SampleDesc { Count = 1, Quality = 0},
                Usage = DXUsage.DEFAULT,
                BindFlags = DXBindFlag.SHADER_RESOURCE,
                CPUAccessFlags = DXCPUAccessFlag.NONE,
                MiscFlags = DXResourceMiscFlag.NONE
            };

            GCHandle handle = GCHandle.Alloc(textureAsset.pixels, GCHandleType.Pinned);

            SubResourceData data = new SubResourceData
            {
                pSysMem = handle.AddrOfPinnedObject(),
                SysMemPitch = (uint)(textureAsset.format.GetPitchBytes() * textureAsset.width),
            };

            texture2D = device.CreateTexture2D(texture2DDesc, data);
            Width = textureAsset.width;
            Height = textureAsset.height;

            handle.Free();

            srv = device.CreateShaderResourceView(texture2D, new DXShaderResourceViewDesc { 
                Format = textureAsset.format,
                ViewDimension = DXShaderResourceViewDimension.TEXTURE2D,
                Texture2D = new DXTexutre2DSRV { MipLevels = 1, MostDetailedMip = 0 }
            });
        }

    }
}
