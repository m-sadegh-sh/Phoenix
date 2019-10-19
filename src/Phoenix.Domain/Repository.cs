namespace Phoenix.Domain {
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Linq;

    using Phoenix.Infrastructure;

    public abstract class Repository<T> where T : class {
        protected readonly DbContext Context = new DbContext();

        public IList<T> GetAll() {
            return Context.GetTable<T>().ToList();
        }

        public IList<T> GetAll(Func<T, bool> func) {
            return Context.GetTable<T>().Where(func).ToList();
        }

        public abstract T Get(T entity);

        public void DeleteAndSubmit(T entity) {
            if (entity != null) {
                SafeAttach(entity);
                Context.GetTable<T>().DeleteOnSubmit(entity);
                Context.SubmitChanges();
            }
        }

        public void InsertOrUpdateAndSubmit(T entity) {
            if (entity == null)
                return;

            var origin = Get(entity);
            if (origin != null) {
                SafeAttach(entity);
                Context.Refresh(RefreshMode.KeepCurrentValues, entity);
            } else
                Context.GetTable<T>().InsertOnSubmit(entity);
            Context.SubmitChanges();
        }

        private void SafeAttach(T entity) {
            try {
                if (entity != null)
                    Context.GetTable<T>().Attach(entity);
            } catch (Exception exception) {
                Logger.Write(exception);
            }
        }

        public T Get(Func<T, bool> predicate) {
            return GetAll(predicate).FirstOrDefault();
        }
    }
}