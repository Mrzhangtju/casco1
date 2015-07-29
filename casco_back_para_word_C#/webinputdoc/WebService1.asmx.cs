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
            public ArrayList steps = new ArrayList();
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
            string OfficeFilePath = "D://pdf/office/";
            string PdfFilePath = "D://pdf/pdf/";
            string SWFFilePath = "D://pdf/swf/";
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
            int end = pcount/8;
            for (int i = start; i <= end; i += 1)
            {
                temp = doc.Paragraphs[i].Range.Text.Trim();//变量i为第i段

                MatchCollection matches = Regex.Matches(temp, pattern1);

                if (matches.Count > 0)
                {
                    hahabaseobject newobject = new hahabaseobject();
                    newobject.Allocation="";
                    newobject.Category="";
                    newobject.Contribution="";
                    newobject.description="";
                    newobject.Implement="";
                    newobject.Priority="";
                    newobject.othercontext="";
                    newobject.title = temp;//还没转换好
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
                        for (int k = i + 1; k <=pcount; k++)
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
            int start = pcount  / 8+1;
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
                    newobject.Allocation="";
                    newobject.Category="";
                    newobject.Contribution="";
                    newobject.description="";
                    newobject.Implement="";
                    newobject.Priority="";
                    newobject.othercontext="";
                    newobject.title = temp;//还没转换好
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
                    newobject.Allocation="";
                    newobject.Category="";
                    newobject.Contribution="";
                    newobject.description="";
                    newobject.Implement="";
                    newobject.Priority="";
                    newobject.othercontext="";
                    newobject.title = temp;//还没转换好
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
                    newobject.Allocation="";
                    newobject.Category="";
                    newobject.Contribution="";
                    newobject.description="";
                    newobject.Implement="";
                    newobject.Priority="";
                    newobject.othercontext="";
                    newobject.title = temp;//还没转换好
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
                    for (int k = i + 1; k <=pcount; k++)
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
                    newobject.Allocation="";
                    newobject.Category="";
                    newobject.Contribution="";
                    newobject.description="";
                    newobject.Implement="";
                    newobject.Priority="";
                    newobject.othercontext="";
                    newobject.title = temp;//还没转换好
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
                    newobject.Allocation="";
                    newobject.Category="";
                    newobject.Contribution="";
                    newobject.description="";
                    newobject.Implement="";
                    newobject.Priority="";
                    newobject.othercontext="";
                    newobject.title = temp;//还没转换好
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
                    newobject.Allocation="";
                    newobject.Category="";
                    newobject.Contribution="";
                    newobject.description="";
                    newobject.Implement="";
                    newobject.Priority="";
                    newobject.othercontext="";
                    newobject.title = temp;//还没转换好
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
                    newobject.Allocation="";
                    newobject.Category="";
                    newobject.Contribution="";
                    newobject.description="";
                    newobject.Implement="";
                    newobject.Priority="";
                    newobject.othercontext="";
                    newobject.title = temp;//还没转换好
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
        [WebMethod(Description = "inputword")]
        public void InputWord(string url)
        {
            Random rd = new Random();
            int rdd = rd.Next(10, 100);
            string filename = "rs"+DateTime.Now.ToString("yyyy-MM-dd") + rdd.ToString() + ".doc";
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
                LocalPath = "D:\\files\\" + filename;
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
            System.Threading.Tasks.Task.WaitAll(t1,t2,t3,t4,t5,t6,t7,t8);
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
                LocalPath = "D:\\files\\" + filename;
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
            }
            catch { }
           //LocalPath = "D:\\files\\testtc.doc";
            FileInfo fi = new FileInfo(LocalPath);
           string pdfpath = showwordfiles(LocalPath);
            //fi.Attributes = FileAttributes.ReadOnly;
            _Application apptc = new Microsoft.Office.Interop.Word.Application();
            _Document doctc;
            Regex patterntc = new Regex(@"\[\w{3,}-.+?-\d+\]");
            Regex tcpattern1 = new Regex(@"Description");
            Regex tcpattern2 = new Regex(@"Comment");
            Regex tctp1=new Regex(@"SyRTC");
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
            string cell11 = doctc.Tables[1].Cell(1,1).Range.Text;
            object cell111 = doctc.Tables[1];
            int truecount=0;
            int flag = 0;

            for (int i = 1; i <= tablecount; i++)
            {
                cell11 = doctc.Tables[i].Cell(1,1).Range.Text;
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
                            typeflag=1;
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
                                newtctable.steps.Add(newstep);
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
                                newtctable.steps.Add(newstep);
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
                                newtctable.steps.Add(newstep);
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
                            
                            newtctable.input=doctc.Tables[i].Cell(3, 2).Range.Text;
                            newtctable.exp_step = doctc.Tables[i].Cell(5, 2).Range.Text;
                            newtctable.exec_step = doctc.Tables[i].Cell(4, 2).Range.Text;
                            step123 newstep = new step123();
                            newstep.expected_result = "";
                            newstep.actions = "";
                            newstep.indata = "";
                            newstep.test_step = "";
                            newtctable.steps.Add(newstep);
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

        [WebMethod (Description="test")]
         public string test(string url)
        {
            int pcount=url.Length;
            int pa;
            string aaa = url;
            //pa=pcount*2/9;
            string filename;
            string snap = "/";
            //filename = url;
            pa = url.LastIndexOf('/');
            filename = aaa.Substring(pa + 1, pcount-pa-1);
            pa = pa + 1;
                string aaaa="d://111.doc";
           snap= showwordfiles(aaaa);
            return null;
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
