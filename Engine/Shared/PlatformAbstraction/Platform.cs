using Silk.NET.OpenGL;
using Hexa.NET.SDL3;
using System.Numerics;

namespace Concrete;

public unsafe class Platform
{
    public static Platform current = null;

    public GL opengl;

    public SDLWindow* window;
    public SDLGLContext glContext;

    // callbacks
    private Action startCallback;
    private Action<float> updateCallback;
    private Action<float> renderCallback;
    private Action<Vector2> resizeCallback;
    private Action<string[]> fileDropCallback;

    // sdl specific
    private Action<nint> SDLEventCallbacks;

    // Input state
    private bool[] keyStates = new bool[512];
    private bool[] mouseButtonStates = new bool[8];
    private Vector2 mousePosition;

    private string[] pendingDroppedFiles = null;

    // Timing
    private ulong lastTime;
    private bool isRunning;

    public Platform(Vector2 windowSize, string windowTitle)
    {
        // make this platform the current active one
        current ??= this;

        // Initialize SDL
        SDL.Init(SDLInitFlags.Video | SDLInitFlags.Events);

        // Set OpenGL attributes
        SDL.GLSetAttribute(SDLGLAttr.ContextMajorVersion, 3);
        SDL.GLSetAttribute(SDLGLAttr.ContextMinorVersion, 3);
        SDL.GLSetAttribute(SDLGLAttr.ContextProfileMask, SDL.SDL_GL_CONTEXT_PROFILE_CORE);
        SDL.GLSetAttribute(SDLGLAttr.Doublebuffer, 1);

        // Create window
        window = SDL.CreateWindow(
            windowTitle,
            (int)windowSize.X,
            (int)windowSize.Y,
            SDLWindowFlags.Opengl | SDLWindowFlags.Resizable
        );

        // Create OpenGL context
        glContext = SDL.GLCreateContext(window);
        SDL.GLMakeCurrent(window, glContext);
        SDL.GLSetSwapInterval(1);

        lastTime = SDL.GetPerformanceCounter();
    }

    public void Run()
    {
        // Call start callback
        opengl = GL.GetApi((name) => (nint)SDL.GLGetProcAddress(name));
        startCallback?.Invoke();

        isRunning = true;

        while (isRunning)
        {
            // Calculate delta time
            ulong currentTime = SDL.GetPerformanceCounter();
            float deltaTime = (currentTime - lastTime) / (float)SDL.GetPerformanceFrequency();
            lastTime = currentTime;

            // Process events
            ProcessSDLEvents();

            // Update
            updateCallback?.Invoke(deltaTime);

            // Render
            renderCallback?.Invoke(deltaTime);

            // File Drop
            if (pendingDroppedFiles != null)
            {
                fileDropCallback?.Invoke(pendingDroppedFiles);
                pendingDroppedFiles = null;
            }

            // Swap buffers
            SDL.GLSwapWindow(window);
        }

        // Cleanup
        SDL.GLDestroyContext(glContext);
        SDL.DestroyWindow(window);
        SDL.Quit();
    }

    private void ProcessSDLEvents()
    {
        var MapSDLMouseButton = (byte sdlButton) =>
        {
            if (sdlButton == 1) return 0;
            if (sdlButton == 3) return 1;
            if (sdlButton == 2) return 2;
            else return -1;
        };

        SDLEvent sdlEvent;
        while (SDL.PollEvent(&sdlEvent) != false)
        {
            SDLEventCallbacks?.Invoke((nint)(&sdlEvent));

            var eventType = (SDLEventType)sdlEvent.Type;

            if (eventType == SDLEventType.Quit)
            {
                isRunning = false;
            }
            else if (eventType == SDLEventType.WindowResized)
            {
                int width, height;
                SDL.GetWindowSize(window, &width, &height);
                resizeCallback?.Invoke(new Vector2(width, height));
            }
            else if (eventType == SDLEventType.KeyDown)
            {
                if ((int)sdlEvent.Key.Scancode < keyStates.Length)
                {
                    keyStates[(int)sdlEvent.Key.Scancode] = true;
                }
            }
            else if (eventType == SDLEventType.KeyUp)
            {
                if ((int)sdlEvent.Key.Scancode < keyStates.Length)
                {
                    keyStates[(int)sdlEvent.Key.Scancode] = false;
                }
            }
            else if (eventType == SDLEventType.MouseButtonDown)
            {
                int mappedButton = MapSDLMouseButton(sdlEvent.Button.Button);
                if (mappedButton >= 0 && mappedButton < mouseButtonStates.Length)
                {
                    mouseButtonStates[mappedButton] = true;
                }
            }
            else if (eventType == SDLEventType.MouseButtonUp)
            {
                int mappedButton = MapSDLMouseButton(sdlEvent.Button.Button);
                if (mappedButton >= 0 && mappedButton < mouseButtonStates.Length)
                {
                    mouseButtonStates[mappedButton] = false;
                }
            }
            else if (eventType == SDLEventType.MouseMotion)
            {
                mousePosition = new Vector2(sdlEvent.Motion.X, sdlEvent.Motion.Y);
            }
            else if (eventType == SDLEventType.DropFile)
            {
                if (fileDropCallback != null)
                {
                    var path = new String((sbyte*)sdlEvent.Drop.Data);
                    pendingDroppedFiles = [path];
                }
            }
        }
    }

    #region Callbacks

    public void SubscribeStart(Action action)
    {
        startCallback = action;
    }

    public void SubscribeUpdate(Action<float> action)
    {
        updateCallback = action;
    }

    public void SubscribeRender(Action<float> action)
    {
        renderCallback = action;
    }

    public void SubscribeResize(Action<Vector2> action)
    {
        resizeCallback = action;
    }

    public void SubscribeFileDrop(Action<string[]> action)
    {
        fileDropCallback = action;
    }

    public void SubscribeExtraSDLEvent(Action<nint> action) // this is sdl specific, platform doesnt require this
    {
        SDLEventCallbacks += action;
    }

    #endregion

    #region Input

    public bool IsKeyPressed(PlatformKey platformKey)
    {
        var sdlKey = PlatformKeyToNative.MapToSDLKey(platformKey);
        return sdlKey < keyStates.Length && keyStates[sdlKey];
    }

    public bool IsMouseButtonPressed(int button)
    {
        return button >= 0 && button < mouseButtonStates.Length && mouseButtonStates[button];
    }

    public Vector2 GetMousePosition()
    {
        return mousePosition;
    }

    #endregion

    #region Other

    public float GetDisplayScalingFactor()
    {
        uint displayId = SDL.GetPrimaryDisplay();
        float scale = SDL.GetDisplayContentScale(displayId);
        return scale;
    }

    public Vector2 GetWindowSize()
    {
        int width, height;
        SDL.GetWindowSize(window, &width, &height);
        return new Vector2(width, height);
    }

    public void SetWindowTitle(string title)
    {
        SDL.SetWindowTitle(window, title);
    }

    #endregion
}