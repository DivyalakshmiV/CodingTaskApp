using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTaskApp
{
    public class Student
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
        public Result Result { get; set; }
    }

    public enum Result
    {
        NA=0,
        PASS=1,
        FAIL=2
    }

    public enum Status
    {
        WAITING_FOR_RESULT = 0,
        GOT_RESULT = 1,
        FAILED_TO_GET_RESULT
    }
}
