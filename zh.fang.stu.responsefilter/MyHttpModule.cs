namespace zh.fang.stu.responsefilter
{
    using System;
    using System.IO;
    using System.Web;

    public class MyHttpModule : IHttpModule
    {
        void IHttpModule.Dispose()
        {
            
        }

        void IHttpModule.Init(HttpApplication context)
        {
            context.BeginRequest += (sender, args) => {
                context.Context.Response.Filter = new MyFilter(context.Context.Response.Filter);
                context.Context.Response.Write("hello");
            };
        }
    }

    public class MyFilter:MemoryStream
    {
        private readonly Stream _stream;
        private readonly MemoryStream _cacheStream;

        public MyFilter(Stream stream)
        {
            _stream = stream;
            _cacheStream = new MemoryStream();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {

            _cacheStream.Write(buffer, offset, count);
        }

        public override void Flush()
        {
            //var buffer = _cacheStream.ToArray();
            //_stream.Write(buffer, 0, buffer.Length);

            _stream.Flush();
            _cacheStream.Flush();
            base.Flush();
        }

        public override void Close()
        {
            base.Close();
        }
    }
}