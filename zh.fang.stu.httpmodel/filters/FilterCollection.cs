namespace zh.fang.stu.httpmodel.filters
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    public class FilterCollection:NameObjectCollectionBase
    {
        public FilterCollection() : base() { }

        public FilterCollection(IEqualityComparer equalityComparer) : base(equalityComparer) { }

        public FilterCollection(int capacity) : base(capacity) { }

        public FilterCollection(int capacity, IEqualityComparer equalityComparer) : base(capacity, equalityComparer) { }

        public FilterCollection(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public void Add(IFilter filter)
        {
            BaseAdd(GetName(filter), filter);
        }

        public void Clear()
        {
            BaseClear();
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (BaseGetAllKeys().AsParallel().Any(t => t == name))
                BaseRemove(name);
        }

        public IEnumerable<IFilter> Filters
        {
            get { return BaseGetAllValues().Select(t => t as IFilter).Where(t => null != t); }
        }

        public IFilter this[int index]
        {
            get {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException(nameof(index));

                return BaseGet(index) as IFilter;
            }
        }

        public IFilter this[string name]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException(nameof(name));

                return BaseGet(name) as IFilter;
            }
        }

        public bool Has(IFilter filter)
        {
            return BaseGetAllKeys().Any(t => t == GetName(filter));
        }

        private string GetName(IFilter filter)
        {
            if (null == filter)
                throw new ArgumentNullException(nameof(filter));

            return filter.GetType().FullName;
        }
    }
}