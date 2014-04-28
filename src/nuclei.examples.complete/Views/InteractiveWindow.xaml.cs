//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Nuclei.Examples.Complete.Models;

namespace Nuclei.Examples.Complete.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    internal partial class InteractiveWindow : Window, IInteractiveWindow
    {
        /// <summary>
        /// The command used to send data to a given endpoint.
        /// </summary>
        public static readonly RoutedCommand SendDataCommand = new RoutedCommand();

        /// <summary>
        /// The command used to send a data message to a given endpoint.
        /// </summary>
        public static readonly RoutedCommand SendMessageCommand = new RoutedCommand();

        /// <summary>
        /// The command used to send a calculate message to a given endpoint.
        /// </summary>
        public static readonly RoutedCommand CalculateTotalCommand = new RoutedCommand();

        /// <summary>
        /// The command used to send a notification to a given endpoint.
        /// </summary>
        public static readonly RoutedCommand NotifyCommand = new RoutedCommand();

        /// <summary>
        /// The context that will be used to exit the application.
        /// </summary>
        private readonly System.Windows.Forms.ApplicationContext m_Context;

        /// <summary>
        /// The object that forwards communication commands onto the real
        /// communication layer.
        /// </summary>
        private readonly IHandleCommunication m_Communicator;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractiveWindow"/> class.
        /// </summary>
        public InteractiveWindow()
        {
            InitializeComponent();

            Title = string.Format(CultureInfo.InvariantCulture, "Process ID: {0}", Process.GetCurrentProcess().Id);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractiveWindow"/> class.
        /// </summary>
        /// <param name="context">The context that is use to terminate the application.</param>
        /// <param name="communicator">The object that forwards communication commands onto the real communicator.</param>
        public InteractiveWindow(System.Windows.Forms.ApplicationContext context, IHandleCommunication communicator)
            : this()
        {
            m_Context = context;
            m_Communicator = communicator;
        }

        /// <summary>
        /// Gets or sets the connection state information for the application.
        /// </summary>
        public ConnectionViewModel ConnectionState
        {
            get
            {
                return (ConnectionViewModel)DataContext;
            }
            
            set
            {
                DataContext = value;
            }
        }

        private void OnExecuteSendMessageCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var model = e.Parameter as Tuple<string, ConnectionInformationViewModel>;
            m_Communicator.SendEchoMessageTo(model.Item2.Id, model.Item1);
        }

        private void OnCanExecuteSendMessageCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            var model = e.Parameter as Tuple<string, ConnectionInformationViewModel>;
            e.Handled = true;
            e.CanExecute = (model != null) 
                && (model.Item2 != null) 
                && m_Communicator.IsConnected && m_Communicator.CanContactEndpoint(model.Item2.Id);
        }

        private void OnExecuteSendDataCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var model = e.Parameter as Tuple<string, ConnectionInformationViewModel>;
            m_Communicator.SendDataTo(model.Item2.Id, model.Item1);
        }

        private void OnCanExecuteSendDataCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            var model = e.Parameter as Tuple<string, ConnectionInformationViewModel>;
            e.Handled = true;
            e.CanExecute = (model != null)
                && (model.Item2 != null)
                && m_Communicator.IsConnected && m_Communicator.CanContactEndpoint(model.Item2.Id);
        }

        private void OnExecuteCalculateTotalCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var model = e.Parameter as Tuple<string, string, ConnectionInformationViewModel>;
            var result = m_Communicator.AddNumbers(model.Item3.Id, int.Parse(model.Item1), int.Parse(model.Item2));
            result.ContinueWith(
                t => 
                {
                    Action action = () => ConnectionState.AddNewMessage(
                        model.Item3.Id,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0} + {1} = {2}",
                            model.Item1,
                            model.Item2,
                            t.Result));

                    if (!Dispatcher.CheckAccess())
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, action);
                    }
                    else
                    {
                        action();
                    }
                });
        }

        private void OnCanExecuteCalculateTotalCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            var model = e.Parameter as Tuple<string, string, ConnectionInformationViewModel>;
            e.Handled = true;
            e.CanExecute = (model != null)
                && (model.Item3 != null)
                && m_Communicator.IsConnected && m_Communicator.CanContactEndpoint(model.Item3.Id);
        }

        private void OnExecuteNotifyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            m_Communicator.Notify();
        }

        private void OnCanExecuteNotifyCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = (m_Communicator != null) && m_Communicator.IsConnected;
        }

        /// <summary>
        /// Handles the window closing event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void HandleWindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            m_Context.ExitThread();
        }
    }
}
