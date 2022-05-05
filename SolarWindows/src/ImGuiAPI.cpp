#include "vendor/imgui/imgui.h"
#include "vendor/imgui/imgui_impl_win32.h"
#include "vendor/imgui/imgui_impl_dx11.h"

#include "Win32Platform.h"
#include "DX11Renderer.h"


namespace ImGuiAPI
{
	EDITOR_INTERFACE(bool) ImGuiInitialzie()
	{
		if (ImGui::CreateContext())
		{
			ImGui::StyleColorsDark();
			ImGui::GetStyle().WindowRounding = 0;

			if (ImGui_ImplWin32_Init(winState.window))
			{
				RenderState* renderState = GetRenderState();

				if (ImGui_ImplDX11_Init(renderState->device, renderState->context))
				{
					return true;
				}
				else
				{
					SOLFATAL("Could not start dx11 imgui");
				}
			}
			else
			{
				SOLFATAL("Could not start win32 imgui");
			}
		}
		else
		{
			SOLFATAL("Could not create imgui context");
		}

		return false;
	}

	EDITOR_INTERFACE(void) ImGuiShutdown()
	{
		ImGui_ImplDX11_Shutdown();
		ImGui_ImplWin32_Shutdown();
		ImGui::DestroyContext();
	}

	EDITOR_INTERFACE(void) ImGuiBeginFrame()
	{
		ImGui_ImplDX11_NewFrame();
		ImGui_ImplWin32_NewFrame();

		ImGui::NewFrame();
	}

	EDITOR_INTERFACE(void) ImGuiEndFrame()
	{
		ImGui::Render();
		ImGui_ImplDX11_RenderDrawData(ImGui::GetDrawData());
	}

	EDITOR_INTERFACE(void) ImGuiShowDemoWindow()
	{
		ImGui::ShowDemoWindow();
	}

	EDITOR_INTERFACE(bool) ImGuiBeginMainMenuBar()
	{
		return ImGui::BeginMainMenuBar();
	}

	EDITOR_INTERFACE(void) ImGuiEndMainMenuBar()
	{
		ImGui::EndMainMenuBar();
	}

	EDITOR_INTERFACE(bool) ImGuiBeginMenuBar()
	{
		return ImGui::BeginMenuBar();
	}

	EDITOR_INTERFACE(void) ImGuiEndMenuBar()
	{
		ImGui::EndMenuBar();
	}

	EDITOR_INTERFACE(bool) ImGuiBeginMenu(const char* label, bool enabled)
	{
		return ImGui::BeginMenu(label, enabled);
	}

	EDITOR_INTERFACE(bool) ImGuiMenuItem(const char* label, const char* shortcut, bool selected, bool enabled)
	{
		return ImGui::MenuItem(label, shortcut, selected, enabled);
	}

	EDITOR_INTERFACE(void) ImGuiEndMenu_()
	{
		ImGui::EndMenu();
	}

	EDITOR_INTERFACE(void) ImGuiSeparator()
	{
		ImGui::Separator();
	}

	EDITOR_INTERFACE(bool) ImGuiBegin(const char* name, int* p_open, int flags)
	{
		if (*p_open == -1) { return ImGui::Begin(name); }
		return ImGui::Begin(name, (bool*)p_open, flags);
	}

	EDITOR_INTERFACE(void) ImGuiEnd()
	{
		ImGui::End();
	}

	EDITOR_INTERFACE(bool) ImGuiCollapsingHeader(const char* label, int flags)
	{
		return ImGui::CollapsingHeader(label, flags);
	}

	EDITOR_INTERFACE(bool) ImGuiInputText(const char* label, char* buf, int buf_size, int flags)
	{
		return ImGui::InputText(label, buf, buf_size, flags);
	}

	EDITOR_INTERFACE(void) ImGuiText(const char* text)
	{
		ImGui::Text(text);	
	}

	EDITOR_INTERFACE(void) ImGuiSameLine(float offset_from_start_x, float spacing_w)
	{
		ImGui::SameLine(offset_from_start_x, spacing_w);
	}

	EDITOR_INTERFACE(bool) ImGuiButton(const char* label, float sizeX, float sizeY)
	{
		return ImGui::Button(label, ImVec2(sizeX, sizeY));
	}

