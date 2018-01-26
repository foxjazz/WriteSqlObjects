using System;
using System.Collections.Generic;
using System.Text;

namespace WriteSqlObjects
{
    public class Procedure
    {
        public string schemaname { get; set; }
        public string objectName { get; set; }
        public int object_id { get; set; }
    }
}
