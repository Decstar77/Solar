using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
    public class Camera
    {
        protected Vector3 position = new Vector3(0, 0, 10);
        protected Quaternion orientation = Quaternion.Identity;
		protected float far = 100.0f;
		protected float near = 0.1f;
		protected float yfov = Util.DegToRad(45.0f);

		protected Vector4 GetNormalisedDeviceCoordinates(float width, float height, float mouseX, float mouseY)
		{
			float x = 2.0f * (mouseX / width) - 1.0f;
			float y = -2.0f * (mouseY / height) + 1.0f;

			return new Vector4(x, y, -1.0f, 1.0f);
		}

		public Ray ShootRayFromMousePos()
		{
			float width = Application.SurfaceWidth;
			float height = Application.SurfaceHeight;

			Vector2 pixelPoint = Application.GetMousePixelPosition();
			Matrix4 proj = GetProjectionMatrix();
			Matrix4 view = GetViewMatrix();

			Vector4 normCoords = GetNormalisedDeviceCoordinates(width, height, pixelPoint.x, pixelPoint.y);
			Vector4 viewCoords = proj.Inverse * normCoords;
			Vector4 worldCoods = view * new Vector4(viewCoords.x, viewCoords.y, -1, 1); // @NOTE: This -1 ensure we a have something pointing in to the screen

			Ray ray = new Ray();
			ray.origin = position;
			ray.direction = Vector3.Normalize(new Vector3(worldCoods.x, worldCoods.y, worldCoods.z));

			return ray;
		}

		public Matrix4 GetViewMatrix() { return Matrix4.TranslateRH(Quaternion.ToMatrix4(orientation), position).Inverse; }
		public Matrix4 GetProjectionMatrix() { return Matrix4.CreatePerspectiveRH(yfov, Application.SurfaceAspect, near, far); }
	}
}
