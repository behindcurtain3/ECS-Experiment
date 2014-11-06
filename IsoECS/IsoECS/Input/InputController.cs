using System;
using System.Collections.Generic;
using IsoECS.Systems;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;

namespace IsoECS.Input
{
    public class InputEventArgs : System.EventArgs
    {
        public bool Handled { get; set; }
        public InputController Input { get; set; }

        public InputEventArgs(InputController input, bool handled = false)
        {
            Input = input;
            Handled = handled;
        }
    }

    public class KeyListener
    {
        public bool Continuous { get; set; }
        public Keys Primary { get; set; }
        public Keys Alternate { get; set; }
        public event InputController.KeyboardEventHandler Event;

        public KeyListener(bool continuous = false)
        {
            Continuous = continuous;
            Alternate = Keys.None;
        }

        public void Trigger(Keys key, InputController input)
        {
            if (Event != null)
                Event.Invoke(key, new InputEventArgs(input));
        }
    }

    [Serializable]
    public class InputController : GameSystem
    {
        #region Events

        public delegate void MouseEventHandler(InputEventArgs e);
        public delegate void KeyboardEventHandler(Keys key, InputEventArgs e);

        public event MouseEventHandler LeftClick;
        public event MouseEventHandler RightClick;
        public event MouseEventHandler LeftButtonDown;
        public event MouseEventHandler RightButtonDown;
        
        #endregion

        #region Fields

        private List<KeyListener> _listeners;

        #endregion

        #region Properties

        public KeyboardState PrevKeyboard { get; set; }
        public KeyboardState CurrentKeyboard { get; set; }
        public MouseState PrevMouse { get; set; }
        public MouseState CurrentMouse { get; set; }

        // camera listeners
        public KeyListener CameraUpListener { get; private set; }
        public KeyListener CameraDownListener { get; private set; }
        public KeyListener CameraLeftListener { get; private set; }
        public KeyListener CameraRightListener { get; private set; }

        // mode listeners
        public KeyListener ConstructionMode { get; private set; }

        // construction hotkeys
        public KeyListener BuildBack { get; private set; }
        public KeyListener BuildHotKey1 { get; private set; }
        public KeyListener BuildHotKey2 { get; private set; }
        public KeyListener BuildHotKey3 { get; private set; }
        public KeyListener BuildHotKey4 { get; private set; }
        public KeyListener BuildHotKey5 { get; private set; }
        public KeyListener BuildHotKey6 { get; private set; }
        public KeyListener BuildHotKey7 { get; private set; }
        public KeyListener BuildHotKey8 { get; private set; }
        public KeyListener BuildHotKey9 { get; private set; }
        public KeyListener BuildHotKey0 { get; private set; }

        // overlays
        public KeyListener DesireabilityOverlay;

        #endregion

        #region Methods

        public override void Init()
        {
            base.Init();

            _listeners = new List<KeyListener>();

            CameraUpListener = new KeyListener(true)
            {
                Primary = Keys.W,
                Alternate = Keys.Up
            };
            _listeners.Add(CameraUpListener);

            CameraDownListener = new KeyListener(true)
            {
                Primary = Keys.S,
                Alternate = Keys.Down
            };
            _listeners.Add(CameraDownListener);

            CameraLeftListener = new KeyListener(true)
            {
                Primary = Keys.A,
                Alternate = Keys.Left
            };
            _listeners.Add(CameraLeftListener);

            CameraRightListener = new KeyListener(true)
            {
                Primary = Keys.D,
                Alternate = Keys.Right
            };
            _listeners.Add(CameraRightListener);

            ConstructionMode = new KeyListener()
            {
                Primary = Keys.B
            };
            _listeners.Add(ConstructionMode);

            DesireabilityOverlay = new KeyListener()
            {
                Primary = Keys.Q
            };
            _listeners.Add(DesireabilityOverlay);

            BuildHotKey0 = new KeyListener() { Primary = Keys.D0 };
            BuildHotKey1 = new KeyListener() { Primary = Keys.D1 };
            BuildHotKey2 = new KeyListener() { Primary = Keys.D2 };
            BuildHotKey3 = new KeyListener() { Primary = Keys.D3 };
            BuildHotKey4 = new KeyListener() { Primary = Keys.D4 };
            BuildHotKey5 = new KeyListener() { Primary = Keys.D5 };
            BuildHotKey6 = new KeyListener() { Primary = Keys.D6 };
            BuildHotKey7 = new KeyListener() { Primary = Keys.D7 };
            BuildHotKey8 = new KeyListener() { Primary = Keys.D8 };
            BuildHotKey9 = new KeyListener() { Primary = Keys.D9 };
            BuildBack = new KeyListener() { Primary = Keys.Escape };
            _listeners.Add(BuildHotKey0);
            _listeners.Add(BuildHotKey1);
            _listeners.Add(BuildHotKey2);
            _listeners.Add(BuildHotKey3);
            _listeners.Add(BuildHotKey4);
            _listeners.Add(BuildHotKey5);
            _listeners.Add(BuildHotKey6);
            _listeners.Add(BuildHotKey7);
            _listeners.Add(BuildHotKey8);
            _listeners.Add(BuildHotKey9);
            _listeners.Add(BuildBack);
        }

        public override void Update(double dt)
        {
            PrevKeyboard = CurrentKeyboard;
            CurrentKeyboard = Keyboard.GetState();

            PrevMouse = CurrentMouse;
            CurrentMouse = Mouse.GetState();

            bool isOverUI = IsOverUI();

            if (CurrentMouse.LeftButton == ButtonState.Pressed)
                if (LeftButtonDown != null && !isOverUI)
                    LeftButtonDown.Invoke(new InputEventArgs(this));

            if (CurrentMouse.RightButton == ButtonState.Pressed)
                if (RightButtonDown != null && !isOverUI)
                    RightButtonDown.Invoke(new InputEventArgs(this));

            if (CurrentMouse.LeftButton == ButtonState.Pressed && PrevMouse.LeftButton == ButtonState.Released)
                if (LeftClick != null && !isOverUI)
                    LeftClick.Invoke(new InputEventArgs(this));

            if (CurrentMouse.RightButton == ButtonState.Pressed && PrevMouse.RightButton == ButtonState.Released)
                if (RightClick != null && !isOverUI)
                    RightClick.Invoke(new InputEventArgs(this));

            foreach (KeyListener listener in _listeners)
            {
                if (CurrentKeyboard.IsKeyDown(listener.Primary) && (listener.Continuous || !PrevKeyboard.IsKeyDown(listener.Primary)))
                {
                    listener.Trigger(listener.Primary, this);
                }
                else if (CurrentKeyboard.IsKeyDown(listener.Alternate) && (listener.Continuous || !PrevKeyboard.IsKeyDown(listener.Alternate)))
                {
                    listener.Trigger(listener.Alternate, this);
                }
            }
        }

        private bool IsOverUI()
        {
            foreach (Control c in World.UI.Controls)
            {
                if (c.Passive || !c.Visible)
                    continue;

                if (c.ControlRect.Contains((int)CurrentMouse.X, (int)CurrentMouse.Y))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
