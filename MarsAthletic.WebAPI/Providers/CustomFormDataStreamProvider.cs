using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MarsAthletic.WebAPI.Providers
{
    public class CustomFormDataStreamProvider : MultipartFormDataStreamProvider
    {

        public CustomFormDataStreamProvider(string path)
            : base(path)
        { }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            // override the filename which is stored by the provider (by default is bodypart_x)
            string oldfileName = headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(oldfileName);

            return newFileName;
        }
    }
}
