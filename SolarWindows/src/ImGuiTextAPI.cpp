#include "Core.h"
#include <memory.h>
#include "vendor/imgui/imgui.h"
#include "vendor/imgui_text/TextEditor.h"

std::unique_ptr<TextEditor> editor;

EDITOR_INTERFACE(void) ImGuiTextInitialize()
{
	editor = std::make_unique<TextEditor>();
	auto lang = TextEditor::LanguageDefinition::HLSL();
    editor->SetLanguageDefinition(lang);    
}

EDITOR_INTERFACE(void) ImGuiTextRender(const char* title)
{
    editor->Render(title);
}

EDITOR_INTERFACE(void) ImGuiTextSetText(const char* text)
{
    editor->SetText(text);
}

EDITOR_INTERFACE(void) ImGuiTextGetText(char* text, uint32 size)
{
    std::string t = editor->GetText();
    Assert(t.length() < size, "ImGuiTextGetText buffer not big enough");
    memcpy(text, t.c_str(), t.length());
}

EDITOR_INTERFACE(bool) ImGuiTextIsShowingWhitespaces()
{
    return editor->IsShowingWhitespaces();
}


EDITOR_INTERFACE(void) ImGuiTextSetShowWhitespaces(bool32 v)
{
    editor->SetShowWhitespaces(v);
}


EDITOR_INTERFACE(bool) ImGuiTextIsReadOnly()
{
    return editor->IsReadOnly();
}


EDITOR_INTERFACE(void) ImGuiTextSetReadOnly(bool32 v)
{    
    editor->SetReadOnly(v);
}

