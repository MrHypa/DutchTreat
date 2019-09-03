using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DutchTreat.Controllers
{
    [Route("api/[Controller]")]
    public class OrdersController : Controller
    {
        private readonly IDutchRepository _repo;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMapper _mapper;

        public OrdersController(IDutchRepository dutchRepository, ILogger<OrdersController> logger, IMapper mapper)
        {
            _repo = dutchRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public IMapper Mapper { get; }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_mapper.Map<IEnumerable<Order>,IEnumerable<OrderViewModel>>(_repo.GetAllOrders()));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured retrieving all orders: {ex}");
                return BadRequest("An Error occured retrieving all orders");
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetOrder(int id)
        {
            try
            {
                var order = _repo.GetOrderById(id);

                if (order != null) return Ok(_mapper.Map<Order, OrderViewModel>(order));
                else return NotFound();
               

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured retrieving all orders: {ex}");
                return BadRequest("An Error occured retrieving all orders");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]OrderViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newOrder = _mapper.Map<OrderViewModel, Order>(model);

                    if (newOrder.OrderDate == DateTime.MinValue)
                    {
                        newOrder.OrderDate = DateTime.Now;
                    }

                    _repo.AddEntity(newOrder);

                    if (_repo.SaveAll())
                    {
  
                        return Created($"/api/order/{newOrder.Id}", _mapper.Map<Order,OrderViewModel>(newOrder)); 
                    }
                }
                else { return BadRequest(ModelState); }


            }
            catch (Exception ex)
            {

                _logger.LogError($"Failed to save new order: {ex}");
            }

            return BadRequest("Fail");
        }
    }
}
