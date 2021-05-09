using System;

namespace Oceanus.Baloot
{
    using Oceanus.Core.Network;
    using Oceanus.Core.Utils;
    using System.Collections;
    using System.Threading.Tasks;
    using XXX;

    class User
    {
        public string name
        {
            get; set;
        }
        public string id
        {
            get; set;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            OceanusTest.run();
            Console.ReadKey();
        }
    }
}
