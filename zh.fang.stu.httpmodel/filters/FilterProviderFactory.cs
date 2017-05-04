namespace zh.fang.stu.httpmodel.filters
{
    using System;

    public class FilterProviderFactory
    {
        public static IFilterProvider GetProvider(string providerType = null)
        {
            if (string.IsNullOrWhiteSpace(providerType))
            {
                return new DefaultFilterProvider();
            }

            if (string.Equals(providerType, typeof(HttpApplicationFilterProvider).FullName))
            {
                return new HttpApplicationFilterProvider();
            }

            throw new NotSupportedException();
        }

        public static void Registe(IFilterProvider provider)
        {
            throw new NotImplementedException();
        }
    }
}
