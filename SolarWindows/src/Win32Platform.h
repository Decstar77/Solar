#pragma once

#define WIN32_LEAN_AND_MEAN            
#include <windows.h>

struct Win32State
{
	HWND window;
	HINSTANCE hinstance;

	bool running;
	bool active;
	bool rawInput;	

	int windowWidth;
	int windowHeight;
	int surfaceWidth;
	int surfaceHeight;
};

inline Win32State winState = {};