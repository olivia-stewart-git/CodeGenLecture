using System.Diagnostics;
using System.Reflection;

namespace CodeGenLecture;

public class CmdLineProcessGenerator : CodeGenerator
{
	public override Assembly Generate(string code, Action<string> onOutput)
	{
		var directory = Path.Combine(Path.GetTempPath(), "CodeGenLecture");
        Directory.CreateDirectory(directory);

        var tempFile = Path.Combine(directory, "Code.cs");
        File.WriteAllText(tempFile, code);

		var projectXml = """
		                 <Project Sdk="Microsoft.NET.Sdk">
		                   <PropertyGroup>
		                 	<OutputType>Exe</OutputType>
		                 	<TargetFramework>net8.0</TargetFramework>
		                 	<OutputPath>.</OutputPath>
		                   </PropertyGroup>
		                 
		                   <ItemGroup>
		                 	<Compile Include="Code.cs" />
		                   </ItemGroup>
		                 </Project>
		                 """;

        File.WriteAllText(Path.Combine(directory, "Code.csproj"), projectXml);

        //Generate dll and return assembly
        var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "dotnet",
				Arguments = " build Code.csproj -o . -nologo -v q",
                RedirectStandardOutput = true,
				RedirectStandardInput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true,
				WorkingDirectory = Path.GetDirectoryName(tempFile)
			}
		};

		process.OutputDataReceived += (_, args) => onOutput(args.Data ?? string.Empty);
		process.ErrorDataReceived += (_, args) => onOutput(args.Data ?? string.Empty);
		process.Start();
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();
		process.WaitForExit();


		var dll = Directory.GetFiles(directory, "*.dll").FirstOrDefault();
		ArgumentNullException .ThrowIfNull(dll);
        return Assembly.LoadFile(dll);
    }
}