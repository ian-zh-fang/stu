namespace zh.fang.stu.httpmodel
{
    using System.Web;

    public class MyHttpModule : IHttpModule
    {
        void IHttpModule.Dispose()
        {
            
        }

        void IHttpModule.Init(HttpApplication context)
        {
            context.BeginRequest +=
                (caller, arg) => {
                    foreach (var key in context.Modules.AllKeys)
                    {
                        var type = context.Modules[key].GetType();
                        context.Response.Write($"{key}\t\t\t{type.FullName}\t\t\t{type.Assembly.FullName}\r\n");
                    }

                    context.CompleteRequest();
                };
        }
    }

    public static class MyHttpModuleRegister
    {
        public static void Registe()
        {
            HttpApplication.RegisterModule(typeof(filters.FilterModule));
        }
    }
}