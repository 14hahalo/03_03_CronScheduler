using OfficeOpenXml;
using Quartz;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;

namespace _03_03_CronScheduler
{
    public class Job : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            DataTable getList = GetData();
            if (getList.Rows.Count > 0)
            {
                FillExcel();
                EmailSender();
            }
            return Task.CompletedTask;
        }
        public static DataTable GetData()
        {
            string connection = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
            DataTable dt;
            SqlConnection con = new SqlConnection(connection);
            con.Open();

            using (SqlCommand cmd = new SqlCommand("SELECT * FROM UserInformations WHERE CreatedDate < GETDATE()-7", con)) // Your SQL query should placed in here
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
                con.Close();
                return dt;
            }
        }
        public static MemoryStream FillExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            MemoryStream ms;

            using (ExcelPackage package = new ExcelPackage())                           //Creating and Filling Excel worksheet
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet 1");
                DataTable GetDT = GetData();

                worksheet.Cells[1, 1].Value = "UserId";
                worksheet.Cells[1, 2].Value = "FirstName";
                worksheet.Cells[1, 3].Value = "LastName";
                worksheet.Cells[1, 4].Value = "Email";
                worksheet.Cells[1, 5].Value = "CreatedDate";
                worksheet.Cells[1, 6].Value = "Activated";

                if (GetDT.Rows.Count > 0)
                {
                    int i = 2;
                    foreach (DataRow dr in GetDT.Rows)
                    {
                        worksheet.Cells[i, 1].Value = Convert.ToInt32(dr["UserId"]);
                        worksheet.Cells[i, 2].Value = (dr["FirstName"].ToString());
                        worksheet.Cells[i, 3].Value = (dr["LastName"].ToString());
                        worksheet.Cells[i, 4].Value = (dr["Email"].ToString());
                        worksheet.Cells[i, 5].Value = Convert.ToDateTime(dr["CreatedDate"]).ToString();
                        worksheet.Cells[i, 6].Value = Convert.ToBoolean(dr["Activated"]);
                        i++;
                    }
                }
                ms = new MemoryStream(package.GetAsByteArray());
                return ms;
            }
        }
        public static void EmailSender()
        {
            using (MailMessage mail = new MailMessage())
            {
                MemoryStream attachment = FillExcel();
                var mailOut = _03_03_CronScheduler.Program.inputEmail;                                    //Sender E-Mail
                var pswMailOut = _03_03_CronScheduler.Program.inputPassword;                              //Sender E-mail's password
                var displayNameMailOut = _03_03_CronScheduler.Program.inputEmail;                         //Sender Name
                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential(mailOut, pswMailOut);
                client.Port = 587;
                try
                {
                    if (_03_03_CronScheduler.Program.inputEmail.EndsWith("@gmail.com"))
                    {
                        client.Host = "smtp.gmail.com";
                    }
                    else if (_03_03_CronScheduler.Program.inputEmail.EndsWith("@metadiag.com.tr"))
                    {
                        client.Host = "smtp.yandex.com";
                    }
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = true;
                    mail.From = new MailAddress(mailOut, displayNameMailOut);                                //Receiver Email and name should replace with 'mailOut' and                                                                                                             'displayNameMailOut' variables
                    var addresses = "halil.toptas@metadiag.com.tr;halil.toptas@metadiag.com.tr;halil.toptas@metadiag.com.tr";
                    foreach (var address in addresses.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        mail.To.Add(address);
                    }
                    //  mail.To.Add(new MailAddress(mailOut));
                    mail.Subject = "Mail Title";
                    mail.Body = "Mail Text";
                    mail.Attachments.Add(new Attachment(attachment, $"{DateTime.Today.ToShortDateString()}_NewUsers.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));

                    client.Send(mail);
                    Console.WriteLine("E-mail gönderildi.");
                }
                catch (Exception)
                {
                    Console.WriteLine("Hatalı E-mail veya Şifre"); ;
                }
                
            }
        }
    }

}