	EDITOR_INTERFACE(bool) ImGuiDragFloat3(const char* label, float* x, float* y, float* z, float v_speed, float v_min, float v_max)
	{
		float v[3] = {*x, *y, *z};
		bool r = ImGui::DragFloat3(label, v, v_speed, v_min, v_max);

		*x = v[0];
		*y = v[1];
		*z = v[2];

		return r;
	}

	EDITOR_INTERFACE(bool) ImGuiTreeNode(const char *title)
	{
		return ImGui::TreeNode(title);
	}

	EDITOR_INTERFACE(void) ImGuiTreePop()
	{
		ImGui::TreePop();
	}

	EDITOR_INTERFACE(bool) ImGuiBeginTabBar(const char* title, int flags)
	{
		return ImGui::BeginTabBar("MyTabBar", flags);
	}

	EDITOR_INTERFACE(void) ImGuiEndTabBar()
	{
		ImGui::EndTabBar();
	}

	EDITOR_INTERFACE(bool) ImGuiBeginTabItem(const char* label, int* open, int flags)
	{
		if (*open == -1) { return ImGui::BeginTabItem(label); }
		return ImGui::BeginTabItem(label, (bool*)open, flags);
	}
	
	EDITOR_INTERFACE(void) ImGuiEndTabItem()
	{
		ImGui::EndTabItem();
	}

	EDITOR_INTERFACE(void) ImGuiColumns(int number)
	{
		ImGui::Columns(number);
	}

	EDITOR_INTERFACE(void) ImGuiColumnsEx(int number, const char* title, bool32 border)
	{
		ImGui::Columns(number, title, border); 
	}

	EDITOR_INTERFACE(void) ImGuiNextColumn()
	{
		ImGui::NextColumn();
	}
	
	EDITOR_INTERFACE(void) ImGuiSepartor()
	{
		ImGui::Separator();
	}

	EDITOR_INTERFACE(bool) ImGuiSelectable(const char* label, bool32 selected, int flags, float xSize, float ySize)
	{
		return ImGui::Selectable(label, selected, flags, ImVec2(xSize, ySize));
	}

	EDITOR_INTERFACE(void) ImGuiOpenPopup(const char *id, int flags)
	{
		ImGui::OpenPopup(id, flags);
	}

	EDITOR_INTERFACE(bool) ImGuiBeginPopupModal(const char *id)
	{
		bool result =  ImGui::BeginPopupModal(id);
		return result;
	}

	EDITOR_INTERFACE(bool) ImGuiBeginPopupModalEx(const char* id, int* open, int flags)
	{
		bool result = false;
		if (*open == -1) { result = ImGui::BeginPopupModal(id, NULL, flags); }
		else { result = ImGui::BeginPopupModal(id, (bool*)open, flags); }

		return result;
	}

	EDITOR_INTERFACE(void) ImGuiCloseCurrentPopup()
	{
		ImGui::CloseCurrentPopup();
	}

	EDITOR_INTERFACE(void) ImGuiEndPopup()
	{
		ImGui::EndPopup();
	}

	EDITOR_INTERFACE(bool) ImGuiBeginPopup(const char* id, int flags)
	{
		bool result = ImGui::BeginPopup(id, flags);
		return result;
	}

	EDITOR_INTERFACE(bool) ImGuiCheckBox(const char* label, bool32* b)
	{
		bool result = ImGui::Checkbox(label, (bool*)b);	
		return result;
	}

	EDITOR_INTERFACE(bool) ImGuiComboStringArr(const char* label, int *currentItem, const char *items[], int count, int heightInItems)
	{
		bool result = ImGui::Combo(label, currentItem, items, count, heightInItems);
		return result;
	}

	EDITOR_INTERFACE(void) ImGuiPushItemWidth(float width)
	{
		ImGui::PushItemWidth(width);
	}

	EDITOR_INTERFACE(void) ImGuiPopItemWidth()
	{
		ImGui::PopItemWidth();
	}

	EDITOR_INTERFACE(bool) ImGuiInputInt(const char *label, int* v, int step, int step_fast, int flags)
	{
		return ImGui::InputInt(label, v, step, step_fast, flags);
	}

	EDITOR_INTERFACE(void) ImGuiPushId(int id)
	{
		ImGui::PushID(id);
	}

	EDITOR_INTERFACE(void) ImGuiPopId()
	{
		ImGui::PopID();
	}

}