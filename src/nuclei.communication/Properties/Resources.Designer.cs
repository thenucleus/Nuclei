﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nuclei.Communication.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Nuclei.Communication.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A channel type must be defined and cannot be the None type..
        /// </summary>
        internal static string Exceptions_Messages_AChannelTypeMustBeDefined {
            get {
                return ResourceManager.GetString("Exceptions_Messages_AChannelTypeMustBeDefined", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A channel type must inherit from IChannelType..
        /// </summary>
        internal static string Exceptions_Messages_AChannelTypeMustDeriveFromIChannelType {
            get {
                return ResourceManager.GetString("Exceptions_Messages_AChannelTypeMustDeriveFromIChannelType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A command set type must inherit from ICommandSet..
        /// </summary>
        internal static string Exceptions_Messages_ACommandSetTypeMustDeriveFromICommandSet {
            get {
                return ResourceManager.GetString("Exceptions_Messages_ACommandSetTypeMustDeriveFromICommandSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A message must have an ID number. It cannot have the None ID as ID..
        /// </summary>
        internal static string Exceptions_Messages_AMessageNeedsToHaveAnId {
            get {
                return ResourceManager.GetString("Exceptions_Messages_AMessageNeedsToHaveAnId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A notification set must inherit from INotificationSet..
        /// </summary>
        internal static string Exceptions_Messages_ANotificationSetTypeMustDeriveFromINotificationSet {
            get {
                return ResourceManager.GetString("Exceptions_Messages_ANotificationSetTypeMustDeriveFromINotificationSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The application must be allowed to connect via at least one channel type..
        /// </summary>
        internal static string Exceptions_Messages_AtLeastOneChannelTypeMustBeAllowed {
            get {
                return ResourceManager.GetString("Exceptions_Messages_AtLeastOneChannelTypeMustBeAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The address of a communication channel must be defined in order to connect to the channel..
        /// </summary>
        internal static string Exceptions_Messages_ChannelAddresssMustBeDefined {
            get {
                return ResourceManager.GetString("Exceptions_Messages_ChannelAddresssMustBeDefined", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given command interface has already been registered..
        /// </summary>
        internal static string Exceptions_Messages_CommandAlreadyRegistered {
            get {
                return ResourceManager.GetString("Exceptions_Messages_CommandAlreadyRegistered", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The invocation of a command failed..
        /// </summary>
        internal static string Exceptions_Messages_CommandInvocationFailed {
            get {
                return ResourceManager.GetString("Exceptions_Messages_CommandInvocationFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The requested command is not supported by the endpoint..
        /// </summary>
        internal static string Exceptions_Messages_CommandNotSupported {
            get {
                return ResourceManager.GetString("Exceptions_Messages_CommandNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The command of type {0} is not supported by the endpoint..
        /// </summary>
        internal static string Exceptions_Messages_CommandNotSupported_WithCommand {
            get {
                return ResourceManager.GetString("Exceptions_Messages_CommandNotSupported_WithCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided command object must implement the provided command interface..
        /// </summary>
        internal static string Exceptions_Messages_CommandObjectMustImplementCommandInterface {
            get {
                return ResourceManager.GetString("Exceptions_Messages_CommandObjectMustImplementCommandInterface", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The download of the desired data failed..
        /// </summary>
        internal static string Exceptions_Messages_DataDownloadFailed {
            get {
                return ResourceManager.GetString("Exceptions_Messages_DataDownloadFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An object data serializer for the given type already exists..
        /// </summary>
        internal static string Exceptions_Messages_DuplicateObjectSerializer {
            get {
                return ResourceManager.GetString("Exceptions_Messages_DuplicateObjectSerializer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An endpoint ID cannot be deserialized from an empty string..
        /// </summary>
        internal static string Exceptions_Messages_EndpointIdCannotBeDeserializedFromAnEmptyString {
            get {
                return ResourceManager.GetString("Exceptions_Messages_EndpointIdCannotBeDeserializedFromAnEmptyString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The endpoint cannot be contacted. No connection information for this endpoint is available..
        /// </summary>
        internal static string Exceptions_Messages_EndpointNotContactable {
            get {
                return ResourceManager.GetString("Exceptions_Messages_EndpointNotContactable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The endpoint [{0}] cannot be contacted. No connection information for this endpoint is available..
        /// </summary>
        internal static string Exceptions_Messages_EndpointNotContactable_WithEndpoint {
            get {
                return ResourceManager.GetString("Exceptions_Messages_EndpointNotContactable_WithEndpoint", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to send the message..
        /// </summary>
        internal static string Exceptions_Messages_FailedToSendMessage {
            get {
                return ResourceManager.GetString("Exceptions_Messages_FailedToSendMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A file path cannot be an empty string..
        /// </summary>
        internal static string Exceptions_Messages_FilePathCannotBeEmpty {
            get {
                return ResourceManager.GetString("Exceptions_Messages_FilePathCannotBeEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No file registration was found for the given token..
        /// </summary>
        internal static string Exceptions_Messages_FileRegistrationNotFound {
            get {
                return ResourceManager.GetString("Exceptions_Messages_FileRegistrationNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No file registration was found for the token: {0}.
        /// </summary>
        internal static string Exceptions_Messages_FileRegistrationNotFound_WithToken {
            get {
                return ResourceManager.GetString("Exceptions_Messages_FileRegistrationNotFound_WithToken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Transferring the information from the stream requires a different set of information than provided. The method required {0} but was provided with {1}..
        /// </summary>
        internal static string Exceptions_Messages_IncorrectStreamTransferInformationObjectFound_WithTypes {
            get {
                return ResourceManager.GetString("Exceptions_Messages_IncorrectStreamTransferInformationObjectFound_WithTypes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The translator has the wrong version to process the discovery channel on the given URI..
        /// </summary>
        internal static string Exceptions_Messages_IncorrectTranslatorVersion {
            get {
                return ResourceManager.GetString("Exceptions_Messages_IncorrectTranslatorVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given channel type is not valid..
        /// </summary>
        internal static string Exceptions_Messages_InvalidChannelType {
            get {
                return ResourceManager.GetString("Exceptions_Messages_InvalidChannelType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The maximum number of channel restarts was exceeded. Not restarting the channel anymore..
        /// </summary>
        internal static string Exceptions_Messages_MaximumNumberOfChannelRestartsExceeded {
            get {
                return ResourceManager.GetString("Exceptions_Messages_MaximumNumberOfChannelRestartsExceeded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to In order to filter on a given message kind the message class must implement the ICommunicationMessage interface..
        /// </summary>
        internal static string Exceptions_Messages_MessageToFilterOnNeedsToImplementICommunicationMessage {
            get {
                return ResourceManager.GetString("Exceptions_Messages_MessageToFilterOnNeedsToImplementICommunicationMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The endpoint discovery message did not provide a binding type. Contact information for the endpoint is incomplete without the binding information..
        /// </summary>
        internal static string Exceptions_Messages_MissingBindingType {
            get {
                return ResourceManager.GetString("Exceptions_Messages_MissingBindingType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The requested command set could not be found..
        /// </summary>
        internal static string Exceptions_Messages_MissingCommandSet {
            get {
                return ResourceManager.GetString("Exceptions_Messages_MissingCommandSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The endpoint discovery message did not provide an endpoint ID. Contact information for the endpoint is incomplete without the ID..
        /// </summary>
        internal static string Exceptions_Messages_MissingEndpointId {
            get {
                return ResourceManager.GetString("Exceptions_Messages_MissingEndpointId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No object data serializer was provided for the given object type..
        /// </summary>
        internal static string Exceptions_Messages_MissingObjectDataSerializer {
            get {
                return ResourceManager.GetString("Exceptions_Messages_MissingObjectDataSerializer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No communication subjects have been registered. This may result in the rejection of all incoming endpoint connections..
        /// </summary>
        internal static string Exceptions_Messages_NoCommunicationSubjectsRegistered {
            get {
                return ResourceManager.GetString("Exceptions_Messages_NoCommunicationSubjectsRegistered", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No object data serializer was found for the given type..
        /// </summary>
        internal static string Exceptions_Messages_NoSerializerForTypeFound {
            get {
                return ResourceManager.GetString("Exceptions_Messages_NoSerializerForTypeFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The requested notification is not supported by the endpoint..
        /// </summary>
        internal static string Exceptions_Messages_NotificationNotSupported {
            get {
                return ResourceManager.GetString("Exceptions_Messages_NotificationNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The notification of type {0} is not supported by the endpoint..
        /// </summary>
        internal static string Exceptions_Messages_NotificationNotSupported_WithNotification {
            get {
                return ResourceManager.GetString("Exceptions_Messages_NotificationNotSupported_WithNotification", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided notification object must implement the provided notification interface..
        /// </summary>
        internal static string Exceptions_Messages_NotificationObjectMustImplementNotificationInterface {
            get {
                return ResourceManager.GetString("Exceptions_Messages_NotificationObjectMustImplementNotificationInterface", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given type is not a valid command set interface..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidCommandSet {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidCommandSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} is not a valid command set because it contains the following events: {1}..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetCannotHaveEvents {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetCannotHaveEvents", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} is not a valid command set because it contains the following properties: {1}..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetCannotHaveProperties {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetCannotHaveProperties", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} is not a valid command set because it contains the method {1} with one or more generic parameters..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetMethodsCannotBeGeneric {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetMethodsCannotBeGeneric", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} is not a valid command set because it contains the method {1} which does not return a void, a Task or a Task&lt;T&gt;..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetMethodsMustHaveCorrectReturnType {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetMethodsMustHaveCorrectRet" +
                        "urnType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} is not a valid command set because it does not have any methods..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetMustHaveMethods {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetMustHaveMethods", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} is not a valid command set because it contains the method {1} which has an out parameter, a ref parameter or a parameter that cannot be serialized..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetParametersMustBeValid {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetParametersMustBeValid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} does not derive from ICommandSet..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidCommandSet_TypeIsNotAnICommandSet {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidCommandSet_TypeIsNotAnICommandSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} is not a valid command set because it is not an interface..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidCommandSet_TypeIsNotAnInterface {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidCommandSet_TypeIsNotAnInterface", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} is not a valid command set because it contains open generic parameters..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidCommandSet_TypeMustBeClosedConstructed {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidCommandSet_TypeMustBeClosedConstructed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given type is not a valid notification set interface..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidNotificationSet {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidNotificationSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} is not a valid notification set because it contains the following methods: {1}..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetCannotHaveMethods {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetCannotHaveMetho" +
                        "ds", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} is not a valid notification set because it contains the following properties: {1}..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetCannotHaveProperties {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetCannotHavePrope" +
                        "rties", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} is not a valid notification set because it contains the event {1} which either does not use the EventHandler or EventHandler&lt;T&gt; delegate, or the EventArgs generic parameter is not serializable..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetEventsMustUseEventHandler {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetEventsMustUseEv" +
                        "entHandler", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} is not a valid notification set because it does not have any events..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetMustHaveEvents {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetMustHaveEvents", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} does not derive from INotificationSet..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidNotificationSet_TypeIsNotAnINotificationSet {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidNotificationSet_TypeIsNotAnINotificationSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} is not a valid notification set because it is not an interface..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidNotificationSet_TypeIsNotAnInterface {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidNotificationSet_TypeIsNotAnInterface", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} is not a valid notification set because it contains open generic parameters..
        /// </summary>
        internal static string Exceptions_Messages_TypeIsNotAValidNotificationSet_TypeMustBeClosedConstructed {
            get {
                return ResourceManager.GetString("Exceptions_Messages_TypeIsNotAValidNotificationSet_TypeMustBeClosedConstructed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to generate a proxy for the given type..
        /// </summary>
        internal static string Exceptions_Messages_UnableToGenerateProxy {
            get {
                return ResourceManager.GetString("Exceptions_Messages_UnableToGenerateProxy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The requested command set has not been registered..
        /// </summary>
        internal static string Exceptions_Messages_UnknownCommandSet {
            get {
                return ResourceManager.GetString("Exceptions_Messages_UnknownCommandSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is no information for the given communication subject..
        /// </summary>
        internal static string Exceptions_Messages_UnknownCommunicationSubject {
            get {
                return ResourceManager.GetString("Exceptions_Messages_UnknownCommunicationSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given endpoint had an unsupported format..
        /// </summary>
        internal static string Exceptions_Messages_UnknownEndpointIdFormat {
            get {
                return ResourceManager.GetString("Exceptions_Messages_UnknownEndpointIdFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given communication message is unknown and cannot be handled..
        /// </summary>
        internal static string Exceptions_Messages_UnknownMessageType {
            get {
                return ResourceManager.GetString("Exceptions_Messages_UnknownMessageType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The requested notification set has not been registered..
        /// </summary>
        internal static string Exceptions_Messages_UnknownNotificationSet {
            get {
                return ResourceManager.GetString("Exceptions_Messages_UnknownNotificationSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An upload for the given token already exists..
        /// </summary>
        internal static string Exceptions_Messages_UploadNotDeregistered {
            get {
                return ResourceManager.GetString("Exceptions_Messages_UploadNotDeregistered", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An upload for the token [{0}] already exists..
        /// </summary>
        internal static string Exceptions_Messages_UploadNotDeregistered_WithToken {
            get {
                return ResourceManager.GetString("Exceptions_Messages_UploadNotDeregistered_WithToken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection to discovery URI {0} failed with error. Error was: {1}.
        /// </summary>
        internal static string Log_Messages_DiscoveryFailedToConnectToEndpoint_WithUriAndError {
            get {
                return ResourceManager.GetString("Log_Messages_DiscoveryFailedToConnectToEndpoint_WithUriAndError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to find a suitable discovery URI for process on {0}..
        /// </summary>
        internal static string Log_Messages_DiscoveryFailedToFindMatchingDiscoveryVersion_WithUri {
            get {
                return ResourceManager.GetString("Log_Messages_DiscoveryFailedToFindMatchingDiscoveryVersion_WithUri", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to start the communication system. Error was: {0}.
        /// </summary>
        internal static string Log_Messages_FailedToStartCommunicationSystem_WithError {
            get {
                return ResourceManager.GetString("Log_Messages_FailedToStartCommunicationSystem_WithError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Signaling manual connection of endpoint. Connection to endpoint {0} at {1}.
        /// </summary>
        internal static string Log_Messages_ManualConnectionOfRemoteEndpoint_WithConnectionInformation {
            get {
                return ResourceManager.GetString("Log_Messages_ManualConnectionOfRemoteEndpoint_WithConnectionInformation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Signalling manual disconnection of endpoint {0}.
        /// </summary>
        internal static string Log_Messages_ManualDisconnectionOfRemoteEndpoint_WithConnectionInformation {
            get {
                return ResourceManager.GetString("Log_Messages_ManualDisconnectionOfRemoteEndpoint_WithConnectionInformation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Signaling new connection through manual discovery. Waiting for communication layer to sign in ....
        /// </summary>
        internal static string Log_Messages_ManualDisoveryWaitingForLayerSignIn {
            get {
                return ResourceManager.GetString("Log_Messages_ManualDisoveryWaitingForLayerSignIn", resourceCulture);
            }
        }
    }
}
