using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Kumulos
{
    public class ApiResponse
    {
        public int responseCode;
        public string responseMessage;
        public object payload;
    }
}
