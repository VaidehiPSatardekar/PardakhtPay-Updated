//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Pardakht.PardakhtPay.Application.Interfaces;
//using Pardakht.PardakhtPay.Shared.Models.WebService;

//namespace Pardakht.PardakhtPay.RestApi.Controllers
//{
//    [Route("api/language")]
//    [ApiController]
//    public class LanguageController : PardakhtPayBaseController
//    {
//        ILanguageService _Service = null;

//        public LanguageController(ILanguageService service, ILogger<LanguageController> logger):base(logger)
//        {
//            _Service = service;
//        }

//        [HttpGet]
//        public async Task<IActionResult> Get()
//        {
//            var result = await _Service.GetAllItemsAsync();

//            return ReturnWebResponse(result);
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> Get(int id)
//        {
//            var result = await _Service.GetItemById(id);

//            return ReturnWebResponse(result);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Post([FromBody]LanguageDTO item)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest();
//            }

//            var result = await _Service.InsertAsync(item);

//            return ReturnWebResponse(result);
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> Put(int id, [FromBody]LanguageDTO item)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            if (id != item.Id)
//            {
//                return BadRequest("Entity and identity is different");
//            }

//            var result = await _Service.UpdateAsync(item);

//            return ReturnWebResponse(result);
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var result = await _Service.DeleteAsync(id);

//            if (result.Success)
//            {
//                return Json("Operation completed successfully");
//            }

//            return BadRequest(result.Message);
//        }
//    }
//}