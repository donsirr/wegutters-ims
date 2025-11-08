using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WEGutters
{
    internal class SKU
    {
        private int SKU_ID;
        private string SKU_Code;
        private string SKU_Details;
        private int quantityPerBundle;

        public SKU (int skuID, string skuCode, string skuDetails, int quantityPerBundle)
        {
            SKU_ID = skuID;
            SKU_Code = skuCode;
            SKU_Details = skuDetails;
            this.quantityPerBundle = quantityPerBundle;
        }

        public int SKUId
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

        public string SKUDetails
        {
            get { return SKU_Details; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    SKU_Details = value;
                }
                else
                {
                    throw new ArgumentException("SKU Details cannot be empty");
                }
            }
        }

        public int QuantityPerBundle
        {
            get { return quantityPerBundle; }
            set
            {
                if (value >= 0)
                {
                    quantityPerBundle = value;
                }
                else
                {
                    throw new ArgumentException("Quantity per bundle cannot be negative");
                }
            }
        }

        public SKU getSKU()
        {
            return this;
        }
    }
}
