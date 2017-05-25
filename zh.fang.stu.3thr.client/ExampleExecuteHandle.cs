namespace zh.fang.stu._3thr.client
{
    public class ExampleExecuteHandle : IExecuteHandle
    {
        IExecuteResult IExecuteHandle.Execute(IExecuteContext context)
        {
            return new ExampleExecuteResult();
        }
    }
}