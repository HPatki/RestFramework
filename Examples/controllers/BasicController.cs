using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Annotations;
using RestFramework.Transport;
using RestFramework.Helpers;

namespace RestApplication.controllers
{
    [RouteAttribute ("/basic")]
    class BasicController
    {
        private static Random m_rndm = new Random();

        //PathVariable usage - plain message as JSON
        [EndPointAttribute (route:"/greet/{name}/dummy/{surname}",produces:MediaType.APPLICATION_JSON)]
        public String greetings ([PathVariable("name")]String name,
            [PathVariable("surname")]String surname)
        {
            return "How are you doing today " + name + " " + surname + "?"; 
        }

        //HeaderVariable (Plain) usage - Plain message as text/Plain
        [EndPointAttribute(route: "/greet/dummy", produces:MediaType.TEXT_PLAIN)]
        public String greetings1([HeaderParam("name")]String name,[HeaderParam("surname")]String surname)
        {
            return "How are you doing today " + name + " " + surname + "?";
        }

        //HeaderVariable (JSON) usage - Object JSON
        [EndPointAttribute(route: "/greet", consumes: MediaType.APPLICATION_JSON)]
        public String greetings2([HeaderParam("name")]Greeting greetings)
        {
            return "How are you doing today " + greetings.name + " " + greetings.surname + "?";
        }

        //BodyParam (JSON) usage
        [EndPointAttribute(route: "/greet/bodyparam", consumes: MediaType.APPLICATION_JSON, method:"POST")]
        public String greetings3([BodyParam("name")]Greeting greetings)
        {
            return "How are you doing today " + greetings.name + " " + greetings.surname + "?";
        }
    }
}
