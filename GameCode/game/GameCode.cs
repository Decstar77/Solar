using System;
using System.Collections.Generic;

using SolarSharp;

namespace GameCode
{
    internal class Entity
    {
        protected Vector3 position = Vector3.Zero;
        public Vector3 Position { get { return position; } }

        protected Quaternion orientation = Quaternion.Identity;
        public Quaternion Orientation { get { return orientation; } }

        protected Vector3 scale = new Vector3(1, 1, 1);
        public Vector3 Scale { get { return scale; } }
    }

    internal class Player : Entity
    {
        public Vector3 velocity = Vector3.Zero;

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

            position += velocity ;

        }
    }

    internal class Room
    {
        internal List<Entity> entities = new List<Entity>(); 
        internal Player player = new Player();
        internal Camera camera = new Camera();
    }

    internal class GameCode
    {
        


    }
}