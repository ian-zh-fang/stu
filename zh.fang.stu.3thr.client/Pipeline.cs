namespace zh.fang.stu._3thr.client
{
    using System;

    public abstract class Pipeline : IPipeline
    {
        private string _name;

        void IPipeline.Init()
        {
            IsContinue = true;
            IsCompleted = false;

            Init();
        }

        protected virtual void Init() { }

        void IPipeline.Execute(PipelineContext context)
        {
            try
            {
                OnExecuteInvoke(context);
                if (IsContinue)
                {
                    Execute(context);
                }
            }
            catch (Exception err)
            {
                context.Error = err;
                ((IPipeline)this).ExecuteError(context);
            }
        }

        protected void OnExecuteInvoke(PipelineContext context)
        {
            if (null != OnExecute)
            {
                OnExecute.Invoke(this, context);
            }
        }

        protected virtual void Execute(PipelineContext context)
        {
            ((IPipeline)this).ExecuteComplete(context);
        }

        void IPipeline.ExecuteComplete(PipelineContext context)
        {
            IsContinue = false;
            IsCompleted = true;

            ExecuteComplete(context);
        }

        void IPipeline.ExecuteError(PipelineContext context)
        {
            ExecuteError(context);
        }

        string IPipeline.Name
        {
            get
            {
                _name = _name ?? GetName();
                if (string.IsNullOrWhiteSpace(_name))
                {
                    throw new ArgumentNullException("Name");
                }

                return _name;
            }
        }

        public bool IsContinue { get; private set; }

        public bool IsCompleted { get; private set; }

        protected abstract string GetName();

        protected abstract void ExecuteComplete(PipelineContext context);

        protected abstract void ExecuteError(PipelineContext context);

        public event Action<Pipeline, PipelineContext> OnExecute;
    }
}