# Notepad.Extensions.Logging

[![NuGet](https://img.shields.io/nuget/v/Notepad.Extensions.Logging)](https://www.nuget.org/packages/Notepad.Extensions.Logging/)

This is a library for .NET / .NET Core to log your program's output to a handy Notepad window!

## Installation

To use:

1. Add a package reference to `Notepad.Extensions.Logging`.
2. In `Startup.cs`, call `AddNotepad()`, like so:

```csharp
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();
    
    // ...
    
    services.AddLogging(lb => lb.AddNotepad()); // This is where the magic happens
}
```

3. Open a new Notepad, Notepad++, or Notepad2 window.
4. Run your application.

## Source Material

Inspired by [this tweet](https://twitter.com/steveklabnik/status/1263190719721766918):

![](https://pbs.twimg.com/media/EYfBQ5cXsAEcu2e?format=jpg&name=orig)
