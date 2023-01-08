using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerPOS.BusinessLayer
{
    internal class BL
    {

        public static DataTable dtProducts = new DataTable();
        public static DataTable dtProductCategories = new DataTable();

        public static DataTable dtCheckoutProducts = new DataTable();

        public static DataTable dtSales = new DataTable();

        public static int selectedCategory { get; set; } = 0;

        public static int productId { get; set; }
        public static string productName { get; set; } = "";
        public static string productDesc { get; set; } = "";
        public static decimal productPrice { get; set; }
        public static int productStock { get; set; }
        public static int productCategory { get; set; }
        public static decimal productWidth { get; set; }
        public static decimal productHeight { get; set; }
        public static decimal productWeight { get; set; }

        public static string searchValue { get; set; } = "";


        public static void fillCheckoutTable()
        {
            dtCheckoutProducts.Columns.Add("productId", typeof(int));
            dtCheckoutProducts.Columns.Add("productName");
            dtCheckoutProducts.Columns.Add("productPrice", typeof(decimal));
            dtCheckoutProducts.Columns.Add("quantity", typeof(int));
            
        }

    }
}
