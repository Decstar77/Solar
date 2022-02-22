using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.GameLogic
{
    public class Entity
    {
        protected Vector3 position = Vector3.Zero;
        public Vector3 Position { get { return position; } set { position = value; } }

        protected Quaternion orientation = Quaternion.Identity;
        public Quaternion Orientation { get { return orientation; } }

        protected Vector3 scale = new Vector3(1, 1, 1);
        public Vector3 Scale { get { return scale; } }

        public Matrix4 ComputeModelMatrix()
        {
            Matrix4 translation = Matrix4.TranslateRH(Matrix4.Identity, position);
            Matrix4 rotation = Quaternion.ToMatrix4(orientation);
            Matrix4 scaling = Matrix4.ScaleCardinal(Matrix4.Identity, scale);

            Matrix4 transform = translation * rotation * scaling;

            return transform;            
        }
    }

    public class Player : Entity
    {
        public Vector3 velocity = Vector3.Zero;
        public Camera camera;

    
        public void Operate()
        {
            float dt = Application.GetDeltaTime();

            Basis basis = Quaternion.ToBasis(orientation);
            basis.forward.y = 0;
            basis.right.y = 0;

            Vector3 forward = Vector3.Normalize(basis.forward);
            Vector3 right = Vector3.Normalize(basis.right);

            float accl = 2.0f;
            Vector3 acceleration = Vector3.Zero;
            acceleration += Application.IsKeyDown('A') ? Vector3.Zero : right;
            acceleration -= Application.IsKeyDown('D') ? Vector3.Zero : right;
            acceleration -= Application.IsKeyDown('S') ? Vector3.Zero : forward;
            acceleration += Application.IsKeyDown('W') ? Vector3.Zero : forward;

            velocity += acceleration != Vector3.Zero ? Vector3.Normalize(acceleration) * accl * dt : Vector3.Zero;

            float maxSpeed = 1.0f;
            if (velocity.Mag > maxSpeed)
            {
                velocity = Vector3.Normalize(velocity) * maxSpeed;
            }

            velocity *= 0.87f;

            position += velocity;

          
            Ray ray = camera.ShootRayFromMousePos();
            Plane plane = new Plane(Vector3.Zero, Vector3.UnitY);

            RaycastInfo info;
            if (Raycast.Plane(ray, plane, out info))
            {
                Matrix4 m=  Quaternion.ToMatrix4(orientation);
                orientation=  Matrix4.ToQuaternion(Matrix4.CreateLookAtRH(position, info.point, Vector3.UnitY));
            }
                     
        }
    }

    public class Room
    {
        private string name = "Untitled";
        public string Name { get { return name; } set { name = value; } }

        public List<Entity> entities = new List<Entity>();
        public Player player = new Player();
        public Camera camera = new Camera();
    }

}
