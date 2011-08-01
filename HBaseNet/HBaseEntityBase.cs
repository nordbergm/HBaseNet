using System;

namespace HBaseNet
{
    public abstract class HBaseEntityBase<T>
    {
        protected HBaseEntityBase(T data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data", "Entity data cannot be null.");
            }

            this.Load(data);
        }

        protected HBaseEntityBase()
        {
        }

        public virtual void Load()
        {
            T data = this.Read();

            if (data == null)
            {
                throw new InvalidOperationException("The entity no longer exists in the database.");
            }

            this.Load(data);
        }

        protected abstract T Read();
        protected abstract void Load(T data);
    }
}
