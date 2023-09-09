using DaemonsRunner.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaemonsRunner.Infrastructure.Extensions
{
    internal static class IListExtensions
    {
        #region --Generic Extensions--

        /// <summary>
        /// Removes all transfered objects from collection.
        /// </summary>
        /// <returns>
        /// true - if any item was deleted, false - if collection wasn't modified.
        /// </returns>
        public static bool RemoveAll<T> (this IList<T> collection, IEnumerable<T> deletingCollection)
        {
            ArgumentNullException.ThrowIfNull(deletingCollection);

            bool isCollectionModified = false;
            foreach (var deletingItem in deletingCollection)
            {
                if (collection.Remove(deletingItem))
                {
                    isCollectionModified = true;
                }
            }
            return isCollectionModified;
        }

        public static void AddRange<T>(this IList<T> collection, IEnumerable<T> addingCollection)
        {
            ArgumentNullException.ThrowIfNull(addingCollection);
          
            foreach (var item in addingCollection)
            {
                collection.Add(item);
            }
        }

        #endregion

        /// <summary>
        /// Add php-file if it not exists in collection.
        /// </summary>
        /// <returns>
        /// true - if any item was added, false - if collection wasn't modified.
        /// </returns>
        public static bool AddFileIfNotExist(this IList<ExecutableFile> files, IEnumerable<ExecutableFile> filesToAdd)
        {
            ArgumentNullException.ThrowIfNull(filesToAdd);

            bool isCollectionModified = false;
            foreach (var file in filesToAdd)
            {
                if (files.FirstOrDefault(d => d.Path == file.Path) is null)
                {
                    isCollectionModified = true;
                    files.Add(file);
                }
            }
            return isCollectionModified;
        }
    }
}
