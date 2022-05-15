using SolarSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			Position = new Vector3(0, 0, 5);
			far = 250.0f;
		}

		internal bool Operate()
		{
			float mouseSensitivity = 0.1f;
			float flySpeed = 20.0f;
			if (Input.IsMouseDown(MouseButton.MOUSE2))
			{
				Input.DisableMouse();

				Vector2 delta = Input.MouseDelta;

				yaw += delta.x * mouseSensitivity;
				pitch -= delta.y * mouseSensitivity;

				pitch = Math.Clamp(pitch, -89.0f, 89.0f);

				Vector3 direction;
				direction.x = MathF.Cos(Util.DegToRad(yaw)) * MathF.Cos(Util.DegToRad(pitch));
				direction.y = MathF.Sin(Util.DegToRad(pitch));
				direction.z = MathF.Sin(Util.DegToRad(yaw)) * MathF.Cos(Util.DegToRad(pitch));

				Matrix4 result = Matrix4.CreateLookAtRH(Position,Position + direction, Vector3.UnitY);
				Orientation = Matrix4.ToQuaternion(result);

				Basis basis = Quaternion.ToBasis(Orientation);
				Vector3 move = Vector3.Zero;

				if (Input.IsKeyDown(KeyCode.W)) { move -= basis.forward; } // @NOTE: Right handed so '-' is comming 'out'
				if (Input.IsKeyDown(KeyCode.S)) { move += basis.forward; }
				if (Input.IsKeyDown(KeyCode.D)) { move += basis.right; }
				if (Input.IsKeyDown(KeyCode.A)) { move -= basis.right; }
				if (Input.IsKeyDown(KeyCode.Q)) { move += Vector3.UnitY; }
				if (Input.IsKeyDown(KeyCode.E)) { move -= Vector3.UnitY; }

				// @TODO: Use epsilon damnit !!
				if (move.MagSqrd > 0.01f)
				{
					move = Vector3.Normalize(move) * flySpeed * Application.DeltaTime;
					Position += move;
				}

				return true;
			}
			else
			{
				Input.EnableMouse();
			}

			return false;
		}

	}
}
