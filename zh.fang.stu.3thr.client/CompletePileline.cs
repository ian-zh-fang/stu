namespace zh.fang.stu._3thr.client
{
    public class CompletePileline : Pipeline
    {
        private const string PipeName = "complete_pipeline";

        protected override void ExecuteComplete(PipelineContext context) { }

        protected override void ExecuteError(PipelineContext context)
        {
            if (null != ErrorPipeline)
            {
                ErrorPipeline.IsExecuteCompletePipeline = false;
            }
        }

        protected override string GetName()
        {
            return PipeName;
        }

        public ErrorPipeline ErrorPipeline { get; set; }
    }
}