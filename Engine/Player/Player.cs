using System.Reflection;
using System.Numerics;
using Silk.NET.OpenGL;

namespace Concrete;

public static class Player
{
    static Platform platform;

    static void Main()
    {
        platform = new Platform(new Vector2(1600, 900), "Concrete Player");

        platform.SubscribeStart(StartWindow);
        platform.SubscribeUpdate(UpdateWindow);
        platform.SubscribeRender(RenderWindow);
        platform.SubscribeResize(ResizeWindow);

        platform.Run();
    }

    static void StartWindow()
    {
        Assembly.LoadFile(Path.GetFullPath("Scripts.dll"));
        ProjectManager.LoadProjectFile("./_Resources/GameData/project.json");
        SceneManager.StartPlaying();
    }

    static void UpdateWindow(float deltaTime)
    {
        Metrics.Update(deltaTime);
        if (SceneManager.playState == PlayState.playing) SceneManager.UpdateSceneObjects(deltaTime);
    }

    static void RenderWindow(float deltaTime)
    {
        platform.opengl.Enable(EnableCap.DepthTest);
        platform.opengl.Enable(EnableCap.Blend);
        platform.opengl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        platform.opengl.ClearColor(Scene.Current.FindCamera().clearColor);
        platform.opengl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        SceneManager.RenderSceneObjects(deltaTime, Scene.Current.FindCamera().view, Scene.Current.FindCamera().proj);
    }

    static void ResizeWindow(Vector2 size)
    {
        platform.opengl.Viewport(new System.Drawing.Size((int)size.X, (int)size.Y));
    }
}