/*
 * Using the DAL Layer

Compile the above project to create DALLayer.dll.  This section shows how we can use the DAL layer for database operations in our projects.  Create a new project and add the reference to the DALLayer.dll in this project.  The following code shows how we can read data from a database table called "emp" using the DAL Layer.

Listing 5: Read data using the DAL Layer

IDBManager dbManager = newDBManager(DataProvider.SqlServer);
dbManager.ConnectionString =ConfigurationSettings.AppSettings[
  "ConnectionString"].ToString();
try
{
  dbManager.Open();
  dbManager.ExecuteReader("Select * fromemp ",CommandType.Text);
  while(dbManager.DataReader.Read())Response.Write(dbManager.
  DataReader["name"].ToString());
}
 
catch (Exception ex)
{
//Usual Code
}
 
finally
{
  dbManager.Dispose();
}
Note that we can read the connection string from the web.config file or we can hard code the same directly using the ConnectionString property.  It is always recommended to store the connection string in the web.config file and not hard code it in our code.

The following code shows how we can use the Execute Scalar method of the DBManager class to obtain a count of the records in the "emp" table.

 * 
 * 
Listing 6: Reading one value using Execute Scalar
 
IDBManager dbManager = newDBManager(DataProvider.OleDb);
dbManager.ConnectionString =ConfigurationSettings.AppSettings[
  "ConnectionString"].ToString();
try
{
  dbManager.Open();
  object recordCount =dbManager.ExecuteScalar("Select count(*) from
  emp ", CommandType.Text);
  Response.Write(recordCount.ToString());
}
 
catch (Exception ce)
{
//Usual Code
}
 
finally
{
  dbManager.Dispose();
}
The following code shows how we can invoke a stored procedure called "Customer_Insert" to insert data in the database using our DAL layer.

 * 
 * 
Listing 7: Inserting data using stored procedure
 
private void InsertData()
{
  IDBManager dbManager = new DBManager(DataProvider.SqlServer);
  dbManager.ConnectionString =ConfigurationSettings.AppSettings[
    "ConnectionString "].ToString();
  try
  {
    dbManager.Open();
    dbManager.CreateParameters(2);
    dbManager.AddParameters(0, "@id",17);
    dbManager.AddParameters(1,"@name", "Joydip Kanjilal");
   dbManager.ExecuteNonQuery(CommandType.StoredProcedure,
    "Customer_Insert");
  }
  catch (Exception ce)
  {
    //Usual code              
  }
  finally
  {
    dbManager.Dispose();
  }
}

*/