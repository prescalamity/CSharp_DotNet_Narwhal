using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CSSharpTools
{
        /// <summary>
        /// 只读的表示键和值（字典）的集合。Lugiyan
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        public  class  ReadOnlyDictionary<TKey, TValue>: IReadOnlyDictionary<TKey, TValue> 
        {
                /// <summary>
                /// 用于保存被包装的普通字典实例。
                /// </summary>
                private  Dictionary<TKey, TValue> _dictionary;


                public ReadOnlyDictionary()
                {
                        _dictionary = new Dictionary<TKey, TValue>();
                }

                public TValue this[TKey key]  =>_dictionary [key]; 

                public IEnumerable<TKey> Keys => _dictionary.Keys;

                public IEnumerable<TValue> Values => _dictionary.Values;

                public int Count => _dictionary.Count;

                public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

                public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

                public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) =>  _dictionary.TryGetValue(key, out value);
                
                IEnumerator IEnumerable.GetEnumerator() =>  ((IEnumerable)_dictionary).GetEnumerator();


                public void Add(TKey key, TValue value) => _dictionary.Add(key, value);

                ///// <summary>
                ///// 从字典中移除所指定的键的值。
                ///// 此实现总是引发<see cref="NotSupportedException"/>异常。
                ///// </summary>
                ///// <param name="key">要移除的元素的键。</param>
                ///// <returns>如果成功找到并移除该元素，则为 true；否则为 false。
                ///// 如果在字典中没有找到 key，此方法则返回 false。</returns>
                //bool IDictionary<TKey, TValue>.Remove(TKey key)
                //{
                //        throw new NotSupportedException();
                //}



                ///// <summary>
                ///// 从特定的数组索引开始，将集合中的元素复制到一个数组中。
                ///// </summary>
                ///// <param name="array">作为从集合复制的元素的目标位置的一维数组。
                ///// 该数组必须具有从零开始的索引。</param>
                ///// <param name="arrayIndex"><see cref="P:array"/>中从零开始的索引，从此处开始复制。</param>
                //public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
                //{
                //        _dictionary.CopyTo(array, arrayIndex);
                //}

                ///// <summary>
                ///// 获取一个值，该值指示集合是否为只读。
                ///// 此实现总是返回true。
                ///// </summary>
                //public bool IsReadOnly
                //{
                //        get { return true; }
                //}

                ///// <summary>
                ///// 从集合中移除特定对象的第一个匹配项。
                ///// 此实现总是引发<see cref="NotSupportedException"/>异常。
                ///// </summary>
                ///// <param name="item">要从集合中移除的对象。</param>
                ///// <returns>如果已从集合中成功移除<see cref="P:item"/>，则为true；否则为false。
                ///// 如果在原始集合中没有找到<see cref="P:item"/>，该方法也会返回 false。</returns>
                //bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
                //{
                //        throw new NotSupportedException();
                //}
                //#endregion

        }


}










