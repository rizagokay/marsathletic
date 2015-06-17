using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAthletic.WebAPI.Models
{
    public class AttachmentData
    {
        public int MainDocumentID { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public byte [] FileData { get; set; }
    }
}
