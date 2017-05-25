namespace zh.fang.stu._3thr.client
{
    public abstract class PipelineList : Pipeline
    {
        protected override void Execute(PipelineContext context)
        {
            if (!Continuation(context))
            {
                base.Execute(context);
            }
        }

        protected virtual bool Continuation(PipelineContext context)
        {
            if (null != Next)
            {
                ((IPipeline)Next).Execute(context);
                return true;
            }

            return false;
        }

        public void Add(Pipeline next)
        {
            Next = next;
        }

        public Pipeline Next { get; private set; }
    }
}