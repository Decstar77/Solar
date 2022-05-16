
#include "Core.h"
#include <memory.h>
#include "vendor/imgui/imgui.h"
#include "vendor/imgizmo/ImGuizmo.h"

#include "SolarMath.h"

EDITOR_INTERFACE(void) ImGizmoEnable(bool enable)
{
	ImGuizmo::Enable(enable);
}

EDITOR_INTERFACE(void) ImGizmoSetRect(float x, float y, float width, float height)
{
	ImGuizmo::SetRect(x, y, width, height);
}

EDITOR_INTERFACE(bool) ImGizmoIsUsing()
{
	return ImGuizmo::IsUsing();
}

EDITOR_INTERFACE(bool) ImGizmoManipulate(Mat4f proj, Mat4f view, Mat4f* world, int operation, int mode, Mat4f *deltaMatrix, float* snap)
{
	if (snap[0] == 0.0f)
		return ImGuizmo::Manipulate(view.ptr, proj.ptr, (ImGuizmo::OPERATION)operation, (ImGuizmo::MODE)mode, world->ptr, deltaMatrix->ptr);
	return ImGuizmo::Manipulate(view.ptr, proj.ptr, (ImGuizmo::OPERATION)operation, (ImGuizmo::MODE)mode, world->ptr, deltaMatrix->ptr, snap);	
}

