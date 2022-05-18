using SolarSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
	public class Camera
	{
		public Vector3 Position { get; set; }
		public Quaternion Orientation { get { return orientation; } set { orientation = value; } }
		private Quaternion orientation = Quaternion.Identity;

		protected float far = 100.0f;
		public float Far { get { return far; } set { far = value; } }

		protected float near = 0.1f;
		public float Near { get { return near; } set { near = value; } }

		protected float yfov = Util.DegToRad(45.0f);
		public float YFovDegrees { get { return Util.RadToDeg(yfov); } set { yfov = Util.DegToRad(value); } }
		public float YFovRadians { get { return yfov; } set { yfov = value; } }

		protected Vector4 GetNormalisedDeviceCoordinates(float width, float height, float mouseX, float mouseY)
		{
			float x = 2.0f * (mouseX / width) - 1.0f;
			float y = -2.0f * (mouseY / height) + 1.0f;

			return new Vector4(x, y, -1.0f, 1.0f);
		}

        public Ray ShootRayFromMousePos()
        {
            float width = Window.SurfaceWidth;
            float height = Window.SurfaceHeight;

			Vector2 pixelPoint = Input.MousePositionPixelCoords;
            Matrix4 proj = GetProjectionMatrix().Inverse;
            Matrix4 view = GetViewMatrix().Inverse;

            Vector4 normCoords = GetNormalisedDeviceCoordinates(width, height, pixelPoint.x, pixelPoint.y);
            Vector4 viewCoords = proj * normCoords;
            Vector4 worldCoods = view * new Vector4(viewCoords.x, viewCoords.y, -1, 0); // @NOTE: This -1 ensure we a have something pointing in to the screen

            Ray ray = new Ray();
            ray.origin = Position;
            ray.direction = Vector3.Normalize(new Vector3(worldCoods.x, worldCoods.y, worldCoods.z));

            return ray;
        }

		public void LookAt(Vector3 worldPoint)
        {
			Matrix4 matrix = Matrix4.CreateLookAtRH(Position, worldPoint, Vector3.UnitY);
			orientation = Matrix4.ToQuaternion(matrix);
        }

        public Matrix4 GetViewMatrix() { return Matrix4.TranslateRH(Quaternion.ToMatrix4(Orientation), Position).Inverse; }
		public Matrix4 GetProjectionMatrix() { return Matrix4.CreatePerspectiveRH(yfov, Window.WindowAspect, near, far); }
	}
}
