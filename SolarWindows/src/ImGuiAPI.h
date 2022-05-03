#pragma once
#include "Core.h"

namespace ImGuiAPI
{
	EDITOR_INTERFACE(bool) ImGuiInitialzie();
	EDITOR_INTERFACE(void) ImGuiShutdown();
	EDITOR_INTERFACE(void) ImGuiBeginFrame();
	EDITOR_INTERFACE(void) ImGuiEndFrame();
	EDITOR_INTERFACE(void) ImGuiShowDemoWindow();

	EDITOR_INTERFACE(bool) ImGuiBeginMainMenuBar();
	EDITOR_INTERFACE(void) ImGuiEndMainMenuBar();
	EDITOR_INTERFACE(bool) ImGuiBeginMenu(const char* label, bool enabled);
	EDITOR_INTERFACE(bool) ImGuiMenuItem(const char* label, const char* shortcut, bool selected, bool enabled);
	EDITOR_INTERFACE(void) ImGuiEndMenu_();

	EDITOR_INTERFACE(void) ImGuiSeparator();

	EDITOR_INTERFACE(bool) ImGuiBegin(const char* name, int* p_open, int flags);
	EDITOR_INTERFACE(void) ImGuiEnd();

	EDITOR_INTERFACE(bool) ImGuiCollapsingHeader(const char* label, int flags);
	EDITOR_INTERFACE(bool) ImGuiInputText(const char* label, char* buf, int buf_size, int flags);
	EDITOR_INTERFACE(void) ImGuiText(const char* text);

	EDITOR_INTERFACE(void) ImGuiSameLine(float offset_from_start_x, float spacing_w);
	EDITOR_INTERFACE(bool) ImGuiButton(const char* label, float sizeX, float sizeY);

	EDITOR_INTERFACE(bool) ImGuiDragFloat3(const char* label, float* x, float* y, float* z, float v_speed, float v_min, float v_max);
}