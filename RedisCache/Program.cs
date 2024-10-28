using StackExchange.Redis;

namespace RedisCache
{
    internal class Program
    {
        /// <summary>
        /// 5 种基础数据类型：String（字符串）、Hash（散列）、List（列表）、Set（集合）、Zset（有序集合）。
        /// 3 种特殊数据类型：HyperLogLog（基数统计）、Bitmap （位图）、Geospatial (地理位置)。
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //1.StackExchange.Redis

            #region String 字符串
            //key - value 键值对: value可以是序列化的数据

            var stringService = RedisHelper.StringService;
            var ddd = stringService.StringSet("key", "value", new TimeSpan(1, 1, 1, 1, 1));
            var ddd1 = stringService.StringGet("key");

            string key1 = "test1";
            //自增
            var inc = stringService.StringIncrement(key1, 1);
            //自减
            var dec = stringService.StringDecrement(key1, 1);
            #endregion

            #region Hash 哈希表
            //Hash: 类似dictionary，通过索引快速定位到指定元素的，耗时均等，跟string的区别在于不用反序列化，直接修改某个字段
            //    string的话要么是 001:序列化整个实体
            //    要么是 001_name: 001_pwd: 多个key - value
            //    Hash的话，一个hashid -{ key: value; key: value; key: value; }
            //    可以一次性查找实体，也可以单个，还可以单个修改

            var hashService = RedisHelper.HashService;
            string key2 = "test2";
            string dataKey = "dataKey";
            var a = hashService.HashSet(key2, dataKey, 1);
            var a2 = hashService.HashGet<int>(key2, dataKey);
            //自增
            var a3 = hashService.HashIncrement(key2, dataKey);
            //自减
            var a4 = hashService.HashDecrement(key2, dataKey);
            #endregion

            #region List 双向链表
            //Redis list的实现为一个双向链表，即可以支持反向查找和遍历，更方便操作，不过带来了部分额外的内存开销，
            //Redis内部的很多实现，包括发送缓冲队列等也都是用的这个数据结构。  
            //一般是左进右出或者右进左出

            var listService = RedisHelper.ListService;
            //模拟队列先进先出
            string key3 = "test3";
            listService.ListLeftPush(key3, "11");
            listService.ListLeftPush(key3, "22");
            listService.ListLeftPush(key3, "33");
            listService.ListLeftPush(key3, "44");
            string str1 = null;
            while ((str1 = listService.ListRightPop<string>(key3)) != null)
            {
                Console.WriteLine(str1);
            }
            #endregion

            #region Set 集合
            //Set：用哈希表来保持字符串的唯一性，没有先后顺序，存储一些集合性的数据
            //1.共同好友、二度好友
            //2.利用唯一性，可以统计访问网站的所有独立 IP

            var setService = RedisHelper.SetService;
            string key4 = "test4";
            setService.SetAdd(key4, "111");
            setService.SetAdd(key4, new List<string>() { "222", "333" });
            var all = setService.SetMembers<List<string>>(key4);
            all.ForEach(str =>
            {
                Console.WriteLine(str);
            });
            //还可以获取几个集合的交集、并集、差集

            #endregion

            #region SortedSet 有序集合
            var sortedSetService = RedisHelper.SortedSetService;
            //Sorted Sets是将 Set 中的元素增加了一个权重参数 score，使得集合中的元素能够按 score 进行有序排列
            //1.带有权重的元素，比如一个游戏的用户得分排行榜
            //2.比较复杂的数据结构，一般用到的场景不算太多

            var added = sortedSetService.SortedSetAdd("mySortedSet", new List<SortedSetEntry>
            {
                new SortedSetEntry("member1", 1),
                new SortedSetEntry("member2", 2),
                new SortedSetEntry("member3", 3)
            });
            //获取SortedSet的成员数：
            var count = sortedSetService.SortedSetLength("mySortedSet");
            //获取SortedSet的成员：
            var members = sortedSetService.SortedSetRangeByRankWithScores<List<SortedSetEntry>>("mySortedSet", 0, -1);
            //删除SortedSet的成员：
            var removed = sortedSetService.SortedSetRemove("mySortedSet", "member1");
            //更新SortedSet成员的分数：
            var updated = sortedSetService.SortedSetIncrement("mySortedSet", "member2", 1);
            #endregion

            //2.FreeRedis  todo...
        }
    }
}
