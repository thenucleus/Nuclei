//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows.Data;
using Nuclei.Examples.Complete.Models;

namespace Nuclei.Examples.Complete.Views
{
    /// <summary>
    /// Converts a string and a <see cref="ConnectionInformationViewModel"/> to a tuple containing both.
    /// </summary>
    internal sealed class MessageSendParameterMultiConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts source values to a value for the binding target. The data binding 
        /// engine calls this method when it propagates the values from source bindings 
        /// to the binding target.
        /// </summary>
        /// <param name="values">
        /// The array of values that the source bindings in the System.Windows.Data.MultiBinding
        /// produces. The value System.Windows.DependencyProperty.UnsetValue indicates
        /// that the source binding has no value to provide for conversion.
        /// </param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A
        /// return value of System.Windows.DependencyProperty.System.Windows.DependencyProperty.UnsetValue
        /// indicates that the converter did not produce a value, and that the binding
        /// will use the System.Windows.Data.BindingBase.FallbackValue if it is available,
        /// or else will use the default value.A return value of System.Windows.Data.Binding.System.Windows.Data.Binding.DoNothing
        /// indicates that the binding does not transfer the value or use the System.Windows.Data.BindingBase.FallbackValue
        /// or the default value.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = null;
            ConnectionInformationViewModel model = null;
            foreach (var obj in values)
            {
                if (obj is string)
                {
                    text = obj as string;
                    continue;
                }

                if (obj is ConnectionInformationViewModel)
                {
                    model = obj as ConnectionInformationViewModel;
                }
            }

            return Tuple.Create(text, model);
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">
        /// The array of types to convert to. The array length indicates the number and
        /// types of values that are suggested for the method to return.
        /// </param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to
        /// the source values.
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
