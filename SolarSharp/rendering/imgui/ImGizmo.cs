using SolarSharp.EngineAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering
{
	public enum ImGizmoOperation
	{
		TRANSLATE = 0,
		ROTATE,
		SCALE,
		BOUNDS,
	};

	public enum ImGizmoMode
	{
		LOCAL = 0,
		WORLD
	};

	public class ImGizmo
    {
        const string DLLName = "SolarWindows";
        public static void Enable(bool enable) 
			=> ImGizmoAPI.GizmoEnable(enable);
        public static void SetRect(float x, float y, float width, float height) 
			=> ImGizmoAPI.GizmoSetRect(x, y, width, height);
        public static bool Manipulate(Matrix4 proj, Matrix4 view, ref Matrix4 world, ImGizmoOperation operation, ImGizmoMode mode) 
			=> ImGizmoAPI.GizmoManipulate(proj,view, ref world, (int)operation, (int)mode);
    }
}
