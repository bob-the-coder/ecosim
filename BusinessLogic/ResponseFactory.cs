using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public static class ResponseFactory
    {
        public static Response CreateResponse(bool success, int errorCode)
        {
            return new Response(success, errorCode);
        }

        public static Response CreateResponse(bool success, ResponseCode errorCode)
        {
            return new Response(success, errorCode);
        }

        public static Response<T> CreateResponse<T>(bool success, int errorCode, T data = null) where T : class
        {
            return new Response<T>(success, errorCode, data);
        }

        public static Response<T> CreateResponse<T>(bool success, ResponseCode errorCode, T data = null) where T : class
        {
            return new Response<T>(success, errorCode, data);
        }

        public static bool InternetNotAvailable(Response response)
        {
            if (response == null)
            {
                return true;
            }

            return !response.Success && response.StatusCode.IsEqualToResponseCode(ResponseCode.ErrorNoInternet);
        }

        public static bool IsSuccessful(Response response)
        {
            return response != null && response.Success;
        }
    }
}
