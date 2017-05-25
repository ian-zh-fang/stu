namespace zh.fang.stu._3thr.client
{
    using System;

    public class ExecuteHandleFactory
    {
        public IExecuteHandle GetHandle(Type type)
        {
            if (null == type)
            {
                throw new ArgumentNullException("type");
            }

            var handleType = typeof(IExecuteHandle);
            if (TypeChecking(handleType, type))
            {
                var instance = Activator.CreateInstance(type);
                return (IExecuteHandle)instance;
            }

            throw new NotSupportedException(string.Format("{0} is not conver to type {1}", type.FullName, handleType.FullName));
        }

        private bool TypeChecking(Type handleType, Type type)
        {
            if (string.Equals(handleType.FullName, type.FullName))
            {
                return true;
            }

            if (type.IsSubclassOf(handleType))
            {
                return true;
            }

            if (handleType.IsAssignableFrom(type))
            {
                return true;
            }

            return false;
        }

        public IExecuteHandle GetHandle<THandle>()
        {
            return GetHandle(typeof(THandle));
        }
    }
}