Motivational:

Instructions on how to compile:
https://github.com/lambdageek/sample-dotnet-wasi-vscode

More links:

https://github.com/SteveSandersonMS/spiderlightning-dotnet -> looks to be the most recent complete example ...

Wasi support: https://github.com/dotnet/runtime/issues/65895
No VFS: https://github.com/dotnet/runtime/issues/81418

A lot of issues with "Security.Crypto" and "HttpClient" missing

- no HTTP client
- usage of VFS -> completely verify OpenAPI too
- InitializeInheritanceIndex -> should be sequential and not use parallel
- make it possible to run completely sequential, introducing an option to completely disable `Parallel` calls
