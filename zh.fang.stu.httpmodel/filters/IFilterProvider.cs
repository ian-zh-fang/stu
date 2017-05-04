namespace zh.fang.stu.httpmodel.filters
{
    using System;

    public interface IFilterProvider
    {
        void Execute(IFilterContext context, Action<IFilterContext> failureAction);
    }
}