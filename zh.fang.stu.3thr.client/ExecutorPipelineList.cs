namespace zh.fang.stu._3thr.client
{
    using System;

    public class ExecutorPipelineList : SimplePipelineList
    {
        public ExecutorPipelineList(string name, IExecuteHandle handler) 
            : base(name)
        {
            if (null == handler)
            {
                throw new ArgumentNullException("handler");
            }

            Handler = handler;
        }

        public IExecuteHandle Handler { get; private set; }

        protected override void Execute(PipelineContext context)
        {
            context.Result = Handler.Execute(context.Context);
            base.Execute(context);
        }
    }
}