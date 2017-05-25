namespace zh.fang.stu._3thr.client
{
    public interface IPipeline
    {
        void Init();

        void Execute(PipelineContext context);

        void ExecuteError(PipelineContext context);

        void ExecuteComplete(PipelineContext context);

        string Name { get; }
    }
}