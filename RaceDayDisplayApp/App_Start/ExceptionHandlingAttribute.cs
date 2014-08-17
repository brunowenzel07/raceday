//using RaceDayDisplayApp.Common;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Text;
//using System.Web.Http;
//using System.Web.Http.Filters;

//namespace RaceDayDisplayApp
//{
//    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
//    {
//        public override void OnException(HttpActionExecutedContext context)
//        {
//            //if (context.Exception is BusinessException)
//            //{
//            //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
//            //    {
//            //        Content = new StringContent(context.Exception.Message),
//            //        ReasonPhrase = "Exception"
//            //    });

//            //}

//            //Log Critical errors
//            Log.Instance.Error(context.ToString(), context.Exception);

//            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
//            {
//                Content = new StringContent("An error occurred, please check log file for more information"),
//                ReasonPhrase = "Critical Exception"
//            });
//        }
//    }
//}
