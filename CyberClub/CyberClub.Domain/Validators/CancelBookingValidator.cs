using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Models;
using CyberClub.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Validators
{
    public class CancelBookingValidator : ICustomValidator<Booking>
    {
        public ValidationResult Validate(Booking model)
        {
            var errors = new List<string>();

            if (model != null)
            {
                return new ValidationResult("Booking is null");
            }

            if(model.Status == Status.Cancelled)
            {
                errors.Add("Booking is already cancelled");
            }
            
            if((model.StartTime - DateTime.Now).TotalMinutes < 30)
            {
                errors.Add("Cancelation is not allowed less than 30 minutes before the start time");
            }
            return errors.Count == 0 ? ValidationResult.Success : new ValidationResult(String.Join("; ", errors));

        }
    }
}
