using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolarSharp.EngineAPI;

namespace SolarSharp.Rendering
{

	[Flags]
	public enum ImGuiWindowFlags
	{
		None = 0,
		NoTitleBar = 1 << 0,                    // Disable title-bar
		NoResize = 1 << 1,                      // Disable user resizing with the lower-right grip
		NoMove = 1 << 2,                        // Disable user moving the window
		NoScrollbar = 1 << 3,                   // Disable scrollbars (window can still scroll with mouse or programmatically)
		NoScrollWithMouse = 1 << 4,             // Disable user vertically scrolling with mouse wheel. On child window, mouse wheel will be forwarded to the parent unless NoScrollbar is also set.
		NoCollapse = 1 << 5,                    // Disable user collapsing window by double-clicking on it
		AlwaysAutoResize = 1 << 6,              // Resize every window to its content every frame
		NoBackground = 1 << 7,                  // Disable drawing background color (WindowBg, etc.) and outside border. Similar as using SetNextWindowBgAlpha(0.0f).
		NoSavedSettings = 1 << 8,               // Never load/save settings in .ini file
		NoMouseInputs = 1 << 9,                 // Disable catching mouse, hovering test with pass through.
		MenuBar = 1 << 10,                      // Has a menu-bar
		HorizontalScrollbar = 1 << 11,          // Allow horizontal scrollbar to appear (off by default). You may use SetNextWindowContentSize(ImVec2(width,0.0f)); prior to calling Begin() to specify width. Read code in imgui_demo in the "Horizontal Scrolling" section.
		NoFocusOnAppearing = 1 << 12,           // Disable taking focus when transitioning from hidden to visible state
		NoBringToFrontOnFocus = 1 << 13,        // Disable bringing window to front when taking focus (e.g. clicking on it or programmatically giving it focus)
		AlwaysVerticalScrollbar = 1 << 14,      // Always show vertical scrollbar (even if ContentSize.y < Size.y)
		AlwaysHorizontalScrollbar = 1 << 15,    // Always show horizontal scrollbar (even if ContentSize.x < Size.x)
		AlwaysUseWindowPadding = 1 << 16,       // Ensure child windows without border uses style.WindowPadding (ignored by default for non-bordered child windows, because more convenient)
		NoNavInputs = 1 << 18,                  // No gamepad/keyboard navigation within the window
		NoNavFocus = 1 << 19,                   // No focusing toward this window with gamepad/keyboard navigation (e.g. skipped by CTRL+TAB)
		UnsavedDocument = 1 << 20,              // Append '*' to title without affecting the ID, as a convenience to avoid using the ### operator. When used in a tab/docking context, tab is selected on closure and closure is deferred by one frame to allow code to cancel the closure (with a confirmation popup, etc.) without flicker.
		NoNav = NoNavInputs | NoNavFocus,
		NoDecoration = NoTitleBar | NoResize | NoScrollbar | NoCollapse,
		NoInputs = NoMouseInputs | NoNavInputs | NoNavFocus,
	}

	[Flags]
	public enum ImGuiTabBarFlags
	{
		None = 0,
		Reorderable = 1 << 0,   // Allow manually dragging tabs to re-order them + New tabs are appended at the end of list
		AutoSelectNewTabs = 1 << 1,   // Automatically select new tabs when they appear
		TabListPopupButton = 1 << 2,   // Disable buttons to open the tab list popup
		NoCloseWithMiddleMouseButton = 1 << 3,   // Disable behavior of closing tabs (that are submitted with p_open != NULL) with middle mouse button. You can still repro this behavior on user's side with if (IsItemHovered() && IsMouseClicked(2)) *p_open = false.
		NoTabListScrollingButtons = 1 << 4,   // Disable scrolling buttons (apply when fitting policy is ImGuiTabBarFlags_FittingPolicyScroll)
		NoTooltip = 1 << 5,   // Disable tooltips when hovering a tab
		FittingPolicyResizeDown = 1 << 6,   // Resize tabs when they don't fit
		FittingPolicyScroll = 1 << 7,   // Add scroll buttons when tabs don't fit
		FittingPolicyMask_ = FittingPolicyResizeDown | FittingPolicyScroll,
		FittingPolicyDefault_ = FittingPolicyResizeDown
	};

	[Flags]
	public enum ImGuiTabItemFlags
	{
		None = 0,
		UnsavedDocument = 1 << 0,   // Append '*' to title without affecting the ID, as a convenience to avoid using the ### operator. Also: tab is selected on closure and closure is deferred by one frame to allow code to undo it without flicker.
		SetSelected = 1 << 1,   // Trigger flag to programmatically make the tab selected when calling BeginTabItem()
		NoCloseWithMiddleMouseButton = 1 << 2,   // Disable behavior of closing tabs (that are submitted with p_open != NULL) with middle mouse button. You can still repro this behavior on user's side with if (IsItemHovered() && IsMouseClicked(2)) *p_open = false.
		NoPushId = 1 << 3,   // Don't call PushID(tab->ID)/PopID() on BeginTabItem()/EndTabItem()
		NoTooltip = 1 << 4,   // Disable tooltip for the given tab
		NoReorder = 1 << 5,   // Disable reordering this tab or having another tab cross over this tab
		Leading = 1 << 6,   // Enforce the tab position to the left of the tab bar (after the tab list popup button)
		Trailing = 1 << 7    // Enforce the tab position to the right of the tab bar (before the scrolling buttons)
	};

