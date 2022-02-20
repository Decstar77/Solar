#include "ImGuiAPI.h"

#include "vendor/imgui/imgui.h"
#include "vendor/imgui/imgui_impl_win32.h"
#include "vendor/imgui/imgui_impl_dx11.h"

#include "vendor/imguizmo/ImGuizmo.h"

#include "Win32Platform.h"
#include "DX11Renderer.h"


namespace ImGuiAPI
{
	EDITOR_INTERFACE(bool) Initialzie()
	{
		if (ImGui::CreateContext())
		{
			ImGui::StyleColorsDark();
			ImGui::GetStyle().WindowRounding = 0;

			if (ImGui_ImplWin32_Init(winState.window))
			{
				if (ImGui_ImplDX11_Init(renderState.deviceContext.device, renderState.deviceContext.context))
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

	EDITOR_INTERFACE(void) Shutdown()
	{
		ImGui_ImplDX11_Shutdown();
		ImGui_ImplWin32_Shutdown();
		ImGui::DestroyContext();
	}

	EDITOR_INTERFACE(void) BeginFrame()
	{
		ImGui_ImplDX11_NewFrame();
		ImGui_ImplWin32_NewFrame();

		ImGui::NewFrame();
		ImGuizmo::BeginFrame();
	}

	EDITOR_INTERFACE(void) EndFrame()
	{
		ImGui::Render();
		ImGui_ImplDX11_RenderDrawData(ImGui::GetDrawData());
	}

	EDITOR_INTERFACE(void) ShowDemoWindow()
	{
		ImGui::ShowDemoWindow();
	}

	EDITOR_INTERFACE(bool) BeginMainMenuBar()
	{
		return ImGui::BeginMainMenuBar();
	}

	EDITOR_INTERFACE(void) EndMainMenuBar()
	{
		ImGui::EndMainMenuBar();
	}

	EDITOR_INTERFACE(bool) BeginMenu(const char* label, bool enabled)
	{
		return ImGui::BeginMenu(label, enabled);
	}

	EDITOR_INTERFACE(bool) MenuItem(const char* label, const char* shortcut, bool selected, bool enabled)
	{
		return ImGui::MenuItem(label, shortcut, selected, enabled);
	}

	EDITOR_INTERFACE(void) EndMenu_()
	{
		ImGui::EndMenu();
	}

	EDITOR_INTERFACE(void) Separator()
	{
		ImGui::Separator();
	}

	EDITOR_INTERFACE(bool) Begin(const char* name, int* p_open, int flags)
	{
		if (*p_open == -1) { return ImGui::Begin(name); }
		return ImGui::Begin(name, (bool*)p_open, flags);
	}

	EDITOR_INTERFACE(void) End()
	{
		ImGui::End();
	}

	EDITOR_INTERFACE(bool) CollapsingHeader(const char* label, int flags)
	{
		return ImGui::CollapsingHeader(label, flags);
	}

	EDITOR_INTERFACE(bool) InputText(const char* label, char* buf, int buf_size, int flags)
	{
		return ImGui::InputText(label, buf, buf_size, flags);
	}

	EDITOR_INTERFACE(void) Text(const char* text)
	{
		ImGui::Text(text);	
	}

	EDITOR_INTERFACE(void) SameLine(float offset_from_start_x, float spacing_w)
	{
		ImGui::SameLine(offset_from_start_x, spacing_w);
	}

	EDITOR_INTERFACE(bool) Button(const char* label, float sizeX, float sizeY)
	{
		return ImGui::Button(label, ImVec2(sizeX, sizeY));
	}

	EDITOR_INTERFACE(bool) DragFloat3(const char* label, float* x, float* y, float* z, float v_speed, float v_min, float v_max)
	{
		float v[3] = {*x, *y, *z};
		bool r = ImGui::DragFloat3(label, v, v_speed, v_min, v_max);

		*x = v[0];
		*y = v[1];
		*z = v[2];

		return r;
	}

	EDITOR_INTERFACE(void) GizmoEnable(bool enable)
	{
		ImGuizmo::Enable(enable);
	}

	EDITOR_INTERFACE(void) GizmoSetRect(float x, float y, float width, float height)
	{
		ImGuizmo::SetRect(x, y, width, height);
	}

	EDITOR_INTERFACE(bool) GizmoManipulate(Mat4f proj, Mat4f view, Mat4f *world, int operation, int mode)
	{
		Mat4f v = Translate(Mat4f(), Vec3f(0,0,10));	
		return ImGuizmo::Manipulate(view.ptr, proj.ptr, (ImGuizmo::OPERATION)operation, (ImGuizmo::MODE)mode, world->ptr);
	}
}