# Integration Testing Comparison

This repository aims to compare the use of XUnit & NUnit for integration testing of a basic service.

Because these both attempt to create the same service, the tests should not be executed in parallel.

```bash
dotnet test IntegrationFrameworks.NUnit
dotnet test IntegrationFrameworks.XUnit
```