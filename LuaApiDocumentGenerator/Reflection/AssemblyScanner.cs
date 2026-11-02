using System.Reflection;

namespace RollPunk.LuaApiGenerator.Reflection;

public static class AssemblyScanner
{
    public static Assembly[] LoadAssemblies()
    {
        var assemblies = new List<Assembly>();

        foreach (var path in Directory.GetFiles(
                     AppContext.BaseDirectory, "*.dll"))
        {
            try
            {
                assemblies.Add(Assembly.LoadFrom(path));
            }
            catch (BadImageFormatException)
            {
                // Native library.
            }
            catch (FileLoadException)
            {
                // Assembly couldn't be loaded.
            }
            catch (FileNotFoundException)
            {
                // Missing dependency.
            }
        }

        return assemblies
            .Distinct()
            .ToArray();
    }

    public static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.OfType<Type>();
        }
    }
}