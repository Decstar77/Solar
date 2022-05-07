using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolarSharp.EngineAPI;

namespace SolarSharp.Rendering
{
    public class ImGuiTextEditor
    {
        private static byte[] buffer = new byte[480000];

        public static void Initialize() => ImGuiTextAPI.ImGuiTextInitialize();
        public static void Render(string title) => ImGuiTextAPI.ImGuiTextRender(title);
        public static void SetText(string text) => ImGuiTextAPI.ImGuiTextSetText(text);
        public static string GetText()
        {
            ImGuiTextAPI.ImGuiTextGetText(buffer, buffer.Length);
            return Util.AsciiBytesToString(buffer, 0);
        }

        public static bool IsShowingWhitespaces() => ImGuiTextAPI.ImGuiTextIsShowingWhitespaces();
        public static void SetShowWhitespaces(bool v) => ImGuiTextAPI.ImGuiTextSetShowWhitespaces(v);
        public static bool IsReadOnly() => ImGuiTextAPI.ImGuiTextIsReadOnly();
        public static void SetReadOnly(bool v) => ImGuiTextAPI.ImGuiTextSetReadOnly(v);
    }
}
