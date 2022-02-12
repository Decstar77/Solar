#include "Core.h"
#include "Win32Platform.h"

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


struct Input
{
	//Vec2f mousePositionPixelCoords;
	//Vec2f mouseNorm;
	//Vec2f mouseDelta;
	//
	//bool8 isPS4Controller;
	//real32 controllerLeftTrigger;
	//real32 controllerRightTrigger;
	//Vec2f controllerLeftThumbDrag;
	//Vec2f controllerRightThumbDrag;

	//bool8 mouse_locked;

	int mb1;
	int mb2;
	int mb3;
	int alt;
	int shift;
	int ctrl;
	int w;
	int s;
	int a;
	int d;
	int q;
	int e;
	int r;
	int t;
	int z;
	int x;
	int c;
	int v;
	int b;
	int del;
	int tlda;
	int K1;
	int K2;
	int K3;
	int K4;
	int K5;
	int K6;
	int K7;
	int K8;
	int K9;
	int K0;
	int f1;
	int f2;
	int f3;
	int f4;
	int f5;
	int f6;
	int f7;
	int f8;
	int f9;
	int f10;
	int f11;
	int f12;
	int escape;
	int space;
	int controllerUp;
	int controllerDown;
	int controllerLeft;
	int controllerRight;
	int controllerStart;
	int controllerBack;
	int controllerLeftThumb;
	int controllerRightThumb;
	int controllerLeftShoulder;
	int controllerRightShoulder;
	int controllerA;
	int controllerB;
	int controllerX;
	int controllerY;
};

static Input input = {};
EDITOR_INTERFACE(bool) Win32PumpMessages(Input& appInput)
{
	if (winState.running)
	{
		//oldInput = input;
		//Input::Get()->mouseDelta = Vec2f(0);

		winState.active = (bool8)GetFocus();

		MSG message = {};
		while (PeekMessageA(&message, NULL, 0, 0, PM_REMOVE))
		{
			TranslateMessage(&message);
			DispatchMessageA(&message);
		}

		appInput = input;
		//appInput.a = true;
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
	if (vkCode == 'W')
	{
		input.w = isDown;
	}
	else if (vkCode == 'A')
	{
		input.a = isDown;
	}
	else if (vkCode == 'S')
	{
		input.s = isDown;
	}
	else if (vkCode == 'D')
	{
		input.d = isDown;
	}
	else if (vkCode == 'Q')
	{
		input.q = isDown;
	}
	else if (vkCode == 'E')
	{
		input.e = isDown;
	}
	else if (vkCode == 'R')
	{
		input.r = isDown;
	}
	else if (vkCode == 'T')
	{
		input.t = isDown;
	}
	else if (vkCode == 'Z')
	{
		input.z = isDown;
	}
	else if (vkCode == 'X')
	{
		input.x = isDown;
	}
	else if (vkCode == 'C')
	{
		input.c = isDown;
	}
	else if (vkCode == 'V')
	{
		input.v = isDown;
	}
	else if (vkCode == 'B')
	{
		input.b = isDown;
	}
	else if (vkCode == '0')
	{
		input.K0 = isDown;
	}
	else if (vkCode == '1')
	{
		input.K1 = isDown;
	}
	else if (vkCode == '2')
	{
		input.K2 = isDown;
	}
	else if (vkCode == '3')
	{
		input.K3 = isDown;
	}
	else if (vkCode == '4')
	{
		input.K4 = isDown;
	}
	else if (vkCode == '5')
	{
		input.K5 = isDown;
	}
	else if (vkCode == '6')
	{
		input.K6 = isDown;
	}
	else if (vkCode == '7')
	{
		input.K7 = isDown;
	}
	else if (vkCode == '8')
	{
		input.K8 = isDown;
	}
	else if (vkCode == '9')
	{
		input.K9 = isDown;
	}
	else if (vkCode == VK_F1)
	{
		input.f1 = isDown;
	}
	else if (vkCode == VK_F2)
	{
		input.f2 = isDown;
	}
	else if (vkCode == VK_F3)
	{
		input.f3 = isDown;
	}
	else if (vkCode == VK_F4)
	{
		input.f4 = isDown;
	}
	else if (vkCode == VK_F5)
	{
		input.f5 = isDown;
	}
	else if (vkCode == VK_F6)
	{
		input.f6 = isDown;
	}
	else if (vkCode == VK_F6)
	{
		input.f7 = isDown;
	}
	else if (vkCode == VK_F7)
	{
		input.f7 = isDown;
	}
	else if (vkCode == VK_F8)
	{
		input.f8 = isDown;
	}
	else if (vkCode == VK_F9)
	{
		input.f9 = isDown;
	}
	else if (vkCode == VK_F10)
	{
		input.f10 = isDown;
	}
	else if (vkCode == VK_F11)
	{
		input.f11 = isDown;
	}
	else if (vkCode == VK_F12)
	{
		input.f12 = isDown;
	}
	else if (vkCode == VK_ESCAPE)
	{
		input.escape = isDown;
	}
	else if (vkCode == VK_OEM_3)
	{
		input.tlda = isDown;
	}
	else if (vkCode == VK_DELETE)
	{
		input.del = isDown;
	}
	else if (vkCode == VK_SHIFT)
	{
		input.shift = isDown;
	}
	else if (vkCode == VK_MENU)
	{
		input.alt = isDown;
	}
	else if (vkCode == VK_CONTROL)
	{
		input.ctrl = isDown;
	}
	else if (vkCode == VK_SPACE)
	{
		input.space = isDown;
	}
}

