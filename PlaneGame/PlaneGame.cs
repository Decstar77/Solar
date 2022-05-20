
using SolarSharp;
using SolarSharp.Core;

namespace PlaneGame
{
    public class AirGame : Game
    {
        Camera camera = new Camera();
        Entity planeEntity;

        public bool Start(GameScene scene)
        {
            scene.Camera = camera;

            camera.Far = 150.0f;
            camera.Near = 50.0f;

            planeEntity = scene.CreateEntity();
            planeEntity.Position = new Vector3(0, 35, 0);

            //Guid guid = Guid.NewGuid();
            //Guid.TryParse("2ce77668-e7d6-428d-b762-ee998d1e7709", out guid);
            //planeEntity.RenderingState.ModelId = guid;

            camera.Position = planeEntity.Position + new Vector3(0, 55, -15) ;
            camera.LookAt(planeEntity.Position );

            return true; 
        }

        public void FrameUpdate(GameScene scene)
        {
            float dt = Application.DeltaTime;
            Vector3 forwardDir = planeEntity.GetForward();

            float planeSpeed = 10.0f;

            if (Input.IsKeyDown(KeyCode.SPACE))
            {
                planeSpeed *= 2.0f;
            }

            planeEntity.Position += forwardDir * dt * planeSpeed;

            camera.Position = planeEntity.Position + new Vector3(0, 55, -25);
            camera.LookAt(planeEntity.Position);


            if (Input.IsKeyDown(KeyCode.A))
            {
                planeEntity.Orientation = Quaternion.RotateLocalY(planeEntity.Orientation, Util.DegToRad(90.0f * dt));
            }

            if (Input.IsKeyDown(KeyCode.D))
            {
                planeEntity.Orientation = Quaternion.RotateLocalY(planeEntity.Orientation, Util.DegToRad(-90.0f * dt));
            }


        }

        public void TickUpdate(GameScene scene)
        {
        }

        public void Shutdown(GameScene scene)
        {
            scene.DestroyEntity(planeEntity.Reference);   
        }
    }

}