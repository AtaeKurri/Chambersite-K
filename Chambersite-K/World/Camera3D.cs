using Microsoft.Xna.Framework;

namespace Chambersite_K.World
{
    public class Camera3D
    {
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public Vector3 Up { get; set; }
        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }

        private float aspectRatio;
        private float fieldOfView;
        private float nearPlaneDistance;
        private float farPlaneDistance;

        public Camera3D(Vector3 position, Vector3 target, Vector3 up,
            float fieldOfView=MathHelper.PiOver4, float nearPlaneDistance=0.1f, float farPlaneDistance=1000f)
        {
            Position = position;
            Target = target;
            Up = up;
            this.fieldOfView = fieldOfView;
            this.nearPlaneDistance = nearPlaneDistance;
            this.farPlaneDistance = farPlaneDistance;
            this.aspectRatio = GAME.GraphicsDevice.Viewport.AspectRatio;

            UpdateViewMatrix();
            UpdateProjectionMatrix();
        }

        public void UpdateViewMatrix()
        {
            ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
        }

        public void UpdateProjectionMatrix()
        {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }

        public void UpdateAspectRatio(float aspectRatio)
        {
            this.aspectRatio = aspectRatio;
            UpdateProjectionMatrix();
        }

        public void Move(Vector3 translation)
        {
            Position += translation;
            Target += translation;
            UpdateViewMatrix();
        }

        public void Rotate(float yaw, float pitch, float roll)
        {
            Matrix rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            Position = Vector3.Transform(Position - Target, rotation) + Target;
            Up = Vector3.Transform(Up, rotation);
            UpdateViewMatrix();
        }
    }
}
