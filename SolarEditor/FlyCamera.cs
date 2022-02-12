using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSharp;

namespace SolarEditor
{
    internal class FlyCamera
    {
		private Vector3 position = new Vector3(8, 8, 8);
		private Quaternion orientation = Quaternion.Identity;
		
		private float pitch;
		private float yaw;

		private float far;
		private float near;
		private float yfov;

		internal FlyCamera()
        {
			pitch = 0;
			yaw = -90.0f; 
			far = 100.0f;
			near = 0.1f;
			yfov = Util.DegToRad(45.0f);
			orientation = Matrix4.ToQuaternion(Matrix4.CreateLookAtRH(position, Vector3.Zero, Vector3.UnitY));
        }

		internal void Operate()
        {
			if (Application.GetMouseDown(2))
            {             
				Application.DisableMouse();

				Vector2 delta = Application.GetMouseDelta();
                
				float mouseSensitivity = 100.1f;
				yaw += delta.x * mouseSensitivity;
				pitch -= delta.y * mouseSensitivity;

				pitch = Math.Clamp(pitch, -89.0f, 89.0f);

				Vector3 direction;
				direction.x = MathF.Cos(Util.DegToRad(yaw)) * MathF.Cos(Util.DegToRad(pitch));
				direction.y = MathF.Sin(Util.DegToRad(pitch));
				direction.z = MathF.Sin(Util.DegToRad(yaw)) * MathF.Cos(Util.DegToRad(pitch));

				Matrix4 result = Matrix4.CreateLookAtRH(position, position + direction, Vector3.UnitY);
				orientation = Matrix4.ToQuaternion(result);
			}
			else
            {
				Application.EnableMouse();
            }
        }

		internal Matrix4 GetViewMatrix() { return Matrix4.TranslateRH(Quaternion.ToMatrix4(orientation), position).Inverse; }
		internal Matrix4 GetProjectionMatrix() { return Matrix4.CreatePerspectiveRH(yfov, Application.WindowAspect, near, far); }
    }
}
