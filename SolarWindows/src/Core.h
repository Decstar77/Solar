#pragma once

#include <cstdint>
#include <iostream>
#include <string>

typedef uint8_t uint8;
typedef uint16_t uint16;
typedef uint32_t uint32;
typedef uint64_t uint64;

typedef int8_t int8;
typedef int16_t int16;
typedef int32_t int32;
typedef int64_t int64;

typedef int32 bool32;
typedef float real32;
typedef double real64;

typedef bool bool8;

#define EDITOR_INTERFACE(type) extern "C" __declspec(dllexport) type __stdcall

#define SOL_DEBUG_RENDERING _DEBUG

#define SOLFATAL(message)	std::cout << "INTERNAL FATAL: " << message << std::endl;
#define SOLERROR(message)	std::cout << "INTERNAL ERROR: " << message << std::endl;
#define SOLWARN(message)	std::cout << "INTERNAL WARN:   " << message << std::endl;
#define SOLINFO(message)	std::cout << "INTERNAL INFO:  " << message << std::endl;
#define SOLDEBUG(message)	std::cout << "INTERNAL DEBUG: " << message << std::endl;
#define SOLTRACE(message)	std::cout << "INTERNAL TRACE: " << message << std::endl;

#if _MSC_VER
#include <intrin.h>
#define debugBreak() __debugbreak()
#else
#define debugBreak() __builtin_trap()
#endif

#define Assert(expr, msg)                                            \
    {                                                                \
        if (expr) {                                                  \
        } else {                                                     \
                                                                     \
            debugBreak();                                            \
        }                                                            \
    }



