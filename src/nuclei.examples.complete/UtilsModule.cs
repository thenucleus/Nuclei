//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using Autofac;
using NLog;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Diagnostics.Profiling;
using Nuclei.Diagnostics.Profiling.Reporting;

namespace Nuclei.Examples.Complete
{
    /// <summary>
    /// Handles the component registrations for the Nuclei part 
    /// of the core.
    /// </summary>
    internal sealed class UtilsModule : Autofac.Module
    {
        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultInfoFileName = "test.manual.nuclei.communication.info.{0}.log";

        /// <summary>
        /// The default name for the profiler log.
        /// </summary>
        private const string DefaultProfilerFileName = "test.manual.nuclei.communication.profile";

        /// <summary>
        /// Gets the assembly that called into this assembly.
        /// </summary>
        /// <returns>
        /// The calling assembly.
        /// </returns>
        private static Assembly GetAssembly()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                // Either we're being called from unmanaged code
                // or we're in a different appdomain than the actual executable
                assembly = Assembly.GetExecutingAssembly();
            }

            return assembly;
        }

        /// <summary>
        /// Gets the attribute from the calling assembly.
        /// </summary>
        /// <typeparam name="T">The type of attribute that should be gotten from the assembly.</typeparam>
        /// <returns>
        /// The requested attribute.
        /// </returns>
        private static T GetAttributeFromAssembly<T>() where T : Attribute
        {
            var attributes = GetAssembly().GetCustomAttributes(typeof(T), false);
            Debug.Assert(attributes.Length == 1, "There should only be one attribute.");

            var requestedAttribute = attributes[0] as T;
            Debug.Assert(requestedAttribute != null, "Found an incorrect attribute type.");

            return requestedAttribute;
        }

        /// <summary>
        /// Gets the name of the company.
        /// </summary>
        /// <value>The name of the company.</value>
        private static string CompanyName
        {
            get
            {
                var assemblyCompany = GetAttributeFromAssembly<AssemblyCompanyAttribute>();
                return assemblyCompany.Company;
            }
        }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        private static string ApplicationName
        {
            get
            {
                var assemblyName = GetAttributeFromAssembly<AssemblyProductAttribute>();
                return assemblyName.Product;
            }
        }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>The application version.</value>
        private static Version ApplicationVersion
        {
            get
            {
                var applicationVersion = GetAssembly().GetName().Version;
                return applicationVersion;
            }
        }

        /// <summary>
        /// Gets the application version with which this application is compatible.
        /// </summary>
        /// <value>The application compatibility version.</value>
        /// <remarks>
        /// A compatible application version indicates that the current version reads the
        /// configuration files of the compatible application.
        /// </remarks>
        private static Version ApplicationCompatibilityVersion
        {
            get
            {
                var fullVersion = ApplicationVersion;
                return new Version(fullVersion.Major, fullVersion.Minor);
            }
        }

        /// <summary>
        /// Returns the path for the directory in the user specific AppData directory which contains
        /// all the product directories for the current company.
        /// </summary>
        /// <returns>
        /// The full path for the AppData directory for the current company.
        /// </returns>
        private static string CompanyUserPath()
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var companyDirectory = Path.Combine(appDataDir, CompanyName);

            return companyDirectory;
        }

        /// <summary>
        /// Returns the path for the directory where the global 
        /// settings for the product are written to.
        /// </summary>
        /// <returns>
        /// The full path for the directory where the global settings
        /// for the product are written to.
        /// </returns>
        private static string ProductSettingsUserPath()
        {
            var companyDirectory = CompanyUserPath();
            var productDirectory = Path.Combine(companyDirectory, ApplicationName);
            var versionDirectory = Path.Combine(productDirectory, ApplicationCompatibilityVersion.ToString(2));

            return versionDirectory;
        }

        /// <summary>
        /// Returns the path for the directory where the log files are
        /// written to.
        /// </summary>
        /// <returns>
        /// The full path for the directory where the log files are written to.
        /// </returns>
        private static string LogPath()
        {
            var versionDirectory = ProductSettingsUserPath();
            var logDirectory = Path.Combine(versionDirectory, "logs");

            return logDirectory;
        }

        private static void RegisterDiagnostics(ContainerBuilder builder)
        {
            builder.Register(
                c =>
                {
                    var loggers = c.Resolve<IEnumerable<ILogger>>();
                    Action<LevelToLog, string> action = (p, s) =>
                    {
                        var msg = new LogMessage(p, s);
                        foreach (var logger in loggers)
                        {
                            try
                            {
                                logger.Log(msg);
                            }
                            catch (NLogRuntimeException)
                            {
                                // Ignore it and move on to the next logger.
                            }
                        }
                    };

                    Profiler profiler = null;
                    if (c.IsRegistered<Profiler>())
                    {
                        profiler = c.Resolve<Profiler>();
                    }

                    return new SystemDiagnostics(action, profiler);
                })
                .As<SystemDiagnostics>()
                .SingleInstance();
        }

        private static void RegisterLoggers(ContainerBuilder builder)
        {
            builder.Register(c => LoggerBuilder.ForFile(
                    Path.Combine(
                        LogPath(), 
                        string.Format(
                            CultureInfo.InvariantCulture,
                            DefaultInfoFileName,
                            Process.GetCurrentProcess().Id)),
                    new DebugLogTemplate(
                        c.Resolve<IConfiguration>(),
                        () => DateTimeOffset.Now)))
                .As<ILogger>()
                .SingleInstance();
        }

        private static void RegisterProfiler(ContainerBuilder builder)
        {
            if (ConfigurationHelpers.ShouldBeProfiling())
            {
                builder.Register((c, p) => new TextReporter(p.TypedAs<Func<Stream>>()))
                        .As<TextReporter>()
                        .As<ITransformReports>();

                builder.Register(c => new TimingStorage())
                    .OnRelease(
                        storage =>
                        {
                            // Write all the profiling results out to disk. Do this the ugly way 
                            // because we don't know if any of the other items in the container have
                            // been removed yet.
                            Func<Stream> factory =
                                () => new FileStream(
                                    Path.Combine(LogPath(), DefaultProfilerFileName),
                                    FileMode.Append,
                                    FileAccess.Write,
                                    FileShare.Read);
                            var reporter = new TextReporter(factory);
                            reporter.Transform(storage.FromStartTillEnd());
                        })
                    .As<IStoreIntervals>()
                    .As<IGenerateTimingReports>()
                    .SingleInstance();

                builder.Register(c => new Profiler(
                        c.Resolve<IStoreIntervals>()))
                    .SingleInstance();
            }
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is not the same one
        /// that the module is being registered by (i.e. it can have its own defaults).
        /// </remarks>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // Register the global application objects
            {
                RegisterLoggers(builder);
                RegisterProfiler(builder);
                RegisterDiagnostics(builder);
            }
        }
    }
}
