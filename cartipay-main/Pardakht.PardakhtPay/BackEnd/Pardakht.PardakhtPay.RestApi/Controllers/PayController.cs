//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Pardakht.PardakhtPay.Application.Interfaces;
//using Pardakht.PardakhtPay.Shared.Models.WebService;

//namespace Pardakht.PardakhtPay.RestApi.Controllers
//{
//    public class PayController : Controller
//    {
//        ITransactionService _TransactionService = null;
//        ILogger<TransactionController> _Logger = null;

//        public PayController(ITransactionService transactionService,
//            ILogger<TransactionController> logger)
//        {
//            _TransactionService = transactionService;
//            _Logger = logger;
//        }

//        [HttpPost]
//        public async Task<IActionResult> Index(string token)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var result = await _TransactionService.GetPaymentInformation(new PaymentInformationRequest()
//            {
//                Token = token
//            });

//            TransactionPaymentInformationResponse response = new TransactionPaymentInformationResponse(result.Result);

//            if (response.ResultCode != TransactionResultEnum.Success)
//            {
//                return BadRequest(response.ResultDescription);
//            }

//            response.CardNumber = result.Item.CardNumber;
//            response.CardHolderName = result.Item.CardHolderName;
//            response.Amount = result.Item.Amount;
//            response.ReturnUrl = result.Item.ReturnUrl;
//            response.Token = token;

//            return View(response);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Complete([FromBody]CompletePaymentRequest request)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            string extensions = string.Empty;

//            var result = await _TransactionService.CompletePayment(request);

//            if (result.ResultCode == TransactionResultEnum.Success || result.ResultCode == TransactionResultEnum.TransactionNotConfirmed)
//            {
//                return View(result);
//            }

//            return View("Error");
//        }

//        [HttpPost]
//        public async Task<IActionResult> Cancel(CancelPaymentRequest request)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var result = await _TransactionService.CancelTransaction(request.Token);

//            var response = new CompletePaymentResponse(result.Result);

//            if (result.Item != null)
//            {
//                response.ReturnUrl = result.Item.ReturnUrl;
//                response.Token = request.Token;

//                return View("Complete", response);
//            }

//            return View("Error");
//        }
//    }
//}