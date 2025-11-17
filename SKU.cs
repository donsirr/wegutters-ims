using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WEGutters
{
    public class SKU
    {
        private int SKU_ID;
        private string SKU_Code;

        public SKU (string skuCode)
        {
            //SKU_ID = skuID;
            SKUCode = skuCode;
        }

        public int SKUID
        {
            get { return SKU_ID; }
            set
            {
                if (value >= 1)
                {
                    SKU_ID = value;
                }
                else
                {
                    throw new ArgumentException("SKU ID cannot be 0 or lower");
                }
            }
        }

        public string SKUCode
        {
            get { return SKU_Code; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    SKU_Code = value;
                }
                else
                {
                    throw new ArgumentException("SKU Code cannot be empty");
                }
            }
        }

        public SKU getSKU()
        {
            return this;
        }
    }
}
