using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SolarSharp
{
	[StructLayout(LayoutKind.Sequential)]
	public struct FrameInput
	{
		public Vector2 mousePositionPixelCoords;
		public Vector2 mouseNorm;
		public Vector2 mouseDelta;

		[MarshalAs(UnmanagedType.Bool)] public bool mouseLocked;
		[MarshalAs(UnmanagedType.Bool)] public bool mb1;
		[MarshalAs(UnmanagedType.Bool)] public bool mb2;
		[MarshalAs(UnmanagedType.Bool)] public bool mb3;
		[MarshalAs(UnmanagedType.Bool)] public bool alt;
		[MarshalAs(UnmanagedType.Bool)] public bool shift;
		[MarshalAs(UnmanagedType.Bool)] public bool ctrl;
		[MarshalAs(UnmanagedType.Bool)] public bool w;
		[MarshalAs(UnmanagedType.Bool)] public bool s;
		[MarshalAs(UnmanagedType.Bool)] public bool a;
		[MarshalAs(UnmanagedType.Bool)] public bool d;
		[MarshalAs(UnmanagedType.Bool)] public bool q;
		[MarshalAs(UnmanagedType.Bool)] public bool e;
		[MarshalAs(UnmanagedType.Bool)] public bool r;
		[MarshalAs(UnmanagedType.Bool)] public bool t;
		[MarshalAs(UnmanagedType.Bool)] public bool z;
		[MarshalAs(UnmanagedType.Bool)] public bool x;
		[MarshalAs(UnmanagedType.Bool)] public bool c;
		[MarshalAs(UnmanagedType.Bool)] public bool v;
		[MarshalAs(UnmanagedType.Bool)] public bool b;
		[MarshalAs(UnmanagedType.Bool)] public bool del;
		[MarshalAs(UnmanagedType.Bool)] public bool tlda;
		[MarshalAs(UnmanagedType.Bool)] public bool K1;
		[MarshalAs(UnmanagedType.Bool)] public bool K2;
		[MarshalAs(UnmanagedType.Bool)] public bool K3;
		[MarshalAs(UnmanagedType.Bool)] public bool K4;
		[MarshalAs(UnmanagedType.Bool)] public bool K5;
		[MarshalAs(UnmanagedType.Bool)] public bool K6;
		[MarshalAs(UnmanagedType.Bool)] public bool K7;
		[MarshalAs(UnmanagedType.Bool)] public bool K8;
		[MarshalAs(UnmanagedType.Bool)] public bool K9;
		[MarshalAs(UnmanagedType.Bool)] public bool K0;
		[MarshalAs(UnmanagedType.Bool)] public bool f1;
		[MarshalAs(UnmanagedType.Bool)] public bool f2;
		[MarshalAs(UnmanagedType.Bool)] public bool f3;
		[MarshalAs(UnmanagedType.Bool)] public bool f4;
		[MarshalAs(UnmanagedType.Bool)] public bool f5;
		[MarshalAs(UnmanagedType.Bool)] public bool f6;
		[MarshalAs(UnmanagedType.Bool)] public bool f7;
		[MarshalAs(UnmanagedType.Bool)] public bool f8;
		[MarshalAs(UnmanagedType.Bool)] public bool f9;
		[MarshalAs(UnmanagedType.Bool)] public bool f10;
		[MarshalAs(UnmanagedType.Bool)] public bool f11;
		[MarshalAs(UnmanagedType.Bool)] public bool f12;
		[MarshalAs(UnmanagedType.Bool)] public bool escape;
		[MarshalAs(UnmanagedType.Bool)] public bool space;
		[MarshalAs(UnmanagedType.Bool)] public bool tab;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerUp;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerDown;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerLeft;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerRight;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerStart;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerBack;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerLeftThumb;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerRightThumb;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerLeftShoulder;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerRightShoulder;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerA;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerB;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerX;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerY;
    }

    public enum KeyCode
    {
		CTRL_L,
		SHIFT_L,
		W,
		A,
		S,
		D,
		C,
		Q,
		E,
		R,
		T,
		Z,
		V,
		F1,
		F2,
		F3,
		F4,
		F5,
		F6,
		F7,
		F8,
		F9,
		TLDA,
		DEL,
		ESCAPE,
		TAB,
		SPACE,
	}

    public enum MouseButton
    {
		MOUSE1,
		MOUSE2,
		MOUSE3,
	}

    public static class Input
    {
		public static Vector2 MousePositionPixelCoords => Application.Input.mousePositionPixelCoords;
		public static Vector2 MouseDelta => Application.Input.mouseDelta;

		public static void DisableMouse() => Application.Input.mouseLocked = true;
		public static void EnableMouse() => Application.Input.mouseLocked = false;

		public static bool IsMouseDown(MouseButton mouse)
        {
			switch (mouse)
			{
				case MouseButton.MOUSE1:
					return Application.Input.mb1;
				case MouseButton.MOUSE2:
					return Application.Input.mb2;
				case MouseButton.MOUSE3:
					return Application.Input.mb3;
			}
			return false;
		}			

		public static bool IsMouseButtonJustDown(MouseButton mouse)
        {
            switch (mouse)
            {
                case MouseButton.MOUSE1:
					return IskeyJustDown(Application.Input.mb1, Application.OldInput.mb1);
				case MouseButton.MOUSE2:
					return IskeyJustDown(Application.Input.mb2, Application.OldInput.mb2);
				case MouseButton.MOUSE3:
					return IskeyJustDown(Application.Input.mb3, Application.OldInput.mb3);
			}
			return false;
        }

		public static bool IsMouseButtonJustUp(MouseButton mouse)
		{
			switch (mouse)
			{
				case MouseButton.MOUSE1:
					return IskeyJustUp(Application.Input.mb1, Application.OldInput.mb1);
				case MouseButton.MOUSE2:
					return IskeyJustUp(Application.Input.mb2, Application.OldInput.mb2);
				case MouseButton.MOUSE3:
					return IskeyJustUp(Application.Input.mb3, Application.OldInput.mb3);
			}
			return false;
		}

		public static bool IskeyJustDown(KeyCode keyCode)
        {
            switch (keyCode)
            {
				case KeyCode.CTRL_L:
					return IskeyJustDown(Application.Input.ctrl, Application.OldInput.ctrl);
				case KeyCode.SHIFT_L:
					return IskeyJustDown(Application.Input.shift, Application.OldInput.shift);
				case KeyCode.W:
					return IskeyJustDown(Application.Input.w, Application.OldInput.w);
				case KeyCode.A:
					return IskeyJustDown(Application.Input.a, Application.OldInput.a);
				case KeyCode.S:
					return IskeyJustDown(Application.Input.s, Application.OldInput.s);
				case KeyCode.D:
					return IskeyJustDown(Application.Input.d, Application.OldInput.d);
				case KeyCode.Q:
					return IskeyJustDown(Application.Input.q, Application.OldInput.q);
				case KeyCode.C:
					return IskeyJustDown(Application.Input.c, Application.OldInput.c);
				case KeyCode.E:
					return IskeyJustDown(Application.Input.e, Application.OldInput.e);
				case KeyCode.R:
					return IskeyJustDown(Application.Input.r, Application.OldInput.r);
				case KeyCode.T:
					return IskeyJustDown(Application.Input.t, Application.OldInput.t);
				case KeyCode.Z:
					return IskeyJustDown(Application.Input.z, Application.OldInput.z);
				case KeyCode.F1:
					return IskeyJustDown(Application.Input.f1, Application.OldInput.f1);
				case KeyCode.F2:
					return IskeyJustDown(Application.Input.f2, Application.OldInput.f2);
				case KeyCode.F3:
					return IskeyJustDown(Application.Input.f3, Application.OldInput.f3);
				case KeyCode.F4:
					return IskeyJustDown(Application.Input.f4, Application.OldInput.f4);
				case KeyCode.F5:
					return IskeyJustDown(Application.Input.f5, Application.OldInput.f5);
				case KeyCode.F6:
					return IskeyJustDown(Application.Input.f6, Application.OldInput.f6);
				case KeyCode.F7:
					return IskeyJustDown(Application.Input.f7, Application.OldInput.f7);
				case KeyCode.F8:
					return IskeyJustDown(Application.Input.f8, Application.OldInput.f8);
				case KeyCode.F9:
					return IskeyJustDown(Application.Input.f9, Application.OldInput.f9);
				case KeyCode.TLDA:
					return IskeyJustDown(Application.Input.tlda, Application.OldInput.tlda);
				case KeyCode.DEL:
					return IskeyJustDown(Application.Input.del, Application.OldInput.del);
				case KeyCode.ESCAPE:
					return IskeyJustDown(Application.Input.escape, Application.OldInput.escape);
				case KeyCode.TAB:
					return IskeyJustDown(Application.Input.tab, Application.OldInput.tab);
				case KeyCode.V:
					return IskeyJustDown(Application.Input.v, Application.OldInput.v);
				case KeyCode.SPACE:
					return IskeyJustDown(Application.Input.space, Application.OldInput.space);
			}
			return false;
        }

		public static bool IsKeyDown(KeyCode keyCode)
        {
			switch (keyCode)
			{
				case KeyCode.CTRL_L:
					return (Application.Input.ctrl);
				case KeyCode.SHIFT_L:
					return (Application.Input.shift);
				case KeyCode.W:
					return (Application.Input.w);
				case KeyCode.A:
					return (Application.Input.a);
				case KeyCode.S:
					return (Application.Input.s);
				case KeyCode.D:
					return (Application.Input.d);
				case KeyCode.Q:
					return (Application.Input.q);
				case KeyCode.E:
					return (Application.Input.e);
				case KeyCode.R:
					return (Application.Input.r);
				case KeyCode.T:
					return (Application.Input.t);
				case KeyCode.Z:
					return (Application.Input.z);
				case KeyCode.F1:
					return (Application.Input.f1);
				case KeyCode.F2:
					return (Application.Input.f2);
				case KeyCode.F3:
					return (Application.Input.f3);
				case KeyCode.F4:
					return (Application.Input.f4);
				case KeyCode.F5:
					return (Application.Input.f5);
				case KeyCode.F6:
					return (Application.Input.f6);
				case KeyCode.F7:
					return (Application.Input.f7);
				case KeyCode.F8:
					return (Application.Input.f8);
				case KeyCode.F9:
					return (Application.Input.f9);
				case KeyCode.TLDA:
					return (Application.Input.tlda);
				case KeyCode.DEL:
					return (Application.Input.del);
				case KeyCode.ESCAPE:
					return (Application.Input.escape);
				case KeyCode.TAB:
					return (Application.Input.tab);
				case KeyCode.C:
					return (Application.Input.c);
				case KeyCode.V:
					return (Application.Input.v);
				case KeyCode.SPACE:
					return (Application.Input.space);

			}
			return false;
		}


		private static bool IskeyJustDown(bool cur, bool old)
        {
			return cur && !old;
        }

		private static bool IskeyJustUp(bool cur, bool old)
		{
			return !cur && old;
		}
	}
}
