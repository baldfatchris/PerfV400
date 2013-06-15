using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using PerfV400.Models;

namespace PerfV400
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");

            OAuthWebSecurity.RegisterFacebookClient(
                // Amovada
                 //appId: "219955008083494",
                 //appSecret: "92f8af70f21b76bcaf829685630326ac");

                // LEDB
                appId: "318328708209008",
                appSecret: "b3cfb8ffc2016c80b1ee4ec4ddac4791");

            //OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
