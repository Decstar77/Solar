using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Assets
{
    public class ShaderAsset : EngineAsset
    {
        private static string VERTEX_BEGIN_TOKEN = "//VertexShaderBegin//";
        private static string VERTEX_END_TOKEN = "//VertexShaderEnd//";

        private static string FRAGMENT_BEGIN_TOKEN = "//FragmentShaderBegin//";
        private static string FRAGMENT_END_TOKEN = "//FragmentShaderEnd//";

        public string name { get; set; }
        public string Path { get; set; }
        public string Src { get; set; }
             
        public string GetVertexSrc()
        {
            int start = Src.IndexOf(VERTEX_BEGIN_TOKEN);
            int end = Src.IndexOf(VERTEX_END_TOKEN);
            return Src.Substring(start, end - start);
        }

        public string GetFragmentSrc()
        {
            int start = Src.IndexOf(FRAGMENT_BEGIN_TOKEN);
            int end = Src.IndexOf(FRAGMENT_END_TOKEN);
            return Src.Substring(start, end - start);
        }
    }
}
