using RollPunk.LuaApiGenerator.Generation;
using RollPunk.LuaApiGenerator.Reflection;

var outputPath = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.Combine(
        Environment.CurrentDirectory,
        "LuaAPI",
        "RollPunk.lua");

try
{
    var assemblies = AssemblyScanner.LoadAssemblies();

    var apiTypes = ApiTypeScanner.FindApiTypes(assemblies);
    var allTypes = ApiTypeScanner.CollectTypes(apiTypes);

    ApiTypeScanner.ValidateTypeNames(allTypes);

    var generator = new EmmyLuaGenerator(apiTypes, allTypes);
    var result = generator.Generate();

    Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

    File.WriteAllText(outputPath, result);

    Console.WriteLine($"Assemblies: {assemblies.Length}");
    Console.WriteLine($"API classes: {apiTypes.Length}");
    Console.WriteLine($"Total types: {allTypes.Count}");
    Console.WriteLine($"Generated: {outputPath}");
}
catch (Exception exception)
{
    Console.Error.WriteLine("Lua API generation failed:");
    Console.Error.WriteLine(exception);
    Environment.ExitCode = 1;
}