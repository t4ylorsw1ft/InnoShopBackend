using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.ProductService.Application.Common.Exceptions
{
    public class AccessException : Exception
    {
        public AccessException() : base("Access restricted") { }
        public AccessException(string message) : base(message) { }
    }
}
