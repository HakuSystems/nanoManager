using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nanoManager.Api
{
    public class NanoUserData
    {
        public string ID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int Permission { get; set; }
        public bool IsVerified { get; set; }
        public bool IsPremium { get; set; }
    }

    public class APILoginData
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class BaseResponse<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class LoginResponse
    {
        public string AuthKey { get; set; }
    }

    public class CreateRedeemData
    {
        public int Type { get; set; }

        public int Amount { get; set; }
    }
    public class CreateRedeemResponse
    {
        public List<string> Codes { get; set; }
    }
}