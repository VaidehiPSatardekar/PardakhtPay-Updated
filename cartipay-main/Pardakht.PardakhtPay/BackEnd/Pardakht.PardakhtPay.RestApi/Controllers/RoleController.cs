//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Pardakht.PardakhtPay.Shared.Models.Models;

//namespace Pardakht.PardakhtPay.RestApi.Controllers
//{
//    [Route("api/role")]
//    [ApiController]
//    public class RoleController : Controller
//    {
//        [HttpGet("get-permissions")]
//        public ActionResult GetPermissions()
//        {
//            return Json(new PermissionDTO[]{
//                new PermissionDTO()
//                {

//                } });
//        }

//        [HttpGet("get-permission-groups")]
//        public ActionResult GetPermissionGroups()
//        {
//            return Json(new PermissionGroupDTO[]{
//                new PermissionGroupDTO()
//                {
//                    Permissions = new List<PermissionDTO>()
//                } });
//        }
//    }
//}