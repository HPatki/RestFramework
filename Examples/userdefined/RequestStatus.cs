using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RestApplication.userdefined
{
    [DataContract]
    class RequestStatus
    {
        [DataMember]
        private String message;
        [DataMember]
        private Int32 errorcode;
        [DataMember]
        private String key;
        [DataMember]
        private String iv;
        [DataMember]
        private Boolean isadmin;

        public String Message
        {
            get { return message; }
            set { message = value;  }
        }

        public Int32 ErrorCode
        {
            get { return errorcode; }
            set { errorcode = value; }
        }

        public String Key
        {
            get { return key; }
            set { key = value; }
        }

        public String Iv
        {
            get { return iv; }
            set { iv = value; }
        }

        public Boolean IsAdmin
        {
            get { return isadmin; }
            set { isadmin = value; }
        }
    }
}
