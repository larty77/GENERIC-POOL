namespace GPool.src
{

    public class Pool<T> where T : IPoolable
    {
        private static Action<string>? _notify;

        private static Dictionary<Type, List<T>> _pool = new();

        public static Action<string>? Notify { get => _notify; set => _notify = value; }

        private Pool() { }

        public static void CreatePool(int count)
        {
            if (IsTypeInPool() is true)
                throw new Exception($"Objects[{GetGenericType()}] also created in Pool!");

            List<T> objects = new();
            _pool.Add(GetGenericType(), objects);

            for (int i = 0; i < count; i++)
                CreateObject();
        }

        private static T CreateObject()
        {
            T createdObject = (T)Activator.CreateInstance(GetGenericType())!;
            _pool[GetGenericType()].Add(createdObject);

            Notify?.Invoke($"{GetGenericType().Name} Created! \n\tID: [{_pool[GetGenericType()].IndexOf(createdObject)}]");

            return createdObject;
        }

        public static T GetFreeElement()
        {
            if (HasFreeElement(out T element))
            {
                Notify?.Invoke($"{GetGenericType().Name} Taked! \n\tID: [{_pool[GetGenericType()].IndexOf(element)}]");

                element.Take();
                return element;
            }

            Notify?.Invoke($"{GetGenericType().Name} Free Element Not Found! Created New!");

            element = CreateObject();
            element.Take();

            return element;
        }

        private static bool HasFreeElement(out T element)
        {
            foreach (var poolElement in _pool[GetGenericType()])
            {
                if (poolElement.IsActive is false)
                {
                    element = poolElement;

                    return true;
                }
            }

            element = default!;
            return false;
        }

        public static void Release(T element)
        {
            Notify?.Invoke($"{GetGenericType().Name} Reset! \n\tID: [{_pool[GetGenericType()].IndexOf(element)}]");

            element.Reset();
        }

        private static Type GetGenericType()
        {
            Type parameterType = typeof(T);
            return parameterType;
        }

        private static bool IsTypeInPool()
        {
            if (_pool.ContainsKey(GetGenericType()) is true)
                return true;

            return false;
        }
    }
}
