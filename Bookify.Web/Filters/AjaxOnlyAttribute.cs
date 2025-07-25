using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Bookify.Web.Filters
{
    public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            //1- this value get in Network/Headers in browser dev tools if you use ajax request from the application
            //   x-requested-with ==> XMLHttpRequest
            //2- if ypu navigate from urlBox write it /Category/Edit/5 ==> will Get Form but not Ajax request "Not has this value
            //   in Network/Headers ==> 'x-requested-with ==> XMLHttpRequest'"
            //3- So We Do it To Prevent the user from navigating to the page by urlBox
            //4- Go To Put This AttributeClass on All Action Method
            var Request = routeContext.HttpContext.Request;
            var isAjaxRequest = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            return isAjaxRequest;
        }
    }
}
