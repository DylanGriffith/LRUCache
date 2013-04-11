using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dylan.Cache
{
    public class LRUCache<K, V>
        where K : class
        where V : class
    {

        public static int DefaultMaxSize
        {
            get
            {
                return mDefaultMaxSize;
            }
        }

        /// <summary>
        /// Constructs a new LRUCache with max size LRUCache.DefaultMax
        /// </summary>
        public LRUCache()
        {
            mMaxSize = DefaultMaxSize;
            GenericConstructor();
        }

        /// <summary>
        /// Constructs a new LRUCache with max size maxSize
        /// </summary>
        /// <param name="maxSize">The max size of the LRU Cache</param>
        public LRUCache( int maxSize )
        {
            if ( maxSize < 1 )
            {
                throw new ArgumentException( "Argument maxSize must be at least 1!" );
            }
            mMaxSize = maxSize;
            GenericConstructor();
        }

        private void GenericConstructor()
        {
            // Make the dictionary twice as big as it needs to be
            // so as to be safe
            mCache = new Dictionary<K, Node<K, V>>( 2 * mMaxSize );
            mHead = null;
            mTail = null;
        }

        /// <summary>
        /// Gets the item indexed by the key if it is in the cache.
        /// </summary>
        /// <returns>The value paired with the key if it is in the cache
        ///  and null if it is not in the cache</returns>
        public V GetIfExists( K key )
        {
            if ( IsInCache( key ) )
            {
                Node<K, V> result = mCache[ key ];
                BringToHead( result );
                return result.Value;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Tries to  get the item from the cache.
        /// </summary>
        /// <param name="key">The key for the item.</param>
        /// <param name="value">The value of the item if it is in the cache and null otherwise.</param>
        /// <returns>Returns true if the item is in the cache and false otherwise.</returns>
        public bool TryGet( K key, out V value )
        {
            if ( IsInCache(key) )
            {
                value = GetIfExists( key );
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// Checks if the item is in the cache.
        /// </summary>
        public bool IsInCache( K key )
        {
            return mCache.ContainsKey( key );
        }

        /// <summary>
        /// Adds the key value pair to the cache
        /// </summary>
        public void Add( K key, V value )
        {
            if ( IsInCache( key ) )
            {
                // It's already in here
                BringToHead( mCache[ key ] );
            }
            else
            {
                if ( mCache.Count == mMaxSize )
                {
                    RemoveLeastRecentlyUsed();
                }
                Node<K,V> nodeValue = new Node<K,V>(key,value);
                mCache.Add( key, nodeValue );
                AddAsHead( nodeValue );
            }
        }

        private void AddAsHead( Node<K,V> newHead )
        {
            if ( mHead == null )
            {
                // List is currently empty so just add it
                mHead = newHead;
                mTail = newHead;
                newHead.Next = null;
                newHead.Previous = null;
            }
            else
            {
                // Add item to head
                newHead.Previous = null;
                newHead.Next = mHead;
                mHead.Previous = newHead;
                mHead = newHead;
            }
        }

        private void BringToHead( Node<K,V> newHead )
        {
            if ( newHead == mHead )
            {
                // Already the head so do nothing
            }
            else if ( newHead == mTail )
            {
                // Tail is moved to head
                mTail = newHead.Previous;
                mTail.Next = null;
                newHead.Next = mHead;
                mHead.Previous = newHead;
                newHead.Previous = null;
                mHead = newHead;
            }
            else
            {
                // Item is neither head nor tail, and is moved to head
                newHead.Previous.Next = newHead.Next;
                newHead.Next.Previous = newHead.Previous;
                newHead.Next = mHead;
                mHead.Previous = newHead;
                newHead.Previous = null;
                mHead = newHead;
            }
        }

        private void RemoveLeastRecentlyUsed()
        {
            if ( mCache.Count == 0 )
            {
                // Nothing to remove (this shouldn't happen)
                throw new NotSupportedException();
            }
            else if ( mCache.Count == 1 )
            {
                // Only one item so just remove it
                mCache.Remove( mTail.Key );
                mHead = null;
                mTail = null;
            }
            else
            {
                // Remove the tail
                mCache.Remove( mTail.Key );
                mTail = mTail.Previous;
                mTail.Next = null;
            }
        }

        #region Private members
        private static int mDefaultMaxSize = 100;
        private int mMaxSize;
        private Dictionary<K, Node<K,V>> mCache;
        private Node<K,V> mHead;
        private Node<K,V> mTail;
        #endregion
    }

    public class Node<K,V>
    {
        #region Public Members
        public K Key
        {
            get
            {
                return mKey;
            }
        }

        public V Value
        {
            get
            {
                return mValue;
            }
        }

        public Node<K,V> Next
        {
            get
            {
                return mNext;
            }
            set
            {
                mNext = value;
            }
        }

        public Node<K,V> Previous
        {
            get
            {
                return mPrevious;
            }
            set
            {
                mPrevious = value;
            }
        }
        #endregion

        #region Constructor
        public Node( K key, V value )
        {
            mKey = key;
            mValue = value;
            mNext = null;
            mPrevious = null;
        }
        #endregion

        #region Private members
        private K mKey;
        private V mValue;
        private Node<K,V> mNext;
        private Node<K,V> mPrevious;
        #endregion
    }
}
