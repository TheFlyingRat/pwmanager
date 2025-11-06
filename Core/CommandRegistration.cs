namespace PWMan.Core;

// wrapper for REPL to initialise commands with custom fields
public record CommandRegistration(
    Command Command,
    string[]? DefaultArgs = null,
    string? Name = null,
    string? Help = null,
    bool RequiresVault = true
);