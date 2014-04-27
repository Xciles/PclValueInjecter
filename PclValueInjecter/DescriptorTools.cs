namespace Xciles.PclValueInjecter
{
    public static class DescriptorTools
    {
        /// <summary>
        /// Seek for a PropertyDescriptor within the collection by Name
        /// </summary>
        /// <returns>the search result or null if nothing was found</returns>
        public static PropertyDescriptor GetByName(this PropertyDescriptorCollection collection, string name)
        {
            return collection.Find(name, false);
        }

        /// <summary>
        /// Seek for a PropertyDescriptor within the collection by Name with option to ignore case
        /// </summary>
        /// <returns>search result or null if nothing was found</returns>
        public static PropertyDescriptor GetByName(this PropertyDescriptorCollection collection,
                                                   string name, bool ignoreCase)
        {
            return collection.Find(name, ignoreCase);
        }

        /// <summary>
        /// Search for a PropertyDescriptor within the collection that is of a specific type T
        /// </summary>
        /// <returns>search result or null if nothing was found</returns>
        public static PropertyDescriptor GetByNameType<T>(this PropertyDescriptorCollection collection, 
            string name)
        {
            var p = collection.GetByName(name);
            if (p != null && p.PropertyType == typeof (T)) return p;
            return null;
        }
    }
}