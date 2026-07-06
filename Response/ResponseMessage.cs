using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inventory_v2.Response
{
    public class ResponseMessage<T>
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public int Count { get; set; } = 0;
        public T? Data { get; set; }
    }
}