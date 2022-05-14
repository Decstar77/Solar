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

	[Flags]
	public enum ImGuiSelectableFlags
	{
		None = 0,
		DontClosePopups = 1 << 0,   // Clicking this don't close parent popup window
		SpanAllColumns = 1 << 1,   // Selectable frame can span all columns (text will still fit in current column)
		AllowDoubleClick = 1 << 2,   // Generate press events on double clicks too
		Disabled = 1 << 3,   // Cannot be selected, display grayed out text
		AllowItemOverlap = 1 << 4    // (WIP) Hit testing to allow subsequent widgets to overlap this one
	};

	[Flags]
	public enum ImGuiPopupFlags
	{
		None = 0,
		MouseButtonLeft = 0,        // For BeginPopupContext*(): open on Left Mouse release. Guaranteed to always be == 0 (same as ImGuiMouseButton_Left)
		MouseButtonRight = 1,        // For BeginPopupContext*(): open on Right Mouse release. Guaranteed to always be == 1 (same as ImGuiMouseButton_Right)
		MouseButtonMiddle = 2,        // For BeginPopupContext*(): open on Middle Mouse release. Guaranteed to always be == 2 (same as ImGuiMouseButton_Middle)
		MouseButtonMask_ = 0x1F,
		MouseButtonDefault_ = 1,
		NoOpenOverExistingPopup = 1 << 5,   // For OpenPopup*(), BeginPopupContext*(): don't open if there's already a popup at the same level of the popup stack
		NoOpenOverItems = 1 << 6,   // For BeginPopupContextWindow(): don't return true when hovering items, only when hovering empty space
		AnyPopupId = 1 << 7,   // For IsPopupOpen(): ignore the ImGuiID parameter and test for any popup.
		AnyPopupLevel = 1 << 8,   // For IsPopupOpen(): search/test at any level of the popup stack (default test in the current level)
		AnyPopup = AnyPopupId | AnyPopupLevel
	};

	[Flags]
	public enum ImGuiInputTextFlags
	{
		None = 0,
		CharsDecimal = 1 << 0,   // Allow 0123456789.+-*/
		CharsHexadecimal = 1 << 1,   // Allow 0123456789ABCDEFabcdef
		CharsUppercase = 1 << 2,   // Turn a..z into A..Z
		CharsNoBlank = 1 << 3,   // Filter out spaces, tabs
		AutoSelectAll = 1 << 4,   // Select entire text when first taking mouse focus
		EnterReturnsTrue = 1 << 5,   // Return 'true' when Enter is pressed (as opposed to every time the value was modified). Consider looking at the IsItemDeactivatedAfterEdit() function.
		CallbackCompletion = 1 << 6,   // Callback on pressing TAB (for completion handling)
		CallbackHistory = 1 << 7,   // Callback on pressing Up/Down arrows (for history handling)
		CallbackAlways = 1 << 8,   // Callback on each iteration. User code may query cursor position, modify text buffer.
		CallbackCharFilter = 1 << 9,   // Callback on character inputs to replace or discard them. Modify 'EventChar' to replace or discard, or return 1 in callback to discard.
		AllowTabInput = 1 << 10,  // Pressing TAB input a '\t' character into the text field
		CtrlEnterForNewLine = 1 << 11,  // In multi-line mode, unfocus with Enter, add new line with Ctrl+Enter (default is opposite: unfocus with Ctrl+Enter, add line with Enter).
		NoHorizontalScroll = 1 << 12,  // Disable following the cursor horizontally
		AlwaysInsertMode = 1 << 13,  // Insert mode
		ReadOnly = 1 << 14,  // Read-only mode
		Password = 1 << 15,  // Password mode, display all characters as '*'
		NoUndoRedo = 1 << 16,  // Disable undo/redo. Note that input text owns the text data while active, if you want to provide your own undo/redo stack you need e.g. to call ClearActiveID().
		CharsScientific = 1 << 17,  // Allow 0123456789.+-*/eE (Scientific notation input)
		CallbackResize = 1 << 18,  // Callback on buffer capacity changes request (beyond 'buf_size' parameter value), allowing the string to grow. Notify when the string wants to be resized (for string types which hold a cache of their Size). You will be provided a new BufSize in the callback and NEED to honor it. (see misc/cpp/imgui_stdlib.h for an example of using this)
		CallbackEdit = 1 << 19,  // Callback on any edit (note that InputText() already returns true on edit, the callback is useful mainly to manipulate the underlying buffer while focus is active)
	};

	[Flags]
	public enum ImGuiFocusedFlags
	{
		None = 0,
		ChildWindows = 1 << 0,   // IsWindowFocused(): Return true if any children of the window is focused
		RootWindow = 1 << 1,   // IsWindowFocused(): Test from root window (top most parent of the current hierarchy)
		AnyWindow = 1 << 2,   // IsWindowFocused(): Return true if any window is focused. Important: If you are trying to tell how to dispatch your low-level inputs, do NOT use this. Use 'io.WantCaptureMouse' instead! Please read the FAQ!
		RootAndChildWindows = RootWindow | ChildWindows
	};

	[Flags]
	public enum ImGuiHoveredFlags
	{
		None = 0,        // Return true if directly over the item/window, not obstructed by another window, not obstructed by an active popup or modal blocking inputs under them.
		ChildWindows = 1 << 0,   // IsWindowHovered() only: Return true if any children of the window is hovered
		RootWindow = 1 << 1,   // IsWindowHovered() only: Test from root window (top most parent of the current hierarchy)
		AnyWindow = 1 << 2,   // IsWindowHovered() only: Return true if any window is hovered
		AllowWhenBlockedByPopup = 1 << 3,   // Return true even if a popup window is normally blocking access to this item/window
		AllowWhenBlockedByActiveItem = 1 << 5,   // Return true even if an active item is blocking access to this item/window. Useful for Drag and Drop patterns.
		AllowWhenOverlapped = 1 << 6,   // Return true even if the position is obstructed or overlapped by another window
		AllowWhenDisabled = 1 << 7,   // Return true even if the item is disabled
		RectOnly = AllowWhenBlockedByPopup | AllowWhenBlockedByActiveItem | AllowWhenOverlapped,
		RootAndChildWindows = RootWindow | ChildWindows
	};

	public enum ImGuiStyleVar
	{
		// Enum name --------------------- // Member in ImGuiStyle structure (see ImGuiStyle for descriptions)
		Alpha,               // float     Alpha
		WindowPadding,       // ImVec2    WindowPadding
		WindowRounding,      // float     WindowRounding
		WindowBorderSize,    // float     WindowBorderSize
		WindowMinSize,       // ImVec2    WindowMinSize
		WindowTitleAlign,    // ImVec2    WindowTitleAlign
		ChildRounding,       // float     ChildRounding
		ChildBorderSize,     // float     ChildBorderSize
		PopupRounding,       // float     PopupRounding
		PopupBorderSize,     // float     PopupBorderSize
		FramePadding,        // ImVec2    FramePadding
		FrameRounding,       // float     FrameRounding
		FrameBorderSize,     // float     FrameBorderSize
		ItemSpacing,         // ImVec2    ItemSpacing
		ItemInnerSpacing,    // ImVec2    ItemInnerSpacing
		IndentSpacing,       // float     IndentSpacing
		ScrollbarSize,       // float     ScrollbarSize
		ScrollbarRounding,   // float     ScrollbarRounding
		GrabMinSize,         // float     GrabMinSize
		GrabRounding,        // float     GrabRounding
		TabRounding,         // float     TabRounding
		ButtonTextAlign,     // ImVec2    ButtonTextAlign
		SelectableTextAlign, // ImVec2    SelectableTextAlign
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
		public static bool CollapsingHeader(string label, int flags = 0) => ImGuiAPI.ImGuiCollapsingHeader(label, flags);
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
			input = Util.AsciiBytesToString(buf, 0);

			return result;
		}
		public static void SameLine(float offsetFromStartX = 0.0f, float spacingW = 1.0f) => ImGuiAPI.ImGuiSameLine(offsetFromStartX, spacingW);
		public static bool Button(string label, float sizeX = 0, float sizeY = 0) => ImGuiAPI.ImGuiButton(label, sizeX, sizeY);
		public static bool DragFloat3(string label, ref float x, ref float y, ref float z, float v_speed = 1.0f, float v_min = 0.0f, float v_max = 0.0f) => ImGuiAPI.ImGuiDragFloat3(label, ref x, ref y, ref z, v_speed, v_max, v_min);
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
		public static bool BeginTabItem(string label, ref bool open, ImGuiTabItemFlags flags)
		{
			int op = open ? 1 : 0;
			bool result = BeginTabItem(label, ref op, flags);
			open = (op == 1 ? true : false);
			return result;
		}				
		public static bool EndTabItem() => ImGuiAPI.ImGuiEndTabItem();
		public static void Columns(int number) => ImGuiAPI.ImGuiColumns(number);
		public static void Columns(int number, string title, bool border) => ImGuiAPI.ImGuiColumnsEx(number, title, border);
		public static void NextColumn() => ImGuiAPI.ImGuiNextColumn();
		public static void Separtor() => ImGuiAPI.ImGuiSepartor();
		public static bool Selectable(string label, bool selected = false, ImGuiSelectableFlags flags = 0, float xSize = 0, float ySize = 0) => ImGuiAPI.ImGuiSelectable(label, selected, (int)flags, xSize, ySize);
		public static void OpenPopup(string id, ImGuiPopupFlags flags = 0) => ImGuiAPI.ImGuiOpenPopup(id, (int)flags);
		public static bool BeginPopupModal(string id) => ImGuiAPI.ImGuiBeginPopupModal(id);

		public static bool BeginPopupModal(string id, ImGuiWindowFlags flags)
		{
			int op = -1;
			bool result = ImGuiAPI.ImGuiBeginPopupModalEx(id, ref op, (int)flags);
			return result;
		}

		public static bool BeginPopupModal(string id, ref bool open, ImGuiWindowFlags flags)
        {
			int op = open ? 1 : 0;
			bool result = ImGuiAPI.ImGuiBeginPopupModalEx(id, ref op, (int)flags);
			open = (op == 1 ? true : false);
			return result;
		}

		public static void CloseCurrentPopup() => ImGuiAPI.ImGuiCloseCurrentPopup();
		public static void EndPopup() => ImGuiAPI.ImGuiEndPopup();
		public static bool BeginPopup(string id, ImGuiWindowFlags flags = 0) => ImGuiAPI.ImGuiBeginPopup(id, (int)flags);
		public static bool CheckBox(string label, ref bool b) => ImGuiAPI.ImGuiCheckBox(label, ref b);
		public static bool Combo(string label, ref int currentItem, string[] items, int heightInItems = -1) => ImGuiAPI.ImGuiComboStringArr(label, ref currentItem, items, items.Length, heightInItems);
		public static void PushItemWidth(float width) => ImGuiAPI.ImGuiPushItemWidth(width);
		public static void PopItemWidth() => ImGuiAPI.ImGuiPopItemWidth();
		public static bool InputInt( string label, ref int v, int step = 1, int stepFast = 100, ImGuiInputTextFlags flags = 0) => ImGuiAPI.ImGuiInputInt(label, ref v, step, stepFast, (int)flags);
		public static void PushId(int id) => ImGuiAPI.ImGuiPushId(id);
		public static void PopId() => ImGuiAPI.ImGuiPopId();
		public static bool IsWindowFocused(ImGuiFocusedFlags flags = 0) => ImGuiAPI.ImGuiIsWindowFocused((int)flags);
		public static bool WantMouseInput() => ImGuiAPI.ImGuiWantMouseInput();
		public static bool IsAnyItemHovered() => ImGuiAPI.ImGuiIsAnyItemHovered();
		public static bool IsWindowHovered(ImGuiHoveredFlags flags = 0) => ImGuiAPI.ImGuiIsWindowHovered((int)flags);
		public static bool WantCaptureKeyboard() => ImGuiAPI.ImGuiWantCaptureKeyboard();
		public static float GetStyleItemSpacingX() => ImGuiAPI.ImGuiGetStyleItemSpacingX();
		public static float GetStyleItemSpacingY() => ImGuiAPI.ImGuiGetStyleItemSpacingY();
		public static float GetFrameHeightWithSpacing() => ImGuiAPI.ImGuiGetFrameHeightWithSpacing();
		public static bool BeginChild(string strId, float sizeX, float sizeY, bool border, ImGuiWindowFlags flags) => ImGuiAPI.ImGuiBeginChild(strId, sizeX, sizeY, border, (int)flags);
		public static void EndChild() => ImGuiAPI.ImGuiEndChild();
		public static void PushStyleVar(ImGuiStyleVar var, float x, float y) => ImGuiAPI.ImGuiPushStyleVar((int)var, x, y);
		public static void PopStyleVar() => ImGuiAPI.ImGuiPopStyleVar();
		public static void TextColored(float r, float g, float b, float a, string text) => ImGuiAPI.ImGuiTextColored(r, g, b, a, text);
		public static void TextUnformatted(string text) => ImGuiAPI.ImGuiTextUnformatted(text);
		public static float GetScrollY() => ImGuiAPI.ImGuiGetScrollY();
		public static float GetScrollMaxY() => ImGuiAPI.ImGuiGetScrollMaxY();
		public static void SetScrollHereY(float v) => ImGuiAPI.ImGuiSetScrollHereY(v);

	}
}
