//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the
// Code Analysis results, point to "Suppress Message", and click
// "In Suppression File".
// You do not need to add suppressions to this file manually.
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Microsoft.Performance",
    "CA1822:MarkMembersAsStatic",
    Scope = "member",
    Target = "Nuclei.Samples.AssemblyExtensionsSample.#LocalFilePath()",
    Justification = "It's a test")]
[assembly: SuppressMessage(
    "Microsoft.Performance",
    "CA1822:MarkMembersAsStatic",
    Scope = "member",
    Target = "Nuclei.Samples.AssemblyExtensionsSample.#LocalDirectoryPath()",
    Justification = "It's a test")]
[assembly: SuppressMessage(
    "Microsoft.Performance",
    "CA1822:MarkMembersAsStatic",
    Scope = "member",
    Target = "Nuclei.Samples.AssemblyExtensionsSample.#StrongName()",
    Justification = "It's a test")]
[assembly: SuppressMessage(
    "Microsoft.Performance",
    "CA1822: MarkMembersAsStatic",
    Scope = "member",
    Target = "Nuclei.Samples.EmbeddedResourceExtracterSample.#LoadFromEmbeddedStream()",
    Justification = "It's a test")]
[assembly: SuppressMessage(
    "Microsoft.Performance",
    "CA1822:MarkMembersAsStatic",
    Scope = "member",
    Target = "Nuclei.Samples.EmbeddedResourceExtracterSample.#LoadFromEmbeddedTextFile()",
    Justification = "It's a test")]
[assembly: SuppressMessage(
    "Microsoft.Performance",
    "CA1822:MarkMembersAsStatic",
    Scope = "member",
    Target = "Nuclei.Samples.TypeSample.#IsAssignableToOpenGenericType()",
    Justification = "It's a test")]
