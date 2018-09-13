using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;



namespace RestFramework.Interface
{
    interface IBroker
    {
        String GenericGet();

        String GenericPost();

        String GenericPut();

        String GenericDelete();
    }
}

