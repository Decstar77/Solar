using SolarSharp.Assets;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEditor
{
    internal class TextureImporter
    {               
        public static TextureAsset? LoadFromFile(string path, MetaFileAsset metaFileAsset)
        {            
            using (var stream = File.OpenRead(path))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                
                TextureAsset texture = new TextureAsset();
                texture.name = Path.GetFileNameWithoutExtension(path);
                texture.path = path;
                texture.pixels = image.Data;
                texture.format = TextureFormat.R8G8B8A8_UNORM;
                texture.width = image.Width;
                texture.height = image.Height;
                
                texture.Guid = metaFileAsset.Guid;                

                return texture;
            }

            return null;
        }
    }
}
