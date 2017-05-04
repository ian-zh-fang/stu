namespace zh.fang.stu.httpmodel.filters
{
    using System;
    using System.Web;

    public class FilterModule : IHttpModule
    {
        void IHttpModule.Dispose()
        {            
        }

        void IHttpModule.Init(HttpApplication context)
        {
            context.BeginRequest += (sender, arg) => BegingRequest(context);
            context.EndRequest += (sender, arg) => End(context);
        }

        private void End(HttpApplication app)
        {
            
        }

        public int StatusCode { get; set; } = 4430;

        public string Message { get; set; } = "提示消息内容";

        private void BegingRequest(HttpApplication context)
        {
            context.Context.Response.StatusCode = 403;
            context.Context.Response.Write(string.Format("<h1>{0}</h1>\r\n<p>{1}</p>", StatusCode, Message));

            //var FilterProvider = FilterProviderFactory.GetProvider(typeof(HttpApplicationFilterProvider).FullName);
            //var filterContext = new HttpApplicationContext(context);
            //FilterProvider.Execute(filterContext,
            //    executeContext =>
            //    {
            //        context.Response.StatusCode = 301;
            //        context.CompleteRequest();
            //    });
        }
    }

    public class MyApplication:HttpApplication
    {
        public HttpApplication HttpApplication { get; private set; }

        public MyApplication(HttpApplication app)
        {
            HttpApplication = app;
        }
    }
}