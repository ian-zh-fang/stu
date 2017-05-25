using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zh.fang.stu._3thr.client
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var context = new ExampleExecuteContext();
            var factor = new ExecuteHandleFactory();
            var handler = factor.GetHandle<ExampleExecuteHandle>();
            var provider = new PipelineProvider();
            ((IPipelineProvider)provider).Init(handler);
            provider.OnBegin += Provider_OnBegin;
            provider.OnEnd += Provider_OnEnd;
            provider.OnError += Provider_OnError;
            provider.OnExecute += Provider_OnExecute;
            provider.OnExecuted += Provider_OnExecuted;
            provider.OnExecuting += Provider_OnExecuting;

            var result = ((IPipelineProvider)provider).Execute(context);
            Console.WriteLine(null == result ? "null" : result.ToString());

            Console.ReadKey();
        }

        private static void Provider_OnExecuting(Pipeline arg1, PipelineContext arg2)
        {
            Console.WriteLine(nameof(Provider_OnExecuting));
        }

        private static void Provider_OnExecuted(Pipeline arg1, PipelineContext arg2)
        {
            Console.WriteLine(nameof(Provider_OnExecuted));
        }

        private static void Provider_OnExecute(Pipeline arg1, PipelineContext arg2)
        {
            Console.WriteLine(nameof(Provider_OnExecute));
        }

        private static void Provider_OnError(Pipeline arg1, PipelineContext arg2)
        {
            Console.WriteLine(nameof(Provider_OnError));
        }

        private static void Provider_OnEnd(Pipeline arg1, PipelineContext arg2)
        {
            Console.WriteLine(nameof(Provider_OnEnd));
        }

        private static void Provider_OnBegin(Pipeline arg1, PipelineContext arg2)
        {
            Console.WriteLine(nameof(Provider_OnBegin));
            ((IPipeline)arg1).ExecuteComplete(arg2);
            throw new ArgumentException();
        }
    }
}