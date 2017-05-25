namespace zh.fang.stu._3thr.client
{
    using System;

    public class PipelineContext
    {
        public IExecuteContext Context { get; set; }

        public IExecuteResult Result { get; set; }

        public Exception Error { get; set; }
    }
}