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
//    [Route("api/merchantbankaccount")]
//    [ApiController]
//    public class MerchantBankAccountController : PardakhtPayBaseController
//    {
//        IMerchantBankAccountService _Service = null;

//        public MerchantBankAccountController(IMerchantBankAccountService service, ILogger<MerchantBankAccountController> logger):base(logger)
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
//            var result = await _Service.GetEntityByIdAsync(id);

//            return ReturnWebResponse(result);
//        }

//        [HttpGet("getbymerchantid/{merchantId}")]
//        public async Task<IActionResult> GetByMerchantId(int merchantId)
//        {
//            var response = await _Service.GetItemsByMerchantId(merchantId);

//            return ReturnWebResponse(response);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Post([FromBody]MerchantBankAccountDTO item)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest();
//            }

//            var result = await _Service.InsertAsync(item);

//            return ReturnWebResponse(result);
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> Put(int id, [FromBody]MerchantBankAccountDTO item)
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