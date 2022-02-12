#include "Core.h"
#include "Win32Platform.h"
#include "SolarMath.h"

BOOL APIENTRY DllMain(HMODULE hModule,	DWORD ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

LRESULT CALLBACK WindProc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam);
EDITOR_INTERFACE(bool) Win32CreateWindow(char* title, int width, int height, int xPos, int yPos)
{
	winState.hinstance = GetModuleHandleA(0);

	WNDCLASSEXA windowClass = {};
	windowClass.cbSize = sizeof(windowClass);
	windowClass.style = CS_OWNDC | CS_HREDRAW | CS_VREDRAW;
	windowClass.lpfnWndProc = &WindProc;
	windowClass.cbClsExtra = 0;
	windowClass.cbWndExtra = 0;
	windowClass.hInstance = 0;
	windowClass.hIcon = LoadIcon(winState.hinstance, IDI_APPLICATION);
	windowClass.hIconSm = nullptr;
	windowClass.hCursor = LoadCursor(NULL, IDC_ARROW);
	windowClass.hbrBackground = nullptr;
	windowClass.lpszClassName = "GaMe eNgInE";
	if (RegisterClassExA(&windowClass))
	{
		// @NOTE: Create window
		uint32 clientX = xPos;
		uint32 clientY = yPos;
		uint32 clientWidth = width;
		uint32 clientHeight = height;

		uint32 windowX = clientX;
		uint32 windowY = clientY;
		uint32 windowWidth = clientWidth;
		uint32 windowHeight = clientHeight;

		uint32 windowStyle = WS_OVERLAPPED | WS_SYSMENU | WS_CAPTION;
		uint32 windowExStyle = WS_EX_APPWINDOW;

		windowStyle |= WS_MAXIMIZEBOX;
		windowStyle |= WS_MINIMIZEBOX;
		windowStyle |= WS_THICKFRAME;

		// @NOTE: Obtain the size of the border.
		RECT border_rect = { 0, 0, 0, 0 };
		AdjustWindowRectEx(&border_rect, windowStyle, 0, windowExStyle);

		// @NOTE: In this case, the border rectangle is negative.
		windowX += border_rect.left;
		windowY += border_rect.top;

		// @NOTE: Grow by the size of the OS border.
		windowWidth += border_rect.right - border_rect.left;
		windowHeight += border_rect.bottom - border_rect.top;

		winState.window = CreateWindowExA(
			NULL, windowClass.lpszClassName, title, windowStyle,
			windowX, windowY, windowWidth, windowHeight,
			NULL, NULL, winState.hinstance, NULL);

		if (winState.window)
		{
			SOLINFO("Win32 Window created and running");
			ShowWindow(winState.window, SW_SHOW);
			winState.running = true;

			winState.windowWidth = windowWidth;
			winState.windowHeight = windowHeight;

			RECT cleintRect = {};
			GetClientRect(winState.window, &cleintRect);
			winState.surfaceWidth = cleintRect.right - cleintRect.left;
			winState.surfaceHeight = cleintRect.bottom - cleintRect.top;

			//InitializeRawInput();
			//InitializeClock();
		}
		else
		{
			MessageBoxA(NULL, "Window creation failed!", "Error!", MB_ICONEXCLAMATION | MB_OK);
			//SOLFATAL("Window creation failed!");
			return false;
		}
	}
	else
	{
		MessageBoxA(0, "Window registration failed", "Error", MB_ICONEXCLAMATION | MB_OK);
		//SOLFATAL("Window registration failed");
		return false;
	}

	return true;
}

EDITOR_INTERFACE(void) Win32PostQuitMessage()
{
	winState.running = false;
	PostQuitMessage(0);
}

EDITOR_INTERFACE(void) Win32DestroyWindow()
{

}

#include "vendor/imgui/imgui_impl_win32.h"
extern IMGUI_IMPL_API LRESULT ImGui_ImplWin32_WndProcHandler(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);

void ProcessKeyboardInput(uint16 vkCode, bool32 isDown);
LRESULT CALLBACK WindProc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
{
	LRESULT result = {};

	result = ImGui_ImplWin32_WndProcHandler(hwnd, msg, wparam, lparam);
	if (result)
	{
		return result;
	}

	switch (msg)
	{
	case WM_DESTROY:
	{
		winState.running = false;
		PostQuitMessage(0);
	} break;
	case WM_CLOSE:
	{
		winState.running = false;
		PostQuitMessage(0);
	} break;
	case WM_SIZE:
	{
		RECT cleintRect = {};
		GetClientRect(hwnd, &cleintRect);

		uint32 surfaceWidth = cleintRect.right - cleintRect.left;
		uint32 surfaceHeight = cleintRect.bottom - cleintRect.top;

		RECT windowRect = {};
		GetWindowRect(hwnd, &windowRect);
		uint32 windowWidth = windowRect.right - windowRect.left;
		uint32 windowHeight = windowRect.bottom - windowRect.top;

		winState.surfaceWidth = surfaceWidth;
		winState.surfaceHeight = surfaceHeight;

		winState.windowWidth = windowWidth;
		winState.windowHeight = windowHeight;
	} break;
	case WM_INPUT:
	{
		//ProcessRawInput(lparam);
	}break;

	case WM_SYSKEYDOWN:
	case WM_SYSKEYUP:
	case WM_KEYDOWN:
	case WM_KEYUP:
	{
		uint32 vkCode = (uint32)wparam;
		bool32 isDown = ((lparam & ((int64)1 << (int64)31)) == 0);
		ProcessKeyboardInput(vkCode, isDown);
	}break;
	default: { result = DefWindowProcA(hwnd, msg, wparam, lparam);	}
	}

	return result;
}

