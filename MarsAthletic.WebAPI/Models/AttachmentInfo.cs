using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAthletic.WebAPI.Models
{
    public class AttachmentInfo
    {
        public CreationStatus CreationStatus { get; set; }
        public int CreatedFileId { get; set; }
    }

    public enum CreationStatus
    {
        Created, NotCreated
    }
}
