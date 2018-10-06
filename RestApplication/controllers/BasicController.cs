using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Annotations;
using RestFramework.Transport;

namespace RestApplication.controllers
{
    [RouteAttribute ("/basic")]
    class BasicController
    {
        private static Random m_rndm = new Random();

        [EndPointAttribute ("/getName/{name}/dummy/{surname}")]
        public void getName ([PathVariable("name")]String name,
            [PathQueryVariable("middlename")]String middleName, 
            [PathVariable("surname")]String surname,
            [PathQueryVariable("salutation")]String salutation,
            HttpResponse response)
        {
            response.StatusCode = 200;
            
            //return "Hello There " + name + " " + middleName + " " + surname; 
        }
    }
}
