using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore
{
    public class SettingsKeys
    {
        public static string AppSettings = "AppSettings";
        public static string AuthSettings = "AuthSettings";
        public static string AdminSettings = "AdminSettings";
        public static string EcPaySettings = "EcPaySettings";
        public static string RootSubjectSettings = "RootSubjectSettings";
        public static string SubscribesSettings = "SubscribesSettings";
        public static string CloudStorageSettings = "CloudStorageSettings";
    }

    public class ClaimsTypeKeys
    {
        public static string Id = "id";
        public static string Roles = "roles";
        public static string Sub = "sub";
    }
    public enum HttpClients
    {
        Google
    }

    public enum AppRoles
    {
        Boss,
        Dev,
        Subscriber
    }
    public enum PaymentTypes
    {
        CREDIT,
        ATM
    }

    public enum ThirdPartyPayment
    { 
        EcPay
    }

    

}
