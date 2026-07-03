namespace CodeWorkspaceTool;

/// <summary>
/// Represents an expected, user-facing failure (bad input, missing file, etc).
/// Caught at the top level and reported without a stack trace.
/// </summary>
public sealed class CodeWorkspaceException(string message) : Exception(message);