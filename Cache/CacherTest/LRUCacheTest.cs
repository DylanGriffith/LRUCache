using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dylan.Cache;

namespace CacheTest
{
    [TestClass]
    public class LRUCacheTest
    {
        [TestMethod]
        public void CacherTestEmpty()
        {
            LRUCache<string, string> cacher = new LRUCache<string, string>();
            Assert.IsFalse( cacher.IsInCache( "hello" ) );
            Assert.AreEqual( null, cacher.GetIfExists( "hello" ) );
        }

        [TestMethod]
        public void CacherTestSingleElement()
        {
            LRUCache<string, string> cacher = new LRUCache<string, string>();
            string key = "hello";
            string value = "world";
            cacher.Add( key, value );
            Assert.IsTrue( cacher.IsInCache( key ) );
            Assert.AreEqual( value, cacher.GetIfExists( key ) );
        }

        [TestMethod]
        public void CacherTestMaxSize()
        {
            LRUCache<string, string> cacher = new LRUCache<string, string>( 1 );
            string key = "hello";
            string value = "world";
            string key2 = "hello2";
            string value2 = "world2";
            cacher.Add( key, value );
            cacher.Add( key2, value2 );
            Assert.IsFalse( cacher.IsInCache( key ) );
            Assert.IsTrue( cacher.IsInCache( key2 ) );
            Assert.AreEqual( null, cacher.GetIfExists( key ) );
            Assert.AreEqual( value2, cacher.GetIfExists( key2 ) );
            string dummy;
            Assert.AreEqual( false, cacher.TryGet( key, out dummy ) );
        }

        [TestMethod]
        public void CacherTestMaxSize2()
        {
            LRUCache<string, string> cacher = new LRUCache<string, string>( 2 );
            string key1 = "hello";
            string value = "world";
            string key2 = "hello2";
            string value2 = "world2";
            string key3 = "another key";
            string value3 = "another value";

            // Add two to cache
            cacher.Add( key1, value );
            cacher.Add( key2, value2 );

            // Check both are in cache
            Assert.IsTrue( cacher.IsInCache( key1 ) );
            Assert.IsTrue( cacher.IsInCache( key2 ) );

            // Get the item so it should be most recently accessed
            string result;
            Assert.AreEqual( true, cacher.TryGet( key1, out result ) );
            Assert.AreEqual( value, result );

            // Assert key2 is removed when adding another
            cacher.Add( key3, value3 );
            Assert.IsTrue( cacher.IsInCache( key1 ) );
            Assert.IsFalse( cacher.IsInCache( key2 ) );
            Assert.IsTrue( cacher.IsInCache( key3 ) );
            Assert.AreEqual( value, cacher.GetIfExists( key1 ) );
            Assert.AreEqual( null, cacher.GetIfExists( key2 ) );
            Assert.AreEqual( value3, cacher.GetIfExists( key3 ) );
        }

        [TestMethod]
        public void CacherTestMaxSize3()
        {
            int maxSize = 5;
            LRUCache<string, string> cacher = new LRUCache<string, string>( maxSize );

            for ( int i = 0; i < maxSize; ++i )
            {
                cacher.Add( i.ToString(), "value: " + i.ToString() );
            }

            cacher.GetIfExists( ( 3 ).ToString() );

            cacher.Add( ( maxSize ).ToString(), "value: " + maxSize );
            cacher.Add( ( maxSize + 1 ).ToString(), "value: " + ( maxSize + 1 ).ToString() );
            cacher.Add( ( maxSize + 2 ).ToString(), "value: " + ( maxSize + 2 ).ToString() );
            cacher.Add( ( maxSize + 3 ).ToString(), "value: " + ( maxSize + 3 ).ToString() );

            // Check that 0,1,2,4 were removed but not 3
            Assert.IsFalse( cacher.IsInCache( ( 0 ).ToString() ) );
            Assert.IsFalse( cacher.IsInCache( ( 1 ).ToString() ) );
            Assert.IsFalse( cacher.IsInCache( ( 2 ).ToString() ) );
            Assert.IsTrue( cacher.IsInCache( ( 3 ).ToString() ) );
            Assert.IsFalse( cacher.IsInCache( ( 4 ).ToString() ) );

            // Check that the cache is getting the right values
            Assert.AreEqual( null, cacher.GetIfExists( ( 0 ).ToString() ) );
            Assert.AreEqual( null, cacher.GetIfExists( ( 1 ).ToString() ) );
            Assert.AreEqual( null, cacher.GetIfExists( ( 2 ).ToString() ) );
            Assert.AreEqual( "value: " + ( 3 ).ToString(), cacher.GetIfExists( ( 3 ).ToString() ) );
            Assert.AreEqual( null, cacher.GetIfExists( ( 4 ).ToString() ) );
            Assert.AreEqual( "value: " + ( 3 ).ToString(), cacher.GetIfExists( ( 3 ).ToString() ) );
            Assert.AreEqual( "value: " + ( 3 ).ToString(), cacher.GetIfExists( ( 3 ).ToString() ) );
            Assert.AreEqual( "value: " + ( 3 ).ToString(), cacher.GetIfExists( ( 3 ).ToString() ) );
            Assert.AreEqual( "value: " + ( maxSize + 1 ).ToString(), cacher.GetIfExists( ( maxSize + 1 ).ToString() ) );
            cacher.Add( ( maxSize + 1 ).ToString(), "value: " + ( maxSize + 1 ).ToString() );
            Assert.AreEqual( "value: " + ( maxSize + 1 ).ToString(), cacher.GetIfExists( ( maxSize + 1 ).ToString() ) );

        }
    }
}
