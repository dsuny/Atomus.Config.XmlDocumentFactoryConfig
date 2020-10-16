using Atomus.Diagnostics;
using Atomus.Properties;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Atomus.Config
{
    public class XmlDocumentFactoryConfig : IFactoryConfig
    {
        XmlDocument IFactoryConfig.XmlDocument { get; set; }

        string IFactoryConfig.ServiceConfigPath
        {
            get
            {
                string value;
#if DEBUG
                DiagnosticsTool.MyDebug(string.Format("string IFactoryConfig.ServiceConfigPath = {0}", Settings.Default.ServiceKey));
#endif
                value = Settings.Default.ServiceKey;

                return value.IsNullOrEmpty() ? (string)Client.GetAttribute("Settings.Default.ServiceKey") : value;

                //return Settings.Default.ServiceKey;
            }
        }
        
        public XmlDocumentFactoryConfig()
        {
#if DEBUG
            DiagnosticsTool.MyDebug("XmlDocumentFactoryConfig()");
#endif
            //WebClient _WebClient;

            if (((IFactoryConfig)this).XmlDocument == null)
            {
                ((IFactoryConfig)this).XmlDocument = new XmlDocument();
            }

            try
            {
                ((IFactoryConfig)this).XmlDocument.Load(((IFactoryConfig)this).ServiceConfigPath);
            }
            catch (AtomusException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                throw new AtomusException(exception);
            }

            //HttpWebRequest _HttpWebRequest;
            //string _Tmp;
            //Byte[] _Bytes;
            //StreamReader _StreamReader;
            //Stream _StreamWriter;
            //WebResponse _HttpWebResponse;
            ////string _PasswordKey;
            ////AESRijndael _AESRijndael;


            //_HttpWebResponse = null;
            //_StreamReader = null;
            //_StreamWriter = null;

            //try
            //{
            //    //_AESRijndael = new AESRijndael();

            //    //_PasswordKey = ((IEncryptor)_AESRijndael).Encrypt("Atomus.dsun.kr", "Atomus.dsun.kr");

            //    _Tmp = "op=" + HttpUtility.UrlEncode(Atomus.Factory.ServiceKey());
            //    _Bytes = Encoding.UTF8.GetBytes(_Tmp);

            //    _HttpWebRequest = HttpWebRequest.CreateHttp(Atomus.Factory.DeployerUrl());
            //    _HttpWebRequest.Credentials = CredentialCache.DefaultCredentials;
            //    _HttpWebRequest.Method = "POST";
            //    _HttpWebRequest.ContentLength = _Bytes.Length;
            //    _HttpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

            //    _StreamWriter = _HttpWebRequest.GetRequestStream();
            //    _StreamWriter.Write(_Bytes, 0, _Bytes.Length);
            //    _StreamWriter.Close();

            //    _HttpWebResponse = _HttpWebRequest.GetResponse();
            //    _StreamReader = new StreamReader(_HttpWebResponse.GetResponseStream());

            //    //_XmlDocument.InnerXml = _StreamReader.ReadToEnd()
            //    //XmlDocument.InnerXml = ((IDecryptor)_AESRijndael).Decrypt(_StreamReader.ReadToEnd(), _PasswordKey);//_PasswordKey로 Xml을 복호화
            //    XmlDocument.InnerXml = _StreamReader.ReadToEnd();//_PasswordKey로 Xml을 복호화
            //    //XmlDocument.Save(Settings.Default.ConfigPath);
            //    _StreamReader.Close();
            //    _HttpWebResponse.Close();
            //}
            //catch (Exception)
            //{
            //    XmlDocument.Load(Atomus.Factory.SecondFactoryConfigFile());
            //}
            //finally
            //{
            //    if (_HttpWebResponse != null) _HttpWebResponse.Close();
            //    if (_StreamReader != null) _StreamReader.Close();
            //    if (_StreamWriter != null) _StreamWriter.Close();
            //}
        }

        /// <summary>
        /// Object의 AttributeName에 해당하는 Attribute값 가져옵니다.
        /// </summary>
        /// <param name="core">Attribute값을 가져올 Object를 지정합니다.</param>
        /// <param name="attributeName">AttributeName을 지정합니다.</param>
        /// <returns></returns>
        string IFactoryConfig.GetAttribute(ICore core, string attributeName)
        {
            return ((IFactoryConfig)this).GetAttribute(core.GetType().FullName, attributeName);
        }
        List<string> IFactoryConfig.GetAttribute(ICore core, List<string> listAttributeName)
        {
            List<string> listAssemblyInformation;

            listAssemblyInformation = new List<string>();

            foreach (string attributeName in listAttributeName)
            {
                listAssemblyInformation.Add(((IFactoryConfig)this).GetAttribute(core, attributeName));
            }

            return listAssemblyInformation;
        }

        /// <summary>
        /// Namespace에 해당하는 클래스의 AttributeName에 해당하는 Attribute값 가져옵니다.
        /// </summary>
        /// <param name="namespaceName">Attribute값을 가져올 Namespace를 지정합니다.</param>
        /// <param name="attributeName">AttributeName을 지정합니다.</param>
        /// <returns></returns>
        string IFactoryConfig.GetAttribute(string namespaceName, string attributeName)
        {
#if DEBUG
            DiagnosticsTool.MyDebug(string.Format("string IFactoryConfig.GetAttribute(string _Namespace = {0}, string _AttributeName = {1})", namespaceName, attributeName));
#endif
            XmlNode xmlNode;
            XmlDocument xmlDocument;

            try
            {
                if (namespaceName.StartsWith("Atomus"))
                    xmlNode = ((IFactoryConfig)this).XmlDocument.SelectSingleNode(namespaceName.Replace(".", "/"));
                else
                    xmlNode = ((IFactoryConfig)this).XmlDocument.SelectSingleNode("Atomus/" + namespaceName.Replace(".", "/"));


                if (xmlNode == null)
                {
                    xmlDocument = new XmlDocument();

                    try
                    {
                        xmlDocument.Load(((IFactoryConfig)this).ServiceConfigPath.Replace("op=xml", "op=xml.namespace") + "&ns=" + namespaceName);

                        ((IFactoryConfig)this).XmlDocument.DocumentElement.InnerXml += xmlDocument.DocumentElement.InnerXml;
                        //((IFactoryConfig)this).XmlDocument.DocumentElement.AppendChild(_XmlDocument.DocumentElement.FirstChild.Clone());
                    }
                    catch (Exception exception)
                    {
                        new AtomusException(exception);
                    }
                }

                if (namespaceName.StartsWith("Atomus"))
                    xmlNode = ((IFactoryConfig)this).XmlDocument.SelectSingleNode(namespaceName.Replace(".", "/"));
                else
                    xmlNode = ((IFactoryConfig)this).XmlDocument.SelectSingleNode("Atomus/" + namespaceName.Replace(".", "/"));

                if (xmlNode != null)
                    if (xmlNode.Attributes[attributeName] != null)
                        return xmlNode.Attributes[attributeName].InnerText;
                    else
                        return null;
                else
                    return null;
            }
            catch (AtomusException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                throw new AtomusException(exception);
            }
        }

        string IFactoryConfig.GetPath(string namespaceName)
        {
#if DEBUG
            DiagnosticsTool.MyDebug(string.Format("string IFactoryConfig.GetPath(string _Namespace = {0})", namespaceName));
#endif
            string url;
            string assemblyID;

            try
            {
                assemblyID = ((IFactoryConfig)this).GetAttribute(namespaceName, "AssemblyID");

                if (assemblyID == null)
                    throw new AtomusException(string.Format("Assembly 메타 정보가 없습니다(Namespace :{0}).", namespaceName));

                //url = string.Format("{0}&aid={1}", ((IFactoryConfig)this).ServiceConfigPath.Replace("op=xml", "op=dll"), assemblyID);
                url = string.Format("{0}&aid={1}", ((IFactoryConfig)this).ServiceConfigPath.Replace("op=xml", "op=dll_text"), assemblyID);
            }
            catch (AtomusException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                throw new AtomusException(exception);
            }

            return url;
        }
        List<string> IFactoryConfig.GetPath(List<string> listNamespaceName)
        {
#if DEBUG
            DiagnosticsTool.MyDebug(string.Format("List<string> IFactoryConfig.GetPath(List<string> _Namespace = {0})", (listNamespaceName != null) ? listNamespaceName.Count : 0));
#endif
            List<string> listAssemblyInformation;

            listAssemblyInformation = new List<string>();

            foreach (string tmp in listNamespaceName)
            {
                listAssemblyInformation.Add(((IFactoryConfig)this).GetPath(tmp));
            }

            return listAssemblyInformation;
        }

        string IFactoryConfig.GetttributeInnerXml(string xPath, int index)
        {
            return ((IFactoryConfig)this).XmlDocument.SelectNodes(xPath).Item(index).InnerXml;
        }
        string IFactoryConfig.GetttributeInnerXml(ICore core, string innerXPathName, int index)
        {
            return ((IFactoryConfig)this).GetttributeInnerXml(string.Format("{0}/{1}", core.GetType().FullName.Replace(".", "/"), innerXPathName), index);
        }

//        string IFactoryConfig.GetNamespace(ICore core, string attributeName)
//        {
//            return ((IFactoryConfig)this).GetAttribute(core, attributeName);

//            //return ((IFactoryConfig)this).GetInformation(((IFactoryConfig)this).GetAttribute(((IFactoryConfig)this).GetAttribute(_object, _AttributeName), "Namespace"));
//        }
//        List<string> IFactoryConfig.GetNamespace(ICore core, List<string> listAttributeName)
//        {
//#if DEBUG
//            DiagnosticsTool.MyDebug(string.Format("List<string> IFactoryConfig.GetNamespace(ICore _object = {0}, List<string> _AttributeName = {1})", (core != null) ? core.ToString() : "null", (listAttributeName != null) ? listAttributeName.Count : 0));
//#endif
//            List<string> listAssemblyInformation;

//            listAssemblyInformation = new List<string>();

//            foreach (string _AN in listAttributeName)
//            {
//                listAssemblyInformation.Add(((IFactoryConfig)this).GetNamespace(core, _AN));
//            }

//            return listAssemblyInformation;
//        }
    }
}
