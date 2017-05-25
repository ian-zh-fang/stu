namespace zh.fang.stu._3thr.client
{
    using System;

    public class SimplePipelineList : PipelineList
    {
        private readonly string _name;

        public SimplePipelineList(string name)
            : base()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            _name = name;
        }

        protected override void ExecuteComplete(PipelineContext context)
        {
            if (null != CompletePileline)
            {
                ((IPipeline)CompletePileline).Execute(context);
            }
        }

        protected override void ExecuteError(PipelineContext context)
        {
            if (null != ErrorPipeline)
            {
                ((IPipeline)ErrorPipeline).Execute(context);
            }
        }

        protected override string GetName()
        {
            return _name;
        }

        public ErrorPipeline ErrorPipeline { get; set; }

        public CompletePileline CompletePileline { get; set; }
    }
}