# CodeWorkspaceTool

A .NET tool for creating and managing [VS Code multi-root workspace files](https://code.visualstudio.com/docs/editor/multi-root-workspaces) (`*.code-workspace`) from the command line — the same way `dotnet sln` manages a `.sln` file's project references.

Built with [Spectre.Console.Cli](https://spectreconsole.net/).

## Install

```bash
dotnet tool install --global CodeWorkspaceTool
```

This installs the `codews` command.

## Usage

Every command that operates on an existing workspace file will auto-discover the single `*.code-workspace` file in the current directory. If there isn't exactly one, pass `-w`/`--workspace <FILE>` explicitly.

### Create a workspace

```bash
codews init [NAME] [-o|--output <DIRECTORY>] [--force]
```

Creates `NAME.code-workspace` (defaults to the output directory's name) with an empty folder list. `--force` overwrites an existing file.

### Manage folders

```bash
codews folder add <PATH>... [--name <LABEL>] [-w|--workspace <FILE>]
codews folder remove <PATH>... [-w|--workspace <FILE>]
codews folder list [--format table|json] [-w|--workspace <FILE>]
```

`--name` is only valid when adding a single folder. Adding a folder already in the workspace is a no-op; removing one that isn't present is an error.

### Manage recommended extensions

```bash
codews extension add <ID>... [--unwanted] [-w|--workspace <FILE>]
codews extension remove <ID>... [--unwanted] [-w|--workspace <FILE>]
codews extension list [--format table|json] [-w|--workspace <FILE>]
```

`--unwanted` targets `extensions.unwantedRecommendations` instead of `extensions.recommendations`.

### Manage settings

```bash
codews settings set <KEY> <VALUE> [--type auto|string|bool|int|number|json] [-w|--workspace <FILE>]
codews settings unset <KEY> [-w|--workspace <FILE>]
codews settings list [--format table|json] [-w|--workspace <FILE>]
```

By default (`--type auto`), the value's type is inferred from its text (`true`/`false` → bool, numeric text → int/number, `{...}`/`[...]` → JSON, otherwise a string). Use `--type` to force a specific interpretation, e.g.:

```bash
codews settings set editor.formatOnSave true
codews settings set "[python].editor.rulers" "[80,120]" --type json
```

## Design notes

- Sections the tool doesn't manage (`launch`, `tasks`, arbitrary top-level keys) are read and written back untouched.
- The workspace file is rewritten as plain, reformatted JSON on every save — comments and custom formatting in a hand-edited file are not preserved.

## Source

<https://github.com/Kralizek/VisualStudioCodeWorkspace>
