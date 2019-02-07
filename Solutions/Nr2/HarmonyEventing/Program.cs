using System;
using System.IO;
using System.Text;

namespace Leander.Nr1.HarmonyEventing
{
    public class AAA
    {
        private int _aaa;

        public int GetAAA { get { return _aaa; } }

        public AAA(int n)
        {
            _aaa = n;
        }
    }

    public class BBB : AAA
    {
        private int _bbb;

        public int GetBBB { get { return _bbb; } }

        public BBB(int m, int n)
           : base(m)
        {
            _bbb = n;
        }
    }

    public class Jonas : IMessage
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Test4();
        }

        public static void Test1()
        {
            Notifications.Publish(new JournalsExported());
        }

        public static void Test2()
        {
            AAA aaa = new AAA(5);
            BBB bbb = new BBB(3, 7);
            Type typeAAA = typeof(AAA);
            Type typeBBB = typeof(BBB);
            bool b1 = typeAAA.IsAssignableFrom(typeBBB);
            bool b2 = typeBBB.IsAssignableFrom(typeAAA);

            int a = aaa.GetAAA;

            aaa = bbb;

            a = aaa.GetAAA;
        }

        public static void Test3()
        {
            MemoryStream memoryStream = new MemoryStream(new byte[] { (byte)65, (byte)66, (byte)0, (byte)1 });
            string str = new UTF8Encoding().GetString(memoryStream.ToArray());
        }

        public static void Test4()
        {
            Jonas jonas1 = new Jonas();
            jonas1.Name = "Carl Jonas Leander";
            jonas1.Age = 49;
            string xml = SerializationHelpers.SerializeObject(jonas1, typeof(Jonas));
            object[] objects = (object[])SerializationHelpers.DeserializeXml(xml, typeof(Jonas));
            Jonas jonas2 = (Jonas)objects[0];
            string name = jonas2.Name;
            int age = jonas2.Age;
        }
    }
}

