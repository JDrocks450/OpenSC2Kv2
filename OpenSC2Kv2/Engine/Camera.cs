using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.Game.Engine
{
    internal class Camera
    {

        const float ZoomTime = .250f;
        protected float _zoom; // Camera Zoom
        protected float _rotation; //Camera Rotation

        public Matrix Transformation
        {
            get; private set;
        }
        public Vector2 Position
        {
            get => _position;
            set
            {
                CameraMoved = true;
                _position = value;
            }
        }
        public Rectangle Screen
        {
            get => new Rectangle(ScreenToWorld(Position).ToPoint(), Size);
        }
        public Point Size
        {
            get => GameResources.Device.Viewport.Bounds.Size;
        }

        public Rectangle VisibleArea
        {
            get; private set;
        }

        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; CameraMoved = true; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        public bool CameraMoved
        {
            get; private set;
        } = true;

        /// <summary>
        /// Freezes the Camera's Position
        /// </summary>
        public bool Frozen { get; set; } = false;
        public bool IsLoaded { get; set; } = true;

        public Point WorldMousePosition
        {
            get => ScreenToWorld(Mouse.GetState().Position.ToVector2()).ToPoint();
        }
        public string Name { get; internal set; }
        public bool Destroyed { get; set; }

        Point mouseDragStartPos = Point.Zero;
        bool mouseRightPressed = false, _transitioning = false;
        int _mouseLastScroll = 0;
        float desiredZoom = 1, origZoom = 1;
        double _transitionDuration = 0;
        DateTime zoomTime, driftTime;
        private Vector2 _position, _transitionPosition, _transitionStartPosition;

        public Camera()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            Position = Vector2.Zero;
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void TransitionToPosition(Vector2 Location, TimeSpan? Duration = null)
        {
            driftTime = DateTime.Now;
            _transitionPosition = Location;
            _transitionStartPosition = Position;
            _transitioning = true;
            if (Duration == null)
                Duration = TimeSpan.FromSeconds(.15 + (.0002 * Math.Abs((_transitionPosition - _transitionStartPosition).Length())));
            _transitionDuration = Duration.Value.TotalSeconds;
        }

        public void StopTransition(bool JumpToEnd = true)
        {
            if (_transitioning)
            {
                _transitioning = false;
                if (JumpToEnd)
                    Position = _transitionPosition;
            }
        }

        public void SetFocus(Vector2 Location)
        {
            TransitionToPosition(Location);
        }

        public void ClearFocus()
        {
            StopTransition(false);
        }

        public void Update(GameTime time)
        {
            doZoom(time);
            doMouseMove(time);
            if (_transitioning)
                doTransition(time);
            mouseRightPressed = Mouse.GetState().RightButton == ButtonState.Pressed;
            if (CameraMoved)
                refreshMatricies();
            CameraMoved = false;
        }

        private void doTransition(GameTime time)
        {
            var percent = (DateTime.Now - driftTime).TotalSeconds / _transitionDuration;
            Position = Vector2.Lerp(_transitionStartPosition, _transitionPosition, (float)percent);
            if (percent >= 1)
                StopTransition();
            CameraMoved = true;
        }

        private void refreshMatricies()
        {
            Transformation = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                             Matrix.CreateRotationZ(0) *
                             Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                             Matrix.CreateTranslation(new Vector3(Size.X / 2, Size.Y / 2, 0));
            var inverseViewMatrix = Matrix.Invert(Transformation);
            var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            var tr = Vector2.Transform(new Vector2(Screen.Size.X, 0), inverseViewMatrix);
            var bl = Vector2.Transform(new Vector2(0, Screen.Size.Y), inverseViewMatrix);
            var br = Vector2.Transform(Screen.Size.ToVector2(), inverseViewMatrix);
            var min = new Vector2(
                MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
            var max = new Vector2(
                MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
            VisibleArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        private void doMouseMove(GameTime time)
        {
            var pos = Mouse.GetState().Position;
            if (!mouseRightPressed && Mouse.GetState().RightButton == ButtonState.Pressed)
                mouseDragStartPos = pos;
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                Position += ((pos - mouseDragStartPos).ToVector2() / new Vector2(Zoom)) / 15;
                ClearFocus();
            }
            var pt_ = (Mouse.GetState().Position).ToVector2();
            pt_ /= new Vector2(Zoom);
        }

        private void doZoom(GameTime time)
        {
            var scrollchange = (float)(Mouse.GetState().ScrollWheelValue - _mouseLastScroll);
            var zoomChange = scrollchange / (1000 / (scrollchange > 0 ? 1 : Zoom));
            if (zoomChange != 0)
            {
                zoomTime = DateTime.Now;
                desiredZoom += zoomChange;
                origZoom = Zoom;
            }
            _mouseLastScroll = Mouse.GetState().ScrollWheelValue;
            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                zoomTime = DateTime.Now;
                desiredZoom = 1;
                origZoom = Zoom;
                TransitionToPosition(WorldMousePosition.ToVector2());
            }
            var percent = (float)(DateTime.Now - zoomTime).TotalSeconds / ZoomTime;
            if (percent < 1)
                Zoom = MathHelper.Lerp(origZoom, desiredZoom, percent);
        }

        public Vector2 ScreenToWorld(Vector2 Position)
        {
            return Vector2.Transform(Position, Matrix.Invert(Transformation));
        }

        public Vector2 WorldToScreen(Vector2 Position)
        {
            return Vector2.Transform(Position, Transformation);
        }

        public void Dispose()
        {

        }
    }
}
