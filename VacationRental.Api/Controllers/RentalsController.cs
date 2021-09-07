using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.ApplicationServices;
using VacationRental.Common;
using VacationRental.Domain.Entities;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _rentalService;
        private readonly IMapper _mapper;

        public RentalsController(IRentalService rentalService,
                                  IMapper mapper)
        {
            _rentalService = rentalService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public ActionResult<RentalViewModel> Get(int rentalId)
        {
            OperationResult<Rental> result = _rentalService.GetRental(rentalId);

            if (result == null) throw new Exception("Internal Server Error.");

            if (!result)
            {
                if (result.HasErrors()) return BadRequest(result.Errors);

                if (result.IsException()) throw new Exception("Internal Server Error.");
            }

            if (result.Value == null) return NotFound();

            return _mapper.Map<Rental, RentalViewModel>(result.Value);
        }

        [HttpPost]
        public ActionResult<ResourceIdViewModel> Post(RentalBindingModel model)
        {
            if (model == null) return BadRequest();
            if (model.Units <= 0) return BadRequest("Units musts be positive");
            if (model.PreparationTimeInDays < 0) return BadRequest("Preparation Time must be positive");

            var rentalNew = _mapper.Map<RentalBindingModel, Rental>(model);

            OperationResult<Rental> result = _rentalService.Create(rentalNew);

            if (result == null) throw new Exception("Internal Server Error.");

            if (!result)
            {
                if (result.HasErrors()) return BadRequest(result.Errors);

                if (result.IsException()) throw new Exception("Internal Server Error.");
            }

            if (result.Value == null) throw new Exception("Internal Server Error.");

            return _mapper.Map<Rental, ResourceIdViewModel>(result.Value);            
        }

        [HttpPut]
        [Route("{rentalId:int}")]
        public IActionResult Put(int rentalId, RentalBindingModel model)
        {
            if (model == null) return BadRequest();
            if (model.Units <= 0) return BadRequest("Units musts be positive");
            if (model.PreparationTimeInDays < 0) return BadRequest("Preparation Time must be positive");

            var rentalUpdate = _mapper.Map<RentalBindingModel, Rental>(model);

            OperationResult result = _rentalService.Update(rentalId, rentalUpdate);

            if (result == null) throw new Exception("Internal Server Error.");

            if (!result)
            {
                if (result.HasErrors()) return BadRequest(result.Errors);

                if (result.IsException()) throw new Exception("Internal Server Error.");
            }

            return NoContent();
        }
    }
}
