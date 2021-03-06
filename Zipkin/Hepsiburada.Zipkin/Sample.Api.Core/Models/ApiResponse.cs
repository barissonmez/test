﻿using System.Net;
using System.Runtime.Serialization;

namespace Sample.Api.Core.Models
{
    public class ApiResponse<T>
    {
        public ApiResponse(HttpStatusCode statusCode, T result, string errorMessage = null)
        {
            StatusCode = (int)statusCode;
            Result = result;
            ErrorMessage = errorMessage;
        }

        public ApiResponse()
        {

        }

        [DataMember]
        public string Version => "1.0";

        [DataMember]
        public int StatusCode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ErrorMessage { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public T Result { get; set; }
    }
}
