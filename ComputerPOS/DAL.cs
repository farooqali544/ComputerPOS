using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using ComputerPOS.BusinessLayer;
using System.Diagnostics;

namespace ComputerPOS
{
    internal class DAL
    {

        private static readonly string connectionString = @"Data Source=FAROOQ-PC\SQLEXPRESS;Initial Catalog=computerPos;Integrated Security=True";
        private static SqlConnection DbConn = new SqlConnection();

        private static void createConn()
        {
            try
            {
                if (DbConn.State == ConnectionState.Closed)
                {
                    DbConn.ConnectionString = connectionString;
                    DbConn.Open();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private static void readData(SqlCommand command, DataTable tableName)
        {
            try
            {
                createConn();
                command.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter ds = new SqlDataAdapter(command);

                tableName.Clear();
                ds.Fill(tableName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private static int executeQuery(SqlCommand command)
        {
            try
            {

                createConn();

                command.CommandType = CommandType.StoredProcedure;
                return command.ExecuteNonQuery();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void getProductCategories(ComboBox cmb, ComboBox cmbFilter)
        {
            SqlCommand getCategoriesCommand = new SqlCommand("GetProductCategories", DbConn);
            readData(getCategoriesCommand, BL.dtProductCategories);
            cmb.ValueMember = "categoryId";
            cmb.DisplayMember = "categoryName";
            cmb.DataSource = BL.dtProductCategories;



            DataTable dtFilteredCategories = BL.dtProductCategories.Copy();
            DataRow newRow = dtFilteredCategories.NewRow();
            newRow["categoryName"] = "All";
            newRow["categoryId"] = 0;
            dtFilteredCategories.Rows.InsertAt(newRow, 0);

            cmbFilter.ValueMember = "categoryId";
            cmbFilter.DisplayMember = "categoryName";
            cmbFilter.DataSource = dtFilteredCategories;

            cmbFilter.SelectedValue = 0;

        }

        public static void insertProduct()
        {
            SqlCommand insertCommand = new SqlCommand("InsertProduct", DbConn);
            insertCommand.Parameters.AddWithValue("@productName", BL.productName);
            insertCommand.Parameters.AddWithValue("@productDesc", BL.productDesc);
            insertCommand.Parameters.AddWithValue("@productPrice", BL.productPrice);
            insertCommand.Parameters.AddWithValue("@productStock", BL.productStock);
            insertCommand.Parameters.AddWithValue("@productCategory", BL.productCategory);
            insertCommand.Parameters.AddWithValue("@productWidth", BL.productWidth);
            insertCommand.Parameters.AddWithValue("@productHeight", BL.productHeight);
            insertCommand.Parameters.AddWithValue("@productWeight", BL.productWeight);

            int addedRowId = executeQuery(insertCommand);
            if (addedRowId >= 1)
            {
                MessageBox.Show("Inserted Product");
            }
            else
            {
                MessageBox.Show("Error Occured");
            }

        }



        public static void editProduct(int editingProductId)
        {
            SqlCommand editCommand = new SqlCommand("EditProduct", DbConn);
            editCommand.Parameters.AddWithValue("@productId", editingProductId);
            editCommand.Parameters.AddWithValue("@productName", BL.productName);
            editCommand.Parameters.AddWithValue("@productDesc", BL.productDesc);
            editCommand.Parameters.AddWithValue("@productPrice", BL.productPrice);
            editCommand.Parameters.AddWithValue("@productStock", BL.productStock);
            editCommand.Parameters.AddWithValue("@productCategory", BL.productCategory);
            editCommand.Parameters.AddWithValue("@productWidth", BL.productWidth);
            editCommand.Parameters.AddWithValue("@productHeight", BL.productHeight);
            editCommand.Parameters.AddWithValue("@productWeight", BL.productWeight);

            int rowEdited = executeQuery(editCommand);
            if (rowEdited == 1)
            {
                MessageBox.Show("Edited Product");
            }
            else
            {
                MessageBox.Show("Error Occured");
            }

        }

        public static void deleteProduct(int deletingProductId)
        {
            SqlCommand deleteCommand = new SqlCommand("DeleteProduct", DbConn);
            deleteCommand.Parameters.AddWithValue("@productId", deletingProductId);

            int rowDeleted = executeQuery(deleteCommand);
            if (rowDeleted == 1)
            {
                MessageBox.Show("Deleted Product");
            }
            else
            {
                MessageBox.Show("Error Occured");
            }

        }

        private static bool isNumber(string str)
        {
            bool isNumber = true;
            foreach (char c in str)
            {
                if (!Char.IsDigit(c))
                {
                    isNumber = false;
                    break;
                }
            }
            return isNumber;
        }

        public static void getProducts()
        {
            SqlCommand searchCommand = new SqlCommand("SelectProducts", DbConn);

            if (BL.searchValue.Length > 0)
            {

                if (!isNumber(BL.searchValue))
                {
                    searchCommand.Parameters.AddWithValue("@productId", -1); //-1 is not a product id for any product
                }
                else
                {
                    searchCommand.Parameters.AddWithValue("@productId", int.Parse(BL.searchValue));
                }
                searchCommand.Parameters.AddWithValue("@productName", BL.searchValue);
            }

            if (BL.selectedCategory != 0)
            {
                searchCommand.Parameters.AddWithValue("@productCategory", BL.selectedCategory);
            }

            readData(searchCommand, BL.dtProducts);
        }

        public static void addProductCategory(string categoryName)
        {

            SqlCommand addCategoryCommand = new SqlCommand("AddCategory", DbConn);
            addCategoryCommand.Parameters.AddWithValue("@categoryName", categoryName);

            int rowAdded = executeQuery(addCategoryCommand);
            if (rowAdded == 1)
            {
                MessageBox.Show("Added Category");
            }
            else
            {
                MessageBox.Show("Error Occured");
            }

        }

        public static void checkoutProduct()
        {
            decimal total = 0;
            foreach (DataRow row in BL.dtCheckoutProducts.Rows)
            {
                SqlCommand chkoutCommand = new SqlCommand("CheckoutProduct", DbConn);
                int productId = (int)row["productId"];
                string productName = (string)row["productName"];
                decimal price = (decimal)row["productPrice"];
                int quantity = (int)row["quantity"];
                decimal subtotal = price * quantity;
                total += subtotal;
                chkoutCommand.Parameters.AddWithValue("@productId", productId);
                chkoutCommand.Parameters.AddWithValue("@productPrice", price);
                chkoutCommand.Parameters.AddWithValue("@quantity", quantity);
                chkoutCommand.Parameters.AddWithValue("@subTotal", subtotal);


                executeQuery(chkoutCommand);

            }


        }

        public static void getSales()
        {
            SqlCommand getsalesCommand = new SqlCommand("SelectSales", DbConn);
            readData(getsalesCommand, BL.dtSales);

        }

    }
}
