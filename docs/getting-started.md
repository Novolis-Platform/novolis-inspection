# Getting started

```powershell
dotnet add package Novolis.Inspection.Json
dotnet add package Novolis.Inspection.ObjectGraph
```

```csharp
var document = new JsonInspectionFormatter().Format(json);
var objectDocument = new ObjectGraphInspectionFormatter().Format(value);
```

Render `InspectionDocument.Root` in the host's tree or text surface. Formatters do not reference UI frameworks.
