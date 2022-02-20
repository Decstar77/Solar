#pragma once
#include "Core.h"
#include "SolarMath.h"

namespace ImGuiAPI
{
	EDITOR_INTERFACE(bool) Initialzie();
	EDITOR_INTERFACE(void) Shutdown();
	EDITOR_INTERFACE(void) BeginFrame();
	EDITOR_INTERFACE(void) EndFrame();
	EDITOR_INTERFACE(void) ShowDemoWindow();

	EDITOR_INTERFACE(bool) BeginMainMenuBar();
	EDITOR_INTERFACE(void) EndMainMenuBar();
	EDITOR_INTERFACE(bool) BeginMenu(const char* label, bool enabled);
	EDITOR_INTERFACE(bool) MenuItem(const char* label, const char* shortcut, bool selected, bool enabled);
	EDITOR_INTERFACE(void) EndMenu_();

	EDITOR_INTERFACE(void) Separator();

	EDITOR_INTERFACE(bool) Begin(const char* name, int* p_open, int flags);
	EDITOR_INTERFACE(void) End();

	EDITOR_INTERFACE(bool) CollapsingHeader(const char* label, int flags);
	EDITOR_INTERFACE(bool) InputText(const char* label, char* buf, int buf_size, int flags);
	EDITOR_INTERFACE(void) Text(const char* text);

	EDITOR_INTERFACE(void) SameLine(float offset_from_start_x, float spacing_w);
	EDITOR_INTERFACE(bool) Button(const char* label, float sizeX, float sizeY);

	EDITOR_INTERFACE(bool) DragFloat3(const char* label, float* x, float* y, float* z, float v_speed, float v_min, float v_max);

	EDITOR_INTERFACE(void) GizmoEnable(bool enable);
	EDITOR_INTERFACE(void) GizmoSetRect(float x, float y, float width, float height);
	EDITOR_INTERFACE(bool) GizmoManipulate(Mat4f proj, Mat4f view, Mat4f *world, int operation, int mode);

	
}