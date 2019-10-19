namespace Phoenix.Domain {
    using System;
    using System.Collections.Generic;

    public abstract class ServiceBase<T> where T : class {
        public abstract bool Remove(T entity);
        public abstract bool Insert(T entity);
        public abstract IList<T> Search(Func<T, bool> func);
        public abstract IList<T> GetAll();
    }
}