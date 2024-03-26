// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
//
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Suppresses Warnings for implicit type usage
/// "IDE0007:Use implicit type"
/// </summary>
[assembly: SuppressMessage(
    "Style", "IDE0007:Use implicit type",
    Justification = "<Pending>",
    Scope = "member",
    Target = "~M:electrifier.Services.DosShellItem.#ctor(Windows.Storage.IStorageItem,Windows.Storage.Search.QueryOptions)"
    )]
