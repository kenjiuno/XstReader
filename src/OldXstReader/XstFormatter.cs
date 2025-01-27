﻿// Project site: https://github.com/iluvadev/XstReader
//
// Based on the great work of Dijji. 
// Original project: https://github.com/dijji/XstReader
//
// Issues: https://github.com/iluvadev/XstReader/issues
// License (Ms-PL): https://github.com/iluvadev/XstReader/blob/master/license.md
//
// Copyright (c) 2022, iluvadev, and released under Ms-PL License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace XstReader
{
    internal static class XstFormatter
    {
        public static string UnknownValueText { get; set; }

        public static Func<XstRecipient, string> RecipientFormatter { get; set; }
        public static Func<IEnumerable<XstRecipient>, string> RecipientListFormatter { get; set; }

        public static Func<DateTime, string> DateFormatter { get; set; }

        public static Func<XstAttachment, string> AttachmentFormatter { get; set; }
        public static Func<IEnumerable<XstAttachment>, string> AttachmentListFormatter { get; set; }

        public static Func<XstMessage, bool> ShowMessageHeader { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        static XstFormatter()
        {
            RecipientFormatter = (r) => r?.DisplayName + (string.IsNullOrEmpty(r?.Address) ? "" : $" <{r.Address}>");
            RecipientListFormatter = (rl) => String.Join("; ", rl.Select(r => Format(r)));

            DateFormatter = (d) => d.ToString("f");

            UnknownValueText = "<unknown>";

            AttachmentFormatter = (a) => a == null ? "" : $"{a.DisplayName ?? a.FileName} ({(a.Size > 1000 ? $"{a.Size / 1000}Kb" : $"{a.Size}b")})";
            AttachmentListFormatter = (al) => String.Join("; ", al.Select(a => Format(a)));

            ShowMessageHeader = (m) => true;
        }

        public static string Format(XstRecipient recipient)
            => RecipientFormatter?.Invoke(recipient) ?? recipient.ToString();

        public static string Format(IEnumerable<XstRecipient> recipientList)
            => RecipientListFormatter?.Invoke(recipientList ?? new List<XstRecipient>()) ?? String.Join("; ", recipientList.Select(r => Format(r)));

        public static string Format(DateTime dateTime)
            => DateFormatter(dateTime);

        public static string Format(DateTime? dateTime)
            => dateTime == null ? UnknownValueText : Format(dateTime.Value);

        public static string Format(XstAttachment attachment)
            => attachment == null ? "" : AttachmentFormatter?.Invoke(attachment) ?? attachment.ToString();

        public static string Format(IEnumerable<XstAttachment> attachmentList)
            => AttachmentListFormatter?.Invoke(attachmentList ?? new List<XstAttachment>()) ?? String.Join("; ", attachmentList.Select(a => Format(a)));

    }
}
