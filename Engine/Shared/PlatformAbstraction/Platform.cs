using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.GLFW;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;

using System.Numerics;

namespace Concrete;

public class Platform
{
    public static Platform current = null;

    public GL opengl;
    private IInputContext input;
    private IWindow window;
    private Glfw glfw;

    // only needed for an edge case
    public IWindow GetSilkWindowReference() => window;
    public IInputContext GetSilkInputReference() => input;

    public Platform(Vector2 windowSize, string windowTitle)
    {
        // make this platform the current active one
        current ??= this;

        // make silk prioritize glfw
        GlfwWindowing.Use();

        // create silk window
        var options = WindowOptions.Default;
        options.Size = new((int)windowSize.X, (int)windowSize.Y);
        options.Title = windowTitle;
        window = Window.Create(options);
    }

    public void Run()
    {
        window.Run();
        window.Dispose();
    }

    #region Callbacks

    public void SubscribeStart(Action action)
    {
        window.Load += () =>
        {
            opengl = GL.GetApi(window);
            input = window.CreateInput();
            glfw = Glfw.GetApi();
            action();
        };
    }

    public void SubscribeUpdate(Action<float> action)
    {
        window.Update += (delta) => action((float)delta);
    }

    public void SubscribeRender(Action<float> action)
    {
        window.Render += (delta) => action((float)delta);
    }

    public void SubscribeResize(Action<Vector2> action)
    {
        window.Resize += (size) => action(new Vector2(size.X, size.Y));
    }

    public void SubscribeFileDrop(Action<string[]> action)
    {
        window.FileDrop += (paths) => action(paths);
    }

    #endregion

    #region Input

    public bool IsKeyPressed(PlatformKey platformKey)
    {
        return input.Keyboards[0].IsKeyPressed((Silk.NET.Input.Key)platformKey);
    }

    public bool IsMouseButtonPressed(int button)
    {
        return input.Mice[0].IsButtonPressed((Silk.NET.Input.MouseButton)button);
    }

    public Vector2 GetMousePosition()
    {
        return input.Mice[0].Position;
    }

    #endregion

    #region Other

    public unsafe float GetDisplayScalingFactor()
    {
        float displayScale = 1.0f;

        if (window.Native.Glfw != null)
        {
            var monitor = glfw.GetPrimaryMonitor();
            glfw.GetMonitorContentScale(monitor, out float xscale, out float yscale);
            displayScale = MathF.Max(xscale, yscale);
        }

        return displayScale;
    }

    public Vector2 GetWindowSize()
    {
        var size = new Vector2(window.Size.X, window.Size.Y);
        return size;
    }

    public void SetWindowTitle(string title)
    {
        window.Title = title;
    }

    #endregion
}