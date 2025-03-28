﻿/*
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
        public static bool ToastsAllowed { get; set; }
        public static void ToastsAllowedCheckBox(bool value)
        {
            ToastsAllowed = value;
        }

        public static void ShowToast(string title, string message, params object[] args)
        {
            if (ToastsAllowed)
            {
                string formattedTitle = string.Format(title, args);
                string formattedMessage = string.Format(message, args);

                new ToastContentBuilder()
                    .AddText(formattedTitle)
                    .AddText(formattedMessage)
                    .Show();
            }
        }
    }
}
