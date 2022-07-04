﻿using XstReader.ElementProperties;

namespace XstReader.App.Common
{
    public static class XstAttachmentExtensions
    {
        public static byte[] GetContent(this XstAttachment attachment)
        {
            using (var ms = new MemoryStream())
            {
                attachment.SaveToStream(ms);
                return ms.ToArray();
            }
        }

        public static string GetContentAsBase64(this XstAttachment attachment)
            => Convert.ToBase64String(GetContent(attachment));


        public static string ContentType(this XstAttachment attachment)
        {
            if (attachment == null)
                return string.Empty;

            if (attachment.IsEmail)
                return string.Empty;

            return (attachment.Properties[PropertyCanonicalName.PidTagAttachMimeTag]?.Value as string) ?? string.Empty;
        }


    }
}
