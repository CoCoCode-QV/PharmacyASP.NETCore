# Manager Pharmacy

The project revolves around a sophisticated system designed for online retailers. Leveraging cutting-edge technology, specifically ASP.NET Core 6.0 and SQL Server, this system specializes in classifying and recommending personalized products in the realm of personalized medicine. By harnessing customer data, the system tailors product recommendations, ensuring a unique and individualized shopping experience. The platform's backbone, ASP.NET Core 6.0, guarantees robustness and efficiency, while SQL Server provides a secure and reliable database environment. This project exemplifies the intersection of e-commerce, technology, and personalized customer service, aiming to redefine the online shopping landscape.

## Prerequisites:
To effectively utilize this project, ensure you have the following prerequisites installed:
- **SQL Server 2019 or higher:** You will need SQL Server installed to manage the database operations.

- **Visual Studio 2019 or higher:** Visual Studio is essential for development and building the project.

## Installation

**1. Clone the GitHub Repository:**

Clone the repository from GitHub to your local machine using the following command:
```bash
git clone <https://github.com/CoCoCode-QV/PharmacyASP.NETCore.git>
```
**2. Restore the Database:**
- Locate the **QlPharmacy.bak** file within the cloned repository.
- Restore the database using SQL Server Management Studio (SSMS) or the SQL Server command-line tools. This step is crucial for the correct functioning of the application.

**3. Configure Connection Strings:**
- Open the appsettings.json file in the project.
- Modify the ConnectionStrings section to reflect the correct SQL Server connection details:

```bash
"ConnectionStrings": {
  "DefaultConnection": "Server=<server_name>;Database=QlPharmacy;User Id=<username>;Password=<password>;"
}
```
**4.Google Authentication Configuration:**

- Update the Google authentication credentials in the appsettings.json file. Change ClientId and ClientSecret to match your Google account's credentials:

``` bash
"Authentication": {
  "Google": {
    "ClientId": "your_client_id",
    "ClientSecret": "your_client_secret"
  }
}
```
- Ensure that you replace "your_client_id" and "your_client_secret" with the actual credentials provided by Google for your application.

**5. Build and Run:**
- Open the solution in Visual Studio.
- Build the solution to ensure all dependencies are resolved.
- Run the application to start the system.

## Contact:

If you have any inquiries or concerns regarding the product, please feel free to reach out via email: hovan12022002@gmail.com. We value your feedback and are here to assist you. Thank you for your interest in our project!


