using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Annotations;
using HttpResp = RestFramework.Annotations.HttpResponse;

using RestApplication.userdefined;

namespace RestApplication.controllers
{
    [RouteAttribute("/admin")] 
    class ComparisonTestWithSpringBoot
    {
        [EndPointAttribute(route:"/login",method:"POST",consumes:HttpdServer.Helpers.MediaType.APPLICATION_JSON, 
            produces:HttpdServer.Helpers.MediaType.APPLICATION_JSON)]
        public RequestStatus login ([BodyParam()] User dtls, HttpResp response)
        {
            RequestStatus sts = new RequestStatus();
            sts.Message = "Login success";
            response.StatusCode = 200;
            return sts;
        }
    }
}
