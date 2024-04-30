namespace Core.Extensions
{
    public static class HashSetExtensions
    {
        public static void AddRange<T>(this HashSet<T> source, IEnumerable<T> items)
        {
            if (source is null || items is null)
                return;

            foreach (var item in items)
                source.Add(item);
        }
    }
}