	public class ImGui
    {
		public static bool Initialzie() => ImGuiAPI.ImGuiInitialzie();
		public static bool Shutdown() => ImGuiAPI.ImGuiShutdown();
		public static bool BeginFrame() => ImGuiAPI.ImGuiBeginFrame();
		public static bool EndFrame() => ImGuiAPI.ImGuiEndFrame();
		public static bool ShowDemoWindow() => ImGuiAPI.ImGuiShowDemoWindow();
		public static bool BeginMainMenuBar() => ImGuiAPI.ImGuiBeginMainMenuBar();
		public static void EndMainMenuBar() => ImGuiAPI.ImGuiEndMainMenuBar();
		public static bool BeginMenuBar() => ImGuiAPI.ImGuiBeginMenuBar();
		public static void EndMenuBar() => ImGuiAPI.ImGuiEndMenuBar();
		public static bool BeginMenu(string label,  bool enabled = true) => ImGuiAPI.ImGuiBeginMenu(label, enabled);
		public static bool MenuItem(string label, string shortcut = null, bool selected = false, bool enabled = true) => ImGuiAPI.ImGuiMenuItem(label, shortcut, selected, enabled);
		public static void EndMenu() => ImGuiAPI.ImGuiEndMenu_();
		public static void Separator() => ImGuiAPI.ImGuiSeparator();
		public static bool Begin(string name)
		{
			int open = -1;
			return ImGuiAPI.ImGuiBegin(name, ref open);
		}
		public static bool Begin(string name, ref bool open, int flags = 0)
		{
			int op = open ? 1 : 0;
			bool result = ImGuiAPI.ImGuiBegin(name, ref op, flags);
			open = (op == 1 ? true : false);
			return result;
		}
		public static void End() => ImGuiAPI.ImGuiEnd();
		public static bool CollapsingHeader(string label, int flags) => ImGuiAPI.ImGuiCollapsingHeader(label, flags);
		public static void Text(string text) => ImGuiAPI.ImGuiText(text);
		private static bool InputText(string label, byte[] buf, int bufSize, int flags) => ImGuiAPI.ImGuiInputText(label, buf, bufSize, flags);
		private static bool InputText(string label, byte[] buf, int flags = 0)
		{
			return InputText(label, buf, buf.Length, flags);
		}
		public static bool InputText(string label, ref string input, int flags = 0)
		{
			byte[] buf = new byte[256];
			byte[] bytes = Encoding.ASCII.GetBytes(input);
			Array.Copy(bytes, buf, bytes.Length);

			bool result = InputText(label, buf, buf.Length, flags);
			string temp = Encoding.ASCII.GetString(buf);
			input = "";
			for (int i = 0; i < temp.Length && temp[i] != '\0'; i++) { input += temp[i]; }

			return true;
		}
		public static void SameLine(float offsetFromStartX = 0.0f, float spacingW = 1.0f) => ImGuiAPI.ImGuiSameLine(offsetFromStartX, spacingW);
		public static bool Button(string label, float sizeX = 0, float sizeY = 0) => ImGuiAPI.ImGuiButton(label, sizeX, sizeY);
		public static bool DragFloat3(string label, ref float x, ref float y, ref float z, float v_speed = 1.0f, float v_min = 0.0f, float v_max = 0.0f) => ImGuiAPI.ImGuiDragFloat3(label, ref x, ref y, ref z, v_max, v_min);
		public static bool DragFloat3(string label, ref Vector3 v, float v_speed = 1.0f, float v_min = 0.0f, float v_max = 0.0f)
		{
			return DragFloat3(label, ref v.x, ref v.y, ref v.z, v_speed, v_min, v_max);
		}

		public static bool TreeNode(string title) => ImGuiAPI.ImGuiTreeNode(title);
		public static bool TreePop() => ImGuiAPI.ImGuiTreePop();
		public static bool BeginTabBar(string title, ImGuiTabBarFlags flags = ImGuiTabBarFlags.None) => ImGuiAPI.ImGuiBeginTabBar(title, (int)flags);
		public static bool EndTabBar() => ImGuiAPI.ImGuiEndTabBar();
		private static bool BeginTabItem(string label, ref int open, ImGuiTabItemFlags flags = ImGuiTabItemFlags.None) => ImGuiAPI.ImGuiBeginTabItem(label, ref open, (int)flags);
		public static bool BeginTabItem(string label)
		{
			int open = -1;
			return BeginTabItem(label, ref open);
		}
		public static bool ImGuiBeginTabItem(string label, ref bool open, ImGuiTabItemFlags flags)
		{
			int op = open ? 1 : 0;
			bool result = BeginTabItem(label, ref op, flags);
			open = (op == 1 ? true : false);
			return result;
		}		
		public static bool EndTabItem() => ImGuiAPI.ImGuiEndTabItem();
	}
}
