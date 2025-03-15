/*
##########################################
#           TikTok Downloader            #
#           Made by Jettcodey            #
#                © 2024                  #
#           DO NOT REMOVE THIS           #
##########################################
*/
using System;
using Microsoft.Toolkit.Uwp.Notifications;

namespace TikTok_Downloader
{
    public class ToastNotification
    {
        public static bool ToastsAllowed { get; private set; } = true;
        public static void ToastsAllowedCheckBox(bool value)
        {
            ToastsAllowed = value;
        }

        public static void ShowToast(string title, string message, params object[] args)
        {
            if (ToastsAllowed)
            {
                string formattedMessage = string.Format(message, args);

                new ToastContentBuilder()
                    .AddText(title)
                    .AddText(formattedMessage)
                    .Show();
            }
        }
    }
}
