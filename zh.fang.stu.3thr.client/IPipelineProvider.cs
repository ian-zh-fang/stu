namespace zh.fang.stu._3thr.client
{
    public interface IPipelineProvider
    {
        IPipeline Pipeline { get; }

        void Init(IExecuteHandle handler);

        IExecuteResult Execute(IExecuteContext context);
    }
}