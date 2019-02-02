using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class BytesInDiaryWarningMessage
    {
        public long BytesInDiary { get; set; }
        public string WarningMessage { get; set; }

        public BytesInDiaryWarningMessage() { }

        public BytesInDiaryWarningMessage(long bytesInDiary, string warningMessage)
        {
            this.BytesInDiary = bytesInDiary;
            this.WarningMessage = warningMessage;
        }
    }
}