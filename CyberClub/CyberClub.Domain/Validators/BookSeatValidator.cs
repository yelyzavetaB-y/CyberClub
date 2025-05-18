using CyberClub.Domain.Interfaces;
using CyberClub.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Validators
{
    public class BookSeatValidator : ICustomValidator<Booking>
    {
        public ValidationResult Validate(Booking model)
        {
            var errors = new List<string>();

            if (model == null)
            {
                errors.Add("Model is null!");
                return new ValidationResult(String.Join("; ", errors));
            }

            if (model.StartTime < DateTime.Now)
            {
                errors.Add("The ordering time should be in the future tense.");
            }

            if(model.SeatId <= 0)
            {
                errors.Add("Seat must be selected.");
            }
            if (model.Duration <= 60) 
            {
                errors.Add("Duration must be greater than 60");
            }
            //...

            return errors.Count == 0 ? ValidationResult.Success : new ValidationResult(String.Join("; ", errors));
        }
    }
}
