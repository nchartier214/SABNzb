using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CA1031, CA1303

namespace Nzb.Business
{
    public class InterfaceWebManager
    {
        private static readonly Encoding encoding = Encoding.UTF8;
        public Uri PostUrl { get; private set; }
        public string UserAgent { get; private set; }

        public InterfaceWebManager(Uri postUrl, string userAgent)
        {
            this.PostUrl = postUrl;
            this.UserAgent = userAgent;
        }
        public HttpWebResponse DataPost(Dictionary<string, object> postParameters)

        {
            string formDataBoundary = String.Format(CultureInfo.InvariantCulture, "----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            var formData =  InterfaceWebManager.GetMultipartFormData(postParameters, formDataBoundary);
            return this.PostForm(contentType, formData);
        }

        private HttpWebResponse PostForm(string contentType, byte[] formData)
        {
            Contract.Requires(formData != null);
            HttpWebRequest request = WebRequest.Create(this.PostUrl) as HttpWebRequest;
            if (request == null)
                throw new NullReferenceException("request is not a http request");

            // Set up the request properties.
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = this.UserAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            // You could add authentication here as well if needed:
            // request.PreAuthenticate = true;
            // request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
            // request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes("username" + ":" + "password")));

            // Send the form data to the request.
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;
        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Contract.Requires(postParameters != null);

            byte[] retour = null;
            using (var formDataStream = new MemoryStream())
            {
                bool needsCLRF = false;

                foreach (var param in postParameters)
                {
                    // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
                    // Skip it on the first parameter, add it to subsequent parameters.
                    if (needsCLRF)
                        formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                    needsCLRF = true;

                    if (param.Value is FileParameter)
                    {
                        FileParameter fileToUpload = param.Value as FileParameter;

                        // Add just the first part of this param, since we will write the file data directly to the Stream
                        string header = string.Format(CultureInfo.InvariantCulture, "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                            boundary,
                            param.Key,
                            fileToUpload.FileName ?? param.Key,
                            fileToUpload.ContentType ?? "application/octet-stream");

                        formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                        // Write the file data directly to the Stream, rather than serializing it to a string.
                        formDataStream.Write(fileToUpload.File.ToArray(), 0, fileToUpload.File.Count());
                    }
                    else
                    {
                        string postData = string.Format(CultureInfo.InvariantCulture, "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                            boundary,
                            param.Key,
                            param.Value);
                        formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                    }
                }

                // Add the end of the request.  Start with a newline
                string footer = "\r\n--" + boundary + "--\r\n";
                formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

                // Dump the Stream into a byte[]
                formDataStream.Position = 0;
                retour = new byte[formDataStream.Length];
                formDataStream.Read(retour, 0, retour.Length);
                formDataStream.Close();
            }

            return retour;
        }
   
    }
}
