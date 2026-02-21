namespace EnsinoApp.Config;

public class CustomAssemblyLoadContext : System.Runtime.Loader.AssemblyLoadContext
{
    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        if (!File.Exists(absolutePath))
            throw new FileNotFoundException($"Arquivo nativo não encontrado: {absolutePath}");
        return LoadUnmanagedDll(absolutePath);
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllPath)
    {
        return LoadUnmanagedDllFromPath(unmanagedDllPath);
    }

    protected override System.Reflection.Assembly Load(System.Reflection.AssemblyName assemblyName)
    {
        return null; // não carrega assemblies gerenciados adicionais
    }
}