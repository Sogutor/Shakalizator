using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shakalizator.Extensions
{
    public static class ListExtension
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            var obColl = new ObservableCollection<T>();
            foreach (var item in collection)
            {
                obColl.Add(item);
            }
                return obColl;
        }
    }
}
