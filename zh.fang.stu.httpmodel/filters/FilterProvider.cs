namespace zh.fang.stu.httpmodel.filters
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    public abstract class FilterProvider : IFilterProvider
    {
        private static object _locked = new object();
        private static readonly FilterCollection Filters = new FilterCollection();

        void IFilterProvider.Execute(IFilterContext context, Action<IFilterContext> failureAction)
        {
            Executing(context);
            ExecuteCore(context, failureAction);
        }

        protected virtual void Executing(IFilterContext context)
        {

        }

        protected virtual void ExecuteCore(IFilterContext context, Action<IFilterContext> failureAction)
        {
            // do something ...            
            if (Filters.Filters.Any(t => !t.Valid(context)))
            {
                // failure filter
                failureAction?.Invoke(context);
            }

            // success
            // continuation doing ...
        }

        public static void Registe(IFilter filter)
        {
            lock(_locked)
            {
                if (!Filters.Has(filter))
                    Filters.Add(filter);
            }
        }
    }

    public class DefaultFilterProvider:FilterProvider
    {

    }

    public class HttpApplicationFilterProvider:FilterProvider
    {
        static HttpApplicationFilterProvider()
        {
            Registe(new HttpApplicationFilter());
        }

        protected override void Executing(IFilterContext context)
        {

            if (context is HttpApplicationContext)
            {
                var executeContxt = (HttpApplicationContext)context;
                ITaskProvider task = new HttpApplicationTaskProvider(executeContxt.Context);
                Parallel.Invoke(() => task.Start());
                return;
            }

            throw new NotSupportedException($"can not conver {context.GetType().FullName} to {typeof(HttpApplicationContext).FullName}");
        }
    }

    public interface ITaskProvider
    {
        void Start();

        void End();
    }

    public class HttpApplicationTaskProvider : ITaskProvider
    {
        private readonly HttpApplication _context;

        public HttpApplicationTaskProvider(HttpApplication context)
        {
            _context = context;
        }

        void ITaskProvider.End()
        {
            
        }

        void ITaskProvider.Start()
        {
            
        }
    }
}