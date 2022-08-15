using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdoNetCourse
{
    public partial class Form1 : Form
    {
        //Connection String
        string ConStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public Form1()
        {
            InitializeComponent();
            PopulateCities();
            PopulateEmployeeStatus();
            GetCountOfEmployees();
            FillDataGridView();
        }

        //Execure Reader
        private void PopulateCities() 
        {
            SqlConnection con = new SqlConnection(ConStr);
            try
            {
                List<string> listofCities = new List<string>();
                
                string getCitiesSql = "SELECT * FROM [dbo].[Cities]";
                SqlCommand cmd = new SqlCommand(getCitiesSql, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    listofCities.Add(reader.GetString(1));
                }
                cmbCity.DataSource = listofCities;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally 
            {
                con.Close();
            }
        }

        //Execure Reader
        private void PopulateEmployeeStatus()
        {
            SqlConnection con = new SqlConnection(ConStr);
            try
            {
                List<string> listofEmployeeStatus = new List<string>();
                string getCitiesSql = "SELECT * FROM [dbo].[EmployeeStatus]";
                SqlCommand cmd = new SqlCommand(getCitiesSql, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    listofEmployeeStatus.Add(reader.GetString(1));
                }
                cmbIsActive.DataSource = listofEmployeeStatus;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        //Execute Scalar
        private void GetCountOfEmployees() 
        {
            SqlConnection con = new SqlConnection(ConStr);
            try
            {
                string getEmployeeCountSql = "SELECT COUNT(*) AS 'TotalEmployees' FROM [dbo].[Employees]";
                SqlCommand cmd = new SqlCommand(getEmployeeCountSql, con);
                con.Open();
                int EmployeeCount = Convert.ToInt32(cmd.ExecuteScalar());
                if (EmployeeCount > 0)
                {
                    lblTotalEmployees.Text = "Total Number of Employees at present is " + EmployeeCount;
                }
                else
                {
                    lblTotalEmployees.Text = "No Employees Hired Yet!";
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally 
            {
                con.Close();
            }
        }

        //Data Set and Data Adapter
        private void FillDataGridView() 
        {
            SqlConnection con = new SqlConnection(ConStr);
            try
            {
                //string getAllEmployeeSql = "SELECT * FROM Employees";
                //SqlDataAdapter da = new SqlDataAdapter(getAllEmployeeSql, ConStr);
                //DataSet ds = new DataSet();
                //da.Fill(ds);
                //dataGridView1.DataSource = ds.Tables[0];
                //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                //dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

                string getAllEmployeeSql = "[dbo].[GetEmployees]";
                SqlCommand cmd = new SqlCommand(getAllEmployeeSql, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = getAllEmployeeSql;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;



            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
            
        }

        //Execure Reader
        private void btnSelectEmployee_Click(object sender, EventArgs e)
        {
            //Connection
            SqlConnection con = new SqlConnection(ConStr);
            
            try
            {
                if (!string.IsNullOrEmpty(txtEmployeeId.Text))
                {
                    string SqlQuery = "SELECT * FROM [dbo].[Employees] WHERE Id=@ID";
                    //Command
                    SqlCommand cmd = new SqlCommand(SqlQuery, con);
                    cmd.Parameters.AddWithValue("@ID", txtEmployeeId.Text);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        txtFirstName.Text = reader.GetString(1);
                        txtLastName.Text = reader.GetString(2);
                        txtGender.Text = reader.GetString(3);
                        cmbCity.Text = reader.GetString(4);
                        cmbIsActive.Text = reader.GetString(5);
                    }
                }
                else
                {
                    MessageBox.Show("Employeed Id cannot be left blank");
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        //Execute NonQuery
        private void btnAddEmployee_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(ConStr);
            try
            {
                string addEmployeesSql = @"INSERT INTO [dbo].[Employees] VALUES(@FirstName,@LastName,@Gender,@City,@IsActive)";
                SqlCommand cmd = new SqlCommand(addEmployeesSql, con);
                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                cmd.Parameters.AddWithValue("@Gender", txtGender.Text);
                cmd.Parameters.AddWithValue("@City", cmbCity.SelectedValue);
                cmd.Parameters.AddWithValue("@IsActive", cmbIsActive.SelectedValue);
                con.Open();
                int rowAffected = cmd.ExecuteNonQuery();
                if (rowAffected > 0)
                {
                    MessageBox.Show("Record Inserted Successfully");
                    GetCountOfEmployees();
                    FillDataGridView();

                }
                else
                {
                    MessageBox.Show("Record was not Inserted ");
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
            
        }

        private void btnUpdateEmployee_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(ConStr);
            try
            {
                string updateEmpSql = @"UPDATE [dbo].[Employees]
                                        SET [FirstName]=@FirstName
                                        ,[LastName]=@LastName
                                        ,[Gender]=@Gender
                                        ,[City]=@City
                                        ,[IsActive]=@IsActive
                                        WHERE Id=@ID";
                SqlCommand cmd = new SqlCommand(updateEmpSql, con);
                cmd.Parameters.AddWithValue("@ID", txtEmployeeId.Text);
                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                cmd.Parameters.AddWithValue("@Gender", txtGender.Text);
                cmd.Parameters.AddWithValue("@City", cmbCity.Text);
                cmd.Parameters.AddWithValue("@IsActive", cmbIsActive.Text);
                con.Open();
                int rowAffected = cmd.ExecuteNonQuery();
                if (rowAffected > 0)
                {
                    MessageBox.Show("Record Updated Successfully.");
                    FillDataGridView();

                }
                else
                {
                    MessageBox.Show("Record was not updated.");
                }
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(ConStr);
            try
            {
                string deleteEmpSql = @"DELETE FROM [dbo].[Employees] WHERE Id=@ID";
                SqlCommand cmd = new SqlCommand(deleteEmpSql, con);
                cmd.Parameters.AddWithValue("@ID", txtEmployeeId.Text);
                con.Open();
                int rowAffected = cmd.ExecuteNonQuery();
                if (rowAffected > 0)
                {
                    MessageBox.Show("Record Deleted Successfully.");
                    FillDataGridView();

                }
                else
                {
                    MessageBox.Show("Record was not deleted.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
    }
}
