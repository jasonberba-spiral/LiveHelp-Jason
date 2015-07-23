﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;


namespace livehelp.Controllers
{
    public class LiveHelpController : Controller
    {
        //
        // GET: /LiveHelp/

        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        public ActionResult CreateCaseViaLiveHelp()
        {
            try
            {
                LiveHelp lh = new LiveHelp();
                LiveHelpMethods lhm = new LiveHelpMethods();

                lhm.WriteLogToFile("Log_" + DateTime.Now.ToString(), ConfigurationManager.AppSettings["logPathLocation"].ToString(), ConfigurationManager.AppSettings["logFileName"].ToString());
                lhm.WriteLogToFile("Call to Method Successful", ConfigurationManager.AppSettings["logPathLocation"].ToString(), ConfigurationManager.AppSettings["logFileName"].ToString());

                System.IO.StreamReader reader = new System.IO.StreamReader(HttpContext.Request.InputStream);

                lh = JsonConvert.DeserializeObject<LiveHelp>(reader.ReadToEnd());

                lhm.CreateCaseViaLiveHelp(lh);

                return View();
            }
            catch (Exception Ex)
            {
                LiveHelpMethods lhm = new LiveHelpMethods();
                lhm.WriteLogToFile("Exception in CreateCaseViaLiveHelp: " + Ex.Message + Ex.StackTrace, ConfigurationManager.AppSettings["logPathLocation"].ToString(), ConfigurationManager.AppSettings["logFileName"].ToString());

                return View();
            }
        }

    }

    public class LiveHelp
    {
        public LiveHelp()
        {
            //
            // TODO: Add constructor logic here
            //
        }


        public Int32 MemberID
        {
            get;
            set;
        }
        public string ParentID
        {
            get;
            set;
        }
        public Int32 RecordTypeID
        {
            get;
            set;
        }
        public Int32 Status
        {
            get;
            set;
        }
        public Int32 Origin
        {
            get;
            set;
        }
        public string Subject
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public bool HasCommentsUnreadByOwner
        {
            get;
            set;
        }
        public string CreatedBy
        {
            get;
            set;
        }
        public DateTime CreatedDate
        {
            get;
            set;
        }
        public string Ticket_ID
        {
            get;
            set;
        }
        public bool Complain
        {
            get;
            set;
        }
        public int Type
        {
            get;
            set;
        }
        public int Sub_Type
        {
            get;
            set;
        }
        public Int32 Referral_Detail
        {
            get;
            set;
        }
        public Int32 HowDoTheyFeelAboutUs
        {
            get;
            set;
        }
        public string Summary_Of_Chat
        {
            get;
            set;
        }
        public string LastModifiedBy
        {
            get;
            set;
        }

    }

