namespace zh.fang.stu.httpmodel.filters
{
    using System;

    public class HttpApplicationFilter : IFilter
    {
        bool IFilter.Valid(IFilterContext context)
        {
            var executeContext = (context as HttpApplicationContext);
            if (null == executeContext)
                throw new NotSupportedException($"can not conver {context.GetType().FullName} to {typeof(HttpApplicationContext).FullName}");

            return Core(executeContext);
        }

        // validation core
        private bool Core(HttpApplicationContext context)
        {
            return true;
        }
    }
}