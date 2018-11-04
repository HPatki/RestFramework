using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace RestApplication.userdefined
{
    [DataContract]
    class User
    {
        [DataMember]
        private String user;
        [DataMember]
        private String passwd;
        private String mobile;
        private String cntrycd;
        private Boolean isadmin;
        private String otp;

        public String UserName
        {
            get { return user; }
            set { user = value; }
        }

        public String Passwd
        {
            get { return passwd; }
            set { passwd = value; }
        }

        public String Mobile
        {
            get { return mobile; }
            set { mobile = value; }
        }

        public String CntryCd
        {
            get { return cntrycd; }
            set { cntrycd = value; }
        }

        public Boolean IsAdmin
        {
            get { return isadmin; }
            set { isadmin = value; }
        }

        public String OTP
        {
            get { return otp; }
            set { otp = value; }
        }
    }
}
