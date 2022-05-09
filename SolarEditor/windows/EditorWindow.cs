using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSharp;

using SolarSharp.Rendering;
using SolarSharp.Assets;
using SolarSharp.Rendering.Graph;

namespace SolarEditor
{
    internal abstract class EditorWindow
    {
        protected bool show = true;
        public abstract void Show(EditorState editorState);
        public abstract void Start();
        public abstract void Shutdown();

        public bool ShouldClose() => !show;
        public void Close() => show = false;
    }
}
