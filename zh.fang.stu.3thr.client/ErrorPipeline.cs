namespace zh.fang.stu._3thr.client
{
    public class ErrorPipeline : Pipeline
    {
        private const string PipeName = "error_pipeline";

        protected override void Init()
        {
            IsExecuteCompletePipeline = true;
            base.Init();
        }

        protected override void ExecuteComplete(PipelineContext context)
        {
            if (IsExecuteCompletePipeline)
            {
                ((IPipeline)CompletePileline).Execute(context);
            }
        }

        protected override void ExecuteError(PipelineContext context)
        {
            OnExecuteInvoke(context);
        }

        protected override string GetName()
        {
            return PipeName;
        }

        public CompletePileline CompletePileline { get; set; }

        public bool IsExecuteCompletePipeline { get; set; }
    }
}