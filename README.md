# Readme

## How to set up VS Code and the project to build a NEW custom deserializer locally

The [original doc](https://docs.microsoft.com/en-us/azure/stream-analytics/visual-studio-code-custom-deserializer) lists the necessary steps:

1. Install the latest [.NET SDK](https://dotnet.microsoft.com/en-us/download)
2. Create a new dotnet class library with `dotnet new classlib -o myCustomDeserializer`
3. In that project, install the ASA package with `dotnet add package Microsoft.Azure.StreamAnalytics`
4. If the deserializer somehow needs to deal with JSON files, also `dotnet add package Newtonsoft.JSON`

**Important note not (yet) in the doc**: the project needs to be built on dotnet standard, not dotnet core (current default when using the dotnet CLI).
It is important to update the `.csproj` with the following [TargetFramework](https://docs.microsoft.com/en-us/dotnet/standard/frameworks):

```XML
...
  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
    <LangVersion>latest</LangVersion>
    ...
  </PropertyGroup>
...
```

[LangVersion](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version#c-language-version-reference) is set to `latest` to be able to use the newest version of C#.

Use `dotnet restore` in the project folder if need be.

## How to set up the job to use a custom deserializer

The ASA job needs to have [a global storage account and a custom code storage account](https://docs.microsoft.com/en-us/azure/stream-analytics/visual-studio-code-custom-deserializer#configure-a-stream-analytics-job). They don't need to be the same.

For local ASA projects these settings are found in the [JobConfig.json](https://docs.microsoft.com/en-us/azure/stream-analytics/job-config-json)

## How to set up a local input with a custom deserializer

Set up the **Format** to `CustomClr` (`Custom` in the picker) in the config file, then set the `SerializationProjectPath`, build and select the `SerializationClassName`.

```JSON
{
    "InputAlias": "Input1",
    "Type": "Data Stream",
    "Format": "CustomClr",
    "SerializationProjectPath": "..\\myJsonDeserializer\\myJsonDeserializer.csproj",
    "SerializationClassName": "myJsonDeserializer.myJsonDeserializer",
    "SerializationDllPath": "myJsonDeserializer.dll",
    "FilePath": "data_input1.json",
    "ScriptType": "InputMock"
}
```

From there on, local runs with local inputs will use the custom deserializer.

## How to set up a live input with a custom deserializer

Similarly, set **Serialization** to `CustomClr` (`Custom` in the picker), then set the `SerializationProjectPath`, build and select the `SerializationClassName`.

```JSON
{
...
    "Serialization": {
        "Type": "CustomClr",
        "SerializationProjectPath": "..\\myJsonDeserializer\\myJsonDeserializer.csproj",
        "SerializationClassName": "myJsonDeserializer.myJsonDeserializer",
        "SerializationDllPath": "myJsonDeserializer.dll"
    },
...
}
```

From there on, local runs on live inputs will use the custom deserializer.