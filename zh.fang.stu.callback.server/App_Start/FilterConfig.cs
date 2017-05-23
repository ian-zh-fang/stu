using System.Web;
using System.Web.Mvc;

namespace zh.fang.stu.callback.server
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
