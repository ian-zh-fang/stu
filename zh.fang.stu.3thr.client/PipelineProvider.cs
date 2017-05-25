namespace zh.fang.stu._3thr.client
{
    using System;

    public class PipelineProvider : IPipelineProvider
    {
        private IPipeline _pipeline;

        public PipelineProvider()
        {
            IsInit = false;
        }

        private void OnBeginInvoke(Pipeline pipeline, PipelineContext context)
        {
            if (null != OnBegin)
            {
                OnBegin.Invoke(pipeline, context);
            }
        }

        private void OnExecutingInvoke(Pipeline pipeline, PipelineContext context)
        {
            if (null != OnExecuting)
            {
                OnExecuting.Invoke(pipeline, context);
            }
        }

        private void OnExecuteInvoke(Pipeline pipeline, PipelineContext context)
        {
            if (null != OnExecute)
            {
                OnExecute.Invoke(pipeline, context);
            }
        }

        private void OnExecutedInvoke(Pipeline pipeline, PipelineContext context)
        {
            if (null != OnExecuted)
            {
                OnExecuted.Invoke(pipeline, context);
            }
        }

        private void OnErrorInvoke(Pipeline pipeline, PipelineContext context)
        {
            if (null != OnError)
            {
                OnError.Invoke(pipeline, context);
            }
        }

        private void OnEndInvoke(Pipeline pipeline, PipelineContext context)
        {
            if (null != OnEnd)
            {
                OnEnd.Invoke(pipeline, context);
            }
        }

        void IPipelineProvider.Init(IExecuteHandle handler)
        {
            var errPipeline = new ErrorPipeline();
            ((IPipeline)errPipeline).Init();
            errPipeline.OnExecute += OnErrorInvoke;

            var endPipeline = new CompletePileline();
            ((IPipeline)endPipeline).Init();
            endPipeline.ErrorPipeline = errPipeline;
            endPipeline.OnExecute += OnEndInvoke;

            errPipeline.CompletePileline = endPipeline;

            var donePipeline = BindPipelineAndEvent("handleexecuted_pipeline", endPipeline, errPipeline, endPipeline, OnExecutedInvoke);

            var doPipeline = new ExecutorPipelineList("handleexecute_pipeline", handler) { CompletePileline = endPipeline, ErrorPipeline = errPipeline };
            doPipeline.Add(donePipeline);
            doPipeline.OnExecute += OnExecuteInvoke;
            ((IPipeline)doPipeline).Init();
                
            var doingPipeline = BindPipelineAndEvent("handleexecuting_pipeline", endPipeline, errPipeline, doPipeline, OnExecutingInvoke);
            var beginPipeline = BindPipelineAndEvent("begin_pipeline", endPipeline, errPipeline, doingPipeline, OnBeginInvoke);

            _pipeline = beginPipeline;

            IsInit = true;
        }

        private PipelineList BindPipelineAndEvent(string pipeName, CompletePileline endPipeline, ErrorPipeline errPipeline, Pipeline next, Action<Pipeline, PipelineContext> action)
        {
            var pipeline = new SimplePipelineList(pipeName) { CompletePileline = endPipeline, ErrorPipeline = errPipeline };
            ((IPipeline)pipeline).Init();
            pipeline.Add(next);
            pipeline.OnExecute += action;

            return pipeline;
        }

        protected virtual void Init() { }

        IExecuteResult IPipelineProvider.Execute(IExecuteContext context)
        {
            if (!IsInit)
            {
                throw new ArgumentException("Pipeline must be initialize .");
            }

            var pipeContext = new PipelineContext
            {
                Context = context,
                Result = null,
                Error = null
            };
            _pipeline.Execute(pipeContext);
            return pipeContext.Result;
        }

        IPipeline IPipelineProvider.Pipeline
        {
            get { return _pipeline; }
        }

        public bool IsInit { get; private set; }

        public event Action<Pipeline, PipelineContext> OnBegin;
        public event Action<Pipeline, PipelineContext> OnExecuting;
        public event Action<Pipeline, PipelineContext> OnExecute;
        public event Action<Pipeline, PipelineContext> OnExecuted;
        public event Action<Pipeline, PipelineContext> OnError;
        public event Action<Pipeline, PipelineContext> OnEnd;
    }
}