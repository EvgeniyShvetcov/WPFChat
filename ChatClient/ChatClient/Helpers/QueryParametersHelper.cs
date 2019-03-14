using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Helpers
{
    static class QueryParametersHelper
    {
        public static string ToQueryString(IDictionary<string, string> queryParameters)
        {
            if (queryParameters == null) return "";
            var queryArray = (from key in queryParameters.Keys 
                              select string.Format("{0}={1}", WebUtility.UrlEncode(key), WebUtility.UrlEncode(queryParameters[key])))
                              .ToArray();
            return "?" + string.Join("&", queryArray);
        }
    }
}