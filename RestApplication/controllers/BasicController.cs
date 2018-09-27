using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Annotations;

namespace RestApplication.controllers
{
    [RouteAttribute ("/basic")]
    class BasicController
    {
        private static Random m_rndm = new Random();

        [EndPointAttribute ("/getName/{name}/dummy/{surname}")]
        public String getName ([PathVariable("name")]String name, [PathVariable("surname")]String surname,
            [PathQueryVariable("middlename")]String middleName, [PathQueryVariable("salutation")]String salutation)
        {
            return "Hello There " + name + " " + middleName + " " + surname; 
        }
    }
}
