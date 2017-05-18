using System;
using System.Reflection;

namespace zh.fang.stu.webapi.responsefilter.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}