    public class LiveHelpMethods
    {
        public LiveHelpMethods()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public void CreateCaseViaLiveHelp(LiveHelp lhInput)
        {
            try
            {
                LiveHelp lhRet = new LiveHelp();

                WriteLogToFile("Validating LiveHelp values.", ConfigurationManager.AppSettings["logPathLocation"].ToString(), ConfigurationManager.AppSettings["logFileName"].ToString());

                lhRet = ValidateLiveHelpInput(lhInput);

                WriteLogToFile("Validation Successful LiveHelp values.", ConfigurationManager.AppSettings["logPathLocation"].ToString(), ConfigurationManager.AppSettings["logFileName"].ToString());

                InsertLiveHelpData_to_TCase1(lhRet);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        private LiveHelp ValidateLiveHelpInput(LiveHelp liveHelpInput)
        {
            try
            {
                #region MemberID

                if (liveHelpInput.MemberID == 0)
                {
                    liveHelpInput.MemberID = 999999;
                }

                #endregion

                #region ParentID

                if (liveHelpInput.ParentID == null || liveHelpInput.ParentID == string.Empty)
                {
                    liveHelpInput.ParentID = "999999";
                }

                #endregion

                #region RecordTypeID

                switch (liveHelpInput.RecordTypeID)
                {
                    case 1:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                        break;
                    //Default the value to 1 "CS Cases"
                    default:
                        liveHelpInput.RecordTypeID = 1;
                        break;
                }

                #endregion

                #region Status

                switch (liveHelpInput.Status)
                {
                    case 1:
                    case 2:
                    case 4:
                    case 5:
                    case 6:
                        break;
                    //Default the value to 1 "Open"
                    default:
                        liveHelpInput.Status = 1;
                        break;
                }

                #endregion

                #region Origin

                switch (liveHelpInput.Origin)
                {
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 144:
                    case 145:
                    case 228:
                    case 229:
                    case 230:
                    case 231:
                    case 232:
                    case 332:
                    case 333:
                        break;
                    //Default the value to 333 "Test" temporarily
                    //Need to create new Origin ID for "LiveHelp"
                    default:
                        liveHelpInput.Origin = 333;
                        break;
                }

                #endregion

                #region Subject

                if (liveHelpInput.Subject == null || liveHelpInput.Subject == string.Empty)
                {
                    liveHelpInput.Subject = "LiveHelpCase_" + liveHelpInput.MemberID.ToString() + "_" + DateTime.Now.ToString("F");
                }

                #endregion

                #region Description

                if (liveHelpInput.Description == null || liveHelpInput.Description == string.Empty)
                {
                    liveHelpInput.Description = "LiveHelpCase for " + liveHelpInput.MemberID.ToString() + " at " + DateTime.Now.ToString("F");
                }

                #endregion

                #region HasCommentsUnreadByOwner

                //Currently defaulst to 0 - false

                #endregion

                #region CreatedBy

                if (liveHelpInput.CreatedBy == null || liveHelpInput.CreatedBy == string.Empty)
                {
                    liveHelpInput.CreatedBy = "user";
                }

                #endregion

                #region CreatedDate

                if (liveHelpInput.CreatedDate == DateTime.MinValue)
                {
                    liveHelpInput.CreatedDate = DateTime.Now;
                }

                #endregion

                #region Ticket_ID

                if (liveHelpInput.Ticket_ID == null || liveHelpInput.Ticket_ID == string.Empty)
                {
                    //Temporary value. Clarify with Ryan the default value
                    liveHelpInput.Ticket_ID = "TestLHTicket" + DateTime.Now.ToString();
                }

                #endregion

                #region Complain

                //Currently defaults to 0 - "No"

                #endregion

                #region Type

                switch (liveHelpInput.Type)
                {
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                        break;
                    //Default the value to 19 "Others"
                    default:
                        liveHelpInput.Type = 19;
                        break;
                }

                #endregion

                #region Sub_Type

                switch (liveHelpInput.Sub_Type)
                {
                    case 29:
                    case 30:
                    case 31:
                    case 32:
                    case 33:
                    case 34:
                    case 35:
                    case 36:
                    case 37:
                    case 38:
                    case 39:
                    case 40:
                    case 41:
                    case 42:
                    case 43:
                    case 44:
                    case 45:
                    case 46:
                    case 47:
                    case 48:
                    case 49:
                    case 50:
                    case 51:
                    case 52:
                    case 53:
                    case 54:
                    case 55:
                    case 56:
                    case 57:
                    case 58:
                    case 59:
                    case 60:
                    case 61:
                    case 62:
                    case 63:
                    case 64:
                    case 65:
                    case 66:
                    case 67:
                    case 68:
                    case 69:
                    case 70:
                    case 71:
                    case 72:
                    case 73:
                    case 74:
                    case 75:
                    case 76:
                    case 77:
                    case 78:
                    case 79:
                    case 80:
                    case 81:
                    case 82:
                    case 83:
                    case 84:
                    case 85:
                    case 86:
                    case 87:
                    case 88:
                    case 89:
                    case 90:
                    case 91:
                    case 92:
                    case 93:
                    case 94:
                    case 95:
                    case 96:
                    case 97:
                    case 98:
                    case 99:
                    case 100:
                    case 101:
                    case 102:
                    case 103:
                    case 104:
                    case 105:
                    case 106:
                    case 107:
                    case 108:
                    case 109:
                    case 110:
                    case 111:
                    case 112:
                    case 113:
                    case 114:
                    case 115:
                    case 147:
                    case 148:
                    case 149:
                    case 150:
                    case 151:
                    case 152:
                    case 153:
                    case 154:
                    case 155:
                    case 156:
                    case 157:
                    case 158:
                    case 159:
                    case 160:
                    case 161:
                    case 162:
                    case 163:
                    case 164:
                    case 165:
                    case 166:
                    case 167:
                    case 168:
                    case 169:
                    case 170:
                    case 171:
                    case 172:
                    case 173:
                    case 174:
                    case 175:
                    case 176:
                    case 177:
                    case 178:
                    case 179:
                    case 180:
                    case 181:
                    case 182:
                    case 183:
                    case 184:
                    case 185:
                    case 186:
                    case 187:
                        break;
                    //Default the value to 68 "Others"
                    default:
                        liveHelpInput.Sub_Type = 68;
                        break;
                }

                #endregion

                #region Referral_Detail

                switch (liveHelpInput.Referral_Detail)
                {
                    case 116:
                    case 0:
                        break;
                    //Default the value to 116 "Others"
                    default:
                        liveHelpInput.Referral_Detail = 116;
                        break;
                }

                #endregion

                #region HowDoTheyFeelAboutUs

                switch (liveHelpInput.HowDoTheyFeelAboutUs)
                {
                    case 117:
                    case 118:
                    case 119:
                        break;
                    //Default the value to 119 "No Comment"
                    default:
                        liveHelpInput.HowDoTheyFeelAboutUs = 119;
                        break;
                }

                #endregion

                #region Summary_Of_Chat

                if (liveHelpInput.Summary_Of_Chat == null)
                {
                    liveHelpInput.Summary_Of_Chat = "";
                }

                //Currently Allows an empty input

                #endregion

                #region LastModifiedBy

                if (liveHelpInput.LastModifiedBy == null || liveHelpInput.LastModifiedBy == string.Empty)
                {
                    liveHelpInput.LastModifiedBy = liveHelpInput.CreatedBy;
                }

                #endregion

                return liveHelpInput;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        private void InsertLiveHelpData_to_TCase1(LiveHelp lhInput)
        {
            try
            {
                WriteLogToFile("Preparing Insert to Database", ConfigurationManager.AppSettings["logPathLocation"].ToString(), ConfigurationManager.AppSettings["logFileName"].ToString());

                string tempConn = ConfigurationManager.AppSettings["connCRMMS1"].ToString();
                string connCRMstr = tempConn.Replace("[xxx]", "testpass");

                using (SqlConnection connection = new SqlConnection(connCRMstr))
                {
                    using (SqlCommand cmd = new SqlCommand("[dbo].[spTCase1Insert]", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@memberid", SqlDbType.BigInt).Value = lhInput.MemberID;
                        cmd.Parameters.Add("@ParentID", SqlDbType.VarChar, 15).Value = lhInput.ParentID;
                        cmd.Parameters.Add("@RecordTypeID", SqlDbType.BigInt).Value = lhInput.RecordTypeID;
                        cmd.Parameters.Add("@Status", SqlDbType.BigInt).Value = lhInput.Status;
                        cmd.Parameters.Add("@Origin", SqlDbType.BigInt).Value = lhInput.Origin;
                        cmd.Parameters.Add("@Subject", SqlDbType.NVarChar, 300).Value = lhInput.Subject;
                        cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = lhInput.Description;
                        cmd.Parameters.Add("@HasCommentsUnreadByOwner", SqlDbType.Bit).Value = lhInput.HasCommentsUnreadByOwner;
                        cmd.Parameters.Add("@createdBy", SqlDbType.VarChar, 50).Value = lhInput.CreatedBy;
                        cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = lhInput.CreatedDate;
                        cmd.Parameters.Add("@Ticket_ID", SqlDbType.VarChar, 50).Value = lhInput.Ticket_ID;
                        cmd.Parameters.Add("@Complain", SqlDbType.Bit).Value = lhInput.Complain;
                        cmd.Parameters.Add("@Type", SqlDbType.BigInt).Value = lhInput.Type;
                        cmd.Parameters.Add("@Sub_Type", SqlDbType.VarChar, 100).Value = lhInput.Sub_Type;
                        cmd.Parameters.Add("@Referral_Detail", SqlDbType.BigInt).Value = lhInput.Referral_Detail;
                        cmd.Parameters.Add("@HowDoTheyFeelAboutUs", SqlDbType.BigInt).Value = lhInput.HowDoTheyFeelAboutUs;
                        cmd.Parameters.Add("@Summary_Of_Chat", SqlDbType.NVarChar).Value = lhInput.Summary_Of_Chat;
                        cmd.Parameters.Add("@LastModifiedBy", SqlDbType.VarChar, 50).Value = lhInput.LastModifiedBy;

                        WriteLogToFile("Opening Database Connection...", ConfigurationManager.AppSettings["logPathLocation"].ToString(), ConfigurationManager.AppSettings["logFileName"].ToString());

                        connection.Open();

                        WriteLogToFile("Connection Successful.", ConfigurationManager.AppSettings["logPathLocation"].ToString(), ConfigurationManager.AppSettings["logFileName"].ToString());

                        object val = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();

                        WriteLogToFile("Insert Successful", ConfigurationManager.AppSettings["logPathLocation"].ToString(), ConfigurationManager.AppSettings["logFileName"].ToString());
                    }
                }

            }
            catch (Exception Ex)
            {
                WriteLogToFile("Exception in Database Insert: " + Ex.Message + Ex.StackTrace, ConfigurationManager.AppSettings["logPathLocation"].ToString(), ConfigurationManager.AppSettings["logFileName"].ToString());
                
                throw Ex;
            }
        }

        private string ParseLiveHelpInputStream(System.IO.Stream oStream)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int streamLength = 0;
            int streamRead = 0;

            streamLength = Convert.ToInt32(oStream.Length);
            Byte[] streamArray = new Byte[streamLength];

            streamRead = oStream.Read(streamArray, 0, streamLength);

            for (int i = 0; i <= streamLength - 1; i++)
            {
                sb.Append(Convert.ToChar(streamArray[i]));
            }

            return sb.ToString();
        }

        public void WriteLogToFile(string logMessage, string logPath, string fileName)
        {
            if (Directory.Exists(logPath))
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(logPath + fileName, true))
                    {
                        file.WriteLine(logMessage);
                    }
                }
                else
                {
                    Directory.CreateDirectory(logPath);

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(logPath + fileName, true))
                    {
                        file.WriteLine(logMessage);
                    }
                }
        }

    }

}
