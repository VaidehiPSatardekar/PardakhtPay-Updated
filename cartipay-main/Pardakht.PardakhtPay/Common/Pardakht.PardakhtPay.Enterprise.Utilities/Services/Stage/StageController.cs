//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using System;
//{
//    [Route("api/me")]
//    [ApiController]
//    [ApiExplorerSettings(IgnoreApi = true)]
//    //[Authorize]
//    public class StageController : Controller
//    {
//        private readonly IConfiguration configuration;
//        public StageController(IConfiguration configuration)
//        {
//            this.configuration = configuration;
//        }

//        [HttpPost("machine")]
//        public IActionResult Machine([FromBody]MachineModel request)
//        {
//            if (request.Name.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase))
//                return Ok(true);

//            return Ok(false);
//        }


//        [HttpPost("app-setting")]
//        public IActionResult ConnectionString([FromBody]AppSettingModel request)
//        {
//            if (request.KeyName.Split('.').Length == 2)
//            {
//                var config_value = configuration.GetSection(request.KeyName.Split('.')[0])[request.KeyName.Split('.')[1]];
//                if (config_value == null)
//                    return Ok(false);

//                if (!request.KeyValue.Equals(config_value, StringComparison.OrdinalIgnoreCase))
//                    return Ok(false);

//                return Ok(true);
//            }
//            else if (request.KeyName.Split('.').Length == 1)
//            {
//                var config_value = configuration.GetSection(request.KeyName).Value;
//                if (config_value == null)
//                    return Ok(false);

//                if (!request.KeyName.Equals(config_value, StringComparison.OrdinalIgnoreCase))
//                    return Ok(false);

//                return Ok(true);

//            }
//            return Ok(false);

//        }
//    }
//}
