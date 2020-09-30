using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Models
{
    public class ConfigClearLog
    {
        private string lOGLOCATION = "";
        private int sAVETIME = 9999;

        public string LOGLOCATION
        {
            get
            {
                return lOGLOCATION;
            }

            set
            {
                lOGLOCATION = value;
            }
        }

        public int SAVETIME
        {
            get
            {
                return sAVETIME;
            }

            set
            {
                sAVETIME = value;
            }
        }
    }
}
