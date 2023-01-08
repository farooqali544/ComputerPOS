using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using ComputerPOS.BusinessLayer;
using System.Diagnostics;

// ...

namespace ComputerPOS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void GridUI()
        {
            gvProducts.DataSource = BL.dtProducts;
            gvSales.DataSource = BL.dtSales;
            DataGridViewButtonColumn btnEdit = new DataGridViewButtonColumn();
            DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn();
            DataGridViewButtonColumn btnCheckout = new DataGridViewButtonColumn();

            btnEdit.HeaderText = "Edit Product";
            btnEdit.Text = "Edit";
            btnEdit.Name = "btnEditProduct";
            btnEdit.UseColumnTextForButtonValue = true;

            btnDelete.HeaderText = "Remove Product";
            btnDelete.Text = "Delete";
            btnDelete.Name = "btnDeleteProduct";
            btnDelete.UseColumnTextForButtonValue = true;

            btnCheckout.HeaderText = "Checkout Product";
            btnCheckout.Text = "Checkout";
            btnCheckout.Name = "btnCheckout";
            btnCheckout.UseColumnTextForButtonValue = true;

            gvProducts.Columns.Add(btnEdit);
            gvProducts.Columns.Add(btnDelete);
            gvProducts.Columns.Add(btnCheckout);
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            
            DAL.getProducts();
            DAL.getProductCategories(cmbCategories, cmbCategoriesFilter);
            GridUI();
            BL.fillCheckoutTable();
            DAL.getSales();
        }





        private void btnSubmit_Click(object sender, EventArgs e)
        {
            BL.productName = txtProductName.Text;
            BL.productDesc = txtProductDesc.Text;
            BL.productPrice = (int)numericProductPrice.Value;
            BL.productStock = (int)numericProductStock.Value;
            BL.productCategory = (int)cmbCategories.SelectedValue;
            BL.productWidth = (decimal)numericWidth.Value;
            BL.productHeight = (decimal)numericHeight.Value;
            BL.productWeight = (decimal)numericWeight.Value;

            if (txtEditingProductId.Text != String.Empty)
            {
                DAL.editProduct(int.Parse(txtEditingProductId.Text));
                tabControl.SelectedIndex = 0;
            }
            else
            {
                DAL.insertProduct();
            }
            DAL.getProducts();

        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtEditingProductId.Text != String.Empty)
            {
                btnSubmit.Text = "Edit Product";
                btnReset.Text = "Discard";

                if (tabControl.SelectedIndex != 1)
                {
                    clearEditingControls();
                }
            }

        }

        private object getColumnValue(int rowIndex, string columnName)
        {
            return gvProducts.Rows[rowIndex].Cells[columnName].Value;
        }

        private void editRow(int rowIndex)
        {
            if (rowIndex >= 0)
            {

                txtEditingProductId.Text = getColumnValue(rowIndex, "productId").ToString();
                txtProductName.Text = getColumnValue(rowIndex, "productName").ToString();
                txtProductDesc.Text = getColumnValue(rowIndex, "productDesc").ToString();
                numericProductStock.Value = (int)getColumnValue(rowIndex, "productStock");
                numericProductPrice.Value = (decimal)(getColumnValue(rowIndex, "productPrice"));
                cmbCategories.SelectedIndex = cmbCategories.FindString(getColumnValue(rowIndex, "categoryName").ToString());
                numericWidth.Value = (decimal)(getColumnValue(rowIndex, "productWidth"));
                numericHeight.Value = (decimal)(getColumnValue(rowIndex, "productHeight"));
                numericWeight.Value = (decimal)(getColumnValue(rowIndex, "productWeight"));

                //after setting values open tab add/edit products
                tabPage2.Text = "Edit Product";
                tabControl.SelectedIndex = 1;
            }
        }

        private void deleteRow(int rowIndex)
        {
            if (rowIndex >= 0)
            {
                DialogResult dr = MessageBox.Show("Are you sure?", "Delete Product", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    int deletingProductId = (int)getColumnValue(rowIndex, "productId");
                    DAL.deleteProduct(deletingProductId);
                    DAL.getProducts();
                }
            }
        }

        private void checkoutProduct(int rowIndex)
        {
            if (rowIndex >= 0)
            {
                int productId = (int)getColumnValue(rowIndex, "productId");
                int stock = (int)getColumnValue(rowIndex, "productStock");
                for (int i = 0; i < BL.dtCheckoutProducts.Rows.Count; i++)
                {
                    if ((int)BL.dtCheckoutProducts.Rows[i]["productId"] == productId)
                    {
                        BL.dtCheckoutProducts.Rows[i].Delete();
                    }
                }

                DataRow newRow = BL.dtCheckoutProducts.NewRow();
                DataRow existingRow = BL.dtProducts.Select("productId=" + productId, "productId, productName, productPrice")[0];

                int quantity = int.Parse(Microsoft.VisualBasic.Interaction.InputBox("Enter Quantity", "Integer Input", "1"));
                if (quantity > stock)
                {
                    MessageBox.Show("Quantity>Stock, Try another quantity");
                }
                else
                {


                    newRow["productId"] = existingRow["productId"];
                    newRow["productName"] = existingRow["productName"];
                    newRow["productPrice"] = existingRow["productPrice"];
                    newRow["quantity"] = quantity;

                    BL.dtCheckoutProducts.Rows.Add(newRow);
                    gvSale.DataSource = BL.dtCheckoutProducts;
                }
            }

        }

        private void gvProducts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gvProducts.Columns[e.ColumnIndex].Name == "btnEditProduct")
            {
                editRow(e.RowIndex);
            }

            if (gvProducts.Columns[e.ColumnIndex].Name == "btnDeleteProduct")
            {
                deleteRow(e.RowIndex);
            }
            if (gvProducts.Columns[e.ColumnIndex].Name == "btnCheckout")
            {
                checkoutProduct(e.RowIndex);
            }


        }

        private void clearEditingControls()
        {
            txtEditingProductId.Text = String.Empty;
            txtProductName.Text = "";
            txtProductDesc.Text = "";
            numericProductPrice.Value = 0;
            numericProductStock.Value = 0;
            numericWidth.Value = 0;
            numericHeight.Value = 0;
            numericWeight.Value = 0;
            cmbCategories.SelectedIndex = 0;
            btnSubmit.Text = "Submit";
            btnReset.Text = "Discard";
            tabPage2.Text = "Add Product";

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (txtEditingProductId.Text != String.Empty)
            {
                clearEditingControls();
                tabControl.SelectedIndex = 0;
            }
        }



        private void txtSearchOrcmbCategoriesFilter_Changes(object sender, EventArgs e)
        {
            BL.searchValue = txtSearch.Text;
            BL.selectedCategory = (int)cmbCategoriesFilter.SelectedValue;
            DAL.getProducts();
        }

        private void btnSubmitCategory_Click(object sender, EventArgs e)
        {
            if (txtAddCategory.Text.Length > 0)
            {
                DAL.addProductCategory(txtAddCategory.Text);
                DAL.getProductCategories(cmbCategories, cmbCategoriesFilter);
            }
        }

        private void PrintReceipt(object sender, PrintPageEventArgs e)
        {
            Font font = new Font("Times New Roman", 10);
            SolidBrush brush = new SolidBrush(Color.Black);
            e.Graphics.DrawString("Receipt", font, brush, 50, 20);


            e.Graphics.DrawString("Id", font, brush, 50, 70);
            e.Graphics.DrawString("Name", font, brush, 100, 70);
            e.Graphics.DrawString("Price", font, brush, 250, 70);
            e.Graphics.DrawString("QTY", font, brush, 300, 70);
            e.Graphics.DrawString("SubTotal", font, brush, 350, 70);

            decimal total = 0;
            int position = 100;

            foreach (DataRow row in BL.dtCheckoutProducts.Rows)
            {
                int productId = (int)row["productId"];
                string productName = (string)row["productName"];
                decimal price = (decimal)row["productPrice"];
                int quantity = (int)row["quantity"];
                decimal subtotal = price * quantity;
                total += subtotal;

                e.Graphics.DrawString(productId.ToString(), font, brush, 50, position);
                e.Graphics.DrawString(productName, font, brush, 100, position);
                e.Graphics.DrawString(price.ToString(), font, brush, 250, position);
                e.Graphics.DrawString(quantity.ToString(), font, brush, 300, position);
                e.Graphics.DrawString(subtotal.ToString(), font, brush, 350, position);
                position += 30;
            }


            e.Graphics.DrawString("Total: " + total, font, brush, 350, position);
        }

        private void btnGenerateReceipt_Click(object sender, EventArgs e)
        {
            DAL.checkoutProduct();
            DAL.getProducts();
            DAL.getSales();

            PrintDocument printDoc = new PrintDocument();
            PrintPreviewDialog printDialog = new PrintPreviewDialog();

            printDialog.Document = printDoc;
            printDoc.PrintPage += new PrintPageEventHandler(PrintReceipt);

            if (printDialog.ShowDialog()    == DialogResult.OK)
            {
                printDoc.Print();
            }
            BL.dtCheckoutProducts.Clear();

        }

        private void btnDiscardReceipt_Click(object sender, EventArgs e)
        {
            BL.dtCheckoutProducts.Clear();
        }


    }
}