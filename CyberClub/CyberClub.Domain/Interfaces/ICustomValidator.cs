using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Interfaces
{
    public interface ICustomValidator<T>
    {
        ValidationResult Validate(T model);
    }
}
