﻿using Files.Common;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace Files.Helpers
{
    public static class FileThumbnailHelper
    {
        private static AppServiceConnection AppServiceConnection { get; set; }
        public static async Task<(byte[] IconData, byte[] OverlayData, bool IsCustom)> LoadIconOverlayAsync(string filePath, uint thumbnailSize)
        {
            AppServiceConnection ??= await AppServiceConnectionHelper.Instance;
            if (AppServiceConnection != null)
            {
                var value = new ValueSet
                {
                    { "Arguments", "GetIconOverlay" },
                    { "filePath", filePath },
                    { "thumbnailSize", (int)thumbnailSize }
                };
                var response = await AppServiceConnection.SendMessageAsync(value);
                var hasCustomIcon = (response.Status == AppServiceResponseStatus.Success)
                    && response.Message.Get("HasCustomIcon", false);
                var icon = response.Message.Get("Icon", (string)null);
                var overlay = response.Message.Get("Overlay", (string)null);

                // BitmapImage can only be created on UI thread, so return raw data and create
                // BitmapImage later to prevent exceptions once SynchorizationContext lost
                return (icon == null ? null : Convert.FromBase64String(icon),
                    overlay == null ? null : Convert.FromBase64String(overlay),
                    hasCustomIcon);
            }
            return (null, null, false);
        }
    }
}