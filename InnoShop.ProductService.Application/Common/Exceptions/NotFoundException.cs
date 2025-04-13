using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.ProductService.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(Type type, object key)
            : base($"Entity {type.Name} with key {key} was not found.")
        {

        }
        public NotFoundException(Type type)
            : base($"No {type.Name} entities were found.")
        {

        }
    }
}
