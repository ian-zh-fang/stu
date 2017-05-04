namespace zh.fang.stu.httpmodel.filters
{
    using System;
    using System.Web;

    public class HttpApplicationContext:IFilterContext
    {
        public HttpApplication Context { get; private set; }

        public HttpApplicationContext(HttpApplication context)
        {
            if (null == context)
                throw new ArgumentNullException(nameof(context));

            Context = context;
        }
    }
}