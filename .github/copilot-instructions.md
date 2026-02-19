# Copilot Instructions for Braze.Api

## Project Overview

This is a .NET client library providing strongly typed API clients for Braze's API. It's an unofficial NuGet package not affiliated with Braze.

## Technology Stack

- **Languages**: C# 12
- **Target Frameworks**: .NET 8.0, 9.0, and 10.0
- **Test Framework**: xUnit
- **Key Dependencies**: 
  - Microsoft.Extensions.DependencyInjection.Abstractions
  - Microsoft.Extensions.Http
  - System.Text.Json

## Build and Test

### Commands
- **Build**: `dotnet build`
- **Test**: `dotnet test`
- **Format**: `dotnet format` (run before build to fix formatting issues)
- **Pack**: `dotnet pack Braze.Api --configuration Release`

### Testing
- Tests use xUnit framework
- Tests are located in `Braze.Api.Tests/`
- Run tests after building: `dotnet test --no-build`
- Test files use file-scoped namespaces and minimal test class structure

## Code Style and Conventions

### General C# Conventions
- **Namespaces**: Use file-scoped namespaces (`namespace Braze.Api;`)
- **Nullable**: Enabled - always consider nullability
- **Language Version**: C# 12
- **Indentation**: 4 spaces (not tabs)
- **Line Endings**: LF (Unix-style) for most files; CRLF for .cmd/.bat
- **Encoding**: UTF-8 with BOM for C# files
- **New Lines**: Opening braces on new lines (Allman style)

### Naming Conventions
- **Private/Internal Fields**: Use `_camelCase` prefix (e.g., `_myField`)
- **Static Private Fields**: Use `s_camelCase` prefix (e.g., `s_staticField`)
- **Constants**: Use `PascalCase` (e.g., `MaxValue`)
- **Public Members**: Use `PascalCase`
- **Local Variables**: Use `camelCase`
- **Type Keywords**: Prefer `int`, `string` over `Int32`, `String`
- **var**: Use `var` when type is apparent or for built-in types

### Code Quality Rules
- **Warnings as Errors**: Enabled - all warnings must be fixed
- **Security**: All security diagnostics are treated as errors
- **Code Style Enforcement**: Enabled in build
- **XML Documentation**: Required for public APIs (GenerateDocumentationFile is enabled)
- **Readonly Fields**: Prefer `readonly` for fields that don't change after initialization
- **Expression-bodied Members**: Use for simple methods, properties, and accessors
- **Pattern Matching**: Prefer pattern matching over `is` with cast or `as` with null check
- **Object/Collection Initializers**: Use when appropriate

### Project Structure
- **Main Library**: `Braze.Api/` - Contains the API client implementation
- **Tests**: `Braze.Api.Tests/` - Contains all test files
- **Shared Configuration**: 
  - `Directory.Build.props` - Shared MSBuild properties
  - `Directory.Packages.props` - Centralized package version management
  - `.editorconfig` - Code style rules

### API Design Patterns
- The client library follows the logical structure of the Braze REST API documentation
- API clients are registered using dependency injection via `AddBrazeApi()` extension methods
- Support for both standard and keyed services
- Use strongly typed models for request and response objects
- JSON serialization with `System.Text.Json` using custom converters where needed

### Documentation
- All public APIs must have XML documentation comments
- Use standard XML doc tags: `<summary>`, `<param>`, `<returns>`, `<remarks>`, etc.
- Keep documentation concise and clear
- Update README.md if adding significant new features

### Git and Version Control
- Version is managed in `Braze.Api/Braze.Api.csproj` via `VersionPrefix` property
- CI/CD uses GitHub Actions (`.github/workflows/nuget.yml`)
- PRs are tested automatically on push to main branch
- Releases are triggered by tags matching `release/*` pattern

## Important Notes

- Do not modify `InternalsVisibleTo` without good reason
- Security-related analyzers are set to error severity - address all security warnings
- Multi-targeting requires testing across all target frameworks (net8.0, net9.0, net10.0)
- The package is dual-licensed: MIT OR Apache-2.0
- Keep dependencies minimal and aligned with target framework versions

## Git Workflow - IMPORTANT

### Avoiding Unintended File Commits

**CRITICAL**: Do NOT use `git add .` or `git add -A` as it will stage files with only BOM (Byte Order Mark) changes that shouldn't be committed.

**Correct workflow**:
1. After making changes, check which files actually have code changes:
   ```bash
   git diff --name-only
   git status
   ```

2. **Only add files you intentionally modified**:
   ```bash
   git add path/to/file1.cs path/to/file2.cs
   ```
   Or use the report_progress tool which will handle git operations correctly.

3. Before committing, verify only intended files are staged:
   ```bash
   git diff --cached --name-only
   ```

### Why This Matters

- C# files in this repo use UTF-8 with BOM encoding (per .editorconfig)
- Opening/viewing files may add BOMs to files that don't have them
- This makes git see them as "changed" even though no code changed
- A .gitattributes file exists to normalize this, but explicit staging is still required

### If You Accidentally Stage Too Many Files

```bash
# Unstage everything
git reset

# Check what's actually changed (not just BOM)
git diff --name-only

# Stage only the files with real changes
git add <file1> <file2> <file3>
```
