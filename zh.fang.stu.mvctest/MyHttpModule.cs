using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace zh.fang.stu.mvctest
{
    public class MyHttpModule : IHttpModule
    {
        private const string PARAMENAMEFMT = "{{${0}}}";
        private const string ACTIONPARAMENAME = "action";
        private const string DATVALPARAMENAME = "datVal";
        private const string RedirecPOSTHtmlFileName = "default.html";
        private const string RedirectRouter = "/default";

        private static readonly string RedirecPOSURL = "/dddd";
        private static readonly string RedirecPOSTHtmlFmt;
        private static readonly string ActionParame;
        private static readonly string DatValParame;

        static MyHttpModule()
        {
            ActionParame = string.Format(PARAMENAMEFMT, ACTIONPARAMENAME);
            DatValParame = string.Format(PARAMENAMEFMT, DATVALPARAMENAME);

            var fileName = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, RedirecPOSTHtmlFileName);
            using (var stream = File.OpenRead(fileName))
            {
                var reader = new StreamReader(stream);
                RedirecPOSTHtmlFmt = reader.ReadToEnd();

                reader.Close();
            }
        }

        void IHttpModule.Dispose()
        {
            
        }

        void IHttpModule.Init(HttpApplication context)
        {
            context.BeginRequest += (sender, args) => 
            {
                var rawUrl = context.Request.Url.AbsolutePath;
                if(rawUrl == RedirecPOSURL)
                {
                    var content = GetContent(context);
                    context.Response.Write(content);
                    context.CompleteRequest();
                }

                if(rawUrl == RedirectRouter)
                {
                    var data = GenPostData();
                    var html = RedirecPOSTHtmlFmt
                        .Replace(ActionParame, RedirecPOSURL)
                        .Replace(DatValParame, data);
                    context.Response.Write(html);
                    context.CompleteRequest();
                }
            };
        }

        private string GetContent(HttpApplication context)
        {
            var stream = context.Request.GetBufferedInputStream();
            var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();

            stream.Close();
            reader.Close();

            return content;
        }

        private string GenPostData()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}