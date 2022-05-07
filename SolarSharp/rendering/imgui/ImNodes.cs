using SolarSharp.EngineAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering
{
    public enum ImNodesPinShape
    {
        Circle,
        CircleFilled,
        Triangle,
        TriangleFilled,
        Quad,
        QuadFilled
    };

    public enum ImNodesCol
    {
        NodeBackground = 0,
        NodeBackgroundHovered,
        NodeBackgroundSelected,
        NodeOutline,
        TitleBar,
        TitleBarHovered,
        TitleBarSelected,
        Link,
        LinkHovered,
        LinkSelected,
        Pin,
        PinHovered,
        BoxSelector,
        BoxSelectorOutline,
        GridBackground,
        GridLine,
        GridLinePrimary,
        MiniMapBackground,
        MiniMapBackgroundHovered,
        MiniMapOutline,
        MiniMapOutlineHovered,
        MiniMapNodeBackground,
        MiniMapNodeBackgroundHovered,
        MiniMapNodeBackgroundSelected,
        MiniMapNodeOutline,
        MiniMapLink,
        MiniMapLinkSelected,
        MiniMapCanvas,
        MiniMapCanvasOutline,
        COUNT
    };

    public class ImNodes
    {
		public static void Initialzie() => ImNodesAPI.ImNodesInitialzie();

		public static void Shutdown() => ImNodesAPI.ImNodesShutdown();

		public static void BeginNodeEditor() => ImNodesAPI.ImNodesBeginNodeEditor();

		public static void EndNodeEditor() => ImNodesAPI.ImNodesEndNodeEditor();

		public static void BeginNode(int id) => ImNodesAPI.ImNodesBeginNode(id);

		public static void EndNode() => ImNodesAPI.ImNodesEndNode();

		public static void BeginNodeTitleBar() => ImNodesAPI.ImNodesBeginNodeTitleBar();

		public static void EndNodeTitleBar() => ImNodesAPI.ImNodesEndNodeTitleBar();

		public static void BeginInputAttribute(int id, ImNodesPinShape shape) => ImNodesAPI.ImNodesBeginInputAttribute(id, (int)shape);

		public static void EndInputAttribute() => ImNodesAPI.ImNodesEndInputAttribute();

		public static void BeginOutputAttribute(int id, ImNodesPinShape shape) => ImNodesAPI.ImNodesBeginOutputAttribute(id, (int)shape);

		public static void EndOutputAttribute() => ImNodesAPI.ImNodesEndOutputAttribute();

		public static void Link(int id, int startAttrId, int endAttrId) => ImNodesAPI.ImNodesLink(id, startAttrId, endAttrId);

		public static bool IsLinkCreated(ref int startedAtPinId, ref int endedAtPinId, ref int createdFromSnap) => ImNodesAPI.ImNodesIsLinkCreated(ref startedAtPinId, ref endedAtPinId, ref createdFromSnap);
        
        public static bool IsLinkDropped(ref int startedAtPinId, bool includingDetachedLinks) => ImNodesAPI.ImNodesIsLinkDropped(ref startedAtPinId, includingDetachedLinks);

        public static void PushColorStyle(ImNodesCol item, uint col) => ImNodesAPI.ImNodesPushColorStyle((int)item, col);

		public static void PopColorStyle() => ImNodesAPI.ImNodesPopColorStyle();

        public static bool IsEditorHovered() => ImNodesAPI.ImNodesIsEditorHovered();

        public static void SetNodeScreenSpacePos(int id, float x, float y) => ImNodesAPI.ImNodesSetNodeScreenSpacePos((int)id, x, y);

        public static void SetNodeEditorSpacePos(int id, float x, float y) => ImNodesAPI.ImNodesSetNodeEditorSpacePos((int)id, x, y);

        public static Vector2 GetNodeEditorSpacePos(int id) {
            Vector2 v = new Vector2();
            ImNodesAPI.ImNodesGetNodeEditorSpacePos(id, ref v.x, ref v.y);
            return v;
        }
    }
}
