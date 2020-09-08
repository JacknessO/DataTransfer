using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Interface
{
    public interface  ITransferDBTask
    {
        void InitTask();
        void RunTask(DateTime currentTime);
        void RunTaskException(DateTime currentTime, Exception exception); 
    }
}
