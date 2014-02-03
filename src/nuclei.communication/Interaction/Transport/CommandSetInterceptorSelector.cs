//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Nuclei.Communication.Interaction.Transport
{
    /// <summary>
    /// An <see cref="IInterceptorSelector"/> that indicates which selector should be used for a
    /// <see cref="ICommandSet"/> proxy based on the existence of a return type for a method.
    /// </summary>
    internal sealed class CommandSetInterceptorSelector : IInterceptorSelector
    {
        /// <summary>
        /// Selects the interceptors that should intercept calls to the given method.
        /// </summary>
        /// <param name="type">The type declaring the method to intercept.</param>
        /// <param name="method">The method that will be intercepted.</param>
        /// <param name="interceptors">All interceptors registered with the proxy.</param>
        /// <returns>An array of interceptors to invoke upon calling the method.</returns>
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var name = ReflectionExtensions.MemberName<CommandSetProxy, object>(p => p.SelfReference());
            if (string.Equals(name, method.Name, StringComparison.Ordinal))
            {
                return interceptors.Where(i => i is ProxySelfReferenceInterceptor).ToArray();
            }

            if (method.ReturnType == typeof(Task))
            {
                return interceptors.Where(i => i is CommandSetMethodWithoutResultInterceptor).ToArray();
            }

            return interceptors.Where(i => i is CommandSetMethodWithResultInterceptor).ToArray();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var selector = obj as CommandSetInterceptorSelector;
            return selector != null;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }
}
