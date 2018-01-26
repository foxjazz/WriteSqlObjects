using System;

namespace WriteSqlObjects
{
    class Program
    {
        static void Main(string[] args)
        {
            //if (checkArgs("-qa", args))
            //{
            //    var sw = new SqlWriter("qa", "service");
            //    sw.WriteSql("procs");
            //    sw.WriteSql("views");
            //}
            //if (checkArgs("-prod",args))
            //{
            //    var swp = new SqlWriter("prod", "service");
            //    swp.WriteSql("procs");
            //    swp.WriteSql("views");
            //}
            if (checkArgs("-qa", args))
            {
                var sw = new SqlWriter("qa", "ImportData");
                sw.WriteSql("procs");
                sw.WriteSql("views");
            }
            if (checkArgs("-prod", args))
            {
                if (checkArgs("-importdata", args))
                {
                    var swp = new SqlWriter("prod", "ImportData");
                    swp.WriteSql("procs");
                    swp.WriteSql("views");
                }
                if (checkArgs("-statebridge", args))
                {
                    var swp = new SqlWriter("prod", "statebridge");
                    swp.WriteSql("procs");
                    swp.WriteSql("views");
                }
                if (checkArgs("-service", args))
                {
                    var swp = new SqlWriter("prod", "service");
                    swp.WriteSql("procs");
                    swp.WriteSql("views");
                }
            }

        }

        private static bool checkArgs(string argument, string[] args)
        {
            foreach (var s in args)
            {
                if (s == argument)
                    return true;
            }
            return false;
        }
    }
}
