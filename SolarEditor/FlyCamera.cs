using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSharp;

namespace SolarEditor
{
    internal class FlyCamera : Camera
    {
		private float pitch;
		private float yaw;

		internal FlyCamera()
        {
			pitch = 0;
			yaw = -90.0f; 
        }

		internal bool Operate()
        {
			float mouseSensitivity = 100.1f;
			float flySpeed = 10.0f;
			if (Application.IsMouseDown(2))
            {             
				Application.DisableMouse();

				Vector2 delta = Application.GetMouseDelta();
                
				yaw += delta.x * mouseSensitivity;
				pitch -= delta.y * mouseSensitivity;

				pitch = Math.Clamp(pitch, -89.0f, 89.0f);

				Vector3 direction;
				direction.x = MathF.Cos(Util.DegToRad(yaw)) * MathF.Cos(Util.DegToRad(pitch));
				direction.y = MathF.Sin(Util.DegToRad(pitch));
				direction.z = MathF.Sin(Util.DegToRad(yaw)) * MathF.Cos(Util.DegToRad(pitch));

				Matrix4 result = Matrix4.CreateLookAtRH(position, position + direction, Vector3.UnitY);
				orientation = Matrix4.ToQuaternion(result);


				Basis basis = Quaternion.ToBasis(orientation);
				Vector3 move = Vector3.Zero;

				if (Application.IsKeyDown('W')) { move -= basis.forward; } // @NOTE: Right handed so '-' is comming 'out'
				if (Application.IsKeyDown('S')) { move += basis.forward; }
				if (Application.IsKeyDown('D')) { move += basis.right; }
				if (Application.IsKeyDown('A')) { move -= basis.right; }
				if (Application.IsKeyDown('Q')) { move += Vector3.UnitY; }
				if (Application.IsKeyDown('E')) { move -= Vector3.UnitY; }

				// @TODO: Use epsilon damnit !!
				if (move.MagSqrd > 0.01f)
                {
					move = Vector3.Normalize(move) * flySpeed * Application.GetDeltaTime();
					position += move;
				}

				return true;
			}
			else
            {
				Application.EnableMouse();
            }

			return false;
        }

    }
}
