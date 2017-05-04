namespace zh.fang.stu.httpmodel.filters
{
    // 过滤器接口
    public interface IFilter
    {

        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="context">应用程序上下文</param>
        /// <returns></returns>
        bool Valid(IFilterContext context);
    }
}