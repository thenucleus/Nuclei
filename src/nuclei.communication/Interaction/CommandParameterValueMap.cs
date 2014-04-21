//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Maps a parameter definition to its value.
    /// </summary>
    internal sealed class CommandParameterValueMap
    {
        /// <summary>
        /// The parameter for the current map.
        /// </summary>
        private readonly CommandParameterDefinition m_Parameter;

        /// <summary>
        /// The value for the current parameter.
        /// </summary>
        private readonly object m_Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandParameterValueMap"/> class.
        /// </summary>
        /// <param name="parameter">The definition of the current parameter.</param>
        /// <param name="value">The value of the current parameter.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameter"/> is <see langword="null" />.
        /// </exception>
        public CommandParameterValueMap(CommandParameterDefinition parameter, object value)
        {
            {
                Lokad.Enforce.Argument(() => parameter);
            }

            m_Parameter = parameter;
            m_Value = value;
        }

        /// <summary>
        /// Gets the parameter for the current map.
        /// </summary>
        public CommandParameterDefinition Parameter
        {
            [DebuggerStepThrough]
            get
            {
                return m_Parameter;
            }
        }

        /// <summary>
        /// Gets the value for the current parameter.
        /// </summary>
        public object Value
        {
            [DebuggerStepThrough]
            get
            {
                return m_Value;
            }
        }
    }
}