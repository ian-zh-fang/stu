namespace zh.fang.stu.webapi.responsefilter
{
    using System.IO;
    using System.Web;

    public class MyHttpModule : IHttpModule
    {
        void IHttpModule.Dispose()
        {

        }

        void IHttpModule.Init(HttpApplication context)
        {
            context.BeginRequest += (sender, args) =>
            {
                context.Context.Response.Filter = new MyFilter(context.Context.Response.Filter);
                Context_Event_Callback(context, nameof(context.BeginRequest));
            };

            context.AcquireRequestState += (sender, arg) => Context_Event_Callback(context, nameof(context.AcquireRequestState));
            context.AuthenticateRequest += (sender, arg) => Context_Event_Callback(context, nameof(context.AuthenticateRequest));
            context.AuthorizeRequest += (sender, arg) => Context_Event_Callback(context, nameof(context.AuthorizeRequest));
            context.Disposed += (sender, arg) => Context_Event_Callback(context, nameof(context.Disposed));
            context.EndRequest += (sender, arg) => Context_Event_Callback(context, nameof(context.EndRequest));
            context.Error += (sender, arg) => Context_Event_Callback(context, nameof(context.Error));
            context.LogRequest += (sender, arg) => Context_Event_Callback(context, nameof(context.LogRequest));
            context.MapRequestHandler += (sender, arg) => Context_Event_Callback(context, nameof(context.MapRequestHandler));
            context.PostAcquireRequestState += (sender, arg) => Context_Event_Callback(context, nameof(context.PostAcquireRequestState));
            context.PostAuthenticateRequest += (sender, arg) => Context_Event_Callback(context, nameof(context.PostAuthenticateRequest));
            context.PostAuthorizeRequest += (sender, arg) => Context_Event_Callback(context, nameof(context.PostAuthorizeRequest));
            context.PostLogRequest += (sender, arg) => Context_Event_Callback(context, nameof(context.PostLogRequest));
            context.PostMapRequestHandler += (sender, arg) => Context_Event_Callback(context, nameof(context.PostMapRequestHandler));
            context.PostReleaseRequestState += (sender, arg) => Context_Event_Callback(context, nameof(context.PostReleaseRequestState));
            context.PostRequestHandlerExecute += (sender, arg) => Context_Event_Callback(context, nameof(context.PostRequestHandlerExecute));
            context.PostResolveRequestCache += (sender, arg) => Context_Event_Callback(context, nameof(context.PostResolveRequestCache));
            context.PostUpdateRequestCache += (sender, arg) => Context_Event_Callback(context, nameof(context.PostUpdateRequestCache));
            context.PreSendRequestContent += (sender, arg) => Context_Event_Callback(context, nameof(context.PreSendRequestContent));
            context.PreSendRequestHeaders += (sender, arg) => Context_Event_Callback(context, nameof(context.PreSendRequestHeaders));
            context.ReleaseRequestState += (sender, arg) => Context_Event_Callback(context, nameof(context.ReleaseRequestState));
            context.RequestCompleted += (sender, arg) => Context_Event_Callback(context, nameof(context.RequestCompleted));
            context.ResolveRequestCache += (sender, arg) => Context_Event_Callback(context, nameof(context.ResolveRequestCache));
            context.UpdateRequestCache += (sender, arg) => Context_Event_Callback(context, nameof(context.UpdateRequestCache));

        }

        private void Context_Event_Callback(HttpApplication context, string mesg)
        {
            context?.Context?.Response?.Write($"{mesg}<br />");
        }
    }

    public class MyFilter : Stream
    {
        private readonly Stream _stream;
        private readonly MemoryStream _cacheStream;

        public override bool CanRead => _cacheStream.CanRead;

        public override bool CanSeek => _cacheStream.CanSeek;

        public override bool CanWrite => _cacheStream.CanWrite;

        public override long Length => _cacheStream.Length;

        public override long Position
        {
            get { return _cacheStream.Position; }
            set { _cacheStream.Position = value; }
        }

        public MyFilter(Stream stream)
        {
            _stream = stream;
            _cacheStream = new MemoryStream();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            //if (buffer.Length > 1000) return;

            _cacheStream.Write(buffer, offset, count);

            buffer = System.Text.Encoding.UTF8.GetBytes("------------------");
            _cacheStream.Write(buffer, 0, buffer.Length);
        }

        public override void Flush()
        {
            var buffer = _cacheStream.ToArray();
            _stream.Write(buffer, 0, buffer.Length);

            _stream.Flush();
            _cacheStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _cacheStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _cacheStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _cacheStream.Read(buffer, offset, count);
        }
    }
}