#include "Core.h"
#include <memory.h>
#include "vendor/imgui/imgui.h"
#include "vendor/imnodes/imnodes.h"

EDITOR_INTERFACE(bool) ImNodesInitialzie()
{
	ImNodes::CreateContext();
	return true;
}

EDITOR_INTERFACE(void) ImNodesShutdown()
{
	ImNodes::DestroyContext();
}

EDITOR_INTERFACE(void) ImNodesBeginNodeEditor()
{
	ImNodes::BeginNodeEditor();
}

EDITOR_INTERFACE(void) ImNodesEndNodeEditor()
{
	ImNodes::EndNodeEditor();
}

EDITOR_INTERFACE(void) ImNodesBeginNode(int id)
{
	ImNodes::BeginNode(id);
}

EDITOR_INTERFACE(void) ImNodesEndNode()
{
	ImNodes::EndNode();
}

EDITOR_INTERFACE(void) ImNodesBeginNodeTitleBar()
{
	ImNodes::BeginNodeTitleBar();
}

EDITOR_INTERFACE(void) ImNodesEndNodeTitleBar()
{
	ImNodes::EndNodeTitleBar();
}

EDITOR_INTERFACE(void) ImNodesBeginInputAttribute(int id, int shape)
{
	ImNodes::BeginInputAttribute(id, shape);
}

EDITOR_INTERFACE(void) ImNodesEndInputAttribute()
{
	ImNodes::EndInputAttribute();
}

EDITOR_INTERFACE(void) ImNodesBeginOutputAttribute(int id, int shape)
{
	ImNodes::BeginOutputAttribute(id, shape);
}

EDITOR_INTERFACE(void) ImNodesEndOutputAttribute()
{
	ImNodes::EndOutputAttribute();
}

EDITOR_INTERFACE(void) ImNodesLink(int id, int start_attr_id, int end_attr_id)
{
	ImNodes::Link(id, start_attr_id, end_attr_id);
}

EDITOR_INTERFACE(bool) ImNodesIsLinkCreated(int* started_at_pin_id, int* ended_at_pin_id, bool* created_from_snap)
{
	return ImNodes::IsLinkCreated(started_at_pin_id, ended_at_pin_id, created_from_snap);	
}

EDITOR_INTERFACE(bool) ImNodesIsLinkDropped(int* started_at_pin_id, bool including_detached_links)
{
	return ImNodes::IsLinkDropped(started_at_pin_id, including_detached_links);
}

EDITOR_INTERFACE(void) ImNodesPushColorStyle(int item, uint32 col)
{
	ImNodes::PushColorStyle(item, col);
}

EDITOR_INTERFACE(void) ImNodesPopColorStyle()
{
	ImNodes::PopColorStyle();	
}

EDITOR_INTERFACE(bool) ImNodesIsEditorHovered()
{
	return ImNodes::IsEditorHovered();
}

EDITOR_INTERFACE(void) ImNodesSetNodeScreenSpacePos(int id, float x, float y)
{
	ImNodes::SetNodeScreenSpacePos(id, ImVec2(x, y));	
}

EDITOR_INTERFACE(void) ImNodesSetNodeEditorSpacePos(int id, float x, float y)
{
	ImNodes::SetNodeEditorSpacePos(id, ImVec2(x, y));
}

EDITOR_INTERFACE(void) ImNodesGetNodeEditorSpacePos(int id, float* x, float* y)
{
	ImVec2 r =  ImNodes::GetNodeEditorSpacePos(id);
	*x = r.x;
	*y = r.y;
}