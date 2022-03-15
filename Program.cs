using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            var obj1 = Pool.GetObject();
            obj1.DoSomething("obj1");

            Thread.Sleep(2000);
            var obj2 = Pool.GetObject();
            obj2.DoSomething("obj2");
            Pool.ReleaseObject(obj1);

            Thread.Sleep(2000);
            var obj3 = Pool.GetObject();
            obj3.DoSomething("obj3");
        }
        public class PooledObject
        {
            DateTime _createdAt = DateTime.Now;

            public DateTime CreatedAt
            {
                get { return _createdAt; }
            }

            public string TempData { get; set; }

            public void DoSomething(string name)
            {
                Console.WriteLine($"{name} : {TempData} is written on {CreatedAt}");
            }
        }
        public static class Pool
        {
            private static List<PooledObject> _available = new List<PooledObject>();
            private static List<PooledObject> _inUse = new List<PooledObject>();

            public static PooledObject GetObject()
            {
                lock (_available)
                {
                    if (_available.Count != 0)
                    {
                        PooledObject po = _available[0];
                        _inUse.Add(po);
                        _available.RemoveAt(0);
                        return po;
                    }
                    else
                    {
                        PooledObject po = new PooledObject();
                        _inUse.Add(po);
                        return po;
                    }
                }
            }

            public static void ReleaseObject(PooledObject po)
            {
                CleanUp(po);

                lock (_available)
                {
                    _available.Add(po);
                    _inUse.Remove(po);
                }
            }

            private static void CleanUp(PooledObject po)
            {
                po.TempData = null;
            }
        }

    }
    
}
