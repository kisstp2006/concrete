using System.Drawing;
using System.Numerics;

using Hexa.NET.ImGui;

using Silk.NET.OpenGL;

namespace Concrete;

public static unsafe class Editor
{
    public static ImGuiController igcontroller;
    static Platform platform;

    static void Main()
    {
        platform = new Platform(new Vector2(1600, 900), "Concrete Player");

        platform.SubscribeStart(StartWindow);
        platform.SubscribeUpdate(UpdateWindow);
        platform.SubscribeRender(RenderWindow);
        platform.SubscribeResize(ResizeWindow);
        platform.SubscribeFileDrop(FileDrop);

        platform.Run();
    }

    static void StartWindow()
    {
        // setup imgui controller and styling
        igcontroller = new ImGuiController(platform.opengl, platform.GetSilkWindowReference(), platform.GetSilkInputReference());
        EditorStyleChanger.ClearFonts();
        EditorStyleChanger.AddFont(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_Resources", "cascadia.ttf"), 14);
        EditorStyleChanger.AddFont(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_Resources", "fontawesome_free_solid.otf"), 14, true);
        EditorStyleChanger.SetupCustomTheme();

        ImGui.GetStyle().FontScaleDpi = platform.GetDisplayScalingFactor();
        
        ProjectManager.TryLoadLastProjectOrCreateTempProject();
    }

    static void UpdateWindow(float deltaTime)
    {
        Metrics.Update(deltaTime);
        if (SceneManager.playState == PlayState.playing) SceneManager.UpdateSceneObjects(deltaTime);
        igcontroller.Update(deltaTime);
    }

    static void RenderWindow(float deltaTime)
    {
        platform.opengl.Enable(EnableCap.DepthTest);
        platform.opengl.Enable(EnableCap.Blend);
        platform.opengl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
        platform.opengl.ClearColor(Color.Black);
        platform.opengl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        Render(deltaTime);
        igcontroller.Render();
    }

    static void ResizeWindow(Vector2 size)
    {
        platform.opengl.Viewport(new Size((int)size.X, (int)size.Y));
    }

    static void FileDrop(string[] paths)
    {
        if (!FilesWindow.hovered) return;
        foreach (var path in paths)
        {
            // calculate destination parent
            var parent = ProjectManager.projectRoot;
            var hovered = FilesWindow.hoveredFileOrDir;
            if (hovered != null)
            {
                if (Directory.Exists(hovered)) parent = Path.GetFullPath(hovered);
                if (File.Exists(hovered)) parent = Path.GetDirectoryName(Path.GetFullPath(hovered));
            }

            if (Directory.Exists(path))
            {
                // move dir to project dir
                var dirname = Path.GetFileName(path);
                var destination = Path.Combine(parent, dirname);
                Directory.Move(path, destination);
                
                // rebuild asset database
                AssetDatabase.Rebuild();
            }
            else if (File.Exists(path))
            {
                // move file to project dir
                var filename = Path.GetFileName(path);
                var destination = Path.Combine(parent, filename);
                Directory.Move(path, destination);
                
                // rebuild asset database
                AssetDatabase.Rebuild();
            }
        }
    }

    private static bool dockbuilderInitialized = false;

    public static void Render(float deltaTime)
    {
        SetupDockSpace();
        MainMenuBar.Draw(deltaTime);
        SceneWindow.Draw(deltaTime);
        GameWindow.Draw(deltaTime);
        HierarchyWindow.Draw(deltaTime);
        FilesWindow.Draw(deltaTime);
        ConsoleWindow.Draw(deltaTime);
        InspectorWindow.Draw(deltaTime);
        MetricsWindow.Draw(deltaTime);
        BuildWindow.Draw(deltaTime);
    }

    private static void SetupDockSpace()
    {
        uint dockspace = ImGui.DockSpaceOverViewport((ImGuiDockNodeFlags)ImGuiDockNodeFlagsPrivate.NoWindowMenuButton);
        if (!dockbuilderInitialized)
        {
            uint left, mid, right;
            uint topleft, lowleft;
            uint topmid, lowmid;

            ImGuiP.DockBuilderSplitNode(dockspace, ImGuiDir.Left, 0.25f, &left, &mid);
            ImGuiP.DockBuilderSplitNode(mid, ImGuiDir.Left, 0.66f, &mid, &right);
            ImGuiP.DockBuilderSplitNode(left, ImGuiDir.Up, 0.5f, &topleft, &lowleft);
            ImGuiP.DockBuilderSplitNode(mid, ImGuiDir.Down, 0.3f, &lowmid, &topmid);

            ImGuiP.DockBuilderDockWindow("\uf009 Scene", topmid);
            ImGuiP.DockBuilderDockWindow("\uf11b Game", topmid);
            ImGuiP.DockBuilderDockWindow("\uf552 Build", topmid);
            ImGuiP.DockBuilderDockWindow("\ue473 Metrics", topmid);
            ImGuiP.DockBuilderDockWindow("\uf0ca Hierarchy", topleft);
            ImGuiP.DockBuilderDockWindow("\uf07c Files", lowleft);
            ImGuiP.DockBuilderDockWindow("\uf002 Inspector", right);
            ImGuiP.DockBuilderDockWindow("\uf120 Console", lowmid);

            ImGuiP.DockBuilderFinish(dockspace);
            dockbuilderInitialized = true;
        }
    }
}