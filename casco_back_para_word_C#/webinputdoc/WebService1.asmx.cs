using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.Office.Interop.Word;
using System.Net;


namespace webinputdoc
{
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://cascoproject.tju")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    [System.Web.Script.Services.ScriptService]


    public class WebService1 : System.Web.Services.WebService
    {

        public class hahabaseobject
        {
            public string title;
            public string tag;
            public string description;
            public ArrayList Source = new ArrayList();
            public string othercontext;
            public string Implement;
            public string Priority;
            public string Contribution;
            public string Category;
            public string Allocation;
            public string sources;

        }
        public class step123
        {
            public int num;
            public string actions;
            public string expected_result;
            public string indata;
            public string test_step;

        }
        public class tctable
        {
            public string tag;
            public string description;
            public string test_item;
            public string test_method;
            public string pre_condition;
            public string result;
            public string comment;
            public ArrayList test_steps = new ArrayList();
            public ArrayList source = new ArrayList();

            public string input;
            public string exec_step;
            public string exp_step;
            //public ArrayList actions = new ArrayList();
            //public ArrayList epr = new ArrayList();
        }
        public class finaljson
        {
            public ArrayList finalstrings = new ArrayList();

        }
        public class JsonTools
        {
            // 从一个对象信息生成Json串
            public static string ObjectToJson(object obj)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                MemoryStream stream = new MemoryStream();
                serializer.WriteObject(stream, obj);
                byte[] dataBytes = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(dataBytes, 0, (int)stream.Length);
                return Encoding.UTF8.GetString(dataBytes);
            }
            // 从一个Json串生成对象信息
            public static object JsonToObject(string jsonString, object obj)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                return serializer.ReadObject(mStream);
            }
        }
        public string swfnanme;
        private string OfficeToPdf(string OfficePath, string OfficeName, string destPath)
        {
            string fullPathName = OfficePath + OfficeName;//包含 路径 的全称
            FileInfo fi1 = new FileInfo(fullPathName);
            fi1.Attributes = ~FileAttributes.ReadOnly;
            string fileNameWithoutEx = System.IO.Path.GetFileNameWithoutExtension(OfficeName);//不包含路径，不包含扩展名
            string extendName = System.IO.Path.GetExtension(OfficeName).ToLower();//文件扩展名
            string saveName = destPath + fileNameWithoutEx + ".pdf";
            string returnValue = fileNameWithoutEx + ".pdf";
            Util.WordToPDF(fullPathName, saveName);
            return returnValue;
        }
        private string PdfToSwf(string pdf2swfPath, string PdfPath, string PdfName, string destPath)
        {
            string fullPathName = PdfPath + PdfName;//包含 路径 的全称
            string fileNameWithoutEx = System.IO.Path.GetFileNameWithoutExtension(PdfName);//不包含路径，不包含扩展名
            string extendName = System.IO.Path.GetExtension(PdfName).ToLower();//文件扩展名
            string saveName = destPath + fileNameWithoutEx + ".swf";
            string returnValue = fileNameWithoutEx + ".swf"; ;
            Util.PDFToSWF(pdf2swfPath, fullPathName, saveName);
            return returnValue;
        }
        public string showwordfiles(string filename)
        {
            string pdf2swfToolPath = System.Web.HttpContext.Current.Server.MapPath("~/FlexPaper/pdf2swf.exe");
            string OfficeFilePath = ConfigurationManager.AppSettings["path"].ToString()+ "office\\";
            string PdfFilePath = ConfigurationManager.AppSettings["path"].ToString() + "pdf\\";
            string SWFFilePath = ConfigurationManager.AppSettings["path"].ToString() + "swf\\";
            string SwfFileName = String.Empty;
            string UploadFileName = System.IO.Path.GetFileNameWithoutExtension(filename) + ".doc";
            string UploadFileType = System.IO.Path.GetExtension(UploadFileName).ToLower();
            string UploadFileNameFileFullName = String.Empty;
            UploadFileNameFileFullName = OfficeFilePath + UploadFileName;
            File.Copy(filename, UploadFileNameFileFullName);
            string PdfFileName = OfficeToPdf(OfficeFilePath, UploadFileName, PdfFilePath);
            SwfFileName = PdfToSwf(pdf2swfToolPath, PdfFilePath, PdfFileName, SWFFilePath);
            return SWFFilePath;
        }
        public string delet_tables(string filename)
        {
            _Application appdelet_tables = new Microsoft.Office.Interop.Word.Application();
            _Document docdelet_tables;
            object fileName = filename;
            object unknow = System.Type.Missing;
            docdelet_tables = appdelet_tables.Documents.Open(ref fileName,
                           ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                           ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                           ref unknow, ref unknow, ref unknow, ref unknow, ref unknow);//input a doc
            int table_num = docdelet_tables.Tables.Count;
            try
            {
                for (int i = 1; i <= table_num; i++)
                {
                    docdelet_tables.Tables[i].Delete();
                }
            }
            catch { }
            docdelet_tables.Close(ref unknow, ref unknow, ref unknow);

            appdelet_tables.Quit(ref unknow, ref unknow, ref unknow);
            return null;
        }
        public void thread1()
        {
            string temp = null;
            string tempf = null;
            int start = 1;
            int end = pcount / 8;
            for (int i = start; i <= end; i += 1)
            {
                temp = doc.Paragraphs[i].Range.Text.Trim();//变量i为第i段

                MatchCollection matches = Regex.Matches(temp, pattern1);

                if (matches.Count > 0)
                {
                    hahabaseobject newobject = new hahabaseobject();
                    newobject.Allocation = "";
                    newobject.Category = "";
                    newobject.Contribution = "";
                    newobject.description = "";
                    newobject.Implement = "";
                    newobject.Priority = "";
                    newobject.othercontext = "";
                    newobject.title = temp;//还没转换好
                    newobject.tag = newobject.title;
                    int endj = 0;
                    int flag = 1;
                    for (int j = i + 1; j <= end; j++)
                    {
                        temp = doc.Paragraphs[j].Range.Text.Trim();//变量i为第i段
                        MatchCollection matchesj = Regex.Matches(temp, pattern3);
                        if (matchesj.Count > 0)//写到段落存储，回去再写
                        {
                            endj = j;
                            break;
                        }
                    }
                    //textBox1.Text = i.ToString();
                    newobject.description = doc.Paragraphs[i + 1].Range.Text;
                    int arraycontentcount = 1;
                    for (int k = i + 1; k <= pcount; k++)
                    {
                        temp = doc.Paragraphs[k].Range.Text;

                        int startflag = 0;
                        MatchCollection matchesjinghao = Regex.Matches(temp, pattern4);
                        //textBox1.Text = matchesjinghao.Count.ToString();
                        if (matchesjinghao.Count < 1 && startflag < 1)
                        {
                            newobject.description = newobject.description + "\n" + temp;
                        }
                        else
                        {
                            startflag = 1;
                            int poseuql = temp.IndexOf('=');
                            tempf = temp.Substring(1, poseuql - 2);
                            temp = temp.Substring(poseuql + 1, temp.Length - poseuql - 1);

                            if (matchesjinghao.Count >= 1)
                            {
                                if (tempf == "Implement")
                                {
                                    newobject.Implement = temp;

                                }
                                if (tempf == "Priority")
                                {
                                    newobject.Priority = temp;
                                }
                                if (tempf == "Contribution")
                                {
                                    newobject.Contribution = temp;
                                }
                                if (tempf == "Category")
                                {
                                    newobject.Category = temp;
                                }
                                if (tempf == "Allocation")
                                {
                                    newobject.Allocation = temp;
                                }
                                if (tempf == "Source")
                                {
                                    newobject.sources = temp;
                                }
                                arraycontentcount++;

                            }

                        }
                        if (newobject.sources != null)
                        {
                            break;
                        }
                    }

                    if (newobject.sources != null)
                    {
                        MatchCollection matchessourse = Regex.Matches(newobject.sources, pattern2);
                        newobject.othercontext = matchessourse.Count.ToString();

                        for (int ksourse = 0; ksourse < matchessourse.Count; ksourse++)
                        {
                            //newobject.source[ksourse] =matchessourse[ksourse].Value;
                            newobject.Source.Add(matchessourse[ksourse].Value);
                        }

                        aaa.finalstrings.Add(newobject);
                    }
                }
            }
        }


        public void thread2()
        {
            int arraycount = 0;
            string temp = null;
            string tempf = null;
            int start = pcount / 8 + 1;
            int end = pcount * 2 / 8;
            /*for (int i = start; i <= end; i++)
            {
                temp = this.doc1.Paragraphs[i].Range.Text.Trim();//变量i为第i段
                MatchCollection matches = Regex.Matches(temp, pattern1);
                if (matches.Count > 0)
                {
                    arraycount++;
                    //Console.WriteLine(arraycount);
                }
            }

            hahabaseobject[] myarray = new hahabaseobject[arraycount + 1];
            int arraycount1 = arraycount;*/
            arraycount = 0;
            for (int i = start; i <= end; i += 1)
            {
                //textBox1.Text = i.ToString();
                //Console.WriteLine(i);
                temp = doc1.Paragraphs[i].Range.Text.Trim();//变量i为第i段

                MatchCollection matches = Regex.Matches(temp, pattern1);

                if (matches.Count > 0)
                {
                    hahabaseobject newobject = new hahabaseobject();
                    newobject.Allocation = "";
                    newobject.Category = "";
                    newobject.Contribution = "";
                    newobject.description = "";
                    newobject.Implement = "";
                    newobject.Priority = "";
                    newobject.othercontext = "";
                    newobject.title = temp;//还没转换好
                    newobject.tag = newobject.title;
                    int endj = 0;
                    for (int j = i + 1; j <= end; j++)
                    {
                        temp = doc1.Paragraphs[j].Range.Text.Trim();//变量i为第i段
                        MatchCollection matchesj = Regex.Matches(temp, pattern3);
                        if (matchesj.Count > 0)//写到段落存储，回去再写
                        {
                            endj = j;
                            break;
                        }
                    }
                    //textBox1.Text = i.ToString();
                    newobject.description = doc1.Paragraphs[i + 1].Range.Text;
                    int arraycontentcount = 1;
                    for (int k = i + 1; k <= pcount; k++)
                    {
                        temp = doc1.Paragraphs[k].Range.Text;

                        int startflag = 0;
                        MatchCollection matchesjinghao = Regex.Matches(temp, pattern4);
                        //textBox1.Text = matchesjinghao.Count.ToString();
                        if (matchesjinghao.Count < 1 && startflag < 1)
                        {
                            newobject.description = newobject.description + "\n" + temp;
                        }
                        else
                        {
                            startflag = 1;
                            int poseuql = temp.IndexOf('=');
                            tempf = temp.Substring(1, poseuql - 2);
                            temp = temp.Substring(poseuql + 1, temp.Length - poseuql - 1);

                            if (matchesjinghao.Count >= 1)
                            {
                                if (tempf == "Implement")
                                {
                                    newobject.Implement = temp;

                                }
                                if (tempf == "Priority")
                                {
                                    newobject.Priority = temp;
                                }
                                if (tempf == "Contribution")
                                {
                                    newobject.Contribution = temp;
                                }
                                if (tempf == "Category")
                                {
                                    newobject.Category = temp;
                                }
                                if (tempf == "Allocation")
                                {
                                    newobject.Allocation = temp;
                                }
                                if (tempf == "Source")
                                {
                                    newobject.sources = temp;
                                }

                                arraycontentcount++;

                            }

                        }
                        //newobject.arraycontent[6] = doc.Paragraphs[endj].Range.Text;//将end赋值给newobject
                        if (newobject.sources != null)
                        {
                            break;
                        }

                    }
                    if (newobject.sources != null)
                    {
                        MatchCollection matchessourse = Regex.Matches(newobject.sources, pattern2);
                        newobject.othercontext = matchessourse.Count.ToString();

                        for (int ksourse = 0; ksourse < matchessourse.Count; ksourse++)
                        {
                            //newobject.source[ksourse] =matchessourse[ksourse].Value;
                            newobject.Source.Add(matchessourse[ksourse].Value);
                        }//将sourse转化成数组格式
                        /*myarray[arraycount] = newobject;
                        arraycount++;*/
                        aaa.finalstrings.Add(newobject);
                    }
                }
            }
            /*int trueaccount = 0;
            for (int k = 0; k < arraycount1; k++)
            {
                if (myarray[k] != null)
                {
                    trueaccount++;
                }
            }
            hahabaseobject[] mytruearray = new hahabaseobject[trueaccount];
            //trueaccount = 0;
            for (int k = 0; k <= trueaccount; k++)
            {
                if (myarray[k] != null)
                {
                    mytruearray[k] = myarray[k];
                    //trueaccount++;
                }
            }
            if (trueaccount > 0)
            {
                aaa.finalstrings.Add(mytruearray);
            }*/
            /*string2 = JsonTools.ObjectToJson(mytruearray);
            int strlen = string2.Length;
            //strlen = strlen;
            if (strlen == 2)
            { string2 = ""; }
            else
            {
                string2 = string2.Substring(1, strlen - 2) + ',';
            }
            //string2 = string2.Substring(1, strlen - 2) + ',';      
            threadsig2 = 1;*/
        }
        public void thread3()
        {
            int arraycount = 0;
            string temp = null;
            string tempf = null;
            int start = pcount * 2 / 8 + 1;
            int end = pcount * 3 / 8;
            /*for (int i = start; i <= end; i++)
            {
                temp = this.doc2.Paragraphs[i].Range.Text.Trim();//变量i为第i段
                MatchCollection matches = Regex.Matches(temp, pattern1);
                if (matches.Count > 0)
                {
                    arraycount++;
                    //Console.WriteLine(arraycount);
                }
            }

            hahabaseobject[] myarray = new hahabaseobject[arraycount + 1];
            int arraycount1 = arraycount;*/
            arraycount = 0;
            for (int i = start; i <= end; i += 1)
            {
                //textBox1.Text = i.ToString();
                //Console.WriteLine(i);
                temp = doc2.Paragraphs[i].Range.Text.Trim();//变量i为第i段

                MatchCollection matches = Regex.Matches(temp, pattern1);

                if (matches.Count > 0)
                {
                    hahabaseobject newobject = new hahabaseobject();
                    newobject.Allocation = "";
                    newobject.Category = "";
                    newobject.Contribution = "";
                    newobject.description = "";
                    newobject.Implement = "";
                    newobject.Priority = "";
                    newobject.othercontext = "";
                    newobject.title = temp;//还没转换好
                    newobject.tag = newobject.title;
                    int endj = 0;
                    for (int j = i + 1; j <= end; j++)
                    {
                        temp = doc2.Paragraphs[j].Range.Text.Trim();//变量i为第i段
                        MatchCollection matchesj = Regex.Matches(temp, pattern3);
                        if (matchesj.Count > 0)//写到段落存储，回去再写
                        {
                            endj = j;
                            break;
                        }
                    }
                    //textBox1.Text = i.ToString();
                    newobject.description = doc1.Paragraphs[i + 1].Range.Text;
                    int arraycontentcount = 1;
                    for (int k = i + 1; k <= pcount; k++)
                    {
                        temp = doc2.Paragraphs[k].Range.Text;

                        int startflag = 0;
                        MatchCollection matchesjinghao = Regex.Matches(temp, pattern4);
                        //textBox1.Text = matchesjinghao.Count.ToString();
                        if (matchesjinghao.Count < 1 && startflag < 1)
                        {
                            newobject.description = newobject.description + "\n" + temp;
                        }
                        else
                        {
                            startflag = 1;
                            int poseuql = temp.IndexOf('=');
                            tempf = temp.Substring(1, poseuql - 2);
                            temp = temp.Substring(poseuql + 1, temp.Length - poseuql - 1);

                            if (matchesjinghao.Count >= 1)
                            {
                                if (tempf == "Implement")
                                {
                                    newobject.Implement = temp;

                                }
                                if (tempf == "Priority")
                                {
                                    newobject.Priority = temp;
                                }
                                if (tempf == "Contribution")
                                {
                                    newobject.Contribution = temp;
                                }
                                if (tempf == "Category")
                                {
                                    newobject.Category = temp;
                                }
                                if (tempf == "Allocation")
                                {
                                    newobject.Allocation = temp;
                                }
                                if (tempf == "Source")
                                {
                                    newobject.sources = temp;
                                }

                                arraycontentcount++;

                            }

                        }
                        //newobject.arraycontent[6] = doc.Paragraphs[endj].Range.Text;//将end赋值给newobject

                        if (newobject.sources != null)
                        {
                            break;
                        }
                    }
                    if (newobject.sources != null)
                    {
                        MatchCollection matchessourse = Regex.Matches(newobject.sources, pattern2);
                        newobject.othercontext = matchessourse.Count.ToString();

                        for (int ksourse = 0; ksourse < matchessourse.Count; ksourse++)
                        {
                            //newobject.source[ksourse] =matchessourse[ksourse].Value;
                            newobject.Source.Add(matchessourse[ksourse].Value);
                        }//将sourse转化成数组格式
                        aaa.finalstrings.Add(newobject);
                    }
                }
            }
            /*string3 = JsonTools.ObjectToJson(mytruearray);
            int strlen = string3.Length;
            //strlen = strlen;
            if (strlen == 2)
            { string3 = ""; }
            else
            {
                string3 = string3.Substring(1, strlen - 2) + ',';
            }
            threadsig2 = 1;*/
        }
        public void thread4()
        {
            int arraycount = 0;
            string temp = null;
            string tempf = null;
            int start = pcount * 3 / 8 + 1;
            int end = pcount * 4 / 8;
            arraycount = 0;
            for (int i = start; i <= end; i += 1)
            {
                //textBox1.Text = i.ToString();
                //Console.WriteLine(i);
                temp = doc3.Paragraphs[i].Range.Text.Trim();//变量i为第i段

                MatchCollection matches = Regex.Matches(temp, pattern1);

                if (matches.Count > 0)
                {
                    hahabaseobject newobject = new hahabaseobject();
                    newobject.Allocation = "";
                    newobject.Category = "";
                    newobject.Contribution = "";
                    newobject.description = "";
                    newobject.Implement = "";
                    newobject.Priority = "";
                    newobject.othercontext = "";
                    newobject.title = temp;//还没转换好
                    newobject.tag = newobject.title;
                    int endj = 0;
                    for (int j = i + 1; j <= end; j++)
                    {
                        temp = doc3.Paragraphs[j].Range.Text.Trim();//变量i为第i段
                        MatchCollection matchesj = Regex.Matches(temp, pattern3);
                        if (matchesj.Count > 0)//写到段落存储，回去再写
                        {
                            endj = j;
                            break;
                        }
                    }
                    //textBox1.Text = i.ToString();
                    newobject.description = doc3.Paragraphs[i + 1].Range.Text;
                    int arraycontentcount = 1;
                    for (int k = i + 1; k <= pcount; k++)
                    {
                        temp = doc3.Paragraphs[k].Range.Text;

                        int startflag = 0;
                        MatchCollection matchesjinghao = Regex.Matches(temp, pattern4);
                        //textBox1.Text = matchesjinghao.Count.ToString();
                        if (matchesjinghao.Count < 1 && startflag < 1)
                        {
                            newobject.description = newobject.description + "\n" + temp;
                        }
                        else
                        {
                            startflag = 1;
                            int poseuql = temp.IndexOf('=');
                            tempf = temp.Substring(1, poseuql - 2);
                            temp = temp.Substring(poseuql + 1, temp.Length - poseuql - 1);

                            if (matchesjinghao.Count >= 1)
                            {
                                if (tempf == "Implement")
                                {
                                    newobject.Implement = temp;

                                }
                                if (tempf == "Priority")
                                {
                                    newobject.Priority = temp;
                                }
                                if (tempf == "Contribution")
                                {
                                    newobject.Contribution = temp;
                                }
                                if (tempf == "Category")
                                {
                                    newobject.Category = temp;
                                }
                                if (tempf == "Allocation")
                                {
                                    newobject.Allocation = temp;
                                }
                                if (tempf == "Source")
                                {
                                    newobject.sources = temp;
                                }

                                arraycontentcount++;

                            }

                        }
                        //newobject.arraycontent[6] = doc.Paragraphs[endj].Range.Text;//将end赋值给newobject
                        if (newobject.sources != null)
                        {
                            break;
                        }

                    }
                    if (newobject.sources != null)
                    {
                        MatchCollection matchessourse = Regex.Matches(newobject.sources, pattern2);
                        newobject.othercontext = matchessourse.Count.ToString();

                        for (int ksourse = 0; ksourse < matchessourse.Count; ksourse++)
                        {
                            //newobject.source[ksourse] =matchessourse[ksourse].Value;
                            newobject.Source.Add(matchessourse[ksourse].Value);
                        }//将sourse转化成数组格式
                        aaa.finalstrings.Add(newobject);
                    }
                }

            }
            /*string4 = JsonTools.ObjectToJson(mytruearray);
            int strlen = string4.Length;
            if (strlen == 2)
            { string4 = ""; }
            else
            {
                string4 = string4.Substring(1, strlen - 2) + ',';
            }
            //string4 = string4.Substring(1, strlen - 2) + ','; 
            threadsig2 = 1;*/
        }
        public void thread5()
        {
            int arraycount = 0;
            string temp = null;
            string tempf = null;
            int start = pcount * 4 / 8 + 1;
            int end = pcount * 5 / 8;
            arraycount = 0;
            for (int i = start; i <= end; i += 1)
            {
                //textBox1.Text = i.ToString();
                //Console.WriteLine(i);
                temp = doc4.Paragraphs[i].Range.Text.Trim();//变量i为第i段

                MatchCollection matches = Regex.Matches(temp, pattern1);

                if (matches.Count > 0)
                {
                    hahabaseobject newobject = new hahabaseobject();
                    newobject.Allocation = "";
                    newobject.Category = "";
                    newobject.Contribution = "";
                    newobject.description = "";
                    newobject.Implement = "";
                    newobject.Priority = "";
                    newobject.othercontext = "";
                    newobject.title = temp;//还没转换好
                    newobject.tag = newobject.title;
                    int endj = 0;
                    for (int j = i + 1; j <= end; j++)
                    {
                        temp = doc4.Paragraphs[j].Range.Text.Trim();//变量i为第i段
                        MatchCollection matchesj = Regex.Matches(temp, pattern3);
                        if (matchesj.Count > 0)//写到段落存储，回去再写
                        {
                            endj = j;
                            break;
                        }
                    }
                    //textBox1.Text = i.ToString();
                    newobject.description = doc4.Paragraphs[i + 1].Range.Text;
                    int arraycontentcount = 1;
                    for (int k = i + 1; k <= pcount; k++)
                    {
                        temp = doc4.Paragraphs[k].Range.Text;

                        int startflag = 0;
                        MatchCollection matchesjinghao = Regex.Matches(temp, pattern4);
                        //textBox1.Text = matchesjinghao.Count.ToString();
                        if (matchesjinghao.Count < 1 && startflag < 1)
                        {
                            newobject.description = newobject.description + "\n" + temp;
                        }
                        else
                        {
                            startflag = 1;
                            int poseuql = temp.IndexOf('=');
                            tempf = temp.Substring(1, poseuql - 2);
                            temp = temp.Substring(poseuql + 1, temp.Length - poseuql - 1);

                            if (matchesjinghao.Count >= 1)
                            {
                                if (tempf == "Implement")
                                {
                                    newobject.Implement = temp;

                                }
                                if (tempf == "Priority")
                                {
                                    newobject.Priority = temp;
                                }
                                if (tempf == "Contribution")
                                {
                                    newobject.Contribution = temp;
                                }
                                if (tempf == "Category")
                                {
                                    newobject.Category = temp;
                                }
                                if (tempf == "Allocation")
                                {
                                    newobject.Allocation = temp;
                                }
                                if (tempf == "Source")
                                {
                                    newobject.sources = temp;
                                }

                                arraycontentcount++;

                            }

                        }
                        //newobject.arraycontent[6] = doc.Paragraphs[endj].Range.Text;//将end赋值给newobject

                        if (newobject.sources != null)
                        {
                            break;
                        }
                    }
                    if (newobject.sources != null)
                    {
                        MatchCollection matchessourse = Regex.Matches(newobject.sources, pattern2);
                        newobject.othercontext = matchessourse.Count.ToString();

                        for (int ksourse = 0; ksourse < matchessourse.Count; ksourse++)
                        {
                            //newobject.source[ksourse] =matchessourse[ksourse].Value;
                            newobject.Source.Add(matchessourse[ksourse].Value);
                        }//将sourse转化成数组格式
                        aaa.finalstrings.Add(newobject);
                    }
                }

            }
            /*string5 = JsonTools.ObjectToJson(mytruearray);
            int strlen = string5.Length;
            if (strlen == 2)
            { string5 = ""; }
            else
            {
                string5 = string5.Substring(1, strlen - 2) + ',';
            }
            //string4 = string4.Substring(1, strlen - 2) + ',';
            //string5 = string5.Substring(1, strlen - 2) + ','; 
            threadsig2 = 1;*/
        }
        public void thread6()
        {
            int arraycount = 0;
            string temp = null;
            string tempf = null;
            int start = pcount * 5 / 8 + 1;
            int end = pcount * 6 / 8;
            arraycount = 0;
            for (int i = start; i <= end; i += 1)
            {
                //textBox1.Text = i.ToString();
                //Console.WriteLine(i);
                temp = doc5.Paragraphs[i].Range.Text.Trim();//变量i为第i段

                MatchCollection matches = Regex.Matches(temp, pattern1);

                if (matches.Count > 0)
                {
                    hahabaseobject newobject = new hahabaseobject();
                    newobject.Allocation = "";
                    newobject.Category = "";
                    newobject.Contribution = "";
                    newobject.description = "";
                    newobject.Implement = "";
                    newobject.Priority = "";
                    newobject.othercontext = "";
                    newobject.title = temp;//还没转换好
                    newobject.tag = newobject.title;
                    int endj = 0;
                    for (int j = i + 1; j <= end; j++)
                    {
                        temp = doc5.Paragraphs[j].Range.Text.Trim();//变量i为第i段
                        MatchCollection matchesj = Regex.Matches(temp, pattern3);
                        if (matchesj.Count > 0)//写到段落存储，回去再写
                        {
                            endj = j;
                            break;
                        }
                    }
                    //textBox1.Text = i.ToString();
                    newobject.description = doc5.Paragraphs[i + 1].Range.Text;
                    int arraycontentcount = 1;
                    for (int k = i + 1; k <= pcount; k++)
                    {
                        temp = doc5.Paragraphs[k].Range.Text;

                        int startflag = 0;
                        MatchCollection matchesjinghao = Regex.Matches(temp, pattern4);
                        //textBox1.Text = matchesjinghao.Count.ToString();
                        if (matchesjinghao.Count < 1 && startflag < 1)
                        {
                            newobject.description = newobject.description + "\n" + temp;
                        }
                        else
                        {
                            startflag = 1;
                            int poseuql = temp.IndexOf('=');
                            tempf = temp.Substring(1, poseuql - 2);
                            temp = temp.Substring(poseuql + 1, temp.Length - poseuql - 1);

                            if (matchesjinghao.Count >= 1)
                            {
                                if (tempf == "Implement")
                                {
                                    newobject.Implement = temp;

                                }
                                if (tempf == "Priority")
                                {
                                    newobject.Priority = temp;
                                }
                                if (tempf == "Contribution")
                                {
                                    newobject.Contribution = temp;
                                }
                                if (tempf == "Category")
                                {
                                    newobject.Category = temp;
                                }
                                if (tempf == "Allocation")
                                {
                                    newobject.Allocation = temp;
                                }
                                if (tempf == "Source")
                                {
                                    newobject.sources = temp;
                                }

                                arraycontentcount++;

                            }

                        }
                        //newobject.arraycontent[6] = doc.Paragraphs[endj].Range.Text;//将end赋值给newobject

                        if (newobject.sources != null)
                        {
                            break;
                        }
                    }
                    if (newobject.sources != null)
                    {
                        MatchCollection matchessourse = Regex.Matches(newobject.sources, pattern2);
                        newobject.othercontext = matchessourse.Count.ToString();

                        for (int ksourse = 0; ksourse < matchessourse.Count; ksourse++)
                        {
                            //newobject.source[ksourse] =matchessourse[ksourse].Value;
                            newobject.Source.Add(matchessourse[ksourse].Value);
                        }//将sourse转化成数组格式
                        aaa.finalstrings.Add(newobject);
                    }
                }

            }
        }
        public void thread7()
        {
            int arraycount = 0;
            string temp = null;
            string tempf = null;
            int start = pcount * 6 / 8 + 1;
            int end = pcount * 7 / 8;
            for (int i = start; i <= end; i += 1)
            {
                //textBox1.Text = i.ToString();
                //Console.WriteLine(i);
                temp = doc6.Paragraphs[i].Range.Text.Trim();//变量i为第i段

                MatchCollection matches = Regex.Matches(temp, pattern1);

                if (matches.Count > 0)
                {
                    hahabaseobject newobject = new hahabaseobject();
                    newobject.Allocation = "";
                    newobject.Category = "";
                    newobject.Contribution = "";
                    newobject.description = "";
                    newobject.Implement = "";
                    newobject.Priority = "";
                    newobject.othercontext = "";
                    newobject.title = temp;//还没转换好
                    newobject.tag = newobject.title;
                    int endj = 0;
                    for (int j = i + 1; j <= end; j++)
                    {
                        temp = doc6.Paragraphs[j].Range.Text.Trim();//变量i为第i段
                        MatchCollection matchesj = Regex.Matches(temp, pattern3);
                        if (matchesj.Count > 0)//写到段落存储，回去再写
                        {
                            endj = j;
                            break;
                        }
                    }
                    //textBox1.Text = i.ToString();
                    newobject.description = doc6.Paragraphs[i + 1].Range.Text;
                    int arraycontentcount = 1;
                    for (int k = i + 1; k <= pcount; k++)
                    {
                        temp = doc6.Paragraphs[k].Range.Text;

                        int startflag = 0;
                        MatchCollection matchesjinghao = Regex.Matches(temp, pattern4);
                        //textBox1.Text = matchesjinghao.Count.ToString();
                        if (matchesjinghao.Count < 1 && startflag < 1)
                        {
                            newobject.description = newobject.description + "\n" + temp;
                        }
                        else
                        {
                            startflag = 1;
                            int poseuql = temp.IndexOf('=');
                            tempf = temp.Substring(1, poseuql - 2);
                            temp = temp.Substring(poseuql + 1, temp.Length - poseuql - 1);

                            if (matchesjinghao.Count >= 1)
                            {
                                if (tempf == "Implement")
                                {
                                    newobject.Implement = temp;

                                }
                                if (tempf == "Priority")
                                {
                                    newobject.Priority = temp;
                                }
                                if (tempf == "Contribution")
                                {
                                    newobject.Contribution = temp;
                                }
                                if (tempf == "Category")
                                {
                                    newobject.Category = temp;
                                }
                                if (tempf == "Allocation")
                                {
                                    newobject.Allocation = temp;
                                }
                                if (tempf == "Source")
                                {
                                    newobject.sources = temp;
                                }

                                arraycontentcount++;

                            }

                        }
                        //newobject.arraycontent[6] = doc.Paragraphs[endj].Range.Text;//将end赋值给newobject
                        if (newobject.sources != null)
                        {
                            break;
                        }

                    }
                    if (newobject.sources != null)
                    {
                        MatchCollection matchessourse = Regex.Matches(newobject.sources, pattern2);
                        newobject.othercontext = matchessourse.Count.ToString();

                        for (int ksourse = 0; ksourse < matchessourse.Count; ksourse++)
                        {
                            //newobject.source[ksourse] =matchessourse[ksourse].Value;
                            newobject.Source.Add(matchessourse[ksourse].Value);
                        }//将sourse转化成数组格式
                        aaa.finalstrings.Add(newobject);
                    }
                }

            }
        }
        public void thread8()
        {
            int arraycount = 0;
            string temp = null;
            string tempf = null;
            int start = pcount * 7 / 8 + 1;
            int end = pcount;
            arraycount = 0;
            for (int i = start; i <= end; i += 1)
            {
                //textBox1.Text = i.ToString();
                //Console.WriteLine(i);
                temp = doc7.Paragraphs[i].Range.Text.Trim();//变量i为第i段

                MatchCollection matches = Regex.Matches(temp, pattern1);

                if (matches.Count > 0)
                {
                    hahabaseobject newobject = new hahabaseobject();
                    newobject.Allocation = "";
                    newobject.Category = "";
                    newobject.Contribution = "";
                    newobject.description = "";
                    newobject.Implement = "";
                    newobject.Priority = "";
                    newobject.othercontext = "";
                    newobject.title = temp;//还没转换好
                    newobject.tag = newobject.title;
                    int endj = 0;
                    for (int j = i + 1; j <= end; j++)
                    {
                        temp = doc7.Paragraphs[j].Range.Text.Trim();//变量i为第i段
                        MatchCollection matchesj = Regex.Matches(temp, pattern3);
                        if (matchesj.Count > 0)//写到段落存储，回去再写
                        {
                            endj = j;
                            break;
                        }
                    }
                    //textBox1.Text = i.ToString();
                    newobject.description = doc7.Paragraphs[i + 1].Range.Text;
                    int arraycontentcount = 1;
                    for (int k = i + 1; k <= endj - 1; k++)
                    {
                        temp = doc7.Paragraphs[k].Range.Text;

                        int startflag = 0;
                        MatchCollection matchesjinghao = Regex.Matches(temp, pattern4);
                        //textBox1.Text = matchesjinghao.Count.ToString();
                        if (matchesjinghao.Count < 1 && startflag < 1)
                        {
                            newobject.description = newobject.description + "\n" + temp;
                        }
                        else
                        {
                            startflag = 1;
                            int poseuql = temp.IndexOf('=');
                            tempf = temp.Substring(1, poseuql - 2);
                            temp = temp.Substring(poseuql + 1, temp.Length - poseuql - 1);

                            if (matchesjinghao.Count >= 1)
                            {
                                if (tempf == "Implement")
                                {
                                    newobject.Implement = temp;

                                }
                                if (tempf == "Priority")
                                {
                                    newobject.Priority = temp;
                                }
                                if (tempf == "Contribution")
                                {
                                    newobject.Contribution = temp;
                                }
                                if (tempf == "Category")
                                {
                                    newobject.Category = temp;
                                }
                                if (tempf == "Allocation")
                                {
                                    newobject.Allocation = temp;
                                }
                                if (tempf == "Source")
                                {
                                    newobject.sources = temp;
                                }

                                arraycontentcount++;

                            }

                        }
                        //newobject.arraycontent[6] = doc.Paragraphs[endj].Range.Text;//将end赋值给newobject


                    }
                    if (newobject.sources != null)
                    {
                        MatchCollection matchessourse = Regex.Matches(newobject.sources, pattern2);
                        newobject.othercontext = matchessourse.Count.ToString();

                        for (int ksourse = 0; ksourse < matchessourse.Count; ksourse++)
                        {
                            //newobject.source[ksourse] =matchessourse[ksourse].Value;
                            newobject.Source.Add(matchessourse[ksourse].Value);
                        }//将sourse转化成数组格式
                        aaa.finalstrings.Add(newobject);
                    }
                }

            }
            /*string8 = JsonTools.ObjectToJson(mytruearray);
            int strlen = string8.Length;
            if (strlen == 2)
            { string8 = "]"; }
            else
            {
                string8 = string8.Substring(1, strlen - 1);
            }
            //string4 = string4.Substring(1, strlen - 2) + ',';
            //string8 = string8.Substring(1,strlen-1); 
            threadsig2 = 1;*/
        }
        public _Document doc;
        public _Document doc1;
        public _Document doc2;
        public _Document doc3;
        public _Document doc4;
        public _Document doc5;
        public _Document doc6;
        public _Document doc7;
        public string pattern1;
        public string pattern2;
        public string pattern3;
        public string pattern4;
        public string type1;
        public string type2;
        public int pcount;
        public string string1;
        public string string2;
        public string string3;
        public string string4;
        public string string5;
        public string string6;
        public string string7;
        public string string8;
        public int threadsig1 = 0;
        public int threadsig2 = 0;
        public finaljson aaa = new finaljson();
        [WebMethod]
        public bool downfile(string url)
        {
            try
            {
                //return false;

                int poseuqlurl = url.IndexOf('=');
                string url1;
                url1 = url.Substring(poseuqlurl + 1, url.Length - poseuqlurl - 1);
                Uri u = new Uri(url1);
                string filename = DateTime.Now.ToString() + ".doc";
                string LocalPath = "D:\\" + filename;
                HttpWebRequest mRequest = (HttpWebRequest)WebRequest.Create(u);
                mRequest.Method = "GET";
                mRequest.ContentType = "application/x-www-form-urlencoded";
                HttpWebResponse wr = (HttpWebResponse)mRequest.GetResponse();
                Stream sIn = wr.GetResponseStream();
                FileStream fs = new FileStream(LocalPath, FileMode.Create, FileAccess.Write);
                //BinaryWriter brnew = new BinaryWriter(fs);
                //brnew.Write(bytContent, 0, bytContent.Length);
                byte[] bytes = new byte[4096];

                int start = 0;

                int length;

                while ((length = sIn.Read(bytes, 0, 4096)) > 0)
                {

                    fs.Write(bytes, 0, length);

                    start += length;

                }
                sIn.Close();
                wr.Close();
                fs.Close();
                return true;
            }
            catch { return false; }
        }
        [WebMethod(Description = "TestRS")]
        public void TestRS(string url)
        {
            String json = @"[{""title"":""[TSP-SyAD-0314]"",""tag"":""[TSP-SyAD-0314]"",""description"":""Humidity condition shall obey the related requirement in EN 50125-3 .Relative humidity: ≤90% (no condensation) (25℃).\r\nHumidity condition shall obey the related requirement in EN 50125-3 .Relative humidity: ≤90% (no condensation) (25℃).\r\n湿度条件应遵循环境标准EN 50125-3相关需求，相对湿度：≤90%（无凝结）（25℃）；\r"",""Source"":[""[TSP-SyRS-0031]""],""othercontext"":""1"",""Implement"":"""",""Priority"":"""",""Contribution"":"" Safety\r"",""Category"":"" Non-Functional\r"",""Allocation"":"" [COTS]\r"",""sources"":"" [TSP-SyRS-0031]\r""},{""title"":""[TSP-SyAD-0017]"",""tag"":""[TSP-SyAD-0017]"",""description"":""MPS shall provide safety clock. The variation (fast/slow) of safety clock shall be less than or equal to 0.1%. If the variation of the safety clock is larger than this value, MPS shall be guided to safety side.\r\nMPS shall provide safety clock. The variation (fast/slow) of safety clock shall be less than or equal to 0.1%. If the variation of the safety clock is larger than this value, MPS shall be guided to safety side.\r\nMPS应提供安全时钟，安全时钟偏差不大于等于0.1%。如检测安全时钟偏差大于该值，MPS导向安全。\r"",""Source"":[""[TSP-SyRS-0113]"",""[TSP-IHA-0020]""],""othercontext"":""2"",""Implement"":"""",""Priority"":"""",""Contribution"":"" SIL4\r"",""Category"":"" Functional\r"",""Allocation"":"" [MPS]\r"",""sources"":"" [TSP-SyRS-0113], [TSP-IHA-0020]\r""}]";
            Context.Response.ContentType = "text/json";
            Context.Response.Write(json);
            Context.Response.End();
        }
        [WebMethod(Description = "TestTC")]
        public void TestTC(string url)
        {
            String json = @"[{""tag"":""TSP-MPS-MCU-SwITC-0001"",""description"":""TSP-MPS-MCU-SwITC-0001\rTo check the TSP_Proc_Init_Pre module could be initialized correctly when received application type is zc or lc.\r\r[Source: [TSP-MPS-MCU-SwAD-0030]]\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0078]]\r[Source: [TSP-MPS-MCU-SwAD-0079]]\r[Source: [TSP-MPS-MCU-SwAD-0011]]\r[Source: [TSP-MPS-MCU-SwAD-0051]]\r[Source: [TSP-MPS-MCU-SwAD-0041]]\r[Source: [TSP-MPS-MCU-SwAD-0023]]\r[Source: [TSP-MPS-MCU-SwAD-0024]]\r[Source: [TSP-MPS-MCU-SwAD-0101]]\r[Source: [TSP-MPS-MCU-SwAD-0102]]\r[Source: [TSP-MPS-MCU-SwAD-0103]]\r[Source: [TSP-MPS-MCU-SwAD-0104]]\r[Source: [TSP-MPS-MCU-SwAD-0105]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0030]"",""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0078]"",""[TSP-MPS-MCU-SwAD-0079]"",""[TSP-MPS-MCU-SwAD-0011]"",""[TSP-MPS-MCU-SwAD-0051]"",""[TSP-MPS-MCU-SwAD-0041]"",""[TSP-MPS-MCU-SwAD-0023]"",""[TSP-MPS-MCU-SwAD-0024]"",""[TSP-MPS-MCU-SwAD-0101]"",""[TSP-MPS-MCU-SwAD-0102]"",""[TSP-MPS-MCU-SwAD-0103]"",""[TSP-MPS-MCU-SwAD-0104]"",""[TSP-MPS-MCU-SwAD-0105]""],""input"":""TSP_Conf.ini\rApp type = 0x01\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rCall the MCU1 TSP_Proc_Init_Pre module.\rStart MPU1_Sim reply the app type message to MCU1\r\u0007"",""exp_step"":""Init start time shall be recorded.\rThe MCU1_State and system state shall be firstly initialized TSP_PROC_STATE_UNKNOWN. \rTSP_Proc_Init_MD5Chk check MD5Chk.lst successfully.\r MCU1 receive app type message successfully.(include two field:apptype=1, mpsReboot=0)\r BIT_Task, SLOT_MGR, GM_MUDP_Rx_Task and Main_Task shall be created.\rThe return value of TSP_Proc_Init_Pre module shall be GM_TRUE.\rThe value of l_globalinfo_struct.MCU1_State shall be TSP_PROC_SELF_INIT_OK.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0131"",""description"":""TSP-MPS-MCU-SwITC-0131\rTo check the TSP_Proc_Init_Pre module could be initialized correctly when received application type is CCS.\r\r[Source: [TSP-MPS-MCU-SwAD-0030]]\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0078]]\r[Source: [TSP-MPS-MCU-SwAD-0079]]\r[Source: [TSP-MPS-MCU-SwAD-0011]]\r[Source: [TSP-MPS-MCU-SwAD-0051]]\r[Source: [TSP-MPS-MCU-SwAD-0041]]\r[Source: [TSP-MPS-MCU-SwAD-0023]]\r[Source: [TSP-MPS-MCU-SwAD-0024]]\r[Source: [TSP-MPS-MCU-SwAD-0101]]\r[Source: [TSP-MPS-MCU-SwAD-0102]]\r[Source: [TSP-MPS-MCU-SwAD-0103]]\r[Source: [TSP-MPS-MCU-SwAD-0104]]\r[Source: [TSP-MPS-MCU-SwAD-0105]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0030]"",""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0078]"",""[TSP-MPS-MCU-SwAD-0079]"",""[TSP-MPS-MCU-SwAD-0011]"",""[TSP-MPS-MCU-SwAD-0051]"",""[TSP-MPS-MCU-SwAD-0041]"",""[TSP-MPS-MCU-SwAD-0023]"",""[TSP-MPS-MCU-SwAD-0024]"",""[TSP-MPS-MCU-SwAD-0101]"",""[TSP-MPS-MCU-SwAD-0102]"",""[TSP-MPS-MCU-SwAD-0103]"",""[TSP-MPS-MCU-SwAD-0104]"",""[TSP-MPS-MCU-SwAD-0105]""],""input"":""TSP_Conf.ini\rApp type = 0x03\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rCall the MCU1 TSP_Proc_Init_Pre module.\rStart MPU1_Sim reply the app type message to MCU1\r\u0007"",""exp_step"":""Init start time shall be recorded.\rThe MCU1_State and system state shall be firstly initialized TSP_PROC_STATE_UNKNOWN. \rTSP_Proc_Init_MD5Chk check MD5Chk.lst successfully.\rMCU1 receive app type message successfully. (include two field:apptype=1, mpsReboot=0)\rBIT_Task, SLOT_MGR, GM_MUDP_Rx_Task and Main_Task shall be created.\rThe return value of TSP_Proc_Init_Pre module shall be GM_TRUE.\rThe value of l_globalinfo_struct.MCU1_State shall be TSP_PROC_SELF_INIT_OK(0x5C8917D9).\rTSP_PROC_SELF_INIT_OK message shall be send to SDMS.\rThe message:\rLocal INIT OK 0x5C8917D9\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0100"",""description"":""TSP-MPS-MCU-SwITC-0100\rTo check when the initialization of Timer module failed, MCU1 will send the error print message to SDMS and enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Stub the TSP_Proc_Timer_Init, let it returns GM_FALSE.\rStart the MCU1 INIT_TASK.\rCall the MCU1 TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU1 shall send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_Proc_Timer_Init Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0102"",""description"":""TSP-MPS-MCU-SwITC-0102\rTo check when the initialization of VSN module failed, MCU1 will send the error print message to SDMS and MCU1 will be fail.\r\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0030]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0030]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let GM_VSN_Init return GM_FALSE.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU1 shall send the error print message to SDMS and enter in endless loop.\rError print message:\rGM_VSN_Init Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0101"",""description"":""TSP-MPS-MCU-SwITC-0101\rTo check when the initialization of Read config file module failed, MCU1 will set the light of TSP_READ_CONF_ERR (0x0101) and enter in endless loop. \r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let GM_Get_File_Size return -1.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU1 shall set the light of TSP_READ_CONF_ERR (0x0101) and enter in endless loop.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0185"",""description"":""TSP-MPS-MCU-SwITC-0185\rTo check when the initialization of load version module failed, MCU1 will set the light of TSP_VER_LOAD_ERR (0x0104) and enter in endless loop. \r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let TSP_Proc_VerChk_load return GM_FALSE.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU1 shall set the light of TSP_VER_LOAD_ERR (0x0104) and enter in endless loop.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0182"",""description"":""TSP-MPS-MCU-SwITC-0182\rTo check when the MD5chk module failed, MCU1 will set the light of TSP_MD5CHK_INIT_ERR (0x0103) and enter in endless loop. \r[Source: [TSP-MPS-MCU-SwAD-0030]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0030]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let theTSP_Proc_Init_MD5Chreturn false.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Entry module.\r\u0007"",""exp_step"":""1, MCU1 shall set the light of TSP_MD5CHK_INIT_ERR (0x0103)and enter in endless loop.\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0060"",""description"":""TSP-MPS-MCU-SwITC-0060\rTo check when the MTYPE_MGR and ADDR_MGR module failed, MCU1 will set the light of TSP_ADDR_INIT_ERR (0x0106) and enter in endless loop. \r[Source: [TSP-MPS-MCU-SwAD-0078]]\r[Source: [TSP-MPS-MCU-SwAD-0079]]\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0078]"",""[TSP-MPS-MCU-SwAD-0079]"",""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let the TSP_Addr_Init return false.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU1 shall set the light of TSP_ADDR_INIT_ERR (0x0106) and enter in endless loop.\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0073"",""description"":""TSP-MPS-MCU-SwITC-0073\rTo check when the initialization of LCOM module failed, MCU1 will send error print message to SDMS and enter in endless loop. \r\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0023]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0023]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let TSP_LCOM_Init return GM_FALSE.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU1 shall send error print message to SDMS and enter in endless loop.\rError print message:\rTSP_LCOM_Interface_Init Fail!\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0186"",""description"":""TSP-MPS-MCU-SwITC-0186\rTo check when the initialization of SCOM module failed, MCU1 will send error print message to SDMS and enter in endless loop. \r\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let TSP_SCOM_Interface_Init return GM_FALSE.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU1 shall send error print message to SDMS and enter in endless loop.\rError print message:\rTSP_SCOM_Interface_Init Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0132"",""description"":""TSP-MPS-MCU-SwITC-0132\rTo check when the initialization of RSSP module failed, MCU1 will set the light of TSP_RSSP_INIT_ERR(0x0102) and enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0105]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0105]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let TSP_RSSP_Interface_Init return GM_FALSE.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU1 shall set the light of TSP_RSSP_INIT_ERR(0x0102) and enter in endless loop.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0134"",""description"":""TSP-MPS-MCU-SwITC-0134\rTo check when the initialization of app configure module failed, MCU1 will send the error print message to SDMS and  enter in endless loop. \r\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let TSP_Proc_Init_AppCfg_Proc return GM_FALSE.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU1 shall send the error print message to SDMS.\rError print message:\rTSP_Proc_Init_AppCfg_Proc Fail!\r2, MCU1 shall enter in endless loop.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0179"",""description"":""TSP-MPS-MCU-SwITC-0179\rTo check when MCU1 is not received application type from MPU1, MCU1 will set the error number in LED, send the error print message to SDMS and enter in endless loop. \r\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0023]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0023]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Stub MCU1 send app type request failed\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU1 shall set the light of TSP_SELF_INIT_ERR (0x0211) and enter in endless loop.\r2,MCU1 shall send the error print message to SDMS.\rError print message:\rSend MSG_APP_CFG_REQ_INFO Fail!\rTSP_Proc_Init_Is_Timeout TIME OUT: (init time)\rMCU1 self init fail\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0188"",""description"":""TSP-MPS-MCU-SwITC-0188\rTo check when MCU1 received unknow application type from MPU1, MCU1 will send the error print message to SDMS and enter in endless loop. \r\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0023]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0023]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let MPU1_Sim send unknow app type to MCU1\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1,MCU1 shall send the error print message to SDMS and enter in endless loop.\rError print message:\rApplicationType is unknow!\rTSP_Proc_Syn_Clock_Init Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0075"",""description"":""TSP-MPS-MCU-SwITC-0075\rTo check when the initialization of Syn Clock module failed, MCU1will send the error print message to SDMS and enter in endless loop\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""Let the TSP_Proc_Syn_Clock_Init returns GM_False.\rStart the MCU1 INIT_TASK.\rCall the MCU1 TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU1will shall send the error print message to SDMS and enter endless loop.\rError print message:\rTSP_Proc_Syn_Clock_Init Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0077"",""description"":""TSP-MPS-MCU-SwITC-0077\rTo check when the initialization of CHKW module failed, MCU1 will send the error print message to SDMS and enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let the TSP_CHKW_Init returns GM_False.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU1 will send the error print message to SDMS and enter endless loop.\rError print message:\rTSP_CHKW_Init Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0078"",""description"":""TSP-MPS-MCU-SwITC-0078\rTo check when the initialization of VPS_CHKW module failed, MCU1 will send the error print message to SDMS and enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""Let the TSP_VPS_CHKW_Init returns GM_False.\rStart the MCU1 INIT_TASK.\rCall the MCU1 TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU1 will send the error print message to SDMS and enter endless loop.\rError print message:\rTSP_VPS_CHKW_Init Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0079"",""description"":""TSP-MPS-MCU-SwITC-0079\rTo check when the initialization of Init_Main module failed, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let the TSP_Proc_Main_Init returns GM_False.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU1 will send the error print message to SDMS and enter endless loop.\rError print message:\rTSP_Proc_Main_Init Fail!\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0080"",""description"":""TSP-MPS-MCU-SwITC-0080\rTo check when the initialization of BIT_Interface module failed, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Stub the TSP_BIT_Interface_Init, let it returns GM_False.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU1 will send the error print message to SDMS and enter endless loop.\rError print message:\rTSP_BIT_Interface_Init Fail!\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0306"",""description"":""TSP-MPS-MCU-SwITC-0306\rTo check when the initialization of GM condition module failed, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Stub the GM_Condition_Init, let it returns GM_False.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU1 will send the error print message to SDMS and enter endless loop.\rError print message:\rGM_Condition_Init Init_cond Fail!\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0302"",""description"":""TSP-MPS-MCU-SwITC-0302\rTo check when the initialization of PCOM module failed, MCU1 will send the error print message to SDMS and enter in endless loop. \r\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let TSP_PCOM_Init return GM_FALSE.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU1 shall send the error print message to SDMS and enter endless loop.\rError print message:\rInit TSP_PCOM_Interface_Init Fail!\rTSP_Proc_Init_Pre Fail!\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0253"",""description"":""TSP-MPS-MCU-SwITC-0253\rTo check when MCU1 not self init ok in 270s, MCU1 will show the error number in LED and enter in endless loop, lock OS.\r\r[Source: [TSP-MPS-MCU-SwAD-0031]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0030]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0031]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0030]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rSleep 270s before MCU1 self init ok.\r\u0007"",""exp_step"":""1, MCU1 shall show error number TSP_SELF_INIT_ERR (0x0211) in LED and enter in endless loop.\r2, enter “i”, no info shall be indicated in telnet.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0004"",""description"":""TSP-MPS-MCU-SwITC-0004\rTo check the MCU1 could negotiate with other three local MCU2, MPU1 and MPU2.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0023]]\r[Source: [TSP-MPS-MCU-SwAD-0069]]\r[Source: [TSP-MPS-MCU-SwAD-0046]]\r[Source: [TSP-MPS-MCU-SwAD-0030]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0023]"",""[TSP-MPS-MCU-SwAD-0069]"",""[TSP-MPS-MCU-SwAD-0046]"",""[TSP-MPS-MCU-SwAD-0030]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the TestBench.\rStart the MCU1 INIT_TASK.\rCall the function TSP_Proc_InnerSyS_Nego of TSP_Proc_Init_Post module.\rLet the local MCU2_Sim, MPU1_Sim and MPU2_Sim in the TestBench reply the TSP_COM_MSG_TYPE_MCU1_INIT_OK message to MCU1.\r\u0007"",""exp_step"":""1, After step 4, TSP_COM_MSG_TYPE_MCU1_INIT_OK shall be sent to MPU1, MPU2 and MCU2.\r2, After step 4, MCU1 shall receive the TSP_COM_MSG_TYPE_MPU1_INIT_OK/TSP_COM_MSG_TYPE_MPU2_INIT_OK from MPU and TSP_COM_MSG_TYPE_MCU2_INIT_OK from MCU2 in continues 3 cycles. And the return value of TSP_Proc_InnerSyS_Nego shall be GM_TRUE. l_globalinfo_struct.g_sys_running_state shall be TSP_PROC_ALL_INIT_OK.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0121"",""description"":""TSP-MPS-MCU-SwITC-0121\rTo check when MCU1 failed to send TSP_COM_MSG_TYPE_MCU1_INIT_OK to MPU1, MCU1 will add the error print message to the msg Queue and send it to SDMS.\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rLet TSP_LCOM_Interface_Send of TSP_COM_MSG_TYPE_MCU1_INIT_OK msg to MPU1 return GM_FALSE.\rCall the TSP_Proc_Init_Negotiation module.\r\u0007"",""exp_step"":""1, MCU1 shall add the error print message to the msg Queue and send to SDMS.\rError print message:\rTSP_SEND_SELFINITOK_TO_MPU1_ERR\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0122"",""description"":""TSP-MPS-MCU-SwITC-0122\rTo check when MCU1 failed to send TSP_COM_MSG_TYPE_MCU1_INIT_OK to MPU2, MCU1 will add the error print message to the msg Queue and send it to SDMS.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rLet TSP_LCOM_Interface_Send of TSP_COM_MSG_TYPE_MCU1_INIT_OK msg to MPU2 return GM_FALSE.\rCall the TSP_Proc_Init_Negotiation module.\r\u0007"",""exp_step"":""1, MCU1 shall add the error print message to the msg Queue and send to SDMS.\rError print message:\rTSP_SEND_SELFINITOK_TO_MPU2_ERR\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0123"",""description"":""TSP-MPS-MCU-SwITC-0123\rTo check when MCU1 failed to send TSP_COM_MSG_TYPE_MCU1_INIT_OK to MCU2, MCU1 will add the error print message to the msg Queue and send it to SDMS.\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rLet TSP_LCOM_Interface_Send of TSP_COM_MSG_TYPE_MCU1_INIT_OK msg to MPU2 return GM_FALSE.\rCall the TSP_Proc_Init_Negotiation module.\r\u0007"",""exp_step"":""1, MCU1 shall add the error print message to the msg Queue and send to SDMS.\rError print message:\rTSP_SEND_SELFINITOK_TO_MCU2_ERR\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0105"",""description"":""TSP-MPS-MCU-SwITC-0105\rTo check when MCU1 does not receive self init ok message from MPU1 in 270 seconds, MCU1 will show error number TSP_MPU1_INIT_ERR (0x020B) in LED, enter in endless loop and lock OS.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""MPU1_Sim don’t send self init ok message to MCU1.\rMPU2_Sim send self init ok message to MCU1.\rMCU2_Sim send self init ok message to MCU1.\rStart the MCU1 INIT_TASK.\rCall the TSP_Proc_Timer_Int module.\r\u0007"",""exp_step"":""1, MCU1 shall show error number TSP_MPU1_INIT_ERR (0x020B) in LED and enter in endless loop.\r2, MCU1 shall be fail.\r3, enter “i”, no info shall be indicated in telnet.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0106"",""description"":""TSP-MPS-MCU-SwITC-0106\rTo check when MCU1 not receive self init ok message from MPU2 in 270 second, MCU1 will show error number TSP_MPU2_INIT_ERR (0x020C) in LED, enter in endless loop and lock OS\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""MPU1_Sim send self init ok message to MCU1.\rMPU2_Sim don’t send self init ok message to MCU1.\rMCU2_Sim send self init ok message to MCU1.\rStart the MCU1 INIT_TASK.\rCall the TSP_Proc_Timer_Int module.\r\u0007"",""exp_step"":""1, MCU1 shall show the error number TSP_MPU2_INIT_ERR (0x020C) in LED and enter in endless loop.\r2, MCU1 shall be fail.\r3, enter “i”, no info shall be indicated in telnet.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0107"",""description"":""TSP-MPS-MCU-SwITC-0107\rTo check when MCU1 not receive self init ok message from MCU2 in 270 second, MCU1will show the error number TSP_MCU2_INIT_ERR (0x020D) in LED, enter in endless loop and lock OS.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""MPU1_Sim send self init ok message to MCU1.\rMPU2_Sim send self init ok message to MCU1.\rMCU2_Sim don’t send self init ok message to MCU1.\rStart the MCU1 INIT_TASK.\rCall the TSP_Proc_Timer_Int module.\r\u0007"",""exp_step"":""1, MCU1 shall show the error number TSP_MCU2_INIT_ERR (0x020D) in LED and enter in endless loop.\r2, MCU1 shall be fail.\r3, enter “i”, no info shall be indicated in telnet.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0007"",""description"":""TSP-MPS-MCU-SwITC-0007\rTo check the MCU1 could control other three local MCU2, MPU1 and MPU2 to complete the version check process.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0069]]\r[Source: [TSP-MPS-MCU-SwAD-0030]]\r[Source: [TSP-MPS-MCU-SwAD-0023]]\r[Source: [TSP-MPS-MCU-SwAD-0098]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0069]"",""[TSP-MPS-MCU-SwAD-0030]"",""[TSP-MPS-MCU-SwAD-0023]"",""[TSP-MPS-MCU-SwAD-0098]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the TestBench.\rStart the MCU1 INIT_TASK.\rCall the TSP_Prco_Version_Check.\rLet the local MPU1_Sim and MPU2_Sim in the TestBench send the version check result message back to the MCU1.\r\u0007"",""exp_step"":""1, The return value of MCU1 sending version exchange message to MCU2 and MPU shall be True.\r2, The time interval between sending version exchange message and version check message shall be 308ms (616ms for CCS).\r3, MCU1 shall read the version result message 28ms (56ms for CCS) after sending version check message.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0112"",""description"":""TSP-MPS-MCU-SwITC-0112\rTo check when MCU1 failed to send version exchange information to MPU1, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let the TSP_LCOM_Interface_Send of version exchange information to MPU1 returns GM_FALSE.\rStart the MCU1 INIT_TASK.\rCall the MCU1 TSP_Proc_Post_Init module.\r\u0007"",""exp_step"":""1, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\rError print message:\rSend MSG_TYPE_VER_EXCHANGE to MPU1 Fail!\rTSP_Prco_Version_Check Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0113"",""description"":""TSP-MPS-MCU-SwITC-0113\rTo check when MCU1 failed to send version exchange information to MPU2, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let the TSP_LCOM_Interface_Send of version exchange information to MPU2 returns GM_FALSE.\rStart the MCU1 INIT_TASK.\rCall the MCU1 TSP_Proc_Post_Init module.\r\u0007"",""exp_step"":""1, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\rError print message:\rSend MSG_TYPE_VER_EXCHANGE to MPU2 Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0114"",""description"":""TSP-MPS-MCU-SwITC-0114\rTo check when MCU1 failed to send version exchange information to MCU2, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task. \r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let the TSP_LCOM_SIO_Write of version exchange information to MCU2 returns GM_FALSE.\rStart the MCU1 INIT_TASK.\rCall the MCU1 TSP_Proc_Post_Init module.\r\u0007"",""exp_step"":""1, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\rError print message:\rSend MSG_TYPE_VER_EXCHANGE to MCU2 Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0115"",""description"":""TSP-MPS-MCU-SwITC-0115\rTo check when MCU failed to send version information to MPU1, MCU1/MCU2 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0083]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0083]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let the TSP_LCOM_Interface_Send of version information to MPU1 returns GM_FALSE.\rStart the MCU1 INIT_TASK.\rCall the MCU1 TSP_Proc_Post_Init module.\r\u0007"",""exp_step"":""1, MCU1/MCU2 will send the error print message to SDMS and enter in endless loop, kill main task.\rError print message:\rTSP_Prco_Version_Exchange,Send Self-Version to MPU1 Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0116"",""description"":""TSP-MPS-MCU-SwITC-0116\rTo check when MCU failed to send version information to MPU2, MCU1/MCU2 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0083]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0083]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let the TSP_LCOM_Interface_Send of version information to MPU2 returns GM_FALSE.\rStart the MCU1 INIT_TASK.\rCall the MCU1 TSP_Proc_Post_Init module.\r\u0007"",""exp_step"":""1, MCU1/MCU2 will send the error print message to SDMS and enter in endless loop, kill main task.\rError print message:\rTSP_Prco_Version_Exchange,Send Self-Version to MPU1 Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0118"",""description"":""TSP-MPS-MCU-SwITC-0118\rTo check when MCU1 don’t send version check message to MPU1, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rLet TSP_LCOM_Interface_Send of version check message to MPU1 return GM_FALSE.\rCall the MCU1 TSP_Proc_Post_Init module.\r\u0007"",""exp_step"":""1, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\rError print message:\rSend MSG_TYPE_VER_CHECK to MPU1 Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0119"",""description"":""TSP-MPS-MCU-SwITC-0119\rTo check when MCU1 don’t send version check message to MPU2, MCU1 will send the error of TSP_TX_VER_CHECK_MPU2_ERR (0x0246) to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rLet TSP_LCOM_Interface_Send of version check message to MPU2 return GM_FALSE.\rCall the MCU1 TSP_Proc_Post_Init module.\r\u0007"",""exp_step"":""1, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\rError print message:\rSend MSG_TYPE_VER_CHECK to MPU2 Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0254"",""description"":""TSP-MPS-MCU-SwITC-0254\rTo check when MCU1 not receive version check result from MPU1, MCU1 will send the error print message to SDMS and MCU1 will be fail, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0030]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0030]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rSet MPU1_Sim do not send Version Check message to MCU1.\rCall the MCU1 TSP_Proc_Post_Init module.\r\u0007"",""exp_step"":""1, MCU1 will send the error print message to SDMS and enter in endless loop.\r2, MCU1 shall be failed.\r3, main task shall be killed.\rError print message: \rg_globalinfo_struct.g_sys_running_state = TSP_PROC_STATE_FAIL\rMPU1’s version check is no-match!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0255"",""description"":""TSP-MPS-MCU-SwITC-0255\rTo check when MCU1 not receive version check result from MPU2, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rSet MPU2_Sim not send Version Check message to MCU1.\rCall the MCU1 TSP_Proc_Post_Init module.\r\u0007"",""exp_step"":""1, MCU1 will send the error print message to SDMS and enter in endless loop.\r2, MCU1 shall be fail.\r3, main task shall be killed.\rError print message:\rMPU2’s version check is no-match!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0307"",""description"":""TSP-MPS-MCU-SwITC-0307\rTo check when MCU1 receive error version check result from MPU1, MCU1 will send the error print message to SDMS and MCU1 will be fail, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0030]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0030]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rSet MPU1_Sim do send error Version Check message to MCU1.\rCall the MCU1 TSP_Proc_Post_Init module.\r\u0007"",""exp_step"":""1, MCU1 will send the error print message to SDMS and enter in endless loop.\r2, MCU1 shall be failed.\r3, main task shall be killed.\rError print message:\rMPU1’s version check is no-match!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0308"",""description"":""TSP-MPS-MCU-SwITC-0308\rTo check when MCU1 receive error version check result from MPU2, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rSet MPU2_Sim send error Version Check message to MCU1.\rCall the MCU1 TSP_Proc_Post_Init module.\r\u0007"",""exp_step"":""1, MCU1 will send the error print message to SDMS and enter in endless loop.\r2, MCU1 shall be fail.\r3, main task shall be killed.\rError print message:\rMPU2’s version check is no-match!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0321"",""description"":""TSP-MPS-MCU-SwITC-0321\rTo check the MCU1 could control local MPU1 send configure data to local and control local MCU2 part2 init process.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0069]]\r[Source: [TSP-MPS-MCU-SwAD-0023]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0069]"",""[TSP-MPS-MCU-SwAD-0023]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the TestBench.\rStart the MCU1 INIT_TASK.\rMCU1 send configure exchange message to MPU1_Sim\rMPU1_Sim receive configure exchange message and send configure data to MCU2_Sim.\rMCU1 send initpart2nd message to MCU2_Sim\rMCU2_Sim read the configure data and initialize MCU2 second stage.\r\u0007"",""exp_step"":""1, MCU1 shall send configure exchange message to MPU1_Sim successfully.\r2, MPU1_Sim shall read configure exchange message from MCU\r3, initpart2nd message shall be sent after 72*28ms (72*56msfor CCS) to MCU2_Sim.\r4, MCU2_Sim shall read the initpart2nd message from MCU1\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0322"",""description"":""TSP-MPS-MCU-SwITC-0322\rTo check when MCU1 failed to send configure exchange information to MPU1, MCU1 shall send the error print message to SDMS and enter in endless loop, kill main task.\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the TestBench.\rStart the MCU1 INIT_TASK.\rLet TSP_LCOM_Interface_Send of configure exchange message to MPU1 return GM_FALSE.\rCall the MCU1 TSP_Proc_Post_Init module.\r\u0007"",""exp_step"":""MCU1 will enter in endless loop, kill main task.\rMCU1 send error print message to SDMS\rError print message:\rSend MSG_TYPE_CONFIG_EXCHANGE to MPU1 Fail!\rTSP_Prco_Config_Interactive Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0323"",""description"":""TSP-MPS-MCU-SwITC-0323\rTo check when MCU1 failed to send self init2nd information to MPU1, MCU1 shall send the error print message to SDMS and enter in endless loop, kill main task.\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the TestBench.\rStart the MCU1 INIT_TASK.\rLet TSP_LCOM_Interface_Send of init 2nd message to MPU1 return GM_FALSE.\rCall the MCU1 TSP_Proc_Post_Init module.\r\u0007"",""exp_step"":""MCU1 will enter in endless loop, kill main task.\rMCU1 send error print message to SDMS\rError print message:\rSend MSG_TYPE_INIT_PART_2ND to MCU2 Fail!\rTSP_Prco_Config_Interactive Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0009"",""description"":""TSP-MPS-MCU-SwITC-0009\rTo check the MCU1 could control MPU executed N/R negotiation with peer MPS, and get N/R state, start running Main_Task if MCU1 is Normal.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0069]]\r[Source: [TSP-MPS-MCU-SwAD-0030]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""TSP_Conf.ini\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0069]"",""[TSP-MPS-MCU-SwAD-0030]""],""input"":""Start the TestBench which will run MCU2_Sim, MPU1_Sim, MPU2_Sim.\rStart the MCU1 INIT_TASK.\rCall the TSP_Proc_NR_Nego function of TSP_Proc_Post_Init module.\rLet MPU1_Sim return message indicate local MPS is Normal.\r\u0007"",""exec_step"":""1, MCU1 shall send NR negotiation message every 336ms (672ms for CCS) for 3 times.\r2, NR check message shall be sent after 336ms (672ms for CCS) to MPU1, and after 28ms (56ms for CCS) to MPU2.\r3, MCU1 shall wait for 56ms (112ms for CCS) before reading the NR information message. \r4, The value of g_globalinfo_struct.g_sys_running_state shall be TSP_PROC_NORMAL_RUNNING.\r5, The g_globalinfo_struct.NR_Flag shall be TSP_Proc_Syn_AS_STATE_ACTIVE.\r6, Main_Task shall be running step by step.\r\u0007"",""exp_step"":""1, MCU1 shall send NR negotiation message every 336ms (672ms for CCS) for 3 times.\r2, NR check message shall be sent after 336ms (672ms for CCS) to MPU1, and after 28ms (56ms for CCS) to MPU2.\r3, MCU1 shall wait for 56ms (112ms for CCS) before reading the NR information message. \r4, The value of g_globalinfo_struct.g_sys_running_state shall be TSP_PROC_NORMAL_RUNNING.\r5, The g_globalinfo_struct.NR_Flag shall be TSP_Proc_Syn_AS_STATE_ACTIVE.\r6, Main_Task shall be running step by step.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0010"",""description"":""TSP-MPS-MCU-SwITC-0010\rTo check the MCU1 could control MPU1 executed N/R negotiation with peer MPS, and get N/R state. If current state is reverse, then waits for starting main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0069]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0069]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the TestBench which will run MCU2_Sim, MPU1_Sim, MPU2_Sim.\rStart the MCU1 INIT_TASK.\rCall the TSP_Proc_NR_Nego function of TSP_Proc_Post_Init module.\rLet MPU1_Sim return message indicate local MPS is Reserve.\r\u0007"",""exp_step"":""1, The value of g_globalinfo_struct.g_sys_running_state shall be TSP_PROC_SYS_INIT_OK.\r2, TSP_SYS_INIT_OK message shall be send to SDMS.\r3, The g_globalinfo_struct.NR_Flag shall be TSP_Proc_Syn_AS_STATE_STANDBY.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0120"",""description"":""TSP-MPS-MCU-SwITC-0120\rTo check when MCU1 failed to send N/R negotiation message to MPU1, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rLet TSP_LCOM_SIO_Write of N/R negotiation message to MPU1 return GM_FALSE.\rCall the TSP_Proc_NR_Nego module.\r\u0007"",""exp_step"":""1, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\rError print message:\rSend MSG_TYPE_START_NR_NEGO to MPU1 Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0128"",""description"":""TSP-MPS-MCU-SwITC-0128\rTo check when MCU1 failed to send NR check message to MPU1, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rLet TSP_LCOM_SIO_Write of NR check messag to MPU1 return GM_FALSE.\rCall the TSP_Proc_NR_Nego module.\r\u0007"",""exp_step"":""1, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\rError print message:\rSend MSG_TYPE_START_NR_CHECK to MPU1 Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0129"",""description"":""TSP-MPS-MCU-SwITC-0129\rTo check when MCU1 failed to send NR check message to MPU2, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rLet TSP_LCOM_SIO_Write of NR check message to MPU2 return GM_FALSE.\rCall the TSP_Proc_NR_Nego module.\r\u0007"",""exp_step"":""1, MCU1 will send the error print message to SDMS and enter in endless loop, kill main task.\rError print message:\rSend MSG_TYPE_START_NR_CHECK to MPU2 Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0108"",""description"":""TSP-MPS-MCU-SwITC-0108\rTo check when MCU1 not receive NR state message from MPU1 in 270 second, MCU1will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rMPU1_Sim don’t send NR flag to MCU1.\rCall the TSP_Proc_NR_Nego module.\r\u0007"",""exp_step"":""1, MCU1will send the error print message to SDMS and enter in endless loop, kill main task.\rError print message:\rNot receive NR message from MPU1!\rTSP_Proc_NR_Nego Fail!\rTSP_Proc_Post_Init Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0252"",""description"":""TSP-MPS-MCU-SwITC-0252\rTo check when MCU1 not receive NR State message from MPU2 in 270 second, MCU1will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rMPU2_Sim don’t send NR flag to MCU1.\rCall the TSP_Proc_NR_Nego module.\r\u0007"",""exp_step"":""1, MCU1will send the error print message to SDMS and enter in endless loop, kill main task.\rError print message:\rNot receive NR message from MPU2!\rTSP_Proc_NR_Nego Fail!\rTSP_Proc_Post_Init Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0187"",""description"":""TSP-MPS-MCU-SwITC-0187\rTo check when MCU1 receive unequal NR State message from MPU1/2, MCU1will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0066]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0066]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\rMPU1 NR State: Normal\rMPU2 NR State: Reserve\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rMPU1/2_Sim send NR flag to MCU1.\rCall the TSP_Proc_NR_Nego module.\r\u0007"",""exp_step"":""1, MCU1will send the error print message to SDMS and enter in endless loop, kill main task.\rError print message:\rMSG_NR_INFO state fail \r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0109"",""description"":""TSP-MPS-MCU-SwITC-0109\rWhen MCU1 is in TSP_PROC_SYS_INIT_OK and Reserved state, no NR clock syn message has been received in 3 cycles, MCU1 will show the error of TSP_INIT_CLK_NOT_RCV (0x0210) in LED, lock OS and enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0069]]\r[Source: [TSP-MPS-MCU-SwAD-0025]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0069]"",""[TSP-MPS-MCU-SwAD-0025]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Stub the function TSP_Proc_Syn_Clock_Latest_RcvStamp, let it return GM_FALSE.\rStart the TestBench which will return the Reserved state to the MCU1.\rStart the MCU1 INIT_TASK.\rCall TSP_Proc_Timer_Int module.\r\u0007"",""exp_step"":""1, MCU1 will show the error of TSP_INIT_CLK_NOT_RCV (0x0210) in LED.\r2, enter “i”, no info shall be indicated in telnet.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0191"",""description"":""TSP-MPS-MCU-SwITC-0191\rWhen MCU1 is in TSP_PROC_SYS_INIT_OK and Reserved state, if MCU1 send VSN message to MPU1 failed, MCU1 will show the error of TSP_INIT_VSN_SND_ERR (0x0705) in LED, lock OS and enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0069]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0069]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Stub MCU1 send VSN message to MPU1 failed\rStart the TestBench which will return the Reserved state to the MCU1.\rStart the MCU1 INIT_TASK.\rCall TSP_Proc_Timer_Int module.\r\u0007"",""exp_step"":""1, MCU1 will show the error of TSP_INIT_VSN_SND_ERR (0x0705) in LED.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0012"",""description"":""TSP-MPS-MCU-SwITC-0012\rTo check the MCU2 TSP_Proc_Init_Pre could initialize correctly.\r\r[Source: [TSP-MPS-MCU-SwAD-0034]]\r[Source: [TSP-MPS-MCU-SwAD-0035]]\r[Source: [TSP-MPS-MCU-SwAD-0078]]\r[Source: [TSP-MPS-MCU-SwAD-0079]]\r[Source: [TSP-MPS-MCU-SwAD-0050]]\r[Source: [TSP-MPS-MCU-SwAD-0067]]\r[Source: [TSP-MPS-MCU-SwAD-0051]]\r[Source: [TSP-MPS-MCU-SwAD-0024]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0034]"",""[TSP-MPS-MCU-SwAD-0035]"",""[TSP-MPS-MCU-SwAD-0078]"",""[TSP-MPS-MCU-SwAD-0079]"",""[TSP-MPS-MCU-SwAD-0050]"",""[TSP-MPS-MCU-SwAD-0067]"",""[TSP-MPS-MCU-SwAD-0051]"",""[TSP-MPS-MCU-SwAD-0024]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU2 INIT_TASK.\rCall the TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, Init start time shall be recorded.\r2, The MCU1_State and System State shall be firstly initialized TSP_PROC_STATE_UNKNOWN (0x71378BC7).\r3, INIT_CHECK, GM_MUDP_Rx_Task shall be created.\r4, The return value of TSP_Proc_Init_Pre module shall be GM_TRUE.\r5, The value of l_globalinfo_struct.MCU2_State shall be TSP_PROC_SELF_INIT_OK.\r6, TSP_SELF_INIT_OK message shall be send to SDMS.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0324"",""description"":""TSP-MPS-MCU-SwITC-0324\rTo check the MCU2 TSP_Proc_Init_AppCfg_Proc could initialize correctly if the application type is ZC/LC.\r[Source: [TSP-MPS-MCU-SwAD-0034]]\r[Source: [TSP-MPS-MCU-SwAD-0099]]\r[Source: [TSP-MPS-MCU-SwAD-0097]]\r[Source: [TSP-MPS-MCU-SwAD-0115]]\r[Source: [TSP-MPS-MCU-SwAD-0092]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0034]"",""[TSP-MPS-MCU-SwAD-0099]"",""[TSP-MPS-MCU-SwAD-0097]"",""[TSP-MPS-MCU-SwAD-0115]"",""[TSP-MPS-MCU-SwAD-0092]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU2 INIT_TASK.\rMPU1_Sim send configure file data message to MCU2\rMCU1 send second stage init message to MCU2\rMCU2 receive init part2 message and call the TSP_Proc_Init_AppCfg_Proc module.\rIn MCU2, stub to print the maximum communication capacity, include message number and size.\rIn MCU2, stub to print the inter red network IP and inter blue network IP.\r\u0007"",""exp_step"":""1, MCU2 read the configure data correctly.\r2, GCOM,SNTP,RMS,RAW and RSSP1 initialize correctly\r3, MainRxTask, Main Task and BIT task shall be created.\r4, The return value of TSP_Proc_Init_AppCfg_Proc module shall be GM_TRUE.\r5, The value of mpsReboot in APP_Config.bin is 0.\r6, The maximum number of communication message is 200 and the total size is 40KB.\r5, The value of l_globalinfo_struct.g_sys_running_state shall be TSP_PROC_SYS_INIT_OK\r6, TSP_SYS_INIT_OK message shall be send to SDMS.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0135"",""description"":""TSP-MPS-MCU-SwITC-0135\rTo check the MCU2 TSP_Proc_Init_AppCfg_Proc could initialize correctly if the application type is CCS. \r[Source: [TSP-MPS-MCU-SwAD-0034]]\r[Source: [TSP-MPS-MCU-SwAD-0099]]\r[Source: [TSP-MPS-MCU-SwAD-0097]]\r[Source: [TSP-MPS-MCU-SwAD-0115]]\r[Source: [TSP-MPS-MCU-SwAD-0092]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0034]"",""[TSP-MPS-MCU-SwAD-0099]"",""[TSP-MPS-MCU-SwAD-0097]"",""[TSP-MPS-MCU-SwAD-0115]"",""[TSP-MPS-MCU-SwAD-0092]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU2 INIT_TASK.\rMPU1_Sim send configure file data message to MCU2\rMCU1 send second stage init message to MCU2\rMCU2 receive init part2 message and call the TSP_Proc_Init_AppCfg_Proc module.\rIn MCU2, stub to print the maximum communication capacity, include message number and size.\rIn MCU2, stub to print the inter red network IP and inter blue network IP.\r\u0007"",""exp_step"":""1, MCU2 read the configure data correctly.\r2, GCOM,SNTP,RMS RSSP1 RSSP2\u0026Subset037initialize correctly\r3, MainRxTask, Main Task and BIT task shall be created.\r3, The return value of TSP_Proc_Init_AppCfg_Proc module shall be GM_TRUE.\r4, The value of mpsReboot in APP_Config.bin is 0.5, The maximum number of communication message is 200 and the total size is 60KB.\r6, MCU-A: \rinter red network IP is 192.100.69.1\rinter blue network IP is 192.100.79.1\rMCU-B:\rinter red network IP is 192.100.69.2\rinter blue network IP is 192.100.79.2\r7, The value of l_globalinfo_struct.g_sys_running_state shall be TSP_PROC_SYS_INIT_OK\r8, TSP_SYS_INIT_OK message shall be send to SDMS.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0183"",""description"":""TSP-MPS-MCU-SwITC-0183\rTo check when the MD5chk module failed, MCU2 will enter in endless loop. \r[Source: [TSP-MPS-MCU-SwAD-0034]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0034]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let theTSP_Proc_Init_MD5Chreturn false.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Entry module.\r\u0007"",""exp_step"":""1, MCU2 shall enter in endless loop.\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0082"",""description"":""TSP-MPS-MCU-SwITC-0082\rTo check when the initialization of GCOM_Init module failed, MCU2 will send the error print message to SDMS and enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0099]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0034]]\r\r [Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0099]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0034]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""Stub the TSP_GCOM_Init, let it returns GM_False.\rStart the MCU2 INIT_TASK.\rCall the MCU2 TSP_Proc_Init_AppCfg_Proc module.\r\u0007"",""exp_step"":""1, MCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_GCOM_Init Fail!\rProcess_APP_Config fail\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0081"",""description"":""TSP-MPS-MCU-SwITC-0081\rTo check when the initialization of SNTP_Init module failed, MCU2 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0099]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0099]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""Stub the TSP_SNTP_Init, let it returns GM_FALSE.\rStart the MCU2 INIT_TASK.\rCall the MCU2 TSP_Proc_Init_AppCfg_Proc module.\r\u0007"",""exp_step"":""1, MCU2 will send the error print message to SDMS and enter in endless loop, kill main task.\rError print message:\rTSP_SNTP_Init Fail!\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0190"",""description"":""TSP-MPS-MCU-SwITC-0190\rTo check if MCU2 read configure message fail, MCU2 will send the error print message to SDMS and enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0099]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0097]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0099]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0097]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""Stub MCU2 read APP_MCU2_CONFIG message failed\rStart the MCU2 INIT_TASK.\rCall the MCU2 TSP_Proc_Init_AppCfg_Proc module.\rSet MPU1_Sim send error type APP_MCU2_CONFIG message to MCU1 and repeat step2~3\rStub MCU2 read APP_RMS_CONFIG message failed and repeat step2~3\rSet MPU1_Sim send error type APP_RMS_CONFIG message to MCU1 and repeat step2~3\rStub MCU2 read APP_GGW_CONFIG message failed and repeat step2~3\rSet MPU1_Sim send error type APP_GGW_CONFIG message to MCU1 and repeat step2~3\rStub MCU2 read APP_RSSP1_CONFIG message failed and repeat step2~3\rSet MPU1_Sim send error type APP_RSSP1_CONFIG message to MCU1 and repeat step2~3\rStub MCU2 read APP_RSSP2_CONFIG message failed and repeat step2~3(CCS)\rSet MPU1_Sim send error type APP_RSSP2_CONFIG message to MCU1 and repeat step2~3(CCS)\rStub MCU2 read APP_S037_CONFIG message failed and repeat step2~3(CCS)\rSet MPU1_Sim send error type APP_S037_CONFIG message to MCU1 and repeat step2~3(CCS)\rStub MCU2 read APP _CONFIG message failed and repeat step2~3\rSet MPU1_Sim send error type APP _CONFIG message to MCU1 and repeat step2~3\rStub MCU2 get APP_MCU2_CONFIG message failed and repeat step2~3\rStub MCU2 get APP_RMS_CONFIG message failed and repeat step2~3\rStub MCU2 get APP _CONFIG message failed and repeat step2~3\rStub MCU2 load MF module failed and repeat step2~3\rStub MCU2 get APP_RSSP1_CONFIG message failed and repeat step2~3\rStub MCU2 get APP_RSSP2_CONFIG message failed and repeat step2~3(CCS)\rStub MCU2 get APP_S037_CONFIG message failed and repeat step2~3(CCS)\rSet MPU1_Sim send error type APP_RAW_CONFIG message to MCU1 and repeat step2~3\rStub MCU2 read APP_RAW_CONFIG message failed and repeat step2~3\rStub MCU2 get APP_RAW_CONFIG message failed and repeat step2~3\rSet MPU1_Sim send error type APP_SNMP_CONFIG message to MCU1 and repeat step2~3\rStub MCU2 read APP_SNMP_CONFIG message failed and repeat step2~3\r\u0007"",""exp_step"":""1, After Step3, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rRead MSG_TYPE_APP_MCU2_CONFIG Fail!\r2, After Step4, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_ConfigMgr_Add MSG_TYPE_APP_MCU2_CONFIG Fail!\r3, After Step5, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rRead MSG_TYPE_APP_RMS_CONFIG Fail!\r4, After Step6, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_ConfigMgr_Add MSG_TYPE_APP_RMS_CONFIG Fail!\r5, After Step7, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rRead MSG_TYPE_APP_GGW_CONFIG Fail!\r6, After Step8, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_ConfigMgr_Add MSG_TYPE_APP_GGW_CONFIG Fail!\r7, After Step9, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rRead MSG_TYPE_APP_RSSP1_CONFIG Fail!\r8, After Step10, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_ConfigMgr_Add MSG_TYPE_APP_RSSP1_CONFIG Fail!\r9, After Step11, the output: (CCS)\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rRead MSG_TYPE_APP_RSSP2_CONFIG Fail!\r10, After Step12, the output: (CCS)\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_ConfigMgr_Add MSG_TYPE_APP_RSSP2_CONFIG Fail!\r11, After Step13, the output: (CCS)\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rRead MSG_TYPE_APP_S037_CONFIG Fail!\r12, After Step14, the output: (CCS)\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_ConfigMgr_Add MSG_TYPE_APP_S037_CONFIG Fail!\r13, After Step15, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rRead MSG_TYPE_APP _CONFIG Fail!\r14, After Step16, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_ConfigMgr_Add MSG_TYPE_APP _CONFIG Fail!\r15, After Step17, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_ConfigMgr_Get CONFIG_TYPE_APP_MCU2 Fail!\r16, After Step18, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_ConfigMgr_Get CONFIG_TYPE_APP_RMS Fail\r17, After Step19, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_ConfigMgr_Get APP_CONF 7 Fail\r18, After Step20, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_MF_Read_AppConf_fromMPU App_Conf Fail\r19, After Step21, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_ConfigMgr_Get CONFIG_TYPE_APP_RSSP1 Fail\r20, After Step22, the output: (CCS)\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_ConfigMgr_Get CONFIG_TYPE_APP_RSSP2 Fail\r21, After Step23, the output: (CCS)\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_ConfigMgr_Get CONFIG_TYPE_APP_S037 Fail\r22, After Step24, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_ConfigMgr_Add MSG_TYPE_APP_RAW_CONFIG Fail!\r23, After Step25, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rRead MSG_TYPE_APP_RAW_CONFIG Fail!\r24, After Step26, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_ConfigMgr_Get CONFIG_TYPE_APP_RAW Fail\r25, After Step27, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_ConfigMgr_Add MSG_TYPE_ APP_SNMP_CONFIG Fail\r25, After Step28, the output:\rMCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rRead MSG_TYPE_ APP_SNMP_CONFIG Fail\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0083"",""description"":""TSP-MPS-MCU-SwITC-0083\rTo check when the initialization of RMS_Init module failed, MCU2 will send the error print message to SDMS and enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0099]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r [Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0099]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""Stub the TSP_RMS_Init, let it returns GM_False.\rStart the MCU2 INIT_TASK.\rCall the MCU2 TSP_Proc_Init_AppCfg_Proc module.\r\u0007"",""exp_step"":""1, MCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_RMS_Init FAIL\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0136"",""description"":""TSP-MPS-MCU-SwITC-0136\rTo check when the initialization of RSSP1_Init module failed, MCU2 will send the error print message to SDMS and enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0099]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0111]]\r\r [Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0099]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0111]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""Stub the TSP_RSSP1_Init, let it returns GM_False.\rStart the MCU2 INIT_TASK.\rCall the MCU2 TSP_Proc_Init_AppCfg_Proc module.\r\u0007"",""exp_step"":""1, MCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_RSSP1_Init Fail\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0137"",""description"":""TSP-MPS-MCU-SwITC-0137\rTo check when the initialization of RSSP2_Init module failed, MCU2 will send the error print message to SDMS and enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0099]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0112]]\r\r [Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0099]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0112]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""Stub the TSP_RSSP2_Init, let it returns GM_False.\rStart the MCU2 INIT_TASK.\rCall the MCU2 TSP_Proc_Init_AppCfg_Proc module.\r\u0007"",""exp_step"":""1, MCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_RSSP2 _Init fail \r \r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0138"",""description"":""TSP-MPS-MCU-SwITC-0138\rTo check when the initialization of S037_Init module failed, MCU2 will send the error print message to SDMS and enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0099]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0113]]\r\r [Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0099]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0113]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""Stub the TSP_S037_Init, let it returns GM_False.\rStart the MCU2 INIT_TASK.\rCall the MCU2 TSP_Proc_Init_AppCfg_Proc module.\r\u0007"",""exp_step"":""1, MCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_S037_Init fail \r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0133"",""description"":""TSP-MPS-MCU-SwITC-0133\rTo check when the initialization of RAW_Init module failed, MCU2 will send the error print message to SDMS and enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0099]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0117]]\r\r [Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0099]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0117]""],""input"":""MPS_Conf.bin.\r\u0007"",""exec_step"":""Stub the TSP_RAW_Init, let it returns GM_False.\rStart the MCU2 INIT_TASK.\rCall the MCU2 TSP_Proc_Init_AppCfg_Proc module.\r\u0007"",""exp_step"":""1, MCU2 will send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_RAW_Init FAIL\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0104"",""description"":""TSP-MPS-MCU-SwITC-0104\rTo check when the initialization of Init_Monitor module failed, MCU2 will enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0035]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0034]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0035]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0034]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""Stub the GM_Thread_Create of Init_Monitor let it returns GM_FALSE.\rStart the MCU2 INIT_TASK.\rCall the MCU2 TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU2 shall be entering in endless loop.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0169"",""description"":""TSP-MPS-MCU-SwITC-0169\rTo check when the initialization of VSN module failed, MCU2 will send the error print message to SDMS and MCU2 will be fail.\r\r[Source: [TSP-MPS-MCU-SwAD-0035]] \r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0035]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let GM_VSN_Init return GM_FALSE.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU2 shall send the error print message to SDMS and enter in endless loop.\rError print message:\rGM_VSN_Init Fail!\rTSP_Proc_Init_Pre Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0170"",""description"":""TSP-MPS-MCU-SwITC-0170\rTo check when the initialization of Read config file module failed, MCU2 will enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0035]] \r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0035]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let GM_Get_File_Size return -1.\rStart the MCU INIT_TASK.\rCall the MCU2 TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU2 shall enter in endless loop.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0171"",""description"":""TSP-MPS-MCU-SwITC-0171\rTo check when the MTYPE_MGR and ADDR_MGR module failed, MCU2 will enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0035]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0035]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let the TSP_Addr_Init return false.\rStart the MCU2 INIT_TASK.\rCall the MCU2 TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU2 shall enter in endless loop.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0192"",""description"":""TSP-MPS-MCU-SwITC-0192\rTo check when the initialization of PCOM module failed,MCU2 will enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0035]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0035]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let TSP_PCOM_Interface_Init return GM_FALSE.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU2 shall send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_PCOM_Interface_Init Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0172"",""description"":""TSP-MPS-MCU-SwITC-0172\rTo check when the initialization of LCOM module failed,MCU2 will enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0035]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0035]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let TSP_LCOM_Interface_Init return GM_FALSE.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU2 shall send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_LCOM_Interface_Init Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0189"",""description"":""TSP-MPS-MCU-SwITC-0189\rTo check when the initialization ofSCOM module failed,MCU2 will enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0035]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0035]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let TSP_SCOM_Interface_Init return GM_FALSE.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU2 shall send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_SCOM_Interface_Init Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0173"",""description"":""TSP-MPS-MCU-SwITC-0173\rTo check when the initialization of CMM module failed, MCU2 will enter in endless loop.\r[Source: [TSP-MPS-MCU-SwAD-0035]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0035]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let TSP_CMM_Interface_Init return GM_FALSE.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU2 shall send the error print message to SDMS and enter in endless loop.\rError print message:\rTSP_CMM_Interface_Init Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0178"",""description"":""TSP-MPS-MCU-SwITC-0178\rTo check when the initialization of CCM module failed, MCU2 will enter in endless loop.\r[Source: [TSP-MPS-MCU-SwAD-0035]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0035]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let TSP_CCM_Interface_Init return GM_FALSE.\rStart the MCU INIT_TASK.\rCall the MCU TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU2 shall enter in endless loop.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0077"",""description"":""TSP-MPS-MCU-SwITC-0077\rTo check when the initialization of CHKW module failed, MCU2 will send the error print message to SDMS and enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0099]] \r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0099]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let the TSP_CHKW_Init returns GM_False.\rStart the MCU INIT_TASK.\rCall the MCU2 TSP_Proc_Init_AppCfg_Proc module.\r\u0007"",""exp_step"":""1, MCU2 will send the error print message to SDMS and enter endless loop.\rError print message:\rTSP_CHKW_Init Fail!\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0175"",""description"":""TSP-MPS-MCU-SwITC-0175\rTo check when the initialization of Init_Main module failed, MCU2 will send the error print message to SDMS and enter in endless loop, kill main task.\r[Source: [TSP-MPS-MCU-SwAD-0099]] \r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0099]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let the TSP_Proc_Main_Init returns GM_False.\rStart the MCU INIT_TASK.\rCall the MCU2 TSP_Proc_Init_AppCfg_Proc module.\r\u0007"",""exp_step"":""1, MCU2 will send the error print message to SDMS and enter endless loop.\rError print message:\rTSP_Proc_Main_Init Fail!\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0176"",""description"":""TSP-MPS-MCU-SwITC-0176\rTo check when the initialization of BIT_Interface module failed, MCU2 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0099]] \r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0099]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Stub the TSP_BIT_Interface_Init, let it returns GM_False.\rStart the MCU INIT_TASK.\rCall the MCU2 TSP_Proc_Init_AppCfg_Proc module.\r\u0007"",""exp_step"":""1, MCU2 will send the error print message to SDMS and enter endless loop.\rError print message:\rTSP_BIT_Interface_Init Fail!\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0177"",""description"":""TSP-MPS-MCU-SwITC-0177\rTo check when the initialization of GM condition module failed, MCU2 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0035]] \r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0035]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Stub the GM_Condition_Init, let it returns GM_False.\rStart the MCU INIT_TASK.\rCall the MCU2 TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU2 will send the error print message to SDMS and enter endless loop.\rError print message:\rInit g_globalinfo_struct.init_cond Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0257"",""description"":""TSP-MPS-MCU-SwITC-0257\rTo check when the initialization of TSP_Proc_Timer_Init\r module failed, MCU2 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0035]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0035]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""Stub the TSP_Proc_Timer_Init, let it returns GM_False.\rStart the MCU2 INIT_TASK.\rCall the MCU2 TSP_Proc_Init_Pre module.\r\u0007"",""exp_step"":""1, MCU2 will send the error print message to SDMS and enter in endless loop, kill main task.\rError Print Message:\rTSP_Proc_Timer_Init Fail!\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0126"",""description"":""TSP-MPS-MCU-SwITC-0126\rTo check when MCU2 not self init ok in 270s, MCU2 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0067]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0067]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""Start the MCU2 INIT_TASK.\rSleep 270s before MCU2 self init ok.\rCall the MCU2 TSP_Proc_Init_Monitor module.\r\u0007"",""exp_step"":""1, Error print message shall be sent to SDMS and enter in endless loop, kill main task.\rError print message:\rTSP_Proc_Init_Is_Timeout TIME OUT: (init time)\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0110"",""description"":""TSP-MPS-MCU-SwITC-0110\rTo check when MCU2 not receive SELT_INIT_OK message from MCU1 in 270s, MCU2 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0067]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0067]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""Start the MCU2 INIT_TASK.\rDo not start MCU1.\rCall the MCU2 TSP_Proc_Init_Monitor module.\r\u0007"",""exp_step"":""1, Error print message shall be sent to SDMS and enter in endless loop, kill main task.\rError print message:\r~~~ TIME OUT (init time)\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0111"",""description"":""TSP-MPS-MCU-SwITC-0111\rTo check when MCU2 not receive version exchange message from MCU1 in 270s, MCU2 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0067]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0046]]\r[Source: [TSP-MPS-MCU-SwAD-0050]]\r[Source: [TSP-MPS-MCU-SwAD-0023]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0067]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0046]"",""[TSP-MPS-MCU-SwAD-0050]"",""[TSP-MPS-MCU-SwAD-0023]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""Start MCU1 INIT_TASK and let it return after TSP_Proc_InnerSyS_Nego.\rStart TestBench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rCall the MCU2 TSP_Proc_Init_Monitor module.\r\u0007"",""exp_step"":""1, Error print message shall be sent to SDMS and enter in endless loop, kill main task.\rError print message:\r~~~ TIME OUT(init time)\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0127"",""description"":""TSP-MPS-MCU-SwITC-0127\rTo check when MCU2 not receive slot control message in 270s, MCU2 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0067]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0067]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""Start TestBench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU1 INIT_TASK.\rStart the MCU2 INIT_TASK.\rDo not send the slot control message from MCU1 to MCU2.\rCall the MCU2 TSP_Proc_Init_Monitor module.\r\u0007"",""exp_step"":""1, Error print message shall be sent to SDMS and enter in endless loop, kill main task.\rError print message:\r~~~ TIME OUT(init time)\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0251"",""description"":""TSP-MPS-MCU-SwITC-0251\rTo check when MCU2 not receive the vsn message in 270s, MCU2 will send the error print message to SDMS and enter in endless loop, kill main task.\r\r[Source: [TSP-MPS-MCU-SwAD-0067]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0067]"",""[TSP-MPS-MCU-SwAD-0090]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""Start TestBench which will run MPU1_Sim and MPU2_Sim, let the MPU1_Sim send NR message to MCU1 indicate the NR state is Standby.\rStart the MCU1 INIT_TASK.\rStart the MCU2 INIT_TASK.\rDo not send the vsn message from MCU1 to MCU2.\rCall the MCU2 TSP_Proc_Init_Monitor module.\r\u0007"",""exp_step"":""1, Error print message shall be sent to SDMS and enter in endless loop, kill main task.\rError print message:\r~~~TIME OUT (init time)\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0124"",""description"":""TSP-MPS-MCU-SwITC-0124\rTo check when MCU2 failed to send self init ok message to MCU1, MCU2 will send the error print message to SDMS.\r\r[Source: [TSP-MPS-MCU-SwAD-0084]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0023]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0084]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0023]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""1.  Let TSP_LCOM_Interface_Send return GM_FALSE.\rStart the MCU2 INIT_TASK.\rCall the MCU2 TSP_Proc_Init_Monitor module.\r\u0007"",""exp_step"":""1, Error print message shall be sent to SDMS.\rError print message:\r~~~Write MSG_SELF_INIT_OK_INFO\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0208"",""description"":""TSP-MPS-MCU-SwITC-0208\rTo check that RSSP-I state is always Normal on MCU2.\r\r[Source: [TSP-MPS-MCU-SwAD-0111]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0111]""],""input"":""TSP_Conf.ini.\r\u0007"",""exec_step"":""1. Set MCU to be system A.\rStart the MCU2 INIT_TASK.\rCall the MCU2 TSP_Proc_Init_AppCfg_Proc module.\rAfter RSSP-I module finishing initialization, print the RSSP-I state.\rSet MCU to be system B, then repeat step2-4.\r\u0007"",""exp_step"":""1, For step4 and step5, RSSP-I state is Normal.\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0014"",""description"":""TSP-MPS-MCU-SwITC-0014\rTo check the MCU1 CheckSafeTime could update the safe time correctly.\r\r[Source: [TSP-MPS-MCU-SwAD-0038]]\r[Source: [TSP-MPS-MCU-SwAD-0068]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""BA\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0038]"",""[TSP-MPS-MCU-SwAD-0068]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the TestBench.\rStart the MCU1 INIT_TASK.\rWhen st.immIndex = 100, modify the st.ccounter and st.lcounter to let (st.ccounter - t.lcounter = 999).\rCall CheckSafeTime module of MCU1 SLOT_MGR task.\rWhen st.immIndex = 101, modify the st.ccounter and st.lcounter to let (st.ccounter - t.lcounter = 1000).\rCall CheckSafeTime module of MCU1 SLOT_MGR task.\rWhen st.immIndex = 102, modify the st.ccounter and st.lcounter to let (st.ccounter - t.lcounter = 1001).\rCall CheckSafeTime module of MCU1 SLOT_MGR task.\rWhen st.immIndex = 103, modify the st.ccounter and st.lcounter to let (st.ccounter - t.lcounter = -64536).\rCall CheckSafeTime module of MCU1 SLOT_MGR task.\rWhen st.immIndex = 104, modify the st.ccounter and st.lcounter to let (st.ccounter - t.lcounter = -64537).\rCall CheckSafeTime module of MCU1 SLOT_MGR task.\rWhen st.immIndex = 105, modify the st.ccounter and st.lcounter to let (st.ccounter - t.lcounter = -64535).\rCall CheckSafeTime module of MCU1 SLOT_MGR task.\r\u0007"",""exp_step"":""1, Every time CheckSafeTime is called, the safe time check word MCU1_CHKW_SAFETY_TMR_ALGO1 shall be equal with\t 0x11F60B3B, MCU1_CHKW_SAFETY_TMR_ALGO2 shall be equal with 0x09F6033B.\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0015"",""description"":""TSP-MPS-MCU-SwITC-0015\rTo check the MCU1 CheckSafeTime will not update the safe time if the counter is not correct.\r\r[Source: [TSP-MPS-MCU-SwAD-0068]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0068]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the TestBench.\rStart the MCU1 INIT_TASK.\rWhen st.immIndex = 107, modify the st.ccounter and st.lcounter to let (st.ccounter - t.lcounter = 997).\rCall CheckSafeTime module of MCU1 SLOT_MGR task.\r\u0007"",""exp_step"":""1, the safe time check word MCU1_CHKW_SAFETY_TMR_ALGO1 shall be not equal with\t 0x11F60B3B, MCU1_CHKW_SAFETY_TMR_ALGO2 shall be not equal with 0x09F6033B.\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0065"",""description"":""TSP-MPS-MCU-SwITC-0065\rTo check the MCU1 CheckSafeTime will calculate the right MSTR_CHKW1 and MSTR_CHKW2 after CheckSafeTime if the application type is ZC/LC.\r.\r\r[Source: [TSP-MPS-MCU-SwAD-0068]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0068]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the TestBench.\rStart the MCU1 INIT_TASK.\rCall CheckSafeTime module of MCU1 SLOT_MGR task.\r\u0007"",""exp_step"":""1, st.MSTR_CHKW1 and st.MSTR_CHKW2 shall have right value after a new cycle.\rexpect 167 MSTR_CHKW1  0e14ac95 MSTR_CHKW2 bcd050cf\rexpect 168 MSTR_CHKW1  e24e3e53 MSTR_CHKW2 c8d7fed0\rexpect 169 MSTR_CHKW1  b4e6b73e MSTR_CHKW2 2c3fcf33\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0139"",""description"":""TSP-MPS-MCU-SwITC-0139\rTo check the MCU1 CheckSafeTime will calculate the right MSTR_CHKW1 and MSTR_CHKW2 after CheckSafeTime if the application type is CCS.\r\r[Source: [TSP-MPS-MCU-SwAD-0068]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0068]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the TestBench.\rStart the MCU1 INIT_TASK.\rCall CheckSafeTime module of MCU1 SLOT_MGR task.\r\u0007"",""exp_step"":""1, st.MSTR_CHKW1 and st.MSTR_CHKW2 shall have right value after a new cycle.\rexpect 249 MSTR_CHKW1  9aeaf5e9 MSTR_CHKW2 9fdaf085\rexpect 250 MSTR_CHKW1  3750e21d MSTR_CHKW2 d2cb6efa\rexpect 251 MSTR_CHKW1  6ba70b1f MSTR_CHKW2 ccbdbc7d\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0016"",""description"":""TSP-MPS-MCU-SwITC-0016\rTo check the MCU1 could reset the SafetyTime correctly.\r\r[Source: [TSP-MPS-MCU-SwAD-0038]]\r[Source: [TSP-MPS-MCU-SwAD-0039]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0038]"",""[TSP-MPS-MCU-SwAD-0039]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the MCU1 INIT_TASK.\rCall the reset SafetyTime module.\r\u0007"",""exp_step"":""1, cvc1 and cvc2 shall be reset to right value, st.isOK shall be false, st.immIndex shall be 0.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0018"",""description"":""TSP-MPS-MCU-SwITC-0018\rCheck when MCU1 is in TSP_PROC_NORMAL_RUNNING and in Normal state, Slot0 is divided into 14ms and 14ms, the TSP_Proc_Timer_Int module will send time_slot message to MCU2, MPU1 and MPU2, trigger BIT_Task and Main_Task if the application type is ZC/LC. \r\r[Source: [TSP-MPS-MCU-SwAD-0069]]\r[Source: [TSP-MPS-MCU-SwAD-0038]]\r[Source: [TSP-MPS-MCU-SwAD-0048]]\r[Source: [TSP-MPS-MCU-SwAD-0051]]\r[Source: [TSP-MPS-MCU-SwAD-0041]]\r[Source: [TSP-MPS-MCU-SwAD-0102]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0069]"",""[TSP-MPS-MCU-SwAD-0038]"",""[TSP-MPS-MCU-SwAD-0048]"",""[TSP-MPS-MCU-SwAD-0051]"",""[TSP-MPS-MCU-SwAD-0041]"",""[TSP-MPS-MCU-SwAD-0102]""],""input"":""TSP_Conf.ini\rNR information from MPU1 indicate the NR status is Normal.\r\u0007"",""exec_step"":""Start the TestBench which will return the Normal state to the MCU1.\rStart the MCU1 INIT_TASK.\rCall TSP_Proc_Timer_Int module.\r\u0007"",""exp_step"":""1, BIT_Task CPU test shall be triggered at slot 1.\r2, BIT_Task RAM test shall be triggered at slot 9.\r3, Main_Task shall be triggered every slot, slot0 to slot8 is 28ms, slot10 is 40ms, slot11 is 16ms.\r4, Slot0 is divided into 14ms and 14ms.\r5,Slot9 is divided into 14ms and 14ms\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0140"",""description"":""TSP-MPS-MCU-SwITC-0140\rCheck when MCU1 is in TSP_PROC_NORMAL_RUNNING and in Normal state, Slot0 is divided into 28ms and 28ms, the TSP_Proc_Timer_Int module will send time_slot message to MCU2, MPU1 and MPU2, trigger BIT_Task and Main_Task if the application type is CCS . \r\r[Source: [TSP-MPS-MCU-SwAD-0069]]\r[Source: [TSP-MPS-MCU-SwAD-0038]]\r[Source: [TSP-MPS-MCU-SwAD-0048]]\r[Source: [TSP-MPS-MCU-SwAD-0051]]\r[Source: [TSP-MPS-MCU-SwAD-0041]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0069]"",""[TSP-MPS-MCU-SwAD-0038]"",""[TSP-MPS-MCU-SwAD-0048]"",""[TSP-MPS-MCU-SwAD-0051]"",""[TSP-MPS-MCU-SwAD-0041]""],""input"":""TSP_Conf.ini\rNR information from MPU1 indicate the NR status is Normal.\r\u0007"",""exec_step"":""Start the TestBench which will return the Normal state to the MCU1.\rStart the MCU1 INIT_TASK.\rCall TSP_Proc_Timer_Int module.\r\u0007"",""exp_step"":""1, BIT_Task CPU test shall be triggered at slot 1.\r2, BIT_Task RAM test shall be triggered at slot 9.\r3, Main_Task shall be triggered every slot, \rslot0, slot1,slot5,slot8 is 56ms,\rslot 2 is 20ms\rslot3 is 36ms\rslot4 is 54ms\rslot6 is 30ms\rslot7 is 24ms\rslot 10 is 40ms \rslot11 is 16ms.\r4, Slot0 is divided into 28ms and 28ms.\r5, Slot9 is divided into 28ms and 28ms.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0066"",""description"":""TSP-MPS-MCU-SwITC-0066\rWhen MCU1 is in TSP_PROC_SYS_INIT_OK and Reserved state, the TSP_Proc_Timer_Int module will start TSP_PROC_NORMAL_RUNNING after receiving the Syn_Clock message if the application type is ZC/LC.\r\r[Source: [TSP-MPS-MCU-SwAD-0069]]\r[Source: [TSP-MPS-MCU-SwAD-0038]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0069]"",""[TSP-MPS-MCU-SwAD-0038]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO from MPU1 indicate the NR status is Reserve.\r\u0007"",""exec_step"":""Stub the function TSP_Proc_Syn_Clock_Latest_RcvStamp, let the latest_time less 238ms than cur_time, vsn0 =0, vsn1=424571664,vsn2=1600355036.\rStart the TestBench which will return the Reserved state to the MCU1.\rStart the MCU1 INIT_TASK.\rCall TSP_Proc_Timer_Int module.\r\u0007"",""exp_step"":""1, MCU1 shall start TSP_PROC_NORMAL_RUNNING after 336ms+2ms-238ms = 100ms.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0141"",""description"":""TSP-MPS-MCU-SwITC-0141\rWhen MCU1 is in TSP_PROC_SYS_INIT_OK and Reserved state, the TSP_Proc_Timer_Int module will start TSP_PROC_NORMAL_RUNNING after receiving the Syn_Clock message if the application type is CCS.\r\r[Source: [TSP-MPS-MCU-SwAD-0069]]\r[Source: [TSP-MPS-MCU-SwAD-0038]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0069]"",""[TSP-MPS-MCU-SwAD-0038]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO from MPU1 indicate the NR status is Reserve.\r\u0007"",""exec_step"":""Stub the function TSP_Proc_Syn_Clock_Latest_RcvStamp, let the latest_time less 302ms than cur_time, vsn0 =0, vsn1=424571664,vsn2=1600355036.\rStart the TestBench which will return the Reserved state to the MCU1.\rStart the MCU1 INIT_TASK.\rCall TSP_Proc_Timer_Int module.\r\u0007"",""exp_step"":""1, MCU1 shall start TSP_PROC_NORMAL_RUNNING after 500ms+2ms-302ms = 200ms.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0043"",""description"":""TSP-MPS-MCU-SwITC-0043\rCheck MCU1 SLOT_MGR task could run completely in 2ms in Normal state.\r\r[Source: [TSP-MPS-MCU-SwAD-0038]]\r[Source: [TSP-MPS-MCU-SwAD-0068]]\r[Source: [TSP-MPS-MCU-SwAD-0039]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0038]"",""[TSP-MPS-MCU-SwAD-0068]"",""[TSP-MPS-MCU-SwAD-0039]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the TestBench which will return the Normal state to the MCU1.\rStart the MCU1 INIT_TASK.\rLog the running time of SLOT_MGR task.\r\u0007"",""exp_step"":""1, The running time shall be less than 2ms.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0256"",""description"":""TSP-MPS-MCU-SwITC-0256\rCheck MCU1 SLOT_MGR task could run completely in 2ms in Reserve state.\r\r[Source: [TSP-MPS-MCU-SwAD-0038]]\r[Source: [TSP-MPS-MCU-SwAD-0068]]\r[Source: [TSP-MPS-MCU-SwAD-0039]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0038]"",""[TSP-MPS-MCU-SwAD-0068]"",""[TSP-MPS-MCU-SwAD-0039]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the TestBench which will return the Reserve state to the MCU1.\rStart the MCU1 INIT_TASK.\rLog the running time of SLOT_MGR task.\r\u0007"",""exp_step"":""1, The running time shall be less than 2ms.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0202"",""description"":""TSP-MPS-MCU-SwITC-0202\rCheck when MCU1 SLOT_MGR task go wrong, MCU1 will restart.\r\r[Source: [TSP-MPS-MCU-SwAD-0030]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0030]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let the MCU1 do not feed watchdog when vsn is 10.\rStart the TestBench.\rStart the MCU1.\r\u0007"",""exp_step"":""1, MCU1 shall restart when vsn is 10.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0019"",""description"":""TSP-MPS-MCU-SwITC-0019\rCheck the SLOT_MGR task could trigger the Main_Task and BIT_Task, feed watchdog, send self init ok message to MCU1, send the version information to MPU1 and MPU2 and execute the secondary initialization.\r\r[Source: [TSP-MPS-MCU-SwAD-0040]]\r[Source: [TSP-MPS-MCU-SwAD-0081]]\r[Source: [TSP-MPS-MCU-SwAD-0083]]\r[Source: [TSP-MPS-MCU-SwAD-0084]]\r[Source: [TSP-MPS-MCU-SwAD-0048]]\r[Source: [TSP-MPS-MCU-SwAD-0051]]\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r[Source: [TSP-MPS-MCU-SwAD-0050]]\r[Source: [TSP-MPS-MCU-SwAD-0098]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0040]"",""[TSP-MPS-MCU-SwAD-0081]"",""[TSP-MPS-MCU-SwAD-0083]"",""[TSP-MPS-MCU-SwAD-0084]"",""[TSP-MPS-MCU-SwAD-0048]"",""[TSP-MPS-MCU-SwAD-0051]"",""[TSP-MPS-MCU-SwAD-0045]"",""[TSP-MPS-MCU-SwAD-0050]"",""[TSP-MPS-MCU-SwAD-0098]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1.\rSend self init ok message from MCU1 to MCU2.\rSend version exchange message from MCU1 to MCU2.\rExecute the secondary initialization.\rSend slot message from MCU1 to MCU2.\r\u0007"",""exp_step"":""1, After step4, MCU2 shall receive the self init ok message from MCU1 and send the self init ok message to the MCU1.\r2, After step5, MCU2 shall receive the version exchange message from MCU1 and send version information to MPU1_Sim and MPU2_Sim, and send TSP_SYS_INIT_OK message to SDMS.\r3, After step6, MCU2 secondary initialization shall be executed.\r4, After step7, MCU2 MAIN_Task and BIT_Task shall be executed, watchdog shall be feed.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0325"",""description"":""TSP-MPS-MCU-SwITC-0325\rTo verify that when receive vsn message, the SLOT_MGR task module shall update VSN value.\r[Source: [TSP-MPS-MCU-SwAD-0040]]\r[Safety: Yes]\r\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0040]""],""input"":""VSN Message:\rl_slot_clock.msgType = TSP_COM_MSG_TYPE_VSN_INFO;\rVSN0=1, ,VSN1=0x8bceedbe,VSN2=0xcfafd011\r\u0007"",""exec_step"":""Initialization MCU2 software\rActive MCU1 to send VSN Message to MCU2\rMCU2 read VSN message and update own VSN value.\rCheck the result\r\u0007"",""exp_step"":""Initialization MCU software successfully\rMCU2 Receive VSN Message successfully\rThe VSN is accordance with the sending one\rMCU2 read VSN message and update own VSN value successfully\rExpect VSN message:\rThe VSN message is accordance with the sending one\rMCU2: VSN0=1, ,VSN1=0x8bceedbe,VSN2=0xcfafd011\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0203"",""description"":""TSP-MPS-MCU-SwITC-0203\rCheck when MCU2 SLOT_MGR task go wrong, MCU2 will restart.\r\r[Source: [TSP-MPS-MCU-SwAD-0034]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0034]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Let the MCU2 do not feed watchdog when vsn is 10.\rStart the TestBench.\rStart the MCU2 and MCU1.\r\u0007"",""exp_step"":""1, MCU2 shall restart when vsn is 10.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0020"",""description"":""TSP-MPS-MCU-SwITC-0020\rTo check MCU1 TSP_Proc_Main_Step_11 will calculate the main task run time check word correctly when MCU1 is normal if the application type is ZC/LC.\r\r[Source: [TSP-MPS-MCU-SwAD-0041]]\r[Source: [TSP-MPS-MCU-SwAD-0059]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP/EG/BA\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0041]"",""[TSP-MPS-MCU-SwAD-0059]"",""[TSP-MPS-MCU-SwAD-0022]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO from MPU1 indicate the NR status is Normal.\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK.\rWhen VSN = 10, set the main task running time to 335ms.\rCall MCU1 TSP_Proc_Main_Step_11.\rWhen VSN = 11, set the main task running time to 337ms.\rCall MCU1 TSP_Proc_Main_Step_11.\rWhen VSN = 12, set the main task running time to 336ms (here the last time is bigger than current time).\rCall MCU1 TSP_Proc_Main_Step_11.\rWhen VSN = 13, set the main task running time to 338ms.\rCall MCU1 TSP_Proc_Main_Step_11.\r\r\u0007"",""exp_step"":""1, After step 4, 6, 8, the timeGap shall be 0x52000, the run time check word shall be 0x21F61B3B.\r2, After step 10, the timeGap shall be 0x52800, the run time check word shall not be 0x21F61B3B.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0021"",""description"":""TSP-MPS-MCU-SwITC-0021\rTo check MCU1 TSP_Proc_Main_Step_11 will calculate the main task run time check word correctly., while MCU1 is reserved if the application type is ZC/LC.\r\r\r[Source: [TSP-MPS-MCU-SwAD-0041]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r[Source: [TSP-MPS-MCU-SwAD-0059]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP/EG/BA\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0041]"",""[TSP-MPS-MCU-SwAD-0022]"",""[TSP-MPS-MCU-SwAD-0059]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO from MPU1 indicate the NR status is Reserve.\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Reserve state.\rStart the MCU1 INIT_TASK.\rWhen VSN = 10, set the main task running time to 333ms.\rCall MCU1 TSP_Proc_Main_Step_11.\rWhen VSN = 11, set the main task running time to 339ms.\rCall MCU1 TSP_Proc_Main_Step_11.\rWhen VSN = 12, set the main task running time to 336ms (here the last time is bigger than current time).\rCall MCU1 TSP_Proc_Main_Step_11.\rWhen VSN = 13, set the main task running time to 340ms.\rCall MCU1 TSP_Proc_Main_Step_11.\r\u0007"",""exp_step"":""1, After step 4, the timeGap shall be 0x51800, the run time check word shall be 0x21F61B3B.\r2, After step 6, the timeGap shall be 0x52800, the run time check word shall be 0x21F61B3B.\r3, After step 8, the timeGap shall be 0x52000, the run time check word shall be 0x21F61B3B.\r4, After step 10, the timeGap shall be 0x53000, the run time check word shall not be 0x21F61B3B.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0142"",""description"":""TSP-MPS-MCU-SwITC-0142\rTo check MCU1 TSP_Proc_Main_Step_11 will calculate the main task run time check word correctly when MCU1 is normal if the application type is CCS.\r\r[Source: [TSP-MPS-MCU-SwAD-0041]]\r[Source: [TSP-MPS-MCU-SwAD-0059]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP/EG/BA\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0041]"",""[TSP-MPS-MCU-SwAD-0059]"",""[TSP-MPS-MCU-SwAD-0022]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO from MPU1 indicate the NR status is Normal.\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK.\rWhen VSN = 10, set the main task running time to 499ms.\rCall MCU1 TSP_Proc_Main_Step_11.\rWhen VSN = 11, set the main task running time to 501ms.\rCall MCU1 TSP_Proc_Main_Step_11.\rWhen VSN = 12, set the main task running time to 500ms (here the last time is bigger than current time).\rCall MCU1 TSP_Proc_Main_Step_11.\rWhen VSN = 13, set the main task running time to 502ms.\rCall MCU1 TSP_Proc_Main_Step_11.\r\r\u0007"",""exp_step"":""1, After step 4, 6, 8, the timeGap shall be 0x52000, the run time check word shall be 0x21F61B3B.\r2, After step 10, the timeGap shall be 0x52800, the run time check word shall not be 0x21F61B3B.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0143"",""description"":""TSP-MPS-MCU-SwITC-0143\rTo check MCU1 TSP_Proc_Main_Step_11 will calculate the main task run time check word correctly., while MCU1 is reserved if the application type is CCS\r\r\r[Source: [TSP-MPS-MCU-SwAD-0041]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r[Source: [TSP-MPS-MCU-SwAD-0059]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP/EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0041]"",""[TSP-MPS-MCU-SwAD-0022]"",""[TSP-MPS-MCU-SwAD-0059]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO from MPU1 indicate the NR status is Reserve.\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Reserve state.\rStart the MCU1 INIT_TASK.\rWhen VSN = 10, set the main task running time to 497ms.\rCall MCU1 TSP_Proc_Main_Step_11.\rWhen VSN = 11, set the main task running time to 503ms.\rCall MCU1 TSP_Proc_Main_Step_11.\rWhen VSN = 12, set the main task running time to 500ms (here the last time is bigger than current time).\rCall MCU1 TSP_Proc_Main_Step_11.\rWhen VSN = 13, set the main task running time to 504ms.\rCall MCU1 TSP_Proc_Main_Step_11.\r\u0007"",""exp_step"":""1, After step 4, the timeGap shall be 0x51800, the run time check word shall be 0x21F61B3B.\r2, After step 6, the timeGap shall be 0x52800, the run time check word shall be 0x21F61B3B.\r3, After step 8, the timeGap shall be 0x52000, the run time check word shall be 0x21F61B3B.\r4, After step 10, the timeGap shall be 0x53000, the run time check word shall not be 0x21F61B3B.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0044"",""description"":""TSP-MPS-MCU-SwITC-0044\rCheck the MCU1 could update task sequence check word correctly. When the task sequence is not correct, the task sequence check word shall be wrong if the application type is ZC/LC.\r\r[Source: [TSP-MPS-MCU-SwAD-0052]]\r[Source: [TSP-MPS-MCU-SwAD-0053]]\r[Source: [TSP-MPS-MCU-SwAD-0054]]\r[Source: [TSP-MPS-MCU-SwAD-0055]]\r[Source: [TSP-MPS-MCU-SwAD-0056]]\r[Source: [TSP-MPS-MCU-SwAD-0057]]\r[Source: [TSP-MPS-MCU-SwAD-0058]]\r[Source: [TSP-MPS-MCU-SwAD-0059]]\r[Source: [TSP-MPS-MCU-SwAD-0047]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP+EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0052]"",""[TSP-MPS-MCU-SwAD-0053]"",""[TSP-MPS-MCU-SwAD-0054]"",""[TSP-MPS-MCU-SwAD-0055]"",""[TSP-MPS-MCU-SwAD-0056]"",""[TSP-MPS-MCU-SwAD-0057]"",""[TSP-MPS-MCU-SwAD-0058]"",""[TSP-MPS-MCU-SwAD-0059]"",""[TSP-MPS-MCU-SwAD-0047]"",""[TSP-MPS-MCU-SwAD-0022]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO from MPU1 indicate the NR status is Normal.\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK.\rWhen vsn = 3, Call MCU1 TSP_Proc_Main_Step_0 to TSP_Proc_Main_Step_11 in order.\rWhen vsn = 4, Call MCU1 TSP_Proc_Main_Step_0 to TSP_Proc_Main_Step_11 in order except step 11.\r\u0007"",""exp_step"":""1, After step 3, the MCU1 task sequence check word shall be 6FBA3301H.\r2, After step 4, the MCU1 task sequence check word shall not be 6FBA3301H.\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0084"",""description"":""TSP-MPS-MCU-SwITC-0084\rCheck the MCU1 task sequence check word will be not correct if the task sequence is not correct if the application type is ZC/LC.\r\r[Source: [TSP-MPS-MCU-SwAD-0052]]\r[Source: [TSP-MPS-MCU-SwAD-0053]]\r[Source: [TSP-MPS-MCU-SwAD-0054]]\r[Source: [TSP-MPS-MCU-SwAD-0055]]\r[Source: [TSP-MPS-MCU-SwAD-0056]]\r[Source: [TSP-MPS-MCU-SwAD-0057]]\r[Source: [TSP-MPS-MCU-SwAD-0058]]\r[Source: [TSP-MPS-MCU-SwAD-0059]]\r[Source: [TSP-MPS-MCU-SwAD-0041]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0052]"",""[TSP-MPS-MCU-SwAD-0053]"",""[TSP-MPS-MCU-SwAD-0054]"",""[TSP-MPS-MCU-SwAD-0055]"",""[TSP-MPS-MCU-SwAD-0056]"",""[TSP-MPS-MCU-SwAD-0057]"",""[TSP-MPS-MCU-SwAD-0058]"",""[TSP-MPS-MCU-SwAD-0059]"",""[TSP-MPS-MCU-SwAD-0041]"",""[TSP-MPS-MCU-SwAD-0022]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO from MPU1 indicate the NR status is Reserve.\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK.\rWhen vsn = 4, Call MCU1 TSP_Proc_Main_Step_0 to TSP_Proc_Main_Step_11, but change the order of step 4and step 5.\r\u0007"",""exp_step"":""1, After step 4, the MCU1 task sequence check word shall not be 6FBA3301H.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0144"",""description"":""TSP-MPS-MCU-SwITC-0144\rCheck the MCU1 could update task sequence check word correctly. When the task sequence is not correct, the task sequence check word shall be wrong if the application type is CCS.\r\r[Source: [TSP-MPS-MCU-SwAD-0052]]\r[Source: [TSP-MPS-MCU-SwAD-0053]]\r[Source: [TSP-MPS-MCU-SwAD-0054]]\r[Source: [TSP-MPS-MCU-SwAD-0055]]\r[Source: [TSP-MPS-MCU-SwAD-0056]]\r[Source: [TSP-MPS-MCU-SwAD-0057]]\r[Source: [TSP-MPS-MCU-SwAD-0058]]\r[Source: [TSP-MPS-MCU-SwAD-0059]]\r[Source: [TSP-MPS-MCU-SwAD-0047]]\r[Source: [TSP-MPS-MCU-SwAD-0022]] \r[Source: [TSP-MPS-MCU-SwAD-0108]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP+EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0052]"",""[TSP-MPS-MCU-SwAD-0053]"",""[TSP-MPS-MCU-SwAD-0054]"",""[TSP-MPS-MCU-SwAD-0055]"",""[TSP-MPS-MCU-SwAD-0056]"",""[TSP-MPS-MCU-SwAD-0057]"",""[TSP-MPS-MCU-SwAD-0058]"",""[TSP-MPS-MCU-SwAD-0059]"",""[TSP-MPS-MCU-SwAD-0047]"",""[TSP-MPS-MCU-SwAD-0022]"",""[TSP-MPS-MCU-SwAD-0108]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO from MPU1 indicate the NR status is Normal.\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK.\rWhen vsn = 3, Call MCU1 TSP_Proc_Main_Step_0 to TSP_Proc_Main_Step_11 in order.\rWhen vsn = 4, Call MCU1 TSP_Proc_Main_Step_0 to TSP_Proc_Main_Step_11 in order except step 11.\r\u0007"",""exp_step"":""1, After step 3, the MCU1 task sequence check word shall be 6FBA3301H.\r2, After step 4, the MCU1 task sequence check word shall not be 6FBA3301H.\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0145"",""description"":""TSP-MPS-MCU-SwITC-0145\rCheck the MCU1 task sequence check word will be not correct if the task sequence is not correct if the application type is CCS.\r\r[Source: [TSP-MPS-MCU-SwAD-0052]]\r[Source: [TSP-MPS-MCU-SwAD-0053]]\r[Source: [TSP-MPS-MCU-SwAD-0054]]\r[Source: [TSP-MPS-MCU-SwAD-0055]]\r[Source: [TSP-MPS-MCU-SwAD-0056]]\r[Source: [TSP-MPS-MCU-SwAD-0057]]\r[Source: [TSP-MPS-MCU-SwAD-0058]]\r[Source: [TSP-MPS-MCU-SwAD-0059]]\r[Source: [TSP-MPS-MCU-SwAD-0041]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r[Source: [TSP-MPS-MCU-SwAD-0109]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0052]"",""[TSP-MPS-MCU-SwAD-0053]"",""[TSP-MPS-MCU-SwAD-0054]"",""[TSP-MPS-MCU-SwAD-0055]"",""[TSP-MPS-MCU-SwAD-0056]"",""[TSP-MPS-MCU-SwAD-0057]"",""[TSP-MPS-MCU-SwAD-0058]"",""[TSP-MPS-MCU-SwAD-0059]"",""[TSP-MPS-MCU-SwAD-0041]"",""[TSP-MPS-MCU-SwAD-0022]"",""[TSP-MPS-MCU-SwAD-0109]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO from MPU1 indicate the NR status is Reserve.\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK.\rWhen vsn = 4, Call MCU1 TSP_Proc_Main_Step_0 to TSP_Proc_Main_Step_11, but change the order of step 4and step 5.\r\u0007"",""exp_step"":""1, After step 4, the MCU1 task sequence check word shall not be 6FBA3301H.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0024"",""description"":""TSP-MPS-MCU-SwITC-0024\rCheck the MCU1 could send the main check-words to the VPS correctly if the application type is ZC/LC.\r\r[Source: [TSP-MPS-MCU-SwAD-0041]]\r[Source: [TSP-MPS-MCU-SwAD-0052]]\r[Source: [TSP-MPS-MCU-SwAD-0021]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0041]"",""[TSP-MPS-MCU-SwAD-0052]"",""[TSP-MPS-MCU-SwAD-0021]"",""[TSP-MPS-MCU-SwAD-0022]""],""input"":""TSP_Conf.ini\rTSP_COM_MSG_NR_INFO from MPU1 indicate the NR status is Normal.\rTSP_COM_MSG_CHKW_INFO msg from MPU1, the check word of MPU1 are as follows:\rMPU1_CHKW_BIT 0x55D5B691\rMPU1_CHKW_MAIN_CYCLE_INTERVAL 0x52000\rMPU1_CHKW_TASK_CHECK 0x660D57BB\rMPU1_CHKW_NR_STATE 0x637A0ABC\rMPU1_CHKW_SACEM_RX_CH1 0x6CDE6DF3\rMPU1_CHKW_SACEM_RX_CH2 0x5359548DTSP_COM_MSG_CHKW_INFO msg from MPU2, the check word of MPU2 are as follows:\rMPU2_CHKW_BIT 0x5C25BD44\rMPU2_CHKW_MAIN_CYCLE_INTERVAL 0x52000\rMPU2_CHKW_TASK_CHECK 0x535BC35F\rMPU2_CHKW_NR_STATE 0x637A0ABC\rMPU2_CHKW_SACEM_RX_CH1 0x6CDE6DF3\rMPU2_CHKW_SACEM_RX_CH2 0x5359548DTSP_COM_MSG_CHKW_INFO msg from MCU2, the check word of MCU2 are as follows:\rMCU2_CHKW_BIT 0x4FA78CDB\rMCU2_CHKW_MAIN_CYCLE_INTERVAL 0x52000\rMCU2_CHKW_TASK_CHECK 0x57BED039\rMCU2_CHKW_JTC_CH1 0xAF206E62\rMCU2_CHKW_JTC_CH2 0x1EF2BFE0\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK.\rSend MSG_CHKW_INFO message from MCU2_Sim, MPU1_Sim and MPU2_Sim to MCU1.\rCall slot 0 to send the main check word to the VPS.\r\u0007"",""exp_step"":""1, Before shifting, the MCHKW shall have the value as follows: (VSN\u003e=4)\r0x79F6733B\r0x71F66B3B\r0x69F6633B\r0x61F65B3B\r0x59F6533B\r0x51F64B3B\r0x49F6433B\r0x41F63B3B\r0x39F6333B\r0x31F62B3B\r0x29F6233B\r0x21F61B3B\r0x19F6133B\r0x11F60B3B\r0x09F6033B\r0x01F6FB3B\r0xF9F6F33B\r0xF1F6EB3B\r0xE9F6E33B\r0xE214E862\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0146"",""description"":""TSP-MPS-MCU-SwITC-0146\rCheck the MCU1 could send the main check-words to the VPS correctly if the application type is CCS.\r\r[Source: [TSP-MPS-MCU-SwAD-0041]]\r[Source: [TSP-MPS-MCU-SwAD-0052]]\r[Source: [TSP-MPS-MCU-SwAD-0021]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0041]"",""[TSP-MPS-MCU-SwAD-0052]"",""[TSP-MPS-MCU-SwAD-0021]"",""[TSP-MPS-MCU-SwAD-0022]""],""input"":""TSP_Conf.ini\rTSP_COM_MSG_NR_INFO from MPU1 indicate the NR status is Normal.\rTSP_COM_MSG_CHKW_INFO msg from MPU1, the check word of MPU1 are as follows:\rMPU1_CHKW_BIT 0x55D5B691\rMPU1_CHKW_MAIN_CYCLE_INTERVAL 0x52000\rMPU1_CHKW_TASK_CHECK 0x660D57BB\rMPU1_CHKW_NR_STATE 0x637A0ABC\rMPU1_CHKW_SACEM_RX_CH1 0x6CDE6DF3\rMPU1_CHKW_SACEM_RX_CH2 0x5359548D\rTSP_COM_MSG_CHKW_INFO msg from MPU2, the check word of MPU2 are as follows:\rMPU2_CHKW_BIT 0x5C25BD44\rMPU2_CHKW_MAIN_CYCLE_INTERVAL 0x52000\rMPU2_CHKW_TASK_CHECK 0x535BC35F\rMPU2_CHKW_NR_STATE 0x637A0ABC\rMPU2_CHKW_SACEM_RX_CH1 0x6CDE6DF3\rMPU2_CHKW_SACEM_RX_CH2 0x5359548D\rTSP_COM_MSG_CHKW_INFO msg from MCU2, the check word of MCU2 are as follows:\rMCU2_CHKW_BIT 0x4FA78CDB\rMCU2_CHKW_MAIN_CYCLE_INTERVAL 0x52000\rMCU2_CHKW_TASK_CHECK 0x57BED039\rMCU2_CHKW_JTC_CH1 0xAF206E62\rMCU2_CHKW_JTC_CH2 0x1EF2BFE0\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK.\rSend TSP_COM_MSG_CHKW_INFO message from MCU2_Sim, MPU1_Sim and MPU2_Sim to MCU1.\rCall slot 0 to send the main check word to the VPS (odd cycle)(VSN\u003e=4).\rCall slot 8 to send the main check word to the VPS (odd cycle).\rCall slot 4 to send the main check word to the VPS (even cycle).\r\u0007"",""exp_step"":""1, Before shifting, the MCHKW shall have the value as follows:\r0x79F6733B\r0x71F66B3B\r0x69F6633B\r0x61F65B3B\r0x59F6533B\r0x51F64B3B\r0x49F6433B\r0x41F63B3B\r0x39F6333B\r0x31F62B3B\r0x29F6233B\r0x21F61B3B\r0x19F6133B\r0x11F60B3B\r0x09F6033B\r0x01F6FB3B\r0xF9F6F33B\r0xF1F6EB3B\r0xE9F6E33B\r0xE214E862\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0047"",""description"":""TSP-MPS-MCU-SwITC-0047\rCheck the MCU1 could generate the safe time check word correctly if the application type is ZC/LC.\r\r[Source: [TSP-MPS-MCU-SwAD-0052]]\r[Source: [TSP-MPS-MCU-SwAD-0047]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP \r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0052]"",""[TSP-MPS-MCU-SwAD-0047]"",""[TSP-MPS-MCU-SwAD-0022]""],""input"":""TSP_Conf.ini\rTSP_COM_MSG_NR_INFO from MPU1 indicate the NR status is Normal.\rTSP_COM_MSG_NR_INFO from MPU1 indicate the NR status is Reserve.\r\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK.\rCall MCU1 slot 0 to calculate the MCU1_CHKW_SAFETY_TMR_ALGO1 and MCU1_CHKW_SAFETY_TMR_ALGO2.\rRestart the TestBench and return MCU1 in Reserve state.\rRestart the MCU1 INIT_TASK.\rWhen VSN0 = 4, let MCU1 delay 2ms.\rCall MCU1 slot 0 to calculate the MCU1_CHKW_SAFETY_TMR_ALGO1 and MCU1_CHKW_SAFETY_TMR_ALGO2.\rWhen VSN0 = 5, let MCU1 reduce 2ms.\rCall MCU1 slot 0 to calculate the MCU1_CHKW_SAFETY_TMR_ALGO1 and MCU1_CHKW_SAFETY_TMR_ALGO2.\r\u0007"",""exp_step"":""1, After step3, the safe time check word MCU1_CHKW_SAFETY_TMR_ALGO1 shall be equal with\t 0x11F60B3B, MCU1_CHKW_SAFETY_TMR_ALGO2 shall be equal with 0x09F6033B.\r2, After step7 and 9, the safe time check word MCU1_CHKW_SAFETY_TMR_ALGO1 shall be equal with\t 0x11F60B3B, MCU1_CHKW_SAFETY_TMR_ALGO2 shall be equal with 0x09F6033B.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0147"",""description"":""TSP-MPS-MCU-SwITC-0147\rCheck the MCU1 could generate the safe time check word correctly if the application type is CCS.\r\r[Source: [TSP-MPS-MCU-SwAD-0052]]\r[Source: [TSP-MPS-MCU-SwAD-0047]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP \r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0052]"",""[TSP-MPS-MCU-SwAD-0047]"",""[TSP-MPS-MCU-SwAD-0022]""],""input"":""TSP_Conf.ini\rTSP_COM_MSG_NR_INFO from MPU1 indicate the NR status is Normal.\rTSP_COM_MSG_NR_INFO from MPU1 indicate the NR status is Reserve.\r\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK.\rCall MCU1 slot 0 to calculate the MCU1_CHKW_SAFETY_TMR_ALGO1 and MCU1_CHKW_SAFETY_TMR_ALGO2.\rRestart the TestBench and return MCU1 in Reserve state.\rRestart the MCU1 INIT_TASK.\rWhen VSN0 = 4, let MCU1 delay 2ms.\rCall MCU1 slot 0 to calculate the MCU1_CHKW_SAFETY_TMR_ALGO1 and MCU1_CHKW_SAFETY_TMR_ALGO2.\rWhen VSN0 = 5, let MCU1 reduce 2ms.\rCall MCU1 slot 0 to calculate the MCU1_CHKW_SAFETY_TMR_ALGO1 and MCU1_CHKW_SAFETY_TMR_ALGO2.\r\u0007"",""exp_step"":""1, After step3, the safe time check word MCU1_CHKW_SAFETY_TMR_ALGO1 shall be equal with\t 0x11F60B3B, MCU1_CHKW_SAFETY_TMR_ALGO2 shall be equal with 0x09F6033B.\r2, After step7 and 9, the safe time check word MCU1_CHKW_SAFETY_TMR_ALGO1 shall be equal with\t 0x11F60B3B, MCU1_CHKW_SAFETY_TMR_ALGO2 shall be equal with 0x09F6033B.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0068"",""description"":""TSP-MPS-MCU-SwITC-0068\rCheck when the MCU1 run for 338ms in Normal state, the safety time check word generated will not be correct.\r\r[Source: [TSP-MPS-MCU-SwAD-0052]]\r[Source: [TSP-MPS-MCU-SwAD-0047]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r[Source: [TSP-MPS-MCU-SwAD-0038]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0052]"",""[TSP-MPS-MCU-SwAD-0047]"",""[TSP-MPS-MCU-SwAD-0022]"",""[TSP-MPS-MCU-SwAD-0038]""],""input"":""TSP_Conf.ini\rTSP_COM_MSG_NR_INFO from MPU1 indicate the NR status is Normal.\r\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK.\rWhen VSN0 = 10, let MCU1 delay 2ms.\rCall MCU1 slot 0 to calculate the MCU1_CHKW_SAFETY_TMR_ALGO1 and MCU1_CHKW_SAFETY_TMR_ALGO2.\r\u0007"",""exp_step"":""1, After step 4, The safe time check word MCU1_CHKW_SAFETY_TMR_ALGO1 shall not be equal with\t 0x11F60B3B, MCU1_CHKW_SAFETY_TMR_ALGO2 shall not be equal with 0x09F6033B.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0200"",""description"":""TSP-MPS-MCU-SwITC-0200\rCheck MCU1 will collect the running state of MCU1, including main cycle NO, NR state, clock sync state, and send to SDMS.\rCheck the MCU1 could update the VSN.\r\r[Source: [TSP-MPS-MCU-SwAD-0024]]\r[Source: [TSP-MPS-MCU-SwAD-0052]]\r[Source: [TSP-MPS-MCU-SwAD-0101]]\r[Source: [TSP-MPS-MCU-SwAD-0102]]\r[Source: [TSP-MPS-MCU-SwAD-0103]]\r[Source: [TSP-MPS-MCU-SwAD-0105]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0024]"",""[TSP-MPS-MCU-SwAD-0052]"",""[TSP-MPS-MCU-SwAD-0101]"",""[TSP-MPS-MCU-SwAD-0102]"",""[TSP-MPS-MCU-SwAD-0103]"",""[TSP-MPS-MCU-SwAD-0105]""],""input"":""TSP_Conf.ini\rTSP_COM_MSG_NR_INFO from MPU1 indicate the NR status is Normal.\r\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK.\r\u0007"",""exp_step"":""1, The message send to SDMS shall have correct value.\r2, VSN shall be updated correctly.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0026"",""description"":""TSP-MPS-MCU-SwITC-0026\rCheck the MCU1 could receive the heartbeat message from MPU1, MPU2 and MCU2.\rCheck the MCU1 could receive the NR information message from MPU1 and update device state correctly.\r\r[Source: [TSP-MPS-MCU-SwAD-0054]]\r[Source: [TSP-MPS-MCU-SwAD-0023]]\r[Source: [TSP-MPS-MCU-SwAD-0041]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0054]"",""[TSP-MPS-MCU-SwAD-0023]"",""[TSP-MPS-MCU-SwAD-0041]""],""input"":""TSP_Conf.ini\rNR information from MPU1 indicate the NR status is Normal.\rheartbeat message from MPU1, MPU2 and MCU2\r\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK.\rSend MSG_HB_INFO message from MCU2_Sim, MPU1_Sim and MPU2_Sim to MCU1.\rSend MSG_NR_INFO message from MPU1_Sim to MCU1.\rCall MCU1 TSP_Proc_Main_Step_2.\r\u0007"",""exp_step"":""1, MSG_HB_INFO message shall be received from MPU1/2, MCU2.\r2, MSG_NR_INFO message shall be received and the NR status shall be updated.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0022"",""description"":""TSP-MPS-MCU-SwITC-0022\rCheck the MCU1 could handle the utc time information message from MCU2.\r\r[Source: [TSP-MPS-MCU-SwAD-0055]]\r[Source: [TSP-MPS-MCU-SwAD-0041]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0055]"",""[TSP-MPS-MCU-SwAD-0041]""],""input"":""TSP_Conf.ini\rutc time information message from MCU2\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK.\rSend MSG_UTC_TIMEINFO message from MCU2_Sim to MCU1.\rCall MCU1 TSP_Proc_Main_Step_4.\r\u0007"",""exp_step"":""1, MSG_UTC_TIMEINFO message shall be received in slot4.\r2,\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0087"",""description"":""TSP-MPS-MCU-SwITC-0087\rCheck the MCU1 will not set to be clock a-sync if have not receive the TSP_PCOM_SYN_CLOCK_Struct message for 2 cycles.\r\r[Source: [TSP-MPS-MCU-SwAD-0053]]\r[Source: [TSP-MPS-MCU-SwAD-0025]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0053]"",""[TSP-MPS-MCU-SwAD-0025]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO from MPU1 indicate the NR status is Normal.\r\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK.\rCall MCU1 TSP_Proc_Main_Step_1.\r\u0007"",""exp_step"":""1, MCU1 shall not be clock sync.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0050"",""description"":""TSP-MPS-MCU-SwITC-0050\rCheck the MCU1 could do time synchronization, calculate adjust parameter of clock and inform the encode synchronization result to MPU1/2.\rCheck MCU1 will refresh its clock syn sending time every cycle.\r\r[Source: [TSP-MPS-MCU-SwAD-0053]]\r[Source: [TSP-MPS-MCU-SwAD-0029]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0053]"",""[TSP-MPS-MCU-SwAD-0029]""],""input"":""TSP_Conf.ini\r\r\u0007"",""exec_step"":""Start the TestBench which will run MPU1_Sim ,MPU2_Sim,MCU2_Sim and SDMS_Sim.\rStart the MCU1 INIT_TASK.\rCall MCU1 TSP_Proc_Main_Step_1 function.\rFrom VSN=110 to VSN=115, set TSP_Proc_Syn_Clock_RcvTime = 22000 + TSP_CLOCK_SYNC_TIME_DELAY;\tTSP_Proc_Syn_Clock_SendTime = 20000 to simulate faster 2ms;\rFrom VSN=120 to VSN=125, set TSP_Proc_Syn_Clock_RcvTime = 12500 + TSP_CLOCK_SYNC_TIME_DELAY;\tTSP_Proc_Syn_Clock_SendTime = 10000 to simulate faster 2.5ms;\rFrom VSN=130 to VSN=135, set TSP_Proc_Syn_Clock_RcvTime = 4294965296 + TSP_CLOCK_SYNC_TIME_DELAY;\tTSP_Proc_Syn_Clock_SendTime = 0 to simulate slower 2ms;\rFrom VSN=140 to VSN=145, set TSP_Proc_Syn_Clock_RcvTime = 0 + TSP_CLOCK_SYNC_TIME_DELAY;\tTSP_Proc_Syn_Clock_SendTime = 4294964796 to simulate faster 2.5ms;\rFrom VSN=150 to VSN=155, set TSP_Proc_Syn_Clock_RcvTime = 0 + TSP_CLOCK_SYNC_TIME_DELAY;\tTSP_Proc_Syn_Clock_SendTime = 0 to simulate the equal value;\rFrom VSN=160 to VSN=165, set TSP_Proc_Syn_Clock_RcvTime = 10000 + TSP_CLOCK_SYNC_TIME_DELAY;\tTSP_Proc_Syn_Clock_SendTime = 23000 to simulate slower 13ms;\rFrom VSN=180 to VSN=185, set TSP_Proc_Syn_Clock_RcvTime = 2000 + TSP_CLOCK_SYNC_TIME_DELAY; TSP_Proc_Syn_Clock_SendTime = 22000 to simulate slower 20ms;\rReboot MCU1 Start the TestBench which will run MPU1_Sim ,MPU2_Sim,MCU2_Sim and SDMS_Sim.\rStub to make the difference between TSP_Proc_Syn_Clock_RcvTime and TSP_Proc_Syn_Clock_SendTime constant, and both of them are added one by one every cycle from 0 to 0xFFFFFFFF+1. (The difference is 2999, 2000, 1, 0, -1, -2000, -2999)\r\u0007"",""exp_step"":""1, MCU1 shall refresh its clock syn sending time every cycle.\r2, After step 4, the result:\rMSG_CLOCK_SYNC_STAT LCOM write shall return GM_TRUE;\rEncode ClockSYN_1 shall be 0x158D4F8D,ClockSYN_2 shall be 0x4C83B716, and shall be send to MPU.\rAdjust shall be TSP_PROC_SYN_CLOCK_ADJUST_NO;\rClockSyned shall be GM_TRUE.\r3, After step 5, the result:\rMSG_CLOCK_SYNC_STAT LCOM write shall return GM_TRUE;\rEncode ClockSYN_1 shall be 0x158D4F8D,ClockSYN_2 shall be 0x4C83B716, and shall be send to MPU.\rAdjust shall be TSP_PROC_SYN_CLOCK_ADJUST_DELAY;\rClockSyned shall be GM_TRUE.\r4, After step 6, the result:\rMSG_CLOCK_SYNC_STAT LCOM write shall return GM_TRUE;\rEncode ClockSYN_1 shall be 0x158D4F8D,ClockSYN_2 shall be 0x4C83B716, and shall be send to MPU.\rAdjust shall be TSP_PROC_SYN_CLOCK_ADJUST_NO;\rClockSyned shall be GM_TRUE.\r5, After step 7, the result:\rMSG_CLOCK_SYNC_STAT LCOM write shall return GM_TRUE;\rEncode ClockSYN_1 shall be 0x158D4F8D,ClockSYN_2 shall be 0x4C83B716, and shall be send to MPU.\rAdjust shall be TSP_PROC_SYN_CLOCK_ADJUST_DELAY;\rClockSyned shall be GM_TRUE.\r6, After step 8, the result:\rMSG_CLOCK_SYNC_STAT LCOM write shall return GM_TRUE;\rEncode ClockSYN_1 shall be 0x158D4F8D,ClockSYN_2 shall be 0x4C83B716, and shall be send to MPU.\rAdjust shall be TSP_PROC_SYN_CLOCK_ADJUST_NO;\rClockSyned shall be GM_TRUE.\r7, After step 9, the result:\rMSG_CLOCK_SYNC_STAT LCOM write shall return GM_TRUE;\rEncode ClockSYN_1 shall be 0x0,ClockSYN_2 shall be 0x0, and shall be send to MPU.\rAdjust shall be TSP_PROC_SYN_CLOCK_ADJUST_REDUCE;\rClockSyned shall be GM_FALSE.\r8, After step 10, the result:\rMSG_CLOCK_SYNC_STAT LCOM write shall return GM_TRUE;\rEncode ClockSYN_1 shall be 0x0,ClockSYN_2 shall be 0x0, and shall be send to MPU.\rAdjust shall be TSP_PROC_SYN_CLOCK_ADJUST_UNKNOWN;\rClockSyned shall be GM_FALSE.\r9, After step 12, the result:\rMSG_CLOCK_SYNC_STAT LCOM write shall return GM_TRUE;\rEncode ClockSYN_1 shall be 0x158D4F8D,ClockSYN_2 shall be 0x4C83B716, and shall be send to MPU.\rClockSyned shall be GM_TRUE.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0184"",""description"":""TSP-MPS-MCU-SwITC-0184\rTo check MCU1 could receive the version request message from SDMS and send version message to SDMS.\r\r[Source: [TSP-MPS-MCU-SwAD-0054]]\r[Source: [TSP-MPS-MCU-SwAD-0024]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0054]"",""[TSP-MPS-MCU-SwAD-0024]""],""input"":""TSP_Conf.ini\r\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim ,MPU2_Sim,MCU2_Sim and SDMS_Sim.\rStart the MCU1 INIT_TASK.\rSend version request message to MCU1\rCall MCU1 TSP_Proc_Main_Step_2 function.\r\u0007"",""exp_step"":""1, MCU1 received version request message correctly.\r2,SDMS_SIm received version file message correctly, include data version, software version, board version, CPLD version, BSP version and DRV version..\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0027"",""description"":""TSP-MPS-MCU-SwITC-0027\rCheck the MCU1 could send the RCHKW to VPS correctly if the application type is ZC/LC.\r\r[Source: [TSP-MPS-MCU-SwAD-0054]]\r[Source: [TSP-MPS-MCU-SwAD-0055]]\r[Source: [TSP-MPS-MCU-SwAD-0056]]\r[Source: [TSP-MPS-MCU-SwAD-0057]]\r[Source: [TSP-MPS-MCU-SwAD-0058]]\r[Source: [TSP-MPS-MCU-SwAD-0021]]\r[Source: [TSP-MPS-MCU-SwAD-0041]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0054]"",""[TSP-MPS-MCU-SwAD-0055]"",""[TSP-MPS-MCU-SwAD-0056]"",""[TSP-MPS-MCU-SwAD-0057]"",""[TSP-MPS-MCU-SwAD-0058]"",""[TSP-MPS-MCU-SwAD-0021]"",""[TSP-MPS-MCU-SwAD-0041]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO from MPU1 indicate the NR status is Normal.\r\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK. \rCall MCU1 Slot 0 to send RCHKW.\rCall MCU1 Slot 2 to send RCHKW.\rCall MCU1 Slot 4 to send RCHKW.\rCall MCU1 Slot 6 to send RCHKW.\rCall MCU1 Slot 8 to send RCHKW.\rCall MCU1 Slot 10 to send RCHKW.\r\u0007"",""exp_step"":""1, Send_MR_CHKW shall return 0 indicate send RCHKW is successful.\r2, the RCHKW send to VPS shall be equal with the value as follows:\r0\t0xBBE31DC6, 0xB8E31AC6, 0xB5E317C6, 0xB2E314C6, 0xAFE311C6, 0xACE30EC6, 0xA9E30BC6, 0xA6E308C6, 0xA3E305C6, 0xA0E302C6,0x9DE3FFC6, 0x9AE3FCC6, 0x97E3F9C6, 0x94E3F6C6, 0x91E3F3C6, 0x8EE3F0C6, 0x8BE3EDC6, 0x88E3EAC6, 0x85E3E7C6, 0x82E3E4C6,0x7FE3E1C6, 0x7CE3DEC6, 0x79E3DBC6, 0x76e3D8C6, 0x6F71D5C6, 0x6C71D2C6, 0x6971CFC6, 0x6671CCC6, 0x6371C9C6, 0x6071C6C6, 0x5D71C3C6\r1\t0xB2E31546, 0xAEE31146, 0xAAE30D46, 0xA6E30946, 0xA2E30546, 0x9EE30146, 0x9AE3FD46, 0x96E3F946, 0x92E3F546, 0x8EE3F146, 0x8AE3ED46, 0x86E3E946, 0x82E3E546, 0x7EE3E146, 0x7AE3DD46, 0x76E3D946, 0x72E3D546, 0x6EE3D146, 0x6AE3CD46, 0x66E3C946, 0x62E3C546, 0x5EE3C146, 0x5AE3BD46, 0x56E3B946, 0x4E71B546, 0x4A71B146, 0x4671AD46, 0x4271A946, 0x3E71A546, 0x3A71A146,0x36719D46\r2\t0xA9E30CC6, 0xA4E307C6, 0x9FE302C6, 0x9AE3FDC6, 0x95E3F8C6, 0x90E3F3C6, 0x8BE3EEC6, 0x86E3E9C6, 0x81E3E4C6, 0x7CE3DFC6, 0x77E3DAC6, 0x72E3D5C6, 0x6DE3D0C6, 0x68E3CBC6, 0x63E3C6C6, 0x5EE3C1C6, 0x59E3BCC6, 0x54E3B7C6, 0x4FE3B2C6, 0x4AE3ADC6, 0x45E3A8C6, 0x40E3A3C6, 0x3BE39EC6, 0x36E399C6, 0x2D7194C6, 0x28718FC6, 0x23718AC6, 0x1E7185C6, 0x197180C6, 0x14717BC6,0xF7176C6\r3\t0xA0E30446, 0x9AE3FE46, 0x94E3F846, 0x8EE3F246, 0x88E3EC46, 0x82E3E646, 0x7CE3E046, 0x76E3DA46, 0x70E3D446, 0x6AE3CE46, 0x64E3C846, 0x5EE3C246, 0x58E3BC46, 0x52E3B646, 0x4CE3B046, 0x46E3AA46, 0x40E3A446, 0x3AE39E46, 0x34E39846, 0x2EE39246, 0x28E38C46, 0x22E38646, 0x1CE38046, 0x16E37A46, 0x0C717446, 0x06716E46, 0x00716846, 0xFA716246, 0xF4715C46, 0xEE715646,0xE8715046\r4\t0x97E3FBC6, 0x90E3F4C6, 0x89E3EDC6, 0x82E3E6C6, 0x7BE3DFC6, 0x74E3D8C6, 0x6DE3D1C6, 0x66E3CAC6, 0x5FE3C3C6, 0x58E3BCC6, 0x51E3B5C6, 0x4AE3AEC6, 0x43E3A7C6, 0x3CE3A0C6, 0x35E399C6, 0x2EE392C6, 0x27E38BC6, 0x20E384C6, 0x19E37DC6, 0x12E376C6, 0x0BE36FC6, 0x04E368C6, 0xFDE361C6, 0xF6E35AC6, 0xEB7153C6, 0xE4714CC6, 0xDD7145C6, 0xD6713EC6, 0xCF7137C6, 0xC87130C6,0xC17129C6\r5\t0x8EE3F346, 0x86E3EB46, 0x7EE3E346, 0x76E3DB46, 0x6EE3D346, 0x66E3CB46, 0x5EE3C346, 0x56E3BB46, 0x4EE3B346, 0x46E3AB46, 0x3EE3A346, 0x36E39B46, 0x2EE39346, 0x26E38B46, 0x1EE38346, 0x16E37B46, 0x0EE37346, 0x06E36B46, 0xFEE36346, 0xF6E35B46, 0xEEE35346, 0xE6E34B46, 0xDEE34346, 0xD6E33B46, 0xCA713346, 0xC2712B46, 0xBA712346, 0xB2711B46, 0xAA711346, 0xA2710B46,0x9A710346\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0148"",""description"":""TSP-MPS-MCU-SwITC-0148\rCheck the MCU1 could send the RCHKW to VPS correctly if the application type is CCS.\r\r[Source: [TSP-MPS-MCU-SwAD-0054]]\r[Source: [TSP-MPS-MCU-SwAD-0055]]\r[Source: [TSP-MPS-MCU-SwAD-0056]]\r[Source: [TSP-MPS-MCU-SwAD-0057]]\r[Source: [TSP-MPS-MCU-SwAD-0058]]\r[Source: [TSP-MPS-MCU-SwAD-0021]]\r[Source: [TSP-MPS-MCU-SwAD-0041]]\r[Source: [TSP-MPS-MCU-SwAD-0107]]\r[Source: [TSP-MPS-MCU-SwAD-0108]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0054]"",""[TSP-MPS-MCU-SwAD-0055]"",""[TSP-MPS-MCU-SwAD-0056]"",""[TSP-MPS-MCU-SwAD-0057]"",""[TSP-MPS-MCU-SwAD-0058]"",""[TSP-MPS-MCU-SwAD-0021]"",""[TSP-MPS-MCU-SwAD-0041]"",""[TSP-MPS-MCU-SwAD-0107]"",""[TSP-MPS-MCU-SwAD-0108]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO from MPU1 indicate the NR status is Normal.\r\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK. \rCall MCU1 Slot 0 to send RCHKW.(even cycle)\rCall MCU1 Slot 1 to send RCHKW. (even cycle)\rCall MCU1 Slot 2 to send RCHKW. (even cycle)\rCall MCU1 Slot 4 to send RCHKW. (even cycle)\rCall MCU1 Slot 5 to send RCHKW. (even cycle)\rCall MCU1 Slot 6 to send RCHKW. (even cycle)\rCall MCU1 Slot 8 to send RCHKW. (even cycle)\rCall MCU1 Slot 9 to send RCHKW. (even cycle)\rCall MCU1 Slot 10 to send RCHKW. (even cycle)\rCall MCU1 Slot 0 to send RCHKW.(next odd cycle)\rCall MCU1 Slot 1 to send RCHKW.(next odd cycle)\rCall MCU1 Slot 2 to send RCHKW.(next odd cycle)\rCall MCU1 Slot 4 to send RCHKW.(next odd cycle)\rCall MCU1 Slot 5 to send RCHKW.(next odd cycle)\rCall MCU1 Slot 6 to send RCHKW.(next odd cycle)\rCall MCU1 Slot 8 to send RCHKW.(next odd cycle)\rCall MCU1 Slot 9 to send RCHKW.(next odd cycle)\rCall MCU1 Slot 10 to send RCHKW.(next odd cycle)\r\u0007"",""exp_step"":""1, Send_MR_CHKW shall return 0 indicate send RCHKW is successful.\r2, the RCHKW send to VPS shall be equal with the value as follows:\r0\t0xBBE31DC6, 0xB8E31AC6, 0xB5E317C6, 0xB2E314C6, 0xAFE311C6, 0xACE30EC6, 0xA9E30BC6, 0xA6E308C6, 0xA3E305C6, 0xA0E302C6,0x9DE3FFC6, 0x9AE3FCC6, 0x97E3F9C6, 0x94E3F6C6, 0x91E3F3C6, 0x8EE3F0C6, 0x8BE3EDC6, 0x88E3EAC6, 0x85E3E7C6, 0x82E3E4C6,0x7FE3E1C6, 0x7CE3DEC6, 0x79E3DBC6, 0x76e3D8C6, 0x6F71D5C6, 0x6C71D2C6, 0x6971CFC6, 0x6671CCC6, 0x6371C9C6, 0x6071C6C6, 0x5D71C3C6\r1\t0xB2E31546, 0xAEE31146, 0xAAE30D46, 0xA6E30946, 0xA2E30546, 0x9EE30146, 0x9AE3FD46, 0x96E3F946, 0x92E3F546, 0x8EE3F146, 0x8AE3ED46, 0x86E3E946, 0x82E3E546, 0x7EE3E146, 0x7AE3DD46, 0x76E3D946, 0x72E3D546, 0x6EE3D146, 0x6AE3CD46, 0x66E3C946, 0x62E3C546, 0x5EE3C146, 0x5AE3BD46, 0x56E3B946, 0x4E71B546, 0x4A71B146, 0x4671AD46, 0x4271A946, 0x3E71A546, 0x3A71A146,0x36719D46\r2\t0xA9E30CC6, 0xA4E307C6, 0x9FE302C6, 0x9AE3FDC6, 0x95E3F8C6, 0x90E3F3C6, 0x8BE3EEC6, 0x86E3E9C6, 0x81E3E4C6, 0x7CE3DFC6, 0x77E3DAC6, 0x72E3D5C6, 0x6DE3D0C6, 0x68E3CBC6, 0x63E3C6C6, 0x5EE3C1C6, 0x59E3BCC6, 0x54E3B7C6, 0x4FE3B2C6, 0x4AE3ADC6, 0x45E3A8C6, 0x40E3A3C6, 0x3BE39EC6, 0x36E399C6, 0x2D7194C6, 0x28718FC6, 0x23718AC6, 0x1E7185C6, 0x197180C6, 0x14717BC6,0xF7176C6\r3\t0xA0E30446, 0x9AE3FE46, 0x94E3F846, 0x8EE3F246, 0x88E3EC46, 0x82E3E646, 0x7CE3E046, 0x76E3DA46, 0x70E3D446, 0x6AE3CE46, 0x64E3C846, 0x5EE3C246, 0x58E3BC46, 0x52E3B646, 0x4CE3B046, 0x46E3AA46, 0x40E3A446, 0x3AE39E46, 0x34E39846, 0x2EE39246, 0x28E38C46, 0x22E38646, 0x1CE38046, 0x16E37A46, 0x0C717446, 0x06716E46, 0x00716846, 0xFA716246, 0xF4715C46, 0xEE715646,0xE8715046\r4\t0x97E3FBC6, 0x90E3F4C6, 0x89E3EDC6, 0x82E3E6C6, 0x7BE3DFC6, 0x74E3D8C6, 0x6DE3D1C6, 0x66E3CAC6, 0x5FE3C3C6, 0x58E3BCC6, 0x51E3B5C6, 0x4AE3AEC6, 0x43E3A7C6, 0x3CE3A0C6, 0x35E399C6, 0x2EE392C6, 0x27E38BC6, 0x20E384C6, 0x19E37DC6, 0x12E376C6, 0x0BE36FC6, 0x04E368C6, 0xFDE361C6, 0xF6E35AC6, 0xEB7153C6, 0xE4714CC6, 0xDD7145C6, 0xD6713EC6, 0xCF7137C6, 0xC87130C6,0xC17129C6\r5\t0x8EE3F346, 0x86E3EB46, 0x7EE3E346, 0x76E3DB46, 0x6EE3D346, 0x66E3CB46, 0x5EE3C346, 0x56E3BB46, 0x4EE3B346, 0x46E3AB46, 0x3EE3A346, 0x36E39B46, 0x2EE39346, 0x26E38B46, 0x1EE38346, 0x16E37B46, 0x0EE37346, 0x06E36B46, 0xFEE36346, 0xF6E35B46, 0xEEE35346, 0xE6E34B46, 0xDEE34346, 0xD6E33B46, 0xCA713346, 0xC2712B46, 0xBA712346, 0xB2711B46, 0xAA711346, 0xA2710B46,0x9A710346\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0028"",""description"":""TSP-MPS-MCU-SwITC-0028\rCheck the MCU1 could dispose the CBIT result correctly.\r\r[Source: [TSP-MPS-MCU-SwAD-0059]]\r[Source: [TSP-MPS-MCU-SwAD-0049]]\r[Source: [TSP-MPS-MCU-SwAD-0051]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0059]"",""[TSP-MPS-MCU-SwAD-0049]"",""[TSP-MPS-MCU-SwAD-0051]"",""[TSP-MPS-MCU-SwAD-0022]""],""input"":""TSP_Conf.ini\rTSP_COM_MSG_NR_INFO from MPU1 indicate the NR status is Normal.\r\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK.\rWhen VSN0 = 50, modify the CBIT return XOR result value equal with 0x12345678.\r\u0007"",""exp_step"":""1, RAM test shall be less than 28ms every cycle.\r2, Every 32 cycle, the CBIT result shall be different.\r3, When 1\u003cVSN0\u003c50, the MCU1_CHKW_BIT shall be equal with 3DD4B0B7H.\r4, When VSN0 = 50, the MCU1_CHKW_BIT shall not be equal with 3DD4B0B7H.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0029"",""description"":""TSP-MPS-MCU-SwITC-0029\rCheck when MCU1 in Normal, MCU1 could get the CHKW message from MPU1, MPU2 and MCU2, refresh checkword according to source address, and combine communication checkword correctly.\r\r[Source: [TSP-MPS-MCU-SwAD-0059]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r[Source: [TSP-MPS-MCU-SwAD-0041]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0059]"",""[TSP-MPS-MCU-SwAD-0022]"",""[TSP-MPS-MCU-SwAD-0041]""],""input"":""TSP_Conf.ini\rTSP_COM_MSG_TYPE_MPU1_NR_INFO from MPU1 indicate the NR status is Normal.\rTSP_COM_MSG_TYPE_MPU1_CHKW msg from MPU1, the check word of MPU1 are as follows:\rMPU1_CHKW_BIT 0x55D5B691\rMPU1_CHKW_MAIN_CYCLE_INTERVAL 0x52000\rMPU1_CHKW_TASK_CHECK 0x660D57BB\rMPU1_CHKW_NR_STATE 0x637A0ABC\rMPU1_CHKW_SACEM_RX_CH1 0x6CDE6DF3\rMPU1_CHKW_SACEM_RX_CH2 0x5359548D\rTSP_COM_MSG_TYPE_MPU2_CHKW msg from MPU2, the check word of MPU2 are as follows:\rMPU2_CHKW_BIT 0x5C25BD44\rMPU2_CHKW_MAIN_CYCLE_INTERVAL 0x52000\rMPU2_CHKW_TASK_CHECK 0x535BC35F\rMPU2_CHKW_NR_STATE 0x637A0ABC\rMPU2_CHKW_SACEM_RX_CH1 0x6CDE6DF3\rMPU2_CHKW_SACEM_RX_CH2 0x5359548D\rTSP_COM_MSG_TYPE_MCU2_CHKW msg from MCU2, the check word of MCU2 are as follows:\rMCU2_CHKW_BIT 0x4FA78CDB\rMCU2_CHKW_MAIN_CYCLE_INTERVAL 0x52000\rMCU2_CHKW_TASK_CHECK 0x57BED039\rMCU2_CHKW_JTC_CH1 0xAF206E62\rMCU2_CHKW_JTC_CH2 0x1EF2BFE0\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Normal state.\rStart the MCU1 INIT_TASK.\rSend CHKW message from MCU2_Sim, MPU1_Sim and MPU2_Sim to MCU1.\rCall MCU1 TSP_Proc_Main_Step_11 function.\r\u0007"",""exp_step"":""1, Check word shall be generated as follows,\rTSP_CHKW_MPU1_BIT,\t\t\t\t\t0x79F6733B\rTSP_CHKW_MPU1_MAIN_CYCLET_INTERVAL,\t0x71F66B3B\rTSP_CHKW_MPU1_TASK_CHECK,\t\t\t0x69F6633B\rTSP_CHKW_MPU1_NR_STATE,\t\t\t\t0x61F65B3B\rTSP_CHKW_MPU1_SAFETYCOM,\t\t\t0x59F6533B\rTSP_CHKW_MPU2_BIT,\t\t\t\t\t0x51F64B3B\rTSP_CHKW_MPU2_MAIN_CYCLE_INTERVAL,\t0x49F6433B\rTSP_CHKW_MPU2_TASK_CHECK,\t\t\t0x41F63B3B\rTSP_CHKW_MPU2_NR_STATE,\t\t\t\t0x39F6333B\t\t\t\rTSP_CHKW_MPU2_SAFETYCOM,\t\t\t0x31F62B3B\rTSP_CHKW_MCU2_BIT,\t\t\t\t\t0x01F6FB3B\rTSP_CHKW_MCU2_MAIN_CYCLE_INTERVAL,\t0xF9F6F33B\rTSP_CHKW_MCU2_TASK_CHECK,\t\t\t0xF1F6EB3B\rTSP_CHKW_MCU2_JTC_CH1,\t\t\t\t0xE9F6E33B\rTSP_CHKW_MCU2_JTC_CH2,\t\t\t\t0xE214E862\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0069"",""description"":""TSP-MPS-MCU-SwITC-0069\rCheck when MCU1 is Reserve, MCU1 could get the MSG_CHKW_INFO message from MPU1, MPU2 and MCU2, refresh checkword according to source address, and combine communication checkword correctly.\r\r[Source: [TSP-MPS-MCU-SwAD-0059]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0059]"",""[TSP-MPS-MCU-SwAD-0022]""],""input"":""TSP_Conf.ini\rTSP_COM_MSG_TYPE_MPU1_NR_INFO from MPU1 indicate the NR status is Reserve.\rTSP_COM_MSG_TYPE_MPU1_CHKW msg from MPU1, the check word of MPU1 are as follows:\rMPU1_CHKW_BIT 0x55D5B691\rMPU1_CHKW_MAIN_CYCLE_INTERVAL 0x52800\rMPU1_CHKW_TASK_CHECK 0x660D57BB\rMPU1_CHKW_NR_STATE 0x637A0ABC\rMPU1_CHKW_SACEM_RX_CH1 0x6CDE6DF3\rMPU1_CHKW_SACEM_RX_CH2 0x5359548D\rTSP_COM_MSG_TYPE_MPU2_CHKW msg from MPU2, the check word of MPU2 are as follows:\rMPU2_CHKW_BIT 0x5C25BD44\rMPU2_CHKW_MAIN_CYCLE_INTERVAL 0x52800\rMPU2_CHKW_TASK_CHECK 0x535BC35F\rMPU2_CHKW_NR_STATE 0x637A0ABC\rMPU2_CHKW_SACEM_RX_CH1 0x6CDE6DF3\rMPU2_CHKW_SACEM_RX_CH2 0x5359548D\rTSP_COM_MSG_TYPE_MCU2_CHKW msg from MCU2, the check word of MCU2 are as follows:\rMCU2_CHKW_BIT 0x4FA78CDB\rMCU2_CHKW_MAIN_CYCLE_INTERVAL 0x52800\rMCU2_CHKW_TASK_CHECK 0x57BED039\rMCU2_CHKW_JTC_CH1 0xAF206E62\rMCU2_CHKW_JTC_CH2 0x1EF2BFE0\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Reserve state.\rStart the MCU1 INIT_TASK.\rLet the MCU1 run for 338ms(502ms for CCS).\rSend CHKW message from MCU2_Sim, MPU1_Sim and MPU2_Sim to MCU1.\rCall MCU1 TSP_Proc_Main_Step_11 function.\r\u0007"",""exp_step"":""1, Check word shall be generated as follows,\rTSP_CHKW_MPU1_BIT,\t\t\t\t\t0x79F6733B\rTSP_CHKW_MPU1_MAIN_CYCLE_INTERVAL,\t0x71F66B3B\rTSP_CHKW_MPU1_TASK_CHECK,\t\t\t0x69F6633B\rTSP_CHKW_MPU1_NR_STATE,\t\t\t\t0x61F65B3B\rTSP_CHKW_MPU1_SAFETYCOM,\t\t\t0x59F6533B\rTSP_CHKW_MPU2_BIT,\t\t\t\t\t0x51F64B3B\rTSP_CHKW_MPU2_MAIN_CYCLE_INTERVAL,\t0x49F6433B\rTSP_CHKW_MPU2_TASK_CHECK,\t\t\t0x41F63B3B\rTSP_CHKW_MPU2_NR_STATE,\t\t\t\t0x39F6333B\t\t\t\rTSP_CHKW_MPU2_SAFETYCOM,\t\t\t0x31F62B3B\rTSP_CHKW_MCU2_BIT,\t\t\t\t\t0x01F6FB3B\rTSP_CHKW_MCU2_MAIN_CYCLE_INTERVAL,\t0xF9F6F33B\rTSP_CHKW_MCU2_TASK_CHECK,\t\t\t0xF1F6EB3B\rTSP_CHKW_MCU2_JTC_CH1,\t\t\t\t0xE9F6E33B\rTSP_CHKW_MCU2_JTC_CH2,\t\t\t\t0xE214E862\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0305"",""description"":""TSP-MPS-MCU-SwITC-0305\rCheck MCU1 Send Debug/Dump/Print message to SDMS correctly.\rCheck MCU1 shall not send message to SDMS if the message length is o.\rCheck MCU1 Send CHKW message to SDMS correctly.\r[Source: [TSP-MPS-MCU-SwAD-0059]]\r[Source: [TSP-MPS-MCU-SwAD-0106]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0059]"",""[TSP-MPS-MCU-SwAD-0106]""],""input"":""TSP_Conf.ini\r\r\u0007"",""exec_step"":""Start the TestBench and run MCU1\rStart the MCU1 INIT_TASK.\rCall MCU1 TSP_Proc_Main_Step_11 function.\rPrint the body of Dump message.\rStub to set the length of message sent to SDMS to be 0 from VSN 10-20.\r\u0007"",""exp_step"":""1, MCU1 send maintain message successfully\u000b2,SDMS received Debug/Dump/Print message successfully\r3,SDMS received CHKW message successfully\r4,SDMS shall not receive Debug/Dump/Print message from VSN 10-20\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0180"",""description"":""TSP-MPS-MCU-SwITC-0180\rCheck MCU1 shall add 2048 bytes tsp print message in each cycle, if the message length is more 2048, MCU1 shall not send to SDMS.\r[Source: [TSP-MPS-MCU-SwAD-0059]]\r[Source: [TSP-MPS-MCU-SwAD-0106]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP/BA\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0059]"",""[TSP-MPS-MCU-SwAD-0106]""],""input"":""TSP_Conf.ini\rMessage 1: length=2048\rMessage 2: length =2049\r\u0007"",""exec_step"":""Start the TestBench and run MCU1\rStart the MCU1 INIT_TASK.\rAdd message 1 to TSP print message buffer(VSN=10)\rCall MCU1 TSP_Proc_Main_Step_11 function.\rAdd message 2 to TSP print message buffer(VSN=11)\rCall MCU1 TSP_Proc_Main_Step_11 function.\r\u0007"",""exp_step"":""MCU1 add message 1 successfully.\rSDMS received message1 which is same with the send one\rMCU1 add message 2 failed.\rSDMS can’t receive message2.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0085"",""description"":""TSP-MPS-MCU-SwITC-0085\rWhen MCU1 is Reverse, check the MCU1 could do time synchronization, adjust clock according to adjust parameter.\r[Source: [TSP-MPS-MCU-SwAD-0047]]\r[Source: [TSP-MPS-MCU-SwAD-0029]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0047]"",""[TSP-MPS-MCU-SwAD-0029]""],""input"":""TSP_Conf.ini\rTSP_COM_MSG_TYPE_MPU1_NR_INFO from MPU1 indicate the NR status is Reserve.\r\u0007"",""exec_step"":""Start the TestBench and return MCU1 in Reserve state.\rStart the MCU1 INIT_TASK.\rWhen VSN = 10, set TSP_Proc_Syn_Clock_RcvTime = 12999 + TSP_CLOCK_SYNC_TIME_DELAY;\tTSP_Proc_Syn_Clock_SendTime = 10000.\rWhen VSN = 11, set TSP_Proc_Syn_Clock_RcvTime = 10000 + TSP_CLOCK_SYNC_TIME_DELAY;\tTSP_Proc_Syn_Clock_SendTime = 12999.\r\u0007"",""exp_step"":""1, After step 3, the l_safetmr_slot_TickNum shall be 9.\r2, After step 4, the l_safetmr_slot_TickNum shall be 7.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0088"",""description"":""TSP-MPS-MCU-SwITC-0088\rCheck the MCU1 CBIT could suspend the task which is being test by CBIT, and release it when test is over.\r\r[Source: [TSP-MPS-MCU-SwAD-0051]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0051]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MCU2_Sim, MPU1_Sim and MPU2_Sim.\rStart the MCU1 INIT_TASK.\rCall taskInfoGet routine to check the task status of MainProc, MUDP_Rcv, LCOM_SIO_RCV and SIO_Rx_Task when CBIT testing it.\r\u0007"",""exp_step"":""1, MainProc, MUDP_Rcv, LCOM_SIO_RCV and SIO_Rx_Task will be suspended when CBIT testing it, resume when testing is over.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0311"",""description"":""TSP-MPS-MCU-SwITC-0311\rCheck MCU1 shall provide interface to transmit information to CBIT module for test.\r\r[Source: [TSP-MPS-MCU-SwAD-0051]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0051]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rPrint the address of TEXT, DATA and BSS segment In vxWorks.\rStart the MCU1 INIT_TASK.\rMCU1transmits the following information to CBIT module:\rName of error log disk\rName of CBIT error log\rTest speed of TEXT segment, DATA segment and task stack(0x00000800, 0x00002000, 0x00000800)\rPriority of CBIT task(65)\rTask number(4)\rTask name to be tested(MainProc, MUDP_Rcv, SIO_Rx_Task, LCOM_SIO_RCV)\rSegment number(TEXT 1, DATA 2)\rAddress and size of TEXT, DATA and BSS segment, MD5 value of TEXT segment\rPrint the information mentioned above in CBIT module.\r\u0007"",""exp_step"":""In CBIT module:\r1, The name of error log disk is /tffs1/.\r2, The name of error log is CBIT_Err.log.\r3, The test speed of TEXT segment, DATA segment and task stack is 0x00000800, 0x00002000 and 0x00000800.\r4, The priority of CBIT task is 65.\r5, The task number is 4.\r6, The tasks to be tested are MainProc, MUDP_Rcv, SIO_Rx_Task and LCOM_SIO_RCV).\r7, The number of TEXT segment is 1. The number of DATA segment is 2.\r6, The address and size of TEXT, DATA and BSS segment is the same with the result printed in vxWorks.\r7, The MD5 of TEXT segment is the same with the result printed in vxWorks.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0091"",""description"":""TSP-MPS-MCU-SwITC-0091\rCheck the MCU1 CBIT will finish one test in 10h.\r\r[Source: [TSP-MPS-MCU-SwAD-0051]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0051]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MCU2_Sim, MPU1_Sim and MPU2_Sim.\rStart the MCU1 INIT_TASK.\rLog the time interval between two RAM_Stack test start time, the time interval is the CBIT running time.\r\u0007"",""exp_step"":""1, The CRC of CPU test result is rotated every 32 cycles.\r2, One full testing of CBIT shall finish in 10h.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0300"",""description"":""TSP-MPS-MCU-SwITC-0300\rIf the application type is ZC/LC:\rCheck the MCU1 TSP_Proc_Init_Pre will be run complete in 270s.\rCheck the MCU1 slot0 will be run complete in 28ms.\rCheck the MCU1 slot1 will be run complete in 28ms.\rCheck the MCU1 slot2 will be run complete in 28ms.\rCheck the MCU1 slot4 will be run complete in 28ms.\rCheck the MCU1 slot6 will be run complete in 28ms.\rCheck the MCU1 slot8 will be run complete in 28ms.\rCheck the MCU1 slot10 will be run complete in 40ms.\rCheck the MCU1 slot11 will be run complete in 16ms.\r\r\r[Source: [TSP-MPS-MCU-SwAD-0052]]\r[Source: [TSP-MPS-MCU-SwAD-0053]]\r[Source: [TSP-MPS-MCU-SwAD-0054]]\r[Source: [TSP-MPS-MCU-SwAD-0055]]\r[Source: [TSP-MPS-MCU-SwAD-0056]]\r[Source: [TSP-MPS-MCU-SwAD-0057]]\r[Source: [TSP-MPS-MCU-SwAD-0058]]\r[Source: [TSP-MPS-MCU-SwAD-0059]]\r[Source: [TSP-MPS-MCU-SwAD-0030]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0052]"",""[TSP-MPS-MCU-SwAD-0053]"",""[TSP-MPS-MCU-SwAD-0054]"",""[TSP-MPS-MCU-SwAD-0055]"",""[TSP-MPS-MCU-SwAD-0056]"",""[TSP-MPS-MCU-SwAD-0057]"",""[TSP-MPS-MCU-SwAD-0058]"",""[TSP-MPS-MCU-SwAD-0059]"",""[TSP-MPS-MCU-SwAD-0030]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MCU2_Sim, MPU1_Sim and MPU2_Sim.\rStart the MCU1 INIT_TASK.\rCheck the time MCU1 slots running.\r\u0007"",""exp_step"":""1, the MCU1 TSP_Proc_Init_Pre shall be run complete in 270s.\rMCU1 slot0 shall be run complete in 28ms.\r MCU1 slot1 shall be run complete in 28ms.\r MCU1 slot2 shall be run complete in 28ms.\r MCU1 slot4 shall be run complete in 28ms.\r MCU1 slot6 shall be run complete in 28ms.\r MCU1 slot8 shall be run complete in 28ms.\r MCU1 slot10 shall be run complete in 40ms.\r MCU1 slot11 shall be run complete in 16ms.\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0149"",""description"":""TSP-MPS-MCU-SwITC-0149\rIf the application type is CCS:\rCheck the MCU1 TSP_Proc_Init_Pre will be run complete in 270s.\rCheck the MCU1 slot0 will be run complete in 56ms.\rCheck the MCU1 slot1 will be run complete in 56ms.\rCheck the MCU1 slot2 will be run complete in 20ms.\rCheck the MCU1 slot3 will be run complete in 36ms.\rCheck the MCU1 slot4 will be run complete in 54ms.\rCheck the MCU1 slot5 will be run complete in 56ms.\rCheck the MCU1 slot6 will be run complete in 30ms.\rCheck the MCU1 slot7will be run complete in 24ms.\rCheck the MCU1 slot8 will be run complete in 56ms.\rCheck the MCU1 slot9 will be run complete in 56ms.\rCheck the MCU1 slot10 will be run complete in 40ms.\rCheck the MCU1 slot11 will be run complete in 16ms.\r\r\r[Source: [TSP-MPS-MCU-SwAD-0052]]\r[Source: [TSP-MPS-MCU-SwAD-0053]]\r[Source: [TSP-MPS-MCU-SwAD-0054]]\r[Source: [TSP-MPS-MCU-SwAD-0055]]\r[Source: [TSP-MPS-MCU-SwAD-0056]]\r[Source: [TSP-MPS-MCU-SwAD-0057]]\r[Source: [TSP-MPS-MCU-SwAD-0058]]\r[Source: [TSP-MPS-MCU-SwAD-0059]]\r[Source: [TSP-MPS-MCU-SwAD-0030]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0052]"",""[TSP-MPS-MCU-SwAD-0053]"",""[TSP-MPS-MCU-SwAD-0054]"",""[TSP-MPS-MCU-SwAD-0055]"",""[TSP-MPS-MCU-SwAD-0056]"",""[TSP-MPS-MCU-SwAD-0057]"",""[TSP-MPS-MCU-SwAD-0058]"",""[TSP-MPS-MCU-SwAD-0059]"",""[TSP-MPS-MCU-SwAD-0030]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MCU2_Sim, MPU1_Sim and MPU2_Sim.\rStart the MCU1 INIT_TASK.\rCheck the time MCU1 slots running.\r\u0007"",""exp_step"":""1, the MCU1 TSP_Proc_Init_Pre shall be run complete in 270s.\rMCU1 slot0 shall be run complete in 56ms.\rMCU1 slot1 shall be run complete in 56ms.\rMCU1 slot2 shall be run complete in 20ms.\rMCU1 slot3 shall be run complete in 36ms.\rMCU1 slot4 shall be run complete in 54ms.\rMCU1 slot5 shall be run complete in 56ms.\rMCU1 slot6 shall be run complete in 30ms.\rMCU1 slot7 shall be run complete in 24ms.\rMCU1 slot8 shall be run complete in 56ms.\rMCU1 slot9 shall be run complete in 56ms.\rMCU1 slot10 shall be run complete in 40ms.\rMCU1 slot11 shall be run complete in 16ms.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0303"",""description"":""TSP-MPS-MCU-SwITC-0303\rCheck when peer MCU1 is Normal, local MCU1 is init Normal, Local MCU1 will send the error number MAIN_BOTH_ACTIVE_ERR (0x1007001B) to SDMS, MCU1 fail and enter endless loop.\r\r\r[Source: [TSP-MPS-MCU-SwAD-0025]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0055]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0025]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0055]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MCU2_Sim, MPU1_Sim and MPU2_Sim, MCU1_Sim\rStart the MCU1 INIT_TASK.\rLet the Local MPU1_Sim send Normal AS state, Peer MCU1_Sim send Normal AS state to MCU1.\r\u0007"",""exp_step"":""1, the MCU1 shall send the error Number MAIN_BOTH_ACTIVE_ERR (0x1007001B) to SDMS, fail and enter in endless loop.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0304"",""description"":""TSP-MPS-MCU-SwITC-0304\rCheck when peer MCU1 is Normal, local MCU1 is change from Reserve to Normal, Local MCU1 will send the error MAIN_BOTH_ACTIVE_ERR (0x1007001B) to SDMS, fail and enter in endless loop.\r\r[Source: [TSP-MPS-MCU-SwAD-0025]]\r[Source: [TSP-MPS-MCU-SwAD-0090]]\r[Source: [TSP-MPS-MCU-SwAD-0055]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0025]"",""[TSP-MPS-MCU-SwAD-0090]"",""[TSP-MPS-MCU-SwAD-0055]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MCU2_Sim, MPU1_Sim and MPU2_Sim, MCU1_Sim\rStart the MCU1 INIT_TASK.\rLet the Local MPU1_Sim send Reserve AS state for the first 10 cycle, and then send Normal AS state, Peer MCU1_Sim send Normal AS state to MCU1 always.\r\u0007"",""exp_step"":""1, the MCU1 shall send the error MAIN_BOTH_ACTIVE_ERR (0x1007001B) to SDMS, fail and enter in endless loop.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0032"",""description"":""TSP-MPS-MCU-SwITC-0032\rTo check MCU2 TSP_Proc_Main_Step_10 will calculate the main task run time check word correctly If the application type is ZC/LC.\r\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r[Source: [TSP-MPS-MCU-SwAD-0063]]\r[Source: [TSP-MPS-MCU-SwAD-0047]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP/BA\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0045]"",""[TSP-MPS-MCU-SwAD-0063]"",""[TSP-MPS-MCU-SwAD-0047]"",""[TSP-MPS-MCU-SwAD-0022]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rWhen VSN = 10, set the main task running time to 333ms.\rCall MCU1 TSP_Proc_Main_Step_10.\rWhen VSN = 11, set the main task running time to 339ms.\rCall MCU1 TSP_Proc_Main_Step_10.\rWhen VSN = 12, set the main task running time to 336ms (here the last time is bigger than current time).\rCall MCU1 TSP_Proc_Main_Step_10.\rWhen VSN = 13, set the main task running time to 340ms.\rCall MCU1 TSP_Proc_Main_Step_10.\r\u0007"",""exp_step"":""1, After step 4, the timeGap shall be 0x51800.\r2, After step 6, the timeGap shall be 0x52800.\r3, After step 8, the timeGap shall be 0x52000.\r4, After step 10, the timeGap shall be 0x53000.\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0150"",""description"":""TSP-MPS-MCU-SwITC-0150\rTo check MCU2 TSP_Proc_Main_Step_10 will calculate the main task run time check word correctly If the application type is CCS.\r\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r[Source: [TSP-MPS-MCU-SwAD-0063]]\r[Source: [TSP-MPS-MCU-SwAD-0047]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0045]"",""[TSP-MPS-MCU-SwAD-0063]"",""[TSP-MPS-MCU-SwAD-0047]"",""[TSP-MPS-MCU-SwAD-0022]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rWhen VSN = 10, set the main task running time to 497ms.\rCall MCU1 TSP_Proc_Main_Step_10.\rWhen VSN = 11, set the main task running time to 503ms.\rCall MCU1 TSP_Proc_Main_Step_10.\rWhen VSN = 12, set the main task running time to 500ms (here the last time is bigger than current time).\rCall MCU1 TSP_Proc_Main_Step_10.\rWhen VSN = 13, set the main task running time to 504ms.\rCall MCU1 TSP_Proc_Main_Step_10.\r\u0007"",""exp_step"":""1, After step 4, the timeGap shall be 0x51800.\r2, After step 6, the timeGap shall be 0x52800.\r3, After step 8, the timeGap shall be 0x52000.\r4, After step 10, the timeGap shall be 0x53000.\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0051"",""description"":""TSP-MPS-MCU-SwITC-0051\rCheck the MCU2 could update task sequence check word correctly. When the task sequence is not correct, the task sequence shall be wrong if the application type is ZC/LC.\r\r[Source: [TSP-MPS-MCU-SwAD-0060]]\r[Source: [TSP-MPS-MCU-SwAD-0061]]\r[Source: [TSP-MPS-MCU-SwAD-0087]]\r[Source: [TSP-MPS-MCU-SwAD-0062]]\r[Source: [TSP-MPS-MCU-SwAD-0063]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP+EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0060]"",""[TSP-MPS-MCU-SwAD-0061]"",""[TSP-MPS-MCU-SwAD-0087]"",""[TSP-MPS-MCU-SwAD-0062]"",""[TSP-MPS-MCU-SwAD-0063]"",""[TSP-MPS-MCU-SwAD-0022]"",""[TSP-MPS-MCU-SwAD-0045]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rWhen vsn = 2, Call MCU2 TSP_Proc_Main_Step_0 to TSP_Proc_Main_Step_11 in order.\rWhen vsn = 4, Call MCU2 TSP_Proc_Main_Step_0 to TSP_Proc_Main_Step_11 in order except step 5.\r\u0007"",""exp_step"":""1, After step 4, the MCU2 task sequence check word shall be 57BED039H.\r2, After step 5, the MCU2 task sequence check word shall not be 57BED039H.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0151"",""description"":""TSP-MPS-MCU-SwITC-0151\rCheck the MCU2 could update task sequence check word correctly. When the task sequence is not correct, the task sequence shall be wrong if the application type is CCS.\r\r[Source: [TSP-MPS-MCU-SwAD-0060]]\r[Source: [TSP-MPS-MCU-SwAD-0061]]\r[Source: [TSP-MPS-MCU-SwAD-0087]]\r[Source: [TSP-MPS-MCU-SwAD-0062]]\r[Source: [TSP-MPS-MCU-SwAD-0063]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP+EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0060]"",""[TSP-MPS-MCU-SwAD-0061]"",""[TSP-MPS-MCU-SwAD-0087]"",""[TSP-MPS-MCU-SwAD-0062]"",""[TSP-MPS-MCU-SwAD-0063]"",""[TSP-MPS-MCU-SwAD-0022]"",""[TSP-MPS-MCU-SwAD-0045]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rWhen vsn = 2, Call MCU2 TSP_Proc_Main_Step_0 to TSP_Proc_Main_Step_11 in order.\rWhen vsn = 4, Call MCU2 TSP_Proc_Main_Step_0 to TSP_Proc_Main_Step_11 in order except step 7.\r\u0007"",""exp_step"":""1, After step 4, the MCU2 task sequence check word shall be 57BED039H.\r2, After step 5, the MCU2 task sequence check word shall not be 57BED039H.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0070"",""description"":""TSP-MPS-MCU-SwITC-0070\rCheck the MCU2 task sequence check word will be not correct if the task sequence is not correct if the application type is ZC/LC.\r\r[Source: [TSP-MPS-MCU-SwAD-0060]]\r[Source: [TSP-MPS-MCU-SwAD-0061]]\r[Source: [TSP-MPS-MCU-SwAD-0087]]\r[Source: [TSP-MPS-MCU-SwAD-0062]]\r[Source: [TSP-MPS-MCU-SwAD-0063]]\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0060]"",""[TSP-MPS-MCU-SwAD-0061]"",""[TSP-MPS-MCU-SwAD-0087]"",""[TSP-MPS-MCU-SwAD-0062]"",""[TSP-MPS-MCU-SwAD-0063]"",""[TSP-MPS-MCU-SwAD-0045]"",""[TSP-MPS-MCU-SwAD-0022]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rWhen vsn = 3, Call MCU2 TSP_Proc_Main_Step_0 to TSP_Proc_Main_Step_11, but change the order of step 2 and step 5.\r\u0007"",""exp_step"":""1, After step 4, the MCU2 task sequence check word shall not be 57BED039H.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0152"",""description"":""TSP-MPS-MCU-SwITC-0152\rCheck the MCU2 task sequence check word will be not correct if the task sequence is not correct if the application type is CCS.\r\r[Source: [TSP-MPS-MCU-SwAD-0060]]\r[Source: [TSP-MPS-MCU-SwAD-0061]]\r[Source: [TSP-MPS-MCU-SwAD-0087]]\r[Source: [TSP-MPS-MCU-SwAD-0062]]\r[Source: [TSP-MPS-MCU-SwAD-0063]]\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0060]"",""[TSP-MPS-MCU-SwAD-0061]"",""[TSP-MPS-MCU-SwAD-0087]"",""[TSP-MPS-MCU-SwAD-0062]"",""[TSP-MPS-MCU-SwAD-0063]"",""[TSP-MPS-MCU-SwAD-0045]"",""[TSP-MPS-MCU-SwAD-0022]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rWhen vsn = 3, Call MCU2 TSP_Proc_Main_Step_0 to TSP_Proc_Main_Step_11, but change the order of step 2 and step 3.\r\u0007"",""exp_step"":""1, After step 4, the MCU2 task sequence check word shall not be 57BED039H.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0072"",""description"":""TSP-MPS-MCU-SwITC-0072\rTo check when the messages received from MPU1/2 are wrong, the JTC Checkword shall not be correct.\r\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r[Source: [TSP-MPS-MCU-SwAD-0027]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0045]"",""[TSP-MPS-MCU-SwAD-0027]"",""[TSP-MPS-MCU-SwAD-0022]""],""input"":""TSP_Conf.ini\rGAPP msg from MPU2_Sim and MPU1_Sim, the data is all 0x5A, msgID is 1, msgSize is 256.\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rSend GAPP data from MPU2_Sim and MPU1_Sim.\rCall MCU2 TSP_Proc_Main_Step_10 function.\rWhen vsn0 = 20, send the different MPU2 GAPP data, set the data to zero.\r\u0007"",""exp_step"":""1, When VSN0 != 20, The MCU2_CHKW_JTC_CH1 shall be 0xAF206E62, MCU2_CHKW_JTC_CH2 shall be 0x1EF2BFE0.\r2, When VSN0 = 20, MCU2_CHKW_JTC_CH1 shall not be 0xAF206E62,   MCU2_CHKW_JTC_CH2 shall be 0x1EF2BFE0.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0037"",""description"":""TSP-MPS-MCU-SwITC-0037\rIf the application type is ZC/LC:\rto check MCU2 could do the following things correctly:\r1, Update the VSN.\r2, Send heartbeat message to MCU1.\r3, Send MSG_GAPP, RAW and RSSP1 message to MPU1/2.\r\r[Source: [TSP-MPS-MCU-SwAD-0060]]\r[Source: [TSP-MPS-MCU-SwAD-0020]]\r[Source: [TSP-MPS-MCU-SwAD-0026]]\r[Source: [TSP-MPS-MCU-SwAD-0047]]\r[Source: [TSP-MPS-MCU-SwAD-0049]]\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r[Source: [TSP-MPS-MCU-SwAD-0023]]\r[Source: [TSP-MPS-MCU-SwAD-0092]]\r[Source: [TSP-MPS-MCU-SwAD-0114]]\r[Source: [TSP-MPS-MCU-SwAD-0101]]\r[Source: [TSP-MPS-MCU-SwAD-0102]]\r[Source: [TSP-MPS-MCU-SwAD-0103]]\r[Source: [TSP-MPS-MCU-SwAD-0104]]\r[Source: [TSP-MPS-MCU-SwAD-0111]]\r[Source: [TSP-MPS-MCU-SwAD-0117]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0060]"",""[TSP-MPS-MCU-SwAD-0020]"",""[TSP-MPS-MCU-SwAD-0026]"",""[TSP-MPS-MCU-SwAD-0047]"",""[TSP-MPS-MCU-SwAD-0049]"",""[TSP-MPS-MCU-SwAD-0045]"",""[TSP-MPS-MCU-SwAD-0023]"",""[TSP-MPS-MCU-SwAD-0092]"",""[TSP-MPS-MCU-SwAD-0114]"",""[TSP-MPS-MCU-SwAD-0101]"",""[TSP-MPS-MCU-SwAD-0102]"",""[TSP-MPS-MCU-SwAD-0103]"",""[TSP-MPS-MCU-SwAD-0104]"",""[TSP-MPS-MCU-SwAD-0111]"",""[TSP-MPS-MCU-SwAD-0117]""],""input"":""TSP_Conf.ini\r\rGAPP msg from MPU2_Sim and MPU1_Sim:\rData: the first 4 bytes are loophour and the following is all 0x5A\r msgSize: 260\rRSSP1 msg from MPU2_Sim and MPU1_Sim:\rData: the first 4 bytes are loophour and the following is all 0x3C, msgSize: 999.\rRAW msg from MPU2_Sim and MPU1_Sim:\rData: the first 4 bytes are loophour and the following is all 0x50, msgSize: 1204.\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rCall MCU2 TSP_Proc_Main_Step_0 function.\r\u0007"",""exp_step"":""1, The VSN shall be updated correctly.\r2, Heartbeat message shall be received in MCU1.\r3, MSG_GAPP, and RSSP1 message received in MPU1_Sim and MPU2_Sim shall have correct value.\r4, For RAW message, MPU only receive the message from red network.\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0153"",""description"":""TSP-MPS-MCU-SwITC-0153\rIf the application type is CCS:\rto check MCU2 could do the following things correctly:\r1, Update the VSN.\r2, Send heartbeat message to MCU1.\r3, Send RSSP1,RSSP2,Subset037 message to MPU1/2.\r\r[Source: [TSP-MPS-MCU-SwAD-0060]]\r[Source: [TSP-MPS-MCU-SwAD-0020]]\r[Source: [TSP-MPS-MCU-SwAD-0026]]\r[Source: [TSP-MPS-MCU-SwAD-0047]]\r[Source: [TSP-MPS-MCU-SwAD-0049]]\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r[Source: [TSP-MPS-MCU-SwAD-0023]]\r[Source: [TSP-MPS-MCU-SwAD-0092]]\r[Source: [TSP-MPS-MCU-SwAD-0111]]\r[Source: [TSP-MPS-MCU-SwAD-0112]]\r[Source: [TSP-MPS-MCU-SwAD-0113]]\r[Source: [TSP-MPS-MCU-SwAD-0114]]\r[Source: [TSP-MPS-MCU-SwAD-0101]]\r[Source: [TSP-MPS-MCU-SwAD-0102]]\r[Source: [TSP-MPS-MCU-SwAD-0103]]\r[Source: [TSP-MPS-MCU-SwAD-0104]]\r[Source: [TSP-MPS-MCU-SwAD-0105]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0060]"",""[TSP-MPS-MCU-SwAD-0020]"",""[TSP-MPS-MCU-SwAD-0026]"",""[TSP-MPS-MCU-SwAD-0047]"",""[TSP-MPS-MCU-SwAD-0049]"",""[TSP-MPS-MCU-SwAD-0045]"",""[TSP-MPS-MCU-SwAD-0023]"",""[TSP-MPS-MCU-SwAD-0092]"",""[TSP-MPS-MCU-SwAD-0111]"",""[TSP-MPS-MCU-SwAD-0112]"",""[TSP-MPS-MCU-SwAD-0113]"",""[TSP-MPS-MCU-SwAD-0114]"",""[TSP-MPS-MCU-SwAD-0101]"",""[TSP-MPS-MCU-SwAD-0102]"",""[TSP-MPS-MCU-SwAD-0103]"",""[TSP-MPS-MCU-SwAD-0104]"",""[TSP-MPS-MCU-SwAD-0105]""],""input"":""TSP_Conf.ini\rRMS msg from ExDevice_Sim, msg data all is 0xA5, msgID is 101, msgSize is 256.\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rCall MCU2 TSP_Proc_Main_Step_0 function.\rFrom VSN=30, stub MCU2 send 1000 bytes RSSP2, S037 management message.\r\u0007"",""exp_step"":""1, The VSN shall be updated correctly.\r2, Heartbeat message shall be received in MCU1.\r3, MSG_GAPP message received in MPU1_Sim and MPU2_Sim shall have correct value.\r4, RSSP1, RSSP2, S037 APP message received in MPU1_Sim and MPU2_Sim shall have correct value.\r5, MCU2 shall divide RSSP2, S037 management message into small packets and then send to MPU. The max length of each small packet is 1420.\r6, From VSN=30, only one packet of RSSP2, S037 management  message is sent to MPU and length is 1000.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0201"",""description"":""TSP-MPS-MCU-SwITC-0201\rCheck MCU2 will collect the running state of MCU2, including main cycle NO, NR state, timeinfo, and send to SDMS.\r\r[Source: [TSP-MPS-MCU-SwAD-0024]]\r[Source: [TSP-MPS-MCU-SwAD-0060]]\r[Source: [TSP-MPS-MCU-SwAD-0101]]\r[Source: [TSP-MPS-MCU-SwAD-0102]]\r[Source: [TSP-MPS-MCU-SwAD-0103]]\r[Source: [TSP-MPS-MCU-SwAD-0105]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0024]"",""[TSP-MPS-MCU-SwAD-0060]"",""[TSP-MPS-MCU-SwAD-0101]"",""[TSP-MPS-MCU-SwAD-0102]"",""[TSP-MPS-MCU-SwAD-0103]"",""[TSP-MPS-MCU-SwAD-0105]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK. \rCall MCU2 TSP_Proc_Main_Step_0 function.\r\u0007"",""exp_step"":""1, Main cycle message send to SDMS shall have correct value.\r2, Main cycle message received in SDMS_Sim shall have correct value.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0204"",""description"":""TSP-MPS-MCU-SwITC-0204\rCheck MCU2 will send UTC message to MCU1, MPU, GGW and peer MCU2 if the state is Normal.\r\r[Source: [TSP-MPS-MCU-SwAD-0060]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0060]""],""input"":""TSP_Conf.ini\rNR Information message from MPU indicate the NR status is Normal\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim, MPU2_Sim and PeerMCU2_Sim.\rStart the MCU2 INIT_TASK. \rCall MCU2 TSP_Proc_Main_Step_0 function.\r\u0007"",""exp_step"":""1, From vsn=2, MCU2 shall send UTC message to other CPU in slot0.\r2, UTC time message received in MCU1 shall have correct value.\r3, UTC time message received in MPU1_Sim and MPU2_Sim shall have correct value.\r4, UTC message received in PeerMCU2_Sim shall have correct value.\r5, UTC time message received in GGW_Sim shall have correct value.\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0054"",""description"":""TSP-MPS-MCU-SwITC-0054\rTo check MCU2 could receive the NR information message from MPU and update the NR status correctly.\r\r[Source: [TSP-MPS-MCU-SwAD-0087]]\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r[Source: [TSP-MPS-MCU-SwAD-0114]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0087]"",""[TSP-MPS-MCU-SwAD-0045]"",""[TSP-MPS-MCU-SwAD-0114]""],""input"":""TSP_Conf.ini\rNR Information message from MPU indicate the NR status is Normal.\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rSend the NR information message which indicate the NR state is Normal from MPU1_Sim and MPU2_Sim.\rCall MCU2 TSP_Proc_Main_Step_2 function.\r\u0007"",""exp_step"":""1, NR status of MCU2 shall be set to Normal.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0205"",""description"":""TSP-MPS-MCU-SwITC-0205\rTo check Non-Normal MCU2 could receive UTC message from Normal MCU2 and update the time correctly.\r\r[Source: [TSP-MPS-MCU-SwAD-0087]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0087]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim, MPU2_Sim and MCU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rSet MPU1_Sim and MPU2_Sim send NR information message which indicate the NR state is Reserve from vsn=1 to vsn=20.\rSet MPU1_Sim and MPU2_Sim send NR information message which indicate the NR state is Offline from vsn=21 to vsn=40.\rCall MCU2 TSP_Proc_Main_Step_2 function.\r\u0007"",""exp_step"":""1, MCU2 shall receive UTC message received in slot2.\r2, From vsn=1 to vsn=20, UTC message received in Reserve MCU2 shall have correct value and update its NTP time correctly.\r3, From vsn=21 to vsn=40, UTC message received in Offline MCU2 shall have correct value and update its NTP time correctly.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0206"",""description"":""TSP-MPS-MCU-SwITC-0206\rTo check Non-Normal MCU2 shall send UTC message to local MCU1, MPU and GGW.\r\r[Source: [TSP-MPS-MCU-SwAD-0087]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0087]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim, MPU2_Sim and MCU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rSet MPU1_Sim and MPU2_Sim send NR information message which indicate the NR state is Reserve from vsn=0 to vsn=20.\rSet MPU1_Sim and MPU2_Sim send NR information message which indicate the NR state is Offline from vsn=21 to vsn=40.\rCall MCU2 TSP_Proc_Main_Step_2 function.\r\u0007"",""exp_step"":""1, MCU2 shall send UTC message to other CPU in slot2.\rMCU1 shall receive UTC message correctly.\r2, MPU1_Sim, MPU2_Sim and GGW_Sim shall receive UTC message correctly.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0154"",""description"":""TSP-MPS-MCU-SwITC-0154\rTo check MCU2 could receive the configure request message from GGW and send configure file message to GGW if the NR state is Normal.\r\r[Source: [TSP-MPS-MCU-SwAD-0061]]\r[Source: [TSP-MPS-MCU-SwAD-0026]]\r[Source: [TSP-MPS-MCU-SwAD-0097]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0061]"",""[TSP-MPS-MCU-SwAD-0026]"",""[TSP-MPS-MCU-SwAD-0097]""],""input"":""TSP_Conf.ini\rNR Information message from MPU indicate the NR status is Normal.\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim ,MPU2_Sim and GGW_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rSend the NR information message which indicate the NR state is Normal from MPU1_Sim and MPU2_Sim.\rSend Configure request message to MCU2\rCall MCU2 TSP_Proc_Main_Step_3 function.\r\u0007"",""exp_step"":""1, MCU2 received configure request message correctly.\r2,GGW_SIm received the following configure message correctly:APP_CONFIG, APP_GGW,_SNMP(ZC/LC), and RSSP-II/SUBSET037(CCS) .\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0207"",""description"":""TSP-MPS-MCU-SwITC-0207\rTo check MCU2 could receive the MIB message from GGW and send MIB Resend message to MPU if the MIB state is NOK.\r\r[Source: [TSP-MPS-MCU-SwAD-0060]]\r[Source: [TSP-MPS-MCU-SwAD-0026]]\r[Source: [TSP-MPS-MCU-SwAD-0117]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0060]"",""[TSP-MPS-MCU-SwAD-0026]"",""[TSP-MPS-MCU-SwAD-0117]""],""input"":""TSP_Conf.ini\rNR Information message from MPU indicate the NR status is Normal.\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim ,MPU2_Sim and GGW_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rSend the NR information message which indicate the NR state is Normal from MPU1_Sim and MPU2_Sim.\rSimulate GGW_Sim to send MIB state NOK message to MCU2 from VSN = 10.\rCall MCU2 TSP_Proc_Main_Step_0 function.\rCall MCU2 TSP_Proc_Main_Step_3 function.\r\u0007"",""exp_step"":""1, MCU2 received MIB state NOK message correctly.\r2, From VSN = 11, MCU2 shall send MIB Resend message to MPU1_Sim and MPU2_Sim.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0155"",""description"":""TSP-MPS-MCU-SwITC-0155\rTo check MCU2 could receive the version request message from SDMS and send version message to SDMS.\rTo check MCU2 could receive the RMS info request message from SDMS and send RMS info message to SDMS.\rTo check MCU2 shall not send RMS info message to SDMS if the message length is 0.\r\r[Source: [TSP-MPS-MCU-SwAD-0061]]\r[Source: [TSP-MPS-MCU-SwAD-0024]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0061]"",""[TSP-MPS-MCU-SwAD-0024]""],""input"":""TSP_Conf.ini\r\r\u0007"",""exec_step"":""1.  Start the Testbench which will run MPU1_Sim ,MPU2_Sim and SDMS_Sim.\r2.  Start the MCU2 INIT_TASK.\r3.  Start the MCU1 which will send the SLOT message to MCU2.\r4.  Send version request message to MCU2\rSend RMS info request message to MCU2\rCall MCU2 TSP_Proc_Main_Step_3 function.\rPrint the body of RMS info message received in SDMS.\rStub to set the length of RMS info message to be 0 from VSN 10-20.\r\u0007"",""exp_step"":""1, MCU2 received version request message correctly.\r2,SDMS_SIm received version file message correctly, include data version, software version, board version, CPLD version, BSP version and DRV version.\r3,MCU2 received RMS info request message correctly.\r4,SDMS_SIm received RMS info message correctly.\r5, SDMS shall not receive RMS info message from VSN 10-20.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0156"",""description"":""TSP-MPS-MCU-SwITC-0156\rTo check MCU2 could read RSSP2\u0026037 syn data message and do syn process if the NR state is not Normal.\r\r[Source: [TSP-MPS-MCU-SwAD-0109]]\r[Source: [TSP-MPS-MCU-SwAD-0025]]\r[Source: [TSP-MPS-MCU-SwAD-0112]]\r[Source: [TSP-MPS-MCU-SwAD-0113]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP/EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0109]"",""[TSP-MPS-MCU-SwAD-0025]"",""[TSP-MPS-MCU-SwAD-0112]"",""[TSP-MPS-MCU-SwAD-0113]""],""input"":""TSP_Conf.ini\rNR Information message from MPU indicate the NR status is Standby.\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim ,MPU2_Sim and PeerMCU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rSend the NR information message which indicate the NR state is standby from MPU1_Sim and MPU2_Sim.\rReceive RSSP2 and 037 Syn state message from peer MCU2\rCall MCU2 TSP_Proc_Main_Step_4 function.\rFrom VSN=21, stub MCU2 receive big packet (1872 bytes) 037 Syn state message from peer MCU2.\rFrom VSN=30 to VSN = 35, stub peer MCU2 send divided 037 Syn state message with wrong sequence.\rFrom VSN=40 to VSN = 45, stub MCU2 receive divided 037 Syn state message and lost the first one packet.\rFrom VSN=50 to VSN = 55, stub MCU2 receive divided 037 Syn state message with repeat pktSN. (pktSN: 0,0)\rFrom VSN=60 to VSN = 65, stub MCU2 receive redundant divided 037 Syn state message. (pktSN:0, 0, 1)\rFrom VSN=70 to VSN = 75, stub MCU2 receive redundant divided 037 Syn state message. (pktSN:0, 1, 1)\r\u0007"",""exp_step"":""1,  MCU2 received RSSP2 and 037 Syn state message correctly.\r2, MCU2 executed RSSP2 and 037 synchronization process correctly.\r3, MCU2 receives 1 packet of 037 Syn state message:\rpktTpye=0, pktSN=0,pktNum=1,pktlen=624\r4, From VSN=21 to VSN=29, MCU2 receives 2 packets of 037 Syn state message:\rpktTpye=0, pktSN=0,pktNum=2,pktlen=1420\rpktTpye=0, pktSN=1,pktNum=2, pktlen=452\rMCU2 shall pack the received small packets as one big packet successfully and the size of the packetis 1872.\rThe packed 037 Syn state message is the same as sent by the peer MCU2(validated by CRC32).\r5, From VSN=30 to VSN=35, MCU2 receives 2 packets of 037 Syn state message:\rpktTpye=0, pktSN=0,pktNum=2,pktlen=1420\rpktTpye=0, pktSN=1,pktNum=2, pktlen=452\rMCU2 shall pack the received small packets as one big packet successfully and the size of the packetis 1872.\r6, From VSN=40 to VSN=45, MCU2 receives one packet of 037 Syn state message:\rpktTpye=0, pktSN=1,pktNum=2, pktlen=452\rMCU2 fails to pack the received 037 Syn state message.\r7, From VSN=50 to VSN=55, MCU2 receives 2 packets 037 Syn state message:\rpktTpye=0, pktSN=0,pktNum=2,pktlen=1420\rpktTpye=0, pktSN=0,pktNum=2, pktlen=452\rMCU2 fails to pack the received 037 Syn state message.\r8, From VSN=60 to VSN=65, MCU2 receives 2 packets of 037 Syn state message:\rpktTpye=0, pktSN=0,pktNum=2,pktlen=1420\rpktTpye=0, pktSN=0,pktNum=2, pktlen=1420\rMCU2 fails to pack the received 037 Syn state message.\r9, From VSN=70 to VSN=75, MCU2 receives 2 packets of 037 Syn state message:\rpktTpye=0, pktSN=0,pktNum=2,pktlen=1420\rpktTpye=0, pktSN=1,pktNum=2, pktlen=452\rMCU2 shall pack the received small packets as one big packet successfully and the size of the packetis 1872.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0039"",""description"":""TSP-MPS-MCU-SwITC-0039\rTo check when MCU2 is Normal this cycle and Reserve or Normal last cycle, the interval time between step0 and step5 is more than 130ms, MCU2 could send RMS data to external system correctly if the application type is ZC/LC.\r\r[Source: [TSP-MPS-MCU-SwAD-0062]]\r[Source: [TSP-MPS-MCU-SwAD-0020]]\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r[Source: [TSP-MPS-MCU-SwAD-0114]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0062]"",""[TSP-MPS-MCU-SwAD-0020]"",""[TSP-MPS-MCU-SwAD-0045]"",""[TSP-MPS-MCU-SwAD-0114]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO message from MPU indicate the NR status is Normal.\rGAPP msg from MPU2_Sim and MPU1_Sim:\rData: the first 4 bytes are loophour and the following is all 0x5A\r msgSize: 260.\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rSend the GAPP data from MPU1_Sim and MPU2_Sim to MCU2.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Normal from VSN=0 to VSN=10.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Reserve from VSN=11 to VSN=20.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Normal from VSN=21 to VSN=30.\rRecord the interval time between step0 and step5.\rCall MCU2 TSP_Proc_Main_Step_5 function.\r\u0007"",""exp_step"":""1, The interval time between step0 and step5 is more than 130ms.\r2, From VSN=2 to 10, the RMS data received in ExDevice_Sim shall have correct value.\r3, From VSN=11 to 20, there is no RMS data received in ExDevice_Sim.\r4, From VSN=21 to 30, the RMS data received in ExDevice_Sim shall have correct value.\r5, The NTPDataTime in GAPP head is UTC time.\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0157"",""description"":""TSP-MPS-MCU-SwITC-0157\rTo check when MCU2 is Normal this cycle and Reserve or Normal last cycle, the interval time between step0 and step7 is more than 130ms, MCU2 could send RMS data to external system correctly if the application type is CCS.\r\r[Source: [TSP-MPS-MCU-SwAD-0062]]\r[Source: [TSP-MPS-MCU-SwAD-0020]]\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r[Source: [TSP-MPS-MCU-SwAD-0114]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0062]"",""[TSP-MPS-MCU-SwAD-0020]"",""[TSP-MPS-MCU-SwAD-0045]"",""[TSP-MPS-MCU-SwAD-0114]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO message from MPU indicate the NR status is Normal.\rGAPP msg from MPU2_Sim and MPU1_Sim\rData: the first 4 bytes are loophour and the following is all 0x5A\r msgSize: 260.\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rSend the GAPP data from MPU1_Sim and MPU2_Sim to MCU2.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Normal from VSN=0 to VSN=10.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Reserve from VSN=11 to VSN=20.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Normal from VSN=21 to VSN=30.\rRecord the interval time between step0 and step7.\rCall MCU2 TSP_Proc_Main_Step_5 function.\r\u0007"",""exp_step"":""1, The interval time between step0 and step7 is more than 130ms.\r2, From VSN=2 to 10, the RMS data received in ExDevice_Sim shall have correct value.\r3, From VSN=11 to 20, there is no RMS data received in ExDevice_Sim.\r4, From VSN=21 to 30, the RMS data received in ExDevice_Sim shall have correct value.\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0164"",""description"":""TSP-MPS-MCU-SwITC-0164\rTo check when MCU2 is Normal this cycle and Reserve or Normal last cycle, the interval time between step0 and step5 is more than 130ms, MCU2 could send RSSP1 data to external system correctly if the application type is ZC/LC.\r\r[Source: [TSP-MPS-MCU-SwAD-0062]]\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r[Source: [TSP-MPS-MCU-SwAD-0111]]\r[Source: [TSP-MPS-MCU-SwAD-0114]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0062]"",""[TSP-MPS-MCU-SwAD-0045]"",""[TSP-MPS-MCU-SwAD-0111]"",""[TSP-MPS-MCU-SwAD-0114]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO message from MPU indicate the NR status is Normal.\rRSSP1 msg from MPU2_Sim and MPU1_Sim:\rData: the first 4 bytes are loophour and the following is all 0x3C, msgSize: 999.\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rSend the RSSP1 data from MPU1_Sim and MPU2_Sim to MCU2.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Normal from VSN=0 to VSN=10.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Reserve from VSN=11 to VSN=20.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Normal from VSN=21 to VSN=30.\rRecord the interval time between step0 and step5.\rCall MCU2 TSP_Proc_Main_Step_5 function.\r\u0007"",""exp_step"":""1, The interval time between step0 and step5 is more than 130ms.\r2, From VSN=2 to 10, the RSSP1 data received in TCC_Sim shall have correct value.\r3, From VSN=11 to 20, the RSSP1 data received in TCC_Sim shall have correct value.\r4, From VSN=21 to 30, the RSSP1 data received in TCC_Sim shall have correct value.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0158"",""description"":""TSP-MPS-MCU-SwITC-0158\rTo check when MCU2 is Normal this cycle and Reserve or Normal last cycle, the interval time between step0 and step7 is more than 130ms, MCU2 could send RSSP1 data to external system correctly if the application type is CCS.\r\r[Source: [TSP-MPS-MCU-SwAD-0062]]\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r[Source: [TSP-MPS-MCU-SwAD-0111]]\r[Source: [TSP-MPS-MCU-SwAD-0114]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0062]"",""[TSP-MPS-MCU-SwAD-0045]"",""[TSP-MPS-MCU-SwAD-0111]"",""[TSP-MPS-MCU-SwAD-0114]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO message from MPU indicate the NR status is Normal.\rRSSP1 msg from MPU2_Sim and MPU1_Sim:\rthe data is all 0x3C,, msgSize is 1022.\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rSend the RSSP1 data from MPU1_Sim and MPU2_Sim to MCU2.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Normal from VSN=0 to VSN=10.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Reserve from VSN=11 to VSN=20.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Normal from VSN=21 to VSN=30.\rRecord the interval time between step0 and step7.\rCall MCU2 TSP_Proc_Main_Step_5 function.\r\u0007"",""exp_step"":""1, The interval time between step0 and step7 is more than 130ms.\r2, From VSN=2 to 10, the RSSP1 data received in TCC_Sim shall have correct value.\r3, From VSN=11 to 20, the RSSP1 data received in TCC_Sim shall have correct value.\r4, From VSN=21 to 30, the RSSP1 data received in TCC_Sim shall have correct value.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0159"",""description"":""TSP-MPS-MCU-SwITC-0159\rTo check when MCU2 is Normal this cycle and Reserve or Normal last cycle, the interval time between step0 and step7 is more than 130ms, MCU2 could send RSSP2 data to external system correctly if the application type is CCS.\r\r[Source: [TSP-MPS-MCU-SwAD-0062]]\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r[Source: [TSP-MPS-MCU-SwAD-0112]]\r[Source: [TSP-MPS-MCU-SwAD-0114]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0062]"",""[TSP-MPS-MCU-SwAD-0045]"",""[TSP-MPS-MCU-SwAD-0112]"",""[TSP-MPS-MCU-SwAD-0114]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO message from MPU indicate the NR status is Normal.\rRSSP2 msg from MPU2_Sim and MPU1_Sim:\rthe data is all 0x5A,, msgSize is 1000.\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim,MPU2_Sim,GGW_Sim and CTC_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rSend the RSSP2 data from MPU1_Sim and MPU2_Sim to MCU2.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Normal from VSN=0 to VSN=10.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Reserve from VSN=11 to VSN=20.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Normal from VSN=21 to VSN=30.Record the interval time between step0 and step7.\rCall MCU2 TSP_Proc_Main_Step_5 function.\r\u0007"",""exp_step"":""1, The interval time between step0 and step7 is more than 130ms.\r2, From VSN=2 to 10,the RSSP2 data received in CTC_Sim shall have correct value.\r3, From VSN=11 to 20, the RSSP2 data received in CTC_Sim shall have correct value.\r4, From VSN=21 to 30, the RSSP2 data received in CTC_Sim shall have correct value.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0160"",""description"":""TSP-MPS-MCU-SwITC-0160\rTo check when MCU2 is Normal this cycle and Reserve or Normal last cycle, the interval time between step0 and step7 is more than 130ms, MCU2 could send 037 data to external system correctly if the application type is CCS.\r\r[Source: [TSP-MPS-MCU-SwAD-0062]]\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r[Source: [TSP-MPS-MCU-SwAD-0113]]\r[Source: [TSP-MPS-MCU-SwAD-0114]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0062]"",""[TSP-MPS-MCU-SwAD-0045]"",""[TSP-MPS-MCU-SwAD-0113]"",""[TSP-MPS-MCU-SwAD-0114]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO message from MPU indicate the NR status is Normal.\r037 msg from MPU2_Sim and MPU1_Sim:\rthe data is all 0x5A,, msgSize is 1000.\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim,MPU2_Sim,GGW_Sim and EVC_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rSend the 037 data from MPU1_Sim and MPU2_Sim to MCU2.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Normal from VSN=0 to VSN=10.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Reserve from VSN=11 to VSN=20.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Normal from VSN=21 to VSN=30.Record the interval time between step0 and step7.\rCall MCU2 TSP_Proc_Main_Step_5 function.\r\u0007"",""exp_step"":""1, The interval time between step0 and step7 is not less than 130ms.\r2, From VSN=2 to 10, the 037 data received in EVC_Sim shall have correct value.\r3, From VSN=11 to 20, the 037 data received in EVC_Sim shall have correct value.\r4, From VSN=21 to 30, the 037 data received in EVC_Sim shall have correct value.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0193"",""description"":""TSP-MPS-MCU-SwITC-0193\rTo check when MCU2 is Normal this cycle and Reserve or Normal last cycle, the interval time between step0 and step5 is more than 130ms, MCU2 could send RAW data to external system correctly if the application type is ZC/LC.\r\r[Source: [TSP-MPS-MCU-SwAD-0062]]\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r[Source: [TSP-MPS-MCU-SwAD-0117]]\r[Source: [TSP-MPS-MCU-SwAD-0114]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0062]"",""[TSP-MPS-MCU-SwAD-0045]"",""[TSP-MPS-MCU-SwAD-0117]"",""[TSP-MPS-MCU-SwAD-0114]""],""input"":""MPS_Conf.bin\rMSG_NR_INFO message from MPU indicate the NR status is Normal.\rRAW msg from MPU2_Sim and MPU1_Sim:\rData: the first 4 bytes are loophour and the following is all 0x50, msgSize: 1204.\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rSend the RAW data from MPU1_Sim and MPU2_Sim to MCU2.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Normal from VSN=0 to VSN=10.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Reserve from VSN=11 to VSN=20.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Normal from VSN=21 to VSN=30.\rRecord the interval time between step0 and step5.\rCall MCU2 TSP_Proc_Main_Step_5 function.\r\u0007"",""exp_step"":""1, The interval time between step0 and step5 is more than 130ms.\r2, From VSN=2 to 10, the RAW data received in RAW_Sim shall have correct value.\r3, From VSN=11 to 20, the RAW data received in RAW_Sim shall have correct value.\r4, From VSN=21 to 30, the RAW data received in RAW_Sim shall have correct value.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0161"",""description"":""TSP-MPS-MCU-SwITC-0161\rTo check when MCU2 send RSSP2 and 037 synchronization data message to peer MCU2 if the application type is CCS and NR state is Normal.\r\r[Source: [TSP-MPS-MCU-SwAD-0062]]\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r[Source: [TSP-MPS-MCU-SwAD-0112]]\r[Source: [TSP-MPS-MCU-SwAD-0113]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0062]"",""[TSP-MPS-MCU-SwAD-0045]"",""[TSP-MPS-MCU-SwAD-0112]"",""[TSP-MPS-MCU-SwAD-0113]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO message from MPU indicate the NR status is Normal.\rRSSP2 and 037 msg from MPU2_Sim and MPU1_Sim:\r\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim, MPU2_Sim, GGW_Sim and CTC_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rAlways send MSG_NR_INFO message indicates the NR state is Normal from MPU1_Sim and MPU2_Sim.\rRecord the interval time between step0 and step7.\rSend RSSP2 and 037 Syn state message to peer MCU2.\rCall MCU2 TSP_Proc_Main_Step_6 function.\rFrom VSN=20, stub MCU2 send big packet (1872 bytes) 037 Syn state message to peer MCU2.\r\u0007"",""exp_step"":""1, The interval time between step0 and step7 is not less than 130ms.\r2, RSSP2 and 037 Synchronization data send successfully.\r3,Before VSN=20, 037 Synchronization data is only one packet:\rpktTpye=0, pktSN=0,pktNum=1,ptklen=624\r4,From VSN=20, 037 Synchronization data is divided into 2 small packets.\rFirst packet: pktTpye=0, pktSN=0,pktNum=2,ptklen=1420\rSecond packet: pktTpye=0, pktSN=1,pktNum=2, ptklen= 452\r\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0071"",""description"":""TSP-MPS-MCU-SwITC-0071\rTo check when MCU2 is not Normal this cycle or Reserve or  Normal last cycle, MCU2 could not send RMS, RAW and RSSP1 data to external system if the application type is ZC/LC.\r\r[Source: [TSP-MPS-MCU-SwAD-0062]]\r[Source: [TSP-MPS-MCU-SwAD-0020]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0062]"",""[TSP-MPS-MCU-SwAD-0020]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO message from MPU indicate the NR status is Normal.\rGAPP msg from MPU2_Sim and MPU1_Sim, the data is all 0x5A, msgID is 1, msgSize is 256.\rRSSP1 msg from MPU2_Sim and MPU1_Sim, the data is all 0x3C, msgSize is 1022.\rRAW msg from MPU2_Sim and MPU1_Sim, the data is all 0x50, msgSize is 1200.\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rSend the GAPP, RAW and RSSP1 data from MPU1_Sim and MPU2_Sim to MCU2.\rLet MPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Reverse from VSN=10 to VSN=20.\rLet MPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Normal from VSN=21 to VSN=30.\rLet MPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Offline from VSN=31.\rCall MCU2 TSP_Proc_Main_Step_5 function.\r\u0007"",""exp_step"":""1, From VSN=10 to VSN=20, RMS , RAW and RSSP1data shall not be sent to external device.\r2. From VSN=31, RMS , RAW and RSSP1data shall not be sent to external device.\r.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0076"",""description"":""TSP-MPS-MCU-SwITC-0076\rTo check when MCU2 is not normal this cycle or Reserve last cycle, MCU2 could not send RMS, RSSP1, RSSP2 and 037 data to external system if the application type is CCS.\r\r[Source: [TSP-MPS-MCU-SwAD-0062]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0062]""],""input"":""TSP_Conf.ini\rMSG_NR_INFO message from MPU indicate the NR status is Reserve.\rGAPP msg from MPU2_Sim and MPU1_Sim, the data is all 0x5A, msgID is 1, msgSize is 256.\rRSSP1 msg from MPU2_Sim and MPU1_Sim, the data is all 0x5A, msgSize is 500.\rRSSP2 msg from MPU2_Sim and MPU1_Sim, the data is all 0x5A, msgSize is 500.\r037 msg from MPU2_Sim and MPU1_Sim, the data is all 0x5A,, msgSize is 256.\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rMPU1_Sim and MPU2_Sim send MSG_NR_INFO message indicates the NR state is Reverse from.\rSend the GAPP, RSSP1, RSSP2 and 037 data from MPU1_Sim and MPU2_Sim to MCU2.\rCall MCU2 TSP_Proc_Main_Step_5 function.\r\u0007"",""exp_step"":""1, GAPP, RSSP1, RSSP2 and 037 data shall not be sent but be refreshed.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0040"",""description"":""TSP-MPS-MCU-SwITC-0040\rCheck the MCU2 could dispose the CBIT result and generate the CBIT check word correctly.\r\r[Source: [TSP-MPS-MCU-SwAD-0063]]\r[Source: [TSP-MPS-MCU-SwAD-0049]]\r[Source: [TSP-MPS-MCU-SwAD-0051]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r[Source: [TSP-MPS-MCU-SwAD-0045]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP/EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0063]"",""[TSP-MPS-MCU-SwAD-0049]"",""[TSP-MPS-MCU-SwAD-0051]"",""[TSP-MPS-MCU-SwAD-0022]"",""[TSP-MPS-MCU-SwAD-0045]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rWhen VSN = 50, modify the CBIT return XOR result value equal with 0x12345678.\r\u0007"",""exp_step"":""1, RAM test shall be less than 28ms every cycle.\r2, Every 32 cycle, the CBIT result shall be different.\r3, When 1\u003cVSN\u003c50, the MCU2_CHKW_BIT shall be equal with 4FA78CDBH.\r4, When VSN = 50, the MCU2_CHKW_BIT shall not be equal with 4FA78CDBH.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0055"",""description"":""TSP-MPS-MCU-SwITC-0055\rCheck the MCU2 could process the MPU1/2’s output data and generate JTC checkword correctly.\r\r[Source: [TSP-MPS-MCU-SwAD-0063]]\r[Source: [TSP-MPS-MCU-SwAD-0027]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r[Source: [TSP-MPS-MCU-SwAD-0023]]\r[Source: [TSP-MPS-MCU-SwAD-0092]]\r[Source: [TSP-MPS-MCU-SwAD-0114]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0063]"",""[TSP-MPS-MCU-SwAD-0027]"",""[TSP-MPS-MCU-SwAD-0022]"",""[TSP-MPS-MCU-SwAD-0023]"",""[TSP-MPS-MCU-SwAD-0092]"",""[TSP-MPS-MCU-SwAD-0114]""],""input"":""TSP_Conf.ini\rRMS msg from MPU2_Sim and MPU1_Sim, the data is all 0x5A, msgID is 1, msgSize is 256.\rRAW msg from MPU2_Sim and MPU1_Sim, the data is all 0x50,  msgSize is 1200.\rRSSP1 msg from MPU2_Sim and MPU1_Sim, the data is all 0x50,  msgSize is 1022\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rSend APP data, RAW and RSSP1 message from MPU1_Sim and MPU2_Sim, where the output data is equal.\rCall MCU2 TSP_Proc_Main_Step_10 function.\r\u0007"",""exp_step"":""1, The MCU2_CHKW_JTC_CH1 shall be 0xAF206E62, MCU2_CHKW_JTC_CH2 shall be 0x1EF2BFE0.\r2, APP data, RAW and RSSP1 message received shall have right value.\r3, APP data, RAW and RSSP1 message is divided correctly.\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0056"",""description"":""TSP-MPS-MCU-SwITC-0056\rCheck the MCU2 could send the checkword to MCU1 correctly.\r\r[Source: [TSP-MPS-MCU-SwAD-0063]]\r[Source: [TSP-MPS-MCU-SwAD-0023]]\r[Source: [TSP-MPS-MCU-SwAD-0022]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0063]"",""[TSP-MPS-MCU-SwAD-0023]"",""[TSP-MPS-MCU-SwAD-0022]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rSend GAPP data from MPU1_Sim and MPU2_Sim, where the output data is equal.\rCall TSP_Proc_Main_Step_10.\r\u0007"",""exp_step"":""1, The return of LCOM write CHKW shall be GM_TRUE.\r2, MCU1 receive MCU2 Check word successfully. \r3, the check word received in MCU1 shall have be as follows:\rMCU2_CHKW_BIT  4FA78CDBH\rMCU2_CHKW_MAIN_CYCLE_INTERVAL  0x52000  MCU2_CHKW_TASK_CHECK  57BED039H\rMCU2_CHKW_JTC_CH1  0xAF206E62\rMCU2_CHKW_JTC_CH2  0x1EF2BFE0.\r\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0089"",""description"":""TSP-MPS-MCU-SwITC-0089\rCheck the MCU2 CBIT could suspend the task which is being test by CBIT, and release it when test is over.\r\r[Source: [TSP-MPS-MCU-SwAD-0051]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0051]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rCall taskInfoGet routine to check the task status of MainProc, MUDP_Rcv and SIO_Rx_Task when CBIT testing it.\r\u0007"",""exp_step"":""1, MainProc, MUDP_Rcv and SIO_Rx_Task will be suspended when CBIT testing it, resume when testing is over.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0090"",""description"":""TSP-MPS-MCU-SwITC-0090\rCheck the MCU2 CBIT could lock the interrupt when CBIT test Data Segment, and unlock it when test is over.\r\r[Source: [TSP-MPS-MCU-SwAD-0051]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0051]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rStart InnerTimer for 2ms.\rCheck the times InnerTimer is triggered in one main cycle, when the times InnerTimer is triggered less than 168, the interrupt is locked when CBIT test Data Segment.\r\u0007"",""exp_step"":""1, the times InnerTimer is triggered every cycle shall less than 168.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0312"",""description"":""TSP-MPS-MCU-SwITC-0312\rCheck MCU2 shall provide interface to transmit information to CBIT module for test.\r\r[Source: [TSP-MPS-MCU-SwAD-0051]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0051]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rPrint the address of TEXT, DATA and BSS segment In vxWorks.\rStart the MCU2 INIT_TASK.\rMCU2 transmits the following information to CBIT module:\rName of error log disk\rName of CBIT error log\rTest speed of TEXT segment, DATA segment and task stack(0x00002000, 0x00000800, 0x00000800)\rPriority of CBIT task(65)\rTask number(3)\rTask name to be tested(MainProc, MUDP_Rcv, SIO_Rx_Task)\rSegment number(TEXT 1, DATA 2)\rAddress and size of TEXT, DATA and BSS segment, MD5 value of TEXT segment\rPrint the information mentioned above in CBIT module.\r\u0007"",""exp_step"":""In CBIT module:\r1, The name of error log disk is /tffs1/.\r2, The name of error log is CBIT_Err.log.\r3, The test speed of TEXT segment, DATA segment and task stack is 0x00002000, 0x00000800 and 0x00000800.\r4, The priority of CBIT task is 65.\r5, The task number is 3.\r6, The tasks to be tested are MainProc, MUDP_Rcv and SIO_Rx_Task).\r7, The number of TEXT segment is 1. The number of DATA segment is 2.\r6, The address and size of TEXT, DATA and BSS segment is the same with the result printed in vxWorks.\r7, The MD5 of TEXT segment is the same with the result printed in vxWorks.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0092"",""description"":""TSP-MPS-MCU-SwITC-0092\rCheck the MCU2 CBIT will finish one test in 10h.\r\r[Source: [TSP-MPS-MCU-SwAD-0051]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0051]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rLog the time interval between two RAM_Stack test start time, and the time interval between two RAM_Data test start time, the longer time interval is the CBIT running time.\r\u0007"",""exp_step"":""1, One full testing of CBIT shall finish in 10h.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0301"",""description"":""TSP-MPS-MCU-SwITC-0301\rIf the application type is ZC/LC:\rCheck the MCU2 TSP_Proc_Init_Pre will be run complete in 270s.\rCheck the MCU2 slot0 will be run complete in 28ms.\rCheck the MCU2 slot2 will be run complete in 28ms.\rCheck the MCU2 slot3 will be run complete in 28ms.\rCheck the MCU2 slot5 will be run complete in 28ms.\rCheck the MCU2 slot9 will be run complete in 28ms.\rCheck the MCU2 slot10 will be run complete in 40ms.\rCheck the MCU2 slot11 will be run complete in 16ms.\r[Source: [TSP-MPS-MCU-SwAD-0035]]\r[Source: [TSP-MPS-MCU-SwAD-0060]]\r[Source: [TSP-MPS-MCU-SwAD-0061]]\r[Source: [TSP-MPS-MCU-SwAD-0087]]\r[Source: [TSP-MPS-MCU-SwAD-0062]]\r[Source: [TSP-MPS-MCU-SwAD-0063]]\r[Source: [TSP-MPS-MCU-SwAD-0034]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0035]"",""[TSP-MPS-MCU-SwAD-0060]"",""[TSP-MPS-MCU-SwAD-0061]"",""[TSP-MPS-MCU-SwAD-0087]"",""[TSP-MPS-MCU-SwAD-0062]"",""[TSP-MPS-MCU-SwAD-0063]"",""[TSP-MPS-MCU-SwAD-0034]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rCheck the time MCU2 slots running.\r\u0007"",""exp_step"":""1, the MCU2 TSP_Proc_Init_Pre shall be run complete in 270s.\rMCU2 slot0 shall be run complete in 28ms.\r MCU2 slot2 shall be run complete in 28ms.\r MCU2 slot3 shall be run complete in 28ms.\r MCU2 slot5 shall be run complete in 28ms.\rMCU2 slot9 shall be run complete in 28ms.\rMCU2 slot10 shall be run complete in 40ms.\rMCU2 slot11 shall be run complete in 16ms.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0162"",""description"":""TSP-MPS-MCU-SwITC-0162\rIf the application type is CCS:\rCheck the MCU2 TSP_Proc_Init_Pre will be run complete in 270s.\rCheck the MCU2 slot0 will be run complete in 56ms.\rCheck the MCU2 slot2 will be run complete in 20ms.\rCheck the MCU2 slot3 will be run complete in 36ms.\rCheck the MCU2 slot4 will be run complete in 54ms.\rCheck the MCU2 slot7 will be run complete in 24ms.\rCheck the MCU2 slot9 will be run complete in 56ms.\rCheck the MCU2 slot10 will be run complete in 40ms.\rCheck the MCU2 slot11 will be run complete in 16ms.\r\r[Source: [TSP-MPS-MCU-SwAD-0035]]\r[Source: [TSP-MPS-MCU-SwAD-0060]]\r[Source: [TSP-MPS-MCU-SwAD-0061]]\r[Source: [TSP-MPS-MCU-SwAD-0087]]\r[Source: [TSP-MPS-MCU-SwAD-0062]]\r[Source: [TSP-MPS-MCU-SwAD-0063]]\r[Source: [TSP-MPS-MCU-SwAD-0034]]\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0035]"",""[TSP-MPS-MCU-SwAD-0060]"",""[TSP-MPS-MCU-SwAD-0061]"",""[TSP-MPS-MCU-SwAD-0087]"",""[TSP-MPS-MCU-SwAD-0062]"",""[TSP-MPS-MCU-SwAD-0063]"",""[TSP-MPS-MCU-SwAD-0034]""],""input"":""TSP_Conf.ini\r\u0007"",""exec_step"":""Start the Testbench which will run MPU1_Sim and MPU2_Sim.\rStart the MCU2 INIT_TASK.\rStart the MCU1 which will send the SLOT message to MCU2.\rCheck the time MCU2 slots running.\r\u0007"",""exp_step"":""1, the MCU2 TSP_Proc_Init_Pre shall be run complete in 270s.\rMCU2 slot0、slot1、slot5、slot8 shall be run complete in 56ms.\r MCU2 slot2 shall be run complete in 20ms.\r MCU2 slot3 shall be run complete in 36ms.\r MCU2 slot4 shall be run complete in 54ms.\rMCU2 slot6 shall be run complete in 30ms.\rMCU2 slot7 shall be run complete in 24ms.\rMCU2 slot9 shall be run complete in 56ms.\rMCU2 slot10 shall be run complete in 40ms.\rMCU2 slot11 shall be run complete in 16ms.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0163"",""description"":""TSP-MPS-MCU-SwITC-0163\rCheck MCU2 Send Dump/Print/Errorlist/Alarm message to SDMS correctly.\rCheck MCU2 shall not send message to SDMS if the message length is o.\r[Source: [TSP-MPS-MCU-SwAD-0096]]\r[Source: [TSP-MPS-MCU-SwAD-0024]]\r[Source: [TSP-MPS-MCU-SwAD-0106]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0096]"",""[TSP-MPS-MCU-SwAD-0024]"",""[TSP-MPS-MCU-SwAD-0106]""],""input"":""TSP_Conf.ini\r\r\u0007"",""exec_step"":""Start the TestBench and run MCU2\rStart the MCU2 INIT_TASK.\rCall MCU2 TSP_Proc_Main_Step_11 function.\rPrint the body of Dump message.\rStub to set the length of message sent to SDMS to be 0 from VSN 10-20.\r\u0007"",""exp_step"":""1, MCU2 send Dump/Print/Errorlist/ Alarm message successfully\u000b2,SDMS received Dump/Print/Errorlist/Alarm message successfully\r3, SDMS shall not receive Debug/Dump/Print message from VSN 10-20\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0181"",""description"":""TSP-MPS-MCU-SwITC-0181\rCheck MCU2 shall add 2048 bytes tsp print message in each cycle, if the message length is more 2048, MCU1 shall not send to SDMS.\r[Source: [TSP-MPS-MCU-SwAD-0059]]\r[Source: [TSP-MPS-MCU-SwAD-0106]]\r\r[Safety:Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EP/BA\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0059]"",""[TSP-MPS-MCU-SwAD-0106]""],""input"":""TSP_Conf.ini\rMessage 1: length=2048\rMessage 2: length =2049\r\u0007"",""exec_step"":""Start the TestBench and run MCU1\rStart the MCU1 INIT_TASK.\rAdd message 1 to TSP print message buffer(VSN=10)\rCall MCU2 TSP_Proc_Main_Step_11 function.\rAdd message 2 to TSP print message buffer(VSN=11)\rCall MCU2 TSP_Proc_Main_Step_11 function.\r\u0007"",""exp_step"":""MCU2 add message 1 successfully.\rSDMS received message1 which is same with the send one\rMCU2 add message 2 failed.\rSDMS can’t receive message2.\r\u0007""},{""tag"":""TSP-MPS-MCU-SwITC-0310"",""description"":""TSP-MPS-MCU-SwITC-0310\rTo verify that MCU2 can work normally when the memory pool is written by old message and the message number can be more than 1.\r[Source: [TSP-MPS-MCU-SwAD-0101]]\r[Safety: Yes]\r[End]\r\u0007"",""test_item"":"""",""test_method"":""EG\r\u0007"",""pre_condition"":"""",""result"":"""",""comment"":"""",""test_steps"":[{""num"":0,""actions"":"""",""expected_result"":"""",""indata"":"""",""test_step"":""""}],""source"":[""[TSP-MPS-MCU-SwAD-0101]""],""input"":""TSP_COM_MSG_TYPE_MPU1_GAPP message from MPU1\r\u0007"",""exec_step"":""Initialization MCU software；\rUse MPU1 Simulator to send TSP_COM_MSG_TYPE_MPU1_GAPP message to MCU2 at cycle 100, the GAPP message (SN is 11) is sent five times, the first vsn is 99, the second is 98, the third is 100, the next two is 97,96.\r\u0007"",""exp_step"":""1,MCU2 software start successfully;\r2,MCU2 receive five TSP_COM_MSG_TYPE_MPU1_GAPP message successfully when the main cycle is 100;\r3,MCU2 only use the message that vsn is 100  and discard the message with other vsn.\r4,The GAPP message can be sent to external system successfully.\r\u0007""}]";
 Context.Response.ContentType = "text/json";
            Context.Response.Write(json);
            Context.Response.End();
        }


        [WebMethod(Description = "inputword")]
        public void InputWord(string url)
        {
            Random rd = new Random();
            int rdd = rd.Next(10, 100);
            string filename = "rs" + DateTime.Now.ToString("yyyy-MM-dd") + rdd.ToString() + ".doc";
            string LocalPath = null;
            //string pdfurl=null;
            try
            {
                int poseuqlurl = url.IndexOf('=');
                int possnap;
                string url1;
                string pdfname;
                url1 = url.Substring(poseuqlurl + 1, url.Length - poseuqlurl - 1);
                possnap = url1.LastIndexOf('/');
                pdfname = url1.Substring(possnap + 1, url1.Length - possnap - 1);
                filename = pdfname;
                /*int poseuqlurl = url.IndexOf('=');
                string url1;
                url1 = url.Substring(poseuqlurl + 1, url.Length - poseuqlurl - 1);
                //pdfurl;*/
                Uri u = new Uri(url1);
                //Uri u = new Uri(url);
                //filename = "123.doc";
                //string time1 = DateTime.Now.ToString();
                //filename = DateTime.Now.ToString() + ".doc";
                LocalPath = ConfigurationManager.AppSettings["path"].ToString() + filename;
             
                if (!File.Exists(@LocalPath))
                {
                    //不存在
                    HttpWebRequest mRequest = (HttpWebRequest)WebRequest.Create(u);
                    mRequest.Method = "GET";
                    mRequest.ContentType = "application/x-www-form-urlencoded";

                    HttpWebResponse wr = (HttpWebResponse)mRequest.GetResponse();
                    Stream sIn = wr.GetResponseStream();
                    FileStream fs = new FileStream(LocalPath, FileMode.Create, FileAccess.Write);
                    byte[] bytes = new byte[4096];
                    int start = 0;
                    int length;
                    while ((length = sIn.Read(bytes, 0, 4096)) > 0)
                    {
                        fs.Write(bytes, 0, length);
                        start += length;
                    }
                    sIn.Close();
                    wr.Close();
                    fs.Close();
                    string pdfpath = showwordfiles(LocalPath);
                }
            }
            catch { }
            //LocalPath = "D:\\1111.doc";
            delet_tables(LocalPath);
            FileInfo fi = new FileInfo(LocalPath);
            fi.Attributes = FileAttributes.ReadOnly;



            //然后完成对文档的解析

            _Application app = new Microsoft.Office.Interop.Word.Application();
            _Application app1 = new Microsoft.Office.Interop.Word.Application();
            _Application app2 = new Microsoft.Office.Interop.Word.Application();
            _Application app3 = new Microsoft.Office.Interop.Word.Application();
            _Application app4 = new Microsoft.Office.Interop.Word.Application();
            _Application app5 = new Microsoft.Office.Interop.Word.Application();
            _Application app6 = new Microsoft.Office.Interop.Word.Application();
            _Application app7 = new Microsoft.Office.Interop.Word.Application();
            //_Document doc;

            //string temp;
            //type1 = "SyRS";
            //type2 = "TSP";
            //string pattern1 = @"...-SyRS-....";
            //pattern1 = @"^\[\w+-\w+-\w+-\d+\]";
            //pattern2 = @"\[\w+-\w+-\d+\]";
            pattern1 = @"^\[\w{3,}-.+\]";
            pattern2 = @"\[\w{3,}-.+?\]";
            /*pattern1 = @"^\[\w+-\w+-\d+\]";
            pattern2 = @"\[\w+-\w+-\d+\]";*/
            pattern3 = @"End";
            pattern4 = @"^#";
            //string pattern5 = @"[*]";

            //object fileName = @"D:\\projectfiles\cascofiles\testdoc.doc"
            //E:\\cascofiles\testdoc.doc
            object fileName = LocalPath;
            // if (dlg.ShowDialog() == DialogResult.OK)
            //  {
            //     fileName = dlg.FileName;
            //  }
            object unknow = System.Type.Missing;
            //object unknow1 = System.Reflection.Missing.Value;
            doc = app.Documents.Open(ref fileName,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow);//input a doc
            doc1 = app1.Documents.Open(ref fileName,
               ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
               ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
               ref unknow, ref unknow, ref unknow, ref unknow, ref unknow);//input a doc
            doc2 = app2.Documents.Open(ref fileName,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow);//input a doc
            doc3 = app3.Documents.Open(ref fileName,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow);//input a doc
            doc4 = app4.Documents.Open(ref fileName,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow);//input a doc
            doc5 = app5.Documents.Open(ref fileName,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow);//input a doc
            doc6 = app6.Documents.Open(ref fileName,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow);//input a doc
            doc7 = app7.Documents.Open(ref fileName,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                ref unknow, ref unknow, ref unknow, ref unknow, ref unknow);//input a doc
            pcount = doc.Paragraphs.Count;//count the paragraphs
            int jjj = pcount;


            //Thread t1 = new Thread(new ThreadStart(thread1));
            //Thread t2 = new Thread(new ThreadStart(thread2));
            var t1 = new System.Threading.Tasks.Task(() => thread1());
            var t2 = new System.Threading.Tasks.Task(() => thread2());
            var t3 = new System.Threading.Tasks.Task(() => thread3());
            var t4 = new System.Threading.Tasks.Task(() => thread4());
            var t5 = new System.Threading.Tasks.Task(() => thread5());
            var t6 = new System.Threading.Tasks.Task(() => thread6());
            var t7 = new System.Threading.Tasks.Task(() => thread7());
            var t8 = new System.Threading.Tasks.Task(() => thread8());
            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();
            t5.Start();
            t6.Start();
            t7.Start();
            t8.Start();
            System.Threading.Tasks.Task.WaitAll(t1, t2, t3, t4, t5, t6, t7, t8);
            doc.Close(ref unknow, ref unknow, ref unknow);

            doc1.Close(ref unknow, ref unknow, ref unknow);

            doc2.Close(ref unknow, ref unknow, ref unknow);

            doc3.Close(ref unknow, ref unknow, ref unknow);

            doc4.Close(ref unknow, ref unknow, ref unknow);

            doc5.Close(ref unknow, ref unknow, ref unknow);

            doc6.Close(ref unknow, ref unknow, ref unknow);

            doc7.Close(ref unknow, ref unknow, ref unknow);

            app.Quit(ref unknow, ref unknow, ref unknow);
            app1.Quit(ref unknow, ref unknow, ref unknow);
            app2.Quit(ref unknow, ref unknow, ref unknow);
            app3.Quit(ref unknow, ref unknow, ref unknow);
            app4.Quit(ref unknow, ref unknow, ref unknow);
            app5.Quit(ref unknow, ref unknow, ref unknow);
            app6.Quit(ref unknow, ref unknow, ref unknow);
            app7.Quit(ref unknow, ref unknow, ref unknow);
            //string jsonString = string1+string2+string3+string4+string5+string6+string7+string8;
            var json = new JavaScriptSerializer().Serialize(aaa.finalstrings);
            //jsonString = string1;
            //return myarray[0].arraycontent[5];
            Context.Response.ContentType = "text/json";
            Context.Response.Write(json);
            Context.Response.End();
            //return jsonString;
            //return myarray[0].sourse[0];
            //return "123";
        }
        [WebMethod(Description = "readtc")]
        public void readtc(string url)
        {
            Random rd = new Random();
            int rdd = rd.Next(10, 100);
            string filename;
            string LocalPath = null;
            try
            {
                int poseuqlurl = url.IndexOf('=');
                int possnap;
                string url1;
                string pdfname;
                url1 = url.Substring(poseuqlurl + 1, url.Length - poseuqlurl - 1);
                possnap = url1.LastIndexOf('/');
                pdfname = url1.Substring(possnap + 1, url1.Length - possnap - 1);
                filename = pdfname;
                Uri u = new Uri(url1);
                //Uri u = new Uri(url);
                //filename = "123.doc";
                //string time1 = DateTime.Now.ToString();
                //filename = DateTime.Now.ToString() + ".doc";
                LocalPath = ConfigurationManager.AppSettings["path"].ToString() + filename;

                if (!File.Exists(@LocalPath))
                {
                    HttpWebRequest mRequest = (HttpWebRequest)WebRequest.Create(u);
                    mRequest.Method = "GET";
                    mRequest.ContentType = "application/x-www-form-urlencoded";
                    HttpWebResponse wr = (HttpWebResponse)mRequest.GetResponse();
                    Stream sIn = wr.GetResponseStream();
                    FileStream fs = new FileStream(LocalPath, FileMode.Create, FileAccess.Write);
                    byte[] bytes = new byte[4096];
                    int start = 0;
                    int length;
                    while ((length = sIn.Read(bytes, 0, 4096)) > 0)
                    {
                        fs.Write(bytes, 0, length);
                        start += length;
                    }
                    sIn.Close();
                    wr.Close();
                    fs.Close();
                    FileInfo fi = new FileInfo(LocalPath);
                    string pdfpath = showwordfiles(LocalPath);
                }
            }
            catch { }
            //LocalPath = "D:\\files\\testtc.doc";
       
            //fi.Attributes = FileAttributes.ReadOnly;
            _Application apptc = new Microsoft.Office.Interop.Word.Application();
            _Document doctc;
            Regex patterntc = new Regex(@"\[\w{3,}-.+?-\d+\]");
            Regex tcpattern1 = new Regex(@"Description");
            Regex tcpattern2 = new Regex(@"Comment");
            Regex tctp1 = new Regex(@"SyRTC");
            Regex tctp2 = new Regex(@"SyITC");
            Regex tctp3 = new Regex(@"SsyRTC");
            Regex tctp4 = new Regex(@"SsyITC");
            Regex tctp5 = new Regex(@"SwRTC");
            Regex tctp6 = new Regex(@"SwITC");
            object fileName = LocalPath;
            object unknow = System.Type.Missing;
            doctc = apptc.Documents.Open(ref fileName,
                           ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                           ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                           ref unknow, ref unknow, ref unknow, ref unknow, ref unknow);//input a doc
            int tablecount = doctc.Tables.Count;
            int rows = 0;
            string cell11 = doctc.Tables[1].Cell(1, 1).Range.Text;
            object cell111 = doctc.Tables[1];
            int truecount = 0;
            int flag = 0;

            for (int i = 1; i <= tablecount; i++)
            {
                cell11 = doctc.Tables[i].Cell(1, 1).Range.Text;
                Match matchMode1 = tcpattern1.Match(cell11);

                if (matchMode1.Success)
                {
                    flag = 1;
                    truecount++;
                }
            }
            tctable[] tctables = new tctable[truecount];

            truecount = 0;
            try
            {
                for (int i = 1; i <= tablecount; i++)
                {
                    cell11 = doctc.Tables[i].Cell(1, 1).Range.Text;
                    rows = doctc.Tables[i].Rows.Count;
                    Match matchMode2 = tcpattern1.Match(cell11);
                    if (matchMode2.Success)
                    {
                        tctable newtctable = new tctable();
                        int typeflag = 0;
                        string temp1 = doctc.Tables[i].Cell(1, 2).Range.Paragraphs[1].Range.Text;//判断tc文档类型
                        Match matchmodetp1 = tctp1.Match(temp1);
                        Match matchmodetp2 = tctp2.Match(temp1);
                        Match matchmodetp3 = tctp3.Match(temp1);
                        Match matchmodetp4 = tctp4.Match(temp1);
                        Match matchmodetp5 = tctp5.Match(temp1);
                        Match matchmodetp6 = tctp6.Match(temp1);
                        if (matchmodetp1.Success || matchmodetp2.Success)
                        {
                            typeflag = 1;
                        }
                        else if (matchmodetp3.Success || matchmodetp4.Success)
                        {
                            typeflag = 2;
                        }
                        else if (matchmodetp5.Success)
                        {
                            typeflag = 3;
                        }
                        else if (matchmodetp6.Success)
                        {
                            typeflag = 4;
                        }

                        newtctable.input = "";
                        newtctable.exec_step = "";
                        newtctable.exp_step = "";
                        if (typeflag == 1)
                        {
                            flag = 1;
                            newtctable.description = doctc.Tables[i].Cell(1, 2).Range.Text;
                            newtctable.test_item = "";
                            newtctable.test_method = doctc.Tables[i].Cell(2, 2).Range.Text;
                            newtctable.pre_condition = doctc.Tables[i].Cell(3, 2).Range.Text;
                            newtctable.result = "";
                            newtctable.comment = "";
                            int acpos;
                            int rowws = rows;
                            string istherecom;
                            istherecom = doctc.Tables[i].Cell(rows, 1).Range.Text;
                            Match matchMode3 = tcpattern2.Match(istherecom);
                            if (matchMode3.Success)
                            {
                                rows = rows - 6;
                            }
                            else
                            {
                                rows = rows - 4;
                            }
                            for (int j = 0; j < rows; j++)
                            {
                                step123 newstep = new step123();
                                acpos = 5;
                                acpos = acpos + j;
                                string sss = doctc.Tables[i].Cell(acpos, 2).Range.Text;
                                newstep.num = j + 1;
                                newstep.test_step = "";
                                newstep.indata = "";
                                newstep.expected_result = doctc.Tables[i].Cell(acpos, 3).Range.Text;
                                newstep.actions = doctc.Tables[i].Cell(acpos, 2).Range.Text;
                                newtctable.test_steps.Add(newstep);
                            }

                            newtctable.tag = doctc.Tables[i].Cell(1, 2).Range.Paragraphs[1].Range.Text;
                            string temp = newtctable.tag;
                            int len = temp.Length;
                            newtctable.tag = temp.Substring(0, len - 1);
                            int paracount = doctc.Tables[i].Cell(1, 2).Range.Paragraphs.Count;
                            for (int k = 3; k <= paracount; k++)
                            {
                                temp = doctc.Tables[i].Cell(1, 2).Range.Paragraphs[k].Range.Text;
                                if (temp != null)
                                {
                                    Match matchMode = patterntc.Match(temp);
                                    string sourcetemp;
                                    while (matchMode.Success)
                                    {
                                        //Console.WriteLine(matchMode.Value);
                                        sourcetemp = matchMode.Value;
                                        //sourcetemp = sourcetemp.Substring(1, sourcetemp.Length - 2);
                                        newtctable.source.Add(sourcetemp);
                                        matchMode = matchMode.NextMatch();
                                    }
                                    /*int sourcecount = matchessourse.Count;
                                    for (int ksourse = 0; ksourse < sourcecount; ksourse++)
                                    {
                                        //newobject.source[ksourse] =matchessourse[ksourse].Value;
                                        newtctable.source.Add(matchessourse[ksourse].Value);
                                    }//将sourse转化成数组格式*/
                                }
                            }
                            tctables[truecount] = newtctable;
                            truecount++;
                        }
                        else if (typeflag == 2)
                        {
                            flag = 1;
                            newtctable.description = doctc.Tables[i].Cell(1, 2).Range.Text;
                            newtctable.test_item = doctc.Tables[i].Cell(2, 2).Range.Text;
                            newtctable.test_method = doctc.Tables[i].Cell(3, 2).Range.Text;
                            newtctable.pre_condition = doctc.Tables[i].Cell(4, 2).Range.Text;
                            newtctable.result = "";
                            newtctable.comment = "";
                            int acpos;
                            int rowws = rows;
                            string istherecom;
                            istherecom = doctc.Tables[i].Cell(rows, 1).Range.Text;
                            Match matchMode3 = tcpattern2.Match(istherecom);
                            if (matchMode3.Success)
                            {
                                rows = rows - 7;
                            }
                            else
                            {
                                rows = rows - 5;
                            }
                            for (int j = 0; j < rows; j++)
                            {
                                step123 newstep = new step123();
                                acpos = 6;
                                acpos = acpos + j;
                                string sss = doctc.Tables[i].Cell(acpos, 2).Range.Text;
                                newstep.num = j + 1;
                                newstep.test_step = "";
                                newstep.indata = "";
                                newstep.expected_result = doctc.Tables[i].Cell(acpos, 3).Range.Text;
                                newstep.actions = doctc.Tables[i].Cell(acpos, 2).Range.Text;
                                newtctable.test_steps.Add(newstep);
                            }

                            newtctable.tag = doctc.Tables[i].Cell(1, 2).Range.Paragraphs[1].Range.Text;
                            string temp = newtctable.tag;
                            int len = temp.Length;
                            newtctable.tag = temp.Substring(0, len - 1);
                            int paracount = doctc.Tables[i].Cell(1, 2).Range.Paragraphs.Count;
                            for (int k = 3; k <= paracount; k++)
                            {
                                temp = doctc.Tables[i].Cell(1, 2).Range.Paragraphs[k].Range.Text;
                                if (temp != null)
                                {
                                    Match matchMode = patterntc.Match(temp);
                                    string sourcetemp;
                                    while (matchMode.Success)
                                    {
                                        //Console.WriteLine(matchMode.Value);
                                        sourcetemp = matchMode.Value;
                                        //sourcetemp = sourcetemp.Substring(1, sourcetemp.Length - 2);
                                        newtctable.source.Add(sourcetemp);
                                        matchMode = matchMode.NextMatch();
                                    }
                                    /*int sourcecount = matchessourse.Count;
                                    for (int ksourse = 0; ksourse < sourcecount; ksourse++)
                                    {
                                        //newobject.source[ksourse] =matchessourse[ksourse].Value;
                                        newtctable.source.Add(matchessourse[ksourse].Value);
                                    }//将sourse转化成数组格式*/
                                }
                            }
                            tctables[truecount] = newtctable;
                            truecount++;
                        }
                        else if (typeflag == 3)
                        {
                            flag = 1;
                            newtctable.description = doctc.Tables[i].Cell(1, 2).Range.Text;
                            //newtctable.test_item = doctc.Tables[i].Cell(2, 2).Range.Text;
                            newtctable.test_method = doctc.Tables[i].Cell(2, 2).Range.Text;
                            newtctable.pre_condition = doctc.Tables[i].Cell(3, 2).Range.Text;
                            newtctable.test_item = "";
                            newtctable.result = "";
                            newtctable.comment = "";
                            int acpos;
                            int rowws = rows;
                            /*string istherecom;
                            istherecom = doctc.Tables[i].Cell(4, 1).Range.Text;
                            Match matchMode3 = tcpattern2.Match(istherecom);
                            if (matchMode3.Success)
                            {
                                rows = rows - 7;
                            }
                            else
                            {
                                rows = rows - 5;
                            }*/
                            rows = rows - 5;
                            for (int j = 0; j < rows; j++)
                            {
                                step123 newstep = new step123();
                                acpos = 5;
                                acpos = acpos + j;
                                //string sss = doctc.Tables[i].Cell(acpos, 2).Range.Text;
                                newstep.num = j + 1;
                                newstep.actions = "";
                                newstep.test_step = doctc.Tables[i].Cell(acpos, 3).Range.Text;
                                newstep.indata = doctc.Tables[i].Cell(acpos, 2).Range.Text;
                                newstep.expected_result = doctc.Tables[i].Cell(acpos, 4).Range.Text;
                                newtctable.test_steps.Add(newstep);
                            }

                            newtctable.tag = doctc.Tables[i].Cell(1, 2).Range.Paragraphs[1].Range.Text;
                            string temp = newtctable.tag;
                            int len = temp.Length;
                            newtctable.tag = temp.Substring(0, len - 1);
                            int paracount = doctc.Tables[i].Cell(1, 2).Range.Paragraphs.Count;
                            for (int k = 3; k <= paracount; k++)
                            {
                                temp = doctc.Tables[i].Cell(1, 2).Range.Paragraphs[k].Range.Text;
                                if (temp != null)
                                {
                                    Match matchMode = patterntc.Match(temp);
                                    string sourcetemp;
                                    while (matchMode.Success)
                                    {
                                        //Console.WriteLine(matchMode.Value);
                                        sourcetemp = matchMode.Value;
                                        //sourcetemp = sourcetemp.Substring(1, sourcetemp.Length - 2);
                                        newtctable.source.Add(sourcetemp);
                                        matchMode = matchMode.NextMatch();
                                    }
                                    /*int sourcecount = matchessourse.Count;
                                    for (int ksourse = 0; ksourse < sourcecount; ksourse++)
                                    {
                                        //newobject.source[ksourse] =matchessourse[ksourse].Value;
                                        newtctable.source.Add(matchessourse[ksourse].Value);
                                    }//将sourse转化成数组格式*/
                                }
                            }
                            tctables[truecount] = newtctable;
                            truecount++;
                        }
                        else if (typeflag == 4)
                        {
                            flag = 1;
                            newtctable.description = doctc.Tables[i].Cell(1, 2).Range.Text;
                            newtctable.test_method = doctc.Tables[i].Cell(2, 2).Range.Text;
                            newtctable.pre_condition = "";
                            newtctable.test_item = "";
                            newtctable.result = "";
                            newtctable.comment = "";

                            newtctable.input = doctc.Tables[i].Cell(3, 2).Range.Text;
                            newtctable.exp_step = doctc.Tables[i].Cell(5, 2).Range.Text;
                            newtctable.exec_step = doctc.Tables[i].Cell(4, 2).Range.Text;
                            step123 newstep = new step123();
                            newstep.expected_result = "";
                            newstep.actions = "";
                            newstep.indata = "";
                            newstep.test_step = "";
                            newtctable.test_steps.Add(newstep);
                            newtctable.tag = doctc.Tables[i].Cell(1, 2).Range.Paragraphs[1].Range.Text;
                            string temp = newtctable.tag;
                            int len = temp.Length;
                            newtctable.tag = temp.Substring(0, len - 1);
                            int paracount = doctc.Tables[i].Cell(1, 2).Range.Paragraphs.Count;
                            for (int k = 3; k <= paracount; k++)
                            {
                                temp = doctc.Tables[i].Cell(1, 2).Range.Paragraphs[k].Range.Text;
                                if (temp != null)
                                {
                                    Match matchMode = patterntc.Match(temp);
                                    string sourcetemp;
                                    while (matchMode.Success)
                                    {
                                        //Console.WriteLine(matchMode.Value);
                                        sourcetemp = matchMode.Value;
                                        //sourcetemp = sourcetemp.Substring(1, sourcetemp.Length - 2);
                                        newtctable.source.Add(sourcetemp);
                                        matchMode = matchMode.NextMatch();
                                    }
                                    /*int sourcecount = matchessourse.Count;
                                    for (int ksourse = 0; ksourse < sourcecount; ksourse++)
                                    {
                                        //newobject.source[ksourse] =matchessourse[ksourse].Value;
                                        newtctable.source.Add(matchessourse[ksourse].Value);
                                    }//将sourse转化成数组格式*/
                                }
                            }
                            tctables[truecount] = newtctable;
                            truecount++;
                        }

                    }

                }
            }
            catch { }
            var json = new JavaScriptSerializer().Serialize(tctables);
            doctc.Close(ref unknow, ref unknow, ref unknow);
            apptc.Quit(ref unknow, ref unknow, ref unknow);
            Context.Response.ContentType = "text/json";
            Context.Response.Write(json);
            Context.Response.End();
        }

 
        [WebMethod(Description = "readtitles")]
        public string readtitles(string filename)
        {
            _Application app = new Microsoft.Office.Interop.Word.Application();
            _Document doc;

            object fileName = filename;
            object unknow = System.Type.Missing;
            doc = app.Documents.Open(ref fileName,
                           ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                           ref unknow, ref unknow, ref unknow, ref unknow, ref unknow,
                           ref unknow, ref unknow, ref unknow, ref unknow, ref unknow);//input a doc
            object pcount = doc.Paragraphs.Count;//count the paragraphs
            object trydocfunc = doc.Tables.Count;
            object listnumbers = doc.Lists.Count;
            object listpnumbers = doc.ListParagraphs.Count;
            object listtbumbers = doc.ListTemplates.Count;
            Lists lists = doc.Lists;
            ListParagraphs listps = doc.ListParagraphs;
            ListTemplates listts = doc.ListTemplates;
            object list1 = lists[1];
            object list2 = lists[2];
            object list3 = listps[1];
            object list4 = listts[1];
            string k = listps[3].Range.Text.Trim();
            //object level = lists[1].ApplyListTemplateWithLevel
            string[] k3 = new string[3];
            for (int i = 0; i <= 1; i++)
            {
                k3[i] = lists[i + 1].Range.Text.Trim();
            }
            int[] num3 = new int[2];
            for (int i = 0; i <= 1; i++)
            {
                num3[i] = lists[i + 1].Range.Start;
            }
            string[] k4 = new string[3];
            for (int i = 0; i <= 2; i++)
            {
                k3[i] = listps[i + 1].Range.Text.Trim();
            }
            listpnumbers = doc.ListParagraphs.Count;
            app.Documents.Close(ref unknow, ref unknow, ref unknow);
            app.Quit(ref unknow, ref unknow, ref unknow);
            return null;
        }


    }
}