#define IsKeyJustDown(input, key) (input->##key && !input->oldInput->##key)
#define IsKeyJustUp(input, key) (!input->##key && input->oldInput->##key)

struct MouseInput
{
	int mouseLocked;
	Vec2d mousePositionPixelCoords;
	Vec2d mousePositionNormalCoords;
	int mb1;
	int mb2;
	int mb3;
};

struct Input
{

	//
	//bool8 isPS4Controller;
	//real32 controllerLeftTrigger;
	//real32 controllerRightTrigger;
	//Vec2f controllerLeftThumbDrag;
	//Vec2f controllerRightThumbDrag;

	MouseInput mouseInput;
	int keys[UINT16_MAX];
};

static Input input = {};
static void ProcessMouseInput()
{
	POINT mousep = {};
	GetCursorPos(&mousep);
	ScreenToClient((HWND)winState.window, &mousep);
	real64 mx = (real64)mousep.x;
	real64 my = (real64)mousep.y;

	mx = Clamp<real64>(mx, 0.0f, (real64)winState.surfaceWidth);
	my = Clamp<real64>(my, 0.0f, (real64)winState.surfaceHeight);

	input.mouseInput.mousePositionPixelCoords.x = mx;
	input.mouseInput.mousePositionPixelCoords.y = my;

	input.mouseInput.mb1 = GetKeyState(VK_LBUTTON) & (1 << 15);
	input.mouseInput.mb2 = GetKeyState(VK_RBUTTON) & (1 << 15);
	input.mouseInput.mb3 = GetKeyState(VK_MBUTTON) & (1 << 15);

	input.mouseInput.mousePositionNormalCoords.x = (mx / (real64)winState.surfaceWidth);
	input.mouseInput.mousePositionNormalCoords.y = (my / (real64)winState.surfaceHeight);

	if (input.mouseInput.mouseLocked && winState.active)
	{
		SetCursor(FALSE);

		POINT p = {};
		p.x = winState.surfaceWidth / 2;
		p.y = winState.surfaceHeight / 2;

		ClientToScreen((HWND)winState.window, &p);
		SetCursorPos(p.x, p.y);
	}	
}

//static void ProcessRawMouseInput()
//{
//	Input* input = Input::Get();
//
//	POINT mousep = {};
//	GetCursorPos(&mousep);
//	ScreenToClient((HWND)winState.window, &mousep);
//	real32 mx = (real32)mousep.x;
//	real32 my = (real32)mousep.y;
//
//	mx = Clamp<real32>(mx, 0.0f, (real32)winState.windowWidth);
//	my = Clamp<real32>(my, 0.0f, (real32)winState.windowHeight);
//
//	input->mousePositionPixelCoords.x = mx;
//	input->mousePositionPixelCoords.y = my;
//
//	if (input->mouse_locked && winState.active)
//	{
//		SetCursor(FALSE);
//
//		input->oldInput->mousePositionPixelCoords = Vec2f((real32)(winState.windowWidth / 2),
//			(real32)(winState.windowHeight / 2));
//
//		POINT p = {};
//		p.x = winState.windowWidth / 2;
//		p.y = winState.windowHeight / 2;
//
//		ClientToScreen((HWND)winState.window, &p);
//
//		SetCursorPos(p.x, p.y);
//	}
//
//	input->mouseNorm.x = mx / (real32)winState.windowWidth;
//	input->mouseNorm.y = my / (real32)winState.windowHeight;
//
//	input->mb1 = GetKeyState(VK_LBUTTON) & (1 << 15);
//	input->mb2 = GetKeyState(VK_RBUTTON) & (1 << 15);
//	input->mb3 = GetKeyState(VK_MBUTTON) & (1 << 15);
//}



EDITOR_INTERFACE(bool) Win32PumpMessages(int* appInput, MouseInput *mouseInput)
{
	if (winState.running)
	{		
		winState.active = (bool8)GetFocus();
		input.mouseInput.mouseLocked = mouseInput->mouseLocked;

		MSG message = {};
		while (PeekMessageA(&message, NULL, 0, 0, PM_REMOVE))
		{
			TranslateMessage(&message);
			DispatchMessageA(&message);
		}

		ProcessMouseInput();

		for (int i = 0; i < UINT16_MAX; i++)
		{
			appInput[i] = input.keys[i];
		}

		mouseInput->mb1 = input.mouseInput.mb1;
		mouseInput->mb2 = input.mouseInput.mb2;
		mouseInput->mb3 = input.mouseInput.mb3;
		
		
		mouseInput->mousePositionNormalCoords = input.mouseInput.mousePositionNormalCoords;
		mouseInput->mousePositionPixelCoords = input.mouseInput.mousePositionPixelCoords;		

		//input->shift = (GetKeyState(VK_SHIFT) & (1 << 15));
		//input->alt = (GetKeyState(VK_MENU) & (1 << 15));
		//input->ctrl = (GetKeyState(VK_CONTROL) & (1 << 15));	

		//if (winState.rawInput)
		//{
		//	ProcessRawMouseInput();
		//}
		//else
		//{
		//	ProcessMouseInput();
		//}
	}

	return winState.running;
}

void ProcessKeyboardInput(uint16 vkCode, bool32 isDown)
{
	input.keys[vkCode] = isDown;
}

