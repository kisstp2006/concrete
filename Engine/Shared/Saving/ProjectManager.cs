namespace Concrete;

public static class ProjectManager
{
    public static string loadedProjectFilePath = null;
    public static ProjectData loadedProjectData = null;

    public static string projectRoot => Directory.GetParent(Path.GetFullPath(loadedProjectFilePath)).FullName;

    public static string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public static string concreteDataPath = Path.Combine(documentsPath, "Concrete");
    public static string lastProjectMemoryPath = Path.Combine(concreteDataPath, "LastProject.txt");
    public static string tempProjectPath = Path.Combine(concreteDataPath, "TempProject");

    public static void TryLoadLastProjectOrCreateTempProject()
    {
        if (File.Exists(lastProjectMemoryPath))
        {
            Debug.Log("Last project memory file found.");
            
            string lastOpenedProjectPath = File.ReadAllText(lastProjectMemoryPath);
            
            if (File.Exists(lastOpenedProjectPath))
            {
                Debug.Log("Last project memory file contained a valid project.");
                LoadProjectFile(lastOpenedProjectPath);
            }
            else
            {
                Debug.Log("Last project memory file did not contain a valid project.");
                File.Delete(lastProjectMemoryPath);
                Debug.Log("Invalid last project memory file deleted.");
                CreateAndLoadTempProject();
            }
        }
        else
        {
            Debug.Log("No last project memory file found.");
            CreateAndLoadTempProject();
        }
    }

    public static void CreateAndLoadTempProject()
    {
        Debug.Log("Creating and loading a temporary project.");

        // make empty temp project directory
        if (Directory.Exists(tempProjectPath)) Directory.Delete(tempProjectPath, true);
        Directory.CreateDirectory(tempProjectPath);

        // create and load and remember temp project
        string projectfilepath = Path.Combine(tempProjectPath, "project.json");
        ProjectSerializer.NewProjectFile(projectfilepath);
        LoadProjectFile(projectfilepath, true);
        Directory.CreateDirectory(Path.Combine(tempProjectPath, "Scenes"));
        Directory.CreateDirectory(Path.Combine(tempProjectPath, "Scripts"));
    }

    // ----

    public static void SaveProjectFile(string path)
    {
        ProjectSerializer.SaveProjectFile(path, loadedProjectData);
    }

    public static void LoadProjectFile(string path, bool isTemp = false)
    {
        // load project
        loadedProjectFilePath = path;
        loadedProjectData = ProjectSerializer.LoadProjectFile(path);
        NativeWindow.window.Title = "Concrete Engine [" + Path.GetFullPath(loadedProjectFilePath) + "]";

        // initialize asset database
        AssetDatabase.Rebuild();

        // try to load startup scene
        if (loadedProjectData.firstScene != "")
        {
            string sceneRelativePath = AssetDatabase.GetPath(Guid.Parse(loadedProjectData.firstScene));
            string sceneFullPath = Path.Combine(projectRoot, sceneRelativePath);
            SceneManager.LoadScene(sceneFullPath);
        }
        else
        {
            SceneManager.CreateAndLoadNewScene();
        }
        
        if (!isTemp)
        {
            // remember project
            if (File.Exists(lastProjectMemoryPath)) File.Delete(lastProjectMemoryPath);
            File.WriteAllText(lastProjectMemoryPath, path);
            Debug.Log("Remembered the newly loaded project.");
        }

        AfterProjectLoad(Path.GetDirectoryName(path));
    }

    // ----

    public static void NewProjectDir(string dir)
    {
        var filepath = Path.Combine(dir, "project.json");
        ProjectSerializer.NewProjectFile(filepath);
        LoadProjectFile(filepath);
        Directory.CreateDirectory("Scenes");
        Directory.CreateDirectory("Scripts");
    }

    public static void SaveProjectDir(string dir)
    {
        if (dir != projectRoot)
        {
            CopyDirectory(projectRoot, dir);
            LoadProjectDir(dir);
        }
        else
        {
            Debug.Log("Project is already up to date.");
        }
    }

    public static void LoadProjectDir(string dir, bool isTemp = false)
    {
        var filepath = Path.Combine(dir, "project.json");
        if (!File.Exists(filepath)) File.Create(filepath);

        // load project
        loadedProjectFilePath = filepath;
        loadedProjectData = ProjectSerializer.LoadProjectFile(filepath);
        NativeWindow.window.Title = "Concrete Engine [" + Path.GetFullPath(loadedProjectFilePath) + "]";

        // initialize asset database
        AssetDatabase.Rebuild();

        // try to load startup scene
        if (loadedProjectData.firstScene != "")
        {
            string sceneRelativePath = AssetDatabase.GetPath(Guid.Parse(loadedProjectData.firstScene));
            string sceneFullPath = Path.Combine(projectRoot, sceneRelativePath);
            SceneManager.LoadScene(sceneFullPath);
        }
        else
        {
            SceneManager.CreateAndLoadNewScene();
        }
        
        if (!isTemp)
        {
            // remember project
            if (File.Exists(lastProjectMemoryPath)) File.Delete(lastProjectMemoryPath);
            File.WriteAllText(lastProjectMemoryPath, filepath);
            Debug.Log("Remembered the newly loaded project.");
        }

        // rebuild shared ref for scripts
        AfterProjectLoad(dir);

        // make sure gitignore exists

    }

    private static void CopyDirectory(string source, string dest)
    {
        // ensure existence
        if (!Directory.Exists(source)) throw new DirectoryNotFoundException($"directory not found: {source}");

        // create destination
        Directory.CreateDirectory(dest);

        // copy files
        foreach (string file in Directory.GetFiles(source))
        {
            string destFile = Path.Combine(dest, Path.GetFileName(file));
            File.Copy(file, destFile, true);
        }

        // copy dirs
        foreach (string subdir in Directory.GetDirectories(source))
        {
            string dubdirdest = Path.Combine(dest, Path.GetFileName(subdir));
            CopyDirectory(subdir, dubdirdest);
        }
    }

    // ----

    static void AfterProjectLoad(string dir)
    {
        // rebuild shared ref for scripts
        string csproj = Path.Combine(dir, "project.csproj");
        if (File.Exists(csproj)) File.Delete(csproj);
        Dotnet.New(csproj);
        Dotnet.AddDll(csproj, Path.GetFullPath("Shared.dll"));

        // make sure gitignore exists
        string[] ignores = ["*.csproj", "bin/", "obj/", ".idea/", ".vscode/", ".vs/"];
        string gitignore_contents = "";
        foreach (var ignore in ignores)
        {
            bool last = ignore == ignores[ignores.Length - 1];
            gitignore_contents += last ? ignore : ignore + "\n";
        }
        string gitignore_path = Path.Combine(dir, ".gitignore");
        if (!File.Exists(gitignore_path)) File.WriteAllText(gitignore_path, gitignore_contents);
    }
}