using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using static System.Environment;

namespace IdentityFramework.Service
{
    public class HttpLoggingHandler : DelegatingHandler
    {
        private readonly Func<HttpRequestMessage, Task> _requestAction;
        private readonly Func<HttpResponseMessage, Task> _responseAction;
        public static string _ResponseLogFile;
        public static string _RequestLogFile;
        public static string _BackupDirectory;
        public static int _KeepLogs;

        public HttpLoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            _responseAction = DefaultResponseAction;
            _requestAction = DefaultRequestAction;
        }

        public HttpLoggingHandler(HttpMessageHandler innerHandler, string ResponseLogFile, string RequestLogFile) : base(innerHandler)
        {
            _responseAction = DefaultResponseAction;
            _requestAction = DefaultRequestAction;
            _ResponseLogFile = ResponseLogFile;
            _RequestLogFile = RequestLogFile;
        }

        public HttpLoggingHandler(HttpMessageHandler innerHandler,
            Func<HttpRequestMessage, Task> requestAction,
            Func<HttpResponseMessage, Task> responseAction)
            : base(innerHandler)
        {
            _responseAction = responseAction;
            _requestAction = requestAction;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (_requestAction != null)
                await _requestAction(request);
            var result = await base.SendAsync(request, cancellationToken);
            if (_responseAction != null)
                await _responseAction(result);
            return result;
        }

        private async Task DefaultRequestAction(HttpRequestMessage request)
        {
            Debug.WriteLine("Request:");
            Debug.WriteLine(request.ToString());
            if (request.Content != null)
                Debug.WriteLine(await request.Content.ReadAsStringAsync());
            Debug.WriteLine("");
        }

        private async Task DefaultResponseAction(HttpResponseMessage response)
        {
            Debug.WriteLine("Response:");
            Debug.WriteLine(response.ToString());
            if (response.Content != null)
                Debug.WriteLine(await response.Content.ReadAsStringAsync());
            Debug.WriteLine("");
        }


        public static async Task ResponseAction(HttpResponseMessage httpResponseMessage)
        {
            string content = null;
            string output = null;
            if (httpResponseMessage.Content != null)
            {
                content = await httpResponseMessage.Content.ReadAsStringAsync();
            }

            //add the https status
            output += $"Status: {httpResponseMessage.StatusCode}{NewLine}";

            //add the headers to the ouput
            if (httpResponseMessage.Headers.Any())
            {
                output += $"{NewLine}-----------Headers--------------{NewLine}";

                foreach (var header in httpResponseMessage.Headers)
                {
                    output += $"{header.Key} : ";

                    //get the values as a string array
                    string[] values = header.Value.ToArray();

                    //add the header values to the output
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (i == header.Value.Count() - 1)
                        {
                            output += $"{values[i]}";
                        }
                        else
                        {
                            output += $"{values[i]}; ";
                        }
                    }
                    output += $"{NewLine}";
                }
            }

            //add the body
            output += $"{NewLine}-----------Body--------------{NewLine}";
            output += content;

            //use the standard response log file
            if (String.IsNullOrEmpty(_ResponseLogFile))
            {
                File.WriteAllText("LastResponse.txt", output);
            }
            else
            {
                //use the specified response log file
                UtilityService.WriteTextFileWithBackup(_ResponseLogFile, _BackupDirectory, _KeepLogs, output);
            }

        }

        public static async Task RequestAction(HttpRequestMessage httpRequestMessage)
        {
            string content = null;
            string output = null;
            if (httpRequestMessage.Content != null)
            {
                content = await httpRequestMessage.Content.ReadAsStringAsync();
            }

            //capture the url and method information
            output += $"URL: {httpRequestMessage.RequestUri}{NewLine}";
            output += $"METHOD: {httpRequestMessage.Method}{NewLine}";

            //add the headers to the ouput
            if (httpRequestMessage.Headers.Any())
            {
                output += $"{NewLine}-----------Headers--------------{NewLine}";

                foreach (var header in httpRequestMessage.Headers)
                {
                    output += $"{header.Key} : ";

                    //get the values as a string array
                    string[] values = header.Value.ToArray();

                    //add the header values to the output
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (i == header.Value.Count() - 1)
                        {
                            output += $"{values[i]}";
                        }
                        else
                        {
                            output += $"{values[i]}; ";
                        }
                    }
                    output += $"{NewLine}";
                }
            }

            //add the body
            output += $"{NewLine}-----------Body--------------{NewLine}";
            output += content;

            //use the standard request log file
            if (String.IsNullOrEmpty(_ResponseLogFile))
            {
                File.WriteAllText("LastRequest.txt", output);
            }
            else
            {
                //use the specified request log file
                UtilityService.WriteTextFileWithBackup(_RequestLogFile, _BackupDirectory, _KeepLogs, output);
            }

        }


    }
}
